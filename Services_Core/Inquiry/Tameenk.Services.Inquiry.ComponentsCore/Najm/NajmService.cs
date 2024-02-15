using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Inquiry.Components.NajmInsuranceInquiryService;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Inquiry.Components
{
    public class NajmService : INajmService
    {
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<NajmResponseEntity> najmResponseRepository;

        public NajmService(TameenkConfig config, ILogger logger, IRepository<NajmResponseEntity> NajmResponse)
        {
            _logger = logger;
            _config = config;
            najmResponseRepository = NajmResponse;
        }

        /// <summary>
        /// Gets the najm service Api...
        /// Najm for Insurance Services Company has created an efficient platform to simplify, address, and resolve accident related procedures and formalities. Since its inception in 2007, this Saudi company has committed itself to providing hassle-free and smooth operations within insurance companies. Headquartered in Riyadh, Najm operates according to the regulations set by the law of Saudi Arabia, Ministry of Interior, and Saudi Arabian Monetary Agency (SAMA).
        /// </summary>
        /// <param name="request">NajmRequestMessage</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for getNajm
        public NajmResponse GetNajm(NajmRequest request, ServiceRequestLog predefinedLogInfo)
        {
            var najmResponse = GetNajmResponse(request, predefinedLogInfo);
            if (najmResponse.ErrorCode == NajmOutput.ErrorCodes.NullResponse|| najmResponse.ErrorCode == NajmOutput.ErrorCodes.ServiceException)
                return null;
            return najmResponse.Output;

        }
        protected NajmOutput GetNajmResponse(NajmRequest request, ServiceRequestLog log)
        {
            NajmOutput output = new NajmOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Najm";
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            try
            {
                NajmBcareService.NCDServiceClient serviceClient = new NajmBcareService.NCDServiceClient();
                serviceClient.ClientCredentials.UserName.UserName = _config.Najm.Username;
                serviceClient.ClientCredentials.UserName.Password = _config.Najm.Password;
                log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                DateTime dtBeforeCalling = DateTime.Now;
                string responseData = serviceClient.NCDEligibility(request.PolicyHolderNin, request.IsVehicleRegistered ? 1 : 2, request.VehicleId);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = responseData;
                if (string.IsNullOrEmpty(responseData))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "responseData return null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                NajmResponse response = null;
                XmlSerializer serializer = new XmlSerializer(typeof(NajmResponse));
                using (StringReader reader = new StringReader(responseData))
                {
                    response = (NajmResponse)serializer.Deserialize(reader);
                }
                if (response == null)
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceErrorCode = response.ErrorCode;
                log.ServiceErrorDescription = response.ErrorMsg;

                if (!string.IsNullOrEmpty(response.ErrorCode) && !string.IsNullOrEmpty(response.ErrorMsg) && response.ErrorMsg.ToLower().Contains("Corporate PolicyHolder ID is NOT allowed".ToLower()))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.CorporatePolicyHolderIDIsNOTAllowed;
                    output.ErrorDescription = "Najm return error Corporate PolicyHolder ID is NOT allowed";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    NajmResponse result = new NajmResponse
                    {
                        StatusCode = (int)NajmOutput.ErrorCodes.CorporatePolicyHolderIDIsNOTAllowed,
                        ErrorMsg = response.ErrorMsg
                    };
                    output.Output = result;
                    return output;
                }
                if (!string.IsNullOrEmpty(response.ErrorCode) && !string.IsNullOrEmpty(response.ErrorMsg) && response.ErrorMsg.ToLower().Contains("Error in processing your request".ToLower()))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.ErrorInprocessingYourRequest;
                    output.ErrorDescription = "Najm return error Error in processing your request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    NajmResponse result = new NajmResponse
                    {
                        StatusCode = (int)NajmOutput.ErrorCodes.ErrorInprocessingYourRequest,
                        ErrorMsg = response.ErrorMsg
                    };
                    output.Output = result;
                    return output;
                }
                if (!string.IsNullOrEmpty(response.ErrorCode) && !string.IsNullOrEmpty(response.ErrorMsg) && response.ErrorMsg.ToLower().Contains("Invalid Credential".ToLower()))
                {
                    var najmResponse = GetNajmFromCache(request);//get from DB
                    if (najmResponse != null)
                    {
                        output.ErrorCode = NajmOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.Output = najmResponse;
                    }
                    else
                    {
                        output.ErrorCode = NajmOutput.ErrorCodes.InvalidCredential;
                        output.ErrorDescription = "Najm return error Invalid Credential";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        NajmResponse result = new NajmResponse
                        {
                            StatusCode = (int)NajmOutput.ErrorCodes.InvalidCredential,
                            ErrorMsg = response.ErrorMsg
                        };
                        output.Output = result;
                    }
                    return output;
                }
                if (!string.IsNullOrEmpty(response.ErrorCode) && !string.IsNullOrEmpty(response.ErrorMsg) && response.ErrorMsg.ToLower().Contains("Invalid Policyholder ID".ToLower()))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.InvalidPolicyholderID;
                    output.ErrorDescription = "Najm return error Invalid Policyholder ID";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    NajmResponse result = new NajmResponse
                    {
                        StatusCode = (int)NajmOutput.ErrorCodes.InvalidPolicyholderID,
                        ErrorMsg = response.ErrorMsg
                    };
                    output.Output = result;
                    return output;
                }
                if (!string.IsNullOrEmpty(response.ErrorCode) && !string.IsNullOrEmpty(response.ErrorMsg) && response.ErrorMsg.ToLower().Contains("Invalid Vehicle ID".ToLower()))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.InvalidVehicleID;
                    output.ErrorDescription = "Najm return error Invalid Vehicle ID";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    NajmResponse result = new NajmResponse
                    {
                        StatusCode = (int)NajmOutput.ErrorCodes.InvalidVehicleID,
                        ErrorMsg = response.ErrorMsg
                    };
                    output.Output = result;
                    return output;
                }

                if (!string.IsNullOrEmpty(response.ErrorCode))
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.UnspecifiedError;
                    output.ErrorDescription = "Najm return error "+ response.ErrorMsg;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    NajmResponse result = new NajmResponse
                    {
                        StatusCode = (int)NajmOutput.ErrorCodes.UnspecifiedError,
                        ErrorMsg = response.ErrorMsg
                    };
                    output.Output = result;
                    return output;
                }
                output.ErrorCode = NajmOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = response;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var najmResponse = GetNajmFromCache(request);//get from DB
                if (najmResponse != null)
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Output = najmResponse;
                }
                else
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)NajmOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var najmResponse = GetNajmFromCache(request);//get from DB
                if (najmResponse != null)
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Output = najmResponse;
                }
                else
                {
                    output.ErrorCode = NajmOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)NajmOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = NajmOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }


        public NajmDriverCaseResponse GetDriverCaseDetailV2(string driverId, int insuranceId)
        {
            var client = NajmNewServiceInit();
            var response = client.GetDriverCaseDetailV2(driverId, insuranceId);
            var result = GetNajmResponseFromXml<NajmDriverCaseResponse>(response);
            return result;
        }

        public NajmVehicleCaseResponse GetVehicleCaseDetailV2(string vehiclePlateNo, int? RegistrationType, int insuranceID)
        {
            var client = NajmNewServiceInit();
            var response = client.GetVehicleCaseDetailV2(vehiclePlateNo, RegistrationType, insuranceID);
            var result = GetNajmResponseFromXml<NajmVehicleCaseResponse>(response);
            return result;

        }

        private InsuranceInquiryClient NajmNewServiceInit()
        {
            InsuranceInquiryClient client = new InsuranceInquiryClient();
            client.ClientCredentials.UserName.UserName = _config.NajmNewService.Username;
            client.ClientCredentials.UserName.Password = _config.NajmNewService.Password;
            return client;
        }

        private T GetNajmResponseFromXml<T>(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return JsonConvert.DeserializeObject<T>(jsonText);
        }
        public NajmOutput GetDriverCaseDetailV2(NajmDriverCaseRequest request,string channel,string referenceId,string vehicleId,string externalId, Guid userId, string userName,int companyId,string companyName)
        {
            NajmOutput output = new NajmOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "GetDriverCaseDetailV2";
            log.DriverNin = request.DriverId.ToString();
            log.ReferenceId = referenceId;
            log.VehicleId = vehicleId;
            log.ExternalId = externalId;
            log.UserID = userId;
            log.UserName = userName;
            log.CompanyID = companyId;
            log.CompanyName = companyName;
            try
            {
                using (InsuranceInquiryClient serviceClient = new InsuranceInquiryClient())
                {
                    serviceClient.ClientCredentials.UserName.UserName = Utilities.GetAppSetting("NajmNewServiceUsername");
                    serviceClient.ClientCredentials.UserName.Password = Utilities.GetAppSetting("NajmNewServicePassword");
                    log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
                    DateTime dtBeforeCalling = DateTime.Now;
                    string responseData = serviceClient.GetDriverCaseDetailV2(request.DriverId, request.InsuranceId);
                    DateTime dtAfterCalling = DateTime.Now;
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    log.ServiceResponse = responseData;
                    if (string.IsNullOrEmpty(responseData))
                    {
                        output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "responseData return null or empty";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    ResponseData response = null;
                    XmlSerializer serializer = new XmlSerializer(typeof(ResponseData));
                    using (StringReader reader = new StringReader(responseData))
                    {
                        response = (ResponseData)serializer.Deserialize(reader);
                    }
                    if (response == null)
                    {
                        output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = "response return null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    log.ServiceErrorCode = response.MessageID;
                    log.ServiceErrorDescription = response.Message;

                    if (!string.IsNullOrEmpty(response.Message) && response.MessageID != "101")
                    {
                        output.ErrorCode = NajmOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "Najm return error " + response.Message;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }
                    output.ErrorCode = NajmOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.NajmDriverCaseResponse = response;
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
                output.ErrorCode = NajmOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        private NajmResponse GetNajmFromCache(NajmRequest request)
        {
            var benchmarkDate = DateTime.Now.AddDays(-45);
            var IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered);
            var entity = najmResponseRepository.Table
                                  .Where(x => x.VehicleId == request.VehicleId.ToString() && x.IsDeleted == false &&
                                   x.PolicyHolderNin == request.PolicyHolderNin.ToString() &&
                                   x.IsVehicleRegistered == IsVehicleRegistered).OrderByDescending(x=>x.CreatedAt).FirstOrDefault();

            if (entity != null && entity.CreatedAt > benchmarkDate)
            {
                NajmOutput najmResponse = new NajmOutput();
                najmResponse.Output = new NajmResponse()
                {
                    NCDReference = entity.NCDReference,
                    NCDFreeYearsText = entity.NCDFreeYears.ToString(),
                    StatusCode = (int)NajmOutput.ErrorCodes.Success
                };
                return najmResponse.Output;
            }
            return null;
        }
    }
}
