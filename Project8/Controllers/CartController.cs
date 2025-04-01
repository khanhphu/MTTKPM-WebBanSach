using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using WebBanSach.Models;
using Newtonsoft.Json;
using Project8.Models.Data;
using System.Web.UI.WebControls;
using WebBanSach.Models.Data.Command;
using System.Data.Entity;

namespace WebBanSach.Controllers
{
    public class CartController : Controller
    {
        //khởi tạo dữ liệu

        private readonly BSDBContext db;
        private readonly IOrderFactory _orderFactory;
        private readonly IBookProcess _bookProcess; // Thêm biến để sử dụng IBookProcess

        public CartController(BSDBContext db, IOrderFactory orderFactory)
        {
            this.db = db;
            this._orderFactory = orderFactory;
            // Khởi tạo IBookProcess với DiscountBook
            IBookProcess baseProcess = new BookProcess();
            _bookProcess = new DiscountBook(baseProcess, 0.1); // Giảm giá 10%
        }

        // Lấy hoặc tạo CommandManager từ Session
        private CommandManager GetCommandManager()
        {
            var manager = Session["CommandManager"] as CommandManager;
            if (manager == null)
            {
                manager = new CommandManager();
                Session["CommandManager"] = manager;
            }
            return manager;
        }

        public ActionResult Index()
        {
            var cart = CartSingleton.Instance.CartItems; // Sửa: Lấy từ CartSingleton thay vì Session
            ViewBag.Total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            return View(cart);
        }

        private readonly CommandManager _commandManager = new CommandManager();

        // POST: /Cart/Add
        [HttpPost]
        public JsonResult Add(int id, int quantity = 1)
        {
            //var book = db.Saches.Find(id);
            var book = _bookProcess.GetBookById(id); // Sử dụng IBookProcess thay vì db trực tiếp
            if (book == null)
            {
                return Json(new { success = false, message = "Sách không tồn tại" });
            }
            /*
            //var cart = GetCartFromSession();
            //var cartItem = cart.FirstOrDefault(item => item.sach.MaSach == id);

            //if (cartItem != null)
            //{
            //    cartItem.Quantity += quantity;
            //}
            //else
            //{
            //    cart.Add(new CartModel { sach = book, Quantity = quantity });
            //}

            //SaveCartToSession(cart);
            //var total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            //var itemCount = cart.Count;
            //return Json(new { success = true, total = total, itemCount = itemCount });
            */
            CartSingleton.Instance.AddToCart(book, quantity);
            var total = CartSingleton.Instance.GetTotal();
            var itemCount = CartSingleton.Instance.CartItems.Count;
            return Json(new { success = true, total = total, itemCount = itemCount });
        }

        [HttpPost]
        public async Task<ActionResult> Update(int id, int quantity)
        {

            try
            {
                // Cập nhật số lượng trong giỏ hàng
                var item = CartSingleton.Instance.CartItems.FirstOrDefault(i => i.sach.MaSach == id);
                if (item == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }

                CartSingleton.Instance.UpdateCartItem(id, quantity);

                // Đồng bộ với đơn hàng trong database (nếu có)
                if (CartSingleton.Instance.CurrentOrderId.HasValue)
                {
                    using (var db = new BSDBContext())
                    {
                        await CartSingleton.Instance.SyncWithOrder(db);
                    }
                }

                // Tính toán kết quả
                var total = CartSingleton.Instance.GetTotal();
                var itemCount = CartSingleton.Instance.CartItems.Count;

                return Json(new { success = true, total = total, itemCount = itemCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        // POST: /Cart/Remove
        [HttpPost]
        public JsonResult Remove(int id)
        {
            /*
            var cart = GetCartFromSession();
            var cartItem = cart.FirstOrDefault(item => item.sach.MaSach == id);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
            }

            cart.Remove(cartItem);
            SaveCartToSession(cart);
            var total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            return Json(new { success = true, total = total, itemCount = cart.Count });
            */
            CartSingleton.Instance.RemoveFromCart(id);
            var total = CartSingleton.Instance.GetTotal();
            return Json(new { success = true, total = total, itemCount = CartSingleton.Instance.CartItems.Count });
        }

        // POST: /Cart/Clear
        [HttpPost]
        public JsonResult Clear()
        {
            /*
            Session["Cart"] = null; // Clear the session
            return Json(new { success = true, total = 0 });
            */
            CartSingleton.Instance.ClearCart();
            return Json(new { success = true, total = 0 });
        }

        public ActionResult CartHeader()
        {
            /*
            var cart = GetCartFromSession();
            var list = new List<CartModel>();
            if (cart != null)
            {
                list = cart;
            }

            return PartialView(list);
            */
            var cart = CartSingleton.Instance.CartItems;
            return PartialView(cart);
        }

        private List<CartModel> GetCartFromSession()
        {
            var cartJson = Session["Cart"] as string;
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartModel>();
            }
            return JsonConvert.DeserializeObject<List<CartModel>>(cartJson);
        }

        private void SaveCartToSession(List<CartModel> cart)
        {
            Session["Cart"] = JsonConvert.SerializeObject(cart, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }


        [HttpGet]
        [ChildActionOnly]
        public PartialViewResult UserInfo()
        {
            //lấy dữ liệu từ session
            var model = Session["User"];

            if (ModelState.IsValid)
            {
                //tìm tên tài khoản
                var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == model);

                //trả về dữ liệu tương ứng
                return PartialView(result);
            }

            return PartialView();
        }



        //private int TaoMaDDH()
        //{
        //    int r = (from ddh in db.DonDatHangs orderby ddh.MaDDH descending select ddh.MaDDH).First();
        //    return r + 1;
        //}

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [HttpGet]
        public ActionResult Payment()
        {
            /*
            //kiểm tra đăng nhập
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("LoginPage", "User");
            }
            else
            {
                var cart = GetCartFromSession();
                if (cart.Count == 0)
                {
                    return RedirectToAction("Index");
                }

                var list = cart;
                var sl = list.Sum(x => x.Quantity);
                decimal? total = list.Sum(x => x.Total);
                ViewBag.Quantity = sl;
                ViewBag.Total = total;
                return View(list);
            }
            */
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("LoginPage", "User");
            }
            var cart = CartSingleton.Instance.CartItems;
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }
            var sl = cart.Sum(x => x.Quantity);
            decimal? total = cart.Sum(x => x.sach.GiaBan.GetValueOrDefault(0) * x.Quantity);
            ViewBag.Quantity = sl;
            ViewBag.Total = total;
            return View(cart);
        }
        //[HttpPost]
        //public async Task<ActionResult> Payment(FormCollection f)
        //{
        //    var userName = Session["User"];
        //    var user = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == userName);
        //    int MaKH = user.MaKH;
        //    //var cart = GetCartFromSession();
        //    var cart = CartSingleton.Instance.CartItems;
        //    if (cart == null || !cart.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }
        
        //    try
        //    {

        //        // Create order using factory
        //        var order = await _orderFactory.CreateOrderAsync(cart, MaKH);
        //        order.ThanhToan = 1;
        //        foreach (var item in order.ChiTietDDHs)
        //        {
        //            var book = db.Saches.Find(item.MaSach);
        //            book.SoLuongTon -= item.SoLuong;
        //        }

        //        // Save to database
        //        db.DonDatHangs.Add(order);
        //        db.SaveChanges();
        //        //xoa sach da mua trong gio hang
        //        var purchasedId = order.ChiTietDDHs.Select(d => d.MaSach).ToList();
        //        cart.RemoveAll(item => purchasedId.Contains(item.sach.MaSach));
        //        //SaveCartToSession(cart); // Không cần SaveCartToSession vì đã dùng Singleton
        //        //chuyen den trang thanh toan thanh cong

        //        return Redirect("/Cart/Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Lỗi khi thanh toán: ", ex);
        //        return Redirect("/Cart/Error");
        //    }

        //}
        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TrackingOder()
        {
            List<DonDatHang> donDatHang = db.DonDatHangs.Where(p => p.MaKH == UserController.khachhangstatic.MaKH).ToList();
            return View(donDatHang);
        }

        [HttpPost]
        public ActionResult CancelOrder(int orderId)
        {
            var command = new CancelOrderCommand(db, orderId);
            GetCommandManager().ExecuteCommand(command); // Sử dụng CommandManager từ Session
            return RedirectToAction("TrackingOder");
        }

        [HttpPost]
        public ActionResult UndoCancelOrder()
        {
            GetCommandManager().Undo(); // Hoàn tác từ CommandManager trong Session
            return RedirectToAction("TrackingOder");
        }

        public ActionResult TrackingOderDetails(int id)
        {
            var result = new OderDetailProcess().ListDetail(id);
            return View(result);
        }
        //các actions khác...
        //Mua lại đơn hàng đã giao thành công
        public ActionResult ReOrder(int maDDH)
        {
           
                var originalOrder = db.DonDatHangs
                    .Include("ChiTietDDHs")
                    .FirstOrDefault(o => o.MaDDH == maDDH);
                if (originalOrder != null)
                {
                    var clonedOrder = originalOrder.CloneForCart();
                    db.DonDatHangs.Add(clonedOrder);
                    db.SaveChanges();

                    CartSingleton.Instance.CurrentOrderId = clonedOrder.MaDDH;
                    CartSingleton.Instance.CartItems.Clear();
                    CartSingleton.Instance.CartItems.AddRange(clonedOrder.ChiTietDDHs.Select(c => new CartModel
                    {
                        sach = db.Saches.Find(c.MaSach),
                        Quantity =(int) c.SoLuong
                    }));
                }
                return RedirectToAction("Index", "Cart");
            
        }
        // Action thanh toán
        [HttpPost]
        public async Task<ActionResult> Payment(FormCollection f)
        {
            var userName = Session["User"].ToString();

            if (string.IsNullOrEmpty(userName))
            {
                return Redirect("/Account/Login");
            }
            var user = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == userName);
            if (user == null)
            {
                return Redirect("/Cart/Error");
            }
            int MaKH = user.MaKH;

            if (user == null)
            {
                return Redirect("/Cart/Error");
            }

            var cart = CartSingleton.Instance.CartItems;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }
            try
            {
                DonDatHang order;

                // Tận dụng Prototype nếu có CurrentOrderId từ ReOrder
                if (CartSingleton.Instance.CurrentOrderId.HasValue)
                {
                    order = await db.DonDatHangs
                        .Include("ChiTietDDHs")
                        .FirstOrDefaultAsync(o => o.MaDDH == CartSingleton.Instance.CurrentOrderId.Value);

                    if (order != null && !order.TinhTrang)
                    {
                        // Cập nhật bản sao dựa trên CartItems
                        var cartDict = cart.ToDictionary(i => i.sach.MaSach, i => i.Quantity);
                        order.ChiTietDDHs.Clear(); // Xóa chi tiết cũ
                        foreach (var cartItem in cartDict)
                        {
                            var book = db.Saches.Find(cartItem.Key);
                            if (book != null && book.SoLuongTon >= cartItem.Value)
                            {
                                order.ChiTietDDHs.Add(new ChiTietDDH
                                {
                                    MaSach = cartItem.Key,
                                    SoLuong = cartItem.Value,
                                    DonGia = book.GiaBan
                                });
                            }
                            else
                            {
                                throw new Exception($"Sản phẩm {cartItem.Key} không đủ hàng.");
                            }
                        }
                        order.MaKH = MaKH;
                        order.NgayDat = DateTime.Now > new DateTime(2079, 6, 6) ? new DateTime(2079, 6, 6) : DateTime.Now;
                        order.NgayGiao = DateTime.Now.AddDays(3);
                        order.ThanhToan = 1;
                    }
                    else
                    {
                        throw new Exception("Đơn hàng sao chép không hợp lệ.");
                    }
                }
                else
                {
                    // Tạo đơn mới thủ công nếu không có CurrentOrderId
                    order = new DonDatHang
                    {
                        MaKH = MaKH,
                        NgayDat = DateTime.Now > new DateTime(2079, 6, 6) ? new DateTime(2079, 6, 6) : DateTime.Now,
                        NgayGiao = DateTime.Now.AddDays(3),
                        TinhTrang = false,
                        ThanhToan = 1,
                        ChiTietDDHs = cart.Select(item => new ChiTietDDH
                        {
                            MaSach = item.sach.MaSach,
                            SoLuong = item.Quantity,
                            DonGia = item.sach.GiaBan
                        }).ToList()
                    };
                }

                // Cập nhật tồn kho
                foreach (var item in order.ChiTietDDHs)
                {
                    var book = db.Saches.Find(item.MaSach);
                    if (book == null || book.SoLuongTon < item.SoLuong)
                    {
                        throw new Exception($"Sản phẩm {item.MaSach} không đủ hàng.");
                    }
                    book.SoLuongTon -= item.SoLuong;
                }

                // Lưu vào database (nếu là đơn mới)
                if (!db.DonDatHangs.Any(o => o.MaDDH == order.MaDDH))
                {
                    db.DonDatHangs.Add(order);
                }
                await db.SaveChangesAsync();

                // Xóa sản phẩm đã mua khỏi giỏ hàng
                var purchasedId = order.ChiTietDDHs.Select(d => d.MaSach).ToList();
                cart.RemoveAll(item => purchasedId.Contains(item.sach.MaSach));
                CartSingleton.Instance.CurrentOrderId = null; // Reset CurrentOrderId

               // TempData["SuccessMessage"] = "Thanh toán thành công đơn hàng #" + order.MaDDH;
                return Redirect("/Cart/Success");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi thanh toán: " + ex.Message;
                Console.WriteLine("Inner Exception: " + ex.InnerException?.Message); // Debug thêm
                return Redirect("/Cart/Error");
            }
        }
    }

}