using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Process;

namespace WebBanSach.Models.Process
{
    public class UserProcess
    {
        //Tầng xử lý dữ liệu khách hàng

        BSDBContext db = null;

        /// <summary>
        /// Contructor
        /// </summary>
        public UserProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// hàm lấy mã khách hàng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>KhachHang</returns>
        public KhachHang GetIdUser(int id)
        {
            return db.KhachHangs.Find(id);
        }

        /// <summary>
        /// Hàm thêm khách hàng mới
        /// </summary>
        /// <param name="entity">KhachHang</param>
        /// <returns>int</returns>
        public int InsertUser(KhachHang entity)
        {
            db.KhachHangs.Add(entity);
            db.SaveChanges();
            return entity.MaKH;
        }

        /// <summary>
        /// hàm đăng nhập của khách hàng
        /// </summary>
        /// <param name="username">string</param>
        /// <param name="password">string</param>
        /// <returns>int</returns>
        public int Login(string username, string password)
        {
            var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == username);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.MatKhau == password)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// hàm kiểm tra đã tồn tại tài khoản trong db
        /// </summary>
        /// <param name="username">string</param>
        /// <param name="password">string</param>
        /// <returns>int</returns>
        public int CheckUsername(string username,string password)
        {
            var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == username);
            if(result == null)
            {
                return 0;
            }
            else
            {
                if(result.MatKhau == password)
                {
                    return 1;
                }
                return -1;
            }
        }

        /// <summary>
        /// hàm lưu thông tin cập nhật khách hàng
        /// </summary>
        /// <param name="entity">KhachHang</param>
        /// <returns>int</returns>
        public int UpdateUser(KhachHang entity)
        {
            try
            {
                var kh = db.KhachHangs.Find(entity.MaKH);
                kh.TenKH = entity.TenKH;
                kh.Email = entity.Email;
                kh.DiaChi = entity.DiaChi;
                kh.DienThoai = entity.DienThoai;
                kh.NgaySinh = entity.NgaySinh;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }



    }
}