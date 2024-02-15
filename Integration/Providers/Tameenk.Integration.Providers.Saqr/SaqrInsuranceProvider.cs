using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
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

namespace Tameenk.Integration.Providers.Saqr
{
    public class SaqrInsuranceProvider : InsuranceProvider
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        public double quotationClientTimeOut = 10;

        #endregion

        #region Ctor
        public SaqrInsuranceProvider(TameenkConfig tameenkConfig,ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "Saqr" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig;
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            this.quotationClientTimeOut = TimeOutConfig.SetTimeOut();
        }
        #endregion

        #region methods
        protected override object ExecuteQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog predefinedLogInfo)
        {
            //if (quotation.ProductTypeCode == 2)
            //{
            //    return null;
            //}
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
            log.CompanyName = "Saqr";
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
                    string nameSpace = "Tameenk.Integration.Providers.Saqr";
                    const string nameOfFileXml = ".TestData.quotationTestData.xml";
                    string responseData = ReadResource(nameSpace, nameOfFileXml);
                    const string nameOfFileJson = ".TestData.quotationTestData.json";
                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaqrService.quoteResponse));
                    SaqrService.quoteResponse Qresponse = new SaqrService.quoteResponse();
                    Qresponse.resFile = Encoding.UTF8.GetBytes(responseDataJson);
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            serializer.Serialize(writer, Qresponse);
                            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            var result = Encoding.UTF8.GetString(Qresponse.resFile);
                            var responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                            output.Output = Qresponse;
                            return output;
                        }
                    }
                }
               
                using (SaqrService.TameenakImplClient serviceClient = new SaqrService.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    var stringPayload = JsonConvert.SerializeObject(quotation);
                    SaqrService.quoteResponse quoteResponse = null;
                    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
                    SaqrService.quoteRequest quoteRequest = new SaqrService.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;

                    DateTime dtBeforeCalling = DateTime.Now;
                    using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(quotationClientTimeOut)))
                    {
                        quoteResponse = serviceClient.Quotation(quoteRequest);
                    }

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
            catch (TimeoutException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
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

                var res = new JavaScriptSerializer().Serialize(response);
                var quoteResponse = JsonConvert.DeserializeObject<SaqrService.quoteResponse>(res);
                var resFile = quoteResponse.resFile;
                result = Encoding.UTF8.GetString(resFile);
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        if (product != null && product.Benefits != null)
                        {
                            foreach (var benefit in product.Benefits)
                            {
                                if (benefit.BenefitPrice == 0)// added as per Moneera 
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                }
                            }
                        }
                    }
                }

                HandleFinalProductPrice(responseValue);
                if (responseValue.Errors != null)
                    responseValue.Errors.Insert(0, new Error { Message = stringPayload });
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _logger.Log($"SaqrInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();
                responseValue.Errors.Insert(0, new Error { Message = stringPayload });
                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            finally{
                if (!_tameenkConfig.Policy.TestMode)
                {
                    LogIntegrationTransaction("Saqar Insuance Company Test With Reference ID : " + request.ReferenceId, 
                        JsonConvert.SerializeObject(request), result, responseValue.StatusCode);
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

            //    string nameSpace = "Tameenk.Integration.Providers.Saqr";


            //    // using Tameenk config 
            //    var testMode = _tameenkConfig.Policy.TestMode;
            //    if (testMode)
            //    {
            //        const string nameOfFileXml = ".TestData.policyTestData.json";

            //        string responseData = ReadResource(nameSpace, nameOfFileXml);

            //        const string nameOfFileJson = ".TestData.policyTestData.json";

            //        string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
            //        XmlSerializer serializer = new XmlSerializer(typeof(SaqrService.polResponse));

            //        SaqrService.polResponse polResponse = new SaqrService.polResponse();
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


            //    SaqrService.TameenakImplClient serviceClient = new SaqrService.TameenakImplClient();
            //    SaqrService.policyRequest policyRequest = new SaqrService.policyRequest();
            //    var stringPayload = JsonConvert.SerializeObject(policy);
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    policyRequest.policyReqFile = plainTextBytes;
            //    SaqrService.polResponse policyResponse = serviceClient.Policy(policyRequest);
            //    return policyResponse;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log($"SaqrInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
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
            log.CompanyName = "Saqr";

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
                    string nameSpace = "Tameenk.Integration.Providers.Saqr";
                    const string nameOfFileXml = ".TestData.policyTestData.json";
                    string responseData = ReadResource(nameSpace, nameOfFileXml);
                    const string nameOfFileJson = ".TestData.policyTestData.json";
                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaqrService.polResponse));

                    SaqrService.polResponse polResponse = new SaqrService.polResponse();
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
                using (SaqrService.TameenakImplClient serviceClient = new SaqrService.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;

                    SaqrService.policyRequest policyRequest = new SaqrService.policyRequest();
                    var stringPayload = JsonConvert.SerializeObject(policy);
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    policyRequest.policyReqFile = plainTextBytes;
                    DateTime dtBeforeCalling = DateTime.Now;
                    SaqrService.polResponse policyResponse = serviceClient.Policy(policyRequest);
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
            string result = "";
            PolicyResponse responseValue = new PolicyResponse();

            var res="";
            try
            {
                res = new JavaScriptSerializer().Serialize(response);
                var policyResponse = JsonConvert.DeserializeObject<SaqrService.polResponse>(res);
                var responce = policyResponse.policyResFile;
                result = System.Text.Encoding.UTF8.GetString(responce);
                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log($"SaqrInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }

            finally
            {
                // Insert row in the integration transaction table to track the input/output for this company, 
                // this will be helpful in case of integration testing phase
                if (!_tameenkConfig.Quotatoin.TestMode)
                {
                    LogIntegrationTransaction("Saqr Insuance Company Policy Test With Reference ID : " + request.ReferenceId, 
                        JsonConvert.SerializeObject(request), result, responseValue?.StatusCode);
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

        //public override PolicyResponse GetPolicy(PolicyRequest policy)
        //{
        //    SaqrService.TameenakImplClient serviceClient = new SaqrService.TameenakImplClient();
        //    SaqrService.policyRequest quoteRequest = new SaqrService.policyRequest();
        //    var stringPayload = JsonConvert.SerializeObject(policy);
        //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
        //    quoteRequest.policyReqFile = plainTextBytes;
        //    SaqrService.polResponse quoteResponse = serviceClient.Policy(quoteRequest);
        //    var responce = quoteResponse.policyResFile;
        //    string result = System.Text.Encoding.UTF8.GetString(responce);
        //    PolicyResponse responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
        //    //responseValue.IssueCompany = Company.Saqr;
        //    return responseValue;
        //}

        protected override QuotationServiceRequest HandleQuotationRequestObjectMapping(QuotationServiceRequest quotation)
        {
            if(quotation.ProductTypeCode==1)
            {
                quotation.VehicleAgencyRepair = null;
            }
           
            return quotation;
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
    }
}
