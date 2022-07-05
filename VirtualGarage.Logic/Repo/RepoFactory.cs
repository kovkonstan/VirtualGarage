using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TmpLabs.DataEngine.Repo;
using VirtualGarage.Logic.Repo.Impl;

namespace VirtualGarage.Logic.Repo
{
    public class RepoFactory : AbstractRepoFactory
    {
        #region constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="RepoFactory"/> class from being created.
        /// </summary>
        private RepoFactory()
            : base(SessionFactory.Instance)
        { }

        #endregion

        #region singleton

        /// <summary>
        /// Instance of SessionFactory (Singleton)
        /// </summary>
        public static RepoFactory Instance
        {
            get
            {
                return SingletonCreator.Instance;
            }
        }

        private sealed class SingletonCreator
        {
            private static RepoFactory instance = null;
            private static readonly Object locker = new Object();

            public static RepoFactory Instance
            {
                get
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new RepoFactory();
                        }
                    }
                    return instance;
                }
            }
        }

        #endregion singleton

        //required
        protected override void FillMap()
        {
            RepoMap[typeof(IUserRepo)] = typeof(UserRepo);
        }
    }
}
