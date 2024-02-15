using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Tameenk.Api.Core;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Models.Checkout;
using Tameenk.Resources.Checkout;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Checkout.Components.Output;
using Microsoft.AspNet.Identity;

using System.Linq;
using Tameenk.Services.Core.Payments;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Security.Encryption;
using System.Globalization;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core;
using Tameenk.Security.CustomAttributes;
using PrometheusLib.Classes;

namespace Tameenk.Services.Checkout.Api.Controllers
{
    public class CheckoutController : BaseApiController
    {
        #region Fields

        private readonly ICheckoutContext _checkoutContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPaymentService _paymentService;

        private const string Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        private readonly HashSet<string> allowedLanguage = new HashSet<string>() { "ar", "en" };

        #endregion

        #region Constructor

        public CheckoutController(ICheckoutContext checkoutContext, IAuthorizationService authorizationService,
             IPaymentService paymentService)
        {
            this._checkoutContext = checkoutContext;
            this._authorizationService = authorizationService;
            _paymentService = paymentService;
        }


        #endregion

        [HttpGet]
        public IHttpActionResult CheckoutDetails(string ReferenceId, string QtRqstExtrnlId, string lang, string Channel, string productId = "", string hashed = "", string selectedProductBenfitId = "")
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            string userId = _authorizationService.GetUserId(User);
            log.UserId = string.IsNullOrEmpty(userId) ? "" : userId;
            log.Channel = Channel;


            var language = GetCurrentLanguage(lang);

            try
            {
                var output = _checkoutContext.CheckoutDetails(ReferenceId, QtRqstExtrnlId, log, language, productId, selectedProductBenfitId, hashed);
                return Ok(output);
            }
            catch (Exception ex)
            {
                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "CheckoutDetails-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }

        }
        [HttpPost]
        [SingleSessionAuthorizeAttribute]
        [Route("api/Checkout/SubmitCheckoutDetails")]
        public IHttpActionResult SubmitCheckoutDetails()
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            string userId = _authorizationService.GetUserId(User);
            log.UserId = userId;
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    var data = HttpContext.Current.Request.Form["data"];
                    var language = GetCurrentLanguage(HttpContext.Current.Request.Form["lang"]);
                    log.Channel = HttpContext.Current.Request.Form["channel"];
                    var request = JsonConvert.DeserializeObject<CheckoutModel>(data);
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request.Form["imageRight"])&&
                        !string.IsNullOrEmpty(HttpContext.Current.Request.Form["imageLeft"]) &&
                        !string.IsNullOrEmpty(HttpContext.Current.Request.Form["imageFront"]) &&
                        !string.IsNullOrEmpty(HttpContext.Current.Request.Form["imageBack"]) &&
                        !string.IsNullOrEmpty(HttpContext.Current.Request.Form["imageBody"]))
                    {
                        request.ImageRight = Convert.FromBase64String(HttpContext.Current.Request.Form["imageRight"]);;
                        request.ImageLeft = Convert.FromBase64String(HttpContext.Current.Request.Form["imageLeft"]);
                        request.ImageFront = Convert.FromBase64String(HttpContext.Current.Request.Form["imageFront"]);
                        request.ImageBack = Convert.FromBase64String(HttpContext.Current.Request.Form["imageBack"]);
                        request.ImageBody = Convert.FromBase64String(HttpContext.Current.Request.Form["imageBody"]);
                    }
                    if (!string.IsNullOrEmpty(request.Channel)&& request.Channel != "null")
                    {
                        log.Channel = request.Channel;
                    }
                    var output = _checkoutContext.SubmitCheckoutDetails(request, log, language);
                    if (output.ErrorCode == CheckoutOutput.ErrorCodes.Success && !output.CheckoutModel.IsCheckoutEmailVerified)
                    {
                        string exception = string.Empty;
                        bool isMailSent = _checkoutContext.SendActivationEmail(request.ReferenceId, userId, out exception);
                    }
                    request = null;
                    decimal paymentAmount = 0;
                    if (output.ErrorCode == CheckoutOutput.ErrorCodes.Success)
                    {
                        paymentAmount = output.CheckoutModel.PaymentAmount;
                    }
                    output.CheckoutModel = null;
                    if(paymentAmount>0)
                    {
                        output.CheckoutModel = new CheckoutModel();
                        output.CheckoutModel.PaymentAmount = paymentAmount;
                    }

                    return Ok(output);
                }
                return Error("");
            }
            catch (Exception ex)
            {
                var lang = GetCurrentLanguage(HttpContext.Current.Request.Form["lang"]).ToString().ToLower();
                if (string.IsNullOrEmpty(lang) || !allowedLanguage.Contains(lang))
                    lang = "ar";

                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "SubmitCheckoutDetails-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }
        }

        [HttpPost]
        [SingleSessionAuthorizeAttribute]
        [Route("api/Checkout/AddToCart")]
        public IHttpActionResult AddItemToCart([FromBody] Tameenk.Core.Domain.Dtos.AddItemToCartModel model)
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            string userId = _authorizationService.GetUserId(User);
            log.UserId = userId;
            log.Channel = model.Channel;

            var language = GetCurrentLanguage(model.lang);
            try
            {
                var output = _checkoutContext.AddItemToCart(model, log, model.lang);
                return Ok(output);
            }
            catch (Exception ex)
            {
                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.lang));

                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "CheckoutDetails-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }
        }


        [HttpGet]
        public IHttpActionResult HyperpayProcessPayment(string id, string Lang, string Channel)
        {
            string userId = _authorizationService.GetUserId(User);
            Guid uId = Guid.Empty;
            Guid.TryParse(userId, out uId);

            var language = GetCurrentLanguage(Lang);
            try
            {
                var output = _checkoutContext.ProcessHyperpayPayment(id, Lang, Channel, uId, (int)PaymentMethodCode.Hyperpay);
                //if (output.PaymentMethodId == (int)PaymentMethodCode.ApplePay)
                //{
                //    var hyperPayUpdateOrderOutput = _checkoutContext.HyperpayUpdateOrder(output.CheckoutDetail, output.HyperpayResponse, output.PaymentId, output.PaymentSucceded);
                //}
                if (output.ErrorCode == HyperPayOutput.ErrorCodes.Success)
                { 
                    PrometheusHttpRequestModule.IncrementCheckoutPaidCounter();
                }

                return Ok(output);
            }
            catch (Exception ex)
            {
                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(Lang));

                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "HyperpayProcessPayment-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }
        }


        private LanguageTwoLetterIsoCode GetCurrentLanguage(string lang)
        {
            return lang.ToLower() == "en" ? LanguageTwoLetterIsoCode.En : LanguageTwoLetterIsoCode.Ar;
        }

        //[HttpPost]        //[Route("api/Checkout/VerifyCheckoutDriverInfo")]        //public IHttpActionResult VerifyCheckoutDriverInfo([FromBody] Tameenk.Core.Domain.Dtos.CheckoutDriverInfoModel model)        //{        //    CheckoutOutput output = new CheckoutOutput();        //    try        //    {
        //        string userId = _authorizationService.GetUserId(User);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;
        //            output.ErrorDescription = "Anonymous User";
        //            return Ok(output);
        //        }        //        output = _checkoutContext.VerifyCheckoutDriverInfo(userId, model.PhoneNumber, model.ReferenceId, model.Email, model.IBAN, model.Channel.ToString(), model.IsReceivePolicyByEmailChecked);
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.EmailAlreadyUsed)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.EmailAlreadyUsed;
        //            output.ErrorDescription = CheckoutResources.EmailUsedWithAnotherDriver;
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.PhoneAlreadyUsed)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneAlreadyUsed;
        //            output.ErrorDescription = CheckoutResources.PhoneNoWithAnotherDriver;
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver;
        //            output.ErrorDescription = CheckoutResources.IBANUsedForOtherDriver;
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.PhoneIsNotVerified)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneIsNotVerified;
        //            output.ErrorDescription = "Sucess";
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear;
        //            output.ErrorDescription = CheckoutResources.UserExceedsInsuranceNumberLimitCheckoutPopup;
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode == CheckoutOutput.ErrorCodes.InvalidIBAN)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
        //            output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
        //            return Ok(output);
        //        }
        //        if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
        //            output.ErrorDescription = CheckoutResources.ErrorGeneric;
        //            return Ok(output);
        //        }
        //        return Ok(output);        //    }        //    catch (Exception ex)        //    {        //        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;        //        output.ErrorDescription = CheckoutResources.ErrorGeneric;        //        return Ok(output);        //    }        //}        [HttpPost]        [Route("api/Checkout/VerifyCode")]        public IHttpActionResult VerifyCode(string phoneNumber, string code,string channel)        {            string userId = _authorizationService.GetUserId(User);
            CheckoutOutput output = new CheckoutOutput();
            if (string.IsNullOrEmpty(userId))            {                output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;                output.ErrorDescription = "Anonymous User";
                return Ok(output);            }            var user = _authorizationService.GetUserDBByID(userId);
            if (user == null)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                output.ErrorDescription = "user is null";
                return Ok(output);
            }            if (user.IsCorporateUser)
            {
                phoneNumber = user.PhoneNumber;            }
          //  var language = GetCurrentLanguage(Lang);            var result = _checkoutContext.ManagePhoneVerification(Guid.Parse(userId), phoneNumber, code,channel);            if (!result)            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidVerifyCode;
               // output.ErrorDescription = "Invalid Verify Code";
                output.ErrorDescription = CheckoutResources.InvalidCode;
                return Ok(output);
            }            output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";            return Ok(output);        }        [HttpPost]        [Route("api/Checkout/ResendVerifyCode")]        public IHttpActionResult ResendVerifyCode(string userId, string phoneNumber, string lang="ar")        {            var output = _checkoutContext.ResendVerifyCode(userId,phoneNumber,lang);            return Ok(output);        }

        [HttpPost]
        [Route("api/checkout/sendactivationemail")]
        public IHttpActionResult SendActivationEmailToReceivePolicy([FromBody] Tameenk.Core.Domain.Dtos.SendActivationEmailModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            CheckoutOutput output = new CheckoutOutput();
            if (string.IsNullOrEmpty(userId))            {                output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                return Ok(output);            }
            model.UserId = userId;
            string exception = string.Empty;
            bool isMailSent = _checkoutContext.SendActivationEmail(model.ReferenceId, model.UserId, out exception);
            if (isMailSent)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
            }
            else
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.MailNotSent;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
            }
            return Ok(output);
        }

        [HttpGet]
        [Route("api/Checkout/paymentmethods")]
        public IHttpActionResult GetAllPaymentMethodByChannel(string channel, bool showWalletPaymentOption = false)
        {
            try
            {
                if (string.IsNullOrEmpty(channel))
                    return Error("channel is null or empty");

                List<Tameenk.Core.Domain.Entities.Payments.PaymentMethod> result = null;
                if (channel.ToLower() == "ios")
                {
                    result = _paymentService.GetActivePaymentMethod().Where(a => a.IosEnabled == true).ToList();
                }
                else if (channel.ToLower() == "android")
                {
                    result = _paymentService.GetActivePaymentMethod().Where(a => a.AndroidEnabled == true).ToList();
                }
                else
                {
                    result = _paymentService.GetActivePaymentMethod();
                }

                if (!showWalletPaymentOption)
                {
                    // get all except Wallet Payment
                    result = result.Where(p => p.Code != (int)PaymentMethodCode.Wallet).ToList();
                }

                if (result != null)
                {
                    if (channel.ToLower() == "portal")
                    {
                        var deviceInfo = Utilities.GetDeviceInfo();

                        if (deviceInfo != null
                        &&!string.IsNullOrEmpty(deviceInfo.Client)
                         &&!string.IsNullOrEmpty(deviceInfo.OS)
                        && (deviceInfo.Client.ToLower().Contains("safari")
                        || deviceInfo.Client.ToLower().Contains("Chrome Mobile iOS".ToLower())))
                        {
                            // same payment methods with no change
                        }
                        else
                        {
                            // get all except apple pay
                            result = result.Where(p => p.Code != (int)PaymentMethodCode.ApplePay).ToList();
                        }
                    }

                    List<Tameenk.Core.Domain.Dtos.PaymentMethodModel> methods = new List<Tameenk.Core.Domain.Dtos.PaymentMethodModel>();
                    foreach (var item in result)
                    {
                        Tameenk.Core.Domain.Dtos.PaymentMethodModel method = new Tameenk.Core.Domain.Dtos.PaymentMethodModel();
                        method.Active = item.Active;
                        method.Code = item.Code;
                        method.Name = item.Name;
                        method.Order = item.Order;
                        method.Brands = item.Brands;
                        method.LogoUrl = item.LogoUrl;
                        method.EnglishDescription = item.EnglishDescription;
                        method.ArabicDescription = item.ArabicDescription;

                        if (item.Code == 16)
                            methods.Insert(0, method);
                        else
                            methods.Add(method);
                    }
                    return Ok(methods);
                }
                return Error("No Payment Method Available");
            }
            catch (Exception ex)
            {
                var lang = GetCurrentLanguage(HttpContext.Current.Request.Form["lang"]).ToString().ToLower();
                if (string.IsNullOrEmpty(lang) || !allowedLanguage.Contains(lang))
                    lang = "ar";

                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "paymentmethods-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }
        }

        [HttpPost]
        [Route("api/Checkout/PaymentUsingHyperpay")]
        public IHttpActionResult PaymentUsingHyperpay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang)
        {
            try
            {
                string userId = _authorizationService.GetUserId(User);
                var output = _checkoutContext.PaymentUsingHyperpay(referenceId, QtRqstExtrnlId, productId, selectedProductBenfitId, hashed, paymentMethodCode, channel, lang, userId);
                if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    return Error(output);
                }
                return Ok(output);
            }
            catch (Exception ex)            {                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "PaymentUsingHyperpay-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);            }
        }

        [HttpPost]
        [Route("api/Checkout/PaymentUsingApplePay")]
        public ApplePaySessionResponseModel PaymentUsingApplePay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang)
        {
            try
            {
                string userId = _authorizationService.GetUserId(User);
                var output = _checkoutContext.PaymentUsingApplePay(referenceId, QtRqstExtrnlId, productId, selectedProductBenfitId, hashed, paymentMethodCode, channel, lang, userId);
                if (output.ErrorCode != ApplePayOutput.ErrorCodes.Success)
                {
                    return null;
                }

                //var output = _checkoutContext.PaymentUsingHyperpay(referenceId, QtRqstExtrnlId, productId, selectedProductBenfitId, hashed, paymentMethodCode, channel, lang, userId);
                //if (output.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                //{
                //    return Error(output);
                //}
                return output.Result;
            }
            catch (Exception ex)            {                return null;            }

        }

        [HttpPost]
        [Route("api/Checkout/processapplepaypayment")]
        public IHttpActionResult ProcessApplePayPayment()
        {
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    string referenceId = HttpContext.Current.Request.Form[0];
                    string paymentToken = HttpContext.Current.Request.Form[1];
                    var lang = GetCurrentLanguage(HttpContext.Current.Request.Form[2]);
                    string userId = _authorizationService.GetUserId(User);
                    var output = _checkoutContext.ApplePayProcessPayment(referenceId, paymentToken, lang.ToString(), userId);
                    if (output.ErrorCode != ApplePayOutput.ErrorCodes.Success)
                    {
                        return Error(output);
                    }
                    return Ok(output);
                }
                return Error("invalid request");
            }
            catch (Exception ex)            {                var lang = GetCurrentLanguage(HttpContext.Current.Request.Form[2]).ToString().ToLower();
                if (string.IsNullOrEmpty(lang) || !allowedLanguage.Contains(lang))
                    lang = "ar";                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "ProcessApplePayPayment-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);            }

        }

        [HttpPost]
        [Route("api/checkout/activateUserEmailToReceivePolicy")]
        public IHttpActionResult ActivateUserEmailToReceivePolicy([FromBody] Tameenk.Core.Domain.Dtos.ActivateUserEmailModel model)
        {
            CheckoutOutput output = new CheckoutOutput();
            string userId = _authorizationService.GetUserId(User);
            model.Channel = (string.IsNullOrEmpty(model.Channel)) ? Channel.Portal.ToString() : model.Channel;

            try
            {
                if (model == null || string.IsNullOrEmpty(model.Token))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString(CheckoutResources.ResourceManager.GetString("ErrorSecurity", CultureInfo.GetCultureInfo(model.Lang)));
                    return Error(output);
                }

                var decryptedToken = AESEncryption.DecryptString(model.Token, Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);
                var decryptedModel = JsonConvert.DeserializeObject<ActivationEmailToReceivePolicyModel>(decryptedToken);
                if (decryptedModel == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString(CheckoutResources.ResourceManager.GetString("ErrorHashing", CultureInfo.GetCultureInfo(model.Lang)));
                    return Error(output);
                }

                string exception = string.Empty;
                var result = _checkoutContext.ActivateUserEmailToReceivePolicy(decryptedModel.ReferenceId, decryptedModel.Email, userId, model.Channel, out exception);
                if (result.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString(CheckoutResources.ResourceManager.GetString("ErrorGenericException", CultureInfo.GetCultureInfo(model.Lang)));
                    return Error(output);
                }

                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString(CheckoutResources.ResourceManager.GetString("EmailActivated", CultureInfo.GetCultureInfo(model.Lang)));
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));

                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.MethodName = "ActivateUserEmailToReceivePolicy-Exception";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return Ok(output);
            }
        }

        [HttpPost]
        [Route("api/checkout/checkDiscountCode")]
        public IHttpActionResult CheckDiscountCode([FromBody] Tameenk.Core.Domain.Dtos.CheckotDiscountModel model)
        {
            CheckoutDiscountOutput output = new CheckoutDiscountOutput();
            string userId = _authorizationService.GetUserId(User);
            model.Channel = (string.IsNullOrEmpty(model.Channel)) ? Channel.Portal.ToString() : model.Channel;
            try
            {
               output = _checkoutContext.CheckDiscountCode(model, userId);
                if (output.ErrorCode != CheckoutDiscountOutput.ErrorCodes.Success)
                    return Error(output);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                return Error(output);
            }
        }

        [HttpPost]
        [Route("api/checkout/logapplepayerrors")]
        public IHttpActionResult LogApplePayError(string userId, string referenceId, string errorDescription)
        {
            try
            {
                string result = _checkoutContext.LogApplePayError(userId, referenceId, errorDescription);
                return Ok(result);
            }
            catch (Exception ex)
            {
              return Error(CheckoutResources.ErrorGenericApplePay);
            }
        }

        public IHttpActionResult PrePaymentChecks(PrePaymentCheckModel model)
        {
            var output = _checkoutContext.PrePaymentChecks(User.Identity.GetUserId(), model);
            return Ok(output);
        }

        [HttpPost]
        [Route("api/Checkout/addIVRItemToChart")]
        public IHttpActionResult AddIVRItemToChart([FromBody] RenewalAddItemToChartModel model)
        {
            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.Method = "AddIVRItemToChart";
            log.ModuleId = (int)IVRModuleEnum.Renewal;
            log.ModuleName = IVRModuleEnum.Renewal.ToString();
            log.CreatedDate = DateTime.Now;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                var output = _checkoutContext.AddIVRItemToChart(model, log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                CheckoutOutput output = new CheckoutOutput();
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"AddIVRItemToChart Exception, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Ok(output);
            }
        }
    }
}
