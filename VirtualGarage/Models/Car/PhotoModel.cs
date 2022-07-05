using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class PhotoModel
    {
        public BaseCarModel BaseModel { get; set; }

        public String ImageType { get; set; }
    }
}