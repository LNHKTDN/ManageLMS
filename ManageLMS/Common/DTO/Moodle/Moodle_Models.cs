using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.DTO.Model
{
    public class Category
    {
        public int id { get; set; }                // ID nội bộ của Moodle
        public string name { get; set; }           // Tên danh mục (VD: Khoa CNTT, Học kỳ 241)
        public string idnumber { get; set; }       // Mã định danh (VD: MASTER_ROOT, KHOA_CNTT)
        public int parent { get; set; }            // ID của danh mục cha (0 là root)
        public int coursecount { get; set; }       // Số lượng khóa học trong danh mục này
        public int depth { get; set; }             // Độ sâu của cây thư mục
        public string path { get; set; }           // Đường dẫn (VD: /1/5)
    }
    public class MoodleUser
    {
        public long id { get; set; }                // ID người dùng trên Moodle
        public string username { get; set; }       // Tên đăng nhập (thường là MSSV/MAGV viết thường)
        public string firstname { get; set; }      // Tên
        public string password { get; set; }
        public string lastname { get; set; }       // Họ và chữ lót
        public string email { get; set; }          // Email liên hệ
        public string idnumber { get; set; }       // Mã số (MSSV hoặc Mã giảng viên) - QUAN TRỌNG ĐỂ MAP
        public string department { get; set; }     // Khoa (TenKhoa)
        public string institution { get; set; }    // Ngành (TenNganh)
        public string phone1 { get; set; }         // Số điện thoại
        public string city { get; set; }           // Thành phố (Mặc định: Đà Nẵng)
        public string country { get; set; }        // Quốc gia (Mặc định: VN)
        public bool suspended { get; set; }         // Trạng thái (0: Active, 1: Suspended)
        public string auth { get; set; }
        public MoodleUser Clone()
        {
            return new MoodleUser
            {
                id = this.id,
                username = this.username,
                password = this.password,
                firstname = this.firstname,
                lastname = this.lastname,
                email = this.email,
                idnumber = this.idnumber,
                department = this.department,
                city = this.city,
                suspended = this.suspended
            };
        }
    }

    public class MoodleCourse
    {
        public long id { get; set; }                // ID khóa học trên Moodle
        public string fullname { get; set; }       // Tên đầy đủ (VD: Lập trình C# (241_CS101_01))
        public string shortname { get; set; }      // Tên ngắn (Unique - VD: 241_CS101_01)
        public string idnumber { get; set; }       // Mã định danh (Dùng để đồng bộ - VD: Mã học phần hoặc Mã lớp TC)
        public int category { get; set; }        // ID của danh mục chứa khóa học này
        public string summary { get; set; }        // Mô tả ngắn gọn của khóa học
        public long startdate { get; set; }        // Ngày bắt đầu (Unix timestamp)
        public int visible { get; set; }           // Trạng thái hiển thị (1: Hiện, 0: Ẩn)
        public string format { get; set; }

    }
    public class MoodleCohort
    {
        public long id { get; set; }                // ID Cohort
        public string name { get; set; }           // Tên lớp (VD: 20K45.1)
        public string idnumber { get; set; }       // Mã định danh lớp
        public string description { get; set; }    // Mô tả
        public int contextid { get; set; }         // ID ngữ cảnh (thường là context của Category hoặc System)
    }
    public class UserEnrolment
    {
        public long userid { get; set; }            // ID người dùng
        public long courseid { get; set; }          // ID khóa học
        public long roleid { get; set; }            // ID vai trò (3: Teacher, 5: Student)
        public long timestart { get; set; }        // Thời điểm bắt đầu (Unix timestamp)
    }

}
