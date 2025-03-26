using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using static System.Net.Mime.MediaTypeNames;
namespace WebBanSach.Models.Data.Builder
{
    /// <summary>
    /// Moi cap nhat sau khi sua nhanh master
    /// </summary>
    public class SachFilter
    {
        //lop chua cac tieu chi loc: loc theo gia, theo the loai, tac gia,...
        public int? MaLoai { get; set; }
        public decimal? GiaBan { get; set; }

        public string TenNXB { get; set; }
        public string TenTG { get; set; }

        public DateTime? NgayCapNhatStart { get; set; }
        public DateTime? NgayCapNhatEnd { get; set; }
        //? nullable cho  truong hop ko loc gi 


        //Phuong thuc loc
        public List<Sach> Apply(List<Sach> sachList)
        {
            var filteredSachs = sachList;

            // Lọc theo thể loại
            if (MaLoai.HasValue)
            {
                filteredSachs = filteredSachs.Where(s => s.MaLoai == MaLoai.Value).ToList();
            }

            // Lọc theo giá
            if (GiaBan.HasValue)
            {
                if (GiaBan.Value == 100000)
                {
                    filteredSachs = filteredSachs.Where(s => s.GiaBan > 0
                    && s.GiaBan <= GiaBan.Value).ToList();

                }
                else if (GiaBan.Value == 200000)
                {
                    filteredSachs = filteredSachs.Where(s => s.GiaBan > 100000
                   && s.GiaBan <= GiaBan.Value).ToList();
                }
                else if (GiaBan.Value == 9999999)
                {
                    filteredSachs = filteredSachs.Where(s => s.GiaBan > 200000
                  && s.GiaBan <= GiaBan.Value).ToList();
                }
            }



            // Lọc theo tên tác giả
            if (!string.IsNullOrEmpty(TenTG))
            {
                filteredSachs = filteredSachs.Where(s => s.TacGia != null && s.TacGia.TenTG.ToLower().Contains(TenTG.ToLower())).ToList();
                // Tính độ tương đồng và sắp xếp theo độ tương đồng
                var sortedSachsByCate = filteredSachs
                    .Select(s => new
                    {
                        Sach = s,
                        Distance = LevenshteinDistance(TenTG, s.TacGia.TenTG.ToLower())
                    })
                    .OrderBy(x => x.Distance) // Sắp xếp theo độ tương đồng (khoảng cách nhỏ nhất lên đầu)
                    .Select(x => x.Sach)
                    .ToList();

            }
            //Loc theo NXB
            if (!string.IsNullOrEmpty(TenNXB))
            {
                var filteredSach = filteredSachs.Where(s => s.NhaXuatBan != null && s.NhaXuatBan.TenNXB.ToLower().Contains(TenNXB.ToLower())).ToList();
                var sortedSachsByNXB = filteredSach.
                    Select(s => new
                    {
                        Sach = s,
                        Distance = LevenshteinDistance(TenNXB, s.NhaXuatBan.TenNXB.ToLower())
                    })
                     .OrderBy(x => x.Distance) // Sắp xếp theo độ tương đồng (khoảng cách nhỏ nhất lên đầu)
                    .Select(x => x.Sach)
                    .ToList();
            }

            // Lọc theo ngày cập nhật
            if (NgayCapNhatStart.HasValue)
            {
                filteredSachs = filteredSachs.Where(s => s.NgayCapNhat.HasValue && s.NgayCapNhat.Value >= NgayCapNhatStart.Value).ToList();
            }
            if (NgayCapNhatEnd.HasValue)
            {
                filteredSachs = filteredSachs.Where(s => s.NgayCapNhat.HasValue && s.NgayCapNhat.Value <= NgayCapNhatEnd.Value).ToList();
            }

            return filteredSachs;
        }


        // Thuật toán Levenshtein Distance để tính độ tương đồng giữa hai chuỗi
        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] matrix = new int[s1.Length + 1, s2.Length + 1];

            // Khởi tạo hàng đầu tiên và cột đầu tiên
            for (int i = 0; i <= s1.Length; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                matrix[0, j] = j;

            // Tính khoảng cách
            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[s1.Length, s2.Length];
        }



    }
}