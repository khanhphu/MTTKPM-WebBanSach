using System.Threading.Tasks;
using WebBanSach.Models.Data;
namespace WebBanSach.Areas.Admin.Code
{
    public interface IOrderObserver
    {
        Task<string> Update(DonDatHang order); // Lớp cần quan sát là đơn đặt hàng, có thể mở rộng 
    }
}