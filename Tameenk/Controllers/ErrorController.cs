using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Resources.Checkout;

namespace Tameenk.Controllers
{
    public class ErrorController : Controller
    {

        System.Resources.ResourceManager rm;


        public ErrorController()
        {
            rm = new System.Resources.ResourceManager("Tameenk.LangText", this.GetType().Assembly);
        }
        //  
        // GET: /Error/  

        public ActionResult Index(Guid? Id, string message = null , string key=null, string returnURL = null)
        {
            if(Id.HasValue)
            {
                ViewBag.ErrorCode = Id.Value;
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                ViewBag.Message = GetResxNameByValue(key);
            }
                if (!string.IsNullOrWhiteSpace(message)) {
                ViewBag.Message = message;
            }

            if (!string.IsNullOrWhiteSpace(returnURL))
            {
                ViewBag.returnURL = returnURL;
            }
            return View("error");
        }

        public ActionResult MissingAddress()
        {
            ViewBag.Message = CheckoutResources.MissingAddress;
            return View("error");
        }
        public ActionResult AccountLocked()
        {
            ViewBag.Message = CheckoutResources.AccountLocked;
            return View("error");
        }
        public ActionResult ErrorMessage()
        {
            ViewBag.Message = CheckoutResources.Error;
            return View("error");
        }
        private string GetResxNameByValue(string value)
        {
           
            if (rm != null)
            {
                var entry =
                    rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                      .OfType<DictionaryEntry>()
                      .FirstOrDefault(e => e.Key.ToString() == value);

                if (entry.Key != null)
                    return entry.Value.ToString();
            }
            return "";

        }

        public ActionResult NotFound()
        {
            HttpContext.Response.StatusCode = 404;
            return View();
        }

        public ActionResult PolicyError()
        {
            return View();
        }
    }
}