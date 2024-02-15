using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Core.Providers
{

    /// <summary>
    /// The base insurance provider class for 3rd-party integration companies.
    /// </summary>
    public abstract class InsuranceProvider : IInsuranceProvider
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ProviderConfiguration _configuration;
        private readonly IRepository<AutomatedTestIntegrationTransaction> _automatedTestIntegrationTransactionRepository;

        #endregion

        #region Ctor

        public InsuranceProvider(
            ProviderConfiguration configuration,
            ILogger logger
            )
        {
            _configuration = configuration;
            _logger = logger;
            _automatedTestIntegrationTransactionRepository = EngineContext.Current.Resolve<IRepository<AutomatedTestIntegrationTransaction>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate insurance policy from 3rd-party service provider
        /// </summary>
        /// <param name="policy">The policy request details required for policy genration</param>
        /// <returns>A policy response details</returns>
        public virtual PolicyResponse GetPolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedPolicy= HandlePolicyRequestObjectMapping(policy);
            var response = ExecutePolicyRequest(policy, predefinedLogInfo);
            // get provider info
            var providerInfo = GetProviderInfo();
            PolicyResponse result = GetPolicyResponseObject(response, policy);
            //HandlePoilcySchedule(result);
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.PolicyUrl, policy, response);
            else
                LogTestIntegrationTransaction($"Policy - {providerInfo.PolicyUrl}", policy, response);
            return result;
        }

        /// <summary>
        /// Companies that have policy schedule should implement it to get the policy file
        /// </summary>
        /// <param name="policyResponse">Policy response</param>
        /// <returns></returns>
        protected virtual PolicyResponse HandlePoilcySchedule(PolicyResponse policyResponse)
        {
            return policyResponse;
        }

        public virtual FileServiceOutput PolicySchedule(string policyNo, string referenceId)
        {
            var success = new FileServiceOutput { ErrorCode = FileServiceOutput.ErrorCodes.Success };
            return success;
        }

        protected abstract object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo);
        protected virtual ServiceOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }


        protected virtual PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            // Log the integration with insurance provider.
            var stringPayload = JsonConvert.SerializeObject(request);

            var policyResponse = JsonConvert.DeserializeObject<PolicyResponse>(result);

            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider get policy with reference id : ${request.ReferenceId}",
                stringPayload,
                result,
                policyResponse == null ? 2 : policyResponse.StatusCode);
            return policyResponse;
        }


        /// <summary>
        /// Generate quotations from 3rd-party service provider.
        /// </summary>
        /// <param name="quotation">The quotation request details required for quoting.</param>
        /// <returns>Quotation response message.</returns>
        public virtual QuotationServiceResponse GetQuotation(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedQuotation = HandleQuotationRequestObjectMapping(quotation);
            var response = ExecuteQuotationRequest(modifiedQuotation, predefinedLogInfo);
            // get provider info
            QuotationServiceResponse result = GetQuotationResponseObject(response, modifiedQuotation);
            var providerInfo = GetProviderInfo();
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.QuotationUrl, modifiedQuotation, response);
            else
                LogTestIntegrationTransaction($"Quotation - {providerInfo.QuotationUrl}", modifiedQuotation, response);
            HandleBenefits(result);
            return result;
        }


        /// <summary>
        /// Desarilize the insurance provider response to QuotationServiceResponse
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {

            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            // Log the integration with insurance provider.
            var stringPayload = JsonConvert.SerializeObject(request);

            var quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
            if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
            {
                quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }

            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider get quote with reference id : ${request.ReferenceId}",
                stringPayload,
                result,
                quotationServiceResponse == null ? 2 : quotationServiceResponse.StatusCode);
            return quotationServiceResponse;
        }


        /// <summary>
        /// Log integration transaction.
        /// </summary>
        /// <param name="transactionMethod">The method of transaction.</param>
        /// <param name="request">The request as input parameter.</param>
        /// <param name="response">The response as output parameter.</param>
        /// <param name="statusCode"></param>
        protected void LogIntegrationTransaction(string transactionMethod, object request, object response, int? statusCode = 0)
        {
            _logger.LogIntegrationTransaction(transactionMethod, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), statusCode);
        }

        protected void LogTestIntegrationTransaction(string transactionMethod, object request, object response, int? statusCode = 0)
        {
            try
            {
                var toBeDeleted = this._automatedTestIntegrationTransactionRepository.TableNoTracking
                                      .Where(x => x.Retrieved == true && string.Equals(x.Message, transactionMethod))
                                      .ToList();
                this._automatedTestIntegrationTransactionRepository.Delete(toBeDeleted);

                this._automatedTestIntegrationTransactionRepository
                    .Insert(new AutomatedTestIntegrationTransaction
                    {
                        Message = transactionMethod,
                        Date = DateTime.Now,
                        InputParams = JsonConvert.SerializeObject(request),
                        OutputParams = JsonConvert.SerializeObject(response),
                        Retrieved = false,
                        StatusId = statusCode.Value
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //_logger.LogIntegrationTransaction(transactionMethod, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), statusCode);
        }

        /// <summary>
        /// Log integration transaction.
        /// </summary>
        /// <param name="transactionMethod">The method of transaction.</param>
        /// <param name="request">The request as input parameter.</param>
        /// <param name="response">The response as output parameter.</param>
        /// <param name="statusCode"></param>
        protected void LogIntegrationTransaction(string transactionMethod, string request, string response, int? statusCode = 0)
        {
            _logger.LogIntegrationTransaction(transactionMethod, request, response, statusCode);
        }

        protected abstract ProviderInfoDto GetProviderInfo();

        protected abstract object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo);

        protected virtual ServiceOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }

        protected virtual void HandleBenefits(QuotationServiceResponse response)
        {
            if (response != null && response.Products != null)
            {
                foreach (var product in response.Products)
                {
                    if (product.Benefits != null)
                    {
                        foreach (var benefit in product.Benefits)
                        {
                            switch (benefit.BenefitCode)
                            {
                                case 0:
                                default:
                                    continue;

                                case 1:
                                    benefit.BenefitNameEn = "Personal Accident coverage for the driver only";
                                    benefit.BenefitNameAr = "تغطية الحوادث الشخصية للسائق فقط";
                                    break;

                                case 2:
                                    benefit.BenefitNameEn = "Personal Accident coverage for the driver & passenger";
                                    benefit.BenefitNameAr = "تغطية الحوادث الشخصية للسائق والركاب";
                                    break;

                                case 3:
                                    benefit.BenefitNameEn = "Natural Disasters";
                                    benefit.BenefitNameAr = "الاخطار الطبيعية";
                                    break;

                                case 4:
                                    benefit.BenefitNameEn = "Windscreen, fires & theft";
                                    benefit.BenefitNameAr = "الزجاج الأمامي والحرائق والسرقة";
                                    break;

                                case 5:
                                    benefit.BenefitNameEn = "Roadside Assistance";
                                    benefit.BenefitNameAr = "المساعدة على الطريق";
                                    break;

                                case 6:
                                    benefit.BenefitNameEn = "Hire Car";
                                    benefit.BenefitNameAr = "سيارة بديلة";
                                    break;

                                case 7:
                                    benefit.BenefitNameEn = "Agency Repairs";
                                    benefit.BenefitNameAr = "أصلاح وكالة";
                                    break;

                                case 8:
                                    benefit.BenefitNameEn = "Personal Accident coverage for the passenger only";
                                    benefit.BenefitNameAr = "تغطية الحوادث الشخصية للركاب فقط";
                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void HandleFinalProductPrice(QuotationServiceResponse responseValue)
        {
            if (responseValue.Products != null)
            {
                foreach (ProductDto product in responseValue.Products)
                {
                    if (product.ProductPrice <= 0)
                    {
                        decimal finalPrice = 0;
                        if (product.PriceDetails != null)
                        {
                            foreach (PriceDto price in product.PriceDetails)
                            {
                                finalPrice += price.PriceTypeCode <= 3 ? price.PriceValue * -1 : price.PriceValue;
                            }
                        }

                        product.ProductPrice = finalPrice;
                    }
                }
            }
        }


        protected virtual QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            return quotation;
        }

        protected virtual PolicyRequest HandlePolicyRequestObjectMapping(PolicyRequest policy)
        {
            return policy;
        }

        public virtual bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }

        #endregion

        #region Properties
        protected ProviderConfiguration Configuration
        {
            get { return _configuration; }
        }

        #endregion

        public virtual ComprehensiveImagesOutput UploadComprehansiveImages(ComprehensiveImagesRequest request, ServiceRequestLog log)
        {
            var success = new ComprehensiveImagesOutput { ErrorCode = ComprehensiveImagesOutput.ErrorCodes.Success };
            return success;
        }
        public virtual ServiceOutput GetQuotationServiceResponse(QuotationServiceRequest quotation, Product product, string proposalNumber, ServiceRequestLog predefinedLogInfo, List<string> selectedbenefits)        {            var modifiedQuotation = HandleQuotationRequestObjectMapping(quotation);            var response = GetTawuniyaQuotation(modifiedQuotation, product, proposalNumber, predefinedLogInfo, selectedbenefits);
            if (response.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return response;
            }
            QuotationServiceResponse result = GetTawuniyaQuotationResponseObject(response.Output, modifiedQuotation);
            // get provider info
            var providerInfo = GetProviderInfo();
            HandleBenefits(result);
            response.QuotationServiceResponse = result;
            return response;
        }
        protected virtual QuotationServiceResponse GetTawuniyaQuotationResponseObject(object response, QuotationServiceRequest request)        {            return null;        }
        public virtual ServiceOutput GetTawuniyaQuotation(QuotationServiceRequest quotationServiceRequest, Product product, string proposalNumber, ServiceRequestLog log, List<string> selectedbenefits)        {            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;        }

        #region Cancel Policy
        //protected abstract object ExecuteCancelPolicyRequest(CancelPolicyRequestDto policy, ServiceRequestLog predefinedLogInfo);
        public virtual CancelPolicyOutput SubmitCancelPolicyRequest(CancelPolicyRequestDto policy, ServiceRequestLog predefinedLogInfo)
        {
            object response = null;// ExecuteCancelPolicyRequest(policy, predefinedLogInfo);
            CancelPolicyOutput result = GetCancelPolicyObject(response, policy);
            return result;
        }

        //public virtual CancelPolicyOutput SendCancelPolicyRequest(CancelPolicyRequestDto policy, ServiceRequestLog predefinedLogInfo)
        //{
        //    object response = null;// ExecuteCancelPolicyRequest(policy, predefinedLogInfo);
        //    CancelPolicyOutput result = GetCancelPolicyObject(response, policy);
        //    return result;
        //}

        private CancelPolicyOutput GetCancelPolicyObject(object response, CancelPolicyRequestDto claim)
        {
            CancelPolicyOutput output = new CancelPolicyOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _cancelPolicyServiceResponse = JsonConvert.DeserializeObject<CancelPolicyResponse>(result);
            output.ErrorCode = CancelPolicyOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.CancelPolicyResponse = _cancelPolicyServiceResponse;
            return output;
        }
        public ClaimRegistrationServiceOutput SendClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            //var response = ExecuteClaimRegistrationRequest(claim, predefinedLogInfo);
            object response = null;
            ClaimRegistrationServiceOutput result = GetClaimRegistrtionResponseObject(response, claim);
            return result;
        }
        private ClaimRegistrationServiceOutput GetClaimRegistrtionResponseObject(object response, ClaimRegistrationRequest claim)
        {
            ClaimRegistrationServiceOutput output = new ClaimRegistrationServiceOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _claimRegistrationServiceResponse = JsonConvert.DeserializeObject<ClaimRegistrationServiceResponse>(result);
            output.ErrorCode = ClaimRegistrationServiceOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.ClaimRegistrationServiceResponse = _claimRegistrationServiceResponse;
            return output;
        }

        public ClaimNotificationServiceOutput SendClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            object response = null;// ExecuteClaimNotificationRequest(claim, predefinedLogInfo);
            ClaimNotificationServiceOutput result = GetClaimNotificationResponseObject(response, claim);
            return result;
        }
        private ClaimNotificationServiceOutput GetClaimNotificationResponseObject(object response, ClaimNotificationRequest claim)
        {
            ClaimNotificationServiceOutput output = new ClaimNotificationServiceOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _claimRegistrationServiceResponse = JsonConvert.DeserializeObject<ClaimNotificationServiceResponse>(result);
            output.ErrorCode = ClaimNotificationServiceOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.ClaimNotificationServiceResponse = _claimRegistrationServiceResponse;
            return output;
        }

        #endregion

        protected virtual QuotationServiceRequest HandleAutoleasingQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            return quotation;
        }

        protected abstract object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo);
        protected virtual QuotationServiceResponse GetAutoleasingQuotationResponseObject(object response, QuotationServiceRequest request)
        {

            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            // Log the integration with insurance provider.
            QuotationServiceResponse quotationServiceResponse = null;
            try
            {
                quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
            }
            catch
            {
                quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(response.ToString());
            }
            if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
            {
                quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            return quotationServiceResponse;
        }

        public virtual PolicyResponse GetAutoleasingPolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedPolicy = HandleAutoleasingPolicyRequestObjectMapping(policy);
            var response = ExecuteAutoleasingPolicyRequest(policy, predefinedLogInfo);
            // get provider info
            var providerInfo = GetProviderInfo();
            PolicyResponse result = GetAutoleasingPolicyResponseObject(response, policy);
            return result;
        }

        protected virtual PolicyResponse GetAutoleasingPolicyResponseObject(object response, PolicyRequest request = null)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            // Log the integration with insurance provider.
            var stringPayload = JsonConvert.SerializeObject(request);

            var policyResponse = JsonConvert.DeserializeObject<PolicyResponse>(result);

            return policyResponse;
        }

        protected abstract object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo);
        protected virtual ServiceOutput SubmitAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }
        protected virtual PolicyRequest HandleAutoleasingPolicyRequestObjectMapping(PolicyRequest policy)
        {
            return policy;
        }

        public virtual ServiceOutput GetTawuniyaAutoleasingQuotation(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)        {            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }

        public virtual QuotationServiceResponse GetQuotationAutoleasing(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            var modifiedQuotation = HandleAutoleasingQuotationRequestObjectMapping(quotation);
            var response = ExecuteAutoleasingQuotationRequest(modifiedQuotation, predefinedLogInfo);
            // get provider info
            QuotationServiceResponse result = GetAutoleasingQuotationResponseObject(response, modifiedQuotation);
            var providerInfo = GetProviderInfo();

            HandleBenefits(result);
            return result;
        }

        protected virtual ServiceOutput SubmitAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }

        public virtual ClaimRegistrationServiceOutput SubmitClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ClaimRegistrationServiceOutput { ErrorCode = ClaimRegistrationServiceOutput.ErrorCodes.Success };
            return success;
        }

        public virtual ClaimNotificationServiceOutput SubmitClaimNotificationRequest(ClaimNotificationRequest claimNotification, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ClaimNotificationServiceOutput { ErrorCode = ClaimNotificationServiceOutput.ErrorCodes.Success };
            return success;
        }

        public virtual ServiceOutput GetWataniyaAutoleasingDraftpolicy(QuotationServiceRequest quotation, Product selectedProduct, ServiceRequestLog predefinedLogInfo)        {            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }

        public virtual ServiceOutput GetWataniyaMotorDraftpolicy(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)        {            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        #region Update Custom Card
        public virtual CustomCardServiceOutput UpdateCustomCard(UpdateCustomCardRequest request, ServiceRequestLog log)
        {
            var modifiedrequest = HandleUpdateCustomCardRequestObjectMapping(request);
            var response = ExecuteUpdateCustomCard(modifiedrequest, log);
            if (response.ErrorCode == CustomCardServiceOutput.ErrorCodes.Success)
            {
                response.UpdateCustomCardResponse = GetUpdateCustomCardResponseObject(response.Output, modifiedrequest);
            }
            return response;
        }
        protected virtual UpdateCustomCardRequest HandleUpdateCustomCardRequestObjectMapping(UpdateCustomCardRequest request)
        {
            return request;
        }
        protected virtual UpdateCustomCardResponse GetUpdateCustomCardResponseObject(object response, UpdateCustomCardRequest request)
        {

            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var customCardResponse = JsonConvert.DeserializeObject<UpdateCustomCardResponse>(result);


            return customCardResponse;
        }
        protected virtual CustomCardServiceOutput ExecuteUpdateCustomCard(UpdateCustomCardRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new CustomCardServiceOutput { ErrorCode = CustomCardServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual CustomCardServiceOutput SubmitUpdateCustomCardRequest(UpdateCustomCardRequest quotation, ServiceRequestLog log)
        {
            return null;
        }

        #endregion

        #region Autolease Update Custom Card
        public virtual CustomCardServiceOutput AutoleaseUpdateCustomCard(UpdateCustomCardRequest request, ServiceRequestLog log)
        {
            var modifiedrequest = AutoleaseHandleUpdateCustomCardRequestObjectMapping(request);
            var response = AutoleaseExecuteUpdateCustomCard(modifiedrequest, log);
            if (response.ErrorCode == CustomCardServiceOutput.ErrorCodes.Success)
            {
                response.UpdateCustomCardResponse = AutoleaseGetUpdateCustomCardResponseObject(response.Output, modifiedrequest);
            }
            return response;
        }
        protected virtual UpdateCustomCardRequest AutoleaseHandleUpdateCustomCardRequestObjectMapping(UpdateCustomCardRequest request)
        {
            return request;
        }
        protected virtual UpdateCustomCardResponse AutoleaseGetUpdateCustomCardResponseObject(object response, UpdateCustomCardRequest request)
        {

            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var customCardResponse = JsonConvert.DeserializeObject<UpdateCustomCardResponse>(result);


            return customCardResponse;
        }
        protected virtual CustomCardServiceOutput AutoleaseExecuteUpdateCustomCard(UpdateCustomCardRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new CustomCardServiceOutput { ErrorCode = CustomCardServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual CustomCardServiceOutput AutoleaseSubmitUpdateCustomCardRequest(UpdateCustomCardRequest quotation, ServiceRequestLog log)
        {
            return null;
        }

        #endregion

        #region Add Driver
        public virtual AddDriverResponse AddDriver(AddDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandleAddDriverRequestObjectMapping(request);
            var response = ExecuteAddDriverRequest(modifiedrequest, predefinedLogInfo);
            AddDriverResponse result = GetAddDriverResponseObject(response, modifiedrequest);
            var providerInfo = GetProviderInfo();
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.QuotationUrl, modifiedrequest, response);
            else
                LogTestIntegrationTransaction($"Quotation - {providerInfo.QuotationUrl}", modifiedrequest, response);
            return result;
        }
        protected virtual AddDriverRequest HandleAddDriverRequestObjectMapping(AddDriverRequest request)
        {
            return request;
        }
        protected virtual AddDriverResponse GetAddDriverResponseObject(object response, AddDriverRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var stringPayload = JsonConvert.SerializeObject(request);
            var Response = JsonConvert.DeserializeObject<AddDriverResponse>(result);
            //if (Response != null && Response.Errors == null)
            //{
            //    Response.Errors = new List<Error>
            //    {
            //        new Error { Message = result }
            //    };
            //}
            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider add driver with nin id : ${request.DriverId}",
                stringPayload, result, Response == null ? 2 : Response.StatusCode);
            return Response;
        }
        protected virtual object ExecuteAddDriverRequest(AddDriverRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitAddDriverRequest(AddDriverRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }
        #endregion

        #region Purchase Driver
        public virtual PurchaseDriverResponse PurchaseDriver(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandlePurchaseDriverRequestObjectMapping(request);
            var response = ExecutePurchaseDriverRequest(modifiedrequest, predefinedLogInfo);
            var result = GetPurchaseDriverResponseObject(response, modifiedrequest);
            var providerInfo = GetProviderInfo();
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.QuotationUrl, modifiedrequest, response);
            else
                LogTestIntegrationTransaction($"Quotation - {providerInfo.QuotationUrl}", modifiedrequest, response);
            return result;
        }
        protected virtual PurchaseDriverRequest HandlePurchaseDriverRequestObjectMapping(PurchaseDriverRequest request)
        {
            return request;
        }
        protected virtual PurchaseDriverResponse GetPurchaseDriverResponseObject(object response, PurchaseDriverRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var stringPayload = JsonConvert.SerializeObject(request);
            var Response = JsonConvert.DeserializeObject<PurchaseDriverResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider Purchase  driver with reference id : ${request.ReferenceId}",
                stringPayload, result, Response == null ? 2 : Response.StatusCode);
            return Response;
        }
        protected virtual object ExecutePurchaseDriverRequest(PurchaseDriverRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitPurchaseDriverRequest(PurchaseDriverRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }

        #endregion

        #region Add Benifit
        public virtual AddBenefitResponse AutoleasingAddBenefit(AddBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandleAutoleasingAddBenifitRequest(request);
            var response = ExecuteAutoleasingAddBenifitRequest(modifiedrequest, predefinedLogInfo);
            AddBenefitResponse result =HandleAutoleasingBenifitResponse(response, modifiedrequest);
            return result;
        }
        protected virtual AddBenefitRequest HandleAutoleasingAddBenifitRequest(AddBenefitRequest request)
        {
            return request;
        }
        protected virtual AddBenefitResponse HandleAutoleasingBenifitResponse(object response, AddBenefitRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var Response = JsonConvert.DeserializeObject<AddBenefitResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            return Response;
        }
        protected virtual object ExecuteAutoleasingAddBenifitRequest(AddBenefitRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitAutoleasingAddBenifitRequest(AddBenefitRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }
        #endregion

        #region Purchase Benifit
        public virtual PurchaseBenefitResponse AutoleasingPurchaseBenefit(PurchaseBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandleAutoleasingPurchaseBenifitRequest(request);
            var response = ExecuteAutoleasingPurchaseBenifitRequest(modifiedrequest, predefinedLogInfo);
            var result = HandleAutoleasingPurchaseBenifitResponse(response, modifiedrequest);
            return result;
        }
        protected virtual PurchaseBenefitRequest HandleAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest request)
        {
            return request;
        }
        protected virtual PurchaseBenefitResponse HandleAutoleasingPurchaseBenifitResponse(object response, PurchaseBenefitRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var Response = JsonConvert.DeserializeObject<PurchaseBenefitResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            return Response;
        }
        protected virtual object ExecuteAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }



        #endregion

        #region Add Vechile Driver
        public virtual AddDriverResponse AddVechileDriver(AddDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandleAddVechileDriverRequestObjectMapping(request);
            var response = ExecuteAddVechileDriverRequest(modifiedrequest, predefinedLogInfo);
            AddDriverResponse result = GetAddVechileDriverResponseObject(response, modifiedrequest);
            var providerInfo = GetProviderInfo();
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.QuotationUrl, modifiedrequest, response);
            else
                LogTestIntegrationTransaction($"Quotation - {providerInfo.QuotationUrl}", modifiedrequest, response);
            return result;
        }
        protected virtual AddDriverRequest HandleAddVechileDriverRequestObjectMapping(AddDriverRequest request)
        {
            return request;
        }
        protected virtual AddDriverResponse GetAddVechileDriverResponseObject(object response, AddDriverRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var stringPayload = JsonConvert.SerializeObject(request);
            var Response = JsonConvert.DeserializeObject<AddDriverResponse>(result);
            //if (Response != null && Response.Errors != null && Response.Errors.Any())
            //{
            //    Response.Errors = new List<Error>
            //    {
            //        new Error { Message = result }
            //    };
            //}
            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider add driver with nin id : ${request.DriverId}",
                stringPayload, result, Response == null ? 2 : Response.StatusCode);
            return Response;
        }
        protected virtual object ExecuteAddVechileDriverRequest(AddDriverRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };
            return success;
        }
        protected virtual ServiceOutput SubmitAddVechileDriverRequest(AddDriverRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }
        #endregion

        #region Purchase Vechile Driver
        public virtual PurchaseDriverResponse PurchaseVechileDriver(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandlePurchaseVechileDriverRequestObjectMapping(request);
            var response = ExecutePurchaseVechileDriverRequest(modifiedrequest, predefinedLogInfo);
            var result = GetPurchaseDriverVechileResponseObject(response, modifiedrequest);
            var providerInfo = GetProviderInfo();
            if (!automatedTest)
                LogIntegrationTransaction(providerInfo.QuotationUrl, modifiedrequest, response);
            else
                LogTestIntegrationTransaction($"Quotation - {providerInfo.QuotationUrl}", modifiedrequest, response);
            return result;
        }
        protected virtual PurchaseDriverRequest HandlePurchaseVechileDriverRequestObjectMapping(PurchaseDriverRequest request)
        {
            return request;
        }
        protected virtual PurchaseDriverResponse GetPurchaseDriverVechileResponseObject(object response, PurchaseDriverRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var stringPayload = JsonConvert.SerializeObject(request);
            var Response = JsonConvert.DeserializeObject<PurchaseDriverResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            LogIntegrationTransaction($"{Configuration.ProviderName} insurance provider Purchase  driver with reference id : ${request.ReferenceId}",
                stringPayload, result, Response == null ? 2 : Response.StatusCode);
            return Response;
        }
        protected virtual object ExecutePurchaseVechileDriverRequest(PurchaseDriverRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitPurchaseVechileDriverRequest(PurchaseDriverRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }

        #endregion

        #region Add Vechile Benifit
        public virtual AddBenefitResponse AddVechileBenefit(AddVechileBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandleAddVechileBenifitRequestObjectMapping(request);
            var response = ExecuteAddVechileBenifitRequest(modifiedrequest, predefinedLogInfo);
            AddBenefitResponse result = GetAddVechileBenifitResponseObject(response, modifiedrequest);
            return result;
        }
        protected virtual AddVechileBenefitRequest HandleAddVechileBenifitRequestObjectMapping(AddVechileBenefitRequest request)
        {
            return request;
        }
        protected virtual AddBenefitResponse GetAddVechileBenifitResponseObject(object response, AddVechileBenefitRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var Response = JsonConvert.DeserializeObject<AddBenefitResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            return Response;
        }
        protected virtual object ExecuteAddVechileBenifitRequest(AddVechileBenefitRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitAddVechileBenifitRequest(AddVechileBenefitRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }
        #endregion

        #region Purchase Vechile Benefit
        public virtual PurchaseBenefitResponse PurchaseVechileBenefit(PurchaseBenefitRequest request, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            var modifiedrequest = HandlePurchaseVechileBenifitRequestObjectMapping(request);
            var response = ExecutePurchaseVechileBenifitRequest(modifiedrequest, predefinedLogInfo);
            var result = GetPurchaseVechileBenifitResponseObject(response, modifiedrequest);
            return result;
        }
        protected virtual PurchaseBenefitRequest HandlePurchaseVechileBenifitRequestObjectMapping(PurchaseBenefitRequest request)
        {
            return request;
        }
        protected virtual PurchaseBenefitResponse GetPurchaseVechileBenifitResponseObject(object response, PurchaseBenefitRequest request)
        {
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);
            var Response = JsonConvert.DeserializeObject<PurchaseBenefitResponse>(result);
            if (Response != null && Response.Errors == null)
            {
                Response.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }
            return Response;
        }
        protected virtual object ExecutePurchaseVechileBenifitRequest(PurchaseBenefitRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ServiceOutput { ErrorCode = ServiceOutput.ErrorCodes.Success };            return success;
        }
        protected virtual ServiceOutput SubmitPurchaseVechileBenifitRequest(PurchaseBenefitRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            return null;
        }

        #endregion

        #region VehicleClaims
        public ClaimRegistrationServiceOutput SendVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            var response = ExecuteVehicleClaimRegistrationRequest(claim, predefinedLogInfo);
            ClaimRegistrationServiceOutput result = GetVehicleClaimRegistrtionResponseObject(response, claim);
            return result;
        }
        protected abstract object ExecuteVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo);

        public virtual ClaimRegistrationServiceOutput GetVehicleClaimRegistrtionResponseObject(object response, ClaimRegistrationRequest claim)
        {
            ClaimRegistrationServiceOutput output = new ClaimRegistrationServiceOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _claimRegistrationServiceResponse = JsonConvert.DeserializeObject<ClaimRegistrationServiceResponse>(result);
            if (_claimRegistrationServiceResponse != null && _claimRegistrationServiceResponse.StatusCode == 1)
            {
                output.ErrorCode = ClaimRegistrationServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
            }
            else
            {
                output.ErrorCode = ClaimRegistrationServiceOutput.ErrorCodes.ServiceError;
            }
            output.ClaimRegistrationServiceResponse = _claimRegistrationServiceResponse;
            return output;
        }

        public virtual ClaimRegistrationServiceOutput SubmitVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ClaimRegistrationServiceOutput { ErrorCode = ClaimRegistrationServiceOutput.ErrorCodes.Success };
            return success;
        }


        //Notification
        public ClaimNotificationServiceOutput SendVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            var response = ExecuteVehicleClaimNotificationRequest(claim, predefinedLogInfo);
            ClaimNotificationServiceOutput result = GetVehicleClaimNotificationResponseObject(response, claim);
            return result;
        }
        protected abstract object ExecuteVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo);

        public virtual ClaimNotificationServiceOutput GetVehicleClaimNotificationResponseObject(object response, ClaimNotificationRequest claim)
        {
            ClaimNotificationServiceOutput output = new ClaimNotificationServiceOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _claimNotificationServiceResponse = JsonConvert.DeserializeObject<ClaimNotificationServiceResponse>(result);
            if (_claimNotificationServiceResponse != null && _claimNotificationServiceResponse.StatusCode == 1)
            {
                output.ErrorCode = ClaimNotificationServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
            }
            else
            {
                output.ErrorCode = ClaimNotificationServiceOutput.ErrorCodes.ServiceError;
            }
            output.ClaimNotificationServiceResponse = _claimNotificationServiceResponse;
            return output;
        }

        public virtual ClaimNotificationServiceOutput SubmitVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            var success = new ClaimNotificationServiceOutput { ErrorCode = ClaimNotificationServiceOutput.ErrorCodes.Success };
            return success;
        }
        #endregion


        #region Vehicle Cancel Policy
        public virtual CancelPolicyOutput SubmitAutoleasingCancelPolicyRequest(CancelPolicyRequestDto policy, int bankId, ServiceRequestLog predefinedLogInfo)
        {
            object response = null;// ExecuteCancelPolicyRequest(policy, predefinedLogInfo);
            CancelPolicyOutput result = GetCancelPolicyObject(response, policy);
            return result;
        }
        public virtual CancelPolicyOutput SubmitVehicleCancelPolicyRequest(CancelVechilePolicyRequestDto policy, ServiceRequestLog predefinedLogInfo)
        {
            object response = null;// ExecuteCancelPolicyRequest(policy, predefinedLogInfo);
            CancelPolicyOutput result = GetVechileCancelPolicyObject(response, policy);
            return result;
        }

        private CancelPolicyOutput GetVechileCancelPolicyObject(object response, CancelVechilePolicyRequestDto claim)
        {
            CancelPolicyOutput output = new CancelPolicyOutput();
            var result = string.Empty;
            if (response is HttpResponseMessage)
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            else
                result = new JavaScriptSerializer().Serialize(response);

            var _cancelPolicyServiceResponse = JsonConvert.DeserializeObject<CancelPolicyResponse>(result);
            output.ErrorCode = CancelPolicyOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.CancelPolicyResponse = _cancelPolicyServiceResponse;
            return output;
        }
        #endregion

        #region Wataniya Najm Status

        public virtual ServiceOutput WataniyaNajmStatus(string policyNo, string referenceId, string customId, string sequenceNo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

