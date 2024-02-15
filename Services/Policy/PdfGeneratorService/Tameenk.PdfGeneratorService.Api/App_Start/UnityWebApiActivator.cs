using System.Web.Http;

using Unity.AspNet.WebApi;

//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Tameenk.PdfGeneratorService.Api.UnityWebApiActivator), nameof(Tameenk.PdfGeneratorService.Api.UnityWebApiActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Tameenk.PdfGeneratorService.Api.UnityWebApiActivator), nameof(Tameenk.PdfGeneratorService.Api.UnityWebApiActivator.Shutdown))]

namespace Tameenk.PdfGeneratorService.Api
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET.
    /// </summary>
    public static class UnityWebApiActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start(HttpConfiguration config) 
        {
            // Use UnityHierarchicalDependencyResolver if you want to use
            // a new child container for each IHttpController resolution.
            // var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.Container);
            var resolver = new UnityDependencyResolver(UnityConfig.Container);

            config.DependencyResolver = resolver;
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }
    }
}