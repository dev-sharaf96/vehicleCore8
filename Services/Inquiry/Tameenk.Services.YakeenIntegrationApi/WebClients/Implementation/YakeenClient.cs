using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegrationApi.Dto;
using Tameenk.Services.YakeenIntegrationApi.Dto.Enums;
using Tameenk.Services.YakeenIntegrationApi.Repository;
using Tameenk.Services.YakeenIntegrationApi.WebClients.Core;
using Tameenk.Services.YakeenIntegrationApi.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegrationApi.WebClients.Implementation
{
    public class YakeenClient : IYakeenClient
    {
        #region Fields
        private Yakeen4BcareClient _client;
        private readonly ILogger _logger;
        private readonly IRepository<Occupation> _occupationRepository;
        #endregion

        #region Ctor
        /// <summary>
        /// The Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="yakeen4BcareClient"></param>
        public YakeenClient(ILogger logger, Yakeen4BcareClient yakeen4BcareClient, IRepository<Occupation> occupationRepository)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            _client = yakeen4BcareClient;
            _occupationRepository = occupationRepository;
            _logger = logger;
        }
        #endregion

        #region Methods
        private void LogIntegrationTransaction(string transactionMethod, object request, object response)
        {
            var basicUrl = "https://yakeen.eserve.com.sa/Yakeen4Bcare/Yakeen4Bcare?wsdl/";
            _logger.LogIntegrationTransaction(basicUrl + transactionMethod, new JavaScriptSerializer().Serialize(request), new JavaScriptSerializer().Serialize(response));
        }

        public VehicleYakeenInfoDto GetVehicleInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            // if car is registered get by seq else get by custom
            if (request.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
            {
                var res = GetCarInfoBySequence(request, predefinedLogInfo);
                if (res.PlateTypeCode == 0)
                    res.PlateTypeCode = 11; // Change from unknown to temp
                return res;
            }
            else
            {
                return GetCarInfoByCustom(request, predefinedLogInfo);
            }
        }

        public VehiclePlateYakeenInfoDto GetVehiclePlateInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehiclePlateYakeenInfoDto res = new VehiclePlateYakeenInfoDto
            {
                Success = false
            };

            YakeenOutput yakeenResult = GetVehiclePlate(request, predefinedLogInfo);
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return res;

            return yakeenResult.VehiclePlateYakeenInfoDto;
        }




        private YakeenOutput GetVehiclePlate(VehicleYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-GetVehiclePlateInfo";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            if (string.IsNullOrEmpty(log.VehicleId))
                log.VehicleId = request.VehicleId.ToString();
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.OwnerNin.ToString();

            VehiclePlateYakeenInfoDto vehiclePlateYakeenInfoDto = new VehiclePlateYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    vehiclePlateYakeenInfoDto.Success = false;
                    vehiclePlateYakeenInfoDto.Error.Type = EErrorType.LocalError;
                    vehiclePlateYakeenInfoDto.Error.ErrorMessage = "nullable request";
                    output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                    return output;
                }
                if (request.VehicleIdTypeId != (int)VehicleIdType.SequenceNumber)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "Car is not Registered";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    vehiclePlateYakeenInfoDto.Success = false;
                    vehiclePlateYakeenInfoDto.Error.Type = EErrorType.LocalError;
                    vehiclePlateYakeenInfoDto.Error.ErrorMessage = "Car is not Registered";
                    output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                    return output;
                }

                getCarPlateInfoBySequence carPlate = new getCarPlateInfoBySequence();
                carPlate.CarPlateInfoBySequenceRequest = new carPlateInfoBySequenceRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    ownerID = request.OwnerNin,
                    sequenceNumber = (int)request.VehicleId
                };
                log.ServiceRequest = JsonConvert.SerializeObject(carPlate);
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _client.getCarPlateInfoBySequence(carPlate.CarPlateInfoBySequenceRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    vehiclePlateYakeenInfoDto.Success = false;
                    vehiclePlateYakeenInfoDto.Error.Type = EErrorType.LocalError;
                    vehiclePlateYakeenInfoDto.Error.ErrorMessage = "nullable response";
                    output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(response);

                vehiclePlateYakeenInfoDto.Success = true;
                vehiclePlateYakeenInfoDto.LogId = response.logId;
                vehiclePlateYakeenInfoDto.ChassisNumber = response.chassisNumber;
                vehiclePlateYakeenInfoDto.OwnerName = response.ownerName;
                vehiclePlateYakeenInfoDto.PlateNumber = response.plateNumber;
                vehiclePlateYakeenInfoDto.PlateText1 = response.plateText1;
                vehiclePlateYakeenInfoDto.PlateText2 = response.plateText2;
                vehiclePlateYakeenInfoDto.PlateText3 = response.plateText3;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    vehiclePlateYakeenInfoDto.Success = false;
                    vehiclePlateYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                    vehiclePlateYakeenInfoDto.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    vehiclePlateYakeenInfoDto.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                vehiclePlateYakeenInfoDto.Success = false;
                vehiclePlateYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                vehiclePlateYakeenInfoDto.Error.ErrorCode ="500";
                vehiclePlateYakeenInfoDto.Error.ErrorMessage = ex.InnerException.ToString();
               

                output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                return output;
            }
        }
        public CustomerNameYakeenInfoDto GetCustomerNameInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            // if user is citizen so get citizen else get alien 
            if (request.IsCitizen)
            {
                // get citizen name
                return GetCitizenNameInfo(request, predefinedLogInfo);
            }
            else
            {
                return GetAlienNameInfoByIqama(request, predefinedLogInfo);
            }
        }

        public CustomerIdYakeenInfoDto GetCustomerIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            if (request.IsCitizen)
                return getCitizenIdInfo(request, predefinedLogInfo);
            else
                return GetAlienInfoByIqama(request, predefinedLogInfo);
        }

        public DriverYakeenInfoDto GetDriverInfo(DriverYakeenRequestDto request, ServiceRequestLog log)
        {
            if (request.IsCitizen)
            {
                return GetCitizenDriverInfo(request, log);
            }
            else
            {
                return GetAlienDriverInfoByIqama(request, log);
            }
        }

        private VehicleYakeenInfoDto GetCarInfoBySequence(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehicleYakeenInfoDto vehicleYakeenInfoDto = new VehicleYakeenInfoDto
            {
                Success = false
            };

            YakeenOutput yakeenResult = GetCarInfoBySequenceInfo(request, predefinedLogInfo);
            return yakeenResult.Output;
           


        }



        private YakeenOutput GetCarInfoBySequenceInfo(VehicleYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCarInfoBySequence";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            if (string.IsNullOrEmpty(log.VehicleId))
                log.VehicleId = request.VehicleId.ToString();
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.OwnerNin.ToString();

            VehicleYakeenInfoDto vehicleYakeenInfoDto = new VehicleYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    vehicleYakeenInfoDto.Success = false;
                    vehicleYakeenInfoDto.Error.Type = EErrorType.LocalError;
                    vehicleYakeenInfoDto.Error.ErrorMessage = "nullable request";
                    output.Output = vehicleYakeenInfoDto;
                    return output;
                }
                getCarInfoBySequence carSequence = new getCarInfoBySequence();
                carSequence.CarInfoBySequenceRequest = new carInfoBySequenceRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    ownerID = request.OwnerNin,
                    sequenceNumber = (int)request.VehicleId
                };
                log.ServiceRequest = JsonConvert.SerializeObject(carSequence);
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _client.getCarInfoBySequence(carSequence.CarInfoBySequenceRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    vehicleYakeenInfoDto.Success = false;
                    vehicleYakeenInfoDto.Error.Type = EErrorType.LocalError;
                    vehicleYakeenInfoDto.Error.ErrorMessage = "nullable response";
                    output.Output = vehicleYakeenInfoDto;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(response);


                vehicleYakeenInfoDto.Success = true;
                vehicleYakeenInfoDto.IsRegistered = true;
                vehicleYakeenInfoDto.Cylinders = response.cylinders;
                vehicleYakeenInfoDto.LicenseExpiryDate = response.licenseExpiryDate;
                vehicleYakeenInfoDto.LogId = response.logId;
                vehicleYakeenInfoDto.MajorColor = response.majorColor;
                vehicleYakeenInfoDto.MinorColor = response.minorColor;
                vehicleYakeenInfoDto.ModelYear = response.modelYear;
                vehicleYakeenInfoDto.PlateTypeCode = response.plateTypeCode;
                vehicleYakeenInfoDto.RegisterationPlace = response.regplace;
                vehicleYakeenInfoDto.BodyCode = response.vehicleBodyCode;
                vehicleYakeenInfoDto.Weight = response.vehicleWeight;
                vehicleYakeenInfoDto.Load = response.vehicleLoad;
                vehicleYakeenInfoDto.MakerCode = response.vehicleMakerCode;
                vehicleYakeenInfoDto.ModelCode = response.vehicleModelCode;
                vehicleYakeenInfoDto.Maker = response.vehicleMaker;
                vehicleYakeenInfoDto.Model = response.vehicleModel;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = vehicleYakeenInfoDto;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    vehicleYakeenInfoDto.Success = false;
                    vehicleYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                    vehicleYakeenInfoDto.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    vehicleYakeenInfoDto.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.Output = vehicleYakeenInfoDto;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                vehicleYakeenInfoDto.Success = false;
                vehicleYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                vehicleYakeenInfoDto.Error.ErrorCode = "500";
                vehicleYakeenInfoDto.Error.ErrorMessage = ex.InnerException.ToString();
               
                output.Output = vehicleYakeenInfoDto;
                return output;
            }
            
        }



        private VehicleYakeenInfoDto GetCarInfoByCustom(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehicleYakeenInfoDto res = new VehicleYakeenInfoDto
            {
                Success = false
            };

            var yakeenResult = GetCarInfoByCustomInfo(request, predefinedLogInfo);

            return yakeenResult.Output;

           
        }
        private YakeenOutput GetCarInfoByCustomInfo(VehicleYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCarInfoByCustom";
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            if (string.IsNullOrEmpty(log.VehicleId))
                log.VehicleId = request.VehicleId.ToString();
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.OwnerNin.ToString();

            VehicleYakeenInfoDto res = new VehicleYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    res.Success = false;
                    res.Error.Type = EErrorType.LocalError;
                    res.Error.ErrorMessage = "nullable request";
                    output.Output = res;
                    return output;
                }
                getCarInfoByCustom carCustom = new getCarInfoByCustom();
                carCustom.CarInfoByCustomRequest = new carInfoByCustomRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    modelYear = request.ModelYear.Value,
                    customCardNumber = request.VehicleId.ToString()
                };
                log.ServiceRequest = JsonConvert.SerializeObject(carCustom);
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _client.getCarInfoByCustom(carCustom.CarInfoByCustomRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    res.Success = false;
                    res.Error.Type = EErrorType.LocalError;
                    res.Error.ErrorMessage = "nullable response";
                    output.Output = res;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(response);


                res.Success = true;
                res.IsRegistered = false;
                res.Cylinders = response.cylinders;
                res.LogId = response.logId;
                res.MajorColor = response.majorColor;
                res.MinorColor = response.minorColor;
                res.ModelYear = response.modelYear;
                res.BodyCode = response.vehicleBodyCode;
                res.Weight = response.vehicleWeight;
                res.Load = response.vehicleLoad;
                res.MakerCode = response.vehicleMakerCode;
                res.ModelCode = response.vehicleModelCode;
                res.Maker = response.vehicleMaker;
                res.Model = response.vehicleModel;
                res.PlateTypeCode = null;
                res.ChassisNumber = response.chassisNumber;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = res;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = EErrorType.YakeenError;
                    res.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.Output = res;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                res.Error.Type = EErrorType.YakeenError;
                res.Error.ErrorCode = "500";
                res.Error.ErrorMessage = ex.InnerException.ToString();
              
                output.Output = res;
                return output;
            }
        }

        private CustomerNameYakeenInfoDto GetCitizenNameInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            CustomerNameYakeenInfoDto result = new CustomerNameYakeenInfoDto
            {
                Success = false
            };

            var yakeenResult = GetCitizenName(request, predefinedLogInfo);
            return yakeenResult.CustomerYakeenInfo;
        }



        private YakeenOutput GetCitizenName(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCitizenNameInfo";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;

            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            CustomerNameYakeenInfoDto result = new CustomerNameYakeenInfoDto
            {
                Success = false
            };

            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.CustomerYakeenInfo = result;

                    return output;
                }
                getCitizenNameInfo citizenName = new getCitizenNameInfo();
                citizenName.CitizenNameInfoRequest = new citizenNameInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };
                log.ServiceRequest = JsonConvert.SerializeObject(citizenName);
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _client.getCitizenNameInfo(citizenName.CitizenNameInfoRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.CustomerYakeenInfo = result;
                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(response);

                result.Success = true;
                result.IsCitizen = true;
                result.LogId = response.logId;
                result.FirstName = response.firstName;
                result.SecondName = response.fatherName;
                result.ThirdName = response.grandFatherName;
                result.LastName = response.familyName;
                result.EnglishFirstName = response.englishFirstName;
                result.EnglishSecondName = response.englishSecondName;
                result.EnglishThirdName = response.englishThirdName;
                result.EnglishLastName = response.englishLastName;
                result.SubtribeName = response.subtribeName;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.CustomerYakeenInfo = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                // _logger.Log($"RestfulInsuranceProvider -> ExecuteQuotationRequest - (Provider name: {Configuration.ProviderName})", ex, LogLevel.Error);
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerYakeenInfo = result;
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();
                output.CustomerYakeenInfo = result;
             
                return output;
            }
        }





        private CustomerNameYakeenInfoDto GetAlienNameInfoByIqama(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            CustomerNameYakeenInfoDto result = new CustomerNameYakeenInfoDto
            {
                Success = false
            };
            var yakeenResult = GetAlienNameInfoByIqamaInfo(request, log);
            return yakeenResult.CustomerYakeenInfo;

        }



        private YakeenOutput GetAlienNameInfoByIqamaInfo(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getAlienNameInfoByIqama";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            CustomerNameYakeenInfoDto result = new CustomerNameYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.CustomerYakeenInfo = result;

                    return output;
                }
                getAlienNameInfoByIqama alienName = new getAlienNameInfoByIqama();
                alienName.AlienNameInfoByIqamaRequest = new alienNameInfoByIqamaRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    iqamaNumber = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };
                log.ServiceRequest = JsonConvert.SerializeObject(alienName);
                DateTime dtBeforeCalling = DateTime.Now;
                var alianNameInfo = _client.getAlienNameInfoByIqama(alienName.AlienNameInfoByIqamaRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (alianNameInfo == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.CustomerYakeenInfo = result;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(alianNameInfo);

                result.Success = true;
                result.IsCitizen = false;
                result.LogId = alianNameInfo.logId;
                result.FirstName = alianNameInfo.firstName;
                result.SecondName = alianNameInfo.secondName;
                result.ThirdName = alianNameInfo.thirdName;
                result.LastName = alianNameInfo.lastName;
                result.EnglishFirstName = alianNameInfo.englishFirstName;
                result.EnglishSecondName = alianNameInfo.englishSecondName;
                result.EnglishThirdName = alianNameInfo.englishThirdName;
                result.EnglishLastName = alianNameInfo.englishLastName;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.CustomerYakeenInfo = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                }
                output.CustomerYakeenInfo = result;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();

                output.CustomerYakeenInfo = result;
                return output;
            }
        }
        private CustomerIdYakeenInfoDto getCitizenIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };
            var yakeenResult = GetCitizenIdInfo(request, log);
            return yakeenResult.CustomerIdYakeenInfoDto;
        }

        private YakeenOutput GetCitizenIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {

            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCitizenIDInfo";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;

            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.CustomerIdYakeenInfoDto = result;

                    return output;
                }
                getCitizenIDInfo citizenId = new getCitizenIDInfo();
                citizenId.CitizenIDInfoRequest = new citizenIDInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };
                log.ServiceRequest = JsonConvert.SerializeObject(citizenId);
                DateTime dtBeforeCalling = DateTime.Now;
                var citizenIdInfo = _client.getCitizenIDInfo(citizenId.CitizenIDInfoRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (citizenIdInfo == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.CustomerIdYakeenInfoDto = result;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(citizenIdInfo);

                result.Success = true;
                result.IsCitizen = true;
                result.FirstName = citizenIdInfo.firstName;
                result.SecondName = citizenIdInfo.fatherName;
                result.ThirdName = citizenIdInfo.grandFatherName;
                result.LastName = citizenIdInfo.familyName;
                result.EnglishFirstName = citizenIdInfo.englishFirstName;
                result.EnglishSecondName = citizenIdInfo.englishSecondName;
                result.EnglishThirdName = citizenIdInfo.englishThirdName;
                result.EnglishLastName = citizenIdInfo.englishLastName;
                result.SubtribeName = citizenIdInfo.subtribeName;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(citizenIdInfo.gender);
                result.LogId = citizenIdInfo.logId;
               if(!string.IsNullOrEmpty(citizenIdInfo.dateOfBirthG))
                    result.DateOfBirthG = DateTime.ParseExact(citizenIdInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = citizenIdInfo.dateOfBirthH;
                result.IdIssuePlace = citizenIdInfo.idIssuePlace;
                result.IdExpiryDate = citizenIdInfo.idExpiryDate;
                result.NationalityCode = RepositoryConstants.SaudiNationalityCode;
                result.SocialStatus = citizenIdInfo.socialStatusDetailedDesc;
                result.OccupationCode = citizenIdInfo.occupationCode;
                result.licenseListListField = citizenIdInfo.licenseListList;

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.CustomerIdYakeenInfoDto = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerIdYakeenInfoDto = result;
                }

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();
                output.CustomerIdYakeenInfoDto = result;
                return output;
            }
        }



        private CustomerIdYakeenInfoDto GetAlienInfoByIqama(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            var occupations = _occupationRepository.TableNoTracking.ToList();
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };

            var yakeenResult = GetAlienInfoByIqamaInfo(request, log);
            return yakeenResult.CustomerIdYakeenInfoDto;
        }



        private YakeenOutput GetAlienInfoByIqamaInfo(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getAlienInfoByIqama";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;

            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.CustomerIdYakeenInfoDto = result;

                    return output;
                }
                getAlienInfoByIqama alienId = new getAlienInfoByIqama();
                alienId.AlienInfoByIqamaRequest = new alienInfoByIqamaRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    iqamaNumber = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };
                log.ServiceRequest = JsonConvert.SerializeObject(alienId);
                DateTime dtBeforeCalling = DateTime.Now;
                var alianInfo = _client.getAlienInfoByIqama(alienId.AlienInfoByIqamaRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (alianInfo == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.CustomerIdYakeenInfoDto = result;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(alianInfo);

                var occupations = _occupationRepository.TableNoTracking.ToList();

                result.Success = true;
                result.IsCitizen = false;

                result.FirstName = alianInfo.firstName;
                result.SecondName = alianInfo.secondName;
                result.ThirdName = alianInfo.thirdName;
                result.LastName = alianInfo.lastName;
                result.EnglishFirstName = alianInfo.englishFirstName;
                result.EnglishSecondName = alianInfo.englishSecondName;
                result.EnglishThirdName = alianInfo.englishThirdName;
                result.EnglishLastName = alianInfo.englishLastName;
                //result.SubtribeName = alianInfo.subtribeName;

                result.NationalityCode = alianInfo.nationalityCode;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(alianInfo.gender);
                result.LogId = alianInfo.logId;
                if (!string.IsNullOrEmpty(alianInfo.dateOfBirthG))
                    result.DateOfBirthG = DateTime.ParseExact(alianInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = alianInfo.dateOfBirthH;
                result.IdIssuePlace = alianInfo.iqamaIssuePlaceDesc;
                result.IdExpiryDate = alianInfo.iqamaExpiryDateH;
                result.SocialStatus = alianInfo.socialStatus;
                result.OccupationCode = occupations.FirstOrDefault(x => x.NameAr.Trim() == alianInfo.occupationDesc || x.NameEn.Trim().ToUpper() == alianInfo.occupationDesc.ToUpper())?.Code;
                if (string.IsNullOrEmpty(result.OccupationCode))
                    result.OccupationCode = occupations.FirstOrDefault(x => x.NameAr.Trim().Contains(alianInfo.occupationDesc) || x.NameEn.Trim().ToUpper().Contains(alianInfo.occupationDesc.ToUpper()))?.Code;
                var licenseListListField = new List<licenseList>();
                if (alianInfo.licensesListList != null)
                {
                    foreach (var item in alianInfo.licensesListList)
                    {
                        licenseListListField.Add(new licenseList()
                        {
                            licnsTypeDesc = item.licnsTypeDesc,
                            licssExpiryDateH = item.licssExpiryDateH,
                            licssIssueDate = item.licssIssueDate
                        });
                    }
                    result.licenseListListField = licenseListListField.ToArray();
                }

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.CustomerIdYakeenInfoDto = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerIdYakeenInfoDto = result;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();
                output.CustomerIdYakeenInfoDto = result;
                return output;
            }
        }







        private DriverYakeenInfoDto GetCitizenDriverInfo(DriverYakeenRequestDto request, ServiceRequestLog log)
        {
            DriverYakeenInfoDto result = new DriverYakeenInfoDto
            {
                Success = false
            };
            var yakeenResult = GetCitizenDriverInformation(request, log);
            return yakeenResult.DriverYakeenInfoDto;
        }


        private YakeenOutput GetCitizenDriverInformation(DriverYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCitizenDriverInfo";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            DriverYakeenInfoDto result = new DriverYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.DriverYakeenInfoDto = result;

                    return output;
                }
                getCitizenDriverInfo citizenId = new getCitizenDriverInfo();
                citizenId.CitizenDriverInfoRequest = new citizenDriverInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    licExpiryDate = request.LicenseExpiryDate
                };
                log.ServiceRequest = JsonConvert.SerializeObject(citizenId);
                DateTime dtBeforeCalling = DateTime.Now;
                var driverInfo = _client.getCitizenDriverInfo(citizenId.CitizenDriverInfoRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (driverInfo == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.DriverYakeenInfoDto = result;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(driverInfo);

                result.Success = true;
                result.IsCitizen = true;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(driverInfo.gender);
                result.FirstName = driverInfo.firstName;
                result.SecondName = driverInfo.fatherName;
                result.ThirdName = driverInfo.grandFatherName;
                result.LastName = driverInfo.familyName;
                result.SubtribeName = driverInfo.subtribeName;
                result.EnglishFirstName = driverInfo.englishFirstName;
                result.EnglishSecondName = driverInfo.englishSecondName;
                result.EnglishThirdName = driverInfo.englishThirdName;
                result.EnglishLastName = driverInfo.englishLastName;
                if (!string.IsNullOrEmpty(driverInfo.dateOfBirthG))
                    result.DateOfBirthG = DateTime.ParseExact(driverInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = driverInfo.dateOfBirthH;
                result.NationalityCode = RepositoryConstants.SaudiNationalityCode;

                foreach (var lic in driverInfo.licenseListList)
                {
                    result.Licenses.Add(new DriverLicenseYakeenInfoDto
                    {
                        TypeDesc = lic.licnsTypeCode,
                        ExpiryDateH = lic.licssExpiryDateH
                    });
                }

                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.DriverYakeenInfoDto = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.DriverYakeenInfoDto = result;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();
                output.DriverYakeenInfoDto = result;
                

                return output;
            }
        }



        private DriverYakeenInfoDto GetAlienDriverInfoByIqama(DriverYakeenRequestDto request, ServiceRequestLog log)
        {
            DriverYakeenInfoDto result = new DriverYakeenInfoDto
            {
                Success = false
            };

            var yakeenResult = GetAlienDriverInfoByIqamaInfo(request, log);
            return yakeenResult.DriverYakeenInfoDto;

        }

        private YakeenOutput GetAlienDriverInfoByIqamaInfo(DriverYakeenRequestDto request, ServiceRequestLog log)
        {
            YakeenOutput output = new YakeenOutput();
            log.Channel = "Portal";
            log.ServiceRequest = JsonConvert.SerializeObject(request);
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getAlienDriverInfoByIqama";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            //log.CompanyID = companyID;
            //log.CompanyName = companyName;
            if (string.IsNullOrEmpty(log.DriverNin))
                log.DriverNin = request.Nin.ToString();
            DriverYakeenInfoDto result = new DriverYakeenInfoDto
            {
                Success = false
            };
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable request";
                    output.DriverYakeenInfoDto = result;

                    return output;
                }
                getCitizenDriverInfo citizenId = new getCitizenDriverInfo();
                getAlienDriverInfoByIqama alienId = new getAlienDriverInfoByIqama();
                alienId.AlienDriverInfoByIqamaRequest = new alienDriverInfoByIqamaRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    iqamaNumber = request.Nin.ToString(),
                    licExpiryDate = request.LicenseExpiryDate
                };
                log.ServiceRequest = JsonConvert.SerializeObject(citizenId);
                DateTime dtBeforeCalling = DateTime.Now;
                var driverInfo = _client.getAlienDriverInfoByIqama(alienId.AlienDriverInfoByIqamaRequest);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (driverInfo == null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    result.Success = false;
                    result.Error.Type = EErrorType.LocalError;
                    result.Error.ErrorMessage = "nullable response";
                    output.DriverYakeenInfoDto = result;

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(driverInfo);

                result.Success = true;
                result.IsCitizen = false;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(driverInfo.gender);
                result.NationalityCode = driverInfo.nationalityCode;
                result.FirstName = driverInfo.firstName;
                result.SecondName = driverInfo.secondName;
                result.ThirdName = driverInfo.thirdName;
                result.LastName = driverInfo.lastName;
                result.EnglishFirstName = driverInfo.englishFirstName;
                result.EnglishSecondName = driverInfo.englishSecondName;
                result.EnglishThirdName = driverInfo.englishThirdName;
                result.EnglishLastName = driverInfo.englishLastName;
                if(!string.IsNullOrEmpty(driverInfo.dateOfBirthG))
                    result.DateOfBirthG = DateTime.ParseExact(driverInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = driverInfo.dateOfBirthH;

                if (driverInfo.licensesListList != null)
                {
                    foreach (var lic in driverInfo.licensesListList)
                    {
                        result.Licenses.Add(new DriverLicenseYakeenInfoDto
                        {
                            TypeDesc = lic.licnsTypeCode,
                            ExpiryDateH = lic.licssExpiryDateH
                        });
                    }
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.DriverYakeenInfoDto = result;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.DriverYakeenInfoDto = result;
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                result.Success = false;
                result.Error.Type = EErrorType.YakeenError;
                result.Error.ErrorCode = "500";
                result.Error.ErrorMessage = ex.InnerException.ToString();
                output.DriverYakeenInfoDto = result;
                return output;
            }
        }


        private EGender convertYakeenGenderEnumToTameenkGenderEnum(gender gender)
        {
            switch (gender)
            {
                case gender.M:
                    return EGender.M;
                case gender.F:
                    return EGender.F;
                case gender.U:
                    return EGender.U;
            }
            return EGender.U;
        }
        #endregion
    }

}