using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Core.Providers.Configuration;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using System.Linq;

namespace Tameenk.Integration.Providers.GGI
{

    internal class GGIInsuranceTPLProvider : InsuranceProvider
    {
        #region fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _tameenkConfig;
        private readonly int[] Weights = new int[5] { 21, 22, 23, 24, 25 };
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        #endregion


        #region ctor
        public GGIInsuranceTPLProvider(TameenkConfig tameenkConfig, ILogger logger, IRepository<PolicyProcessingQueue> policyProcessingQueueRepository)
             : base(new ProviderConfiguration() { ProviderName = "GGI" }, logger)
        {
            _logger = logger;
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
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


            var quoteResponse = SubmitQuotationRequest(quotation, predefinedLogInfo);
            if (quoteResponse.ErrorCode != QuotationOutput.ErrorCodes.Success)
                return null;

            return quoteResponse.Output;
        }


        protected QuotationOutput SubmitQuotationRequest(QuotationServiceRequest quotation, ServiceRequestLog log)
        {
            QuotationOutput output = new QuotationOutput();
            log.ReferenceId = quotation.ReferenceId;
            log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Quotation";
            log.CompanyName = "GGI";
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
                    string nameSpace = "Tameenk.Integration.Providers.GGI";
                    const string nameOfFileJson = ".TestData.quotationTestData.json";
                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(GGIService.quoteResponse));
                    GGIService.quoteResponse Qresponse = new GGIService.quoteResponse();
                    Qresponse.resFile = Encoding.UTF8.GetBytes(responseDataJson);
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
                using (GGIService.TameenakImplClient serviceClient = new GGIService.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;

                    quotation.VehicleWeight = Weights[quotation.VehicleWeight / 1000];
                    QuotationServiceResponse responseValue = new QuotationServiceResponse();
                    var stringPayload = JsonConvert.SerializeObject(quotation);
                    if (quotation.VehicleValue.HasValue && quotation.ProductTypeCode == 1)
                    {
                        string removal = "\"VehicleValue\":" + quotation.VehicleValue.Value + ",";
                        stringPayload = stringPayload.Replace(removal, "");
                    }
                    GGIService.quoteRequest quoteRequest = new GGIService.quoteRequest();
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.quoteFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    DateTime dtBeforeCalling = DateTime.Now;
                    GGIService.quoteResponse quoteResponse = serviceClient.Quotation(quoteRequest);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (quoteResponse == null)
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
                        output.ErrorDescription = "quoteResponse.resFile return null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }

                    var result = Encoding.UTF8.GetString(quoteResponse.resFile);
                    var respValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);
                    log.ServiceResponse = result;
                    if (respValue.Errors != null)
                    {
                        StringBuilder servcieErrors = new StringBuilder();
                        StringBuilder servcieErrorsCodes = new StringBuilder();

                        foreach (var error in respValue.Errors)
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
            string result = string.Empty;
            var stringPayload = JsonConvert.SerializeObject(request);
            var res = string.Empty;

            if (request.VehicleValue.HasValue && request.ProductTypeCode == 1)
            {
                string removal = "\"VehicleValue\":" + request.VehicleValue.Value + ",";
                stringPayload = stringPayload.Replace(removal, "");
            }

            try
            {

                //res = new JavaScriptSerializer().Serialize(response);
                //var quoteResponse = JsonConvert.DeserializeObject<GGIService.quoteResponse>(res);
                var quoteResponse = response as GGIService.quoteResponse;//.resFile; //quoteResponse.resFile;

                result = Encoding.UTF8.GetString(quoteResponse.resFile);
                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(result);

                if (responseValue != null && responseValue.Products != null)
                {
                    foreach (var product in responseValue.Products)
                    {
                        if (product != null && product.Benefits != null)
                        {
                            foreach (var benefit in product.Benefits)
                            {
                                if (benefit.BenefitCode == 1 || benefit.BenefitCode == 5)
                                {
                                    benefit.IsReadOnly = true;
                                    benefit.IsSelected = true;
                                    benefit.BenefitPrice = 0;
                                }
                            }
                        }
                    }
                }



                HandleFinalProductPrice(responseValue);



                //if (responseValue.Errors != null)
                //    responseValue.Errors.Insert(0, new Error { Message = stringPayload });
            }
            catch (Exception ex)
            {
                _logger.Log("GGIInsuranceProvider -> GetQuotationResponseObject", ex, LogLevel.Error);
                responseValue.StatusCode = 2;
                if (responseValue.Errors == null)
                    responseValue.Errors = new List<Error>();
                //responseValue.Errors.Insert(0, new Error { Message = stringPayload });
                responseValue.Errors.Add(new Error { Message = ex.GetBaseException().Message });
            }
            finally
            {
                LogIntegrationTransaction($"Test Get Quotation with reference id: {request.ReferenceId} for company: GGI", stringPayload, responseValue, responseValue?.StatusCode);
            }

            return responseValue;
        }


        protected override object ExecutePolicyRequest(PolicyRequest policy, ServiceRequestLog predefinedLogInfo)
        {
            //try
            //{


            //    // Fake repsonse Data 

            //    string nameSpace = "Tameenk.Integration.Providers.GGI";
            //    // using Tameenk config 
            //    var testMode = _tameenkConfig.Policy.TestMode;
            //    if (testMode)
            //    {
            //        const string nameOfFileXml = ".TestData.policyTestData.json";

            //        string responseData = ReadResource(nameSpace, nameOfFileXml);

            //        const string nameOfFileJson = ".TestData.policyTestData.json";

            //        string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
            //        XmlSerializer serializer = new XmlSerializer(typeof(GGIService.polResponse));

            //        GGIService.polResponse polResponse = new GGIService.polResponse();
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

            //    // End fake repsonse Data



            //    GGIService.TameenakImplClient serviceClient = new GGIService.TameenakImplClient("TameenakImplPort_GGI");
            //    GGIService.policyRequest quoteRequest = new GGIService.policyRequest();
            //    var stringPayload = JsonConvert.SerializeObject(policy);
            //    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
            //    quoteRequest.policyReqFile = plainTextBytes;
            //    GGIService.polResponse policyResponse = serviceClient.Policy(quoteRequest);
            //    return policyResponse;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("GGIInsuranceProvider -> ExecutePolicyRequest", ex, LogLevel.Error);
            //}

            //return null;
            var policyResponse = SubmitPolicyRequest(policy, predefinedLogInfo);
            if (policyResponse.ErrorCode != PolicyOutput.ErrorCodes.Success)
                return null;

            return policyResponse.Output;
        }



        protected PolicyOutput SubmitPolicyRequest(PolicyRequest policy, ServiceRequestLog log)
        {
            PolicyOutput output = new PolicyOutput();
            log.ReferenceId = policy.ReferenceId;
            log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Policy";
            log.CompanyName = "GGI";

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

                    string nameSpace = "Tameenk.Integration.Providers.GGI";
                    const string nameOfFileXml = ".TestData.policyTestData.json";
                    string responseData = ReadResource(nameSpace, nameOfFileXml);
                    const string nameOfFileJson = ".TestData.policyTestData.json";
                    string responseDataJson = ReadResource(nameSpace, nameOfFileJson);
                    XmlSerializer serializer = new XmlSerializer(typeof(GGIService.polResponse));

                    GGIService.polResponse polResponse = new GGIService.polResponse();
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
                using (GGIService.TameenakImplClient serviceClient = new GGIService.TameenakImplClient())
                {
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;

                    GGIService.policyRequest quoteRequest = new GGIService.policyRequest();
                    var stringPayload = JsonConvert.SerializeObject(policy);
                    var plainTextBytes = Encoding.UTF8.GetBytes(stringPayload);
                    quoteRequest.policyReqFile = plainTextBytes;
                    log.ServiceRequest = stringPayload;
                    request.ServiceRequest = log.ServiceRequest;

                    DateTime dtBeforeCalling = DateTime.Now;
                    GGIService.polResponse policyResponse = serviceClient.Policy(quoteRequest);
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
                        output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
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
                var res = new JavaScriptSerializer().Serialize(response);
                var policyResponse = JsonConvert.DeserializeObject<GGIService.polResponse>(res);
                var responce = policyResponse.policyResFile;
                result = System.Text.Encoding.UTF8.GetString(responce);
                responseValue = JsonConvert.DeserializeObject<PolicyResponse>(result);
                //responseValue.IssueCompany = Company.GGI;
                return responseValue;
            }
            catch (Exception ex)
            {
                _logger.Log("GGIInsuranceProvider -> GetPolicyResponseObject", ex, LogLevel.Error);
            }
            finally
            {
                LogIntegrationTransaction($"Test Get Policy with reference id: {request.ReferenceId} for company: GGI", JsonConvert.SerializeObject(request), result, responseValue?.StatusCode);

            }

            return null;
        }


        protected override ProviderInfoDto GetProviderInfo()
        {
            var providerInfo = new ProviderInfoDto
            {
                QuotationUrl = "https://bcarews.ggi-sa.com:7788/Tameenak-WSUAT/TechnoSys?WSDL/Quotation",
                PolicyUrl = "https://bcarews.ggi-sa.com:7788/Tameenak-WSUAT/TechnoSys?WSDL/Policy"
            };
            return providerInfo;
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
