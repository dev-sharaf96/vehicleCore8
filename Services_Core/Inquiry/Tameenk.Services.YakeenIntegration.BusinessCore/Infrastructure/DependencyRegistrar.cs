using Autofac;
using Autofac.Integration.WebApi;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Context;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Fakes;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Data;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.YakeenIntegration.Business.Services.Implementation;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using AspNetUser = Tameenk.Core.Domain.Entities.AspNetUser;
using Tameenk.Services.YakeenIntegration.Business;

namespace Tameenk.Services.YakeenIntegration.Business.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, TameenkConfig config)
        {
            //controllers
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());

            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            if (config.HttpClient.UseResillientHttpClient)
            {
                //ToDo fix resilient dependancy before make the below code working
                // Problem found with System.Net.Http

                //var httpClientFactory = new ResilientHttpClientFactory(config);
                //builder.RegisterInstance<IResilientHttpClientFactory>(httpClientFactory);
                //builder.RegisterInstance<IHttpClient>(httpClientFactory.CreateResilientHttpClient());
            }
            else
            {
                builder.RegisterType<StandardHttpClient>().As<IHttpClient>().SingleInstance();
            }
            // data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.Path);
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();

            builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            builder.RegisterType<YakeenVehicleServices>().As<IYakeenVehicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<DriverServices>().As<IDriverServices>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();

            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.Register(c => new Yakeen4BcareClient()).InstancePerLifetimeScope();
            if (config.Yakeen.TestMode)
            {
                builder.RegisterType<YakeenFakeClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<YakeenClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            }


        }
    }
}