using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace ManageLMS.DAL.MoodleAPI
{
    public class ApiClient
    {
        private string _domain;
        private string _token;

        public ApiClient(string domain, string token)
        {
            _domain = domain;
            _token = token;
        }

        // Hàm GET: Dùng để lấy dữ liệu (VD: Lấy danh sách)
        public string Post(string functionName, NameValueCollection parameters)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string url = _domain + "/webservice/rest/server.php";

                // 1. Chuẩn bị dữ liệu Form-Data
                if (parameters == null) parameters = new NameValueCollection();

                // Các tham số bắt buộc của Moodle
                parameters.Add("wstoken", _token);
                parameters.Add("wsfunction", functionName);
                parameters.Add("moodlewsrestformat", "json");

                try
                {
                    // 2. Gửi POST request (UploadValues tự động xử lý header Content-Type)
                    byte[] responseBytes = client.UploadValues(url, "POST", parameters);

                    // 3. Đọc kết quả
                    return Encoding.UTF8.GetString(responseBytes);
                }
                catch (Exception ex)
                {
                    throw new Exception("ApiClient.Post Error: " + ex.Message);
                }
            }
        }
        public string Get(string functionName, NameValueCollection parameters = null)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                // 1. Xây dựng Query String
                StringBuilder queryString = new StringBuilder();
                queryString.Append(string.Format("?wstoken={0}&wsfunction={1}&moodlewsrestformat=json",
                                                 _token, functionName));

                // 2. Thêm các tham số phụ (nếu có)
                if (parameters != null)
                {
                    foreach (string key in parameters.AllKeys)
                    {
                        // UrlEncode để xử lý các ký tự đặc biệt như dấu cách, /, @...
                        string value = Uri.EscapeDataString(parameters[key]);
                        queryString.Append(string.Format("&{0}={1}", key, value));
                    }
                }

                // 3. Ghép thành URL hoàn chỉnh
                string url = _domain + "/webservice/rest/server.php" + queryString.ToString();

                try
                {
                    // Gửi GET request
                    return client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    // Ném lỗi ra để tầng trên (Repo/BLL) xử lý
                    throw new Exception("ApiClient.Get Error: " + ex.Message);
                }
            }
        }
    }
}
