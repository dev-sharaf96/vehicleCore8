using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Services;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Orders;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Quotation.Components;
using Tameenk.Models.Checkout;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Policies.Renewal;
using Tameenk.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{

    /// <summary>
    /// Policy Controller
    /// </summary>
    public class PolicyController : AdminBaseApiController
    {
        #region Fields

        private readonly IExcelService _excelService;
        private readonly IPolicyService _policyService;
        private readonly Policy.Components.IPolicyContext _policyContext;
        private readonly TameenkConfig tameenkConfig;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserPageService _userPageService;
        private IWebApiContext _webApiContext;
        private readonly IOrderService _orderService;
        private readonly ICheckoutsService _checkoutsService;
        private readonly IPaymentService _paymentService;
        private readonly Tameenk.Services.Inquiry.Components.IInquiryContext _inquiryContext;
        private readonly IQuotationContext _quotationContext;
        private readonly ICheckoutContext _checkoutContext;
        private readonly Policy.Components.IPolicyFileContext _policyFileContext;
        private readonly Policy.Components.IPolicyModificationContext _policyModificationContext;
        private readonly IOwnDamageQueueService _OwnDamageQueueService;

        #endregion


        #region The Ctor


        /// <summary>
        /// the Constructor
        /// </summary>
        public PolicyController(IExcelService excelService, IPolicyService policyService, Policy.Components.IPolicyContext policyContext,
            TameenkConfig tameenkConfig, IAuthorizationService authorizationService, IUserPageService userPageService, IOrderService orderService,ICheckoutsService checkoutsService, IPaymentService paymentService,
            Tameenk.Services.Inquiry.Components.IInquiryContext inquiryContext, IQuotationContext quotationContext, ICheckoutContext checkoutContext
            , Policy.Components.IPolicyFileContext policyFileContext
            , Policy.Components.IPolicyModificationContext policyModificationContext, IOwnDamageQueueService ownDamageQueueService)
        {
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
            _policyService = policyService ?? throw new ArgumentNullException(nameof(IPolicyService));
            _policyContext = policyContext;
            this.tameenkConfig = tameenkConfig;
            _authorizationService = authorizationService ?? throw new TameenkArgumentNullException(nameof(IAuthorizationService));
            _userPageService = userPageService ?? throw new TameenkArgumentNullException(nameof(IUserPageService));
            _orderService = orderService;
            _checkoutsService = checkoutsService;
            _paymentService = paymentService ?? throw new TameenkArgumentNullException(nameof(IPaymentService));
            _inquiryContext = inquiryContext;
            _quotationContext = quotationContext;
            _checkoutContext = checkoutContext;
            _policyFileContext = policyFileContext;
            _policyModificationContext = policyModificationContext;
            _OwnDamageQueueService = ownDamageQueueService;

        }

        #endregion


        #region Methods

        /// <summary>
        /// Get Najm status lookup
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/all-najm-status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<NajmStatusModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetNajmStatuses(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            try
            {
                var result = _policyService.GetNajmStatuses(pageIndex, pageSize, sortField, sortOrder);
                return Ok(result.Select(e => e.ToModel()), result.Count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Edit in fail policy
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/edit-fail")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<FailPolicyModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult EditFailPolicy(FailPolicyModel failPolicyModel)
        {
            try
            {

                FailPolicy failPolicy = failPolicyModel.ToServiceModel();
                var failPolicyResult = _policyService.EditFailPolicy(failPolicy);

                if (failPolicyResult == null)
                    throw new TameenkArgumentException("ReferenceId Not Found.");

                return Ok(failPolicyResult.ToModel());

            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Re-Generate Policy File Pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/re-generate")]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public async Task<IActionResult> ReGeneratePolicyFilePdf(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReGeneratePolicyFilePdf";
            log.ServiceRequest = $"referenceId: {referenceId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("ReferenceId");

                //var policyFileBytes = await _policyService.ReGeneratePolicyFilePdf(referenceId);

                //if (policyFileBytes != null && policyFileBytes.Length > 0)
                //    return Ok(Convert.ToBase64String(policyFileBytes));
                //else
                //    return Ok("");

                var output = _policyContext.GetFailedPolicyFile(referenceId, Utilities.GetInternalServerIP(), Channel.Dashboard.ToString());
                if (output.ErrorCode == Policy.Components.PdfGenerationOutput.ErrorCodes.Success)
                {
                    return Ok(Convert.ToBase64String(output.File));
                }
                else
                {
                    return Error(output.ErrorDescription);
                }

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
        /// Get details to Fail policy by reference Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/fail-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<FailPolicyModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public IActionResult GetDetailsToFailPolicyByReferenceId(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetDetailsToFailPolicyByReferenceId";
            log.ServiceRequest = $"referenceId: {referenceId}"; ;
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("ReferenceId");


                var result = _policyService.GetDetailsToFailPolicyByReferenceId(referenceId);


                return Ok(result.ToModel());

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
        /// reset Max tries to generate policy again and again
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/set-policy-tries")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public IActionResult ResetMaxTriesToPolicy(string ReferenceId, int processingTries = 0)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ResetMaxTriesToPolicy";
            log.ServiceRequest = $"referenceId: {ReferenceId},processingTries: {processingTries}" ;
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = ReferenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(ReferenceId))
                    throw new TameenkArgumentNullException("ReferenceId");

                bool result = _policyService.UpdateMaxTriesInPolicy(ReferenceId, processingTries);

                return Ok(result);
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
        /// Get fail Policies with details 
        /// based on filter 
        /// </summary>
        /// <param name="policyFilter">policy Filter</param>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">Sort Field</param>
        /// <param name="sortOrder">sort Order</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/fail-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<FailPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public IActionResult GetFailPoliciesWithFilter([FromBody]FailPolicyFilterModel policyFilter)
        {
            Utilities.SendRequestToQueue();
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyFilter.InsuranceCompanyId;
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetFailPoliciesWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (log.Headers["User-Agent"].ToString().ToLower().Contains("postman"))
                {
                    log.ErrorCode = 1000;
                    log.ErrorDescription = "request from post man";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Go Away !!!!!!!!!");
                }

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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate.Value.Date > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }
               
                int totalCount = 0;
                string exception = string.Empty;
                var result = GetAllFailedPoliciesFromDBWithFilter(policyFilter, out totalCount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 500;
                    log.ErrorDescription = $"GetAllFailedPoliciesFromDBWithFilter return exception: {exception}";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Service exception error");
                }

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result, totalCount);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("Exception error has occured");
            }
        }

        /// <summary>
        /// Get details to Success policies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/success-details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<SuccessPolicyModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetDetailsToSuccessPolicies(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Success policies";
            log.PageURL = "/admin/policies/success";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetDetailsToSuccessPolicies";
            log.ServiceRequest = $"referenceId: {referenceId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                //var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                //if (!isAuthorized)
                //{
                //    log.ErrorCode = 10;
                //    log.ErrorDescription = "User not authenticated : " + log.UserID;
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error("Not Authorized : " + log.UserID);
                //}
                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("ReferenceId");


                var result = _policyService.AdminGetDetailsToSuccessPolicies(referenceId);


                return Ok(result);

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
        /// Get Success Policies with details 
        /// based on filter 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/sama-report-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SamaReportResultModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 13)]
        public IActionResult GetSamaReportWithFilter([FromBody]SamaReportFilterModel reportFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "InvoiceDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = reportFilter.InsuranceCompanyId;
            log.PageName = "finance";
            log.PageURL = "/admin/finance";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSamaReportWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(reportFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = reportFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(13, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (reportFilter == null)
                    throw new TameenkArgumentNullException("reportFilter");


                if (reportFilter.InvoiceDateFrom != null && reportFilter.InvoiceDateTo != null)
                {
                    if (reportFilter.InvoiceDateFrom > reportFilter.InvoiceDateTo)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                if (reportFilter.DriverBirthDateFrom != null && reportFilter.DriverBirthDateTo != null)
                {
                    if (reportFilter.DriverBirthDateFrom > reportFilter.DriverBirthDateTo)
                        throw new TameenkArgumentException("Driver End Date must be greater than start date.", "Driver Date To");
                }
                int totalCount = 0;
                string exception = string.Empty;
                List<SamaReportModel> samaData = GetSamaReport(reportFilter, pageIndex, 10, sortField, sortOrder, 60 * 60 * 60, out totalCount,out exception);
                if(!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 900;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(new SamaReportResultModel(), 0);
                }
                var result = new SamaReportResultModel
                {
                    SamaReports = samaData
                };
                if (samaData != null && samaData.Count > 0)
                {
                    result.SamaReportStatistics = new List<SamaReportStatisticsModel>();
                    var companyGrp = samaData.GroupBy(e => e.InsuranceCompanyName);
                    foreach (var item in companyGrp)
                    {
                        result.SamaReportStatistics.Add(new SamaReportStatisticsModel
                        {
                            InsuranceCompanyName = item.Key,
                            Count = item.Count()
                        });
                    }
                }
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/policy/success-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SuccessPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GetSuccessPoliciesWithFilter([FromBody]SuccessPoliciesFilterModel policyFilter)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyFilter.InsuranceCompanyId;
            log.PageName = "Success policies";
            log.PageURL = "/admin/policies/success";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSuccessPoliciesWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                int totalCount = 0;                string exception = string.Empty;                var result = GetAllPoliciesFromDBWithFilter(policyFilter, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error");
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }


        /// <summary>
        /// Get All policy status except Avaliable Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/policy-status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PolicyStatusModel>>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllPolicyStatusExceptAvaliable()
        {
            try
            {
                var result = _policyService.GetAllPolicyStatusExceptAvaliable();


                return Ok(result.Select(s => s.ToModel()), result.Count());

            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }


        /// <summary>
        /// download policy File
        /// Api return string64
        ///<para>if file not found or file empty return empty string</para>
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [Route("api/policy/download-policy")]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult DownloadPolicyFile(string fileId)
        {

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Success policies";
            log.PageURL = "/admin/policies/success";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DownloadPolicyFile";
            log.ServiceRequest = $"fileId: {fileId}";
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
                //var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                //if (!isAuthorized)
                //{
                //    log.ErrorCode = 10;
                //    log.ErrorDescription = "User not authenticated : " + log.UserID;
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error("Not Authorized : " + log.UserID);
                //}
                if (string.IsNullOrEmpty(fileId))
                    throw new TameenkArgumentNullException("fileId");

                Guid guidResult = new Guid();
                bool isValid = Guid.TryParse(fileId, out guidResult);

                if (!isValid)
                    throw new TameenkArgumentException("Not Vaild File ID", "fileId");
                var policyFile = _policyService.DownloadPolicyFile(fileId);
                if (policyFile != null)
                {
                    if (policyFile.PolicyFileByte != null)
                    {
                        return Ok(Convert.ToBase64String(policyFile.PolicyFileByte));
                    }
                    else if (!string.IsNullOrEmpty(policyFile.FilePath))
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();
                        string exception = string.Empty;
                        if (tameenkConfig.RemoteServerInfo.UseNetworkDownload) //server 2
                        {
                            var file = fileShare.GetFile(policyFile.FilePath, out exception);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            if (file==null)
                            {
                                string newPath = Utilities.ConvertOldPdfPathToNewPath(policyFile.FilePath);
                                file = fileShare.GetFile(newPath, out exception);
                            }
                            if (file != null)
                                return Ok(Convert.ToBase64String(file));
                            else
                                return Error("exception " + exception);
                        }
                        else
                        {
                            exception = string.Empty;
                            var file = File.ReadAllBytes(policyFile.FilePath);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            if (file==null)
                            {
                                string newPath = Utilities.ConvertOldPdfPathToNewPath(policyFile.FilePath);
                                file = File.ReadAllBytes(newPath);
                            }
                            if (file != null)
                            {
                                return Ok(Convert.ToBase64String(file));
                            }
                            else
                            {
                                return Error("No file exists");
                            }
                        }
                    }
                    else
                    {
                        return Error("No PolicyFileByte or FilePath for this id " + fileId);
                    }
                }
                else
                {
                    return Error("policyFile is null with this id " + fileId);
                }

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
        /// Re-download File Again 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [Route("api/policy/re-download-policy")]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public async Task<IActionResult> ReDownloadFileAgain(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReDownloadFileAgain";
            log.ServiceRequest = $"referenceId: {referenceId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                _policyContext.remoteServerInfo = tameenkConfig.RemoteServerInfo;
                var output = _policyContext.GetFailedPolicyFile(referenceId, Utilities.GetInternalServerIP(), Channel.Dashboard.ToString());
                if (output.ErrorCode == Policy.Components.PdfGenerationOutput.ErrorCodes.Success)
                {
                    return Ok(Convert.ToBase64String(output.File));
                }
                else
                {
                    return Error(output.ErrorDescription);
                }

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
        /// Generate Excel Sheet Fail Policy
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/fail-policy-excel")]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public IActionResult GenerateExcelSheetForFailPolicies(FailPolicyFilterModel filter)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GenerateExcelSheetForFailPolicies";
            log.ServiceRequest = JsonConvert.SerializeObject(filter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = filter.ReferenceNo;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (log.Headers["User-Agent"].ToString().ToLower().Contains("postman"))
                {
                    log.ErrorCode = 1000;
                    log.ErrorDescription = "request from post man";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Go Away !!!!!!!!!");
                }
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

                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                int count = 0;
                string exception = string.Empty;
                filter.PageIndex = 0;
                filter.pageSize = int.MaxValue;
                filter.IsExcel = true;
                var policies = GetAllFailedPoliciesFromDBWithFilter(filter, out count, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 500;
                    log.ErrorDescription = $"Export GetAllFailedPoliciesFromDBWithFilter return exception: {exception}";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Export Service exception error");
                }

                byte[] file = _excelService.GenerateFailPoliciesExcel(policies);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("Export error while generate file");
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("Export Exception error has occured");
            }
        }

        /// <summary>
        /// Generate Excel Sheet 
        /// </summary>
        /// <param name="filter">Policy Filter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/success-policy-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GenerateExcelSheetForSuccessPolicies(SuccessPoliciesFilterModel filter)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "Success policies";
            log.PageURL = "/admin/policies/success";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GenerateExcelSheetForSuccessPolicies";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                int count = 0;
                string exception = string.Empty;
                //List<Tameenk.Core.Domain.Entities.Policy> policies = _policyService.GetCountSuccessPoliciesWithFilter(filter.ToModel()).ToList();

                filter.PageIndex = 0;
                filter.pageSize = int.MaxValue;
                var policies = GetAllPoliciesFromDBWithFilter(filter, out count, out exception);
                /* System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
                s.Start();*/
                if (policies == null)
                    return Ok("");

                byte[] file = _excelService.GenerateExcelSuccessPolicies(policies);
                /*s.Stop();
                var r1 = s.Elapsed.TotalSeconds;
                s = new System.Diagnostics.Stopwatch();
                s.Start();
                byte[] file1 = _excelService.GenerateExcelSheet<Policy>(policies, nameOfExcelSheet+"k", dateTime);
                s.Stop();
                var r2 = s.Elapsed.TotalSeconds;*/
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
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
        /// Generate Excel Sheet for sama
        /// </summary>
        /// <param name="filter">Policy Filter</param>
        /// <returns></returns>


        [HttpPost]
        [Route("api/policy/sama-report-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 13)]
        public IActionResult GenerateExcelSheetForSama([FromBody]SamaReportFilterModel filter)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = filter.InsuranceCompanyId;
            log.PageName = "finance";
            log.PageURL = "/admin/finance";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GenerateExcelSheetForSama";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(13, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                int totalCount = 0;
                filter.IsExcel = true;
                string exception = string.Empty;
                var samaData = GetSamaReport(filter, 0, int.MaxValue, "InvoiceDate", false, 60 * 60 * 60, out totalCount,out exception);
                if (samaData == null)
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 14;
                    log.ErrorDescription = "samaData is null and excption is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Data");
                }

                var items = samaData.Select(e => e.ToServiceModel()).ToList();
                if (items == null)
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 16;
                    log.ErrorDescription = "items is null after mapping";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("No Data");
                }
                byte[] file = _excelService.GenerateExcelSamaReport(items);

                if (file != null && file.Length > 0)
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Convert.ToBase64String(file));
                }
                else
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 15;
                    log.ErrorDescription = "Failed as file is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok("");
                }
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }
        #endregion


        [Authorize]
        [HttpPost]
        [Route("api/policy/generate-policy-manually")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 8)]
        public IActionResult GeneratePolicyManually([FromBody]PolicyData policyInfo)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyInfo.CompanyID;
            log.PageName = "Generate Policy";
            log.PageURL = "/admin/policies/generate";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GeneratePolicyManually";
            log.ServiceRequest = JsonConvert.SerializeObject(policyInfo);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyInfo.ReferenceId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                //var output = _policyService.GeneratePolicyManually(policyInfo);
                //return Ok(output);
                string userName = User.Identity.GetUserName();
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
                var isAuthorized = _userPageService.IsAuthorizedUser(8, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var output = _policyContext.GeneratePolicyManually(policyInfo, Utilities.GetInternalServerIP(), Channel.Dashboard.ToString(), userName, tameenkConfig.RemoteServerInfo.UseNetworkDownload, tameenkConfig.RemoteServerInfo.DomainName, tameenkConfig.RemoteServerInfo.ServerIP, tameenkConfig.RemoteServerInfo.ServerUserName, tameenkConfig.RemoteServerInfo.ServerPassword);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if(output.ErrorCode!=1)
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "failed to generate due to "+output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        private List<SamaReportModel> GetSamaReport(SamaReportFilterModel filter, int pageIndex, int pageSize, string sortField, bool sortOrder, int commandTimeout, out int totalCount,out string exption)
        {
            totalCount = 0;
            exption = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetSamaReport";
                command.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrWhiteSpace(filter.MerchantId))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@merchantId", Value = filter.MerchantId });
                }
                if (filter.InvoiceDateFrom.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.InvoiceDateFrom.Value.Year, filter.InvoiceDateFrom.Value.Month, filter.InvoiceDateFrom.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }
                if (filter.InvoiceDateTo.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.InvoiceDateTo.Value.Year, filter.InvoiceDateTo.Value.Month, filter.InvoiceDateTo.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }

                if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@referenceId", Value = filter.ReferenceNo });

                if (filter.InsuranceCompanyId.HasValue)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyID", Value = filter.InsuranceCompanyId.Value });

                if (!string.IsNullOrWhiteSpace(filter.PolicyHolderName))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@policyHolderName", Value = filter.PolicyHolderName });

                if (!string.IsNullOrWhiteSpace(filter.Mobile))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@mobile", Value = filter.Mobile });

                if (!string.IsNullOrWhiteSpace(filter.Email))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@email", Value = filter.Email });

                if (!string.IsNullOrWhiteSpace(filter.PolicyNo))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@policyNo", Value = filter.PolicyNo });

                if (filter.ProductTypeCode.HasValue)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@productTypeCode", Value = filter.ProductTypeCode.Value });

                if (filter.DriverBirthDateFrom.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.DriverBirthDateFrom.Value.Year, filter.DriverBirthDateFrom.Value.Month, filter.DriverBirthDateFrom.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@driverBirthDateFrom", Value = dtStart });
                }

                if (filter.DriverBirthDateTo.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.DriverBirthDateTo.Value.Year, filter.DriverBirthDateTo.Value.Month, filter.DriverBirthDateTo.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@driverBirthDateFrom", Value = dtEnd });
                }

                if (filter.Channel.HasValue)
                {
                    var stringChannel = Enum.GetName(typeof(Channel), filter.Channel.Value);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@channel", Value = stringChannel });
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = pageIndex + 1 });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = pageSize });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@isExcel", Value = filter.IsExcel ? 1 : 0 });
                if (filter.IsCorporate.HasValue)
                {
                    if (filter.IsCorporate == 1)
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@isCorprate", Value = 1 });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@isIndividual", Value = null });
                    }
                    else
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@isCorprate", Value = null });
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@isIndividual", Value = 1 });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<SamaReportModel>(reader).ToList();

                if (filter.IsExcel)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return data;
                }

                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exption = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;

            }
        }
        private string GetSamaReportWhereClause(SamaReportFilterModel filter)
        {
            var qury = new StringBuilder();
            if (filter.InvoiceDateFrom.HasValue)
            {

                DateTime dtStart = new DateTime(filter.InvoiceDateFrom.Value.Year, filter.InvoiceDateFrom.Value.Month, filter.InvoiceDateFrom.Value.Day, 0, 0, 0);

                qury.Append($" AND i.InvoiceDate >= '{dtStart}'");
            }
            if (filter.InvoiceDateTo.HasValue)
            {
                DateTime dtEnd = new DateTime(filter.InvoiceDateTo.Value.Year, filter.InvoiceDateTo.Value.Month, filter.InvoiceDateTo.Value.Day, 23, 59, 59);
                qury.Append($" AND i.InvoiceDate <= '{dtEnd}'");
            }

            if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
                qury.Append($" AND i.ReferenceId = '{filter.ReferenceNo}'");

            if (filter.InsuranceCompanyId.HasValue)
                qury.Append($" AND comp.InsuranceCompanyID = {filter.InsuranceCompanyId.Value}");

            if (!string.IsNullOrWhiteSpace(filter.PolicyHolderName))
                qury.Append($" AND CONCAT(d.FirstName, ' ', d.SecondName, ' ', d.LastName) = '{filter.PolicyHolderName}'");

            if (!string.IsNullOrWhiteSpace(filter.Mobile))
                qury.Append($" AND checkout.Phone = '{filter.Mobile}'");

            if (!string.IsNullOrWhiteSpace(filter.Email))
                qury.Append($" AND checkout.Email = '{filter.Email}'");

            if (!string.IsNullOrWhiteSpace(filter.PolicyNo))
                qury.Append($" AND p.PolicyNo = '{filter.PolicyNo}'");

            if (filter.ProductTypeCode.HasValue)
                qury.Append($" AND insuranceType.Code = {filter.ProductTypeCode}");

            if (filter.DriverBirthDateFrom.HasValue)
            {
                DateTime dtStart = new DateTime(filter.DriverBirthDateFrom.Value.Year, filter.DriverBirthDateFrom.Value.Month, filter.DriverBirthDateFrom.Value.Day, 0, 0, 0);
                qury.Append($" AND d.DateOfBirthG >= '{dtStart}'");
            }

            if (filter.DriverBirthDateTo.HasValue)
            {
                DateTime dtEnd = new DateTime(filter.DriverBirthDateTo.Value.Year, filter.DriverBirthDateTo.Value.Month, filter.DriverBirthDateTo.Value.Day, 23, 59, 59);
                qury.Append($" AND d.DateOfBirthG <= '{dtEnd}'");
            }

            return qury.ToString();
        }



        [HttpGet]
        [Route("api/policy/fail-policies-reached-max-trials")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<FailPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetFailedPoliciesThatReachedMaxTrials()
        {
            try
            {
                List<FailPolicy> data = _policyService.GetFailedPoliciesThatReachedMaxTrials();
                byte[] file = _excelService.GenerateExcelFailPolicies(data);

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

        [HttpPost]
        [Route("api/policy/all-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SuccessPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllPoliciesWithFilter([FromBody]SuccessPoliciesFilterModel policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "policyIssueDate", bool sortOrder = false)

        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            try
            {

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }


                IQueryable<Tameenk.Core.Domain.Entities.Policy> query = _policyService.GetAllPoliciesWithFilter(policyFilter.ToModel());

                var result = _policyService.GetAllPoliciesWithFilter(query, pageIndex, pageSize, sortField, sortOrder);



                return Ok(result.Select(e => e.ToServiceModel()), query.Count());

            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }
        [HttpGet]
        [Route("api/policy/cancel-policy")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<bool>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 21)]
        public IActionResult CancelPolicy(string id, bool isCancelled)
        {
            bool result = false;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
           
            log.PageName = "Cancel Policy";
            log.PageURL = "/admin/policies/cancel";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "CancelPolicy";
            log.ServiceRequest = $"ReferenceId: {id}, isCancelled: {isCancelled}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = id;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(21, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string userName = User.Identity.GetUserName();
                result = _policyService.CancelPolicy(id, isCancelled, userName);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
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
        /// download invoice File
        /// Api return string64
        ///<para>if file not found or file empty return empty string</para>
        /// </summary>
        /// <param name="referenceId">file id</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [Route("api/policy/download-invoice")]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult DownloadInvoice(string referenceId)
        {
            try
            {
                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("referenceId");
                 string exception = string.Empty;
                //update Fees calculation  details 

                var checkout = _checkoutsService.GetFromCheckoutDeatilsbyReferenceId(referenceId);
                if (checkout != null)
                {
                    int paymentId = checkout.PaymentMethodId.Value;
                    if (checkout.PaymentMethodId == 7)
                    {
                        var hyperpayResponse = _paymentService.GetFromHyperpayResponseSuccessTransaction(referenceId);
                        if (hyperpayResponse != null)
                        {
                            string brandName = hyperpayResponse.PaymentBrand.ToLower();
                            paymentId = 0;
                            if (brandName == "mada")
                                paymentId = 10;
                            if (brandName == "visa" || brandName == "master")
                                paymentId = 4;
                            if (brandName == "amex")
                                paymentId = 13;
                        }
                    }
                    _orderService.UpdateInvoiceFeesCalculationDetails(referenceId, checkout.SelectedInsuranceTypeCode.Value, checkout.InsuranceCompanyId.Value, paymentId);
                }
                var invoiceFile = _policyService.GetInvoiceFileByRefrenceId(referenceId);
                if (invoiceFile != null)
                {
                    if (invoiceFile.InvoiceData != null)
                    {
                        return Ok(Convert.ToBase64String(invoiceFile.InvoiceData));
                    }
                    else if (!string.IsNullOrEmpty(invoiceFile.FilePath))
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();

                        //server 2
                        if (tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            var file = fileShare.GetFile(invoiceFile.FilePath, out exception);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(invoiceFile.FilePath, out exception);
                            if (file != null)
                                return Ok(Convert.ToBase64String(file));
                            else
                            {
                                bool isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(referenceId, out exception);
                                if (isInvoiceFileGenerated)
                                {
                                    return DownloadInvoice(referenceId);
                                }
                                return Error("Error occured while generating invoice file due to " + exception);

                            }
                        }
                        else
                        {
                            exception = string.Empty;
                            var file = File.ReadAllBytes(invoiceFile.FilePath);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(invoiceFile.FilePath, out exception);

                            if (file != null)
                                return Ok(Convert.ToBase64String(file));
                            else
                            {
                                bool isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(referenceId, out exception);
                                if (isInvoiceFileGenerated)
                                {
                                    return DownloadInvoice(referenceId);
                                }
                                return Error("Error occured while generating invoice file due to " + exception);
                            }

                            //if (File.Exists(invoiceFile.FilePath))
                            //    return Ok(Convert.ToBase64String(File.ReadAllBytes(invoiceFile.FilePath)));
                            //else
                            //{
                            //    bool isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(referenceId, out exception);
                            //    if (isInvoiceFileGenerated)
                            //    {
                            //        return DownloadInvoice(referenceId);
                            //    }
                            //    return Error("Error occured while generating invoice file due to " + exception);
                            //}
                        }
                    }
                    else // Generate invoice 
                    {
                        bool isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(referenceId, out exception);
                        if (isInvoiceFileGenerated)
                        {
                            return DownloadInvoice(referenceId);
                        }
                        return Error("Error occured while generating invoice file due to " + exception);
                    }
                }
                else
                {
                    bool isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(referenceId, out exception);
                    if (isInvoiceFileGenerated)
                    {
                        return DownloadInvoice(referenceId);
                    }
                    return Error("Error occured while generating invoice file due to " + exception);
                    //return Error("invoceFile record not found for this referenceId " + referenceId);
                }

            }
            catch (Exception ex)
            {
                return Error("Error occured while generating invoice file");
            }
        }

        [HttpPost]
        [Route("api/policy/income-report-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SamaReportModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetIncomeReportWithFilter([FromBody]SuccessPoliciesFilterModel policyFilter)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            try
            {

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                var reportResult = _policyService.GetIncomeReportWithFilter(policyFilter.ToModel()).ToList();
                var result = new SamaReportResultModel();

                result.SamaReportStatistics = new List<SamaReportStatisticsModel>();
                int totalCount = 0;
                foreach (var item in reportResult)
                {
                    result.SamaReportStatistics.Add(new SamaReportStatisticsModel
                    {
                        InsuranceCompanyName = item.InsuranceCompanyName,
                        Count = item.Count
                    });
                    totalCount += item.Count;
                }

                return Ok(result, totalCount);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }
        [HttpPost]
        [Route("api/policy/policies-status-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SuccessPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 32)]
        public IActionResult GetPoliciesStatusWithFilter([FromBody]SuccessPoliciesFilterModel statusPolicyFilter)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = statusPolicyFilter.InsuranceCompanyId;
            log.PageName = "Policy Status";
            log.PageURL = "/admin/policies/policystatus";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPoliciesStatusWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(statusPolicyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = statusPolicyFilter.ReferenceNo;
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (log.Headers["User-Agent"].ToString().ToLower().Contains("postman"))
                {
                    log.ErrorCode = 1000;
                    log.ErrorDescription = "request from post man";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Go Away !!!!!!!!!");
                }

                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(32, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (statusPolicyFilter == null)
                    throw new TameenkArgumentNullException("statusPolicyFilter");

                int successPolicyListCount = 0;
                string exception = string.Empty;
                var successPolicyList = GetPolicyStatusFromDBWithFilter(statusPolicyFilter, out successPolicyListCount, out exception);

                PagedList<PolicyListingModel> failPolicyList = null;
                int failPolicyListCount = 0;
                if (successPolicyList != null && successPolicyList.Any())
                {
                    return Ok(successPolicyList, successPolicyListCount);
                }
                //else if(string.IsNullOrEmpty(statusPolicyFilter.PolicyNo))
                //{
                //    failPolicyList = GetAllFailedPoliciesFromDBWithFilter(statusPolicyFilter.ToFailPolicyModel(), out failPolicyListCount);
                //}
                else
                {
                    return Ok(new List<PolicyListingModel>());
                }

                return Ok(failPolicyList, failPolicyListCount);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        private List<PolicyListingModel> GetAllPoliciesFromDBWithFilter(SuccessPoliciesFilterModel policyFilter, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllPoliciesFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60*60;

                if (!string.IsNullOrWhiteSpace(policyFilter.MerchantId))
                {
                    SqlParameter merchantId = new SqlParameter() { ParameterName = "merchantId", Value = (policyFilter.MerchantId)};
                    command.Parameters.Add(merchantId);
                }

                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateString", Value = (policyFilter.StartDate.HasValue) ? policyFilter.StartDate.Value.ToString("yyyy-MM-dd 00:00:00") : null };
                command.Parameters.Add(startDateParameter);

                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateString", Value = (policyFilter.EndDate.HasValue) ? policyFilter.EndDate.Value.ToString("yyyy-MM-dd 23:59:59") : null };
                command.Parameters.Add(endDateParameter);

                //SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = (pageIndex > 1) ? pageIndex : 1 };
                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = policyFilter.PageIndex > 0 ? policyFilter.PageIndex + 1 : 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = policyFilter.pageSize > 0 ? policyFilter.pageSize : 10 };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo)) ? policyFilter.PolicyNo : "" };
                command.Parameters.Add(PolicyNoParameter);

                SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = (!string.IsNullOrWhiteSpace(policyFilter.NationalId)) ? policyFilter.NationalId : "" };
                command.Parameters.Add(NationalIdParameter);

                SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo)) ? policyFilter.ReferenceNo : "" };
                command.Parameters.Add(ReferenceNoParameter);

                SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "SequenceNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.SequenceNo)) ? policyFilter.SequenceNo : "" };
                command.Parameters.Add(SequenceNoParameter);

                SqlParameter CustomNoParameter = new SqlParameter() { ParameterName = "CustomNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.CustomNo)) ? policyFilter.CustomNo : "" };
                command.Parameters.Add(CustomNoParameter);

                SqlParameter InsuredEmailParameter = new SqlParameter() { ParameterName = "InsuredEmail", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredEmail)) ? policyFilter.InsuredEmail : "" };
                command.Parameters.Add(InsuredEmailParameter);

                SqlParameter InsuredFirstNameArParameter = new SqlParameter() { ParameterName = "InsuredFirstNameAr", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredFirstNameAr)) ? policyFilter.InsuredFirstNameAr : "" };
                command.Parameters.Add(InsuredFirstNameArParameter);

                SqlParameter InsuredLastNameArParameter = new SqlParameter() { ParameterName = "InsuredLastNameAr", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredLastNameAr)) ? policyFilter.InsuredLastNameAr : "" };
                command.Parameters.Add(InsuredLastNameArParameter);

                SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId ?? 0 };
                command.Parameters.Add(NajmStatusIdParameter);

                SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo ?? 0 };
                command.Parameters.Add(InvoiceNoParameter);

                SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId ?? 0 };
                command.Parameters.Add(InsuranceCompanyIdParameter);

                SqlParameter ProductTypeIdParameter = new SqlParameter() { ParameterName = "ProductTypeId", Value = policyFilter.ProductTypeId ?? 0 };
                command.Parameters.Add(ProductTypeIdParameter);

                SqlParameter ChannelParameter = new SqlParameter() { ParameterName = "Channel", Value = (policyFilter.Channel.HasValue) ? Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), policyFilter.Channel.Value) : "" };
                command.Parameters.Add(ChannelParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<PolicyListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyListingModel>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private List<PolicyListingModel> GetPolicyStatusFromDBWithFilter(SuccessPoliciesFilterModel policyFilter, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyStatusFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60;

                if (!string.IsNullOrWhiteSpace(policyFilter.MerchantId))
                {
                    SqlParameter merchantId = new SqlParameter() { ParameterName = "merchantId", Value = (policyFilter.MerchantId) };
                    command.Parameters.Add(merchantId);
                }

                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateString", Value = (policyFilter.StartDate.HasValue) ? policyFilter.StartDate.Value.ToString("yyyy-MM-dd 00:00:00") : null };
                command.Parameters.Add(startDateParameter);

                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateString", Value = (policyFilter.EndDate.HasValue) ? policyFilter.EndDate.Value.ToString("yyyy-MM-dd 23:59:59") : null };
                command.Parameters.Add(endDateParameter);

                //SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = (pageIndex > 1) ? pageIndex : 1 };
                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = policyFilter.PageIndex > 0 ? policyFilter.PageIndex + 1 : 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = policyFilter.pageSize > 0 ? policyFilter.pageSize : 10 };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo)) ? policyFilter.PolicyNo : "" };
                command.Parameters.Add(PolicyNoParameter);

                SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = (!string.IsNullOrWhiteSpace(policyFilter.NationalId)) ? policyFilter.NationalId : "" };
                command.Parameters.Add(NationalIdParameter);

                SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo)) ? policyFilter.ReferenceNo : "" };
                command.Parameters.Add(ReferenceNoParameter);

                SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "SequenceNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.SequenceNo)) ? policyFilter.SequenceNo : "" };
                command.Parameters.Add(SequenceNoParameter);

                SqlParameter CustomNoParameter = new SqlParameter() { ParameterName = "CustomNo", Value = (!string.IsNullOrWhiteSpace(policyFilter.CustomNo)) ? policyFilter.CustomNo : "" };
                command.Parameters.Add(CustomNoParameter);

                SqlParameter InsuredEmailParameter = new SqlParameter() { ParameterName = "InsuredEmail", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredEmail)) ? policyFilter.InsuredEmail : "" };
                command.Parameters.Add(InsuredEmailParameter);

                SqlParameter InsuredFirstNameArParameter = new SqlParameter() { ParameterName = "InsuredFirstNameAr", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredFirstNameAr)) ? policyFilter.InsuredFirstNameAr : "" };
                command.Parameters.Add(InsuredFirstNameArParameter);

                SqlParameter InsuredLastNameArParameter = new SqlParameter() { ParameterName = "InsuredLastNameAr", Value = (!string.IsNullOrWhiteSpace(policyFilter.InsuredLastNameAr)) ? policyFilter.InsuredLastNameAr : "" };
                command.Parameters.Add(InsuredLastNameArParameter);

                SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId ?? 0 };
                command.Parameters.Add(NajmStatusIdParameter);

                SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo ?? 0 };
                command.Parameters.Add(InvoiceNoParameter);

                SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId ?? 0 };
                command.Parameters.Add(InsuranceCompanyIdParameter);

                SqlParameter ProductTypeIdParameter = new SqlParameter() { ParameterName = "ProductTypeId", Value = policyFilter.ProductTypeId ?? 0 };
                command.Parameters.Add(ProductTypeIdParameter);

                SqlParameter ChannelParameter = new SqlParameter() { ParameterName = "Channel", Value = (policyFilter.Channel.HasValue) ? Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), policyFilter.Channel.Value) : "" };
                command.Parameters.Add(ChannelParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<PolicyListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyListingModel>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private string GetPoliciesWhereClause(SuccessPoliciesFilterModel filter)
        {
            var qury = new StringBuilder();

            if (filter.EndDate != null && filter.StartDate != null && filter.StartDate?.Date == filter.EndDate?.Date)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND Invoice.InvoiceDate > '{dtStart}' AND Invoice.InvoiceDate < '{dtEnd}'");
            }
            else if (filter.EndDate != null && filter.StartDate != null && filter.StartDate != filter.EndDate)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND Invoice.InvoiceDate >= '{dtStart}' AND Invoice.InvoiceDate <= '{dtEnd}'");
            }
            else if (filter.StartDate.HasValue)
            {
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND Invoice.InvoiceDate >= '{dtStart}'");
            }
            else if (filter.EndDate.HasValue)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                qury.Append($" AND Invoice.InvoiceDate <= '{dtEnd}'");
            }

            if (!string.IsNullOrEmpty(filter.PolicyNo))
                qury.Append($" AND Policy.PolicyNo = '{filter.PolicyNo}'");

            if (!string.IsNullOrEmpty(filter.NationalId))
                qury.Append($" AND Driver.NIN = '{filter.NationalId}'");

            if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
                qury.Append($" AND Policy.CheckOutDetailsId = '{filter.ReferenceNo}'");

            if (!string.IsNullOrWhiteSpace(filter.SequenceNo))
                qury.Append($" AND Vehicles.SequenceNumber = '{filter.SequenceNo}'");

            if (!string.IsNullOrWhiteSpace(filter.CustomNo))
                qury.Append($" AND Vehicles.CustomCardNumber = '{filter.CustomNo}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredEmail))
                qury.Append($" AND CheckoutDetails.Email = '{filter.InsuredEmail}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredFirstNameAr))
                qury.Append($" AND Driver.FirstName = '{filter.InsuredFirstNameAr}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredLastNameAr))
                qury.Append($" AND Driver.LastName = '{filter.InsuredLastNameAr}'");

            if (filter.NajmStatusId.HasValue)
                qury.Append($" AND Policy.NajmStatusId = '{filter.NajmStatusId.Value}'");

            if (filter.InvoiceNo.HasValue)
                qury.Append($" AND Invoice.InvoiceNo = '{filter.InvoiceNo.Value}'");

            if (filter.InsuranceCompanyId.HasValue)
                qury.Append($" AND Policy.InsuranceCompanyID = '{filter.InsuranceCompanyId.Value}'");

            if (filter.ProductTypeId.HasValue)
                qury.Append($" AND CheckoutDetails.SelectedInsuranceTypeCode = '{filter.ProductTypeId.Value}'");

            if (filter.Channel.HasValue)
            {
                string EnumValue = Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), filter.Channel.Value);
                qury.Append($" AND CheckoutDetails.Channel = '{EnumValue}'");
            }

            if (qury.ToString() == string.Empty)
            {
                filter.EndDate = DateTime.Now;
                filter.StartDate = DateTime.Now;
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND Invoice.InvoiceDate >= '{dtStart}' AND Invoice.InvoiceDate <= '{dtEnd}'");
            }

            return qury.ToString();
        }

        private List<PolicyListingModel> GetAllFailedPoliciesFromDBWithFilter(FailPolicyFilterModel policyFilter, out int totalCount, out string exption)
        {
            totalCount = 0;
            exption = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            List<PolicyListingModel> output = new List<PolicyListingModel>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyStatusInfoDetailsWithFilter";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = policyFilter.PageIndex + 1 });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = policyFilter.pageSize });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IsExcel", Value = policyFilter.IsExcel ? 1 : 0 });

                if (policyFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }
                if (policyFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.MerchantId))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@merchantId", Value = policyFilter.MerchantId });

                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@referenceId", Value = policyFilter.ReferenceNo });

                if (policyFilter.InsuranceCompanyId.HasValue)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyID", Value = policyFilter.InsuranceCompanyId.Value });

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredFirstNameAr))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InsuredFirstNameAr", Value = policyFilter.InsuredFirstNameAr });

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredLastNameAr))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InsuredLastNameAr", Value = policyFilter.InsuredLastNameAr });

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredPhone))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InsuredPhone", Value = policyFilter.InsuredPhone });

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredEmail))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InsuredEmail", Value = policyFilter.InsuredEmail });

                if (!string.IsNullOrWhiteSpace(policyFilter.CustomNo))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@CustomNo", Value = policyFilter.CustomNo });

                if (!string.IsNullOrWhiteSpace(policyFilter.SequenceNo))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@SequenceNo", Value = policyFilter.SequenceNo });

                if (policyFilter.ProductTypeId.HasValue)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@productTypeId", Value = policyFilter.ProductTypeId.Value });

                if (policyFilter.InvoiceNo.HasValue)
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InvoiceNo", Value = policyFilter.InvoiceNo.Value });

                if (!string.IsNullOrEmpty(policyFilter.NationalId))
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@NationalId", Value = policyFilter.NationalId });

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                output = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyListingModel>(reader).ToList();

                if (output != null && output.Count > 0 && !policyFilter.IsExcel)
                {
                    //get data count
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                return output;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exption = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        private string GetFailedPoliciesWhereClause(FailPolicyFilterModel filter)
        {
            var qury = new StringBuilder();

            if (filter.EndDate != null && filter.StartDate != null && filter.StartDate?.Date == filter.EndDate?.Date)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND PolicyProcessingQueue.CreatedDate > '{dtStart}' AND PolicyProcessingQueue.CreatedDate < '{dtEnd}'");
            }
            else if (filter.EndDate != null && filter.StartDate != null && filter.StartDate != filter.EndDate)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND PolicyProcessingQueue.CreatedDate >= '{dtStart}' AND PolicyProcessingQueue.CreatedDate <= '{dtEnd}'");
            }
            else if (filter.StartDate.HasValue)
            {
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND PolicyProcessingQueue.CreatedDate >= '{dtStart}'");
            }
            else if (filter.EndDate.HasValue)
            {
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                qury.Append($" AND PolicyProcessingQueue.CreatedDate <= '{dtEnd}'");
            }
            if (!string.IsNullOrWhiteSpace(filter.MerchantId))
            {
                qury.Append($" AND CheckoutDetails.MerchantTransactionId = '{filter.MerchantId}'");
            }
            if (!string.IsNullOrEmpty(filter.NationalId))
                qury.Append($" AND Driver.NIN = '{filter.NationalId}'");

            if (!string.IsNullOrWhiteSpace(filter.ReferenceNo))
                qury.Append($" AND Invoice.ReferenceId = '{filter.ReferenceNo}'");

            if (!string.IsNullOrWhiteSpace(filter.SequenceNo))
                qury.Append($" AND Vehicles.SequenceNumber = '{filter.SequenceNo}'");

            if (!string.IsNullOrWhiteSpace(filter.CustomNo))
                qury.Append($" AND Vehicles.CustomCardNumber = '{filter.CustomNo}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredEmail))
                qury.Append($" AND CheckoutDetails.Email = '{filter.InsuredEmail}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredFirstNameAr))
                qury.Append($" AND Driver.FirstName = '{filter.InsuredFirstNameAr}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredLastNameAr))
                qury.Append($" AND Driver.LastName = '{filter.InsuredLastNameAr}'");

            if (!string.IsNullOrWhiteSpace(filter.InsuredPhone))
                qury.Append($" AND CheckoutDetails.Phone = '{filter.InsuredPhone}'");

            if (filter.InvoiceNo.HasValue)
                qury.Append($" AND Invoice.InvoiceNo = '{filter.InvoiceNo.Value}'");

            if (filter.InsuranceCompanyId.HasValue)
                qury.Append($" AND Invoice.InsuranceCompanyID = '{filter.InsuranceCompanyId.Value}'");

            if (filter.ProductTypeId.HasValue)
                qury.Append($" AND CheckoutDetails.SelectedInsuranceTypeCode = '{filter.ProductTypeId.Value}'");

            if (filter.PolicyStatusId.HasValue)
                qury.Append($" AND CheckoutDetails.PolicyStatusId = '{filter.PolicyStatusId.Value}'");

            if (filter.Channel.HasValue)
            {
                string EnumValue = Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), filter.Channel.Value);
                qury.Append($" AND CheckoutDetails.Channel = '{EnumValue}'");
            }

            if (qury.ToString() == string.Empty)
            {
                filter.EndDate = DateTime.Now;
                filter.StartDate = DateTime.Now;
                DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                qury.Append($" AND PolicyProcessingQueue.CreatedDate >= '{dtStart}' AND PolicyProcessingQueue.CreatedDate <= '{dtEnd}'");
            }

            return qury.ToString();
        }

        /// <summary>
        /// Get User Info By Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/getUserInfoByEmail")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AspNetUserModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetUserInfoByEmail(string email, string userId, string mobile, string sadadNo)        {            AdminRequestLog log = new AdminRequestLog();
            log.PageName = "Users";
            log.PageURL = "/admin/users";
            log.MethodName = "GetUserInfoBy";
            log.ApiURL = Utilities.GetCurrentURL;
            log.UserID = User.Identity.GetUserId();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject($"Email: {email}, UserId: {userId}, Mobile: {mobile}, SadadNo: {sadadNo}");

            try
            {
                AspNetUser user = null;
                if (!string.IsNullOrEmpty(sadadNo))
                {
                    string exception = string.Empty;
                    user = _policyService.GetUserBySadadNo(sadadNo, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 500;
                        log.ErrorDescription = $"Error when try to get data from _policyService.GetUserBySadadNo, and error is: {exception}";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error("Error has occured when getting data from DB, please call Development Team to checkthe logs");
                    }
                    if (user == null)
                    {
                        log.ErrorCode = 200;
                        log.ErrorDescription = "_policyService.GetUserBySadadNo return null result";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(user.ToServiceModel());
                    }
                }
                else
                    user = _authorizationService.GetUserInfoByEmail(email, userId, mobile);

                return Ok(user.ToServiceModel());
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = $"Exception error, and error is: {ex.ToString()}";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("Exception error has occured, please call Development Team to checkthe logs");
            }        }


        /// <summary>
        /// Get User Info By Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/deleteUser")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AspNetUserModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 34)]
        public IActionResult DeleteUser(string mobile)        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "users";
            log.PageURL = "/admin/users";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteUser";
            log.ServiceRequest = $"mobile: {mobile}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try            {
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
                var isAuthorized = _userPageService.IsAuthorizedUser(34, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }                var result = _authorizationService.DeleteUser(mobile);

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result);            }            catch (Exception ex)            {                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");            }        }


        /// <summary>
        /// Get User Info By Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/manageUserLock")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 34)]
        public IActionResult ManageUserLock(string userId, bool toLock,string LockedReason)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Users";
            log.PageURL = "/admin/Users";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ManageUserLock";
            log.ServiceRequest = $"userId: {userId}, toLock: {toLock}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new TameenkArgumentNullException("userId");
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
                var isAuthorized = _userPageService.IsAuthorizedUser(34, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var result = _authorizationService.ManageUserLock(userId, toLock, log.UserName, LockedReason);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return Ok(result);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [HttpPost]
        [Route("api/policy/manageUserMobileVerification")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 34)]
        public IActionResult ManageUserYakeenMobileVerification(YakeenMobileVerificationDahsboardModel model)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Users";
            log.PageURL = "/admin/Users";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ManageUserYakeenMobileVerification";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            
            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "model.UserId is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not Found");
                }
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authorized";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authenticated";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(34, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                string exception = string.Empty;
                var result = _authorizationService.ManageUserMobileVerification(model.Email, model.UserId, model.SetYakeenmobileVerification, out exception);
                if (!result || !string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "result return false";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Service exception: an error has occured");
                }
                
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = $"ManageUserYakeenMobileVerification exception: {ex.ToString()}";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("Exception: an error has occured");
            }
        }

        /// <summary>
        /// Get User Info By Email
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/policy/regeneratePolicy")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 35)]
        public IActionResult RegeneratePolicy(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Regenerate Policy";
            log.PageURL = "/admin/policies/regenerate";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "RegeneratePolicy";
            log.ServiceRequest = $"ReferenceId: {referenceId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("referenceId");

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
                var isAuthorized = _userPageService.IsAuthorizedUser(35, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                string exception = string.Empty;
                if(!_policyService.RegeneratePolicy(referenceId.Trim(),out exception))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("an error has occured, please try again later");
                }
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
        /// resend poliocy file by ewmail
        /// </summary>
        /// <param name="email"></param>
        /// <param name="policyNo"></param>
        /// <param name="fileId"></param>
        /// <param name="referenceId"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [Route("api/policy/reSendPolicyByMail")]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult ReSendPolicyByMail(string email, string policyNo, string fileId, string referenceId, int langCode)
        {

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Success policies";
            log.PageURL = "/admin/policies/success";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReSendPolicyByMail";
            log.ServiceRequest = $"email: {email},policyNo: {policyNo},fileId: {fileId},referenceId: {referenceId},langCode: {langCode}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                if (string.IsNullOrEmpty(fileId))
                    throw new TameenkArgumentNullException("fileId");

                Guid guidResult = new Guid();
                bool isValid = Guid.TryParse(fileId, out guidResult);

                if (!isValid)
                    throw new TameenkArgumentException("Not Vaild File ID", "fileId");
                bool sendPolicy = false;
                var policyFile = _policyService.DownloadPolicyFile(fileId);
                if (policyFile != null)
                {
                    if (policyFile.PolicyFileByte != null)
                    {

                        sendPolicy = _policyContext.ResendPolicyFileByMail(policyFile.PolicyFileByte, email, policyNo, referenceId, string.Empty, langCode);
                        if (!sendPolicy)
                            return Ok("Failed to send email");
                        return Ok(true);
                    }
                    else if (!string.IsNullOrEmpty(policyFile.FilePath))
                    {
                        //server 2
                        if (tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            FileNetworkShare fileShare = new FileNetworkShare();
                            string exception = string.Empty;
                            var file = fileShare.GetFile(policyFile.FilePath, out exception);
                            if (file != null)
                            {
                                sendPolicy = _policyContext.ResendPolicyFileByMail(file, email, policyNo, referenceId, string.Empty, langCode);
                                if (!sendPolicy)
                                    return Ok("Failed to send email");
                                return Ok(true);
                            }
                            else
                                return Error("No PolicyFileByte or FilePath for this id " + fileId + " exception " + exception); 
                        }
                        sendPolicy = _policyContext.ResendPolicyFileByMail(policyFile.PolicyFileByte, email, policyNo, referenceId, string.Empty, langCode);
                        if (!sendPolicy)
                            return Ok("Failed to send email");
                        return Ok(true);
                    }
                    else
                    {
                        return Error("No PolicyFileByte or FilePath for this id " + fileId);
                    }
                }
                else
                {
                    return Error("policyFile is null with this id " + fileId);
                }

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
        /// get policy details by policy no
        /// </summary>
        /// <param name="policyNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/policy-detailsByPolicyNo")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<SuccessPolicyModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetDetailsToSuccessPoliciesByPolicyNo(string policyNo)
        {
            try
            {
                if (string.IsNullOrEmpty(policyNo))
                    throw new TameenkArgumentNullException("PolicyNo");

                var policyData = _policyService.GetPolicyByPolicyNo(policyNo);

                if (policyData == null)
                    return Ok("");

                var result = _policyService.GetDetailsToSuccessPolicies(policyData.CheckOutDetailsId);
                return Ok(result.ToServiceModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }


        /// <summary>
        /// reupload policy file
        /// </summary>
        /// <param name="policyModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SuccessPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [Route("api/policy/reUploadPolicyFile")]
        [AdminAuthorizeAttribute(pageNumber: 42)]
        public IActionResult ReUploadPolicyFile([FromBody]SuccessPolicyModel policyModel)
        {
           Tameenk.Services.Policy.Components.PdfGenerationOutput output = new Tameenk.Services.Policy.Components.PdfGenerationOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Re-Upload Policy File";
            log.PageURL = "/admin/policies/reupload";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReUploadPolicyFile";
            log.ServiceRequest = JsonConvert.SerializeObject(policyModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyModel.CheckOutDetailsId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(42, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                var policyFile = _policyService.GetPolicyFileByFileID(policyModel.PolicyFileId.ToString());
                if (policyFile==null)
                {
                    output.ErrorCode = Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "policyFile is null";
                    log.ErrorCode = 8;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                output = _policyContext.ReplacePolicyFile(policyModel.PoloicyFileToUpload,policyFile.FilePath, tameenkConfig.RemoteServerInfo.ServerIP, tameenkConfig.RemoteServerInfo.UseNetworkDownload, tameenkConfig.RemoteServerInfo.ServerUserName, tameenkConfig.RemoteServerInfo.DomainName, tameenkConfig.RemoteServerInfo.ServerPassword);
                if(output.ErrorCode!=Policy.Components.PdfGenerationOutput.ErrorCodes.Success)
                {
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ErrorCode = 7;
                    log.ErrorDescription = "Failed to replace file due to "+ output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Tameenk.Services.Policy.Components.PdfGenerationOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");

            }
        }


        /// <summary>
        /// Update User Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/policy/updateuserMail")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 34)]
        public IActionResult UpdateUserMail(string email, string userId,string phoneNumber)
        {
            var outPut = new PolicyOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "UpdateUserMail";
            log.PageURL = "/admin/users";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "UpdateUserMail";
            log.ServiceRequest = $"email:{email}, userid:{userId}, phoneNumber:{phoneNumber}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            //log.ReferenceId = policyFilter.ReferenceNo;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    outPut.ErrorCode = 2;
                    outPut.ErrorDescription = "User not authorized";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = outPut.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    outPut.ErrorCode = 3;
                    outPut.ErrorDescription = "User not authenticated";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = outPut.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(34, log.UserID);
                if (!isAuthorized)
                {
                    outPut.ErrorCode = 4;
                    outPut.ErrorDescription = "User not authenticated";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }
                if (!string.IsNullOrEmpty(email))
                {
                    if (!Utilities.IsValidMail(email))
                    {
                        outPut.ErrorCode = 5;
                        outPut.ErrorDescription = "Wrong email format";
                        log.ErrorCode = outPut.ErrorCode;
                        log.ErrorDescription = "Wrong email format : " + email;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(outPut);
                    }
                    var exist = _authorizationService.CheckEmailExist(email);
                    if (exist)
                    {
                        outPut.ErrorCode = 5;
                        outPut.ErrorDescription = "email already exist";
                        log.ErrorCode = outPut.ErrorCode;
                        log.ErrorDescription = "email exist: " + email;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(outPut);
                    }
                   
                }
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    if (!Utilities.IsValidPhoneNo(phoneNumber))
                    {
                        outPut.ErrorCode = 5;
                        outPut.ErrorDescription = "Wrong phone number format";
                        log.ErrorCode = outPut.ErrorCode;
                        log.ErrorDescription = "Wrong phone number format : " + phoneNumber;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(outPut);
                    }
                    var exist = _authorizationService.CheckPhoneExist(phoneNumber);
                    if (exist)
                    {
                        outPut.ErrorCode = 5;
                        outPut.ErrorDescription = "phone already exist";
                        log.ErrorCode = outPut.ErrorCode;
                        log.ErrorDescription = "phone exist: " + phoneNumber;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Error(outPut);
                    }
                    phoneNumber = Utilities.ValidatePhoneNumber(phoneNumber);
                    phoneNumber = "0" + phoneNumber.Substring(Utilities.SaudiInternationalPhoneCode.Length); 
                }
                var result = _authorizationService.UpdateUserEmail(email, userId, phoneNumber);
                if (!result)
                {
                    outPut.ErrorCode = 6;
                    outPut.ErrorDescription = "Failed to update email or Phone number";
                    log.ErrorCode = outPut.ErrorCode;
                    log.ErrorDescription = outPut.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(outPut);
                }
                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "success";
                log.ErrorCode = outPut.ErrorCode;
                log.ErrorDescription = outPut.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                outPut.ErrorCode =7;
                outPut.ErrorDescription = "service exception";
                log.ErrorCode = outPut.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(outPut);
            }
        }

        #region New Success Policies Info

        /// <summary>
        /// this action to get new success policies ifno
        /// </summary>
        /// <param name="policyFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/policy/success-policiesInfo-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<List<SuccessPoliciesInfoListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GetSuccessPoliciesInfoWithFilter([FromBody]SuccessPoliciesInfoFilterModel policyFilter, int pageIndex = 0, int pageSize = 10)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Policies Info";
            log.PageURL = "/admin/policies/success-new";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSuccessPoliciesInfoWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;                string exception = string.Empty;                var result = GetAllPoliciesInfoFromDBWithFilter(policyFilter, pageIndex, pageSize, false, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error");
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        /// <summary>
        /// this action to generate excel file with success pilicies info data
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/success-policiesInfo-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
        [AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GenerateExcelSheetForSuccessPoliciesInfo(SuccessPoliciesInfoFilterModel filter, string lang)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Policies Info";
            log.PageURL = "/admin/policies/success-new";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GenerateExcelSheetForSuccessPoliciesInfo";
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
                var isAuthorized = _userPageService.IsAuthorizedUser(6, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                int count = 0;
                string exception = string.Empty;
                var policies = GetAllPoliciesInfoFromDBWithFilter(filter, 0, int.MaxValue, true, out count, out exception);
                if (policies == null)
                    return Ok("");

                byte[] file = _excelService.GenerateExcelSuccessPoliciesInfo(policies, lang);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        private List<SuccessPoliciesInfoListingModel> GetAllPoliciesInfoFromDBWithFilter(SuccessPoliciesInfoFilterModel policyFilter, int pageIndex, int pageSize, bool export, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllPoliciesInfoFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60*60*60;

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (policyFilter.InsuranceCompanyId > 0)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }

                if (!string.IsNullOrEmpty(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (policyFilter.NajmStatusId > 0)
                {
                    SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId };
                    command.Parameters.Add(NajmStatusIdParameter);
                }

                if (policyFilter.InvoiceNo > 0)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (policyFilter.paymentMethodId > 0)
                {
                    SqlParameter PaymentMethodIdParameter = new SqlParameter() { ParameterName = "PaymentMethodId", Value = policyFilter.paymentMethodId };
                    command.Parameters.Add(PaymentMethodIdParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = (export == true) ? 1 : 0 };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<SuccessPoliciesInfoListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<SuccessPoliciesInfoListingModel>(reader).ToList();

                if (filteredData != null && filteredData.Count > 0 && !export)
                {
                    //get data count
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                return filteredData;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        #region Sama Report Page Methods

        [Authorize]
        [HttpPost]
        [Route("api/policy/sama-report-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SamaReportPoliciesListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 49)]
        public IActionResult GetSamaReportPoliciesWithFilter([FromBody]SamaReportPoliciesFIlter policyFilter, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyFilter.InsuranceCompanyId;
            log.PageName = "Sama Report";
            log.PageURL = "/admin/policies/sama-report";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSamaReportPoliciesWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(49, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                int totalCount = 0;                string exception = string.Empty;                var result = GetSamaReportPoliciesFromDBWithFilter(policyFilter, pageIndex, pageSize, false, 240, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(exception);
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/policy/sama-report-policies-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SamaReportPoliciesListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 49)]
        public IActionResult GetSamaReportPoliciesExcel([FromBody]SamaReportPoliciesFIlter policyFilter, string lang)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyFilter.InsuranceCompanyId;
            log.PageName = "Sama Report";
            log.PageURL = "/admin/policies/sama-report";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSamaReportPoliciesExcel";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(49, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                int totalCount = 0;                string exception = string.Empty;                var result = GetSamaReportPoliciesFromDBWithFilter(policyFilter, 0, int.MaxValue, true, 240, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.GenerateSamaReportPoliciesExcel(result, lang);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }

        private List<SamaReportPoliciesListingModel> GetSamaReportPoliciesFromDBWithFilter(SamaReportPoliciesFIlter policyFilter, int pageIndex, int pageSize, bool export, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetSamaReportPoliciesFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (policyFilter.StartDate.HasValue)
                {
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "StartDateString", Value = policyFilter.StartDate.Value.ToString("yyyy-MM-dd 00:00:00") };
                    command.Parameters.Add(startDateParameter);
                }

                if (policyFilter.EndDate.HasValue)
                {
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "EndDateString", Value = policyFilter.EndDate.Value.ToString("yyyy-MM-dd 23:59:59") };
                    command.Parameters.Add(endDateParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = policyFilter.NationalId };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.SequenceNo))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "SequenceNo", Value = policyFilter.SequenceNo };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.CustomNo))
                {
                    SqlParameter CustomNoParameter = new SqlParameter() { ParameterName = "CustomNo", Value = policyFilter.CustomNo };
                    command.Parameters.Add(CustomNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredEmail))
                {
                    SqlParameter InsuredEmailParameter = new SqlParameter() { ParameterName = "InsuredEmail", Value = policyFilter.InsuredEmail };
                    command.Parameters.Add(InsuredEmailParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredFirstNameAr))
                {
                    SqlParameter InsuredFirstNameArParameter = new SqlParameter() { ParameterName = "InsuredFirstNameAr", Value = policyFilter.InsuredFirstNameAr };
                    command.Parameters.Add(InsuredFirstNameArParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.InsuredLastNameAr))
                {
                    SqlParameter InsuredLastNameArParameter = new SqlParameter() { ParameterName = "InsuredLastNameAr", Value = policyFilter.InsuredLastNameAr };
                    command.Parameters.Add(InsuredLastNameArParameter);
                }

                if (policyFilter.NajmStatusId.HasValue)
                {
                    SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId };
                    command.Parameters.Add(NajmStatusIdParameter);
                }

                if (policyFilter.InvoiceNo.HasValue)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }

                if (policyFilter.ProductTypeId.HasValue)
                {
                    SqlParameter ProductTypeIdParameter = new SqlParameter() { ParameterName = "ProductTypeId", Value = policyFilter.ProductTypeId };
                    command.Parameters.Add(ProductTypeIdParameter);
                }

                if (policyFilter.Channel.HasValue)
                {
                    SqlParameter ChannelParameter = new SqlParameter() { ParameterName = "Channel", Value = Enum.GetName(typeof(Tameenk.Common.Utilities.Channel), policyFilter.Channel.Value) };
                    command.Parameters.Add(ChannelParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = (export == true) ? 1 : 0 };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<SamaReportPoliciesListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<SamaReportPoliciesListingModel>(reader).ToList();

                if (filteredData != null && filteredData.Count > 0 && !export)
                {
                    //get data count
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                return filteredData;
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message, exp);
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        #endregion

        #endregion

        #region Sams-Statistics-Report

        [HttpPost]
        [Route("api/policy/SamaStatisticsReport")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<SamaStatisticsCountReport>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult SamaStatisticsReport([FromBody] SamaStatisticsReportModel model)
        {
            var output = new PolicyOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Sama Statistics";
            log.PageURL = "/admin/policies/sama-statistics-report";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "SamaStatisticsReport";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            // log.ReferenceId = model.ReferenceId;

            try
            {
                if (model == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Model is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.StartDate.ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "StartDate is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.EndDate.ToString()))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "EndDate is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }

                string exception = string.Empty;
                var result = SamaStatisticsCountReportRequestService(model.ToModel(), out exception);
                if (result == null)
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = "Search SamaStatisticsCountReportRequestService return null result";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = "No Data found in DB";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "Search Error when GetData From DB, please call Development Team to check the logs";
                    log.ErrorCode = 5;
                    log.ErrorDescription = "Search Error when GetData From DB" + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                output.ErrorCode = (int)log.ErrorCode;
                output.ErrorDescription = log.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(output);
            }
        }


        #endregion

        #region Sams-StatisticsExel-Report

        [HttpPost]
        [Route("api/policy/SamaStatisticsExcelReport")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<PolicyOutput>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult SamaStatisticsExcelReport([FromBody] SamaStatisticsReportModel model)
        {
            var output = new PolicyOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Sama Statistics";
            log.PageURL = "/admin/policies/sama-statistics-report";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "SamaStatisticsExcelReport";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            // log.ReferenceId = model.ReferenceId;

            try
            {
                if (model == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Model is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.StartDate.ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "StartDate is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    return Error(output);
                }
                if (string.IsNullOrEmpty(model.EndDate.ToString()))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "EndDate is null";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = log.ErrorDescription;
                    return Error(output);
                }

                string exception = string.Empty;
                model.IsExcel = 1;
                var result = SamaStatisticsCountReportRequestService(model.ToModel(), out exception);
                if (result == null)
                {
                    log.ErrorCode = 5;
                    log.ErrorDescription = "Export SamaStatisticsCountReportRequestService return null result";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = "No Data found in DB";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "Export Error when GetData From DB, please call Development Team to check the logs";
                    log.ErrorCode = 5;
                    log.ErrorDescription = "Export Error when GetData From DB" + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                
                byte[] file = _excelService.GenerateSamaStatisticsReportExcel(result);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "file == null or file.Length <= 0";
                    output.ErrorCode = (int)log.ErrorCode;
                    output.ErrorDescription = "Export Error when try to generate excel data, please call Development Team to check the logs";
                    return Ok(output);
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = (int)log.ErrorCode;
                output.ErrorDescription = log.ErrorDescription;
                log.ErrorCode = 500;
                log.ErrorDescription = $"Exception error when export sama report, and error is: {ex.ToString()}";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(output);
            }
        }

        public SamaStatisticsCountReport SamaStatisticsCountReportRequestService(SamaStatisticsReportFilter model, out string exception)
        {
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                exception = string.Empty;
                dbContext.DatabaseInstance.CommandTimeout = 15 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "SamaStatisticsCountReport";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 7 * 60;

                if (model.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, model.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@StartDate", Value = dtStart });
                }
                if (model.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@EndDate", Value = dtEnd });
                }
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IsExcel", Value = model.IsExcel });

                dbContext.DatabaseInstance.Connection.Open();

                var reader = command.ExecuteReader();
                SamaStatisticsCountReport statisticsCountReport = new SamaStatisticsCountReport();
                TotalIndividualPoliciesTPLSandWafi_Male_Model sand_Wafi_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualPoliciesTPLSandWafi_Male_Model>(reader).FirstOrDefault<TotalIndividualPoliciesTPLSandWafi_Male_Model>();
                reader.NextResult();
                TotalIndividualPoliciesTPLSandWafi_Female_Model sand_Wafi_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualPoliciesTPLSandWafi_Female_Model>(reader).FirstOrDefault<TotalIndividualPoliciesTPLSandWafi_Female_Model>();
                reader.NextResult();
                TotalIndividualPoliciesComprehensive_Male_Model comprehensive_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualPoliciesComprehensive_Male_Model>(reader).FirstOrDefault<TotalIndividualPoliciesComprehensive_Male_Model>();
                reader.NextResult();
                TotalIndividualPoliciesComprehensive_Female_Model comprehensive_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualPoliciesComprehensive_Female_Model>(reader).FirstOrDefault<TotalIndividualPoliciesComprehensive_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_17_18_Male_Model age_17_18_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_17_18_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_17_18_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_17_18_Female_Model age_17_18_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_17_18_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_17_18_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_19_28_Male_Model age_19_28_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_19_28_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_19_28_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_19_28_Female_Model age_19_28_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_19_28_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_19_28_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_29_38_Male_Model age_29_38_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_29_38_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_29_38_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_29_38_Female_Model age_29_38_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_29_38_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_29_38_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_39_48_Male_Model age_39_48_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_39_48_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_39_48_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_39_48_Female_Model  age_39_48_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_39_48_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_39_48_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_49_58_Male_Model age_49_58_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_49_58_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_49_58_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_49_58_Female_Model age_49_58_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_49_58_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_49_58_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_59_68_Male_Model age_59_68_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_59_68_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_59_68_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_59_68_Female_Model age_59_68_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_59_68_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_59_68_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_69_78_Male_Model age_69_78_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_69_78_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_69_78_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_69_78_Female_Model age_69_78_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_69_78_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_69_78_Female_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_80_83_Male_Model age_80_83_Male = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_80_83_Male_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_80_83_Male_Model>();
                reader.NextResult();
                TotalSuccessPoliciesPerAge_80_83_Female_Model age_80_83_Female = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesPerAge_80_83_Female_Model>(reader).FirstOrDefault<TotalSuccessPoliciesPerAge_80_83_Female_Model>();
                reader.NextResult();
                TotalPoliciesVisaMasterAmex_CreditCardModel Amex_CreditCard = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesVisaMasterAmex_CreditCardModel>(reader).FirstOrDefault<TotalPoliciesVisaMasterAmex_CreditCardModel>();
                reader.NextResult();
                TotalPoliciesSadadModel policiesSadadModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesSadadModel>(reader).FirstOrDefault<TotalPoliciesSadadModel>();
                reader.NextResult();
                TotalPoliciesMadaModel policiesMadaModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesMadaModel>(reader).FirstOrDefault<TotalPoliciesMadaModel>();
                reader.NextResult();
                TotalPoliciesWalletModel policiesWalletModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesWalletModel>(reader).FirstOrDefault<TotalPoliciesWalletModel>();
                reader.NextResult();
                TotalCorporateUsersInTheSystemModel totalCorprateInTheSystem = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalCorporateUsersInTheSystemModel>(reader).FirstOrDefault<TotalCorporateUsersInTheSystemModel>();
                reader.NextResult();
                TotalIndividualUserInTheSystemModel totalIndividualInTheSystem= ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualUserInTheSystemModel>(reader).FirstOrDefault<TotalIndividualUserInTheSystemModel>();
                reader.NextResult();
                TotalIndividualUsersIntervalModel totalUsersIndividualInterval = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualUsersIntervalModel>(reader).FirstOrDefault<TotalIndividualUsersIntervalModel>();
                reader.NextResult();
                TotalCorporateUsersIntervalModel totalUsersCorprateInterval = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalCorporateUsersIntervalModel>(reader).FirstOrDefault<TotalCorporateUsersIntervalModel>();
                reader.NextResult();
                TotalIndividualUsersPurchasedPoliciesIntervalModel policiesIndividualInterval = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalIndividualUsersPurchasedPoliciesIntervalModel>(reader).FirstOrDefault<TotalIndividualUsersPurchasedPoliciesIntervalModel>();
                reader.NextResult();
                TotalCorporateUsersPurchasedPoliciesIntervalModel policiesCorporateInterval = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalCorporateUsersPurchasedPoliciesIntervalModel>(reader).FirstOrDefault<TotalCorporateUsersPurchasedPoliciesIntervalModel>();
                reader.NextResult();
                TotalPoliciesPortalModel policiesPortalModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesPortalModel>(reader).FirstOrDefault<TotalPoliciesPortalModel>();
                reader.NextResult();
                TotalPoliciesMobileModel policiesMobileModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalPoliciesMobileModel>(reader).FirstOrDefault<TotalPoliciesMobileModel>();
                if (model.IsExcel == 1)
                {
                    reader.NextResult();
                    List<TotalSuccessPoliciesIndividualPerCityModel> list1 = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesIndividualPerCityModel>(reader).ToList<TotalSuccessPoliciesIndividualPerCityModel>();
                    reader.NextResult();
                    List<TotalSuccessPoliciesCorporatePerCityModel> list2 = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<TotalSuccessPoliciesCorporatePerCityModel>(reader).ToList<TotalSuccessPoliciesCorporatePerCityModel>();
                    List<TotalPolicesPerCity> totalPolicesPerCityList1 = new List<TotalPolicesPerCity>();
                    List<TotalPolicesPerCity> totalPolicesPerCityList2 = new List<TotalPolicesPerCity>();
                    int index1 = 1;
                    foreach (TotalSuccessPoliciesIndividualPerCityModel individualPerCityModel in list1)
                    {
                        TotalPolicesPerCity totalPolicesPerCity = new TotalPolicesPerCity()
                        {
                            TotalPolices = individualPerCityModel.TotalSuccessPoliciesIndividualPerCity,
                            RegionName = individualPerCityModel.RegionName,
                            Index = index1
                        };
                        totalPolicesPerCityList1.Add(totalPolicesPerCity);
                        index1++;
                    }
                    int index2 = 1;
                    foreach (TotalSuccessPoliciesCorporatePerCityModel corporatePerCityModel in list2)
                    {
                        TotalPolicesPerCity totalPolicesPerCity = new TotalPolicesPerCity()
                        {
                            TotalPolices = corporatePerCityModel.TotalSuccessPoliciesCorporatePerCity,
                            RegionName = corporatePerCityModel.RegionName,
                            Index = index2
                        };
                        totalPolicesPerCityList2.Add(totalPolicesPerCity);
                        index2++;
                    }
                    statisticsCountReport.TotalIndividualPolicesPerCity = totalPolicesPerCityList1;
                    statisticsCountReport.TotalCorpratePolicesPerCity = totalPolicesPerCityList2;
                }
                TotalPolicesPerGenderModel policesPerGenderModel1 = new TotalPolicesPerGenderModel()
                {
                    Item = sand_Wafi_Male.Item,
                    TotalPolicesPerGenderMale = sand_Wafi_Male.TotalIndividualPoliciesTPLSandWafi_Male,
                    TotalPolicesPerGenderFemale = sand_Wafi_Female.TotalIndividualPoliciesTPLSandWafi_Female,
                    Index = 1
                };
                TotalPolicesPerGenderModel policesPerGenderModel2 = new TotalPolicesPerGenderModel()
                {
                    Item = comprehensive_Male.Item,
                    TotalPolicesPerGenderMale = comprehensive_Male.TotalIndividualPoliciesComprehensive_Male,
                    TotalPolicesPerGenderFemale = comprehensive_Female.TotalIndividualPoliciesComprehensive_Female,
                    Index = 2
                };
                List<TotalPolicesPerGenderModel> policesPerGenderModelList = new List<TotalPolicesPerGenderModel>();
                policesPerGenderModelList.Add(policesPerGenderModel1);
                policesPerGenderModelList.Add(policesPerGenderModel2);
                List<TotalcontPerAgeForAllRanges> perAgeForAllRangesList = new List<TotalcontPerAgeForAllRanges>();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_17_18 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_19_28 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_29_38 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_39_48 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_49_58 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_59_68 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_69_78 = new TotalcontPerAgeForAllRanges();
                TotalcontPerAgeForAllRanges perAgeForAllRanges_80_83 = new TotalcontPerAgeForAllRanges();
                perAgeForAllRanges_17_18.range = age_17_18_Male.rangeItem;
                perAgeForAllRanges_17_18.totalCountMale = age_17_18_Male.TotalSuccessPoliciesPerAge_17_18_Male;
                perAgeForAllRanges_17_18.totalCountFemale = age_17_18_Female.TotalSuccessPoliciesPerAge_17_18_Female;
                perAgeForAllRanges_17_18.Index = 1;
                perAgeForAllRanges_19_28.range = age_19_28_Male.rangeItem;
                perAgeForAllRanges_19_28.totalCountMale = age_19_28_Male.TotalSuccessPoliciesPerAge_19_28_Male;
                perAgeForAllRanges_19_28.totalCountFemale = age_19_28_Female.TotalSuccessPoliciesPerAge_19_28_Female;
                perAgeForAllRanges_19_28.Index = 2;
                perAgeForAllRanges_29_38.range = age_29_38_Male.rangeItem;
                perAgeForAllRanges_29_38.totalCountMale = age_29_38_Male.TotalSuccessPoliciesPerAge_29_38_Male;
                perAgeForAllRanges_29_38.totalCountFemale = age_29_38_Female.TotalSuccessPoliciesPerAge_29_38_Female;
                perAgeForAllRanges_29_38.Index = 3;
                perAgeForAllRanges_39_48.range = age_39_48_Male.rangeItem;
                perAgeForAllRanges_39_48.totalCountMale = age_39_48_Male.TotalSuccessPoliciesPerAge_39_48_Male;
                perAgeForAllRanges_39_48.totalCountFemale = age_39_48_Female.TotalSuccessPoliciesPerAge_39_48_Female;
                perAgeForAllRanges_39_48.Index = 4;
                perAgeForAllRanges_49_58.range = age_49_58_Male.rangeItem;
                perAgeForAllRanges_49_58.totalCountMale = age_49_58_Male.TotalSuccessPoliciesPerAge_49_58_Male;
                perAgeForAllRanges_49_58.totalCountFemale = age_49_58_Female.TotalSuccessPoliciesPerAge_49_58_Female;
                perAgeForAllRanges_49_58.Index = 5;
                perAgeForAllRanges_59_68.range = age_59_68_Male.rangeItem;
                perAgeForAllRanges_59_68.totalCountMale = age_59_68_Male.TotalSuccessPoliciesPerAge_59_68_Male;
                perAgeForAllRanges_59_68.totalCountFemale = age_59_68_Female.TotalSuccessPoliciesPerAge_59_68_Female;
                perAgeForAllRanges_59_68.Index = 6;
                perAgeForAllRanges_69_78.range = age_69_78_Male.rangeItem;
                perAgeForAllRanges_69_78.totalCountMale = age_69_78_Male.TotalSuccessPoliciesPerAge_69_78_Male;
                perAgeForAllRanges_69_78.totalCountFemale = age_69_78_Female.TotalSuccessPoliciesPerAge_69_78_Female;
                perAgeForAllRanges_69_78.Index = 7;
                perAgeForAllRanges_80_83.range = age_80_83_Male.rangeItem;
                perAgeForAllRanges_80_83.totalCountMale = age_80_83_Male.TotalSuccessPoliciesPerAge_80_83_Male;
                perAgeForAllRanges_80_83.totalCountFemale = age_80_83_Female.TotalSuccessPoliciesPerAge_80_83_Female;
                perAgeForAllRanges_80_83.Index = 8;
                perAgeForAllRangesList.Add(perAgeForAllRanges_17_18);
                perAgeForAllRangesList.Add(perAgeForAllRanges_19_28);
                perAgeForAllRangesList.Add(perAgeForAllRanges_29_38);
                perAgeForAllRangesList.Add(perAgeForAllRanges_39_48);
                perAgeForAllRangesList.Add(perAgeForAllRanges_49_58);
                perAgeForAllRangesList.Add(perAgeForAllRanges_59_68);
                perAgeForAllRangesList.Add(perAgeForAllRanges_69_78);
                perAgeForAllRangesList.Add(perAgeForAllRanges_80_83);
                AllPaymentMethodModel paymentMethodAmex_CreditCard = new AllPaymentMethodModel()
                {
                    paymentMethod = Amex_CreditCard.PaymentMethod,
                    totalPolices = Amex_CreditCard.TotalPoliciesVisaMasterAmex_CreditCard,
                    Index = 1
                };
                AllPaymentMethodModel paymentMethodSadad = new AllPaymentMethodModel()
                {
                    paymentMethod = policiesSadadModel.PaymentMethod,
                    totalPolices = policiesSadadModel.TotalPoliciesSadad,
                    Index = 2
                };
                AllPaymentMethodModel paymentMethodMada = new AllPaymentMethodModel()
                {
                    paymentMethod = policiesMadaModel.PaymentMethod,
                    totalPolices = policiesMadaModel.TotalPoliciesMada,
                    Index = 3
                };
                AllPaymentMethodModel paymentMethodWalle = new AllPaymentMethodModel()
                {
                    paymentMethod = policiesWalletModel.PaymentMethod,
                    totalPolices = policiesWalletModel.TotalPoliciesWallet,
                    Index = 4
                };
                List<AllPaymentMethodModel> paymentMethodModelList = new List<AllPaymentMethodModel>();
                paymentMethodModelList.Add(paymentMethodAmex_CreditCard);
                paymentMethodModelList.Add(paymentMethodSadad);
                paymentMethodModelList.Add(paymentMethodMada);
                paymentMethodModelList.Add(paymentMethodWalle);
                TotalInSystemModel totalusersInSystem= new TotalInSystemModel()
                {
                    Item = totalCorprateInTheSystem.Item,
                    TotalIndividualUsersInTheSystem = totalIndividualInTheSystem.TotalIndividualUsersInTheSystem,
                    TotalCorporateUsersInTheSystem = totalCorprateInTheSystem.TotalCorporateUsersInTheSystem,
                    Index = 1
                };
                TotalInSystemModel totalUsersInSystemIterval = new TotalInSystemModel()
                {
                    Item = totalUsersIndividualInterval.Item,
                    TotalIndividualUsersInTheSystem = totalUsersIndividualInterval.TotalIndividualUsersInterval,
                    TotalCorporateUsersInTheSystem = totalUsersCorprateInterval.TotalCorporateUsersInterval,
                    Index = 2
                };
                TotalInSystemModel totalUsersPurchasedPolicies = new TotalInSystemModel()
                {
                    Item = policiesIndividualInterval.Item,
                    TotalIndividualUsersInTheSystem = policiesIndividualInterval.TotalIndividualUsersPurchasedPoliciesInterval,
                    TotalCorporateUsersInTheSystem = policiesCorporateInterval.TotalCorporateUsersPurchasedPoliciesInterval,
                    Index = 3
                };
                List<TotalInSystemModel> totalInSystemModelList = new List<TotalInSystemModel>();
                totalInSystemModelList.Add(totalusersInSystem);
                totalInSystemModelList.Add(totalUsersInSystemIterval);
                totalInSystemModelList.Add(totalUsersPurchasedPolicies);
                AllChannelModel policiesPortal = new AllChannelModel()
                {
                    TotalPurchasedPolicies = policiesPortalModel.TotalPoliciesPortal,
                    Channel = policiesPortalModel.Channel,
                    Index = 1
                };
                AllChannelModel policiesMobile = new AllChannelModel()
                {
                    TotalPurchasedPolicies = policiesMobileModel.TotalPoliciesMobile,
                    Channel = policiesMobileModel.Channel,
                    Index = 2
                };
                List<AllChannelModel> allChannelModelList = new List<AllChannelModel>();
                allChannelModelList.Add(policiesPortal);
                allChannelModelList.Add(policiesMobile);
                statisticsCountReport.AllAgeRanges = perAgeForAllRangesList;
                statisticsCountReport.AllChannelModel = allChannelModelList;
                statisticsCountReport.AllPaymentMethodModel = paymentMethodModelList;
                statisticsCountReport.totalInSystemModel = totalInSystemModelList;
                statisticsCountReport.TotalPolicesPerGenderModel = policesPerGenderModelList;
                return statisticsCountReport;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return (SamaStatisticsCountReport)null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion

        #region Renenwal

        [HttpPost]
        [Route("api/policy/all-renewals-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<RenewalDataOutput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        //[AdminAuthorizeAttribute(pageNumber: 6)]
        public IActionResult GetAllRenewalDataWithFilter([FromBody]RenewalFiltrationModel filterModel, int pageIndex = 1, int pageSize = 10)
        {
            AdministrationOutput<RenewalDataOutput> Output = new AdministrationOutput<RenewalDataOutput>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = (filterModel.InsuranceCompanyId.HasValue) ? filterModel.InsuranceCompanyId : null;
            log.PageName = "Renewal Details";
            log.PageURL = "/promotion/approvals";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = (filterModel.Export) ? "ExportRenewalPoliciesWithFilter" : "GetAllRenewalPoliciesWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(65, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                int totalCount = 0;                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.GetAllRenewalPoliciesFromDBWithFilter(filterModel, pageIndex, pageSize, 60 * 60, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new RenewalDataOutput();
                if (filterModel.Export)
                {
                    if (result == null || (result.RenewalData == null || result.RenewalData.Count == 0))
                    {
                        Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.NullResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(filterModel.Lang));
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "No data found for this filteration model";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(Output);
                    }

                    byte[] file = _excelService.ExportRenewalData(result.RenewalData, filterModel.Lang);
                    if (file == null || file.Length == 0)
                    {
                        Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.NullResult;
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(filterModel.Lang));
                        log.ErrorCode = (int)Output.ErrorCode;
                        log.ErrorDescription = "File is null or file length is 0";
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(Output);
                    }

                    Output.Result.ExportBase64String = Convert.ToBase64String(file);
                    Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.Success;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Success";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Output);
                }
                
                Output.Result = result;
                Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<RenewalDataOutput>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }


        #endregion

        [HttpPost]
        [Route("api/policy/getPolicyNajmResponseTime")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<NajmResponseOutput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 66)]
        public IActionResult GetPolicyNajmResponseTime([FromBody] NajmResponseTimeFilter NajmPolicyFilter, int pageIndex = 0, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)        {            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "NajmResponseTime";            log.PageURL = "/policies/getPolicyNajmResponseTime";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetPolicyNajmResponseTime";            log.ServiceRequest = JsonConvert.SerializeObject(NajmPolicyFilter);            string currentUserId = _authorizationService.GetUserId(User);            if (string.IsNullOrEmpty(currentUserId))            {                currentUserId = User.Identity.GetUserId();            }            log.UserID = currentUserId;            int totalCount = 0;            try            {                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");                if (NajmPolicyFilter == null)                    throw new TameenkArgumentNullException("policyFilter");                string exception = string.Empty;                if (this._policyService == null)                {                    return Error("_policyService is null");                }

                DateTime dtBeforeCalling = DateTime.Now;                var result = _policyContext.GetNajmResponseTimeForConnectWithPolicy(NajmPolicyFilter, pageIndex, pageSize, 60 * 60, out exception);                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                if (!string.IsNullOrEmpty(exception))                {                    log.ErrorCode = 11;                    log.ErrorDescription = exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    result.ErrorCode = NajmResponseOutput.ErrorCodes.ExceptionError;                    result.ErrorDescription = "exption  Error ";                    return Error(result);

                }                if (result == null)                {                    log.ErrorCode = 12;                    log.ErrorDescription = "Result is NULL";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    result.ErrorCode = NajmResponseOutput.ErrorCodes.NullResult;                    result.ErrorDescription = "Result is NULL";                    return Error(result);                }

               if (NajmPolicyFilter.Exports)
                {
                    result.File = _excelService.GetNajmResponseTimeForConnectWithPolicyExcel(result.Result);
                }                log.ErrorCode = 1;                log.ErrorDescription = "Success";                log.ServiceResponse = JsonConvert.SerializeObject(result);                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                result.ErrorCode = NajmResponseOutput.ErrorCodes.Success;                result.ErrorDescription = "success";                return Ok(result);            }            catch (Exception ex)            {                log.ErrorCode = 500;                log.ErrorDescription = ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en")));            }            finally            {                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);            }        }
        [HttpPost]
        [Route("api/policy/getPolicyNotificationResponse")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyNotificationOutput>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 67)]
        public IActionResult GetPolicyNotificationResponse([FromBody] PolicyNotificationFilter NajmNotificationPolicyFilter, int pageIndex = 0, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "PolicyNotificationResponse";
            log.PageURL = "/policies/getPolicyNotificationResponse";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPolicyNotificationResponse";
            log.ServiceRequest = JsonConvert.SerializeObject(NajmNotificationPolicyFilter);
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }

            log.UserID = currentUserId;
            int totalCount = 0;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
                if (NajmNotificationPolicyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                string exception = string.Empty;                if (this._policyService == null)
                {
                    return Error("_policyService is null");
                }                var result = _policyContext.GetpolicyNotificationLog(NajmNotificationPolicyFilter, pageIndex, pageSize, 240, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    result.ErrorCode = PolicyNotificationOutput.ErrorCodes.ExceptionError;
                    result.ErrorDescription = "Exception Error";
                    return Error(result);

                }
                if (result == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    result.ErrorCode = PolicyNotificationOutput.ErrorCodes.NullResult;
                    result.ErrorDescription = "Result is NULL";
                    return Error(result);
                }

                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                result.ErrorCode = PolicyNotificationOutput.ErrorCodes.Success;
                result.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
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

        #region Renewal Discount

        [HttpPost]
        [Route("api/policy/renewal-discount-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<RenewalDiscountListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 70)]
        public IActionResult GetAllRenewalDiscountWithFilter([FromBody]RenewalDiscountFilterModel filterModel, int pageIndex = 1, int pageSize = 10)
        {
            AdministrationOutput<List<RenewalDiscountListingModel>> Output = new AdministrationOutput<List<RenewalDiscountListingModel>>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Renewal Discount";
            log.PageURL = "/renewalDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllRenewalDiscountWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(70, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                int totalCount = 0;                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.GetAllRenewalDiscountFromDBWithFilter(filterModel, pageIndex, pageSize, 60, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new List<RenewalDiscountListingModel>();
                Output.Result = result;
                Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output, totalCount);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<List<RenewalDiscountListingModel>>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/policy/renewal-discount-addNew")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 70)]
        public IActionResult AddNewRenewalDiscount([FromBody]RenewalDiscountFilterModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Renewal Discount";
            log.PageURL = "/renewalDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "AddNewRenewalDiscount";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(70, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (model == null)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Discount model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!Enum.IsDefined(typeof(RenewalDiscountEnum), model.DiscountType))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "RenewalDiscountEnum does not contains model.DiscountType, since model.DiscountType = " + model.DiscountType;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!Enum.IsDefined(typeof(RenewalDiscountCodeTypeEnum), model.CodeType))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "RenewalDiscountCodeTypeEnum does not contains model.CodeType, since model.CodeType = " + model.CodeType;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!Enum.IsDefined(typeof(RenewalMessageTypeEnum), model.MessageType))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "RenewalMessageTypeEnum does not contains model.MessageType, since model.MessageType = " + model.MessageType;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                int userId = 0;
                int.TryParse(log.UserID, out userId);
                var discountModel = new RenewalDiscount()
                {
                    Code = model.Code,
                    DiscountType = model.DiscountType,
                    Amount = model.Amount,
                    Percentage = model.Percentage,
                    StartDate = model.StartDate,    // this means policyExpiryDate start from as per @Mubarak && @Felwa at 2022-11-08 15:18:30.947
                    EndDate = model.EndDate,        // this means policyExpiryDate end to as per @Mubarak && @Felwa at 2022-11-08 15:18:30.947
                    CreatedDate = DateTime.Now,
                    CreatedBy = log.UserName,
                    UserId = userId,
                    MessageType = model.MessageType,
                    IsActive = true,
                    CodeType = model.CodeType
                };

                string exception = string.Empty;
                bool isServiceException;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.AddNewRenewalDiscount(discountModel, model.Lang, out isServiceException, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = Output.ErrorDescription = !isServiceException ? exception : WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while adding new data to db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new bool();
                Output.Result = result;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/policy/renewal-discount-activation")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 70)]
        public IActionResult ManageRenewalDiscountActivation([FromBody]RenewalDiscountActionModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Renewal Discount";
            log.PageURL = "/renewalDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ManageRenewalDiscountActivation";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(70, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.ManageRenewalDiscountActivation(model, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while approve, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = true;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/policy/renewal-discount-delete")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 70)]
        public IActionResult DeleteRenewalDiscount([FromBody]RenewalDiscountActionModel model)
        {
            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Renewal Discount";
            log.PageURL = "/renewalDiscount";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "DeleteRenewalDiscount";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(70, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.DeleteRenewalDiscount(model.Id, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while delete discount, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = true;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        #endregion

        #region Vehicle Add/Purchase Benefits
        [HttpPost]
        [Route("api/policy/getVehiclePoliciesListWithFilterForAddBenefits")]
        [AdminAuthorizeAttribute(pageNumber: 72)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<AutoleaseCancelledPoliciesFilter>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehiclePoliciesListWithFilterForAddBenefits([FromBody] VehicleSuccessPoliciesFilterForAddBenefits policyFilter, int pageIndex = 0, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Add Benefits";
            log.PageURL = "/admin/policy/add-benefits";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetVehiclePoliciesListWithFilterForAddBenefits";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            int totalCount = 0;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                string exception = string.Empty;

                if (this._policyService == null)
                {
                    return Error("_policyService is null");
                }
                var result = _policyContext.GetVehicleSuccessPoliciesWithFilterForAddBenefits(policyFilter, pageIndex, pageSize, 240, out totalCount, out exception);
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result, totalCount);
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

        [HttpPost]
        [Route("api/policy/addBenefit")]
        [AdminAuthorizeAttribute(pageNumber: 72)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddVehicleBenefitModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult AddVehicleBenefit([FromBody]AddVehicleBenefitModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "AddVehicleBenefit";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            log.UserName = User.Identity.GetUserName();
            log.PageName = "Add Benefit";
            log.PageURL = "/admin/policy/addBenefit";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            model.Channel = Channel.Dashboard;
            try
            {
                AddVehicleBenefitOutput result = _quotationContext.AddVehicleBenefit(model, currentUserId, User.Identity.GetUserName());
                if(result.ErrorCode!=AddVehicleBenefitOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Failed to add benefit due to "+ result.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }

        /// <summary>
        /// after calling the insurance company for add benefits to policy then purchase benefit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/policy/purchaseBenefit")]
        [AdminAuthorizeAttribute(pageNumber: 72)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PurchaseBenefitModel>))]
        public IActionResult PurchaseVechileBenefit([FromBody] PurchaseBenefitModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "PurchaseVechileBenefit";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserName = User.Identity.GetUserName();
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            log.UserName = User.Identity.GetUserName();
            log.PageName = "PurchaseBenefit";
            log.PageURL = "/purchaseBenefit";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            model.Channel = Channel.Dashboard;
            try
            {
                AddBenefitOutput result = _policyModificationContext.PurchaseVechileBenefit(model, currentUserId, log.UserName);
                if (result.ErrorCode != AddBenefitOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Failed to purchase benefit due to " + result.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }
        #endregion

        #region Vehicle Claims
        [HttpPost]
        [Route("api/policy/getVehiclePolicyWithFilterForClaim")]
        [AdminAuthorizeAttribute(pageNumber: 74)]
        public IActionResult GetVehiclePolicyWithFilterForClaim([FromBody] PoliciesForClaimFilterModel policyFilter, int pageIndex = 0, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Claims";
            log.PageURL = "/admin/policy/claims-registration";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetVehiclePolicyWithFilterForClaim";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                int totalCount = 0;
                string exception = string.Empty;
                var result = _policyContext.GetAllVehiclePoliciesFromDBWithFilter(policyFilter, pageIndex, pageSize, 240, out totalCount, out exception);
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result, totalCount);
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


        [HttpPost]
        [Route("api/policy/sendClaimRegistration")]
        [AdminAuthorizeAttribute(pageNumber: 74)]
        public IActionResult SendVehicleClaimRegistrationRequest(PoliciesForClaimsListingModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            try
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                DateTime dtBeforeCalling = DateTime.Now;
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "Claims Registration";
                log.PageURL = "/admin/policy/claims-registration";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "SendVehicleClaimRegistrationRequest";
                log.ServiceRequest = JsonConvert.SerializeObject(model);
                string currentUserId = _authorizationService.GetUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.Identity.GetUserId();
                }
                log.UserID = currentUserId;
                log.UserName = User.Identity.GetUserName();
                log.CompanyID = model.CompanyId;
                Policy.Components.VehicleClaimRegistrationOutput output = new Policy.Components.VehicleClaimRegistrationOutput();
                ClaimRegistrationRequest request = new ClaimRegistrationRequest()
                {
                    ReferenceId = model.ReferenceId,
                    PolicyNo = model.PolicyNo,
                    AccidentReportNumber = model.AccidentReportNumber,
                    InsuredId = (!string.IsNullOrEmpty(model.InsuredId)) ? int.Parse(model.InsuredId) : 0,
                    InsuredMobileNumber = model.InsuredMobileNumber,
                    InsuredIBAN = model.InsuredIBAN,
                    InsuredBankCode = model.InsuredBankCode?.ToString(),
                    DriverLicenseTypeCode = model.DriverLicenseTypeCode?.ToString(),
                    DriverLicenseExpiryDate = model.DriverLicenseExpiryDate,
                    AccidentReport = model.AccidentReport
                };

                var result = _policyContext.SendVehicleClaimRegistrationRequest(request, model.CompanyId.Value, currentUserId);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (result == null)
                {
                    output.ErrorCode = Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Result is null";
                    log.ErrorCode = (int)Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Result is null"; ;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (result.ErrorCode != Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = result.ErrorCode;
                    output.ErrorDescription = "Claim Registration response error";
                    log.ErrorCode = (int)result.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    log.ServiceResponse = JsonConvert.SerializeObject(result);
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }

                output.ErrorCode = Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.Success;
                output.ErrorDescription = $"Success - Claim number: {result.ClaimRegistrationServiceResponse.ClaimNo}";
                log.ErrorCode = (int)Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = (int)Policy.Components.VehicleClaimRegistrationOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/policy/sendClaimNotification")]
        [AdminAuthorizeAttribute(pageNumber: 73)]
        public IActionResult SendVehicleClaimNotificationRequest(ClaimNotificationModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            try
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "Claims Notification";
                log.PageURL = "/admin/policy/claims-notification";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "SendVehicleClaimNotificationRequest";
                log.ServiceRequest = JsonConvert.SerializeObject(model);
                string currentUserId = _authorizationService.GetUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.Identity.GetUserId();
                }
                log.UserID = currentUserId;
                log.UserName = User.Identity.GetUserName();
                Policy.Components.VehicleClaimNotificationOutput output = new Policy.Components.VehicleClaimNotificationOutput();

                ClaimNotificationRequest request = new ClaimNotificationRequest()
                {
                    ReferenceId = model.ReferenceId,
                    PolicyNo = model.PolicyNo,
                    ClaimNo = model.ClaimNo
                };
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyContext.SendVehicleClaimNotificationRequest(request, currentUserId);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (result == null)
                {
                    output.ErrorCode = Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Result is null";
                    log.ErrorCode = (int)Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Result is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    return Ok(output);
                }
                if (result.ErrorCode != Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = result.ErrorCode;
                    output.ErrorDescription = "Claim Notification response error";
                    log.ErrorCode = (int)result.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    log.ServiceResponse = JsonConvert.SerializeObject(result);
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                output.ClaimNotificationServiceResponse = result.ClaimNotificationServiceResponse;
                output.ErrorCode = Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                log.ErrorCode = (int)Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return Ok(output);
            }
            catch (Exception ex)
            {
                log.ErrorCode = (int)Policy.Components.VehicleClaimNotificationOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }

        #endregion

        #region Vehicle Add/Purchase Drivers
        [HttpPost]
        [Route("api/policy/getVehiclePoliciesListWithFilterForAddDriver")]
        [AdminAuthorizeAttribute(pageNumber: 71)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<VehicleSuccessPoliciesForAddDriverListing>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult GetVehiclePoliciesListWithFilterForAddDriver([FromBody] VehicleSuccessPoliciesFilterForAddDriver policyFilter, int pageIndex = 0, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "SuccessPolicies";
            log.PageURL = "/admin/insuranceproposal";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetSuccessPolicies";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }

            log.UserID = currentUserId;
            int totalCount = 0;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");
                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                string exception = string.Empty;

                if (this._policyService == null)
                {
                    return Error("_policyService is null");
                }
                var result = _policyContext.GetVehicleSuccessPoliciesWithFilterForAddDriver(policyFilter, pageIndex, pageSize, 240, out totalCount, out exception);
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result, totalCount);
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

        [HttpPost]
        [Route("api/policy/adddriver")]
        [AdminAuthorizeAttribute(pageNumber: 71)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Tameenk.Services.Inquiry.Components.AddDriverOutput>))]
        public IActionResult AddVechileDriver([FromBody] Tameenk.Services.Inquiry.Components.AddDriverModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "AddVehicleDriver";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            log.UserName = User.Identity.GetUserName();
            log.PageName = "AddDriver";
            log.PageURL = "/addDriver";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            model.Channel = Channel.Dashboard;
            try
            {
                var result = _inquiryContext.AddVechileDriver(model, currentUserId, log.UserName);
                if(result.ErrorCode!=Inquiry.Components.AddDriverOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Failed to add driver due to " + result.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/policy/purchaseDriver")]
        [AdminAuthorizeAttribute(pageNumber: 71)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<AddDriverOutput>))]
        public IActionResult PurchaseDriver([FromBody] PurchaseDriverModel model)
        {
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = "PurchaseVechileDriver";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            log.PageName = "AddDriver";
            log.PageURL = "/addDriver";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserName = User.Identity.GetUserName();
            model.Channel = Channel.Dashboard;
            try
            {
                //result = _checkoutContext.PurchaseVechileDriver(model, currentUserId, "");
                var result = _policyModificationContext.PurchaseVechileDriver(model, currentUserId, log.UserName);
                if(result.ErrorCode!=AddDriverOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Failed to Purchase driver due to "+ result.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.ToString());
            }
        }
        #endregion


        [HttpPost]        [Route("api/policy/renewal-stopsms-filter")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SMSSkippedNumbersModel>>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 77)]        public IActionResult GetAllRenewalPhoneNumberWithFilter([FromBody] SMSSkippedNumbersFilterModel filterModel, int pageIndex = 1, int pageSize = 10)        {            AdministrationOutput<List<SMSSkippedNumbersModel>> Output = new AdministrationOutput<List<SMSSkippedNumbersModel>>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "RenewalStopSMS";            log.PageURL = "/StopSMS";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "RenewalStopSMS";            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(77, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                int totalCount = 0;                string exception = string.Empty;                DateTime dtBeforeCalling = DateTime.Now;                var result = _policyService.GetFromSMSSkippedNumbers(filterModel, 60, out totalCount, out exception, pageIndex, pageSize);                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                if (!string.IsNullOrEmpty(exception))                {                    Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.ExceptionError;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                Output.Result = new List<SMSSkippedNumbersModel>();                Output.Result = result;                Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output, totalCount);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<List<SMSSkippedNumbersModel>>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }        [HttpGet]        [Route("api/policy/renewal-stopsms-addNew")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 77)]        public IActionResult AddNewRenewalStopPhone(string mobilePhone)        {            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "AddNewRenewalStopPhone";            log.PageURL = "/AddNewRenewalStopPhone";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "AddNewRenewalStopPhone";            log.ServiceRequest = mobilePhone;            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(77, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!string.IsNullOrEmpty(mobilePhone))                {                    if (!Utilities.IsValidPhoneNo(mobilePhone))                    {                        Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotFound;                        Output.ErrorDescription = "Wrong phone number format";                        log.ErrorCode = (int)Output.ErrorCode;                        log.ErrorDescription = "Wrong phone number format : " + mobilePhone;                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                        return Error(Output);                    }                    string validinternationalphonenumber = Utilities.ValidatePhoneNumber(mobilePhone);                    string validlocalphonenumber = "0" + validinternationalphonenumber.Substring(Utilities.SaudiInternationalPhoneCode.Length);                    mobilePhone = validlocalphonenumber;                }
                int userId = 0;
                int.TryParse(log.UserID, out userId);
                SMSSkippedNumbers smsSkippedNumber = new SMSSkippedNumbers();
                smsSkippedNumber.CreatedDate = DateTime.Now;
                smsSkippedNumber.PhoneNo = mobilePhone;
                smsSkippedNumber.CreatedBy = log.UserName;
                smsSkippedNumber.UserId = userId;
                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.AddNewRenewalStopSMSPhon(smsSkippedNumber, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while adding new data to db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new bool();
                Output.Result = result;
                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }        [HttpPost]        [Route("api/policy/renewal-stopsms-delete")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 77)]        public IActionResult DeleteRenewalStopPhone([FromBody] SMSSkippedNumbersFilterModel model)        {            AdministrationOutput<bool> Output = new AdministrationOutput<bool>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "DeleteRenewalStopPhone";            log.PageURL = "/DeleteRenewalStopPhone";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "DeleteRenewalStopPhone";            log.ServiceRequest = JsonConvert.SerializeObject(model);            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(77, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                string exception = string.Empty;                DateTime dtBeforeCalling = DateTime.Now;                var result = _policyService.DeleteRenewalStopSMSPhon(model.PhoneNo, out exception);                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                if (!string.IsNullOrEmpty(exception))                {                    Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ExceptionError;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "Error happend while delete discount, and the error is: " + exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                Output.Result = true;                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<bool>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }
        [HttpGet]
        [Route("api/policy/regeneratefile")]
        [AdminAuthorizeAttribute(pageNumber: 7)]
        public async Task<IActionResult> ReGeneratePolicyFile(string referenceId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Failure policies";
            log.PageURL = "/admin/policies/failure";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "ReGeneratePolicyFile";
            log.ServiceRequest = $"referenceId: {referenceId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = referenceId;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(7, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("ReferenceId");

                //PolicyResponse policy = new PolicyResponse();
                //policy.ReferenceId = referenceId;
                //policy.PolicyNo = policyNo;

                //var output = _policyFileContext.GeneratePolicyPdfFile(policy,companyId,"Portaal",selectedLanguage.ToLower()=="ar"? LanguageTwoLetterIsoCode.Ar: LanguageTwoLetterIsoCode.En);
                var output = _policyFileContext.GetPolicyTemplateGenerationModel(referenceId);
                if (output.ErrorCode == Policy.Components.PolicyGenerationOutput.ErrorCodes.Success)
                {
                    return Ok(System.Text.RegularExpressions.Regex.Unescape(JsonConvert.SerializeObject(output.PolicyModel)));
                }
                else
                {
                    return Error(output.ErrorDescription);
                }

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/policy/comprehinsive-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<SuccessPolicyModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 61)]
        public IActionResult GetComprehinsivePoliciesWithFilter([FromBody]SuccessPoliciesFilterModel policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = policyFilter.InsuranceCompanyId;
            log.PageName = "Comprehinsive policies";
            log.PageURL = "/admin/policies/comprehensive";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetComprehinsivePoliciesWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.ReferenceId = policyFilter.ReferenceNo;
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
                var isAuthorized = _userPageService.IsAuthorizedUser(61, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Not Authorized : " + log.UserID);
                }

                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                    throw new TameenkArgumentNullException("policyFilter");

                if (policyFilter.StartDate != null)
                {
                    if (policyFilter.EndDate == null)
                    {
                        if (policyFilter.StartDate > DateTime.Now.Date)
                            throw new TameenkArgumentException("Search Date", "Start Date must be smaller than today.");
                    }

                    if (policyFilter.StartDate > policyFilter.EndDate)
                        throw new TameenkArgumentException("Search Date", "End Date must be greater than start date.");
                }

                int totalCount = 0;                string exception = string.Empty;                var result = GetAllPoliciesFromDBWithFilter(policyFilter, out totalCount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result Error");
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
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(result, totalCount);

            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
            finally
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }
        }


        [HttpPost]        [Route("api/policy/checkoutDetails")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber:79)]        public IActionResult CheckoutDetails([FromBody] CheckOutDetailsFilter model)        {            AdministrationOutput<CheckoutDetailsOutPut> Output = new AdministrationOutput<CheckoutDetailsOutPut>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "sama-checkout-details";            log.PageURL = "/sama-checkout-details";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "CheckoutDetails";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<CheckoutDetailsOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<CheckoutDetailsOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(79, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<CheckoutDetailsOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }
                string exception = string.Empty;
                int totalcount = 0;                CheckoutDetailsOutPut result = new CheckoutDetailsOutPut();
                var CheckOutDetails = _policyService.GetChekoutDetailsWithFilter(model, out totalcount, out exception);
                result.Data = CheckOutDetails;
                result.TotalCount = totalcount;
                DateTime dtBeforeCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (model.Exports == true)
                {
                    result.File = _excelService.GeneratePoliciesDetailsExcel(CheckOutDetails);
                }                Output.Result = result;                Output.ErrorCode = AdministrationOutput<CheckoutDetailsOutPut>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<CheckoutDetailsOutPut>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }
        [HttpGet]
        [Route("api/policy/policy-GetDetailsToEditSuccessByRef")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<SuccessPolicyModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetDetailsToEditSuccessPoliciesByRef(string referenceId)
        {
            try
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                DateTime dtBeforeCalling = DateTime.Now;
                AdminRequestLog log = new AdminRequestLog();
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "policy-GetDetailsToEditSuccessByRef";
                log.PageURL = "/admin/policy-GetDetailsToEditSuccessByRef";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "policy-GetDetailsToEditSuccessByRef";
                log.UserID = User.Identity.GetUserId();
                log.UserName = User.Identity.GetUserName();
                log.RequesterUrl = Utilities.GetUrlReferrer();
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

                if (string.IsNullOrEmpty(referenceId))
                    throw new TameenkArgumentNullException("ReferenceId");


                var result = _policyService.GetDetailsToSuccessPolicies(referenceId);


                return Ok(result.ToServiceModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        #region Renewal send SMS message

        [HttpPost]
        [Route("api/policy/renewal-message-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 81)]
        public IActionResult RenewalMessageFilter([FromBody] RenewalMessageFilter model)
        {
            PolicyFilterOutput Output = new PolicyFilterOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "renewal-message-filter ";
            log.PageURL = "/renewal-message-filter";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "RenewalMessageFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(81, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.RenewalMessageFilter(model, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ExceptionError; 
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while Get Data , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = result.Result;
                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.Success; 
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpGet]        [Route("api/policy/GetDiscountType")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 81)]        public IActionResult GetDiscountType()        {            AdministrationOutput<List<RenewalDiscount>> Output = new AdministrationOutput<List<RenewalDiscount>>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "GetDiscountType";            log.PageURL = "/GetDiscountType";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetDiscountType";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();            int count = 0;            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(81, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                string exception = string.Empty;                DateTime dtBeforeCalling = DateTime.Now;                var result = _policyService.getDiscountType(out exception);                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                if (!string.IsNullOrEmpty(exception))                {                    Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.ExceptionError;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "Error happend while delete discount, and the error is: " + exception;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                count = result.Count();                Output.Result = result;                Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output,count);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<List<RenewalDiscount>>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }

        [HttpPost]
        [Route("api/policy/renewal-message-send")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 81)]
        public IActionResult RenewalMessageSend([FromBody] RenewalSendSMS model)
        {
            PolicyFilterOutput Output = new PolicyFilterOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "renewal-message-send ";
            log.PageURL = "/renewal-message-send";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "SendRenewalSmsMsg";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(81, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
              
                VehicleDiscounts VehicleDiscountmodel = new VehicleDiscounts()
                {
                    CreatedDate = DateTime.Now,
                    DiscountCode = model.Code,
                    IsUsed = false,
                    SequenceNumber = model.SequenceNumber,
                    CustomCardNumber = model.CustomCardNumber,
                    Nin = model.NIN
                };
                string exception = string.Empty;
                var CodeWithVech = _policyContext.LinkVechWithDiscountCode(VehicleDiscountmodel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while insert Data , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyContext.SendRenewalSmsMsg(model, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while Get Data , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);

            }
            catch (Exception ex)
            {
                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        #endregion
        #region Own Damage send SMS message
        [HttpPost]        [Route("api/policy/OwnDamage")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]        [AdminAuthorizeAttribute(pageNumber: 82)]        public IActionResult GetOwnDamagePolicy([FromBody] OwnDamageFilter model)        {            AdministrationOutput<OwnDamageOutPut> Output = new AdministrationOutput<OwnDamageOutPut>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "GetOwnDamagePolicy";            log.PageURL = "/send-Own-Damage-SMS";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetOwnDamagePolicy";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);            if (string.IsNullOrEmpty(model.Lang))
                model.Lang = "en";            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(82, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }
                string exception = string.Empty;
                int totalcount = 0;                DateTime dtBeforeCalling = DateTime.Now;
                var OwnDamagePolicyInfo = _policyService.GetOwnDamagePolicyForSMS(model, out totalcount, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while get data to send sms , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                OwnDamageOutPut result = new OwnDamageOutPut();
                result.Data = new List<OwnDamagePolicyInfo>();
                result.Data = OwnDamagePolicyInfo;
                result.TotalCount = totalcount;
                Output.Result = result;                Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<OwnDamageOutPut>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }

        [HttpPost]
        [Route("api/policy/own-damage")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 82)]
        public IActionResult OwnDamageAddOnQueue([FromBody] OwnDamageFilter model)
        {
            PolicyFilterOutput Output = new PolicyFilterOutput();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "own-damage";
            log.PageURL = "/own-damage";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "OwnDamageAddOnQueue";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    Output.ErrorDescription = " not NotAuthorized";
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    Output.ErrorDescription = "not IsAuthenticated";
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(82, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var selectedPolicy = _policyService.GetpolicyForOD(model, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while Get policy  Data , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                exception = string.Empty;
                _OwnDamageQueueService.AddOwnDamageQueue(selectedPolicy, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while send sms , and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = PolicyFilterOutput.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        #endregion

        #region Bcare Withdrawal

        [HttpPost]
        [Route("api/policy/bcare-withdrawal-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<BcareWithdrawalListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 83)]
        public IActionResult GetBcareWithdrawalWithFilter([FromBody]BcareWithdrawalFilterModel filterModel)
        {
            AdministrationOutput<List<BcareWithdrawalListingModel>> Output = new AdministrationOutput<List<BcareWithdrawalListingModel>>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "BcareWithdrawal";
            log.PageURL = "/withdrawal";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetBcareWithdrawalWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(filterModel);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(83, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                if (filterModel == null)
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "filterModel == null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!Enum.IsDefined(typeof(BcareWithdrawalProductsEnum), filterModel.ProductType))
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "BcareWithdrawalProductsEnum does not contains filterModel.ProductType, since filterModel.ProductType = " + filterModel.ProductType;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!Enum.IsDefined(typeof(BcareWithdrawalPrizeEnum), filterModel.PrizeNumber))
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "BcareWithdrawalPrizeEnum does not contains filterModel.PrizeNumber, since filterModel.PrizeNumber = " + filterModel.PrizeNumber
;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                //if ((filterModel.WeekNumber < 1) || (filterModel.WeekNumber > 8))
                //{
                //    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                //    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                //    log.ErrorCode = (int)Output.ErrorCode;
                //    log.ErrorDescription = "Week number is wrong, since filterModel.WeekNumber = " + filterModel.WeekNumber;
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error(Output);
                //}

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                var result = _policyService.GetBcareWithdrawalListWithFilter(filterModel, 60*60, out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                exception = string.Empty;
                var insert = _policyService.InsertIntoWinnersTable(filterModel, result, log.UserID, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while inserting winners into db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.Result = new List<BcareWithdrawalListingModel>();
                Output.Result = result;
                Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output, filterModel.ReturnedNumber);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<List<BcareWithdrawalListingModel>>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(filterModel.Lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        [HttpPost]
        [Route("api/policy/get-withdrawal-statistics")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<BcareWithdrawalListingModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 83)]
        public IActionResult GetBcareWithdrawalStatistics(string lang)
        {
            AdministrationOutput<BcareWithdrawalStatisticsModel> Output = new AdministrationOutput<BcareWithdrawalStatisticsModel>();
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "BcareWithdrawal";
            log.PageURL = "/withdrawal";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetBcareWithdrawalStatistics";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(83, log.UserID);
                if (!isAuthorized)
                {
                    Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                string exception = string.Empty;
                DateTime dtBeforeCalling = DateTime.Now;
                Output.Result = _policyService.GetBcareWithdrawalStatistics(out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (!string.IsNullOrEmpty(exception))
                {
                    Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.ExceptionError;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "Error happend while getting data from db, and the error is: " + exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }

                Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.Success;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = AdministrationOutput<BcareWithdrawalStatisticsModel>.ErrorCodes.ServiceException;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(Output);
            }
        }

        #endregion

        #region Pending Processing Queue

        [HttpPost]        [Route("api/policy/processing-queue")]        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<ProcessingQueueOutput>))]        [AdminAuthorizeAttribute(pageNumber: 85)]        public IActionResult GetProcessingQueue([FromBody] ProcessingQueueFilter model)        {            AdministrationOutput<ProcessingQueueOutput> Output = new AdministrationOutput<ProcessingQueueOutput>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "GetProcessingQueue";            log.PageURL = "/get-processing-queue";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetProcessingQueue";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<ProcessingQueueOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<ProcessingQueueOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(85, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<ProcessingQueueOutput>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Error(Output);                }
                string exception = string.Empty;
                int totalcount = 0;                ProcessingQueueOutput result = new ProcessingQueueOutput();
                var processingQueueInfo = _policyService.GetProcessingQueue(model, out totalcount, out exception);

                result.Data = processingQueueInfo;
                result.TotalCount = totalcount;
                Output.Result = result;                Output.ErrorCode = AdministrationOutput<ProcessingQueueOutput>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                if (model.IsExport == true)
                {
                    byte[] file = _excelService.GenerateExcelPendingPolicies(processingQueueInfo);
                    if (file != null && file.Length > 0)
                        return Ok(Convert.ToBase64String(file));
                }                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<ProcessingQueueOutput>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Error(Output);            }        }

        #endregion


        #region Over 5 Policies
        [HttpPost]        [Route("api/policy/GetClientsWithOverFivePolicy")]        [AdminAuthorizeAttribute(pageNumber: 96)]        public IActionResult GetClientsWithOverFivePolicy( DriverWithOverFivePoliciesFilter model)        {            AdministrationOutput <List<DriverswithPolicyDetails>> Output = new AdministrationOutput<List<DriverswithPolicyDetails>>();            AdminRequestLog log = new AdminRequestLog();            log.UserIP = Utilities.GetUserIPAddress();            log.ServerIP = Utilities.GetInternalServerIP();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.PageName = "GetClientsWithOverFivePolicy";            log.PageURL = "/GetCustomersOverFivePolicy";            log.ApiURL = Utilities.GetCurrentURL;            log.MethodName = "GetClientsWithOverFivePolicy";            log.UserID = User.Identity.GetUserId();            log.UserName = User.Identity.GetUserName();            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);            try            {                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))                {                    Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authorized";                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(Output);                }                if (!User.Identity.IsAuthenticated)                {                    Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(Output);                }                var isAuthorized = _userPageService.IsAuthorizedUser(96, log.UserID);                if (!isAuthorized)                {                    Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.NotAuthorized;                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo("en"));                    log.ErrorCode = (int)Output.ErrorCode;                    log.ErrorDescription = "User not authenticated since user id is: " + User.Identity.GetUserId();                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return Ok(Output);                }
                string exception = string.Empty;
                int totalcount = 0;                Output.Result = new List<DriverswithPolicyDetails>();
                Output.Result = _policyService.GetOverFivePolicies(model, out totalcount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 100;                    log.ErrorDescription = exception;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                    Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.ServiceException;                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Output);                }
                Output.TotalCount = totalcount;
                Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.Success;                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo("en"));
                if (model.IsExport == true && Output.Result.Count > 0)
                {
                    byte[] file = _excelService.GenerateExcelDriverswithPolicyDetails(Output.Result);
                    if (file != null && file.Length > 0)
                        Output.sheet = file;
                }                return Ok(Output);            }            catch (Exception ex)            {                Output.ErrorCode = AdministrationOutput<List<DriverswithPolicyDetails>>.ErrorCodes.ServiceException;                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("en"));                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);            }        }

        #endregion

        #region Policies with duplcate data email,phone ,Iban
        [HttpPost]
        [Route("api/policy/check-all-repeated-policies-with-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PoliciesDuplicationModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AdminAuthorizeAttribute(pageNumber: 98)]
        public IActionResult GetAllPoliciesDuplicationWithFilter([FromBody] PoliciesDuplicationFilter duplicationFilter)

        {
            AdministrationOutput<List<PoliciesDuplicationModel>> Output = new AdministrationOutput<List<PoliciesDuplicationModel>>();

            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "Duplicated Policies";
            log.PageURL = "policies/repeated-policies-with-filter";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllPoliciesDuplicationService";
            log.ServiceRequest = JsonConvert.SerializeObject(duplicationFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();


            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 7;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    return Ok(Output);
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 8;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    return Ok(Output);
                }
                var isAuthorized = _userPageService.IsAuthorizedUser(98, log.UserID);
                if (!isAuthorized)
                {
                    log.ErrorCode = 9;
                    log.ErrorDescription = "User not authenticated : " + log.UserID;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.NotAuthorized;
                    Output.ErrorDescription = Output.ErrorDescription = WebResources.ResourceManager.GetString("NotAuthorized", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    return Ok(Output);
                }


                if (duplicationFilter == null)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "Repeated Policies Model is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.EmptyInputParamter;
                    Output.ErrorDescription = Output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    return Ok(Output);
                }

                if (duplicationFilter.StartDate != null && duplicationFilter.EndDate != null)
                {
                    DateTime dtStart = new DateTime(duplicationFilter.StartDate.Value.Year, duplicationFilter.StartDate.Value.Month, duplicationFilter.StartDate.Value.Day, 0, 0, 0);
                    DateTime dtEnd = new DateTime(duplicationFilter.EndDate.Value.Year, duplicationFilter.EndDate.Value.Month, duplicationFilter.EndDate.Value.Day, 23, 59, 59);
                    if (dtStart > dtEnd)
                    {
                        log.ErrorCode = 5;
                        log.ErrorDescription = "Start date must be less than End date";
                        Output.ErrorDescription = WebResources.ResourceManager.GetString("NotFound", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                        Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.NotFound;
                        AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return Ok(Output);
                    }
                }
                if (!duplicationFilter.DuplicatedData.HasValue || duplicationFilter.DuplicatedData.Value < 1)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "please select Duplicate Value Type";
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.EmptyInputParamter;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Output);
                }
                int totalCount = 0;
                string exception = string.Empty;

                var result = _policyService.GetAllPoliciesDuplicationService(duplicationFilter, 60*60*60, out totalCount, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = exception;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.ServiceException;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Output);
                }
                if (result == null)
                {
                    log.ErrorCode = 10;
                    log.ErrorDescription = exception;
                    Output.ErrorDescription = WebResources.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                    Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.NullResult;
                    Output.Result = null;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(Output);
                }

                if (duplicationFilter.IsExport)
                {
                    byte[] file = _excelService.GenerateExcelRepeatedPolicies(result, duplicationFilter.Language);
                    if (file != null && file.Length > 0)
                        Output.sheet = file;
                }
                Output.Result = result;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.Success;
                Output.TotalCount = totalCount;
                return Ok(Output);
            }
            catch (Exception ex)
            {
                Output.Result = null;
                Output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(duplicationFilter.Language));
                Output.ErrorCode = AdministrationOutput<List<PoliciesDuplicationModel>>.ErrorCodes.ExceptionError;
                log.ErrorCode = (int)Output.ErrorCode;                log.ErrorDescription = "Service Exception, and the error is: " + ex.ToString();                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);                return Ok(Output);
            }
        }
        #endregion
    }
}