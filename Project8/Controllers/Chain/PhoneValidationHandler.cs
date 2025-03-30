using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using WebBanSach.Controllers.Chain;
namespace WebBanSach.Controllers.Chain
{
    public class PhoneValidationHandler : BaseRegistrationHandler
    {
        public override string Handle(KhachHang model)
        {
            string phone = model.DienThoai;

            // Kiểm tra số điện thoại không được null hoặc rỗng
            if (string.IsNullOrEmpty(phone))
            {
                return "Số điện thoại không được để trống.";
            }

            // Kiểm tra số điện thoại phải bắt đầu bằng số 0
            if (!phone.StartsWith("0"))
            {
                return "Số điện thoại phải bắt đầu bằng số 0.";
            }

            // Kiểm tra độ dài của số điện thoại (phải từ 10 đến 11 số)
            if (phone.Length < 10)
            {
                return "Số điện thoại quá ngắn. Phải có ít nhất 10 số.";
            }

            if (phone.Length > 11)
            {
                return "Số điện thoại quá dài. Chỉ được tối đa 11 số.";
            }

            // Nếu không có lỗi, tiếp tục xử lý bước tiếp theo trong chuỗi trách nhiệm
            return base.Handle(model);
        }

    }
}