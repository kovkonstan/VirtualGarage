using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VirtualGarage.Logic.BLL;
using TmpLabs.DataEngine.Repo;

namespace VirtualGarage.Logic.Repo
{
    /// <summary>
    /// User repository
    /// </summary>
    public interface IUserRepo : IUserRepoBase<User>
    {
    }
}
