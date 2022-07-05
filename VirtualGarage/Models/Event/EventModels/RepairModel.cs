using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class RepairModel : EventModel
    {
        public List<SparePartModel> SpareParts { get; set; }
    }
}