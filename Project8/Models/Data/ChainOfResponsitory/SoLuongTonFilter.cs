using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Data.ChainOfResponsitory
{
    public class SoLuongTonFilter : ISachFilter
    {
        private ISachFilter _filter;
        public IQueryable<Sach> Apply(IQueryable<Sach> sachQuery, SachFilter sachFilter)
        {
            if (sachFilter.SoLuongTon.HasValue)
            {
                sachQuery = sachQuery.Where(s => s.MaTG >= sachFilter.SoLuongTon.Value);
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