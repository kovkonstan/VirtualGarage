using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Models;
using VirtualGarage.Logic;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Enums;
using System.Web.Routing;
using VirtualGarage.Helpers;

namespace VirtualGarage.Controllers
{
    
    public class CarController : Controller
    {
        /// <summary>
        /// Информация об автомобиле (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CarInfo(Int32 carID)
        {
            // Создание экземпляра UnitOfWork
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                // Создание модели и заполнение данными базовой модели
                CarInfoModel model = new CarInfoModel() 
                {
                    BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
                };

                // Проверка на возможность получения доступа к данному автомобилю
                if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));                    
                }

                model.BaseModel.PageName = "CarInfo";

                // Создание экземпляра репозитория
                var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                Car car = unitOfWork.CreateInterfacedRepo<ICarRepo>().Single(it => it.CarID == carID);

                try
                {
                    // Получение вью модели из модели данных с помощью автомаппера
                    CarMapper mapper = new CarMapper();
                    model.CarModel = mapper.GetCarModel(car);
                    model.BaseModel.CarName = model.CarModel.MarkName + " " +
                                        model.CarModel.ModelName + " " +
                                        model.CarModel.Year;

                }
                catch (Exception)
                {
                    return View("Message",new MessageWithRedirectModel("Невозможно отобразить страницу","","",null));
                }

                return View(model);
            }            
        }

        /// <summary>
        /// Список событий (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="eventTypeID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Events(Int32 carID, Int32? eventTypeID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                // Заполнение базовой модели для CarController
				EventsModel model = new EventsModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

				model.BaseModel.PageName = "Events";

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

                var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();

                var events = (from ev in eventRepo
                                    .Where(it => (it.CarID == carID &&
                                                (eventTypeID != null ? it.EventTypeID == eventTypeID : true)))
                                select new EventModel()
                                {
                                    CurrencyName = ev.Currency.CurrencyName,
                                    Date = ev.Date,
                                    EventTypeName = ev.EventType.EventTypeName,
                                    GeneralCost = ev.GeneralCost,
                                    Mileage = ev.Mileage,
                                    EventID = ev.EventID
                                }).ToList();
                model.Events = events;

                return View(model);
            }          
        }

        /// <summary>
        /// Статистика (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Statistics(Int32 carID, Int32? year)
        {
            
            Int32 yearOfMonthMileage = (year ?? DateTime.Today.Year);

            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                try
                {                
                    // Создаем модель и заполняем базовую модель данными
                    MileageStatisticsModel model = new MileageStatisticsModel()
                    {
                        BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
                    };

                    // Проверяем возможность получения доступа к авто
                    if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                    {
                        return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                    }




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

                

                    model.BaseModel.PageName = "Statistics";

                    // Создаем репозиторий для авто, получаем из него авто
                    // и проверяем на наличие авто
                    var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                    var car = carRepo.SingleOrDefault(it => it.CarID == carID);
                    if (car == null)
                    {
                        return View("Message",
                                            new MessageWithRedirectModel(
                                                "Автомобиль не найден",
                                                "",
                                                "",
                                                null));
                    }


                    model.GeneralCostsByCurrency = new List<String>();

                    foreach (var item in unitOfWork.CreateRepo<Currency>().All())
                    {
                        Decimal sum = (Decimal)car.Events.Where(it => it.CurrencyID == item.CurrencyID)
                                    .Sum(it => it.GeneralCost);

                        if (sum != 0)
                        {
                            model.GeneralCostsByCurrency
                                .Add(sum.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture(item.CultureName)));
                        }
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
                    model.MileageInMonth = avgMileageOnMonth.ToString();

                    model.EventsYears = (from ev in events
                                         select ev.Date.Year)
                                         .Distinct()
                                         .OrderByDescending(it => it)
                                         .Select(it => new SelectListItem()
                                         {
                                             Text = it.ToString(),
                                             Value = it.ToString()
                                         })
                                         .ToList();
                
                    return View(model);
                }
                catch (Exception)
                {

                    return View("Message",
                                            new MessageWithRedirectModel(
                                                "Недостаточно данных для просмотра статистики",
                                                "",
                                                "",
                                                null));
                }
            }
        }

        /// <summary>
        /// Возвращает массив пробегов по месяцам
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult GetMonthMileages(Int32 carID, Int32? year)
        {
            return Json(DataHelper.GetMonthMileagesForJS(carID, year));  
        }

        /// <summary>
        /// Фильтрация статистики (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="eventTypeID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPartialStatistics(Int32 carID, Int32 eventTypeID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                StatisticsModel model = new StatisticsModel();

                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                var car = carRepo.SingleOrDefault(it => it.CarID == carID);

                if (car == null)
                {
                    return PartialView("MessageWithRedirect",
                                        new MessageWithRedirectModel(
                                            "Автомобиль не найден",
                                            "",
                                            "",
                                            null));
                }

                model.GeneralCostsByCurrency = new List<String>();

                foreach (var item in unitOfWork.CreateRepo<Currency>().All())
                {
                    Decimal sum = (Decimal)car.Events.Where(it => it.CurrencyID == item.CurrencyID 
                                                            && (eventTypeID == 0 || it.EventTypeID == eventTypeID))
                                .Sum(it => it.GeneralCost);

                    if (sum != 0)
                    {
                        model.GeneralCostsByCurrency
                            .Add(sum.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture(item.CultureName)));
                    }
                }

                try
                {
                    var events = car.Events;
                    Event firstEvent, lastEvent;
                    Boolean removeLast = true;
                    Boolean done = false;

                    events = events.OrderBy(it => it.Date)
                            .Where(it => it.Mileage != null && it.Mileage != 0).ToList();

                    do
                    {
                        firstEvent = events.First();
                        lastEvent = events.Last();

                        if (firstEvent.Mileage >= lastEvent.Mileage)
                        {
                            events.Remove((removeLast ? lastEvent : firstEvent));
                            removeLast = !removeLast;
                        }
                        else
                        {
                            done = true;
                        }

                    } while (!done);

                    Int32 countOfMonths = Convert.ToInt32((lastEvent.Date - firstEvent.Date).TotalDays / 30);
                    Int32 avgMileageOnMonth = Convert.ToInt32(((Int32)lastEvent.Mileage - (Int32)firstEvent.Mileage) / countOfMonths);

                    model.MileageInMonth = avgMileageOnMonth.ToString();
                }
                catch (Exception)
                { }

                return PartialView("StatisticsDiv", model);
            }
        }

        /// <summary>
        /// Статистика расхода топлива (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Consumption(Int32 carID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				ConsumptionModel model = new ConsumptionModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

				model.BaseModel.PageName = "Consumption";

                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                var car = carRepo.SingleOrDefault(it => it.CarID == carID);

                if (car == null)
                {
                    return View("Message",
                                        new MessageWithRedirectModel(
                                            "Автомобиль не найден",
                                            "",
                                            "",
                                            null));
                }

                model.Consumptions = new List<ConsumptionElementModel>();



                foreach (var fuelItem in car.Events.Where(it => it.EventTypeID == 2)
                                        .Select(it => it.Fillings.First().FuelType).Distinct())
                {

                    ConsumptionElementModel consumption = new ConsumptionElementModel();
                    consumption.ConsumptionName = fuelItem.FuelTypeName + " (" + fuelItem.FuelMeasure + ")";
                    consumption.ConsumtionUnit = new Dictionary<string, double>();

                    var eventsByFuel = car.Events.Where(it =>it.EventTypeID == 2 && it.Fillings.First().FuelType == fuelItem);

                    foreach (var yearItem in eventsByFuel.Select(it => it.Date.Year).Distinct())
	                {
                        var eventsByYear = eventsByFuel.Where(it => it.Date.Year == yearItem);

                        foreach (var monthItem in eventsByYear.Select(it => it.Date.Month).Distinct())
                        {
                            Double fuelCountByMonth = (Double)eventsByYear.Where(it => it.Date.Month == monthItem)
                                                            .Sum(it => it.Fillings.First().FuelCount);

                            if (fuelCountByMonth != 0)
                            {


                                String monthName = "";
                                switch (monthItem)
                                {
                                    case 1:
                                        monthName = "Январь";
                                        break;
                                    case 2:
                                        monthName = "Февраль";
                                        break;
                                    case 3:
                                        monthName = "Март";
                                        break;
                                    case 4:
                                        monthName = "Апрель";
                                        break;
                                    case 5:
                                        monthName = "Май";
                                        break;
                                    case 6:
                                        monthName = "Июнь";
                                        break;
                                    case 7:
                                        monthName = "Июль";
                                        break;
                                    case 8:
                                        monthName = "Август";
                                        break;
                                    case 9:
                                        monthName = "Сентябрь";
                                        break;
                                    case 10:
                                        monthName = "Октябрь";
                                        break;
                                    case 11:
                                        monthName = "Ноябрь";
                                        break;
                                    case 12:
                                        monthName = "Декабрь";
                                        break;
                                    default:
                                        break;
                                }

                                consumption.ConsumtionUnit.Add(monthName + " " + yearItem.ToString(), fuelCountByMonth);
                            }
                                                        
                        }                        
	                }

                    model.Consumptions.Add(consumption);
                }               

                return View(model);
            }           
        }

        /// <summary>
        /// Стоимость авто из яндекс-маркета (to do)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CarCost(Int32 carID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				CarCostModel model = new CarCostModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

				model.BaseModel.PageName = "CarCost";

                return View(model);
            }              
        }

        /// <summary>
        /// Фото (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Photo(Int32 carID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				PhotoModel model = new PhotoModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

				model.BaseModel.PageName = "Photo";

                var car = unitOfWork.CreateInterfacedRepo<ICarRepo>().SingleOrDefault(it => it.CarID == carID);

                if (car == null)
	            {
                    return View("Message", new MessageWithRedirectModel("Автомобиль не найден", "", "", null));		 
	            }

                model.ImageType = car.ImageType;
                return View(model);
            }
        }

        /// <summary>
        /// Фото (post)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Photo(PhotoModel model, HttpPostedFileBase photo)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

				if (carRepo.CheckUserAcces(model.BaseModel.CarID, GetAuthenticatedName()) != UserAccesOnCar.Own)
				{
					return View("Message", new MessageWithRedirectModel("Недостаточно прав для выполнения операции", "", "", null));	   										
				}

                if (photo != null && photo.ContentLength > 1024000)
                {
                    ModelState.AddModelError("", "Размер изображения должен быть меньше 1 мегабайта");
                }
                else
                {
					Car car = carRepo
                                .SingleOrDefault(it => it.CarID == model.BaseModel.CarID);

                    if (car == null)
                    {
                        return View("Message", new MessageWithRedirectModel("Автомобиль не найден", "", "", null));	                        
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
                        return RedirectToAction("Photo", "Car", 
                                                new RouteValueDictionary() { {"carID", model.BaseModel.CarID } });
                    }
                }
                return View(model);
            }
        }

        /// <summary>
        /// Удалить фото (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePhoto(PhotoModel model)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

                UserAccesOnCar acces = carRepo.CheckUserAcces(model.BaseModel.CarID, GetAuthenticatedName());

                if (acces != UserAccesOnCar.Own)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

                Car car = carRepo.SingleOrDefault(it => it.CarID == model.BaseModel.CarID);

                if (car == null)
                {
                    return View("Message", new MessageWithRedirectModel("Автомобиль не найден", "", "", null));
                }

                car.ImageType = null;
                car.CarPhoto = null;

                String error;
                if ((error = unitOfWork.SaveAndGetError()) != null)
                {
                    ModelState.AddModelError("", error);
                }
                else
                {
                    return RedirectToAction("Photo", "Car",
                                            new RouteValueDictionary() { { "carID", model.BaseModel.CarID } });
                }
                
                return View(model);
            }
        }

        /// <summary>
        /// Загрузка авто (post)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        public FileContentResult GetImage(Int32 carID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                Car car = (unitOfWork.CreateInterfacedRepo<ICarRepo>()).FirstOrDefault(it => it.CarID == carID);

                if (car.CarPhoto != null)
                {
                    return File(car.CarPhoto, car.ImageType);
                }
            }
            return null;
        }
        
        /// <summary>
        /// Добавить событие (get)
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult AddEvent(Int32 carID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				AddEventModel model = DataHelper.GetAddEventModel(unitOfWork, GetAuthenticatedName(), carID);

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
                {
                    return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
                }

                return View(model);
            }
        }

        /// <summary>
        /// Добавить событие (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult AddEvent(AddEventDivModel model)
        {             
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("AddEventDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");

						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName()); 
						return PartialView("AddEventDiv", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddEventDiv", model);
					}

                    var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                    var car = carRepo.FirstOrDefault(it => it.CarID == model.CarID);

                    if (car == null)
                    {
						return PartialView("MessageWithRedirect",
									new MessageWithRedirectModel(
										"Автомобиль не найден",
										"",
										"",
										null));	
                    }

                    if ((carRepo.CheckUserAcces(model.CarID, GetAuthenticatedName()) != UserAccesOnCar.Own &&
                        carRepo.CheckUserAcces(model.CarID, GetAuthenticatedName()) != UserAccesOnCar.Manage) ||
                        car.CarIsReadOnly)
                    {
						return PartialView("MessageWithRedirect",
                                    new MessageWithRedirectModel(
                                        "У вас не хватает прав для выполнения данной операции",
                                        "",
                                        "",
                                        null));

                    }

                    var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();

                    Event ev = new Event();

                    try
                    {
                        Int32 userID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                        .GetByLogin(GetAuthenticatedName()).UserID;

                        ev.CarID = model.CarID;
                        ev.CurrencyID = model.CurrencyID;
                        ev.Date = model.Date;
                        ev.EventComments = model.EventComments;
                        ev.EventTypeID = model.EventTypeID;
                        ev.GeneralCost = model.GeneralCost;
                        ev.Mileage = model.Mileage;                        
                        ev.UserID = userID;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = false,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };                        
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;                                    
                        }

                        eventRepo.Add(ev);

                        if (model.IsAddReminderByDays || model.IsAddReminderByMileage)
                        {
                            var reminderRepo = unitOfWork.CreateRepo<Reminder>();

                            Int32? countOfDaysByMileage = null;
                            try
                            {
                                countOfDaysByMileage = (Int32)(model.CountOfMileageForRemind / (car.MonthMileage / 30));
                            }
                            catch (Exception)
                            { }
                              
                            Int32 countOfDaysForRemind = 0;
                            if (model.CountOfDaysForRemind != null)
	                        {
                                countOfDaysForRemind = ((countOfDaysByMileage != null && 
                                                          countOfDaysByMileage < model.CountOfDaysForRemind) ?
                                                          (Int32)countOfDaysByMileage : (Int32)model.CountOfDaysForRemind);		 
	                        }
                            else
                            {
                                countOfDaysForRemind = (Int32)countOfDaysByMileage;
                            }							

                            Reminder reminder = new Reminder()
                            {
                                CarID = model.CarID,
                                StartDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind),
								FinishDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind)
                                                            .AddHours(23).AddMinutes(59),
                                IsRemindByEmail = true,
                                IsReminderDone = false,
                                ReminderCountOfDays = 1,
                                ReminderName = unitOfWork.CreateRepo<EventType>()
                                                        .Single(it => it.EventTypeID == model.EventTypeID).EventTypeName,
                                UserID = userID
                            };

                            reminderRepo.Add(reminder);
                        }

                        String error;
                        if ((error = unitOfWork.SaveAndGetError()) != null)
                        {
                            ModelState.AddModelError("", error);
                        }
                        else
                        {
                            return PartialView("MessageWithRedirect",
                                        new MessageWithRedirectModel("Событие добавлено", "Events", "Car",
                                                                        new RouteValueDictionary() 
                                                                        {
                                                                            {"carID", model.CarID}
                                                                        }));
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddEventDiv", model);
                    } 					
                }

				model = DataHelper.GetAddEventDivModel(model, unitOfWork,  GetAuthenticatedName());
				return PartialView("AddEventDiv", model);	
		
			}			
        }

        /// <summary>
        /// Добавить событие Заправка (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult AddFilling(AddFillingDivModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("AddEventDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");
						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddFillingDiv", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddFillingDiv", model);
					}

					var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
					Event ev = new Event();

					try
					{
						Int32 userID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
										.GetByLogin(GetAuthenticatedName()).UserID;

						ev.CarID = model.CarID;
						ev.CurrencyID = model.CurrencyID;
						ev.Date = model.Date;
						ev.EventComments = model.EventComments;
						ev.EventTypeID = model.EventTypeID;
						ev.GeneralCost = model.GeneralCost;
						ev.Mileage = model.Mileage;                        
						ev.UserID = userID;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = true,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;
                        }

						Filling filling = new Filling()
						{
							FuelCount = model.FuelCount,
							FuelTypeID = model.FuelTypeID,
							IsForgotPreviousFilling = model.IsForgotPreviousFilling,
							IsFullTank = model.IsFullTank,
							UnitCost = model.UnitCost
						};

						ev.Fillings.Add(filling);

						eventRepo.Add(ev);

						if (model.IsAddReminderByDays || model.IsAddReminderByMileage)
						{
							var reminderRepo = unitOfWork.CreateRepo<Reminder>();

                            Int32? countOfDaysByMileage = null;
                            try
                            {
                                countOfDaysByMileage = (Int32)(model.CountOfMileageForRemind / (ev.Car.MonthMileage / 30));
                            }
                            catch (Exception)
                            { }

                            Int32 countOfDaysForRemind = ((countOfDaysByMileage != null &&
                                                          countOfDaysByMileage < model.CountOfDaysForRemind) ?
                                                          (Int32)countOfDaysByMileage : (Int32)model.CountOfDaysForRemind);

							Reminder reminder = new Reminder()
							{
								CarID = model.CarID,
								StartDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind),
								FinishDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind)
															.AddHours(23).AddMinutes(59),
								IsRemindByEmail = true,
								IsReminderDone = false,
								ReminderCountOfDays = 1,
								ReminderName = unitOfWork.CreateRepo<EventType>()
														.Single(it => it.EventTypeID == model.EventTypeID).EventTypeName,
								UserID = userID
							};

							reminderRepo.Add(reminder);
						}

						String error;
						if ((error = unitOfWork.SaveAndGetError()) != null)
						{
							ModelState.AddModelError("", error);
						}
						else
						{
							return PartialView("MessageWithRedirect",
										new MessageWithRedirectModel("Событие добавлено", "Events", "Car",
																		new RouteValueDictionary() 
                                                                        {
                                                                            {"carID", model.CarID}
                                                                        }));
						}
					}
					catch (Exception)
					{
						ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddFillingDiv", model);
					}
				}

				model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
				return PartialView("AddFillingDiv", model);
			}											   			
		}

        /// <summary>
        /// Добавить событие Ремонт (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult AddRepair(AddRepairDivModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("AddEventDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");
						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddRepairDivModel", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddRepairDivModel", model);
					}

					var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
					Event ev = new Event();

					try
					{
						Int32 userID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
										.GetByLogin(GetAuthenticatedName()).UserID;

						ev.CarID = model.CarID;
						ev.CurrencyID = model.CurrencyID;
						ev.Date = model.Date;
						ev.EventComments = model.EventComments;
						ev.EventTypeID = model.EventTypeID;
						ev.GeneralCost = model.GeneralCost;
						ev.Mileage = model.Mileage;                        
						ev.UserID = userID;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = false,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;
                        }

						Boolean isSparePartAdded = false;  // Добавлена ли хоть одна деталь

						foreach (var repair in model.SpareParts)
						{
							if (repair.SparePartTypeID != null && repair.SparePartTypeID != 0)
							{
								isSparePartAdded = true;
								ev.Repairs.Add(new Repair()
								{
									IsRepair = repair.IsRepair,
									SparePartCost = repair.SparePartCost,
									SparePartModel = repair.Model,
									SparePartProducer = repair.Producer,
									SparePartTypeID = Convert.ToInt32(repair.SparePartTypeID),
									WorkCost = repair.WorkCost
								});
							}
						}

						if (!isSparePartAdded)
						{
							ModelState.AddModelError("", "Для события \"Ремонт\" нужно добавить хотя бы одну деталь");
							model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
							return PartialView("AddRepairDivModel", model);

						}

						eventRepo.Add(ev);

						if (model.IsAddReminderByDays || model.IsAddReminderByMileage)
						{
							var reminderRepo = unitOfWork.CreateRepo<Reminder>();

                            Int32? countOfDaysByMileage = null;
                            try
                            {
                                countOfDaysByMileage = (Int32)(model.CountOfMileageForRemind / (ev.Car.MonthMileage / 30));
                            }
                            catch (Exception)
                            { }

                            Int32 countOfDaysForRemind = ((countOfDaysByMileage != null &&
                                                          countOfDaysByMileage < model.CountOfDaysForRemind) ?
                                                          (Int32)countOfDaysByMileage : (Int32)model.CountOfDaysForRemind);

							Reminder reminder = new Reminder()
							{
								CarID = model.CarID,
								StartDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind),
								FinishDateTime = DateTime.Now.Date.AddDays((Double)countOfDaysForRemind)
															.AddHours(23).AddMinutes(59),
								IsRemindByEmail = true,
								IsReminderDone = false,
								ReminderCountOfDays = 1,
								ReminderName = unitOfWork.CreateRepo<EventType>()
														.Single(it => it.EventTypeID == model.EventTypeID).EventTypeName,
								UserID = userID
							};

							reminderRepo.Add(reminder);
						}

						String error;
						if ((error = unitOfWork.SaveAndGetError()) != null)
						{
							ModelState.AddModelError("", error);
						}
						else
						{
							return PartialView("MessageWithRedirect",
										new MessageWithRedirectModel("Событие добавлено", "Events", "Car",
																		new RouteValueDictionary() 
                                                                        {
                                                                            {"carID", model.CarID}
                                                                        }));
						}
					}
					catch (Exception)
					{
						ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("AddRepairDivModel", model);
					}
				}

				model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
				return PartialView("AddRepairDivModel", model);
			}												   
		}

        /// <summary>
        /// Редактировать событие (get)
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult EditEvent(Int32 eventID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
				var ev = eventRepo.FirstOrDefault(it => it.EventID == eventID);

				if (ev == null)
				{
					return View("Message", new MessageWithRedirectModel("Событие не найдено", "", "", null));							 
				}

				EditEventModel model = DataHelper.GetEditEventModel(unitOfWork, GetAuthenticatedName(), ev.CarID);

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
				{
					return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
				}

				model.EventTypeName = ev.EventType.EventTypeName;
				model.EventID = eventID;

				return View(model);

			}


		}

        /// <summary>
        /// Редактировать событие (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult EditEvent(AddEventDivModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("EditEventDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");

						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditEventDiv", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditEventDiv", model);
					}

					var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
					Event ev = eventRepo.FirstOrDefault(it => it.EventID == model.EventID);

					if (ev == null)
					{
						return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Событие не найдено", "", "", null));							
					}

					try
					{
						ev.CurrencyID = model.CurrencyID;
						ev.Date = model.Date;
						ev.EventComments = model.EventComments;
						ev.GeneralCost = model.GeneralCost;
						ev.Mileage = model.Mileage;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = false,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;
                        }

						String error;
						if ((error = unitOfWork.SaveAndGetError()) != null)
						{
							ModelState.AddModelError("", error);
						}
						else
						{
							return PartialView("MessageWithRedirect",
										new MessageWithRedirectModel("Изменения сохранены", "EventInfo", "Car",
																		new RouteValueDictionary() 
                                                                        {
                                                                            {"eventID", model.EventID}
                                                                        }));
						}
					}
					catch (Exception)
					{
						ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());	
						return PartialView("EditEventDiv", model);
					}
				}

				model = DataHelper.GetAddEventDivModel(model, unitOfWork, GetAuthenticatedName());
				return PartialView("EditEventDiv", model);
			}
		}

        /// <summary>
        /// Редактировать событие Запрвка (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult EditFilling(AddFillingDivModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("EditFillingDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");

						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditFillingDiv", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditFillingDiv", model);
					}

				
					var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
					Event ev = eventRepo.FirstOrDefault(it => it.EventID == model.EventID);

					if (ev == null)
					{
						return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Событие не найдено", "", "", null));
					}

					try
					{
						ev.CurrencyID = model.CurrencyID;
						ev.Date = model.Date;
						ev.EventComments = model.EventComments;
						ev.GeneralCost = model.GeneralCost;
						ev.Mileage = model.Mileage;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = true,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;
                        }

						Filling filling = ev.Fillings.First();

						filling.FuelCount = model.FuelCount;
						filling.FuelTypeID = model.FuelTypeID;
						filling.IsForgotPreviousFilling = model.IsForgotPreviousFilling;
						filling.IsFullTank = model.IsFullTank;
						filling.UnitCost = model.UnitCost;

						String error;
						if ((error = unitOfWork.SaveAndGetError()) != null)
						{
							ModelState.AddModelError("", error);
						}
						else
						{
							return PartialView("MessageWithRedirect",
										new MessageWithRedirectModel("Изменения сохранены", "EventInfo", "Car",
																		new RouteValueDictionary() 
                                                                        {
                                                                            {"eventID", model.EventID}
                                                                        }));
						}
					}
					catch (Exception)
					{
						ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditFillingDiv", model);
					}
				}

				model = DataHelper.GetAddFillingDivModel(model, unitOfWork, GetAuthenticatedName());
				return PartialView("EditFillingDiv", model);
			}
		}

        /// <summary>
        /// Редактировать событие Ремонт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult EditRepair(AddRepairDivModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
                    if (model.PlaceID == null && String.IsNullOrEmpty(model.NewPlaceName))
                    {
                        ModelState.AddModelError("PlaceID", "Выберите место проведения или введите свое");
                        model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
                        return PartialView("EditRepairDiv", model);
                    }

					if (model.IsAddReminderByDays && model.CountOfDaysForRemind == null)
					{
						ModelState.AddModelError("CountOfDaysForRemind", "Введите значение");

						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditRepairDiv", model);
					}

					if (model.IsAddReminderByMileage && model.CountOfMileageForRemind == null)
					{
						ModelState.AddModelError("CountOfMileageForRemind", "Введите значение");
						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditRepairDiv", model);
					}

					var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
					Event ev = eventRepo.Single(it => it.EventID == model.EventID);

					if (ev == null)
					{
						return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Событие не найдено", "", "", null));
					}

					try
					{
						ev.CurrencyID = model.CurrencyID;
						ev.Date = model.Date;
						ev.EventComments = model.EventComments;
						ev.GeneralCost = model.GeneralCost;
						ev.Mileage = model.Mileage;

                        if (!String.IsNullOrEmpty(model.NewPlaceName))
                        {
                            ev.Place = new Place()
                            {
                                IsFillingStation = false,
                                PlaceName = model.NewPlaceName,
                                UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                        .GetByLogin(GetAuthenticatedName())
                                                        .UserID
                            };
                        }
                        else
                        {
                            ev.PlaceID = (Int32)model.PlaceID;
                        }

						Boolean isSparePartAdded = false;  // Добавлена ли хоть одна деталь

						while (ev.Repairs.Count != 0)
						{
							unitOfWork.CreateRepo<Repair>().Delete(ev.Repairs.First());
						}

						unitOfWork.SaveAndGetError();

						foreach (var repair in model.SpareParts)
						{
							if (repair.SparePartTypeID != null && repair.SparePartTypeID != 0)
							{
								isSparePartAdded = true;
								ev.Repairs.Add(new Repair()
								{
									IsRepair = repair.IsRepair,
									SparePartCost = repair.SparePartCost,
									SparePartModel = repair.Model,
									SparePartProducer = repair.Producer,
									SparePartTypeID = Convert.ToInt32(repair.SparePartTypeID),
									WorkCost = repair.WorkCost
								});
							}
						}

						if (!isSparePartAdded)
						{
							ModelState.AddModelError("", "Для события \"Ремонт\" нужно добавить хотя бы одну деталь");
							model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
							return PartialView("EditRepairDiv", model);

						}

						String error;
						if ((error = unitOfWork.SaveAndGetError()) != null)
						{
							ModelState.AddModelError("", error);
						}
						else
						{
							return PartialView("MessageWithRedirect",
										new MessageWithRedirectModel("Изменения сохранены", "EventInfo", "Car",
																		new RouteValueDictionary() 
                                                                        {
                                                                            {"eventID", model.EventID}
                                                                        }));
						}
					}
					catch (Exception)
					{
						ModelState.AddModelError("", "Введены некорректные данные. Проверьте введенные данные еще раз");
						model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
						return PartialView("EditRepairDiv", model);
					}
				}

				model = DataHelper.GetAddRepairDivModel(model, unitOfWork, GetAuthenticatedName());
				return PartialView("EditRepairDiv", model);
			} 			
		}

        /// <summary>
        /// Удалить событие (post)
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult DeleteEvent(Int32 eventID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
				var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
				Event ev = eventRepo.FirstOrDefault(it => it.EventID == eventID);

				if (ev == null)
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Событие не найдено", "", "", null));
				}

				Car car = ev.Car;
				UserAccesOnCar acces = carRepo.CheckUserAcces(car.CarID, GetAuthenticatedName());

				if (acces == UserAccesOnCar.Own || acces == UserAccesOnCar.Manage)
				{
					// Удалить событие
					eventRepo.Delete(ev);

					String error;
					if ((error = unitOfWork.SaveAndGetError()) != null)
					{
						return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Не удалось удалить событие",
																			"Events",
																			"Car",
																			new RouteValueDictionary() { { "carID", car.CarID } }));
					}
					else
					{
						return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Событие успешно удалено",
																			"Events",
																			"Car",
																			new RouteValueDictionary() { { "carID", car.CarID } }));
					}
				}
				else
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("У Вас недостаточно прав на совершение данной операции",
																			"Events",
																			"Car",
																			new RouteValueDictionary() { { "carID", car.CarID } }));
				}
			}
		}

        /// <summary>
        /// Информация о событии
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
		[HttpGet]
		public ActionResult EventInfo(Int32 eventID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
				var ev = eventRepo.SingleOrDefault(it => it.EventID == eventID);
				if (ev == null)
				{
					return View("Message", new MessageWithRedirectModel("Событие не найдено",
														"CarsInGarage",
														"Garage",
														null));
				}

				EventInfoModel model = new EventInfoModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), ev.CarID)
				};

				if (model.BaseModel.UserAcces == UserAccesOnCar.Close)
				{
					return View("Message", new MessageWithRedirectModel("Доступ запрещен", "", "", null));
				}

				model.EventID = eventID;							

				return View(model);
			}
		}

        /// <summary>
        /// Возвращает частичное представление для отображения информации о событии
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult GetEventDiv(Int32 eventID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
				Int32 eventTypeID = eventRepo.Single(it => it.EventID == eventID).EventTypeID;

				EventTypeEnum type;
				Enum.TryParse(Enum.GetName(typeof(EventTypeEnum), eventTypeID), out type);

				switch (type)
				{
					case EventTypeEnum.Repair:		   // ремонт   
						var repair = eventRepo.Single(it => it.EventID == eventID);
						RepairModel repairModel = new RepairModel()
						{
							CurrencyName = repair.Currency.CurrencyName,
							CurrencyAbbreviation = repair.Currency.CultureName,
							Date = repair.Date,
							EventComments = repair.EventComments,
							EventID = eventID,
							EventTypeName = repair.EventType.EventTypeName,
							GeneralCost = repair.GeneralCost,
							Mileage = repair.Mileage,
							PlaceName = repair.Place.PlaceName,
							CarID = repair.CarID,
							SpareParts = (from sp in repair.Repairs
										  select new SparePartModel()
										  {
											  IsRepair = sp.IsRepair,
											  Model = sp.SparePartModel,
											  Producer = sp.SparePartProducer,
											  SparePartCost = sp.SparePartCost,
											  SparePartTypeName = sp.SparePartType.SparePartTypeName,
											  WorkCost = sp.WorkCost
										  }).ToList()
						};
						return PartialView("RepairInfoDiv", repairModel);
					case EventTypeEnum.Filling:		   // заправка
						var filling = eventRepo.Single(it => it.EventID == eventID);
						FillingModel fillingModel = new FillingModel()
						{
							CurrencyName = filling.Currency.CurrencyName,
							CurrencyAbbreviation = filling.Currency.CultureName,
							Date = filling.Date,
							EventComments = filling.EventComments,
							EventID = eventID,
							EventTypeName = filling.EventType.EventTypeName,
							GeneralCost = filling.GeneralCost,
							Mileage = filling.Mileage,
							PlaceName = filling.Place.PlaceName,
							CarID = filling.CarID,
							FuelTypeName = filling.Fillings.First().FuelType.FuelTypeName,
							FuelCount = filling.Fillings.First().FuelCount,
							UnitCost = filling.Fillings.First().UnitCost
						};
						return PartialView("FillingInfoDiv", fillingModel);
					default:
						var ev = eventRepo.Single(it => it.EventID == eventID);
						EventModel evModel = new EventModel()
						{
							CurrencyName = ev.Currency.CurrencyName,
							CurrencyAbbreviation = ev.Currency.CultureName,
							Date = ev.Date,
							EventComments = ev.EventComments,
							EventID = eventID,
							EventTypeName = ev.EventType.EventTypeName,
							GeneralCost = ev.GeneralCost,
							Mileage = ev.Mileage,
							PlaceName = ev.Place.PlaceName,
							CarID = ev.CarID
						};
						return PartialView("EventInfoDiv", evModel);
				}
			}
		}

        /// <summary>
        /// Доверить авто
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult TrustCar(Int32 carID)
		{	
			using(var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				TrustCarModel model = new TrustCarModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				return View(model);
			}
		}

        /// <summary>
        /// Доверить авто (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult TrustCar(TrustCarModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{ 
 					var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();									   
					UserAccesOnCar acces = carRepo.CheckUserAcces(model.BaseModel.CarID, GetAuthenticatedName());

					if (acces != UserAccesOnCar.Own)
					{
						return View("Message", new MessageWithRedirectModel("У Вас недостаточно прав для доверения авто", 
																			"", 
																			"", 
																			null));	 					
					}

					var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

					if (userRepo.IsEmailExist(model.Email))
					{
						var user = userRepo.Single(it => it.UserEmail == model.Email);

						var car = carRepo.FirstOrDefault(it => it.CarID == model.BaseModel.CarID);

						if (car == null)
						{
							return View("Message", new MessageWithRedirectModel("Автомобиль не найден", 
																			"", 
																			"", 
																			null));	 
		 
						}

						if (car.CarInGarages.Any(it => it.UserID == user.UserID))
						{
							ModelState.AddModelError("", "Этот автомобиль уже находится в гараже указанного пользователя");
						}
						else
						{
							car.CarInGarages.Add(new CarInGarage()
							{
								CarID = model.BaseModel.CarID,
								IsOwner = false,
								UserID = user.UserID
							});

							String error;
							if ((error = unitOfWork.SaveAndGetError()) == null)
							{
								return View("Message",
											new MessageWithRedirectModel
											("Процедура доверения прошла успешно",
											 "CarInfo",
											 "Car",
											 new RouteValueDictionary { { "carID", model.BaseModel.CarID } }));								
							}
							else
							{
								ModelState.AddModelError("", error);
							}
						}
					}
					else
					{
						ModelState.AddModelError("", "Пользователя с введенным E-mail'ом не существует");
					}
				}

				model.BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), model.BaseModel.CarID);
				return View(model);
			}
		}

        /// <summary>
        /// Передать авто
        /// </summary>
        /// <param name="carID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult TransmitCar(Int32 carID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				TransmitCarModel model = new TransmitCarModel()
				{
					BaseModel = DataHelper.GetBaseCarModel(unitOfWork, GetAuthenticatedName(), carID)
				};

				return View(model);
			}
		}

        /// <summary>
        /// Передать авто (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		[Authorize]
		public ActionResult TransmitCar(TransmitCarModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
					var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();									   
					UserAccesOnCar acces = carRepo.CheckUserAcces(model.BaseModel.CarID, GetAuthenticatedName());

					if (acces != UserAccesOnCar.Own)
					{
						return View("Message", new MessageWithRedirectModel("У Вас недостаточно прав для передачи авто", 
																			"", 
																			"", 
																			null));	 					
					} 

					var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

					if (userRepo.IsEmailExist(model.Email))
					{
						var recipient = userRepo.Single(it => it.UserEmail == model.Email);

						
						if (carRepo.CheckUserAcces(model.BaseModel.CarID, GetAuthenticatedName()) == UserAccesOnCar.Own)
						{
							var car = unitOfWork.CreateInterfacedRepo<ICarRepo>()
									.SingleOrDefault(it => it.CarID == model.BaseModel.CarID);

							Car transmitingCar = new Car();

							transmitingCar.BuyDate = car.BuyDate;
							transmitingCar.CarcaseTypeID = car.CarcaseTypeID;
							transmitingCar.CarModelID = car.CarModelID;

							if (!String.IsNullOrEmpty(car.ImageType))
							{
								transmitingCar.ImageType = car.ImageType;
								transmitingCar.CarPhoto = GetImage(car.CarID).FileContents;
							}

							transmitingCar.CarVisible = car.CarVisible;
							transmitingCar.ColorID = car.ColorID;
							transmitingCar.Comment = car.Comment;
							transmitingCar.CurrencyID = car.CurrencyID;
							transmitingCar.EngineVolume = car.EngineVolume;
							transmitingCar.FuelTypeID = car.FuelTypeID;
							transmitingCar.GearBoxID = car.GearBoxID;

							transmitingCar.Mileage = car.Mileage;
							transmitingCar.MonthMileage = car.MonthMileage;
							transmitingCar.Year = car.Year;

							transmitingCar.Events = new List<Event>();

                            for (int i = 0; i < car.Events.Count; i++)
                            {
                                var ev = car.Events.Skip(i).First();

                                var transmitingEvent = new Event()
                                                        {
                                                            //CarID = ev.CarID,
                                                            CurrencyID = ev.CurrencyID,
                                                            Date = ev.Date,
                                                            EventComments = ev.EventComments,
                                                            EventTypeID = ev.EventTypeID,
                                                            GeneralCost = ev.GeneralCost,
                                                            Mileage = ev.Mileage,
                                                            PlaceID = ev.PlaceID,
                                                            UserID = recipient.UserID                                                            
                                                        };

                                EventTypeEnum type;
                                Enum.TryParse(Enum.GetName(typeof(EventTypeEnum), ev.EventTypeID), out type);

                                if (type == EventTypeEnum.Filling)
                                {
                                    var fil = ev.Fillings.First();
                                    transmitingEvent.Fillings = new List<Filling>()
                                                                {
                                                                    new Filling()
                                                                    {
                                                                        FuelCount = fil.FuelCount,
                                                                        FuelTypeID = fil.FuelTypeID,
                                                                        IsForgotPreviousFilling = fil.IsForgotPreviousFilling,
                                                                        IsFullTank = fil.IsFullTank,
                                                                        UnitCost = fil.UnitCost                                                                        
                                                                    }
                                                                };
                                    
                                    
                                }
                                else if (type == EventTypeEnum.Repair)
                                {
                                    transmitingEvent.Repairs = new List<Repair>();
                                    foreach (var item in ev.Repairs)
                                    {
                                        transmitingEvent.Repairs.Add(new Repair()
                                                                    {
                                                                        IsRepair = item.IsRepair,
                                                                        SparePartCost = item.SparePartCost,
                                                                        SparePartModel = item.SparePartModel,
                                                                        SparePartProducer = item.SparePartProducer,
                                                                        SparePartTypeID = item.SparePartTypeID,
                                                                        WorkCost = item.WorkCost
                                                                    });                                                  
                                    }
                                    
                                }

                                transmitingCar.Events.Add(transmitingEvent);
                            } 

							transmitingCar.CarInGarages.Add(new CarInGarage()
							{
								UserID = recipient.UserID,
								IsOwner = true
							});

							carRepo.Add(transmitingCar);

							String error;		
							if ((error = unitOfWork.SaveAndGetError()) == null)
							{
								List<CarInGarage> carInGarages = car.CarInGarages.Where(it => !it.IsOwner).ToList();

								foreach (var item in carInGarages)
								{
									car.CarInGarages.Remove(item);
								}	
								
								car.CarIsReadOnly = true;
								unitOfWork.Save();

								return View("Message", new MessageWithRedirectModel("Автомобиль успешно передан",
																			"",
																			"",
																			null));	 

							}
							else
							{
								ModelState.AddModelError("", "Не удалось передать автомобиль");
							}
						}
					}
					else
					{
						ModelState.AddModelError("", "Пользователя с введенным E-mail'ом не существует");
					}
				}
				model.BaseModel = DataHelper.GetBaseCarModel(unitOfWork, 
																	GetAuthenticatedName(), 
																	model.BaseModel.CarID);
				

				return View(model);
			}  
		}

        /// <summary>
        /// Возвращает частичное представление для Добавления события
        /// </summary>
        /// <param name="eventTypeID"></param>
        /// <param name="carID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult GetAddEventPartialView(Int32 eventTypeID, Int32 carID)
		{
			EventTypeEnum type;
			Enum.TryParse(Enum.GetName(typeof(EventTypeEnum), eventTypeID), out type);

			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				switch (type)
				{
					case EventTypeEnum.Repair: // Ремонт
						var repModel = DataHelper.GetAddRepairDivModel(unitOfWork, eventTypeID, GetAuthenticatedName(), carID);
						return PartialView("AddRepairDiv", repModel);
					case EventTypeEnum.Filling: // Заправка
						var filModel = DataHelper.GetAddFillingDivModel(unitOfWork, eventTypeID, GetAuthenticatedName(), carID);
						return PartialView("AddFillingDiv", filModel); 
					default:
						var evModel = DataHelper.GetAddEventDivModel(unitOfWork, eventTypeID, GetAuthenticatedName(), carID);
						return PartialView("AddEventDiv", evModel); 
				}
			}
		}

        /// <summary>
        /// Возвращает частичное представление для Списка событий с фильтрацией
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="eventTypeID"></param>
        /// <param name="startDate"></param>
        /// <param name="finishDate"></param>
        /// <returns></returns>
		[HttpGet]
		public ActionResult GetEventsPartialView(Int32 carID, Int32? eventTypeID, String startDate, String finishDate)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				DateTime? stDate = null;
				DateTime? finDate = null;
				try
				{
					if (startDate != null && startDate != "")
					{
						stDate = Convert.ToDateTime(startDate);
					}
				}
				catch (Exception)
				{
					stDate = null;
				}

				try
				{
					if (finishDate != null && finishDate != "")
					{
						finDate = Convert.ToDateTime(finishDate);
					}
				}
				catch (Exception)
				{
					finDate = null;
				}

				if (stDate > finDate)
				{
					finDate = null;
				}

				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();

				var events = (from ev in eventRepo
									.Where(it => (it.CarID == carID &&
												(stDate != null ? it.Date >= stDate : true) &&
												(finDate != null ? it.Date <= finDate : true) &&
												((eventTypeID != null && eventTypeID != 0) ? it.EventTypeID == eventTypeID : true)))
									.OrderByDescending(it => it.Date)
							  select new EventModel()
							  {
								  CurrencyName = ev.Currency.CurrencyName,
								  Date = ev.Date,
								  EventTypeName = ev.EventType.EventTypeName,
								  GeneralCost = ev.GeneralCost,
								  Mileage = ev.Mileage,
								  EventID = ev.EventID,
								  CurrencyAbbreviation = ev.Currency.CultureName

							  }).ToList();


				return PartialView("EventsDiv", events);
			}
		}

        /// <summary>
        /// Возвращает частичное представление для Редактирования события
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
		[HttpGet]
		[Authorize]
		public ActionResult GetEditEventPartialView(Int32 eventID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var eventRepo = unitOfWork.CreateInterfacedRepo<IEventRepo>();
				var ev = eventRepo.Single(it => it.EventID == eventID);

				EventTypeEnum type;
				Enum.TryParse(Enum.GetName(typeof(EventTypeEnum), ev.EventTypeID), out type);

				switch (type)
				{
					case EventTypeEnum.Repair: // Ремонт
						var repModel = new AddRepairDivModel()
						{

							CarID = ev.CarID,
							CurrencyID = ev.CurrencyID,
							Date = ev.Date,
							EventComments = ev.EventComments,
							EventID = eventID,
							EventTypeID = ev.EventTypeID,
							GeneralCost = ev.GeneralCost,
							Mileage = ev.Mileage,
							PlaceID = ev.PlaceID,
							UserID = ev.UserID,
							SpareParts = (from sp in ev.Repairs
										  select new SparePartModel()
										  {
											  IsRepair = sp.IsRepair,
											  Model = sp.SparePartModel,
											  Producer = sp.SparePartProducer,
											  SparePartCost = sp.SparePartCost,
											  SparePartTypeID = sp.SparePartTypeID,
											  WorkCost = sp.WorkCost
										  }).ToList()
						};

						repModel = DataHelper.GetAddRepairDivModel(repModel, 
																		unitOfWork, 
																		ev.EventTypeID, 
																		GetAuthenticatedName(),
																		ev.CarID);
						return PartialView("EditRepairDiv", repModel);
					case EventTypeEnum.Filling: // Заправка
						var filModel = new AddFillingDivModel()
						{
							CarID = ev.CarID,
							CurrencyID = ev.CurrencyID,
							Date = ev.Date,
							EventComments = ev.EventComments,
							EventID = eventID,
							EventTypeID = ev.EventTypeID,
							GeneralCost = ev.GeneralCost,
							Mileage = ev.Mileage,
							PlaceID = ev.PlaceID,
							UserID = ev.UserID,

							FuelCount = ev.Fillings.First().FuelCount,
							FuelTypeID = ev.Fillings.First().FuelTypeID,
							IsForgotPreviousFilling = ev.Fillings.First().IsForgotPreviousFilling,
							IsFullTank = ev.Fillings.First().IsFullTank,
							UnitCost = ev.Fillings.First().UnitCost
						};

						filModel = DataHelper.GetAddFillingDivModel(filModel,
												unitOfWork,
												ev.EventTypeID,
												GetAuthenticatedName(),
												ev.CarID);
						return PartialView("EditFillingDiv", filModel);
					default:
						var evModel = new AddEventDivModel()
						{
							CarID = ev.CarID,
							CurrencyID = ev.CurrencyID,
							Date = ev.Date,
							EventComments = ev.EventComments,
							EventID = eventID,
							EventTypeID = ev.EventTypeID,
							GeneralCost = ev.GeneralCost,
							Mileage = ev.Mileage,
							PlaceID = ev.PlaceID,
							UserID = ev.UserID,
						};

						evModel = DataHelper.GetAddEventDivModel(evModel,
												unitOfWork,
												ev.EventTypeID,
												GetAuthenticatedName(),
												ev.CarID);						
						return PartialView("EditEventDiv", evModel);
				}
			}
		}

        /// <summary>
        /// Возвращает частичное представление для Добавления новой запчасти
        /// на странице добавления Ремонта
        /// </summary>
        /// <param name="numberInPage">Номер добавляемой запчасти</param>
        /// <returns></returns>
		[HttpGet]
		public ActionResult GetSparePartDiv(Int32 numberInPage)
		{
			return PartialView("SparePartDiv", new SparePartModel(numberInPage));
		}	          

        /// <summary>
        /// Возвращает имя пользоваьеля
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
    }
}
