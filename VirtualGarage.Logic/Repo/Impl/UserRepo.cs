using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VirtualGarage.Logic.BLL;
using TmpLabs.DataEngine.EF.Repo;
using TmpLabs.DataEngine;

namespace VirtualGarage.Logic.Repo.Impl
{
    /// <summary>
    /// User repository
    /// </summary>
    internal class UserRepo : AbstractRepo<User>, IUserRepo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepo"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public UserRepo(IDataSession session)
            : base(session)
        { }

        /// <summary>
        /// Gets the user by login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public User GetByLogin(String login)
        {
            return GetAll().FirstOrDefault(x => x.Login == login);
        }
    }
}
