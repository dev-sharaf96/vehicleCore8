using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Core.Data;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Administration.Identity;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Quotations;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Resources.WebResources;
using System.Globalization;
using Tameenk.Loggin.DAL.DAL;
using Tameenk.Services.Core.BlockNins;
using Tameenk.Services.Core.SMSRenewal;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// Service Controller
    /// </summary>
    public class ServiceController : AdminBaseApiController
    {

        #region Fields

        private readonly IExcelService _excelService;
        private readonly IInsuranceCompanyService insuranceCompanyService;
        private readonly IUserPageService _userPageService;
        private readonly IRepository<Occupation> _occupationRepository;
        private readonly IQuotationService _quotationService;
        #endregion

        #region The Ctor

        /// <summary>
        /// the constructor.
        /// </summary>
        public ServiceController(IExcelService excelService, IInsuranceCompanyService insuranceCompanyService, IUserPageService userPageService,
            IRepository<Occupation> occupationRepository, IQuotationService quotationService)
        {
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
            this.insuranceCompanyService = insuranceCompanyService;
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _occupationRepository = occupationRepository;
            _quotationService = quotationService;
        }

        #endregion


        #region Methods


        /// <summary>
        /// Get All Method in service Request Log
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/service/methods-all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<string>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllMethodInServiceRequestLog()
        {
            try
            {
                return Ok(ServiceRequestLogDataAccess.GetAllMethodInServiceRequestLogNew());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All Service Request Log based on filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/all")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ServiceRequestModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllServiceRequestLogBasedOnFilter(ServiceRequestFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)
        {

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Requests";
            log.PageURL = "/admin/requests-new";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllServiceRequestLogBasedOnFilterNew";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(39, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string connectionString = "TameenkLogOld";
                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("");
                }
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out int count, pageIndex, pageSize, sortField, sortOrder);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToModel()), count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }
        /// <summary>
        /// Export All service request
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllServiceRequest(ServiceRequestFilterModel filter)
        {
            try
            {
                string connectionString = "TameenkLogOld";
                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("");
                }
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out int count, 0, 50000, "ID", true);

                if (result == null)
                    return Ok("");
                byte[] file = _excelService.GenerateServiceRequest(result, "Requests");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("No File");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }
        #region Checkout Request Log        /// <summary>        /// Get All Checkout Request Log based on filter        /// </summary>        /// <returns></returns>        [HttpPost]        [Route("api/service/getAllCheckoutRequestLogBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<CheckoutRequestLogModel>>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetAllCheckoutRequestLogBasedOnFilter(RequestLogsFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)        {            try            {                int count = 0;                string connectionString = "TameenkLog";                List<CheckoutRequestLog> result = null;                result = CheckoutRequestLogDataAccess.GetAllCheckoutRequestLogBasedOnFilter(connectionString, filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);

                if (result == null)                    return Ok(new List<CheckoutRequestLog>().Select(res => res.ToModel()));                return Ok(result.Select(res => res.ToModel()), count);            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }









        /// <summary>        /// Get Checkout Request Log Details By Id        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/service/getCheckoutRequestLogDetails")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CheckoutRequestLogModel>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetCheckoutRequestLogDetails(int id)        {            try            {                string connectionString = "TameenkLog";                CheckoutRequestLog result = null;                result = CheckoutRequestLogDataAccess.GetCheckoutRequestLogDetails(connectionString, id);                if (result == null)                    return Ok("");                return Ok(result.ToModel());            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }

        /// <summary>
        /// Get All Checkout Request Log Methods
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/service/all-checkoutRequestLog-methods")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<string>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllCheckoutRequestLogMethods()
        {
            try
            {
                var list = new List<string>() {
                    "ActivateUserEmailToReceivePolicy",
                    "AddAutoleasingItemToCart",
                    "AddItemToCart",
                    "ApplePayProcessPayment",
                    "AutoleasingDraftPolicy",
                    "AutoleasingSubmitCheckoutDetails",
                    "CheckoutDetails",
                    "GetTotalVehicleSadadRequestsPerDay",
                    "HyperpayProcessPayment",
                    "HyperpayProcessPaymentByRetrialMechanism",
                    "HyperpayUpdateOrder",
                    "NationalAddress",
                    "PaymentUsingApplePay",
                    "PaymentUsingHyperpay",
                    "PaymentUsingSTCPay",
                    "PhoneVerification",
                    "ResendVerifyCode",
                    "SendActivationEmail",
                    "SendWataniyaDraftpolicy",
                    "SubmitCheckoutDetails",
                    "UserHasActivePolicy",
                    "ValidateNumberOfAccident",
                    "ValidateProductPrice",
                    "ValidateTawuniyaAutoleasingQuotation",
                    "ValidateTawuniyaQuotation",
                    "VerifyCheckoutInfo",
                    "WalletUpdateOrder"
                };

                return Ok(list, list.Count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// export Checkout request logs
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/excel-checkout-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportCheckoutRequestLog(RequestLogsFilterModel filter)
        {
            try
            {
                int count = 0;
                string connectionString = "TameenkLog";                var result = CheckoutRequestLogDataAccess.GetAllCheckoutRequestLogBasedOnFilter(connectionString, filter.ToServiceModel(), out count);
                if (result == null)
                    return Ok("");
                byte[] file = _excelService.ExportCheckoutRequestLog(result, "Checkout Requests");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }












        #endregion
        #region Inquiry Request Log        /// <summary>                                            /// Get All Inquiry Request Log based on filter                                            /// </summary>                                            /// <returns></returns>        [HttpPost]        [Route("api/service/getAllInquiryRequestLogBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<CheckoutRequestLogModel>>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetAllInquiryRequestLogBasedOnFilter(RequestLogsFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)        {            try            {                int count = 0;                string connectionString = "TameenkLog";                List<InquiryRequestLog> result = null;                result = InquiryRequestLogDataAccess.GetAllInquiryRequestLogBasedOnFilter(connectionString, filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);                if (result == null)                    return Ok(new List<InquiryRequestLog>().Select(res => res.ToModel()));                return Ok(result.Select(res => res.ToModel()), count);            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }


        /// <summary>
        /// export init inquiry request logs
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/excel-inquiry-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportInquiryRequestLog(RequestLogsFilterModel filter)
        {
            try
            {
                int count = 0;
                string connectionString = "TameenkLog";                List<InquiryRequestLog> result = null;                result = InquiryRequestLogDataAccess.GetAllInquiryRequestLogBasedOnFilter(connectionString, filter.ToServiceModel(), out count, 0, 50000);
                if (result == null)
                    return Ok("");
                byte[] file = _excelService.ExportInquiryRequestLog(result, "Inquiry Requests");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

















        /// <summary>        /// Get Inquiry Request Log Details By Id        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/service/getInquiryRequestLogDetails")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryRequestLogModel>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetInquiryRequestLogDetails(int id)        {            try            {                string connectionString = "TameenkLog";                InquiryRequestLog result = null;                result = InquiryRequestLogDataAccess.GetInquiryRequestLogDetails(connectionString, id);                if (result == null)                    return Ok("");                return Ok(result.ToModel());            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }













        #endregion
        #region Quotation Request Log        /// <summary>                                              /// Get All Quotation Request Log based on filter                                              /// </summary>                                              /// <returns></returns>        [HttpPost]        [Route("api/service/getAllQuotationRequestLogBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<QuotationRequestLogModel>>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetAllQuotationRequestLogBasedOnFilter(RequestLogsFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)        {            try            {                int count = 0;                string connectionString = "TameenkLog";                List<QuotationRequestLog> result = null;                result = QuotationRequestLogDataAccess.GetAllQuotationRequestLogBasedOnFilter(connectionString, filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);                if (result == null)                    return Ok(new List<QuotationRequestLog>().Select(res => res.ToModel()));                return Ok(result.Select(res => res.ToModel()), count);            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }








        /// <summary>        /// Get Quotation Request Log Details By Id        /// </summary>        /// <returns></returns>        [HttpGet]        [Route("api/service/getQuotationRequestLogDetails")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<QuotationRequestLogModel>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetQuotationRequestLogDetails(int id)        {            try            {                string connectionString = "TameenkLog";                QuotationRequestLog result = null;                result = QuotationRequestLogDataAccess.GetQuotationRequestLogDetails(connectionString, id);                if (result == null)                    return Ok("");                return Ok(result.ToModel());            }            catch (Exception ex)            {                ErrorLogger.LogError(ex.Message, ex, false);                return Error("an error has occured");            }        }        #endregion

        /// <summary>
        /// Get All SMS Log Methods
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/service/all-sms-methods")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<string>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllSMSMethods()
        {
            try
            {
                var list = Enum.GetNames(typeof(SMSMethod)).ToList();
                if (list == null)
                    return Ok("");

                return Ok(list, list.Count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpPost]        [Route("api/service/getAllSMSLogBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<SMSLogModel>>))]        [AdminAuthorizeAttribute(pageNumber: 0)]        public IActionResult GetAllSMSLogBasedOnFilter(SMSLogFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)        {            try            {                int count = 0;                string exception = string.Empty;                string connectionString = "TameenkLogLive";
                List<SMSLog> result = null;                if (filter == null)
                    return Error("Filteration Model is empty, please add values to filter");                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Error("Start date must be less than End date");
                }                var model = filter.ToModel();                if (model == null)
                    return Error("Error happend when converting to ToModel()");                result = SMSLogsDataAccess.GetAllSMSLogBasedOnFilter(connectionString, model, out count, out exception, false, pageIndex, pageSize, sortField, sortOrder);                if (!string.IsNullOrEmpty(exception))
                    return Error(exception);                if (result == null)
                    return Ok(new List<SMSLog>().Select(res => res.ToModel()));
                return Ok(result.Select(res => res.ToModel()), count);            }            catch (Exception ex)            {
                return Error("an error has occured");            }        }

        /// <summary>
        /// export all sms logs to excel file
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/sms-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllSmsServiceLogs(SMSLogFilterModel filter)
        {
            try
            {
                int count = 0;                string exception = string.Empty;                string connectionString = "TameenkLogLive";
                List<SMSLog> result = null;

                if (filter == null)
                    return Error("Filteration Model is empty, please add values to filter");
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Error("Start date must be less than End date");
                }

                var model = filter.ToModel();                if (model == null)
                    return Error("Error happend when converting to ToModel()");

                result = SMSLogsDataAccess.GetAllSMSLogBasedOnFilter(connectionString, model, out count, out exception, true, 0, 0, null, false);                if (!string.IsNullOrEmpty(exception))
                    return Error(exception);                if (result == null)
                    return Ok("");
                byte[] file = _excelService.GenerateSmsServiceLogExcel(result);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        #region New Services


        /// <summary>
        /// Get All Method in service Request Log
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/service/methods-all-new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<string>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllMethodInServiceRequestLogNew()
        {
            try
            {
                return Ok(ServiceRequestLogDataAccess.GetAllMethodInServiceRequestLogNew());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All Service Request Log based on filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/all-new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ServiceRequestModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 39)]
        public IActionResult GetAllServiceRequestLogBasedOnFilterNew(ServiceRequestFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Requests";
            log.PageURL = "/admin/requests-new";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllServiceRequestLogBasedOnFilterNew";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(39, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string connectionString = "TameenkLogLive";
                
                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    //DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    //DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (filter.StartDate > filter.EndDate)
                        return Ok("");
                }

                int count;
                if (!string.IsNullOrEmpty(filter.Method) && filter.Method.ToLower() == ("TabbyPaymentWebHook").ToLower())
                {
                    var output = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterForTabby(connectionString, companyKey, filter.ToServiceModel(), out count, pageIndex, pageSize);

                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output, count);
                }

                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToModel()), count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }


        /// <summary>
        /// Export All service request
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/excel-new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 39)]
        public IActionResult ExportAllServiceRequestNew(ServiceRequestFilterModel filter)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Requests";
            log.PageURL = "/admin/requests-new";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ExportAllServiceRequestNew";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(39, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                string connectionString = "TameenkLogLive";
                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Error("Start date is greater than end date");
                }

                int count;
                if (!string.IsNullOrEmpty(filter.Method) && filter.Method.ToLower() == ("TabbyPaymentWebHook").ToLower())
                {
                    var output = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterForTabby(connectionString, companyKey, filter.ToServiceModel(), out count, 1, 5000);
                    if (output == null)
                    {
                        log.ErrorCode = 2;
                        log.ErrorDescription = "Data is null";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok("Data is null");
                    }

                    byte[] outputFile = _excelService.GenerateServiceRequestForTabbby(output, "Requests");
                    if (outputFile != null && outputFile.Length > 0)
                    {
                        log.ErrorCode = 1;
                        log.ErrorDescription = "success";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(Convert.ToBase64String(outputFile));
                    }
                    else
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "file is null";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok("file is null");
                    }
                }

                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out count, 0, 50000, "ID", true);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (result == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Data is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok("Data is null");
                }

                byte[] file = _excelService.GenerateServiceRequest(result, "Requests");
                if (file != null && file.Length > 0)
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Convert.ToBase64String(file));
                }
                else
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "file is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok("file is null");
                }
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Get All Service Request Log (new log) based on filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/all-service-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ServiceRequestModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllServiceRequestLogWithOnFilterNew(ServiceRequestFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)
        {
            try
            {
                string connectionString = "TameenkLogLive";
                List<ServiceRequestLog> result = new List<ServiceRequestLog>();
                string companyKey = "";

                if (filter.InsuranceCompanyId.HasValue)
                {
                    var insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Ok("start date greater than end date");
                }
                string exception = string.Empty;
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogWithOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out int count, out exception, pageIndex, pageSize, sortField, sortOrder);

                if (result == null || !string.IsNullOrEmpty(exception))
                    return Error(exception);

                return Ok(result.Select(res => res.ToModel()), count);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Export All service request
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/excel-service-request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllNewServiceRequest(ServiceRequestFilterModel filter)
        {
            try
            {
                string connectionString = "TameenkLogLive";
                List<ServiceRequestLog> result = new List<ServiceRequestLog>();
                string companyKey = "";
                if (filter.InsuranceCompanyId.HasValue)
                {
                    var insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Error("start date greater than end date");
                }
                string exception = string.Empty;
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogWithOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out int count, out exception, 0, 0, null, false, true);

                if (result == null || !string.IsNullOrEmpty(exception))
                    return Error(exception);
                byte[] file = _excelService.GenerateServiceRequest(result, "Services Requests");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Error("file is null");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }


        #endregion


        #region Occupations

        [HttpGet]
        [Route("api/service/occupations-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<OccupationModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetOccupations(string code, string description, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = GetOccupationsWithFilter(out int count, false, code, description, pageIndex, pageSize);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToOccupationModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpGet]
        [Route("api/service/occupations-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllOccupations(string code, string description)
        {
            try
            {
                if (!string.IsNullOrEmpty(description) && description.Contains("\n"))
                {
                    var desc = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    description = desc;
                }

                var result = GetOccupationsWithFilter(out int count, true, code, description, 0, int.MaxValue);

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.ExportOccupations(result, "Occupations");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        public List<Occupation> GetOccupationsWithFilter(out int total, bool export, string code, string description, int pageIndex, int pageSize)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllOccupationsWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                if (!string.IsNullOrEmpty(code))
                {
                    SqlParameter CodeParameter = new SqlParameter() { ParameterName = "code", Value = code };
                    command.Parameters.Add(CodeParameter);
                }

                if (!string.IsNullOrEmpty(description))
                {
                    SqlParameter DescriptionParameter = new SqlParameter() { ParameterName = "description", Value = description ?? "" };
                    command.Parameters.Add(DescriptionParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<Occupation> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Occupation>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        [HttpGet]
        [Route("api/service/occupation-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<OccupationModel>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetOccupationDetails(int id)
        {
            try
            {
                if (id <= 0)
                    throw new TameenkArgumentNullException("Occupation Id");

                var occupation = _occupationRepository.TableNoTracking.FirstOrDefault(a => a.ID == id);
                if (occupation == null)
                    return Ok("");

                return Ok(occupation.ToOccupationModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/service/occupation-checkOccupationCodeExist")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult CheckOccupationCode([FromBody]OccupationModel model)
        {
            try
            {
                var occupation = _occupationRepository.TableNoTracking.FirstOrDefault(a => a.Code == model.Code);
                if (occupation != null)
                    return Ok(true);

                return Ok(false);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/service/occupation-addOrUpdateOccupation")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult AddOrUpdateOccupation([FromBody]OccupationModel model, string action)
        {
            var outPut = new PolicyOutput();
            try
            {
                if (action == "add")
                {
                    var occupationModel = new Occupation()
                    {
                        Code = model.Code,
                        NameAr = model.NameAr,
                        NameEn = model.NameEn,
                        IsCitizen = false,
                        IsMale = false
                    };
                    _occupationRepository.Insert(occupationModel);
                }
                else
                {
                    var oldOccupation = _occupationRepository.Table.FirstOrDefault(a => a.ID == model.ID);
                    oldOccupation.Code = model.Code;
                    oldOccupation.NameAr = model.NameAr;
                    oldOccupation.NameEn = model.NameEn;
                    _occupationRepository.Update(oldOccupation);
                }

                outPut.ErrorCode = PolicyOutput.ErrorCodes.Success;
                outPut.ErrorDescription = "success";
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                outPut.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                outPut.ErrorDescription = "an error has occured";
                return Error(outPut);
            }
        }


        #endregion

        #region AvgResponseTime
        //AvgResponseTime 
        [HttpPost]
        [Route("api/service/GetTheAVGServiceRequestResponseTime")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<ServiceRequestResponseTimeFromDBModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 39)]
        public IActionResult GetAVGServiceRequestResponseTime([FromBody] ServiceResponseTimeFilterModel responsetimefilter, int pageIndex = 0, int pageSize = 10, string sortField = "Company Name", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "GetAVGServiceRequestResponseTime";
            log.PageURL = "/admin/GetAVGServiceRequestResponseTime";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAVGServiceRequestResponseTime";
            log.ServiceRequest = JsonConvert.SerializeObject(responsetimefilter);
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (responsetimefilter == null)
                    throw new TameenkArgumentNullException("responsetimefilter");


                string exception = string.Empty;                var result = QuotationResponseLogDataAccess.ServiceRequestResponseTimeFromDBFilter(responsetimefilter, 240, out int count, out exception, pageIndex, pageSize, sortField, sortOrder);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error --> " + exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        #endregion

        #region GetRequests Per Company
        [HttpPost]
        [Route("api/service/GetServiceRequestPerCompany")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<RequestsPerCompanyModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 60)]
        public IActionResult GetServiceRequestsPerCompany([FromBody] ServiceRequestsPerCompany RequestFilter, int pageIndex = 0, int pageSize = 10, string sortField = "Company Name", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "GetServiceRequestPerCompany";
            log.PageURL = "/admin/Request-Per-Comany/Request-Per-Comany";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetServiceRequestPerCompany";
            log.ServiceRequest = JsonConvert.SerializeObject(RequestFilter);

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (RequestFilter == null)
                    throw new TameenkArgumentNullException("responsetimefilter");

                string exception = string.Empty;
                var result = QuotationRequestLogDataAccess.ServiceRequestsFromDBWithFilter(RequestFilter, out exception)?.OrderByDescending(a => a.CountOfRequest);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error --> " + exception);
                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }

                var AllRequests = new List<RequestsPerCompanyModel>();
                foreach (var item in result)
                {
                    var singleModel = new RequestsPerCompanyModel()
                    {
                        CompanyName = item.CompanyName,
                        CountOfRequest = item.CountOfRequest
                    };
                    AllRequests.Add(singleModel);
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(AllRequests/*, count*/);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        #endregion

        #region Get All Policy Statistics Report
        [HttpPost]
        [Route("api/service/GetAllPolicyStatistics")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyStatisticsDataModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 76)]
        public IActionResult GetAllPolicyStatistics([FromBody] PolicyStatisticsFilterModel RequestFilter)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            PolicyStatisticsOutput Output = new PolicyStatisticsOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "GetAllPolicyStatistics";
            log.PageURL = "/admin/GetAllPolicyStatistics";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllPolicyStatistics";
            log.ServiceRequest = JsonConvert.SerializeObject(RequestFilter);
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(76, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                int totalCount = 0;
                var result = QuotationRequestLogDataAccess.GetPolicyStatisticsData(RequestFilter, out exception, out totalCount);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                    return Error(Output);
                }

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while delete discount, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (RequestFilter.IsExcel.HasValue && RequestFilter.IsExcel.Value == 1)
                {
                    byte[] file = _excelService.GenerateExcelPoliciesStatisticsInfo(result);
                    if (file != null && file.Length > 0)
                        return Ok(Convert.ToBase64String(file));
                    else
                        return Ok("");
                }

                Output.Result = result;
                Output.TotalCount = totalCount;
                Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = PolicyStatisticsOutput.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(RequestFilter.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }
        #endregion
        #endregion

        #region Get Old Quotation Log Details

        [HttpPost]
        [Route("api/service/get-old-quotation-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyStatisticsDataModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 84)]
        public IActionResult GetOldQuotationDetails([FromBody] OldQuotationDetailsFilter model)
        {
            AdministrationOutput<OldQuotationDetailsOutput> Output = new AdministrationOutput<OldQuotationDetailsOutput>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "OldQuotationDetails";            log.PageURL = "/get-old-quotation-details";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetOldQuotationDetails";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(84, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (string.IsNullOrEmpty(model.NationalId) && string.IsNullOrEmpty(model.referenceNo) && string.IsNullOrEmpty(model.SequenceNumber))
                {
                    Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.EmptyInputParamter;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "National Id or Sequence or Reference number is Null" + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);
                }                string exception = string.Empty;
                int totalcount = 0;
                OldQuotationDetailsOutput result = new OldQuotationDetailsOutput();
                var oldQuotationDetails = _quotationService.GetOldQuotationDetails(model, out totalcount, out exception);

                if (model.isExport == true)
                {
                    byte[] file = _excelService.GenearetOldQuotation(oldQuotationDetails);
                    if (file != null && file.Length > 0)
                        return Ok(Convert.ToBase64String(file));
                }

                result.Data = oldQuotationDetails;
                result.TotalCount = totalcount;
                DateTime dtBeforeCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                Output.Result = result;                Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<OldQuotationDetailsOutput>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }
        }













        #endregion


        #region BlockNin 

        /// <summary>        /// Add Blocked User        /// </summary>        /// <param name="model">BlockedUser Model</param>        /// <returns></returns>        [HttpPost]        [AllowAnonymous]        [Route("api/service/addBlockedNin")]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]        [AdminAuthorizeAttribute(pageNumber: 94)]        public IActionResult AddBlockedUser([FromBody] AddBlockedNinModel model)        {            var output = new AdministrationOutput<bool>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "addBlockedNin";            log.PageURL = "/service/AddBlockedNin";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "AddBlockedNin";            log.ServiceRequest = JsonConvert.SerializeObject(model);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            log.DriverNin = model.NationalId;            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))                {                    log.ErrorCode = 3;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error("User not authorized");                }                if (!User.Identity.IsAuthenticated)                {                    log.ErrorCode = 2;                    log.ErrorDescription = "User not authenticated";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error("User not authenticated");                }                var isAuthorized = _userPageService.IsAuthorizedUser(94, log.UserID);                if (!isAuthorized)                {                    log.ErrorCode = 10;                    log.ErrorDescription = "User not authenticated : " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error("Not Authorized : " + log.UserID);                }                if (model == null)                {                    output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NullResult;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = 9;                    log.ErrorDescription = "Invalid result";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (string.IsNullOrEmpty(model.NationalId))                {                    output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NullResult;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = 9;                    log.ErrorDescription = "NationaL Id  is null ";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }
                if (string.IsNullOrEmpty(model.BlockReason))                {                    output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NullResult;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = 9;                    log.ErrorDescription = "Block Reason Id  is null ";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                string exption = string.Empty;                bool response = _quotationService.AddBlockedUser(model, log.UserName, out exption);                if (!string.IsNullOrEmpty(exption) || !response)                {                    output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotFound;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = 5;                    log.ErrorDescription = " Exception error as : " + exption;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));                log.ErrorCode = 1;                log.ErrorDescription = "Success";                return Ok(output);            }            catch (Exception ex)            {                output.ErrorDescription = WebResources.ResourceManager.GetString("Exception", CultureInfo.GetCultureInfo(model.Lang));                output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;                log.ErrorCode = 4;                log.ErrorDescription = "exception error as Error is  : " + ex;                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(output);            }        }
        
        /// <summary>        /// Get All Service Blocked Users based on filter        /// </summary>        /// <returns></returns>        [HttpPost]        [Route("api/service/getBlockNinsBasedOnFilter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<BlockedNinsListModel>>))]        [AdminAuthorizeAttribute(pageNumber: 94)]        public IActionResult GetBlockUsersListBasedOnFilter(BlockedNinFilter filtration)        {            DateTime dtBeforeCalling = DateTime.Now;            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "getBlockNinsBasedOnFilter";            log.PageURL = "/service/getBlockNinsBasedOnFilter";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "getBlockNinsBasedOnFilter";            log.ServiceRequest = JsonConvert.SerializeObject(filtration);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            var output = new AdministrationOutput<BlockedNinsDataOutput>();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    log.ErrorCode = 3;                    log.ErrorDescription = "User not authorized";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filtration.Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("User not authorized");                }                if (!User.Identity.IsAuthenticated)                {                    log.ErrorCode = 2;                    log.ErrorDescription = "User not authenticated";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filtration.Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("User not authenticated");                }                var isAuthorized = _userPageService.IsAuthorizedUser(94, log.UserID);                if (!isAuthorized)                {                    log.ErrorCode = 10;                    log.ErrorDescription = "User not authenticated : " + log.UserID;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filtration.Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("Not Authorized : " + log.UserID);                }                string exception = string.Empty;                int totalcount = 0;                BlockedNinsDataOutput result = new BlockedNinsDataOutput();                result.Data = _quotationService.GetQuotationBlockFilterService(filtration, out totalcount, out exception);                if (filtration.IsExport == true)                {                    result.ExcelSheet = _excelService.GenearetBlockedUsers(result.Data);                    if (result.ExcelSheet != null && result.ExcelSheet.Length > 0)                        return Ok(Convert.ToBase64String(result.ExcelSheet));                }                result.TotalCount = totalcount;                if (!string.IsNullOrEmpty(exception) || result.Data == null)                {                    output.ErrorCode = AdministrationOutput<BlockedNinsDataOutput>.ErrorCodes.NullResult;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(filtration.Lang));                    log.ErrorCode = 5;                    log.ErrorDescription = exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                log.ServiceResponse = result.Data.ToString();                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                log.ErrorCode = 1;                log.ErrorDescription = "Success";                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filtration.Lang));

                return Ok(result);            }            catch (Exception ex)            {                log.ErrorCode = 400;                log.ErrorDescription = ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                output.ErrorDescription = WebResources.ResourceManager.GetString("ExceptionError", CultureInfo.GetCultureInfo(filtration.Lang));                return Ok(log);            }        }        [HttpPost]        [Route("api/service/deleteBlockedNin")]        [AdminAuthorizeAttribute(pageNumber: 94)]        public IActionResult DeleteBlockedNin(int Id, string Lang)        {            DateTime dtBeforeCalling = DateTime.Now;            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "BlockUsers";            log.PageURL = "/service/deleteBlockedNin";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "deleteBlockedNin";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            var output = new AdministrationOutput<BlockedNinsDataOutput>();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    log.ErrorCode = 3;                    log.ErrorDescription = "User not authorized";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("User not authorized");                }                if (!User.Identity.IsAuthenticated)                {                    log.ErrorCode = 2;                    log.ErrorDescription = "User not authenticated";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("User not authenticated");                }                var isAuthorized = _userPageService.IsAuthorizedUser(94, log.UserID);                if (!isAuthorized)                {                    log.ErrorCode = 10;                    log.ErrorDescription = "User not authenticated : " + log.UserID;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(Lang));                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("Not Authorized : " + log.UserID);                }                string exception = string.Empty;
                string nin = string.Empty;
                bool result = _quotationService.RemoveBlockNin(Id,out nin, out exception);                if (!string.IsNullOrEmpty(exception) || !result)                {                    output.ErrorCode = AdministrationOutput<BlockedNinsDataOutput>.ErrorCodes.NullResult;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(Lang));                    log.ErrorCode = 5;                    log.ErrorDescription = exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                log.DriverNin = nin;                log.ServiceResponse = "sucess";                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                log.ErrorCode = 1;                log.ErrorDescription = "Success";                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(Lang));                output.ErrorCode = output.ErrorCode = AdministrationOutput<BlockedNinsDataOutput>.ErrorCodes.Success; ;                return Ok(output);            }            catch (Exception ex)            {                log.ErrorCode = 400;                log.ErrorDescription = ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                output.ErrorDescription = WebResources.ResourceManager.GetString("ExceptionError", CultureInfo.GetCultureInfo(Lang));                return Ok(log);            }        }        #endregion

     
        #region getAllSMSRenewalLogBasedOnFilter
         /// <summary>
        /// GetAllSMSRenewalLogBasedOnFilter
        /// </summary>
        [HttpPost]  [Route("api/service/getAllSMSRenewalLogBasedOnFilter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<AllTypeSMSRenewalLogModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllSMSRenewalLogBasedOnFilter(SMSRenewalLogFilterModel filter)
        {
            DateTime dtBeforeCalling = DateTime.Now;            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "sms-renewal-log";            log.PageURL = "/requestLogs/sms-renewal-log";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetAllSMSRenewalLogBasedOnFilter";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            var output = new AdministrationOutput<SmsRenewalOutPut>();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    log.ErrorCode = 3;                    log.ErrorDescription = "User not authorized";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filter.Language));                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NotAuthorized;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (!User.Identity.IsAuthenticated)                {                    log.ErrorCode = 9;                    log.ErrorDescription = "User not authenticated";                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NotAuthorized;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok("User not authenticated");                }                var isAuthorized = _userPageService.IsAuthorizedUser(94, log.UserID);                if (!isAuthorized)                {                    log.ErrorCode = 4;                    log.ErrorDescription = "User not authenticated : " + log.UserID;                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NotAuthorized;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }
                if (filter == null)
                {
                    log.ErrorCode = 5;                    log.ErrorDescription = "Filteration Model is empty, please add values to filter";                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.EmptyInputParamter;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                    {
                        log.ErrorCode = 2;
                        log.ErrorDescription = "Start date must be less than End date";
                        output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(filter.Language));
                        output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NotFound;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(output);
                    }
                }
                string exception = string.Empty; 
                string connectionString = "TameenkLogLive";
                AllTypeSMSRenewalLogModel result = null;
                var model = filter.ToModel();
                if (model == null)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "Error happend when Mapping";
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NotFound;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                result = SMSLogsDataAccess.GetAllSMSRenewalLogBasedOnFilter(connectionString, model, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.ServiceException;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (result == null)
                {
                    log.ErrorCode = 13;
                    log.ErrorDescription = exception;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(filter.Language));
                    output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.NullResult;
                    output.Result = null;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                log.ErrorCode = 1;
                log.ErrorDescription = exception;
                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filter.Language));
                output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.Success;
                output.Result = new SmsRenewalOutPut();
                output.Result.Result = result;
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 14;
                log.ErrorDescription = ex.ToString();
                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filter.Language));
                output.ErrorCode = AdministrationOutput<SmsRenewalOutPut>.ErrorCodes.ServiceDown;
                output.Result = null;
                return Ok(output);
            }
        }
        #endregion



        #region App notifications

        [HttpPost]        [Route("api/service/getAllAppNotificationLogLogBasedOnFilter")]        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<SMSLogModel>>))]        [AdminAuthorizeAttribute(pageNumber: 99)]        public IActionResult GetAllAppNotificationLogBasedOnFilter(Loggin.DAL.AppNotificationsLogFilterModel filter, int pageIndex = 0, int pageSize = 10)        {            var output = new AdministrationOutput<List<FirebaseNotificationLog>>();            output.Result = new List<FirebaseNotificationLog>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "AppNotifications";            log.PageURL = "/service/getAllAppNotificationLogLogBasedOnFilter";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "appNotifications";            log.ServiceRequest = JsonConvert.SerializeObject(filter);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))                {                    output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "Utilities.IsBlockedUser: " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (!User.Identity.IsAuthenticated)                {                    output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "User.Identity.IsAuthenticated : " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(99, log.UserID);                if (!isAuthorized)                {                    output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "isAuthorized is false: " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (filter == null)                {                    output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NullResult;                    output.ErrorDescription = "Filteration Model is empty, please add values to filter";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                    {
                        output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NotAuthorized;
                        output.ErrorDescription = "Start date must be less than End date";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(output);
                    }
                }                int count = 0;                string exception = string.Empty;                string connectionString = "TameenkLogLive";                List<FirebaseNotificationLog> result = ServiceRequestLogDataAccess.GetAllAppNotificationLogBasedOnFilter(connectionString, filter, out count, out exception, pageIndex, pageSize);                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = " Service exception has occured";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result = result;                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(output, count);            }            catch (Exception ex)            {
                output.ErrorCode = AdministrationOutput<List<FirebaseNotificationLog>>.ErrorCodes.ExceptionError;
                output.ErrorDescription = "Exception has occured";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);            }        }

        /// <summary>
        /// export all App Notification logs to excel file
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/service/appNotification-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult ExportAllAppNotificationServiceLogs(Loggin.DAL.AppNotificationsLogFilterModel filter)
        {
            var output = new AdministrationOutput<string>();            output.Result = string.Empty;            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "AppNotifications";            log.PageURL = "/service/getAllAppNotificationLogLogBasedOnFilter";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "appNotificationsExport";            log.ServiceRequest = JsonConvert.SerializeObject(filter);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, Utilities.GetUserAgent()))                {                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "Utilities.IsBlockedUser: " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (!User.Identity.IsAuthenticated)                {                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "User.Identity.IsAuthenticated : " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(99, log.UserID);                if (!isAuthorized)                {                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NotAuthorized;                    output.ErrorDescription = "User not authenticated";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "isAuthorized is false: " + log.UserID;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (filter == null)                {                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NullResult;                    output.ErrorDescription = "Filteration Model is empty, please add values to filter";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(output);                }                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                    {
                        output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NotAuthorized;
                        output.ErrorDescription = "Start date must be less than End date";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(output);
                    }
                }

                int count = 0;                string exception = string.Empty;                string connectionString = "TameenkLogLive";                List<FirebaseNotificationLog> result = ServiceRequestLogDataAccess.GetAllAppNotificationLogBasedOnFilter(connectionString, filter, out count, out exception, 0, 0);                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = " Service exception has occured";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                byte[] file = _excelService.GenerateAppnotificationsServiceLogExcel(result, "App Notification Logs");
                if (file == null && file.Length <= 0)
                {
                    output.ErrorCode = AdministrationOutput<string>.ErrorCodes.NullResult;
                    output.ErrorDescription = "file is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                output.Result = Convert.ToBase64String(file);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = AdministrationOutput<string>.ErrorCodes.ExceptionError;
                output.ErrorDescription = "Exception has occured";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
        }

        #endregion

        #region Old Logs After Create New DataBase

        [HttpPost]
        [Route("api/service/logs-old")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<ServiceRequestModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 100)]
        public IActionResult GetAllServiceRequestLogBasedOnFilterFromOldDB(ServiceRequestFilterModel filter, int pageIndex = 0, int pageSize = 10, string sortField = "Id", bool sortOrder = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Requests";
            log.PageURL = "/admin/logs-old";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllServiceRequestLogBasedOnFilterFromOldDB";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(100, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string connectionString = "TameenkLogLive_OldLogs";

                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    //DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    //DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (filter.StartDate > filter.EndDate)
                        return Ok("");
                }

                int count;
                if (!string.IsNullOrEmpty(filter.Method) && filter.Method.ToLower() == ("TabbyPaymentWebHook").ToLower())
                {
                    var output = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterForTabby(connectionString, companyKey, filter.ToServiceModel(), out count, pageIndex, pageSize);

                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output, count);
                }

                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out count, pageIndex, pageSize, sortField, sortOrder);

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToModel()), count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/service/logs-old-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 100)]
        public IActionResult ExportAllServiceRequestLogBasedOnFilterFromOldDB(ServiceRequestFilterModel filter)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Requests";
            log.PageURL = "/admin/logs-old";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ExportAllServiceRequestLogBasedOnFilterFromOldDB";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(100, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                string connectionString = "TameenkLogLive_OldLogs";
                List<IServiceRequestLog> result = new List<IServiceRequestLog>();
                string companyKey = "";
                InsuranceCompany insuranceCompany = null;
                if (string.IsNullOrEmpty(filter.Method))
                    return Error("Please select service");
                if ((filter.Method.ToLower() == "quotation" || filter.Method.ToLower() == "policy" || filter.Method.ToLower() == "proposal") && !filter.InsuranceCompanyId.HasValue)
                    return Error("Please select insurance company");

                if (filter.InsuranceCompanyId.HasValue)
                {
                    insuranceCompany = insuranceCompanyService.GetById(filter.InsuranceCompanyId.Value);
                    if (insuranceCompany != null)
                        companyKey = insuranceCompany.Key;
                }
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                        return Error("Start date is greater than end date");
                }

                int count;
                if (!string.IsNullOrEmpty(filter.Method) && filter.Method.ToLower() == ("TabbyPaymentWebHook").ToLower())
                {
                    var output = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterForTabby(connectionString, companyKey, filter.ToServiceModel(), out count, 1, 5000);
                    if (output == null)
                    {
                        log.ErrorCode = 2;
                        log.ErrorDescription = "Data is null";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok("Data is null");
                    }

                    byte[] outputFile = _excelService.GenerateServiceRequestForTabbby(output, "Requests");
                    if (outputFile != null && outputFile.Length > 0)
                    {
                        log.ErrorCode = 1;
                        log.ErrorDescription = "success";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(Convert.ToBase64String(outputFile));
                    }
                    else
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "file is null";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok("file is null");
                    }
                }

                result = ServiceRequestLogDataAccess.GetAllServiceRequestLogBasedOnFilterNew(connectionString, companyKey, filter.ToServiceModel(), out count, 0, 50000, "ID", true);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (result == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Data is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok("Data is null");
                }

                byte[] file = _excelService.GenerateServiceRequest(result, "Requests");
                if (file != null && file.Length > 0)
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Convert.ToBase64String(file));
                }
                else
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "file is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok("file is null");
                }
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }

        #endregion
    }
}