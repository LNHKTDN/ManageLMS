using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.Common.DTO.Database
{
    public class TTSinhVien
    {

        public string LopSV { get; set; }

        public string MaSinhVien { get; set; }

        public string HoLot { get; set; }

        public string Ten { get; set; }

        public string NgaySinh { get; set; }

        public string GioiTinh { get; set; }

        public string MaKhoaHoc { get; set; }

        public int TotalRecords { get; set; }

    }

    public class TTGiangVien
    {

        public string MaGiaoVien { get; set; }

        public string HoLot { get; set; }

        public string Ten { get; set; }

        public string NgaySinh { get; set; }

        public string ThangSinh { get; set; }

        public string NamSinh { get; set; }

        public string E_Mail { get; set; }

        public string CoQuanCongTac { get; set; }

        public string DienThoaiDD { get; set; }

        public string TenKhoa { get; set; }

        public int TotalRecords { get; set; }

    }

    public class TTHocPhan
    {

        public string MaHocPhan { get; set; }

        public string TenHocPhan { get; set; }

        public string MaKhoa { get; set; }

        public string TenKhoa { get; set; }

        public string TenKhoaRutGon { get; set; }

        public string MaHe { get; set; }

        public string TenHe { get; set; }

    }

    public class TTDsLopTheoKiTheoTKB
    {

        public string KyHoc { get; set; }

        public string MaGiaoVien { get; set; }

        public string GVHoLot { get; set; }

        public string GVTen { get; set; }

        public string MaHocPhan { get; set; }

        public string MonHoc { get; set; }

        public string SoTiet { get; set; }

        public string MaLopTinChi { get; set; }

        public string MaSinhVien { get; set; }

        public string SVTen { get; set; }

        public string NgaySinh { get; set; }

        public string GioiTinh { get; set; }

        public string LopSV { get; set; }

    }

    public class TTDsSVTheoLopHP
    {

        public string MaHocPhan { get; set; }

        public string TenHocPhan { get; set; }

        public string MaHe { get; set; }

        public string TenHe { get; set; }

        public string MaKhoa { get; set; }

        public string TenKhoa { get; set; }

        public string TenKhoaRutGon { get; set; }

        public string KyHoc { get; set; }

        public string MaLopTinChi { get; set; }

        public string MaGiaoVien { get; set; }

        public string GVHoLot { get; set; }

        public string GVTen { get; set; }

        public string SoTiet { get; set; }

        public string MaSinhVien { get; set; }

        public string LopSV { get; set; }

        public string SVHoLot { get; set; }

        public string SVTen { get; set; }

        public string SVNgaySinh { get; set; }

        public string GioiTinh { get; set; }

        public int TotalRecords { get; set; }

    }
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
    }

    public class HeDaoTao
    {
        public string maHe { get; set; }
        public string tenHe { get; set; }
    }

    public class KiHoc
    {
        public string MA_KY_HOC { get; set; }
        public string TEN_KY_HOC { get; set; }
    }
}
