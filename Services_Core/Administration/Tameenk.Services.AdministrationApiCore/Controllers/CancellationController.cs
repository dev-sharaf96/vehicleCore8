using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Services;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.ReasonCodes;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Policy.Components;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using Tameenk.Api.Core.Models;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// Cancellation Controller For Handel Policy Cancellation for Veichle 
    /// </summary>
    [AdminAuthorizeAttribute(pageNumber: 75)]
    public class CancellationController : BaseApiController
    {
        #region Fields
        private readonly IPolicyService _policyService;
        private readonly IAuthorizationService _authorizationService;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IPolicyContext _policyctx;
        private readonly IRepository<ReasonCode> _reasonCodesRepository;
        private readonly IRepository<CancellationRequest> _cancellationRequestService;
        #endregion
        #region CTOR
        /// <summary>
        /// For handel Depencey injection 
        /// </summary>
        /// <param name="reasonCodesRepository"></param>
        /// <param name="authorizationService"></param>
        /// <param name="bankService"></param>
        /// <param name="policyService"></param>
        /// <param name="tameenkConfig"></param>
        /// <param name="insuranceCompanyRepository"></param>
        /// <param name="policyctx"></param>
        public CancellationController(IRepository<ReasonCode> reasonCodesRepository, 
           IAuthorizationService authorizationService,
           IPolicyService policyService, 
           TameenkConfig tameenkConfig, 
           IRepository<InsuranceCompany> insuranceCompanyRepository, 
           IPolicyContext policyctx,
           IRepository<CancellationRequest> cancellationRequestService
           )
        {
            this._reasonCodesRepository = reasonCodesRepository;
            this._authorizationService = authorizationService;
            this._tameenkConfig = tameenkConfig;
            this._policyService = policyService;
            this._insuranceCompanyRepository = insuranceCompanyRepository;
            this._policyctx = policyctx;
            _cancellationRequestService = cancellationRequestService;
        }
        #endregion
        #region Methods
        /// <summary>
        /// For Handle CancelPolicy Request Comming from angular 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>CancellPolicyOutput</returns>
        [HttpPost]
        [Route("api/cancellation/cancelPolicy")]
        public IActionResult CancelPolicy([FromBody] CancelVechilePolicyRequestDto request)
        {
            AdminRequestLog log = new AdminRequestLog();
            CancellPolicyOutput output = new CancellPolicyOutput();
            try
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                DateTime dtBeforeCalling = DateTime.Now;
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "Cancellation";
                log.PageURL = "/admin/cancel";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "CancelPolicy";
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                log.UserID = User.Identity.GetUserId();
                log.UserName = User.Identity.GetUserName();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                if (request == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    log.ErrorDescription = "request is null"; ;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                var policyInfo = _policyService.GetPolicyByReferenceId(request.ReferenceId);
                var companyinfo = _insuranceCompanyRepository.TableNoTracking.FirstOrDefault(
                  i => i.InsuranceCompanyID == policyInfo.InsuranceCompanyID);
                CancellationRequest cancellationRequest = new CancellationRequest();
                cancellationRequest.ReferenceId = request.ReferenceId;
                cancellationRequest.PolicyNo = request.PolicyNo;
                cancellationRequest.CancelDate = request.CancelDate;
                cancellationRequest.CancellationReasonCode = request.CancellationReasonCode;
                cancellationRequest.CancellationAttachment = Utilities.SaveCancelPolicyRequestAttachmentFromDashboard(request.ReferenceId, request.CancellationAttachment, companyinfo.Key,
                        _tameenkConfig.RemoteServerInfo.UseNetworkDownload,
                        _tameenkConfig.RemoteServerInfo.DomainName,
                        _tameenkConfig.RemoteServerInfo.ServerIP,
                        _tameenkConfig.RemoteServerInfo.ServerUserName,
                        _tameenkConfig.RemoteServerInfo.ServerPassword,
                        out string exception);
                cancellationRequest.UserName = log.UserName;
                cancellationRequest.IsAutolease = false;
                _cancellationRequestService.Insert(cancellationRequest);
                _policyService.CancelPolicy(request.ReferenceId, true, log.UserName);
                var result = _policyctx.SendCancellationRequest(request, log.UserID, log.UserName);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (result == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Result is null";
                    log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "Result is null"; ;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (result.ErrorCode != (int)CancellPolicyOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = result.ErrorCode;
                    output.ErrorDescription = result.ErrorDescription;
                    //output.CancelPolicyResponse = result.CancelPolicyResponse;
                    log.ErrorCode = (int)result.ErrorCode;
                    log.ErrorDescription = result.ErrorDescription;
                    log.ServiceResponse = JsonConvert.SerializeObject(result);
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(output);
                }
                output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                //output.CancelPolicyResponse = result.CancelPolicyResponse;
                log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(result);
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceException;
                log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(ex.GetBaseException().ToString());
            }
        }
        /// <summary>
        ///  For Handle getPolicyWithFilterForCancellation Request Comming from angular
        /// </summary>
        /// <param name="policyFilter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cancellation/getPolicyWithFilterForCancellation")]
        public IActionResult GetPolicyWithFilterForcancellation([FromBody] PoliciesForCancellationFilterModel policyFilter, int pageIndex = 1, int pageSize = 10, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "cancellation";
            log.PageURL = "/admin/cancellation";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetPolicyWithFilterForCancellation";
            log.ServiceRequest = JsonConvert.SerializeObject(policyFilter);
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User.Identity.GetUserId();
            }
            log.UserID = currentUserId;
            log.UserName = User.Identity.GetUserName();

            try
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                if (policyFilter == null)
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "policyFilter is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Please enter data for search");
                }
                if (string.IsNullOrEmpty(policyFilter.VehicleId) && string.IsNullOrEmpty(policyFilter.NationalId)
                    && string.IsNullOrEmpty(policyFilter.PolicyNo) && string.IsNullOrEmpty(policyFilter.ReferenceId))
                {
                    log.ErrorCode = 11;
                    log.ErrorDescription = "all policyFilter info is null";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Please enter data for search");
                }

                int totalCount = 0;
                string exception = string.Empty;
                pageIndex = pageIndex == 0 ? 1 : pageIndex;
                var result = _policyService.GetAllCancellationPoliciesFromDBWithFilter(policyFilter, pageIndex, pageSize, 240, out totalCount, out exception);
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
        /// <summary>
        /// For Handle Get Reason Codes to fill dropdown of ReasonCodes in angular
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/cancellation/ReasonCodes")]
        public IActionResult ReasonCodes(string lang = "en")
        {
            List<Tameenk.Core.Domain.Entities.ReasonCode> data = GetAll(lang);
            Tameenk.Services.Inquiry.Components.LookupsOutput output = new Tameenk.Services.Inquiry.Components.LookupsOutput();
            output.Result = new List<Tameenk.Core.Domain.Dtos.Lookup>();
            if (lang == "en")
            {
                foreach (Tameenk.Core.Domain.Entities.ReasonCode model in data)
                {
                    output.Result.Add(new Tameenk.Core.Domain.Dtos.Lookup() { Id = (model.Code).ToString(), Name = model.EnglishDescription });
                }
            }
            else if (lang == "ar")
            {
                foreach (Tameenk.Core.Domain.Entities.ReasonCode model in data)
                {
                    output.Result.Add(new Tameenk.Core.Domain.Dtos.Lookup() { Id = (model.Code).ToString(), Name = model.ArabicDescription });
                }
            }
            output.Result = output.Result.OrderBy(x => x.Name).ToList();
            output.ErrorCode = Tameenk.Services.Inquiry.Components.LookupsOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            return Single(output);
        }
        /// <summary>
        /// Get All ReasonCodes from table ReasonCodes 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public List<ReasonCode> GetAll(string lang = "en")
        {

            return _reasonCodesRepository.TableNoTracking.ToList();

        }
        [HttpPost]
        [Route("api/cancellation/activateCancelledPolicy")]
        public IActionResult ActivateCancelledPolicy([FromBody] ActivateVechilePolicyRequestDto request)
        {
            AdminRequestLog log = new AdminRequestLog();
            CancellPolicyOutput output = new CancellPolicyOutput();
            try
            {
                string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                DateTime dtBeforeCalling = DateTime.Now;
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.PageName = "Cancellation";
                log.PageURL = "/admin/cancel";
                log.ApiURL = Utilities.GetCurrentURL;
                log.MethodName = "UnCancelPolicy";
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                log.UserID = User.Identity.GetUserId();
                log.UserName = User.Identity.GetUserName();
                log.RequesterUrl = Utilities.GetUrlReferrer();
                log.ReferenceId = request.ReferenceId;
                if (request == null || request.ReferenceId == null || request.PolicyNo == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "Reference Or Policy Number is Missing";
                    log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                CancellationRequest cancellationRequest = _cancellationRequestService
                                                        .Table
                                                        .FirstOrDefault(c => c.ReferenceId == request.ReferenceId && c.PolicyNo == request.PolicyNo && c.IsDeleted == false);
                if (cancellationRequest == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "This Policy is not canceled";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                string exception = string.Empty;
                bool isCancelled = _policyService.UnCancelPolicy(request.ReferenceId,out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Failure;
                    output.ErrorDescription = "Faild to reactivate the policy due to "+exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }
                if (!isCancelled)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Failure;
                    output.ErrorDescription = "Fail To Activate cancelled policy";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Ok(output);
                }

                cancellationRequest.IsDeleted = true;
                cancellationRequest.ModifiedDate = DateTime.Now;
                _cancellationRequestService.Update(cancellationRequest);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Success;
                log.ErrorDescription = output.ErrorDescription;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error happened please contact customer services";
                log.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error(output);
            }
        }
        #endregion
    }
}