//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Text;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Hosting;
//using System.Web.Mvc;
//using System.Web.Security;
//using Tameenk.App_Start;
//using Tameenk.Common.Utilities;
//using Tameenk.Core.Configuration;
//using Tameenk.Core.Data;
//using Tameenk.Core.Domain.Entities;
//using Tameenk.Core.Domain.Enums;
//using Tameenk.Core.Infrastructure;
//using Tameenk.Integration.Dto.Providers;
//using Tameenk.Loggin.DAL;
//using Tameenk.Models;
//using Tameenk.Resources.Checkout;
//using Tameenk.Resources.WebResources;
//using Tameenk.Security.Encryption;
//using Tameenk.Security.Services;
//using Tameenk.Services.Checkout.Components;
//using Tameenk.Services.Core;
//using Tameenk.Services.Core.Checkouts;
//using Tameenk.Services.Core.Notifications;
//using Tameenk.Services.Core.Policies;
//using Tameenk.Services.Implementation.Policies;
//using Tameenk.Services.Orders;
//using Tameenk.Services.Policy.Components;
//using Tameenk.Services.Profile.Component;
//using Tameenk.Services.Profile.Component.Output;
//using TameenkDAL;
//using System.Text;

//namespace Tameenk.Controllers
//{
//    [Authorize]
//    public class AccountController : BaseController
//    {
//        #region Declarations
//        private TameenkDbContext _db = new TameenkDbContext();
//        private Tameenk.Services.Profile.Component.Membership.ClientSignInManager _signInManager;
//        private Tameenk.Services.Profile.Component.Membership.UserManager _userManager;
//        private readonly INotificationService _notificationService;
//        private readonly IShoppingCartService _shoppingCartService;
//        private readonly IAuthorizationService _authorizationService;//        private const string Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";//        private readonly TameenkConfig _tameenkConfig;
//        private readonly ICheckoutContext _checkoutContext;//        private readonly IPolicyService _policyService;
//        private readonly IPolicyContext _policyContext;
//        private readonly IAuthenticationContext _authenticationContext;
//        private readonly IRepository<ExpiredTokens> _expiredCookiesRepository;
//        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";
//        private readonly IRepository<CorporateVerifyUsers> _corporateVerifyUsersRepository;
//        private readonly IRepository<CorporateUsers> _corporateUsersRepository;
//        private readonly ICorporateContext _corporateContext;

//        #endregion

//        public AccountController(
//            INotificationService notificationService, IShoppingCartService shoppingCartService, IAuthorizationService authorizationService, TameenkConfig tameenkConfig
//            , ICheckoutContext checkoutContext, IPolicyService policyService, IPolicyContext policyContext, IAuthenticationContext authenticationContext, IRepository<ExpiredTokens> expiredCookiesRepository
//            , IRepository<CorporateVerifyUsers> corporateVerifyUsersRepository, IRepository<CorporateUsers> corporateUsersRepository, ICorporateContext corporateContext)
//        {
//            _notificationService = notificationService;
//            _shoppingCartService = shoppingCartService;
//            _authorizationService = authorizationService;
//            _tameenkConfig = tameenkConfig;
//            _checkoutContext = checkoutContext;
//            _policyService = policyService;
//            _policyContext = policyContext;
//            _authenticationContext = authenticationContext;
//            _expiredCookiesRepository = expiredCookiesRepository;
//            _corporateVerifyUsersRepository = corporateVerifyUsersRepository;
//            _corporateUsersRepository = corporateUsersRepository;
//            _corporateContext = corporateContext;
//        }

//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }

//        public Tameenk.Services.Profile.Component.Membership.ClientSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<Tameenk.Services.Profile.Component.Membership.ClientSignInManager>();
//            }
//            private set
//            {
//                _signInManager = value;
//            }
//        }

//        public Tameenk.Services.Profile.Component.Membership.UserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<Tameenk.Services.Profile.Component.Membership.UserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        private string CurrentUserID
//        {
//            get
//            {
//                return User.Identity.GetUserId<string>();
//            }
//        }

//        //
//        // GET: /Account/Login
//        [AllowAnonymous]
//        public ActionResult Login(string returnUrl)
//        {
//            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
//            {
//                var cookie = Utilities.GetCookie("_authCookie");
//                if (string.IsNullOrEmpty(cookie))
//                {
//                    var user = _authorizationService.GetUserDBByID(User.Identity.GetUserId());
//                    if (user == null)
//                    {
//                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//                        Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//                        Utilities.RemoveCookie("_authCookie");
//                        return RedirectToAction("Index", "Home");
//                    }
//                    string exception = "";
//                    _authenticationContext.SetUserAuthenticationCookies(user, out exception);
//                }
//                return RedirectToLocal(returnUrl);

//            }
//            if (returnUrl == "/Account/LogOff")
//            {
//                returnUrl = "";
//            }
//            ViewBag.ReturnUrl = returnUrl;
//            ViewBag.loginModel = new LoginViewModel();
//            ViewBag.islogin = 1;
//            ViewBag.IsCorporateUser = 0;
//            ViewBag.registerModel = new RegisterViewModel();
//            return View("Auth");
//        }

//        //
//        // POST: /Account/Login
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
//        {
//            if (string.IsNullOrEmpty(model.UserName))
//            {
//                AddErrorsToModelStateErors(new Dictionary<string, string>() { { "Email", "مطلوب*" } });
//                return View(model);
//            }
//            if (string.IsNullOrEmpty(model.PWD))
//            {
//                AddErrorsToModelStateErors(new Dictionary<string, string>() { { "Password", "مطلوب*" } });
//                return View(model);
//            }

//            var encryptedEmail = Convert.FromBase64String(model.UserName.Trim());
//            var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
//            model.Email = plainEmail;

//            var encryptedPassword = Convert.FromBase64String(model.PWD.Trim());
//            var plainPassword = SecurityUtilities.DecryptStringFromBytes_AES(encryptedPassword, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
//            model.Password = plainPassword;

//            if (!model.IsValid)
//            {
//                AddErrorsToModelStateErors(model.ModelErrors);
//                return View(model);
//            }
//            Tameenk.Core.Domain.Dtos.LoginViewModel newModel = new Tameenk.Core.Domain.Dtos.LoginViewModel();
//            newModel.Email = model.Email;
//            newModel.Password = model.Password;
//            var result = await _authenticationContext.Login(newModel, returnUrl);
//            if (result.ErrorCode == ProfileOutput<LoginOutput>.ErrorCodes.EmptyInputParamter)
//            {
//                ModelState.AddModelError("", Tameenk.LangText.login_incorrect_email_message);
//                ViewBag.loginModel = model;
//                ViewBag.registerModel = new RegisterViewModel();
//                ViewBag.islogin = 1;
//                ViewBag.IsCorporateUser = 0;
//                return View("Auth");
//            }
//            if (result.ErrorCode == ProfileOutput<LoginOutput>.ErrorCodes.Success)
//            {
//                if (result.Result.IsCorporateUser)
//                {
//                    ViewBag.loginModel = new LoginViewModel()
//                    {
//                        Email = model.Email,
//                        Password = model.Password,
//                        Lang = newModel.Language
//                    };
//                    ViewBag.registerModel = new RegisterViewModel();
//                    ViewBag.islogin = 1;
//                    ViewBag.IsCorporateUser = 1;
//                    ViewBag.returnUrl = returnUrl;
//                    return View("Auth");
//                }

//                return RedirectToAction("ClearBrowserLocalStorage", new { returnUrl = returnUrl });
//            }
//            if (result.ErrorCode == ProfileOutput<LoginOutput>.ErrorCodes.Lockout)
//                return View("Lockout");
//            else
//            {
//                ModelState.AddModelError("", Tameenk.LangText.login_incorrect_password_message);
//                ViewBag.ReturnUrl = returnUrl;
//                ViewBag.loginModel = model;
//                ViewBag.islogin = 1;
//                ViewBag.registerModel = new RegisterViewModel();
//                ViewBag.IsCorporateUser = 0;
//                return View("Auth");
//            }

//        }

//        //
//        // GET: /Account/VerifyCode
//        [AllowAnonymous]
//        public async Task<ActionResult> VerifyCode(string userId, string PhoneNumber, string returnUrl, bool rememberMe, string email, bool isEdit)
//        {
//            // Check if editable phone number is registered with another user
//            if (isEdit)
//            {
//                var dbContext = EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>();
//                if (dbContext.Users.Any(u => u.PhoneNumber == PhoneNumber && u.Id != userId))
//                    ModelState.AddModelError("", LangText.Tameenk_VerifyCode_ResendError);

//                return View(new VerifyCodeViewModel { UserId = userId, PhoneNumber = PhoneNumber, ReturnUrl = returnUrl, RememberMe = rememberMe });
//            }

//            // Require that the user has already logged in via username/password or external login
//            if (!await SendTwoFactorCodeSmsAsync(userId, PhoneNumber, email))
//            {
//                return View("Error");
//            }

//            return View(new VerifyCodeViewModel { UserId = userId, PhoneNumber = PhoneNumber, ReturnUrl = returnUrl, RememberMe = rememberMe });
//        }

//        //
//        // POST: /Account/VerifyCode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
//        {
//            if (!model.IsValid)
//            {
//                AddErrorsToModelStateErors(model.ModelErrors);
//                return View(model);
//            }

//            // The following code protects for brute force attacks against the two factor codes. 
//            // If a user enters incorrect codes for a specified amount of time then the user account 
//            // will be locked out for a specified amount of time. 
//            // You can configure the account lockout settings in IdentityConfig
//            var result = UserManager.ChangePhoneNumber(model.UserId, model.PhoneNumber, model.Code);
//            if (result.Succeeded)
//            {
//                var user = await UserManager.FindByIdAsync(model.UserId);
//                if (user != null)
//                {
//                    await SignInManager.SignInAsync(user, model.RememberMe, model.RememberBrowser);
//                    string exception = string.Empty;
//                    _authenticationContext.SetUserAuthenticationCookies(user, out exception);
//                    MigrateUser(user.Id);
//                }
//                return RedirectToLocal(model.ReturnUrl);
//            }
//            else
//            {             // If we got this far, something failed, redisplay form
//                ModelState.AddModelError("", LangText.VerifyCode_Failed);
//                return View(model);
//            }
//        }

//        //
//        // GET: /Account/Register
//        [AllowAnonymous]
//        public ActionResult Register(string returnUrl)
//        {
//            ViewBag.ReturnUrl = returnUrl;
//            ViewBag.islogin = 0;
//            ViewBag.registerModel = new RegisterViewModel();
//            ViewBag.loginModel = new LoginViewModel();
//            return View("Auth");
//        }

//        public string ValidationInSignUp(string email, string mobile)
//        {
//            var user = UserManager.FindByEmailAsync(email).Result;
//            if (user != null)
//            {
//                if (user.EmailConfirmed && user.PhoneNumberConfirmed)
//                {
//                    return Tameenk.LangText.exist_email_signup_error;
//                }
//                else
//                {
//                    UserManager.Delete(user);
//                }
//            }

//            var dbContext = EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>();
//            if (dbContext.Users.Any(u => u.PhoneNumber == mobile))
//                return Tameenk.LangText.exist_phone_signup_error;
//            return null;
//        }
//        //
//        // POST: /Account/Register
//        //[HttpPost]
//        //[AllowAnonymous]
//        //[ValidateAntiForgeryToken]
//        //public async Task<ActionResult> Register(RegisterationModel model, string returnUrl)
//        //{
//        //    model.Channel = Channel.Portal;
//        //    var output = await _authenticationContext.Register(model);
//        //    if (output.ErrorCode != ProfileOutput<IdentityResult>.ErrorCodes.Success)
//        //    {
//        //        ModelState.AddModelError("", output.ErrorDescription);
//        //        ViewBag.islogin = 0;
//        //        ViewBag.loginModel = new LoginViewModel();
//        //        ViewBag.registerModel = model;
//        //        return View("Auth");
//        //    }
//        //    var userByEmail = await UserManager.FindByEmailAsync(model.Email);
//        //    if (userByEmail == null)
//        //    {
//        //        return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });
//        //    }
//        //    return RedirectToAction("ClearBrowserLocalStorage", new { returnUrl = returnUrl });

//        //}

//        //
//        // POST: /Account/LogOff

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult LogOff()
//        {

//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            try
//            {
//                var cookie = Utilities.GetCookie("_authCookie");
//                var userTicket = System.Web.Security.FormsAuthentication.Decrypt(cookie);
//                string[] values = userTicket.UserData.Split(';');
//                string keyValue = "";
//                foreach (var value in values)
//                {
//                    if (value.Contains("Key"))
//                    {
//                        keyValue = value.Substring(value.LastIndexOf("=") + 1);
//                        break;
//                    }
//                }
//                ExpiredTokens expiredCookie = new ExpiredTokens();
//                expiredCookie.UserId = User.Identity.GetUserId();
//                expiredCookie.CreatedDate = DateTime.Now;
//                expiredCookie.Key = keyValue;
//                _expiredCookiesRepository.Insert(expiredCookie);
//            }
//            catch
//            {

//            }
//            Utilities.RemoveCookie("_authCookie");
//            return RedirectToAction("Index", "Home");
//        }


//        [AllowAnonymous]
//        public async Task<ActionResult> ResetPassword(string email)
//        {
//            if (string.IsNullOrEmpty(email))
//                return Json(new { Success = false, Message = "Email is required." }, JsonRequestBehavior.AllowGet);

//            //check if the user email exist in db and if yes then send email to reset password else show error to user
//            var user = await UserManager.FindByEmailAsync(email);
//            if (user == null)
//            {
//                return Json(new { Success = false, Message = LangText.ResetPasswordUserEmailNotFound }, JsonRequestBehavior.AllowGet);
//            }

//            //1- send email with reset password
//            //2- tell user to check his mail
//            var lang = LanguageTwoLetterIsoCode.Ar;
//            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
//            //HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
//            //{
//            //    await SendResetPasswordEmail(user, lang, token);
//            //});
//            //});
//            bool sendingResult = SendResetPasswordEmailSync(user, lang, token);
//            if (sendingResult)
//            {
//                return Json(new { Success = true, Message = LangText.forget_password_email_sent }, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                return Json(new { Success = false, Message = LangText.GenericError }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        // [HttpGet]
//        [AllowAnonymous]
//        public ActionResult ChangePassword(string userId, string token)
//        {
//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            ViewBag.UserId = userId;
//            ViewBag.Token = token;
//            return View();
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<ActionResult> ChangePassword(string Password, string confirmNewPassword, string userId, string token)
//        {
//            // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

//            //validate that  password and confirm password are the same and if not equal redirect to error page
//            if (string.Equals(Password, confirmNewPassword))
//            {
//                //get the user by id and if not found redirect to error page
//                var user = await UserManager.FindByIdAsync(userId);
//                if (user != null)
//                {
//                    LoginViewModel model = new LoginViewModel() { Email = user.Email, Password = Password };
//                    var res = await UserManager.ResetPasswordAsync(userId, token, Password);
//                    if (res.Succeeded)
//                    {
//                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//                        // await SingInUser(user.Email, model, null, true)

//                        return RedirectToAction("Login", "Account");
//                    }
//                    else
//                    {
//                        return RedirectToAction("Index", "Error", new { Message = LangText.ResetPaswordTokenInvalid });
//                    }

//                }
//                return RedirectToAction("Index", "Error", new { Message = "There is no user with this id." });
//            }
//            else
//            {
//                ViewBag.UserId = userId;
//                ViewBag.Token = token;
//                ModelState.AddModelError("", Tameenk.LangText.password_confirm_error);
//                return View("");

//            }
//        }

//        public ActionResult ClearBrowserLocalStorage(string returnUrl)
//        {
//            returnUrl = returnUrl ?? "";
//            var model = new ClearBrowserLocalStorageModel { ReturnUrl = Server.UrlDecode(returnUrl.Replace("&amp;", "&").Replace("amp;", "&")) };
//            return View(model);
//        }

//        private ActionResult RedirectToLocal(string returnUrl)
//        {
//            if (Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }

//            return RedirectToAction("Index", "Home");
//        }

//        private void AddErrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }

//        public virtual async Task<bool> SendTwoFactorCodeSmsAsync(string userId, string phoneNumber, string email)
//        {
//            if (userId == null)
//            {
//                return false;
//            }

//            var token = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
//            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
//            await UserManager.NotifyTwoFactorTokenAsync(userId, LangText.SmsTwoFactorCodeProviderName, token);
//            if (Utilities.GetAppSetting("EnableSendCodeViaEmail").ToLower() == "true")
//            {

//                if (string.IsNullOrEmpty(email))
//                {
//                    var user = UserManager.FindById(userId);
//                    if (user != null)
//                    {
//                        email = user.Email;
//                    }
//                }
//                if (!string.IsNullOrEmpty(email))
//                {
//                    string mailFrom = Utilities.GetAppSetting("NoReplayAdminEmail");
//                    if (string.IsNullOrEmpty(mailFrom))
//                        mailFrom = "noreplay@bcare.com.sa";
//                    string body = "<h1>عزيزنا العميل، </h1><br /><br /><h2>شكرًا لاعطائنا الفرصة لخدمتكم رمز تحقق #تأمينك من بي كير هو</h2>";
//                    body += "<b>" + token + "</b>";
//                    MailUtilities.SendMailToUser(body, "رمز تحقق #تأمينك من بي كير", mailFrom, email);
//                }
//            }
//            return true;
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_userManager != null)
//                {
//                    _userManager.Dispose();
//                    _userManager = null;
//                }

//                if (_signInManager != null)
//                {
//                    _signInManager.Dispose();
//                    _signInManager = null;
//                }
//            }

//            base.Dispose(disposing);
//        }


//        #region Reset Password Helper Methods

//        private async System.Threading.Tasks.Task SendResetPasswordEmail(AspNetUser clientUser, LanguageTwoLetterIsoCode userLanguage, string token)
//        {
//            await _notificationService.SendEmailAsync(clientUser.Email, GetResetPasswordEmailSubject(userLanguage), GetResetPasswordEmailBody(userLanguage, clientUser, token), null, true);
//        }
//        private bool SendResetPasswordEmailSync(AspNetUser clientUser, LanguageTwoLetterIsoCode userLanguage, string token)
//        {
//            var emailSubject = GetResetPasswordEmailSubject(userLanguage);
//            string url = Url.Action("ChangePassword", "Account", new { userId = clientUser.Id, token }, protocol: Request.Url.Scheme);
//            string emailBody = string.Format(WebResources.EmailPasswordResetBody, url);
//            MessageBodyModel messageBodyModel = new MessageBodyModel();
//            messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/PasswordReset.png";
//            messageBodyModel.Language = CultureInfo.CurrentCulture.Name;
//            messageBodyModel.MessageBody = emailBody;
//            return MailUtilities.SendMail(messageBodyModel, emailSubject, "account1@bcare.com.sa", clientUser.Email);
//        }

//        private string GetResetPasswordEmailSubject(LanguageTwoLetterIsoCode userLanguage)
//        {
//            string emailSubject = userLanguage == LanguageTwoLetterIsoCode.Ar
//             ? "تغير كلمه السر"
//             : "Change password";
//            return emailSubject;
//        }
//        private string GetResetPasswordEmailBody(LanguageTwoLetterIsoCode userLanguage, AspNetUser user, string token)
//        {
//            string url = Url.Action("ChangePassword", "Account", new { userId = user.Id, token }, protocol: Request.Url.Scheme);
//            string emailBody = userLanguage == LanguageTwoLetterIsoCode.Ar
//                ? string.Format("<h1>عزيزنا العميل، </h1><br /><br /><h2> لتغير كلمه السر الخاصه بكم يرجي الضغط علي اللينك الاتي: </h2><br/><h2> {0} </h2>", url)
//          : string.Format("<h1>Dear Customer، </h1><br /><br /><h2>To change your password please click on the blow link : </h2><br /><h2> {0} </h2>", url);
//            return emailBody;
//        }


//        #endregion

//        public static string Base64Encode(string plainText)
//        {
//            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
//            return System.Convert.ToBase64String(plainTextBytes);
//        }
//        public static string Base64Decode(string base64EncodedData)
//        {
//            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
//            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
//        }

//        /// <summary>
//        /// Sign In user and if sucess then redirect to returnUrl
//        /// </summary>
//        /// <param name="email">User Email</param>
//        /// <param name="model">Login View Model</param>
//        /// <param name="returnUrl"></param>
//        /// <returns></returns>
//        private async Task<ActionResult> SingInUser(string email, LoginViewModel model, string returnUrl)
//        {
//            AspNetUser user = await UserManager.FindByEmailAsync(email);//            if (user != null && user.EmailConfirmed == false)//            {//                SendConfirmationEmailAfterPhoneVerificationCode(email, user.Id);//                ModelState.AddModelError("", Tameenk.LangText.EmailNotConfirmed + ", " + LangText.CheckYourEmail);//                ViewBag.ReturnUrl = returnUrl;//                ViewBag.loginModel = model;//                ViewBag.islogin = 1;//                ViewBag.registerModel = new RegisterViewModel();//                return View("Auth");//            }//            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, true, shouldLockout: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    {
//                        string exception = string.Empty;
//                        _authenticationContext.SetUserAuthenticationCookies(user, out exception);
//                        MigrateUser(user.Id);
//                        return RedirectToAction("ClearBrowserLocalStorage", new { returnUrl = returnUrl });
//                        //return RedirectToLocal(returnUrl);
//                    }
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", Tameenk.LangText.login_incorrect_password_message);
//                    ViewBag.ReturnUrl = returnUrl;
//                    ViewBag.loginModel = model;
//                    ViewBag.islogin = 1;
//                    ViewBag.registerModel = new RegisterViewModel();
//                    return View("Auth");
//            }
//        }
//        private void MigrateUser(string userId)
//        {
//            var anonymousId = Request.AnonymousID;
//            //var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
//            if (string.IsNullOrWhiteSpace(anonymousId)) return;

//            if (string.IsNullOrWhiteSpace(userId)) return;

//            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

//            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
//            //AnonymousIdentificationModule.ClearAnonymousIdentifier();

//        }

//        private bool SendConfirmationEmailAfterPhoneVerificationCode(string userEmail, string userId)//        {//            try//            {//                ConfirmationEmailAfterVerifyCodeModel model = new ConfirmationEmailAfterVerifyCodeModel()//                {//                    UserId = userId,//                    RequestedDate = DateTime.Now,//                    Email = userEmail//                };//                var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);//                string hashed = SecurityUtilities.HashData(token, null);
//                var emailSubject = LangText.BcareConfirmationEmail;//                string url = Url.Action("EmailConfirmation", "Account", new { token, hashed }, protocol: Request.Url.Scheme);//                string emailBody = string.Format(LangText.ConfirmationEmailAfterVerificationCode, url);
//                //await _notificationService.SendEmailAsync(userEmail, emailSubject, emailBody, null, true);

//                //return MailUtilities.SendMailOfRegistration(emailBody, emailSubject, "validation@bcare.com.sa", userEmail);
//                MessageBodyModel messageBodyModel = new MessageBodyModel();
//                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/Welcome.png";
//                messageBodyModel.Language = CultureInfo.CurrentCulture.Name;
//                messageBodyModel.MessageBody = emailBody;
//                return MailUtilities.SendMailOfRegistration(messageBodyModel, emailSubject, "validation@bcare.com.sa", userEmail);//            }//            catch (Exception ex)//            {//                return false;//            }//        }

//        //
//        // GET: /Account/EmailConfirmation
//        [AllowAnonymous]//        public async Task<ActionResult> EmailConfirmation(string token, string hashed)//        {//            if (string.IsNullOrEmpty(token))//            {//                return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorSecurity });//            }//            if (string.IsNullOrEmpty(hashed))//            {//                return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorHashing });//            }//            try//            {//                var decryptedToken = AESEncryption.DecryptString(token, Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);//                if (!SecurityUtilities.VerifyHashedData(hashed, token))
//                {
//                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorHashing });
//                }

//                var model = JsonConvert.DeserializeObject<ConfirmationEmailAfterVerifyCodeModel>(decryptedToken);//                if (model == null)//                {//                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorHashing });//                }//                var user = _authorizationService.GetUserDBByID(model.UserId);//                if (user == null)//                {//                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });//                }//                var result = _authorizationService.ConfirmUserEmailDB(model.UserId);//                if (result != 1)//                {//                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });//                }//                await SignInManager.SignInAsync(user, true, true);
//                string exception = string.Empty;
//                _authenticationContext.SetUserAuthenticationCookies(user, out exception);//                MigrateUser(user.Id);//                return RedirectToLocal(Utilities.SiteURL);//            }//            catch (Exception)//            {//                return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });//            }//        }











//        #region captcha//        public CaptchaModel GetCaptcha()//        {//            bool noisy = true;
//            //image stream 
//            string img = null;//            string token = null;//            using (var mem = new MemoryStream())//            using (var bmp = new Bitmap(100, 50))//            using (var gfx = Graphics.FromImage((Image)bmp))//            {//                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;//                gfx.SmoothingMode = SmoothingMode.AntiAlias;
//                //gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
//                var assembly = Assembly.GetExecutingAssembly();
//                //var resourceName = "MyCompany.MyProduct.MyFile.txt";
//                using (Stream stream = assembly.GetManifestResourceStream($"Tameenk.Captcha.captcha{GetRandomNumber(1, 10)}.jpg"))//                {//                    gfx.DrawImage(Image.FromStream(stream), new Rectangle(0, 0, bmp.Width, bmp.Height));//                }
//                //add noise 
//                if (noisy)//                {//                    int i, r, x, y;//                    var pen = new Pen(Color.Yellow);//                    for (i = 1; i < 10; i++)//                    {//                        pen.Color = Color.FromArgb(//                        (GetRandomNumber(0, 255)),//                        (GetRandomNumber(0, 255)),//                        (GetRandomNumber(0, 255)));//                        r = GetRandomNumber(0, (150 / 3));//                        x = GetRandomNumber(0, 150);//                        y = GetRandomNumber(0, 50);//                        gfx.DrawEllipse(pen, x - r, y - r, r, r);//                    }//                }//                string captcha = string.Empty;//                int captchaLength = 4;//                int charWidth = bmp.Width / captchaLength;//                for (var i = 0; i < captchaLength; i++)//                {//                    var digit = GetRandomNumber(1, 9);//                    captcha += digit;//                    gfx.DrawString(digit.ToString(),//                        new Font("Tahoma", GetRandomNumber(25, 40),//                        GetRandomFontStyle(),//                        GraphicsUnit.Pixel),//                        GetRandomBrush(),//                        GetRandomNumber(-3, 3) + (charWidth * i),//                        GetRandomNumber(2, 10));//                }

//                //render as Jpeg 
//                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);//                img = $"data:image/jpeg;base64,{Convert.ToBase64String(mem.ToArray())}";//                var captchaToken = new CaptchaModel//                {//                    Image = img,//                    Token = token,//                    ExpiredInSeconds = 600,//                    Captcha = captcha,//                    ExpiryDate = DateTime.Now.AddSeconds(605)//                };//                token = AESEncryption.EncryptString(JsonConvert.SerializeObject(captchaToken), SHARED_SECRET);//            }//            return new CaptchaModel { Image = img, Token = token, ExpiryDate = DateTime.Now, ExpiredInSeconds = 600 };//        }//        public bool ValidateCaptcha(CaptchaModel model, out string exp)//        {//            exp = "";//            var encryptedCaptcha = AESEncryption.DecryptString(model.Token, SHARED_SECRET);//            exp = encryptedCaptcha;//            try//            {//                var captchaToken = JsonConvert.DeserializeObject<CaptchaModel>(encryptedCaptcha);//                if (captchaToken.ExpiryDate.Value.CompareTo(DateTime.Now.AddSeconds(-captchaToken.ExpiredInSeconds)) < 0)//                {//                    return false;//                }//                if (captchaToken.Captcha.Equals(model.Input, StringComparison.Ordinal))//                {//                    return true;//                }//            }//            catch (Exception)//            {//                return false;//            }//            return false;//        }//        private static Random random;//        private static int GetRandomNumber(int min, int max)//        {//            random = random ?? new Random((int)DateTime.Now.Ticks);//            lock (random) // synchronize
//            {//                return random.Next(min, max);//            }//        }//        private FontStyle GetRandomFontStyle()//        {//            Dictionary<int, FontStyle> fontStyles = new Dictionary<int, FontStyle>();//            fontStyles.Add(0, FontStyle.Bold);//            fontStyles.Add(1, FontStyle.Italic);//            fontStyles.Add(2, FontStyle.Regular);//            fontStyles.Add(3, FontStyle.Underline);//            fontStyles.Add(4, FontStyle.Bold | FontStyle.Italic);//            fontStyles.Add(5, FontStyle.Italic | FontStyle.Underline);//            fontStyles.Add(6, FontStyle.Italic | FontStyle.Underline | FontStyle.Bold);//            fontStyles.Add(7, FontStyle.Bold | FontStyle.Regular);//            fontStyles.Add(8, FontStyle.Underline | FontStyle.Bold);//            return fontStyles[GetRandomNumber(1, 8)];//        }//        private Brush GetRandomBrush()//        {//            Dictionary<int, Brush> brushes = new Dictionary<int, Brush>();//            brushes.Add(1, Brushes.Black);//            brushes.Add(2, Brushes.Blue);//            brushes.Add(3, Brushes.Gray);//            brushes.Add(4, Brushes.Brown);//            brushes.Add(5, Brushes.Chocolate);//            brushes.Add(6, Brushes.Indigo);//            brushes.Add(7, Brushes.BlueViolet);//            return brushes[GetRandomNumber(1, 7)];//        }

//        #endregion
//        [AllowAnonymous]//        public ActionResult Policy()//        {//            PolicyViewModel model = new PolicyViewModel();//            model.Captcha = GetCaptcha();//            return View(model);//        }//        [AllowAnonymous]//        [HttpPost]//        public ActionResult Policy(PolicyViewModel model)//        {//            if (model.ButtonType == "Refresh")//            {//                model.PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();//                model.PolicyOutput.ErrorDescription = "Refresh";//                model.Captcha = GetCaptcha();//                return View(model);//            }//            string exp = "";//            try//            {//                if (model.Captcha != null)//                {//                    if (!string.IsNullOrEmpty(model.Captcha.Token))//                    {//                        if (ValidateCaptcha(model.Captcha, out exp))//                        {//                            var output = _checkoutContext.GetUserPolicies(model.NIN, model.SequenceNumber, model.CustomCardNumber, out exp);//                            output.ErrorDescription = exp;//                            model.PolicyOutput = output;//                        }//                        else//                        {//                            model.PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();//                            model.PolicyOutput.ErrorDescription = "Invalid Captcha";//                        }//                    }//                    else//                    {//                        model.PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();//                        model.PolicyOutput.ErrorDescription = "Token is null";//                    }//                }//                else//                {//                    model.PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();//                    model.PolicyOutput.ErrorDescription = "Captcha is null";//                }

//            }//            catch//            {//                model.PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();//                model.PolicyOutput.ErrorDescription = exp;//            }//            model.Captcha = GetCaptcha();//            return View(model);//        }//        [AllowAnonymous]//        [HttpGet]//        public FileResult DownloadPolicyFile(string fileId)//        {//            var policyFile = _policyService.DownloadPolicyFile(fileId);//            if (policyFile != null)//            {//                if (policyFile.PolicyFileByte != null)//                {//                    return File(policyFile.PolicyFileByte, "application/pdf", fileId + ".pdf");//                }//                else if (!string.IsNullOrEmpty(policyFile.FilePath))//                {//                    if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)//                    {//                        FileNetworkShare fileShare = new FileNetworkShare();//                        string exception = string.Empty;//                        var file = fileShare.GetFileFromShare(_tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP, _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, policyFile.FilePath, out exception);//                        if (file != null)//                            return File(file, "application/pdf", fileId + ".pdf");//                        else//                            return null;//                    }//                    return File(System.IO.File.ReadAllBytes(policyFile.FilePath), "application/pdf", fileId + ".pdf");//                }//                else//                {//                    return null;//                }//            }//            else//            {//                return null;//            }//        }

//        [AllowAnonymous]//        [HttpGet]//        public ActionResult GenerateAndDownloadFailedPolicyFile(string referenceId, string lang)//        {//            string exception = string.Empty;//            var FailedPolicies = _checkoutContext.GetUserFailedPoliciesByReference(referenceId, out exception);//            if (FailedPolicies == null || FailedPolicies.FailedPolicies == null || FailedPolicies.FailedPolicies.Count == 0)
//            {
//                return RedirectToAction("Index", "Error", new { message = "FailedPolicies is null" });//            }//            if (FailedPolicies.ErrorCode != Services.Checkout.Components.Output.PolicyOutput.ErrorCodes.Success)
//            {
//                return RedirectToAction("Index", "Error", new { message = FailedPolicies.ErrorDescription });//            }
//            var tempPolicy = FailedPolicies.FailedPolicies.FirstOrDefault();//            PdfGenerationLog log = new PdfGenerationLog();
//            log.Channel = Channel.Portal.ToString();
//            log.CompanyID = tempPolicy.InsuranceCompanyID;
//            log.ReferenceId = referenceId;
//            log.ServerIP = Utilities.GetInternalServerIP();

//            PolicyDetails policyDetails = new PolicyDetails();
//            policyDetails.PolicyNo = string.Format("QTN{0}", tempPolicy.QuotationNo);
//            policyDetails.ReferenceNo = tempPolicy.ReferenceId;
//            //policyDetails.VehicleCapacity = "5";
//            //policyDetails.PolicyCoverAgeLimitEn = new List<string>();
//            //policyDetails.PolicyCoverAgeLimitAr = new List<string>();
//            //policyDetails.PolicyAdditionalCoversEn = new List<string>();
//            //policyDetails.PolicyAdditionalCoversAr = new List<string>();

//            var policyResponseMessage = new PolicyResponse
//            {
//                ReferenceId = referenceId,
//                PolicyNo = string.Format("QTN{0}", tempPolicy.QuotationNo),
//                PolicyExpiryDate = tempPolicy.PolicyExpiryDate,
//                PolicyIssuanceDate = DateTime.Now,
//                PolicyEffectiveDate = tempPolicy.PolicyEffectiveDate,
//                PolicyDetails = policyDetails
//            };
//            LanguageTwoLetterIsoCode language = lang == "en" ? LanguageTwoLetterIsoCode.En : LanguageTwoLetterIsoCode.Ar;
//            var policyFile = _policyContext.GeneratePolicyFileFromPolicyDetails(policyResponseMessage, tempPolicy.InsuranceCompanyID, language, log);
//            if (policyFile.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
//            {
//                return RedirectToAction("Index", "Error", new { message = "policyFile: " + policyFile.ErrorDescription });
//            }
//            if (policyFile != null)//            {//                if (policyFile.File != null)//                {//                    return File(policyFile.File, "application/pdf", referenceId + ".pdf");//                }//                else//                {
//                    return RedirectToAction("Index", "Error", new { message = "policyFile.File is null" });//                }//            }//            else//            {
//                return RedirectToAction("Index", "Error", new { message = "policyFile is null" });//            }//        }

//        [AllowAnonymous]//        public ActionResult CheckDiscount()//        {//            CheckDiscountViewModel model = new CheckDiscountViewModel();//            model.Captcha = GetCaptcha();//            return View(model);//        }

//        [AllowAnonymous]//        [HttpPost]//        public ActionResult CheckDiscount(CheckDiscountViewModel model)//        {//            string exp = "";//            try//            {//                if (model.Captcha != null)//                {//                    if (!string.IsNullOrEmpty(model.Captcha.Token))//                    {//                        if (ValidateCaptcha(model.Captcha, out exp))//                        {//                            CheckDiscountOutput output = _policyService.CheckDiscountByNIN(model.NIN);//                            if (output.ErrorCode == CheckDiscountOutput.ErrorCodes.Success)
//                            {
//                                output.ErrorDescription = LangText.DeserveDiscount;//                            }//                            else if (output.ErrorCode == CheckDiscountOutput.ErrorCodes.EmptyReturnObject)
//                            {
//                                output.ErrorDescription = LangText.NotDeserveDiscount;//                            }//                            else
//                            {
//                                output.ErrorDescription = exp;//                            }//                            model.CheckDiscountOutput = output;//                        }//                        else//                        {//                            model.CheckDiscountOutput = new CheckDiscountOutput();//                            model.CheckDiscountOutput.ErrorDescription = "Invalid Captcha";//                        }//                    }//                    else//                    {//                        model.CheckDiscountOutput = new CheckDiscountOutput();//                        model.CheckDiscountOutput.ErrorDescription = "Token is null";//                    }//                }//                else//                {//                    model.CheckDiscountOutput = new CheckDiscountOutput();//                    model.CheckDiscountOutput.ErrorDescription = "Captcha is null";//                }
//            }//            catch//            {//                model.CheckDiscountOutput = new CheckDiscountOutput();//                model.CheckDiscountOutput.ErrorDescription = exp;//            }//            model.Captcha = GetCaptcha();//            return View(model);//        }

//        [HttpPost]
//        [AllowAnonymous]
//        //[ValidateAntiForgeryToken]
//        public async Task<ActionResult> VerifyOTP(LoginViewModel model, string returnUrl) //OneTimePasswordViewModel model
//        {
//            if (string.IsNullOrEmpty(model.UserName))
//            {
//                AddErrorsToModelStateErors(new Dictionary<string, string>() { { "Email", "مطلوب*" } });
//                return View(model);
//            }

//            var encryptedEmail = Convert.FromBase64String(model.UserName.Trim());
//            var plainEmail = SecurityUtilities.DecryptStringFromBytes_AES(encryptedEmail, Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"), Encoding.UTF8.GetBytes("BcARe_2021_N0MeM"));
//            model.Email = plainEmail;

//            var output = await _corporateContext.VerifyCorporateOneTimePassword(model.Email, model.Otp,Channel.Portal, model.Lang = "ar");
//            if (output.ErrorCode != ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.Success)
//            {
//                ModelState.AddModelError("", output.ErrorDescription);
//                ViewBag.islogin = 1;
//                ViewBag.IsCorporateUser = 1;
//                ViewBag.returnUrl = returnUrl;
//                ViewBag.loginModel = model;
//                ViewBag.registerModel = new RegisterViewModel();
//                return View("Auth");
//            }

//            string exception;
//            var isAuthenticationCookiesSet = _authenticationContext.SetCorporateUserAuthenticationCookies(output.Result.CorporateUser, out exception);
//            if (!isAuthenticationCookiesSet || !string.IsNullOrEmpty(exception))
//            {
//                output.ErrorCode = ProfileOutput<VerifyCorporateOTPOutput>.ErrorCodes.ServiceException;
//                output.ErrorDescription = exception;

//                return RedirectToAction("Index", "Error", new { message = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang)) });
//            }

//            return RedirectToAction("ClearBrowserLocalStorage", new { returnUrl = returnUrl });
//        } 
//    }
//}