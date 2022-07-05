using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class EditEventModel : BaseCarModel
    {
        public Int32 EventID { get; set; }

        [DisplayName("Тип события")]
        public Int32 EventTypeID { get; set; }

        [DisplayName("Тип события")]
        public String EventTypeName { get; set; }

        //public IEnumerable<SelectListItem> AllEventTypes { get; set; }

        public override void Fill(string userName, int carID)
        {
            base.Fill(userName, carID);

            //using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            //{
            //    this.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
            //    {
            //        Text = eventType.EventTypeName,
            //        Value = eventType.EventTypeID
            //    }).ToList().Select(t => new SelectListItem()
            //    {
            //        Text = t.Text,
            //        Value = t.Value.ToString()
            //    }).ToList();
            //}
        }
    }
}