using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.Common.Helpers
{
    public static class AppConstant
    {
        public static class Defaults
        {
            public const string PasswordPrefix = "DUE@Moodle2026";
            public const string EmailDomain = "@due.edu.vn";
        }
        public static class MoodleRoles
        {
            public const int Teacher = 3;         // Editing Teacher
            public const int NonEditingTeacher = 4;
            public const int Student = 5;         // Student
        }
        public static class MoodleField
        {

            public const string Course_Id = "id";
            public const string Course_IdNumber = "idnumber";
            public const string Course_Shortname = "shortname";
            public const string Course_Category = "category";

            public const string User_Id = "id";
            public const string User_IdNumber = "idnumber";
            public const string User_Username = "username";
            public const string User_Email = "email";

            public const string Category_Id = "id";
            public const string Category_IdNumber = "idnumber";
            public const string Category_Parent = "parent";
        }
        public static class MoodleFunctions
        {
            // Site
            public const string GetSiteInfo = "core_webservice_get_site_info";

            // Course & Category
            public const string GetCourses = "core_course_get_courses";
            public const string GetCategories = "core_course_get_categories";
            public const string GetCoursesByField = "core_course_get_courses_by_field";
            public const string CreateCourses = "core_course_create_courses";
            public const string CreateCategories = "core_course_create_categories";
            public const string GetUserCourses = "core_enrol_get_users_courses";
            public const string GetEnrolledUser = "core_enrol_get_enrolled_users";
            // User
            public const string CreateUser = "core_user_create_users";
            public const string UpdateUsers = "core_user_update_users";
            public const string GetUsers = "core_user_get_users";
            public const string GetUsersByField = "core_user_get_users_by_field"; // core_user_get_users_by_field
            public const string DeleteUser = "core_user_delete_users";

            // Enrollment
            public const string EnrollUser = "enrol_manual_enrol_users";
            public const string AssignRoles = "core_role_assign_roles";
            public const string UnenrollUser = "enrol_manual_unenrol_users";
            // Cohort
            public const string CreateCohorts = "core_cohort_create_cohorts";
            public const string GetCohorts = "core_cohort_get_cohorts";
            public const string AddCohortMembers = "core_cohort_add_cohort_members";
        }
        public static class MoodleString
        {
            public const string masterIdNumber = "MASTER_ROOT";
            public const string masterCategoryName = "Master Courses";
        }
    }
}
