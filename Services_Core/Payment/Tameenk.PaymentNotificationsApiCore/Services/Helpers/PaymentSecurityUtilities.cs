using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using Tameenk.PaymentNotificationsApi.Services.Core;

namespace Tameenk.PaymentNotificationsApi.Services.Helpers
{
    public static class PaymentSecurityUtilities
    {
        public static string GetIP(HttpRequestMessage requestMessage)
        {
            try
            {
                // Owin Hosting 
                if (requestMessage.Properties.ContainsKey("MS_OwinContext"))
                {
                    return HttpContext.Current != null ? HttpContext.Current.Request.GetOwinContext()
                .Request.RemoteIpAddress : null;
                }
                // Web Hosting 
                if (requestMessage.Properties.ContainsKey("MS_HttpContext"))
                {
                    return HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
                }
                // Self Hosting 
                if (requestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                {
                    RemoteEndpointMessageProperty property =
                (RemoteEndpointMessageProperty)requestMessage.Properties[RemoteEndpointMessageProperty.Name];
                    return property != null ? property.Address : null;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}