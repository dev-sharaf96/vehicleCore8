using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Models;
using Tameenk.Models.Checkout;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;


using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Tameenk.Resources.Checkout;
using Tameenk.Security.Encryption;
using Tameenk.Services.Implementation;

namespace Tameenk.Controllers
{
    //[Authorize]
    [TameenkAuthorizeAttribute]
    public class CheckoutController : Controller
    {
        #region Fields


        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IQuotationService _quotationService;
        private readonly IOrderService _orderService;
        private readonly IDriverService _driverService;
        private readonly Random _rnd;
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ISadadPaymentService _sadadPaymentService;
        private readonly ILogger _logger;
        private readonly ICheckoutContext _checkoutContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotificationService _notificationService;
        private const string Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        #endregion

        #region Ctor


        public CheckoutController(IPayfortPaymentService payfortPaymentService,
            IShoppingCartService shoppingCartService,
            IQuotationService quotationService,
            IOrderService orderService,
            IDriverService driverService,
            IHttpClient httpClient,
            IInsuranceCompanyService insuranceCompanyService,
            ISadadPaymentService sadadPaymentService,
            TameenkConfig tameenkConfig,
            ILogger logger,
            ICheckoutContext checkoutContext,
            IAuthorizationService authorizationService,
            INotificationService notificationService)
        {
            _payfortPaymentService = payfortPaymentService;
            _shoppingCartService = shoppingCartService;
            _quotationService = quotationService;
            _orderService = orderService;
            _driverService = driverService;
            _rnd = new Random(System.Environment.TickCount);
            _httpClient = httpClient;
            _config = tameenkConfig;
            _insuranceCompanyService = insuranceCompanyService;
            _sadadPaymentService = sadadPaymentService;
            _logger = logger;
            _checkoutContext = checkoutContext;
            _authorizationService = authorizationService;
            _notificationService = notificationService;

        }

        #endregion

        private string CurrentUserID
        {
            get
            {
                return User.Identity.GetUserId<string>();
            }
        }

        #region Actions

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_accessToken))
                    {
                        var formParamters = new Dictionary<string, string>();
                        formParamters.Add("grant_type", "client_credentials");
                        formParamters.Add("client_Id", _config.Identity.ClientId);
                        formParamters.Add("client_secret", _config.Identity.ClientSecret);

                        var content = new FormUrlEncodedContent(formParamters);
                        var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                        postTask.ConfigureAwait(false);
                        postTask.Wait();
                        var response = postTask.Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = response.Content.ReadAsStringAsync().Result;
                            var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                            _accessToken = result.access_token;
                        }
                    }
                    return _accessToken;

                }
                catch (Exception ex)
                {
                    var logId = DateTime.Now.GetTimestamp();
                    _logger.Log($"CheckoutController -> GetAccessToken [key={logId}]", ex);
                    return _accessToken;
                }
            }
        }
        public class AccessTokenResult
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }
            [JsonProperty("expires_in")]
            public int expires_in { get; set; }


        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        public ActionResult CheckoutDetails(string ReferenceId, string QtRqstExtrnlId, string productId,string selectedProductBenfitId,string hashed)
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = User.Identity.GetUserId();
            log.Channel = "Portal";
            
            var lang = GetCurrentLanguage();
            var output = _checkoutContext.CheckoutDetails(ReferenceId, QtRqstExtrnlId, log, lang,productId,selectedProductBenfitId,hashed);
            if (output.ErrorCode == CheckoutOutput.ErrorCodes.UserLockedOut)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            else if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            else
            {
                ViewBag.ErrorValidatingUserData = output.CheckoutModel.ErrorValidatingUserData;
            }
            output.CheckoutModel.AccessToken = AccessToken;
            return View(output.CheckoutModel);

        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult SubmitCheckoutDetails(CheckoutModel model)
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = User.Identity.GetUserId();
            log.Channel = "Portal";

            var lang = GetCurrentLanguage();
            var output = _checkoutContext.SubmitCheckoutDetails(model, log, lang);
            if (output.ErrorCode == CheckoutOutput.ErrorCodes.UserLockedOut)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            else if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription, returnURL = Request.UrlReferrer.PathAndQuery });
            }
            else
            {
                if (!output.CheckoutModel.IsCheckoutEmailVerified)
                {
                    string exception = string.Empty;
                    bool isMailSent = _checkoutContext.SendActivationEmail(model.ReferenceId,CurrentUserID,out exception);
                }
                if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Hyperpay)
                {
                    return RedirectToAction("PaymentUsingHyperpay", "TameenkPayment", new { model.ReferenceId, QtRqstExtrnlId = output.CheckoutModel.QtRqstExtrnlId,model.ProductId,model.SelectedProductBenfitId,model.Hashed, paymentMethodCode = (int)PaymentMethodCode.Hyperpay });
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Mada)
                {
                    return RedirectToAction("PaymentUsingHyperpay", "TameenkPayment", new { model.ReferenceId, QtRqstExtrnlId = output.CheckoutModel.QtRqstExtrnlId, model.ProductId, model.SelectedProductBenfitId, model.Hashed, paymentMethodCode = (int)PaymentMethodCode.Mada });
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Edaat)
                { 
                    return View("PaymentUsingEdaat", output.EdaatPaymentResponseModel);
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.AMEX)
                {
                    return RedirectToAction("PaymentUsingHyperpay", "TameenkPayment", new { model.ReferenceId, QtRqstExtrnlId = output.CheckoutModel.QtRqstExtrnlId, model.ProductId, model.SelectedProductBenfitId, model.Hashed, paymentMethodCode = (int)PaymentMethodCode.AMEX });
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                {
                    return RedirectToAction("PaymentUsingHyperpay", "TameenkPayment", new { model.ReferenceId, QtRqstExtrnlId = output.CheckoutModel.QtRqstExtrnlId, model.ProductId, model.SelectedProductBenfitId, model.Hashed, paymentMethodCode = (int)PaymentMethodCode.ApplePay });
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Wallet)
                {
                    return RedirectToAction("Purchased", "Checkout", new { referenceId = model.ReferenceId });
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                {
                    return RedirectToAction("PaymentUsingHyperpay", "TameenkPayment", new { model.ReferenceId, QtRqstExtrnlId = output.CheckoutModel.QtRqstExtrnlId, model.ProductId, model.SelectedProductBenfitId, model.Hashed, paymentMethodCode = (int)PaymentMethodCode.ApplePay });
                }
                else
                {
                    return View("PaymentUsingSadad", output.SadadPaymentResponseModel);
                }
            }

        }
        public ActionResult PaymentUsingHyperpay(string checkoutId, string referenceId, string email, bool isEmailVerified, int paymentMethod)
        {
            ViewBag.Id = checkoutId.Trim();//TempData.Peek("id");
            ViewBag.Email = email;
            ViewBag.ReferenceId = referenceId;
            ViewBag.IsCheckoutEmailVerified = isEmailVerified;
            ViewBag.PaymentMethod = paymentMethod;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult VerifyCode(VerifyCodeCheckoutViewModel model)
        {
            var result = _checkoutContext.ManagePhoneVerification(Guid.Parse(model.UserId), model.PhoneNumber, model.Code,"Portal");

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ResendVerifyCode(string userId, string phoneNumber)
        {
            var checkoutUser = _orderService.GetByUserIdAndPhoneNumber(Guid.Parse(userId), phoneNumber);
            if(checkoutUser!=null)
            {
                Random rnd = new Random();
                int verifyCode = rnd.Next(1000, 9999);
                checkoutUser.VerificationCode = verifyCode;
                _orderService.UpdateCheckoutUser(checkoutUser);
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneNumber,
                    MessageBody = string.Format(LangText.VerificationCodeMessage, verifyCode),
                    Method = SMSMethod.ResendOTP.ToString(),
                    Module = Module.Vehicle.ToString()
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                return Json(new { data = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private  Methods


       

        private LanguageTwoLetterIsoCode GetCurrentLanguage()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase) ?
                LanguageTwoLetterIsoCode.En : LanguageTwoLetterIsoCode.Ar;
        }

        //public ActionResult VerifyCheckoutDriverInfo(string userId, string PhoneNumber, string referenceId, string email,string iban, bool isReceivePolicyByEmailChecked)
        //{
        //    var output = _checkoutContext.VerifyCheckoutDriverInfo(userId, PhoneNumber, referenceId, email, iban,Channel.Portal.ToString(), isReceivePolicyByEmailChecked);
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.Success)
        //    {
        //        return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode==CheckoutOutput.ErrorCodes.EmailAlreadyUsed)
        //    {
        //        return Json(new { data = 4 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.PhoneAlreadyUsed)
        //    {
        //        return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver)
        //    {
        //        return Json(new { data = 6 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.PhoneIsNotVerified)
        //    {
        //        return Json(new { data = 3 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.InvalidEmail)
        //    {
        //        return Json(new { data = 7 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.InvalidPhone)
        //    {
        //        return Json(new { data = 8 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.InvalidIBAN)
        //    {
        //        return Json(new { data = 9 }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (output.ErrorCode == CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear)
        //    {
        //        return Json(new { data = 10 }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { data = 5 }, JsonRequestBehavior.AllowGet);
        //    }
        //}

       

        public ActionResult SendActivationEmailToReceivePolicy(string referenceId)
        {
            string exception = string.Empty;
            bool isMailSent = _checkoutContext.SendActivationEmail(referenceId,CurrentUserID,out exception);
            if (isMailSent)
            {
                return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult ActivateUserEmailToReceivePolicy(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorSecurity });
            }
            try
            {
                var decryptedToken = AESEncryption.DecryptString(token, Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);
                var model = JsonConvert.DeserializeObject<ActivationEmailToReceivePolicyModel>(decryptedToken);
                if (model == null)
                {
                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorHashing });
                }

                //var user = _authorizationService.GetUserDBByID(model.UserId);
                //if (user == null)
                //{
                //    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });
                //}
                 string exception = string.Empty;
                var output = _checkoutContext.ActivateUserEmailToReceivePolicy(model.ReferenceId, model.Email, model.UserId,Channel.Portal.ToString(),out exception);
                if(output.ErrorCode == CheckoutOutput.ErrorCodes.ServiceDown)
                {
                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorNoRecordFound });
                }
                if (output.ErrorCode == CheckoutOutput.ErrorCodes.ServiceException)
                {
                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGenericException });
                }
                if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGeneric });
                }
                return RedirectToAction("Index", "Success", new { message = LangText.EmailActivated });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { message = CheckoutResources.ErrorGenericException });
            }
        }


        [HttpGet]
        public ActionResult Purchased(string referenceId)
        {
            var lang = GetCurrentLanguage();
            var checkoutDetail = _orderService.GetCheckoutDetailByReferenceId(referenceId);
            var insuranceCompany = checkoutDetail.OrderItems.FirstOrDefault().Product.InsuranceCompany;
            var model = new PurchasedProductModel()
            {
                InsuranceCompanyDesc = lang == LanguageTwoLetterIsoCode.Ar
                    ? insuranceCompany.DescAR
                    : insuranceCompany.DescEN,
                InsuranceType = lang == LanguageTwoLetterIsoCode.Ar
                    ? checkoutDetail.ProductType.ArabicDescription
                    : checkoutDetail.ProductType.EnglishDescription,
                InsuranceCompanyKey = insuranceCompany.Key
            };
            model.InsuraneCompanyId = insuranceCompany.InsuranceCompanyID;
            model.CheckoutEmail = checkoutDetail.Email;
            if (insuranceCompany.Contact != null)
            {
                model.CompanyEmail = insuranceCompany.Contact.Email;
                model.CompanyFax = insuranceCompany.Contact.Fax;
                model.CompanyHomePhone = insuranceCompany.Contact.HomePhone;
                model.CompanyMobile = insuranceCompany.Contact.MobileNumber;
            }
            return View(model);
        }

        #endregion

    }
}