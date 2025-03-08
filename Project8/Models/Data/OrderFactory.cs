using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBanSach.Models;
using WebBanSach.Models.Data;

namespace Project8.Models.Data
{
    public class OrderFactory : IOrderFactory
    {
        private readonly BSDBContext _context;
        public OrderFactory(BSDBContext context)
        {
            _context = context;
        }

        public async Task<DonDatHang> CreateOrderAsync(List<CartModel> cartModel, int cusId)
        {
            if (cartModel == null || !cartModel.Any())
            {
                throw new ArgumentException("Khong the tao don hang vi gio hang trong!");
            }
            var order = new DonDatHang
            {
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Now.AddDays(3),
                TinhTrang = false,
                MaKH = cusId,
                ThanhToan = null,
                Tracking = null,
                ChiTietDDHs = new HashSet<ChiTietDDH>(),
            };
            foreach (var item in cartModel)
            {
                var book = await _context.Saches.FindAsync(item.sach.MaSach);
                if (book == null || book.SoLuongTon < item.Quantity)
                {
                    throw new InvalidOperationException($"{book.MaSach} khong ton tai hoac het hang! ");
                }
                order.ChiTietDDHs.Add(new ChiTietDDH
                {
                    MaSach = book.MaSach,
                    SoLuong = item.Quantity,
                });
            }
            return order;


        }
    }
}