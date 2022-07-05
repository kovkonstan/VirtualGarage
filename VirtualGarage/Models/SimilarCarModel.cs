using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class SimilarCarModel
    {
        public Int32 CarID { get; set; }

        public Int32 UserID { get; set; }

        public String CarName { get; set; }

        public String UserName { get; set; }
    }
}