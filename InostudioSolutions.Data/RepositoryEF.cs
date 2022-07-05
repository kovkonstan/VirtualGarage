using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InostudioSolutions.Data
{
    /// <summary>
    /// The base class describes the main methods
    /// </summary>
    /// <typeparam name="T">type of entity</typeparam>
    public class RepositoryEF<T> : IRepository<T> 
        where T : class, new()
    {
        public RepositoryEF(IUnitOfWorkEF unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ObjectSet = _unitOfWork.Context.Set<T>();
        }

        #region implementation of IRepository
        /// <summary>
        /// Get or set unit of work operation
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        /// <summary>
        /// Get all entities with a type T
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> All()
        {
            return ObjectSet;
        }

        /// <summary>
        /// Get custom entities
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return ObjectSet.Where(expression);
        }
        /// <summary>
        /// Add entity T type
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            ObjectSet.Add(entity);
        }

        /// <summary>
        /// Delete entity T type
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            ObjectSet.Remove(entity);
        }

        /// <summary>
        /// Create new entity (proxified but now attached)
        /// </summary>
        public T CreateEntity()
        {
            return ObjectSet.Create<T>();
        }
        
        #endregion
        
        /// <summary>
        /// Get unit of work context
        /// </summary>
        protected IDbSet<T> ObjectSet { get; private set; }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return ObjectSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        public Expression Expression
        {
            get { return ObjectSet.Expression; }
        }

        public Type ElementType
        {
            get { return ObjectSet.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return ObjectSet.Provider; }
        }

        #endregion

        #region privates
        
        private readonly IUnitOfWorkEF _unitOfWork;
        
        #endregion
    }
}
