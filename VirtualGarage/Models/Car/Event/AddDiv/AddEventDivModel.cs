using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Repository;
using System.Web.Mvc;

namespace VirtualGarage.Models
{
    public class AddEventDivModel : EventModel
    {        
        public void Fill(Int32 eventTypeID, String userName, Int32 carID)
        {
            this.CarID = carID;
            this.EventTypeID = eventTypeID;

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

                this.AllEventPlaces = unitOfWork.CreateRepo<Place>()
                                        .Where(it => it.User.UserNick == userName)
                                        .Select(pl => new
                {
                    Text = pl.PlaceName,
                    Value = pl.PlaceID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();
            }
        }

        public IEnumerable<SelectListItem> AllCurrences { get; set; }

        public IEnumerable<SelectListItem> AllEventPlaces { get; set; }
    }
}