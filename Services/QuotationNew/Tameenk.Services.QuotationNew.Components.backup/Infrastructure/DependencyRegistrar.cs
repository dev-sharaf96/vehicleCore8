//using Autofac;
//using Autofac.Integration.WebApi;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System.Linq;
//using System.Web;
//using Tameenk.Api.Core.Context;
//using Tameenk.Core.Caching;
//using Tameenk.Core.Configuration;
//using Tameenk.Core.Data;
//using Tameenk.Core.Fakes;
//using Tameenk.Core.Infrastructure;
//using Tameenk.Core.Infrastructure.DependencyManagement;

//namespace Tameenk.Services.QuotationNewApi.Infrastructure
//{
//    public class DependencyRegistrar : IDependencyRegistrar
//    {
//        public int Order => 0;

//        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, TameenkConfig config)
//        {
//            //controllers
//            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray());

//            // data layer
//            var dataSettingsManager = new DataSettingsManager();
//            var dataProviderSettings = dataSettingsManager.LoadSettings(config.Settings.Path);
//            builder.Register<IDbContext>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
//            builder.Register<IdentityDbContext<AspNetUser>>(c => new TameenkObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();

//            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

//            builder.RegisterType<WebApiContext>().As<IWebApiContext>().InstancePerLifetimeScope();

//            //HTTP context and other related stuff
//            builder.Register(c =>
//                //register FakeHttpContext when HttpContext is not available
//                HttpContext.Current != null ?
//                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
//                (new FakeHttpContext("~/") as HttpContextBase))
//                .As<HttpContextBase>()
//                .InstancePerLifetimeScope();
//            builder.Register(c => c.Resolve<HttpContextBase>().Request)
//                .As<HttpRequestBase>()
//                .InstancePerLifetimeScope();
//            builder.Register(c => c.Resolve<HttpContextBase>().Response)
//                .As<HttpResponseBase>()
//                .InstancePerLifetimeScope();
//            builder.Register(c => c.Resolve<HttpContextBase>().Server)
//                .As<HttpServerUtilityBase>()
//                .InstancePerLifetimeScope();
//            builder.Register(c => c.Resolve<HttpContextBase>().Session)
//                .As<HttpSessionStateBase>()
//                .InstancePerLifetimeScope();

//            builder.RegisterType<QyadatSmsProvider>().As<ISmsProvider>().InstancePerLifetimeScope();
//            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
//            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
//            builder.RegisterType<PolicyGenerationTask>().As<IPolicyGenerationTask>().InstancePerLifetimeScope();
//            builder.RegisterType<PayfortNotificationsServices>().As<IPayfortNotificationsServices>().InstancePerLifetimeScope();
//            builder.RegisterType<SadadNotificationsServices>().As<ISadadNotificationsServices>().InstancePerLifetimeScope();
//            builder.RegisterType<PolicyProcessingService>().As<IPolicyProcessingService>().InstancePerLifetimeScope();
//            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
//            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
//            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
//            builder.RegisterType<PaymentNotificationSmsSender>().As<IPaymentNotificationSmsSender>().InstancePerLifetimeScope();
//            builder.RegisterType<PayfortPaymentService>().As<IPayfortPaymentService>().InstancePerLifetimeScope();
//            builder.RegisterType<SadadPaymentService>().As<ISadadPaymentService>().InstancePerLifetimeScope();

//            builder.RegisterType<StandardHttpClient>().As<IHttpClient>().InstancePerLifetimeScope();
//            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
//            builder.RegisterType<VehicleService>().As<IVehicleService>().InstancePerLifetimeScope();
//            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
//            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
//            builder.RegisterType<RequestInitializer>().As<IRequestInitializer>().InstancePerLifetimeScope();
//            builder.RegisterType<QuotationApiService>().As<IQuotationApiService>().InstancePerLifetimeScope();
//            builder.RegisterType<QuotationService>().As<IQuotationService>().InstancePerLifetimeScope();
//            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(dataSettingsManager.GetAbsoluteFilePath(config.Settings.LogConfigPath)));
//            builder.RegisterType<Log4netLogger>().As<ILogger>().InstancePerLifetimeScope();
//            builder.RegisterType<InsuranceCompanyService>().As<IInsuranceCompanyService>().InstancePerLifetimeScope();
//            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
//            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
//            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();

//            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
//            builder.RegisterType<DriverService>().As<IDriverService>().InstancePerLifetimeScope();
//            builder.RegisterType<RiyadBankMigsPaymentService>().As<IRiyadBankMigsPaymentService>().InstancePerLifetimeScope();
//            builder.RegisterType<QuotationContext>().As<IQuotationContext>().InstancePerLifetimeScope();

//            // data layer
//            builder.RegisterType<Tameenk.Services.Implementation.Invoices.InvoiceService>().As<Core.Invoices.IInvoiceService>().InstancePerLifetimeScope();

//            builder.RegisterType<NajmService>().As<INajmService>().InstancePerLifetimeScope();
//            builder.RegisterType<HyperpayPaymentService>().As<IHyperpayPaymentService>().InstancePerLifetimeScope();
//            builder.RegisterType<CheckoutContext>().As<ICheckoutContext>().InstancePerLifetimeScope();

//            // builder.RegisterType<EsalPaymentService>().As<IEsalPaymentService>().InstancePerLifetimeScope();

//            builder.RegisterType<AutoleasingQuotationFormService>().As<IAutoleasingQuotationFormService>().InstancePerLifetimeScope();
//            builder.RegisterType<AutoleasingUserService>().As<IAutoleasingUserService>().InstancePerLifetimeScope();
//            builder.RegisterType<BankService>().As<IBankService>().InstancePerLifetimeScope();
//            builder.RegisterType<WataniyaNajmQueueService>().As<IWataniyaNajmQueueService>().InstancePerLifetimeScope();
//        }
//    }
//}