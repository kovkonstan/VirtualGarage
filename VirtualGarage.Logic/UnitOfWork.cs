using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Repository;

namespace VirtualGarage.Logic
{
    internal class UnitOfWork : UnitOfWorkEF<VirtualGarageEntities>
    {
        public override TRepoInterface CreateInterfacedRepo<TRepoInterface>()
        {
            if (typeof(TRepoInterface) == typeof(ICarRepo))
                return (new CarRepo(this)) as TRepoInterface;
            if (typeof(TRepoInterface) == typeof(IUserRepo))
                return (new UserRepo(this)) as TRepoInterface;
            if (typeof(TRepoInterface) == typeof(IEventRepo))
                return (new EventRepo(this)) as TRepoInterface;
            return base.CreateInterfacedRepo<TRepoInterface>();
        }

        public override IRepository<TEntity> CreateRepo<TEntity>()
        {
            //if (typeof(TEntity) == typeof(User))
            //    return (new UserRepo(this)) as IRepository<TEntity>;
            //if (typeof(TEntity) == typeof(Car))
            //    return (new CarRepo(this)) as IRepository<TEntity>;
            return base.CreateRepo<TEntity>();
        }
    }
}