using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Controllers.Chain
{
    public class AddressValidationHandler : BaseRegistrationHandler
    {
        public override string Handle(KhachHang model)
        {
            string address = model.DiaChi;

            // Kiểm tra địa chỉ không được null hoặc rỗng
            if (string.IsNullOrEmpty(address))
            {
                return "Địa chỉ không được để trống.";
            }

            // Kiểm tra độ dài tối thiểu của địa chỉ (ví dụ: 5 ký tự)
            if (address.Length < 5)
            {
                return "Địa chỉ quá ngắn. Vui lòng nhập địa chỉ chi tiết hơn.";
            }

            // Nếu không có lỗi, tiếp tục xử lý bước tiếp theo
            return base.Handle(model);
        }
    }
}