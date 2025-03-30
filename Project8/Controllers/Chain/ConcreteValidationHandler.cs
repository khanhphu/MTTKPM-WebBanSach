using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using WebBanSach.Controllers.Chain;
namespace WebBanSach.Controllers.Chain
{
  


    public class EmailValidationHandler : BaseRegistrationHandler
    {
        public override string Handle(KhachHang model)
        {
            if (!model.Email.Contains("@"))
            {
                return "Email không hợp lệ.";
            }
            return base.Handle(model);
        }
    }

    public class SaveUserHandler : BaseRegistrationHandler
    {
        private UserProcess _userProcess = new UserProcess();

        public override string Handle(KhachHang model)
        {
            model.NgayTao = DateTime.Now;
            model.TrangThai = false;

            var result = _userProcess.InsertUser(model);
            if (result > 0)
            {
                return null; // Đăng ký thành công
            }
            return "Đăng ký không thành công.";
        }
    }
    public class UsernameExistsHandler : BaseRegistrationHandler
    {
        private UserProcess _userProcess = new UserProcess();

        public override string Handle(KhachHang model)
        {
            if (_userProcess.CheckUsername(model.TaiKhoan, model.MatKhau) != 0)
            {
                return "Tài khoản đã tồn tại.";
            }
            return base.Handle(model);
        }
    }
}