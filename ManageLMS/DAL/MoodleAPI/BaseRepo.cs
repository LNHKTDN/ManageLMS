using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.DAL.MoodleAPI
{
    public abstract class BaseRepo
    {
        protected ApiClient _client;

            public BaseRepo()
            {
                // Tự động load cấu hình, các lớp con không cần lo việc này
                string domain = ConfigHelper.MoodleDomain;
                string token = ConfigHelper.MoodleToken;
                _client = new ApiClient(domain, token);
            }

            // Hàm tiện ích để check lỗi chung cho tất cả Repo
            protected void CheckMoodleError(string jsonResponse)
            {
                if (jsonResponse.Contains("\"exception\""))
                {
                    throw new Exception("Moodle API Error: " + jsonResponse);
                }
            }
    }
}
