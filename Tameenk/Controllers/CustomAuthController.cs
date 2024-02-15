using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Models;

namespace Tameenk.Controllers
{
    public class CustomAuthController : Controller
    {
        // GET: CustomAuth/Login
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (model.Email == "test@test.com" && model.Password == "test1234")
            {
                var cookie = new HttpCookie("AuthenticationCustom123", "P$3wssd@sdad1245");
                cookie.HttpOnly = true;
                cookie.Secure = false;
                cookie.Expires = DateTime.Now.AddDays(1);
                Request.Cookies.Add(cookie);
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }
    }
}