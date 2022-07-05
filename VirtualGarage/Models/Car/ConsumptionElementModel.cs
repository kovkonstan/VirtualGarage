using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class ConsumptionElementModel
    {
        public String ConsumptionName { get; set; }

        public Dictionary<String, Double> ConsumtionUnit { get; set; }
    }
}