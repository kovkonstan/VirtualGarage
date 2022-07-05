using System.Data.Entity;

namespace InostudioSolutions.Data
{
    public interface IUnitOfWorkEF : IUnitOfWork
    {
        /// <summary>
        /// context object
        /// </summary>
        DbContext Context { get; }
    }
}
