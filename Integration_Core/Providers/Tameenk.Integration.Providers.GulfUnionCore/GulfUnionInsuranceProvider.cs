using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Linq;
namespace Tameenk.Integration.Providers.GulfUnion
{
    public class GulfUnionInsuranceProvider : InsuranceProvider
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        #endregion

        #region Ctor
        public GulfUnionInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "Gulf Union" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
        }
        #endregion

        #region methods
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {

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
            log.CompanyName = "GulfUnion";
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
                    string responseData = ReadResource("Tameenk.Integration.Providers.GulfUnion", nameOfFile);


                    XmlSerializer serializer = new XmlSerializer(typeof(ServiceReference.quoteResponse));
                    ServiceReference.quoteResponse Qresponse = new ServiceReference.quoteResponse();
                    Qresponse.resFile = Encoding.UTF8.GetBytes(responseData);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Qresponse);
                            output.Output = Qresponse;
                            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            return output;
                        }
                    }
                }
                using (ServiceReference.TameenakImplClient serviceClient = new ServiceReference.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    var stringPayload = JsonConvert.SerializeObject(quotation);
                    log.ServiceRequest = stringPayload;
                   
                    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
                    ServiceReference.quoteRequest quoteRequest = new ServiceReference.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    DateTime dtBeforeCalling = DateTime.Now;
                    ServiceReference.quoteResponse quoteResponse = serviceClient.Quotation(quoteRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                    if (quoteResponse==null)
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
                    if (quoteResponse.resFile == null)
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
                //var quoteResponse = JsonConvert.DeserializeObject<SaqrService.quoteResponse>(response.ToString());
                //var resFile = quoteResponse.resFile;
                //string result = Encoding.UTF8.GetString(resFile);

                //responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                var res = JsonConvert.SerializeObject(response);
                var quoteResponse = JsonConvert.DeserializeObject<ServiceReference.quoteResponse>(res);
                var resFile = quoteResponse.resFile;
                result = Encoding.UTF8.GetString(resFile);
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        // disable the benefits for GUI cause they have issue in it from their side
                        // when they fix it we should remove this line
                        product.Benefits = new List<BenefitDto>();
                    }
                }
                HandleFinalProductPrice(responseValue);
                if (responseValue.Errors != null)
                    responseValue.Errors.Insert(0, new Error { Message = stringPayload });
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _logger.Log("GulfUnionInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                if (responseValue != null)
                {
                    responseValue.StatusCode = 2;
                    if (responseValue.Errors == null)
                        responseValue.Errors = new List<Error>();
                    responseValue.Errors.Insert(0, new Error { Message = stringPayload });
                    responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
                }
            }
            finally{
                if (!_tameenkConfig.Policy.TestMode)
                {
                    LogIntegrationTransaction($"{Configuration.ProviderName} insuance company test get quote with reference id : {request.ReferenceId}", 
                        JsonConvert.SerializeObject(request),
                        result, 
                        responseValue.StatusCode);

                }
            }
          

            return responseValue;
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


        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //try
            //{

            //    string nameSpace = "Tameenk.Integration.Providers.GulfUnion";


            //    // using Tameenk config 
            //    var testMode = _tameenkConfig.Policy.TestMode;
            //    if (testMode)
            //    {
            //        const string nameOfFileXml = ".TestData.policyTestData.json";

            //        string responseData = ReadResource(nameSpace, nameOfFileXml);

            //        const string nameOfFileJson = ".TestData.policyTestData.json";

            //        string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
            //        XmlSerializer serializer = new XmlSerializer(typeof(ServiceReference.polResponse));

            //        ServiceReference.polResponse polResponse = new ServiceReference.polResponse();
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


            //    ServiceReference.TameenakImplClient serviceClient = new ServiceReference.TameenakImplClient("TameenakImplPort_GulfUnion");
            //    ServiceReference.policyRequest policyRequest = new ServiceReference.policyRequest();
            //    var stringPayload = JsonConvert.SerializeObject(policy);
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    policyRequest.policyReqFile = plainTextBytes;
            //    ServiceReference.polResponse policyResponse = serviceClient.Policy(policyRequest);
            //    return policyResponse;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("GulfUnionInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
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
            log.CompanyName = "GulfUnion";

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
                    const string nameOfFileJson = ".TestData.policyTestData.json";
                    string responseDataJson = ReadResource("Tameenk.Integration.Providers.GulfUnion", nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(ServiceReference.polResponse));
                    ServiceReference.polResponse polResponse = new ServiceReference.polResponse();
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
                using (ServiceReference.TameenakImplClient serviceClient = new ServiceReference.TameenakImplClient("TameenakImplPort_GulfUnion"))
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;


                    ServiceReference.policyRequest policyRequest = new ServiceReference.policyRequest();
                    var stringPayload = JsonConvert.SerializeObject(policy);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    policyRequest.policyReqFile = plainTextBytes;

                    DateTime dtBeforeCalling = DateTime.Now;
                    ServiceReference.polResponse policyResponse = serviceClient.Policy(policyRequest);
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
            string result = string.Empty;
            PolicyResponse responseValue = new PolicyResponse();
            try
            {
                var res = JsonConvert.SerializeObject(response);
                var policyResponse = JsonConvert.DeserializeObject<ServiceReference.polResponse>(res);
                var responce = policyResponse.policyResFile;
                 result = System.Text.Encoding.UTF8.GetString(responce);
                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log("GulfUnionInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                if (!_tameenkConfig.Policy.TestMode)
                {
                    LogIntegrationTransaction($"{Configuration.ProviderName} insuance company test get policy with reference id : {request.ReferenceId} ",
                        JsonConvert.SerializeObject(request),
                        result,
                        responseValue.StatusCode);

                }
            }

            return null;
        }


        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "http://213.236.57.14:5555/Tameenak-WSUAT/TechnoSys?WSDL/Quotation",
                PolicyUrl = "http://213.236.57.14:5555/Tameenak-WSUAT/TechnoSys?WSDL/Policy"
            };
            return providerInfo;
        }

        protected override object ExecuteAutoleasingQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            ServiceOutput output = SubmitAutoleasingQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;
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

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            if (!quotation.VehicleEngineSizeCode.HasValue || quotation.VehicleEngineSizeCode.Value == 0)
                quotation.VehicleEngineSizeCode = 1;

            return quotation;
        }
        #endregion
    }
}
