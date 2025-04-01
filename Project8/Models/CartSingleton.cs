using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBanSach.Models.Data;


namespace WebBanSach.Models
{
    public class CartSingleton
    {
        private static CartSingleton _instance;
        private static readonly object _lock = new object();
        private List<CartModel> _cartItems;
        public int? CurrentOrderId { get; set; } // Thêm để lưu MaDDH của đơn hàng mới

        private CartSingleton()
        {
            _cartItems = new List<CartModel>();
        }

        public static CartSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CartSingleton();
                        }
                    }
                }
                return _instance;
            }
        }

        public List<CartModel> CartItems => _cartItems;

        public void AddToCart(Sach book, int quantity)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.sach.MaSach == book.MaSach);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartModel { sach = book, Quantity = quantity });
            }
        }

        public void UpdateCartItem(int maSach, int quantity)
        {
            var item = _cartItems.FirstOrDefault(i => i.sach.MaSach == maSach);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _cartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void RemoveFromCart(int maSach)
        {
            var item = _cartItems.FirstOrDefault(i => i.sach.MaSach == maSach);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }

        public decimal? GetTotal()
        {
            return _cartItems.Sum(item => item.sach.GiaBan * item.Quantity);
        }



        ////thêm một chức năng cho singleton hỗ trợ đồng bộ đơn hàng clone
        //public async Task SyncWithOrder(BSDBContext db)
        //{
        //    if (CurrentOrderId.HasValue)
        //    {
        //        var order = await db.DonDatHangs
        //                    .Include("ChiTietDDHs") // EF6
        //                    .FirstOrDefaultAsync(o => o.MaDDH == CurrentOrderId.Value);
        //        if (order != null && !order.TinhTrang)
        //        {
        //            var cartDict = CartItems.ToDictionary(i => i.sach.MaSach, i => i.Quantity);
        //            foreach (var item in order.ChiTietDDHs.ToList())
        //            {
        //                if (cartDict.ContainsKey(item.MaSach))

        //                //if (cartDict.TryGetValue(item.MaSach  ,   out int newQuantity)) // Đúng cú pháp                        {
        //                {
        //                    item.SoLuong = cartDict[item.MaSach];
        //                    cartDict.Remove(item.MaSach);
        //                }
        //                else
        //                {
        //                    order.ChiTietDDHs.Remove(item);
        //                }
        //            }
        //            foreach (var newItem in cartDict)
        //            {
        //                var book = await db.Saches.FindAsync(newItem.Key);
        //                if (book != null && book.SoLuongTon >= newItem.Value)
        //                {
        //                    order.ChiTietDDHs.Add(new ChiTietDDH
        //                    {
        //                        MaSach = newItem.Key,
        //                        SoLuong = newItem.Value,
        //                        DonGia = book.GiaBan
        //                    });
        //                }
        //            }
        //            await db.SaveChangesAsync();
        //        }

        //    }
        //}
    }
}
