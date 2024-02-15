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
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Repositories;
using Tameenk.Services.Administration.Identity.Services;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Files;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Invoices;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Products;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Attachments;
using Tameenk.Services.Implementation.Drivers;
using Tameenk.Services.Implementation.Excel;
using Tameenk.Services.Implementation.Files;
using Tameenk.Services.Implementation.Http;
using Tameenk.Services.Implementation.InsuranceCompanies;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Implementation.Notifications;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Products;
using Tameenk.Services.Implementation.Promotions;
using Tameenk.Services.Implementation.Vehicles;
using Tameenk.Services.Logging;

namespace Tameenk.Services.AdministrationApi.Infrastructure
{
    public static class AuthDependencyRegistrar 
    {

        public static void AuthRegister(this ContainerBuilder builder)
        {
            //Ahmed Hassan
            builder.Register<IIdentityDbContext>(c => new AdminIdentityContext()).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(DataRepository<>)).As(typeof(IDataRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CUDRepository<>)).As(typeof(ICUDRepository<>)).InstancePerLifetimeScope();
          
            builder.RegisterType<PageRepository>().As<IPageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserPageRepository>().As<IUserPageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserLoginsConfirmationRepository>().As<IUserLoginsConfirmationRepository>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(CRUDServiceBase<,>)).As(typeof(ICRUDService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ServiceBase<,>)).As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterType<PageService>().As<IPageService>().InstancePerLifetimeScope();
         
            builder.RegisterType<UserPageService>().As<IUserPageService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<UserLoginsConfirmationService>().As<IUserLoginsConfirmationService>().InstancePerLifetimeScope();
            builder.RegisterType<ExpiredTokensService>().As<IExpiredTokensService>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomCardQueueService>().As<ICustomCardQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<ExpiredTokensRepository>().As<IExpiredTokensRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UsersLocationsDeviceInfoRepository>().As<IUsersLocationsDeviceInfoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UsersLocationsDeviceInfoService>().As<IUsersLocationsDeviceInfoService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            



        }
    }
}