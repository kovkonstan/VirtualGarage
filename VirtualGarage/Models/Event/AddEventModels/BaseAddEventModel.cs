using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Models
{
    public class BaseAddEventModel : BaseCarModel
    {
        public BaseAddEventDivModel AddEventDivModel { get; set; }

        public IEnumerable<SelectListItem> AllEventTypes { get; set; }

        public virtual void Fill(string userName, int carID)
        {
            base.Fill(userName, carID);

            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                this.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
                {
                    Text = eventType.EventTypeName,
                    Value = eventType.EventTypeID
                }).ToList().Select(t => new SelectListItem()
                {
                    Text = t.Text,
                    Value = t.Value.ToString()
                }).ToList();
            }
        }
    }
}