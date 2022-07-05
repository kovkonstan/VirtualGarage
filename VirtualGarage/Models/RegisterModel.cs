using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VirtualGarage.Models.Attributes;
using System.Web.Mvc;

namespace VirtualGarage.Models
{
    public class RegisterModel : IDataErrorInfo
    {
        [Required]
        [DisplayName("Имя пользователя")]
        [UserName(ErrorMessage = "Имя пользователя может состоять только из латинских букв и цифр и начинаться с буквы")]
        public String UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        [Email(ErrorMessage = "Введите корректный E-mail")]
        public String UserEmail { get; set; }


        [Required]      
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть больше 6 и меньше 50 знаков")]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public String Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароль не совпадает с подтверждением")]
        [DataType(DataType.Password)]
        [DisplayName("Подтверждение пароля")]
        public String ConfirmPassword { get; set; }

        [Required]
        [DisplayName("Я прочитал и обязуюсь соблюдать ")]
        public Boolean IsReadRules { get; set; }


        public string Error
        {
            get 
            {
                return null; 
            }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName == "IsReadRules" &&
                    !IsReadRules)
                {
                    return "При несогласии с Правилами сайта регистрация невозможна";
                }

                return null; 
            }
        }
    }
}