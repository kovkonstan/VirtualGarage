using System;
using System.Linq;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Logic
{
    public class CarRepo : RepositoryEF<Car>, ICarRepo
    {
        public CarRepo(IUnitOfWorkEF unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Car> UserOwnedCars(Int32 userId)
        {
            return from car in this
                   where car.CarInGarages.Any(it => it.UserID == userId && it.IsOwner)
                   select car;
        }
    }
}