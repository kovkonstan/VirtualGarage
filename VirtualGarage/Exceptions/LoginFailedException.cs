using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(String message)
            : base(message)
        { }
    }
}