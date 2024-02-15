﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Tameenk.Services.Capcha.API
{
    public class WebApiApplication : System.Web.HttpApplication
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
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}