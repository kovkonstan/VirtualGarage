using System;
using System.Linq;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Enums;
using VirtualGarage.Logic.Exceptions;
using System.Collections.Generic;

namespace VirtualGarage.Logic.Repository
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

        public IQueryable<Car> CarsInUserGarage(int userId)
        {
            return from car in this
                   where car.CarInGarages.Any(it => it.UserID == userId)
                   select car;
        }        

        public UserAccesOnCar CheckUserAcces(int carID, String userName)
        {
            var car = this.SingleOrDefault(it => it.CarID == carID);
			if (car != null)
			{  
				var carInGarage = car.CarInGarages.FirstOrDefault(it => it.User.UserNick == userName);

				if (carInGarage != null && !car.CarIsReadOnly)
				{
					return carInGarage.IsOwner ? UserAccesOnCar.Own : UserAccesOnCar.Manage;
				}
				else if (carInGarage != null && car.CarIsReadOnly)
				{
					return UserAccesOnCar.Transmitted;
					
				}
				else if (car.CarVisible)
				{
					return UserAccesOnCar.Read;
				}
			}
			return UserAccesOnCar.Close;         
        } 

        public IEnumerable<Car> GetSimilarCars(int carID)
        {
			List<Car> similarCars;
            var car = this.Single(it => it.CarID == carID);

			// ѕоиск авто с такой же маркой
			Int32 count = this.Count(it => it.CarModel.CarMarkID == car.CarModel.CarMarkID && it.CarVisible && it.CarID != carID);			
			switch (count)
			{
				case 0:
				case 1:
				case 2:
					// ≈сли количество таких от 0 до 2, добавл€ем любые автомобили
					similarCars = this.Where(it => it.CarModel.CarMarkID == car.CarModel.CarMarkID && it.CarVisible && it.CarID != carID)
										.ToList();

                    foreach (var item in this.Where(it => it.CarVisible && 
												it.CarID != carID))
                    {
                        if (!similarCars.Any(simCar => simCar.CarID == item.CarID))
                        {
                            similarCars.Add(item);                            
                        }

                        if (similarCars.Count == 3)
                        {
                            break;                            
                        }
                    }

                    return similarCars;

				case 3:
					// ≈сли их количество равно 3, возвращаем их
					return this.Where(it => it.CarModel.CarMarkID == car.CarModel.CarMarkID && it.CarVisible && it.CarID != carID);
					
				default:
					// ≈сли их количество больше 3, то ищем из оставшихс€ автомобили с такой же моделью
					count = this.Count(it => it.CarModelID == car.CarModelID && 
											 it.CarVisible && 
											 it.CarID != carID);
										
					switch (count)
					{
						case 0:
						case 1:
						case 2:
							similarCars = this.Where(it => it.CarModelID == car.CarModelID && 
														   it.CarVisible && 
														   it.CarID != carID)
												.ToList();

                            foreach (var item in this.Where(it => it.CarVisible &&
																it.CarModel.CarMarkID == car.CarModel.CarMarkID && 
																it.CarID != carID))
                            {
                                if (!similarCars.Any(simCar => simCar.CarID == item.CarID))
                                {
                                    similarCars.Add(item);
                                }

                                if (similarCars.Count == 3)
                                {
                                    break;
                                }
                            }

                            //similarCars = similarCars.Concat(this.Where(it => it.CarVisible &&
                            //                                    it.CarModelID == car.CarModelID && 
                            //                                    it.CarID != carID &&
                            //                                    !similarCars.Contains(it))
                            //                    .Take(3 - count)).ToList();
							return similarCars;							
						case 3:
							similarCars = this.Where(it => it.CarModelID == car.CarModelID && 
											 it.CarVisible && 
											 it.CarID != carID &&
											 it.CarModel.CarMarkID == car.CarModel.CarMarkID).ToList();
							return similarCars;
						default:
							similarCars = this.Where(it => it.CarModelID == car.CarModelID &&
													 it.CarVisible &&
													 it.CarID != carID &&
													 it.CarModel.CarMarkID == car.CarModel.CarMarkID).Take(3).ToList();
							return similarCars;
					}
			}         
        }
    }
}