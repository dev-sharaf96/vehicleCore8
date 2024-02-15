using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;
using Tameenk.Services.InsuranceCompaniesCallBack.Repository;
using Tameenk.Services.Logging;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Services
{
    public class InsuranceCompaniesCallBackService : IInsuranceCompaniesCallBackService
    {
        private readonly IPolicyRepository _iPolicyRepository;
        private readonly ICheckoutDetailsRepository _iCheckoutDetailsRepository;
        private readonly ILogger _logger;
        private readonly IWataniyaNajmQueueService _wataniyaNajmQueueService;
        private readonly IRepository<PolicyRenewedByCompany> _policyRenewedByCompany;
        private readonly IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepo;

        public InsuranceCompaniesCallBackService(IPolicyRepository iPolicyRepository, ICheckoutDetailsRepository iCheckoutDetailsRepository
            ,ILogger logger, IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepository,
            IRepository<CheckoutDetail> checkoutRepository, IWataniyaNajmQueueService wataniyaNajmQueueService, IRepository<PolicyRenewedByCompany> policyRenewedByCompany
            , IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepo)
        {
            _iPolicyRepository = iPolicyRepository;
            _iCheckoutDetailsRepository = iCheckoutDetailsRepository;
            _logger = logger;
            _wataniyaNajmQueueService = wataniyaNajmQueueService;
            _policyRenewedByCompany = policyRenewedByCompany;
            _policyRepo = policyRepo;
        }

        public PolicyAttachmentResponseModel GetPolicyAttachments(CommonPolicyRequestModel policyAttachmentModel)
        {
            PolicyAttachmentResponseModel result = new PolicyAttachmentResponseModel();
            try
            {
                ValidatePolicyAttachmentModel(policyAttachmentModel, result);
                //if the model not valid then return
                if (result.StatusCode == 2)
                {
                    _logger.Log($"GetPolicyAttachments validation errors in the model, errors:{JsonConvert.SerializeObject(result)}");
                    return result;
                }
                else
                {
                    //if the policy exist then add the car images
                    result.Attachments = _iCheckoutDetailsRepository.GetCarImages(policyAttachmentModel.ReferenceId);
                    result.ReferenceId = policyAttachmentModel.ReferenceId;
                    result.StatusCode = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Log("GetPolicyAttachments an error occurred see the exception", ex);
                result.Errors.Add(new ErrorModel { Message = "An error occurred while processing your request.  Please refer to the following techinical error details :" + ex.GetBaseException().Message});
                result.StatusCode = 2;

            }

            _logger.LogIntegrationTransaction("InsuranceCompaniesCallBackController/PolicyUploadNotification",
                JsonConvert.SerializeObject(policyAttachmentModel),JsonConvert.SerializeObject(result),result.StatusCode);

            return result;
        }

        public CommonResponseModel NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            if (policyUploadNotificationModel == null)
                throw new TameenkArgumentNullException(nameof(policyUploadNotificationModel), "Policy notification cant be null");

            CommonResponseModel result = new CommonResponseModel();
            result.ReferenceId = policyUploadNotificationModel.ReferenceId;

            ValidateGivenPolicyUploadNotificationModel(result, policyUploadNotificationModel);

            if (result.StatusCode != 2)
            {
                try
                {
                    _logger.Log("NotifyPolicyUploadCompletion adding najm status history");
                    _iPolicyRepository.NotifyPolicyUploadCompletion(policyUploadNotificationModel);
                    _logger.Log("NotifyPolicyUploadCompletion updating policy najm status");
                    _iPolicyRepository.SavePolicyWithNajmStatus(policyUploadNotificationModel);
                    result.StatusCode = 1;
                }
                catch (Exception ex)
                {
                    _logger.Log("NotifyPolicyUploadCompletion error happen while updating policy najm status and adding najm status history", ex.GetBaseException());
                    result.StatusCode = 2;
                    if (result.Errors == null)
                        result.Errors = new List<ErrorModel>();

                    result.Errors.Add(new ErrorModel { Message = "Error occured while saving Najm status history. Please refer to the following techinical error details  :" + ex.GetBaseException().Message });
                }
            }

            _logger.LogIntegrationTransaction("InsuranceCompaniesCallBackController/PolicyUploadNotification",
                new JavaScriptSerializer().Serialize(policyUploadNotificationModel),new JavaScriptSerializer().Serialize(result),result.StatusCode);

            return result;
        }

        public PolicyRequestAdditionalInfoResponseModel GetPolicyRequestAdditionalInfo(CommonPolicyRequestModel policyRequestModel)
        {
            PolicyRequestAdditionalInfoResponseModel result = new PolicyRequestAdditionalInfoResponseModel();
            try
            {
                ValidatePolicyRequestAdditionalInfoModel(policyRequestModel, result);
                //if the model not valid then return
                if (result.StatusCode == 2)
                {
                    return result;
                }
                else
                {
                    //if the policy exist then add the car images
                    result.AdditionalInfo = _iCheckoutDetailsRepository.GetPolicyRequestAdditionalInfo(policyRequestModel.ReferenceId, policyRequestModel.PolicyNo);
                    result.ReferenceId = policyRequestModel.ReferenceId;
                    result.StatusCode = 1;
                }
            }
            catch (ApplicationException appEx)
            {
                result.Errors.Add(new ErrorModel { Message = appEx.Message });
                result.StatusCode = 2;
            }
            catch (Exception ex)
            {
                //TODO: Log the ex
                result.Errors.Add(new ErrorModel { Message = "An error occurred while processing your request.  Please refer to the following techinical error details  :" + ex.GetBaseException() });
                result.StatusCode = 2;
            }
            _logger.LogIntegrationTransaction("InsuranceCompaniesCallBackController/GetPolicyRequestAdditionalInfo",
                new JavaScriptSerializer().Serialize(policyRequestModel), new JavaScriptSerializer().Serialize(result), result.StatusCode);

            return result;
        }


        #region Private Methods

        private void ValidateGivenPolicyUploadNotificationModel(CommonResponseModel result, PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            if (policyUploadNotificationModel == null)
            {
                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "There is no given parameters" });
            }

            if (string.IsNullOrEmpty(policyUploadNotificationModel.ReferenceId))
            {
                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "The ReferenceId can not be empty" });
            }

            if (string.IsNullOrEmpty(policyUploadNotificationModel.PolicyNo))
            {
                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "The Policy number can not be empty" });
            }

            if (policyUploadNotificationModel.StatusCode < 1)
            {
                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "The Status code can not be less than 1" });
            }

            if (policyUploadNotificationModel.StatusCode ==1 && policyUploadNotificationModel.UploadedDate == null )
            {
                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "Upload date is required when status code is 1." });
            }

            if (_iPolicyRepository.GetPolicyWithReferenceIdAndPolicyNumber(policyUploadNotificationModel) == null)
            {

                result.StatusCode = 2;
                if (result.Errors == null)
                    result.Errors = new List<ErrorModel>();
                result.Errors.Add(new ErrorModel { Message = "There is no defined policy with the given ReferenceId and Policy Number" });
            }
        }

        /// <summary>
        /// Validate the Policy Attachment Model 
        /// </summary>
        /// <param name="policyAttachmentModel"></param>
        /// <param name="result"></param>
        private void ValidatePolicyAttachmentModel(CommonPolicyRequestModel policyAttachmentModel, CommonResponseModel result)
        {
            result.StatusCode = 1;
            if (policyAttachmentModel == null)
            {
                result.Errors.Add(new ErrorModel { Message = "ReferenceId and PolicyNo are required." });
                result.StatusCode = 2;
            }
            if (!_iPolicyRepository.IsPolicyExist(policyAttachmentModel.ReferenceId, policyAttachmentModel.PolicyNo))
            {
                result.Errors.Add(new ErrorModel { Message = "There is no policy with this ReferenceId and PolicyNo" });
                result.StatusCode = 2;
            }

        }

        /// <summary>
        /// Validate the Policy Request Additional Info Model 
        /// </summary>
        /// <param name="policyRequestModel"></param>
        /// <param name="result"></param>
        private void ValidatePolicyRequestAdditionalInfoModel(CommonPolicyRequestModel policyRequestModel, CommonResponseModel result)
        {
            result.StatusCode = 1;
            if (policyRequestModel == null)
            {
                result.Errors.Add(new ErrorModel { Message = "ReferenceId and PolicyNo are required." });
                result.StatusCode = 2;
            }
            if (!_iPolicyRepository.CheckPolicyExistenceByReferenceIdOrPolicyNo(policyRequestModel.ReferenceId, policyRequestModel.PolicyNo))
            {
                result.Errors.Add(new ErrorModel { Message = "There is no checkout details with this referenceId or policy with the given policy number." });
                result.StatusCode = 2;
            }

        }

        public PolicyAttachmentResponseModel GetPolicyAttachmentsWithURL(CommonPolicyRequestModel policyAttachmentModel)
        {
            PolicyAttachmentResponseModel result = new PolicyAttachmentResponseModel();
            try
            {
                ValidatePolicyAttachmentModel(policyAttachmentModel, result);
                if (result.StatusCode == 2)
                {
                    _logger.Log($"GetPolicyAttachments validation errors in the model, errors:{JsonConvert.SerializeObject(result)}");
                    return result;
                }
                else
                {
                    result.Attachments = _iCheckoutDetailsRepository.GetCarImagesWithURL(policyAttachmentModel.ReferenceId);
                    result.ReferenceId = policyAttachmentModel.ReferenceId;
                    result.StatusCode = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Log("GetPolicyAttachments an error occurred see the exception", ex);
                result.Errors.Add(new ErrorModel { Message = "An error occurred while processing your request.  Please refer to the following techinical error details :" + ex.ToString() });
                result.StatusCode = 2;

            }

            return result;
        }

        public WataniyaCallBackOutput UpdateWataniyaPolicyinfo(WataniyaPolicyinfoCallbackRequest callBackRequest)
        {
            WataniyaCallBackOutput output = new WataniyaCallBackOutput();
            PolicyNotificationLog log = new PolicyNotificationLog();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(callBackRequest);
            log.CompanyName = "Wataniya";
            log.MethodName = "WataniyaPolicyinfoCallback";
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.Requester = Utilities.GetUrlReferrer();

            try
            {
                if (callBackRequest == null)
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Request is null";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }
                if (string.IsNullOrEmpty(callBackRequest.RequestReferenceNo) && string.IsNullOrEmpty(callBackRequest.PolicyRequestReferenceNo))
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Request Reference Id is null";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }
                if (string.IsNullOrEmpty(callBackRequest.NewPolicyNumber))
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Policy Number is null";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }

                log.ReferenceId = (callBackRequest.InsuranceTypeID != null && callBackRequest.InsuranceTypeID.HasValue && callBackRequest.InsuranceTypeID == 1)
                                    ? callBackRequest.RequestReferenceNo.ToString() : callBackRequest.PolicyRequestReferenceNo.ToString();
                WataniyaMotorPolicyInfo policyInitialInfo = new WataniyaMotorPolicyInfo();
                if (callBackRequest.InsuranceTypeID.HasValue && callBackRequest.InsuranceTypeID == 1)
                    policyInitialInfo = _iPolicyRepository.CheckTplWataniyaInitialPolicyExistenceByReferenceId(callBackRequest.RequestReferenceNo.ToString());
                else
                    policyInitialInfo = _iPolicyRepository.CheckCompWataniyaInitialPolicyExistenceByReferenceId(callBackRequest.PolicyRequestReferenceNo.ToString());

                if (policyInitialInfo == null)
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullResponse;
                    if (callBackRequest.InsuranceTypeID != null && callBackRequest.InsuranceTypeID.HasValue && callBackRequest.InsuranceTypeID == 1)
                        output.ErrorDescription = "There is no policy info with this RequestReferenceNo: " + log.ReferenceId;
                    else
                        output.ErrorDescription = "There is no policy info with this PolicyRequestReferenceNo: " + log.ReferenceId;
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }

                policyInitialInfo.PolicyNo = callBackRequest.NewPolicyNumber;
                policyInitialInfo.OldPolicyNo = callBackRequest.OldPolicyNumber;
                policyInitialInfo.InsuranceCompanyCode = callBackRequest.InsuranceCompanyCode;
                policyInitialInfo.InsuranceTypeID = (callBackRequest.InsuranceTypeID.HasValue && callBackRequest.InsuranceTypeID == 1) ? 1 : 2;
                policyInitialInfo.CallBackRequest = JsonConvert.SerializeObject(callBackRequest);
                if (callBackRequest.InsuranceTypeID.HasValue && callBackRequest.InsuranceTypeID == 1)
                    policyInitialInfo.QuoteReferenceNo = callBackRequest.QuoteReferenceNo.ToString();
                else
                    policyInitialInfo.QuoteReferenceNo = callBackRequest.PolicyReferenceNo.ToString();

                string exception = string.Empty;
                var update = _iPolicyRepository.UpdateWataniyaPolicyInfoCallback(policyInitialInfo, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "An error occurred while processing your request. Please refer to the following techinical error details :" + exception;
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }

                _wataniyaNajmQueueService.AddWataniyaNajmQueue(callBackRequest.NewPolicyNumber, log.ReferenceId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend while inserting in Wataniya Najm Processing Queue, and exception is :" + exception;
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                }

                output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.StatusCode = (int)output.ErrorCode;
                log.StatusDescription = output.ErrorDescription;
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                output.CallbackResponse.Status = true;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.StatusCode = (int)output.ErrorCode;
                log.StatusDescription = output.ErrorDescription;
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                output.CallbackResponse.Status = false;
                return output;
            }
        }

        public WataniyaCallBackOutput UpdateWataniyaVehicleId(WataniyaUpdateVehicleIdCallbackRequest callBackRequest)
        {
            WataniyaCallBackOutput output = new WataniyaCallBackOutput();
            PolicyNotificationLog log = new PolicyNotificationLog();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(callBackRequest);
            log.CompanyName = "Wataniya";
            log.MethodName = "WataniyaUpdateVehicleIdCallback";
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.Requester = Utilities.GetUrlReferrer();

            try
            {
                if (callBackRequest == null)
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Request is null";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }
                if (string.IsNullOrEmpty(callBackRequest.RequestReferenceNo) && string.IsNullOrEmpty(callBackRequest.PolicyRequestReferenceNo))
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Request Reference Id is null";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }
                if (callBackRequest.NajmVehicleId <= 0)
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "NajmVehicleId value is invalid";
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }

                log.ReferenceId = (callBackRequest.InsuranceTypeID == 1) ? callBackRequest.RequestReferenceNo.ToString() : callBackRequest.PolicyRequestReferenceNo.ToString();
                WataniyaMotorPolicyInfo policyInitialInfo = new WataniyaMotorPolicyInfo();
                if (callBackRequest.InsuranceTypeID == 1)
                    policyInitialInfo = _iPolicyRepository.CheckTplWataniyaInitialPolicyExistenceByReferenceId(callBackRequest.RequestReferenceNo.ToString());
                else
                    policyInitialInfo = _iPolicyRepository.CheckCompWataniyaInitialPolicyExistenceByReferenceId(callBackRequest.PolicyRequestReferenceNo.ToString());

                if (policyInitialInfo == null)
                {
                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.NullResponse;
                    if (callBackRequest.InsuranceTypeID == 1)
                        output.ErrorDescription = "There is no policy info with this RequestReferenceNo: " + log.ReferenceId;
                    else
                        output.ErrorDescription = "There is no policy info with this PolicyRequestReferenceNo: " + log.ReferenceId;
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                    output.CallbackResponse.Status = false;
                    return output;
                }

                PolicyUploadNotificationModel policyUploadNotificationModel = new PolicyUploadNotificationModel();
                policyUploadNotificationModel.ReferenceId = policyInitialInfo.ReferenceId; // (callBackRequest.InsuranceTypeID == 1) ? callBackRequest.RequestReferenceNo : callBackRequest.PolicyRequestReferenceNo;
                policyUploadNotificationModel.PolicyNo = policyInitialInfo.PolicyNo;
                policyUploadNotificationModel.StatusCode = 1; // set it's value with 1 as they will call me back after success with najm
                //policyUploadNotificationModel.StatusDescription = (policyUploadNotificationModel.StatusCode == 1) ? "Under Process" : (policyUploadNotificationModel.StatusCode == 2) ? "Failed" : "Submitted";
                policyUploadNotificationModel.UploadedDate = callBackRequest.PolicyUploadedDateTime;
                policyUploadNotificationModel.UploadedReference = callBackRequest.NajmVehicleId.ToString();

                CommonResponseModel result = new CommonResponseModel();
                result.ReferenceId = policyUploadNotificationModel.ReferenceId;

                ValidateGivenPolicyUploadNotificationModel(result, policyUploadNotificationModel);
                if (result.StatusCode == 2)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();
                    var validateModelErrors = new List<Errors>();

                    foreach (var error in result.Errors)
                    {
                        servcieErrors.AppendLine(error.Message);
                        var errormodel = new Errors() {
                            code = error.Code,
                            message = error.Message
                        };

                        validateModelErrors.Add(errormodel);
                    }

                    output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Error happend while try to validate UpdateVehicleId model and the error is " + servcieErrors.ToString();
                    log.StatusCode = (int)output.ErrorCode;
                    log.StatusDescription = output.ErrorDescription;
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                    output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                    output.CallbackResponse.errors = validateModelErrors;
                    output.CallbackResponse.Status = false;
                    return output;
                }

             
                _iPolicyRepository.NotifyPolicyUploadCompletion(policyUploadNotificationModel);
                _iPolicyRepository.SavePolicyWithNajmStatus(policyUploadNotificationModel);
                result.StatusCode = 1;

                output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.StatusCode = (int)output.ErrorCode;
                log.StatusDescription = output.ErrorDescription;
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                output.CallbackResponse.Status = true;

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = WataniyaCallBackOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.StatusCode = (int)output.ErrorCode;
                log.StatusDescription = output.ErrorDescription;
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);

                output.CallbackResponse = new WataniyaPolicyinfoCallbackResponse();
                output.CallbackResponse.errors = new List<Errors>() { new Errors { message = output.ErrorDescription } };
                output.CallbackResponse.Status = false;
                return output;
            }
        }

        #endregion

        #region Renewal Policies Call Back

        public RenewedPoliciesServiceResponse RenewedPoliciesServiceByCompany(RenewedPoliciesServiceRequest callBackRequest)
        {
            RenewedPoliciesServiceResponse output = new RenewedPoliciesServiceResponse();
            PolicyNotificationLog log = new PolicyNotificationLog();
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.ServiceRequest = JsonConvert.SerializeObject(callBackRequest);
            log.MethodName = "RenewedPoliciesServiceByCompany";
            log.UserIP = Utilities.GetUserIPAddress();
            log.UserAgent = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.Requester = Utilities.GetUrlReferrer();

            try
            {
                if (!ValidateRequest(callBackRequest, log, output))
                    return output;

                var renewalPoliciesInDB = _policyRenewedByCompany.TableNoTracking.FirstOrDefault(p => p.ReferenceId == callBackRequest.ReferenceId);
                if (renewalPoliciesInDB != null)
                {
                    output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.ServiceError.ToString(), Message = $"policy data with the same referenceId  = {callBackRequest.ReferenceId} already exists" });
                    log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                    log.StatusDescription = "policy data with the same referenceId already exists";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    output.ReferenceId = callBackRequest.ReferenceId;
                    output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.ServiceError;
                    return output;
                }

                var bcarePolicy = _policyRepo.TableNoTracking.Include(p => p.CheckoutDetail).Include(p => p.CheckoutDetail.Vehicle).FirstOrDefault(p => p.PolicyNo == callBackRequest.OldPolicyNo);
                if (bcarePolicy == null)
                {
                    output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = $"Old policy No {callBackRequest.OldPolicyNo} not exist " });
                    log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                    log.StatusDescription = "old policy not exist on DB";
                    PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                    output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                    return output;
                }

                PolicyRenewedByCompany renewalPolicies = new PolicyRenewedByCompany();
                renewalPolicies.VehicleId = bcarePolicy.CheckoutDetail?.VehicleId;
                renewalPolicies.SequanceNo = bcarePolicy.CheckoutDetail?.Vehicle?.SequenceNumber;
                renewalPolicies.InsuranceCompanyId = bcarePolicy.InsuranceCompanyID.Value;
                renewalPolicies.OldPolicyNo = bcarePolicy.PolicyNo;
                renewalPolicies.ReferenceId = callBackRequest.ReferenceId;
                renewalPolicies.NewPolicyNo = callBackRequest.NewPolicyNo;
                renewalPolicies.ProductTypeCode = callBackRequest.ProductTypeCode;
                renewalPolicies.RenewalDate = callBackRequest.RenewalDate;
                renewalPolicies.PolicyAmount = callBackRequest.PolicyAmount;
                renewalPolicies.PolicyVAT = callBackRequest.PolicyVAT;
                renewalPolicies.PolicyTotalAmount = callBackRequest.PolicyTotalAmount;
                renewalPolicies.PolicyRenewalCommission = callBackRequest.PolicyRenewalCommission;
                _policyRenewedByCompany.Insert(renewalPolicies);

                output.ReferenceId = callBackRequest.ReferenceId;
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.Success;
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.Success;
                log.StatusDescription = "Success";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.ServiceException.ToString(), Message = ex.ToString() });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.ServiceException;
                log.StatusDescription = ex.ToString();
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.ServiceException;
                return output;
            }
        }

        private bool ValidateRequest(RenewedPoliciesServiceRequest callBackRequest, PolicyNotificationLog log, RenewedPoliciesServiceResponse output)
        {
            if (callBackRequest == null)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = "Request model is null" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = "Request model is null";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (string.IsNullOrEmpty(callBackRequest.ReferenceId))
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = "Reference Id is null" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Reference Id is null";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (string.IsNullOrEmpty(callBackRequest.OldPolicyNo))
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = "Old policy No. is null" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = "Old policy No. is null";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (string.IsNullOrEmpty(callBackRequest.NewPolicyNo))
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = "Request new policy no. is null" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = "Request new policy no. is null";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (string.IsNullOrEmpty(callBackRequest.RenewalDate))
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = " Renewal Date is null" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Renewal Date is null";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (callBackRequest.PolicyAmount <= 0)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = " Invalid PolicyAmount" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Invalid PolicyAmount ";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (callBackRequest.PolicyTotalAmount <= 0)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = " Invalid PolicyTotalAmount" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Invalid PolicyTotalAmount ";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (callBackRequest.PolicyVAT <= 0)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = " Invalid PolicyVAT" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Invalid PolicyVAT ";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }
            if (callBackRequest.PolicyRenewalCommission <= 0)
            {
                output.Errors.Add(new ErrorModel { Code = RenewedPoliciesServiceResponse.ErrorCodes.NullRequest.ToString(), Message = " Invalid PolicyRenewalCommission" });
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = " Invalid PolicyRenewalCommission ";
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
                output.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                return false;
            }

            return true;
        }

        public void AddRequestLog(PolicyNotificationLog log)
        {
            try
            {
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
            }
            catch (Exception ex)
            {
                log.StatusDescription = ex.ToString();
                PolicyNotificationLogDataAccess.AddtoPolicyNotificationLogs(log);
            }
        }

        #endregion
    }
}
