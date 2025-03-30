using System;
using System.Collections.Generic;
using System.Linq;
using WebBanSach.Models.Data;
using System.Data.Entity;

namespace WebBanSach.Models.Repository
{
    public class TacGiaRepository : ITacGiaRepository
    {
        private readonly BSDBContext db;

        public TacGiaRepository()
        {
            db = new BSDBContext(); // Khởi tạo trực tiếp BSDBContext
        }

        public List<TacGia> GetAll()
        {
            return db.TacGias.OrderBy(x => x.MaTG).ToList();
        }

        public TacGia GetById(int id)
        {
            return db.TacGias.Find(id);
        }

        public int Add(TacGia entity)
        {
            db.TacGias.Add(entity);
            db.SaveChanges();
            return entity.MaTG;
        }

        public int Update(TacGia entity)
        {
            var tg = db.TacGias.Find(entity.MaTG);
            if (tg == null) return 0;

            tg.TenTG = entity.TenTG;
            tg.QueQuan = entity.QueQuan;
            tg.NgaySinh = entity.NgaySinh;
            tg.NgayMat = entity.NgayMat;
            tg.TieuSu = entity.TieuSu;
            db.SaveChanges();
            return 1;
        }

        public bool Delete(int id)
        {
            var tg = db.TacGias.Find(id);
            if (tg == null) return false;

            db.TacGias.Remove(tg);
            db.SaveChanges();
            return true;
        }
    }
}