using ManageLMS.BLL.Cache;
using ManageLMS.BLL.Manager.EnrollManager;
using ManageLMS.Common.DTO.Database;
using ManageLMS.Common.Helpers;
using ManageLMS.DTO.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManageLMS.BLL.Manager.UserManager
{
    public class TeacherManager : UserManager
    {
        private EnrollManager.EnrollManager _enrollMgr;

        public TeacherManager()
        {
            _enrollMgr = new EnrollManager.EnrollManager();
        }

        // =============================================================
        // 1. XỬ LÝ DATA & CACHE
        // =============================================================

        private string CleanUsername(string raw)
        {
            return string.IsNullOrEmpty(raw) ? string.Empty : raw.Trim().ToLower();
        }

        public void LoadSpecificUsersToCache(List<string> listMaGiaoVien)
        {
            try
            {
                List<string> listUsernames = listMaGiaoVien
                                                .Where(x => !string.IsNullOrEmpty(x))
                                                .Select(x => CleanUsername(x))
                                                .Distinct()
                                                .ToList();

                if (listUsernames.Count == 0) return;

                List<MoodleUser> users = _userRepo.GetUsersByListUsername(listUsernames);

                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.username))
                    {
                        MoodleCache.AddToCache(user.username, user);
                    }
                }
            }
            catch { }
        }

        public MoodleUser ConvertToMoodleUser(TTGiangVien gvSql)
        {
            string username = CleanUsername(gvSql.MaGiaoVien);
            string email = !string.IsNullOrWhiteSpace(gvSql.E_Mail) ? gvSql.E_Mail.Trim() : string.Format("{0}@due.edu.vn",username);

            return new MoodleUser
            {
                username = username,
                password = AppConstant.Defaults.PasswordPrefix,
                firstname = gvSql.Ten,
                lastname = gvSql.HoLot,
                email = email,
                idnumber = gvSql.MaGiaoVien.Trim(),
                auth = "manual"
                
            };
        }

        // =============================================================
        // 2. ENROLL LOGIC (Role: editingteacher)
        // =============================================================
        public int EnrollTeachersToCourse(long courseId, List<string> listMaGV)
        {
            if (listMaGV == null || listMaGV.Count == 0) return 0;

            // 1. Chuẩn hóa
            var distinctUsernames = listMaGV
                                    .Select(x => CleanUsername(x))
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Distinct()
                                    .ToList();

            if (distinctUsernames.Count == 0) return 0;

            // 2. Tìm cache thiếu
            var missingInCache = distinctUsernames
                                    .Where(u => !MoodleCache.Contains(u))
                                    .ToList();

            // 3. Gọi API Batch
            if (missingInCache.Count > 0)
            {
                var usersFromApi = GetUsersByListUsername(missingInCache);
                foreach (var user in usersFromApi)
                {
                    if (user != null && !string.IsNullOrEmpty(user.username))
                    {
                        MoodleCache.AddToCache(user.username, user);
                    }
                }
            }

            // 4. Lấy ID
            var userIdsToEnroll = new List<long>();
            foreach (var username in distinctUsernames)
            {
                var user = MoodleCache.GetUser(username);
                if (user != null && user.id > 0)
                {
                    userIdsToEnroll.Add(user.id);
                }
            }

            // 5. Enroll (Role: editingteacher)
            if (userIdsToEnroll.Count > 0)
            {
                // Lưu ý role giảng viên
                _enrollMgr.EnrollUsersToCourse(courseId, userIdsToEnroll, AppConstant.MoodleRoles.Teacher);
                return userIdsToEnroll.Count;
            }

            return 0;
        }
    }
}