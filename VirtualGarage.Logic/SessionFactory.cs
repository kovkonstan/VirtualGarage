using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TmpLabs.DataEngine;

namespace VirtualGarage.Logic
{
    /// <summary>
    /// Session factory
    /// </summary>
    public class SessionFactory : DefaultSessionFactory, ISessionFactory
    {
        #region constructors

        private SessionFactory()
            : base(new DataSessionProvider())
        {
        }

        #endregion

        #region singleton

        /// <summary>
        /// Instance of SessionFactory (Singleton)
        /// </summary>
        public static ISessionFactory Instance
        {
            get
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionFactory();
                    }
                }

                return _instance;
            }
        }

        private static ISessionFactory _instance = null;
        private static readonly Object locker = new Object();

        #endregion singleton
    }
}
