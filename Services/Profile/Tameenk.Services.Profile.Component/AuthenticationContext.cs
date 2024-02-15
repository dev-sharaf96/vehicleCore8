using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
using Tameenk.Redis;
using Tameenk.Resources.Inquiry;
using Tameenk.Resources.Profile;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Implementation;
using Tameenk.Services.Orders;
using Tameenk.Services.Profile.Component.Membership;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.VehicleYakeenBcareService;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;

namespace Tameenk.Services.Profile.Component
{
    public class AuthenticationContext : IAuthenticationContext
    {
        private UserManager _userManager;
        private ClientSignInManager _signInManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICorporateContext _iCorporateContext;
        private readonly IQuotationService _quotationService;
        private readonly INotificationService _notificationService;
        private readonly IYakeenClient _iYakeenClient;
        private readonly IRepository<OtpInfo> _otpInfo;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IHttpClient _httpClient;
        private readonly IRepository<LoginSwitchAccount> _loginSwitchAccountRepository;
        private readonly IRepository<LoginActiveTokens> _loginActiveTokensRepository;
        private readonly IRepository<ExpiredTokens> _expiredTokensRepository;
        private readonly IRepository<ForgotPasswordToken> _forgotPasswordTokenRepository;

        private const string SHARED_KEY = "Tameenk_BcARe_2021_N0MeM_SharedKey_@$";
        private readonly string GeneralAccessTokenCredentials = "Bcare:m3~dsMbYjLbSPRmJ";
        private readonly string General_travel_URL = "https://gapis.bcare.com.sa/Identity/api/Account/GetUserData";
        private readonly string General_mmalpractice_URL = "https://mmapi.bcare.com.sa/IdentityApi/api/Account/GetUserData";

        private readonly string General_travel_ForgotPasswordConfirmation_URL = "https://travel.bcare.com.sa/(auth:auth/userId)?userId=";
        private readonly string General_mmalpractice_ForgotPasswordConfirmation_URL = "https://mm.bcare.com.sa/(auth:auth/userId)?userId=";

        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";
        private readonly string SHARED_PARTIAL_LOCK_SECRET = "(hrd8Af#qtuZ!93myA%5t^4uDU$A+zx!";
        private readonly string base_KEY = "iDeNtItY_cAcH";

        private readonly List<string> generalChannels = new List<string>()
        {
            Channel.Travel.ToString().ToLower(),
            Channel.Medical.ToString().ToLower(),
            Channel.Home.ToString().ToLower(),
            Channel.Aman.ToString().ToLower(),
            Channel.MedicalMalpractices.ToString().ToLower()
        };
        private readonly List<string> appChannels = new List<string>()
        {
            Channel.ios.ToString().ToLower(),
            Channel.android.ToString().ToLower(),
            Channel.Huawei.ToString().ToLower()
        };

        private readonly List<string> emailsToSkipSendingOTP = new List<string>()
        {
            "f.alazhary@bcare.com.sa"
        };

        private readonly List<string> invalidKeyWords = new List<string>()
        {
            "select",
            "update",
            "delete"
        };

        public AuthenticationContext(IShoppingCartService shoppingCartService, IAuthorizationService authorizationService, ICorporateContext iCorporateContext, IQuotationService quotationService,
            INotificationService notificationService, IYakeenClient iYakeenClient, IRepository<OtpInfo> otpInfo, IRepository<CorporateUsers> corporateUsersRepository,
            IRepository<Driver> driverRepository, IRepository<LoginSwitchAccount> loginSwitchAccountRepository, IRepository<LoginActiveTokens> loginActiveTokensRepository,
            IRepository<ExpiredTokens> expiredTokensRepository, IRepository<ForgotPasswordToken> forgotPasswordTokenRepository)
        {
            this._shoppingCartService = shoppingCartService;
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            this._iCorporateContext = iCorporateContext;
            _quotationService = quotationService;
            _notificationService = notificationService;
            _iYakeenClient = iYakeenClient;
            _otpInfo = otpInfo;
            _corporateUsersRepository = corporateUsersRepository;
            _driverRepository = driverRepository;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _notificationService = notificationService;
            _loginSwitchAccountRepository = loginSwitchAccountRepository;
            _loginActiveTokensRepository = loginActiveTokensRepository;
            _expiredTokensRepository = expiredTokensRepository;
            _forgotPasswordTokenRepository = forgotPasswordTokenRepository;
        }
        public async Task<ProfileOutput<LoginOutput>> Login(LoginViewModel model, string returnUrl = null)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            LoginRequestsLog log = new LoginRequestsLog();
            log.Email = model.Email;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //loginLog.Password = model.Password;
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Email is empty";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "password is empty";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }

                // TODO add remember me
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "user is null";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                log.UserID = user.Id;
                log.Mobile = user.PhoneNumber;
                //if (!user.PhoneNumberConfirmed)
                //{
                //    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
                //    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "Phone number is not Confirmed";
                //    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                //    return output;
                //}
                if (!user.EmailConfirmed)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "email is not Confirmed";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }
                if (user.LockoutEndDateUtc > DateTime.UtcNow)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.AccountLocked;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Account is Locked";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return output;
                }

                if (user.IsCorporateUser)
                    return await _iCorporateContext.CorporateUserSignIn(user, model.Password, model.Language, log);

                var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            string exception = string.Empty;
                            bool value = SetUserAuthenticationCookies(user, model.Channel, out exception);
                            if (!value)
                            {
                                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
                                output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = "Failed to save authentication Cookie due to : " + exception;
                                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                                return output;
                            }

                            var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                            if (accessTokenResult == null)
                            {
                                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NullResult;
                                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = "accessTokenResult is null";
                                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                                return output;
                            }

                            if (string.IsNullOrEmpty(accessTokenResult.access_token))
                            {
                                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyResult;
                                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = "accessTokenResult.access_token is null";
                                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                                return output;
                            }

                            string accessTokenResultGWT = string.Empty;
                            if (model.Channel.ToString().ToLower() == Channel.Travel.ToString().ToLower()
                              || model.Channel.ToString().ToLower() == Channel.Medical.ToString().ToLower()
                              || model.Channel.ToString().ToLower() == Channel.Home.ToString().ToLower()
                              || model.Channel.ToString().ToLower() == Channel.Aman.ToString().ToLower())
                            {
                                string gwtKey = string.Empty;
                                accessTokenResultGWT = _authorizationService.GenerateTokenJWT(user.Id.ToString(), user.Email, user.UserName,user.PhoneNumber, user.FullNameAr, user.FullName, out gwtKey);
                            }

                            output.Result = new LoginOutput { UserId = user.Id, AccessToken = accessTokenResult.access_token, Email = user.Email, AccessTokenGwt = accessTokenResultGWT, TokenExpiryDate = 30 };
                            MigrateUser(user.Id);
                            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Success";
                            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                            return output;
                        }
                    case SignInStatus.LockedOut:
                        {
                            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Lockout;
                            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "This Account is LockedOut";
                            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                            return output;
                        }
                    case SignInStatus.RequiresVerification:
                        {
                            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
                            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Phone number is not Confirmed";
                            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                            return output;
                        }
                    case SignInStatus.Failure:
                        {
                            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
                            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "SignInStatus.Failure";
                            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                            return output;
                        }
                    default:
                        {
                            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
                            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "incorrect username and password";
                            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                            return output;
                        }
                }
            }
            catch (Exception exp)
            {
                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                return output;
            }
        }

        public async Task<ProfileOutput<RegisterOutput>> Register(RegisterationModel model)
        {
            ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Method = "Register";
            log.Mobile = model.Mobile;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            var Language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "mail is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (!Utilities.IsValidMail(model.Email))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"mail is not valid: {model.Email}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Password is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                if (string.IsNullOrEmpty(model.Mobile))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Mobile is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (!Utilities.IsValidPhoneNo(model.Mobile))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"Mobile is not valid: {model.Mobile}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                var userInfo = UserManager.Users.Where(x => x.Email == model.Email || x.PhoneNumber == model.Mobile).FirstOrDefault();

                if (userInfo != null && userInfo.Email == model.Email && userInfo.EmailConfirmed && userInfo.PhoneNumberConfirmed)
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmailAlreadyExist;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "email already exist";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                if (userInfo != null && userInfo.PhoneNumber == model.Mobile && userInfo.EmailConfirmed && userInfo.PhoneNumberConfirmed)
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.PhoneAlreadyExist;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "mobile already exist";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                if (userInfo != null)
                {
                    if (!userInfo.EmailConfirmed || !userInfo.PhoneNumberConfirmed)
                    {
                        string exception = string.Empty;
                        try
                        {
                            _quotationService.RemoveUserIdFromQuotationRequest(userInfo.Id,out exception);
                            var resultInfo = UserManager.Delete(userInfo);
                            if (!resultInfo.Succeeded)
                            {
                                Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
                                Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(Language));
                                log.ErrorCode = (int)Output.ErrorCode;
                                log.ErrorDescription = "can not delete user due to "+exception+"; " + string.Join(",", resultInfo.Errors);
                                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                                return Output;
                            }
                        }
                        catch (Exception ex)
                        {
                            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
                            Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(Language));
                            log.ErrorCode = (int)Output.ErrorCode;
                            log.ErrorDescription = "can not delete user due to "+exception+"; " + ex.ToString();
                            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                            return Output;
                        }
                    }
                }

                var user = new AspNetUser
                {
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    LockoutEndDateUtc = DateTime.UtcNow.AddDays(-1),
                    DeviceToken = "",
                    Email = model.Email,
                    EmailConfirmed = true,
                    RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"), //_db.Roles.ToList()[0].ID,
                    LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"), //_db.Languages.Where(l => l.isDefault).Select(l => l.Id).SingleOrDefault(),
                    PhoneNumber = model.Mobile,
                    PhoneNumberConfirmed = false,
                    UserName = model.Email,
                    //  FullName = model.FullName
                    FullName = "",
                    TwoFactorEnabled = false,
                    Channel = model.Channel.ToString()
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.CanNotCreate; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "can not create user due to " + string.Join(",", result.Errors);
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                //// send sms 
                if (model.Channel == Channel.ios || model.Channel == Channel.android)
                {
                    if (!await _authorizationService.SendTwoFactorCodeSmsAsync(user.Id, user.PhoneNumber))
                    {
                        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.CanNotSendSMS; ;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "Can Not Send SMS";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Output;
                    }
                }

                if (model.Channel == Channel.Portal)
                {
                    var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                    if (accessTokenResult == null)
                    {
                        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.NullResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "accessTokenResult is null";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Output;
                    }

                    if (string.IsNullOrEmpty(accessTokenResult.access_token))
                    {
                        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "accessTokenResult.access_token is null";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Output;
                    }

                    Output.Result.AccessToken = accessTokenResult.access_token;
                    Output.Result.Email = user.Email;
                }

                MigrateUser(user.Id);
                Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
                var RegisteredUser = UserManager.FindByEmailAsync(model.Email).Result;
                Output.Result.UserId = RegisteredUser.Id;
                Output.Result.PhoneNumber = RegisteredUser.PhoneNumber;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }

        }
        public bool SetUserAuthenticationCookies(AspNetUser userObject, Channel channel, out string exception)
        {
            exception = string.Empty;
            try
            {
                string userTicketData = string.Empty;

                #region Build User Ticket Data String
                userTicketData = "UserID=" + userObject.Id + ";"
                + "Email=" + userObject.Email + ";"
                + "CreatedDate=" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + ";"
                + "Key=" + Guid.NewGuid().ToString();

                #endregion
                #region Set main first ticket (For Non-SSL Mode) object
                HttpCookie cookieMain = new HttpCookie("_authCookie");
                cookieMain.HttpOnly = true;
                //cookieMain.Expires = DateTime.Now.AddDays(1);
                cookieMain.Expires = (appChannels.Contains(channel.ToString().ToLower())) ? DateTime.Now.AddDays(1) : DateTime.Now.AddMinutes(30);
                //Create a new FormsAuthenticationTicket that includes Custom User Data
                FormsAuthenticationTicket firstTicketUserData = new FormsAuthenticationTicket(1, userObject.Id, DateTime.Now
                    , cookieMain.Expires, false, userTicketData);
                //add cookie with the new ticket value
                cookieMain.Value = FormsAuthentication.Encrypt(firstTicketUserData);
                cookieMain.Secure = true;
                HttpContext.Current.Response.Cookies.Add(cookieMain);
                return true;
                #endregion


            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public bool SetCorporateUserAuthenticationCookies(CorporateUsers userObject, out string exception)
        {
            exception = string.Empty;
            try
            {
                string userTicketData = string.Empty;

                #region Build User Ticket Data String
                userTicketData = "UserID=" + userObject.UserId + ";"
                + "Email=" + userObject.UserName + ";"
                + "CreatedDate=" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + ";"
                + "Key=" + Guid.NewGuid().ToString() + ";"
                + "IsCorporateUser=" + userObject.IsActive.ToString().ToLower() + ";"
                + "IsCorporateSuperAdmin=" + userObject.IsSuperAdmin?.ToString().ToLower();

                #endregion
                #region Set main first ticket (For Non-SSL Mode) object
                HttpCookie cookieMain = new HttpCookie("_authCorporateCookie");
                cookieMain.HttpOnly = true;
                cookieMain.Expires = DateTime.Now.AddDays(1);
                //Create a new FormsAuthenticationTicket that includes Custom User Data
                FormsAuthenticationTicket firstTicketUserData = new FormsAuthenticationTicket(1, userObject.UserId, DateTime.Now
                    , cookieMain.Expires, false, userTicketData);
                //add cookie with the new ticket value
                cookieMain.Value = FormsAuthentication.Encrypt(firstTicketUserData);
                cookieMain.Secure = true;
                HttpContext.Current.Response.Cookies.Add(cookieMain);
                return true;
                #endregion
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public async Task<ProfileOutput<RegisterOutput>> ForgetPassword(ForgetPasswordModel model)
        {
            ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Method = "ForgetPassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "mail is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                var user = _authorizationService.GetUserInfoByEmail(model.Email, string.Empty, string.Empty);
                if (user == null)
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.UserNotFound;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User is null email: " + model.Email;
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if (string.IsNullOrEmpty(token))
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.TokenIsEmpty;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "token is null";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                string subject = WebResources.ResourceManager.GetString("ForgetPasswordEmailSubject", CultureInfo.GetCultureInfo(model.Language));

                string url = Utilities.SiteURL + "/ConfirmResetPassword?userId=" + user.Id + "&token=" + token;

                //string body = string.Format(WebResources.ResourceManager.GetString("ForgetPasswordEmailBody", CultureInfo.GetCultureInfo(model.Language)), url);

                string body = string.Format(WebResources.ResourceManager.GetString("EmailPasswordResetBody", CultureInfo.GetCultureInfo(model.Language)), url);
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PasswordReset.png";
                messageBodyModel.Language = model.Language;
                messageBodyModel.MessageBody = body;
                
                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(model.Email);
                emailModel.Subject = subject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "ForgetPassword";
                emailModel.Channel = model.Channel.ToString();
                var sendMail = _notificationService.SendEmail(emailModel);
                if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmailNotSent;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "can not send email, and error is: " + sendMail.ErrorDescription;
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
        }

        public async Task<ProfileOutput<bool>> ConfirmResetPassword(ConfirmResetPasswordModel model)
        {
            ProfileOutput<bool> Output = new ProfileOutput<bool>();
            Output.Result = false;
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "ConfirmResetPassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "UserId is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (string.IsNullOrEmpty(model.Token))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Token is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Password is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "ConfirmNewPassword is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (model.Password != model.ConfirmNewPassword)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NewPassordNotMatchConfirmNewPassword; ;
                    Output.ErrorDescription = ProfileResources.ResourceManager.GetString("password_confirm_error", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"Password : {model.Password} not equal ConfirmNewPassword:{model.ConfirmNewPassword}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                var user = await UserManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"There is no user with this id: {model.UserId}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                log.Email = user.Email;

                var forgotPasswordToken = _forgotPasswordTokenRepository.Table.Where(a => a.UserId == user.Id && a.ForgotPasswordVerificationType == (int)ForgotPasswordVerificationTypeEnum.Email).OrderByDescending(a => a.Id).FirstOrDefault();
                if (forgotPasswordToken == null || string.IsNullOrEmpty(forgotPasswordToken.Token))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"No reset password token for userId {user.Id}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                if (forgotPasswordToken.IsCodeVerified)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ActivationEmailUsedBefore", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "The activation emil is used before";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }
                if (forgotPasswordToken.CreatedDate.AddMinutes(10) < DateTime.Now)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ActivationEmailIsExpired", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "The activation emil is expired";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                var res = await UserManager.ResetPasswordAsync(model.UserId, Utilities.GetDecodeUrl(model.Token), model.Password);
                if (!res.Succeeded)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceDown;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = $"ResetPasswordAsync Failed and the error is: " + String.Join(",", res.Errors.ToList());
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                if (user.IsCorporateUser)
                {
                    var corporateUser = _corporateUsersRepository.Table.FirstOrDefault(u => u.UserId == user.Id && u.IsActive);
                    if (corporateUser != null)
                    {
                        corporateUser.PasswordHash = SecurityUtilities.HashData(model.Password, null);
                        _corporateUsersRepository.Update(corporateUser);
                    }
                }

                forgotPasswordToken.IsCodeVerified = true;
                forgotPasswordToken.ModifiedDate = DateTime.Now;
                _forgotPasswordTokenRepository.Update(forgotPasswordToken);

                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ForgotPasswordSuccess", CultureInfo.GetCultureInfo(model.Language));
                Output.Result = true;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
        }


        #region Change Password

        public async Task<ProfileOutput<bool>> ChangePassword(ChangePasswordViewModel model)
        {
            ProfileOutput<bool> Output = new ProfileOutput<bool>();
            Output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "ChangePassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserID = Guid.Parse(model.UserId);

            try
            {
                var validateRequestOutput = await ValidateChangePasswordReuest(model, false);
                if (validateRequestOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)validateRequestOutput.ErrorCode;
                    log.ErrorDescription = validateRequestOutput.LogDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return validateRequestOutput;
                }

                //var user = UserManager.FindById(model.UserId);
                var user = await _authorizationService.GetUser(model.UserId);
                log.Email = user.Email;

                var otpOutput = HandleSendingOTP(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "ChangePasswordVerifyOTP", "ChangePassword", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                    Output.ErrorDescription = otpOutput.ErrorDescription;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = otpOutput.LogDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Output;
                }
                //var otpOutput = SendOTPCode(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "ChangePasswordVerifyOTP", model.Language);
                //if (otpOutput.ErrorCode == 2)
                //{
                //    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                //    Output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(model.Language));
                //    log.ErrorCode = (int)Output.ErrorCode;
                //    log.ErrorDescription = $"ErrorCode == 2, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //    return Output;
                //}
                //if (otpOutput.ErrorCode != 0)
                //{
                //    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                //    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language));
                //    log.ErrorCode = (int)Output.ErrorCode;
                //    log.ErrorDescription = $"ErrorCode != 0, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //}

                Output.Result = true;
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = ProfileResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
        }
         
        public async Task<ProfileOutput<bool>> ChangePasswordConfirm(ChangePasswordViewModel model)
        {
            ProfileOutput<bool> Output = new ProfileOutput<bool>();
            Output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "ConfirmChangePassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserID = Guid.Parse(model.UserId);

            try
            {
                var validateRequestOutput = await ValidateChangePasswordReuest(model, true);
                if (validateRequestOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)validateRequestOutput.ErrorCode;
                    log.ErrorDescription = validateRequestOutput.LogDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return validateRequestOutput;
                }

                var result = await UserManager.ChangePasswordAsync(model.UserId, model.OldPassword, model.NewPassword);
                if (result.Errors.Any())
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.CanNotChangePassword;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "can not change password due to " + string.Join(",", result.Errors);
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Output;
                }

                Output.Result = true;
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = ProfileResources.ResourceManager.GetString("changePasswordSuccess", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
        }

        public async Task<ProfileOutput<bool>> ChangePasswordReSendOTP(ChangePasswordViewModel model)
        {
            ProfileOutput<bool> Output = new ProfileOutput<bool>();
            Output.Result = false;
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "ChangePasswordReSendOTP";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserID = Guid.Parse(model.UserId);

            try
            {
                var validateRequestOutput = await ValidateChangePasswordReuest(model, false);
                if (validateRequestOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)validateRequestOutput.ErrorCode;
                    log.ErrorDescription = validateRequestOutput.LogDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return validateRequestOutput;
                }

                //var user = UserManager.FindById(model.UserId);
                var user = await _authorizationService.GetUser(model.UserId);
                log.Email = user.Email;

                var otpOutput = HandleSendingOTP(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "ChangePasswordVerifyOTP", "ChangePasswordReSendOTP", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                    Output.ErrorDescription = otpOutput.ErrorDescription;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = otpOutput.LogDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return Output;
                }
                //var otpOutput = SendOTPCode(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "ChangePasswordVerifyOTP", model.Language);
                //if (otpOutput.ErrorCode == 2)
                //{
                //    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                //    Output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(model.Language));
                //    log.ErrorCode = (int)Output.ErrorCode;
                //    log.ErrorDescription = $"ErrorCode == 2, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //    return Output;
                //}
                //if (otpOutput.ErrorCode != 0)
                //{
                //    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToSendOtp;
                //    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language));
                //    log.ErrorCode = (int)Output.ErrorCode;
                //    log.ErrorDescription = $"ErrorCode != 0, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //}

                Output.Result = true;
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = ProfileResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return Output;
            }
        }

        private async Task<ProfileOutput<bool>> ValidateChangePasswordReuest(ChangePasswordViewModel model, bool validateOTP)
        {
            ProfileOutput<bool> Output = new ProfileOutput<bool>();
            Output.Result = false;
            
            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = "User Id is empty";
                    return Output;
                }
                if (string.IsNullOrEmpty(model.OldPassword))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = "Old Password is empty";
                    return Output;
                }
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = "New Password is empty";
                    return Output;
                }
                if (string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = "Confirm New Password is empty";
                    return Output;
                }
                if (!string.Equals(model.NewPassword, model.ConfirmNewPassword))
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NewPassordNotMatchConfirmNewPassword;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordsNotMatch", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = "New Passord Not Match Confirm New Password";
                    return Output;
                }

                var user = await _authorizationService.GetUser(model.UserId);
                if (user == null)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NotFound;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = $"No user found for this id: {model.UserId}";
                    return Output;
                }

                var result = SignInManager.UserManager.CheckPassword(user, model.OldPassword);
                if (!result)
                {
                    Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOldPassword", CultureInfo.GetCultureInfo(model.Language));
                    Output.LogDescription = $"Old password is not correct";
                    return Output;
                }

                if (validateOTP)
                {
                    if (!model.OTP.HasValue)
                    {
                        Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyOTP", CultureInfo.GetCultureInfo(model.Language));
                        Output.LogDescription = "Change password otp is empty";
                        return Output;
                    }

                    var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == user.PhoneNumber && a.UserEmail == user.Email && a.Nin == user.NationalId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (otpInfo == null)
                    {
                        Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        Output.LogDescription = "Change otp info is null";
                        return Output;
                    }
                    else if (otpInfo.VerificationCode != model.OTP)
                    {
                        Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        Output.LogDescription = "Change password invalid otp as we recived:" + model.OTP + " and correct one is:" + otpInfo.VerificationCode;
                        return Output;
                    }
                    else if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
                    {
                        Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.OTPExpire;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        Output.LogDescription = "Change password otp expire";
                        return Output;
                    }

                    otpInfo.IsCodeVerified = true;
                    otpInfo.ModifiedDate = DateTime.Now;
                    _otpInfo.Update(otpInfo);
                }

                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                Output.Result = true;
                return Output;
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                Output.LogDescription = $"ValidateChangePasswordReuest exception: {ex.ToString()}";
                return Output;
            }
        }

        #endregion

        #region  Helper
        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ClientSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ClientSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        private void MigrateUser(string userId)
        {
            var anonymousId = HttpContext.Current.Request.AnonymousID;
            // var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
            if (string.IsNullOrWhiteSpace(anonymousId)) return;

            if (string.IsNullOrWhiteSpace(userId)) return;

            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
        }
        #endregion

        public async Task<ConfirmOutPutModel> ConfirmPhoneAndEmail(ConfirmModel model)
        {
            ConfirmOutPutModel output = new ConfirmOutPutModel();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "ConfirmPhoneAndEmail";
            log.Channel = model.Channel;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.InvalidData;
                    output.ErrorDescription = "";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.UserId))
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.InvalidData;
                    output.ErrorDescription = "";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userId is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                string clearText = string.Format("{0}_{1}_{2}", true, model.UserId, SecurityUtilities.HashKey);
                var hashed = SecurityUtilities.HashData(clearText, null);
                if (!SecurityUtilities.VerifyHashedData(model.Hashed, clearText))
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + model.Hashed;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (!model.ConfirmEmail && !model.ConfirmPhone)
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.InvalidData;
                    output.ErrorDescription = "email and phone are false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                var user = await _authorizationService.GetUser(model.UserId);
                if (user == null)
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.NullResult;
                    output.ErrorDescription = "User Not Found";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                string Exception = string.Empty;

                if (model.ConfirmPhone)
                    user.PhoneNumberConfirmed = true;
                if (model.ConfirmEmail)
                    user.EmailConfirmed = true;
                bool result = _authorizationService.UpdateUserInfo(user, out Exception);
                if (!result || !string.IsNullOrEmpty(Exception))
                {
                    output.ErrorCode = ConfirmOutPutModel.ErrorCodes.UserNotFound;
                    output.ErrorDescription = "failed to Update User due to:" + Exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                output.ErrorCode = ConfirmOutPutModel.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ConfirmOutPutModel.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }
        //public async Task<ProfileOutput<RegisterOutput>> BeginRegister(RegisterationModel model)
        //{
        //    ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
        //    Output.Result = new RegisterOutput();
        //    RegistrationRequestsLog log = new RegistrationRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "BeginRegister";
        //    log.Mobile = model?.Mobile;
        //    log.Channel = model?.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.Nin = model?.NationalId;
        //    var Language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        //    try
        //    {
        //        ProfileOutput<RegisterOutput> validateOutput = ValidateRegistrationRequest(model);
        //        if(validateOutput.ErrorCode!=ProfileOutput<RegisterOutput>.ErrorCodes.Success)
        //        {
        //            Output.ErrorCode = validateOutput.ErrorCode;
        //            Output.ErrorDescription = validateOutput.ErrorDescription;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = validateOutput.LogDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        var yakeenMobileVerificationDto = new YakeenMobileVerificationDto();
        //        yakeenMobileVerificationDto.NationalId = model.NationalId;
        //        yakeenMobileVerificationDto.Phone = model.Mobile;
        //        var yakeenMobileVerification = _iYakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, Language);
        //        // Mobile verification failed return
        //        if (yakeenMobileVerification.ErrorCode == YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.VerificationFaield;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = yakeenMobileVerification.ErrorDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.VerificationFaield;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = yakeenMobileVerification.ErrorDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        var otpOutput = SendOTPCode(model.Mobile, model.Email, model.NationalId, Guid.Empty, SMSMethod.WebsiteOTP);
        //        if (otpOutput.ErrorCode==2)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToSendOtp;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "Failed To Send Otp due to:"+ otpOutput.ErrorDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        }
        //        if (otpOutput.ErrorCode != 0)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToSendOtp;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "Failed To Send Otp due to:"+otpOutput.ErrorDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        }
        //        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = "Success";
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Output;
        //    }
        //    catch (Exception ex)
        //    {
        //        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError;
        //        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Output;
        //    }
        //}
        //public async Task<ProfileOutput<RegisterOutput>> EndRegister(RegisterationModel model)
        //{
        //    ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
        //    Output.Result = new RegisterOutput();
        //    RegistrationRequestsLog log = new RegistrationRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "EndRegister";
        //    log.Mobile = model.Mobile;
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.Nin = model?.NationalId;
        //    var Language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        //    try
        //    {
        //        ProfileOutput<RegisterOutput> validateOutput = ValidateRegistrationRequest(model);
        //        if (validateOutput.ErrorCode != ProfileOutput<RegisterOutput>.ErrorCodes.Success)
        //        {
        //            Output.ErrorCode = validateOutput.ErrorCode;
        //            Output.ErrorDescription = validateOutput.ErrorDescription;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = validateOutput.LogDescription;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        if (!model.OTP.HasValue)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPEmpty", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "OTP is empty";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == model.Mobile && a.UserEmail == model.Email && a.Nin == model.NationalId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
        //        if(otpInfo==null)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "OTP info is null";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        if(otpInfo.VerificationCode!=model.OTP)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "Invalid otp as we recived:"+model.OTP +" and correct one is:"+otpInfo.VerificationCode;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.OTPExpire;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(Language));
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "OTP Expire";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        otpInfo.IsCodeVerified = true;
        //        otpInfo.ModifiedDate = DateTime.Now;
        //        _otpInfo.Update(otpInfo);

        //        var yakeeenOutput = YakeenRequestLogDataAccess.GetYakeenMobileVerification(model.NationalId);
        //        if(yakeeenOutput==null)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "yakeeenOutput is null";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(yakeeenOutput.ServiceResponse);
        //        MobileVerificationModel responseObject = JsonConvert.DeserializeObject<MobileVerificationModel>(responseAfterDeserialize.ToString());
        //         if(!responseObject.isOwner)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "national id:"+model.NationalId+" is not the owner of mobile phone:"+model.Mobile;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        var user = new AspNetUser
        //        {
        //            CreatedDate = DateTime.Now,
        //            LastModifiedDate = DateTime.Now,
        //            LastLoginDate = DateTime.Now,
        //            LockoutEndDateUtc = DateTime.UtcNow.AddDays(-1),
        //            DeviceToken = "",
        //            Email = model.Email,
        //            EmailConfirmed = true,
        //            RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"),
        //            LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"),
        //            PhoneNumber = model.Mobile,
        //            PhoneNumberConfirmed = true,
        //            UserName = model.Email,
        //            FullName = "",
        //            TwoFactorEnabled = false,
        //            Channel = model.Channel.ToString(),
        //            IsPhoneVerifiedByYakeen = true,
        //            NationalId=model.NationalId
        //        };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (!result.Succeeded)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.CanNotCreate; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "can not create user due to " + string.Join(",", result.Errors);
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
        //        if (accessTokenResult == null)
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.NullResult;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "accessTokenResult is null";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        if (string.IsNullOrEmpty(accessTokenResult.access_token))
        //        {
        //            Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyResult;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "accessTokenResult.access_token is null";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Output;
        //        }
        //        Output.Result.AccessToken = accessTokenResult.access_token;
        //        Output.Result.Email = user.Email;
        //        MigrateUser(user.Id);
        //        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
        //        var RegisteredUser = UserManager.FindByEmailAsync(model.Email).Result;
        //        Output.Result.UserId = RegisteredUser.Id;
        //        Output.Result.PhoneNumber = RegisteredUser.PhoneNumber;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = "Success";
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Output;
        //    }
        //    catch (Exception ex)
        //    {
        //        Output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError; ;
        //        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Language)); ;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Output;
        //    }
        //}
        //public ProfileOutput<RegisterOutput> ValidateRegistrationRequest(RegisterationModel model)
        //{
        //    ProfileOutput<RegisterOutput> output = new ProfileOutput<RegisterOutput>();
        //    try
        //    {
        //        if (model == null)
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "model is empty";
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(model.Email))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "mail is empty";
        //            return output;
        //        }
        //        if (!Utilities.IsValidMail(model.Email))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = $"mail is not valid: {model.Email}";
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(model.Password))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "Password is empty";
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(model.Mobile))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("MobileEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "Mobile is empty";
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(model.NationalId))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "NationalId is empty";
        //            return output;
        //        }
        //        if (!Utilities.IsValidPhoneNo(model.Mobile))
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
        //            output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = $"Mobile is not valid: {model.Mobile}";
        //            return output;
        //        }
        //        var userInfo = UserManager.Users.Where(x => x.Email == model.Email || x.PhoneNumber == model.Mobile).FirstOrDefault();

        //        if (userInfo != null && userInfo.Email == model.Email && userInfo.EmailConfirmed && userInfo.PhoneNumberConfirmed)
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmailAlreadyExist;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
        //            output.LogDescription = "email already exist";
        //            return output;
        //        }
        //        if (userInfo != null && userInfo.PhoneNumber == model.Mobile && userInfo.EmailConfirmed && userInfo.PhoneNumberConfirmed)
        //        {
        //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.PhoneAlreadyExist;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(model.Language));
        //            output.LogDescription = "mobile already exist";
        //            return output;
        //        }
        //        if (userInfo != null && (!userInfo.EmailConfirmed || !userInfo.PhoneNumberConfirmed || !userInfo.IsPhoneVerifiedByYakeen))
        //        {
        //            string exception = string.Empty;
        //            try
        //            {
        //                _quotationService.RemoveUserIdFromQuotationRequest(userInfo.Id, out exception);
        //                var resultInfo = UserManager.Delete(userInfo);
        //                if (!resultInfo.Succeeded)
        //                {
        //                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
        //                    output.LogDescription = "can not delete user due to " + exception + "; " + string.Join(",", resultInfo.Errors);
        //                    return output;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
        //                output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
        //                output.LogDescription = "can not delete user due to " + exception + "; " + ex.ToString();
        //                return output;
        //            }
        //        }
        //        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        output.LogDescription = "Success";
        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
        //        output.LogDescription = ex.ToString();
        //        return output;
        //    }
        //}
        //public async Task<ProfileOutput<LoginOutput>> BeginLogin(LoginViewModel model, string returnUrl = null)
        //{
        //    ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
        //    LoginRequestsLog log = new LoginRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "BeginLogin";
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    try
        //    {
        //        ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model,false);
        //        if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = validateOutput.ErrorCode;
        //            output.ErrorDescription = validateOutput.ErrorDescription;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = validateOutput.LogDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        // TODO add remember me
        //        var user = UserManager.FindByEmail(model.Email);
        //        if (user == null)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "user is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        log.UserID = user.Id;
        //        log.Mobile = user.PhoneNumber;
        //        if (!user.EmailConfirmed)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "email is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (user.LockoutEndDateUtc > DateTime.UtcNow)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.AccountLocked;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Account is Locked";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (user.IsCorporateUser)
        //            return await _iCorporateContext.CorporateUserSignIn(user.Email, model.Password, model.Language, log);
        //        var result = SignInManager.PasswordSignIn(user.UserName, model.Password, true, shouldLockout: false);
        //        switch (result)
        //        {
        //            case SignInStatus.Success:
        //                {
        //                    // check if existing user not verified from Yakeen
        //                    if (!user.IsPhoneVerifiedByYakeen)
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.IsYakeenNationalIdVerified;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("UserYakeenNationalIdNotVerified", CultureInfo.GetCultureInfo(model.Language));
        //                        output.Result = new LoginOutput { PhoneNumber = user.PhoneNumber, UserId = user.Id, AccessToken = null, Email = user.Email, AccessTokenGwt = null, TokenExpiryDate = 0 };
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "Phone is not Verified By Yakeen";
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }
        //                    string exception = string.Empty;
        //                    bool value = SetUserAuthenticationCookies(user, out exception);
        //                    if (!value)
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "Failed to save authentication Cookie due to : " + exception;
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }

        //                    var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
        //                    if (accessTokenResult == null)
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NullResult;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "accessTokenResult is null";
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }

        //                    if (string.IsNullOrEmpty(accessTokenResult.access_token))
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyResult;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "accessTokenResult.access_token is null";
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }

        //                    string accessTokenResultGWT = string.Empty;
        //                    if (model.Channel.ToString().ToLower() == Channel.Travel.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Medical.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Home.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Aman.ToString().ToLower())
        //                    {
        //                        string gwtKey = string.Empty;
        //                        accessTokenResultGWT = _authorizationService.GenerateTokenJWT(user.Id.ToString(), user.Email, user.UserName, user.PhoneNumber, out gwtKey);
        //                    }

        //                    output.Result = new LoginOutput { UserId = user.Id, AccessToken = accessTokenResult.access_token, Email = user.Email, AccessTokenGwt = accessTokenResultGWT, TokenExpiryDate = 30 };
        //                    MigrateUser(user.Id);
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
        //                    output.ErrorDescription = "Success";
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Success";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.LockedOut:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Lockout;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "This Account is LockedOut";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.RequiresVerification:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Phone number is not Confirmed";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.Failure:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "SignInStatus.Failure";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            default:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "incorrect username and password";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = exp.ToString();
        //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //        return output;
        //    }
        //}
        //public async Task<ProfileOutput<LoginOutput>> VerifyYakeenMobile(LoginViewModel model)
        //{
        //    ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
        //    LoginRequestsLog log = new LoginRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "VerifyYakeenMobile";
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    try
        //    {
        //        ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model,true);
        //        if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = validateOutput.ErrorCode;
        //            output.ErrorDescription = validateOutput.ErrorDescription;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = validateOutput.LogDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }

        //        var user = UserManager.FindByEmail(model.Email);
        //        if (user == null)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "user is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (!user.EmailConfirmed)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "email is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (user.LockoutEndDateUtc > DateTime.UtcNow)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.AccountLocked;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Account is Locked";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        var result = SignInManager.PasswordSignIn(user.UserName, model.Password, true, shouldLockout: false);
        //        if (result == SignInStatus.LockedOut)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Lockout;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "This Account is LockedOut";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (result == SignInStatus.RequiresVerification)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Phone number is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (result == SignInStatus.Failure || result != SignInStatus.Success)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "incorrect username and password";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        YakeenMobileVerificationDto yakeenMobileVerificationDto = new YakeenMobileVerificationDto();
        //        yakeenMobileVerificationDto.NationalId = model.NationalId;
        //        yakeenMobileVerificationDto.Phone = model.Phone;
        //        var yakeenMobileVerification = _iYakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, model.Language);
        //        if (yakeenMobileVerification.ErrorCode == YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.VerificationFaield;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = yakeenMobileVerification.ErrorDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.IsYakeenNationalIdVerified;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("YakeenMobileVerificationError", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = yakeenMobileVerification.ErrorDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        Guid userId = Guid.Empty;
        //        Guid.TryParse(user.Id, out userId);

        //        var otpOutput = SendOTPCode(model.Phone, model.Email, model.NationalId, userId, SMSMethod.WebsiteOTP);
        //        if (otpOutput.ErrorCode == 2)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.FailedToSendOtp;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Failed To Send Otp due to:" + otpOutput.ErrorDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (otpOutput.ErrorCode != 0)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.FailedToSendOtp;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Failed To Send Otp due to:" + otpOutput.ErrorDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = "Success";
        //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotSuccess;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //        return output;
        //    }
        //}
        //public async Task<ProfileOutput<LoginOutput>> EndLogin(LoginViewModel model, string returnUrl = null)
        //{
        //    ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
        //    LoginRequestsLog log = new LoginRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "EndLogin";
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    try
        //    {
        //        ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model,true);
        //        if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = validateOutput.ErrorCode;
        //            output.ErrorDescription = validateOutput.ErrorDescription;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = validateOutput.LogDescription;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        var user = UserManager.FindByEmail(model.Email);
        //        if (user == null)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "user is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        log.UserID = user.Id;
        //        log.Mobile = user.PhoneNumber;

        //        if (!user.EmailConfirmed)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "email is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (user.LockoutEndDateUtc > DateTime.UtcNow)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.AccountLocked;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Account is Locked";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }

        //        var yakeeenOutput = YakeenRequestLogDataAccess.GetYakeenMobileVerification(model.NationalId);
        //        if (yakeeenOutput == null)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "yakeeenOutput is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(yakeeenOutput.ServiceResponse);
        //        MobileVerificationModel responseObject = JsonConvert.DeserializeObject<MobileVerificationModel>(responseAfterDeserialize.ToString());
        //        if (!responseObject.isOwner)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "national id:" + model.NationalId + " is not the owner of mobile phone:" + model.Phone;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if(!model.OTP.HasValue)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPEmpty", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "OTP is empty";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == model.Phone && a.UserEmail == model.Email && a.Nin == model.NationalId&&a.IsCodeVerified==false).OrderByDescending(a => a.Id).FirstOrDefault();
        //        if (otpInfo == null)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language)); 
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "OTP info is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (otpInfo.VerificationCode != model.OTP)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Invalid otp as we recived:" + model.OTP + " and correct one is:" + otpInfo.VerificationCode;
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.OTPExpire;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(model.Language));
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "OTP Expire";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }
        //        otpInfo.IsCodeVerified = true;
        //        otpInfo.ModifiedDate = DateTime.Now;
        //        _otpInfo.Update(otpInfo);
        //        user.IsPhoneVerifiedByYakeen = true;
        //        user.NationalId = model.NationalId;
        //        user.PhoneNumberConfirmed = true;
        //        user.PhoneNumber = model.Phone;
        //        user.LastModifiedDate = DateTime.Now;
        //        var userResult = await UserManager.UpdateAsync(user);
        //        if (!userResult.Succeeded)
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.CanNotCreate; ;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_Update_USER", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "can not Update user due to " + string.Join(",", userResult.Errors);
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return output;
        //        }

        //        if (user.IsCorporateUser)
        //            return await _iCorporateContext.CorporateUserSignIn(user.Email, model.Password, model.Language, log);

        //        var result = SignInManager.PasswordSignIn(user.UserName, model.Password, true, shouldLockout: false);
        //        switch (result)
        //        {
        //            case SignInStatus.Success:
        //                {
        //                    string exception = string.Empty;
        //                    bool value = SetUserAuthenticationCookies(user, out exception);
        //                    if (!value)
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "Failed to save authentication Cookie due to : " + exception;
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }
        //                    var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
        //                    if (accessTokenResult == null)
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NullResult;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "accessTokenResult is null";
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }
        //                    if (string.IsNullOrEmpty(accessTokenResult.access_token))
        //                    {
        //                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyResult;
        //                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //                        log.ErrorCode = (int)output.ErrorCode;
        //                        log.ErrorDescription = "accessTokenResult.access_token is null";
        //                        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                        return output;
        //                    }
        //                    string accessTokenResultGWT = string.Empty;
        //                    if (model.Channel.ToString().ToLower() == Channel.Travel.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Medical.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Home.ToString().ToLower()
        //                      || model.Channel.ToString().ToLower() == Channel.Aman.ToString().ToLower())
        //                    {
        //                        string gwtKey = string.Empty;
        //                        accessTokenResultGWT = _authorizationService.GenerateTokenJWT(user.Id.ToString(),user.Email, user.UserName, user.PhoneNumber, out gwtKey);
        //                    }

        //                    output.Result = new LoginOutput { UserId = user.Id, AccessToken = accessTokenResult.access_token, Email = user.Email, AccessTokenGwt = accessTokenResultGWT, TokenExpiryDate = 30 };
        //                    MigrateUser(user.Id);
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
        //                    output.ErrorDescription = "Success";
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Success";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.LockedOut:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Lockout;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "This Account is LockedOut";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.RequiresVerification:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Phone number is not Confirmed";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            case SignInStatus.Failure:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "SignInStatus.Failure";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //            default:
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "incorrect username and password";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return output;
        //                }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = exp.ToString();
        //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //        return output;
        //    }
        //}
        //public ProfileOutput<LoginOutput> ValidateLoginRequest(LoginViewModel model,bool validatePhoneAndNin)
        //{
        //    ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
        //    try
        //    {
        //        var encryptedEmail = Convert.FromBase64String(model.UserName.Trim());
        //        var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
        //        model.Email = plainEmail;

        //        var encryptedPassword = Convert.FromBase64String(model.PWD.Trim());
        //        var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
        //        model.Password = plainPassword;

        //        if (string.IsNullOrEmpty(model.Email))
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "Email is empty";
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(model.Password))
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "password is empty";
        //            return output;
        //        }
        //        if (validatePhoneAndNin&&string.IsNullOrEmpty(model.Phone))
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("MobileEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "Mobile is empty";
        //            return output;
        //        }
        //        if (validatePhoneAndNin && string.IsNullOrEmpty(model.NationalId))
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = "NationalId is empty";
        //            return output;
        //        }
        //        if (validatePhoneAndNin && !Utilities.IsValidPhoneNo(model.Phone))
        //        {
        //            output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.InvalidData;
        //            output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language)); ;
        //            output.LogDescription = $"Mobile is not valid: {model.Phone}";
        //            return output;
        //        }
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        output.LogDescription = "Success";
        //        return output;
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        output.LogDescription = exp.ToString();
        //        return output;
        //    }
        //}
        //public SMSOutput SendOTPCode(string phoneNumber,string email,string nin,Guid userId, SMSMethod smsMethod)
        //{
        //    DateTime dateFrom = DateTime.Now.AddMinutes(-10);
        //    DateTime dateTo = DateTime.Now.AddMinutes(10);
        //    SMSOutput output = new SMSOutput();
        //    var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == phoneNumber && a.UserEmail ==email
        //    && a.Nin == nin &&a.IsCodeVerified==false&&a.CreatedDate>=dateFrom
        //    &&a.CreatedDate <= dateTo).OrderByDescending(a => a.Id).ToList();
        //    if (otpInfo != null&& otpInfo.Count()>=6)
        //    {
        //        output.ErrorCode = 2;
        //        output.ErrorDescription ="User Reached the maximum 4 numbers allowed of sending OTP";
        //        return output;
        //    }
        //    Random rnd = new Random();
        //    int verifyCode = rnd.Next(1000, 9999);
        //    OtpInfo info = new OtpInfo();
        //    info.UserId = userId;
        //    info.PhoneNumber =phoneNumber;
        //    info.UserEmail =email;
        //    info.Nin = nin;
        //    info.VerificationCode = verifyCode;
        //    info.CreatedDate = DateTime.Now;
        //    info.ModifiedDate = DateTime.Now;
        //    _otpInfo.Insert(info);
        //    var smsModel = new SMSModel()
        //    {
        //        PhoneNumber = phoneNumber,
        //        MessageBody = WebResources.VerifyOTP.Replace("{0}", verifyCode.ToString()),
        //        Method = smsMethod.ToString(),
        //        Module = Module.Vehicle.ToString()
        //    };
        //    return _notificationService.SendSmsBySMSProviderSettings(smsModel);
        //}
        //public SMSOutput ReSendOTPCode(LoginViewModel model)
        //{
        //    SMSOutput output = new SMSOutput();
        //    if (string.IsNullOrEmpty(model.Email))
        //    {
        //        output.ErrorCode = 2;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    if (!Utilities.IsValidMail(model.Email))
        //    {
        //        output.ErrorCode =3;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    if (string.IsNullOrEmpty(model.Phone))
        //    {
        //        output.ErrorCode = 4;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("MobileEmpty", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    if (string.IsNullOrEmpty(model.NationalId))
        //    {
        //        output.ErrorCode =5;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    if (!Utilities.IsValidPhoneNo(model.Phone))
        //    {
        //        output.ErrorCode = 6;
        //        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    var user = UserManager.FindByEmail(model.Email);
        //    Guid userId = Guid.Empty;
        //    if (user != null)
        //    {
        //        Guid.TryParse(user.Id, out userId);
        //    }
        //   var otpOutput= SendOTPCode(model.Phone, model.Email, model.NationalId, userId,SMSMethod.ResendOTP);
        //    if(otpOutput.ErrorCode==2)
        //    {
        //        output.ErrorCode = 12;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    if (otpOutput.ErrorCode != 0)
        //    {
        //        output.ErrorCode =13;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language));
        //        return output;
        //    }
        //    output.ErrorCode = 1;
        //    output.ErrorDescription ="Success";
        //    return output;
        //}

        public UserInfoOutput GetUserInfo(string userId)
        {
            UserInfoOutput output = new UserInfoOutput();
            if(string.IsNullOrEmpty(userId))
            {
                output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = "userId is null";
                return output;
            }
           var info= UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
            if(info==null)
            {
                output.ErrorCode = UserInfoOutput.ErrorCodes.NotFound;
                output.ErrorDescription = "user not found";
                return output;
            }
            output.ErrorCode = UserInfoOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.UserInfo = info;
            return output;
        }

        #region New (Register / login) logic

        #region Register

        /// <summary>
        /// this method 
        /// 1- to verify mobile from Yakeen,
        /// 2- check for user data
        ///     1- if exist (fullName) --> send OTP
        ///     2- if not --> return to user to get birth(year / month)
        /// 3- send OTP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<RegisterOutput>> BeginRegister(RegisterationModel model)
        {
            ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Method = "BeginRegister";
            log.Mobile = model?.Mobile;
            log.Channel = model?.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Nin = model.NationalId;

            try
            {
                var cachKey = $"{model.Channel}_Register_{base_KEY}_{log.UserIP}";
                UserPartialLockModel partialLock = await ValidateIfUserPartiallyLocked(model.Hashed, log.UserIP, cachKey, model.Language);
                if (partialLock.IsLocked)
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.Lockout, WebResources.ResourceManager.GetString("LoginUserPartialLock", CultureInfo.GetCultureInfo(model.Language)), $"user is partially locked as he exceeds the max tries: {partialLock.ErrorTimesUserTries}", model.Language);

                ProfileOutput<RegisterOutput> validateOutput = ValidateRegistrationRequest(model, false, false);
                if (validateOutput.ErrorCode == ProfileOutput<RegisterOutput>.ErrorCodes.EmailAlreadyExist || validateOutput.ErrorCode == ProfileOutput<RegisterOutput>.ErrorCodes.NationalIdAlreadyExist)
                {
                    partialLock.ErrorTimesUserTries += 1;
                    if (partialLock.ErrorTimesUserTries >= 10)
                    {
                        partialLock.IsLocked = true;
                        partialLock.LockDueDate = DateTime.Now.AddMinutes(5);
                        await RedisCacheManager.Instance.SetAsync(cachKey, partialLock, 5 * 60);
                    }
                    else
                        Output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);

                    return HandleRegisterOutput(Output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }

                if (validateOutput.ErrorCode != ProfileOutput<RegisterOutput>.ErrorCodes.Success)
                    return HandleRegisterOutput(Output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);

                string exception = string.Empty;
                YakeenMobileVerificationOutput yakeenMobileVerificationOutput = VerifyMobileFromYakeen(model.NationalId, model.Mobile, model.Language, out exception);
                if (yakeenMobileVerificationOutput == null
                    || yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success
                    || yakeenMobileVerificationOutput.mobileVerificationModel == null
                    || !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner)
                {
                    var logErrorMessage = (yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                                            ? exception
                                            : yakeenMobileVerificationOutput.mobileVerificationModel == null ? "yakeenMobileVerificationOutput.mobileVerificationModel return null"
                                            : !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner ? $"the nationalId {model.NationalId} is not the owner of this mobile {model.Mobile}"
                                            : "yakeenMobileVerificationOutput return null";

                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, (ProfileOutput<RegisterOutput>.ErrorCodes)yakeenMobileVerificationOutput.ErrorCode, yakeenMobileVerificationOutput.ErrorDescription, logErrorMessage, model.Language);
                }

                exception = string.Empty;
                UserDataModel userData = GetUserData(model.NationalId, model.Channel, out exception);
                if (userData == null || !userData.IsExist)
                {
                    Output.Result.GetBirthDate = true;
                    //Output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(hashed), SHARED_KEY);
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.UserNotFound, WebResources.ResourceManager.GetString("YakeenDriverData", CultureInfo.GetCultureInfo(model.Language)), (!string.IsNullOrEmpty(exception) ? exception : "userData.IsExist = false"), model.Language);
                }

                var otpOutput = HandleSendingOTP(model.Mobile, model.Email, model.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "VerifyOTP", "BeginRegister", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.FailedToSendOtp, otpOutput.ErrorDescription, otpOutput.LogDescription, model.Language);
                }

                Output.Result.GetBirthDate = false;
                Output.Result.PhoneVerification = true;
                Output.Result.FullNameAr = userData.FullNameAr;
                Output.Result.FullNameEn = userData.FullNameEn;
                //Output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.VerifyPhone, WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language)), "VerifyPhone", model.Language);
            }
            catch (Exception ex)
            {
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"BeginRegister exception, and error is: {ex.ToString()}", model.Language);
            }
        }

        /// <summary>
        /// this method to get user data (fullName) from yakeen
        ///     1- if success --> send OTP
        ///     2- else --> return error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<RegisterOutput>> EndRegister(RegisterationModel model)
        {
            ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Method = "EndRegister";
            log.Mobile = model.Mobile;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Nin = model.NationalId;

            try
            {
                ProfileOutput<RegisterOutput> validateOutput = ValidateRegistrationRequest(model, true, false);
                if (validateOutput.ErrorCode != ProfileOutput<RegisterOutput>.ErrorCodes.Success)
                    return HandleRegisterOutput(Output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);

                string outputDescription = string.Empty;
                string logException = string.Empty;
                var userData = GetUserDataFromYakeen(model.NationalId, model.BirthYear.Value, model.BirthMonth.Value, model.Channel, model.Language, out logException, out outputDescription);
                if (userData == null || !userData.IsExist || !string.IsNullOrEmpty(logException))
                {
                    Output.Result.GetBirthDate = false;
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter, outputDescription, logException, model.Language);
                }

                var otpOutput = HandleSendingOTP(model.Mobile, model.Email, model.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "VerifyOTP", "EndRegister", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.FailedToSendOtp, otpOutput.ErrorDescription, otpOutput.LogDescription, model.Language);
                }

                Output.Result.GetBirthDate = false;
                Output.Result.PhoneVerification = true;
                Output.Result.FullNameAr = userData.FullNameAr;
                Output.Result.FullNameEn = userData.FullNameEn;
                //Output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.VerifyPhone, WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language)), "VerifyPhone", model.Language);
            }
            catch (Exception ex)
            {
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"EndRegister exception, and error is: {ex.ToString()}", model.Language);
            }
        }

        /// <summary>
        /// this method to verify otp in register step
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<RegisterOutput>> VerifyRegisterOTP(RegisterationModel model)
        {
            ProfileOutput<RegisterOutput> Output = new ProfileOutput<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Method = "VerifyRegisterOTP";
            log.Mobile = model.Mobile;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Nin = model.NationalId;

            try
            {
                ProfileOutput<RegisterOutput> validateOutput = ValidateRegistrationRequest(model, false, true);
                if (validateOutput.ErrorCode != ProfileOutput<RegisterOutput>.ErrorCodes.Success)
                    return HandleRegisterOutput(Output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);

                var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == model.Mobile && a.UserEmail == model.Email && a.Nin == model.NationalId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
                otpInfo.IsCodeVerified = true;
                otpInfo.ModifiedDate = DateTime.Now;
                _otpInfo.Update(otpInfo);

                var mobileInternalFormated = Utilities.ValidateInternalPhoneNumber(model.Mobile);
                var mobileInternationalFormated = Utilities.ValidatePhoneNumber(model.Mobile);
                var userInfo = UserManager.Users.Where(x => x.PhoneNumber == mobileInternalFormated || x.PhoneNumber == mobileInternationalFormated).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.PhoneNumber = null;
                    userInfo.PhoneNumberConfirmed = false;
                    userInfo.IsPhoneVerifiedByYakeen = false;
                    //userInfo.NationalId = null;
                    var updateUserInfo = UserManager.Update(userInfo);
                    if (!updateUserInfo.Succeeded)
                    {
                        //Output.Result.Hashed = string.Empty;
                        return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.NotSuccess, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"updateUserInfo.Succeeded is: {updateUserInfo.Succeeded} and error is: {string.Join(",", updateUserInfo.Errors)}", model.Language);
                    }
                }

                var user = new AspNetUser
                {
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    LockoutEndDateUtc = DateTime.UtcNow.AddDays(-1),
                    DeviceToken = "",
                    Email = model.Email,
                    EmailConfirmed = true,
                    RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"),
                    LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"),
                    PhoneNumber = mobileInternalFormated,
                    PhoneNumberConfirmed = true,
                    UserName = model.Email,
                    FullName = model.FullNameEn,
                    FullNameAr = model.FullNameAr,
                    TwoFactorEnabled = false,
                    Channel = model.Channel.ToString(),
                    IsPhoneVerifiedByYakeen = true,
                    NationalId = model.NationalId
                };
                var result = UserManager.Create(user, model.Password);
                if (!result.Succeeded)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.CanNotCreate, WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(model.Language)), $"can not create user due to {string.Join(",", result.Errors)}", model.Language);
                }

                var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                if (accessTokenResult == null)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.NullResult, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"accessTokenResult is null", model.Language);
                }
                if (string.IsNullOrEmpty(accessTokenResult.access_token))
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.NullResult, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"accessTokenResult.access_token is null", model.Language);
                }

                SendSMS(user.PhoneNumber, user.NationalId, SMSMethod.WebsiteOTP, "YakeenMobileVerificationSMS", model.Language);
                MigrateUser(user.Id);

                Output.Result.UserId = user.Id;
                Output.Result.PhoneNumber = user.PhoneNumber;
                Output.Result.AccessToken = accessTokenResult.access_token;
                Output.Result.Email = user.Email;
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.Success, WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Language)), $"Success", model.Language);
            }
            catch (Exception ex)
            {
                return HandleRegisterOutput(Output, log, ProfileOutput<RegisterOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"VerifyRegisterOTP exception, and error is: {ex.ToString()}", model.Language);
            }
        }


        private ProfileOutput<RegisterOutput> ValidateRegistrationRequest(RegisterationModel model, bool validateBirthYearAndMonth, bool verifyOTP)
        {
            ProfileOutput<RegisterOutput> output = new ProfileOutput<RegisterOutput>();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "mail is empty";
                    return output;
                }
                if (!Utilities.IsValidMail(model.Email))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"mail is not valid: {model.Email}";
                    return output;
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "Password is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(model.Mobile))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("MobileEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "Mobile is empty";
                    return output;
                }
                if (!Utilities.IsValidPhoneNo(model.Mobile))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"Mobile is not valid: {model.Mobile}";
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "NationalId is empty";
                    return output;
                }

                var userInfo = UserManager.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                if (userInfo != null)
                {
                    if (userInfo.EmailConfirmed)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmailAlreadyExist;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = $"email already exist and Confirmed";
                        return output;
                    }

                    string exception = string.Empty;
                    var deleteUser = RemoveUserIdFromQuotationRequest(userInfo, "Email", model.Language, out exception);
                    if (!deleteUser || !string.IsNullOrEmpty(exception))
                    {
                        var logMessage = !string.IsNullOrEmpty(exception) ? exception : "deleteUser return false";
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = $"can not delete user due to {logMessage}";
                        return output;
                    }
                }

                var userInfoWithSameNationalId = UserManager.Users.Where(x => x.NationalId == model.NationalId).FirstOrDefault();
                if (userInfoWithSameNationalId != null)
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.NationalIdAlreadyExist;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("exist_nationalId_signup_error", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"Registration NationalId already exist";
                    return output;
                }

                ////
                /// here we will validate for account changed
                /// 
                var currentYear = DateTime.Now.Year;
                var start = new DateTime(currentYear, 01, 01);
                var end = new DateTime(currentYear, 12, 31);
                var switchedAccountsDetails = _loginSwitchAccountRepository.TableNoTracking
                                                .Where(a => a.Nin == model.NationalId && a.PhoneNumber == model.Mobile && a.CreatedDate >= start && a.CreatedDate <= end).ToList();

                if (switchedAccountsDetails != null && switchedAccountsDetails.Count >= 3)
                {
                    output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.SwitchingAccountsHasReachedTheLimit;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SwitchingAccountsHasReachedTheLimit", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"Register validation --> This user with NationalId: {model.NationalId} and PhoneNumber: {model.Mobile} reaches the limit for switching between his accounts";
                    return output;
                }

                if (validateBirthYearAndMonth)
                {
                    if (!model.BirthYear.HasValue || model.BirthYear.Value <= 0)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorBirthYear", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "birth year is empty or not valid";
                        return output;
                    }
                    if (!model.BirthMonth.HasValue || model.BirthMonth > 12)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorBirthMonth", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "birth month is empty or not valid";
                        return output;
                    }
                }
                if (verifyOTP)
                {
                    if (string.IsNullOrEmpty(model.FullNameAr))
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "FullNameAr is empty";
                        return output;
                    }
                    if (string.IsNullOrEmpty(model.FullNameEn))
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "FullNameEn is empty";
                        return output;
                    }
                    if (!model.OTP.HasValue)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyOTP", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "registration otp is empty";
                        return output;
                    }

                    var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == model.Mobile && a.UserEmail == model.Email && a.Nin == model.NationalId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (otpInfo == null)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "OTP info is null";
                        return output;
                    }
                    else if (otpInfo.VerificationCode != model.OTP)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "Invalid otp as we recived:" + model.OTP + " and correct one is:" + otpInfo.VerificationCode;
                        return output;
                    }
                    else if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
                    {
                        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.OTPExpire;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "OTP Expire";
                        return output;
                    }
                }
                //if (validateBirthYearAndMonth || verifyOTP)
                //{
                //    var decryptedHashed = AESEncryption.DecryptString(Utilities.GetDecodeUrl(model.Hashed), SHARED_KEY);
                //    var decryptRegistrationModel = JsonConvert.DeserializeObject<RegisterationModel>(decryptedHashed);
                //    if (decryptRegistrationModel == null)
                //    {
                //        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language));
                //        output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel is null as hashed is: {model.Hashed} and after decrypt is {decryptRegistrationModel}";
                //        return output;
                //    }
                //    if (!decryptRegistrationModel.NationalId.Equals(model.NationalId))
                //    {
                //        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //        output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_NationalId", CultureInfo.GetCultureInfo(model.Language));
                //        output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.NationalId: {decryptRegistrationModel.NationalId} != model.NationalId {model.NationalId}";
                //        return output;
                //    }
                //    if (!decryptRegistrationModel.Mobile.Equals(model.Mobile))
                //    {
                //        output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //        output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_Mobile", CultureInfo.GetCultureInfo(model.Language));
                //        output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.Mobile: {decryptRegistrationModel.Mobile} != model.Mobile {model.Mobile}";
                //        return output;
                //    }

                //    if (verifyOTP)
                //    {
                //        if (validateBirthYearAndMonth)
                //        {
                //            if (!decryptRegistrationModel.BirthMonth.Value.Equals(model.BirthMonth.Value))
                //            {
                //                output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //                output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_BirthMonth", CultureInfo.GetCultureInfo(model.Language));
                //                output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.BirthMonth: {decryptRegistrationModel.BirthMonth} != model.BirthMonth {model.BirthMonth}";
                //                return output;
                //            }
                //            if (!decryptRegistrationModel.BirthYear.Value.Equals(model.BirthYear.Value))
                //            {
                //                output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //                output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_BirthYear", CultureInfo.GetCultureInfo(model.Language));
                //                output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.BirthYear: {decryptRegistrationModel.BirthYear} != model.BirthYear {model.BirthYear}";
                //                return output;
                //            }
                //        }

                //        if (!decryptRegistrationModel.FullNameAr.Equals(model.FullNameAr))
                //        {
                //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //            output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_FullNameAr", CultureInfo.GetCultureInfo(model.Language));
                //            output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.FullNameAr: {decryptRegistrationModel.FullNameAr} != model.FullNameAr {model.FullNameAr}";
                //            return output;
                //        }
                //        if (!decryptRegistrationModel.FullNameEn.Equals(model.FullNameEn))
                //        {
                //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.InvalidData;
                //            output.ErrorDescription = WebResources.ResourceManager.GetString("Hash_Error_FullNameEn", CultureInfo.GetCultureInfo(model.Language));
                //            output.LogDescription = $"ValidateRegistrationRequest decryptRegistrationModel.FullNameEn: {decryptRegistrationModel.BirthYear} != model.FullNameEn {model.FullNameEn}";
                //            return output;
                //        }
                //    }
                //}
                
                //if (!validateBirthYearAndMonth && !verifyOTP)
                //{
                //    var userInfo = UserManager.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                //    if (userInfo != null)
                //    {
                //        if (userInfo.EmailConfirmed)
                //        {
                //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.EmailAlreadyExist;
                //            output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                //            output.LogDescription = $"email already exist and Confirmed";
                //            return output;
                //        }

                //        string exception = string.Empty;
                //        var deleteUser = RemoveUserIdFromQuotationRequest(userInfo, "Email", model.Language, out exception);
                //        if (!deleteUser || !string.IsNullOrEmpty(exception))
                //        {
                //            var logMessage = !string.IsNullOrEmpty(exception) ? exception : "deleteUser return false";
                //            output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
                //            output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                //            output.LogDescription = $"can not delete user due to {logMessage}";
                //            return output;
                //        }
                //    }
                //}

                output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<RegisterOutput>.ErrorCodes.FailedToDelete;
                output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Language));
                output.LogDescription = $"ValidateRegistrationRequest exception error, and error is: {ex.ToString()}";
                return output;
            }
        }

        private ProfileOutput<RegisterOutput> HandleRegisterOutput(ProfileOutput<RegisterOutput> output, RegistrationRequestsLog log, ProfileOutput<RegisterOutput>.ErrorCodes outputCode, string outputMessageKey, string logMessage, string lang)
        {
            output.ErrorCode = outputCode;
            output.ErrorDescription = outputMessageKey;
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = logMessage;
            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
            return output;
        }

        #endregion


        #region Login

        /// <summary>
        /// this method
        /// 1- to check for mobile verification
        ///     1- if yes --> make login
        ///     2- if no --> verify mobile from Yakeen
        /// 2- check for user data
        ///     1- if exist (fullName) --> send OTP
        ///     2- if not --> return to user to get birth(year / month)
        /// 3- send OTP
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<LoginOutput>> BeginLogin(LoginViewModel model, string returnUrl = null)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            output.Result = new LoginOutput() { PhoneNumberConfirmed = null };
            LoginRequestsLog log = new LoginRequestsLog();
            log.Method = "BeginLogin";
            log.Email = model.UserName;
            log.Mobile = model.Phone;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //log.MachineUniqueUUID = Utilities.GetMachineUniqueUUID();

            try
            {
                string exception = string.Empty;
                ////
                /// here will validate if user is partially locked or not
                var cachKey = $"{model.Channel}_Login_{base_KEY}_{log.UserIP}";
                UserPartialLockModel partialLock = await ValidateIfUserPartiallyLocked(model.Hashed, log.UserIP, cachKey, model.Language);
                if (partialLock.IsLocked)
                {
                    //await _redisCacheManager.SetAsync(cachKey, partialLock, 5 * 60);
                    //output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.Lockout, WebResources.ResourceManager.GetString("LoginUserPartialLock", CultureInfo.GetCultureInfo(model.Language)), $"user is partially locked as he exceeds the max tries: {partialLock.ErrorTimesUserTries}", model.Language);
                }

                string email = string.Empty;
                UserDataModel userData = null;
                ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, false, false, false, out email);
                if (validateOutput.ErrorCode == ProfileOutput<LoginOutput>.ErrorCodes.UserAccountNotExist || validateOutput.ErrorCode == ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized)
                {
                    log.Email = !string.IsNullOrEmpty(email) ? email : model.UserName;

                    partialLock.ErrorTimesUserTries += 1;
                    if (partialLock.ErrorTimesUserTries >= 10)
                    {
                        partialLock.IsLocked = true;
                        partialLock.LockDueDate = DateTime.Now.AddMinutes(5);
                        await RedisCacheManager.Instance.SetAsync(cachKey, partialLock, 5 * 60);
                    }
                    else
                        output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);
                    //if (partialLock.IsLocked || partialLock.LockDueDate >= DateTime.Now)
                    //    return HandleLoginOutput(output, log, validateOutput.ErrorCode, partialLock.ErrorDescription, partialLock.LogDescription, model.Language);
                    return HandleLoginOutput(output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }
                if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                {
                    log.Email = !string.IsNullOrEmpty(email) ? email : model.UserName;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }

                var user = UserManager.FindByEmail(model.Email);
                log.Email = model.Email;
                //log.Mobile = model.Phone;
                log.UserID = user.Id;
                log.Mobile = user.PhoneNumber;

                if (!user.IsPhoneVerifiedByYakeen || (string.IsNullOrEmpty(user.NationalId) || string.IsNullOrWhiteSpace(user.NationalId)))
                {
                    output.Result.PhoneNumberConfirmed = false;
                    output.Result.PhoneNo = user.PhoneNumber;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.IsYakeenNationalIdVerified, WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)), "user.IsPhoneVerifiedByYakeen is not verified or user.NationalId is null", model.Language);
                }

                exception = string.Empty;
                string fullNameEn = (!string.IsNullOrEmpty(user.FullName) && !string.IsNullOrWhiteSpace(user.FullName)) ? user.FullName : null;
                string fullNameAr = (!string.IsNullOrEmpty(user.FullNameAr) && !string.IsNullOrWhiteSpace(user.FullNameAr)) ? user.FullNameAr : null;
                if (string.IsNullOrEmpty(fullNameEn) || string.IsNullOrEmpty(fullNameAr))
                {
                    userData = GetUserData(user.NationalId, model.Channel, out exception);
                    if (userData == null || !userData.IsExist)
                    {
                        output.Result.GetBirthDate = true;
                        //output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                        return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.UserNotFound, WebResources.ResourceManager.GetString("YakeenDriverData", CultureInfo.GetCultureInfo(model.Language)), (!string.IsNullOrEmpty(exception) ? exception : "userData.IsExist = false"), model.Language);
                    }

                    fullNameEn = userData.FullNameEn;
                    fullNameAr = userData.FullNameAr;
                }

                var updateUserInfo = UpdateAcountsWithSamePhone(user.PhoneNumber, user.Id);
                if (updateUserInfo != null && !updateUserInfo.Succeeded)
                {
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.NotSuccess, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"Login updateUserInfo.Succeeded is: {updateUserInfo.Succeeded} and error is: {string.Join(",", updateUserInfo.Errors)}", model.Language);
                }

                ////
                /// this change to skip send OTP for this email (f.alazhary@bcare.com.sa) on all app channels to test (huawei)
                /// as per Shehata email subject (Your app update application has been rejected)

                //System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\OTP_Email.txt", model.Email);
                //System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\OTP_Channel.txt", model.Channel.ToString().ToLower());

                if (emailsToSkipSendingOTP.Contains(model.Email)) //  && appChannels.Contains(model.Channel.ToString().ToLower())
                {
                    //user.LastLoginDate = DateTime.Now;
                    //user.LastModifiedDate = DateTime.Now;
                    return await HandleLogin(output, user, model.Password, model.Language, log, model.Channel, model.IsYakeenChecked);
                }

                var loginOtpOutput = HandleSendingOTP(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "LoginVerifyOTP", "BeginLogin", model.Language);
                if (loginOtpOutput.ErrorCode != 0)
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.FailedToSendOtp, loginOtpOutput.ErrorDescription, loginOtpOutput.LogDescription, model.Language);

                output.Result.GetBirthDate = false;
                output.Result.PhoneVerification = true;
                output.Result.FullNameAr = fullNameAr;
                output.Result.FullNameEn = fullNameEn;
                output.Result.PhoneNo = user.PhoneNumber;
                //output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, Guid.NewGuid().ToString(), 0);
                //output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.VerifyPhone, WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language)), $"VerifyPhone", model.Language);
            }
            catch (Exception ex)
            {
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"BeginLogin exception, and error is: {ex.ToString()}", model.Language);
            }
        }

        /// <summary>
        /// this method to verify mobile from yakeen only
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<LoginOutput>> VerifyYakeenMobile(LoginViewModel model)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            output.Result = new LoginOutput() { PhoneNumberConfirmed = null };
            LoginRequestsLog log = new LoginRequestsLog();
            log.Method = "VerifyYakeenMobile";
            log.Email = model.UserName;
            log.Mobile = model.Phone;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                string email = string.Empty;
                ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, true, false, false, out email);
                if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                {
                    log.Email = !string.IsNullOrEmpty(email) ? email : model.UserName;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }

                log.Email = model.Email;
                log.Mobile = model.Phone;

                string exception = string.Empty;
                YakeenMobileVerificationOutput yakeenMobileVerificationOutput = VerifyMobileFromYakeen(model.NationalId, model.Phone, model.Language, out exception);
                if (yakeenMobileVerificationOutput == null
                    || yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success
                    || yakeenMobileVerificationOutput.mobileVerificationModel == null
                    || !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner)
                {
                    var logErrorMessage = (yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                                            ? exception
                                            : yakeenMobileVerificationOutput.mobileVerificationModel == null ? "yakeenMobileVerificationOutput.mobileVerificationModel return null"
                                            : !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner ? $"the nationalId {model.NationalId} is not the owner of this mobile {model.Phone}"
                                            : "yakeenMobileVerificationOutput return null";

                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, (ProfileOutput<LoginOutput>.ErrorCodes)yakeenMobileVerificationOutput.ErrorCode, yakeenMobileVerificationOutput.ErrorDescription, logErrorMessage, model.Language);
                }
                output.Result.IsYakeenChecked = true;

                exception = string.Empty;
                UserDataModel userData = GetUserData(model.NationalId, model.Channel, out exception);
                if (userData == null || !userData.IsExist)
                {
                    output.Result.GetBirthDate = true;
                    //output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.UserNotFound, WebResources.ResourceManager.GetString("YakeenDriverData", CultureInfo.GetCultureInfo(model.Language)), (!string.IsNullOrEmpty(exception) ? exception : "userData.IsExist = false"), model.Language);
                }

                var otpOutput = HandleSendingOTP(model.Phone, model.Email, model.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "VerifyOTP", "VerifyYakeenMobile", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.FailedToSendOtp, otpOutput.ErrorDescription, otpOutput.LogDescription, model.Language);
                }

                output.Result.GetBirthDate = false;
                output.Result.PhoneVerification = true;
                output.Result.FullNameAr = userData.FullNameAr;
                output.Result.FullNameEn = userData.FullNameEn;
                output.Result.PhoneNo = model.Phone;
                //output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.VerifyPhone, WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language)), $"VerifyPhone", model.Language);
            }
            catch (Exception ex)
            {
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"VerifyYakeenMobile exception, and error is: {ex.ToString()}", model.Language);
            }
        }

        /// <summary>
        /// this method to get user data (fullName) from yakeen
        ///     1- if success --> send OTP
        ///     2- else --> return error
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<LoginOutput>> EndLogin(LoginViewModel model, string returnUrl = null)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            output.Result = new LoginOutput() { PhoneNumberConfirmed = null };
            LoginRequestsLog log = new LoginRequestsLog();
            log.Method = "EndLogin";
            log.Email = model.UserName;
            log.Mobile = model.Phone;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                string email = string.Empty;
                //ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, true, true, false, out email);
                ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, false, true, false, out email);
                if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                {
                    log.Email = !string.IsNullOrEmpty(email) ? email : model.UserName;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }

                log.Email = model.Email;
                log.Mobile = model.Phone;

                string outputDescription = string.Empty;
                string logException = string.Empty;
                var userData = GetUserDataFromYakeen(model.NationalId, model.BirthYear.Value, model.BirthMonth.Value, model.Channel, model.Language, out logException, out outputDescription);
                if (userData == null || !userData.IsExist || !string.IsNullOrEmpty(logException))
                {
                    output.Result.GetBirthDate = false;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter, outputDescription, (!string.IsNullOrEmpty(logException) ? logException : (!userData.IsExist) ? "userData.IsExist is false" : "userData return null"), model.Language);
                }

                var otpOutput = HandleSendingOTP(model.Phone, model.Email, model.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "VerifyOTP", "EndLogin", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    //Output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.FailedToSendOtp, otpOutput.ErrorDescription, otpOutput.LogDescription, model.Language);
                }
                
                output.Result.GetBirthDate = false;
                output.Result.PhoneVerification = true;
                output.Result.FullNameAr = userData.FullNameAr;
                output.Result.FullNameEn = userData.FullNameEn;
                output.Result.PhoneNo = model.Phone;
                //output.Result.Hashed = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), SHARED_KEY);
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.VerifyPhone, WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language)), $"VerifyPhone", model.Language);
            }
            catch (Exception ex)
            {
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"EndLogin exception, and error is: {ex.ToString()}", model.Language);
            }
        }

        /// <summary>
        /// this method to verify otp in login step and update user mobile --> IsYakeenVerified
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileOutput<LoginOutput>> VerifyLoginOTP(LoginViewModel model)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            output.Result = new LoginOutput() { PhoneNumberConfirmed = null };
            LoginRequestsLog log = new LoginRequestsLog();
            log.Email = model.UserName;
            log.Mobile = model.Phone;
            log.Method = "VerifyLoginOTP";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Email = model.Email;
            log.Mobile = model.Phone;

            try
            {
                string exception = string.Empty;
                string email = string.Empty;
                //ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, true, true, true, out email);
                ProfileOutput<LoginOutput> validateOutput = ValidateLoginRequest(model, false, false, true, out email);
                if (validateOutput.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                {
                    log.Email = !string.IsNullOrEmpty(email) ? email : model.UserName;
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);
                }

                //var user = await UserManager.FindByEmailAsync(model.Email);
                var user = UserManager.Users.Where(a => a.Email == model.Email).FirstOrDefault();
                log.UserID = user.Id;
                log.Mobile = model.Phone;

                var updateUserInfo = UpdateAcountsWithSamePhone(model.Phone, user.Id);
                if (updateUserInfo != null && !updateUserInfo.Succeeded)
                {
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.NotSuccess, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"updateUserInfo.Succeeded is: {updateUserInfo.Succeeded} and error is: {string.Join(",", updateUserInfo.Errors)}", model.Language);
                }

                if (string.IsNullOrEmpty(user.PhoneNumber) || model.IsYakeenChecked)
                {
                    user.PhoneNumber = Utilities.ValidateInternalPhoneNumber(model.Phone);
                    user.PhoneNumberConfirmed = true;
                    user.IsPhoneVerifiedByYakeen = true;
                }
                if (!string.IsNullOrEmpty(user.PhoneNumber) && !user.IsPhoneVerifiedByYakeen)
                {
                    user.PhoneNumberConfirmed = true;
                    user.IsPhoneVerifiedByYakeen = true;
                }
                if (string.IsNullOrEmpty(user.NationalId) || model.IsYakeenChecked)
                {
                    user.NationalId = model.NationalId;
                }
                if (string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.FullNameAr))
                {
                    user.FullName = model.FullNameEn;
                    user.FullNameAr = model.FullNameAr;
                }

                user.LastLoginDate = DateTime.Now;
                user.LastModifiedDate = DateTime.Now;
                var userResult = await UserManager.UpdateAsync(user);
                if (!userResult.Succeeded)
                {
                    //output.Result.Hashed = string.Empty;
                    return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.CanNotCreate, WebResources.ResourceManager.GetString("CAN_NOT_Update_USER", CultureInfo.GetCultureInfo(model.Language)), $"can not Update user due to {string.Join(",", userResult.Errors)}", model.Language);
                }

                return await HandleLogin(output, user, model.Password, model.Language, log, model.Channel, model.IsYakeenChecked);
            }
            catch (Exception ex)
            {
                return HandleLoginOutput(output, log, ProfileOutput<LoginOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"VerifyLoginOTP exception, and error is: {ex.ToString()}", model.Language);
            }
        }


        private ProfileOutput<LoginOutput> ValidateLoginRequest(LoginViewModel model, bool validatePhoneAndNin, bool validateBirthYearAndMonth, bool verifyOTP, out string email)
        {
            email = string.Empty;
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(model.UserName))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model.UserName is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(model.PWD))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "password is empty";
                    return output;
                }

                var encryptedEmail = Convert.FromBase64String(model.UserName.Trim());
                var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                model.Email = plainEmail;

                var encryptedPassword = Convert.FromBase64String(model.PWD.Trim());
                var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                model.Password = plainPassword;
                email = model.Email;

                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "Email is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "password is empty";
                    return output;
                }

                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.UserAccountNotExist;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "user is null";
                    return output;
                }

                var result = SignInManager.UserManager.CheckPassword(user, model.Password);
                if (!result)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"Invalid userName or password as user enter userName: {model.Email} and password: {model.Password}";
                    return output;
                }


                if (!user.EmailConfirmed)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "email is not Confirmed";
                    return output;
                }
                if (user.LockoutEnabled && user.LockoutEndDateUtc > DateTime.UtcNow)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.AccountLocked;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "Account is Locked";
                    return output;
                }

                ////
                /// here we will validate for account changed
                /// 
                //if (user.IsPhoneVerifiedByYakeen || validatePhoneAndNin)
                //{
                //    //var userPhone = user.IsPhoneVerifiedByYakeen ? user.PhoneNumber : model.Phone;
                //    //var checkSwitchAccountResult = CheckForSwitchedAccounts(user.NationalId, user.Email, userPhone, model.Language);
                //    //if (checkSwitchAccountResult.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                //    //    return checkSwitchAccountResult;
                //}

                if ((validatePhoneAndNin || validateBirthYearAndMonth) && !user.IsPhoneVerifiedByYakeen)
                {
                    var anotherUserWithSameNationalId = _authorizationService.CheckUserWithNationalAndDifferentEmail(model.NationalId, model.Email);
                    if (anotherUserWithSameNationalId)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.NationalIdAlreadyExist;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("exist_nationalId_signup_error", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = $"Login NationalId already exist";
                        return output;
                    }
                }
                if (validatePhoneAndNin)
                {
                    if (string.IsNullOrEmpty(model.Phone))
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("MobileEmpty", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "Mobile is empty";
                        return output;
                    }
                    else if (string.IsNullOrEmpty(model.NationalId))
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "NationalId is empty";
                        return output;
                    }
                    else if (!Utilities.IsValidPhoneNo(model.Phone))
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = $"Mobile is not valid: {model.Phone}";
                        return output;
                    }
                }
                if (validateBirthYearAndMonth)
                {
                    if (!model.BirthYear.HasValue || model.BirthYear.Value <= 0)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorBirthYear", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "birth year is empty or not valid";
                        return output;
                    }
                    if (!model.BirthMonth.HasValue || model.BirthMonth > 12)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorBirthMonth", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "birth month is empty or not valid";
                        return output;
                    }
                }
                if (verifyOTP)
                {
                    if (string.IsNullOrEmpty(model.FullNameAr))
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "FullNameAr is empty";
                        return output;
                    }
                    if (string.IsNullOrEmpty(model.FullNameEn))
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "FullNameEn is empty";
                        return output;
                    }
                    if (!model.OTP.HasValue)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.InvalidData;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyOTP", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "registration otp is empty";
                        return output;
                    }

                    var _phone = user.IsPhoneVerifiedByYakeen ? user.PhoneNumber : model.Phone;
                    var _email = user.IsPhoneVerifiedByYakeen ? user.Email : model.Email;
                    var _nationalId = user.IsPhoneVerifiedByYakeen ? user.NationalId : model.NationalId;

                    var otpInfo = _otpInfo.Table.Where(a => a.PhoneNumber == _phone && a.UserEmail == _email && a.Nin == _nationalId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (otpInfo == null)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "OTP info is null";
                        return output;
                    }
                    else if (otpInfo.VerificationCode != model.OTP)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "Invalid otp as we recived:" + model.OTP + " and correct one is:" + otpInfo.VerificationCode;
                        return output;
                    }
                    else if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
                    {
                        output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.OTPExpire;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(model.Language));
                        output.LogDescription = "OTP Expire";
                        return output;
                    }

                    otpInfo.IsCodeVerified = true;
                    otpInfo.ModifiedDate = DateTime.Now;
                    _otpInfo.Update(otpInfo);
                }

                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                output.LogDescription = exp.ToString();
                return output;
            }
        }

        //private bool IsUserPartiallyLocked(UserPartialLockModel partialLock, string userIP, bool mustHaveHashed, out string exception)
        //{
        //    exception = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(model.Hashed))
        //        {
        //            var encryptedCaptcha = AESEncryption.DecryptString(model.Hashed, SHARED_PARTIAL_LOCK_SECRET);
        //            if (!string.IsNullOrEmpty(encryptedCaptcha))
        //            {
        //                string exception = string.Empty;
        //                partialLock = JsonConvert.DeserializeObject<UserPartialLockModel>(encryptedCaptcha);
        //                bool mustHaveHasheedKey = (!validatePhoneAndNin && !validateBirthYearAndMonth && !verifyOTP) ? false : true;
        //                var isLocked = IsUserPartiallyLocked(partialLock, userIP, mustHaveHasheedKey, out exception);
        //                if (isLocked)
        //                {
        //                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Lockout;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("                                                ", CultureInfo.GetCultureInfo(model.Language));
        //                    output.LogDescription = exception;
        //                    return output;
        //                }
        //            }
        //        }


        //        if (partialLock == null)
        //        {
        //            if (mustHaveHashed)
        //            {
        //                exception = "hashed value must be provided";
        //                return true;
        //            }
        //            return false;
        //        }
        //        if (partialLock.ErrorTimesUserTries >= 3)
        //        {
        //            exception = $"user is partially locked as error tries times is: {partialLock.ErrorTimesUserTries}";
        //            return true;
        //        }

        //        var clearText = $"{partialLock.SessionId}_{SHARED_PARTIAL_LOCK_SECRET}_{userIP}";
        //        if (!SecurityUtilities.VerifyHashedData(partialLock.Hashed, clearText))
        //        {
        //            exception = $"Hashed Not Matched as we received hashed:{partialLock.Hashed}, clearText is: {clearText} and hash is {SecurityUtilities.HashData(clearText, null)}";
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex.ToString();
        //        return false;
        //    }
        //}

        private async Task<UserPartialLockModel> ValidateIfUserPartiallyLocked(string hashedValue, string userIP, string cachKey, string lang)
        {
            UserPartialLockModel partialLock = null;
            var sessionId = Guid.NewGuid().ToString();

            try
            {
                var lockedFromCach = await RedisCacheManager.Instance.GetAsync<UserPartialLockModel>(cachKey);
                if (lockedFromCach != null && lockedFromCach.IsLocked)
                    return lockedFromCach; // HandleUserPartialLockReturnModel(lockedFromCach.SessionId, true, lockedFromCach.LockDueDate, lockedFromCach.ErrorTimesUserTries, "LoginUserPartialLock", $"user is partially locked from cach as error tries times is: {partialLock.ErrorTimesUserTries}", lang);

                if (string.IsNullOrEmpty(hashedValue))
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "hashedValue is empty", lang);

                var encryptedCaptcha = AESEncryption.DecryptString(hashedValue, SHARED_PARTIAL_LOCK_SECRET);
                if (string.IsNullOrEmpty(encryptedCaptcha))
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "encryptedCaptcha after DecryptString is empty", lang);

                partialLock = JsonConvert.DeserializeObject<UserPartialLockModel>(encryptedCaptcha);
                if (partialLock == null)
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "ErrorGeneric", "partialLock model after DeserializeObject is null", lang);

                if (partialLock.LockDueDate >= DateTime.Now)
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 0, "LoginUserPartialLock", $"user is still locked as partialLock.LockDueDate = {partialLock.LockDueDate}", lang);

                if (partialLock.ErrorTimesUserTries >= 10)
                    return HandleUserPartialLockReturnModel(partialLock.SessionId, true, DateTime.Now.AddMinutes(5), partialLock.ErrorTimesUserTries, "LoginUserPartialLock", $"user is partially locked as error tries times is: {partialLock.ErrorTimesUserTries}", lang);

                var clearText = $"{partialLock.SessionId}_{SHARED_PARTIAL_LOCK_SECRET}_{userIP}";
                if (!SecurityUtilities.VerifyHashedData(partialLock.Hashed, clearText))
                    return HandleUserPartialLockReturnModel(partialLock.SessionId, false, DateTime.Now, partialLock.ErrorTimesUserTries, "ErrorGeneric", $"Hashed Not Matched as we received hashed:{partialLock.Hashed}, clearText is: {clearText} and hash is {SecurityUtilities.HashData(clearText, null)}", lang);

                return partialLock;
            }
            catch (Exception ex)
            {
                return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 1, "ErrorGeneric", $"Exception error when ValidateIfUserPartiallyLocked: {ex.ToString()}", lang);
            }
        }

        private UserPartialLockModel HandleUserPartialLockReturnModel(string sessionId, bool isLocked, DateTime lockedDueDate, int errorTimesUserTries, string errorDescriptionKey, string logDescription, string lang)
        {
            UserPartialLockModel partialLock = new UserPartialLockModel()
            {
                LogDescription = logDescription,
                ErrorDescription = WebResources.ResourceManager.GetString(errorDescriptionKey, CultureInfo.GetCultureInfo(lang)),
                ErrorTimesUserTries = errorTimesUserTries,
                IsLocked = isLocked,
                LockDueDate = lockedDueDate,
                SessionId = sessionId,
            };
            return partialLock;
        }

        private string GeneratePartiallyLockHashedKey(string userIP, UserPartialLockModel lockModel)
        {
            //UserPartialLockModel lockModel = new UserPartialLockModel()
            //{
            //    SessionId = sessionId,
            //    ErrorTimesUserTries = tries
            //};

            string clearText = $"{lockModel.SessionId}_{SHARED_PARTIAL_LOCK_SECRET}_{userIP}";
            lockModel.Hashed = SecurityUtilities.HashData(clearText, null);

            return AESEncryption.EncryptString(JsonConvert.SerializeObject(lockModel), SHARED_PARTIAL_LOCK_SECRET);
        }

        private IdentityResult UpdateAcountsWithSamePhone(string phone, string currentUserId)
        {
            string exception = string.Empty;

            try
            {
                _authorizationService.GetAllUsersByPhoneAndUpdate(phone, currentUserId, out exception);
                if (!string.IsNullOrEmpty(exception))
                    return IdentityResult.Failed(exception);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.ToString());
            }
        }

        private async Task<ProfileOutput<LoginOutput>> HandleLogin(ProfileOutput<LoginOutput> Output, AspNetUser user, string password, string language, LoginRequestsLog log, Channel channel, bool sendYakeenMobileVerificationSMS)
        {
            try
            {
                if (user.IsCorporateUser)
                    return await _iCorporateContext.CorporateUserSignIn(user, password, language, log);

                var result = await SignInManager.PasswordSignInAsync(user.UserName, password, true, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            var exception = string.Empty;
                            bool value = SetUserAuthenticationCookies(user, channel, out exception);
                            if (!value)
                                return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized, WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(language)), $"Failed to save authentication Cookie due to: {exception}", language);

                            var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                            if (accessTokenResult == null)
                                return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.NullResult, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language)), $"accessTokenResult is null", language);
                            if (string.IsNullOrEmpty(accessTokenResult.access_token))
                                return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.EmptyResult, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language)), $"accessTokenResult.access_token is null", language);

                            string accessTokenResultGWT = string.Empty;
                            if (generalChannels.Contains(channel.ToString().ToLower()))
                            {
                                string gwtKey = string.Empty;
                                accessTokenResultGWT = _authorizationService.GenerateTokenJWT(user.Id.ToString(), user.Email, user.UserName, user.PhoneNumber, user.FullNameAr, user.FullName, out gwtKey);
                            }

                            ////
                            /// blew insert into table (LoginSwitchAccount) --> to check when login if user switched between his accounts more than 3 times in one year
                            /// 
                            var mobileInternalFormated = Utilities.ValidateInternalPhoneNumber(user.PhoneNumber);
                            var mobileInternationalFormated = Utilities.ValidatePhoneNumber(user.PhoneNumber);
                            //var checkSwitchAccountResult = CheckForSwitchedAccounts(user.NationalId, user.Email, user.PhoneNumber, DateTime.Now.Year, language);
                            //if (checkSwitchAccountResult.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                            //    return checkSwitchAccountResult;

                            //var checkSwitchAccountResult = CheckForSwitchedAccounts(user.NationalId, user.Email, user.PhoneNumber, language);
                            //if (checkSwitchAccountResult.ErrorCode != ProfileOutput<LoginOutput>.ErrorCodes.Success)
                            //    return checkSwitchAccountResult;

                            //var newSwitchedAccount = new LoginSwitchAccount() { UserId = user.Id, Nin = user.NationalId, PhoneNumber = mobileInternalFormated, Email = user.Email, CreatedDate = DateTime.Now };
                            //_loginSwitchAccountRepository.Insert(newSwitchedAccount);
                            Output.Result = new LoginOutput
                            {
                                UserId = user.Id,
                                AccessToken = accessTokenResult.access_token,
                                TokenExpirationDate = DateTime.Now.AddMinutes(30),
                                Email = user.Email,
                                AccessTokenGwt = accessTokenResultGWT,
                                TokenExpiryDate = appChannels.Contains(channel.ToString().ToLower()) ? (24 * 60) : 30,
                                DisplayNameAr = user.FullNameAr?.Split(' ')?[0],
                                DisplayNameEn = user.FullName?.Split(' ')?[0],
                            };

                            if (sendYakeenMobileVerificationSMS)
                                SendSMS(user.PhoneNumber, user.NationalId, SMSMethod.WebsiteOTP, "YakeenMobileVerificationSMS", language);
                            
                            MigrateUser(user.Id);

                            //// 
                            /// here add the user to loged in user table
                            /// 
                            var sessionOutput = HandleUserSession(user, accessTokenResult.access_token, channel);
                            if (sessionOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                            {
                                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\HandleSingleSession_error.txt", JsonConvert.SerializeObject(sessionOutput.LogDescription));
                            }

                            return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.Success, WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(language)), $"Success", language);
                        }
                    case SignInStatus.LockedOut:
                        return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.Lockout, WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(language)), $"This Account is LockedOut", language);

                    case SignInStatus.RequiresVerification:
                        return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed, WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(language)), $"Phone number is not Confirmed", language);

                    case SignInStatus.Failure:
                        return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized, WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(language)), $"SignInStatus.Failure", language);
                    default:
                        return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.NotAuthorized, WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(language)), $"incorrect username and password", language);
                }
            }
            catch (Exception ex)
            {
                return HandleLoginOutput(Output, log, ProfileOutput<LoginOutput>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(language)), $"HandleLogin exception, and error is: {ex.ToString()}", language);
            }
        }

        private ProfileOutput<bool> HandleUserSession(AspNetUser user, string token, Channel channel)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>();

            try
            {
                var userAgent = Utilities.GetUserAgent();
                var userIp = Utilities.GetUserIPAddress();
                var domain = (HttpContext.Current.Request.Url.Host).ToLower();
                var sessionData = _loginActiveTokensRepository.Table.Where(a => a.UserId == user.Id && a.IsValid && a.Channel.ToString().ToLower() == channel.ToString().ToLower()).ToList();
                if (sessionData != null)
                {
                    if (sessionData.Any(a => a.UserIP != userIp || a.UserAgent != userAgent))
                    {
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = user.PhoneNumber,
                            MessageBody = $"New Device signed-in  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))}",
                            Method = SMSMethod.NewDeviceNotification.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = Channel.Portal.ToString()
                        };

                        _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    }

                    foreach (var session in sessionData)
                    {
                        session.IsValid = false;
                        session.ExpiredDate = DateTime.Now.AddMinutes(-30);
                    }
                    _loginActiveTokensRepository.Update(sessionData);
                }

                var newSession = new LoginActiveTokens();
                newSession.Email = user.Email;
                newSession.PhoneNumber = user.PhoneNumber;
                newSession.UserId = user.Id;
                newSession.SessionId = token;
                newSession.IsValid = true;
                newSession.UserIP = userIp;
                newSession.UserAgent = userAgent;
                newSession.Channel = channel.ToString();
                newSession.Domain = domain;
                newSession.CreatedDate = DateTime.Now;
                newSession.ExpiredDate = DateTime.Now.AddMinutes(30);
                _loginActiveTokensRepository.Insert(newSession);

                output.Result = true;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.Result = false;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                output.ErrorDescription = "ExceptionError";
                output.LogDescription = ex.ToString();
                return output;
            }
        }

        //public async Task<ProfileOutput<bool>> HandleUserSession(string userId, string sessionToken)
        //{
        //    ProfileOutput<bool> output = new ProfileOutput<bool>();

        //    try
        //    {
        //        var user = await _authorizationService.GetUser(userId);
        //        //var deviceInfo = Utilities.GetFullDeviceInfo();
        //        var userAgent = Utilities.GetUserAgent();
        //        var userIp = Utilities.GetUserIPAddress();

        //        var sessionData = _loginActiveTokensRepository.Table.Where(a => a.UserId == userId).ToList(); //.FirstOrDefault(); //  && a.UserIP != userIp && a.IsValid
        //        if (sessionData != null)
        //        {
        //            foreach (var session in sessionData)
        //            {
        //                session.IsValid = false;
        //                session.ExpiredDate = DateTime.Now.AddMinutes(-30);
        //            }
        //            _loginActiveTokensRepository.Update(sessionData);

        //            if (sessionData.Any(a => a.SessionId != sessionToken))
        //            {
        //                var smsModel = new SMSModel()
        //                {
        //                    PhoneNumber = user.PhoneNumber,
        //                    MessageBody = "New Session signed-in with same account",
        //                    Method = SMSMethod.NewDeviceNotification.ToString(),
        //                    Module = Module.Vehicle.ToString(),
        //                    Channel = Channel.Portal.ToString()
        //                };

        //                _notificationService.SendSmsBySMSProviderSettings(smsModel);
        //            }
        //        }

        //        var newSession = new LoginActiveTokens();
        //        newSession.Email = user.Email;
        //        newSession.PhoneNumber = user.PhoneNumber;
        //        newSession.UserId = user.Id;
        //        newSession.SessionId = sessionToken; // token;
        //        newSession.IsValid = true;
        //        newSession.UserIP = userIp;
        //        newSession.UserAgent = userAgent;
        //        newSession.CreatedDate = DateTime.Now;
        //        newSession.ExpiredDate = DateTime.Now.AddMinutes(30);
        //        //if (deviceInfo != null)
        //        //{
        //        //    newSession.DeviceName = deviceInfo.DeviceName;
        //        //    newSession.DeviceType = deviceInfo.DeviceType;
        //        //    newSession.DeviceClient = deviceInfo.Client;
        //        //    newSession.DeviceOS = deviceInfo.OS;
        //        //}

        //        _loginActiveTokensRepository.Insert(newSession);

        //        output.Result = true;
        //        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\HandleUserSession_Login_Exception.txt", ex.ToString());
        //        output.Result = false;
        //        //output.HasAnotherSession = false;
        //        //output.UserId = string.Empty;
        //        //output.OldSessionToken = string.Empty;
        //        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
        //        output.ErrorDescription = "ExceptionError";
        //        output.LogDescription = ex.ToString();
        //        return output;
        //    }
        //}

        private ProfileOutput<LoginOutput> CheckForSwitchedAccounts(string nationalId, string email, string phone, string lang)
        {
            ProfileOutput<LoginOutput> output = new ProfileOutput<LoginOutput>()
            {
                ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.Success,
                ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(lang)),
                LogDescription = "Success"
            };

            try
            {
                var currentYear = DateTime.Now.Year;
                var start = new DateTime(currentYear, 1, 1);
                var end = new DateTime(currentYear, 12, 31);
                var mobileInternalFormated = Utilities.ValidateInternalPhoneNumber(phone);
                var mobileInternationalFormated = Utilities.ValidatePhoneNumber(phone);

                if (nationalId == "2529504074")
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_NationalId.txt", JsonConvert.SerializeObject(nationalId));
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_currentYear.txt", JsonConvert.SerializeObject(currentYear));
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_start.txt", JsonConvert.SerializeObject(start));
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_end.txt", JsonConvert.SerializeObject(end));
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_mobileInternalFormated.txt", JsonConvert.SerializeObject(mobileInternalFormated));
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\checkaccountsswitch_mobileInternationalFormated.txt", JsonConvert.SerializeObject(mobileInternationalFormated));
                }

                var switchedAccountsDetails = _loginSwitchAccountRepository.TableNoTracking
                                                .Where(a => a.Nin == nationalId
                                                    && (a.PhoneNumber == mobileInternalFormated || a.PhoneNumber == mobileInternationalFormated)).OrderByDescending(a => a.Id).ToList();

                if (nationalId == "2529504074")
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\login_checkaccountsswitch_switchedAccountsDetails.txt", JsonConvert.SerializeObject(switchedAccountsDetails));

                if (switchedAccountsDetails == null || switchedAccountsDetails.Count < 3)
                    return output;

                switchedAccountsDetails = switchedAccountsDetails.Where(a => a.CreatedDate >= start && a.CreatedDate <= end).ToList();
                if (switchedAccountsDetails == null || switchedAccountsDetails.Count < 3)
                    return output;

                var lastSwitchedAccount = switchedAccountsDetails.FirstOrDefault();
                if (lastSwitchedAccount.Email != email)
                {
                    output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.SwitchingAccountsHasReachedTheLimit;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SwitchingAccountsHasReachedTheLimit", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = $"Login validation --> This user with NationalId: {nationalId} and PhoneNumber: {phone} reaches the limit for switching between his accounts";
                    return output;
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<LoginOutput>.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = $"CheckForSwitchedAccounts validation --> This user with NationalId: {nationalId} and PhoneNumber: {phone} reaches the limit for switching between his accounts";
                return output;
            }
        }

        private ProfileOutput<LoginOutput> HandleLoginOutput(ProfileOutput<LoginOutput> output, LoginRequestsLog log, ProfileOutput<LoginOutput>.ErrorCodes outputCode, string outputMessageKey, string logMessage, string lang)
        {
            output.ErrorCode = outputCode;
            output.ErrorDescription = outputMessageKey;
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = logMessage;
            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
            return output;
        }

        #endregion


        #region Resend OTP

        public SMSOutput ReSendOTPCode(ResendOTPModel model)
        {
            SMSOutput output = new SMSOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.Email;
            log.Mobile = model.Phone;
            log.Method = "ReSendOTP";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();

            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = output.ErrorCode;
                    log.ErrorDescription = "model.Email";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return output;
                }
                var encryptedEmail = Convert.FromBase64String(model.Email.Trim());
                var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                if (string.IsNullOrEmpty(plainEmail))
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = output.ErrorCode;
                    log.ErrorDescription = "ReSendOTPCode model.Email is not valid email";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return output;
                }
                if (!Utilities.IsValidMail(plainEmail))
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("checkout_error_email", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = output.ErrorCode;
                    log.ErrorDescription = $"mail is not valid as the passed email is: {plainEmail}";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return output;
                }
                log.Email = plainEmail;

                var user = UserManager.FindByEmail(plainEmail);
                Guid userId = Guid.Empty;
                if (user != null)
                    Guid.TryParse(user.Id, out userId);

                var otpOutput = HandleSendingOTP(user.PhoneNumber, user.Email, user.NationalId, Guid.Empty, SMSMethod.WebsiteOTP, "VerifyOTP", "ReSendOTPCode", model.Language);
                if (otpOutput.ErrorCode != 0)
                {
                    output.ErrorCode = otpOutput.ErrorCode;
                    output.ErrorDescription = otpOutput.ErrorDescription;
                    log.ErrorCode = output.ErrorCode;
                    log.ErrorDescription = otpOutput.LogDescription;
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return output;
                }

                output.ErrorCode = 1;
                output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = 500;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return output;
            }
        }

        #endregion


        #region New Logic Shared Methods

        private bool RemoveUserIdFromQuotationRequest(AspNetUser userInfo, string errorForField, string lang, out string exception)
        {
            exception = string.Empty;

            try
            {
                _quotationService.RemoveUserIdFromQuotationRequest(userInfo.Id, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    exception = $"RemoveUserIdFromQuotationRequest for {errorForField} return error, and error is: {exception}";
                    return false;
                }

                var resultInfo = UserManager.Delete(userInfo);
                if (!resultInfo.Succeeded)
                {
                    exception = $"RemoveUserIdFromQuotationRequest resultInfo.Succeeded for {errorForField} is: {resultInfo.Succeeded} and error is: {string.Join(",", resultInfo.Errors)}";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = $"RemoveUserIdFromQuotationRequest for {errorForField} exception error, userId is {userInfo.Id} and error is: {ex.ToString()}";
                return false;
            }
        }

        private YakeenMobileVerificationOutput VerifyMobileFromYakeen(string nationalId, string mobile, string lang, out string exception)
        {
            YakeenMobileVerificationOutput yakeenMobileVerification = null;
            exception = string.Empty;

            try
            {
                var yakeenMobileVerificationDto = new YakeenMobileVerificationDto()
                {
                    NationalId = nationalId,
                    Phone = mobile
                };
                yakeenMobileVerification = _iYakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, lang);
                if (yakeenMobileVerification.ErrorCode == YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner)
                {
                    yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner;
                    yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(lang));
                    exception = yakeenMobileVerification.ErrorDescription;
                    return yakeenMobileVerification;
                }
                if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                {
                    yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceError;
                    yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang));
                    exception = yakeenMobileVerification.ErrorDescription;
                    return yakeenMobileVerification;
                }

                return yakeenMobileVerification;
            }
            catch (Exception ex)
            {
                yakeenMobileVerification = new YakeenMobileVerificationOutput()
                {
                    ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceException,
                    ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang))
                };
                exception = ex.ToString();
                return yakeenMobileVerification;
            }
        }

        private UserDataModel GetUserData(string nationalId, Channel channel, out string exception)
        {
            exception = string.Empty;
            UserDataModel userDataModel = new UserDataModel() { IsExist = false };

            try
            {
                var userData = _driverRepository.TableNoTracking.Where(a => a.NIN == nationalId && !a.IsDeleted).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (userData != null)
                {
                    userDataModel.IsExist = true;
                    userDataModel.FullNameAr = HandleUserName(userData.FirstName, userData.SecondName, userData.ThirdName, userData.LastName);
                    userDataModel.FullNameEn = HandleUserName(userData.EnglishFirstName, userData.EnglishSecondName, userData.EnglishThirdName, userData.EnglishLastName);
                }

                if (!userDataModel.IsExist && generalChannels.Contains(channel.ToString().ToLower()))
                {
                    exception = string.Empty;
                    userDataModel = GetUserDataFromGeneralAsync(nationalId, channel, out exception);
                }

                return userDataModel;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        private UserDataModel GetUserDataFromGeneralAsync(string nationalId, Channel channel, out string exception)
        {
            exception = string.Empty;
            UserDataModel generalUserData = null;
            try
            {
                string Url = string.Empty;
                if (channel.ToString().ToLower() == Channel.Travel.ToString().ToLower())
                    Url = $"{General_travel_URL}?NIN={nationalId}";
                else if (channel.ToString().ToLower() == Channel.Medical.ToString().ToLower())
                    Url = $"{General_mmalpractice_URL}?NIN={nationalId}";

                var tokenBase46 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(GeneralAccessTokenCredentials));
                var response = _httpClient.GetAsync(Url, tokenBase46, "Basic", null).Result;
                if (response == null)
                    exception = "response is null";
                else if (!response.IsSuccessStatusCode)
                    exception = $"response.IsSuccessStatusCode is false as {response.IsSuccessStatusCode}";
                else if (response.Content == null)
                    exception = $"response.Content != null";
                else
                {
                    var value = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(value))
                        exception = "response.Content.ReadAsStringAsync().Result is null";
                    else
                        generalUserData = JsonConvert.DeserializeObject<UserDataModel>(value);
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }

            return generalUserData;
        }

        private UserDataModel GetUserDataFromYakeen(string nationalId, int birthYear, int birthMonth, Channel channel, string lang, out string logException, out string outputDescription)
        {
            logException = string.Empty;
            outputDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
            UserDataModel userDataModel = null;

            try
            {
                long _nationalId = 0;
                long.TryParse(nationalId, out _nationalId);
                var customerYakeenRequest = new CustomerYakeenRequestDto()
                {
                    Nin = _nationalId,
                    IsCitizen = nationalId.StartsWith("1"),
                    DateOfBirth = string.Format("{0}-{1}", birthMonth.ToString("00"), birthYear)
                };
                ServiceRequestLog predefinedlog = new ServiceRequestLog() { DriverNin = nationalId };
                var customerIdInfo = _iYakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedlog);
                if (!customerIdInfo.Success)
                {
                    string ErrorMessage = SubmitInquiryResource.ResourceManager.GetString($"YakeenError_{customerIdInfo.Error?.ErrorCode}", CultureInfo.GetCultureInfo(lang));
                    var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                    outputDescription = GenericErrorMessage;
                    logException = $"iYakeenClient.GetCustomerIdInfo return status {customerIdInfo.Success}, and error is: {JsonConvert.SerializeObject(customerIdInfo.Error?.ErrorMessage)}";
                    return userDataModel;
                }

                userDataModel = new UserDataModel()
                {
                    IsExist = true,
                    FullNameAr = HandleUserName(customerIdInfo.FirstName, customerIdInfo.SecondName, customerIdInfo.ThirdName, customerIdInfo.LastName),
                    FullNameEn = HandleUserName(customerIdInfo.EnglishFirstName, customerIdInfo.EnglishSecondName, customerIdInfo.EnglishThirdName, customerIdInfo.EnglishLastName)
                };
                return userDataModel;
            }
            catch (Exception ex)
            {
                logException = $"GetUserDataFromYakeen exception, and error is: {ex.ToString()}";
                return null;
            }
        }

        private string HandleUserName(string firstName, string secondName, string thirdName, string lastName)
        {
            var name = new List<string>
            {
                (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrEmpty(firstName)) ? "-" : firstName,
                (string.IsNullOrWhiteSpace(secondName) || string.IsNullOrEmpty(secondName)) ? "-" : secondName,
                (string.IsNullOrWhiteSpace(thirdName) || string.IsNullOrEmpty(thirdName)) ? "-" : thirdName,
                (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrEmpty(lastName)) ? "-" : lastName
            };

            return string.Join(" ", name);
        }

        private SMSOutput HandleSendingOTP(string phoneNumber, string email, string nin, Guid userId, SMSMethod smsMethod, string messageResourceCode, string messageSource, string lang)
        {
            SMSOutput output = new SMSOutput() { ErrorCode = 0, ErrorDescription = "Success" };
            try
            {
                var otpOutput = SendOTPCode(phoneNumber, email, nin, userId, smsMethod, messageResourceCode, lang);
                if (otpOutput.ErrorCode == 2)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = $"{messageSource} ErrorCode == 2, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                }
                if (otpOutput.ErrorCode != 0)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = $"{messageSource} ErrorCode != 0, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = 500;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = $"{messageSource} Exception, failed To Send Otp due to: {ex.ToString()}";
                return output;
            }
        }

        private SMSOutput SendOTPCode(string phoneNumber, string email, string nin, Guid userId, SMSMethod smsMethod, string messageResourceCode, string lang)
        {
            Random rnd = new Random();
            int verifyCode = rnd.Next(1000, 9999);

            OtpInfo info = new OtpInfo();
            info.UserId = userId;
            info.PhoneNumber = phoneNumber;
            info.UserEmail = email;
            info.Nin = nin;
            info.VerificationCode = verifyCode;
            info.CreatedDate = DateTime.Now;
            info.ModifiedDate = DateTime.Now;
            _otpInfo.Insert(info);
            var smsModel = new SMSModel()
            {
                PhoneNumber = phoneNumber,
                MessageBody = WebResources.ResourceManager.GetString(messageResourceCode, CultureInfo.GetCultureInfo(lang)).Replace("{0}", verifyCode.ToString()), //WebResources.VerifyOTP.Replace("{0}", verifyCode.ToString()),
                Method = smsMethod.ToString(),
                Module = Module.Vehicle.ToString()
            };

            return _notificationService.SendSmsBySMSProviderSettings(smsModel);
        }


        private SMSOutput SendSMS(string phone, string nationalId, SMSMethod smsMethod, string messageResourceCode, string lang)
        {
            try
            {
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phone,
                    MessageBody = WebResources.ResourceManager.GetString(messageResourceCode, CultureInfo.GetCultureInfo(lang)).Replace("{0}", nationalId), //WebResources.VerifyOTP.Replace("{0}", verifyCode.ToString()),
                    Method = smsMethod.ToString(),
                    Module = Module.Vehicle.ToString()
                };

                return _notificationService.SendSmsBySMSProviderSettings(smsModel);
            }
            catch (Exception ex)
            {
                SMSOutput output = new SMSOutput();
                output.ErrorCode = 500;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = ex.ToString();
                throw;
            }
        }

        //private DeviceInfo GetFullDeviceInfo()
        //{
        //    var deviceInfo = new DeviceDetector(Utilities.GetUserAgent());
        //    deviceInfo.Parse();
        //    if (deviceInfo == null)
        //        return null;

        //    DeviceInfo deviceInfoModel = new DeviceInfo();
        //    deviceInfoModel.DeviceType = $"{deviceInfo.GetDeviceName()}";
        //    deviceInfoModel.DeviceName = $"{deviceInfo.GetBrandName()}-{deviceInfo.GetModel()}";
        //    if (deviceInfo.GetOs().Success)
        //        deviceInfoModel.OS = $"{deviceInfo.GetOs().Match.Name}-{deviceInfo.GetOs().Match.Platform}-{deviceInfo.GetOs().Match.Version}";

        //    if (deviceInfo.GetBrowserClient().Success)
        //        deviceInfoModel.Client = $"{deviceInfo.GetBrowserClient().Match.Name}-{deviceInfo.GetBrowserClient().Match.Version}";

        //    return deviceInfoModel;
        //}

        #endregion

        #endregion


        #region Forgot Password New Logic

        public async Task<ProfileOutput<ForgotPasswordResponseModel>> BeginForgetPassword(ForgotPasswordRequestViewModel model)
        {
            ProfileOutput<ForgotPasswordResponseModel> Output = new ProfileOutput<ForgotPasswordResponseModel>();
            Output.Result = new ForgotPasswordResponseModel();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Email = model.MobileOrEmail;
            log.Nin = model.NationalId;
            log.Mobile = model.MobileOrEmail;
            log.Method = "BeginForgetPassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //log.Code = JsonConvert.SerializeObject(model);

            try
            {
                var cachKey = $"{model.Channel}_ForgetPassword_{base_KEY}_{log.UserIP}";
                UserPartialLockModel partialLock = await ValidateIfUserPartiallyLocked(model.Hashed, log.UserIP, cachKey, model.Language);
                if (partialLock.IsLocked)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Lockout, WebResources.ResourceManager.GetString("LoginUserPartialLock", CultureInfo.GetCultureInfo(model.Language)), $"user is partially locked as he exceeds the max tries: {partialLock.ErrorTimesUserTries}", model.Language);

                int forgotBbyTypeId = 0;
                var validateOutput = ValidateForgetPasswordRequestModel(model, out forgotBbyTypeId);
                if (validateOutput.ErrorCode != ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success)
                    return HandleForgetPasswordOutput(Output, log, validateOutput.ErrorCode, validateOutput.ErrorDescription, validateOutput.LogDescription, model.Language);

                if (!Enum.IsDefined(typeof(ForgotBbyTypeEnum), forgotBbyTypeId))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"Forgot type is invalid after validate as value is {forgotBbyTypeId}", model.Language);

                AspNetUser user = null;
                string errorDescriptionKey = "ErrorGeneric";
                switch (forgotBbyTypeId)
                {
                    case (int)ForgotBbyTypeEnum.ByEmail:
                        user = _authorizationService.GetUserInfoByEmail(model.MobileOrEmail, string.Empty, string.Empty);
                        errorDescriptionKey = "login_incorrect_email_message";
                        break;

                    case (int)ForgotBbyTypeEnum.ByMobile:
                        user = _authorizationService.GetUserInfoByEmail(string.Empty, string.Empty, model.MobileOrEmail);
                        errorDescriptionKey = "PhoneNotExist";
                        break;

                    case (int)ForgotBbyTypeEnum.ByNationalId:
                        user = _authorizationService.GetUserInfoByNationalId(model.NationalId);
                        errorDescriptionKey = "NationalNotExist";
                        break;

                    default:
                        break;
                }

                if (user == null)
                {
                    ////
                    /// here will insert in redis if user tryies >= 10
                    partialLock.ErrorTimesUserTries += 1;
                    if (partialLock.ErrorTimesUserTries >= 10)
                    {
                        partialLock.IsLocked = true;
                        partialLock.LockDueDate = DateTime.Now.AddMinutes(5);
                        await RedisCacheManager.Instance.SetAsync(cachKey, partialLock, 5 * 60);
                    }
                    else
                        Output.Result.Hashed = GeneratePartiallyLockHashedKey(log.UserIP, partialLock);

                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.UserNotFound, WebResources.ResourceManager.GetString(errorDescriptionKey, CultureInfo.GetCultureInfo(model.Language)), $"User is null email: {model.MobileOrEmail}", model.Language);
                }
                if (forgotBbyTypeId == (int)ForgotBbyTypeEnum.ByNationalId && !user.IsPhoneVerifiedByYakeen)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.UserNotFound, WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)), $"User phone is not verified by yakeen", model.Language);

                return await HandleForgotPasswordSendVerificationCode(model, user, forgotBbyTypeId, Output, log);
            }
            catch (Exception ex)
            {
                return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"Exception error, and error is: {ex.ToString()}", model.Language);
            }
        }

        public async Task<ProfileOutput<ForgotPasswordResponseModel>> EndForgetPassword(ForgotPasswordRequestViewModel model)
        {
            ProfileOutput<ForgotPasswordResponseModel> Output = new ProfileOutput<ForgotPasswordResponseModel>();
            Output.Result = new ForgotPasswordResponseModel();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "EndForgetPassword";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //log.Code = JsonConvert.SerializeObject(model);

            try
            {
                if (model == null)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model is empty", model.Language);

                if (!model.IsResetByEmail && !model.IsResetByMobile)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "forgot method is not selected", model.Language);
                
                if (string.IsNullOrEmpty(model.UserId))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model.UserId", model.Language);

                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"There is no user with this id: {model.UserId}", model.Language);

                log.Email = user.Email;
                log.Nin = user.NationalId;
                log.Mobile = user.PhoneNumber;

                int forgotBbyTypeId = (model.IsResetByEmail) ? (int)ForgotBbyTypeEnum.ByEmail : (int)ForgotBbyTypeEnum.ByMobile;
                return await HandleForgotPasswordSendVerificationCode(model, user, forgotBbyTypeId, Output, log);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
        }

        public async Task<ProfileOutput<ForgotPasswordResponseModel>> VerifyForgetPasswordOTP(VerifyForgotPasswordOTPRequestModel model)
        {
            ProfileOutput<ForgotPasswordResponseModel> Output = new ProfileOutput<ForgotPasswordResponseModel>();
            Output.Result = new ForgotPasswordResponseModel();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "VerifyForgetPasswordOTP";
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //log.Code = JsonConvert.SerializeObject(model);

            try
            {
                if (model == null)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model is empty", model.Language);

                if (string.IsNullOrEmpty(model.UserId))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model.UserId is null", model.Language);
                
                if (!model.Otp.HasValue)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ErrorOTPEmpty", CultureInfo.GetCultureInfo(model.Language)), "model.otp is null", model.Language);

                if (string.IsNullOrEmpty(model.NewPassword))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model.NewPassword is null", model.Language);
                
                if (string.IsNullOrEmpty(model.ConfirmNewPassword))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), "model.ConfirmNewPassword is null", model.Language);

                if (model.NewPassword != model.ConfirmNewPassword)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter, WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language)), $"Password : {model.NewPassword} not equal ConfirmNewPassword:{model.ConfirmNewPassword}", model.Language);

                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"There is no user with this id: {model.UserId}", model.Language);

                log.Email = user.Email;
                log.Nin = user.NationalId;
                log.Mobile = user.PhoneNumber;

                var forgotPasswordToken = _forgotPasswordTokenRepository.Table.Where(a => a.UserId == user.Id && a.ForgotPasswordVerificationType == (int)ForgotPasswordVerificationTypeEnum.Phone).OrderByDescending(a => a.Id).FirstOrDefault();
                if (forgotPasswordToken == null || string.IsNullOrEmpty(forgotPasswordToken.Token) || forgotPasswordToken.VerificationCode <= 0)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"No reset password token for userId {user.Id}", model.Language);

                if (forgotPasswordToken.VerificationCode != model.Otp.Value)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData, WebResources.ResourceManager.GetString("ForgotPasswordInvalidOTP", CultureInfo.GetCultureInfo(model.Language)), $"Invalid otp as we recived: {model.Otp.Value}, and correct one is:{forgotPasswordToken.VerificationCode}", model.Language);

                if (forgotPasswordToken.CreatedDate.AddMinutes(10) < DateTime.Now)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData, WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(model.Language)), $"OTP Expire", model.Language);

                var res = await UserManager.ResetPasswordAsync(model.UserId, forgotPasswordToken.Token, model.NewPassword);
                if (!res.Succeeded)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), $"ResetPasswordAsync Failed and the error is: " + String.Join(",", res.Errors.ToList()), model.Language);

                if (user.IsCorporateUser)
                {
                    var corporateUser = _corporateUsersRepository.Table.FirstOrDefault(u => u.UserId == user.Id && u.IsActive);
                    if (corporateUser != null)
                    {
                        corporateUser.PasswordHash = SecurityUtilities.HashData(model.NewPassword, null);
                        _corporateUsersRepository.Update(corporateUser);
                    }
                }

                forgotPasswordToken.IsCodeVerified = true;
                forgotPasswordToken.ModifiedDate = DateTime.Now;
                _forgotPasswordTokenRepository.Update(forgotPasswordToken);

                return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success, WebResources.ResourceManager.GetString("ForgotPasswordSuccess", CultureInfo.GetCultureInfo(model.Language)), $"Success", model.Language);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
        }

        #region Forgot Password New Logic Private methods

        private ProfileOutput<ForgotPasswordResponseModel> ValidateForgetPasswordRequestModel(ForgotPasswordRequestViewModel model, out int forgotBbyType)
        {
            forgotBbyType = 0;
            ProfileOutput<ForgotPasswordResponseModel> output = new ProfileOutput<ForgotPasswordResponseModel>()
            {
                ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success,
                ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Language))
            };

            if (model == null)
            {
                output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ModelIsEmpty", CultureInfo.GetCultureInfo(model.Language));
                output.LogDescription = "model is empty";
                return output;
            }
            //if (string.IsNullOrEmpty(model.CaptchaInput) || string.IsNullOrEmpty(model.CaptchaToken))
            //{
            //    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter;
            //    output.ErrorDescription = WebResources.ResourceManager.GetString("CaptchaIsNull", CultureInfo.GetCultureInfo(model.Language));
            //    output.LogDescription = "model.CaptchaInput is empty or model.CaptchaToken is empty";
            //    return output;
            //}

            ////
            /// stop validate captcha as we lock the user if he reaches 10 times tryies with wrong(Email / Phone / NatinalId)
            //var validCaptchaOutput = ValidateCaptcha(model.CaptchaInput, model.CaptchaToken, model.Language);
            //if (validCaptchaOutput.ErrorCode != ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success)
            //    return validCaptchaOutput;

            if (model.IsResetByNationalId)
            {
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model is empty";
                    return output;
                }
                if (model.NationalId.Length != 10)
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdIsNotValid", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"model.NationalId length is not valid as the passed value is {model.NationalId}";
                    return output;
                }
                if (!model.NationalId.StartsWith("1") && !model.NationalId.StartsWith("2"))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NationalIdIsNotValid", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = $"model.NationalId is not valid as the passed value is {model.NationalId}";
                    return output;
                }
                if (invalidKeyWords.Contains(model.NationalId))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model.NationalId contains sensitive words";
                    return output;
                }

                forgotBbyType = (int)ForgotBbyTypeEnum.ByNationalId;
                return output;
            }
            else if (model.IsResetByEmailOrMobile)
            {
                if (string.IsNullOrEmpty(model.MobileOrEmail))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ForgetPasswordEmailAndMobileEmpty", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "Email is empty or Mobile is empty";
                    return output;
                }
                if (invalidKeyWords.Contains(model.MobileOrEmail))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.InvalidData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    output.LogDescription = "model.MobileOrEmail contains sensitive words";
                    return output;
                }

                if (Utilities.IsValidPhoneNo(model.MobileOrEmail))
                {
                    forgotBbyType = (int)ForgotBbyTypeEnum.ByMobile;
                    return output;
                }

                string logException = string.Empty;
                string outException = string.Empty;
                if (IsValidEmail(model.MobileOrEmail, model.Language, out logException, out outException))
                {
                    forgotBbyType = (int)ForgotBbyTypeEnum.ByEmail;
                    return output;
                }

                output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = outException;
                output.LogDescription = logException;
                return output;
            }

            output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError;
            output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
            output.LogDescription = "model.IsResetByNationalId is false & model.IsResetByEmailOrMobile is false";
            return output;
        }

        private ProfileOutput<ForgotPasswordResponseModel> ValidateCaptcha(string captchaInput, string captchaToken, string lang)
        {
            ProfileOutput<ForgotPasswordResponseModel> output = new ProfileOutput<ForgotPasswordResponseModel>() { ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success };
            var encryptedCaptcha = AESEncryption.DecryptString(captchaToken, SHARED_SECRET);
            try
            {
                var captchaTokenDeserialized = JsonConvert.DeserializeObject<IdentityCaptchaToken>(encryptedCaptcha);
                if (captchaTokenDeserialized.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExpiredCaptcha;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("CaptchaExpired", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = "Expired Captcha";
                    return output;
                }
                if (!captchaTokenDeserialized.Captcha.Equals(captchaInput, StringComparison.Ordinal))
                {
                    output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.WrongInputCaptcha;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidCaptcha", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = "Wrong Input Captcha";
                    return output;
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.ExceptionError;
                output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidCaptcha", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = $"Exception happend while validate captcha: {ex.ToString()}";
                return output;
            }
        }

        private bool IsValidEmail(string emailaddress, string lang, out string logException, out string outException)
        {
            logException = string.Empty;
            outException = string.Empty;
            MailAddress address = new MailAddress(emailaddress);
            if (address != null && string.IsNullOrEmpty(address.Host))
            {
                outException = ProfileResources.ResourceManager.GetString("InvalidEmailAddress", CultureInfo.GetCultureInfo(lang));
                logException = $"email is not valid as address = {address}";
                return false;
            }

            //var exist = _authorizationService.CheckEmailExist(emailaddress);
            //if (!exist)
            //{
            //    outException = WebResources.ResourceManager.GetString("EmailNotExist", CultureInfo.GetCultureInfo(lang));
            //    logException = $"email is exist with another user";
            //    return false;
            //}

            return true;
        }

        private async Task<ProfileOutput<ForgotPasswordResponseModel>> HandleForgotPasswordSendVerificationCode(ForgotPasswordRequestViewModel model, AspNetUser user, int forgotBbyTypeId, ProfileOutput<ForgotPasswordResponseModel> Output, RegistrationRequestsLog log)
        {
            Output.Result = new ForgotPasswordResponseModel();
            Output.Result.UserId = user.Id;

            if (forgotBbyTypeId == (int)ForgotBbyTypeEnum.ByNationalId)
            {
                var splitedEmail = user.Email.Split('@');
                Output.Result.MaskedEmail = $"{MaskString(splitedEmail[0], 2, 2, '*')}@{MaskString(splitedEmail[1], 0, 0, '*')}";
                Output.Result.MaskedPhone = MaskString(user.PhoneNumber, 0, 4, '*');

                Output.Result.NationalIdExist = true;
                Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.Success;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
            else if (forgotBbyTypeId == (int)ForgotBbyTypeEnum.ByMobile)
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if (string.IsNullOrEmpty(token))
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.TokenIsEmpty, WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)), "token is null", model.Language);

                var otpOutput = SendForgotPasswordOTPCode(user.PhoneNumber, user.Id, token,model.Language);
                if (otpOutput.ErrorCode != 0)
                    return HandleForgetPasswordOutput(Output, log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.CanNotSendSMS, otpOutput.ErrorDescription, otpOutput.LogDescription, model.Language);

                Output.ErrorDescription = WebResources.ResourceManager.GetString("ForgotPasswordPhoneSent", CultureInfo.GetCultureInfo(model.Language)).Replace("{0}", MaskString(user.PhoneNumber, 0, 4, '*'));
                Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.VerifyPhone;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }
            else if (forgotBbyTypeId == (int)ForgotBbyTypeEnum.ByEmail)
            {
                string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                if (string.IsNullOrEmpty(token))
                {
                    Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.TokenIsEmpty;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "token is null";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                ForgotPasswordToken forgotPasswordToken = new ForgotPasswordToken()
                {
                    UserId = user.Id,
                    Token = token,
                    CreatedDate = DateTime.Now,
                    ForgotPasswordVerificationType = (int)ForgotPasswordVerificationTypeEnum.Email
                };
                _forgotPasswordTokenRepository.Insert(forgotPasswordToken);

                string subject = WebResources.ResourceManager.GetString("ForgetPasswordEmailSubject", CultureInfo.GetCultureInfo(model.Language));
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PasswordReset.png";
                messageBodyModel.Language = model.Language;
                messageBodyModel.MessageBody = HandleForgotPasswordBodyURL(user.Id, token, model.Channel.ToString().ToLower(), model.Language);

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(user.Email);
                emailModel.Subject = subject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "ForgetPassword";
                emailModel.Channel = model.Channel.ToString();
                var sendMail = _notificationService.SendEmail(emailModel);
                if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmailNotSent;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "can not send email, and error is: " + sendMail.ErrorDescription;
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Output;
                }

                Output.ErrorDescription = WebResources.ResourceManager.GetString("ForgotPasswordEmailSent", CultureInfo.GetCultureInfo(model.Language));
                Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.EmailSent;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Output;
            }

            Output.ErrorCode = ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes.NotSuccess;
            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
            log.ErrorCode = (int)Output.ErrorCode;
            log.ErrorDescription = $"ForgotBbyTypeEnum.ByEmail is not supported as passe value is {forgotBbyTypeId}";
            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
            return Output;
        }

        private string HandleForgotPasswordBodyURL(string userId, string token, string channel, string lang)
        {
            string url = string.Empty;
            switch (channel)
            {
                case "travel":
                    url = $"{General_travel_ForgotPasswordConfirmation_URL}{userId}&token={token}";
                    break;
                case "medicalmalpractices":
                    url = $"{General_mmalpractice_ForgotPasswordConfirmation_URL}{userId}&token={token}";
                    break;
                default:
                    url = $"{Utilities.SiteURL}/ConfirmResetPassword?userId={userId}&token={token}";
                    break;
            }
            
            var body = string.Format(WebResources.ResourceManager.GetString("EmailPasswordResetBody", CultureInfo.GetCultureInfo(lang)), url);
            return body;
        }

        private SMSOutput SendForgotPasswordOTPCode(string phoneNumber, string userId, string token, string lang)
        {
            SMSOutput output = new SMSOutput() { ErrorCode = 0, ErrorDescription = "Success" };
            try
            {
                Random rnd = new Random();
                int verifyCode = rnd.Next(1000, 9999);

                ForgotPasswordToken forgotPasswordToken = new ForgotPasswordToken()
                {
                    UserId = userId,
                    Token = token,
                    Phone = phoneNumber,
                    VerificationCode = verifyCode,
                    CreatedDate = DateTime.Now,
                    ForgotPasswordVerificationType = (int)ForgotPasswordVerificationTypeEnum.Phone
                };
                _forgotPasswordTokenRepository.Insert(forgotPasswordToken);

                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneNumber,
                    MessageBody = WebResources.ResourceManager.GetString("ForgetPasswordVerifyOTP", CultureInfo.GetCultureInfo(lang)).Replace("{0}", verifyCode.ToString()),
                    Method = SMSMethod.WebsiteOTP.ToString(),
                    Module = Module.Vehicle.ToString()
                };

                var otpOutput =  _notificationService.SendSmsBySMSProviderSettings(smsModel);
                if (otpOutput.ErrorCode == 2)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("OtpReachedMaximum", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = $"SendForgotPasswordOTPCode ErrorCode == 2, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                }
                if (otpOutput.ErrorCode != 0)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(lang));
                    output.LogDescription = $"SendForgotPasswordOTPCode ErrorCode != 0, failed To Send Otp due to: {otpOutput.ErrorDescription}";
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = 500;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = $"SendForgotPasswordOTPCode Exception, failed To Send Otp due to: {ex.ToString()}";
                return output;
            }
        }

        private string MaskString(string inputString, int leftUnMaskLength, int rightUnMaskLength, char mask)
        {
            if ((leftUnMaskLength + rightUnMaskLength) > inputString.Length)
                return inputString;

            return inputString.Substring(0, leftUnMaskLength) +
                new string(mask, inputString.Length - (leftUnMaskLength + rightUnMaskLength)) +
                inputString.Substring(inputString.Length - rightUnMaskLength);
        }

        private ProfileOutput<ForgotPasswordResponseModel> HandleForgetPasswordOutput(ProfileOutput<ForgotPasswordResponseModel> output, RegistrationRequestsLog log, ProfileOutput<ForgotPasswordResponseModel>.ErrorCodes outputCode, string outputMessageKey, string logMessage, string lang)
        {
            output.ErrorCode = outputCode;
            output.ErrorDescription = outputMessageKey;
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = logMessage;
            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
            return output;
        }


        #endregion

        #endregion
    }
}
