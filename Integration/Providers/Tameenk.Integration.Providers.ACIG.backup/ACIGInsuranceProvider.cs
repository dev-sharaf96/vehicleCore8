using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.ACIG
{
    public class ACIGInsuranceProvider : RestfulInsuranceProvider
    {
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IHttpClient _httpClient;

        private readonly string AutoleasingBenefitServiceAccessToken = "6460662E217C7A9F899208DD70A2C28ABDEA42F128666A9B78E6C0C064846493";

        public ACIGInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "https://eservices.acig.com.sa/BcareLive/Api/MotorService/Quote",
                 GeneratePolicyUrl = "https://eservices.acig.com.sa/BcareLive/Api/MotorService/TPLPolicy",
                 AccessToken = "AcigBcare:AcigBcareLive@2018",
                 GenerateAutoleasingQuotationUrl = "https://leasing.acig.com.sa/AcigRetail/api/quotation",
                 GenerateAutoleasingPolicyUrl = "https://leasing.acig.com.sa/AcigRetail/api/policy",
                 AutoleasingAccessToken = "Bcare@Acig_api",
                 AutoleaseUpdateCustomCardUrl = "https://leasing.acig.com.sa/AcigRetail/api/Endorsement/UpdateCustomCard",
                 AutoleasingCancelPolicyUrl = "https://leasing.acig.com.sa/AcigRetail/api/Endorsement/cancelpolicy",
                 AutoleasingAddBenifitUrl = "https://leasing.acig.com.sa/acigRetail/api/Endorsement/GetBenefitList",
                 AutoleasingPurchaseBenifitUrl = "https://leasing.acig.com.sa/acigRetail/api/Endorsement/purchasebenefit",
                 UpdateCustomCardUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/UpdateCustomCard",
                 AutoleasingAddDriverUrl = "",
                 AutoleasingPurchaseDriverUrl = "",
                 ProviderName = "ACIG"
             },  logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
        }
        public override bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            errors = new List<string>();
            var mainDriverAddress = quotationRequest.Driver.Addresses.FirstOrDefault();
            if (mainDriverAddress != null)
            {
                var nationalAddressCity = addressService.GetCityCenterById(mainDriverAddress.CityId);
                // var insuredCityCenter = addressService.GetCityCenterCenterByElmCode(quotationRequest.Insured.CityId.ToString());
                if (nationalAddressCity.IsActive && nationalAddressCity?.ELM_Code != quotationRequest.Insured.CityId.ToString())
                {
                    errors.Add(InsuranceProvidersResource.InsuredCityDoesNotMatchAddressCity);
                    long cityId = 0;
                    long.TryParse(nationalAddressCity.ELM_Code, out cityId);
                    quotationRequest.Insured.CityId = cityId;
                    quotationRequest.Insured.WorkCityId = cityId;
                    return false;
                }
            }
            return true;
        }
        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            string result = string.Empty;
            var stringPayload = result;
            var res = string.Empty;

            try
            {
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        if (product != null && product.Benefits != null)
                        {
                            foreach (var benefit in product.Benefits)
                            {
                                if (benefit.BenefitPrice == 0)
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                            }
                        }
                    }
                }

                HandleFinalProductPrice(responseValue);
            }
            catch (Exception ex)
            {
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();

                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            finally
            {
                LogIntegrationTransaction($"Test Get Quotation with reference id: {request.ReferenceId} for company: GGI Comprehensive", stringPayload, responseValue, responseValue?.StatusCode);
            }

            return responseValue;
        }


        #region Add Benefit Service

        protected override object ExecuteAutoleasingAddBenifitRequest(AddBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = SubmitAutoleasingAddBenifitRequest(request, log);
            return output.Output;
        }

        protected override ServiceOutput SubmitAutoleasingAddBenifitRequest(AddBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "autoleasing";
            log.ServiceURL = _restfulConfiguration.AutoleasingAddBenifitUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AddBenefit";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.PolicyNo = request.PolicyNo;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingAddBenifitUrl, request, AutoleasingBenefitServiceAccessToken, authorizationMethod: "Bearer");
                postTask.Wait();
                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var addBenifitResponse = JsonConvert.DeserializeObject<AddBenefitResponse>(response.Content.ReadAsStringAsync().Result);
                output.Output = response;
                if (addBenifitResponse != null && addBenifitResponse.Errors != null && addBenifitResponse.Errors.Any())
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();
                    foreach (var error in addBenifitResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "AddBenifit Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        protected override object ExecuteAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = SubmitAutoleasingPurchaseBenifitRequest(request, log);
            return output.Output;
        }
        protected override ServiceOutput SubmitAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "autoleasing";
            log.ServiceURL = _restfulConfiguration.AutoleasingPurchaseBenifitUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "PurchaseBenefit";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingPurchaseBenifitUrl, request, AutoleasingBenefitServiceAccessToken, authorizationMethod: "Bearer");
                postTask.Wait();
                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var addDriverResponse = JsonConvert.DeserializeObject<PurchaseBenefitResponse>(response.Content.ReadAsStringAsync().Result);
                if (addDriverResponse != null && addDriverResponse.Errors != null && addDriverResponse.Errors.Any())
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();
                    foreach (var error in addDriverResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "PurchaseBenefit Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        #endregion
    }
}
