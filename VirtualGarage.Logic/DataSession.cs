using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using TmpLabs.DataEngine.EF;
using TmpLabs.DataEngine;


namespace VirtualGarage.Logic
{
    /// <summary>
    /// Data session implementation
    /// </summary>
    internal class DataSession : EntityFrameworkDataSession, IDataSession
    {
        #region constructors

        /// <summary>
        /// Anonymous constructor
        /// </summary>
        /// <param name="context"></param>
        protected internal DataSession(DbContext context)
            : base(context)
        {
        }

        #endregion constructors

        #region public properties

        /// <summary>
        /// Current session user (null if anonymous session)
        /// </summary>
        public new BLL.User User
        {
            get
            {
                return (BLL.User)base.User;
            }
        }

        #endregion public properties
    }
}
