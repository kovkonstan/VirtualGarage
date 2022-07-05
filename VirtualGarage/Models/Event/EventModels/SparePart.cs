using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class SparePart
    {
        [Required]
        [DisplayName("Тип запчасти")]
        public Int32 SparePartTypeID { get; set; }

        [DisplayName("Тип запчасти")]
        public String SparePartTypeName { get; set; }

        [DisplayName("Производитель")]
        public String SparePartProducer { get; set; }

        [DisplayName("Модель")]
        public String SparePartModel { get; set; }

        [DisplayName("Цена")]
        public String SparePartCost { get; set; }

        [DisplayName("Цена за работу(установку)")]
        public String WorkCost { get; set; }

        public Boolean IsRepair { get; set; }
    }
}