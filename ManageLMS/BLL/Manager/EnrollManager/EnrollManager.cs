using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.BLL.Manager.EnrollManager
{
    class EnrollManager : BaseManager
    {
        public void EnrollUsersToCourse(long courseId, List<long> userIds, int roleId)
        {
            var enrollments = new List<UserEnrolment>();

            foreach (var uid in userIds)
            {
                enrollments.Add(new UserEnrolment
                {
                    courseid = courseId,
                    userid = uid,
                    roleid = roleId
                });
            }

            
            _enrollRepo.EnrollUsers(enrollments);
        }
        public void UnenrollExtraStudents(long courseId, List<long> userIds)
        {
            if (userIds == null || userIds.Count == 0) return;

            var unenrollList = new List<UserEnrolment>();
            foreach (var uid in userIds)
            {
                unenrollList.Add(new UserEnrolment
                {
                    courseid = courseId,
                    userid = uid
                    // roleid không bắt buộc khi unenroll manual
                });
            }

            // Gọi xuống Repo
            _enrollRepo.UnenrollUsers(unenrollList);
        }
    }
}
