using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Resources.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Net.Http.Headers;

namespace Tameenk.Integration.Providers.ArabianShield
{
    public class ArabianShieldInsuranceProvider : RestfulInsuranceProvider
    {

        #region fields

        private readonly ILogger _logger;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly string _accessTokenBase64;
        #endregion

        #region ctor
        public ArabianShieldInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(tameenkConfig, new RestfulConfiguration
             {
                 GenerateQuotationUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/Quotation",
                 GeneratePolicyUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/Policy",
                 //SchedulePolicyUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/PolicySchedule",
                 UpdateCustomCardUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/UpdateCustomCard",
                 CancelPolicyUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/PolicyCancellation",
                 GenerateVehicleClaimRegistrationUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/ClaimRegistration",
                 GenerateVehicleClaimNotificationUrl = "https://aggregatorprd.der3.com/BcareWebApi/Tameenk/api/ClaimsNotificationservice",
                 AccessToken = "BCARE:Bcare@123",
                 ProviderName = "ArabianShield"
             }, logger, policyProcessingQueueRepository)
        {
            _logger = logger;
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _accessTokenBase64 = _restfulConfiguration.AccessToken;


        }
        #endregion

        #region Quotation Methods


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
            DateTime dtBeforeCalling = DateTime.Now;
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

                stringPayload = JsonConvert.SerializeObject(quotation);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(4);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_accessTokenBase64)));
                var postTask = client.PostAsync(_restfulConfiguration.GenerateQuotationUrl, httpContent);
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
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
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
        }




        #endregion

        #region methods
        private string ReadResource(string strnamespace, string strFileName)
        {

            try
            {

                var assembly = Assembly.Load(strnamespace);
                string result = "";
                Stream stream = assembly.GetManifestResourceStream(strnamespace + strFileName);

                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {

            if (!_tameenkConfig.Quotatoin.TestMode)
            {
                if (string.IsNullOrEmpty(request.VehicleRegExpiryDate))
                    request.VehicleRegExpiryDate = "21-05-1442";
            }

            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            var stringPayload = JsonConvert.SerializeObject(request);
            string quoteResponse = string.Empty;

            try
            {
                string result = string.Empty;
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                if (responseValue != null && responseValue.Products == null && (responseValue.Errors != null && responseValue.Errors.Count > 0))
                {
                    responseValue.Errors = new List<Error> { new Error { Message = result } };
                }
                else
                {
                    responseValue.Errors = null;
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
                }
            }
            catch (Exception ex)
            {
                _logger.Log("ArabianShieldInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();
                responseValue.Errors.Insert(0, new Error { Message = stringPayload });
                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }

            return responseValue;
        }
        #endregion
        #region Autoleasing Methods
        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            var result = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);
            if (result.ErrorCode != QuotationOutput.ErrorCodes.Success)
            {
                return null;
            }

            return result.Output;
        }

        protected QuotationOutput SubmitAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.Channel = "autoleasing";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingQuotation";
            log.CompanyName = "ArabianShield";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource("Tameenk.Integration.Providers.ArabianShield", nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }
                using (ArabianShieldAutoleasingService.BcareAutoLeaseClient serviceClient = new ArabianShieldAutoleasingService.BcareAutoLeaseClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    string stringPayload = JsonConvert.SerializeObject(quotation);
                    string authorization = "{\"Authorization\":{\"UserName\":\"BcareLease\",\"Password\":\"Ag_2020$SA*LEA\"},";
                    stringPayload = stringPayload.Trim();
                    stringPayload = stringPayload.Remove(0, 1);
                    stringPayload = stringPayload.Insert(0, authorization);
                    log.ServiceRequest = stringPayload;

                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = serviceClient.Quotation(stringPayload);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (string.IsNullOrEmpty(result))
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "response return null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    log.ServiceResponse = result;
                    var response = JsonConvert.DeserializeObject<object>(result);
                    var policyServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                    if (policyServiceResponse != null && policyServiceResponse.Errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in policyServiceResponse.Errors)
                        {
                            servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                            servcieErrorsCodes.AppendLine(error.Code);

                        }
                        output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Service response error is : " + servcieErrors.ToString();
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = servcieErrorsCodes.ToString();
                        log.ServiceErrorDescription = servcieErrors.ToString();
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    output.Output = response;
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        protected override object ExecuteAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            var result = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (result.ErrorCode != PolicyOutput.ErrorCodes.Success)
            {
                return null;
            }

            return result.Output;

        }

        protected PolicyOutput SubmitAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {
            PolicyOutput output = new PolicyOutput();
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "autoleasing";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingPolicy";
            log.CompanyName = "ArabianShield";

            var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
            if (request != null)
            {
                request.RequestID = log.RequestId;
                request.CompanyName = log.CompanyName;
                request.CompanyID = log.CompanyID;
                request.InsuranceTypeCode = log.InsuranceTypeCode;
                request.DriverNin = log.DriverNin;
                request.VehicleId = log.VehicleId;
            }

            try
            {
                var testMode = _tameenkConfig.Policy.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.policyTestData.json";

                    string responseData = ReadResource("Tameenk.Integration.Providers.ArabianShield", nameOfFile);

                    output.Output = JsonConvert.DeserializeObject<object>(responseData);
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }
                using (ArabianShieldAutoleasingService.BcareAutoLeaseClient serviceClient = new ArabianShieldAutoleasingService.BcareAutoLeaseClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;

                    var stringPayload = JsonConvert.SerializeObject(policy);
                    string authorization = "{\"Authorization\":{\"UserName\":\"BcareLease\",\"Password\":\"Ag_2020$SA*LEA\"},";
                    stringPayload = stringPayload.Trim();
                    stringPayload = stringPayload.Remove(0, 1);
                    stringPayload = stringPayload.Insert(0, authorization);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;
                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = serviceClient.Policy(stringPayload);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (string.IsNullOrEmpty(result))
                    {
                        output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "response return null";
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
                    log.ServiceResponse = result;
                    request.ServiceResponse = log.ServiceResponse;
                    var policyServiceResponse = JsonConvert.DeserializeObject<PolicyResponse>(result);
                    if (policyServiceResponse != null && policyServiceResponse.Errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in policyServiceResponse.Errors)
                        {
                            servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                            servcieErrorsCodes.AppendLine(error.Code);

                        }

                        output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
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
                        output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
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


                    var response = JsonConvert.DeserializeObject<object>(result);
                    output.Output = response;
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.PolicyNo = policyServiceResponse.PolicyNo;
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
            }
            catch (Exception ex)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
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

        protected override PolicyResponse GetAutoleasingPolicyResponseObject(object response, PolicyRequest request = null)
        {
            string policyResponse = string.Empty;
            PolicyResponse responseValue = null;
            try
            {
                policyResponse = JsonConvert.SerializeObject(response);
                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(policyResponse);
                //responseValue.IssueCompany = Company.ArabianShield;

                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log("ArabianShieldInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            return null;
        }

        #endregion


    }
}
