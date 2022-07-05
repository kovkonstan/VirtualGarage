using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace VirtualGarage.Models
{
    public class CarInfoModel
    {
        public BaseCarModel BaseModel { get; set; }

        public CarModel CarModel { get; set; }
    }
}