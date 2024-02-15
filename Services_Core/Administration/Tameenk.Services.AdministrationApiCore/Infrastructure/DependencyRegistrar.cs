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
using Tameenk.Payment.Esal.Component;
using Tameenk.Security.Services;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Repositories;
using Tameenk.Services.Administration.Identity.Services;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Files;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Invoices;
using Tameenk.Services.Core.Najm;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Products;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Settings;
using Tameenk.Services.Core.Ticket;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Excel;
using Tameenk.Services.Implementation.Files;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Implementation.Najm;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Products;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Implementation.Settings;
using Tameenk.Services.Implementation.Ticket;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Implementation;
using Tameenk.Services.Logging;
using Tameenk.Services.Notifications;
using Tameenk.Services.Orders;
using Tameenk.Services.Policy.Components;
using Tameenk.Services.Profile.Component;
using Tameenk.Services.UserTicket.Components;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.Generic.Component;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Implementation.Leasing;
using Tameenk.Services.Core.Leasing;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Implementation.IVR;

namespace Tameenk.Services.AdministrationApi.Infrastructure
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
            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.AdminPath);
            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();

            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();

            builder.RegisterType<Implementation.Invoices.InvoiceService>().As<Core.Invoices.IInvoiceService>().InstancePerLifetimeScope();

            builder.RegisterType<DriverService>().As<IDriverService>().InstancePerLifetimeScope();


            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenCityCenterService>().As<IYakeenCityCenterService>().InstancePerLifetimeScope();

            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().SingleInstance();
            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelService>().As<IExcelService>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();

            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();

            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyContext>().As<IPolicyContext>().InstancePerLifetimeScope();
            builder.RegisterType<Policy.Components.InvoiceService>().As<Policy.Components.IInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyEmailService>().As<IPolicyEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();

            builder.RegisterType<NajmService>().As<INajmService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutsService>().As<ICheckoutsService>().InstancePerLifetimeScope();
            builder.RegisterType<EsalSearchService>().As<IEsalSearchService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<EsalPaymentService>().As<IEsalPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();

            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<RiyadBankMigsPaymentService>().As<IRiyadBankMigsPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<PayfortPaymentService>().As<IPayfortPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<UsersLocationsDeviceInfoService>().As<IUsersLocationsDeviceInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();

            builder.RegisterType<GenericContext>().As<IGenericContext>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyNotificationContext>().As<IPolicyNotificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyFileContext>().As<IPolicyFileContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyModificationContext>().As<IPolicyModificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<InsuredService>().As<IInsuredService>().InstancePerLifetimeScope();            builder.RegisterType<YakeenClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            builder.RegisterType<WareefService>().As<IWareefService>().InstancePerLifetimeScope();            //HTTP context and other related stuff
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

            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<TicketService>().As<ITicketService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileContext>().As<IProfileContext>().InstancePerLifetimeScope();
            builder.RegisterType<UserTicketContext>().As<IUserTicketContext>().InstancePerLifetimeScope();

            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
            builder.RegisterType<BankNinService>().As<IBankNinService>().InstancePerLifetimeScope();
            builder.RegisterType<BankCompanyService>().As<IBankCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<UserPageService>().As<IUserPageService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
            builder.RegisterType<CorporateUserService>().As<ICorporateUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CorporateAccountService>().As<ICorporateAccountService>().InstancePerLifetimeScope();
            //Ahmed Hassan

            builder.RegisterType<Tameenk.Services.Inquiry.Components.InquiryContext>().As<Tameenk.Services.Inquiry.Components.IInquiryContext>().InstancePerLifetimeScope();            builder.AuthRegister();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<OwnDamageQueueService>().As<IOwnDamageQueueService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomCardQueueService>().As<ICustomCardQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<Tameenk.Services.Inquiry.Components.AutoleasingInquiryContext>().As<Tameenk.Services.Inquiry.Components.IAutoleasingInquiryContext>().InstancePerLifetimeScope();
            builder.RegisterType<TabbyPaymentService>().As<ITabbyPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<Tameenk.Services.Inquiry.Components.InquiryUtilities>().As<Tameenk.Services.Inquiry.Components.IInquiryUtilities>().InstancePerLifetimeScope();

            builder.RegisterType<LeasingPolicyService>().As<ILeasingPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<LeasingProfileService>().As<ILeasingProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<AdministrationPolicyService>().As<IAdministrationPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<IVRService>().As<IIVRService>().InstancePerLifetimeScope();
            builder.RegisterType<LeasingUserService>().As<ILeasingUserService>().InstancePerLifetimeScope();

        }
    }
}