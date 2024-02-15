using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.Clear();
                context.Response.ClearContent();
                context.Response.ClearHeaders();
                context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetValidUntilExpires(true);
                context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
                context.Response.Cache.SetProxyMaxAge(new TimeSpan(24, 0, 0));
                context.Response.ContentEncoding = Encoding.UTF8;
                //if (!string.IsNullOrEmpty(context.Request["r"]))
                //{
                //    string referenceId = context.Request["r"];
                //    string providerFullTypeName = "Tameenk.Services.Policy.Components.PolicyFileContext, Tameenk.Services.Policy.Components";
                //    IPolicyFileContext provider = null;
                //    object instance = null;
                //    var scope = EngineContext.Current.ContainerManager.Scope();
                //    if (instance != null)
                //    {
                //        provider = instance as IPolicyFileContext;
                //    }
                //    var providerType = Type.GetType(providerFullTypeName);

                //    if (providerType != null)
                //    {
                //        if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                //        {
                //            //not resolved
                //            instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                //        }
                //        provider = instance as IPolicyFileContext;
                //    }
                //    if (provider == null)
                //    {
                //        throw new Exception("Unable to find provider.");
                //    }

                //    byte[] buffer = provider.DownloadPolicyFile(referenceId);
                //    scope.Dispose();
                //    if (buffer != null)
                //    {
                //        context.Response.ContentType = "application/pdf";
                //        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                //        context.Response.Flush();
                //        context.Response.Clear();
                //    }
                //}
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                context.Response.Write(exp.ToString());
            }
        }


    }
}
