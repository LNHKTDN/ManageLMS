using ManageLMS.BLL.Manager.CourseManager;
using ManageLMS.BLL.Manager.EnrollManager;
using ManageLMS.BLL.Manager.UserManager; // Cần thêm UserManager để map MSSV -> UserID
using ManageLMS.Common.DTO.Database;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.Common.Helpers;
using ManageLMS.DAL.Database;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManageLMS.BLL.SyncEngine
{
    public class CourseSyncEngine
    {
        private DataDaoTao _sqlDal;
        private CourseManager _courseMgr;
        private UserManager _userMgr;
        private EnrollManager _enrollMgr;
        
        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

        public CourseSyncEngine()
        {
            _sqlDal = new DataDaoTao();
            _courseMgr = new CourseManager();
            _userMgr = new UserManager();
        }

        public void SyncSemesterCourses(int kyHoc, string maKyHoc, int categoryId)
        {
            Log("--- BẮT ĐẦU ĐỒNG BỘ HỌC KỲ " + maKyHoc + " ---");


            Log("Đang tải dữ liệu từ SQL...");
            var rawData = _sqlDal.GetDsLopTheoKiTheoTKB(kyHoc, "", 1, 10000);

            if (rawData == null || rawData.Count == 0)
            {
                Log("Không tìm thấy dữ liệu lớp học phần nào.");
                return;
            }


            var courseGroups = rawData.GroupBy(x => x.MaLopTinChi).ToList();
            int totalCourses = courseGroups.Count;
            int processedCount = 0;

            Log(string.Format("Tìm thấy {0} lớp tín chỉ. Bắt đầu xử lý...", totalCourses));

            // 3. Xử lý theo Batch (Ví dụ 50 khóa / lần gửi)
            int batchSize = 50; 

            for (int i = 0; i < totalCourses; i += batchSize)
            {
                var batchGroups = courseGroups.Skip(i).Take(batchSize).ToList();

                try
                {
                    ProcessCourseBatch(batchGroups, maKyHoc, categoryId);
                }
                catch (Exception ex)
                {
                    Log(string.Format("[LỖI BATCH {0}] {1}",i,ex.Message));
                }

                processedCount += batchGroups.Count;
                ReportProgress(processedCount, totalCourses);
                
                // Nghỉ 1 chút để không spam server quá gắt
                Thread.Sleep(500); 
            }

            Log("--- HOÀN TẤT ĐỒNG BỘ ---");
        }


        private void ProcessCourseBatch(List<IGrouping<string, TTDsLopTheoKiTheoTKB>> batchGroups, string maKyHoc, int categoryId)
        {
            
            var coursesToCreate = new List<MoodleCourse>();
            var courseStudentsMap = new Dictionary<string, List<string>>();

            foreach (var g in batchGroups)
            {
                string maLopTinChi = g.Key;
                string tenMonHoc = g.First().MonHoc;

                
                string uniqueIdNumber = string.Format("{0}_{1}", maKyHoc, maLopTinChi);

                
                string fullName = string.Format("[{0}] {1} [{2}]", maKyHoc, tenMonHoc, maLopTinChi);

                
                string shortName = uniqueIdNumber;

                var moodleCourse = new MoodleCourse
                {
                    fullname = fullName,
                    shortname = shortName,
                    idnumber = uniqueIdNumber,
                    category = categoryId,
                    visible = 1,
                    format = "topics"
                };

                coursesToCreate.Add(moodleCourse);

                
                var listMSSV = g.Select(x => x.MaSinhVien)
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct()
                                .ToList();

                courseStudentsMap[uniqueIdNumber] = listMSSV;
            }

            
            try
            {
                _courseMgr.CreateCoursesBatch(coursesToCreate);
                Log(string.Format("-> Đã gửi lệnh tạo/cập nhật {0} khóa học.", coursesToCreate.Count));
            }
            catch (Exception ex)
            {
                Log("Lỗi tạo khóa học: " + ex.Message);
                return; // Nếu tạo lỗi thì dừng batch này luôn
            }


            var listIdNumbers = coursesToCreate.Select(x => x.idnumber).ToList();

            var moodleCourses = _courseMgr.GetCoursesByField("idnumber", listIdNumbers);

            if (moodleCourses == null || moodleCourses.Count == 0)
            {
                Log("Không lấy được thông tin khóa học sau khi tạo. Bỏ qua enroll.");
                return;
            }

            // 2. Duyệt từng khóa học đã lấy được ID
            foreach (var mc in moodleCourses)
            {
                // Kiểm tra xem khóa này có trong map sinh viên không
                if (courseStudentsMap.ContainsKey(mc.idnumber))
                {
                    var listMSSV = courseStudentsMap[mc.idnumber];
                    if (listMSSV.Count == 0) continue;

                    List<long> userIds = new List<long>();

                    foreach (string mssv in listMSSV)
                    {                   
                        long uid = _userMgr.GetMoodleIdByUsername(mssv);

                        if (uid > 0)
                        {
                            userIds.Add(uid);
                        }
                        else
                        {
                            // Console.WriteLine($"Cảnh báo: Không tìm thấy user có IDNumber = {mssv}");
                        }
                    }

                    if (userIds != null && userIds.Count > 0)
                    {

                        try
                        {
                            _enrollMgr.EnrollUsersToCourse(mc.id, userIds, AppConstant.MoodleRoles.Student);
                            Log(string.Format("   + {0}: Enrolled {1}/{2} sinh viên.", mc.shortname, userIds.Count, listMSSV.Count));
                        }
                        catch (Exception ex)
                        {
                            Log(string.Format("   ! Lỗi enroll lớp {0}: {1}", mc.shortname, ex.Message));
                        }
                    }
                    else
                    {
                        Log(string.Format("   - {0}: Không tìm thấy User ID nào khớp với danh sách MSSV.", mc.shortname));
                    }
                }
            }
        }
        public List<SemesterSyncViewModel> SearchAndCompare(int kyHoc, string maKyHoc, string keyword, int page, int pageSize, bool onlyDiff = false)
        {
            var resultList = new List<SemesterSyncViewModel>();

            Log(string.Format("Đang tải dữ liệu SQL trang {0}...",page));

            // 1. Lấy dữ liệu thô từ SQL (Đã bao gồm danh sách sinh viên)
            var rawData = _sqlDal.GetDsLopTheoKiTheoTKB(kyHoc, keyword, page, pageSize);

            if (rawData == null || rawData.Count == 0) return resultList;

            // 2. Group theo Mã lớp tín chỉ
            var sqlGroups = rawData.GroupBy(x => x.MaLopTinChi).ToList();
            Log(string.Format("Tìm thấy {0} lớp trong SQL. Đang đối chiếu với Moodle...",sqlGroups.Count));

            // 3. Chuẩn bị danh sách IDNumber để check trên Moodle 1 lần (Batch Get)
            var idNumbersToCheck = sqlGroups.Select(g => string.Format("{0}_{1}",maKyHoc,g.Key)).ToList();
            
            // Lấy thông tin cơ bản các Course từ Moodle
            var moodleCourses = _courseMgr.GetCoursesByField("idnumber", idNumbersToCheck);
            var moodleCourseMap = moodleCourses.ToDictionary(c => c.idnumber, c => c);

            // 4. Duyệt từng lớp để so sánh chi tiết
            // Dùng ConcurrentBag để thread-safe khi chạy Parallel
            var processedList = new ConcurrentBag<SemesterSyncViewModel>();
            int processedCount = 0;
            int total = sqlGroups.Count;

            // Chạy song song để tăng tốc độ gọi API GetEnrolledUser (Vì phải gọi từng khóa)
            Parallel.ForEach(sqlGroups, new ParallelOptions { MaxDegreeOfParallelism = 5 }, group =>
            {
                string maLop = group.Key;
                string idNumber = string.Format("{0}_{1}",maKyHoc,maKyHoc);
                var firstRow = group.First();

                var vm = new SemesterSyncViewModel
                {
                    MaLopTinChi = maLop,
                    MonHoc = firstRow.MonHoc,
                    MaKyHoc = maKyHoc,
                    SiSoSQL = group.Select(x => x.MaSinhVien).Distinct().Count()
                };

                var sqlMSSVs = new HashSet<string>(
                    group.Select(x => x.MaSinhVien.Trim().ToLower())
                         .Where(x => !string.IsNullOrEmpty(x))
                         .Distinct()
                );

                // Check xem Moodle có khóa này chưa
                if (moodleCourseMap.ContainsKey(idNumber))
                {
                    var mCourse = moodleCourseMap[idNumber];
                    vm.MoodleCourseId = mCourse.id;
                    vm.MoodleShortname = mCourse.shortname;

                    // [QUAN TRỌNG] Lấy danh sách Enroll thực tế để so sánh
                    var enrolledUsers = _courseMgr.GetEnrolledUser(mCourse.id);

                    // Lọc lấy Role Student và lấy Username (MSSV)
                    // Lưu ý: Kiểm tra null cho Roles để tránh lỗi
                    var moodleStudents = enrolledUsers
                                            .Where(u => u.Roles != null && u.Roles.ToLower().Contains("student"))
                                            .ToList();

                    vm.SiSoMoodle = moodleStudents.Count;

                    // --- SO SÁNH ---

                    // 2. Tạo HashSet danh sách username trên Moodle để tra cứu cho nhanh
                    // SỬA: Thay .ToHashSet() bằng new HashSet<string>(...)
                    var moodleUsernames = new HashSet<string>(
                        moodleStudents.Select(u => u.Username.Trim().ToLower())
                    );

                    // 3. Tìm SV có trong SQL mà thiếu trên Moodle (Có trong sqlMSSVs nhưng k có trong moodleUsernames)
                    vm.ListMSSV_Thieu = sqlMSSVs.Where(s => !moodleUsernames.Contains(s)).ToList();
                    vm.SoLuongThieu = vm.ListMSSV_Thieu.Count;

                    // 4. Tìm SV thừa trên Moodle (Có trong moodleStudents nhưng username k có trong sqlMSSVs)
                    var extraStudents = moodleStudents.Where(u => !sqlMSSVs.Contains(u.Username.Trim().ToLower())).ToList();

                    vm.ListUserID_Thua = extraStudents.Select(u => u.Id).ToList();
                    vm.SoLuongThua = extraStudents.Count;
                }
                else
                {
                    
                    vm.MoodleCourseId = 0;
                    vm.SiSoMoodle = 0;
                    vm.SoLuongThieu = vm.SiSoSQL; // Thiếu toàn bộ
                    vm.TrangThai = "Chưa tạo Course";
                    vm.ListMSSV_Thieu = sqlMSSVs.ToList(); // Cần enroll hết sau khi tạo
                }

                // Logic lọc "Only Diff"
                if (!onlyDiff || (onlyDiff && vm.TrangThai != "Đã đồng bộ"))
                {
                    processedList.Add(vm);
                }

                // Update progress an toàn thread
                System.Threading.Interlocked.Increment(ref processedCount);
                ReportProgress(processedCount, total);
            });

            // Sắp xếp lại theo mã lớp cho đẹp vì Parallel làm lộn xộn
            resultList = processedList.OrderBy(x => x.MaLopTinChi).ToList();

            return resultList;
        }
        // --- Helper Methods ---
        private void Log(string msg)
        {
            if (OnLogMessage != null) OnLogMessage(msg);
        }

        private void ReportProgress(int current, int total)
        {
            if (OnProgress != null) OnProgress(current, total);
        }
    }
}