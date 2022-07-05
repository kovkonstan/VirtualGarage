using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualGarage.Models
{
    public class StatisticsModel
    {
        public BaseCarModel BaseModel { get; set; }

        public List<String> GeneralCostsByCurrency { get; set; }

        public Int32 EventTypeID { get; set; }

        public IEnumerable<SelectListItem> AllEventTypes { get; set; }

        public String MileageInMonth { get; set; }
    }
}