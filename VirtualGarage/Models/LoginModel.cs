using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VirtualGarage.Models.Attributes;

namespace VirtualGarage.Models
{
    public class LoginModel
    {
        [Required]
        [DisplayName("Имя пользователя")]
        public String UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public String Password { get; set; }

        public Boolean IsRememberMe { get; set; }
    }
}