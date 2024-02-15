using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using PrometheusLib;

namespace Tameenk.Services.InquiryGateway
{
    public partial class WebApiApplication : System.Web.HttpApplication
    {
        public static IHttpModule Module = new PrometheusLib.Classes.PrometheusHttpRequestModule();
        public WebApiApplication()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public override void Init()
        {
            base.Init();
            Module.Init(this);
        }
         

    }
}