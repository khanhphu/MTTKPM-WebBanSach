using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanSach.Models;
using WebBanSach.Models.Data;

namespace Project8.Models.Data
{
    public interface IOrderFactory
    {
        Task<DonDatHang> CreateOrderAsync(List<CartModel> cartModel, int cusId);
    }
}
