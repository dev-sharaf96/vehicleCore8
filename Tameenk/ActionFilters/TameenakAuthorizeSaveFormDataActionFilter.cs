using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Tameenk.ActionFilters
{
    public class TameenakAuthorizeSaveFormDataActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                foreach (var formDataKey in filterContext.HttpContext.Request.Form.AllKeys)
                {
                    if (filterContext.HttpContext.Session[formDataKey] != null)
                        filterContext.HttpContext.Session.Remove(formDataKey);
                    filterContext.HttpContext.Session.Add(formDataKey, filterContext.HttpContext.Request.Form[formDataKey]);
                }

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary{
                        { "controller", Constants.LoginControllerName },
                        { "action", Constants.LoginActionName },
                        { "returnUrl", string.Format("/{0}/{1}", controllerName, actionName) }
                    });
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}