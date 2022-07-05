using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class TestEventModel
    {
        public Int32 EventTypeID { get; set; }

        public DateTime Date { get; set; }

        public Int32 Mileage { get; set; }

        public Int32 CurrencyID { get; set; }

        public Int32 GeneralCost { get; set; }

        public Int32 EventComments { get; set; }
    }
}