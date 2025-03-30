using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;
using WebBanSach.Controllers.Chain;

namespace WebBanSach.Controllers.Chain { 

    public interface IRegistrationHandler
    {
        void SetNext(IRegistrationHandler nextHandler);
        string Handle(KhachHang model); 
    }
}