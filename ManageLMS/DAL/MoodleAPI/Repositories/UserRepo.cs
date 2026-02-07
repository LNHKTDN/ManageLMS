using ManageLMS.Common.Helpers;
using ManageLMS.DTO.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ManageLMS.DAL.MoodleAPI.Repositories
{
    public class UserRepo : BaseRepo
    {
        public string GetAllUsersList()
        {

            NameValueCollection param = new NameValueCollection();
            param.Add("criteria[0][key]", "email");
            param.Add("criteria[0][value]", "%");

            string response = _client.Get(AppConstant.MoodleFunctions.GetUsers, param);
            CheckMoodleError(response);
            return response;
        }
        public void CreateUsers(List<MoodleUser> users)
        {
            if (users == null || users.Count == 0) return;

            string funcName = AppConstant.MoodleFunctions.CreateUser;
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < users.Count; i++)
            {
                postData.Add(string.Format("users[{0}][username]", i), users[i].username);
                postData.Add(string.Format("users[{0}][password]", i), users[i].password);
                postData.Add(string.Format("users[{0}][firstname]", i), users[i].firstname);
                postData.Add(string.Format("users[{0}][lastname]", i), users[i].lastname);
                postData.Add(string.Format("users[{0}][email]", i), users[i].email);
                postData.Add(string.Format("users[{0}][idnumber]", i), users[i].idnumber);
                postData.Add(string.Format("users[{0}][auth]", i), "manual");
            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }

        public MoodleUser GetUserByUsername(string username)
        {
            NameValueCollection param = new NameValueCollection();
            param.Add("field", "username");
            param.Add("values[0]", username.Trim().ToLower());

            string jsonResponse = _client.Get(AppConstant.MoodleFunctions.GetUsersByField, param);
            CheckMoodleError(jsonResponse);

            try
            {
                var users = JsonConvert.DeserializeObject<List<MoodleUser>>(jsonResponse);
                return (users != null && users.Count > 0) ? users[0] : null;
            }
            catch { }
            return null;
        }
        public List<MoodleUser> GetUsersByListUsername(List<string> listUsernames)
        {
            if (listUsernames == null || listUsernames.Count == 0) return new List<MoodleUser>();

            string funcName = AppConstant.MoodleFunctions.GetUsersByField;
            NameValueCollection param = new NameValueCollection();

            
            param.Add("field", "username");

            
            for (int i = 0; i < listUsernames.Count; i++)
            {
                // Username trên Moodle luôn là chữ thường, nên Trim().ToLower() cho chắc
                param.Add(string.Format("values[{0}]", i), listUsernames[i].Trim().ToLower());
            }

            
            string jsonResponse = _client.Post(funcName, param);
            CheckMoodleError(jsonResponse);

            try
            {
                return JsonConvert.DeserializeObject<List<MoodleUser>>(jsonResponse);
            }
            catch
            {
                return new List<MoodleUser>();
            }
        }

        public MoodleUser GetUserByIdNumber(string idNumber)
        {
            NameValueCollection param = new NameValueCollection();
            param.Add("field", "idnumber");
            param.Add("values[0]", idNumber);

            string jsonResponse = _client.Get(AppConstant.MoodleFunctions.GetUsersByField, param);
            CheckMoodleError(jsonResponse);

            try
            {
                // Moodle luôn trả về mảng [], kể cả khi tìm 1 người
                var users = JsonConvert.DeserializeObject<List<MoodleUser>>(jsonResponse);
                if (users != null && users.Count > 0)
                {
                    return users[0];
                }
            }
            catch { }

            return null; // Không tìm thấy
        }
        public void UpdateUsers(List<MoodleUser> users)
        {
            if (users == null || users.Count == 0) return;

            string funcName = AppConstant.MoodleFunctions.UpdateUsers; // Nên đưa vào AppConstant
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < users.Count; i++)
            {
                // ID là bắt buộc để biết sửa ai (Moodle User ID, không phải IDNumber)
                postData.Add(string.Format("users[{0}][id]", i), users[i].id.ToString());

                // Các trường muốn sửa (chỉ truyền những gì cần sửa)
                if (!string.IsNullOrEmpty(users[i].firstname))
                    postData.Add(string.Format("users[{0}][firstname]", i), users[i].firstname);

                if (!string.IsNullOrEmpty(users[i].lastname))
                    postData.Add(string.Format("users[{0}][lastname]", i), users[i].lastname);

                if (!string.IsNullOrEmpty(users[i].email))
                    postData.Add(string.Format("users[{0}][email]", i), users[i].email);

                if (!string.IsNullOrEmpty(users[i].department))
                    postData.Add(string.Format("users[{0}][department]", i), users[i].department);

                if (!string.IsNullOrEmpty(users[i].city))
                    postData.Add(string.Format("users[{0}][city]", i), users[i].city);
                // Quan trọng: Update IDNumber nếu cần đồng bộ lại
                if (!string.IsNullOrEmpty(users[i].idnumber))
                    postData.Add(string.Format("users[{0}][idnumber]", i), users[i].idnumber);
            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }


        public void DeleteUsers(List<long> moodleUserIds)
        {
            if (moodleUserIds == null || moodleUserIds.Count == 0) return;

            string funcName = AppConstant.MoodleFunctions.DeleteUser;
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < moodleUserIds.Count; i++)
            {
                postData.Add(string.Format("userids[{0}]", i), moodleUserIds[i].ToString());
            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }


        public void ChangeUserStatus(long moodleUserId, bool isSuspended)
        {
            string funcName = AppConstant.MoodleFunctions.UpdateUsers;
            NameValueCollection postData = new NameValueCollection();

            postData.Add("users[0][id]", moodleUserId.ToString());
            postData.Add("users[0][suspended]", isSuspended ? "1" : "0");

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }


        public void EnrolUserToCourse(long userId, long courseId, int roleId = 5)
        {
            string funcName = AppConstant.MoodleFunctions.EnrollUser;
            NameValueCollection postData = new NameValueCollection();

            postData.Add("enrolments[0][roleid]", roleId.ToString());
            postData.Add("enrolments[0][userid]", userId.ToString());
            postData.Add("enrolments[0][courseid]", courseId.ToString());

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }


        public void UnenrolUserFromCourse(long userId, long courseId)
        {
            // Có thể cần roleid tùy phiên bản, nhưng thường chỉ cần user và course
            string funcName = AppConstant.MoodleFunctions.UnenrollUser;
            NameValueCollection postData = new NameValueCollection();

            postData.Add("enrolments[0][userid]", userId.ToString());
            postData.Add("enrolments[0][courseid]", courseId.ToString());

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }
    }
}
