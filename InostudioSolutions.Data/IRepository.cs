using System;
using System.Linq;
using System.Linq.Expressions;

namespace InostudioSolutions.Data
{
    /// <summary>
    /// The base class with the main methods
    /// </summary>
    /// <typeparam name="T">type of entity</typeparam>
    public interface IRepository<T> : IQueryable<T>
    {
        /// <summary>
        /// unit of work this repo attached to 
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
        /// <summary>
        /// get all entities
        /// </summary>
        /// <returns></returns>
        IQueryable<T> All();
        /// <summary>
        /// filtered query
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable<T> Where(Expression<Func<T, bool>> expression); 
                

        /// <summary>
        /// add entity to context. no records actually saved to database. use IUnitOfWork.Save to commit
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);
        /// <summary>
        /// delete entity from context. no records deleted at this stage. use IUnitOfWork.Save to commit
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Create detached entity
        /// </summary>
        /// <returns></returns>
        T CreateEntity();

    }
}