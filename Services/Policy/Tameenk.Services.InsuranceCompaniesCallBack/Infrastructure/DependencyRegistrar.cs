using Autofac;
using Autofac.Integration.WebApi;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Context;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Fakes;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Data;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Najm;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Settings;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Implementation.IVR;
using Tameenk.Services.Implementation.Najm;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Implementation.Settings;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.InsuranceCompaniesCallBack.Repository;
using Tameenk.Services.InsuranceCompaniesCallBack.Services;
using Tameenk.Services.Logging;
using Tameenk.Services.Notifications;
using Tameenk.Services.Orders;
using Tameenk.Services.Policy.Components;
using Tameenk.Services.Quotation.Components;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Infrastructure
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

            builder.RegisterType<PolicyRepository>().As<IPolicyRepository>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompaniesCallBackService>().As<IInsuranceCompaniesCallBackService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutDetailsRepository>().As<ICheckoutDetailsRepository>().InstancePerLifetimeScope();
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyNotificationContext>().As<IPolicyNotificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutsService>().As<ICheckoutsService>().InstancePerLifetimeScope();

            builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<WataniyaNajmQueueService>().As<IWataniyaNajmQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyFileContext>().As<IPolicyFileContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyEmailService>().As<IPolicyEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<DriverService>().As<IDriverService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<NajmService>().As<INajmService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceService>().As<IInvoiceService>().InstancePerLifetimeScope();


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

            builder.RegisterType<IVRService>().As<IIVRService>().InstancePerLifetimeScope();
            builder.RegisterType<TabbyPaymentService>().As<ITabbyPaymentService>().InstancePerLifetimeScope();

        }
    }
}