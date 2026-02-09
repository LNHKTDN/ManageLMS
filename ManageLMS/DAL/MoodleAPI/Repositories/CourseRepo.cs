using ManageLMS.Common.DTO.ViewModel;
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
    public class CourseRepo : BaseRepo
    {
        public List<MoodleCourse> GetCoursesByUserId(long moodleUserId)
        {
            if (moodleUserId <= 0) return new List<MoodleCourse>();

            string funcName = ManageLMS.Common.Helpers.AppConstant.MoodleFunctions.GetUserCourses;
            NameValueCollection param = new NameValueCollection();
            param.Add("userid", moodleUserId.ToString());

            // Gọi API
            string jsonResponse = _client.Post(funcName, param);
            CheckMoodleError(jsonResponse);

            try
            {

                var courses = JsonConvert.DeserializeObject<List<MoodleCourse>>(jsonResponse);
                if (courses != null && courses.Count > 0)
                {
                    return courses;
                }
                return new List<MoodleCourse>();
            }
            catch
            {
                return new List<MoodleCourse>();
            }
        }
        public List<MoodleCourse> GetUserCoursesInRootCategory(long userId, string rootIdNumber)
        {
            if (userId <= 0 || string.IsNullOrEmpty(rootIdNumber)) return new List<MoodleCourse>();

            // BƯỚC 1: Lấy toàn bộ danh mục để phân tích cây (Dựa vào Path)
    
            string jsonCats = _client.Get("core_course_get_categories", new NameValueCollection());
            CheckMoodleError(jsonCats);

            List<Category> allCategories = new List<Category>();
            try
            {
                allCategories = JsonConvert.DeserializeObject<List<Category>>(jsonCats);
            }
            catch
            {
                return new List<MoodleCourse>();
            }


            // BƯỚC 2: Tìm ID của Node Cha và tất cả Node Con Cháu

            var rootCat = allCategories.FirstOrDefault(c => c.idnumber == rootIdNumber);

            // Nếu không tìm thấy category gốc -> Trả về rỗng luôn
            if (rootCat == null) return new List<MoodleCourse>();

            // Tạo danh sách chứa ID hợp lệ (HashSet để tra cứu cực nhanh)
            HashSet<long> validCategoryIds = new HashSet<long>();
            validCategoryIds.Add(rootCat.id); // Thêm chính nó vào

            // Chuỗi cần tìm trong path. Ví dụ ID cha là 10.
            // Path con sẽ là /1/10/50 -> Chứa "/10/"
            string pathFragment = "/" + rootCat.id + "/";
            string pathEnd = "/" + rootCat.id; // Trường hợp nó nằm cuối

            foreach (var cat in allCategories)
            {
                if (!string.IsNullOrEmpty(cat.path))
                {
                    
                    if (cat.path.Contains(pathFragment) || cat.path.EndsWith(pathEnd))
                    {
                        validCategoryIds.Add(cat.id);
                    }
                }
            }

            
            // BƯỚC 3: Lấy khóa học của User và Lọc
            
            
            List<MoodleCourse> allUserCourses = GetCoursesByUserId(userId);

            // Chỉ giữ lại những khóa học có category nằm trong danh sách hợp lệ
            var filteredCourses = allUserCourses
                                  .Where(c => validCategoryIds.Contains(c.category)) 
                                  .ToList();

            return filteredCourses;
        }
        // Thêm vào CourseRepo.cs
        public List<EnrolledUserViewModel> GetEnrolledUsersByCourseId(long courseId)
        {
            if (courseId <= 0) return new List<EnrolledUserViewModel>();

            string funcName = AppConstant.MoodleFunctions.GetEnrolledUser;
            NameValueCollection param = new NameValueCollection();
            param.Add("courseid", courseId.ToString());

            // Gọi API
            string jsonResponse = _client.Post(funcName, param);
            CheckMoodleError(jsonResponse);

            try
            {

                var users = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);

                var result = new List<EnrolledUserViewModel>();

                if (users != null)
                {
                    foreach (var u in users)
                    {
                        // Xử lý chuỗi Role (vì 1 người có thể vừa là Teacher vừa là Manager)
                        List<string> roleNames = new List<string>();
                        if (u.roles != null)
                        {
                            foreach (var r in u.roles)
                            {
                                roleNames.Add((string)r.shortname); // hoặc r.name nếu muốn tên hiển thị
                            }
                        }

                        // Convert timestamp sang ngày tháng
                        long lastAccessTs = (long)(u.lastaccess ?? 0);
                        string lastAccessStr = lastAccessTs > 0
                            ? new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(lastAccessTs).ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                            : "Chưa truy cập";

                        result.Add(new EnrolledUserViewModel
                        {
                            Id = (long)u.id,
                            Username = (string)u.username,
                            Fullname = (string)u.fullname,
                            Email = (string)u.email,
                            Roles = string.Join(", ", roleNames), // Nối các role lại
                            LastAccess = lastAccessStr
                        });
                    }
                }
                return result;
            }
            catch
            {
                return new List<EnrolledUserViewModel>();
            }
        }
        public void CreateCoursesBatch(List<MoodleCourse> courses)
        {
            if (courses == null || courses.Count == 0) return;

            string funcName = AppConstant.MoodleFunctions.CreateCourses;
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < courses.Count; i++)
            {
                
                postData.Add(string.Format("courses[{0}][fullname]", i), courses[i].fullname);
                postData.Add(string.Format("courses[{0}][shortname]", i), courses[i].shortname);
                postData.Add(string.Format("courses[{0}][categoryid]", i), courses[i].category.ToString());

                
                if (!string.IsNullOrEmpty(courses[i].idnumber))
                {
                    postData.Add(string.Format("courses[{0}][idnumber]", i), courses[i].idnumber);
                }
                postData.Add(string.Format("courses[{0}][enddate]", i), "0");

            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);

        }
        public List<MoodleCourse> GetCoursesBySingleField(string field, string value)
        {
            string funcName = AppConstant.MoodleFunctions.GetCoursesByField;
            NameValueCollection param = new NameValueCollection();
            param.Add("field", field); 
            param.Add("value", value);

            string jsonResponse = _client.Post(funcName, param);
            

            try
            {
                var wrapper = JsonConvert.DeserializeObject<MoodleCourseResponseWrapper>(jsonResponse);
                if (wrapper != null && wrapper.courses != null)
                {
                    return wrapper.courses;
                }
                return new List<MoodleCourse>();
            }
            catch
            {
                return new List<MoodleCourse>();
            }
        }

        // Class wrapper để hứng JSON trả về (vì API này trả về object { courses: [...] })
        public class MoodleCourseResponseWrapper
        {
            public List<MoodleCourse> courses { get; set; }
        }
    }
}
