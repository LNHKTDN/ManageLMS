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
    public class CategoryRepo : BaseRepo
    {
        public string GetAllCategories()
        {
            string response = _client.Get(AppConstant.MoodleFunctions.GetCategories);
            CheckMoodleError(response);
            return response;
        }
        public long CreateCategory(string name, string idNumber, string description)
        {
            string funcName = AppConstant.MoodleFunctions.CreateCategories;
            NameValueCollection postData = new NameValueCollection();
            postData.Add("categories[0][name]", name);
            postData.Add("categories[0][idnumber]", idNumber);
            postData.Add("categories[0][description]", description);
            postData.Add("categories[0][parent]", "0"); 

            string response = _client.Post(funcName, postData);

            // Parse response để lấy ID
            try
            {
                var cats = JsonConvert.DeserializeObject<List<Category>>(response);
                if (cats != null && cats.Count > 0) return cats[0].id;
            }
            catch { }
            return 0;
        }
        public Category GetCategoryByIdNumber(string categoryIdNumber)
        {
            if (string.IsNullOrEmpty(categoryIdNumber)) return null;

            string funcName = AppConstant.MoodleFunctions.GetCategories;
            NameValueCollection param = new NameValueCollection();

            // Tìm chính xác theo idnumber
            param.Add("criteria[0][key]", "idnumber");
            param.Add("criteria[0][value]", categoryIdNumber.Trim());

            string jsonResponse = _client.Get(funcName, param);
            CheckMoodleError(jsonResponse);

            try
            {
                var categories = JsonConvert.DeserializeObject<List<Category>>(jsonResponse);
                if (categories != null && categories.Count > 0)
                {
                    return categories[0];    
                }
            }
            catch { }

            return null; // Không tìm thấy
        }
        public long CreateCategory(string name, string idNumber, string description, int parentId = 0)
        {
            string funcName = AppConstant.MoodleFunctions.CreateCategories;
            NameValueCollection postData = new NameValueCollection();

            postData.Add("categories[0][name]", name);
            postData.Add("categories[0][idnumber]", idNumber);
            postData.Add("categories[0][description]", description);

            // Truyền parentId vào đây (Moodle API nhận int hoặc string đều được)
            postData.Add("categories[0][parent]", parentId.ToString());

            string response = _client.Post(funcName, postData);

            // Parse response để lấy ID
            try
            {
                var cats = JsonConvert.DeserializeObject<List<Category>>(response);
                if (cats != null && cats.Count > 0) return cats[0].id;
            }
            catch { }
            return 0;
        }
    }
}
