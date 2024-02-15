using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Fakes;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Data;
using Tameenk.Redis;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.QuotationNew.Components;

namespace Tameenk.Services.QuotationNew.Api.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, TameenkConfig config)
        {
            //controllers
            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());

            // data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.Path);
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();

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

            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationAuthorizationService>().As<IQuotationAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationNewContext>().As<IQuotationNewContext>().InstancePerLifetimeScope();
            builder.RegisterType<AsyncQuotationContext>().As<IAsyncQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<AsyncCompanyQuotationContext>().As<IAsyncCompanyQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<CompanyQuotationService>().As<ICompanyQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<WataniyaNajmQueueService>().As<IWataniyaNajmQueueService>().InstancePerLifetimeScope();

            //builder.RegisterType<RedisCacheManager>().As<IRedisCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<Tameenk.Services.Core.Quotations.IQuotationService > ().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<Tameenk.Services.QuotationNew.Components.Services.QuotationService>().As<Tameenk.Services.QuotationNew.Components.IQuotationService>().InstancePerLifetimeScope();

        }
    }
}