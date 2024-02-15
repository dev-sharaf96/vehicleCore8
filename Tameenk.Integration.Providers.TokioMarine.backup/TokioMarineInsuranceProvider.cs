using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;
using System.Linq;
using System.Net.Http;
using System.Dynamic;
using System;
using Tameenk.Services.Core.Http;
using Tameenk.Core;
using System.Text;
using System.Net.Http.Headers;

namespace Tameenk.Integration.Providers.TokioMarine
{
    public class TokioMarineInsuranceProvider : RestfulInsuranceProvider
    {
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _accessTokenBase64;
        private readonly string _autoleasingAccessToken;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetail;

        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        public TokioMarineInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IRepository<CheckoutDetail> checkoutDetail)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "https://bcare.atmc.com.sa:8080/api/Quotation",
                 GeneratePolicyUrl = "https://bcare.atmc.com.sa:8080/api/Policy",
                 AddBenifitUrl= "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/AddBenefit",
                 PurchaseBenifitUrl= "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PurchaseBenefit",
                 AddDriverUrl= "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/AddDriver",
                 PurchaseDriverUrl= "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PurchaseAddDriver",
                 AccessToken = "5717602d6dcfdca75693e73aa48750e6f5ed1dcd68b4ab397d41d2552cbb28e5",
                 GenerateAutoleasingQuotationUrl = "https://bcare.atmc.com.sa:8080/api/Quotation",
                 GenerateAutoleasingPolicyUrl = "https://bcare.atmc.com.sa:8080/api/Policy",
                 AutoleasingAccessToken = "5717602d6dcfdca75693e73aa48750e6f5ed1dcd68b4ab397d41d2552cbb28e5",
                 ProviderName = "TokioMarine",
                 AutoleaseUpdateCustomCardUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/UpdateCustomCard",
                 UpdateCustomCardUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/UpdateCustomCard",
                 CancelPolicyUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PolicyCancellation",
                 AutoleasingCancelPolicyUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PolicyCancellation",
                 AutoleasingAddBenifitUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/AddBenefit",
                 AutoleasingPurchaseBenifitUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PurchaseBenefit",
                 AutoleasingAddDriverUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/AddDriver",
                 AutoleasingPurchaseDriverUrl = "https://bcareendt.atmc.com.sa:8085/BCareEndorsement/BCareEndorsementWebAPI/PurchaseAddDriver",

             }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _logger = logger;
            _accessTokenBase64 = _restfulConfiguration.AccessToken;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _autoleasingAccessToken = _restfulConfiguration.AutoleasingAccessToken;

            _checkoutDetail = checkoutDetail;
        }
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }

        protected override ServiceOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = quotation.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.GenerateQuotationUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = _restfulConfiguration.ProviderName;

            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                if (quotation.ProductTypeCode != 2)
                    quotation.DeductibleValue = null;

                log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateQuotationUrl, quotation, _accessTokenBase64, authorizationMethod: "Basic");
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
                var quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(response.Content.ReadAsStringAsync().Result);
                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in quotationServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);

                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
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
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

                //return response;
            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                //log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;


            }
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
                                if (benefit.BenefitPrice == 0)// added as per waleed 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                            }
                        }
                    }
                }
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

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }


        protected override ServiceOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {

            ServiceOutput output = new ServiceOutput();

            log.ReferenceId = policy.ReferenceId;
            log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.GeneratePolicyUrl;
            log.ServiceRequest = JsonConvert.SerializeObject(policy);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Policy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
                request.ServiceRequest = log.ServiceRequest;
            }
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";

                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                stringPayload = JsonConvert.SerializeObject(policy);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                var postTask = client.PostAsync(_restfulConfiguration.GeneratePolicyUrl, httpContent);
                postTask.Wait();

                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    //update policyProcessingQueue Table;
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }


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
                    if (request != null)
                    {
                        request.ErrorDescription = " service response content return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
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
                    if (request != null)
                    {
                        request.ErrorDescription = " Service response content result return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
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
                request.ServiceResponse = log.ServiceResponse;
                var policyServiceResponse = JsonConvert.DeserializeObject<PolicyResponse>(response.Content.ReadAsStringAsync().Result);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }

                    return output;
                }
                if (string.IsNullOrEmpty(policyServiceResponse.PolicyNo))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "No  PolicyNo returned from company";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }
                if (policyServiceResponse.PolicyNo.ToLower() == "null")
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "PolicyNo returned from company as null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }
                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = policyServiceResponse.PolicyNo;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }

                return output;


            }
        }


        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            if (quotation.VehicleEngineSizeCode == 0)
                quotation.VehicleEngineSizeCode = null;

            return quotation;
        }


        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            if (quotation.VehicleEngineSizeCode == 0)
                quotation.VehicleEngineSizeCode = null;

            var result = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);

            if (result.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return result.Output;
        }

        protected override ServiceOutput SubmitAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.Channel = "autoleasing";
            //log.UserName = "";
            log.ServiceURL = _restfulConfiguration.GenerateAutoleasingQuotationUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingQuotation";
            //log.CompanyID = insur;
            log.CompanyName = _restfulConfiguration.ProviderName;

            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;

            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                if (quotation.ProductTypeCode != 2)
                    quotation.DeductibleValue = null;

                log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateAutoleasingQuotationUrl, quotation, _autoleasingAccessToken);
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
                //if (response.StatusCode != System.Net.HttpStatusCode.OK)
                //{
                //    output.ErrorCode = ServiceOutput.ErrorCodes.HttpStatusCodeNotOk;
                //    output.ErrorDescription = "Http Status Code is Not Ok";
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    log.ServiceErrorCode = log.ErrorCode.ToString();
                //    log.ServiceErrorDescription = log.ServiceErrorDescription;
                //    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return output;
                //}
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

                var quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(response.Content.ReadAsStringAsync().Result);
                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in quotationServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);

                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
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
                //log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
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
        protected override QuotationServiceResponse GetAutoleasingQuotationResponseObject(object response, QuotationServiceRequest request)
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
                                if (benefit.BenefitPrice == 0)// added as per waleed 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                            }
                        }
                    }
                }
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
                LogIntegrationTransaction($"Test Get Quotation with reference id: {request.ReferenceId} for company: TokioMarine Comprehensive", stringPayload, responseValue, responseValue?.StatusCode);
            }

            return responseValue;
        }


        protected override object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }
        protected override ServiceOutput SubmitAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {

            ServiceOutput output = new ServiceOutput();

            log.ReferenceId = policy.ReferenceId;
            log.Channel = "autoleasing";
            log.ServiceURL = _restfulConfiguration.GenerateAutoleasingPolicyUrl;
            log.ServiceRequest = JsonConvert.SerializeObject(policy);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingPolicy";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
                request.ServiceRequest = log.ServiceRequest;
            }
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";

                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                stringPayload = JsonConvert.SerializeObject(policy);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _autoleasingAccessToken);
                var postTask = client.PostAsync(_restfulConfiguration.GenerateAutoleasingPolicyUrl, httpContent);
                postTask.Wait();

                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    //update policyProcessingQueue Table;
                    if (request != null)
                    {
                        request.ErrorDescription = " service Return null";
                        _policyProcessingQueueRepository.Update(request);
                    }


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
                    if (request != null)
                    {
                        request.ErrorDescription = " service response content return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
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
                    if (request != null)
                    {
                        request.ErrorDescription = " Service response content result return null";
                        _policyProcessingQueueRepository.Update(request);
                    }
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
                request.ServiceResponse = log.ServiceResponse;
                var policyServiceResponse = JsonConvert.DeserializeObject<PolicyResponse>(response.Content.ReadAsStringAsync().Result);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }

                    return output;
                }
                if (string.IsNullOrEmpty(policyServiceResponse.PolicyNo))
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "No  PolicyNo returned from company";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }
                if (policyServiceResponse.PolicyNo.ToLower() == "null")
                {
                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "PolicyNo returned from company as null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    if (request != null)
                    {
                        request.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(request);
                    }
                    return output;
                }
                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = policyServiceResponse.PolicyNo;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                if (request != null)
                {
                    request.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(request);
                }

                return output;


            }
        }

        protected override CustomCardServiceOutput AutoleaseExecuteUpdateCustomCard(UpdateCustomCardRequest reqtest, ServiceRequestLog predefinedLogInfo)
        {
            return AutoleaseSubmitUpdateCustomCardRequest(reqtest, predefinedLogInfo);
        }
        protected override CustomCardServiceOutput AutoleaseSubmitUpdateCustomCardRequest(UpdateCustomCardRequest request, ServiceRequestLog log)
        {
            CustomCardServiceOutput output = new CustomCardServiceOutput();
            log.ReferenceId = request.ReferenceId;
            log.Channel = "autoleasing";
            //log.UserName = "";
            log.ServiceURL = _restfulConfiguration.GenerateAutoleasingQuotationUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "UpdateCustomCard";
            //log.CompanyID = insur;
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleaseUpdateCustomCardUrl, request, _autoleasingAccessToken);
                postTask.Wait();
                response = postTask.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = CustomCardServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = CustomCardServiceOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = CustomCardServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;

                var updateCustomCardServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(response.Content.ReadAsStringAsync().Result);
                if (updateCustomCardServiceResponse != null && updateCustomCardServiceResponse.Products == null && updateCustomCardServiceResponse.Errors != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in updateCustomCardServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);

                    }

                    output.ErrorCode = CustomCardServiceOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.Output = response;
                output.ErrorCode = CustomCardServiceOutput.ErrorCodes.Success;
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
                output.ErrorCode = CustomCardServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public override CancelPolicyOutput SubmitAutoleasingCancelPolicyRequest(CancelPolicyRequestDto policy, int bankId, ServiceRequestLog log)
        {
            CancelPolicyOutput output = new CancelPolicyOutput();
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "Autoleasing";
            //log.UserName = "";
            log.ServiceURL = _restfulConfiguration.AutoleasingCancelPolicyUrl;
            log.ServiceRequest = JsonConvert.SerializeObject(policy);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "CancelPolicy";
            //log.CompanyID = insur;
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.PolicyNo = policy.PolicyNo;
            log.ReferenceId = policy.ReferenceId;
            //log.ClaimsNo=policy.claimsNo;
            var stringPayload = string.Empty;
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                stringPayload = JsonConvert.SerializeObject(policy);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _autoleasingAccessToken);
                var postTask = client.PostAsync(_restfulConfiguration.AutoleasingCancelPolicyUrl, httpContent);
                postTask.Wait();
                response = postTask.Result;

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    output.ErrorCode = CancelPolicyOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = CancelPolicyOutput.ErrorCodes.NullResponse;
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
                    output.ErrorCode = CancelPolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var policyServiceResponse = JsonConvert.DeserializeObject<CancelPolicyResponse>(response.Content.ReadAsStringAsync().Result);

                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in policyServiceResponse.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = CancelPolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                //if (string.IsNullOrEmpty(policyServiceResponse.ReferenceId))
                //{
                //    output.ErrorCode = CancelPolicyOutput.ErrorCodes.InValidData;
                //    output.ErrorDescription = "Refrence Id return null";
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    log.ServiceErrorCode = log.ErrorCode.ToString();
                //    log.ServiceErrorDescription = log.ServiceErrorDescription;
                //    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return output;
                //}
                if (policyServiceResponse.RefundAmount == null)
                {
                    output.ErrorCode = CancelPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "Refund amout return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = policyServiceResponse;
                output.ErrorCode = CancelPolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.PolicyNo = policy.PolicyNo;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = CancelPolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;


            }
        }


        protected override object ExecuteAddVechileDriverRequest(AddDriverRequest request, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAddVechileDriverRequest(request, predefinedLogInfo);
            return output.Output;
        }

        protected override ServiceOutput SubmitAddVechileDriverRequest(AddDriverRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.AddDriverUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AddVehicleDriver";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.DriverNin = request.DriverId.ToString();
            log.PolicyNo = request.PolicyNo;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.addDriverTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.AddDriverUrl, request, _accessTokenBase64, authorizationMethod: "Bearer");
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
                var addDriverResponse = JsonConvert.DeserializeObject<AddDriverResponse>(response.Content.ReadAsStringAsync().Result);
                output.Output = response;
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
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

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


        protected override object ExecutePurchaseVechileDriverRequest(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitPurchaseVechileDriverRequest(request, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }
        protected override ServiceOutput SubmitPurchaseVechileDriverRequest(PurchaseDriverRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.PurchaseDriverUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "PurchaseVehicleDriver";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.purchaseDriverTestData.json";
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ServiceResponse = responseData;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.PurchaseDriverUrl, request, _accessTokenBase64, authorizationMethod: "Bearer");
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
                var addDriverResponse = JsonConvert.DeserializeObject<PurchaseDriverResponse>(response.Content.ReadAsStringAsync().Result);
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
                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
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


        protected override object ExecuteAddVechileBenifitRequest(AddVechileBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = SubmitAddVechileBenifitRequest(request, log);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
        }
        protected override ServiceOutput SubmitAddVechileBenifitRequest(AddVechileBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.AddBenifitUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AddVehicleBenefit";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.PolicyNo = request?.PolicyNo;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.AddBenifitUrl, request, _accessTokenBase64, authorizationMethod: "Bearer");
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
        protected override ServiceOutput SubmitPurchaseVechileBenifitRequest(PurchaseBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.PurchaseBenifitUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "PurchaseVehicleBenefit";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                //var testMode = _tameenkConfig.Quotatoin.TestMode;
                //if (testMode)
                //{
                //    const string nameOfFile = ".TestData.purchaseDriverTestData.json";
                //    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                //    HttpResponseMessage message = new HttpResponseMessage();
                //    message.Content = new StringContent(responseData);
                //    message.StatusCode = System.Net.HttpStatusCode.OK;
                //    output.Output = message;
                //    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                //    output.ErrorDescription = "Success";
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    log.ServiceErrorCode = log.ErrorCode.ToString();
                //    log.ServiceErrorDescription = log.ServiceErrorDescription;
                //    log.ServiceResponse = responseData;
                //    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return output;
                //}
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.PurchaseBenifitUrl, request, _accessTokenBase64, authorizationMethod: "Bearer");
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

        protected override object ExecuteAutoleasingAddBenifitRequest(AddBenefitRequest request, ServiceRequestLog log)        {            ServiceOutput output = SubmitAutoleasingAddBenifitRequest(request, log);
            return output.Output;        }        protected override ServiceOutput SubmitAutoleasingAddBenifitRequest(AddBenefitRequest request, ServiceRequestLog log)        {            ServiceOutput output = new ServiceOutput();            log.ReferenceId = request.ReferenceId;            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "autoleasing";            log.ServiceURL = _restfulConfiguration.AutoleasingAddBenifitUrl;            log.ServerIP = ServicesUtilities.GetServerIP();            log.Method = "AddBenefit";            log.CompanyName = _restfulConfiguration.ProviderName;            log.PolicyNo = request.PolicyNo;            var stringPayload = string.Empty;            HttpResponseMessage response = new HttpResponseMessage();            try            {                log.ServiceRequest = JsonConvert.SerializeObject(request);                DateTime dtBeforeCalling = DateTime.Now;                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingAddBenifitUrl, request, _autoleasingAccessToken, authorizationMethod: "Bearer");                postTask.Wait();                response = postTask.Result;                DateTime dtAfterCalling = DateTime.Now;                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;                if (response == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (response.Content == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content result return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                var addBenifitResponse = JsonConvert.DeserializeObject<AddBenefitResponse>(response.Content.ReadAsStringAsync().Result);                output.Output = response;                if (addBenifitResponse != null && addBenifitResponse.Errors != null && addBenifitResponse.Errors.Any())                {                    StringBuilder servcieErrors = new StringBuilder();                    StringBuilder servcieErrorsCodes = new StringBuilder();                    foreach (var error in addBenifitResponse.Errors)                    {                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);                        servcieErrorsCodes.AppendLine(error.Code);                    }                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;                    output.ErrorDescription = "AddBenifit Service response error is : " + servcieErrors.ToString();                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = servcieErrorsCodes.ToString();                    log.ServiceErrorDescription = servcieErrors.ToString();                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                output.Output = response;                output.ErrorCode = ServiceOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceErrorCode = log.ErrorCode.ToString();                log.ServiceErrorDescription = log.ServiceErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }            catch (Exception ex)            {                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;                output.ErrorDescription = ex.ToString();                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }        }        protected override object ExecuteAddDriverRequest(AddDriverRequest request, ServiceRequestLog predefinedLogInfo)        {            ServiceOutput output = SubmitAddDriverRequest(request, predefinedLogInfo);
            //if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            //{
            //    return null;
            //}

            return output.Output;        }        protected override ServiceOutput SubmitAddDriverRequest(AddDriverRequest request, ServiceRequestLog log)        {            ServiceOutput output = new ServiceOutput();            log.ReferenceId = request.ReferenceId;            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "Portal";            log.ServiceURL = _restfulConfiguration.AutoleasingAddDriverUrl;            log.ServerIP = ServicesUtilities.GetServerIP();            log.Method = "AddDriver";            log.CompanyName = _restfulConfiguration.ProviderName;            log.DriverNin = request.DriverId.ToString();            log.PolicyNo = request.PolicyNo;            var stringPayload = string.Empty;            HttpResponseMessage response = new HttpResponseMessage();            try            {                var testMode = _tameenkConfig.Quotatoin.TestMode;                if (testMode)                {                    const string nameOfFile = ".TestData.addDriverTestData.json";                    string responseData = ReadResource(GetType().Namespace, nameOfFile);                    HttpResponseMessage message = new HttpResponseMessage();                    message.Content = new StringContent(responseData);                    message.StatusCode = System.Net.HttpStatusCode.OK;                    output.Output = message;                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;                    output.ErrorDescription = "Success";                    return output;                }                log.ServiceRequest = JsonConvert.SerializeObject(request);                DateTime dtBeforeCalling = DateTime.Now;                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingAddDriverUrl, request, _autoleasingAccessToken, authorizationMethod: "Bearer");                postTask.Wait();                response = postTask.Result;                DateTime dtAfterCalling = DateTime.Now;                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;                if (response == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (response.Content == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content result return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                var addDriverResponse = JsonConvert.DeserializeObject<AddDriverResponse>(response.Content.ReadAsStringAsync().Result);                output.Output = response;                if (addDriverResponse != null && addDriverResponse.Errors != null && addDriverResponse.Errors.Any())                {                    StringBuilder servcieErrors = new StringBuilder();                    StringBuilder servcieErrorsCodes = new StringBuilder();                    foreach (var error in addDriverResponse.Errors)                    {                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);                        servcieErrorsCodes.AppendLine(error.Code);                    }                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = servcieErrorsCodes.ToString();                    log.ServiceErrorDescription = servcieErrors.ToString();                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                output.ErrorCode = ServiceOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceErrorCode = log.ErrorCode.ToString();                log.ServiceErrorDescription = log.ServiceErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }            catch (Exception ex)            {                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;                output.ErrorDescription = ex.ToString();                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }        }        protected override object ExecutePurchaseDriverRequest(PurchaseDriverRequest request, ServiceRequestLog predefinedLogInfo)        {            ServiceOutput output = SubmitPurchaseDriverRequest(request, predefinedLogInfo);            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)            {                return null;            }            return output.Output;        }        protected override ServiceOutput SubmitPurchaseDriverRequest(PurchaseDriverRequest request, ServiceRequestLog log)        {            ServiceOutput output = new ServiceOutput();            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "Portal";            log.ServiceURL = _restfulConfiguration.GenerateQuotationUrl;            log.ServerIP = ServicesUtilities.GetServerIP();            log.Method = "PurchaseDriver";            log.CompanyName = _restfulConfiguration.ProviderName;            var stringPayload = string.Empty;            HttpResponseMessage response = new HttpResponseMessage();            try            {                var testMode = _tameenkConfig.Quotatoin.TestMode;                if (testMode)                {                    const string nameOfFile = ".TestData.purchaseDriverTestData.json";                    string responseData = ReadResource(GetType().Namespace, nameOfFile);                    HttpResponseMessage message = new HttpResponseMessage();                    message.Content = new StringContent(responseData);                    message.StatusCode = System.Net.HttpStatusCode.OK;                    output.Output = message;                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;                    output.ErrorDescription = "Success";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    log.ServiceResponse = responseData;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceRequest = JsonConvert.SerializeObject(request);                DateTime dtBeforeCalling = DateTime.Now;                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingPurchaseDriverUrl, request, _autoleasingAccessToken, authorizationMethod: "Bearer");                postTask.Wait();                response = postTask.Result;                DateTime dtAfterCalling = DateTime.Now;                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;                if (response == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (response.Content == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content result return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                var addDriverResponse = JsonConvert.DeserializeObject<PurchaseDriverResponse>(response.Content.ReadAsStringAsync().Result);                if (addDriverResponse != null && addDriverResponse.Errors != null && addDriverResponse.Errors.Any())                {                    StringBuilder servcieErrors = new StringBuilder();                    StringBuilder servcieErrorsCodes = new StringBuilder();                    foreach (var error in addDriverResponse.Errors)                    {                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);                        servcieErrorsCodes.AppendLine(error.Code);                    }                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = servcieErrorsCodes.ToString();                    log.ServiceErrorDescription = servcieErrors.ToString();                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                output.Output = response;                output.ErrorCode = ServiceOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceErrorCode = log.ErrorCode.ToString();                log.ServiceErrorDescription = log.ServiceErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }            catch (Exception ex)            {                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;                output.ErrorDescription = ex.ToString();                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }        }
        protected override object ExecuteAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest request, ServiceRequestLog log)
        {
            ServiceOutput output = SubmitAutoleasingPurchaseBenifitRequest(request, log);
            return output.Output;
        }
        protected override ServiceOutput SubmitAutoleasingPurchaseBenifitRequest(PurchaseBenefitRequest request, ServiceRequestLog log)        {            ServiceOutput output = new ServiceOutput();            log.ReferenceId = request.ReferenceId;            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "autoleasing";            log.ServiceURL = _restfulConfiguration.GenerateQuotationUrl;            log.ServerIP = ServicesUtilities.GetServerIP();            log.Method = "PurchaseBenefit";            log.CompanyName = _restfulConfiguration.ProviderName;            var stringPayload = string.Empty;            HttpResponseMessage response = new HttpResponseMessage();            try            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);                DateTime dtBeforeCalling = DateTime.Now;                var postTask = _httpClient.PostAsync(_restfulConfiguration.AutoleasingPurchaseBenifitUrl, request, _autoleasingAccessToken, authorizationMethod: "Bearer");                postTask.Wait();                response = postTask.Result;                DateTime dtAfterCalling = DateTime.Now;                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;                if (response == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (response.Content == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content result return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                var addDriverResponse = JsonConvert.DeserializeObject<PurchaseBenefitResponse>(response.Content.ReadAsStringAsync().Result);                if (addDriverResponse != null && addDriverResponse.Errors != null && addDriverResponse.Errors.Any())                {                    StringBuilder servcieErrors = new StringBuilder();                    StringBuilder servcieErrorsCodes = new StringBuilder();                    foreach (var error in addDriverResponse.Errors)                    {                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);                        servcieErrorsCodes.AppendLine(error.Code);                    }                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;                    output.ErrorDescription = "PurchaseBenefit Service response error is : " + servcieErrors.ToString();                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = servcieErrorsCodes.ToString();                    log.ServiceErrorDescription = servcieErrors.ToString();                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                output.Output = response;                output.ErrorCode = ServiceOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceErrorCode = log.ErrorCode.ToString();                log.ServiceErrorDescription = log.ServiceErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }            catch (Exception ex)            {                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;                output.ErrorDescription = ex.ToString();                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }        }
    }
}
