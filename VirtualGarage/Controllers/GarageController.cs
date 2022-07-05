using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InostudioSolutions.Data;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Models;
using VirtualGarage.Logic.Enums;
using VirtualGarage.Logic.Exceptions;
using VirtualGarage.Helpers;
using System.Web.Routing;

namespace VirtualGarage.Controllers
{
    [Authorize]
    public class GarageController : Controller
    {
        /// <summary>
        /// Гараж пользователя
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CarsInGarage(Int32? userID, Int32? page)
        {
            // По умолчанию стр.1
            page = (page ?? 1);

            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();
                var me = userRepo.GetByLogin(GetAuthenticatedName());
                userID = (userID ?? me.UserID);

                Boolean isMyGarage = (userID == me.UserID);	  

                try
                {
                    // Получить все автомобили пользователя
                    CarMapper mapper = new CarMapper();
                    List<VirtualGarage.Models.CarModel> carModels = (from car
                                                                         in userRepo.GetUserCarsByPage((Int32)page, _pageCarsSize, (Int32)userID, isMyGarage)                                                                         
                                                                         select mapper.GetCarModel(car)).ToList();
                    var model = new CarsInGarageModel()
                    {
                        Cars = carModels, 
                        CurrentPage = (Int32)page,
                        TotalPages = (Int32)Math.Ceiling((Double)userRepo.GetCountOfUserCars((Int32)userID) / _pageCarsSize),
                        IsMyGarage = isMyGarage
                    };

                    var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();				

                    model.Cars.ForEach(it => it.UserAcces = carRepo.CheckUserAcces(it.CarID, GetAuthenticatedName()));

					model.BaseModel = DataHelper.GetBaseGarageModel(unitOfWork, GetAuthenticatedName());

                    return View(model);
                }
                catch (Exception)
                {
                    return View("Message",new MessageWithRedirectModel("Невозможно отобразить страницу","","",null));
                }                
            }
        }

        /// <summary>
        /// Добавить автомобиль
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddCar()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				AddCarModel model = DataHelper.GetAddCarModel(unitOfWork, GetAuthenticatedName());
				return View(model);
			}
        }

        /// <summary>
        /// Добавить автомобиль (post)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCar(AddCarModel model, HttpPostedFileBase photo)
        {
            Car car = new Car();

			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{ 
				if (photo != null && photo.ContentLength > 1024000)
				{
					ModelState.AddModelError("", "Размер изображения должен быть меньше 1 мегабайта");
				}
				else
				{
				
					if (ModelState.IsValid)
					{	                     
                        var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                        try
                        {
                            car.CarIsReadOnly = false;
                            car.CarModelID = model.CarModel.ModelID;
                            car.CarVisible = model.CarModel.Visible;
                            car.CarcaseTypeID = model.CarModel.CarcaseTypeID;
                            car.ColorID = model.CarModel.ColorID;
                            car.Comment = model.CarModel.AdditionalFeatures;
                            car.CurrencyID = model.CarModel.CurrencyID;
                            car.EngineVolume = (model.CarModel.EngineVolume != "" ? Convert.ToInt32(model.CarModel.EngineVolume) : car.EngineVolume);
                            car.FuelTypeID = model.CarModel.FuelTypeID;
                            car.GearBoxID = model.CarModel.BoxTypeID;
                            car.BuyDate = model.CarModel.BuyDate;
                            car.Mileage = (model.CarModel.Mileage != "" ? Convert.ToInt32(model.CarModel.Mileage) : car.Mileage);
                            car.MonthMileage = (model.CarModel.MonthMileage != "" ? Convert.ToInt32(model.CarModel.MonthMileage) : car.MonthMileage);
                            car.Year = model.CarModel.Year;
                            car.CarInGarages = new List<CarInGarage>()
                        {
                            new CarInGarage()
                            {
                                UserID = (unitOfWork.CreateInterfacedRepo<IUserRepo>()).GetByLogin(GetAuthenticatedName()).UserID,
                                IsOwner = true
                            }
                        };

                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
							model = DataHelper.GetAddCarModel(model, unitOfWork, GetAuthenticatedName());

                            if (model.CarModel.MarkID != 0)
                            {
                                model.AllModels = from carModel in unitOfWork.CreateRepo<CarMark>()
                                    .SingleOrDefault(it => it.CarMarkID == model.CarModel.MarkID)
                                    .CarModels
                                                  select new SelectListItem()
                                                  {
                                                      Text = carModel.CarModelName,
                                                      Value = carModel.CarModelID.ToString()
                                                  };
                            }

                            return View(model);
                        }

                        if (photo != null)
                        {
                            car.CarPhoto = new Byte[photo.ContentLength];
                            photo.InputStream.Read(car.CarPhoto, 0, photo.ContentLength);
                            car.ImageType = photo.ContentType;
                        }

                        carRepo.Add(car);

                        String error;
                        if ((error = unitOfWork.SaveAndGetError()) != null)
                        {
                            ModelState.AddModelError("", error);
                        }
                        else
                        {
							return View("Message",
										new MessageWithRedirectModel("Автомобиль добавлен", 
																	"CarsInGarage", 
																	"Garage",
																	null));
                        }
                    }					
                }

				model = DataHelper.GetAddCarModel(model, unitOfWork, GetAuthenticatedName());
				if (model.CarModel.MarkID != 0)
				{
					model.AllModels = GetModelsListByMarkID(model.CarModel.MarkID);
				}

				return View(model);
            } 
        }

        /// <summary>
        /// Изменить автомобиль
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCar(Int32 carID)
        {
            AddCarModel model;
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                Car car = carRepo.SingleOrDefault(it => it.CarID == carID);

                if (car != null)
                {
                    model = new EditCarModel();
                    CarMapper mapper = new CarMapper();

                    try
                    {
                        model.CarModel = mapper.GetCarModel(car);
                        //model.CarID = car.CarID;
                        //model.AdditionalFeatures = car.Comment;
                        //model.BoxTypeID = (car.GearBoxType != null ? car.GearBoxType.GearBoxID : model.BoxTypeID);
                        //model.BuyDate = car.BuyDate;
                        //model.CarcaseTypeID = (car.CarcaseType != null ? car.CarcaseType.CarcaseTypeID : model.CarcaseTypeID);
                        //model.ColorID = (car.Color != null ? car.Color.ColorID : model.ColorID);
                        //model.CurrencyID = car.Currency.CurrencyID;
                        //model.EngineVolume = (car.EngineVolume == null || car.EngineVolume == 0 ? "" : car.EngineVolume.ToString());
                        //model.FuelTypeID = (car.FuelType != null ? car.FuelType.FuelTypeID : model.FuelTypeID);
                        //model.ModelID = car.CarModel.CarModelID;
                        //model.MarkID = car.CarModel.CarMark.CarMarkID;
                        //model.Mileage = (car.Mileage == null || car.Mileage == 0 ? "" : car.Mileage.ToString());
                        //model.MonthMileage = (car.MonthMileage == null || car.MonthMileage == 0 ? "" : car.MonthMileage.ToString());
                        //model.ReadOnly = car.CarIsReadOnly;
                        //model.Visible = car.CarVisible;
                        //model.Year = car.Year;
                        //model.ImageType = car.ImageType;
                    }
                    catch (Exception)
                    {
                        return View("Message",new MessageWithRedirectModel("Невозможно отобразить страницу","","",null));
                    }
                }
                else
                {
                    return View("Message",new MessageWithRedirectModel("Невозможно отобразить страницу","","",null));
                }

				model = DataHelper.GetAddCarModel(model, unitOfWork, GetAuthenticatedName());
				if (model.CarModel.MarkID != 0)
				{
					model.AllModels = GetModelsListByMarkID(model.CarModel.MarkID);
				}

				return View(model);
            }  
        }

        /// <summary>
        /// Изменить автомобиль (post)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCar(EditCarModel model, HttpPostedFileBase photo)
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{

				if (photo != null && photo.ContentLength > 1024000)
				{
					ModelState.AddModelError("", "Размер изображения должен быть меньше 1 мегабайта");
				}
				else
				{
                
                    var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                    var car = carRepo.SingleOrDefault(it => it.CarID == model.CarModel.CarID);

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            car.CarModelID = model.CarModel.ModelID;
                            car.CarVisible = model.CarModel.Visible;
                            car.CarcaseTypeID = model.CarModel.CarcaseTypeID;
                            car.ColorID = model.CarModel.ColorID;
                            car.Comment = model.CarModel.AdditionalFeatures;
                            car.CurrencyID = model.CarModel.CurrencyID;
                            car.EngineVolume = (model.CarModel.EngineVolume != "" ? Convert.ToInt32(model.CarModel.EngineVolume) : car.EngineVolume);
                            car.FuelTypeID = model.CarModel.FuelTypeID;
                            car.GearBoxID = model.CarModel.BoxTypeID;
                            car.BuyDate = model.CarModel.BuyDate;
                            car.Mileage = (model.CarModel.Mileage != "" ? Convert.ToInt32(model.CarModel.Mileage) : car.Mileage);
                            car.MonthMileage = (model.CarModel.MonthMileage != "" ? Convert.ToInt32(model.CarModel.MonthMileage) : car.MonthMileage);
                            car.Year = model.CarModel.Year;
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
							model = DataHelper.GetEditCarModel(model, unitOfWork, GetAuthenticatedName());
                            if (model.CarModel.MarkID != 0)
                            {
                                model.AllModels = GetModelsListByMarkID(model.CarModel.MarkID);
                            }
                            
                            return View(model);
                        }

                        if (photo != null)
                        {
                            car.CarPhoto = new Byte[photo.ContentLength];
                            photo.InputStream.Read(car.CarPhoto, 0, photo.ContentLength);
                            car.ImageType = photo.ContentType;
                        }

                        String error;
                        if ((error = unitOfWork.SaveAndGetError()) != null)
                        {
                            ModelState.AddModelError("", error);
                        }
                        else
                        {
							return View("Message",
										new MessageWithRedirectModel("Изменения сохранены",
																	"CarsInGarage",
																	"Garage",
																	null));
                        }
                    }
                }

				model = DataHelper.GetEditCarModel(model, unitOfWork, GetAuthenticatedName());
				if (model.CarModel.MarkID != 0)
				{
					model.AllModels = GetModelsListByMarkID(model.CarModel.MarkID);
				}

				return View(model);
            }					   			
        }        

        /// <summary>
        /// Возвращает все модели указанной марки
        /// </summary>
        /// <param name="markID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetModelsByMark(Int32 markID)
        {
            return Json(GetModelsListByMarkID(markID), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Удалить автомобиль
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="pageStr"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCar(Int32 carID, String pageStr)
        {
            Int32 page = Convert.ToInt32(pageStr);

            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                Car car = carRepo.SingleOrDefault(it => it.CarID == carID);

				if (car == null)
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Автомобиль не найден", "", "", null));
					
				}

				UserAccesOnCar acces = carRepo.CheckUserAcces(car.CarID, GetAuthenticatedName());
								

                if (acces == UserAccesOnCar.Own || acces == UserAccesOnCar.Transmitted || acces == UserAccesOnCar.Manage) 
                {
					if (acces == UserAccesOnCar.Own || acces == UserAccesOnCar.Transmitted)
					{
						carRepo.Delete(car);
					}
					else if (acces == UserAccesOnCar.Manage)
					{
						var carInGarage = car.CarInGarages.Single(it => it.User.UserNick == GetAuthenticatedName());
						car.CarInGarages.Remove(carInGarage);
					}	   				
			
                    String error;
                    if ((error = unitOfWork.SaveAndGetError()) != null)
                    {
                        ModelState.AddModelError("", "Не удалось удалить автомобиль");
                    }
					else
					{
						return PartialView("MessageWithRedirect",
									new MessageWithRedirectModel("Автомобиль успешно удален",
																 "",
																 "Garage",
																 new System.Web.Routing.RouteValueDictionary() { { "page", page } }
																 ));
					}



					try
					{
						var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();
						var totalPages = (Int32)Math.Ceiling((Double)userRepo
														.GetCountOfUserCars(GetAuthenticatedName()) / _pageCarsSize);
						if (page > totalPages) page = totalPages;

						CarMapper mapper = new CarMapper();
						List<VirtualGarage.Models.CarModel> cars = (from carInGarage
																	   in userRepo.GetUserCarsByPage(page, _pageCarsSize, GetAuthenticatedName())
																	select mapper.GetCarModel(carInGarage)).ToList();

						var model = new CarsInGarageModel()
						{
							Cars = cars,
							CurrentPage = (Int32)page,
							TotalPages = totalPages
						};

						return PartialView("CarsDiv", model);
					}
					catch (Exception)
					{
						return RedirectToAction("LoginFailed", "Account");
					}
                }
                else
                {
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Невозможно отобразить страницу", "", "", null));
                }
            }
        }

        [HttpGet]
        public ActionResult GarageStatistic()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                try
                {
                    var model = new GarageStatisticModel();
                    model.BaseModel = DataHelper.GetBaseGarageModel(unitOfWork, GetAuthenticatedName());

                    

                    // Заполнение AllEventTypes
                    model.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
                    {
                        Text = eventType.EventTypeName,
                        Value = eventType.EventTypeID
                    }).ToList().Select(t => new SelectListItem()
                    {
                        Text = t.Text,
                        Value = t.Value.ToString()
                    }).ToList();

                    Decimal sum = 0;
                    Single mileageInMonthSum = 0;
                    model.EventsYears = new List<SelectListItem>();

                    foreach (var c in model.BaseModel.LoginUserModel.UserCars)
                    {
                        // Создаем репозиторий для авто, получаем из него авто
                        // и проверяем на наличие авто
                        var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                        var car = carRepo.SingleOrDefault(it => it.CarID == c.CarID);

                        model.GeneralCostsByCurrency = new List<String>();                        

                        foreach (var item in unitOfWork.CreateRepo<Currency>().All())
                        {
                            sum += (Decimal)car.Events.Where(it => it.CurrencyID == item.CurrencyID)
                                        .Sum(it => it.GeneralCost);

                            //if (sum != 0)
                            //{
                            //    model.GeneralCostsByCurrency
                            //        .Add(sum.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture(item.CultureName)));
                            //}
                        }


                        // Получаем события для данного автомобиля,
                        // для которых определен текущий пробег, 
                        // и сортируем их по дате
                        var events = car.Events.Where(it => it.Mileage != null && it.Mileage != 0)
                            .OrderBy(it => it.Date).ToList();
                        // Определяем количество дней между первым и последним событием
                        Int32 countOfDays = Convert.ToInt32((events.Last().Date - events.First().Date).TotalDays);
                        // Определяем средний пробег в день, и умножаем полученное значение на 30
                        Single avgMileageOnMonth = Convert.ToInt32((((Int32)events.Last().Mileage - (Int32)events.First().Mileage) / countOfDays) * 30);
                        mileageInMonthSum += avgMileageOnMonth;

                        model.EventsYears = model.EventsYears.Concat((from ev in events
                                           select ev.Date.Year)
                                            .Distinct()
                                            .OrderByDescending(it => it)
                                            .Select(it => new SelectListItem()
                                            {
                                                Text = it.ToString(),
                                                Value = it.ToString()
                                            })
                                            .ToList()).Distinct().ToList();
                    }
                    var carsCount = model.BaseModel.LoginUserModel.UserCars.Count;
                    model.MileageInMonth = (mileageInMonthSum / carsCount).ToString();

                    model.GeneralCostsByCurrency
                                    .Add(sum.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("ru-ru")));

                    return View(model);
                }
                catch (Exception)
                {
                    return View("Message", new MessageWithRedirectModel("Невозможно отобразить страницу", "", "", null));
                }
            }
        }

        [HttpGet]
        public ActionResult GarageConsumptionStatistic()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                try
                {
                    var model = new GarageConsumptionStatisticModel();
                    model.BaseModel = DataHelper.GetBaseGarageModel(unitOfWork, GetAuthenticatedName());

                    return View(model);
                }
                catch (Exception)
                {
                    return View("Message", new MessageWithRedirectModel("Невозможно отобразить страницу", "", "", null));
                }
            }
        }
        
        /// <summary>
        /// Возвращает имя пользователя из куков
        /// </summary>
        /// <returns></returns>
        private String GetAuthenticatedName()
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                return User.Identity.Name;
            }

            return null;
        }

        /// <summary>
        /// Возвращает все модели указанной марки
        /// </summary>
        /// <param name="markID"></param>
        /// <returns></returns>
        private List<SelectListItem> GetModelsListByMarkID(Int32 markID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				var car = unitOfWork.CreateRepo<CarMark>()
					.FirstOrDefault(it => it.CarMarkID == markID);

				if (car != null)
				{
					return (from carModel in car.CarModels
							select new SelectListItem()
							{
								Text = carModel.CarModelName,
								Value = carModel.CarModelID.ToString()
							}).ToList();
				}
				else
				{
					return null;
				}

            }
        }        

        /// <summary>
        /// Количество автомобилей, отображаемых на странице 
        /// </summary>
        private Int32 _pageCarsSize = 5;
    }
}
