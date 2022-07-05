using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Exceptions;

namespace VirtualGarage.Models
{
    public class LoginUserModel
    {
        public List<CarInLeftMenuModel> UserCars { get; set; }

        public virtual void Fill(String userName)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                var me = unitOfWork.CreateInterfacedRepo<IUserRepo>().GetByLogin(userName);
                    var cars = from carInGarage in me.CarInGarages
                                let car = carInGarage.Car
                                let model = car.CarModel
                                select new CarInLeftMenuModel()
                                    {
                                        CarName = model.CarMark.CarMarkName + " " +
                                                model.CarModelName + " " +
                                                car.Year,
                                        CarID = car.CarID
                                    };
                this.UserCars = cars.ToList();

            }
         
        }
    }
}