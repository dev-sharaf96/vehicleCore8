using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity.EntityFramework;
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
using Tameenk.Payment.Esal.Component;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.PaymentNotificationsApi.Services.Implementation;
using Tameenk.Reports.Quotation;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Excel;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Implementation.IVR;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tameenk.Services.Quotation.Components;

namespace Tameenk.PaymentNotificationsApi.Infrastructure
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

            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();

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
            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();


            builder.RegisterType<PolicyGenerationTask>().As<IPolicyGenerationTask>().InstancePerLifetimeScope();
            builder.RegisterType<PayfortNotificationsServices>().As<IPayfortNotificationsServices>().InstancePerLifetimeScope();
            builder.RegisterType<SadadNotificationsServices>().As<ISadadNotificationsServices>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentNotificationSmsSender>().As<IPaymentNotificationSmsSender>().InstancePerLifetimeScope();
            builder.RegisterType<PayfortPaymentService>().As<IPayfortPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<DriverService>().As<IDriverService>().InstancePerLifetimeScope();
            builder.RegisterType<RiyadBankMigsPaymentService>().As<IRiyadBankMigsPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<EsalPaymentService>().As<IEsalPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenCityCenterService>().As<IYakeenCityCenterService>().InstancePerLifetimeScope();            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();


            builder.RegisterType<HyperPayNotificationSersvice>().As<IHyperPayNotificationSersvice>().InstancePerLifetimeScope();
            builder.RegisterType<EdaatNotificationsServices>().As<IEdaatNotificationsServices>().InstancePerLifetimeScope();



            builder.RegisterType<Tameenk.Services.Quotation.Components.QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();

            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutsService>().As<ICheckoutsService>().InstancePerLifetimeScope();

            builder.RegisterType<EsalPaymentService>().As<IEsalPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();


            //builder.RegisterType<YakeenCityCenterService>().As<IYakeenCityCenterService>().InstancePerLifetimeScope();
            //builder.RegisterType<TicketService>().As<ITicketService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProfileContext>().As<IProfileContext>().InstancePerLifetimeScope();
            //builder.RegisterType<UserTicketContext>().As<IUserTicketContext>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelService>().As<IExcelService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
            builder.RegisterType<WataniyaNajmQueueService>().As<IWataniyaNajmQueueService>().InstancePerLifetimeScope();
            // builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<TabbyPaymentService>().As<ITabbyPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<IVRService>().As<IIVRService>().InstancePerLifetimeScope();

        }
    }
}