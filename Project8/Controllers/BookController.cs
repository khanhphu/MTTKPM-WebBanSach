using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using PagedList;
using PagedList.Mvc;
using WebBanSach.Models.Data.Builder;

namespace WebBanSach.Controllers
{
    public class BookController : Controller
    {
        BSDBContext db = BSDBContext.Instance;
        private IBookProcess _bookProcess;
        public BookController()
        {
            // Khởi tạo BookProcess với một decorator
            IBookProcess baseProcess = new BookProcess();
            _bookProcess = new DiscountBook(baseProcess, 0.1);
        }

        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Book/TopDateBook : hiển thị ra 6 cuốn sách mới cập nhật theo ngày cập nhật
        //Parital View : TopDateBook
        public ActionResult TopDateBook()
        {
            //var result = new BookProcess().NewDateBook(6);
            var result = _bookProcess.NewDateBook(6);
            return PartialView(result);
        }

        //GET : /Book/Details/:id : hiển thị chi tiết thông tin sách
        public ActionResult Details(int id)
        {
            //var result = new AdminProcess().GetIdBook(id);
            var book = _bookProcess.ShowAllBook().FirstOrDefault(x => x.MaSach == id);

            if (book == null)
            {
                return HttpNotFound();
            }
            //return View(result);
            return View(book);
        }

        //GET : /Book/Favorite : hiển thị ra 3 cuốn sách bán chạy theo ngày cập nhật (silde trên cùng)
        //Parital View : FavoriteBook
        public ActionResult FavoriteBook()
        {
            //var result = new BookProcess().NewDateBook(3);
            var result = _bookProcess.NewDateBook(3);
            return PartialView(result);
        }

        //GET : /Book/DidYouSee : hiển thị ra 3 cuốn sách giảm dần theo ngày
        //Parital View : DidYouSee
        public ActionResult DidYouSee()
        {
            //var result = new BookProcess().TakeBook(3);
            var result = _bookProcess.TakeBook(3);
            return PartialView(result);
        }

        //GET : /Book/All : hiển thị tất cả sách trong db
        public ActionResult ShowAllBook(int? page)
        {
            //tạo biến số sản phẩm trên trang
            int pageSize = 10;

            //tạo biến số trang
            int pageNumber = (page ?? 1);

            //var result = new BookProcess().ShowAllBook().ToPagedList(pageNumber, pageSize);
            var result = _bookProcess.ShowAllBook().ToPagedList(pageNumber, pageSize);
            return View(result);
        }
        //GET : /Book/ThemesBook/:id : hiển thị sách theo chủ đề với giá giảm
        public ActionResult ThemesBook(int id)
        {
            var books = _bookProcess.ThemeBook(id); // Đảm bảo gọi qua Decorator để áp dụng giảm giá

            if (books == null || !books.Any())
            {
                return HttpNotFound();
            }

            return View(books);
        }


        //Filter Sach
        [HttpGet]
        public ActionResult Filter(SachFilter filter = null)
        {
            //lay ds sach tu csdl
            var sachList=db.Saches.AsNoTracking().ToList();
            var filterBuilder = new SachFilterBuilder();
            if (filter != null)
            {
                filterBuilder.TheLoaiFilter(filter.MaLoai)
                             .GiaBanFilter(filter.GiaBan)
                             .TacgiaFilter(filter.TenTG)
                             .NXBFilter(filter.TenNXB)
                             .NgayCapNhatStartFilter(filter.NgayCapNhatStart)
                             .NgayCapNhatEndFilter(filter.NgayCapNhatEnd);
            }
            var sachFilter=filterBuilder.Build();
            //ap dung bo loc vao dsach sach lay tu csdl
            var filteredSach=sachFilter.Apply(sachList);
            // Chuẩn bị dữ liệu cho view
            var theLoaiList = db.TheLoais.AsNoTracking().ToList();
            ViewBag.TheLoaiList = theLoaiList;
            ViewBag.filterResult = filter ?? new SachFilter();

            return View("Filter", filteredSach);
        }

        // Action mới để trả về panel lọc
        [HttpGet]
        public ActionResult GetFilterPanel()
        {
            try
            {
                ViewBag.TheLoaiList = db.TheLoais.ToList();
                var filter = new SachFilter();
                return PartialView("_FilterPanel", filter);
            }
            catch (Exception ex)
            {
                return Content("Lỗi: " + ex.Message);
            }
        }
    }
}