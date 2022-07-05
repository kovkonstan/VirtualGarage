using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class ConsumptionModel
    {
        public BaseCarModel BaseModel { get; set; }

        public List<ConsumptionElementModel> Consumptions { get; set; }
    }
}