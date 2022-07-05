using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VirtualGarage.Models.Attributes;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class TransmitCarModel
    {
        public BaseCarModel BaseModel { get; set; }

        [Required]
        [Email(ErrorMessage = "Введите корректный E-mail")]
        [DisplayName("E-mail")]
        public String Email { get; set; }
    }
}