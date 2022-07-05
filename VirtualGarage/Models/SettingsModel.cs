using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Models
{
    public class SettingsModel : LoginUserModel
    {
        public String UserName { get; set; }

        public String UserEmail { get; set; }
    }
}