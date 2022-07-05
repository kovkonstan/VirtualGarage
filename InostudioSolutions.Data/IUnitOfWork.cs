using System;

namespace InostudioSolutions.Data
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// commit changes
        /// </summary>
        void Save();

        /// <summary>
        /// try to commit changes
        /// </summary>
        /// <returns>null if OK, otherwise returns error string</returns>
        String SaveAndGetError();

        /// <summary>
        /// Create repository for entity type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IRepository<TEntity> CreateRepo<TEntity>()
            where TEntity : class, new();

        /// <summary>
        /// Create repository object for repo interface type
        /// </summary>
        /// <typeparam name="TRepoInterface"></typeparam>
        /// <returns></returns>
        TRepoInterface CreateInterfacedRepo<TRepoInterface>()
            where TRepoInterface : class;
    }
}