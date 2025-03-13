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

namespace WebBanSach.Controllers
{
    public class CartController : Controller
    {
        //khởi tạo dữ liệu

        private readonly BSDBContext db;
        private readonly IOrderFactory _orderFactory;
       
        public CartController(BSDBContext db, IOrderFactory orderFactory)
        {
            this.db = db;
            this._orderFactory = orderFactory;
        }



        public ActionResult Index()
        {
            var cart = GetCartFromSession();
            ViewBag.Total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public JsonResult Add(int id, int quantity = 1)
        {
            var book = db.Saches.Find(id);
            if (book == null)
            {
                return Json(new { success = false, message = "Sách không tồn tại" });
            }

            var cart = GetCartFromSession();
            var cartItem = cart.FirstOrDefault(item => item.sach.MaSach == id);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartModel { sach = book, Quantity = quantity });
            }

            SaveCartToSession(cart);
            var total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            var itemCount = cart.Count; 
            return Json(new { success = true, total = total, itemCount=itemCount });
        }

        [HttpPost]
        public JsonResult Update(int id, int quantity)
        {
            var cart = GetCartFromSession();
            var cartItem = cart.FirstOrDefault(item => item.sach.MaSach == id);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
            }

            if (quantity <= 0)
            {
                cart.Remove(cartItem); //xóa khỏi giỏ hàng nếu sl=0
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            SaveCartToSession(cart);
            var total = cart.Sum(item => item.sach.GiaBan.GetValueOrDefault(0) * item.Quantity);
            return Json(new { success = true, total = total, itemCount = cart.Count });
        }

        // POST: /Cart/Remove
        [HttpPost]
        public JsonResult Remove(int id)
        {
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
        }

        // POST: /Cart/Clear
        [HttpPost]
        public JsonResult Clear()
        {
            Session["Cart"] = null; // Clear the session
            return Json(new { success = true, total = 0 });
        }

        public ActionResult CartHeader()
        {
            var cart = GetCartFromSession();
            var list = new List<CartModel>();
            if (cart != null)
            {
                list = cart;
            }

            return PartialView(list);
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
        }
        [HttpPost]
        public async Task<ActionResult> Payment(FormCollection f)
        {
            var userName = Session["User"];
            var user = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == userName);
            int MaKH = user.MaKH;
            var cart = GetCartFromSession();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index"); 
            }

            try
            {
               
                // Create order using factory
                var order = await _orderFactory.CreateOrderAsync(cart, MaKH); 
                order.ThanhToan = 1;                                                                       
                foreach (var item in order.ChiTietDDHs)
                {
                    var book = db.Saches.Find(item.MaSach);
                    book.SoLuongTon -= item.SoLuong;
                }

                // Save to database
                db.DonDatHangs.Add(order);
                db.SaveChanges();
                //xoa sach da mua trong gio hang
                var purchasedId=order.ChiTietDDHs.Select(d=> d.MaSach).ToList();
                cart.RemoveAll(item => purchasedId.Contains(item.sach.MaSach));
                SaveCartToSession(cart);
                //chuyen den trang thanh toan thanh cong

                return Redirect("/Cart/Success");
            }
            catch (Exception ex)
            {
                log.Error("Lỗi khi thanh toán: ", ex);
                return Redirect("/Cart/Error");
            }

        }
        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult TrackingOder()
        {
            List<DonDatHang> donDatHang = db.DonDatHangs.Where(p => p.MaKH == UserController.khachhangstatic.MaKH).ToList();
            return View(donDatHang);
        }

        public ActionResult TrackingOderDetails(int id)
        {
            var result = new OderDetailProcess().ListDetail(id);

            return View(result);
        }
        public JsonResult loadOrder()
        {
            
            db.Configuration.ProxyCreationEnabled = false;
            var donDatHang = db.DonDatHangs.ToList();

            return Json(new { data = donDatHang }
                , JsonRequestBehavior.AllowGet);
          
        }
    }
}