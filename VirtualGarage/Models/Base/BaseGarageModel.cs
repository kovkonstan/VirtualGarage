using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class BaseGarageModel
    {
        public BaseGarageModel()
        {
            LoginUserModel = new LoginUserModel();
        }

        public LoginUserModel LoginUserModel { get; set; }
    }
}