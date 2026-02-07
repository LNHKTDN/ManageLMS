using ManageLMS.DTO.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManageLMS.BLL.Manager.UserManager
{
    public class UserManager : BaseManager
    {
        public long CreateUser(MoodleUser user)
        {
            if (string.IsNullOrEmpty(user.username)) return 0;

            var list = new List<MoodleUser> { user };
            _userRepo.CreateUsers(list);

            if (user.id > 0)
            {
                ManageLMS.BLL.Cache.MoodleCache.AddToCache(user.username, user);
                return user.id;
            }

            return GetMoodleIdByUsername(user.username);
        }
        public long GetMoodleIdByUsername(string username)
        {
            
            MoodleUser user = _userRepo.GetUserByUsername(username.Trim().ToLower());
            return user != null ? user.id : 0;
        }
        public MoodleUser GetMoodleUserByUsername(string username)
        {

            MoodleUser user = _userRepo.GetUserByUsername(username.Trim().ToLower());
            return user ;
        }
        public List<MoodleUser> GetUsersByListUsername(List<string> listUsernames)
        {
            
            return _userRepo.GetUsersByListUsername(listUsernames);
        }
        public void CreateUsersBatch(List<MoodleUser> users)
        {
            var validUsers = users.Where(u => !string.IsNullOrEmpty(u.username)).ToList();
            if (validUsers.Count > 0)
            {
                _userRepo.CreateUsers(validUsers);
            }
        }

        public void UpdateUsersBatch(List<MoodleUser> users)
        {
            var validUsers = users.Where(u => u.id > 0).ToList();
            if (validUsers.Count > 0)
            {
                _userRepo.UpdateUsers(validUsers);
                foreach (var user in validUsers)
                {

                    if (!string.IsNullOrEmpty(user.username))
                    {
                        ManageLMS.BLL.Cache.MoodleCache.AddToCache(user.username, user);
                    }
                }
            }
          
        }

        
        public bool UpdateUserPassword(string username, string newPassword)
        {
            long moodleId = GetMoodleIdByUsername(username);
            if (moodleId <= 0) return false;

            var userToUpdate = new MoodleUser { id = moodleId, password = newPassword };
            _userRepo.UpdateUsers(new List<MoodleUser> { userToUpdate });
            return true;
        }

        public bool SuspendUser(string username)
        {
            long moodleId = GetMoodleIdByUsername(username);
            if (moodleId <= 0) return false;

            _userRepo.ChangeUserStatus(moodleId, true); // true = suspended

            var cachedUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);
            if (cachedUser != null) cachedUser.suspended = true;

            return true;
        }

        public bool UnlockUser(string username)
        {
            long moodleId = GetMoodleIdByUsername(username);
            if (moodleId <= 0) return false;

            _userRepo.ChangeUserStatus(moodleId, false); // false = active

            // Cập nhật Cache
            var cachedUser = ManageLMS.BLL.Cache.MoodleCache.GetUser(username);
            if (cachedUser != null) cachedUser.suspended = false;

            return true;
        }

        public bool DeleteUser(string username)
        {
            long moodleId = GetMoodleIdByUsername(username);
            if (moodleId <= 0) return false;

            _userRepo.DeleteUsers(new List<long> { moodleId });

            // Xóa khỏi Cache
            ManageLMS.BLL.Cache.MoodleCache.RemoveFromCache(username);

            return true;
        }
    }
}