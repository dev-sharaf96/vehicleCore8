using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities.Policies;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Loggin.DAL;
using Tameenk.Core;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Http;
using Tameenk.Core.Infrastructure;
using System.Net;

namespace Tameenk.Integration.Providers.Solidarity
{
    public class SolidarityInsuranceProvider : RestfulInsuranceProvider
    {

        #region Fields

        private readonly ILogger _logger;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly RestfulConfiguration _restfulConfiguration;
        private string _accessTokenBase64;
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        private readonly IHttpClient _httpClient;
        private const string QUOTATION_TPL_URL = "https://bcare.solidaritytakaful.com/BcareMotorApi/Quotation";
        private const string QUOTATION_COMPREHENSIVE_URL = "https://bcare.solidaritytakaful.com/BcareMotorApi/Quotation";
        #endregion

        public SolidarityInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger
            , IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IRepository<CheckoutDetail> checkoutDetail)
            : base(tameenkConfig, new RestfulConfiguration
            {
                GenerateQuotationUrl = "",
                GeneratePolicyUrl = "https://bcare.solidaritytakaful.com/BcareMotorApi/Policy",
                AccessToken = "URNM1JqWnBhVFJFUVc4OU9qRTNWekZITDBGT1oyMDVUR1ZPWm5oNmQwbGxVSGM5UFE9PQ==",
                ProviderName = "Solidarity"
            },  logger, policyProcessingQueueRepository)
        {
            _logger = logger;
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _accessTokenBase64 = _restfulConfiguration.AccessToken;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _checkoutDetail = checkoutDetail;
        }
        protected override ServiceOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)        {            ServiceOutput output = new ServiceOutput();            log.ReferenceId = quotation.ReferenceId;            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "Portal";            log.ServiceURL = _restfulConfiguration.GenerateQuotationUrl;            log.ServerIP = ServicesUtilities.GetServerIP();            log.Method = "Quotation";            log.CompanyName = _restfulConfiguration.ProviderName;            log.VehicleMaker = quotation?.VehicleMaker;            log.VehicleMakerCode = quotation?.VehicleMakerCode;            log.VehicleModel = quotation?.VehicleModel;            log.VehicleModelCode = quotation?.VehicleModelCode;            log.VehicleModelYear = quotation?.VehicleModelYear;            log.CompanyName = "Solidarity";            var stringPayload = string.Empty;
            DateTime dtBeforeCalling = DateTime.Now;            HttpResponseMessage response = new HttpResponseMessage();            try            {                log.ServiceRequest = JsonConvert.SerializeObject(quotation);                dtBeforeCalling = DateTime.Now;                if (quotation.ProductTypeCode == 1)                {                    _restfulConfiguration.GenerateQuotationUrl = QUOTATION_TPL_URL;                }                else                {                    _restfulConfiguration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;                }                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateQuotationUrl, quotation, _accessTokenBase64, authorizationMethod: "Bearer");                postTask.Wait();                response = postTask.Result;                DateTime dtAfterCalling = DateTime.Now;                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;                if (response == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (response.Content == null)                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))                {                    output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;                    output.ErrorDescription = "Service response content result return null";                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = log.ErrorCode.ToString();                    log.ServiceErrorDescription = log.ServiceErrorDescription;                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                var quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(response.Content.ReadAsStringAsync().Result);                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors != null)                {                    StringBuilder servcieErrors = new StringBuilder();                    StringBuilder servcieErrorsCodes = new StringBuilder();                    foreach (var error in quotationServiceResponse.Errors)                    {                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);                        servcieErrorsCodes.AppendLine(error.Code);                    }                    output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;                    output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = output.ErrorDescription;                    log.ServiceErrorCode = servcieErrorsCodes.ToString();                    log.ServiceErrorDescription = servcieErrors.ToString();                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                    return output;                }                output.Output = response;                output.ErrorCode = ServiceOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceErrorCode = log.ErrorCode.ToString();                log.ServiceErrorDescription = log.ServiceErrorDescription;                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }
            catch (TimeoutException ex)
            {
                throw ex;
            }            catch (Exception ex)            {                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;                output.ErrorDescription = ex.ToString();                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = output.ErrorDescription;                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);                return output;            }        }

        protected override ServiceOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {

            ServiceOutput output = new ServiceOutput();

            log.ReferenceId = policy.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))                log.Channel = "Portal";
            //log.UserName = "";
            log.ServiceURL = _restfulConfiguration.GeneratePolicyUrl;
            log.ServiceRequest = JsonConvert.SerializeObject(policy);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Policy";
            //log.CompanyID = insur;
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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;
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

                //return response;
            }
            catch (Exception ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
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
    }
}
