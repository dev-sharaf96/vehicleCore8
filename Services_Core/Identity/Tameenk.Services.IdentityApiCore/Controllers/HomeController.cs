using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.IdentityApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
