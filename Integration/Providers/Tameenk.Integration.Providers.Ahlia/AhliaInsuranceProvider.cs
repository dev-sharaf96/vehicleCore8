using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using System.Threading;

namespace Tameenk.Integration.Providers.Ahlia
{
    public class AhliaInsuranceProvider : InsuranceProvider
    {
        private readonly ILogger _logger;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        public double quotationClientTimeOut = 10;
        public AhliaInsuranceProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "Ahlia" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
           this.quotationClientTimeOut = TimeOutConfig.SetTimeOut();
        }

        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {

            //string nameSpace = "Tameenk.Integration.Providers.Ahlia";

            //// using Tameenk config 
            //var testMode = _tameenkConfig.Quotatoin.TestMode;
            //if (testMode)
            //{
            //    const string nameOfFileXml = ".TestData.quotationTestData.xml";


            //    string responseData = ReadResource(nameSpace, nameOfFileXml);
            //    const string nameOfFileJson = ".TestData.quotationTestData.json";

            //    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);




            //    XmlSerializer serializer = new XmlSerializer(typeof(AhliaService.quoteResponse));
            //    AhliaService.quoteResponse Qresponse = new AhliaService.quoteResponse();
            //    Qresponse.resFile = Encoding.UTF8.GetBytes(responseDataJson);
            //    using (var sww = new StringWriter())
            //    {
            //        using (XmlWriter writer = XmlWriter.Create(sww))
            //        {
            //            serializer.Serialize(writer, Qresponse);
            //            return Qresponse;

            //        }
            //    }





            //}


            ////QuotationServiceResponse responseValue = new QuotationServiceResponse();
            //var stringPayload = JsonConvert.SerializeObject(quotation);
            //if (quotation.VehicleValue.HasValue && quotation.ProductTypeCode == 1)
            //{
            //    string removal = "\"VehicleValue\":" + quotation.VehicleValue.Value + ",";
            //    stringPayload = stringPayload.Replace(removal, "");
            //}

            //try
            //{
            //    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
            //    AhliaService.TameenakImplClient serviceClient = new AhliaService.TameenakImplClient();
            //    AhliaService.quoteRequest quoteRequest = new AhliaService.quoteRequest();
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    quoteRequest.quoteFile = plainTextBytes;
            //    AhliaService.quoteResponse quoteResponse = serviceClient.Quotation(quoteRequest);
            //    return quoteResponse;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("AhliaInsuranceProvider -> ExecuteQuotationRequest", ex, LogLevel.Error);
            //}
            //return null;

            ServiceOutput output = SubmitQuotationRequest(quotation, predefinedLogInfo);
            if (output.ErrorCode != ServiceOutput.ErrorCodes.Success)
            {
                return null;
            }

            return output.Output;

        }

        protected ServiceOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
          

            ServiceOutput output = new ServiceOutput();
            log.ReferenceId = quotation.ReferenceId;
            if (string.IsNullOrEmpty(log.Channel))
                log.Channel = "Portal";

            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "Ahlia";
            log.VehicleMaker = quotation?.VehicleMaker;
            log.VehicleMakerCode = quotation?.VehicleMakerCode;
            log.VehicleModel = quotation?.VehicleModel;
            log.VehicleModelCode = quotation?.VehicleModelCode;
            log.VehicleModelYear = quotation?.VehicleModelYear;
            var stringPayload = string.Empty;
            
            try
            {
               
                    var testMode = _tameenkConfig.Quotatoin.TestMode;
                    if (testMode)
                    {
                    string nameSpace = "Tameenk.Integration.Providers.Ahlia";
                    const string nameOfFileXml = ".TestData.quotationTestData.xml";

                        string responseData = ReadResource(nameSpace, nameOfFileXml);
                        const string nameOfFileJson = ".TestData.quotationTestData.json";

                        string responseDataJson = ReadResource(nameSpace, nameOfFileJson);

                        XmlSerializer serializer = new XmlSerializer(typeof(AhliaService.quoteResponse));
                        AhliaService.quoteResponse Qresponse = new AhliaService.quoteResponse();
                        Qresponse.resFile = Encoding.UTF8.GetBytes(responseDataJson);
                        using (var sww = new StringWriter())
                        {
                            using (XmlWriter writer = XmlWriter.Create(sww))
                            {
                            serializer.Serialize(writer, Qresponse);
                            output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            var result = Encoding.UTF8.GetString(Qresponse.resFile);
                            var responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                            output.Output = Qresponse;
                            return output;

                            }
                        }
                    }

                //QuotationServiceResponse responseValue = new QuotationServiceResponse();
                stringPayload = JsonConvert.SerializeObject(quotation);
                if (quotation.VehicleValue.HasValue && quotation.ProductTypeCode == 1)
                {
                    string removal = "\"VehicleValue\":" + quotation.VehicleValue.Value + ",";
                    stringPayload = stringPayload.Replace(removal, "");
                }
                quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];

                using (AhliaService.TameenakImplClient serviceClient = new AhliaService.TameenakImplClient())
                {

                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
            
                    AhliaService.quoteRequest quoteRequest = new AhliaService.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    DateTime dtBeforeCalling = DateTime.Now;
                    AhliaService.quoteResponse quoteResponse = null; 
                    using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(quotationClientTimeOut)))
                    {
                        quoteResponse = serviceClient.Quotation(quoteRequest);
                    }
                    DateTime dtAfterCalling = DateTime.Now;

                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (quoteResponse == null)
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
                    if (quoteResponse.resFile == null)
                    {
                        output.ErrorCode = ServiceOutput.ErrorCodes.ResFileIsNull;
                        output.ErrorDescription = "ResFile is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    var result = Encoding.UTF8.GetString(quoteResponse.resFile);
                    log.ServiceResponse = result;
                    var responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                    if (responseValue.Errors != null)
                    { 
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in responseValue.Errors)
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
                    output.Output = quoteResponse;
                    output.ErrorCode = ServiceOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                    //return response;
                }

            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;


            }

            //  return null;
        }

        protected override QuotationServiceResponse GetQuotationResponseObject(object response, QuotationServiceRequest request)
        {
            QuotationServiceResponse responseValue = new QuotationServiceResponse();
            var stringPayload = JsonConvert.SerializeObject(request);
            string result = string.Empty;

            if (request.VehicleValue.HasValue && request.ProductTypeCode == 1)
            {
                string removal = "\"VehicleValue\":" + request.VehicleValue.Value + ",";
                stringPayload = stringPayload.Replace(removal, "");
            }

            try
            {
                //var quoteResponse = JsonConvert.DeserializeObject<AhliaService.quoteResponse>(response.ToString());
                //var resFile = quoteResponse.resFile;
                //string result = Encoding.UTF8.GetString(resFile);
                //responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                var res = new JavaScriptSerializer().Serialize(response);
                var quoteResponse = JsonConvert.DeserializeObject<AhliaService.quoteResponse>(res);
                var resFile = quoteResponse.resFile;
                 result = Encoding.UTF8.GetString(resFile);
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                HandleFinalProductPrice(responseValue);
                if (responseValue.Errors != null)
                    responseValue.Errors.Insert(0, new Error { Message = stringPayload });

               

            }
            catch (Exception ex)
            {
                _logger.Log("AhliaInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
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
                    LogIntegrationTransaction("Ahlia Insuance Company Test With Reference ID : " + request.ReferenceId, 
                        stringPayload, result, responseValue.StatusCode);
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
            log.CompanyName = "Ahlia";

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
                    string responseDataJson = ReadResource("Tameenk.Integration.Providers.Ahlia", nameOfFileJson);

                    XmlSerializer serializer = new XmlSerializer(typeof(AhliaService.polResponse));
                    AhliaService.polResponse Policyresponse = new AhliaService.polResponse();
                    Policyresponse.policyResFile = Encoding.UTF8.GetBytes(responseDataJson);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Policyresponse);
                            output.Output = Policyresponse;
                            output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            return output;
                        }
                    }
                }
                using (AhliaService.TameenakImplClient serviceClient = new AhliaService.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    AhliaService.policyRequest quoteRequest = new AhliaService.policyRequest();

                    var stringPayload = JsonConvert.SerializeObject(policy);
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.policyReqFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;
                    DateTime dtBeforeCalling = DateTime.Now;
                    AhliaService.polResponse ploicyResponse = serviceClient.Policy(quoteRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (ploicyResponse==null)
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
                    
                    if (ploicyResponse.policyResFile == null)
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

                    var responce = ploicyResponse.policyResFile;
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
                    output.Output = ploicyResponse;
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
                var stringPayload = JsonConvert.SerializeObject(request);
                var res = new JavaScriptSerializer().Serialize(response);
                var policyResponse = JsonConvert.DeserializeObject<AhliaService.polResponse>(res);
                var responce = policyResponse.policyResFile;
                result = System.Text.Encoding.UTF8.GetString(responce);

                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
                

                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log("AhliaInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                // Insert row in the integration transaction table to track the input/output for this company, 
                // this will be helpful in case of integration testing phase
                if (!_tameenkConfig.Policy.TestMode)
                {
                    LogIntegrationTransaction(
                        "Ahlia Insuance Company Test Policy With Reference ID : " + request.ReferenceId, 
                        JsonConvert.SerializeObject(request), result, responseValue.StatusCode);
                }
            }

            return null;
        }

        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "http://46.235.92.5:5555/TameenakWSProd/TameenakImplPort?WSDL/Quotation",
                PolicyUrl = "http://46.235.92.5:5555/TameenakWSProd/TameenakImplPort?WSDL/Policy"
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
    }
}
