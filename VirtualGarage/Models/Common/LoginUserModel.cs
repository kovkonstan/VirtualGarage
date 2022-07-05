using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.Exceptions;

namespace VirtualGarage.Models
{
    public class LoginUserModel
    {
        public String LoginName { get; set; }

        public Int32 CountOfReminders { get; set; }

        public List<CarInLeftMenuModel> UserCars { get; set; }

        //public virtual void Fill(String name)
        //{ }

        
    }
}