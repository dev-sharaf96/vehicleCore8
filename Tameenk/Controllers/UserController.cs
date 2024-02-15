using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tameenk.Controllers
{
    public class UserController : Controller
    {


        #region Actions

        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult IsValidUser(long userId, int userMonthBirthDate, int userYearBirthDate)
        {
            bool isUserValid = true;

            return Json(isUserValid, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}