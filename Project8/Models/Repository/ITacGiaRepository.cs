using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Repository
{
    public interface ITacGiaRepository
    {
        List<TacGia> GetAll();
        TacGia GetById(int id);
        int Add(TacGia entity);
        int Update(TacGia entity);
        bool Delete(int id);
    }
}