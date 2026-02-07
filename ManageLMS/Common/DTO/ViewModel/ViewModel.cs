using ManageLMS.Common.DTO.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.Common.DTO.ViewModel
{
    public class TeacherSyncViewModel : TTGiangVien
    {
        public string TrangThaiMoodle { get; set; } // "Đã có", "Chưa có"
        public string HanhDongGoiY { get; set; }    // "Tạo mới", "Cập nhật", "Bỏ qua"

        // Màu sắc để tô trên Grid (Optional)
        public bool IsMissing { get; set; }
        public bool IsSelected { get; set; }
        public bool IsSuspended { get; set; }
        // Hàm copy dữ liệu từ cha sang con
        public TeacherSyncViewModel(TTGiangVien parent)
        {
            this.MaGiaoVien         = parent.MaGiaoVien;
            this.HoLot              = parent.HoLot;
            this.Ten                = parent.Ten;
            this.E_Mail             = parent.E_Mail;
            this.TenKhoa            = parent.TenKhoa;
            this.CoQuanCongTac      = parent.CoQuanCongTac;
            this.DienThoaiDD = parent.DienThoaiDD;
            this.NamSinh = parent.NamSinh;
            this.TenKhoa = parent.TenKhoa;
            this.ThangSinh = parent.ThangSinh;
            this.TotalRecords = parent.TotalRecords;
            this.IsSuspended = false;
        }
        public string HoTen
        {
            get { return this.HoLot + " " + this.Ten; }
        }
    }

    public class StudentSyncViewModel : TTSinhVien
    {
        public string TrangThaiMoodle { get; set; } // "Đã có", "Chưa có"

        // Màu sắc để tô trên Grid (Optional)
        public bool IsMissing { get; set; }
        public bool IsSelected { get; set; }
        public bool IsSuspended { get; set; }
        // Hàm copy dữ liệu từ cha sang con
        public StudentSyncViewModel(TTSinhVien parent)
        {
            this.MaSinhVien = parent.MaSinhVien;
            this.HoLot = parent.HoLot;
            this.Ten = parent.Ten;
            this.GioiTinh = parent.GioiTinh;
            this.NgaySinh = parent.NgaySinh;
            this.LopSV = parent.LopSV;
            this.MaKhoaHoc = this.MaKhoaHoc;
            this.TotalRecords = parent.TotalRecords;
            this.IsSuspended = false;
        }
        public string HoTen
        {
            get { return this.HoLot + " " + this.Ten; }
        }
    }

    public class UserCourseViewModel
    {
        public long CourseId { get; set; }
        public string Fullname { get; set; }      // Tên khóa học
        public string Shortname { get; set; }     // Mã học phần
        public string CategoryName { get; set; }  // Tên kỳ học (VD: Học kỳ 1 2024)
        public string RoleName { get; set; }      // Student hoặc Teacher
        public string IdNumber { get; set; }      // Mã khóa học (để đối chiếu)
    }
    public class EnrolledUserViewModel
    {
        public long Id { get; set; }           // ID Moodle của User
        public string Username { get; set; }   // MSSV hoặc Mã GV
        public string Fullname { get; set; }   // Họ tên đầy đủ
        public string Email { get; set; }      // Email
        public string Roles { get; set; }      // Vai trò (Teacher, Student...)
        public string LastAccess { get; set; } // Lần truy cập cuối
    }

    public class SemesterSyncViewModel
    {
        public string MaLopTinChi { get; set; }
        public string MonHoc { get; set; }
        public string MaKyHoc { get; set; }
        public string MaGiaoVien { get; set; }
        
        public long MoodleCourseId { get; set; } // 0 nếu chưa có trên Moodle
        public string MoodleShortname { get; set; }

        public int SiSoSQL { get; set; }        // Số SV trong SQL
        public int SiSoMoodle { get; set; }     // Số SV hiện có trên Moodle

        public int SoLuongThieu { get; set; }   // Có SQL, thiếu trên Moodle (Cần Enroll)
        public int SoLuongThua { get; set; }    // Có Moodle, không có SQL (Cần Unenroll)

        public string TrangThai { get; set; }   // "Chưa tạo", "Đồng bộ", "Lệch số liệu"
        public bool IsSelected { get; set; }

        // Danh sách chi tiết để xử lý sau này (nếu cần)
        public List<string> ListMSSV_Thieu { get; set; }
        public List<long> ListUserID_Thua { get; set; }
        public List<TTDsSVTheoLopHP> ListData_SV_Thieu { get; set; }
        public SemesterSyncViewModel()
        {
            ListMSSV_Thieu = new List<string>();
            ListUserID_Thua = new List<long>();
        }
    }
    public class MasterSyncViewModel
    {
        public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }
        public string MaKyHoc { get; set; }

        
        public string MaHe { get; set; }
        public string TenHe { get; set; }
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }

        public long MoodleCourseId { get; set; }
        public string MoodleShortname { get; set; }

        public int SiSoSQL { get; set; }
        public int SiSoMoodle { get; set; }
        public int SoLuongThieu { get; set; }
        public int SoLuongThua { get; set; } // Luôn = 0 với Master (Accumulative)

        public string TrangThai { get; set; }

        public List<string> ListMaGiaoVien { get; set; }
        public List<string> ListMSSV_Thieu { get; set; }
        public List<long> ListUserID_Thua { get; set; }
        public List<TTDsSVTheoLopHP> ListData_SV_Thieu { get; set; }
        public MasterSyncViewModel()
        {
            ListMSSV_Thieu = new List<string>();
            ListUserID_Thua = new List<long>();
            ListMaGiaoVien = new List<string>();
        }

        public string GetExpectedIdNumber()
        {
            // Xử lý null safe cho MaKhoa
            string safeMaKhoa = string.IsNullOrEmpty(MaKhoa) ? "UNK" : MaKhoa;
            return string.Format("MASTER_{0}_{1}_{2}", MaHe, safeMaKhoa, MaHocPhan);
        }

        // [SỬA] Format Tên: [TenHe] [MaHocPhan] TenHocPhan
        public string GetExpectedFullName()
        {
            return string.Format("[{0}] [{1}] {2}", TenHe, MaHocPhan, TenHocPhan);
        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public ComboBoxItem(string text, int value) { Text = text; Value = value; }
        public override string ToString() { return Text; }
    }
    public class LoadArgs
    {
        public bool IsReload { get; set; }
        public int KyHoc { get; set; }
        public string MaKyHoc { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public ManageLMS.BLL.SyncEngine.SemesterSyncEngine.SyncStatusFilter FilterMode { get; set; }
    }
}
