using EllipticCurve.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Areas.Admin.Code
{
    public class EmailObserver : IOrderObserver
    {
        private readonly string _apiKey;
        private readonly BSDBContext _context;
        public EmailObserver(string apiKey, BSDBContext context)
        {
            _apiKey = apiKey;
            _context = context;
        }
        private async Task<string> SendConfirmationEmail(DonDatHang order)
        {
            var customerEmail = _context.Database.SqlQuery<string>(
                "SELECT Email FROM KhachHang WHERE MaKH = {0}", order.MaKH)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(customerEmail))
            {
                return $"Không tìm thấy email cho MaKH: {order.MaKH}";
            }

            // Lấy chi tiết đơn hàng
            var orderDetails = _context.ChiTietDDHs
                .Where(ct => ct.MaDDH == order.MaDDH)
                .Join(_context.Saches,
                      ct => ct.MaSach,
                      s => s.MaSach,
                      (ct, s) => new { ct.SoLuong, s.TenSach, s.AnhBia })
                .ToList();

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new SendGrid.Helpers.Mail.EmailAddress("nghkphung95@gmail.com", "Book Store");
                var to = new SendGrid.Helpers.Mail.EmailAddress(customerEmail);
                var subject = $"Xác nhận đơn hàng #{order.MaDDH} - Book Store";

                var plainTextContent = $"Đơn hàng #{order.MaDDH} của bạn đã được xác nhận. Ngày giao dự kiến: {order.NgayGiao.ToShortDateString()}. Cảm ơn bạn đã mua sắm tại Book Store!";

                // Tạo danh sách sản phẩm trong email
                string productList = "";
                foreach (var item in orderDetails)
                {
                    productList += $@"
                <tr>
                   
                    <td style='padding: 10px; color: #555;'>
                        <strong>{item.TenSach}</strong><br/>
                        Số lượng: {item.SoLuong}
                    </td>
                </tr>";
                }

                var htmlContent = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                <h2 style='color: #2c3e50; text-align: center;'>Xác nhận đơn hàng #{order.MaDDH}</h2>
                <p style='color: #555; font-size: 16px;'>Cảm ơn bạn đã mua sắm tại <strong>Book Store</strong>! Đơn hàng của bạn đã được xác nhận thành công.</p>
                <hr style='border: 0; border-top: 1px solid #e0e0e0;'>
                <h3 style='color: #34495e;'>Thông tin đơn hàng</h3>
                <p><strong>Mã đơn hàng:</strong> #{order.MaDDH}</p>
                <p><strong>Ngày đặt:</strong> {order.NgayDat.ToShortDateString()}</p>
                <p><strong>Ngày giao dự kiến:</strong> {order.NgayGiao.ToShortDateString()}</p>
                <p><strong>Trạng thái:</strong> {(order.TinhTrang ? "Đã xác nhận" : "Chưa xác nhận")}</p>
                <hr style='border: 0; border-top: 1px solid #e0e0e0;'>
                <h3 style='color: #34495e;'>Sản phẩm đã đặt</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    {productList}
                </table>
                <hr style='border: 0; border-top: 1px solid #e0e0e0;'>
                <p style='color: #777; font-size: 14px; text-align: center;'>Nếu bạn có thắc mắc, vui lòng liên hệ qua email: <a href='mailto:nghkphung95@gmail.com' style='color: #3498db;'>nghkphung95@gmail.com</a></p>
                <p style='color: #777; font-size: 12px; text-align: center;'>© 2025 Book Store. All rights reserved.</p>
            </div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return $"Email xác nhận đơn hàng #{order.MaDDH} đã được gửi tới {customerEmail}";
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    return $"Gửi email xác nhận đơn hàng #{order.MaDDH} thất bại với mã trạng thái: {response.StatusCode}. Chi tiết: {errorBody}";
                }
            }
            catch (Exception ex)
            {
                string errorDetails = ex.Message;
                if (ex.InnerException != null)
                {
                    errorDetails += $" | Nội bộ: {ex.InnerException.Message}";
                }
                return $"Lỗi khi gửi email xác nhận đơn hàng #{order.MaDDH}: {errorDetails}";
            }
        }
        public async Task<string> Update(DonDatHang order)
        {
            if (order.TinhTrang)
            {
                return await SendConfirmationEmail(order);
            }
            return "Không có hành động gửi email.";
        }
        // Phương thức kiểm tra lấy email thành công hay không
        public bool TryGetCustomerEmail(int maKH, out string customerEmail)
        {
            customerEmail = _context.Database.SqlQuery<string>(
                "SELECT Email FROM KhachHang WHERE MaKH = {0}", maKH)
                .FirstOrDefault();

            return !string.IsNullOrEmpty(customerEmail);
        }
    }
}