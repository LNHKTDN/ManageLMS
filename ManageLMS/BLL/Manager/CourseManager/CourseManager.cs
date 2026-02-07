using ManageLMS.Common.DTO.ViewModel;
using ManageLMS.Common.Helpers;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageLMS.BLL.Manager.CourseManager
{
    class CourseManager : BaseManager
    {
        public List<UserCourseViewModel> GetUserCoursesBySemester(long moodleUserId, string semesterIdNumber)
        {
            // 1. Tìm ID của Học kỳ (VD: Nhập "241" -> Tìm ra ID 50)
            Category targetCategory = _cateRepo.GetCategoryByIdNumber(semesterIdNumber);
            if (targetCategory == null || string.IsNullOrEmpty(targetCategory.idnumber) ) return new List<UserCourseViewModel>();

            // 2. Lấy tất cả khóa học của User
            var allCourses = _courseRepo.GetCoursesByUserId(moodleUserId);

            // 3. Lọc những khóa thuộc Category đó
            var result = allCourses
                .Where(c => c.category == targetCategory.id)
                .Select(c => new UserCourseViewModel
                {
                    CourseId = c.id,
                    Fullname = c.fullname,
                    Shortname = c.shortname,
                    IdNumber = c.idnumber,
                    CategoryName = targetCategory.name, 
                   
                })
                .ToList();

            return result;
        }

        public List<MoodleCourse> GetMasterCourses(long moodleUserId)
        {
            var listCourse  = _courseRepo.GetUserCoursesInRootCategory(moodleUserId,AppConstant.MoodleString.masterIdNumber);
            return listCourse;
        }
        public List<EnrolledUserViewModel> GetEnrolledUser(long courseId)
        {
            var listUser = _courseRepo.GetEnrolledUsersByCourseId(courseId);
            return listUser;
        }
        public void CreateCoursesBatch(List<MoodleCourse> listCourse)
        {
             _courseRepo.CreateCoursesBatch(listCourse);
        }
        public List<MoodleCourse> GetCoursesByField(string field, List<string> values)
        {
            // Dùng ConcurrentBag thay vì List để an toàn khi nhiều luồng cùng Add vào
            var resultBag = new ConcurrentBag<MoodleCourse>();

            // Lọc trùng để đỡ tốn request thừa
            var distinctValues = values.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            // Chạy song song (Giới hạn khoảng 10-15 luồng để không bị Moodle chặn IP do spam request)
            Parallel.ForEach(distinctValues, new ParallelOptions { MaxDegreeOfParallelism = 15 }, val =>
            {
                try
                {
                    
                    var courses = _courseRepo.GetCoursesBySingleField(field, val);

                    if (courses != null && courses.Count > 0)
                    {
                        foreach (var c in courses)
                        {
                            resultBag.Add(c);
                        }
                    }
                }
                catch
                {
                    
                }
            });

            
            return resultBag.ToList();
        }
        public List<MoodleCourse> GetCoursesByField(string field, string value)
        {
            
            if (string.IsNullOrEmpty(value))
                return new List<MoodleCourse>();

            try
            {
                
                var result = _courseRepo.GetCoursesBySingleField(field, value);
                return result ?? new List<MoodleCourse>();
            }
            catch (Exception)
            {
                
                return new List<MoodleCourse>();
            }
        }
    }
}
