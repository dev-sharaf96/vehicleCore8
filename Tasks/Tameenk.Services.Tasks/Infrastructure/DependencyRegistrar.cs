using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Data;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Infrastructure;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Core.Payments;
using Tameenk.Core.Fakes;
using Tameenk.Services.Implementation.Localizations;
using Tameenk.Services.Localizations;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Orders;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Implementation.Attachments;
using Microsoft.AspNet.Identity.EntityFramework;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Core.Addresses;
//using Tameenk.Services.Implementation.Settings;
//using Tameenk.Services.Core.Settings;
using Tameenk.Services.Policy.Components;
using System.Web.Configuration;
using Tameenk.Services.Checkout.Components;
using Tamkeen.bll.Services.Najm;
using Tameenk.Security.Services;
using TameenkDAL.UoW;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Payment.Esal.Component;
using Tameenk.Services.Notifications;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.Core.Settings;
using Tameenk.Services.Implementation.Settings;
using Tameenk.Services.Implementation.Excel;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Implementation;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.YakeenIntegration.Business.Services.Implementation;
using Tameenk.Services.Core.Occupations;
using Tameenk.Services.Implementation.Occupations;
using Tameenk.Services.Core.Leasing;
using Tameenk.Services.Implementation.Leasing;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Implementation.IVR;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
//using Tameenk.Services.Implementation.Offers;
//using Tameenk.Services.Core.Offers;

namespace Tameenk.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, TameenkConfig config)
        {

            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            // data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.Path);
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();


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

            builder.RegisterType<WebContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            if (config.RunOnAzureWebsites)
            {
                builder.RegisterType<AzureWebsitesMachineNameProvider>().As<IMachineNameProvider>().SingleInstance();
            }
            else
            {
                builder.RegisterType<DefaultMachineNameProvider>().As<IMachineNameProvider>().SingleInstance();
            }

             builder.RegisterType<TameenkUoW>().As<ITameenkUoW>().InstancePerLifetimeScope();

            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
             builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();

            //builder.RegisterType<YakeenService>().As<IYakeenService>().InstancePerLifetimeScope();

            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();


            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyContext>().As<IPolicyContext>().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceService>().As<IInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyEmailService>().As<IPolicyEmailService>().InstancePerLifetimeScope();

            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<PayfortPaymentService>().As<IPayfortPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<RiyadBankMigsPaymentService>().As<IRiyadBankMigsPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<DriverService>().As<IDriverService>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();

            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<NajmService>().As<INajmService>().InstancePerLifetimeScope();
            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutsService>().As<ICheckoutsService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<EsalPaymentService>().As<IEsalPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelService>().As<IExcelService>().InstancePerLifetimeScope();
            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyFileContext>().As<IPolicyFileContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyNotificationContext>().As<IPolicyNotificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<CustomCardQueueService>().As<ICustomCardQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
            builder.RegisterType<WataniyaNajmQueueService>().As<IWataniyaNajmQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<CorporateUserService>().As<ICorporateUserService>().InstancePerLifetimeScope();
            builder.RegisterType<TabbyPaymentService>().As<ITabbyPaymentService>().InstancePerLifetimeScope();

            builder.RegisterType<Tameenk.Services.Inquiry.Components.AutoleasingInquiryContext>().As<Tameenk.Services.Inquiry.Components.IAutoleasingInquiryContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyModificationContext>().As<IPolicyModificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<Tameenk.Services.Inquiry.Components.InquiryUtilities>().As<Tameenk.Services.Inquiry.Components.IInquiryUtilities>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenVehicleServices>().As<IYakeenVehicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<OccupationService>().As<IOccupationService>().InstancePerLifetimeScope();
            builder.RegisterType<LeasingUserService>().As<ILeasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();
            builder.RegisterType<IVRService>().As<IIVRService>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenClient>().As<IYakeenClient>().InstancePerLifetimeScope();

        }
    }
}