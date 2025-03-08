using Project8.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;
using WebBanSach.Models.Data;

namespace Project8.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly BSDBContext _context;
        private readonly IOrderFactory _orderFactory;

        public CheckoutController(BSDBContext context, IOrderFactory orderFactory)
        {
            _context = context;
            _orderFactory = orderFactory;
        }
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<CartModel> ?? new List<CartModel>();
            return View(cart);
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(int cusId, List<CartModel> cartModels)
        {
            if (!ModelState.IsValid || cartModels == null || !cartModels.Any())
            {
                ModelState.AddModelError("", "Gio hang trong hoac khong hop le!");
                return View(cartModels);
            }
            try
            {
                var order = _orderFactory.CreateOrderAsync(cartModels, cusId).Result; // Synchronous for simplicity

                foreach (var item in order.ChiTietDDHs)
                {
                    var book = _context.Saches.Find(item.MaSach);
                    book.SoLuongTon -= item.SoLuong;
                }

                _context.DonDatHangs.Add(order);
                _context.SaveChanges();

                Session["Cart"] = new List<CartModel>(); // Clear cart after success
                return RedirectToAction("OrderConfirmation", new { orderId = order.MaDDH });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cartModels);
            }
        }
        [HttpGet]
        public ActionResult OrderConfirmation(int orderId)
        {
            return View(orderId);
        }
    }
}