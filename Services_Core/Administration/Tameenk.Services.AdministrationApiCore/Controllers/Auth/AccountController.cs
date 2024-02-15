using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Encryption;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Core.ViewModels;
using Tameenk.Services.Administration.Identity.Repositories;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{

    [RoutePrefix("api/account")]
    public class AccountController : ControllerBase
    {
        private const string LocalLoginProvider = "Local";
        private readonly IdentityDbContext<AppUser, RoleEntity, int, UserLogin, UserRole, UserClaim> context;
        private readonly AdminIdentityContext identityDbContext;
        private readonly IUserService userService;
        private readonly IExpiredTokensService _expiredTokensService;
        private ApplicationUserManager _userManager;
        private static readonly Regex EmailRegex = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string SHARED_Key = "bC@rEsEt_2020_PaSsWoRd_0123456789";
        private readonly INotificationService _notificationService;

        private readonly IUserLoginsConfirmationService _userLoginsConfirmation;
        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";


        // private  UserManager<AppUser,int> _userManager;
        public AccountController(IUserService userService, INotificationService notificationService, IExpiredTokensService expiredTokensService, IUserLoginsConfirmationService userLoginsConfirmation)
        {
            this.identityDbContext = new AdminIdentityContext();
            this.userService = userService;
            AppUser appUser = new AppUser();
            // UserManager.UserValidator = new CustomUserValidator<AppUser>(UserManager);
            _notificationService = notificationService;
            _expiredTokensService = expiredTokensService;
            _userLoginsConfirmation = userLoginsConfirmation;
        }

        //public AccountController(ApplicationUserManager userManager,
        //    ISecureDataFormat<AuthenticationTicket> accessTokenFormat,
        //    IdentityDbContext<AppUser, Role, int, UserLogin, UserRole, UserClaim> context,
        //    AdminIdentityContext identityDbContext
        //  )
        //{
        //    // UserManager = userManager;
        //    //  UserManager = _userManager; // _userManager = new UserManager<AppUser,int>(new UserStore<AppUser,Role,int,UserLogin,UserRole,UserClaim>(context));
        //    // AccessTokenFormat = accessTokenFormat;

        //    //  this.identityDbContext = new AdminIdentityContext();
        //    //this.context = context;

        //}

        public ApplicationUserManager UserManager
        {
            get
            {
                //return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager ?? new ApplicationUserManager(new UserStore<AppUser, RoleEntity, int, UserLogin, UserRole, UserClaim>(identityDbContext)); //new UserManager<AppUser, int>(new UserStore<AppUser, Role, int, UserLogin, UserRole, UserClaim>(context));
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        [AdminAuthorizeAttribute(pageNumber: 10000)]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                ExpiredTokens expiredToken = new ExpiredTokens();
                expiredToken.UserName = User.Identity.GetUserName();
                expiredToken.UserId = User.Identity.GetUserId();
                expiredToken.CreatedDate = DateTime.Now;
                IEnumerable<Claim> claims = identity.Claims;
                expiredToken.Token = identity.FindFirst("key").Value;
                _expiredTokensService.Add(expiredToken);
                _expiredTokensService.SaveChanges();
            }

            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            Authentication.SignOut(OAuthDefaults.AuthenticationType);
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Request.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
            return Ok(true);
        }


        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var user = userService.Get(userId);
            var result = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.OldPassword);
            if (result.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("Invalid Password", "The old password you have entered is incorrect");
                return BadRequest(ModelState);
            }
            user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);

            //IdentityResult result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            if (user != null)
            {
                user.ChangePasswordAfterLogin = false;
                userService.Update(user);
                userService.SaveChangesAsync();
            }
            return Ok();
        }

        [Route("ResetPassword")]
        [AdminAuthorizeAttribute(pageNumber: 10000)]
        public async Task<IActionResult> ResetPassword(ResetPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = model.UserId;
                var user = userService.Get(userId);
                if (user == null)
                {
                    ModelState.AddModelError("Invalid User", "This user is not exist");
                    return BadRequest(ModelState);
                }

                var newPassword = GeneratePomlexPassword(10);
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(newPassword);
                if (user != null)
                {
                    user.ChangePasswordAfterLogin = true;
                    userService.Update(user);
                    userService.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = user.PhoneNumber,
                            MessageBody = newPassword,
                            Method = SMSMethod.Resetpassword.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = Channel.Dashboard.ToString()
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        return Ok("password sent to your phone");
                    }
                    else
                    {
                        var emailSubject = "Change password";
                        string emailBody = "<p> <b> Dear User </b> ,</p>" +
                                           "<p> Kindly, find below your new password </p>" +
                                           "<p> <b>" + newPassword + "</b> </p>";
                        EmailModel emailModel = new EmailModel();
                        emailModel.To = new List<string>();
                        emailModel.To.Add(user.Email);
                        emailModel.Subject = emailSubject;
                        emailModel.EmailBody = emailBody;
                        emailModel.Module = "Vehicle";
                        emailModel.Method = "Dashboard";
                        emailModel.Channel = Channel.Dashboard.ToString();
                        var sendMail = _notificationService.SendEmail(emailModel);
                        if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                            return Ok("Failed to send mail due to: " + sendMail.ErrorDescription);
                        else
                            return Ok("password sent to your email");
                    }
                }
                else
                {
                    ModelState.AddModelError("Invalid User", "This user is not exist");
                    return BadRequest(ModelState);
                }
            }
            catch(Exception exp)
            {
                ModelState.AddModelError("exception","service exception");
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// this function to generate dynamic complex password
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected static string GeneratePomlexPassword(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                builder.Append(SHARED_Key[random.Next(0, SHARED_Key.Length)]);
            }

            return builder.ToString();
        }

        // POST api/Account/ChangeName
        [Route("ChangeName")]
        [AdminAuthorizeAttribute(pageNumber: 10000)]
        public async Task<IActionResult> ChangeName(ChangeNameBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId<int>();
            var user = userService.Get(userId);
            if (user != null)
            {
                user.Name = model.Name;
                userService.Update(user);
                var result = await userService.SaveChangesAsync();
                if (result > 0) return Ok();
            }
            return BadRequest();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        [AdminAuthorizeAttribute(pageNumber: 10000)]
        public async Task<IActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId<int>(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //// POST api/Account/AddExternalLogin
        //[Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.Now))
        //    {
        //        return BadRequest("External login failure.");
        //    }

        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId<int>(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId<int>());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId<int>(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        // POST api/Account/Register
        [Route("register")]
        [AdminAuthorizeAttribute(pageNumber: 10000)]
        public async Task<IActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!EmailRegex.IsMatch(model.Email))
            {
                ModelState.AddModelError("Invalid Email", "Enter a valid email address from ahmed hassan.");
                return BadRequest(ModelState);
            }
            if (UserManager.FindByEmail(model.Email) != null)
            {
                ModelState.AddModelError("Invalid Email", "Email is already exist.");
                return BadRequest(ModelState);
            }
            var user = new AppUser() { UserName = model.Email, Email = model.Email, Name = model.Name, CompanyId = model.CompanyId };
            try
            {
                // UserManager.UserValidator = new CustomUserValidator<AppUser>(UserManager);
                // IdentityResult result = UserManager.Create(user, model.Password);

                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);

                userService.Add(user);
                var result = userService.SaveChanges();
                if (result > 0)
                {
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("Server Error", "Something wrong");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {

            }


            return Ok();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// send verification code to the logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendVerificationCode")]
        public async Task<IActionResult> SendVerificationCodeToLoggedInUserAsync([FromBody]LoginConfirmationCodeViewModel user)
        {
            var outPut = new OtpOutputModel();
            LoginRequestsLog log = new LoginRequestsLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Channel = "DashBoard-OTP";
            try
            {
                if (user == null)
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "model is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }
                if (string.IsNullOrEmpty(user.UserName))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "UserName is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Password is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }
                if (string.IsNullOrEmpty(user.CaptchaToken))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Captcha is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }

                var encryptedCaptcha = AESEncryption.DecryptString(user.CaptchaToken, SHARED_SECRET);
                if (string.IsNullOrEmpty(encryptedCaptcha))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Captcha is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }

                var captchaToken = JsonConvert.DeserializeObject<CaptchaToken>(encryptedCaptcha);
                if (captchaToken == null || string.IsNullOrEmpty(captchaToken.Captcha))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Captcha is empty" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }
                if (captchaToken.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Captcha is expired" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }

                var keybytes = Encoding.UTF8.GetBytes("BcARe_2021_N0MeM");
                var iv = Encoding.UTF8.GetBytes("BcARe_2021_N0MeM");
                var encryptedCaptchaInput = Convert.FromBase64String(user.CaptchaInput.Trim());
                var plainCaptchaInput = SecurityUtilities.DecryptStringFromBytes_AES(encryptedCaptchaInput, keybytes, iv);
                if (!captchaToken.Captcha.Equals(plainCaptchaInput, StringComparison.Ordinal))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Wrong Input Captcha" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    return Ok(outPut);
                }
                
                var encryptedUserName = Convert.FromBase64String(user.UserName.Trim());
                var encryptedPassword = Convert.FromBase64String(user.Password.Trim());
                var plainUserName = SecurityUtilities.DecryptStringFromBytes_AES(encryptedUserName, keybytes, iv);
                var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, keybytes, iv);

                log.Email = plainUserName;
                AppUser userData = await UserManager.FindAsync(plainUserName, plainPassword);
                if (userData == null)
                {
                    outPut = new OtpOutputModel() { ErrorCode = 2, ErrorDescription = "Invalid user name or password" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return Ok(outPut);
                }
                if (string.IsNullOrEmpty(userData.PhoneNumber))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 3, ErrorDescription = "Phone number is null" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = outPut.ErrorDescription;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return Ok(outPut);
                }

                log.UserID = userData.Id.ToString();
                log.Mobile = userData.PhoneNumber;

                var userLoginsData = _userLoginsConfirmation.GetAllActiveCode(userData.Id);
                foreach (var login in userLoginsData)
                {
                     login.IsDeleted = true;
                    _userLoginsConfirmation.Update(login);
                    _userLoginsConfirmation.SaveChanges();
                }
                int verifyCode = GenerateRendomCode();
                var userPhoneConfirmationModel = new UserLoginsConfirmation()
                {
                    UserId = userData.Id,
                    ConfirmationCode = verifyCode,
                    IsCodeConfirmed = false,
                    CodeExpiryDate = DateTime.Now.AddSeconds(60),
                    IsDeleted = false
                };
                _userLoginsConfirmation.Add(userPhoneConfirmationModel);
                _userLoginsConfirmation.SaveChanges();

                string exception = string.Empty;
                bool isSmsSent = userService.SendVerificationCode(userData.PhoneNumber, verifyCode, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    outPut = new OtpOutputModel() { ErrorCode = 4, ErrorDescription = "The service is currently down, Please try again latere" };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = "Failed to send sms due to: " + exception;
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return Ok(outPut);
                }
                if (!isSmsSent)
                {
                    outPut = new OtpOutputModel() { ErrorCode = 4, ErrorDescription = "Failed to send sms vrefication code." };
                    log.ErrorCode = outPut.ErrorCode.Value;
                    log.ErrorDescription = "Failed to send sms vrefication code";
                    LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                    return Ok(outPut);
                }

                //else
                //{
                //    var emailSubject = "Dashboard Code";
                //    string emailBody = "<p> <b> Dear User </b> ,</p>" +
                //                       "<p> Kindly, find below your verification code </p>" +
                //                       "<p> <b>" + verifyCode + "</b> </p>";
                //    EmailModel emailModel = new EmailModel();
                //    emailModel.To = new List<string>();
                //    emailModel.To.Add(userData.Email);
                //    emailModel.Subject = emailSubject;
                //    emailModel.EmailBody = emailBody;
                //    emailModel.Module = "Vehicle";
                //    emailModel.Method = "Dashboard";
                //    emailModel.Channel = Channel.Dashboard.ToString();
                //    var sendMail = _notificationService.SendEmail(emailModel);
                //    if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                //    {
                //        outPut = new OtpOutputModel() { ErrorCode = 4, ErrorDescription = "The service is currently down, Please try again latere" };
                //        log.ErrorCode = outPut.ErrorCode.Value;
                //        log.ErrorDescription = "Failed to send mail due to: " + sendMail.ErrorDescription;
                //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                //        return Ok(outPut);
                //    }
                //    else
                //    {
                //        outPut = new OtpOutputModel() { ErrorCode = 1, ErrorDescription = $"Verification email was sent to this email: {userData.Email} .Please see your inbox." };
                //        log.ErrorCode = outPut.ErrorCode.Value;
                //        log.ErrorDescription = "Success";
                //        LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                //        return Ok(outPut);
                //    }
                //}
                outPut = new OtpOutputModel()
                {
                    ErrorCode = 1,
                    ErrorDescription = "Verification Code is Sent Successfully, Please check your mobile"
                };
                log.ErrorCode = outPut.ErrorCode.Value;
                log.ErrorDescription = outPut.ErrorDescription;
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                return Ok(outPut);
            }
            catch (Exception exp)
            {
                outPut = new OtpOutputModel() { ErrorCode = 5, ErrorDescription = "The service is currently down, Please try again later" };
                log.ErrorCode = outPut.ErrorCode.Value;
                log.ErrorDescription = exp.ToString();
                LoginRequestsLogDataAccess.AddLoginRequestsLog(log);
                return Ok(outPut);
            }
        }

        private int GenerateRendomCode()
        {
            Random rnd = new Random();
            int code = rnd.Next(100000, 999999);
            var userCode = _userLoginsConfirmation.CheckVerificationCodeExist(code);
            if (userCode != null)
                GenerateRendomCode();

            return code;
        }

        /// <summary>
        /// phone number verification model
        /// </summary>
        public class VerificationPhoneViewModel
        {
            /// <summary>
            /// verification code
            /// </summary>
            public string access_token { get; set; }

            /// <summary>
            /// code expiration date
            /// </summary>
            public DateTime expires_in { get; set; }
        }

        public class CaptchaToken
        {
            /// <summary>
            /// Captcha value.
            /// </summary>
            [JsonProperty("captcha")]
            public string Captcha { get; set; }

            /// <summary>
            /// Captcha expiration date.
            /// </summary>
            [JsonProperty("expiryDate")]
            public DateTime ExpiryDate { get; set; }
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
