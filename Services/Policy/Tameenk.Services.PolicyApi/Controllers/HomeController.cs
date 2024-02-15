using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tameenk.Services.PolicyApi.Controllers
{
    /// <summary>
    /// Home controller for dicumentation
    /// </summary>
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}