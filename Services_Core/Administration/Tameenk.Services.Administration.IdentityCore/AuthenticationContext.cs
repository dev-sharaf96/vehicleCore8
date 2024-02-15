
using DeviceDetectorNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Output;
using Tameenk.Services.Administration.Identity.Repositories;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Administration.Identity
{
    public class AuthenticationContext : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private AdminIdentityContext identityDbContext;


        public AuthenticationContext(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }
            _publicClientId = publicClientId;

        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            LoginRequestsLog log = new LoginRequestsLog();
            identityDbContext = new AdminIdentityContext();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Channel = "DashBoard";
            try
            {
                var userManager = new ApplicationUserManager(new UserStore<AppUser, RoleEntity, int, UserLogin, UserRole, UserClaim>(identityDbContext));
                var keybytes = Encoding.UTF8.GetBytes("BcARe_2021_N0MeM");
                var iv = Encoding.UTF8.GetBytes("BcARe_2021_N0MeM");

                if (string.IsNullOrEmpty(context.UserName))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Email is empty";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                if (string.IsNullOrEmpty(context.Password))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "Password is empty";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                var encryptedUserName = Convert.FromBase64String(context.UserName);
                var encryptedPassword = Convert.FromBase64String(context.Password);
                var plainUserName = SecurityUtilities.DecryptStringFromBytes_AES(encryptedUserName, keybytes, iv);
                var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, keybytes, iv);
                AppUser user = await userManager.FindAsync(plainUserName, plainPassword);
                if (user == null)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "invalid username or password";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                log.UserID = user.Id.ToString();
                log.Email = user.UserName;
                if (!user.IsActivated)
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = "The user is not active";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "Your account is blocked");
                    return;
                }
                var form = await context.Request.ReadFormAsync();
                var code = form.Get("confirmation_code");
                if (string.IsNullOrEmpty(code))
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "confirmation code is null";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "Please Enter the verification code");
                    return;
                }
                var encryptedUCode = Convert.FromBase64String(code);
                var plainCode = SecurityUtilities.DecryptStringFromBytes_AES(encryptedUCode, keybytes, iv);
                int number = 0;
                if (!int.TryParse(plainCode, out number))
                {
                    log.ErrorCode = 9;
                    log.ErrorDescription = "invalid code format as we received " + plainCode;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "Please enter a valid verification code");
                    return;
                }
                var userData = LoadUserConfirmationCode(identityDbContext, user.Id);
                if (userData == null)
                {
                    log.ErrorCode = 6;
                    log.ErrorDescription = "The user is has no code in db with this user";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "Invalid verification code");
                    return;
                }
                if (!userData.ConfirmationCode.ToString().Equals(plainCode, StringComparison.Ordinal))
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "Verification code is in correct since the code is " + plainCode;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The verification code is incorrect.");
                    return;
                }
                if (userData.CodeExpiryDate.CompareTo(DateTime.Now.AddSeconds(-59)) < 0)
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "confirmation code is expired";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The verification code is expired");
                    return;
                }
                userData.IsCodeConfirmed = true;
                userData.CodeExpiryDate = userData.CodeExpiryDate.AddMinutes(-1);
                string exception = string.Empty;
                if (!UpdateConfirmation(userData, out exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    context.SetError("invalid_grant", "The service is currently down, Please try again later");
                    return;
                }

                ClaimsIdentity oAuthIdentity = await GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType, user);
                ClaimsIdentity cookiesIdentity = await GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType, user);
                var pages = LoadPages(identityDbContext, user.Id);
                AuthenticationProperties properties = CreateProperties(user.UserName, Newtonsoft.Json.JsonConvert.SerializeObject(pages), user.ChangePasswordAfterLogin, user.Name, user.IsAdmin);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);

                exception = string.Empty;
                var deviceInfo = AddDeviceInfo(user);
                if (deviceInfo.ErrorCode == Output<DeviceInfo>.ErrorCodes.DeviceAdded)
                {
                    //send sms
                    var notificationService = EngineContext.Current.Resolve<INotificationService>();
                    string message = string.Empty;
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        if (!string.IsNullOrEmpty(deviceInfo.Result.DeviceName))
                        {
                            message = "New Device signed-in on an " + deviceInfo.Result.DeviceName + " device with " + user.UserName + " your account is at risk if this wasn't you please change password ";
                        }
                        else
                        {
                            message = "New Device signed-in on an " + deviceInfo.Result.OS + " device with " + user.UserName + " your account is at risk if this wasn't you please change password ";

                        }
                        var smsModel = new SMSModel() {
                            PhoneNumber = user.PhoneNumber,
                            MessageBody = message,
                            Method = SMSMethod.NewDeviceNotification.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = Channel.Dashboard.ToString()
                        };
                        notificationService.SendSmsBySMSProviderSettings(smsModel);
                    }
                    log.ErrorCode = 1;
                    log.ErrorDescription = "New Device for login";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                }
                else if (deviceInfo.ErrorCode == Output<DeviceInfo>.ErrorCodes.DeviceInfoIsNull
                    || deviceInfo.ErrorCode == Output<DeviceInfo>.ErrorCodes.ServiceDown)
                {
                    //send sms
                    log.ErrorCode = 1;
                    log.ErrorDescription = "there is an error due to " + deviceInfo.ErrorDescription;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                }
                else
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                }
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                context.SetError("invalid_grant", "The service is currently down, Please try again later");
                return;
            }
        }

        private dynamic LoadPages(AdminIdentityContext identityDbContext, int userId)
        {
            return ((System.Data.Entity.DbSet<UserPage>)identityDbContext.Set<UserPage>())
                             .Include("Page.Children")
                             .Where(x => x.UserId == userId && (x.Page.IsActive || x.Page.Children.Any(p => p.IsActive)))
                             .Select(x => new { x.Page }).ToList();
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string pages, bool ChangePassword, string Name, bool IsAdmin)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "pages", pages },
                { "changePassword",ChangePassword.ToString()},
                { "name",Name},
                { "isAdmin",IsAdmin.ToString()}
            };
            return new AuthenticationProperties(data);
        }
        public bool AddAllowedDevice(AppUser userObject, DeviceDetector deviceInfo, out string exception)
        {
            exception = string.Empty;
            try
            {
                var userDevicesLocationService = EngineContext.Current.Resolve<IUsersLocationsDeviceInfoService>();
                UsersLocationsDeviceInfo userDeviceInfo = new UsersLocationsDeviceInfo();
                userDeviceInfo.ServerIP = Utilities.GetInternalServerIP();
                userDeviceInfo.UserIP = Utilities.GetUserIPAddress();
                userDeviceInfo.UserId = userObject.Id.ToString();
                userDeviceInfo.CreatedDate = DateTime.Now;
                if (deviceInfo != null)
                {
                    userDeviceInfo.DeviceName = deviceInfo.GetBrandName() + " " + deviceInfo.GetModel();
                    userDeviceInfo.OS = deviceInfo.GetOs().Match.Name + " " + deviceInfo.GetOs().Match.Platform + " " + deviceInfo.GetOs().Match.Version;
                    userDeviceInfo.DeviceType = deviceInfo.GetDeviceName();
                    userDeviceInfo.Client = deviceInfo.GetBrowserClient().Match.Name + " " + deviceInfo.GetBrowserClient().Match.Version;
                }
                userDevicesLocationService.Add(userDeviceInfo);
                userDevicesLocationService.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;

            }
        }
        public Output<DeviceInfo> AddDeviceInfo(AppUser userObject)
        {
            Output<DeviceInfo> output = new Output<DeviceInfo>();
            try
            {
                var userDevicesLocationService = EngineContext.Current.Resolve<IUsersLocationsDeviceInfoService>();
                UsersLocationsDeviceInfo userDeviceInfo = new UsersLocationsDeviceInfo();
                userDeviceInfo.ServerIP = Utilities.GetInternalServerIP();
                userDeviceInfo.UserIP = Utilities.GetUserIPAddress();
                userDeviceInfo.UserId = userObject.Id.ToString();
                userDeviceInfo.CreatedDate = DateTime.Now;
                userDeviceInfo.UserName = userObject.UserName;
                var deviceInfo = new DeviceDetector(Utilities.GetUserAgent());
                deviceInfo.Parse();
                if (deviceInfo == null)
                {
                    output.ErrorCode = Output<DeviceInfo>.ErrorCodes.DeviceInfoIsNull;
                    output.ErrorDescription = "deviceInfo is null";
                    return output;
                }
                output.Result = new DeviceInfo();
                output.Result.DeviceName = deviceInfo.GetBrandName() + " " + deviceInfo.GetModel();
                output.Result.OS = deviceInfo.GetOs().Match.Name + " " + deviceInfo.GetOs().Match.Platform + " " + deviceInfo.GetOs().Match.Version;
                output.Result.DeviceType = deviceInfo.GetDeviceName();
                output.Result.Client = deviceInfo.GetBrowserClient().Match.Name + " " + deviceInfo.GetBrowserClient().Match.Version;
                var info = userDevicesLocationService.Find(x => x.UserId == userObject.Id.ToString() && x.OS == output.Result.OS && x.DeviceType == output.Result.DeviceType && x.DeviceName == output.Result.DeviceName && x.Client == output.Result.Client).FirstOrDefault();
                if (info != null)
                {
                    output.ErrorCode = Output<DeviceInfo>.ErrorCodes.DeviceAlreadyExists;
                    output.ErrorDescription = "deviceInfo already exists";
                    return output;
                }
                userDeviceInfo.OS = output.Result.OS;
                userDeviceInfo.DeviceType = output.Result.DeviceType;
                userDeviceInfo.DeviceName = output.Result.DeviceName;
                userDeviceInfo.Client = output.Result.Client;
                userDevicesLocationService.Add(userDeviceInfo);
                userDevicesLocationService.SaveChanges();
                output.ErrorCode = Output<DeviceInfo>.ErrorCodes.DeviceAdded;
                output.ErrorDescription = "DeviceAdded";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<DeviceInfo>.ErrorCodes.ServiceDown;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
UserManager<AppUser, int> manager, string authenticationType, AppUser user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Name", user.UserName));
            userIdentity.AddClaim(new Claim("Email", user.Email));
            userIdentity.AddClaim(new Claim("Id", user.Id.ToString()));
            userIdentity.AddClaim(new Claim("PageIds", JsonConvert.SerializeObject(user.PageIds)));
            userIdentity.AddClaim(new Claim("dashboardUser", "true"));
            userIdentity.AddClaim(new Claim("key", Guid.NewGuid().ToString()));
            return userIdentity;
        }

        private UserLoginsConfirmation LoadUserConfirmationCode(AdminIdentityContext identityDbContext, int userId)
        {
            return ((System.Data.Entity.DbSet<UserLoginsConfirmation>)identityDbContext.Set<UserLoginsConfirmation>())
                             .Where(x => x.UserId == userId && x.IsCodeConfirmed == false && x.IsDeleted == false)
                             .OrderByDescending(x => x.Id).FirstOrDefault();
        }

        private bool UpdateConfirmation(UserLoginsConfirmation model, out string exception)
        {
            exception = string.Empty;
            try
            {
                var userLoginsConfirmationService = EngineContext.Current.Resolve<IUserLoginsConfirmationService>();
                userLoginsConfirmationService.Update(model);
                userLoginsConfirmationService.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
    }
}
