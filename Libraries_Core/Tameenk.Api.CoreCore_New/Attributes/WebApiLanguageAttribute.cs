using Tameenk.Api.Core.Context;
using Tameenk.Core.Infrastructure;

namespace Tameenk.Api.Core.Attributes
{
    public class WebApiLanguageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var webApiContext = EngineContext.Current.Resolve<IWebApiContext>();
            // Get the curren language to set the culture.
            var currentLanguage = webApiContext.CurrentLanguage;
            var culture = new System.Globalization.CultureInfo(currentLanguage.ToString());
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            base.OnActionExecuting(actionContext);
        }
    }
}
