using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace VirtualGarage.Models
{
    public class MessageWithRedirectModel
    {
        public MessageWithRedirectModel()
        { }

        public MessageWithRedirectModel(String message, 
                        String actionName, 
                        String controllerName, 
                        RouteValueDictionary routeValueDictionary)
        {
            this.Message = message;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.RouteValueDictionary = routeValueDictionary;

        }

        public String Message { get; set; }

        public String ActionName { get; set; }

        public String ControllerName { get; set; }

        public RouteValueDictionary RouteValueDictionary { get; set; }
    }
}