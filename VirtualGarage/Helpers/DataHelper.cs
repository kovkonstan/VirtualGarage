using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InostudioSolutions.Data;
using VirtualGarage.Models;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Logic.Enums;
using VirtualGarage.Logic;
using AutoMapper;
using VirtualGarage.Logic.DataModel;
using System.Web.Mvc;

namespace VirtualGarage.Helpers
{
    public static class DataHelper
    {
        public static LoginUserModel GetLoginUserModel(IUnitOfWork unitOfWork, String userName)
        {
            if (userName != null)
            {
                LoginUserModel result = new LoginUserModel();

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
                result.UserCars = cars.ToList();

                result.CountOfReminders = me.Reminders.Count(it => !it.IsReminderDone);
												//(
												//    ((it.StartDateTime - DateTime.Now).Days <= 0 && !it.IsReminderDone) ||

												//    ((it.StartDateTime - DateTime.Now).Days > 0 &&
												//        (it.StartDateTime - DateTime.Now).Days <= it.ReminderCountOfDays &&
												//        !it.IsReminderDone)
												//)
												//);
                return result;

                
            }

            return null;
        }

		#region Methods for CarController

		public static BaseCarModel GetBaseCarModel(IUnitOfWork unitOfWork, String userName, Int32 carID)
        {
            BaseCarModel result = new BaseCarModel();

            result.CarID = carID;
            result.LoginUserModel = GetLoginUserModel(unitOfWork, userName);

            var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

            var car = carRepo.SingleOrDefault(it => it.CarID == carID);

            if (car != null)
            {
                result.UserAcces = carRepo.CheckUserAcces(carID, userName);

                if (result.UserAcces != UserAccesOnCar.Close)
                {
                    result.CarName = car.CarModel.CarMark.CarMarkName + " " +
                                        car.CarModel.CarModelName + " " +
                                        car.Year;

                    result.SimilarCars = (from simCar in carRepo.GetSimilarCars(carID)
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
                    return result;                        
                } 
            }
                

            // Автомобиль не найден или
            // авто закрыт
            result.UserAcces = UserAccesOnCar.Close;
            return result;            
        }


		public static AddEventModel GetAddEventModel(AddEventModel model, IUnitOfWork unitOfWork, String userName, Int32 carID)
		{
			model.BaseModel = GetBaseCarModel(unitOfWork, userName, carID);

			model.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
			{
				Text = eventType.EventTypeName,
				Value = eventType.EventTypeID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			return model;
		}

		public static AddEventModel GetAddEventModel(IUnitOfWork unitOfWork, String userName, Int32 carID)
		{
			return GetAddEventModel(new AddEventModel(), unitOfWork, userName, carID);
		}


		public static AddEventDivModel GetAddEventDivModel(AddEventDivModel model, IUnitOfWork unitOfWork, String userName)
		{

			model.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
			{
				Text = cur.CurrencyName,
				Value = cur.CurrencyID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllEventPlaces = unitOfWork.CreateRepo<Place>()
									.Where(it => it.User.UserNick == userName && it.IsFillingStation == false)
									.Select(pl => new
									{
										Text = pl.PlaceName,
										Value = pl.PlaceID
									}).ToList().Select(t => new SelectListItem()
									{
										Text = t.Text,
										Value = t.Value.ToString()
									}).ToList();

			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
			var car = carRepo.Single(it => it.CarID == model.CarID);

			model.IsKnownMileage = (car.MonthMileage != null && car.MonthMileage != 0);

			return model;
		}

		public static AddEventDivModel GetAddEventDivModel(AddEventDivModel model, IUnitOfWork unitOfWork, Int32 eventTypeID, String userName, Int32 carID)
		{

			model.CarID = carID;
			model.EventTypeID = eventTypeID;

			return GetAddEventDivModel(model, unitOfWork, userName);        
		}

		public static AddEventDivModel GetAddEventDivModel(IUnitOfWork unitOfWork, 
															Int32 eventTypeID, 
															String userName, 
															Int32 carID)
		{
			var model = new AddEventDivModel();			
			return GetAddEventDivModel(model, unitOfWork, eventTypeID, userName, carID);
		}


		public static AddFillingDivModel GetAddFillingDivModel(AddFillingDivModel model,
														IUnitOfWork unitOfWork,	 														
														String userName)
		{
			model.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
			{
				Text = cur.CurrencyName,
				Value = cur.CurrencyID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

            //var places = unitOfWork.CreateRepo<Place>().;
            //foreach (var item in places)
            //{
            //    if (item.User == null || item.User.UserNick == userName )
            //    {
                    
            //    }
                
            //}

			model.AllEventPlaces = unitOfWork.CreateRepo<Place>()
									.Where(it => it.IsFillingStation && 
                                                (it.User == null || it.User.UserNick == userName))
									.Select(pl => new
									{
										Text = pl.PlaceName,
										Value = pl.PlaceID
									}).ToList().Select(t => new SelectListItem()
									{
										Text = t.Text,
										Value = t.Value.ToString()
									}).ToList();

			model.AllFuelTypes = unitOfWork.CreateRepo<FuelType>()
									.Select(fType => new
									{
										Text = fType.FuelTypeName,
										Value = fType.FuelTypeID
									}).ToList().Select(t => new SelectListItem()
									{
										Text = t.Text,
										Value = t.Value.ToString()
									}).ToList();

			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
			var car = carRepo.Single(it => it.CarID == model.CarID);

			model.IsKnownMileage = (car.MonthMileage != null && car.MonthMileage != 0);

			return model;
		}

		public static AddFillingDivModel GetAddFillingDivModel(AddFillingDivModel model, 
																IUnitOfWork unitOfWork, 
																Int32 eventTypeID, 
																String userName, 
																Int32 carID)
		{
			model.CarID = carID;
			model.EventTypeID = eventTypeID;

			return GetAddFillingDivModel(model, unitOfWork, userName);
		}

		public static AddFillingDivModel GetAddFillingDivModel(IUnitOfWork unitOfWork, Int32 eventTypeID, String userName, Int32 carID)
		{
			var model = new AddFillingDivModel();
			return GetAddFillingDivModel(model, unitOfWork, eventTypeID, userName, carID);
		}


		public static AddRepairDivModel GetAddRepairDivModel(AddRepairDivModel model, 
															 IUnitOfWork unitOfWork, 
															 String userName)
		{
			model.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
			{
				Text = cur.CurrencyName,
				Value = cur.CurrencyID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllEventPlaces = unitOfWork.CreateRepo<Place>()
                                    .Where(it => it.User.UserNick == userName && it.IsFillingStation == false)
									.Select(pl => new
									{
										Text = pl.PlaceName,
										Value = pl.PlaceID
									}).ToList().Select(t => new SelectListItem()
									{
										Text = t.Text,
										Value = t.Value.ToString()
									}).ToList();

			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>(); 
			var car = carRepo.Single(it => it.CarID == model.CarID);

			model.IsKnownMileage = (car.MonthMileage != null && car.MonthMileage != 0);
			

			return model;
		}

		public static AddRepairDivModel GetAddRepairDivModel(AddRepairDivModel model, IUnitOfWork unitOfWork, Int32 eventTypeID, String userName, Int32 carID)
		{
			model.CarID = carID;
			model.EventTypeID = eventTypeID;

			return GetAddRepairDivModel(model, unitOfWork, userName);
		}

		public static AddRepairDivModel GetAddRepairDivModel(IUnitOfWork unitOfWork, Int32 eventTypeID, String userName, Int32 carID)
		{
			var model = new AddRepairDivModel();  
			return GetAddRepairDivModel(model, unitOfWork, eventTypeID, userName, carID);
		}

		public static EditEventModel GetEditEventModel(EditEventModel model, IUnitOfWork unitOfWork, String userName, Int32 carID)
		{
			model.BaseModel = GetBaseCarModel(unitOfWork, userName, carID);

			//model.AllEventTypes = unitOfWork.CreateRepo<EventType>().Select(eventType => new
			//{
			//    Text = eventType.EventTypeName,
			//    Value = eventType.EventTypeID
			//}).ToList().Select(t => new SelectListItem()
			//{
			//    Text = t.Text,
			//    Value = t.Value.ToString()
			//}).ToList();

			return model;
		}

		public static EditEventModel GetEditEventModel(IUnitOfWork unitOfWork, String userName, Int32 carID)
		{
			return GetEditEventModel(new EditEventModel(), unitOfWork, userName, carID);
		}

        public static List<Object> GetMonthMileagesForJS(Int32 carID, Int32? year)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                // Создаем репозиторий для авто, получаем из него авто
                // и проверяем на наличие авто
                var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();
                var car = carRepo.SingleOrDefault(it => it.CarID == carID);

                // Получаем события для данного автомобиля,
                // для которых определен текущий пробег, 
                // и сортируем их по дате
                var events = car.Events.Where(it => it.Mileage != null && it.Mileage != 0)
                    .OrderBy(it => it.Date).ToList();

                year = (year ?? (from ev in events
                                 select ev.Date.Year)
                                .Distinct()
                                .OrderByDescending(it => it)
                                .First());

                // ********** Определение среднего пробега по месяцам ***********

                // Находим первое событие года
                var firstYearEv = events.First(it => it.Date.Year == year);
                // Находим предыдущее событие
                var prevEvent = events.LastOrDefault(it => it.Date < firstYearEv.Date);
                // Определяем первый месяц года, с которого возможно определить статистику
                Int32 numOfFirstMonth = 0;
                Int32 startMonthMileage = 0; // Значение пробега на начало месяца

                if (prevEvent != null) // Если предыдущее событие существует
                {
                    numOfFirstMonth = 1; // Опеределяем статистику с января
                    // Определяем значение пробега на начало месяца
                    startMonthMileage = GetMileageOnStartMonth(prevEvent, firstYearEv, new DateTime((Int32)year, 1, 1));
                }
                else
                {
                    numOfFirstMonth = firstYearEv.Date.Month;
                    startMonthMileage = (Int32)firstYearEv.Mileage;
                }

                // Определяем последний месяц года, для которого возможно определить статистику
                Int32 numOfLastMonth = 0;
                numOfLastMonth = (events.Any(it => it.Date.Year > year) ? 12 : events.Last().Date.Month);

                List<Int32> monthMileages = new List<Int32>();  // Список значений среднего пробега в месяц

                // Проходим в цикле по всем месяцам года,
                // для которых возможно определить статистику
                for (int month = numOfFirstMonth; month <= numOfLastMonth; month++)
                {
                    var firEv = events.LastOrDefault(it => it.Date.Month == month && it.Date.Year == year);
                    if (firEv == null)
                    {
                        firEv = new Event()
                        {
                            Date = new DateTime((Int32)year, month, 1),
                            Mileage = startMonthMileage
                        };
                    }

                    var lastEv = events.FirstOrDefault(it => it.Date > firEv.Date);
                    if (lastEv == null)
                    {
                        monthMileages.Add((Int32)((firEv.Mileage - startMonthMileage)));
                    }
                    else
                    {
                        monthMileages.Add((Int32)(
                            (GetMileageOnStartMonth(firEv, lastEv, 
                                            new DateTime(
                                                (firEv.Date.Month == 12 ? firEv.Date.Year + 1 : firEv.Date.Year),
                                                (firEv.Date.Month == 12 ? 1 : firEv.Date.Month + 1),
                                                1))
                            - startMonthMileage)));
                        startMonthMileage = GetMileageOnStartMonth(firEv, lastEv,
                                                new DateTime(
                                                (firEv.Date.Month == 12 ? firEv.Date.Year + 1 : firEv.Date.Year),
                                                (firEv.Date.Month == 12 ? 1 : firEv.Date.Month + 1),
                                                1));
                    }
                }                 

                var objects = new List<Object>();

                Int32 currentMonth = numOfFirstMonth;
                foreach (var item in monthMileages)
                {
                    objects.Add(new
                    {
                        Month = GetMonthNameByNumber(currentMonth),
                        Value = item
                    });
                    currentMonth++;
                }

                return objects;
            }
        }

		#endregion



		#region Methods for DefaultController

		public static BaseDefaultModel GetBaseDefaultModel(IUnitOfWork unitOfWork, String userName)
		{
			BaseDefaultModel result = new BaseDefaultModel();  
			result.LoginUserModel = GetLoginUserModel(unitOfWork, userName);
			return result;
		}

		public static List<Object> GetRemindersForJS(String userName)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var me = unitOfWork.CreateInterfacedRepo<IUserRepo>().GetByLogin(userName);
				var reminders = me.Reminders.ToList();

				var objects = new List<Object>();

				for (int i = 0; i < reminders.Count; i++)
				{
					String stTime = reminders[i].StartDateTime.ToShortTimeString();
					String finTime = reminders[i].FinishDateTime.ToShortTimeString();

					objects.Add(new
					{
						Id = reminders[i].ReminderID,
						Title = reminders[i].ReminderName,
						StartDateTime = reminders[i].StartDateTime.ToString().Split('.', ':', ' '),
						FinishDateTime = reminders[i].FinishDateTime.ToString().Split('.', ':', ' '),
						AllDay = (stTime == "0:00" && finTime == "23:59" ? true : false),
						IsNeedRemind = !reminders[i].IsReminderDone
					});
				}

				return objects;
			}
		}
        
		public static SearchModel GetSearchModel(SearchModel model, IUnitOfWork unitOfWork, String userName)
		{
			model.BaseModel = GetBaseDefaultModel(unitOfWork, userName); 
			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

			model.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
			{
				Text = mark.CarMarkName,
				Value = mark.CarMarkID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllModels = new List<SelectListItem>();

			return model;
		}

		public static SearchModel GetSearchModel(IUnitOfWork unitOfWork, String userName)
		{
			SearchModel model = new SearchModel();		
			return GetSearchModel(model, unitOfWork, userName);
		}


		public static AddReminderModel GetAddReminderModel(AddReminderModel model, IUnitOfWork unitOfWork, String userName)
		{
			model.BaseModel = GetBaseDefaultModel(unitOfWork, userName);
			var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

			model.AllUserCars = (from car in userRepo.GetUserCars(userName)
								select new
								{
									CarID = car.CarID,
									Mark = car.CarModel.CarMark.CarMarkName,
									Model = car.CarModel.CarModelName,
									Year = car.Year
								}).ToList()
									.Select(it => new SelectListItem()
									{
										Text = it.Mark + " " + it.Model + " " + it.Year.ToString(),
										Value = it.CarID.ToString()

									}).ToList();  
			return model;
		}

		public static AddReminderModel GetAddReminderModel(IUnitOfWork unitOfWork, String userName)
		{
			AddReminderModel model = new AddReminderModel();
			return GetAddReminderModel(model, unitOfWork, userName);
		}


		public static EditReminderModel GetEditReminderModel(EditReminderModel model, IUnitOfWork unitOfWork, String userName)
		{
			model.BaseModel = GetBaseDefaultModel(unitOfWork, userName);
			var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

			model.AllUserCars = (from car in userRepo.GetUserCars(userName)
								 select new
								 {
									 CarID = car.CarID,
									 Mark = car.CarModel.CarMark.CarMarkName,
									 Model = car.CarModel.CarModelName,
									 Year = car.Year
								 }).ToList()
									.Select(it => new SelectListItem()
									{
										Text = it.Mark + " " + it.Model + " " + it.Year.ToString(),
										Value = it.CarID.ToString()

									}).ToList();
			return model;
		}

		public static EditReminderModel GetEditReminderModel(IUnitOfWork unitOfWork, String userName)
		{
			EditReminderModel model = new EditReminderModel();
			return GetEditReminderModel(model, unitOfWork, userName);
		}	 

		#endregion  		



		#region Methods for GarageController

		public static BaseGarageModel GetBaseGarageModel(IUnitOfWork unitOfWork, String userName)
		{
			BaseGarageModel result = new BaseGarageModel();
			result.LoginUserModel = GetLoginUserModel(unitOfWork, userName);
			return result;
		}


		public static AddCarModel GetAddCarModel(AddCarModel model, IUnitOfWork unitOfWork, String userName)
		{
			model.BaseModel = GetBaseGarageModel(unitOfWork, userName);

			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

			model.AllColors = unitOfWork.CreateRepo<Color>().Select(color => new
			{
				Text = color.ColorName,
				Value = color.ColorID,
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
			{
				Text = cur.CurrencyName,
				Value = cur.CurrencyID,
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllFuelTypes = unitOfWork.CreateRepo<FuelType>().Select(ftype => new
			{
				Text = ftype.FuelTypeName,
				Value = ftype.FuelTypeID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllGearBoxTypes = unitOfWork.CreateRepo<GearBoxType>().Select(gbtype => new
			{
				Text = gbtype.GearBoxName,
				Value = gbtype.GearBoxID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
			{
				Text = mark.CarMarkName,
				Value = mark.CarMarkID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllModels = new List<SelectListItem>();

			model.AllCarcaseTypes = unitOfWork.CreateRepo<CarcaseType>().Select(carctype => new
			{
				Text = carctype.CarcaseTypeName,
				Value = carctype.CarcaseTypeID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			return model;
		}

		public static AddCarModel GetAddCarModel(IUnitOfWork unitOfWork, String userName)
		{
			AddCarModel model = new AddCarModel();
			return GetAddCarModel(model, unitOfWork, userName);
		}


		public static EditCarModel GetEditCarModel(EditCarModel model, IUnitOfWork unitOfWork, String userName)
		{
			model.BaseModel = GetBaseGarageModel(unitOfWork, userName);

			var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

			model.AllColors = unitOfWork.CreateRepo<Color>().Select(color => new
			{
				Text = color.ColorName,
				Value = color.ColorID,
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllCurrences = unitOfWork.CreateRepo<Currency>().Select(cur => new
			{
				Text = cur.CurrencyName,
				Value = cur.CurrencyID,
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllFuelTypes = unitOfWork.CreateRepo<FuelType>().Select(ftype => new
			{
				Text = ftype.FuelTypeName,
				Value = ftype.FuelTypeID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllGearBoxTypes = unitOfWork.CreateRepo<GearBoxType>().Select(gbtype => new
			{
				Text = gbtype.GearBoxName,
				Value = gbtype.GearBoxID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllMarks = unitOfWork.CreateRepo<CarMark>().Select(mark => new
			{
				Text = mark.CarMarkName,
				Value = mark.CarMarkID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			model.AllModels = new List<SelectListItem>();

			model.AllCarcaseTypes = unitOfWork.CreateRepo<CarcaseType>().Select(carctype => new
			{
				Text = carctype.CarcaseTypeName,
				Value = carctype.CarcaseTypeID
			}).ToList().Select(t => new SelectListItem()
			{
				Text = t.Text,
				Value = t.Value.ToString()
			}).ToList();

			return model;
		}

		public static AddCarModel GetEditCarModel(IUnitOfWork unitOfWork, String userName)
		{
			EditCarModel model = new EditCarModel();
			return GetEditCarModel(model, unitOfWork, userName);
		}	 

		#endregion

        /// <summary>
        /// Возвращает значение пробега на указанную дату
        /// </summary>
        /// <param name="firstEvent"></param>
        /// <param name="lastEvent"></param>
        /// <returns></returns>
        private static Int32 GetMileageOnStartMonth(Event firstEvent, Event lastEvent, DateTime date)
        {
            // Определяем средний пробег в день
            Double mileagePerDay = (Double)((lastEvent.Mileage - firstEvent.Mileage) / (lastEvent.Date - firstEvent.Date).Days);
            // Получаем первое число месяца
            //DateTime firstDateOfMonth = new DateTime(
            //                (firstEvent.Date.Month == 12 ? firstEvent.Date.Year + 1 : firstEvent.Date.Year),
            //                (firstEvent.Date.Month == 12 ? 1 : firstEvent.Date.Month + 1),
            //                1);
            // Определяем количество дней до 1 числа
            Int32 countDaysTo1th = (date - firstEvent.Date).Days;
            // Определяем значение пробега на начало месяца
            return (Int32)(countDaysTo1th * mileagePerDay + firstEvent.Mileage);
        }

        private static String GetMonthNameByNumber(Int32 number)
        {
            String monthName = "";
            switch (number)
            {
                case 1:
                    monthName = "Янв";
                    break;
                case 2:
                    monthName = "Фев";
                    break;
                case 3:
                    monthName = "Мар";
                    break;
                case 4:
                    monthName = "Апр";
                    break;
                case 5:
                    monthName = "Май";
                    break;
                case 6:
                    monthName = "Июн";
                    break;
                case 7:
                    monthName = "Июл";
                    break;
                case 8:
                    monthName = "Авг";
                    break;
                case 9:
                    monthName = "Сен";
                    break;
                case 10:
                    monthName = "Окт";
                    break;
                case 11:
                    monthName = "Ноя";
                    break;
                case 12:
                    monthName = "Дек";
                    break;

            }

            return monthName;
        }
	}
}