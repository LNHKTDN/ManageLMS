using ManageLMS.BLL.Cache;
using ManageLMS.BLL.Manager.UserManager;
using ManageLMS.Common.DTO.Database;
using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.DAL.Database;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ManageLMS.BLL.SyncEngine
{
    class TeacherSyncEngine
    {
        private DataDaoTao _sqlDal;
        private TeacherManager _teacherMgr;

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgress;

        public TeacherSyncEngine()
        {
            _sqlDal = new DataDaoTao();
            _teacherMgr = new TeacherManager();
        }


        public List<TeacherSyncViewModel> SearchAndCompare(string keyword, int page, int size, bool onlyMissing = false)
        {
            var finalViewList = new List<TeacherSyncViewModel>();

            if (!onlyMissing)
            {
                var sqlList = _sqlDal.GetDsGiangVien(keyword, page, size);
                ProcessList(sqlList, finalViewList, false);
                return finalViewList;
            }

            
            int currentSqlPage = page;
            int maxSqlPagesToScan = 20; 
            int scannedCount = 0;

            while (finalViewList.Count < size && scannedCount < maxSqlPagesToScan)
            {
                
                var sqlList = _sqlDal.GetDsGiangVien(keyword, currentSqlPage, 200);

                if (sqlList.Count == 0) break;

                
                ProcessList(sqlList, finalViewList, true);

                currentSqlPage++;
                scannedCount++;
            }

            
            if (finalViewList.Count > size)
            {
                finalViewList = finalViewList.GetRange(0, size);
            }

            return finalViewList;
        }

        
        private void ProcessList(List<TTGiangVien> sqlList, List<TeacherSyncViewModel> targetList, bool onlyMissing)
        {
            if (sqlList.Count == 0) return;

            
            var listUsernames = sqlList.Select(x => x.MaGiaoVien.Trim()).Distinct().ToList();

            
            if (onlyMissing)
            {
                foreach (var u in listUsernames) MoodleCache.RemoveFromCache(u.ToLower());
            }

            try
            {
                _teacherMgr.LoadSpecificUsersToCache(listUsernames);
            }
            catch { }
            foreach (var gv in sqlList)
            {
                string cacheKey = gv.MaGiaoVien.Trim().ToLower();
                MoodleUser moodleInfo = MoodleCache.GetUser(cacheKey); // Lấy object user từ cache
                bool onMoodle = (moodleInfo != null);

                if (onlyMissing && onMoodle) continue;

                var viewItem = new TeacherSyncViewModel(gv);

                if (onMoodle)
                {
                    viewItem.IsMissing = false;

                    if (moodleInfo.suspended == true)
                    {
                        viewItem.IsSuspended = true;
                        viewItem.TrangThaiMoodle = "Đã bị khóa";
                        viewItem.HanhDongGoiY = "Mở khóa";
                    }
                    else
                    {
                        viewItem.IsSuspended = false;
                        viewItem.TrangThaiMoodle = "Đang hoạt động";
                        viewItem.HanhDongGoiY = "Bỏ qua";
                    }
                }
                else
                {
                    viewItem.IsMissing = true;
                    viewItem.IsSuspended = false;
                    viewItem.TrangThaiMoodle = "Chưa có";
                    viewItem.HanhDongGoiY = "Tạo mới";
                }

                targetList.Add(viewItem);
            }
        }


        public void SyncFromViewList(List<TeacherSyncViewModel> viewList)
        {
            
            var listToCreate = viewList
                .Where(x => x.IsMissing == true && x.IsSelected == true)
                .Select(x => _teacherMgr.ConvertToMoodleUser(x))
                .ToList();

            if (listToCreate.Count > 0)
            {
                Log(string.Format("Tìm thấy {0} người cần tạo mới.", listToCreate.Count));

               
                ExecuteBatchCreation(listToCreate);

                
                foreach (var u in listToCreate)
                {

                    u.suspended = false;
                    MoodleCache.AddToCache(u.username, u);
                }

                Log("-> Đã cập nhật Cache cục bộ.");
            }
            else
            {
                Log("-> Không có ai cần tạo mới trong danh sách chọn.");
            }
        }

        public void SyncAllDatasbase()
        {
            int page = 1;
            int pageSize = 200;
            bool hasData = true;

            while (hasData)
            {
                Log(string.Format("--- Đang xử lý trang {0} ---", page));

                
                var listSql = _sqlDal.GetDsGiangVien("", page, pageSize);

                if (listSql.Count == 0)
                {
                    hasData = false;
                    break;
                }

                
                var listUsernames = listSql.Select(x => x.MaGiaoVien.Trim()).Distinct().ToList();
                try { _teacherMgr.LoadSpecificUsersToCache(listUsernames); }
                catch { }

                
                var usersToCreate = new List<MoodleUser>();

                foreach (var gv in listSql)
                {
                    // Check Cache
                    string key = gv.MaGiaoVien.Trim().ToLower();
                    if (!MoodleCache.Contains(key))
                    {
                        // Convert và Add vào list tạo
                        var moodleUser = _teacherMgr.ConvertToMoodleUser(gv);
                        usersToCreate.Add(moodleUser);
                    }
                }

                
                if (usersToCreate.Count > 0)
                {
                    Log(string.Format("-> Trang {0}: Phát hiện {1} người thiếu. Đang đồng bộ...", page, usersToCreate.Count));
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
                    _teacherMgr.CreateUsersBatch(batch);
                    processed += count;
                    Log(string.Format("   + Đã tạo batch {0} người (Tổng: {1}/{2})", count, processed, total));
                }
                catch (Exception ex)
                {
                    Log("   [LỖI] Batch index " + i + ": " + ex.Message);
                }

                ReportProgress(processed, total);
                Thread.Sleep(500);
            }
        }


        public void SyncUpdateFromViewList(List<TeacherSyncViewModel> viewList)
        {
            var listToUpdate = viewList
                .Where(x => x.IsSelected == true && x.IsMissing == false) 
                .ToList();

            if (listToUpdate.Count == 0) return;

            Log(string.Format("Đang cập nhật thông tin cho {0} người...", listToUpdate.Count));

            
            var moodleUsers = new List<MoodleUser>();
            foreach (var item in listToUpdate)
            {
                var user = _teacherMgr.ConvertToMoodleUser(item);

                
                var cachedUser = MoodleCache.GetUser(user.username);
                if (cachedUser != null && cachedUser.id > 0)
                {
                    user.id = cachedUser.id; // Gán ID vào để biết update ai
                    moodleUsers.Add(user);
                }
            }

            
            _teacherMgr.UpdateUsersBatch(moodleUsers);

            
            foreach (var u in moodleUsers) MoodleCache.AddToCache(u.username, u);

            Log("-> Cập nhật hoàn tất.");
        }

        // Helper
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