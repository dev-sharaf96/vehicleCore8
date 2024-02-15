using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Occupations;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Settings;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Occupations;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Implementation.Settings;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegrationApi.Services.Core;
using Tameenk.Services.YakeenIntegrationApi.Services.Implementation;
using Tameenk.Services.YakeenIntegrationApi.WebClients.Core;
using Tameenk.Services.YakeenIntegrationApi.WebClients.Implementation;
using Tameenk.Services.YakeenIntegrationApi.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegrationApi.Infrastructure
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

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();


            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();

            //if (config.Najm.TestMode)
            //{
            //    builder.RegisterType<FakeNajmService>().As<INajmService>().InstancePerLifetimeScope();
            //}
            //else
            //{
            //}
            builder.RegisterType<NajmService>().As<INajmService>().InstancePerLifetimeScope();

            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();

            builder.RegisterType<QuotationService>().As<Core.Quotations.IQuotationService>().InstancePerLifetimeScope();
            //builder.RegisterType<QuotationRequestService>().As<IQuotationRequestService>().InstancePerLifetimeScope();

           // builder.RegisterType<Services.Implementation.QuotationRequestService>().As<Services.Core.IQuotationRequestService>().InstancePerLifetimeScope();

            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<OccupationService>().As<IOccupationService>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();

            //if (config.SaudiPost.TestMode)
            //{
            //    builder.RegisterType<FakeSaudiPostService>().As<ISaudiPostService>().InstancePerLifetimeScope();
            //}
            //else
            //{

            //}
           // builder.RegisterType<SaudiPostService>().As<ISaudiPostService>().InstancePerLifetimeScope();


            builder.RegisterType<YakeenVehicleServices>().As<IYakeenVehicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<DriverServices>().As<IDriverServices>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();

            builder.Register(c => new Yakeen4BcareClient()).InstancePerLifetimeScope();

            //builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();


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
            builder.RegisterType<InquiryContext>().As<IInquiryContext>().InstancePerLifetimeScope();

            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.Register(c => new Yakeen4BcareClient()).InstancePerLifetimeScope();
            //if (!config.Yakeen.TestMode)
            //{
            //    builder.RegisterType<YakeenFakeClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            //}
            //else
            //{

            //}

            builder.RegisterType<YakeenClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuredService>().As<IInsuredService>().InstancePerLifetimeScope();
            builder.RegisterType<Implementation.Checkouts.CheckoutsService>().As<Core.Checkouts.ICheckoutsService>().InstancePerLifetimeScope();



        }
    }
}