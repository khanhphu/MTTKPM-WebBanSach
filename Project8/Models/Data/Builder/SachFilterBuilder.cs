using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data.Builder;

namespace WebBanSach.Models.Data.Builder
{
    public class SachFilterBuilder
    {
        private SachFilter _filter;
        public SachFilterBuilder()
        {
            _filter = new SachFilter();
        }
        public SachFilterBuilder TheLoaiFilter(int? maLoai)
        {
            _filter.MaLoai = maLoai;
            return this;
        }
        public SachFilterBuilder GiaBanFilter(decimal? gia)
        {
            _filter.GiaBan = gia;
            return this;
        }

        public SachFilterBuilder TacgiaFilter(string tacgia)
        {
            _filter.TenTG = tacgia;
            return this;
        }
        public SachFilterBuilder NXBFilter(string nxb)
        {
            _filter.TenNXB = nxb;
            return this;
        }
        public SachFilterBuilder NgayCapNhatStartFilter(DateTime? start)
        {
            _filter.NgayCapNhatStart = start;
            return this;
        }
        public SachFilterBuilder NgayCapNhatEndFilter(DateTime? end)
        {
            _filter.NgayCapNhatEnd = end;
            return this;
        }

        public SachFilter Build()
        {
            return _filter;
        }



    }
}