using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VirtualGarage.Models
{
    public class UserInfoModel : LoginUserModel
    {
        public Int32 UserID { get; set; }

        [Required]
        [DisplayName("Имя пользователя")]
        public String UserName { get; set; }


    }
}