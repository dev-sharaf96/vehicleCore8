using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
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
using Tameenk.Services.Logging;


namespace Tameenk.Integration.Providers.MedGulf
{
    public class MedGulfInsuranceProvider : InsuranceProvider
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private const string _userName = "4491";
        private const string _password = "4491@TKZUPZ";
        private const string _userNameAutoleasing = "TmnkMg";
        private const string _passwordAutoleasing = "KpNg@3075";
        #endregion

        #region ctor
        public MedGulfInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "Med Gulf" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
        }
        #endregion

        #region methods

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            quotation.InsuredBirthDate = quotation.InsuredBirthDateG;
            if (quotation.Drivers != null)
            {
                foreach (DriverDto driver in quotation.Drivers)
                {
                    driver.DriverBirthDate = driver.DriverBirthDateG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                }
            }
            return quotation;
        }

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


        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {

            var result = SubmitQuotationRequest(quotation, predefinedLogInfo);

            if (result.ErrorCode != QuotationOutput.ErrorCodes.Success)
            {
                return null;
            }

            return result.Output;
        }



        protected QuotationOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "MedGulf";
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
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(quotation);
                var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                log.ServiceRequest = stringPayload;
                using (MedGulfService.TameenkSoapClient client = new MedGulfService.TameenkSoapClient())
                {
                    log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri; ;
                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = client.Quotes(_userName, _password, stringPayload);
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

                    var policyServiceResponse = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);
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
                        output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = servcieErrorsCodes.ToString();
                        log.ServiceErrorDescription = servcieErrors.ToString();
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(result);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
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
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "https://onlineservices.medgulf.com.sa/tameenk/webServices/tameenk.asmx?WSDL/Quotes",
                PolicyUrl = "https://onlineservices.medgulf.com.sa/tameenk/webServices/tameenk.asmx?WSDL/Policy"
            };
            return providerInfo;
        }


        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //string result = "";
            //HttpResponseMessage message = new HttpResponseMessage();
            //try
            //{


            //    string nameSpace = "Tameenk.Integration.Providers.MedGulf";
            //    // using Tameenk config 
            //    var testMode = _tameenkConfig.Policy.TestMode;
            //    if (testMode)
            //    {
            //        const string nameOfFile = ".TestData.policyTestData.json";

            //        string responseData = ReadResource(nameSpace, nameOfFile);
            //        var json = JsonConvert.DeserializeObject<object>(responseData);

            //        message.Content = new StringContent(responseData.ToString());
            //        message.StatusCode = System.Net.HttpStatusCode.OK;
            //        return message;
            //    }


            //    var stringPayload = JsonConvert.SerializeObject(policy);
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    MedGulfService.TameenkSoapClient client = new MedGulfService.TameenkSoapClient();
            //    result = client.Policy("10282", "10282@QNZULPZ", stringPayload);

            //    message.Content = new StringContent(result);
            //    message.StatusCode = System.Net.HttpStatusCode.OK;
            //    return message;

            //    ////var response = JsonConvert.DeserializeObject<object>(result);
            //    ////return response;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log($"MedGulfInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
            //}


            //return null;

            var result = SubmitPolicyRequest(policy, predefinedLogInfo);

            if (result.ErrorCode != PolicyOutput.ErrorCodes.Success)
            {
                return null;
            }

            return result.Output;
        }


        protected PolicyOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {
            PolicyOutput output = new PolicyOutput();
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Policy";
            log.CompanyName = "MedGulf";


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

                    string responseData = ReadResource("Tameenk.Integration.Providers.MedGulf", nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                using (MedGulfService.TameenkSoapClient client = new MedGulfService.TameenkSoapClient())
                {
                    log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri;

                    var stringPayload = JsonConvert.SerializeObject(policy);
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;

                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = client.Policy(_userName, _password, stringPayload);
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

                    var policyServiceResponse = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);
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

                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(result);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
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




        protected override PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        {
            MedGulfPolicyResponse responseValue = new MedGulfPolicyResponse();
            string result = string.Empty;
            var stringPayload = JsonConvert.SerializeObject(request);
            try
            {
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                // result = JsonConvert.SerializeObject(res);
                responseValue = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);



                PolicyResponse policyResponse = new PolicyResponse
                {
                    Errors = responseValue.Errors,
                    PolicyFileUrl = responseValue.PolicyFileUrl,
                    PolicyNo = responseValue.PolicyNo,
                    ReferenceId = responseValue.ReferenceId,
                    StatusCode = responseValue.StatusCode,
                    PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyEffectiveDate),
                    PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyExpiryDate),
                    PolicyIssuanceDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyIssuanceDate)
                };

                return policyResponse;
            }
            catch (Exception ex)
            {
                _logger.Log($"MedGulfInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                LogIntegrationTransaction($"Test Get policy with reference id: {request.ReferenceId} for company: MedGulf", stringPayload, result, responseValue?.StatusCode);
            }


            return null;
        }

        public PolicyScheduleResponse PolicySchedule(PolicyScheduleRequest policy)
        {
            var stringPayload = JsonConvert.SerializeObject(policy);
            var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            MedGulfService.TameenkSoapClient client = new MedGulfService.TameenkSoapClient();
            string result = client.Policy(_userName, _password, stringPayload);
            PolicyScheduleResponse responseValue = JsonConvert.DeserializeObject<PolicyScheduleResponse>(result);
            return responseValue;
        }

        public override bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            errors = new List<string>();
            var mainDriverAddress = quotationRequest.Driver.Addresses.FirstOrDefault();
            if (mainDriverAddress != null)
            {
                var nationalAddressCity = addressService.GetCityCenterById(mainDriverAddress.CityId);
                //var insuredCityCenter = addressService.GetCityCenterById(quotationRequest.Insured.CityId.ToString());
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
        protected override object ExecuteVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            ClaimRegistrationServiceOutput output = SubmitVehicleClaimRegistrationRequest(claim, predefinedLogInfo);
            if (output.ErrorCode != ClaimRegistrationServiceOutput.ErrorCodes.Success)
                return null;

            return output.Output;
        }

        protected override object ExecuteVehicleClaimNotificationRequest(ClaimNotificationRequest claim, ServiceRequestLog predefinedLogInfo)
        {
            ClaimNotificationServiceOutput output = SubmitVehicleClaimNotificationRequest(claim, predefinedLogInfo);
            if (output.ErrorCode != ClaimNotificationServiceOutput.ErrorCodes.Success)
                return null;

            return output.Output;
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
            log.CompanyName = "MedGulf";
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
                    string responseData = ReadResource(GetType().Namespace, nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(quotation);
                var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                log.ServiceRequest = stringPayload;
                using (MedGulfAutoleasingService.TameenkSoapClient client = new MedGulfAutoleasingService.TameenkSoapClient())
                {
                    log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri; ;
                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = client.Quotes(_userNameAutoleasing, _passwordAutoleasing, stringPayload);
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

                    var policyServiceResponse = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);
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
                        output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = servcieErrorsCodes.ToString();
                        log.ServiceErrorDescription = servcieErrors.ToString();
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(result);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
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
                output.ErrorDescription = ex.Message;
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
            log.CompanyName = "MedGulf";


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

                    string responseData = ReadResource("Tameenk.Integration.Providers.MedGulf", nameOfFile);
                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(responseData);
                    message.StatusCode = System.Net.HttpStatusCode.OK;
                    output.Output = message;
                    output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";

                    return output;
                }

                using (MedGulfAutoleasingService.TameenkSoapClient client = new MedGulfAutoleasingService.TameenkSoapClient())
                {
                    log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri;

                    var stringPayload = JsonConvert.SerializeObject(policy);
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;

                    DateTime dtBeforeCalling = DateTime.Now;
                    string result = client.Policy(_userNameAutoleasing, _passwordAutoleasing, stringPayload);
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

                    var policyServiceResponse = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);
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

                    HttpResponseMessage message = new HttpResponseMessage();
                    message.Content = new StringContent(result);
                    message.StatusCode = System.Net.HttpStatusCode.OK;

                    output.Output = message;
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
            MedGulfPolicyResponse responseValue = new MedGulfPolicyResponse();
            string result = string.Empty;
            var stringPayload = JsonConvert.SerializeObject(request);
            try
            {
                result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
                responseValue = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(result);

                PolicyResponse policyResponse = new PolicyResponse
                {
                    Errors = responseValue.Errors,
                    PolicyFileUrl = responseValue.PolicyFileUrl,
                    PolicyNo = responseValue.PolicyNo,
                    ReferenceId = responseValue.ReferenceId,
                    StatusCode = responseValue.StatusCode,
                    PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyEffectiveDate),
                    PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyExpiryDate),
                    PolicyIssuanceDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyIssuanceDate)
                };

                return policyResponse;
            }
            catch (Exception ex)
            {
                _logger.Log($"MedGulfInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            return null;
        }

        #endregion


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
                        if (product == null && product.Benefits == null)
                            continue;

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
    }
}
