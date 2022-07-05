using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VirtualGarage.Models.Attributes;

namespace VirtualGarage.Models
{
    public class UserModel : LoginUserModel
    {
        public Int32 UserID { get; private set; }

        [Required]
        [DisplayName("Имя пользователя")]
        public String UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        [Email(ErrorMessage = "Введите корректный E-mail")]
        public String UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public String Password { get; set; }

        [DisplayName("Роль")]
        public String UserRole { get; private set; }

    }
}