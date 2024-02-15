using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Loggin.DAL;
//using Tameenk.Loggin.DAL.Entities;
using Tameenk.Resources;
using Tameenk.Resources.Account;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.Profile;
using Tameenk.Resources.Vehicles;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Extensions;
using Tameenk.Services.IdentityApi.App_Start;
using Tameenk.Services.IdentityApi.Output;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;
using Tameenk.Services.Profile.Component.Models;
using ProfileComponent = Tameenk.Services.Profile.Component;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.IdentityApi.Controllers
{
    //[Authorize]
    public class AccountController : IdentityBaseController
    {

        #region Fields
        private readonly IAuthorizationService _authorizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly TameenkConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private LoginRequestsLog loginLog;
        private ProfileRequestsLog profileLog;
        private RegistrationRequestsLog registrationLog;
        private readonly Profile.Component.ICorporateContext _iCorporateContext;
        private readonly Tameenk.Services.Profile.Component.IAuthenticationContext _authenticationContext;

        private readonly IRepository<ExpiredTokens> _expiredTokensRepository;
        private readonly IRepository<LoginActiveTokens> _loginActiveTokensRepository;
        private const string Send_Confirmation_Email_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        #endregion

        #region Ctor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="authorizationService">The authorization service.</param>
        /// <param name="shoppingCartService">The authorization service.</param>
        public AccountController(IAuthorizationService authorizationService, IShoppingCartService shoppingCartService, TameenkConfig tameenkConfig, IHttpClient httpClient, ILogger logger, Tameenk.Services.Profile.Component.IAuthenticationContext authenticationContext, IRepository<ExpiredTokens> expiredTokensRepository, Profile.Component.ICorporateContext iCorporateContext,
            IRepository<LoginActiveTokens> loginActiveTokensRepository)
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _config = tameenkConfig ?? throw new ArgumentNullException(nameof(tameenkConfig));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _expiredTokensRepository = expiredTokensRepository ?? throw new ArgumentNullException(nameof(expiredTokensRepository)); ;
            _authenticationContext = authenticationContext ?? throw new ArgumentNullException(nameof(authenticationContext));
            _iCorporateContext = iCorporateContext;
            _loginActiveTokensRepository = loginActiveTokensRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// register new user 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/register")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Tameenk.Services.Profile.Component.RegisterationModel model)
        {
            var outputResult = await _authenticationContext.Register(model);
            return Single(outputResult);
        }
        //[HttpPost]        //[Route("api/identity/registerUser")]        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]        //[AllowAnonymous]
        //public async Task<IHttpActionResult> RegisterUser([FromBody] RegisterModel model)
        //{
        //    Output<RegisterOutput> Output = new Output<RegisterOutput>();
        //    Output.Result = new RegisterOutput();
        //    RegistrationRequestsLog log = new RegistrationRequestsLog();
        //    log.Email = model.Email;
        //    log.Method = "Register";
        //    log.Mobile = model.Mobile;
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.Email))
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "mail is empty";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }
        //        if (string.IsNullOrEmpty(model.Password))
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "Password is empty";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }

        //        if (string.IsNullOrEmpty(model.Mobile))
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "Mobile is empty";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }
        //        var userVal = _authorizationService.IsEmailOrPhoneExist(model.Email, model.Mobile);
        //        if (userVal != null && userVal.Email.ToLower() == model.Email.ToLower())
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "email already exist";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }
        //        if (userVal != null && userVal.PhoneNumber == model.Mobile)
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_signup_error", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "mobile already exist";
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }
        //        var user = new Tameenk.Core.Domain.Entities.AspNetUser
        //        {
        //            CreatedDate = DateTime.Now,
        //            LastModifiedDate = DateTime.Now,
        //            LastLoginDate = DateTime.Now,
        //            LockoutEndDateUtc = DateTime.UtcNow,
        //            DeviceToken = "",
        //            Email = model.Email,
        //            EmailConfirmed = false,
        //            RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"), //_db.Roles.ToList()[0].ID,
        //            LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"), //_db.Languages.Where(l => l.isDefault).Select(l => l.Id).SingleOrDefault(),
        //            PhoneNumber = model.Mobile,
        //            PhoneNumberConfirmed = false,
        //            UserName = model.Email,
        //            FullName = model.FullName,
        //            TwoFactorEnabled = false,        //            Channel = model.Channel.ToString()
        //        };
        //        var result = await _authorizationService.CreateUser(user, model.Password);
        //        if (result.Any())
        //        {
        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.CanNotCreate; ;
        //            Output.ErrorDescription = WebResources.ResourceManager.GetString("CAN_NOT_CREATE_USER", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)Output.ErrorCode;
        //            log.ErrorDescription = "can not create user due to " + string.Join(",", result);
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //            return Single(Output);
        //        }
        //        var RegisteredUser = UserManager.FindByEmailAsync(user.Email).Result;
        //        Output.Result.UserId = RegisteredUser.Id;
        //        Output.Result.PhoneNumber = RegisteredUser.PhoneNumber;
        //        // Send Confirmation Email
        //        string exception = string.Empty;
        //        var emailSent = SendConfirmationEmail(user.Email, RegisteredUser.Id, model.Channel, out exception, model.Language);        //        if (!emailSent)        //        {        //            Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmailNotSent;        //            Output.ErrorDescription = CheckoutResources.ErrorGeneric;        //            log.ErrorCode = (int)Output.ErrorCode;        //            log.ErrorDescription = "Can Not Send Email due to " + exception;
        //            RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);        //            return Single(Output);        //        }
        //        Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
        //        Output.ErrorDescription = LangText.CheckYourEmail;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = "Success";
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Single(Output);
        //    }
        //    catch (Exception ex)
        //    {
        //        Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
        //        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        log.ErrorCode = (int)Output.ErrorCode;
        //        log.ErrorDescription = ex.ToString();
        //        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
        //        return Single(Output);
        //    }

        //}

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/identity/login")]
        //[AllowAnonymous]
        //public async Task<IHttpActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    Output<LoginOutput> output = new Output<LoginOutput>();
        //    LoginRequestsLog log = new LoginRequestsLog();
        //    log.Email = model.Email;
        //    log.Channel = model.Channel.ToString();
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    //loginLog.Password = model.Password;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.Email))
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("EmailIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Email is empty";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }
        //        if (string.IsNullOrEmpty(model.Password))
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("PasswordIsEmpty", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "password is empty";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }

        //        // TODO add remember me
        //        var user = await UserManager.FindByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_email_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "user is null";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }
        //        log.UserID = user.Id;
        //        log.Mobile = user.PhoneNumber;
        //        if (!user.PhoneNumberConfirmed)
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Phone number is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }
        //        if (!user.EmailConfirmed)
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmailIsNotConfirmed;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("email_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "email is not Confirmed";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }
        //        if (user.LockoutEndDateUtc > DateTime.UtcNow)
        //        {
        //            output.ErrorCode = Output<LoginOutput>.ErrorCodes.AccountLocked;
        //            output.ErrorDescription = WebResources.ResourceManager.GetString("AccountLocked", CultureInfo.GetCultureInfo(model.Language)); ;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = "Account is Locked";
        //            LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //            return Single(output);
        //        }
        //        var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, shouldLockout: false);
        //        switch (result)
        //        {
        //            case SignInStatus.Success:
        //                {
        //                    MigrateUser(user.Id);
        //                    output.Result = new LoginOutput { UserId = user.Id };
        //                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.Success;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Success";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return Single(output);
        //                }
        //            //case SignInStatus.LockedOut:
        //            //    return View("Lockout");
        //            case SignInStatus.RequiresVerification:
        //                {
        //                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_phonenumber_not_verifed", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "Phone number is not Confirmed";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return Single(output);
        //                }
        //            case SignInStatus.Failure:
        //                {
        //                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "SignInStatus.Failure";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return Single(output);
        //                }
        //            default:
        //                {
        //                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.NotAuthorized;
        //                    output.ErrorDescription = WebResources.ResourceManager.GetString("login_incorrect_password_message", CultureInfo.GetCultureInfo(model.Language)); ;
        //                    log.ErrorCode = (int)output.ErrorCode;
        //                    log.ErrorDescription = "incorrect username and password";
        //                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //                    return Single(output);
        //                }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = exp.ToString();
        //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
        //        return Single(output);
        //    }

        //}

        [HttpPost]        [Route("api/identity/loginUser")]        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginViewModel model, string returnUrl = null)
        {
            Output<LoginOutput> output = new Output<LoginOutput>();
            try
            {
                //if (model.Channel != Channel.ios && model.Channel != Channel.android)
                //{
                    var encryptedEmail = Convert.FromBase64String(model.UserName.Trim());
                    var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                    model.Email = plainEmail;

                    var encryptedPassword = Convert.FromBase64String(model.PWD.Trim());
                    var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
                    model.Password = plainPassword;
                //}

                var outputResult = await _authenticationContext.Login(model, returnUrl);
                return Single(outputResult);
            }
            catch (Exception exp)
            {
                output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = exp.ToString();
                return Single(output);
            }
        }
        
        [HttpPost]
        [Route("api/identity/GetAccessToken")]
        [AllowAnonymous]
        //[TameenkAuthorizeAttribute]
        public IActionResult GetAccessToken([FromBody] UserData UserId)
        {
            Output<Tameenk.Core.Domain.Dtos.AccessTokenResult> output = new Output<Tameenk.Core.Domain.Dtos.AccessTokenResult>();
            loginLog = new LoginRequestsLog();
            loginLog.UserID = UserId.UserId;
            loginLog.Channel = UserId.Channel.ToString();
            output.Result = new Tameenk.Core.Domain.Dtos.AccessTokenResult();
            if (string.IsNullOrEmpty(UserId.UserId))
            {
                output.ErrorCode = Output<Tameenk.Core.Domain.Dtos.AccessTokenResult>.ErrorCodes.EmptyInputParamter;
                output.ErrorDescription = WebResources.ResourceManager.GetString("UserIdNotExist", CultureInfo.GetCultureInfo(UserId.Language));
                return Single(output);
                //return OutputHandler<Tameenk.Core.Domain.Dtos.AccessTokenResult>(output, loginLog, Output<Tameenk.Core.Domain.Dtos.AccessTokenResult>.ErrorCodes.EmptyInputParamter, "UserIdNotExist", UserId.Language);
            }
            try
            {
                output.Result = new Tameenk.Core.Domain.Dtos.AccessTokenResult();
                output.Result = _authorizationService.GetAccessToken(UserId?.UserId);
                output.ErrorCode = Output<Tameenk.Core.Domain.Dtos.AccessTokenResult>.ErrorCodes.Success;
                return Single(output);
            }
            catch (Exception ex)
            {
                var logId = DateTime.Now.GetTimestamp();
                _logger.Log($"AccountController-> GetAccessToken [key={logId}]", ex);
                output.ErrorCode = Output<Tameenk.Core.Domain.Dtos.AccessTokenResult>.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                return Single(output);
            }
        }


        

        /// <summary>
        /// Forget Password
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/identity/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] Tameenk.Services.Profile.Component.ForgetPasswordModel model)
        {
            var result = await _authenticationContext.ForgetPassword(model);
            return Single(result);
        }

        /// <summary>
        /// Forget Password
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/identity/ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] Tameenk.Services.Profile.Component.ConfirmResetPasswordModel model)
        {
            var result = await _authenticationContext.ConfirmResetPassword(model);
            return Single(result);
        }

        [Route("api/identity/Logout")]
        public async Task<IActionResult> Logout()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var UserSessionId = identity.FindFirst("UserSessionId")?.Value;
                if (!string.IsNullOrEmpty(UserSessionId))
                {
                    var userId = identity.FindFirst("curent_user_id")?.Value;
                    ExpiredTokens expiredToken = new ExpiredTokens();
                    expiredToken.UserId = userId;
                    expiredToken.CreatedDate = DateTime.Now;
                    IEnumerable<Claim> claims = identity.Claims;
                    expiredToken.Key = UserSessionId;
                    _expiredTokensRepository.Insert(expiredToken);

                    var userIp = Utilities.GetUserIPAddress();
                    var userAgent = Utilities.GetUserAgent();
                    var session = _loginActiveTokensRepository.Table.Where(a => a.UserId == userId && a.UserIP == userIp && a.Headers["User-Agent"].ToString() == userAgent).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (session != null)
                    {
                        session.IsValid = false;
                        session.ExpiredDate = DateTime.Now.AddMinutes(-30);
                        _loginActiveTokensRepository.Update(session);
                    }
                }
            }
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            Authentication.SignOut(OAuthDefaults.AuthenticationType);
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Request.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
            return Single(true);
        }
        private bool SendConfirmationEmail(string userEmail, string userId, Channel channel, out string exception, string lang = "ar")        {
            exception = string.Empty;
            try            {                Models.ConfirmationEmailModel model = new Models.ConfirmationEmailModel()                {                    UserId = userId,                    RequestedDate = DateTime.Now,                    Email = userEmail                };                var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), Send_Confirmation_Email_SHARED_KEY);                var emailSubject = LangText.BcareConfirmationEmail;                string url = string.Empty;
                string emailBody = string.Empty;                if (channel == Channel.android)                {                    url = $"{Utilities.SiteURL}/api/identity/EmailConfirmation?token={HttpUtility.UrlEncode(token)}";                }                else if (channel == Channel.ios)                {                    url = $"mailconfirmationscheme://bcare.com/mailconfirmation?token={HttpUtility.UrlEncode(token)}";
                    emailBody = WebResources.ConfirmationEmailAfterVerificationCode + "<br/><h2>" + url + "</h2><br/><h2>OR</h2><br/><h2>" +                        $"{Utilities.SiteURL}/api/identity/EmailConfirmation?token={HttpUtility.UrlEncode(token)}" + "</h2>";                }                else                {                    url = $"{Utilities.SiteURL}/Account/EmailConfirmation?token={HttpUtility.UrlEncode(token)}";                }                if (string.IsNullOrEmpty(emailBody))                    emailBody = WebResources.ConfirmationEmailAfterVerificationCode + "<br/><h2>" + url + "</h2>";                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/Welcome.png";
                messageBodyModel.Language = lang;
                messageBodyModel.MessageBody = emailBody;
                return MailUtilities.SendMailOfRegistration(messageBodyModel, emailSubject, "validation@bcare.com.sa", userEmail);
                //return MailUtilities.SendMailOfRegistration(emailBody, emailSubject, "validation@bcare.com.sa", userEmail);
            }            catch (Exception ex)            {
                exception = ex.ToString();                return false;            }        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Sign In user and if sucess then redirect to returnUrl
        /// </summary>
        /// <param name="email">User Email</param>
        /// <param name="model">Login View Model</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //private async Task<IHttpActionResult> SingInUser(string email, LoginViewModel model/*, string returnUrl*/)
        //{
        //    Output<LoginOutput> output = new Output<LoginOutput>();
        //    loginLog = new LoginRequestsLog();
        //    loginLog.Email = model.Email;
        //    loginLog.Channel = model.Channel.ToString();
        //    loginLog.Password = model.Password;

        //    Tameenk.Core.Domain.Entities.AspNetUser user = await UserManager.FindByEmailAsync(email);
        //    var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            {
        //                MigrateUser(user.Id);
        //                output.Result = new LoginOutput { UserId = user.Id };
        //                output.ErrorCode = Output<LoginOutput>.ErrorCodes.Success;
        //                return Single(output);
        //            }
        //        //case SignInStatus.LockedOut:
        //        //    return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            var code = Output<LoginOutput>.ErrorCodes.LoginIncorrectPhoneNumberNotVerifed;
        //            return OutputHandler<LoginOutput>(output, (object)loginLog, code, "login_incorrect_phonenumber_not_verifed", model.Language);

        //        case SignInStatus.Failure:
        //        default:
        //            var defaultcode = Output<LoginOutput>.ErrorCodes.NotAuthorized;
        //            return OutputHandler<LoginOutput>(output, (object)loginLog, defaultcode, "login_incorrect_password_message", model.Language);
        //    }
        //}
        private void MigrateUser(string userId)
        {
            //var anonymousId = HttpContext.Current.Request.AnonymousID;
            var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
            if (string.IsNullOrWhiteSpace(anonymousId)) return;

            if (string.IsNullOrWhiteSpace(userId)) return;

            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
            //AnonymousIdentificationModule.ClearAnonymousIdentifier();

        }
        //
        // GET: /Account/EmailConfirmation
        [AllowAnonymous]        [HttpGet]        [Route("api/identity/EmailConfirmation")]        public IActionResult EmailConfirmation(string token)        {            Output<LoginOutput> output = new Output<LoginOutput>();            output.Result = new LoginOutput();            if (string.IsNullOrEmpty(token))            {                output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;                output.ErrorDescription = CheckoutResources.ErrorSecurity;                return Single(output);            }            try            {                var decryptedToken = AESEncryption.DecryptString(token, Send_Confirmation_Email_SHARED_KEY);                var model = JsonConvert.DeserializeObject<Models.ConfirmationEmailModel>(decryptedToken);                if (model == null)                {                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.ServiceException;                    output.ErrorDescription = CheckoutResources.ErrorHashing;                    return Single(output);                }                var user = _authorizationService.GetUserDBByID(model.UserId);                if (user == null)                {                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.NotFound;                    output.ErrorDescription = CheckoutResources.ErrorGeneric;                    return Single(output);                }                var result = _authorizationService.ConfirmUserEmailDB(model.UserId);                if (result != 1)                {                    output.ErrorCode = Output<LoginOutput>.ErrorCodes.NotSuccess;                    output.ErrorDescription = CheckoutResources.ErrorGeneric;                    return Single(output);                }
                //SignInManager.SignInAsync(user, true, true);
                MigrateUser(user.Id);                output.ErrorCode = Output<LoginOutput>.ErrorCodes.Success;                output.ErrorDescription = "Success";                output.Result.UserId = user.Id;                output.Result.Email = user.Email;                return Single(output);            }            catch (Exception ex)            {                output.ErrorCode = Output<LoginOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = "tokeen coming is " + token + " =====> exp is " + ex.ToString();                return Single(output);            }        }
        #endregion

        public class UserData : BaseViewModel
        {
            public string UserId { get; set; }
        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        //[HttpPost]
        //[Route("api/identity/beginregister")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        //[AllowAnonymous]
        //public async Task<IHttpActionResult> BeginRegister([FromBody] Tameenk.Services.Profile.Component.RegisterationModel model)
        //{
        //    Output<RegisterOutput> output = new Output<RegisterOutput>();
        //    try
        //    {
        //        var outputResult = await _authenticationContext.BeginRegister(model);
        //        return Single(outputResult);
        //    }
        //    catch
        //    {
        //        output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
        //        return Single(output);
        //    }
        //}
        //[HttpPost]
        //[Route("api/identity/endregister")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        //[AllowAnonymous]
        //public async Task<IHttpActionResult> EndRegister([FromBody] Tameenk.Services.Profile.Component.RegisterationModel model)
        //{
        //    Output<RegisterOutput> output = new Output<RegisterOutput>();
        //    try
        //    {
        //        var outputResult = await _authenticationContext.EndRegister(model);
        //        return Single(outputResult);
        //    }
        //    catch
        //    {
        //        output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
        //        return Single(output);
        //    }
        //}
        //[HttpPost]        //[Route("api/identity/BeginLogin")]        //[AllowAnonymous]
        //public async Task<IHttpActionResult> BeginLogin(LoginViewModel model, string returnUrl = null)
        //{
        //    Output<LoginOutput> output = new Output<LoginOutput>();
        //    try
        //    {
        //        var outputResult = await _authenticationContext.BeginLogin(model, returnUrl);
        //        return Single(outputResult);
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
        //        return Single(output);
        //    }
        //}
        //[HttpPost]        //[Route("api/identity/VerifyYakeenMobile")]        //[AllowAnonymous]
        //public async Task<IHttpActionResult> VerifyYakeenMobile(LoginViewModel model)
        //{
        //    Output<LoginOutput> output = new Output<LoginOutput>();
        //    try
        //    {
        //        var outputResult = await _authenticationContext.VerifyYakeenMobile(model);
        //        return Single(outputResult);
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
        //        return Single(output);
        //    }
        //}
        //[HttpPost]        //[Route("api/identity/EndLogin")]        //[AllowAnonymous]
        //public async Task<IHttpActionResult> EndLogin(LoginViewModel model, string returnUrl = null)
        //{
        //    Output<LoginOutput> output = new Output<LoginOutput>();
        //    try
        //    {
        //        var outputResult = await _authenticationContext.EndLogin(model, returnUrl);
        //        return Single(outputResult);
        //    }
        //    catch 
        //    {
        //        output.ErrorCode = Output<LoginOutput>.ErrorCodes.EmptyInputParamter;
        //        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language));
        //        return Single(output);
        //    }
        //}

        //[HttpPost]        //[Route("api/identity/ResendOTP")]        //[AllowAnonymous]
        //public async Task<IHttpActionResult> ResendOTP(LoginViewModel model)
        //{
          
        //    try
        //    {
        //        var isSentOTP =  _authenticationContext.ReSendOTPCode(model);
        //        return Single(isSentOTP);
        //    }
        //    catch 
        //    {
        //        return Single(WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Language)));
        //    }
        //}

        #region New (Register / login) logic

        [HttpPost]
        [Route("api/identity/beginregister")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> BeginRegister([FromBody] ProfileComponent.RegisterationModel model)
        {
            var data = await _authenticationContext.BeginRegister(model);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/endregister")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> EndRegister([FromBody] ProfileComponent.RegisterationModel model)
        {
            var data = await _authenticationContext.EndRegister(model);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/verifyRegisterOTP")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyRegisterOTP(ProfileComponent.RegisterationModel model)
        {
            var data = await _authenticationContext.VerifyRegisterOTP(model);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/beginLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> BeginLogin(LoginViewModel model, string returnUrl = null)
        {
            var data = await _authenticationContext.BeginLogin(model, returnUrl);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/verifyYakeenMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyYakeenMobile(LoginViewModel model)
        {
            var data = await _authenticationContext.VerifyYakeenMobile(model);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/endLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> EndLogin(LoginViewModel model, string returnUrl = null)
        {
            var data = await _authenticationContext.EndLogin(model, returnUrl);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/verifyLoginOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyLoginOTP(LoginViewModel model)
        {
            var data = await _authenticationContext.VerifyLoginOTP(model);
            return Single(data);
        }

        [HttpPost]
        [Route("api/identity/resendOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOTP(ProfileComponent.ResendOTPModel model)
        {
            var data = _authenticationContext.ReSendOTPCode(model);
            return Single(data);
        }

        #endregion

        #region New Forgot Password Logic

        [AllowAnonymous]
        [HttpPost]
        [Route("api/identity/beginForgetPassword")]
        public async Task<IActionResult> BeginForgetPassword([FromBody] Tameenk.Services.Profile.Component.ForgotPasswordRequestViewModel model)
        {
            var result = await _authenticationContext.BeginForgetPassword(model);
            return Single(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/identity/endForgetPassword")]
        public async Task<IActionResult> endForgetPassword([FromBody] Tameenk.Services.Profile.Component.ForgotPasswordRequestViewModel model)
        {
            var result = await _authenticationContext.EndForgetPassword(model);
            return Single(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/identity/verifyForgetPasswordOTP")]
        public async Task<IActionResult> VerifyForgetPasswordOTP([FromBody] Tameenk.Services.Profile.Component.VerifyForgotPasswordOTPRequestModel model)
        {
            var result = await _authenticationContext.VerifyForgetPasswordOTP(model);
            return Single(result);
        }

        #endregion
    }

}