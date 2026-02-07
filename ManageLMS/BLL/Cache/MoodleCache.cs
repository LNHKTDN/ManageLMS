using System.Collections.Concurrent;
using ManageLMS.DTO.Model;
using System.Collections.Generic;
using System.Linq;
namespace ManageLMS.BLL.Cache
{
    public static class MoodleCache
    {
        // Dùng ConcurrentDictionary để an toàn với Parallel.ForEach
        private static ConcurrentDictionary<string, MoodleUser> _cache = new ConcurrentDictionary<string, MoodleUser>();

        public static void AddToCache(string username, MoodleUser user)
        {
            if (string.IsNullOrEmpty(username) || user == null) return;
            string key = username.Trim().ToLower();
            _cache.AddOrUpdate(key, user, (k, oldVal) => user);
        }

        public static MoodleUser GetUser(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;
            string key = username.Trim().ToLower();
            MoodleUser user;
            _cache.TryGetValue(key, out user);
            return user;
        }

        public static bool Contains(string username)
        {
            if (string.IsNullOrEmpty(username)) return false;
            return _cache.ContainsKey(username.Trim().ToLower());
        }

        public static void Clear() { _cache.Clear(); }
        public static void RemoveFromCache(string username)
        {
            if (string.IsNullOrEmpty(username)) return;
            string key = username.Trim().ToLower();


            MoodleUser removedUser;
            _cache.TryRemove(key, out removedUser);
        }
        // Hàm lấy danh sách Username chưa có trong Cache để đi fetch 1 lần
        public static List<string> GetMissingKeys(List<string> keysToCheck)
        {
            return keysToCheck.Distinct()
                              .Where(k => !string.IsNullOrEmpty(k) && !_cache.ContainsKey(k.Trim().ToLower()))
                              .ToList();
        }
    }
}