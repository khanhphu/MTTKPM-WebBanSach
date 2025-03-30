using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Controllers.Chain
{
    public class PasswordValidationHandler : BaseRegistrationHandler
    {
        public override string Handle(KhachHang model)
        {
            string password = model.MatKhau;

            // Kiểm tra mật khẩu không được null hoặc rỗng
            if (string.IsNullOrEmpty(password))
            {
                return "Mật khẩu không được để trống.";
            }

            // Kiểm tra độ dài tối thiểu của mật khẩu (ví dụ: 8 ký tự)
            if (password.Length < 8)
            {
                return "Mật khẩu phải có ít nhất 8 ký tự.";
            }

            // Kiểm tra mật khẩu có chứa ít nhất một chữ cái in hoa
            if (!password.Any(char.IsUpper))
            {
                return "Mật khẩu phải chứa ít nhất một chữ cái in hoa.";
            }

            // Kiểm tra mật khẩu có chứa ít nhất một số
            if (!password.Any(char.IsDigit))
            {
                return "Mật khẩu phải chứa ít nhất một số.";
            }

            // Nếu không có lỗi, tiếp tục xử lý bước tiếp theo
            return base.Handle(model);
        }
    }
}