using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Data.ChainOfResponsitory
{
    public class TacGiaFilter : ISachFilter
    {
        private ISachFilter _filter;
        public IQueryable<Sach> Apply(IQueryable<Sach> sachQuery, SachFilter sachFilter)
        {
            if (!string.IsNullOrEmpty(sachFilter.TenTG))
            {
                string tenTG = sachFilter.TenTG.Trim().ToLower();

                sachQuery = sachQuery.Where(s => s.TacGia.TenTG != null && s.TacGia.TenTG.Contains(tenTG));

                // Tính độ tương đồng và sắp xếp theo độ tương đồng
                var filteredSachs = sachQuery
                      .Where(s => s.TacGia != null && s.TacGia.TenTG != null && s.TacGia.TenTG.ToLower().Contains(tenTG))
                      .ToList(); // Chuyển sang List để làm việc với LINQ to Objects

                // Tính độ tương đồng và sắp xếp theo độ tương đồng
                var sortedSachs = filteredSachs
                    .Select(s => new
                    {
                        Sach = s,
                       Distance = LevenshteinDistance(tenTG, s.TacGia.TenTG.ToLower())
                    })
                    .OrderBy(x => x.Distance) // Sắp xếp theo độ tương đồng (khoảng cách nhỏ nhất lên đầu)
                    .Select(x => x.Sach)
                    .ToList();
             

                sachQuery = sortedSachs.AsQueryable();
            }
            if (_filter != null)
            {
                return _filter.Apply(sachQuery, sachFilter);
            }
            return sachQuery;
        }
        public ISachFilter setNext(ISachFilter next)
        {
            _filter = next;
            return next;

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