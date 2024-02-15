using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk
{
    //ToDo: please delete this file ASAP
    public class AuthenticationModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest +=
            (new EventHandler(this.Application_BeginRequest));

        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            var context = application.Context;
            if (context.Request.Url.PathAndQuery.Contains("CustomAuth/Login")) return;

            if (context.Request.Cookies["AuthenticationCustom123"] != null)
            {
                if (context.Request.Cookies["AuthenticationCustom123"].Value == "P$3wssd@sdad1245")
                {
                    return;
                }
            }

            context.Response.Redirect(context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + "/CustomAuth/Login");
        }
    }
}