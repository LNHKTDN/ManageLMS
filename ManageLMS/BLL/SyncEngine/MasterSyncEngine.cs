using ManageLMS.BLL.Cache;
using ManageLMS.BLL.Manager.CategoryManager;
using ManageLMS.BLL.Manager.CourseManager;
using ManageLMS.BLL.Manager.EnrollManager;
using ManageLMS.BLL.Manager.OtherManager;
using ManageLMS.BLL.Manager.UserManager;
using ManageLMS.Common.DTO.Database;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.Common.Helpers;
using ManageLMS.DAL.Database;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageLMS.BLL.SyncEngine
{
    public class MasterSyncEngine
    {
        private DataDaoTao _sqlDal;
        private CourseManager _courseMgr;
        private CategoryManager _cateMgr;
        private OtherManager _otherMgr;
        private EnrollManager _enrollMgr;
        private StudentManager _studentMgr;
        private TeacherManager _teacherMgr;

        private List<IGrouping<string, TTDsSVTheoLopHP>> _cachedMasterGroups;
        private string _cachedMaKyHoc;
        private ConcurrentDictionary<string, long> _categoryCache;

        // Lưu TrainingType hiện tại để dùng cho ProcessGroups
        private TrainingType _currentTrainingType;

        private const string MASTER_ROOT_ID = AppConstant.MoodleString.masterIdNumber;
        private const string MASTER_ROOT_NAME = AppConstant.MoodleString.masterCategoryName;

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

        public enum TrainingType
        {
            DaiHoc_ChinhQuy,
            DaiHoc_VHVL,
            SauDaiHoc
        }

        public enum SyncStatusFilter { All = 0, NotCreated = 1, Created = 2, Diff = 3, Synced = 4 }

        public MasterSyncEngine()
        {
            _sqlDal = new DataDaoTao();
            _courseMgr = new CourseManager();
            _cateMgr = new CategoryManager();
            _otherMgr = new OtherManager();
            _enrollMgr = new EnrollManager();
            _studentMgr = new StudentManager();
            _teacherMgr = new TeacherManager();

            _cachedMasterGroups = new List<IGrouping<string, TTDsSVTheoLopHP>>();
            _categoryCache = new ConcurrentDictionary<string, long>();
        }


        public int LoadAndGroupMasterData(int kyHoc, string maKyHoc, string keyword, TrainingType trainingType, string filterMaKhoa = "")
        {
            Log("Đang tải dữ liệu Lớp Học Phần (Master) từ SQL...");
            _cachedMaKyHoc = maKyHoc;
            _currentTrainingType = trainingType; // Lưu lại để dùng sau
            _categoryCache.Clear();

            var allData = new List<TTDsSVTheoLopHP>();
            int fetchPage = 1;
            int fetchSize = 20000;

            // Lấy mã hệ từ TrainingType để truyền vào SQL (nếu cần lọc bước 1)
            string filterMaHe = "";
            string dummyName;
            GetHeInfoFromType(trainingType, out filterMaHe, out dummyName);

            while (true)
            {
                
                var chunk = _sqlDal.GetDsSinhVienTheoLopHocPhan(kyHoc, keyword, filterMaKhoa, fetchPage, fetchSize);
                if (chunk == null || chunk.Count == 0) break;

                foreach (var item in chunk)
                {
                    if (!string.IsNullOrEmpty(item.MaHocPhan) && item.MaHocPhan.Contains("_"))
                    {
                        item.MaHocPhan = item.MaHocPhan.Split('_')[0];
                    }
                }

                allData.AddRange(chunk);
                if (chunk.Count < fetchSize) break;
                fetchPage++;
            }

            Log(string.Format("Đã tải {0} dòng. Đang gom nhóm...", allData.Count));


            _cachedMasterGroups = allData
                .GroupBy(x => string.Format("{0}_{1}", x.MaKhoa, x.MaHocPhan))
                .OrderBy(g => g.Key)
                .ToList();

            Log(string.Format("-> Tổng hợp được: {0} khóa Master.", _cachedMasterGroups.Count));
            return _cachedMasterGroups.Count;
        }

        // =================================================================
        // 2. PROCESS LOGIC
        // =================================================================
        private List<MasterSyncViewModel> ProcessGroups(List<IGrouping<string, TTDsSVTheoLopHP>> groups)
        {
            var resultList = new ConcurrentBag<MasterSyncViewModel>();

            // Lấy thông tin Hệ chuẩn từ TrainingType đã lưu
            string stdMaHe, stdTenHe;
            GetHeInfoFromType(_currentTrainingType, out stdMaHe, out stdTenHe);

            // A. Pre-load Cache (Batch)
            var allMSSV = groups.SelectMany(g => g.Select(x => x.MaSinhVien.Trim())).Distinct().ToList();
            var missingKeys = MoodleCache.GetMissingKeys(allMSSV);
            if (missingKeys.Count > 0)
            {
                var batches = SplitList(missingKeys, 200);
                Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = 8 }, b => _studentMgr.LoadSpecificUsersToCache(b));
            }

            var allGV = groups.SelectMany(g => g.Select(x => x.MaGiaoVien.Trim())).Distinct().ToList();
            var missingGV = MoodleCache.GetMissingKeys(allGV);
            if (missingGV.Count > 0) _teacherMgr.LoadSpecificUsersToCache(missingGV);

            // B. Map ID (Sử dụng stdMaHe chuẩn thay vì lấy từ DB)
            var idNumbersToCheck = groups.Select(g =>
            {
                var f = g.First();
                // Format IDNumber: MASTER_[STD_MAHE]_[MAKHOA]_[MAHP]
                return string.Format("MASTER_{0}_{1}_{2}", stdMaHe, f.MaKhoa, f.MaHocPhan);
            }).ToList();

            var moodleCourses = _courseMgr.GetCoursesByField("idnumber", idNumbersToCheck);
            var moodleCourseMap = moodleCourses.ToDictionary(c => c.idnumber, c => c);

            // C. Process Parallel
            Parallel.ForEach(groups, new ParallelOptions { MaxDegreeOfParallelism = 10 }, group =>
            {
                var firstRow = group.First();
                // Sử dụng stdMaHe chuẩn
                string idNumber = string.Format("MASTER_{0}_{1}_{2}", stdMaHe, firstRow.MaKhoa, firstRow.MaHocPhan);

                var vm = new MasterSyncViewModel
                {
                    MaHocPhan = firstRow.MaHocPhan,
                    TenHocPhan = firstRow.TenHocPhan,
                    MaKyHoc = _cachedMaKyHoc,
                    MaHe = stdMaHe,   // Gán mã hệ chuẩn
                    TenHe = stdTenHe, // Gán tên hệ chuẩn
                    MaKhoa = firstRow.MaKhoa,
                    TenKhoa = firstRow.TenKhoa,
                    SiSoSQL = group.Select(x => x.MaSinhVien).Distinct().Count(),
                    ListMaGiaoVien = group.Select(x => x.MaGiaoVien).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).Distinct().ToList()
                };

                var sqlMSSVs = new HashSet<string>(group.Select(x => x.MaSinhVien.Trim().ToLower()).Where(x => !string.IsNullOrEmpty(x)).Distinct());

                if (moodleCourseMap.ContainsKey(idNumber))
                {
                    var mCourse = moodleCourseMap[idNumber];
                    vm.MoodleCourseId = mCourse.id;
                    vm.MoodleShortname = mCourse.shortname;

                    var enrolledUsers = _courseMgr.GetEnrolledUser(mCourse.id);
                    var moodleStudents = enrolledUsers.Where(u => u.Roles != null && u.Roles.ToLower().Contains("student")).ToList();
                    vm.SiSoMoodle = moodleStudents.Count;

                    foreach (var s in moodleStudents)
                        if (!MoodleCache.Contains(s.Username)) MoodleCache.AddToCache(s.Username, new MoodleUser { id = s.Id, username = s.Username });

                    var moodleUsernames = new HashSet<string>(moodleStudents.Select(u => u.Username.Trim().ToLower()));

                    vm.ListData_SV_Thieu = group.Where(x => !string.IsNullOrEmpty(x.MaSinhVien)
                                                         && !moodleUsernames.Contains(x.MaSinhVien.Trim().ToLower()))
                                                .GroupBy(x => x.MaSinhVien).Select(g => g.First())
                                                .ToList();

                    vm.SoLuongThieu = vm.ListData_SV_Thieu.Count;
                    vm.SoLuongThua = 0;
                    vm.TrangThai = (vm.SoLuongThieu == 0) ? "Đã đồng bộ" : string.Format("Thiếu {0} SV", vm.SoLuongThieu);
                }
                else
                {
                    vm.MoodleCourseId = 0;
                    vm.SiSoMoodle = 0;
                    vm.SoLuongThieu = vm.SiSoSQL;
                    vm.TrangThai = "Chưa tạo";
                    vm.ListData_SV_Thieu = group.GroupBy(x => x.MaSinhVien).Select(g => g.First()).ToList();
                }
                resultList.Add(vm);
            });

            return resultList.OrderBy(x => x.MaKhoa).ThenBy(x => x.MaHocPhan).ToList();
        }

        // =================================================================
        // 3. SYNC ACTIONS
        // =================================================================

        public void SyncAllDatabase(string maKyHoc, TrainingType type)
        {
            if (_cachedMasterGroups == null || _cachedMasterGroups.Count == 0) { Log("Vui lòng tải dữ liệu trước."); return; }

            Log(string.Format("=== BẮT ĐẦU ĐỒNG BỘ MASTER ({0}) ===", type.ToString()));

            long targetParentId = EnsureTrainingTypeCategoryTree(type);
            if (targetParentId == 0) { Log("Lỗi: Không thể tạo cây thư mục."); return; }

            // Lấy thông tin hệ chuẩn
            string maHe, tenHe;
            GetHeInfoFromType(type, out maHe, out tenHe);

            int total = _cachedMasterGroups.Count;
            int processed = 0;
            int batchSize = 20;

            for (int i = 0; i < total; i += batchSize)
            {
                var batchGroups = _cachedMasterGroups.Skip(i).Take(batchSize).ToList();
                Log(string.Format("--- Lô {0}-{1}/{2} ---", i + 1, i + batchGroups.Count, total));

                // ProcessGroups sẽ tự động dùng _currentTrainingType (đã gán ở hàm Load)
                // Nhưng để an toàn nếu gọi Sync trực tiếp, ta gán lại
                _currentTrainingType = type;
                var batchVMs = ProcessGroups(batchGroups);

                foreach (var item in batchVMs)
                {
                    processed++;
                    try
                    {
                        // Truyền maHe chuẩn vào để tạo category con
                        long finalCatId = EnsureFacultyCategory(targetParentId, item, maHe);

                        if (item.MoodleCourseId == 0)
                            CreateAndEnrollNewCourse(item, finalCatId, type);
                        else
                            FixDiffMasterCourse(item);
                    }
                    catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaHocPhan, ex.Message)); }
                    ReportProgress(processed, total);
                }
            }
            Log("=== HOÀN TẤT SYNC ALL ===");
        }

        public void SyncList(List<MasterSyncViewModel> itemsToSync, string maKyHoc, TrainingType type)
        {
            Log(string.Format("=== BẮT ĐẦU ĐỒNG BỘ CHỌN ({0}) ===", type.ToString()));

            long rootId = EnsureTrainingTypeCategoryTree(type);
            if (rootId == 0) { Log("Lỗi tạo danh mục."); return; }

            string maHe, tenHe;
            GetHeInfoFromType(type, out maHe, out tenHe);

            int total = itemsToSync.Count;
            int count = 0;

            foreach (var item in itemsToSync)
            {
                count++;
                Log(string.Format("[{0}/{1}] Xử lý: {2}...", count, total, item.MaHocPhan));
                try
                {
                    long finalCatId = EnsureFacultyCategory(rootId, item, maHe);

                    if (item.MoodleCourseId == 0)
                        CreateAndEnrollNewCourse(item, finalCatId, type);
                    else
                        FixDiffMasterCourse(item);

                    ReportProgress(count, total);
                }
                catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaHocPhan, ex.Message)); }
            }
            Log("--- Hoàn tất ---");
        }

        // =================================================================
        // 4. CATEGORY MANAGEMENT
        // =================================================================

        private long EnsureTrainingTypeCategoryTree(TrainingType type)
        {
            long rootId = GetOrCreateCategory(MASTER_ROOT_ID, MASTER_ROOT_NAME, 0);

            switch (type)
            {
                case TrainingType.DaiHoc_ChinhQuy:
                    long dhId = GetOrCreateCategory(AppConstant.MoodleString.ctdtDHIdNumber, AppConstant.MoodleString.ctdtDHCategoryName, rootId);
                    return GetOrCreateCategory(AppConstant.MoodleString.CQ_DH_IdNumber, AppConstant.MoodleString.CQ_DH_CategoryName, dhId);

                case TrainingType.DaiHoc_VHVL:
                    long dhId2 = GetOrCreateCategory(AppConstant.MoodleString.ctdtDHIdNumber, AppConstant.MoodleString.ctdtDHCategoryName, rootId);
                    return GetOrCreateCategory(AppConstant.MoodleString.VHVL_DH_IdNumber, AppConstant.MoodleString.VHVL_DH_CategoryName, dhId2);

                case TrainingType.SauDaiHoc:
                    return GetOrCreateCategory(AppConstant.MoodleString.ctdtSDHIdNumber, AppConstant.MoodleString.ctdtSDHCategoryName, rootId);

                default:
                    return rootId;
            }
        }

        // [SỬA] Thêm tham số maHe để tạo IDNumber chuẩn cho Khoa
        private long EnsureFacultyCategory(long parentTypeId, MasterSyncViewModel item, string maHe)
        {
            if (string.IsNullOrEmpty(item.MaKhoa)) return parentTypeId;

            string khoaIdNumber = string.Format("MASTER_{0}_KHOA_{1}", maHe, item.MaKhoa);
            string khoaName = string.IsNullOrEmpty(item.TenKhoa) ? item.MaKhoa : item.TenKhoa;

            return GetOrCreateCategory(khoaIdNumber, khoaName, parentTypeId);
        }

        private long GetOrCreateCategory(string idNumber, string name, long parentId)
        {
            if (_categoryCache.ContainsKey(idNumber)) return _categoryCache[idNumber];
            var existing = _cateMgr.GetCategoryByIdNumber(idNumber);
            if (existing != null) { _categoryCache.TryAdd(idNumber, existing.id); return existing.id; }
            long newId = _cateMgr.CreateCategory(name, idNumber, "", (int)parentId);
            if (newId > 0) { _categoryCache.TryAdd(idNumber, newId); return newId; }
            throw new Exception("Không thể tạo Category: " + name);
        }

        // =================================================================
        // 5. CRUD ACTIONS
        // =================================================================

        private void CreateAndEnrollNewCourse(MasterSyncViewModel item, long categoryId, TrainingType type)
        {
            // Lấy thông tin chuẩn từ Type
            string maHe, tenHe;
            GetHeInfoFromType(type, out maHe, out tenHe);

            // Sinh ID và Tên dựa trên tham số chuẩn
            string idNumber = item.GetExpectedIdNumber(maHe, item.MaKhoa);
            string fullName = item.GetExpectedFullName(tenHe);
            string shortName = idNumber;

            var newCourse = new MoodleCourse
            {
                fullname = fullName,
                shortname = shortName,
                idnumber = idNumber,
                category = (int)categoryId,
                visible = 1,
                format = "topics",
            };

            try
            {
                _courseMgr.CreateCoursesBatch(new List<MoodleCourse> { newCourse });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("shortnametaken") || ex.Message.Contains("already used"))
                {
                    Log(string.Format(" -> ⚠️ Khóa học {0} đã tồn tại. Chuyển sang cập nhật.", idNumber));
                    var existingCourses = _courseMgr.GetCoursesByField("idnumber", new List<string> { idNumber });
                    if (existingCourses == null || existingCourses.Count == 0) existingCourses = _courseMgr.GetCoursesByField("shortname", new List<string> { shortName });

                    if (existingCourses != null && existingCourses.Count > 0)
                    {
                        item.MoodleCourseId = existingCourses[0].id;
                        FixDiffMasterCourse(item);
                        return;
                    }
                }
                throw ex;
            }

            var createdList = _courseMgr.GetCoursesByField("idnumber", new List<string> { idNumber });
            if (createdList.Count > 0)
            {
                Log(string.Format(" -> Tạo mới: {0}", fullName));
                long newId = createdList[0].id;
                EnrollStudents(newId, item.ListData_SV_Thieu);
                EnrollTeachers(newId, item.ListMaGiaoVien);
            }
            else Log(" ! Lỗi: API báo thành công nhưng không tìm thấy ID.");
        }

        private void FixDiffMasterCourse(MasterSyncViewModel item)
        {
            if (item.ListData_SV_Thieu != null && item.ListData_SV_Thieu.Count > 0)
                EnrollStudents(item.MoodleCourseId, item.ListData_SV_Thieu);

            EnrollTeachers(item.MoodleCourseId, item.ListMaGiaoVien);
        }

        private void EnrollStudents(long courseId, List<TTDsSVTheoLopHP> listStudentData)
        {
            int count = _studentMgr.EnrollStudentsToCourse(courseId, listStudentData);
            if (count > 0) Log(string.Format(" -> Enroll & Create {0} sinh viên.", count));
        }

        private void EnrollTeachers(long courseId, List<string> teacherUsernames)
        {
            int count = _teacherMgr.EnrollTeachersToCourse(courseId, teacherUsernames);
            if (count > 0) Log(string.Format(" -> Gán {0} giảng viên.", count));
        }

        // [QUAN TRỌNG] Hàm Map Enum -> String
        private void GetHeInfoFromType(TrainingType type, out string maHe, out string tenHe)
        {
            switch (type)
            {
                case TrainingType.DaiHoc_ChinhQuy:
                    maHe = "CQ";
                    tenHe = "ĐH Chính Quy";
                    break;
                case TrainingType.DaiHoc_VHVL:
                    maHe = "VHVL";
                    tenHe = "ĐH Vừa Làm Vừa Học";
                    break;
                case TrainingType.SauDaiHoc:
                    maHe = "SDH";
                    tenHe = "Sau Đại Học";
                    break;
                default:
                    maHe = "UNK";
                    tenHe = "Khác";
                    break;
            }
        }

        // =================================================================
        // 6. UTILS
        // =================================================================
        public List<MasterSyncViewModel> GetPageData(int page, int pageSize, SyncStatusFilter statusFilter)
        {
            if (_cachedMasterGroups == null || _cachedMasterGroups.Count == 0) return new List<MasterSyncViewModel>();
            var pageGroups = _cachedMasterGroups.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            Log(string.Format("Đang xử lý trang {0}...", page));
            return ProcessGroups(pageGroups);
        }

        public List<MasterSyncViewModel> GetAllMissingData()
        {
            if (_cachedMasterGroups == null || _cachedMasterGroups.Count == 0) return new List<MasterSyncViewModel>();
            return ProcessGroups(_cachedMasterGroups).Where(x => x.MoodleCourseId == 0 || x.SoLuongThieu > 0).ToList();
        }

        private List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < locations.Count; i += nSize) list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            return list;
        }

        private void Log(string msg) { if (OnLogMessage != null) OnLogMessage(msg); }
        private void ReportProgress(int current, int total) { if (OnProgress != null) OnProgress(current, total); }
    }
}