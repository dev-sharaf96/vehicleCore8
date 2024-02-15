using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Tameenk.Controllers
{
    public class LangController : Controller
    {

        public ActionResult Change(string lang)
        {
            if (lang != null)
            {

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            }
            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = lang;
            Response.Cookies.Add(cookie);

            return RedirectToLocal();
        }

        private ActionResult RedirectToLocal()
        {
            if(Request==null ||Request.UrlReferrer==null)
                return Redirect("/");
            else
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}