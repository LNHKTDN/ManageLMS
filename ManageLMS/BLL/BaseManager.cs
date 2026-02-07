using ManageLMS.DAL.MoodleAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.BLL
{
    public abstract class BaseManager
    {
        protected UserRepo _userRepo;
        protected CourseRepo _courseRepo;
        protected CategoryRepo _cateRepo;
        protected EnrollmentRepo _enrollRepo;

        public BaseManager()
        {
            // Khởi tạo các Repo từ DAL
            _userRepo = new UserRepo();
            _courseRepo = new CourseRepo();
            _cateRepo = new CategoryRepo();
            _enrollRepo = new EnrollmentRepo();
        }
    }
}
