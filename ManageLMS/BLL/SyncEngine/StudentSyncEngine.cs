using ManageLMS.BLL.Cache;
using ManageLMS.BLL.Manager.UserManager; // Đảm bảo đã có StudentManager trong này
using ManageLMS.Common.DTO.Database;     // Chứa TTSinhVien
using ManageLMS.Common.DTO.ViewModel;    // Chứa StudentSyncViewModel
using ManageLMS.DAL.Database;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ManageLMS.BLL.SyncEngine
{
    class StudentSyncEngine
    {
        private DataDaoTao _sqlDal;
        private StudentManager _stdMgr; // Logic xử lý sinh viên

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

        public StudentSyncEngine()
        {
            _sqlDal = new DataDaoTao();
            _stdMgr = new StudentManager();
        }


        public List<StudentSyncViewModel> SearchAndCompare(string khoa, string lop, string keyword, int page, int size, bool onlyMissing = false)
        {
            var finalViewList = new List<StudentSyncViewModel>();

            if (!onlyMissing)
            {
                
                var sqlList = _sqlDal.GetAllSinhVien(khoa, keyword, page, size, lop);
                ProcessList(sqlList, finalViewList, false);
                return finalViewList;
            }

            
            int currentSqlPage = page;
            int maxSqlPagesToScan = 20; // Quét 20 trang (20 * 200 = 4000 sv) để tìm
            int scannedCount = 0;

            while (finalViewList.Count < size && scannedCount < maxSqlPagesToScan)
            {
                // Lấy gói to (200 người)
                var sqlList = _sqlDal.GetAllSinhVien(khoa, keyword, currentSqlPage, 200, lop);

                if (sqlList.Count == 0) break; // Hết dữ liệu

                // Xử lý lọc
                ProcessList(sqlList, finalViewList, true);

                currentSqlPage++;
                scannedCount++;
            }

            // Cắt đúng số lượng hiển thị
            if (finalViewList.Count > size)
            {
                finalViewList = finalViewList.GetRange(0, size);
            }

            return finalViewList;
        }

        
        private void ProcessList(List<TTSinhVien> sqlList, List<StudentSyncViewModel> targetList, bool onlyMissing)
        {
            if (sqlList.Count == 0) return;

            
            var listUsernames = sqlList.Select(x => x.MaSinhVien.Trim()).Distinct().ToList();

            if (onlyMissing)
            {
                foreach (var u in listUsernames) MoodleCache.RemoveFromCache(u.ToLower());
            }

            try
            {
                _stdMgr.LoadSpecificUsersToCache(listUsernames);
            }
            catch { }

            // 2. So sánh từng sinh viên
            foreach (var sv in sqlList)
            {
                // Key Cache là MSSV viết thường
                string cacheKey = sv.MaSinhVien.Trim().ToLower();

                MoodleUser moodleInfo = MoodleCache.GetUser(cacheKey);
                bool onMoodle = (moodleInfo != null);

                if (onlyMissing && onMoodle) continue;

                // Tạo ViewModel (Giả sử bạn đã tạo class này kế thừa TTSinhVien)
                var viewItem = new StudentSyncViewModel(sv);

                if (onMoodle)
                {
                    viewItem.IsMissing = false;

                    // Check bị khóa
                    if (moodleInfo.suspended == true) // 1 = Suspended
                    {
                        viewItem.IsSuspended = true;
                        viewItem.TrangThaiMoodle = "Đã bị khóa";
                    }
                    else
                    {
                        viewItem.IsSuspended = false;
                        viewItem.TrangThaiMoodle = "Đang hoạt động";
                    }
                }
                else
                {
                    viewItem.IsMissing = true;
                    viewItem.IsSuspended = false;
                    viewItem.TrangThaiMoodle = "Chưa có";
                }

                targetList.Add(viewItem);
            }
        }

        // =============================================================
        // 2. ĐỒNG BỘ TỪ DANH SÁCH CHỌN (UI)
        // =============================================================
        public void SyncFromViewList(List<StudentSyncViewModel> viewList)
        {
            var listToCreate = viewList
                .Where(x => x.IsMissing == true && x.IsSelected == true)
                .Select(x => _stdMgr.ConvertToMoodleUser(x)) // Hàm convert bên StudentManager
                .ToList();

            if (listToCreate.Count > 0)
            {
                Log(string.Format("Tìm thấy {0} sinh viên cần tạo mới.", listToCreate.Count));

                ExecuteBatchCreation(listToCreate);

                // Cập nhật Cache
                foreach (var u in listToCreate)
                {
                    u.suspended = true;
                    MoodleCache.AddToCache(u.username, u);
                }

                Log("-> Đã cập nhật Cache cục bộ.");
            }
            else
            {
                Log("-> Không có sinh viên nào cần tạo mới trong danh sách chọn.");
            }
        }


        public void SyncAllDatasbase()
        {
            int page = 1;
            int pageSize = 200;
            bool hasData = true;

            // Mặc định Sync All là không lọc Khoa/Lớp (hoặc bạn có thể truyền tham số vào đây nếu muốn)
            string filterKhoa = "";
            string filterLop = "";
            string filterKey = "";

            while (hasData)
            {
                Log(string.Format("--- Đang xử lý trang {0} ---", page));

                // 1. Lấy dữ liệu SQL
                var listSql = _sqlDal.GetAllSinhVien(filterKhoa, filterKey, page, pageSize, filterLop);

                if (listSql.Count == 0)
                {
                    hasData = false;
                    break;
                }

                // 2. Load Cache
                var listUsernames = listSql.Select(x => x.MaSinhVien.Trim()).Distinct().ToList();
                try { _stdMgr.LoadSpecificUsersToCache(listUsernames); }
                catch { }

                // 3. Lọc người thiếu
                var usersToCreate = new List<MoodleUser>();

                foreach (var sv in listSql)
                {
                    string key = sv.MaSinhVien.Trim().ToLower();
                    if (!MoodleCache.Contains(key))
                    {
                        var moodleUser = _stdMgr.ConvertToMoodleUser(sv);
                        usersToCreate.Add(moodleUser);
                    }
                }

                // 4. Tạo Batch
                if (usersToCreate.Count > 0)
                {
                    Log(string.Format("-> Trang {0}: Phát hiện {1} SV thiếu. Đang đồng bộ...", page, usersToCreate.Count));
                    ExecuteBatchCreation(usersToCreate);

                    foreach (var u in usersToCreate) MoodleCache.AddToCache(u.username, u);
                }
                else
                {
                    Log("-> Trang này đã đồng bộ hết.");
                }

                page++;
                Thread.Sleep(500);
            }
        }

        private void ExecuteBatchCreation(List<MoodleUser> usersToCreate)
        {
            int batchSize = 50;
            int total = usersToCreate.Count;
            int processed = 0;

            for (int i = 0; i < total; i += batchSize)
            {
                int count = Math.Min(batchSize, total - i);
                var batch = usersToCreate.GetRange(i, count);

                try
                {
                    _stdMgr.CreateUsersBatch(batch);
                    processed += count;
                    Log(string.Format("   + Đã tạo batch {0} SV (Tổng: {1}/{2})", count, processed, total));
                }
                catch (Exception ex)
                {
                    Log("   [LỖI] Batch index " + i + ": " + ex.Message);
                }

                ReportProgress(processed, total);
                Thread.Sleep(500);
            }
        }


        public void SyncUpdateFromViewList(List<StudentSyncViewModel> viewList)
        {
            var listToUpdate = viewList
                .Where(x => x.IsSelected == true && x.IsMissing == false)
                .ToList();

            if (listToUpdate.Count == 0) return;

            Log(string.Format("Đang cập nhật thông tin cho {0} sinh viên...", listToUpdate.Count));

            var moodleUsers = new List<MoodleUser>();
            foreach (var item in listToUpdate)
            {
                var user = _stdMgr.ConvertToMoodleUser(item); // Convert TTSinhVien -> MoodleUser

                // Lấy ID từ Cache để biết update ai
                var cachedUser = MoodleCache.GetUser(user.username);
                if (cachedUser != null && cachedUser.id > 0)
                {
                    user.id = cachedUser.id;
                    moodleUsers.Add(user);
                }
            }

            _stdMgr.UpdateUsersBatch(moodleUsers);

            foreach (var u in moodleUsers) MoodleCache.AddToCache(u.username, u);

            Log("-> Cập nhật hoàn tất.");
        }

        // Helper Logs
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