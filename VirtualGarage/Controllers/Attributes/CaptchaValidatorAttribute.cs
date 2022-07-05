using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualGarage.Controllers.Attributes
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string CHALLENGE_FIELD_KEY = "recaptcha_challenge_field";
        private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";
        private const string CAPTCHA_MODEL_KEY = "Captcha";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[CHALLENGE_FIELD_KEY];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY];
            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {
                PrivateKey = "6LfDnckSAAAAAIqqxhjBAbH6kn5dGmPmjwaKGmKg",
                RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };

            var recaptchaResponse = captchaValidtor.Validate();

            if (!recaptchaResponse.IsValid)
            {
                filterContext.Controller
                    .ViewData.ModelState
                    .AddModelError(
                        CAPTCHA_MODEL_KEY,
                        "Неправильно введен код с картинки. Пожалуйста, введите код с картинки заново");
            }

            base.OnActionExecuting(filterContext);
        }
    }

}