using ManageLMS.Common.DTO.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ManageLMS.DAL.Database
{
    class DataDaoTao
    {
        private readonly string _connString = ConfigurationManager.ConnectionStrings["DUE_DashBoardConnection"].ConnectionString;

        public List<TTSinhVien> GetAllSinhVien(string khoa, string searchKey, int pageNumber, int pageSize, string LopSV)
        {

            var listSinhVien = new List<TTSinhVien>();



            using (SqlConnection conn = new SqlConnection(_connString))
            {

                using (SqlCommand cmd = new SqlCommand("DB_sp_LayDsSinhVien", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Khoa", SqlDbType.VarChar, 10).Value = string.IsNullOrEmpty(khoa) ? "" : khoa;
                    cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(searchKey) ? "" : searchKey;
                    cmd.Parameters.Add("@Lop", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(LopSV) ? "" : LopSV;
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                    try
                    {

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                var sinhVien = new TTSinhVien

                                {
                                    LopSV = reader["LopSV"] == DBNull.Value
                                        ? string.Empty
                                        : reader["LopSV"].ToString(),
                                    MaSinhVien = reader["MA_SINH_VIEN"] == DBNull.Value
                                        ? string.Empty
                                        : reader["MA_SINH_VIEN"].ToString(),
                                    HoLot = reader["HO_LOT"] == DBNull.Value
                                        ? string.Empty
                                        : reader["HO_LOT"].ToString(),
                                    Ten = reader["TEN"] == DBNull.Value
                                        ? string.Empty
                                        : reader["TEN"].ToString(),
                                    NgaySinh = reader["NGAY_SINH"] == DBNull.Value
                                        ? string.Empty
                                        : reader["NGAY_SINH"].ToString(),
                                    GioiTinh = reader["gioitinh"] == DBNull.Value
                                        ? string.Empty
                                        : reader["gioitinh"].ToString(),
                                    TotalRecords = reader["TotalRecords"] != DBNull.Value
                                        ? Convert.ToInt32(reader["TotalRecords"])
                                        : 0,
                                };
                                listSinhVien.Add(sinhVien);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(string.Format("Lỗi cơ sở dữ liệu trong Stored Procedure 'DB_sp_LayDsSinhVien': {0}", ex.Message));
                    }

                    finally
                    {
                        if (conn.State == ConnectionState.Open)

                            conn.Close();
                    }
                }
            }
            return listSinhVien;
        }

        // Master course
        public List<TTDsSVTheoLopHP> GetDsSinhVienTheoLopHocPhan(int kyHoc, string maHocPhan,string maKhoa ,int pageNumber, int pageSize)
        {
            var listResult = new List<TTDsSVTheoLopHP>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand("DB_sp_LayDsSinhVien_TheoLopHocPhan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120; // Set timeout for safety

                    // Add Parameters
                    cmd.Parameters.Add("@KY_HOC", SqlDbType.Int).Value = kyHoc;

                    var paramMaHocPhan = new SqlParameter("@MA_HOC_PHAN", SqlDbType.NVarChar, 50);
                    paramMaHocPhan.Value = string.IsNullOrEmpty(maHocPhan) ? (object)DBNull.Value : maHocPhan;

                    //var paramMaHe = new SqlParameter("@MA_HE", SqlDbType.NVarChar, 50);
                    //paramMaHe.Value = string.IsNullOrEmpty(maHe) ? (object)DBNull.Value : maHe;

                    var paramMaKhoa = new SqlParameter("@MA_KHOA", SqlDbType.NVarChar, 50);
                    paramMaKhoa.Value = string.IsNullOrEmpty(maKhoa) ? (object)DBNull.Value : maKhoa;
                    cmd.Parameters.Add(paramMaHocPhan);
                    //cmd.Parameters.Add(paramMaHe);
                    cmd.Parameters.Add(paramMaKhoa);
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listResult.Add(new TTDsSVTheoLopHP
                                {
                                    MaHocPhan = reader["MA_HOC_PHAN"] != DBNull.Value ? reader["MA_HOC_PHAN"].ToString() : string.Empty,
                                    TenHocPhan = reader["TEN_HOC_PHAN"] != DBNull.Value ? reader["TEN_HOC_PHAN"].ToString() : string.Empty,
                                    MaHe = reader["MaHe"] != DBNull.Value ? reader["MaHe"].ToString() : string.Empty,
                                    TenHe = reader["TenHe"] != DBNull.Value ? reader["TenHe"].ToString() : string.Empty,
                                    MaKhoa = reader["MaKhoa"] != DBNull.Value ? reader["MaKhoa"].ToString() : string.Empty,
                                    TenKhoa = reader["TenKhoa"] != DBNull.Value ? reader["TenKhoa"].ToString() : string.Empty,
                                    TenKhoaRutGon = reader["TenKhoaRutGon"] != DBNull.Value ? reader["TenKhoaRutGon"].ToString() : string.Empty,
                                    KyHoc = reader["KY_HOC"] != DBNull.Value ? reader["KY_HOC"].ToString() : string.Empty,
                                    MaLopTinChi = reader["MA_LOP_TIN_CHI"] != DBNull.Value ? reader["MA_LOP_TIN_CHI"].ToString() : string.Empty,
                                    MaGiaoVien = reader["MA_GIAO_VIEN"] != DBNull.Value ? reader["MA_GIAO_VIEN"].ToString() : string.Empty,
                                    GVHoLot = reader["GV_HoLot"] != DBNull.Value ? reader["GV_HoLot"].ToString() : string.Empty,
                                    GVTen = reader["GV_Ten"] != DBNull.Value ? reader["GV_Ten"].ToString() : string.Empty,
                                    SoTiet = reader["SoTiet"] != DBNull.Value ? reader["SoTiet"].ToString() : string.Empty,
                                    MaSinhVien = reader["MA_SINH_VIEN"] != DBNull.Value ? reader["MA_SINH_VIEN"].ToString() : string.Empty,
                                    SVHoLot = reader["SV_HoLot"] != DBNull.Value ? reader["SV_HoLot"].ToString() : string.Empty,
                                    SVTen = reader["SV_Ten"] != DBNull.Value ? reader["SV_Ten"].ToString() : string.Empty,
                                    SVNgaySinh = reader["NGAY_SINH"] != DBNull.Value ? reader["NGAY_SINH"].ToString() : string.Empty,
                                    GioiTinh = reader["gioitinh"] != DBNull.Value ? reader["gioitinh"].ToString() : string.Empty,
                                    LopSV = reader["LopSV"] != DBNull.Value ? reader["LopSV"].ToString() : string.Empty,
                                    TotalRecords = reader["TotalRecords"] != DBNull.Value ? Convert.ToInt32(reader["TotalRecords"]) : 0
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(string.Format("Database error in 'DB_sp_LayDsSinhVien_TheoLopHocPhan': {0}", ex.Message));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                }
            }
            return listResult;
        }
        // semester course
        public List<TTDsLopTheoKiTheoTKB> GetDsLopTheoKiTheoTKB(int kyHoc, string maLopTinChi, int pageNumber, int pageSize)
        {
            var listResult = new List<TTDsLopTheoKiTheoTKB>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand("DB_sp_LayDsLop_TheoKi_TheoTKB", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.Add("@KY_HOC", SqlDbType.Int).Value = kyHoc;

                    var paramMaLop = new SqlParameter("@MA_LOP_TIN_CHI", SqlDbType.NVarChar, 50);
                    paramMaLop.Value = string.IsNullOrEmpty(maLopTinChi) ? (object)DBNull.Value : maLopTinChi;
                    cmd.Parameters.Add(paramMaLop);

                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    listResult.Add(new TTDsLopTheoKiTheoTKB
                                    {
                                        KyHoc = reader["KY_HOC"] != DBNull.Value ? reader["KY_HOC"].ToString() : string.Empty,
                                        MaGiaoVien = reader["MA_GIAO_VIEN"] != DBNull.Value ? reader["MA_GIAO_VIEN"].ToString() : string.Empty,
                                        GVHoLot = reader["GV_HoLot"] != DBNull.Value ? reader["GV_HoLot"].ToString() : string.Empty,
                                        GVTen = reader["GV_Ten"] != DBNull.Value ? reader["GV_Ten"].ToString() : string.Empty,
                                        MaHocPhan = reader["MA_HOC_PHAN"] != DBNull.Value ? reader["MA_HOC_PHAN"].ToString() : string.Empty,
                                        MonHoc = reader["MonHoc"] != DBNull.Value ? reader["MonHoc"].ToString() : string.Empty,
                                        SoTiet = reader["SoTiet"] != DBNull.Value ? reader["SoTiet"].ToString() : string.Empty,
                                        MaLopTinChi = reader["MA_LOP_TIN_CHI"] != DBNull.Value ? reader["MA_LOP_TIN_CHI"].ToString() : string.Empty,
                                        MaSinhVien = reader["MA_SINH_VIEN"] != DBNull.Value ? reader["MA_SINH_VIEN"].ToString() : string.Empty,
                                        SVTen = reader["SV_Ten"] != DBNull.Value ? reader["SV_Ten"].ToString() : string.Empty,
                                        NgaySinh = reader["NGAY_SINH"] != DBNull.Value ? reader["NGAY_SINH"].ToString() : string.Empty,
                                        GioiTinh = reader["gioitinh"] != DBNull.Value ? reader["gioitinh"].ToString() : string.Empty,
                                        LopSV = reader["LopSV"] != DBNull.Value ? reader["LopSV"].ToString() : string.Empty
                                        // Note: TotalRecords is also available in this result set if needed for pagination
                                    });
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(string.Format("Database error in 'DB_sp_LayDsLop_TheoKi_TheoTKB': {0}", ex.Message));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                }
            }
            return listResult;
        }
        public List<TTHocPhan> GetDsHocPhan()
        {
            var listResult = new List<TTHocPhan>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand("DB_sp_LayDsHocPhan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listResult.Add(new TTHocPhan
                                {
                                    MaHocPhan = reader["MA_HOC_PHAN"] != DBNull.Value ? reader["MA_HOC_PHAN"].ToString() : string.Empty,
                                    TenHocPhan = reader["TEN_HOC_PHAN"] != DBNull.Value ? reader["TEN_HOC_PHAN"].ToString() : string.Empty,
                                    MaKhoa = reader["MaKhoa"] != DBNull.Value ? reader["MaKhoa"].ToString() : string.Empty,
                                    TenKhoa = reader["TenKhoa"] != DBNull.Value ? reader["TenKhoa"].ToString() : string.Empty,
                                    TenKhoaRutGon = reader["TenKhoaRutGon"] != DBNull.Value ? reader["TenKhoaRutGon"].ToString() : string.Empty,
                                    MaHe = reader["Mahe"] != DBNull.Value ? reader["Mahe"].ToString() : string.Empty,
                                    TenHe = reader["Tenhe"] != DBNull.Value ? reader["Tenhe"].ToString() : string.Empty
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(string.Format("Database error in 'DB_sp_LayDsHocPhan': {0}", ex.Message));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                }
            }
            return listResult;
        }
        public List<TTGiangVien> GetDsGiangVien(string searchKey = "", int pageNumber = 1, int pageSize = 200)
        {
            var listResult = new List<TTGiangVien>();

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand("DB_sp_LayDsGiangVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.CommandTimeout = 120;
                    cmd.Parameters.Add("@SearchKey", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(searchKey) ? "" : searchKey;
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listResult.Add(new TTGiangVien
                                {
                                    MaGiaoVien = reader["MA_GIAO_VIEN"] != DBNull.Value ? reader["MA_GIAO_VIEN"].ToString() : string.Empty,
                                    HoLot = reader["HO_LOT"] != DBNull.Value ? reader["HO_LOT"].ToString() : string.Empty,
                                    Ten = reader["TEN"] != DBNull.Value ? reader["TEN"].ToString() : string.Empty,
                                    NgaySinh = reader["NgaySinh"] != DBNull.Value ? reader["NgaySinh"].ToString() : string.Empty,
                                    ThangSinh = reader["ThangSinh"] != DBNull.Value ? reader["ThangSinh"].ToString() : string.Empty,
                                    NamSinh = reader["NamSinh"] != DBNull.Value ? reader["NamSinh"].ToString() : string.Empty,
                                    E_Mail = reader["E_Mail"] != DBNull.Value ? reader["E_Mail"].ToString() : string.Empty,
                                    CoQuanCongTac = reader["CoQuanCongTac"] != DBNull.Value ? reader["CoQuanCongTac"].ToString() : string.Empty,
                                    DienThoaiDD = reader["DienThoaiDD"] != DBNull.Value ? reader["DienThoaiDD"].ToString() : string.Empty,
                                    TenKhoa = reader["TenKhoa"] != DBNull.Value ? reader["TenKhoa"].ToString() : string.Empty,
                                    TotalRecords = reader["TotalRecords"] != DBNull.Value ? Convert.ToInt32(reader["TotalRecords"]) : 0
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(string.Format("Database error in 'DB_sp_LayDsGiangVien': {0}", ex.Message));
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                }
            }
            return listResult;
        }
        public List<string> GetDanhSachLop(string khoaHoc, string maKhoa)
        {
            var result = new List<string>();

            string query = @"SELECT Lop 
                             FROM S_DANH_MUC_LOP 
                             WHERE KHOA_HOC = @Khoa_hoc 
                             AND Khoa = @Khoa";

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add("@Khoa_hoc", SqlDbType.VarChar).Value = string.IsNullOrEmpty(khoaHoc) ? "" : khoaHoc;
                    cmd.Parameters.Add("@Khoa", SqlDbType.VarChar).Value = string.IsNullOrEmpty(maKhoa) ? "" : maKhoa;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["Lop"] != DBNull.Value)
                                {
                                    result.Add(reader["Lop"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi lấy danh sách lớp: " + ex.Message);
                    }
                }
            }
            return result;
        }

        public List<Khoa> GetDanhSachKhoa()
        {
            var result = new List<Khoa>();

            string query = @"SELECT     
	                        MaKhoa, 
	                        TenKhoa
	                        FROM         S_DANH_MUC_KHOA
	                        WHERE     (ThuocTruong = 1) AND (BaoGiang = 1)";

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                var item = new Khoa
                                {
                                    MaKhoa = reader["MaKhoa"] != DBNull.Value ? reader["MaKhoa"].ToString() : string.Empty,
                                    TenKhoa = reader["TenKhoa"] != DBNull.Value ? reader["TenKhoa"].ToString() : string.Empty
                                };
                                result.Add(item);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi lấy danh sách khoa: " + ex.Message);
                    }
                }
            }
            return result;
        }
        public KiHoc GetKyHocByMaKiHoc(string maKiHoc)
        {
            string query = @"SELECT MA_KY_HOC, TEN_KY_HOC
                             FROM S_KY_HOC
                             WHERE MA_KY_HOC = @maKiHoc";

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@maKiHoc", maKiHoc);
            
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                return new KiHoc
                                {
                                    MA_KY_HOC =  reader["MA_KY_HOC"] != DBNull.Value ? reader["MA_KY_HOC"].ToString() : string.Empty,
                                    TEN_KY_HOC = reader["TEN_KY_HOC"] != DBNull.Value ? reader["TEN_KY_HOC"].ToString() : string.Empty,
                                };
                            }
                    
                            
                            return null; // hoặc return new KiHoc();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi lấy thông tin kỳ học: " + ex.Message);
                    }
                }
            }
        }
        public List<HeDaoTao> GetDanhSachHeDaoTao()
        {
            var result = new List<HeDaoTao>();

            string query = @"SELECT     Mahe, Tenhe
                             FROM       S_DANH_MUC_HE";

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new HeDaoTao
                                {
                                    maHe = reader["Mahe"] != DBNull.Value ? reader["Mahe"].ToString() : string.Empty,
                                    tenHe = reader["Tenhe"] != DBNull.Value ? reader["Tenhe"].ToString() : string.Empty
                                };
                                result.Add(item);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi lấy danh sách hệ đào tạo: " + ex.Message);
                    }
                }
            }
            return result;
        }
    }
}
