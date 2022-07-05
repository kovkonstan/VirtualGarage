using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualGarage.Exceptions
{
    public class UserNotExistException : Exception
    {
        public UserNotExistException()
            : base()
        { }

    }
}