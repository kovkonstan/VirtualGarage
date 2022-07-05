using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace VirtualGarage.Models.Attributes
{
    public class UserNameAttribute : RegularExpressionAttribute
    {
        public UserNameAttribute()
            : base("^[a-zA-Z][a-zA-Z0-9]+$") { }
    }
}



    
