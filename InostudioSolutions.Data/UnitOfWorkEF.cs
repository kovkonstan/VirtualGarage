using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace InostudioSolutions.Data
{
    /// <summary>
    /// Inplementation of UnitOfWork for EntityFramework
    /// </summary>
    /// <typeparam name="T">type of context, inherited from DbContext</typeparam>
    public class UnitOfWorkEF <T>: IUnitOfWorkEF 
        where T: DbContext, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UnitOfWorkEF()
        {
            Context = new T();
        }

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Submit changes 
        /// </summary>
        public void Save()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// Submit changes. return null if successful, otherwise returns error message
        /// </summary>
        public String SaveAndGetError()
        {
            try
            {                
                Context.SaveChanges();
                return null;
            }
            catch (DataException exception)
            {
                return exception.Message;
            }
            catch (DbException exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// Create repository object for entity type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IRepository<TEntity> CreateRepo<TEntity>() 
            where TEntity : class, new()
        {
            return new RepositoryEF<TEntity>(this);
        }
        /// <summary>
        /// Create repository object for repo interface type
        /// </summary>
        /// <typeparam name="TRepoInterface"></typeparam>
        /// <returns></returns>
        public virtual TRepoInterface CreateInterfacedRepo<TRepoInterface>()
            where TRepoInterface : class
        {
            var iface = typeof(TRepoInterface);
            if (!iface.IsInterface || !iface.IsGenericType)
                return null;
            var typeDef = iface.GetGenericTypeDefinition();
            if (typeDef == typeof(IRepository<>))
                return (TRepoInterface)Activator.CreateInstance(typeof(RepositoryEF<>).MakeGenericType(iface.GetGenericArguments()[0]), this);
            return null;
        }

        #endregion

        #region Implementation of IUnitOfWorkEF
        /// <summary>
        /// Get or set data context
        /// </summary>
        public DbContext Context { get; protected set; }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                Context.Dispose();
            _disposed = true;
        }

        ~UnitOfWorkEF()
        {
            Dispose(false);
        }

        private bool _disposed;

        #endregion

    }
}
