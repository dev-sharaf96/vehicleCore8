using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Linq;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Addresses;
using Tameenk.Resources.Quotations;

namespace Tameenk.Integration.Providers.TUIC
{
    public class TUICInsuranceProviderSoap : InsuranceProvider
    {
        #region fileds
        private readonly ILogger _logger;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        #endregion

        #region Ctor
        public TUICInsuranceProviderSoap(TameenkConfig tameenkConfig , ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "TUIC" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
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

        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            if (quotation.VehicleLoad == 0)
                quotation.VehicleLoad = 2;
            QuotationOutput output = SubmitQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != QuotationOutput.ErrorCodes.Success)
            {
                return null;
            }
            return output.Output;
        }




        protected QuotationOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "TUIC";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                var testMode = _tameenkConfig.Quotatoin.TestMode;
                if (testMode)
                {
                    const string nameOfFile = ".TestData.quotationTestData.json";
                    string responseData = ReadResource("Tameenk.Integration.Providers.TUIC", nameOfFile);

                    XmlSerializer serializer = new XmlSerializer(typeof(TUICService.quoteResponse));
                    TUICService.quoteResponse Qresponse = new TUICService.quoteResponse();
                    Qresponse.resFile = Encoding.UTF8.GetBytes(responseData);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Qresponse);
                            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.Output = Qresponse;
                            return output;

                        }
                    }
                }
                var stringPayload = JsonConvert.SerializeObject(quotation);
                using (TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient("TameenakImplPort_TUIC"))
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
                    TUICService.quoteRequest quoteRequest = new TUICService.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    dtBeforeCalling = DateTime.Now;
                    TUICService.quoteResponse quoteResponse = serviceClient.Quotation(quoteRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (quoteResponse == null)
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "Service return null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    if (quoteResponse.resFile == null)
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.ResFileIsNull;
                        output.ErrorDescription = "ResFile is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    var result = Encoding.UTF8.GetString(quoteResponse.resFile);
                    var responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                    log.ServiceResponse = result;
                    if (responseValue.Errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in responseValue.Errors)
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

                    output.Output = quoteResponse;
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
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }




        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            var stringPayload = JsonConvert.SerializeObject(request);
            string result = "";
            try
            {
                //var quoteResponse = JsonConvert.DeserializeObject<TUICService.quoteResponse>(response.ToString());
                //var resFile = quoteResponse.resFile;
                //string result = Encoding.UTF8.GetString(resFile);

                //responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                var res = new JavaScriptSerializer().Serialize(response);
                var quoteResponse = JsonConvert.DeserializeObject<TUICService.quoteResponse>(res);
                var resFile = quoteResponse.resFile;
                result = Encoding.UTF8.GetString(resFile);
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                if (request.ProductTypeCode == 2)
                {
                    if (responseValue != null && responseValue.Products != null)
                    {
                        foreach (var product in responseValue.Products)
                        {
                            BenefitDto Windscreen = new BenefitDto();
                            Windscreen.BenefitCode = 4;
                            Windscreen.BenefitId = "4";
                            Windscreen.BenefitNameAr = "الزجاج الأمامي والحرائق والسرقة";
                            Windscreen.BenefitNameEn = "Windscreen, fires & theft";
                            Windscreen.BenefitPrice = 0;
                            Windscreen.IsReadOnly = true;
                            Windscreen.IsSelected = true;
                            Windscreen.BenefitDescEn = "الزجاج الأمامي والحرائق والسرقة";
                            Windscreen.BenefitDescAr = "Windscreen, fires & theft";
                            product.Benefits.Add(Windscreen);
                            BenefitDto naturalDisasters = new BenefitDto();
                            naturalDisasters.BenefitCode = 3;
                            naturalDisasters.BenefitId = "3";
                            naturalDisasters.BenefitNameAr = "الاخطار الطبيعية";
                            naturalDisasters.BenefitNameEn = "Natural Disasters";
                            naturalDisasters.BenefitPrice = 0;
                            naturalDisasters.IsReadOnly = true;
                            naturalDisasters.IsSelected = true;
                            naturalDisasters.BenefitDescEn = "Natural Disasters";
                            naturalDisasters.BenefitDescAr = "الاخطار الطبيعية";
                            product.Benefits.Add(naturalDisasters);
                            BenefitDto thirdParty = new BenefitDto();
                            thirdParty.BenefitCode = 21;
                            thirdParty.BenefitId = "21";
                            thirdParty.BenefitNameAr = "مسؤولية الطرف الثالث عن الاضرار الجسدية";
                            thirdParty.BenefitNameEn = "Third Party Bodily Injury";
                            thirdParty.BenefitPrice = 0;
                            thirdParty.IsReadOnly = true;
                            thirdParty.IsSelected = true;
                            thirdParty.BenefitDescEn = "Third Party Bodily Injury";
                            thirdParty.BenefitDescAr = "مسؤولية الطرف الثالث عن الاضرار الجسدية";
                            product.Benefits.Add(thirdParty);
                        }
                    }
                }
                HandleFinalProductPrice(responseValue);
                if (responseValue.Errors != null)
                    responseValue.Errors.Insert(0, new Error { Message = stringPayload });

                
            }
            catch (Exception ex)
            {
                _logger.Log($"TUICInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();
                responseValue.Errors.Insert(0, new Error { Message = stringPayload });
                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            finally
            {
                // Insert row in the integration transaction table to track the input/output for this company, 
                // this will be helpful in case of integration testing phase
                if (!_tameenkConfig.Quotatoin.TestMode)
                {
                    LogIntegrationTransaction("TUIC Insuance Company Test Get Quotation With Reference ID : " + request.ReferenceId, stringPayload, result, responseValue.StatusCode);
                }
            }

            return responseValue;
        }

        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //try
            //{
            //    string nameSpace = "Tameenk.Integration.Providers.TUIC";


            //    // using Tameenk config 
            //    var testMode = _tameenkConfig.Policy.TestMode;
            //    if (testMode)
            //    {
            //        const string nameOfFileXml = ".TestData.policyTestData.json";

            //        string responseData = ReadResource(nameSpace, nameOfFileXml);

            //        const string nameOfFileJson = ".TestData.policyTestData.json";

            //        string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
            //        XmlSerializer serializer = new XmlSerializer(typeof(TUICService.polResponse));

            //        TUICService.polResponse polResponse = new TUICService.polResponse();
            //        polResponse.policyResFile = Encoding.UTF8.GetBytes(responseDataJson);
            //        using (var sww = new StringWriter())
            //        {
            //            using (XmlWriter writer = XmlWriter.Create(sww))
            //            {
            //                serializer.Serialize(writer, polResponse);
            //                return polResponse;

            //            }
            //        }

            //    }



            //    TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient("TameenakImplPort_TUIC");
            //    TUICService.policyRequest policyRequest = new TUICService.policyRequest();
            //    var stringPayload = JsonConvert.SerializeObject(policy);
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    policyRequest.policyReqFile = plainTextBytes;
            //    TUICService.polResponse policyResponse = serviceClient.Policy(policyRequest);
            //    return policyResponse;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log($"TUICInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
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
            log.CompanyName = "TUIC";

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

                    string nameSpace = "Tameenk.Integration.Providers.TUIC";
                    const string nameOfFileXml = ".TestData.policyTestData.json";

                    string responseData = ReadResource(nameSpace, nameOfFileXml);

                    const string nameOfFileJson = ".TestData.policyTestData.json";

                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(TUICService.polResponse));

                    TUICService.polResponse polResponse = new TUICService.polResponse();
                    polResponse.policyResFile = Encoding.UTF8.GetBytes(responseDataJson);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, polResponse);
                            output.Output = polResponse;
                            output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            return output;

                        }
                    }
                }
                using (TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient("TameenakImplPort_TUIC"))
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    TUICService.policyRequest policyRequest = new TUICService.policyRequest();
                    var stringPayload = JsonConvert.SerializeObject(policy);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;

                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    policyRequest.policyReqFile = plainTextBytes;
                    DateTime dtBeforeCalling = DateTime.Now;
                    TUICService.polResponse policyResponse = serviceClient.Policy(policyRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (policyResponse == null)
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
                  
                    if (policyResponse.policyResFile == null)
                    {
                        output.ErrorCode = PolicyOutput.ErrorCodes.policyResFileNullResponse;
                        output.ErrorDescription = "policyResFile return null";
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

                    var responce = policyResponse.policyResFile;
                    var result = System.Text.Encoding.UTF8.GetString(responce);

                   
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
                    output.Output = policyResponse;
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
            var policyRequest = JsonConvert.SerializeObject(request);
            string result = string.Empty;
            PolicyResponse responseValue = new PolicyResponse();
            try
            {
                var res = new JavaScriptSerializer().Serialize(response);

                
                var policyResponse = JsonConvert.DeserializeObject<TUICService.polResponse>(res);
                var responce = policyResponse.policyResFile;
                result = System.Text.Encoding.UTF8.GetString(responce);
                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);



                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log($"TUICInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                // Insert row in the integration transaction table to track the input/output for this company, 
                // this will be helpful in case of integration testing phase
                if (!_tameenkConfig.Quotatoin.TestMode)
                {
                    LogIntegrationTransaction("TUIC Insuance Company Test Generate Policy With Reference ID : " + request.ReferenceId,
                        policyRequest,result,responseValue.StatusCode);
                }
            }

                return null;
        }

        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "http://88.85.249.30:5555/Tameenak-WSUAT/TechnoSys?WSDL/Quotation",
                PolicyUrl = "http://88.85.249.30:5555/Tameenak-WSUAT/TechnoSys?WSDL/Policy"
            };
            return providerInfo;
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


        //public override PolicyResponse GetPolicy(PolicyRequest policy)
        //{
        //    TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient();
        //    TUICService.policyRequest quoteRequest = new TUICService.policyRequest();
        //    var stringPayload = JsonConvert.SerializeObject(policy);
        //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
        //    quoteRequest.policyReqFile = plainTextBytes;
        //    TUICService.polResponse quoteResponse = serviceClient.Policy(quoteRequest);
        //    var responce = quoteResponse.policyResFile;
        //    string result = System.Text.Encoding.UTF8.GetString(responce);
        //    PolicyResponse responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
        //    //responseValue.IssueCompany = Company.TUIC;
        //    return responseValue;
        //}

        #endregion
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
        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            if (quotation.VehicleLoad == 0)
                quotation.VehicleLoad = 2;
            QuotationOutput output = SubmitAutoleaseQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != QuotationOutput.ErrorCodes.Success)
            {
                return null;
            }
            return output.Output;
        }
        protected QuotationOutput SubmitAutoleaseQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "autoleasing";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingQuotation";
            log.CompanyName = "TUIC";
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
                    string responseData = ReadResource("Tameenk.Integration.Providers.TUIC", nameOfFile);

                    XmlSerializer serializer = new XmlSerializer(typeof(TUICService.quoteResponse));
                    TUICService.quoteResponse Qresponse = new TUICService.quoteResponse();
                    Qresponse.resFile = Encoding.UTF8.GetBytes(responseData);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Qresponse);
                            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.Output = Qresponse;
                            return output;

                        }
                    }
                }
                var stringPayload = JsonConvert.SerializeObject(quotation);
                using (TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient("TameenakImplPort_TUIC"))
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
                    TUICService.quoteRequest quoteRequest = new TUICService.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    DateTime dtBeforeCalling = DateTime.Now;
                    TUICService.quoteResponse quoteResponse = serviceClient.Quotation(quoteRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (quoteResponse == null)
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "Service return null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    if (quoteResponse.resFile == null)
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.ResFileIsNull;
                        output.ErrorDescription = "ResFile is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    var result = Encoding.UTF8.GetString(quoteResponse.resFile);
                    var responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                    log.ServiceResponse = result;
                    if (responseValue.Errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in responseValue.Errors)
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
            predefinedLogInfo.Method = "AutoleasingPolicy";
            PolicyOutput output = SubmitAutoleasingPolicyRequest(policy, predefinedLogInfo);
            if (output.ErrorCode != PolicyOutput.ErrorCodes.Success)
            {
                return null;
            }
            return output.Output;
        }
        protected PolicyOutput SubmitAutoleasingPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {
            PolicyOutput output = new PolicyOutput();
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "autoleasing";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "AutoleasingPolicy";
            log.CompanyName = "TUIC";
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

                    string nameSpace = "Tameenk.Integration.Providers.TUIC";
                    const string nameOfFileXml = ".TestData.policyTestData.json";

                    string responseData = ReadResource(nameSpace, nameOfFileXml);

                    const string nameOfFileJson = ".TestData.policyTestData.json";

                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(TUICService.polResponse));

                    TUICService.polResponse polResponse = new TUICService.polResponse();
                    polResponse.policyResFile = Encoding.UTF8.GetBytes(responseDataJson);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, polResponse);
                            output.Output = polResponse;
                            output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            return output;

                        }
                    }
                }
                using (TUICService.TameenakImplClient serviceClient = new TUICService.TameenakImplClient("TameenakImplPort_TUIC"))
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    TUICService.policyRequest policyRequest = new TUICService.policyRequest();
                    var stringPayload = JsonConvert.SerializeObject(policy);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;

                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    policyRequest.policyReqFile = plainTextBytes;
                    DateTime dtBeforeCalling = DateTime.Now;
                    TUICService.polResponse policyResponse = serviceClient.Policy(policyRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (policyResponse == null)
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

                    if (policyResponse.policyResFile == null)
                    {
                        output.ErrorCode = PolicyOutput.ErrorCodes.policyResFileNullResponse;
                        output.ErrorDescription = "policyResFile return null";
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

                    var responce = policyResponse.policyResFile;
                    var result = System.Text.Encoding.UTF8.GetString(responce);


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
                    output.Output = policyServiceResponse;
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

    }
}
