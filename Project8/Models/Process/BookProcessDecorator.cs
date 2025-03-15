using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Process
{
    public class BookProcessDecorator: IBookProcess
    {
        protected IBookProcess _bookProcess;

        public BookProcessDecorator(IBookProcess bookProcess)
        {
            this._bookProcess = bookProcess;
        }

        public virtual List<Sach> NewDateBook(int count)
        {
            return _bookProcess.NewDateBook(count);
        }

        public virtual List<Sach> ThemeBook(int id)
        {
            return _bookProcess.ThemeBook(id);
        }

        public virtual List<Sach> TakeBook(int count)
        {
            return _bookProcess.TakeBook(count);
        }

        public virtual List<Sach> ShowAllBook()
        {
            return _bookProcess.ShowAllBook();
        }
    }
}