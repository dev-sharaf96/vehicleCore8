using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Models.Payments;
using Tameenk.Resources.Payment;
using Tameenk.Resources.WebResources;
using Tameenk.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Orders;

namespace Tameenk.Controllers
{
    [TameenkAuthorizeAttribute]
    public class TameenkPaymentController : Controller
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IRiyadBankMigsPaymentService _riyadBankMigsPaymentService;
        private readonly TameenkConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly ICheckoutContext checkoutContext;
        private readonly IHyperpayPaymentService hyperpayPaymentService;

        #endregion

        #region Ctor
        public TameenkPaymentController(IOrderService orderService,
            IPaymentMethodService paymentMethodService,
            IRiyadBankMigsPaymentService riyadBankMigsPaymentService,
            TameenkConfig tameenkConfig,
           IHttpClient _httpClient,
           ICheckoutContext checkoutContext,
           IHyperpayPaymentService hyperpayPaymentService)
        {
            _orderService = orderService ?? throw new TameenkArgumentNullException(nameof(IOrderService));
            _paymentMethodService = paymentMethodService ?? throw new TameenkArgumentNullException(nameof(IPaymentMethodService));
            _riyadBankMigsPaymentService = riyadBankMigsPaymentService ?? throw new TameenkArgumentNullException(nameof(IPaymentMethodService));
            _config = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            this._httpClient = _httpClient;
            this.checkoutContext = checkoutContext;
            this.hyperpayPaymentService = hyperpayPaymentService;
        }
        #endregion

        #region Actions

        public ActionResult PaymentMethods(bool showWalletPaymentOption)
        {
            var paymentMethodModel = new PaymentMethodModel();
            var paymentMethods = _paymentMethodService.GetPaymentMethods();

            //if (User.Identity.GetUserId() != null)
            //{
            //    if (User.Identity.GetUserId().ToLower() == "06501157-46af-4639-a85e-f29060e21fce"
            //        || User.Identity.GetUserId().ToLower() == "e9952fe9-5f0d-44bf-9f6f-4e8b789b78b8"
            //        || User.Identity.GetUserId().ToLower() == "d9bd3f6e-b9ba-42bf-a902-2f232cd928ce")
            //    {
            //        paymentMethods.Add(new Core.Domain.Entities.Payments.PaymentMethod()
            //        {
            //            Active = true,
            //            Code = 12,
            //            Id = 12,
            //            Name = "Edaat",
            //            Order = 12,
            //            PaymentMethodCode = Core.Domain.Enums.Payments.PaymentMethodCode.Edaat
            //        });
            //    }
            //}
            paymentMethodModel.PaymentMethodCodes = paymentMethods.Select(pm => pm.PaymentMethodCode).ToList();
            paymentMethodModel.ShowWalletPaymentOption = showWalletPaymentOption;
            return View(paymentMethodModel);
        }

       
        #endregion


        #region HyperPay

        //public ActionResult PaymentUsingHyperpay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode)
        //{
        //    DateTime dtBeforeCalling = DateTime.Now;
        //    DateTime dtAfterCalling = DateTime.Now;
        //    CheckoutRequestLog log = new CheckoutRequestLog();
        //    log.ReferenceId = referenceId;
        //    log.MethodName = "PaymentUsingHyperpay";
        //    log.ServerIP = Utilities.GetInternalServerIP();
        //    log.UserAgent = Utilities.GetUserAgent();
        //    log.UserIP = Utilities.GetUserIPAddress();
        //    log.PaymentMethod = "Hyperpay";
        //    log.Channel = "Portal";
        //    log.UserId = User.Identity.GetUserId();
        //    log.UserName = User.Identity.GetUserName();
        //    string exception = string.Empty;
        //    try
        //    {
        //        // Check if reference id is not set.
        //        if (string.IsNullOrWhiteSpace(referenceId))
        //        {
        //            log.ErrorCode = (int)CheckoutOutput.ErrorCodes.EmptyInputParamter;
        //            log.ErrorDescription = "ReferenceId is null";
        //            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        string clearText = referenceId + "_" + QtRqstExtrnlId + "_" + productId;
        //        if (string.IsNullOrEmpty(selectedProductBenfitId))
        //            clearText += SecurityUtilities.HashKey;
        //        else
        //            clearText += selectedProductBenfitId + SecurityUtilities.HashKey;

        //        if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
        //        {
        //            log.ErrorCode = (int)CheckoutOutput.ErrorCodes.HashedNotMatched;
        //            log.ErrorDescription = "Hashed Not Matched";
        //            dtAfterCalling = DateTime.Now;
        //            log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
        //            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //            return RedirectToAction("ErrorMessage", "Error");
        //        }

        //        var checkoutDetail = _orderService.GetCheckoutDetailByReferenceId(referenceId);
        //        if (checkoutDetail == null)
        //        {

        //            log.ErrorCode = (int)CheckoutOutput.ErrorCodes.EmptyReturnObject;
        //            log.ErrorDescription = "Checkout is null";
        //            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //            return RedirectToAction("CheckoutDetails", "Checkout", new { QtRqstExtrnlId, ReferenceId = referenceId });
        //        }

        //        if (checkoutDetail.Vehicle?.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
        //            log.VehicleId = checkoutDetail.Vehicle?.CustomCardNumber;
        //        else
        //            log.VehicleId = checkoutDetail.Vehicle?.SequenceNumber;

        //        log.DriverNin = checkoutDetail.Driver?.NIN;
        //        log.CompanyId = checkoutDetail.InsuranceCompanyId.Value;
        //        log.CompanyName = checkoutDetail.InsuranceCompany.Key;

        //        // Check if checkout detail is already paid.
        //        if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentSuccess)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        var hyperpayRequest = new HyperpayRequest();
        //        // hyperpayRequest.Currency = _config.Hyperpay.Currency;
        //        //  hyperpayRequest.PaymentType = _config.Hyperpay.PaymentType;
        //        hyperpayRequest.AccessToken = _config.Hyperpay.AccessToken;
        //        hyperpayRequest.EntityId = (paymentMethodCode == (int)PaymentMethodCode.ApplePay) ? _config.Hyperpay.ApplePayEntityId : _config.Hyperpay.EntityId;
        //        hyperpayRequest.ReferenceId = referenceId;
        //        hyperpayRequest.Amount = Math.Round(_orderService.CalculateCheckoutDetailTotal(checkoutDetail), 2);
        //        hyperpayRequest.ReturnUrl = Url.Action("PaymentHyperpayResponse", "TameenkPayment", new { }, Request.Url.Scheme);
        //        hyperpayRequest.CreatedDate = DateTime.Now;
        //        hyperpayRequest.UserId = log.UserId.ToString();
        //        hyperpayRequest.UserEmail = checkoutDetail.Email;
        //        log.Amount = hyperpayRequest.Amount;

        //        hyperpayRequest.CheckoutDetails.Add(checkoutDetail);
        //        HyperpayRequest response = null;
        //         var ouput = hyperpayPaymentService.RequestHyperpayUrlWithSplitOption(hyperpayRequest, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.InsuranceCompanyName, checkoutDetail.Channel, out exception);
        //        if(ouput.ErrorCode== HyperSplitOutput.ErrorCodes.Success&& ouput.HyperpayRequest != null && ouput.HyperpayRequest.ResponseCode == "000.200.100")
        //        {
        //            response = ouput.HyperpayRequest;
        //            TempData["id"] = response.ResponseId;
        //            log.ErrorCode = (int)CheckoutOutput.ErrorCodes.Success;
        //            log.ErrorDescription = "Sucess";
        //            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //            return RedirectToAction("PaymentUsingHyperpay", "Checkout", new { checkoutId = response.ResponseId, referenceId = response.ReferenceId, email = response.UserEmail, isEmailVerified = checkoutDetail.IsEmailVerified, paymentMethod = paymentMethodCode });
        //        }
        //        log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceDown;
        //        log.ErrorDescription = "response.ResponseCode is not success it return " + response.ResponseCode + " sent data is " + exception;
        //        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //        return RedirectToAction("Index", "Error", new { message = RiyadBankResources.InvalidPayment });

        //    }
        //    catch (Exception ex)
        //    {
        //        log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
        //        log.ErrorDescription = ex.ToString() + " exception: " + exception;
        //        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
        //        return RedirectToAction("Index", "Error", new { message = RiyadBankResources.InvalidPayment });
        //    }

        //}

        public ActionResult PaymentHyperpayResponse()
        {
            string id = string.Empty;
            try
            {
                id = Request.QueryString["id"];
                Guid UserId = Guid.Empty;
                if (User != null && User.Identity != null && User.Identity.GetUserId() != null)
                {
                    Guid.TryParse(User.Identity.GetUserId(), out UserId);
                }
                var output = checkoutContext.ProcessHyperpayPayment(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, "Portal", UserId, (int)PaymentMethodCode.Hyperpay);
                
                //for apple pay calculate fees again 
               //if (output.PaymentMethodId == (int)PaymentMethodCode.ApplePay)
               // {
               //     var hyperPayUpdateOrderOutput = checkoutContext.HyperpayUpdateOrder(output.CheckoutDetail, output.HyperpayResponse, output.PaymentId, output.PaymentSucceded);
               // }

                if (output.ErrorCode == HyperPayOutput.ErrorCodes.Success)
                    return RedirectToAction("Purchased", "Checkout", new { referenceId = output.ReferenceId });
                else
                    return RedirectToAction("Index", "Error", new { message = RiyadBankResources.InvalidPayment });
            }
            catch (Exception exp)
            {
                CheckoutRequestLog log = new CheckoutRequestLog();
                log.ReferenceId = id;
                log.MethodName = "HyperpayProcessPayment";
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.UserIP = Utilities.GetUserIPAddress();
                log.PaymentMethod = "Hyperpay";
                log.UserId = User.Identity.GetUserId();
                log.UserName = User.Identity.GetUserName();
                log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = exp.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return RedirectToAction("Index", "Error", new { message = WebResources.SerivceIsCurrentlyDown });
            }
        }

        #endregion

        //public ActionResult GetDeviceInfo()
        //{
        //    var deviceInfo = Utilities.GetDeviceInfo();

        //    if (deviceInfo == null) { return Content("null"); }
        //    return Content($"Client: {deviceInfo.Client}, OS {deviceInfo.OS}");
        //}
    }
}