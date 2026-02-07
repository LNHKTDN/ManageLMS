using ManageLMS.Common.Helpers;
using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ManageLMS.DAL.MoodleAPI.Repositories
{
    public class EnrollmentRepo : BaseRepo
    {
        public void EnrollUsers(List<UserEnrolment> enrollments)
        {
            if (enrollments == null || enrollments.Count == 0) return;

            string funcName = AppConstant.MoodleFunctions.EnrollUser;
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < enrollments.Count; i++)
            {
                postData.Add(string.Format("enrolments[{0}][roleid]", i), enrollments[i].roleid.ToString());
                postData.Add(string.Format("enrolments[{0}][userid]", i), enrollments[i].userid.ToString());
                postData.Add(string.Format("enrolments[{0}][courseid]", i), enrollments[i].courseid.ToString());
            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }
        public void UnenrollUsers(List<UserEnrolment> enrollments)
        {
            if (enrollments == null || enrollments.Count == 0) return;

            
            string funcName = AppConstant.MoodleFunctions.UnenrollUser;
            NameValueCollection postData = new NameValueCollection();

            for (int i = 0; i < enrollments.Count; i++)
            {
                postData.Add(string.Format("enrolments[{0}][userid]", i), enrollments[i].userid.ToString());
                postData.Add(string.Format("enrolments[{0}][courseid]", i), enrollments[i].courseid.ToString());
                
            }

            string response = _client.Post(funcName, postData);
            CheckMoodleError(response);
        }
    }
}
