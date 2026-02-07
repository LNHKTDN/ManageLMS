using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ManageLMS.DAL.MoodleAPI
{
    class MoodleRepo
    {
        private ApiClient _client;

        public MoodleRepo()
        {
            // Lấy cấu hình từ App.config thông qua Helper
            string domain = ConfigHelper.MoodleDomain;
            string token = ConfigHelper.MoodleToken;
            _client = new ApiClient(domain, token);
        }

        // Ví dụ 1: Lấy thông tin Site (GET đơn giản)
        public string GetSiteInfo()
        {
            // Gọi hàm GET của ApiClient
            string json = _client.Get("core_webservice_get_site_info");

            // Ở đây bạn có thể Parse JSON sang Object nếu muốn, hoặc trả về check lỗi
            if (json.Contains("\"exception\""))
                throw new Exception("Moodle API Error: " + json);

            return json;
        }

        // Ví dụ 2: Tạo User theo lô (POST phức tạp)
        public void CreateUsers(List<MoodleUser> users)
        {
            if (users == null || users.Count == 0) return;

            string funcName = "core_user_create_users";

            // 1. Chuyển đổi List Object sang NameValueCollection (Format của Moodle)
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < users.Count; i++)
            {
                // Cú pháp Moodle: users[0][username], users[1][username]...
                postData.Add(string.Format("users[{0}][username]", i), users[i].username);
                postData.Add(string.Format("users[{0}][password]", i), users[i].password); // Mật khẩu (phải thỏa mãn policy)
                postData.Add(string.Format("users[{0}][firstname]", i), users[i].firstname);
                postData.Add(string.Format("users[{0}][lastname]", i), users[i].lastname);
                postData.Add(string.Format("users[{0}][email]", i), users[i].email);
                postData.Add(string.Format("users[{0}][idnumber]", i), users[i].idnumber); // Quan trọng để map với SQL

                // Mặc định Auth là 'manual' (hoặc 'ldap' nếu dùng LDAP)
                postData.Add(string.Format("users[{0}][auth]", i), "manual");
            }

            // 2. Gọi hàm POST
            string responseJson = _client.Post(funcName, postData);

            // 3. Kiểm tra lỗi
            // Nếu thành công, Moodle trả về List ID. Nếu lỗi, trả về "exception"
            if (responseJson.Contains("\"exception\""))
            {
                // Bạn nên Parse JSON lỗi để lấy message chi tiết hơn
                throw new Exception("Lỗi tạo User trên Moodle: " + responseJson);
            }
        }
    }
}
