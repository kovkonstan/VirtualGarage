using System;
using System.Linq;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Logic
{
    public interface ICarRepo : IRepository<Car>
    {
        IQueryable<Car> UserOwnedCars(Int32 userId);
    }
}