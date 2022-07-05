using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class BaseDefaultModel
    {
        public BaseDefaultModel()
        {
            LoginUserModel = new LoginUserModel();
        }

        public LoginUserModel LoginUserModel { get; set; }

        public virtual void Fill(String userName)
        {
            LoginUserModel.Fill(userName);
        }
        
    }
}