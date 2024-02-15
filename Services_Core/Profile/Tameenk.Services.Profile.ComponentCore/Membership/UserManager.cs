﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Notifications;

namespace Tameenk.Services.Profile.Component.Membership
{
    public class UserManager : // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
UserManager<AspNetUser>
    {
        public UserManager(IUserStore<AspNetUser> store)
            : base(store)
        {
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(new UserStore<AspNetUser>(EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<AspNetUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("EnglishPhoneCodeMessage", new PhoneNumberTokenProvider<AspNetUser>
            {
                MessageFormat = "Your #Tameenk security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("ArabicPhoneCodeMessage", new PhoneNumberTokenProvider<AspNetUser>
            {
                MessageFormat = "رمز تحقق #تأمينك من بي كير هو: {0}"
            });
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ClientUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your Security Code is{0}"
            //});
            //manager.EmailService = new EmailService();

            manager.SmsService = new SmsService(EngineContext.Current.Resolve<ISmsProvider>());
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<AspNetUser>(dataProtectionProvider.Create("TameenkIdentityConfirmation"));
            }
            return manager;
        }
    }
}
