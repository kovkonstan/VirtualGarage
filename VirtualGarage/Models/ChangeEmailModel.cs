using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VirtualGarage.Models.Attributes;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class ChangeEmailModel : LoginUserModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        [Email(ErrorMessage = "Введите корректный E-mail")]  
        public String NewEmail { get; set;}
    }
}