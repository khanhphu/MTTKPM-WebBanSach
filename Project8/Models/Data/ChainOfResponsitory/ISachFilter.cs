using WebBanSach.Models.Data.ChainOfResponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Models.Data;

namespace  WebBanSach.Models.Data
{
    public interface ISachFilter
    {
        ISachFilter setNext(ISachFilter next);
        //khi loc xong 1 yeu cau- > chuyen sang yeu cau khac neu co
        IQueryable<Sach> Apply(IQueryable<Sach> sachQuery, SachFilter sachFilter);
    }
}
