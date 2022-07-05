using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class ChangePasswordModel : LoginUserModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Старый пароль")]
        public String OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Новый пароль")]
        public String NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Подтверждение пароля")]
        public String ConfirmPassword { get; set; }
    }
}