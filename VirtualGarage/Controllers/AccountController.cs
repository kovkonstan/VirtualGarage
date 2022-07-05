using System;
using System.Web.Mvc;
using System.Web.Security;
using VirtualGarage.Logic;
using VirtualGarage.Logic.DataModel;
using VirtualGarage.Logic.Repository;
using VirtualGarage.Models;
using VirtualGarage.HtmlHelpers;
using VirtualGarage.Controllers.Attributes;
using System.Text;
using System.Net.Mail;
using System.Linq;
using VirtualGarage.Logic.Enums;

namespace VirtualGarage.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult LogOn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
                {
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    if (userRepo.CheckLoginAndPass(model.UserName, model.Password) == LoginResult.Success)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.IsRememberMe);
                        return RedirectToAction("Index", "Default");
                    }
                }
            }

            return RedirectToAction("LoginFailed", "Account");         
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Default");
        }

        /// <summary>
        /// Регистрация пользователя (get)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Регистрация пользователя (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[CaptchaValidator]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
                {
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    // Проверить, существует ли пользователь с таким именем
                    if (userRepo.IsUserNameExist(model.UserName))
                    {
                        ModelState.AddModelError("", "Пользователь с таким именем уже существует. Выберите другое имя");
                    }
                    // Проверить, существует ли пользователь с таким e-mail'ом
                    else if (userRepo.IsEmailExist(model.UserEmail))
                    {
                        ModelState.AddModelError("", "Пользователь с таким E-mail'ом уже существует. Выберите другой E-mail");
                    }                        
                    else
                    {
                        userRepo.Add(new User()
                        {
                            UserNick = model.UserName,
                            UserEmail = model.UserEmail,
                            UserPassword = model.Password,
                            UserRoleID = _roleIDOfUser
                            //IsCorporativeAccount = model.IsCorporativeAccount
                        });

                        String error;
                        if ((error = unitOfWork.SaveAndGetError()) != null)
                        {
                            ModelState.AddModelError("", error);
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            return RedirectToAction("Index", "Default");
                        }                           
                    }                        
                }                    
            }

            model.Password = String.Empty;
            model.ConfirmPassword = String.Empty;
            return View(model);
        }

        /// <summary>
        /// Страница восстановления пароля (get)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Страница восстановления пароля (post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
                {
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    User user = userRepo.FirstOrDefault(it => it.UserEmail == model.Email);

                    if (user != null)
                    {
                        var message = new StringBuilder();
                        message.AppendFormat("Здравствуйте, {0}!\n\n", user.UserNick);

                        message.AppendFormat("Это письмо было сгенерированно по Вашему запросу на восстановление пароля ");
                        message.AppendFormat("от учетной записи на сайте Virtual Garage. ");
                        message.AppendFormat("Если Вы не делали этот запрос, проигнорируйте письмо.\n");
                        message.AppendFormat("Имя пользователя: {0}\n", user.UserNick);
                        message.AppendFormat("Пароль: {0}\n\n", user.UserPassword);

                        message.AppendFormat("С уважением, Служба поддержки сайта Virtual Garage.");

                        SmtpClient smtpClient = new SmtpClient();

                        try
                        {
                            smtpClient.Send(new MailMessage(
                                "Служба поддержки VirtualGarage <vittualgarage@mail.ru>",   // From
                                user.UserEmail,   // To
                                "Восстановление пароля",    // Subject
                                message.ToString()  // Body
                            ));
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", "Ошибка при отправке письма на указанный E-mail");
                            return View(model);
                        }

                        return RedirectToAction("", "");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Пользователя с таким E-mail'ом не существует");
                    }
                }
            }

            return View(model);
        }
                
        /// <summary>
        /// Страница при неудачном входе в систему(get)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoginFailed()
        {
            return View();
        }

        /// <summary>
        /// Страница при неудачном входе в систему(post)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginFailed(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkProvider.CreateUnitOfWork())
                {
                    var userRepo = unitOfWork.CreateInterfacedRepo<IUserRepo>();

                    LoginResult loginResult = userRepo.CheckLoginAndPass(model.UserName, model.Password);

                    if (loginResult == LoginResult.UserNotExist)
                    {
                        ModelState.AddModelError("", "Пользователя с таким именем не существует");
                    }
                    else if (loginResult == LoginResult.UncorrectPass)
                    {
                        ModelState.AddModelError("", "Неправильный пароль");
                    }
                    else if (loginResult == LoginResult.Success)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.IsRememberMe);
                        return RedirectToAction("Index", "Default");
                    }
                }
            }

            model.Password = String.Empty;
            return View(model);   
        }

        private const Int32 _roleIDOfUser = 1;
    }
}
