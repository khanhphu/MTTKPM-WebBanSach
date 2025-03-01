using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Process
{
    public class BookProcess
    {
        //Khởi tạo biến dữ liệu : db
        BSDBContext db = null;

        //constructor :  khởi tạo đối tượng
        public BookProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// lấy cuốn mới nhất theo ngày cập nhật
        /// </summary>
        /// <param name="count">int</param>
        /// <returns>List</returns>
        public List<Sach> NewDateBook(int count)
        {
            return db.Saches.OrderByDescending(x => x.NgayCapNhat).Take(count).ToList();
        }

        /// <summary>
        /// lọc sách theo chủ đề
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>List</returns>
        public List<Sach> ThemeBook(int id)
        {
            return db.Saches.Where(x => x.MaLoai == id).ToList();
        }

        /// <summary>
        /// Lấy sách chọn lọc
        /// </summary>
        /// <param name="count">int</param>
        /// <returns>List</returns>
        public List<Sach> TakeBook(int count)
        {
            return db.Saches.OrderBy(x => x.NgayCapNhat).Take(count).ToList();
        }

        /// <summary>
        /// Xem tất cả cuốn sách
        /// </summary>
        /// <returns>List</returns>
        public List<Sach> ShowAllBook()
        {
            return db.Saches.OrderBy(x => x.MaSach).ToList();
        }

    }
}