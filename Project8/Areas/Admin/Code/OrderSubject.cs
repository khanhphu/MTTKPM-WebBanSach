using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Areas.Admin.Code
{
    public class OrderSubject
    {
        //quản lý các observers
        private readonly List<IOrderObserver> _orderObservers = new List<IOrderObserver>();
        public void Attach(IOrderObserver observer)
        {
            _orderObservers.Add(observer);  
        }
        public void Detach(IOrderObserver observer)
        {
            _orderObservers.Remove(observer);
        }
        public async Task<string> Notify(DonDatHang order)
        {
            string result = "";
            foreach (var observer in _orderObservers)
            {
                result = await observer.Update(order);
            }
            return result;
        }
    }
}