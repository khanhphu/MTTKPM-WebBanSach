using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Strategies
{
    public class FixedShippingCostStrategy : IShippingCostStrategy
    {
        private readonly decimal _fixedCost;

        public FixedShippingCostStrategy(decimal fixedCost)
        {
            _fixedCost = fixedCost;
        }

        public decimal CalculateShippingCost(List<CartModel> cart)
        {
            return _fixedCost;
        }
    }
}