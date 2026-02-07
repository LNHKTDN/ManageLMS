using ManageLMS.BLL.Cache;
using ManageLMS.BLL.Manager.EnrollManager;
using ManageLMS.Common.DTO.Database;
using ManageLMS.Common.Helpers;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManageLMS.BLL.Manager.UserManager
{
    public class StudentManager : UserManager
    {
        private EnrollManager.EnrollManager _enrollMgr;

        public StudentManager()
        {
            _enrollMgr = new EnrollManager.EnrollManager();
        }

        private string CleanUsername(string raw)
        {
            return string.IsNullOrEmpty(raw) ? string.Empty : raw.Trim().ToLower();
        }

        public void LoadSpecificUsersToCache(List<string> listStudent)
        {
            try
            {
                var distinctUsernames = listStudent
                                        .Where(x => !string.IsNullOrEmpty(x))
                                        .Select(x => CleanUsername(x))
                                        .Distinct()
                                        .ToList();

                if (distinctUsernames.Count == 0) return;

                var missingInCache = distinctUsernames.Where(u => !MoodleCache.Contains(u)).ToList();

                if (missingInCache.Count > 0)
                {
                    List<MoodleUser> users = _userRepo.GetUsersByListUsername(missingInCache);
                    foreach (var user in users)
                    {
                        if (!string.IsNullOrEmpty(user.username))
                            MoodleCache.AddToCache(user.username, user);
                    }
                }
            }
            catch { }
        }

        // Overload 1: Dùng cho TTSinhVien (nếu có)
        public MoodleUser ConvertToMoodleUser(TTSinhVien svSql)
        {
            return new MoodleUser
            {
                username = CleanUsername(svSql.MaSinhVien),
                password = AppConstant.Defaults.PasswordPrefix,
                firstname = svSql.Ten,
                lastname = svSql.HoLot,
                email = string.Format("{0}@due.udn.vn", CleanUsername(svSql.MaSinhVien)),
                idnumber = svSql.MaSinhVien.Trim(),
                auth = "manual"
            };
        }

        // [FIX LỖI] Overload 2: Dùng cho TTDsSVTheoLopHP (Master/Semester)
        public MoodleUser ConvertToMoodleUser(TTDsSVTheoLopHP svSql)
        {
            string cleanUsername = CleanUsername(svSql.MaSinhVien);
            // Xử lý an toàn cho Họ Tên
            string firstname = !string.IsNullOrEmpty(svSql.SVTen) ? svSql.SVTen : "Ten";
            string lastname = !string.IsNullOrEmpty(svSql.SVHoLot) ? svSql.SVHoLot : "Ho";

            return new MoodleUser
            {
                username = cleanUsername,
                password = AppConstant.Defaults.PasswordPrefix,
                firstname = firstname,
                lastname = lastname,
                email = string.Format("{0}@due.udn.vn", cleanUsername),
                idnumber = svSql.MaSinhVien.Trim(),
                auth = "manual"
            };
        }

        // [HÀM CHÍNH] Enroll có tạo User mới
        public int EnrollStudentsToCourse(long courseId, List<TTDsSVTheoLopHP> listStudentData)
        {
            if (listStudentData == null || listStudentData.Count == 0) return 0;

            // 1. Load Cache trước để kiểm tra
            var listUsernames = listStudentData
                                .Select(x => CleanUsername(x.MaSinhVien))
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct()
                                .ToList();

            if (listUsernames.Count == 0) return 0;

            LoadSpecificUsersToCache(listUsernames);

            // 2. Phân loại
            var userIdsToEnroll = new List<long>();
            var usersToCreate = new List<MoodleUser>();

            foreach (var sv in listStudentData)
            {
                string username = CleanUsername(sv.MaSinhVien);
                if (string.IsNullOrEmpty(username)) continue;

                var cachedUser = MoodleCache.GetUser(username);

                if (cachedUser != null && cachedUser.id > 0)
                {
                    userIdsToEnroll.Add(cachedUser.id);
                }
                else
                {
                    // Convert sang MoodleUser để tạo mới
                    usersToCreate.Add(ConvertToMoodleUser(sv));
                }
            }

            // 3. Tạo mới Batch (QUAN TRỌNG: Phải gọi hàm CreateUsersBatch)
            if (usersToCreate.Count > 0)
            {
                // Gọi hàm tạo user xuống DB Moodle
                CreateUsersBatch(usersToCreate);

                // Sau khi tạo xong, load lại cache để lấy ID mới sinh ra
                var newNames = usersToCreate.Select(u => u.username).ToList();
                LoadSpecificUsersToCache(newNames);

                // Thêm các ID mới vào danh sách cần enroll
                foreach (var newUser in usersToCreate)
                {
                    var u = MoodleCache.GetUser(newUser.username);
                    if (u != null && u.id > 0) userIdsToEnroll.Add(u.id);
                }
            }

            // 4. Enroll (Chia lô 200 để tránh lỗi Max Input Vars)
            if (userIdsToEnroll.Count > 0)
            {
                userIdsToEnroll = userIdsToEnroll.Distinct().ToList();
                int totalEnrolled = 0;
                int batchSize = 200;

                for (int i = 0; i < userIdsToEnroll.Count; i += batchSize)
                {
                    var batchIds = userIdsToEnroll.Skip(i).Take(batchSize).ToList();
                    _enrollMgr.EnrollUsersToCourse(courseId, batchIds, AppConstant.MoodleRoles.Student);
                    totalEnrolled += batchIds.Count;
                }
                return totalEnrolled;
            }

            return 0;
        }
    }
}