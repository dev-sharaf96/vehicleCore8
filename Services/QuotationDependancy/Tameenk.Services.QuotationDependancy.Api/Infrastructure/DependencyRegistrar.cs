using Autofac;
using Autofac.Integration.WebApi;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Context;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Fakes;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Data;
using Tameenk.Redis;
using Tameenk.Services.QuotationDependancy.Component;

namespace Tameenk.Services.QuotationDependancy.Api
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
            HttpContext.Current != null 
                        ? (new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                        : (new FakeHttpContext("~/") as HttpContextBase)
                        ).As<HttpContextBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerLifetimeScope();

            // data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.Path);
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            //builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleServices>().As<IVehicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<CompaniesGradesService>().As<ICompaniesGradesService>().InstancePerLifetimeScope();

            //builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            //builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            //builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            //builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<RedisCacheManager>().As<IRedisCacheManager>().InstancePerLifetimeScope();
        }
    }
}