using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Resources.Checkout;

namespace Tameenk.Controllers
{
    public class SuccessController : Controller
    {

        System.Resources.ResourceManager rm;


        public SuccessController()
        {
            rm = new System.Resources.ResourceManager("Tameenk.LangText", this.GetType().Assembly);
        }
        //  
        // GET: /Success/  

        public ActionResult Index(string message = null)
        {
            ViewBag.Message = message;
            return View("Success");
        }
    }
}