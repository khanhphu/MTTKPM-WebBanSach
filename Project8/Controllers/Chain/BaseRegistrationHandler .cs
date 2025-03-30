using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;
namespace WebBanSach.Controllers.Chain
{
    public abstract class BaseRegistrationHandler : IRegistrationHandler
    {
        protected IRegistrationHandler _nextHandler;

        public void SetNext(IRegistrationHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public virtual string Handle(KhachHang model)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(model);
            }
            return null;
        }
    }

}