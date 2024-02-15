using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Inquiry.Components;
using QuotationComponent = Tameenk.Services.Quotation.Components;
using IntegrationtoQuotation = Tameenk.Integration.Dto.Quotation;
using Tameenk.Security.Encryption;
using System.Web;
using System.Threading.Tasks;

namespace Tameenk.IVR.Component
{
    public class IVRenewalContext : IIVRenewalContext
    {
        private readonly IIVRService _iVRService;
        private readonly IInquiryContext _inquiryContext;
        private readonly IRepository<QuotationBlockedNins> _quotationBlockUser;
        private readonly QuotationComponent.IQuotationContext _quotationContext;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<QuotationResponseCache> _quotationResponseCache;
        private readonly ICheckoutContext _checkoutContext;
        private const string IVR_SHARED_KEY = "Tameenk_IVR_2022_SharedKey_@$";

        public IVRenewalContext(IIVRService iVRService, IInquiryContext inquiryContext, IRepository<QuotationBlockedNins> quotationBlockUser, QuotationComponent.IQuotationContext quotationContext,
                                IRepository<InsuranceCompany> insuranceCompanyRepository, IRepository<QuotationResponseCache> quotationResponseCache, ICheckoutContext checkoutContext)
        {
            _iVRService = iVRService;
            _inquiryContext = inquiryContext;
            _quotationBlockUser = quotationBlockUser;
            _quotationContext = quotationContext;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _quotationResponseCache = quotationResponseCache;
            _checkoutContext = checkoutContext;
        }

        public IVRRenewalOutput<RenewalQuotationResponseModel> ProcessRenewalRequest(RenewalInquiryRequestModel renewalModel)
        {
            var inquiryResult = ProcessInquireDetails(renewalModel, "SubmitRenewalInquiryRequest");
            if (inquiryResult.ErrorCode != IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.Success || !inquiryResult.Result.IsSuccess)
            {
                var output = new IVRRenewalOutput<RenewalQuotationResponseModel>();
                output.ErrorCode = (IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes)inquiryResult.ErrorCode;
                output.ErrorDescription = inquiryResult.ErrorDescription;
                output.Result = new RenewalQuotationResponseModel() { IsSuccess = false };
                return output;
            }

            var quotaitonResult = ProcessQuotationDetails(inquiryResult.Result.OldPolicyDetails, inquiryResult.Result.ExternalId, renewalModel.VehicleId, "SubmitRenewalQuotationRequest");
            return quotaitonResult;
        }

        public IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> SendSadadNumberSMS(RenewalSendLowestLinkSMSRequestModel request)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> output = new IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>();
            output.Result = new RenewalSendLowestLinkSMSResponseModel() { IsSuccess = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            AddBasicLog(log, "SendSadadNumberBySMS", IVRModuleEnum.Renewal);

            try
            {
                if (request == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyDataModel;
                    output.ErrorDescription = "Sadad model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(request.Token))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Missing data in request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var decryptedToken = AESEncryption.DecryptString(Utilities.GetDecodeUrl(request.Token), IVR_SHARED_KEY);
                var lowestProduct = JsonConvert.DeserializeObject<RenewalAddItemToChartUserModel>(decryptedToken);
                if (lowestProduct == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Token sent in request is invalid";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model after DeserializeObject is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(lowestProduct.ReferenceId))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Token sent in request is invalid";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Can not proceed request, as lowestProduct.ReferenceId is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(lowestProduct.ExternalId))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Token sent in request is invalid";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Can not proceed request, as lowestProduct.ExternalId is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(lowestProduct.ProductId.ToString()))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Token sent in request is invalid";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Can not proceed request, as lowestProduct.ProductId is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(lowestProduct);

                var sadadoutput = _checkoutContext.HandleIVRSendSadadNumber(lowestProduct, request.SendToCallerPhone, Utilities.ValidatePhoneNumber(request.PhoneNumber), log);
                return sadadoutput;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error happend while send sned sadad number";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Error happend while send sned sadad number, and error is: {ex.ToString()}";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> SendLowestPriceLinkBySMS(RenewalSendLowestLinkSMSRequestModel request)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> output = new IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>();
            output.Result = new RenewalSendLowestLinkSMSResponseModel() { IsSuccess = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            AddBasicLog(log, "SendLowestPriceLinkBySMS", IVRModuleEnum.Renewal);

            try
            {
                if (request == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyDataModel;
                    output.ErrorDescription = "SMS model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(request.Token))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Missing data in request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var decryptedToken = AESEncryption.DecryptString(Utilities.GetDecodeUrl(request.Token), IVR_SHARED_KEY);
                var model = JsonConvert.DeserializeObject<RenewalAddItemToChartUserModel>(decryptedToken);
                if (model == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Missing data in request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model after DeserializeObject is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(model);

                string exception;
                string phoneToSendSMS = request.SendToCallerPhone ? Utilities.ValidatePhoneNumber(request.PhoneNumber) : model.Phone;
                string smsBody = " من فضلك اتبع الرابط التالى لإكمال طلب التجديد, \n\n https://bcare.com.sa/checkoutIVR?tkn=" + (HttpUtility.UrlEncode(request.Token)).Trim();
                //var sendSms = SendSmsMessage("0543873218", smsBody, SMSMethod.QuotationShare, out exception);
                var sendSms = SendSmsMessage(phoneToSendSMS, smsBody, SMSMethod.QuotationShare, out exception);
                if (!string.IsNullOrEmpty(exception) || !sendSms)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send lowest product link sms ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "sendSms is false";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result.IsSuccess = true;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Send Lowest Price Link SMS Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing SendLowestPriceLinkBySMS ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }


        #region Shared Methods

        public void AddBasicLog(IVRServicesLog log, string methodName, IVRModuleEnum module)
        {
            log.Method = methodName;
            log.ModuleId = (int)module;
            log.ModuleName = module.ToString();
            log.CreatedDate = DateTime.Now;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
        }

        #endregion

        #region Private Methods

        #region Inquiry

        private IVRRenewalOutput<RenewalInquiryResponseModel> ProcessInquireDetails(RenewalInquiryRequestModel renewalModel, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRRenewalOutput<RenewalInquiryResponseModel> output = new IVRRenewalOutput<RenewalInquiryResponseModel>();
            output.Result = new RenewalInquiryResponseModel() { IsSuccess = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(renewalModel);
            AddBasicLog(log, methodName, IVRModuleEnum.Renewal);

            try
            {
                if (renewalModel == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.EmptyDataModel;
                    output.ErrorDescription = "Renewal data model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(renewalModel.VehicleId))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "VehicleId value is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(renewalModel.NationalId))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "NationalId value is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                string exception;
                IVRTicketPolicyDetails oldPolicyDetails = _iVRService.GetLastPolicyBySequenceOrCustomCardNumber(renewalModel.VehicleId, out exception);
                if (!string.IsNullOrEmpty(exception) || oldPolicyDetails == null)
                {
                    output.ErrorCode = !string.IsNullOrEmpty(exception) ? IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.ServiceException : IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = !string.IsNullOrEmpty(exception) ? "Error happend while renewal process" : "There is no old policy data for the provided vehicleId " + renewalModel.VehicleId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "oldPolicyDetails is null for the provided vehilceId " + renewalModel.VehicleId;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (!oldPolicyDetails.PolicyExpiryDate.HasValue || oldPolicyDetails.PolicyExpiryDate.Value > DateTime.Now.AddDays(28) || oldPolicyDetails.PolicyExpiryDate.Value.AddDays(30) < DateTime.Now)
                {
                    var errorDescription = !oldPolicyDetails.PolicyExpiryDate.HasValue
                                            ? "Error happend while renewal process"
                                            : oldPolicyDetails.PolicyExpiryDate.Value > DateTime.Now.AddDays(28)
                                                ? "Can not renew your policy as it's expiration is > 28 days"
                                                : "Can not renew your policy since it has been more than a month since the expiration date";

                    output.ErrorCode = !oldPolicyDetails.PolicyExpiryDate.HasValue ? IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.ServiceException : IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.OldPolicyExpirationGreaterThen28Days;
                    output.ErrorDescription = errorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !oldPolicyDetails.PolicyExpiryDate.HasValue ? "oldPolicyDetails.PolicyExpiryDate value is null" : "Can not renew your policy as it's expiration is > 28 days";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                exception = string.Empty;
                InquiryRequestModel submitInquiryRequestModel = InitInquiryRequest(oldPolicyDetails, out exception);
                log.ServiceRequest = JsonConvert.SerializeObject(submitInquiryRequestModel);
                if (!string.IsNullOrEmpty(exception) || submitInquiryRequestModel == null)
                {
                    output.ErrorCode = !string.IsNullOrEmpty(exception) ? IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.InitInquiryProcessServiceException : IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "Error happend while renewal process";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "InitInquiryRequest return null";

                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var submitInquiryResult = _inquiryContext.SubmitInquiryRequest(submitInquiryRequestModel, "IVR", Guid.Empty, "IVR");
                if (!submitInquiryResult.InquiryResponseModel.IsValidInquiryRequest && submitInquiryResult.InquiryResponseModel.YakeenMissingFields != null && submitInquiryResult.InquiryResponseModel.YakeenMissingFields.Count > 0)
                {
                    exception = string.Empty;
                    var isSent = SendRenewalLink(oldPolicyDetails, renewalModel.VehicleId, out exception);

                    output.ErrorCode = isSent ? IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.MissingFields : IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.MissingFieldsSendSmsException;
                    output.ErrorDescription = "Processing SubmitInquiryRequest error: " + (isSent ? "MissingFields" : "MissingFieldsSendSmsException");
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = isSent ? "Request is invalid as there is missing fields" : "Error happend while send sms link, and error is: " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (submitInquiryResult.ErrorCode != InquiryOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.SubmitInquiryRequestServiceException;
                    output.ErrorDescription = "Processing SubmitInquiryRequest error: SubmitInquiryRequestServiceException";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "SubmitInquiryRequest return error, and error is: " + submitInquiryResult.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result.IsSuccess = true;
                output.Result.ExternalId = submitInquiryResult.InquiryResponseModel.QuotationRequestExternalId;
                output.Result.OldPolicyDetails = oldPolicyDetails;
                output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRRenewalOutput<RenewalInquiryResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Inquiry Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing Inquiry ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        private InquiryRequestModel InitInquiryRequest(IVRTicketPolicyDetails oldPolicyDetails, out string exception)
        {
            exception = string.Empty;
            InquiryRequestModel output = null;
            try
            {
                if (oldPolicyDetails.VehicleIdTypeId == 0)
                {
                    exception = "Vehicle Id type is Zero";
                    return output;
                }

                var newPolicyEffectiveDate = (oldPolicyDetails.PolicyExpiryDate.Value.Date < DateTime.Now.Date) ? DateTime.Now.AddDays(1) : oldPolicyDetails.PolicyExpiryDate.Value.AddDays(1);
                if (newPolicyEffectiveDate < DateTime.Now.Date.AddDays(1) || newPolicyEffectiveDate > DateTime.Now.AddDays(29))
                {
                    exception = "Policy effective date should be within 29 days starts from toworrow";
                    return output;
                }

                RenewalPolicyDriversDataModel oldPolicyDriversDetails = _iVRService.GetLastPolicyDriversCheckoutuserIdAndPolicyNo(oldPolicyDetails.CheckoutUserId, oldPolicyDetails.PolicyNo, out exception);
                if (!string.IsNullOrEmpty(exception) || oldPolicyDetails == null)
                {
                    exception = !string.IsNullOrEmpty(exception) ? exception : "GetLastPolicyDriversCheckoutuserIdAndPolicyNo return null for the provided userId: " + oldPolicyDetails.CheckoutUserId + " and policyNo: " + oldPolicyDetails.PolicyNo;
                    return output;
                }

                List<string> NationalIds = new List<string>();
                NationalIds.Add(oldPolicyDriversDetails.NIN);
                if (!string.IsNullOrEmpty(oldPolicyDriversDetails.NINDriver1))
                    NationalIds.Add(oldPolicyDriversDetails.NINDriver1);
                if (!string.IsNullOrEmpty(oldPolicyDriversDetails.NINDriver2))
                    NationalIds.Add(oldPolicyDriversDetails.NINDriver2);
                var checkBlockUsers = IsRequestValidForBlockedUsers(NationalIds, out exception);
                if (!string.IsNullOrEmpty(exception) || !checkBlockUsers)
                {
                    exception = "IsRequestValidForBlockedUsers return: " + exception;
                    return output;
                }

                output = HandleRenewalSubmitInquiryData(oldPolicyDriversDetails, newPolicyEffectiveDate);
                return output;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return output;
            }
        }

        private bool SendRenewalLink(IVRTicketPolicyDetails oldPolicyDetails, string vehicleId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var smsBody = "بيانات مركبتك  " + oldPolicyDetails.VehicleMaker + " " + oldPolicyDetails.VehicleModel + " غير مكتمملة, من فضلك اتبع الرابط التالى لإكمال طلب التجديد \n\n https://bcare.com.sa/?eid=" + oldPolicyDetails.ExternalId + "&r=1&re=" + oldPolicyDetails.ReferenceId;
                _iVRService.SendSMS("966599000578", smsBody, SMSMethod.PolicyRenewal, out exception);
                //_iVRService.SendSMS(oldPolicyDetails.CheckoutDetailPhone, smsBody, SMSMethod.PolicyRenewal, out exception);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private bool IsRequestValidForBlockedUsers(List<string> nationalIds, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (nationalIds == null || nationalIds.Count <= 0)
                {
                    exception = "nationalIds in null";
                    return false;
                }

                var blockUsers = _quotationBlockUser.TableNoTracking.Where(a => nationalIds.Contains(a.NationalId)).ToList();
                if (blockUsers != null && blockUsers.Count > 0)
                {
                    exception = "Request have blocked users, and users are: " + String.Join(", ", blockUsers.Select(a => a.NationalId));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private InquiryRequestModel HandleRenewalSubmitInquiryData(RenewalPolicyDriversDataModel driversDataModel, DateTime effectiveDate)
        {
            InquiryRequestModel submitInquiryData = new InquiryRequestModel();
            try
            {
                submitInquiryData.CityCode = driversDataModel.CityId ?? 1;
                submitInquiryData.PolicyEffectiveDate = effectiveDate;
                submitInquiryData.IsVehicleUsedCommercially = driversDataModel.IsUsedCommercially ?? false;
                submitInquiryData.IsCustomerSpecialNeed = driversDataModel.IsSpecialNeed;
                submitInquiryData.IsRenewalRequest = true;
                submitInquiryData.PreviousReferenceId = driversDataModel.ReferenceId;
                if (driversDataModel.OwnerTransfer)
                {
                    long oldOwnerNin = 0;
                    long.TryParse(driversDataModel.OwnerNationalId, out oldOwnerNin);
                    submitInquiryData.OldOwnerNin = oldOwnerNin;
                }

                #region Vehicle Data

                long vehicleId = 0;
                if (driversDataModel.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                    long.TryParse(driversDataModel.CustomCardNumber, out vehicleId);
                else
                    long.TryParse(driversDataModel.SequenceNumber, out vehicleId);

                submitInquiryData.Vehicle = new Tameenk.Services.Inquiry.Components.VehicleModel();
                if (driversDataModel.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                    submitInquiryData.Vehicle.CustomCardNumber = driversDataModel.CustomCardNumber;
                else
                    submitInquiryData.Vehicle.SequenceNumber = driversDataModel.SequenceNumber;
                submitInquiryData.Vehicle.ID = driversDataModel.ID;
                submitInquiryData.Vehicle.VehicleId = vehicleId;
                submitInquiryData.Vehicle.ApproximateValue = driversDataModel.VehicleValue.Value;
                submitInquiryData.Vehicle.VehicleMaker = driversDataModel.VehicleMaker;
                submitInquiryData.Vehicle.VehicleMakerCode = driversDataModel.VehicleMakerCode;
                submitInquiryData.Vehicle.Model = driversDataModel.VehicleModel;
                submitInquiryData.Vehicle.VehicleModelCode = driversDataModel.VehicleModelCode;
                submitInquiryData.Vehicle.VehicleModelYear = driversDataModel.ModelYear;
                submitInquiryData.Vehicle.CarPlateText1 = driversDataModel.CarPlateText1;
                submitInquiryData.Vehicle.CarPlateText2 = driversDataModel.CarPlateText2;
                submitInquiryData.Vehicle.CarPlateText3 = driversDataModel.CarPlateText3;
                submitInquiryData.Vehicle.CarPlateNumber = driversDataModel.CarPlateNumber;
                submitInquiryData.Vehicle.PlateTypeCode = driversDataModel.PlateTypeCode;
                submitInquiryData.Vehicle.VehicleIdTypeId = driversDataModel.VehicleIdTypeId;
                submitInquiryData.Vehicle.Cylinders = driversDataModel.Cylinders;
                submitInquiryData.Vehicle.LicenseExpiryDate = driversDataModel.LicenseExpiryDate;
                submitInquiryData.Vehicle.MajorColor = driversDataModel.MajorColor;
                submitInquiryData.Vehicle.MinorColor = driversDataModel.MinorColor;
                submitInquiryData.Vehicle.ModelYear = driversDataModel.ModelYear;
                submitInquiryData.Vehicle.ManufactureYear = driversDataModel.ModelYear;
                submitInquiryData.Vehicle.RegisterationPlace = driversDataModel.RegisterationPlace;
                submitInquiryData.Vehicle.VehicleBodyCode = driversDataModel.VehicleBodyCode;
                submitInquiryData.Vehicle.VehicleWeight = driversDataModel.VehicleWeight;
                submitInquiryData.Vehicle.VehicleLoad = driversDataModel.VehicleLoad;
                submitInquiryData.Vehicle.TransmissionTypeId = (driversDataModel.TransmissionTypeId.HasValue && driversDataModel.TransmissionTypeId.Value > 0) ? driversDataModel.TransmissionTypeId.Value : 1;
                submitInquiryData.Vehicle.ChassisNumber = driversDataModel.ChassisNumber;
                submitInquiryData.Vehicle.HasModification = driversDataModel.HasModifications;
                submitInquiryData.Vehicle.Modification = driversDataModel.ModificationDetails;
                submitInquiryData.Vehicle.MileageExpectedAnnualId = driversDataModel.MileageExpectedAnnualId ?? 1;
                submitInquiryData.Vehicle.ParkingLocationId = driversDataModel.ParkingLocationId ?? 1;
                submitInquiryData.Vehicle.OwnerNationalId = driversDataModel.CarOwnerNIN;
                submitInquiryData.Vehicle.OwnerTransfer = driversDataModel.OwnerTransfer;
                submitInquiryData.Vehicle.Modification = string.Empty;
                submitInquiryData.Vehicle.HasModification = false;

                #endregion

                #region Main Driver Data

                submitInquiryData.Drivers = new List<DriverModel>();
                DriverModel mainDriver = new DriverModel
                {
                    NationalId = driversDataModel.NIN,
                    ChildrenBelow16Years = driversDataModel.ChildrenBelow16Years ?? 0,
                    EducationId = driversDataModel.EducationId,
                    DrivingPercentage = 100,
                    MedicalConditionId = driversDataModel.MedicalConditionId ?? 1,
                    DriverNOALast5Years = driversDataModel.NOALast5Years ?? 0,
                    RelationShipId = driversDataModel.RelationShipId ?? 0
                };

                submitInquiryData.Insured = new InsuredModel
                {
                    NationalId = driversDataModel.NIN,
                    ChildrenBelow16Years = driversDataModel.ChildrenBelow16Years ?? 0,
                    EducationId = driversDataModel.EducationId
                };
                if (driversDataModel.IsCitizen)
                {
                    if (!string.IsNullOrWhiteSpace(driversDataModel.DateOfBirthH))
                    {
                        var dateH = driversDataModel.DateOfBirthH.Split('-');
                        submitInquiryData.Insured.BirthDateMonth = Convert.ToByte(dateH[1]);
                        submitInquiryData.Insured.BirthDateYear = short.Parse(dateH[2]);
                        mainDriver.BirthDateMonth = Convert.ToByte(dateH[1]);
                        mainDriver.BirthDateYear = short.Parse(dateH[2]);
                    }
                }
                else
                {
                    var dateG = driversDataModel.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                    submitInquiryData.Insured.BirthDateMonth = Convert.ToByte(dateG[1]);
                    submitInquiryData.Insured.BirthDateYear = short.Parse(dateG[2]);
                    mainDriver.BirthDateMonth = Convert.ToByte(dateG[1]);
                    mainDriver.BirthDateYear = short.Parse(dateG[2]);
                }

                mainDriver.ViolationIds = HandleDriverViolations(driversDataModel.MainDriverViolations);
                mainDriver.DriverExtraLicenses = HandleDriverExtraLicenses(driversDataModel.MainDriverExtraLicense);
                submitInquiryData.Drivers.Add(mainDriver);

                #endregion

                #region Additional Drivers Data

                var additionalDrivers = HandleAdditionalDrivers(driversDataModel);
                if (additionalDrivers != null && additionalDrivers.Count > 0)
                    submitInquiryData.Drivers.AddRange(additionalDrivers);

                #endregion

                return submitInquiryData;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\IVRRenewal_submitInquiryData.txt", " Exception is:" + ex.ToString());
                return submitInquiryData;
            }
        }

        private List<int> HandleDriverViolations(string driverViolations)
        {
            List<int> violations = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(driverViolations))
                {
                    var deserializedViolations = JsonConvert.DeserializeObject<List<int>>(driverViolations);
                    if (deserializedViolations != null && deserializedViolations.Count > 0)
                        violations.AddRange(deserializedViolations);
                }

                return violations;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\IVRRenewal_submitInquiryData_HandleDriverViolations.txt", " Exception is:" + ex.ToString());
                return violations;
            }
        }

        private List<DriverExtraLicenseModel> HandleDriverExtraLicenses(string driverExtraLicense)
        {
            List<DriverExtraLicenseModel> licenses = new List<DriverExtraLicenseModel>();
            try
            {
                if (!string.IsNullOrEmpty(driverExtraLicense))
                {
                    var deserializedLicenses = JsonConvert.DeserializeObject<List<DriverExtraLicenseModel>>(driverExtraLicense);
                    if (deserializedLicenses != null && deserializedLicenses.Count > 0)
                        licenses.AddRange(deserializedLicenses);
                }

                return licenses;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\IVRRenewal_submitInquiryData_HandleDriverExtraLicenses.txt", " Exception is:" + ex.ToString());
                return licenses;
            }
        }

        private List<DriverModel> HandleAdditionalDrivers(RenewalPolicyDriversDataModel item)
        {
            List<DriverModel> drivers = new List<DriverModel>();
            try
            {
                // additional driver 1 data
                if (!string.IsNullOrEmpty(item.NINDriver1))
                {
                    DriverModel driver1 = new DriverModel();
                    driver1.NationalId = item.NINDriver1;
                    driver1.MedicalConditionId = item.MedicalConditionIdDriver1.Value;
                    if (item.EducationIdDriver1.HasValue)
                        driver1.EducationId = item.EducationIdDriver1.Value;
                    driver1.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver1.Value;
                    driver1.DrivingPercentage = item.DrivingPercentageDriver1.Value;
                    if (item.IsCitizenDriver1.HasValue && item.IsCitizenDriver1.Value)
                    {
                        var dateH = item.DateOfBirthHDriver1.Split('-');
                        driver1.BirthDateMonth = Convert.ToByte(dateH[1]);
                        driver1.BirthDateYear = short.Parse(dateH[2]);
                    }
                    else
                    {
                        var dateG = item.DateOfBirthGDriver1.Value.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        driver1.BirthDateMonth = Convert.ToByte(dateG[1]);
                        driver1.BirthDateYear = short.Parse(dateG[2]);
                    }

                    if (item.EducationIdDriver1.HasValue)
                        driver1.EducationId = item.EducationIdDriver1.Value;
                    driver1.DriverNOALast5Years = item.NOALast5YearsDriver1;

                    if (item.WorkCityIdDriver1.HasValue)
                    {
                        int workCity = 0;
                        int.TryParse(item.WorkCityIdDriver1.Value.ToString(), out workCity);
                        driver1.DriverWorkCityCode = workCity;
                    }
                    if (item.CityIdDriver1.HasValue)
                    {
                        int homeCity = 0;
                        int.TryParse(item.CityIdDriver1.Value.ToString(), out homeCity);
                        driver1.DriverHomeCityCode = homeCity;
                    }

                    driver1.DriverWorkCity = item.WorkCityNameDriver1;
                    driver1.DriverHomeCity = item.CityNameDriver1;
                    driver1.RelationShipId = item.RelationShipIdDriver1;
                    driver1.ViolationIds = HandleDriverViolations(item.ViolationsDriver1);
                    driver1.DriverExtraLicenses = HandleDriverExtraLicenses(item.ExtraLicenseDriver1);
                    drivers.Add(driver1);
                }

                // additional driver 2 data
                if (!string.IsNullOrEmpty(item.NINDriver2))
                {
                    DriverModel driver2 = new DriverModel();
                    driver2.NationalId = item.NINDriver2;
                    driver2.MedicalConditionId = item.MedicalConditionIdDriver2.Value;
                    if (item.EducationIdDriver2.HasValue)
                        driver2.EducationId = item.EducationIdDriver2.Value;
                    driver2.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver2.Value;
                    driver2.DrivingPercentage = item.DrivingPercentageDriver2.Value;
                    if (item.IsCitizenDriver2.HasValue && item.IsCitizenDriver2.Value)
                    {
                        var dateH = item.DateOfBirthHDriver2.Split('-');
                        driver2.BirthDateMonth = Convert.ToByte(dateH[1]);
                        driver2.BirthDateYear = short.Parse(dateH[2]);
                    }
                    else
                    {
                        var dateG = item.DateOfBirthGDriver2.Value.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        driver2.BirthDateMonth = Convert.ToByte(dateG[1]);
                        driver2.BirthDateYear = short.Parse(dateG[2]);
                    }
                    if (item.EducationIdDriver2.HasValue)
                        driver2.EducationId = item.EducationIdDriver2.Value;
                    driver2.DriverNOALast5Years = item.NOALast5YearsDriver2;

                    if (item.WorkCityIdDriver2.HasValue)
                    {
                        int workCity = 0;
                        int.TryParse(item.WorkCityIdDriver2.Value.ToString(), out workCity);
                        driver2.DriverWorkCityCode = workCity;
                    }
                    if (item.CityIdDriver2.HasValue)
                    {
                        int homeCity = 0;
                        int.TryParse(item.CityIdDriver2.Value.ToString(), out homeCity);
                        driver2.DriverHomeCityCode = homeCity;
                    }

                    driver2.DriverWorkCity = item.WorkCityNameDriver2;
                    driver2.DriverHomeCity = item.CityNameDriver2;
                    driver2.RelationShipId = item.RelationShipIdDriver2;
                    driver2.ViolationIds = HandleDriverViolations(item.ViolationsDriver2);
                    driver2.DriverExtraLicenses = HandleDriverExtraLicenses(item.ExtraLicenseDriver2);
                    drivers.Add(driver2);
                }

                return drivers;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\IVRRenewal_submitInquiryData_HandleAdditionalDrivers.txt", " Exception is:" + ex.ToString());
                return drivers;
            }
        }

        #endregion

        #region Quotaiton

        private IVRRenewalOutput<RenewalQuotationResponseModel> ProcessQuotationDetails(IVRTicketPolicyDetails oldPolicyDetails, string externalId, string vehicleId, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRRenewalOutput<RenewalQuotationResponseModel> output = new IVRRenewalOutput<RenewalQuotationResponseModel>();
            output.Result = new RenewalQuotationResponseModel() { IsSuccess = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = $"externalId: {externalId}, vehicleId: {vehicleId}";
            AddBasicLog(log, methodName, IVRModuleEnum.Renewal);

            try
            {
                string exception;
                int insuranceCompanyID;
                string InsuranceCompanyNameAr;
                string InsuranceCompanyNameEn;
                string InsuranceCompanyKey;
                var lowestProduct = HandleGetQuotaions(oldPolicyDetails, externalId, out insuranceCompanyID, out InsuranceCompanyNameAr, out InsuranceCompanyNameEn, out InsuranceCompanyKey, out exception);
                if (!string.IsNullOrEmpty(exception) || lowestProduct == null)
                {
                    output.ErrorCode = !string.IsNullOrEmpty(exception) ? IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes.ServiceException : IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes.LowestPriceIsEmpty;
                    output.ErrorDescription = !string.IsNullOrEmpty(exception) ? "Error happend while renewal process" : "Processing Exception error: LowestPriceIsEmpty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "lowestProduct is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var renewalAddItemToChartUserModel = new RenewalAddItemToChartUserModel()
                {
                    IsIVR = true,
                    VehicleId = vehicleId,
                    ExternalId = externalId,
                    ProductId = lowestProduct.Id,
                    ReferenceId = lowestProduct.ReferenceId,
                    InsuranceCompanyId = insuranceCompanyID,
                    InsuranceCompanyName = InsuranceCompanyKey,
                    OldPolicyReferenceId = oldPolicyDetails.ReferenceId,
                    SelectedInsuranceTypeCode = lowestProduct.InsuranceTypeCode.Value,
                    OldPolicyUserId = oldPolicyDetails.CheckoutUserId,
                    Phone = oldPolicyDetails.CheckoutDetailPhone
                };
                if (lowestProduct.Quotation_Product_Benefits != null || lowestProduct.Quotation_Product_Benefits.Count() > 0)
                    renewalAddItemToChartUserModel.SelectedProductBenfitId = lowestProduct.Quotation_Product_Benefits.Where(a => a.IsReadOnly && a.IsSelected.HasValue && a.IsSelected == true).Select(a => a.Id).ToList();

                output.Result.IsSuccess = true;
                output.Result.ProductTypeCode = oldPolicyDetails.SelectedInsuranceTypeCode.Value;
                output.Result.CompanyNameAr = InsuranceCompanyNameAr;
                output.Result.CompanyNameEn = InsuranceCompanyNameEn;
                output.Result.ProductPrice = lowestProduct.ProductPrice;
                output.Result.Token = AESEncryption.EncryptString(JsonConvert.SerializeObject(renewalAddItemToChartUserModel), IVR_SHARED_KEY);
                output.ErrorCode = IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ServiceRequest = $"{log.ServiceRequest}, token: {output.Result.Token}";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Quotation Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing Quotation ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        private Product HandleGetQuotaions(IVRTicketPolicyDetails oldPolicyDetails, string externalId, out int companyId, out string companyNameAr, out string companyNameEn, out string companyKey, out string exception)
        {
            companyId = 0;
            exception = string.Empty;
            companyNameAr = string.Empty;
            companyNameEn = string.Empty;
            companyKey = string.Empty;
            Product lowestProduct = null;
            try
            {
                var companies = _insuranceCompanyRepository.TableNoTracking.Where(a => a.IsActive).ToList();
                if (companies == null || companies.Count <= 0)
                    exception = "There is no active companies to get quotations";

                foreach (var company in companies)
                {
                    if (oldPolicyDetails.SelectedInsuranceTypeCode == 1 && company.InsuranceCompanyID != 12)
                        oldPolicyDetails.VehicleAgencyRepair = false;

                    QuotationRequestLog quotationLog = new QuotationRequestLog();
                    quotationLog.UserIP = Utilities.GetUserIPAddress();
                    quotationLog.ServerIP = Utilities.GetInternalServerIP();
                    quotationLog.UserAgent = Utilities.GetUserAgent();
                    quotationLog.ExtrnlId = externalId;
                    quotationLog.InsuranceTypeCode = oldPolicyDetails.SelectedInsuranceTypeCode.Value;
                    quotationLog.CompanyId = company.InsuranceCompanyID;
                    quotationLog.Channel = "IVR";

                    var insuranceTypeCode = oldPolicyDetails.SelectedInsuranceTypeCode ?? 1;
                    var vehicleAgencyRepair = oldPolicyDetails.VehicleAgencyRepair ?? false;
                    var deductibleValue = oldPolicyDetails.DeductableValue;

                    var quotationOutput = HandleGetQuotationTask(company.InsuranceCompanyID, externalId, quotationLog, insuranceTypeCode, vehicleAgencyRepair, deductibleValue);
                    if (quotationOutput == null || quotationOutput.ErrorCode != QuotationComponent.QuotationOutput.ErrorCodes.Success || (quotationOutput.QuotationResponse.Products == null || quotationOutput.QuotationResponse.Products.Count() == 0))
                        continue;

                    IntegrationtoQuotation.ProductModel lowestProductResult = AddQuotationToCach(quotationOutput, company.InsuranceCompanyID, insuranceTypeCode, externalId, vehicleAgencyRepair, deductibleValue, Guid.Empty, out exception);

                    if (lowestProduct == null)
                        lowestProduct = quotationOutput.QuotationResponse.Products.OrderBy(a => a.ProductPrice).FirstOrDefault();
                    else if (quotationOutput.QuotationResponse.Products.Any(a => a.ProductPrice < lowestProduct.ProductPrice))
                        lowestProduct = quotationOutput.QuotationResponse.Products.Where(a => a.ProductPrice < lowestProduct.ProductPrice).FirstOrDefault();

                    companyId = company.InsuranceCompanyID;
                    companyNameAr = company.NameAR;
                    companyNameEn = company.NameEN;
                    companyKey = company.Key;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }

            return lowestProduct;
        }

        private QuotationComponent.QuotationOutput HandleGetQuotationTask(int insuranceCompanyID, string externalId, QuotationRequestLog quotationLog, short insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return _quotationContext.GetQuote(insuranceCompanyID, externalId, quotationLog.Channel, Guid.Empty, "IVR",
                                                        quotationLog, DateTime.Now, new Guid(), insuranceTypeCode, vehicleAgencyRepair, deductibleValue);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(10000));
                if (!isCompletedSuccessfully)
                    return null;

                return task.Result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\HandleGetQuotationTask_" + insuranceCompanyID + "_Exception.txt", ex.ToString());
                return null;
            }
        }

        private IntegrationtoQuotation.ProductModel AddQuotationToCach(QuotationComponent.QuotationOutput quotationOutput, int insuranceCompanyId, int insuranceTypeCode, string qtRqstExtrnlId, bool? vehicleAgencyRepair, int? deductibleValue, Guid selectedUserId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var responseModel = quotationOutput.QuotationResponse.ToModel();
                responseModel.Products.Add(responseModel.Products.OrderBy(a => a.ProductPrice).FirstOrDefault());
                foreach (var product in responseModel.Products)
                {
                    if (product.InsuranceTypeCode == 1)
                        product.ShowTabby = quotationOutput.ActiveTabbyTPL;
                    else if (product.InsuranceTypeCode == 2)
                        product.ShowTabby = quotationOutput.ActiveTabbyComp;
                    else if (product.InsuranceTypeCode == 7)
                        product.ShowTabby = quotationOutput.ActiveTabbySanadPlus;
                    else if (product.InsuranceTypeCode == 8)
                        product.ShowTabby = quotationOutput.ActiveTabbyWafiSmart;
                    else
                        product.ShowTabby = false;

                    if (insuranceCompanyId == 8 && insuranceTypeCode == 2)
                    {
                        if (product.DeductableValue == 0)
                            product.DeductableValue = 2000;
                    }
                    if (product.InsuranceTypeCode == 1 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePath))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePath.Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePath.Replace("_ar", "_en");
                    }
                    else if ((product.InsuranceTypeCode == 2 || product.InsuranceTypeCode == 8 || product.InsuranceTypeCode == 9) && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathComp))
                    {
                        if (product.InsuranceTypeCode == 9)
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_OD").Replace("_ar", "_en");
                        }
                        else if (product.InsuranceTypeCode == 8)
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_Comp", "_Wafi").Replace("_ar", "_en");
                        }
                        else
                        {
                            product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathComp.Replace("_en", "_ar").Replace("_en", "_ar");
                            product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathComp.Replace("_en", "_ar").Replace("_ar", "_en");
                        }
                    }
                    else if (product.InsuranceTypeCode == 7 && !string.IsNullOrEmpty(quotationOutput.TermsAndConditionsFilePathSanadPlus))
                    {
                        product.TermsFilePathAr = quotationOutput.TermsAndConditionsFilePathSanadPlus.Replace("_en", "_ar");
                        product.TermsFilePathEn = quotationOutput.TermsAndConditionsFilePathSanadPlus.Replace("_ar", "_en");
                    }
                    if (product.PriceDetails != null)
                    {
                        List<IntegrationtoQuotation.PriceDetailModel> priceDetails = new List<IntegrationtoQuotation.PriceDetailModel>();
                        var prices = product.PriceDetails.OrderBy(a => a.PriceType.Order).ToList();
                        foreach (var price in prices)
                        {
                            if (price.PriceValue > 0)
                            {
                                if (insuranceCompanyId == 22 && price.PriceTypeCode == 12)
                                {
                                    price.PriceType.EnglishDescription = "COVID-19 Vaccine campaign";
                                    price.PriceType.ArabicDescription = "خصم مبادرة اللقاح كرونا";
                                }
                                if (price.PriceTypeCode == 1 && DateTime.Now.Date <= new DateTime(2022, 09, 30))
                                {
                                    price.PriceType.EnglishDescription = "National Day Discount";
                                    price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                                }
                                if (price.PriceTypeCode == 1)
                                {
                                    if (insuranceCompanyId == 5 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 22))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 14 && DateTime.Now.Date >= new DateTime(2022, 02, 20) && DateTime.Now.Date <= new DateTime(2022, 02, 27))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 20 && DateTime.Now.Date >= new DateTime(2022, 02, 18) && DateTime.Now.Date <= new DateTime(2022, 02, 26))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 26 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 24))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 27 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 22))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 22 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 9 && DateTime.Now.Date >= new DateTime(2022, 02, 21) && DateTime.Now.Date <= new DateTime(2022, 02, 23))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                    else if (insuranceCompanyId == 4 && DateTime.Now.Date >= new DateTime(2022, 02, 22) && DateTime.Now.Date <= new DateTime(2022, 02, 28))
                                    {
                                        price.PriceType.EnglishDescription = "Saudi Foundation Day discount";
                                        price.PriceType.ArabicDescription = "خصم يوم التأسيس السعودي";
                                    }
                                }

                                priceDetails.Add(price);
                            }
                        }
                        if (priceDetails.Count() > 0)
                            product.PriceDetails = priceDetails;
                    }
                }

                QuotationResponseCache cache = new QuotationResponseCache();
                cache.InsuranceCompanyId = insuranceCompanyId;
                cache.ExternalId = qtRqstExtrnlId;
                cache.InsuranceTypeCode = insuranceTypeCode;
                cache.VehicleAgencyRepair = vehicleAgencyRepair;
                cache.DeductibleValue = deductibleValue;
                cache.UserId = selectedUserId;
                cache.QuotationResponse = JsonConvert.SerializeObject(responseModel);
                cache.CreateDateTime = DateTime.Now;
                _quotationResponseCache.Insert(cache);

                return responseModel.Products.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\AddIVRQuotationToCach_Exception.txt", ex.ToString());
                exception = ex.ToString();
                return null;
            }
        }

        #endregion

        #region Send Message

        private bool SendSmsMessage(string phoneNumber, string body, SMSMethod method, out string exception)
        {
            exception = string.Empty;
            try
            {
                _iVRService.SendSMS(phoneNumber, body, method, out exception);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        #endregion

        #endregion
    }
}
