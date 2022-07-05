using System;
using System.Linq;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using System.Collections.Generic;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Logic.Repository
{
    public interface ICarRepo : IRepository<Car>
    {
        /// <summary>
        /// Определяет права пользователя на автомобиль
        /// </summary>
        /// <param name="carID">Идентификатор автомобиля</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        UserAccesOnCar CheckUserAcces(int carID, String userName);

        /// <summary>
        /// Возвращает автомобили, владельцем которых является указанный пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Car> UserOwnedCars(Int32 userId);

        /// <summary>
        /// Возвращает все автомобили в гараже пользователя(в т.ч доверенные)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<Car> CarsInUserGarage(Int32 userId);

        /// <summary>
        /// Возвращает похожие автомобили
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        IEnumerable<Car> GetSimilarCars(Int32 carID);
    }
}