using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class BaseAddEventDivModel
    {
        public BaseEventModel EventModel { get; set; }

        public IEnumerable<SelectListItem> AllCurrences { get; set; }

        public IEnumerable<SelectListItem> AllEventPlaces { get; set; }

        public virtual void Fill(Int32 eventID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                this.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
                {
                    Text = cur.CurrencyName,
                    Value = cur.CurrencyID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();
            }
        }
    }
}