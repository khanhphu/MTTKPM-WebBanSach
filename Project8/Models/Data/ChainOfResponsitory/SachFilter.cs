using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
namespace WebBanSach.Models.Data.ChainOfResponsitory
{ 
    public class SachFilter
    {
        //lop chua cac tieu chi loc: loc theo gia, theo the loai, tac gia,...
        public int? MaLoai { get; set; }
        public decimal? GiaBanMin { get; set; }
        public decimal? GiaBanMax { get; set; }
        public int? MaNXB {  get; set; }    
        public string TenTG { get; set; }
        public int? SoLuongTon { get; set; }    

        public DateTime? NgayCapNhatStart { get; set; }
        public DateTime? NgayCapNhatEnd { get; set; }
        //? nullable cho  truong hop ko loc gi 

    }
}