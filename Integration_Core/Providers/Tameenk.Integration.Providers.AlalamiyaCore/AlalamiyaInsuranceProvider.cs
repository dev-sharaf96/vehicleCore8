using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Logging;

namespace Tameenk.Integration.Providers.Alalamiya
{
    public class AlalamiyaInsuranceProvider : RestfulInsuranceProvider
    {
        #region Fields
        private readonly TameenkConfig _tameenkConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private string _accessTokenBase64;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IAddressService _addressService;
        #endregion
        private const string QUOTATION_COMPREHENSIVE_URL = "https://tameenkcomp.alalamiya.sa/RSABPMWeb/rsaservices/CreateMotorQuote.do";
        private const string POLICY_COMPREHENSIVE_URL = "https://tameenkcomp.alalamiya.sa/RSABPMWeb/rsaservices/CreateMotorPolicy.do";
        private readonly IRepository<CheckoutDetail> _checkoutDetail;
        #region Ctor
        public AlalamiyaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository
            , IAddressService addressService, IRepository<CheckoutDetail> checkoutDetail)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "https://tameenk.alalamiya.sa/RSABPMWeb/rsaservices/CreateMotorQuote.do",
                 GeneratePolicyUrl = "https://tameenk.alalamiya.sa/RSABPMWeb/rsaservices/CreateMotorPolicy.do",
                 UpdateCustomCardUrl= "https://tameenk.alalamiya.sa/RSABPMWeb/rsaservices/CreateMotorNilEndtPolicy.do",
                 AccessToken = "VEFNRUVOS19VU0VSOlczQHIzYjM1dCE=",
                 ProviderName = "Alalamiya"
             }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _logger = logger;
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
               null : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));
            _checkoutDetail = checkoutDetail;
        }

        #endregion

        #region Methods

        protected override PolicyRequest HandlePolicyRequestObjectMapping(PolicyRequest policy)
        {
            if (!string.IsNullOrEmpty(policy.InsuredCity))
            {
                var address = _addressService.GetFromCityByArabicName(policy.InsuredCity);
                if (address != null)
                {
                    policy.InsuredCity = address.EnglishDescription;
                }
                else
                {
                    policy.InsuredCity = policy.InsuredCity;
                }
            }
            return policy;
        }

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            //according to our discussion with Alalamiya on mail, we agreed that sending this values
            if (quotation.InsuredOccupationCode == "G")
                quotation.InsuredOccupationCode = "169";

            else if (quotation.InsuredOccupationCode == "O")
                quotation.InsuredOccupationCode = "168";

            foreach (var driver in quotation.Drivers)
            {
                if (driver.DriverOccupationCode == "G")
                    driver.DriverOccupationCode = "169";

                else if (driver.DriverOccupationCode == "O")
                    driver.DriverOccupationCode = "168";
            }

            /* Name En in Our integartion guide v3.1 is Mandatory 
             * But some times , it send with null or empty value
             * So we assign dash to English name 
             * */
            if (string.IsNullOrEmpty(quotation.InsuredFirstNameEn))
                quotation.InsuredFirstNameEn = "-";

            if (string.IsNullOrEmpty(quotation.InsuredLastNameEn))
                quotation.InsuredLastNameEn = "-";

            if (quotation.Drivers != null && quotation.Drivers.Count() > 0)
            {
                foreach (var driver in quotation.Drivers)
                {

                    if (string.IsNullOrEmpty(driver.DriverFirstNameEn))
                        driver.DriverFirstNameEn = "-";
                    if (string.IsNullOrEmpty(driver.DriverLastNameEn))
                        driver.DriverLastNameEn = "-";
                }
            }


            return quotation;
        }

        //For Alalamiya we override execute quotation so that we add params in request headers
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            //in case test mode execute the code from the base.
            if (_tameenkConfig.Quotatoin.TestMode)
                return base.ExecuteQuotationRequest(quotation, predefinedLogInfo);
            if (quotation.ProductTypeCode == 2)
            { 
                _restfulConfiguration.GenerateQuotationUrl = QUOTATION_COMPREHENSIVE_URL;
            }
            ServiceOutput output = SubmitQuotationRequest(quotation, predefinedLogInfo);
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

                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "Location", "12" },
                    { "PartnerId", "Tameenk" }/*,
                    { "Accept", "application/json" },
                    { "Content-Type", "application/json" }*/
                };
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GeneratePolicyUrl, policy, "VEFNRUVOS19VU0VSOlczQHIzYjM1dCE=", authorizationMethod: "Basic", headers: headers);
                //var postTask = _httpClient.PostAsync(_restfulConfiguration.GeneratePolicyUrl, policy, _accessTokenBase64, authorizationMethod: "Basic", headers: headers);
                postTask.Wait();

                /* DateTime dtBeforeCalling = DateTime.Now;
                 var client = new HttpClient();
                 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                 var postTask = client.PostAsync(_restfulConfiguration.GeneratePolicyUrl, httpContent);
                 postTask.Wait();
                 */
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

                var policyServiceResponse = AlamiaDeserializePolicyJson(response.Content.ReadAsStringAsync().Result);
                //var policyServiceResponse = JsonConvert.DeserializeObject<PolicyResponse>(response.Content.ReadAsStringAsync().Result);
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
                    output.ErrorDescription = "No PolicyNo returned from company";
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
                output.ErrorDescription = ex.ToString();
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

        private static PolicyResponse AlamiaDeserializePolicyJson(string json)
        {
            try
            {
                AlamiaPolicyResponse deserializeResult = JsonConvert.DeserializeObject<AlamiaPolicyResponse>(json);
                if (deserializeResult == null)
                    return new PolicyResponse();

                PolicyResponse policyResponse = new PolicyResponse()
                {
                    ReferenceId = deserializeResult.ReferenceId,
                    StatusCode = deserializeResult.StatusCode,
                    PolicyNo = deserializeResult.PolicyNo,
                    PolicyIssuanceDate = (deserializeResult.PolicyIssuanceDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyIssuanceDate)
                                            : null,
                    PolicyEffectiveDate = (deserializeResult.PolicyEffectiveDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyEffectiveDate)
                                            : null,
                    PolicyExpiryDate = (deserializeResult.PolicyExpiryDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyExpiryDate)
                                            : null,
                    Errors = deserializeResult.Errors,
                    //PolicyFile = deserializeResult.PolicyFile[0]
                };

                var _stringFile = deserializeResult.PolicyFile[0];
                if (_stringFile.Contains("\n"))
                    _stringFile = _stringFile.Replace("\n", "");

                byte[] data = System.Convert.FromBase64String(_stringFile);
                var base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);

                byte[] bytes = System.Convert.FromBase64String(base64Decoded);

                policyResponse.PolicyFile = bytes;

                return policyResponse;
            }
            catch (Exception ex)
            {
                return new PolicyResponse();
            }
        }

        protected override PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        {
            PolicyResponse policyResponse = new PolicyResponse();

            AlamiaPolicyResponse responseValue = new AlamiaPolicyResponse();
            string result = string.Empty;
            var stringPayload = JsonConvert.SerializeObject(request);
            try
            {
                var httpResponse = (response as HttpResponseMessage);
                if (httpResponse != null && httpResponse.IsSuccessStatusCode)
                {
                    result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                    // result = JsonConvert.SerializeObject(res);
                    responseValue = JsonConvert.DeserializeObject<AlamiaPolicyResponse>(result);

                    policyResponse = new PolicyResponse
                    {
                        ReferenceId = responseValue.ReferenceId,
                        StatusCode = responseValue.StatusCode,
                        PolicyNo = responseValue.PolicyNo,
                        PolicyIssuanceDate = (responseValue.PolicyIssuanceDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(responseValue.PolicyIssuanceDate)
                                            : null,
                        PolicyEffectiveDate = (responseValue.PolicyEffectiveDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(responseValue.PolicyEffectiveDate)
                                            : null,
                        PolicyExpiryDate = (responseValue.PolicyExpiryDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(responseValue.PolicyExpiryDate)
                                            : null,
                        Errors = responseValue.Errors,
                    };

                    var _stringFile = responseValue.PolicyFile[0];
                    if (_stringFile.Contains("\n"))
                        _stringFile = _stringFile.Replace("\n", "");

                    byte[] data = System.Convert.FromBase64String(_stringFile);
                    var base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);

                    byte[] bytes = System.Convert.FromBase64String(base64Decoded);

                    policyResponse.PolicyFile = bytes;
                }

                return policyResponse;
            }
            catch (Exception ex)
            {
                _logger.Log($"AlalamiyaInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                LogIntegrationTransaction($"Test Get policy with reference id: {request.ReferenceId} for company: Alalamiya", stringPayload, result, responseValue?.StatusCode);
            }


            return null;
        }
    
        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //in case test mode execute the code from the base.
            if (_tameenkConfig.Quotatoin.TestMode)
                return base.ExecutePolicyRequest(policy, predefinedLogInfo);

            short productTypeCode = 1;
            var info = _checkoutDetail.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (info != null)
            {
                if (info.SelectedInsuranceTypeCode.HasValue)
                    productTypeCode = info.SelectedInsuranceTypeCode.Value;
            }
            if (productTypeCode == 2)
            {
                _restfulConfiguration.GeneratePolicyUrl = POLICY_COMPREHENSIVE_URL;
            }
            ServiceOutput output = SubmitPolicyRequest(policy, predefinedLogInfo);
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
            //log.CompanyID = insur;
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            var stringPayload = string.Empty;
            DateTime dtBeforeCalling = DateTime.Now;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                quotation.PolicyEffectiveDate = DateTime.ParseExact(quotation.PolicyEffectiveDate.ToString("yyyy-MM-ddTHH:mm:sszzz", new CultureInfo("en-US")), "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

                foreach (var driver in quotation.Drivers)
                {
                    driver.DriverBirthDateG = DateTime.ParseExact(driver.DriverBirthDateG.ToString("yyyy-MM-ddTHH:mm:sszzz", new CultureInfo("en-US")), "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
                }

                if (quotation.ProductTypeCode != 2)
                    quotation.DeductibleValue = null;
                if (quotation.VehicleEngineSizeCode == 0)
                    quotation.VehicleEngineSizeCode = null;

                log.ServiceRequest = JsonConvert.SerializeObject(quotation);

                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "Location", "12" },
                    { "PartnerId", "Tameenk" }/*,
                    { "Accept", "application/json" },
                    { "Content-Type", "application/json" }*/
                };
                dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateQuotationUrl, quotation, "VEFNRUVOS19VU0VSOlczQHIzYjM1dCE=", authorizationMethod: "Basic", headers: headers);
                //var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateQuotationUrl, quotation, _accessTokenBase64, authorizationMethod: "Basic", headers: headers);
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
                AlamiaQuotationServiceResponse quotationServiceResponse;

                if (quotation.ProductTypeCode == 1)
                {
                    quotationServiceResponse = new AlamiaQuotationServiceResponse();
                    var deserialised = AlamiaDeserializeJsonWithErrorObject(response.Content.ReadAsStringAsync().Result, quotationServiceResponse, out bool isDeserialised);
                    if (!isDeserialised)
                        quotationServiceResponse = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponse>(response.Content.ReadAsStringAsync().Result);
                    else
                        quotationServiceResponse = deserialised;

                    if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in quotationServiceResponse.errors)
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
                }
                if (quotation.ProductTypeCode == 2)
                {
                    var quotationServiceResponsecom = new AlamiaQuotationServiceResponseComp();
                    var deserialised = AlamiaDeserializeJsonWithErrorObjectComp(response.Content.ReadAsStringAsync().Result, quotationServiceResponsecom, out bool isDeserialised);
                    if (!isDeserialised)
                        quotationServiceResponsecom = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponseComp>(response.Content.ReadAsStringAsync().Result);
                    else
                        quotationServiceResponsecom = deserialised;
                    if (quotationServiceResponsecom != null && quotationServiceResponsecom.Products == null && quotationServiceResponsecom.errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in quotationServiceResponsecom.errors)
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
                }
                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            //  return null;
        }

        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            var quotationServiceResponse = new QuotationServiceResponse();
            string result = string.Empty;
            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;

            if (request.ProductTypeCode == 1)
            {
                AlamiaQuotationResponseWithErrorObject alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObject();
                AlamiaQuotationServiceResponse serviceResponse = new AlamiaQuotationServiceResponse();

                var deserialised = AlamiaDeserializeJsonWithErrorObject(result, serviceResponse, out bool isDeserialised);
                if (!isDeserialised)
                    serviceResponse = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponse>(result);
                else
                    serviceResponse = deserialised;

                if (serviceResponse != null)
                {
                    quotationServiceResponse.ReferenceId = serviceResponse.ReferenceId;
                    quotationServiceResponse.StatusCode = serviceResponse.StatusCode;
                    quotationServiceResponse.Errors = serviceResponse.errors;
                    quotationServiceResponse.QuotationNo = serviceResponse.QuotationNo;
                    quotationServiceResponse.QuotationDate = serviceResponse.QuotationDate;
                    quotationServiceResponse.QuotationExpiryDate = serviceResponse.QuotationExpiryDate;
                    if (serviceResponse.Products != null)
                    {
                        quotationServiceResponse.Products = new List<ProductDto>();
                        quotationServiceResponse.Products.Add(serviceResponse.Products);


                    }
                }

                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
                {

                    quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
                }
            }
            else
            {
                AlamiaQuotationResponseWithErrorObjectComp alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObjectComp();
                AlamiaQuotationServiceResponseComp serviceResponse = new AlamiaQuotationServiceResponseComp();

                var deserialised = AlamiaDeserializeJsonWithErrorObjectComp(result, serviceResponse, out bool isDeserialised);
                if (!isDeserialised)
                    serviceResponse = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponseComp>(result);
                else
                    serviceResponse = deserialised;

                if (serviceResponse != null)
                {
                    quotationServiceResponse.ReferenceId = serviceResponse.ReferenceId;
                    quotationServiceResponse.StatusCode = serviceResponse.StatusCode;
                    quotationServiceResponse.Errors = serviceResponse.errors;
                    quotationServiceResponse.QuotationNo = serviceResponse.QuotationNo;
                    quotationServiceResponse.QuotationDate = serviceResponse.QuotationDate;
                    quotationServiceResponse.QuotationExpiryDate = serviceResponse.QuotationExpiryDate;
                    if (serviceResponse.Products != null)
                    {
                        quotationServiceResponse.Products = new List<ProductDto>();
                        foreach (var product in serviceResponse.Products)
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

                            quotationServiceResponse.Products.Add(product);
                        }
                    }
                }

                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
                {
                    quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
                }
            }

            return quotationServiceResponse;
        }

        private static AlamiaQuotationServiceResponse AlamiaDeserializeJsonWithErrorObject(string json, AlamiaQuotationServiceResponse quotationServiceResponse, out bool isDeserialised)
        {
            isDeserialised = false;
            AlamiaQuotationResponseWithErrorObject alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObject();
            try
            {
                alamiaQuotationResponseWithErrorObject = JsonConvert.DeserializeObject<AlamiaQuotationResponseWithErrorObject>(json);

                if (alamiaQuotationResponseWithErrorObject == null)
                    return new AlamiaQuotationServiceResponse();

                quotationServiceResponse = new AlamiaQuotationServiceResponse()
                {
                    ReferenceId = alamiaQuotationResponseWithErrorObject.ReferenceId,
                    StatusCode = alamiaQuotationResponseWithErrorObject.StatusCode,
                    QuotationNo = alamiaQuotationResponseWithErrorObject.QuotationNo,
                    QuotationDate = alamiaQuotationResponseWithErrorObject.QuotationDate,
                    QuotationExpiryDate = alamiaQuotationResponseWithErrorObject.QuotationExpiryDate,
                    Products = alamiaQuotationResponseWithErrorObject.Products,
                    errors = (alamiaQuotationResponseWithErrorObject.errors != null)
                                    ? new List<Error> { alamiaQuotationResponseWithErrorObject.errors }
                                    : null,
                };

                isDeserialised = true;
                return quotationServiceResponse;
            }
            catch (Exception ex)
            {
                return new AlamiaQuotationServiceResponse();
            }
        }

        protected override CustomCardServiceOutput SubmitUpdateCustomCardRequest(UpdateCustomCardRequest request, ServiceRequestLog log)
        {
            CustomCardServiceOutput output = new CustomCardServiceOutput();
            log.ReferenceId = request.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServiceURL = _restfulConfiguration.UpdateCustomCardUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "UpdateCustomCard";
            log.CompanyName = _restfulConfiguration.ProviderName;
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                DateTime dtBeforeCalling = DateTime.Now;

                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "Location", "12" },
                    { "PartnerId", "Tameenk" }
                };
                var postTask = _httpClient.PostAsync(_restfulConfiguration.UpdateCustomCardUrl, request, "VEFNRUVOS19VU0VSOlczQHIzYjM1dCE=", authorizationMethod: "Basic", headers: headers);
                //var postTask = _httpClient.PostAsync(_restfulConfiguration.UpdateCustomCardUrl, request, _accessTokenBase64, authorizationMethod: "Basic", headers: headers);
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
                output.ServiceResponse = log.ServiceResponse;
                var responseInfo = JsonConvert.DeserializeObject<UpdateCustomCardResponse>(response.Content.ReadAsStringAsync().Result);
                if (responseInfo != null && responseInfo.Errors != null)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();
                    foreach (var error in responseInfo.Errors)
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
                if (string.IsNullOrEmpty(responseInfo.ReferenceId))
                {
                    output.ErrorCode = CustomCardServiceOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "responseInfo ReferenceId is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Output = response;
                output.UpdateCustomCardResponse = responseInfo;
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

        private static AlamiaQuotationServiceResponseComp AlamiaDeserializeJsonWithErrorObjectComp(string json, AlamiaQuotationServiceResponseComp quotationServiceResponse, out bool isDeserialised)
        {
            isDeserialised = false;
            AlamiaQuotationResponseWithErrorObjectComp alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObjectComp();
            try
            {
                alamiaQuotationResponseWithErrorObject = JsonConvert.DeserializeObject<AlamiaQuotationResponseWithErrorObjectComp>(json);

                if (alamiaQuotationResponseWithErrorObject == null)
                    return new AlamiaQuotationServiceResponseComp();

                quotationServiceResponse = new AlamiaQuotationServiceResponseComp()
                {
                    ReferenceId = alamiaQuotationResponseWithErrorObject.ReferenceId,
                    StatusCode = alamiaQuotationResponseWithErrorObject.StatusCode,
                    QuotationNo = alamiaQuotationResponseWithErrorObject.QuotationNo,
                    QuotationDate = alamiaQuotationResponseWithErrorObject.QuotationDate,
                    QuotationExpiryDate = alamiaQuotationResponseWithErrorObject.QuotationExpiryDate,
                    Products = alamiaQuotationResponseWithErrorObject.Products,
                    errors = (alamiaQuotationResponseWithErrorObject.errors != null)
                                    ? new List<Error> { alamiaQuotationResponseWithErrorObject.errors }
                                    : null,
                };

                isDeserialised = true;
                return quotationServiceResponse;
            }
            catch (Exception ex)
            {
                return new AlamiaQuotationServiceResponseComp();
            }
        }
        #endregion
    }
}
