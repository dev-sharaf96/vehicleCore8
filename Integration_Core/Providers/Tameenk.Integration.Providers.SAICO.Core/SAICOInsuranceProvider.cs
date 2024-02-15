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
using Tameenk.Core.Domain.Dtos;
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

namespace Tameenk.Integration.Providers.SAICO
{
    public class SAICOInsuranceProvider : RestfulInsuranceProvider
    {
        private readonly RestfulConfiguration _restfulConfiguration;
        private readonly IHttpClient _httpClient;
        private readonly string _accessTokenBase64;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;

        public SAICOInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
          : base(tameenkConfig, new RestfulConfiguration
          {
              GenerateQuotationUrl = "https://tmk.saico.com.sa:8074/TameenK/quotes",
              GeneratePolicyUrl = "https://tmk.saico.com.sa:8074/TameenK/policy",
              UploadImageServiceUrl = "https://tmk.saico.com.sa:8074/TameenK/attachments",
              AccessToken = "TMK_WS_PROD:DOp*aPrUS@9o23",
              GenerateAutoleasingQuotationUrl = "https://tmkmals.saico.com.sa:8065/TameenK/quotes",
              GenerateAutoleasingPolicyUrl = "https://tmkmals.saico.com.sa:8065/TameenK/policy",
              AutoleasingAccessToken = "TameenK:SAic0@12~I",
              ProviderName = "SAICO"
          }, logger, policyProcessingQueueRepository)
        {
            _restfulConfiguration = Configuration as RestfulConfiguration;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
             null : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
        }

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {

            quotation.VehiclePlateText1 = CarPlateUtils.ConvertCarPlateTextFromArabicToEnglish(quotation.VehiclePlateText1, false);
            quotation.VehiclePlateText2 = CarPlateUtils.ConvertCarPlateTextFromArabicToEnglish(quotation.VehiclePlateText2, false);
            quotation.VehiclePlateText3 = CarPlateUtils.ConvertCarPlateTextFromArabicToEnglish(quotation.VehiclePlateText3, false);
            if (quotation.VehicleEngineSizeCode == 0)
                quotation.VehicleEngineSizeCode = null;
            if (quotation.ProductTypeCode == 1)
            {
                quotation.VehicleValue = null;
            }
            if (DateTime.Now.Hour >= 23 && DateTime.Now.Minute >= 45)
            {
                quotation.PolicyEffectiveDate = quotation.PolicyEffectiveDate.AddDays(1);
            }
            return quotation;

        }

        protected override QuotationServiceRequest HandleAutoleasingQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            quotation.VehicleEngineSizeCode = 1;
            return quotation;
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
    
        public override ComprehensiveImagesOutput UploadComprehansiveImages(ComprehensiveImagesRequest request, ServiceRequestLog log)
        {
            ComprehensiveImagesOutput output = new ComprehensiveImagesOutput();
            log.ServiceURL = _restfulConfiguration.UploadImageServiceUrl;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "UploadComprehansiveImages";
            log.CompanyName = _restfulConfiguration.ProviderName;
            log.ReferenceId = request.ReferenceId;
           
            var policyProcessingQueueRequest = _policyProcessingQueueRepository.Table.Where(a => a.ReferenceId == request.ReferenceId).FirstOrDefault();
            if (policyProcessingQueueRequest != null)
            {
                policyProcessingQueueRequest.RequestID = log.RequestId;
                policyProcessingQueueRequest.CompanyName = log.CompanyName;
                policyProcessingQueueRequest.CompanyID = log.CompanyID;
                policyProcessingQueueRequest.InsuranceTypeCode = 2;
                policyProcessingQueueRequest.DriverNin = log.DriverNin;
                policyProcessingQueueRequest.VehicleId = log.VehicleId;
                policyProcessingQueueRequest.ServiceRequest = log.ServiceRequest;
            }
            try
            {
                if (request == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(request.Nin))
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "Nin Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(request.ReferenceId))
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "ReferenceId Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "attachments Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments[0] == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "request.attachments[0] Is Null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments[1] == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "request.attachments[1] Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments[2] == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "request.attachments[2] Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments[3] == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "request.attachments[3]  Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (request.Attachments[4] == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.NullInput;
                    output.ErrorDescription = "request.attachments[4]  Is Null";
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var stringPayload = JsonConvert.SerializeObject(request);
                /*for logging purpose*/
                request.Attachments[0].AttachmentFile = null;
                request.Attachments[1].AttachmentFile = null;
                request.Attachments[2].AttachmentFile = null;
                request.Attachments[3].AttachmentFile = null;
                request.Attachments[4].AttachmentFile = null;
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                if (policyProcessingQueueRequest != null)
                {
                    policyProcessingQueueRequest.ServiceRequest = log.ServiceRequest;
                }
                /*end of for logging purpose*/
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.PostAsync(_restfulConfiguration.UploadImageServiceUrl, httpContent, _accessTokenBase64, authorizationMethod: "Basic").Result;
                DateTime dtAfterCalling = DateTime.Now;
                output.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponseTimeInSeconds = output.ServiceResponseTimeInSeconds;
               
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.SchedulePolicyReturnError;
                    output.ErrorDescription = "service return an error and error is " + response;

                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    return output;
                }
                var value = response.Content.ReadAsStringAsync().Result;
                output.ServiceResponse = value;
                log.ServiceResponse = output.ServiceResponse;
                if (policyProcessingQueueRequest != null)
                {
                    policyProcessingQueueRequest.ServiceResponse = log.ServiceResponse;
                }
                var info = JsonConvert.DeserializeObject<ComprehensiveImageResponse>(value);
                if (info == null)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.SchedulePolicyDeserializeError;
                    output.ErrorDescription = "Failed to deserialize the returned value which is : " + value;
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to deserialize the returned value";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    return output;
                }
                if (info.Errors != null && info.Errors.Count > 0)
                {
                    StringBuilder servcieErrors = new StringBuilder();
                    StringBuilder servcieErrorsCodes = new StringBuilder();

                    foreach (var error in info.Errors)
                    {
                        servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                        servcieErrorsCodes.AppendLine(error.Code);
                    }

                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "PolicySchedule Service response error is : " + servcieErrors.ToString();
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = servcieErrorsCodes.ToString();
                    log.ServiceErrorDescription = servcieErrors.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (info.StatusCode!=1)
                {
                    output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "status returned not success as it returned "+info.StatusCode;
                    if (policyProcessingQueueRequest != null)
                    {
                        policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                        _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ReferenceId = info.ReferenceId;
                output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                if (policyProcessingQueueRequest != null)
                {
                    policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                }

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ComprehensiveImagesOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                if (policyProcessingQueueRequest != null)
                {
                    policyProcessingQueueRequest.ErrorDescription = output.ErrorDescription;
                    _policyProcessingQueueRepository.Update(policyProcessingQueueRequest);
                }

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return output;

            }

        }

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            if (!string.IsNullOrEmpty(policy.InsuredCity)&& string.IsNullOrEmpty(policy.InsuredCityCode))
            {
                var addressService = EngineContext.Current.Resolve<IAddressService>();
                var city = addressService.GetCityCenterByArabicName(policy.InsuredCity);//trying to get city by arabic name 
                if (city == null)//if no try to get by english name
                {
                    city = addressService.GetCityCenterByEnglishName(policy.InsuredCity);
                }
                if(city==null)
                {
                    if (policy.InsuredCity.Trim().Contains("ه"))
                        city = addressService.GetCityCenterByArabicName(policy.InsuredCity.Trim().Replace("ه", "ة"));
                    else if (city == null&&policy.InsuredCity.Trim().Contains("ة"))
                        city = addressService.GetCityCenterByEnglishName(policy.InsuredCity.Trim().Replace("ة", "ه"));
                }
                if (city != null)
                {
                    policy.InsuredCityCode = city.ELM_Code.ToString();
                }
            }
            return base.ExecutePolicyRequest( policy,  predefinedLogInfo);
         
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
    }
}
