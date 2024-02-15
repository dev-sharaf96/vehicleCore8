using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Common.Utilities;

namespace Tameenk.Controllers
{

    /// <summary>
    /// Base controller contains any shared prop and methods
    /// </summary>
    public class BaseController : Controller
    {
        public BaseController()
        {
            ViewBag.IsCorporateAdmin = IsCorporateAdmin();
        }

        /// <summary>
        /// Add errors to model state
        /// </summary>
        /// <param name="modelErrors">Key for element name, Value is error message</param>
        protected void AddErrorsToModelStateErors(Dictionary<string, string> modelErrors)
        {
            foreach (var modelError in modelErrors)
            {
                ModelState.AddModelError(modelError.Key, modelError.Value);
            }
        }

        protected bool IsCorporateAdmin()
        {
            var isCorporateAdmin = false;
            var cookie = Utilities.GetCookie("_authCorporateCookie");
            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }
            var userTicket = System.Web.Security.FormsAuthentication.Decrypt(cookie);
            if (userTicket.Expired)
            {
                return false;
            }
            string[] values = userTicket.UserData.Split(';');

            foreach (var value in values)
            {
                if (value.Contains("IsCorporateSuperAdmin"))
                {
                    var isCorporateSuperAdminValue = value.Substring(value.LastIndexOf("=") + 1);
                    if (isCorporateSuperAdminValue.ToLower() == "true")
                    {
                        isCorporateAdmin = true;
                    }
                }
            }

            return isCorporateAdmin;
        }
    }
}