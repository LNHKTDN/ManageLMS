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
        }

        public enum SyncStatusFilter { All = 0, NotCreated = 1, Created = 2, Diff = 3, Synced = 4 }

        
        public int LoadAndGroupSqlData(int kyHoc, string maKyHoc, string keyword)
        {
            Log("Đang tải TOÀN BỘ dữ liệu từ SQL để phân tích...");
            _cachedMaKyHoc = maKyHoc;
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

        // 2. Process Logic (SỬA ĐỔI QUAN TRỌNG TẠI ĐÂY)
        private List<SemesterSyncViewModel> ProcessGroups(List<IGrouping<string, TTDsLopTheoKiTheoTKB>> groups)
        {
            var result = new ConcurrentBag<SemesterSyncViewModel>();

            // Pre-load Cache SV
            var allMSSV = groups.SelectMany(g => g.Select(x => x.MaSinhVien)).Distinct().ToList();
            var missingStudents = MoodleCache.GetMissingKeys(allMSSV);
            if (missingStudents.Count > 0)
            {
                var batches = SplitList(missingStudents, 200);
                Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = 8 }, batch => _studentMgr.LoadSpecificUsersToCache(batch));
            }

            // Pre-load Cache GV
            var allGV = groups.Select(g => g.First().MaGiaoVien).Distinct().ToList();
            var missingGV = MoodleCache.GetMissingKeys(allGV);
            if (missingGV.Count > 0) _teacherMgr.LoadSpecificUsersToCache(missingGV);

            var idNumbers = groups.Select(g => string.Format("{0}_{1}", _cachedMaKyHoc, g.Key)).ToList();
            var moodleMap = _courseMgr.GetCoursesByField("idnumber", idNumbers).ToDictionary(c => c.idnumber, c => c);

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

                    // [SỬA ĐỔI]: Tạo List Object cho SV thiếu
                    // Map TTDsLopTheoKiTheoTKB -> TTDsSVTheoLopHP (để tái sử dụng DTO chuẩn)
                    vm.ListData_SV_Thieu = group
                        .Where(x => !string.IsNullOrEmpty(x.MaSinhVien) && !moodleUsernames.Contains(x.MaSinhVien.Trim().ToLower()))
                        .Select(x => new TTDsSVTheoLopHP
                        {
                            MaSinhVien = x.MaSinhVien,
                            SVTen = x.SVTen,
                            // Lưu ý: TTDsLopTheoKiTheoTKB có thể thiếu cột HoLot, nên ta chấp nhận rỗng hoặc map tạm
                            SVHoLot = "",
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

        // 4. Sync Functions
        public void SyncList(List<SemesterSyncViewModel> itemsToSync, string maKyHoc)
        {
            long categoryId = EnsureCategoryExists(maKyHoc);
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

        public void SyncAllDatabase(string maKyHoc)
        {
            if (_cachedGroups == null || _cachedGroups.Count == 0) { Log("Vui lòng tải dữ liệu trước."); return; }
            Log("=== BẮT ĐẦU SYNC ALL (SEMESTER) ===");
            long categoryId = EnsureCategoryExists(maKyHoc);
            if (categoryId == 0) return;

            int total = _cachedGroups.Count;
            int processed = 0;
            int batchSize = 20;

            for (int i = 0; i < total; i += batchSize)
            {
                var batchGroups = _cachedGroups.Skip(i).Take(batchSize).ToList();
                Log(string.Format("--- Lô {0}-{1}/{2} ---", i + 1, i + batchGroups.Count, total));
                var batchVMs = ProcessGroups(batchGroups);

                foreach (var item in batchVMs)
                {
                    processed++;
                    try
                    {
                        if (item.MoodleCourseId == 0) CreateAndEnrollNewCourse(item, categoryId);
                        else FixDiffCourse(item);
                    }
                    catch (Exception ex) { Log(string.Format("Lỗi {0}: {1}", item.MaLopTinChi, ex.Message)); }
                    ReportProgress(processed, total);
                }
            }
            Log("=== HOÀN TẤT SYNC ALL ===");
        }

        // 5. Internal Helpers
        private void CreateAndEnrollNewCourse(SemesterSyncViewModel item, long categoryId)
        {
            string idNumber = string.Format("{0}_{1}", item.MaKyHoc, item.MaLopTinChi);
            string fullName = string.Format("[{0}] {1} [{2}]", item.MaKyHoc, item.MonHoc, item.MaLopTinChi);
            var newCourse = new MoodleCourse { fullname = fullName, shortname = idNumber, idnumber = idNumber, category = (int)categoryId, visible = 1, format = "topics" };

            _courseMgr.CreateCoursesBatch(new List<MoodleCourse> { newCourse });
            var createdList = _courseMgr.GetCoursesByField("idnumber", new List<string> { idNumber });

            if (createdList.Count > 0)
            {
                Log(" -> Tạo course thành công.");
                long newId = createdList[0].id;
                // [FIX] Truyền List Object xuống
                EnrollStudents(newId, item.ListData_SV_Thieu);
                EnrollTeacher(newId, item.MaGiaoVien);
            }
            else Log(" -> Lỗi tạo course.");
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


        }

        // [FIX] Sửa tham số nhận List Object
        private void EnrollStudents(long courseId, List<TTDsSVTheoLopHP> listStudentData)
        {
            // Gọi StudentManager (Hàm này đã được sửa để tạo user mới rồi)
            int count = _studentMgr.EnrollStudentsToCourse(courseId, listStudentData);
            if (count > 0) Log(string.Format(" -> Enroll & Create {0} sinh viên.", count));
        }

        private void EnrollTeacher(long courseId, string teacherUsername)
        {
            if (string.IsNullOrEmpty(teacherUsername)) return;
            int count = _teacherMgr.EnrollTeachersToCourse(courseId, new List<string> { teacherUsername });
            if (count > 0) Log(string.Format(" -> Gán GV: {0}", teacherUsername));
        }

        private long EnsureCategoryExists(string maKyHoc)
        {
            var existingCat = _cateMgr.GetCategoryByIdNumber(maKyHoc);
            if (existingCat != null) return existingCat.id;
            var kiHocInfo = _otherMgr.GetKyHoc(maKyHoc);
            string catName = (kiHocInfo != null) ? kiHocInfo.TEN_KY_HOC : ("Học kỳ " + maKyHoc);
            return _cateMgr.CreateSemesterCategory(catName, maKyHoc);
        }

        private List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < locations.Count; i += nSize) list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            return list;
        }

        private bool CheckFilter(SemesterSyncViewModel item, SyncStatusFilter filter) { return true; }
        private void Log(string msg) { if (OnLogMessage != null) OnLogMessage(msg); }
        private void ReportProgress(int current, int total) { if (OnProgress != null) OnProgress(current, total); }
    }
}