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
using Tameenk.Services.Core.Http;
using Tameenk.Services.Logging;


namespace Tameenk.Integration.Providers.AlRajhi
{
    public class AlRajhiInsuranceProvider : RestfulInsuranceProvider
    {
        //#region Fields
        //private readonly ILogger _logger;
        //private readonly TameenkConfig _tameenkConfig;
        //private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        //#endregion
        private readonly IHttpClient _httpClient;
        private readonly string _accessTokenBase64;
        private readonly RestfulConfiguration _restfulConfiguration;
        #region ctor
        //public AlRajhiInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
        //     : base(new ProviderConfiguration() { ProviderName = "AlRajhi" }, logger)
        //{
        //    _logger = logger;
        //    _tameenkConfig = tameenkConfig;
        //    _policyProcessingQueueRepository = policyProcessingQueueRepository;
        //}

        public AlRajhiInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
           : base(tameenkConfig, new RestfulConfiguration
           {
               GenerateQuotationUrl = "https://dspeai.alrajhitakaful.com/ART.NCIS.TameenK/TameenK.svc/Quotation",
               GeneratePolicyUrl = "https://dspeai.alrajhitakaful.com/ART.NCIS.TameenK/TameenK.svc/Policy",
               AccessToken = "BcareProd:aRt!968",
               ProviderName = "AlRajhi",
               GenerateAutoleasingQuotationUrl= "https://dspeai.alrajhitakaful.com/ART.NCIS.BcareLeasing/TameenK.svc/Quotation",
               GenerateAutoleasingPolicyUrl= "https://dspeai.alrajhitakaful.com/ART.NCIS.BcareLeasing/TameenK.svc/Quotation",
               AutoleasingAccessToken= "BcareLeaseUsr:kfTds!45754"

           }, logger, policyProcessingQueueRepository)
        {
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AutoleasingAccessToken) ?
             null :
             Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AutoleasingAccessToken));
        }

        public override bool ValidateQuotationBeforeCheckout(QuotationRequest quotationRequest, out List<string> errors)
        {
            var addressService = EngineContext.Current.Resolve<IAddressService>();
            errors = new List<string>();
            var mainDriverAddress = quotationRequest.Driver.Addresses.FirstOrDefault();
            if (mainDriverAddress != null)
            {
                var nationalAddressCity = addressService.GetCityCenterById(mainDriverAddress.CityId);
                //var insuredCityCenter = addressService.GetCityCenterCenterByElmCode(quotationRequest.Insured.CityId.ToString());
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

            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;
            responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
            if(request.ProductTypeCode == 2)
            {
                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        if (product != null)
                        {
                            if (product.ProductId == "8") //Wafi Smart
                            {
                                product.InsuranceTypeCode = 8;
                            }

                            if (product.Benefits != null)
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
            }

            return responseValue;
        }


        #endregion



        #region autoleasing
        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
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


                //if (quotation.ProductTypeCode != 2)
                //    quotation.DeductibleValue = null;

                log.ServiceRequest = JsonConvert.SerializeObject(quotation);
                DateTime dtBeforeCalling = DateTime.Now;
                var postTask = _httpClient.PostAsync(_restfulConfiguration.GenerateAutoleasingQuotationUrl, quotation, _accessTokenBase64, authorizationMethod: "Basic");
                postTask.Wait();
                response = postTask.Result;
                //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\Alrajhi_wataniy.txt", JsonConvert.SerializeObject(response));
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
                        output.ErrorDescription = "Service response content result return null and httpstatus is: " + response.StatusCode;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                        //File.WriteAllText(@"C:\inetpub\WataniyaLog\Sala_response.txt", JsonConvert.SerializeObject(response));
                        //File.WriteAllText(@"C:\inetpub\WataniyaLog\Sala_response.Content.txt", JsonConvert.SerializeObject(response.Content.ReadAsStringAsync().Result));

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

                output.Output = response;
                output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                log.ServiceResponse = response.Content?.ReadAsStringAsync()?.Result;
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
        #endregion
        // #region methods

        //protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        //{
        //    //quotation.InsuredBirthDate = quotation.InsuredBirthDateG;
        //    //if (quotation.Drivers != null)
        //    //{
        //    //    foreach (DriverDto driver in quotation.Drivers)
        //    //    {
        //    //        driver.DriverBirthDate = driver.DriverBirthDateG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
        //    //    }
        //    //}
        //    return quotation;
        //}

        //private string ReadResource(string strnamespace, string strFileName)
        //{

        //    try
        //    {

        //        var assembly = Assembly.Load(strnamespace);
        //        string result = "";
        //        Stream stream = assembly.GetManifestResourceStream(strnamespace + strFileName);

        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            result = reader.ReadToEnd();
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}


        //protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        //{

        //    var result = SubmitQuotationRequest(quotation, predefinedLogInfo);

        //    if (result.ErrorCode != QuotationOutput.ErrorCodes.Success)
        //    {
        //        return null;
        //    }

        //    return result.Output;
        //}



        //protected QuotationOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        //{
        //    QuotationOutput output = new QuotationOutput();
        //    log.ReferenceId = quotation.ReferenceId;
        //    log.Channel = "Portal";
        //    log.ServerIP = ServicesUtilities.GetServerIP();
        //    log.Method = "Quotation";
        //    log.CompanyName = "AlRajhi";
        //    log.VehicleMaker = quotation?.VehicleMaker;
        //    log.VehicleMakerCode = quotation?.VehicleMakerCode;
        //    log.VehicleModel = quotation?.VehicleModel;
        //    log.VehicleModelCode = quotation?.VehicleModelCode;
        //    log.VehicleModelYear = quotation?.VehicleModelYear;
        //    try
        //    {
        //        var testMode = _tameenkConfig.Quotatoin.TestMode;
        //        if (testMode)
        //        {
        //            const string nameOfFile = ".TestData.quotationTestData.json";
        //            string responseData = ReadResource(GetType().Namespace, nameOfFile);
        //            HttpResponseMessage message = new HttpResponseMessage();
        //            message.Content = new StringContent(responseData);
        //            message.StatusCode = System.Net.HttpStatusCode.OK;

        //            output.Output = message;
        //            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";

        //            return output;
        //        }
        //        var stringPayload = JsonConvert.SerializeObject(quotation);
        //        //var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
        //        log.ServiceRequest = stringPayload;
        //        using (AlRajhi.TameenKClient client = new AlRajhi.TameenKClient())
        //        {

        //            client.ClientCredentials.UserName.UserName = "TameenkTest";
        //            client.ClientCredentials.UserName.Password = "aJk!123";

        //            log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri; ;
        //            DateTime dtBeforeCalling = DateTime.Now;
        //            string result = client.Quotation(stringPayload);
        //            DateTime dtAfterCalling = DateTime.Now;
        //            log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
        //            if (string.IsNullOrEmpty(result))
        //            {
        //                output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
        //                output.ErrorDescription = "response return null";
        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                log.ServiceErrorCode = log.ErrorCode.ToString();
        //                log.ServiceErrorDescription = log.ServiceErrorDescription;
        //                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //                return output;
        //            }
        //            log.ServiceResponse = result;

        //            var quotationServiceResponse = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
        //            if (quotationServiceResponse != null && quotationServiceResponse.Errors != null)
        //            {
        //                StringBuilder servcieErrors = new StringBuilder();
        //                StringBuilder servcieErrorsCodes = new StringBuilder();

        //                foreach (var error in quotationServiceResponse.Errors)
        //                {
        //                    servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
        //                    servcieErrorsCodes.AppendLine(error.Code);
        //                }
        //                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceError;
        //                output.ErrorDescription = "Quotation Service response error is : " + servcieErrors.ToString();
        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                log.ServiceErrorCode = servcieErrorsCodes.ToString();
        //                log.ServiceErrorDescription = servcieErrors.ToString();
        //                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //                return output;
        //            }
        //            HttpResponseMessage message = new HttpResponseMessage();
        //            message.Content = new StringContent(result);
        //            message.StatusCode = System.Net.HttpStatusCode.OK;

        //            output.Output = message;
        //            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            log.ServiceErrorCode = log.ErrorCode.ToString();
        //            log.ServiceErrorDescription = log.ServiceErrorDescription;
        //            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //            return output;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = ex.Message;
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = output.ErrorDescription;
        //        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //        return output;
        //    }
        //}

        //protected override ProviderInfoDto GetProviderInfo()
        //{
        //    var providerInfo = new ProviderInfoDto
        //    {
        //        QuotationUrl = "https://tameeniservice.alrajhitakaful.com/ART.NCIS.TameeniK/TameenK.svc?wsdl/Quotation",
        //        PolicyUrl = "https://tameeniservice.alrajhitakaful.com/ART.NCIS.TameeniK/TameenK.svc?wsdl/Policy"
        //    };
        //    return providerInfo;
        //}


        //protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        //{
        //    var result = SubmitPolicyRequest(policy, predefinedLogInfo);
        //    if (result.ErrorCode != PolicyOutput.ErrorCodes.Success)
        //    {
        //        return null;
        //    }

        //    return result.Output;
        //}


        //protected PolicyOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        //{
        //    PolicyOutput output = new PolicyOutput();
        //    log.ReferenceId = policy.ReferenceId;
        //    log.Channel = "Portal";
        //    log.ServerIP = ServicesUtilities.GetServerIP();
        //    log.Method = "Policy";
        //    log.CompanyName = "AlRajhi";


        //    var request = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
        //    if (request != null)
        //    {
        //        request.RequestID = log.RequestId;
        //        request.CompanyName = log.CompanyName;
        //        request.CompanyID = log.CompanyID;
        //        request.InsuranceTypeCode = log.InsuranceTypeCode;
        //        request.DriverNin = log.DriverNin;
        //        request.VehicleId = log.VehicleId;
        //    }


        //    try
        //    {
        //        var testMode = _tameenkConfig.Policy.TestMode;
        //        if (testMode)
        //        {
        //            const string nameOfFile = ".TestData.policyTestData.json";

        //            string responseData = ReadResource("Tameenk.Integration.Providers.AlRajhi", nameOfFile);
        //            HttpResponseMessage message = new HttpResponseMessage();
        //            message.Content = new StringContent(responseData);
        //            message.StatusCode = System.Net.HttpStatusCode.OK;
        //            output.Output = message;
        //            output.ErrorCode = PolicyOutput.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";

        //            return output;
        //        }

        //        using (AlRajhi.TameenKClient client = new AlRajhi.TameenKClient())
        //        {
        //            client.ClientCredentials.UserName.UserName = "TameenkTest";
        //            client.ClientCredentials.UserName.Password = "aJk!123";
        //            log.ServiceURL = client.Endpoint.ListenUri.AbsoluteUri;

        //            var stringPayload = JsonConvert.SerializeObject(policy);
        //            var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
        //            log.ServiceRequest = stringPayload;
        //            request.ServiceRequest = log.ServiceRequest;

        //            DateTime dtBeforeCalling = DateTime.Now;
        //            string result = client.Policy(stringPayload);
        //            DateTime dtAfterCalling = DateTime.Now;
        //            log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
        //            if (string.IsNullOrEmpty(result))
        //            {
        //                output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
        //                output.ErrorDescription = "response return null";
        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                log.ServiceErrorCode = log.ErrorCode.ToString();
        //                log.ServiceErrorDescription = log.ServiceErrorDescription;
        //                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

        //                if (request != null)
        //                {
        //                    request.ErrorDescription = output.ErrorDescription;
        //                    _policyProcessingQueueRepository.Update(request);
        //                }

        //                return output;
        //            }

        //            log.ServiceResponse = result;
        //            request.ServiceResponse = log.ServiceResponse;

        //            var policyServiceResponse = JsonConvert.DeserializeObject<PolicyResponse>(result);
        //            if (policyServiceResponse != null && policyServiceResponse.Errors != null)
        //            {
        //                StringBuilder servcieErrors = new StringBuilder();
        //                StringBuilder servcieErrorsCodes = new StringBuilder();

        //                foreach (var error in policyServiceResponse.Errors)
        //                {
        //                    servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
        //                    servcieErrorsCodes.AppendLine(error.Code);

        //                }

        //                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
        //                output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                log.ServiceErrorCode = servcieErrorsCodes.ToString();
        //                log.ServiceErrorDescription = servcieErrors.ToString();
        //                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //                if (request != null)
        //                {
        //                    request.ErrorDescription = output.ErrorDescription;
        //                    _policyProcessingQueueRepository.Update(request);
        //                }
        //                return output;
        //            }
        //            if (string.IsNullOrEmpty(policyServiceResponse.PolicyNo))
        //            {
        //                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceError;
        //                output.ErrorDescription = "No PolicyNo returned from company";
        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                log.ServiceErrorCode = log.ErrorCode.ToString();
        //                log.ServiceErrorDescription = log.ServiceErrorDescription;
        //                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //                if (request != null)
        //                {
        //                    request.ErrorDescription = output.ErrorDescription;
        //                    _policyProcessingQueueRepository.Update(request);
        //                }
        //                return output;
        //            }

        //            HttpResponseMessage message = new HttpResponseMessage();
        //            message.Content = new StringContent(result);
        //            message.StatusCode = System.Net.HttpStatusCode.OK;

        //            output.Output = message;
        //            output.ErrorCode = PolicyOutput.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";
        //            log.PolicyNo = policyServiceResponse.PolicyNo;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            log.ServiceErrorCode = log.ErrorCode.ToString();
        //            log.ServiceErrorDescription = log.ServiceErrorDescription;
        //            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //            if (request != null)
        //            {
        //                request.ErrorDescription = output.ErrorDescription;
        //                _policyProcessingQueueRepository.Update(request);
        //            }
        //            return output;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = ex.GetBaseException().ToString();
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = output.ErrorDescription;
        //        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
        //        if (request != null)
        //        {
        //            request.ErrorDescription = output.ErrorDescription;
        //            _policyProcessingQueueRepository.Update(request);
        //        }
        //        return output;
        //    }
        //}
        //protected override PolicyResponse GetPolicyResponseObject(object response, PolicyRequest request = null)
        //{
        //    string policyResponse = string.Empty;
        //    PolicyResponse responseValue = null;
        //    try
        //    {
        //        policyResponse = JsonConvert.SerializeObject(response);
        //        responseValue = JsonConvert.DeserializeObject<PolicyResponse>(policyResponse);
        //        //responseValue.IssueCompany = Company.ArabianShield;

        //        return responseValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log("AlRajhiInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
        //    }
        //    finally
        //    {
        //        // Insert row in the integration transaction table to track the input/output for this company, 
        //        // this will be helpful in case of integration testing phase
        //        if (!_tameenkConfig.Policy.TestMode)
        //        {
        //            LogIntegrationTransaction($"AlRajhi Insurance company policy test with reference id : {request.ReferenceId}",
        //                JsonConvert.SerializeObject(request), policyResponse, responseValue != null ? responseValue.StatusCode : 2);
        //        }
        //    }

        //    return null;
        //}

        //#endregion

    }
}
