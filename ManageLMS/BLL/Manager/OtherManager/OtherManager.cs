using ManageLMS.Common.DTO.Database;
using ManageLMS.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.BLL.Manager.OtherManager
{
    public class OtherManager
    {
        private DataDaoTao _dal = new DataDaoTao();

        public List<Khoa> GetListKhoa(bool includeAll = true)
        {
            var list = _dal.GetDanhSachKhoa();

            if (includeAll)
            {
                // Thêm item mặc định vào đầu danh sách
                list.Insert(0, new Khoa 
                { 
                    MaKhoa = "", 
                    TenKhoa = "-- Tất cả Khoa --" 
                });
            }
            return list;
        }


        public List<HeDaoTao> GetListHeDaoTao(bool includeAll = true)
        {
            var list = _dal.GetDanhSachHeDaoTao();

            if (includeAll)
            {
                list.Insert(0, new HeDaoTao 
                { 
                    maHe = "", 
                    tenHe = "-- Tất cả Hệ --" 
                });
            }
            return list;
        }


        public List<string> GetListLop(string khoaHoc, string maKhoa, bool includeAll = true)
        {
            // Gọi DAL lấy danh sách lớp
            var list = _dal.GetDanhSachLop(khoaHoc, maKhoa);

            if (includeAll)
            {
                // List<string> nên chỉ cần insert string
                list.Insert(0, ""); // Để rỗng hoặc "-- Tất cả --"
            }

            return list;
        }
        public KiHoc GetKyHoc(string maKiHoc)
        {
            return _dal.GetKyHocByMaKiHoc(maKiHoc);
        }

    }
}
