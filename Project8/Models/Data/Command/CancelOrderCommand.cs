using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Data.Command
{
    public class CancelOrderCommand : ICommand
    {
        private readonly BSDBContext _context;
        private readonly int _orderId;
        private bool _previousStatus;

        public CancelOrderCommand(BSDBContext context, int orderId)
        {
            _context = context;
            _orderId = orderId;
        }

        public void Execute()
        {
            var order = _context.DonDatHangs.Find(_orderId);
            if (order != null)
            {
                _previousStatus = order.TinhTrang; // Lưu trạng thái trước đó
                order.TinhTrang = false; // 0 - Đã hủy
                _context.SaveChanges();
            }
        }

        public void Undo()
        {
            var order = _context.DonDatHangs.Find(_orderId);
            if (order != null)
            {
                order.TinhTrang = _previousStatus; // Khôi phục trạng thái trước đó
                _context.SaveChanges();
            }
        }
    }
}