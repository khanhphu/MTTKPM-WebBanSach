using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Process
{
    public class DiscountBook : BookProcessDecorator
    {
        private double _discountRate;

        public DiscountBook(IBookProcess bookProcess, double discountRate) : base(bookProcess)
        {
            this._discountRate = discountRate;
        }

        public override List<Sach> NewDateBook(int count)
        {
            var books = base.NewDateBook(count);
            return ApplyDiscount(books);
        }

        public override List<Sach> ThemeBook(int id)
        {
            var books = base.ThemeBook(id);
            return ApplyDiscount(books);
        }

        public override List<Sach> TakeBook(int count)
        {
            var books = base.TakeBook(count);
            return ApplyDiscount(books);
        }

        public override List<Sach> ShowAllBook()
        {
            var books = base.ShowAllBook();
            return ApplyDiscount(books);
        }

        public override Sach GetBookById(int id) // Triển khai phương thức mới
        {
            var book = base.GetBookById(id);
            if (book != null)
            {
                return ApplyDiscount(new List<Sach> { book })[0];
            }
            return null;
        }

        private List<Sach> ApplyDiscount(List<Sach> books)
        {
            foreach (var book in books)
            {
                // Nếu GiaBan là null, đặt mặc định là 0 để tránh lỗi
                if (!book.GiaBan.HasValue)
                {
                    book.GiaBan = 0;
                }

                // Luôn gán lại GiaBanGoc từ giá gốc của database để đảm bảo nhất quán
                book.GiaBanGoc = book.GiaBanGoc ?? book.GiaBan;

                // Tính lại giá sau khi giảm
                book.GiaBan = book.GiaBanGoc * (decimal)(1 - _discountRate);
            }
            return books;
        }
    }
}