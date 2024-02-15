using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Implementation.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// Payment Controller
    /// </summary>
    public class PaymentController : AdminBaseApiController
    {
        #region Fields
        private readonly IPaymentService _paymentService;
        private readonly IUserPageService _userPageService;
        private readonly ICheckoutContext _checkoutContext;
        #endregion


        #region The Ctor

        /// <summary>
        /// Payment Controller
        /// </summary>
        public PaymentController(IPaymentService paymentService, IUserPageService userPageService, ICheckoutContext checkoutContext)
        {
            _paymentService = paymentService ?? throw new TameenkArgumentNullException(nameof(IPaymentService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _checkoutContext = checkoutContext ?? throw new TameenkArgumentNullException(nameof(ICheckoutContext));
        }
        #endregion


        #region Methods 
        /// <summary>
        /// ReProcess Fail Payment
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/payment/fail-reproccess")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 11)]
        public IActionResult ReProcessFailPayment(HyperPayResponseModel hyperPayResponseModel)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Payments";
            log.PageURL = "/admin/payments";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReProcessFailPayment";
            log.ServiceRequest = JsonConvert.SerializeObject(hyperPayResponseModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = hyperPayResponseModel.ReferenceId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(11, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                bool isHyperPayResponseAdded = _paymentService.AddHyperPayResponse(hyperPayResponseModel.ToModel());

                string userName = User.Identity.GetUserName();
                _paymentService.ReProcessFailPayment(hyperPayResponseModel.ReferenceId, userName);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(true);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }


        /// <summary>
        /// Get details Of Fail Payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/payment/fail-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PaymentAdminModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetDetailsOfFailPayment(string referenceId)
        {
            try
            {
                var result = _paymentService.GetDetailsOfFailPayment(referenceId);
                return Ok(result.ToModel());

            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All Payment Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/payment/method")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<Tameenk.Core.Domain.Dtos.PaymentMethodModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllPaymentMethod()
        {
            try
            {
                var result = _paymentService.GetAllPaymentMethod();

                return Ok(result.Select(e => e.ToModel()));
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// get all Fail Payment and pending
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payment/fail-pending")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PaymentAdminModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllFailAndPendingPayment(PaymentFilterModel filter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "InvoiceDate", bool sortOrder = false)
        {
            try
            {
                int count = 0;
                AdminRequestLog log = new AdminRequestLog();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "Payments";
                log.PageURL = "/admin/payments/edaat-notification";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "Getfail-pending";
                log.ServiceRequest = JsonConvert.SerializeObject(filter);
                log.UserID = User.Identity.GetUserId();
                log.UserName = User.Identity.GetUserName();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                var result = _paymentService.GetAllFailAndPendingPayment(filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);
                log.ServiceResponse = result.Count.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result.Select(q => q.ToModel()), count);
            }
            catch (Exception ex)
            {
                AdminRequestLog log = new AdminRequestLog();
                log.MethodName = "Getfail-pending";
                log.ServiceResponse = ex.Message;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return Error("an error has occured");
            }

        }

        #endregion


        [HttpPost]
        [Route("api/payment/edaat-notification")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PaymentAdminModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 53)]
        public IActionResult GetAllEdaatNotificationPayment(EdaatFilterModel filter, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Payments";
            log.PageURL = "/admin/payments/edaat-notification";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllEdaatNotificationPayment";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(53, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                int count = 0;
                string exception = string.Empty;
                var result = _checkoutContext.GetAllEdaatNotificationPayment(filter.ToServiceModel(), out count, out exception, pageIndex, pageSize);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = -1;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result.Select(q => q.ToModel()), count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }
    }
}
