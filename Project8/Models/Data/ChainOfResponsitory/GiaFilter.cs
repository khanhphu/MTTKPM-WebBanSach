using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Data.ChainOfResponsitory
{
    public class GiaFilter : ISachFilter
    {
        private ISachFilter _filter;
        public IQueryable<Sach> Apply(IQueryable<Sach> sachQuery, SachFilter sachFilter)
        {

            if (sachFilter.GiaBanMin.HasValue)
            {
                sachQuery = sachQuery.Where(s => s.GiaBan.HasValue && s.GiaBan.Value >= sachFilter.GiaBanMin.Value);
                System.Diagnostics.Debug.WriteLine($"GiaFilter applied: GiaBanMin = {sachFilter.GiaBanMin.Value}");
            }
            if (sachFilter.GiaBanMax.HasValue)
            {
                sachQuery = sachQuery.Where(s => s.GiaBan.HasValue && s.GiaBan.Value <= sachFilter.GiaBanMax.Value);
                System.Diagnostics.Debug.WriteLine($"GiaFilter applied: GiaBanMin = {sachFilter.GiaBanMax.Value}");

            }
            if (_filter != null)
            {
                return _filter.Apply(sachQuery, sachFilter);
            }
            return sachQuery;
        }
        public ISachFilter setNext(ISachFilter next)
        {
            _filter = next;
            return next;

        }
    }
}