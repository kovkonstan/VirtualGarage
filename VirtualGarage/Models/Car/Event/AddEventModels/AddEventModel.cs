using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.DataModel;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class AddEventModel
    {
        public BaseCarModel BaseModel { get; set; }

        [DisplayName("Тип события")]
        public Int32 EventTypeID { get; set; }

        public IEnumerable<SelectListItem> AllEventTypes { get; set; }
    }
}