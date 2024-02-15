using Microsoft.AspNet.Identity.Owin;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
//using Tameenk.Loggin.DAL.Entities;
using Tameenk.Resources;
using Tameenk.Resources.Account;
using Tameenk.Resources.Vehicles;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Extensions;
using Tameenk.Services.IdentityApi.App_Start;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.IdentityApi.Output;
using Tameenk.Services.Implementation;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.Services.IdentityApi.Controllers
{
    [AllowAnonymous]
    [Authorize]
    public class VerficationController : IdentityBaseController
    {

        #region Fields
        private readonly IAuthorizationService _authorizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly TameenkConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly INotificationService notificationService;
        private readonly IOrderService orderService;
        private LoginRequestsLog loginLog;
        private ProfileRequestsLog profileLog;
        private RegistrationRequestsLog registrationLog;
        private readonly Profile.Component.ICorporateContext _corporateContext;
        #endregion

        #region Ctor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="authorizationService">The authorization service.</param>
        /// <param name="shoppingCartService">The authorization service.</param>
        public VerficationController(IAuthorizationService authorizationService, 
            IShoppingCartService shoppingCartService, TameenkConfig tameenkConfig,
            IHttpClient httpClient, ILogger logger,
            INotificationService notificationService,
            IOrderService orderService,
            Profile.Component.ICorporateContext corporateContext)
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _config = tameenkConfig ?? throw new ArgumentNullException(nameof(tameenkConfig));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.notificationService = notificationService;
            this.orderService = orderService;
            _corporateContext = corporateContext;
        }

        #endregion

        #region Methods


        /// <summary>
        /// ResendVerficationCode 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/resendverficationcode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> ResendVerficationCode([FromBody]ResendVerifyCodeModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "ResendVerficationCode";
            log.Email = model.Email;
            log.Mobile = model.PhoneNumber;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Mobile is empty";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);
            }
            if (string.IsNullOrEmpty(model.UserId))
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "UserId is empty";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);
            }
            try
            {
                // send sms 
                if (!await _authorizationService.SendTwoFactorCodeSmsAsync(model.UserId, model.PhoneNumber))
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Failed to Send SMS code";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }

                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);
            }

        }

        /// <summary>
        /// VerifyCode 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/verifycode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> VerifyCode([FromBody]VerifyCodeModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.Method = "VerifyCode";
            log.Mobile = model.PhoneNumber;
            log.Code = model.Code;
            log.Channel = model.Channel.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            try
            {
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Mobile is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
                if (string.IsNullOrEmpty(model.UserId))
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "UserId is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
                if (string.IsNullOrEmpty(model.Code))
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter; ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "code is empty";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }

                // The following code protects for brute force attacks against the two factor codes. 
                // If a user enters incorrect codes for a specified amount of time then the user account 
                // will be locked out for a specified amount of time. 
                // You can configure the account lockout settings in IdentityConfig
                if (_authorizationService.ChangePhoneNumber(model.UserId, model.PhoneNumber, model.Code, out string errors))
                {
                    var user = await _authorizationService.GetUser(model.UserId);
                    if (user == null)
                    {
                        Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.NotFound;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "user is null";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Single(Output);
                    }

                    var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                    if (accessTokenResult == null)
                    {
                        Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.NullResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "accessTokenResult is null";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Single(Output);
                    }

                    if (string.IsNullOrEmpty(accessTokenResult.access_token))
                    {
                        Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.EmptyResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "accessTokenResult.access_token is null";
                        RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                        return Single(Output);
                    }

                    Output.Result.AccessToken = accessTokenResult.access_token;
                    Output.Result.Email = user.Email;
                    Output.Result.UserId = user.Id;
                    MigrateUser(user.Id);

                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                    Output.ErrorDescription = "Success";
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Success";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
                else
                {
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.VerificationFaield ;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("VerifyCode_Failed", CultureInfo.GetCultureInfo(model.Language)); ;
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Verification Faield";
                    RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                    return Single(Output);
                }
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError; ;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Language)); ;
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                RegistrationRequestsLogDataAccess.AddRegistrationRequestsLog(log);
                return Single(Output);
            }
        }


        /// <summary>
        /// ResendVerficationCode 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/resendVerficationCodeForCheckout")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> CheckoutResendVerficationCode([FromBody]ResendVerifyCodeModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            registrationLog = new RegistrationRequestsLog();
            registrationLog.UserID = new Guid(model.UserId);
            registrationLog.Method = "ResendVerficationCode";
            registrationLog.Mobile = model.PhoneNumber;
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                var code = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                return OutputHandler<RegisterOutput>(Output, registrationLog, code, "MobileNotExist", model.Language);
            }
            if (string.IsNullOrEmpty(model.UserId))
            {
                var code = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                return OutputHandler<RegisterOutput>(Output, registrationLog, code, "UserIdNotExist", model.Language);
            }
            try
            {
                // send sms 
                Random rnd = new Random();
                int verifyCode = rnd.Next(1000, 9999);
                string msg = string.Format(LangText.ResourceManager.GetString("VerificationCodeMessage", CultureInfo.GetCultureInfo(model.Language)), verifyCode);
                string exception = string.Empty;
                var smsModel = new SMSModel()
                {
                    PhoneNumber = model.PhoneNumber,
                    MessageBody = msg,
                    Method = SMSMethod.ResendOTP.ToString(),
                    Module = Common.Utilities.Module.Vehicle.ToString()
                };
                notificationService.SendSmsBySMSProviderSettings(smsModel);
                orderService.CreateCheckoutUser(new CheckoutUsers()
                {
                    UserId = Guid.Parse(model.UserId),
                    PhoneNumber = model.PhoneNumber,
                    VerificationCode = verifyCode,
                    UserEmail = model.Email,
                    ReferenceId = model.ReferenceId,
                    CreatedDate = DateTime.Now,
                    Nin = model.Nin
                });

                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                return Single(Output);
            }
            catch (Exception ex)
            {
                var code = Output<RegisterOutput>.ErrorCodes.ExceptionError;
                return OutputHandler<RegisterOutput>(Output, registrationLog, code, "UserIdNotExist", model.Language, ex.Message);
            }
        }


        /// <summary>
        /// VerifyCode 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/verifyCodeForCheckout")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> VerifyCodeForCheckout([FromBody]VerifyCodeModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            registrationLog = new RegistrationRequestsLog();
            registrationLog.UserID = new Guid(model.UserId);
            registrationLog.Method = "VerifyCode";
            registrationLog.Code = model.Code;
            registrationLog.Mobile = model.PhoneNumber;
            try
            {
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    var code = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    return OutputHandler<RegisterOutput>(Output, registrationLog, code, "MobileNotExist", model.Language);
                }
                if (string.IsNullOrEmpty(model.UserId))
                {
                    var code = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    return OutputHandler<RegisterOutput>(Output, registrationLog, code, "UserIdNotExist", model.Language);
                }
                if (string.IsNullOrEmpty(model.Code))
                {
                    var code = Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    return OutputHandler<RegisterOutput>(Output, registrationLog, code, "UserIdEmptyInputParameter", model.Language);
                }

                var checkoutUser = orderService.GetByUserIdAndPhoneNumber(Guid.Parse(model.UserId), model.PhoneNumber);
                if (checkoutUser.VerificationCode == int.Parse(model.Code))
                {
                    checkoutUser.IsCodeVerified = true;
                    orderService.UpdateCheckoutUser(checkoutUser);
                    Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                    Output.ErrorDescription = "Success";
                    return Single(Output);
                }
                else
                {
                    return OutputHandler<RegisterOutput>(Output, registrationLog, Output<RegisterOutput>.ErrorCodes.VerificationFaield, "VerificationFailed", model.Language);
                }
            }
            catch (Exception e)
            {
                return OutputHandler<RegisterOutput>(Output, registrationLog, Output<RegisterOutput>.ErrorCodes.VerificationFaield, "VerificationFailed", model.Language, e.Message);
            }

        }


        /// <summary>
        /// VerifyCode 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/VerifyCorporateOTP")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        public async Task<IHttpActionResult> VerifyCorporateOTP([FromBody]VerifyCorporateCodeModel model)
        {
            var output = await _corporateContext.VerifyCorporateOneTimePassword(model.Email, model.OTP, model.Channel, model.Language = "ar");
            return Single(output);
        }
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

        #endregion
    }

}