using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using System.IO;
using Project8.Areas.Admin.Code;
using System.Text.RegularExpressions;
using WebBanSach.Areas.Admin.Code;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using WebBanSach.Models.Repository;

namespace WebBanSach.Areas.Admin.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        //Trang quản lý

        //Khởi tạo biến dữ liệu : db
        BSDBContext db = BSDBContext.Instance;
        //Khoi tao ObserverSubject 

        private readonly OrderSubject _orderSubject;
        public HomeController()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; // Buộc dùng TLS 1.2
            _orderSubject = new OrderSubject();
            _tacGiaRepository = new TacGiaRepository(); // Khởi tạo trực tiếp

            //Phải comment lại mỗi khi push lên github ko nó sẽ xóa ở Sendgrid
            //string sendGridAPIKey = "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";

            //  _orderSubject.Attach(new EmailObserver(sendGridAPIKey,db));


        }

        // GET: Admin/Home : trang chủ Admin
        public ActionResult Index()
        {
            return View();
        }

        #region Sản phẩm

        //GET : Admin/Home/ShowListBook : Trang quản lý sách
        [HttpGet]
        public ActionResult ShowListBook()
        {
            //Gọi hàm ListAllBook và truyền vào model trả về View
            var model = new AdminProcess().ListAllBook();

            return View(model);
        }

        //GET : Admin/Home/AddBook : Trang thêm sách mới
        public ActionResult AddBook()
        {
            //lấy mã mà hiển thị tên
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG");

            return View();
        }

        private int TaoMaSach()
        {
            int r = (from ddh in db.Saches orderby ddh.MaSach descending select ddh.MaSach).First();
            return r + 1;
        }

        //POST : Admin/Home/AddBook : thực hiện thêm sách
        [HttpPost]
        public ActionResult AddBook(Sach model, HttpPostedFileBase fileUpload)
        {

            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG");
            //kiểm tra việc upload ảnh
            if (fileUpload == null)
            {
                ViewBag.Alert = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                //kiểm tra dữ liệu db có hợp lệ?
                if (ModelState.IsValid)
                {
                    //lấy file đường dẫn
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //chuyển file đường dẫn và biên dịch vào /images
                    var path = Path.Combine(Server.MapPath("/images"), fileName);
                    //kiểm tra đường dẫn ảnh có tồn tại?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Alert = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }

                    Sach.SachBuilder sachBuilder = new Sach.SachBuilder();


                    sachBuilder.SetMaLoai(model.MaLoai)
                              .SetMaNXB(model.MaNXB)
                              .SetMaTG(model.MaTG)
                              .SetTenSach(model.TenSach)
                              .SetGiaBan(model.GiaBan)
                              .SetMota(model.Mota)
                              .SetNguoiDich(model.NguoiDich)
                              .SetAnhBia(fileName)
                              .SetNgayCapNhat(DateTime.Now)
                              .SetSoLuongTon(model.SoLuongTon);


                    var result = new AdminProcess().InsertBook(sachBuilder);

                    if (result > 0)
                    {
                        ViewBag.Success = "Thêm mới thành công";
                        //xóa trạng thái để thêm mới
                        ModelState.Clear();
                    }
                    else
                    {
                        ModelState.AddModelError("", "thêm không thành công.");
                    }
                }
            }
            return View();
        }

        //GET : Admin/Home/DetailsBook/:id : Trang xem chi tiết 1 cuốn sách
        [HttpGet]
        public ActionResult DetailsBook(int id)
        {
            //gọi hàm lấy id sách và truyền vào View
            var sach = new AdminProcess().GetIdBook(id);

            return View(sach);
        }


       

        public ActionResult UpdateBook(int id)
        {
            //gọi hàm lấy mã sách
            var sach = new AdminProcess().GetIdBook(id);

            //thực hiện việc lấy mã nhưng hiển thị tên và đúng tại mã đang chỉ định và gán vào ViewBag
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai", sach.MaLoai);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG", sach.MaTG);

            return View(sach);
        }

        //POST : /Admin/Home/UpdateBook : thực hiện việc cập nhật sách
        //Tương tự như thêm sách
        [HttpPost]
        public ActionResult UpdateBook(Sach sach, HttpPostedFileBase fileUpload)
        {
            //thực hiện việc lấy mã nhưng hiển thị tên ngay đúng mã đã chọn và gán vào ViewBag
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai", sach.MaLoai);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG", sach.MaTG);

            //Nếu không thay đổi ảnh bìa thì làm
            if (fileUpload == null)
            {
                //kiểm tra hợp lệ dữ liệu
                if (ModelState.IsValid)
                {
                    //gọi hàm UpdateBook cho việc cập nhật sách
                    var result = new AdminProcess().UpdateBook(sach);

                    if (result == 1)
                    {
                        ViewBag.Success = "Cập nhật thành công";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật không thành công.");
                    }
                }
            }
            //nếu thay đổi ảnh bìa thì làm
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/images"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Alert = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }

                    sach.AnhBia = fileName;
                    var result = new AdminProcess().UpdateBook(sach);
                    if (result == 1)
                    {
                        ViewBag.Success = "Cập nhật thành công";
                    }
                    else
                    {
                        ModelState.AddModelError("", "cập nhật không thành công.");
                    }
                }
            }

            return View(sach);
        }

        //DELETE : Admin/Home/DeleteBook/:id : thực hiện xóa 1 cuốn sách
        [HttpDelete]
        public ActionResult DeleteBook(int id)
        {
            //gọi hàm DeleteBook để thực hiện xóa
            new AdminProcess().DeleteBook(id);

            //trả về trang quản lý sách
            return RedirectToAction("ShowListBook");
        }

        //Category

        //GET : /Admin/Home/ShowListCategory : trang quản lý thể loại
        [HttpGet]
        public ActionResult ShowListCategory()
        {
            //gọi hàm ListAllCategory để hiện những thể loại trong db
            var model = new AdminProcess().ListAllCategory();

            return View(model);
        }

        //GET : Admin/Home/AddCategory : trang thêm thể loại
        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        //POST : Admin/Home/AddCategory/:model : thực hiện việc thêm thể loại vào db
        [HttpPost]
        public ActionResult AddCategory(TheLoai model)
        {
            //kiểm tra dữ liệu hợp lệ
            if (ModelState.IsValid)
            {
                //khởi tao biến admin trong WebBanSach.Models.Process
                var admin = new AdminProcess();

                //khởi tạo biến thuộc đối tượng thể loại trong db
                var tl = new TheLoai();

                //gán thuộc tính tên thể loại
                tl.TenLoai = model.TenLoai;

                //gọi hàm thêm thể loại (InsertCategory) trong biến admin
                var result = admin.InsertCategory(tl);

                //kiểm tra hàm
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    //xóa trạng thái
                    ModelState.Clear();

                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }

            return View(model);
        }

        //GET : Admin/Home/UpdateCategory/:id : trang cập nhật thể loại
        [HttpGet]
        public ActionResult UpdateCategory(int id)
        {
            //gọi hàm lấy mã thể loại
            var tl = new AdminProcess().GetIdCategory(id);

            //trả về dữ liệu View tương ứng
            return View(tl);
        }

        //POST : /Admin/Home/UpdateCategory/:id : thực hiện việc cập nhật thể loại
        [HttpPost]
        public ActionResult UpdateCategory(TheLoai tl)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //gọi hàm cập nhật thể loại
                var result = admin.UpdateCategory(tl);

                //thực hiện kiểm tra
                if (result == 1)
                {
                    return RedirectToAction("ShowListCategory");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(tl);
        }

        //DELETE : /Admin/Home/DeleteCategory:id : thực hiện xóa thể loại
        [HttpDelete]
        public ActionResult DeleteCategory(int id)
        {
            // gọi hàm xóa thể loại
            new AdminProcess().DeleteCategory(id);

            //trả về trang quản lý thể loại
            return RedirectToAction("ShowListCategory");
        }

        //Author

        private readonly ITacGiaRepository _tacGiaRepository;

       
        
        

        [HttpGet]
        public ActionResult ShowListAuthor()
        {
            var model = _tacGiaRepository.GetAll();
            return View(model);
        }

        public ActionResult AddAuthor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAuthor(TacGia model)
        {
            if (ModelState.IsValid)
            {
                var result = _tacGiaRepository.Add(model);
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdateAuthor(int id)
        {
            var tg = _tacGiaRepository.GetById(id);
            if (tg == null) return HttpNotFound();
            return View(tg);
        }

        [HttpPost]
        public ActionResult UpdateAuthor(TacGia tg)
        {
            if (ModelState.IsValid)
            {
                var result = _tacGiaRepository.Update(tg);
                if (result == 1)
                {
                    ViewBag.Success = "Cập nhật thành công";
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }
            return View(tg);
        }

        [HttpDelete]
        public ActionResult DeleteAuthor(int id)
        {
            _tacGiaRepository.Delete(id);
            return RedirectToAction("ShowListAuthor");
        }
    

    //Publish

    //GET : /Admin/Home/ShowListPublish : trang quản lý nhà xuất bản
    [HttpGet]
        public ActionResult ShowListPublish()
        {
            //gọi hàm xuất danh sách nhà xuất bản
            var model = new AdminProcess().ListAllPublish();

            return View(model);
        }

        //GET : /Admin/Home/AddPublish : trang quản lý nhà xuất bản
        public ActionResult AddPublish()
        {
            return View();
        }

        //POST : /Admin/Home/AddPublish/:model : thực hiện việc thêm nhà xuất bản
        [HttpPost]
        public ActionResult AddPublish(NhaXuatBan model)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //khởi tạo object(đối tượng) nhà xuất bản
                var nxb = new NhaXuatBan();

                //gán dữ liệu
                nxb.TenNXB = model.TenNXB;
                nxb.DiaChi = model.DiaChi;
                nxb.DienThoai = model.DienThoai;

                //gọi hàm thêm nhà xuất bản
                var result = admin.InsertPublish(nxb);
                //kiểm tra hàm
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }

            return View(model);
        }

        //GET : /Admin/Home/UpdatePublish/:id : trang thêm nhà xuất bản
        [HttpGet]
        public ActionResult UpdatePublish(int id)
        {
            //gọi hàm lấy mã nhà xuất bản
            var nxb = new AdminProcess().GetIdPublish(id);

            return View(nxb);
        }

        //GET : /Admin/Home/UpdatePublish/:id : thực hiện thêm nhà xuất bản
        [HttpPost]
        public ActionResult UpdatePublish(NhaXuatBan nxb)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //gọi hàm cập nhật nhà xuất bản
                var result = admin.UpdatePublish(nxb);
                //kiểm tra hàm
                if (result == 1)
                {
                    ViewBag.Success = "Cập nhật nhật thành công";
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(nxb);
        }

        //DELETE : Admin/Home/DeletePublish/:id : thực hiện xóa nhà xuất bản
        [HttpDelete]
        public ActionResult DeletePublish(int id)
        {
            //gọi hàm xóa hàm xuất bản
            new AdminProcess().DeletePublish(id);

            //trả về trang quản lý nhà xuất bản
            return RedirectToAction("ShowListPublish");
        }

        #endregion

        #region Phản hồi

        //Contact/Feedback : Liên hệ / phản hồi khách hàng

        [HttpGet]
        //GET : Admin/Home/FeedBack : xem danh sách thông báo phản hồi
        public ActionResult FeedBack()
        {
            var result = new AdminProcess().ShowListContact();

            return View(result);
        }

        //GET : Admin/Home/FeedDetail/:id : xem nội dung phản hồi khách hàng
        public ActionResult FeedDetail(int id)
        {
            var result = new AdminProcess().GetIdContact(id);

            return View(result);
        }

        //DELETE : Admin/Home/DeleteFeedBack/:id : xóa thông tin phản hồi khách hàng
        [HttpDelete]
        public ActionResult DeleteFeedBack(int id)
        {
            new AdminProcess().deleteContact(id);

            return RedirectToAction("FeedBack");
        }

        #endregion

        #region Người dùng

        //GET : /Admin/Home/ShowUser : trang quản lý người dùng
        public ActionResult ShowUser()
        {
            var result = new AdminProcess().ListUser();

            return View(result);
        }

        //GET : /Admin/Home/DetailsUser/:id : trang xem chi tiết người dùng
        public ActionResult DetailsUser(int id)
        {
            var result = new AdminProcess().GetIdCustomer(id);

            return View(result);
        }

        //DELETE : Admin/Home/DeleteUser/:id : xóa thông tin người dùng
        [HttpDelete]
        public ActionResult DeleteUser(int id)
        {
            new AdminProcess().DeleteUser(id);

            return RedirectToAction("ShowUser");
        }

        #endregion

        #region Đơn đặt hàng

       // GET : Admin/Home/Order : trang quản lý đơn đặt hàng
        public ActionResult Order()
        {
            var result = new OrderProcess().ListOrder();

            return View(result);
        }
       // POST: 
        [HttpPost]
        public async Task<ActionResult> UpdateOrder(int maDDH, bool tinhTrang, int tracking)
        {

            var order = db.DonDatHangs.FirstOrDefault(o => o.MaDDH == maDDH);
            if (order != null)
            {
                order.TinhTrang = tinhTrang;
                order.Tracking = tracking;
                db.SaveChanges();

                if (tinhTrang)
                {
                    var customerEmail = db.Database.SqlQuery<string>(
                        "SELECT Email FROM KhachHang WHERE MaKH = {0}", order.MaKH)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(customerEmail))
                    {
                        TempData["Message"] = $"Tìm thấy MaKH: {order.MaKH}";
                        var emailResult = await _orderSubject.Notify(order);
                        TempData["Message"] += $" | {emailResult}";
                    }
                    else
                    {
                        TempData["Message"] = $"Không tìm thấy email cho MaKH: {order.MaKH}";
                    }
                }
            }
            return RedirectToAction("Order");
        }

       // GET : /Admin/Home/DetailsOrder : trang xem chi tiết đơn hàng
        public ActionResult DetailsOrder(int id)
        {
            var result = new OderDetailProcess().ListDetail(id);

            return View(result);
        }

        #endregion

    }
}