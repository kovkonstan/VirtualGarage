using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TmpLabs.DataEngine.BLL;

namespace VirtualGarage.Logic.BLL
{
    public class User : AbstractUser
    {
        protected User()
        { }

        public User(String nick, String password, String email)
            : base(nick, password, email)
        { }
    }
}
