using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualGarage.Logic.DataModel;
using InostudioSolutions.Data;
using VirtualGarage.Logic;
using VirtualGarage.Models;
using VirtualGarage.Logic.Repository;
using System.Web.Security;
using VirtualGarage.Helpers;

namespace VirtualGarage.Controllers
{
    public class DefaultController : Controller
    {
        /// <summary>
        /// Главная страница сайта
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
			using(var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{				  
				if (User.Identity.IsAuthenticated)
				{
					var model = new IndexModel();	  
					model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());

					return View(model);
		 
				}

				return View();
			}
        }

        /// <summary>
        /// Календарь
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Calendar()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{

				var model = new CalendarModel();
				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());

				return View(model);

			}

        }

        /// <summary>
        /// Возвращает напоминания пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult GetReminders()
        {
            CalendarModel mod = new CalendarModel();

			return Json(DataHelper.GetRemindersForJS(GetAuthenticatedName())); 
        }

        /// <summary>
        /// Информация о напоминании
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReminderInfo(Int32 reminderID)
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var reminderRepo = unitOfWork.CreateRepo<Reminder>();

                ReminderInfoMapper mapper = new ReminderInfoMapper();
                Reminder reminder = reminderRepo.SingleOrDefault(it => it.ReminderID == reminderID);

                ReminderInfoModel model = mapper.GetReminderInfoModel(reminder);
                model.CarName = reminder.Car.CarModel.CarMark.CarMarkName + " "
                                + reminder.Car.CarModel.CarModelName + " "
                                + reminder.Car.Year;

				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());
				return View(model);
            }					    
        }
        
        // Зарезервировано
        [HttpGet]
        [Authorize]
        public ActionResult IncomingMessages()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var model = new IncomingMessagesModel();
				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());
				model.PageName = "IncomingMessages";

				return View(model);
			}
        }

        // Зарезервировано
        [HttpGet]
        [Authorize]
        public ActionResult OutgoingMessages()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var model = new OutgoingMessagesModel();
				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());				
				model.PageName = "OutgoingMessages";

				return View(model);
			}
        }

        /// <summary>
        /// Настройки
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Settings()
        {
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                User user = userRepo.GetByLogin(GetAuthenticatedName());

                if (user != null)
                {
                    var model = new SettingsModel()
                    {
                        UserName = user.UserNick,
                        UserEmail = user.UserEmail
                    };

					model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());
                    return View(model);
                }
				else
				{
					return View("Message",
									new MessageWithRedirectModel("Невозможно отобразить страницу",
										"",
										"",
										null));
				}
            }					   			
        }

        /// <summary>
        /// Поиск
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var model = DataHelper.GetSearchModel(unitOfWork, GetAuthenticatedName());
				return View(model);
			}
        }

        /// <summary>
        /// Поиск (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		public ActionResult Search(SearchModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{
					if (model.LowYear == null || model.HighYear == null || model.LowYear <= model.HighYear)
					{
						var carRepo = unitOfWork.CreateInterfacedRepo<ICarRepo>();

						CarMapper mapper = new CarMapper();

						var result = (from car in carRepo.All()
									  where (car.CarVisible &&
                                          (model.MarkID == null || model.MarkID == 0 || car.CarModel.CarMark.CarMarkID == model.MarkID) &&
                                          (model.ModelID == null || model.ModelID == 0 || car.CarModelID == model.ModelID) &&
										  (model.LowYear != null ? car.Year >= model.LowYear : true) &&
										  (model.HighYear != null ? car.Year <= model.HighYear : true) &&
										  (model.LowMileage != null ? car.Mileage >= model.LowMileage : true) &&
										  (model.HighMileage != null ? car.Mileage <= model.HighMileage : true))
									  select car).ToList();
						model.Cars = (from rc in result
									  select mapper.GetCarModel(rc)).ToList();

						model = DataHelper.GetSearchModel(model, unitOfWork, GetAuthenticatedName());
						return View("SearchResult", model);
					}
					else
					{
						ModelState.AddModelError("", "Начальное значение года поиска должно быть меньше конечного");
					}
				}

				model = DataHelper.GetSearchModel(model, unitOfWork, GetAuthenticatedName());
				return View(model);
			}
		}

        /// <summary>
        /// Информация о пользователе
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserInfo(Int32 userID)
        {	
            using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
            {
                User user = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                .SingleOrDefault(it => it.UserID == userID);
                if (user != null)
                {
                    UserInfoModel model = new UserInfoModel()
                    {
                        UserID = user.UserID,
                        UserName = user.UserNick,
						BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName())
                    };
					                    
                    return View(model);
                }
            }

            return View("Message",
							new MessageWithRedirectModel("Невозможно отобразить страницу",
								"",
								"",
								null));	
		}

        /// <summary>
        /// Изменить e-mail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult ChangeEmail()
        {
			using(var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var model = new ChangeEmailModel()
				{
					BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName())
				};

				return View(model);
			}
        }

        /// <summary>
        /// Изменить e-mail (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{                
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    // Проверить, существует ли пользователь с таким e-mail'ом
                    if (userRepo.IsEmailExist(model.NewEmail))
                    {
                        ModelState.AddModelError("", "Пользователь с таким E-mail'ом уже существует. Выберите другой E-mail");
                    }
                    else
                    {
                        userRepo.GetByLogin(GetAuthenticatedName()).UserEmail = model.NewEmail;

                        String error = unitOfWork.SaveAndGetError();

                        if (error == null)
                        {
                            return RedirectToAction("Settings", "Default");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Проверьте еще раз Ваш E-mail");
                        }
                    }
                }

				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());				
				return View(model);
            }


        }

        /// <summary>
        /// Изменрить пароль
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var model = new ChangePasswordModel()
				{
					BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName())
				};

				return View(model);
			}
        }
         
        /// <summary>
        /// Изменрить пароль (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{                
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    if (userRepo.GetByLogin(GetAuthenticatedName()).UserPassword == model.OldPassword)
                    {
                        userRepo.GetByLogin(GetAuthenticatedName())
                            .UserPassword = model.NewPassword;

                        String error = unitOfWork.SaveAndGetError();
                        if (error != null)
                        {
                            ModelState.AddModelError("", "Попробуйте еще раз. При повторном " +
                                "появлении ошибки обратитесь к администратору");
                        }
                        else
                        {
                            return RedirectToAction("Settings", "Default");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильно введен старый пароль");
                    }
                }

				model.NewPassword = String.Empty;
				model.ConfirmPassword = String.Empty;

				model.BaseModel = DataHelper.GetBaseDefaultModel(unitOfWork, GetAuthenticatedName());
				return View(model);
            }
        }

        /// <summary>
        /// Добавить напоминание
        /// </summary>
        /// <param name="stTime"></param>
        /// <param name="finTime"></param>
        /// <param name="allDay"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult AddReminder(String stTime, String finTime, Boolean allDay)
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{  
				AddReminderModel model = new AddReminderModel();
				model = DataHelper.GetAddReminderModel(unitOfWork, GetAuthenticatedName());

				var stArr = stTime.Split(' ');
				var finArr = finTime.Split(' ');

				try
				{
					model.StartDateTime = Convert.ToDateTime(stArr[0] + ", "
															+ stArr[2] + " "
															+ stArr[1] + " "
                                                            + stArr[3] + " "
															+ stArr[4]);
				}
				catch (Exception)
				{
                    try
                    {
                        model.StartDateTime = Convert.ToDateTime(stArr[0] + ", "
                                                            + stArr[2] + " "
                                                            + stArr[1] + " "
                                                            + stArr[6] + " "
                                                            + stArr[3]);

                    }
                    catch (Exception)
                    { }
                
                }

				try
				{
					model.FinishDateTime = Convert.ToDateTime(finArr[0] + ", "
															+ finArr[2] + " "
															+ finArr[1] + " "
                                                            + finArr[3] + " "
															+ finArr[4]);                                                        //+ finArr[5] + " " 

				}
				catch (Exception)
				{
                    try
                    {
                        model.FinishDateTime = Convert.ToDateTime(finArr[0] + ", "
                                                            + finArr[2] + " "
                                                            + finArr[1] + " "
                                                            + finArr[6] + " "
                                                            + finArr[3]); 

                    }
                    catch (Exception)
                    { }
                }

				if (allDay && model.StartDateTime != null && model.StartDateTime == model.FinishDateTime)
				{
					model.FinishDateTime = model.FinishDateTime.Value.AddHours(23 - model.FinishDateTime.Value.Hour);
					model.FinishDateTime = model.FinishDateTime.Value.AddMinutes(59 - model.FinishDateTime.Value.Minute);
				}

				return View(model);
			}
        }

        /// <summary>
        /// Добавить напоминание (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddReminder(AddReminderModel model)
        {
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{                
                    var reminderRepo = unitOfWork.CreateRepo<Reminder>();

                    ReminderMapper mapper = new ReminderMapper();
                    Reminder reminder = mapper.GetReminder(model);
                    reminder.UserID = unitOfWork.CreateInterfacedRepo<IUserRepo>()
                                                    .GetByLogin(GetAuthenticatedName()).UserID;
                    reminderRepo.Add(reminder);

                    String error;
                    if ((error = unitOfWork.SaveAndGetError()) != null)
                    {
                        ModelState.AddModelError("", error);
                    }
                    else
                    {
                        return RedirectToAction("Calendar", "Default");
                    }
                }

				model = DataHelper.GetAddReminderModel(model, unitOfWork, GetAuthenticatedName());
				return View(model);
            }
        }

        /// <summary>
        /// Изменить напоминание 
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns></returns>
		[HttpGet]
		public ActionResult EditReminder(Int32 reminderID)
		{  
			EditReminderModel model;
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var reminder = unitOfWork.CreateRepo<Reminder>()
											.SingleOrDefault(it => it.ReminderID == reminderID);

				EditReminderModelMapper mapper = new EditReminderModelMapper();
				model = mapper.GetEditReminderModel(reminder);

				model = DataHelper.GetEditReminderModel(model, unitOfWork, GetAuthenticatedName());				
				return View(model);				
			}	
		}	  		

        /// <summary>
        /// Изменить напоминание (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost]
		public ActionResult EditReminder(EditReminderModel model)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				if (ModelState.IsValid)
				{ 				
					var reminderRepo = unitOfWork.CreateRepo<Reminder>();
					Reminder reminder = reminderRepo.Single(it => it.ReminderID == model.ReminderID);

					reminder.CarID = model.CarID;
					reminder.StartDateTime = (DateTime)model.StartDateTime;
					reminder.FinishDateTime = (DateTime)model.FinishDateTime;
                    //reminder.IsRemindByEmail = model.IsRemindByEmail;
					reminder.IsReminderDone = model.IsReminderDone;
                    //reminder.ReminderCountOfDays = model.ReminderCountOfDays;
					reminder.ReminderName = model.Title;   											 

					String error;
					if ((error = unitOfWork.SaveAndGetError()) != null)
					{
						ModelState.AddModelError("", error);
					}
					else
					{
						return RedirectToAction("ReminderInfo", "Default",
												new System.Web.Routing.RouteValueDictionary() 
												{
													{"reminderID", model.ReminderID}
												});
					}
				}

				model = DataHelper.GetEditReminderModel(model, unitOfWork, GetAuthenticatedName());
				return View(model);
			}	
		}

        /// <summary>
        /// Удалить напоминание
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns></returns>
		[HttpPost]
		public ActionResult DeleteReminder(Int32 reminderID)
		{
			using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
			{
				var reminderRepo = unitOfWork.CreateRepo<Reminder>();
				var reminder = reminderRepo.SingleOrDefault(it => it.ReminderID == reminderID);

				if (reminder == null)
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Напоминание не найдено",
																							"",
																							"",
																							null));
				}

				var acces = unitOfWork.CreateInterfacedRepo<ICarRepo>()
							.CheckUserAcces(reminder.CarID, GetAuthenticatedName());
				if (acces != Logic.Enums.UserAccesOnCar.Own && acces != Logic.Enums.UserAccesOnCar.Transmitted)
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("У Вас недостаточно прав для удаления напоминания",
																							"",
																							"",
																							null));

				}

				reminderRepo.Delete(reminder);

				String Error;
				if ((Error = unitOfWork.SaveAndGetError()) != null)
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Не удалось удалить напоминание",
																							"Calendar",
																							"Default",
																							null));
				}
				else
				{
					return PartialView("MessageWithRedirect", new MessageWithRedirectModel("Напоминание успешно удалено",
																							"Calendar",
																							"Default",
																							null));
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

    }
}
