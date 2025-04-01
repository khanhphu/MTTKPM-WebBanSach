using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanSach.Models.Strategies
{
    public interface IShippingCostStrategy
    {
        decimal CalculateShippingCost(List<CartModel> cart);
    }
}
