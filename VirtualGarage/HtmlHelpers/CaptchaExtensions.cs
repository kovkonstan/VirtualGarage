using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace VirtualGarage.HtmlHelpers
{
    public static class CaptchaExtensions
    {
        public static String GenerateCaptcha(this HtmlHelper helper)
        {
            var captchaControl = new Recaptcha.RecaptchaControl
            {
                ID = "recaptcha",
                Theme = "white",
                PublicKey = "6LfDnckSAAAAANrFm1b9N1b5esww4OqQHyQvQxzO",
                PrivateKey = "6LfDnckSAAAAAIqqxhjBAbH6kn5dGmPmjwaKGmKg"
            };
            var htmlWriter = new HtmlTextWriter(new StringWriter());
            captchaControl.RenderControl(htmlWriter);
            return htmlWriter.InnerWriter.ToString();
        }
    }
}