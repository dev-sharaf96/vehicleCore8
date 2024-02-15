using System.Web.Mvc;

namespace Tameenk.Services.QuotationApi.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        // GET: Home
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}