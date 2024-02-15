//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Script.Serialization;
//using System.Xml.Serialization;
//using Tameenk.Core;
//using Tameenk.Core.Configuration;
//using Tameenk.Core.Data;
//using Tameenk.Core.Domain.Entities;
//using Tameenk.Integration.Dto.Najm;
//using Tameenk.Loggin.DAL;
//using Tameenk.Services.InquiryGateway.Services.Core;
//using Tameenk.Services.Logging;

//namespace Tameenk.Services.InquiryGateway.Services.Implementation
//{
//    public class NajmService : INajmService
//    {
//        private readonly ILogger _logger;
//        private readonly IRepository<NajmResponseEntity> najmResponseRepository;
//        private readonly TameenkConfig _config;

//        public NajmService(TameenkConfig config, ILogger logger, 
//            IRepository<NajmResponseEntity> _NajmResponseRepository)
//        {
//            _logger = logger;
//            this.najmResponseRepository = _NajmResponseRepository;
//            _config = config;
//        }

//        /// <summary>
//        /// Gets the najm service Api...
//        /// Najm for Insurance Services Company has created an efficient platform to simplify, address, and resolve accident related procedures and formalities. Since its inception in 2007, this Saudi company has committed itself to providing hassle-free and smooth operations within insurance companies. Headquartered in Riyadh, Najm operates according to the regulations set by the law of Saudi Arabia, Ministry of Interior, and Saudi Arabian Monetary Agency (SAMA).
//        /// </summary>
//        /// <param name="request">NajmRequestMessage</param>
//        /// <returns></returns>
//        /// TODO Edit XML Comment Template for getNajm
//        public NajmResponse GetNajm(NajmRequest request, ServiceRequestLog predefinedLogInfo)
//        {
//            //NajmResponseEntity entity;
//            // Get only the valid request the are not expired within 3 days
//            var VehicleId = request.VehicleId.ToString();
//            var PolicyHolderNin = request.PolicyHolderNin.ToString();
//            var IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered);
//            var benchmarkDate = DateTime.Now.AddDays(-29);
//          var  entity = najmResponseRepository.Table
//                                .FirstOrDefault(x => x.VehicleId == VehicleId && x.IsDeleted == false &&
//                                 x.PolicyHolderNin == PolicyHolderNin &&
//                                 x.IsVehicleRegistered == IsVehicleRegistered
//                                );

//            if (entity != null && entity.CreatedAt > benchmarkDate)
//            {
//                NajmOutput najmResponse = new NajmOutput();
//                najmResponse.Output = new NajmResponse()
//                {
//                    NCDReference = entity.NCDReference,
//                    NCDFreeYearsText = entity.NCDFreeYears.ToString()
//                };
//              return  najmResponse.Output;
//            }
//            else
//            {
//                var najmResponse = GetNajmResponse(request, predefinedLogInfo);
//                if (najmResponse.ErrorCode != NajmOutput.ErrorCodes.Success)
//                    return null;

//                if (entity != null)
//                {
//                    entity.IsDeleted = true;
//                    najmResponseRepository.Update(entity);
//                }
//                entity = new NajmResponseEntity()
//                {
//                    IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered),
//                    VehicleId = request.VehicleId.ToString(),
//                    PolicyHolderNin = request.PolicyHolderNin.ToString(),
//                    NCDFreeYears = najmResponse.Output.NCDFreeYears.Value,
//                    NCDReference = najmResponse.Output.NCDReference
//                };
//                najmResponseRepository.Insert(entity);
//                return najmResponse.Output;
//            }
//        }

//        protected  NajmOutput GetNajmResponse(NajmRequest request, ServiceRequestLog log)
//        {
//            NajmOutput output = new NajmOutput();
//            log.Channel = "Portal";
//            log.ServiceRequest = JsonConvert.SerializeObject(request);
//            log.ServerIP = ServicesUtilities.GetServerIP();
//            log.Method = "Najm";
//            //log.CompanyID = companyID;
//            //log.CompanyName = companyName;
           
//            try
//            {
//                NajmBcareService.NCDServiceClient serviceClient = new NajmBcareService.NCDServiceClient();
//                serviceClient.ClientCredentials.UserName.UserName = _config.Najm.Username;
//                serviceClient.ClientCredentials.UserName.Password = _config.Najm.Password;
//                log.ServiceURL = serviceClient.Endpoint.ListenUri.AbsoluteUri;
//                DateTime dtBeforeCalling = DateTime.Now;
//                string responseData = serviceClient.NCDEligibility(request.PolicyHolderNin, request.IsVehicleRegistered ? 1 : 2, request.VehicleId);
//                DateTime dtAfterCalling = DateTime.Now;
//                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
//                log.ServiceResponse = responseData;
//                if (string.IsNullOrEmpty(responseData))
//                {
//                    output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
//                    output.ErrorDescription = "responseData return null or empty";
//                    log.ErrorCode = (int)output.ErrorCode;
//                    log.ErrorDescription = output.ErrorDescription;
//                    log.ServiceErrorCode = log.ErrorCode.ToString();
//                    log.ServiceErrorDescription = log.ServiceErrorDescription;
//                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
//                    return output;
//                }
//                NajmResponse response = null;
//                XmlSerializer serializer = new XmlSerializer(typeof(NajmResponse));
//                using (StringReader reader = new StringReader(responseData))
//                {
//                    response=(NajmResponse)serializer.Deserialize(reader);
//                }
//                if (response==null)
//                {
//                    output.ErrorCode = NajmOutput.ErrorCodes.NullResponse;
//                    output.ErrorDescription = "response return null";
//                    log.ErrorCode = (int)output.ErrorCode;
//                    log.ErrorDescription = output.ErrorDescription;
//                    log.ServiceErrorCode = log.ErrorCode.ToString();
//                    log.ServiceErrorDescription = log.ServiceErrorDescription;
//                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
//                    return output;
//                }
//                log.ServiceErrorCode = response.ErrorCode;
//                log.ServiceErrorDescription = response.ErrorMsg;

//                if (!string.IsNullOrEmpty(response.ErrorCode))
//                {
//                    output.ErrorCode = NajmOutput.ErrorCodes.ServiceError;
//                    output.ErrorDescription = "Najm return error";
//                    log.ErrorCode = (int)output.ErrorCode;
//                    log.ErrorDescription = output.ErrorDescription;
//                    log.ServiceErrorCode = log.ErrorCode.ToString();
//                    log.ServiceErrorDescription = log.ServiceErrorDescription;
//                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
//                    return output;
//                }
//                output.ErrorCode = NajmOutput.ErrorCodes.Success;
//                output.ErrorDescription = "Success";
//                output.Output = response;
//                log.ErrorCode = (int)output.ErrorCode;
//                log.ErrorDescription = output.ErrorDescription;
//                log.ServiceErrorCode = log.ErrorCode.ToString();
//                log.ServiceErrorDescription = log.ServiceErrorDescription;
//                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
//                return output;
//            }
//            catch (Exception ex)
//            {
//                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
//                output.ErrorCode = NajmOutput.ErrorCodes.ServiceException;
//                output.ErrorDescription = ex.GetBaseException().Message;
//                log.ErrorCode = (int)output.ErrorCode;
//                log.ErrorDescription = output.ErrorDescription;
//                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
//                return output;
//            }
//        }


//    }
//}
