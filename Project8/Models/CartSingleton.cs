using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models
{
    public class CartSingleton
    {
        private static CartSingleton _instance;
        private static readonly object _lock = new object();
        private List<CartModel> _cartItems;

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
    }
}