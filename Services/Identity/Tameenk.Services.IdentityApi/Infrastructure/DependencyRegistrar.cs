using Autofac;
using Autofac.Integration.WebApi;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Infrastructure.DependencyManagement;
using Tameenk.Services.Logging;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Data;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Core.Attachments;
using Tameenk.Security.Services;
using Tameenk.Core.Caching;
using Microsoft.AspNet.Identity.EntityFramework;
using Tameenk.Api.Core.Context;
using System.Web;
using Tameenk.Core.Fakes;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Orders;
using Tameenk.Services.Notifications;
using Tameenk.Services.UserTicket.Components;
using Tameenk.Services.Profile.Component;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Implementation;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Implementation.Addresses;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tameenk.Services.Implementation.Excel;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Policy.Components;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Implementation.Quotations;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Implementation.Occupations;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.Core.Occupations;
using Tameenk.Services.Implementation.Leasing;
using Tameenk.Services.Core.Leasing;
using Tameenk.Services.YakeenIntegration.Business.Services.Implementation;
using Tameenk.Services.Implementation.IVR;
using Tameenk.Services.Core.IVR;
using Tameenk.Redis;

namespace Tameenk.Services.IdentityApi.Infrastructure
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
            
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();

            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<FireBaseNotificationService>().As<INotificationServiceProvider>().InstancePerLifetimeScope();            builder.RegisterType<UserTicketContext>().As<IUserTicketContext>().InstancePerLifetimeScope();            builder.RegisterType<ProfileContext>().As<IProfileContext>().InstancePerLifetimeScope();            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();            builder.RegisterType<AuthenticationContext>().As<IAuthenticationContext>().InstancePerLifetimeScope();




            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyContext>().As<IPolicyContext>().InstancePerLifetimeScope();
            builder.RegisterType<Policy.Components.InvoiceService>().As<Policy.Components.IInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyEmailService>().As<IPolicyEmailService>().InstancePerLifetimeScope();

            builder.RegisterType<CustomCardQueueService>().As<ICustomCardQueueService>().InstancePerLifetimeScope();

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

            builder.RegisterType<PromotionContext>().As<IPromotionContext>().InstancePerLifetimeScope();            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryContext>().As<IInquiryContext>().InstancePerLifetimeScope();
            builder.RegisterType<DriverServices>().As<IDriverServices>().InstancePerLifetimeScope();
            builder.RegisterType<CorporateContext>().As<ICorporateContext>().InstancePerLifetimeScope();
            builder.RegisterType<CorporateUserService>().As<ICorporateUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyNotificationContext>().As<IPolicyNotificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyFileContext>().As<IPolicyFileContext>().InstancePerLifetimeScope();
            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelService>().As<IExcelService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingInquiryContext>().As<IAutoleasingInquiryContext>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyModificationContext>().As<IPolicyModificationContext>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenClient>().As<IYakeenClient>().InstancePerLifetimeScope();
            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutsService>().As<ICheckoutsService>().InstancePerLifetimeScope();
            builder.RegisterType<QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<TabbyPaymentService>().As<ITabbyPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<YakeenVehicleServices>().As<IYakeenVehicleServices>().InstancePerLifetimeScope();
            builder.RegisterType<OccupationService>().As<IOccupationService>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryUtilities>().As<IInquiryUtilities>().InstancePerLifetimeScope();
            builder.RegisterType<LeasingUserService>().As<ILeasingUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();
            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
            builder.RegisterType<IVRService>().As<IIVRService>().InstancePerLifetimeScope();
        }
    }
}