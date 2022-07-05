using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VirtualGarage.Models.Attributes;

namespace VirtualGarage.Models
{
    public class ForgotPasswordModel : LoginUserModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        [Email(ErrorMessage = "Введите корректный E-mail")]
        public String Email { get; set; }
    }
}