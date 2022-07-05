using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Models
{
    public class BaseCarModel
    {
        public BaseCarModel()
        {
            LoginUserModel = new LoginUserModel();
        }

        public virtual Int32 CarID { get; set; }

        public String CarName { get; set; }

        public UserAccesOnCar UserAcces { get; set; }

        public String PageName { get; set; } 

        public List<SimilarCarModel> SimilarCars { get; set; }

        public LoginUserModel LoginUserModel { get; set; }
	          
        public virtual void Fill(String userName, Int32 carID)
        {
            this.CarID = carID;
            this.LoginUserModel.Fill(userName);

            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                var car = carRepo.SingleOrDefault(it => it.CarID == carID);

                if (car != null)
                {
                    this.UserAcces = carRepo.CheckUserAcces(carID, userName);

                    if (this.UserAcces != UserAccesOnCar.Close)
                    {
                        this.CarName = car.CarModel.CarMark.CarMarkName + " " +
                                        car.CarModel.CarModelName + " " +
                                        car.Year;

                        this.SimilarCars = (from simCar in carRepo.GetSimilarCars(carID)
                                            let user = simCar.CarInGarages
                                                                 .Single(it => it.IsOwner).User
                                            select new SimilarCarModel()
                                            {
                                                CarID = simCar.CarID,
                                                CarName = simCar.CarModel.CarMark.CarMarkName + " " +
                                                             simCar.CarModel.CarModelName + " " +
                                                             simCar.Year,
                                                UserID = user.UserID,
                                                UserName = user.UserNick,
												IsContainImage = !String.IsNullOrEmpty(simCar.ImageType)
                                            }).ToList();
                        return;
                        
                    }                    
                }

                // Автомобиль не найден или
                // авто закрыт
                this.UserAcces = UserAccesOnCar.Close;

                


            }
        }
    }
}