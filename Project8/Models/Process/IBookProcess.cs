using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Process
{
    public interface IBookProcess
    {
        // Khai báo các phương thức cần thiết
        List<Sach> NewDateBook(int count);
        List<Sach> ThemeBook(int id);
        List<Sach> TakeBook(int count);
        List<Sach> ShowAllBook();
    }
}
