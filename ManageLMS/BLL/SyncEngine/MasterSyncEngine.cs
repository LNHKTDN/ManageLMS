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

        private const string MASTER_CATEGORY_IDNUMBER = "MASTER_ROOT";
        private const string MASTER_CATEGORY_NAME = "KHÓA HỌC CHUNG (MASTER COURSES)";

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

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

        public enum SyncStatusFilter { All = 0, NotCreated = 1, Created = 2, Diff = 3, Synced = 4 }

        // =================================================================
        // 1. LOAD & GROUP
        // =================================================================
        public int LoadAndGroupMasterData(int kyHoc, string maKyHoc, string keyword, string filterMaHe = "", string filterMaKhoa = "")
        {
            Log("Đang tải dữ liệu Lớp Học Phần (Master) từ SQL...");
            _cachedMaKyHoc = maKyHoc;

            var allData = new List<TTDsSVTheoLopHP>();
            int fetchPage = 1;
            int fetchSize = 20000;

            while (true)
            {
                var chunk = _sqlDal.GetDsSinhVienTheoLopHocPhan(kyHoc, keyword, filterMaHe, filterMaKhoa, fetchPage, fetchSize);
                if (chunk == null || chunk.Count == 0) break;

                // [FIX]: Chuẩn hóa Mã Học Phần (Cắt bỏ đuôi _1, _2...)
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
                .GroupBy(x => string.Format("{0}_{1}_{2}", x.MaHe, x.MaKhoa, x.MaHocPhan))
                .OrderBy(g => g.Key)
                .ToList();

            Log(string.Format("-> Tổng hợp được: {0} khóa Master.", _cachedMasterGroups.Count));
            return _cachedMasterGroups.Count;
        }

        // =================================================================
        // 2. PROCESS LOGIC (TÁCH BIỆT ĐỂ TÁI SỬ DỤNG)
        // =================================================================
        private List<MasterSyncViewModel> ProcessGroups(List<IGrouping<string, TTDsSVTheoLopHP>> groups)
        {
            var resultList = new ConcurrentBag<MasterSyncViewModel>();

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

            // B. Map ID
            var idNumbersToCheck = groups.Select(g =>
            {
                var f = g.First();
                return string.Format("MASTER_{0}_{1}_{2}", f.MaHe, f.MaKhoa, f.MaHocPhan);
            }).ToList();

            var moodleCourses = _courseMgr.GetCoursesByField("idnumber", idNumbersToCheck);
            var moodleCourseMap = moodleCourses.ToDictionary(c => c.idnumber, c => c);

            // C. Process Parallel
            Parallel.ForEach(groups, new ParallelOptions { MaxDegreeOfParallelism = 10 }, group =>
            {
                var firstRow = group.First();
                string idNumber = string.Format("MASTER_{0}_{1}_{2}", firstRow.MaHe, firstRow.MaKhoa, firstRow.MaHocPhan);

                var vm = new MasterSyncViewModel
                {
                    MaHocPhan = firstRow.MaHocPhan,
                    TenHocPhan = firstRow.TenHocPhan,
                    MaKyHoc = _cachedMaKyHoc,
                    MaHe = firstRow.MaHe,
                    TenHe = firstRow.TenHe,
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

                    // [FIX] Lọc ra LIST OBJECT thay vì String
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

                    // Chưa có -> Convert toàn bộ sang List Object
                    vm.ListData_SV_Thieu = group.GroupBy(x => x.MaSinhVien).Select(g => g.First()).ToList();
                }
                resultList.Add(vm);
            });

            return resultList.OrderBy(x => x.MaHe).ThenBy(x => x.MaKhoa).ThenBy(x => x.MaHocPhan).ToList();
        }

        // =================================================================
        // 3. UI HELPERS
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
            Log("Đang quét toàn bộ DB...");
            return ProcessGroups(_cachedMasterGroups).Where(x => x.MoodleCourseId == 0 || x.SoLuongThieu > 0).ToList();
        }

        // =================================================================
        // 4. SYNC ACTIONS
        // =================================================================
        public void SyncList(List<MasterSyncViewModel> itemsToSync, string maKyHoc)
        {
            _categoryCache.Clear();
            long rootId = EnsureRootCategory();
            if (rootId == 0) return;

            int total = itemsToSync.Count;
            int count = 0;

            foreach (var item in itemsToSync)
            {
                count++;
                Log(string.Format("[{0}/{1}] Xử lý: {2} - {3}...", count, total, item.MaHe, item.MaHocPhan));
                try
                {
                    long finalCatId = EnsureCategoryTree(rootId, item);
                    if (item.MoodleCourseId == 0) CreateAndEnrollNewCourse(item, finalCatId);
                    else FixDiffMasterCourse(item);
                    ReportProgress(count, total);
                }
                catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaHocPhan, ex.Message)); }
            }
            Log("--- Hoàn tất ---");
        }

        public void SyncAllDatabase(string maKyHoc)
        {
            if (_cachedMasterGroups == null || _cachedMasterGroups.Count == 0) { Log("Vui lòng tải dữ liệu trước."); return; }

            Log("=== BẮT ĐẦU SYNC ALL (MASTER) ===");
            _categoryCache.Clear();
            long rootId = EnsureRootCategory();
            if (rootId == 0) { Log("Lỗi Fatal: Không thể tạo Root Category."); return; }

            int total = _cachedMasterGroups.Count;
            int processed = 0;
            int batchSize = 20;

            for (int i = 0; i < total; i += batchSize)
            {
                var batchGroups = _cachedMasterGroups.Skip(i).Take(batchSize).ToList();
                Log(string.Format("--- Lô {0}-{1}/{2} ---", i + 1, i + batchGroups.Count, total));

                var batchVMs = ProcessGroups(batchGroups);

                foreach (var item in batchVMs)
                {
                    processed++;
                    try
                    {
                        long finalCatId = EnsureCategoryTree(rootId, item);
                        if (item.MoodleCourseId == 0) CreateAndEnrollNewCourse(item, finalCatId);
                        else FixDiffMasterCourse(item);
                    }
                    catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaHocPhan, ex.Message)); }
                    ReportProgress(processed, total);
                }
            }
            Log("=== HOÀN TẤT SYNC ALL ===");
        }

        // =================================================================
        // INTERNAL LOGIC
        // =================================================================
        private long EnsureRootCategory()
        {
            return GetOrCreateCategory(MASTER_CATEGORY_IDNUMBER, MASTER_CATEGORY_NAME, 0);
        }

        private long EnsureCategoryTree(long rootId, MasterSyncViewModel item)
        {
            string heId = string.Format("MASTER_HE_{0}", item.MaHe);
            string heName = string.IsNullOrEmpty(item.TenHe) ? item.MaHe : item.TenHe;
            long heCatId = GetOrCreateCategory(heId, heName, rootId);

            string khoaId = string.Format("MASTER_{0}_KHOA_{1}", item.MaHe, item.MaKhoa);
            string khoaName = string.IsNullOrEmpty(item.TenKhoa) ? item.MaKhoa : item.TenKhoa;

            if (string.IsNullOrEmpty(item.MaKhoa)) return heCatId;
            return GetOrCreateCategory(khoaId, khoaName, heCatId);
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

        private void CreateAndEnrollNewCourse(MasterSyncViewModel item, long categoryId)
        {
            string idNumber = item.GetExpectedIdNumber();
            string fullName = item.GetExpectedFullName();
            string shortName = idNumber;

            var newCourse = new MoodleCourse
            {
                fullname = fullName,
                shortname = shortName,
                idnumber = idNumber,
                category = (int)categoryId,
                visible = 1,
                format = "topics"
            };

            try
            {
                _courseMgr.CreateCoursesBatch(new List<MoodleCourse> { newCourse });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("courseidnumbertaken") || ex.Message.Contains("shortnametaken") || ex.Message.Contains("already used"))
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

                // [FIX] Truyền List Object
                EnrollStudents(newId, item.ListData_SV_Thieu);
                EnrollTeachers(newId, item.ListMaGiaoVien);
            }
            else Log(" ! Lỗi: API báo thành công nhưng không tìm thấy ID.");
        }

        private void FixDiffMasterCourse(MasterSyncViewModel item)
        {
            // [FIX] Truyền List Object
            if (item.ListData_SV_Thieu != null && item.ListData_SV_Thieu.Count > 0)
                EnrollStudents(item.MoodleCourseId, item.ListData_SV_Thieu);

            EnrollTeachers(item.MoodleCourseId, item.ListMaGiaoVien);
        }

        // [FIX] Sửa tham số nhận List Object TTDsSVTheoLopHP
        private void EnrollStudents(long courseId, List<TTDsSVTheoLopHP> listStudentData)
        {
            // Batching đã được chuyển vào trong StudentManager
            int count = _studentMgr.EnrollStudentsToCourse(courseId, listStudentData);
            if (count > 0) Log(string.Format(" -> Enroll & Create {0} sinh viên.", count));
        }

        private void EnrollTeachers(long courseId, List<string> teacherUsernames)
        {
            int count = _teacherMgr.EnrollTeachersToCourse(courseId, teacherUsernames);
            if (count > 0) Log(string.Format(" -> Gán {0} giảng viên.", count));
        }

        private List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < locations.Count; i += nSize) list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            return list;
        }

        private bool CheckFilter(MasterSyncViewModel item, SyncStatusFilter filter) { return true; } // Logic filter UI đã chuyển sang client-side
        private void Log(string msg) { if (OnLogMessage != null) OnLogMessage(msg); }
        private void ReportProgress(int current, int total) { if (OnProgress != null) OnProgress(current, total); }
    }
}