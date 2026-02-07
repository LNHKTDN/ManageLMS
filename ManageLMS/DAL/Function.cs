using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Data;
using ManageLMS.Common.Helpers;
namespace ManageLMS.DAL
{
    class Function
    {
        public void CheckConnections()
        {
            StringBuilder report = new StringBuilder();
            bool isSqlOk = false;
            bool isMoodleOk = false;

            // --- 1. KIỂM TRA SQL SERVER ---
            report.AppendLine("--- 1. SQL SERVER ---");
            try
            {
                string connString = ConfigHelper.SqlConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open(); // Thử mở kết nối
                    // Nếu dòng này chạy qua được nghĩa là OK
                    isSqlOk = true;
                    report.AppendLine("✅ Kết nối Database thành công!");
                    report.AppendLine("Server: " + conn.DataSource);
                    report.AppendLine("Database: " + conn.Database);
                }
            }
            catch (Exception ex)
            {
                isSqlOk = false;
                report.AppendLine("❌ Lỗi kết nối Database:");
                report.AppendLine(ex.Message);
            }

            report.AppendLine(); // Xuống dòng

            // --- 2. KIỂM TRA MOODLE API ---
            report.AppendLine("--- 2. MOODLE API ---");
            try
            {
                string domain = ConfigHelper.MoodleDomain;
                string token = ConfigHelper.MoodleToken;
                string func = AppConstant.MoodleFunctions.GetSiteInfo;

                // Tạo URL test (Hàm get_site_info)
                string apiUrl = string.Format("{0}/webservice/rest/server.php?wstoken={1}&wsfunction={2}&moodlewsrestformat=json",
                                              domain, token, func);

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string jsonResponse = client.DownloadString(apiUrl);

                    // Kiểm tra xem Moodle có trả về lỗi (exception) không
                    if (jsonResponse.Contains("\"exception\""))
                    {
                        isMoodleOk = false;
                        report.AppendLine("❌ Kết nối được Server nhưng Token hoặc Hàm bị sai!");
                        report.AppendLine("Phản hồi: " + jsonResponse);
                    }
                    else
                    {
                        isMoodleOk = true;
                        report.AppendLine("✅ Gọi API Moodle thành công!");
                        // Nếu muốn xịn hơn, bạn có thể Deserialize JSON để lấy Site Name
                        // report.AppendLine("Data received (Raw): " + jsonResponse.Substring(0, 50) + "...");
                    }
                }
            }
            catch (Exception ex)
            {
                isMoodleOk = false;
                report.AppendLine("❌ Lỗi mạng hoặc URL Moodle không đúng:");
                report.AppendLine(ex.Message);
            }

            // --- 3. HIỂN THỊ KẾT QUẢ ---
            string finalTitle = (isSqlOk && isMoodleOk) ? "HỆ THỐNG SẴN SÀNG" : "CÓ LỖI XẢY RA";
            MessageBoxIcon icon = (isSqlOk && isMoodleOk) ? MessageBoxIcon.Information : MessageBoxIcon.Warning;

            MessageBox.Show(report.ToString(), finalTitle, MessageBoxButtons.OK, icon);
        }

        public DataTable GetDanhSachDanhMuc()
        {
            // 1. Lấy thông tin cấu hình (Dùng lại ConfigHelper bài trước)
            string domain = ConfigHelper.MoodleDomain;
            string token = ConfigHelper.MoodleToken;
            string funcName = AppConstant.MoodleFunctions.GetCategories; 

            // 2. Tạo URL
            string apiUrl = string.Format("{0}/webservice/rest/server.php?wstoken={1}&wsfunction={2}&moodlewsrestformat=json",
                                          domain, token, funcName);

            // 3. Gọi API
            string jsonResponse = "";
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                try
                {
                    jsonResponse = client.DownloadString(apiUrl);
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi kết nối mạng tới Moodle: " + ex.Message);
                }
            }

            // 4. Kiểm tra lỗi nghiệp vụ từ Moodle
            if (jsonResponse.Contains("\"exception\""))
            {
                throw new Exception("Moodle trả về lỗi: " + jsonResponse);
            }

            // 5. Parse JSON sang List Object
            var listCategories = JsonConvert.DeserializeObject<List<Category>>(jsonResponse);

            // 6. Chuyển đổi sang DataTable (Logic xử lý nằm gọn ở đây)
            return ConvertListToDataTable(listCategories);
        }
        private DataTable ConvertListToDataTable(List<Category> list)
        {
            DataTable dt = new DataTable();

            // Định nghĩa cột tiếng Việt cho đẹp
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Tên Danh Mục", typeof(string));
            dt.Columns.Add("Số Lượng Khóa", typeof(int));
            dt.Columns.Add("Cấp Độ", typeof(int));

            foreach (var item in list)
            {
                // Xử lý hiển thị phân cấp (thụt đầu dòng) ngay tại đây
                string displayName = item.name;
                if (item.depth > 1)
                {
                    displayName = new string('-', item.depth) + " " + item.name;
                }

                dt.Rows.Add(item.id, displayName, item.coursecount, item.depth);
            }

            return dt;
        }
    }
}
