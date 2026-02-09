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
    public class SemesterSyncEngine
    {
        private DataDaoTao _sqlDal;
        private CourseManager _courseMgr;
        private CategoryManager _cateMgr;
        private OtherManager _otherMgr;
        private EnrollManager _enrollMgr;
        private StudentManager _studentMgr;
        private TeacherManager _teacherMgr;

        private List<IGrouping<string, TTDsLopTheoKiTheoTKB>> _cachedGroups;
        private string _cachedMaKyHoc;

        // Cache Category để giảm tải API
        private ConcurrentDictionary<string, long> _categoryCache;

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

        public SemesterSyncEngine()
        {
            _sqlDal = new DataDaoTao();
            _courseMgr = new CourseManager();
            _cateMgr = new CategoryManager();
            _otherMgr = new OtherManager();
            _enrollMgr = new EnrollManager();
            _studentMgr = new StudentManager();
            _teacherMgr = new TeacherManager();

            _cachedGroups = new List<IGrouping<string, TTDsLopTheoKiTheoTKB>>();
            _categoryCache = new ConcurrentDictionary<string, long>();
        }

        public enum SyncStatusFilter { All = 0, NotCreated = 1, Created = 2, Diff = 3, Synced = 4 }


        public int LoadAndGroupSqlData(int kyHoc, string maKyHoc, string keyword, string filterMaHe)
        {
            Log(string.Format("Đang tải dữ liệu SQL ({0}) để phân tích...", filterMaHe));
            _cachedMaKyHoc = maKyHoc;
            _categoryCache.Clear();

            var allSqlData = new List<TTDsLopTheoKiTheoTKB>();
            int fetchPage = 1;
            int fetchSize = 20000;

            while (true)
            {
                var chunk = _sqlDal.GetDsLopTheoKiTheoTKB(kyHoc, keyword, fetchPage, fetchSize);
                if (chunk == null || chunk.Count == 0) break;
                allSqlData.AddRange(chunk);
                if (chunk.Count < fetchSize) break;
                fetchPage++;
            }



            Log(string.Format("Đã tải {0} dòng chi tiết. Đang gom nhóm...", allSqlData.Count));
            _cachedGroups = allSqlData.GroupBy(x => x.MaLopTinChi).OrderBy(g => g.Key).ToList();
            Log(string.Format("-> Tổng hợp được: {0} lớp tín chỉ.", _cachedGroups.Count));
            return _cachedGroups.Count;
        }



        // Hàm này dùng chung để hiển thị lên GridView hoặc chạy Sync
        private List<SemesterSyncViewModel> ProcessGroups(List<IGrouping<string, TTDsLopTheoKiTheoTKB>> groups)
        {
            var result = new ConcurrentBag<SemesterSyncViewModel>();

            // A. Pre-load Cache SV
            var allMSSV = groups.SelectMany(g => g.Select(x => x.MaSinhVien)).Distinct().ToList();
            var missingStudents = MoodleCache.GetMissingKeys(allMSSV);
            if (missingStudents.Count > 0)
            {
                var batches = SplitList(missingStudents, 200);
                Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = 8 }, batch => _studentMgr.LoadSpecificUsersToCache(batch));
            }

            // B. Pre-load Cache GV
            var allGV = groups.Select(g => g.First().MaGiaoVien).Distinct().ToList();
            var missingGV = MoodleCache.GetMissingKeys(allGV);
            if (missingGV.Count > 0) _teacherMgr.LoadSpecificUsersToCache(missingGV);

            // C. Map Course ID
            // Format IDNumber: [MaKyHoc]_[MaLop] (Ví dụ: 241_MKT3001)
            var idNumbers = groups.Select(g => string.Format("{0}_{1}", _cachedMaKyHoc, g.Key)).ToList();
            var moodleMap = _courseMgr.GetCoursesByField("idnumber", idNumbers).ToDictionary(c => c.idnumber, c => c);

            // D. Parallel Process
            Parallel.ForEach(groups, new ParallelOptions { MaxDegreeOfParallelism = 5 }, group =>
            {
                string maLop = group.Key;
                string idNumber = string.Format("{0}_{1}", _cachedMaKyHoc, maLop);
                var firstRow = group.First();

                var vm = new SemesterSyncViewModel
                {
                    MaLopTinChi = maLop,
                    MonHoc = firstRow.MonHoc,
                    MaKyHoc = _cachedMaKyHoc,
                    MaGiaoVien = !string.IsNullOrEmpty(firstRow.MaGiaoVien) ? firstRow.MaGiaoVien.Trim() : "",
                    SiSoSQL = group.Select(x => x.MaSinhVien).Distinct().Count()
                };

                var sqlMSSVs = new HashSet<string>(group.Select(x => x.MaSinhVien.Trim().ToLower()).Where(x => !string.IsNullOrEmpty(x)).Distinct());

                if (moodleMap.ContainsKey(idNumber))
                {
                    var mCourse = moodleMap[idNumber];
                    vm.MoodleCourseId = mCourse.id;
                    vm.MoodleShortname = mCourse.shortname;

                    var enrolledUsers = _courseMgr.GetEnrolledUser(mCourse.id);
                    var moodleStudents = enrolledUsers.Where(u => u.Roles != null && u.Roles.ToLower().Contains("student")).ToList();

                    // Check GV có trong lớp chưa
                    string gvClean = vm.MaGiaoVien.ToLower();
                    bool isTeacherEnrolled = enrolledUsers.Any(u => u.Username.ToLower() == gvClean
                                             && u.Roles != null && (u.Roles.Contains("teacher") || u.Roles.Contains("editingteacher")));



                    vm.SiSoMoodle = moodleStudents.Count;

                    foreach (var s in moodleStudents)
                        if (!MoodleCache.Contains(s.Username)) MoodleCache.AddToCache(s.Username, new MoodleUser { id = s.Id, username = s.Username });

                    var moodleUsernames = new HashSet<string>(moodleStudents.Select(u => u.Username.Trim().ToLower()));

                    // [LỌC SINH VIÊN THIẾU -> OBJECT]
                    vm.ListData_SV_Thieu = group
                        .Where(x => !string.IsNullOrEmpty(x.MaSinhVien) && !moodleUsernames.Contains(x.MaSinhVien.Trim().ToLower()))
                        .Select(x => new TTDsSVTheoLopHP
                        {
                            MaSinhVien = x.MaSinhVien,
                            SVTen = x.SVTen,
                            SVHoLot = "", // Dữ liệu nguồn thiếu họ lót
                            SVNgaySinh = x.NgaySinh
                        })
                        .GroupBy(x => x.MaSinhVien).Select(g => g.First()) // Distinct
                        .ToList();

                    vm.SoLuongThieu = vm.ListData_SV_Thieu.Count;

                    var extra = moodleStudents.Where(u => !sqlMSSVs.Contains(u.Username.Trim().ToLower())).ToList();
                    vm.ListUserID_Thua = extra.Select(u => u.Id).ToList();
                    vm.SoLuongThua = extra.Count;

                    vm.TrangThai = (vm.SoLuongThieu == 0 && vm.SoLuongThua == 0) ? "Đã đồng bộ" : "Lệch số liệu";
                }
                else
                {
                    vm.MoodleCourseId = 0;
                    vm.SiSoMoodle = 0;
                    vm.SoLuongThieu = vm.SiSoSQL;
                    vm.TrangThai = "Chưa tạo";

                    // Chưa có course -> Thiếu toàn bộ -> Convert sang List Object
                    vm.ListData_SV_Thieu = group.Select(x => new TTDsSVTheoLopHP
                    {
                        MaSinhVien = x.MaSinhVien,
                        SVTen = x.SVTen,
                        SVHoLot = "",
                        SVNgaySinh = x.NgaySinh
                    })
                    .GroupBy(x => x.MaSinhVien).Select(g => g.First()).ToList();
                }
                result.Add(vm);
            });

            return result.ToList();
        }

        // =================================================================
        // 3. UI HELPERS
        // =================================================================
        public List<SemesterSyncViewModel> GetPageData(int page, int pageSize, SyncStatusFilter statusFilter)
        {
            if (_cachedGroups == null || _cachedGroups.Count == 0) return new List<SemesterSyncViewModel>();
            var pageGroups = _cachedGroups.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            Log(string.Format("Đang xử lý trang {0}...", page));
            return ProcessGroups(pageGroups).OrderBy(x => x.MaLopTinChi).ToList();
        }

        public List<SemesterSyncViewModel> GetAllMissingData()
        {
            if (_cachedGroups == null || _cachedGroups.Count == 0) return new List<SemesterSyncViewModel>();
            Log("Đang quét toàn bộ DB...");
            return ProcessGroups(_cachedGroups).Where(x => x.MoodleCourseId == 0 || x.SoLuongThieu > 0).ToList();
        }

        // =================================================================
        // 4. SYNC FUNCTIONS (PHÂN HỆ RÕ RÀNG)
        // =================================================================

        public void SyncAllDatabase_CQ(string maKyHoc)
        {
            if (_cachedGroups == null || _cachedGroups.Count == 0) { Log("Vui lòng tải dữ liệu trước."); return; }

            Log("=== BẮT ĐẦU ĐỒNG BỘ HỆ CHÍNH QUY (SEMESTER) ===");

            // 1. Chuẩn bị cây thư mục cho Chính Quy
            long finalCatId = EnsureSemesterCategoryTree(maKyHoc, isChinhQuy: true);
            if (finalCatId == 0) return;

            // 2. Thực hiện đồng bộ (Tái sử dụng logic Batch)
            ExecuteBatchSync(_cachedGroups, maKyHoc, finalCatId);

            Log("=== HOÀN TẤT ĐỒNG BỘ CHÍNH QUY ===");
        }


        public void SyncAllDatabase_VHVL(string maKyHoc)
        {
            if (_cachedGroups == null || _cachedGroups.Count == 0) { Log("Vui lòng tải dữ liệu trước."); return; }

            Log("=== BẮT ĐẦU ĐỒNG BỘ HỆ VHVL (SEMESTER) ===");
            Log("⚠️ Chức năng đang phát triển (Pending implementation)...");

            // Logic tương lai:
            // 1. Lọc ra các lớp thuộc hệ VHVL từ _cachedGroups (nếu trong Model có field phân loại hệ)
            // 2. long finalCatId = EnsureSemesterCategoryTree(maKyHoc, isChinhQuy: false);
            // 3. ExecuteBatchSync(vhvlGroups, maKyHoc, finalCatId);

            Log("=== KẾT THÚC (NO ACTION) ===");
        }

        // =================================================================
        // 5. SHARED BATCH PROCESSING
        // =================================================================
        private void ExecuteBatchSync(List<IGrouping<string, TTDsLopTheoKiTheoTKB>> groups, string maKyHoc, long categoryId)
        {
            int total = groups.Count;
            int processed = 0;
            int batchSize = 20;

            for (int i = 0; i < total; i += batchSize)
            {
                var batchGroups = groups.Skip(i).Take(batchSize).ToList();
                Log(string.Format("--- Lô {0}-{1}/{2} ---", i + 1, i + batchGroups.Count, total));

                // Phân tích lô dữ liệu (Tái sử dụng ProcessGroups)
                var batchVMs = ProcessGroups(batchGroups);

                foreach (var item in batchVMs)
                {
                    processed++;
                    try
                    {
                        if (item.MoodleCourseId == 0)
                            CreateAndEnrollNewCourse(item, categoryId); // Tạo mới vào Category đã định
                        else
                            FixDiffCourse(item); // Cập nhật (không đổi Category)
                    }
                    catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaLopTinChi, ex.Message)); }
                    ReportProgress(processed, total);
                }
            }
        }

        // =================================================================
        // 6. CATEGORY MANAGEMENT (LOGIC MỚI)
        // =================================================================
        private long EnsureSemesterCategoryTree(string maKyHoc, bool isChinhQuy)
        {
            // B1: Đảm bảo Root tổng (SEMESTER_ROOT)
            long rootId = GetOrCreateCategory(AppConstant.MoodleString.semesterIdNumber,
                                              AppConstant.MoodleString.semesterCategoryName, 0);

            // B2: Đảm bảo Root theo Hệ (CQ hoặc VHVL)
            string heIdNumber, heName;
            if (isChinhQuy)
            {
                heIdNumber = AppConstant.MoodleString.CQsemesterIdNumber;
                heName = AppConstant.MoodleString.CQsemesterCategoryName;
            }
            else
            {
                heIdNumber = AppConstant.MoodleString.VHVLsemesterIdNumber;
                heName = AppConstant.MoodleString.VHVLsemesterCategoryName;
            }

            long heCatId = GetOrCreateCategory(heIdNumber, heName, rootId);

            // B3: Đảm bảo Học kỳ con nằm trong Hệ
            // Format IDNumber học kỳ: [HE]_[MAKYHOC] (Ví dụ: CQ_241) để tránh trùng với hệ khác
            string semesterIdNumber = string.Format("{0}_{1}", isChinhQuy ? "CQ" : "VHVL", maKyHoc);

            // Lấy tên học kỳ từ DB hoặc tạo tên mặc định
            var kiHocInfo = _otherMgr.GetKyHoc(maKyHoc);
            string semesterName = (kiHocInfo != null) ? kiHocInfo.TEN_KY_HOC : ("Học kỳ " + maKyHoc);

            return GetOrCreateCategory(semesterIdNumber, semesterName, heCatId);
        }

        private long GetOrCreateCategory(string idNumber, string name, long parentId)
        {
            if (_categoryCache.ContainsKey(idNumber)) return _categoryCache[idNumber];

            var existing = _cateMgr.GetCategoryByIdNumber(idNumber);
            if (existing != null)
            {
                _categoryCache.TryAdd(idNumber, existing.id);
                return existing.id;
            }

            long newId = _cateMgr.CreateCategory(name, idNumber, "", (int)parentId);
            if (newId > 0)
            {
                _categoryCache.TryAdd(idNumber, newId);
                return newId;
            }

            throw new Exception("Không thể tạo Category: " + name);
        }

        // =================================================================
        // 7. CRUD ACTIONS (Create, Enroll, Fix)
        // =================================================================
        private void CreateAndEnrollNewCourse(SemesterSyncViewModel item, long categoryId)
        {
            string idNumber = string.Format("{0}_{1}", item.MaKyHoc, item.MaLopTinChi);
            string fullName = string.Format("[{0}] {1} [{2}]", item.MaKyHoc, item.MonHoc, item.MaLopTinChi);
            string shortName = idNumber; // Thường shortname trùng idnumber

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
                // Cố gắng tạo mới
                _courseMgr.CreateCoursesBatch(new List<MoodleCourse> { newCourse });
            }
            catch (Exception ex)
            {
                // [FIX] Bắt lỗi trùng Shortname
                if (ex.Message.Contains("shortnametaken") || ex.Message.Contains("already used"))
                {
                    Log(string.Format(" -> ⚠️ Shortname '{0}' đã tồn tại. Chuyển sang chế độ Cập nhật.", shortName));

                    // Tìm lại khóa học đang chiếm dụng shortname này
                    // Lưu ý: Hàm GetCoursesByField trả về List, ta lấy phần tử đầu tiên
                    var existingCourses = _courseMgr.GetCoursesByField("shortname", new List<string> { shortName });

                    if (existingCourses != null && existingCourses.Count > 0)
                    {
                        var existingCourse = existingCourses[0];

                        // Gán ID tìm được vào Item
                        item.MoodleCourseId = existingCourse.id;
                        item.MoodleShortname = existingCourse.shortname;

                        // Gọi hàm FixDiff để Enroll sinh viên vào khóa học cũ này
                        FixDiffCourse(item);
                        return; // Kết thúc, coi như đã xử lý xong
                    }
                }

                // Nếu là lỗi khác thì ném ra ngoài như bình thường
                throw ex;
            }

            // [LOGIC CŨ] Nếu tạo thành công thì chạy xuống đây
            var createdList = _courseMgr.GetCoursesByField("idnumber", new List<string> { idNumber });

            if (createdList.Count > 0)
            {
                Log(" -> Tạo course thành công.");
                long newId = createdList[0].id;

                // Truyền List Object xuống
                EnrollStudents(newId, item.ListData_SV_Thieu);
                EnrollTeacher(newId, item.MaGiaoVien);
            }
            else
            {
                // Trường hợp API báo thành công nhưng không tìm lại được ID (hiếm gặp)
                Log(" -> Lỗi: API báo tạo thành công nhưng không tìm thấy ID.");
            }
        }

        private void FixDiffCourse(SemesterSyncViewModel item)
        {
            // [FIX] Enroll Thiếu (List Object)
            if (item.ListData_SV_Thieu != null && item.ListData_SV_Thieu.Count > 0)
                EnrollStudents(item.MoodleCourseId, item.ListData_SV_Thieu);

            // Unenroll Thừa
            if (item.ListUserID_Thua != null && item.ListUserID_Thua.Count > 0)
            {
                _enrollMgr.UnenrollExtraStudents(item.MoodleCourseId, item.ListUserID_Thua);
                Log(string.Format(" -> Gỡ {0} sinh viên thừa.", item.ListUserID_Thua.Count));
            }

            // Enroll Teacher (Chỉ khi thiếu - Đã check ở ProcessGroups)
            // if (item.IsTeacherMissing) // Tùy chọn: Bật lên nếu muốn check kỹ
            EnrollTeacher(item.MoodleCourseId, item.MaGiaoVien);
        }

        private void EnrollStudents(long courseId, List<TTDsSVTheoLopHP> listStudentData)
        {
            int count = _studentMgr.EnrollStudentsToCourse(courseId, listStudentData);
            if (count > 0) Log(string.Format(" -> Enroll & Create {0} sinh viên.", count));
        }

        private void EnrollTeacher(long courseId, string teacherUsername)
        {
            if (string.IsNullOrEmpty(teacherUsername)) return;
            int count = _teacherMgr.EnrollTeachersToCourse(courseId, new List<string> { teacherUsername });
            if (count > 0) Log(string.Format(" -> Gán GV: {0}", teacherUsername));
        }

        private List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < locations.Count; i += nSize) list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            return list;
        }

        // Giữ lại hàm SyncList (Manual Sync) để tương thích với nút "Đồng bộ chọn" trên UI
        // Mặc định Sync manual sẽ vào hệ Chính Quy (hoặc bạn có thể truyền tham số)
        public void SyncList(List<SemesterSyncViewModel> itemsToSync, string maKyHoc)
        {
            // Mặc định manual sync vào hệ Chính Quy
            long categoryId = EnsureSemesterCategoryTree(maKyHoc, true);
            if (categoryId == 0) return;

            int total = itemsToSync.Count;
            int count = 0;

            foreach (var item in itemsToSync)
            {
                count++;
                Log(string.Format("[{0}/{1}] Xử lý: {2}...", count, total, item.MaLopTinChi));
                try
                {
                    if (item.MoodleCourseId == 0) CreateAndEnrollNewCourse(item, categoryId);
                    else FixDiffCourse(item);
                    ReportProgress(count, total);
                }
                catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaLopTinChi, ex.Message)); }
            }
            Log("--- Hoàn tất ---");
        }

        private void Log(string msg) { if (OnLogMessage != null) OnLogMessage(msg); }
        private void ReportProgress(int current, int total) { if (OnProgress != null) OnProgress(current, total); }
    }
}