using WebBanSach.Areas.Admin.Models;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebBanSach.Controllers.Chain;

namespace WebBanSach.Controllers
{
    public class UserController : Controller
    {
        //Khởi tạo biến dữ liệu : db
        BSDBContext db = BSDBContext.Instance;
        public static KhachHang khachhangstatic;
        [HttpGet]
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //GET: /User/Register : đăng kí tài khoản thành viên
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Tạo chuỗi xử lý
                IRegistrationHandler usernameHandler = new UsernameExistsHandler();
                IRegistrationHandler passwordHandler = new PasswordValidationHandler();
                IRegistrationHandler emailHandler = new EmailValidationHandler();
                IRegistrationHandler phoneHandler = new PhoneValidationHandler();
                IRegistrationHandler addressHandler = new AddressValidationHandler();
                IRegistrationHandler saveUserHandler = new SaveUserHandler();

                // Thiết lập chuỗi
                usernameHandler.SetNext(passwordHandler);
                passwordHandler.SetNext(emailHandler);
                emailHandler.SetNext(phoneHandler);
                phoneHandler.SetNext(addressHandler);
                addressHandler.SetNext(saveUserHandler);

                // Thực thi kiểm tra
                var errorMessage = usernameHandler.Handle(model);

                if (errorMessage == null)
                {
                    return RedirectToAction("KiemTraThongBaoKichHoat", "User");
                }
                else
                {
                    ModelState.AddModelError("", errorMessage);
                }
            }

            return View(model);
        }

        public ActionResult XacNhan(int khMaKh)
        {
            ViewBag.Makh = khMaKh;
            return View();
        }

        public JsonResult XacNhanEmail(int khMaKh)
        {
            KhachHang Data = db.KhachHangs.Where(x => x.MaKH == khMaKh).FirstOrDefault();
            Data.TrangThai = true;
            db.SaveChanges();
            var msg = "Đã Xác Nhận Email!";
            Session["User"] = null;
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "ngocson16032001@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }

            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html")));
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("ngocson16032001@gmail.com", "sonlaso119");
            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public ActionResult ThongBaoKichHoat()
        {
            return View();
        }
        public ActionResult KiemTraThongBaoKichHoat()
        {
            return View();
        }
        //GET : /User/LoginPage : trang đăng nhập

        public ActionResult LoginPage()
        {
            return View();
        }

        //POST : /User/LoginPage : thực hiện đăng nhập
        [HttpPost]
        public ActionResult LoginPage(LoginModel model)
        {
            //kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //gọi hàm đăng nhập trong AdminProcess và gán dữ liệu trong biến model
                var result = new UserProcess().Login(model.TaiKhoan, model.MatKhau);
                //Nếu đúng
                if (result == 1)
                {
                    //gán Session["LoginAdmin"] bằng dữ liệu đã đăng nhập
                    Session["User"] = model.TaiKhoan;
                    var kh = db.KhachHangs.Where(x => x.TaiKhoan == model.TaiKhoan).FirstOrDefault();
                    khachhangstatic = kh;
                    

                    //trả về trang chủ
                    return RedirectToAction("Index", "Home");
                }
                //nếu tài khoản không tồn tại
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                    //return RedirectToAction("LoginPage", "User");
                }
                //nếu nhập sai tài khoản hoặc mật khẩu
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác");
                    //return RedirectToAction("LoginPage", "User");
                }
            }

            return View();
        }

        //GET : /User/Login : đăng nhập tài khoản
        //Parital View : Login
        
        [ChildActionOnly]
        public ActionResult Login()
        {
            return PartialView();
        }

        //POST : /User/Login : thực hiện đăng nhập
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Login(LoginModel model)
        {
            //kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //gọi hàm đăng nhập trong AdminProcess và gán dữ liệu trong biến model
                var result = new UserProcess().Login(model.TaiKhoan, model.MatKhau);

                //Nếu đúng
                if (result == 1)
                {
                    //gán Session["LoginAdmin"] bằng dữ liệu đã đăng nhập
                    Session["User"] = model.TaiKhoan;
                    var kh = db.KhachHangs.Where(x => x.TaiKhoan == model.TaiKhoan).FirstOrDefault();
                    khachhangstatic = kh;
                    //trả về trang chủ
                    return PartialView();
                }
                //nếu tài khoản không tồn tại
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                    //return RedirectToAction("LoginPage", "User");
                }
                //nếu nhập sai tài khoản hoặc mật khẩu
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác");
                    //return RedirectToAction("LoginPage", "User");
                }
            }

            return PartialView();
        }

        //GET : /User/Logout : đăng xuất tài khoản khách hàng
        [HttpGet]
        public ActionResult Logout()
        {
            Session["User"] = null;
            khachhangstatic = null;
            return RedirectToAction("Index", "Home");
        }

        //GET : /User/EditUser : cập nhật thông tin khách hàng
        [HttpGet]
        public ActionResult EditUser()
        {
            //lấy dữ liệu từ session
            var model = Session["User"];

            if (ModelState.IsValid)
            {
                //tìm tên tài khoản
                var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == model);

                //trả về dữ liệu tương ứng
                return View(result);
            }

            return View();
        }

        //POST : /User/EditUser : thực hiện việc cập nhật thông tin khách hàng
        [HttpPost]
        public ActionResult EditUser(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                //gọi hàm cập nhật thông tin khách hàng
                var result = new UserProcess().UpdateUser(model);

                //thực hiện kiểm tra
                if (result == 1)
                {
                    return RedirectToAction("EditUser");                  
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(model);
        }

    }
}