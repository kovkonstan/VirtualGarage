using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualGarage.Models
{
    public class MileageStatisticsModel
    {
        public BaseCarModel BaseModel { get; set; }

        public String MileageInMonth { get; set; }

        public List<PointOnGraph> PointsYearMileage { get; set; }

        public List<SelectListItem> EventsYears { get; set; }


        public List<String> GeneralCostsByCurrency { get; set; }

        public Int32 EventTypeID { get; set; }

        public IEnumerable<SelectListItem> AllEventTypes { get; set; }
    }
}