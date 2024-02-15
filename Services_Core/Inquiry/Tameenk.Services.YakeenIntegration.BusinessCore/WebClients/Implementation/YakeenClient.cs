using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.Dto.Enums;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation
{
    public class YakeenClient : IYakeenClient
    {
        #region Fields
        private Yakeen4BcareClient _client;
        private readonly ILogger _logger;
        private readonly IRepository<Occupation> _occupationRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IAddressService addressService;
        #endregion

        #region Ctor
        /// <summary>
        /// The Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="yakeen4BcareClient"></param>
        public YakeenClient(ILogger logger, Yakeen4BcareClient yakeen4BcareClient, IRepository<Occupation> occupationRepository
            , IRepository<Vehicle> vehicleRepository,
            IRepository<YakeenDrivers> yakeenDriversRepository
            ,IRepository<Driver> driverRepository
            , IAddressService addressService)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            _client = yakeen4BcareClient;
            _occupationRepository = occupationRepository;
            _logger = logger;
            _vehicleRepository = vehicleRepository;
            _yakeenDriversRepository = yakeenDriversRepository;
            _driverRepository = driverRepository;
            this.addressService = addressService;
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
                var res = GetCarInfoBySequenceInfo(request, predefinedLogInfo);
                if (res.Output.PlateTypeCode == 0)
                    res.Output.PlateTypeCode = 11; // Change from unknown to temp
                return res.Output;
            }
            else
            {
                var res = GetCarInfoByCustomInfo(request, predefinedLogInfo);
                return res.Output;
            }
        }

        public VehiclePlateYakeenInfoDto GetVehiclePlateInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            YakeenOutput yakeenResult = GetVehiclePlate(request, predefinedLogInfo);
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
            {
                VehiclePlateYakeenInfoDto res = new VehiclePlateYakeenInfoDto
                {
                    Success = false
                };

                return res;
            }
            return yakeenResult.VehiclePlateYakeenInfoDto;

        }


        private YakeenOutput GetVehiclePlate(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            YakeenOutput output = new YakeenOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.UserID = predefinedLogInfo.UserID;
            log.UserName = predefinedLogInfo.UserName;
            log.Channel = predefinedLogInfo.Channel;
            log.ServerIP = predefinedLogInfo.ServerIP;
            log.RequestId = predefinedLogInfo.RequestId;
            log.DriverNin = predefinedLogInfo.DriverNin;
            log.VehicleId = predefinedLogInfo.VehicleId;
            log.ExternalId = predefinedLogInfo.ExternalId;
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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    vehiclePlateYakeenInfoDto.Success = false;
                    vehiclePlateYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                    vehiclePlateYakeenInfoDto.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    vehiclePlateYakeenInfoDto.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.VehiclePlateYakeenInfoDto = vehiclePlateYakeenInfoDto;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var vehicleInfo = GetCarPlateInfoFromCache(request);//get from cache
                if (vehicleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.VehiclePlateYakeenInfoDto = vehicleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var vehicleInfo = GetCarPlateInfoFromCache(request);//get from cache
                if (vehicleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.VehiclePlateYakeenInfoDto = vehicleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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

            YakeenOutput yakeenResult = GetCarInfoBySequenceInfo(request, predefinedLogInfo);
            return yakeenResult.Output;
        }



        public YakeenOutput GetCarInfoBySequenceInfo(VehicleYakeenRequestDto request, ServiceRequestLog log)
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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    vehicleYakeenInfoDto.Success = false;
                    vehicleYakeenInfoDto.Error.Type = EErrorType.YakeenError;
                    vehicleYakeenInfoDto.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    vehicleYakeenInfoDto.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.Output = vehicleYakeenInfoDto;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var vehicleInfo = GetCarInfoBySequenceFromCache(request);//get from cache
                if(vehicleInfo!=null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Output = vehicleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var vehicleInfo = GetCarInfoBySequenceFromCache(request);//get from cache
                if (vehicleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Output = vehicleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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

            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return res;

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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = EErrorType.YakeenError;
                    res.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.Output = res;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return result;

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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerYakeenInfo = result;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return result;

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

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.CustomerYakeenInfo = result;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        private CustomerIdYakeenInfoDto getCitizenIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            var yakeenResult = GetCitizenIdInfo(request, log);
            if (yakeenResult.ErrorCode == YakeenOutput.ErrorCodes.YakeenServiceException && yakeenResult.CustomerIdYakeenInfoDto != null && yakeenResult.CustomerIdYakeenInfoDto.Error != null)
            {
                CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
                {
                    Success = false,
                    Error = new YakeenErrorDto { ErrorMessage = yakeenResult.CustomerIdYakeenInfoDto.Error.ErrorMessage, ErrorCode = yakeenResult.CustomerIdYakeenInfoDto.Error.ErrorCode, Type = EErrorType.YakeenError }
                };
            }
            else if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
            {
                CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
                {
                    Success = false,
                    Error = new YakeenErrorDto { ErrorMessage = yakeenResult.ErrorDescription, ErrorCode = "100", Type = EErrorType.YakeenError }
                };
                return result;
            }
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
                if (!string.IsNullOrEmpty(citizenIdInfo.dateOfBirthG))
                {
                    result.DateOfBirthG = DateTime.ParseExact(citizenIdInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                }
                else
                {
                    DateTime dateOfBirth = new DateTime(1, 1, 1);
                    if (DateTime.TryParse("01-" + request.DateOfBirth, out dateOfBirth))
                    {
                        result.DateOfBirthG = dateOfBirth;
                    }
                }
                result.DateOfBirthH = citizenIdInfo.dateOfBirthH;
                result.IdIssuePlace = citizenIdInfo.idIssuePlace;
                result.IdExpiryDate = citizenIdInfo.idExpiryDate;
                result.NationalityCode = RepositoryConstants.SaudiNationalityCode;
                result.SocialStatus = citizenIdInfo.socialStatusDetailedDesc;
                result.OccupationCode = citizenIdInfo.occupationCode;
                var licenseListListField = new List<licenseList>();
                if (citizenIdInfo.licenseListList != null)
                {
                    foreach (var item in citizenIdInfo.licenseListList)
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

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerIdYakeenInfoDto = result;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;

                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var citizenleInfo = GetDriverInfoFromCache(request);//get from cache
                if (citizenleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.CustomerIdYakeenInfoDto = citizenleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var citizenleInfo = GetDriverInfoFromCache(request);//get from cache
                if (citizenleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.CustomerIdYakeenInfoDto = citizenleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }



        private CustomerIdYakeenInfoDto GetAlienInfoByIqama(CustomerYakeenRequestDto request, ServiceRequestLog log)
        {
            var yakeenResult = GetAlienInfoByIqamaInfo(request, log);
            if ((yakeenResult.ErrorCode == YakeenOutput.ErrorCodes.DateOfBirthGIsEmpty ||
                yakeenResult.ErrorCode == YakeenOutput.ErrorCodes.YakeenServiceException)
                && yakeenResult.CustomerIdYakeenInfoDto != null && yakeenResult.CustomerIdYakeenInfoDto.Error != null)
            {
                CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
                {
                    Success = false,
                    Error = new YakeenErrorDto { ErrorMessage = yakeenResult.CustomerIdYakeenInfoDto.Error.ErrorMessage, ErrorCode = yakeenResult.CustomerIdYakeenInfoDto.Error.ErrorCode, Type = EErrorType.YakeenError }
                };
            }
            else if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
            {
                CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
                {
                    Success = false,
                    Error = new YakeenErrorDto { ErrorMessage = yakeenResult.ErrorDescription, ErrorCode = "100", Type = EErrorType.YakeenError }
                };
                return result;
            }
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
                if (string.IsNullOrEmpty(alianInfo.dateOfBirthG))
                {
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = "16";
                    result.Error.ErrorMessage = "(AlienInfoByIqama) returned DateOfBirthG empty";
                    output.CustomerIdYakeenInfoDto = result;

                    output.ErrorCode = YakeenOutput.ErrorCodes.DateOfBirthGIsEmpty;
                    output.ErrorDescription = "(AlienInfoByIqama) returned DateOfBirthG empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
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
                result.DateOfBirthG = DateTime.ParseExact(alianInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = alianInfo.dateOfBirthH;
                result.IdIssuePlace = alianInfo.iqamaIssuePlaceDesc;
                result.IdExpiryDate = alianInfo.iqamaExpiryDateH;
                result.SocialStatus = alianInfo.socialStatus;
                result.OccupationDesc = alianInfo.occupationDesc;
                if (!string.IsNullOrEmpty(alianInfo.occupationDesc))
                {
                    result.OccupationCode = occupations.FirstOrDefault(x => x.NameAr.Trim() == alianInfo.occupationDesc || x.NameEn.Trim().ToUpper() == alianInfo.occupationDesc.ToUpper())?.Code;
                    if (string.IsNullOrEmpty(result.OccupationCode))
                        result.OccupationCode = occupations.FirstOrDefault(x => x.NameAr.Trim().Contains(alianInfo.occupationDesc) || x.NameEn.Trim().ToUpper().Contains(alianInfo.occupationDesc.ToUpper()))?.Code;
                }
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

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.CustomerIdYakeenInfoDto = result;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var citizenleInfo = GetDriverInfoFromCache(request);//get from cache
                if (citizenleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.CustomerIdYakeenInfoDto = citizenleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var citizenleInfo = GetDriverInfoFromCache(request);//get from cache
                if (citizenleInfo != null)
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.CustomerIdYakeenInfoDto = citizenleInfo;
                }
                else
                {
                    output.ErrorCode = YakeenOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return result;
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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.DriverYakeenInfoDto = result;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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
            if (yakeenResult.ErrorCode != YakeenOutput.ErrorCodes.Success)
                return result;
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
                result.DateOfBirthG = DateTime.ParseExact(driverInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = driverInfo.dateOfBirthH;

                if (driverInfo.licensesListList != null)
                {
                    foreach (var lic in driverInfo.licensesListList)
                    {
                        result.Licenses.Add(new DriverLicenseYakeenInfoDto
                        {
                            TypeDesc = lic.licnsTypeCode,
                            ExpiryDateH =lic.licssExpiryDateH
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
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    output.DriverYakeenInfoDto = result;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
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

        public YakeenAddressOutput GetYakeenAddress(string referenceNumber, string idNumber, string birthDate, string addressLanguageField, bool isCitizen, string channel, string vehicleId,string externalId)
        {
            if (isCitizen)
            {
                return GetCitizenAddress(referenceNumber, idNumber, birthDate, addressLanguageField, channel,vehicleId,externalId);
            }
            else
            {
                return GetAlienAddress(referenceNumber, idNumber, birthDate, addressLanguageField, channel,vehicleId,externalId);
            }
        }


        private YakeenAddressOutput GetCitizenAddress(string referenceNumber, string nin, string birthDate, string addressLanguage, string channel,string vehicleId,string externalId)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            YakeenAddressOutput output = new YakeenAddressOutput();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-getCitizenNatAddress";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            log.DriverNin = nin;
            log.VehicleId = vehicleId;
            log.ExternalId = externalId;
            try
            {
                citizenNatAddressRequest addressRequest = new citizenNatAddressRequest()
                {
                    nin = nin,
                    referenceNumber = referenceNumber,
                    birthDate = birthDate,
                    addressLanguage = addressLanguage,
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode
                };
                log.ServiceRequest = JsonConvert.SerializeObject(addressRequest);

                DateTime dtBeforeCalling = DateTime.Now;
                var citizenAddress = _client.getCitizenNatAddress(addressRequest);

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (citizenAddress == null)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(citizenAddress);
                if (citizenAddress.addressListList==null || citizenAddress.addressListList.Count()==0)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = "addressListList is null or total count is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                List<YakeenAddressResult> addressess = new List<YakeenAddressResult>();
                foreach(var address in citizenAddress.addressListList)
                {
                    YakeenAddressResult addressInfo = new YakeenAddressResult();
                    addressInfo.AdditionalNumber = address.additionalNumber;
                    addressInfo.BuildingNumber = address.buildingNumber;
                    addressInfo.City = address.city;
                    addressInfo.District = address.district;
                    addressInfo.LocationCoordinates = address.locationCoordinates;
                    addressInfo.PostCode = address.postCode;
                    addressInfo.StreetName = address.streetName;
                    addressInfo.UnitNumber = address.unitNumber;
                    addressInfo.IsPrimaryAddress = address.isPrimaryAddress;
                    addressess.Add(addressInfo);
                }
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Addresses = addressess;
                output.LogId = citizenAddress.logId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
            catch (System.ServiceModel.FaultException exp)
            {
                var msgFault = exp.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    log.ServiceErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    log.ServiceErrorDescription = errorDetail.commonErrorObject.ErrorMessage;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = exp.ToString();
                if (log.ServiceErrorCode=="4")
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = log.ServiceErrorDescription;
                }
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var addresses = GetAddressFromCache(nin);//get from cache
                if (addresses != null&& addresses.Any())
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Addresses = addresses;
                }
                else
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenAddressOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var addresses = GetAddressFromCache(nin);//get from cache
                if (addresses != null && addresses.Any())
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Addresses = addresses;
                }
                else
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenAddressOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

        }

        private YakeenAddressOutput GetAlienAddress(string referenceNumber, string iqamaNumber, string birthDate, string addressLanguage, string channel, string vehicleId,string externalId)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            YakeenAddressOutput output = new YakeenAddressOutput();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "Yakeen-GetAlienAddress";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            log.DriverNin = iqamaNumber;
            log.VehicleId = vehicleId;
            log.ExternalId = externalId;
            try
            {
                alienNatAddressRequest addressRequest = new alienNatAddressRequest()
                {
                    iqamaNumber = iqamaNumber,
                    referenceNumber = referenceNumber,
                    dateOfBirth = birthDate,
                    addressLanguage = addressLanguage,
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode
                };
                log.ServiceRequest = JsonConvert.SerializeObject(addressRequest);

                DateTime dtBeforeCalling = DateTime.Now;
                var alienAddress = _client.getAlienNatAddress(addressRequest);

                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (alienAddress == null)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(alienAddress);
                if (alienAddress.addressListList == null || alienAddress.addressListList.Count() == 0)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = "addressListList is null or total count is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                List<YakeenAddressResult> addressess = new List<YakeenAddressResult>();
                foreach (var address in alienAddress.addressListList)
                {
                    YakeenAddressResult addressInfo = new YakeenAddressResult();
                    addressInfo.AdditionalNumber = address.additionalNumber;
                    addressInfo.BuildingNumber = address.buildingNumber;
                    addressInfo.City = address.city;
                    addressInfo.District = address.district;
                    addressInfo.LocationCoordinates = address.locationCoordinates;
                    addressInfo.PostCode = address.postCode;
                    addressInfo.StreetName = address.streetName;
                    addressInfo.UnitNumber = address.unitNumber;
                    addressInfo.IsPrimaryAddress = address.isPrimaryAddress;
                    addressess.Add(addressInfo);
                }

                output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Addresses = addressess;
                output.LogId = alienAddress.logId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;

            }
            catch (System.ServiceModel.FaultException exp)
            {
                var msgFault = exp.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    log.ServiceErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    log.ServiceErrorDescription = errorDetail.commonErrorObject.ErrorMessage;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = exp.ToString();
                if (log.ServiceErrorCode == "4")
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = log.ServiceErrorDescription;
                }
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (CommunicationException exp)
            {
                var addresses = GetAddressFromCache(iqamaNumber);//get from cache
                if (addresses != null && addresses.Any())
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Addresses = addresses;
                }
                else
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.CommunicationException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenAddressOutput.ErrorCodes.CommunicationException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (TimeoutException exp)
            {
                var addresses = GetAddressFromCache(iqamaNumber);//get from cache
                if (addresses != null && addresses.Any())
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Addresses = addresses;
                }
                else
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.TimeoutException;
                    output.ErrorDescription = exp.GetBaseException().Message;
                }
                log.ErrorCode = (int)YakeenAddressOutput.ErrorCodes.TimeoutException;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public YakeenVehicleOutput CarInfoByCustomTwo(CarInfoCustomTwoDto request, ServiceRequestLog log)
        {
            YakeenVehicleOutput output = new YakeenVehicleOutput();
            log.Method = "Yakeen-getCarInfoByCustomTwo";
            log.ServiceURL = _client.Endpoint.ListenUri.AbsoluteUri;
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenVehicleOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                carInfoByCustomTwoRequest carInfo = new carInfoByCustomTwoRequest()
                {
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    password = RepositoryConstants.YakeenPassword,
                    userName = RepositoryConstants.YakeenUserName,
                    modelYear = (short)request.ModelYear,
                    customCardNumber = request.CustomCrdNumber

                };

                log.ServiceRequest = JsonConvert.SerializeObject(carInfo);
                DateTime dtBeforeCalling = DateTime.Now;
                var carInfoOutput = _client.getCarInfoByCustomTwo(carInfo);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (carInfoOutput == null)
                {
                    output.ErrorCode = YakeenVehicleOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "service response return null";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = JsonConvert.SerializeObject(carInfoOutput);
                output.result = new carInfoByCustomTwoResult();
                output.result.chassisNumber = carInfoOutput.chassisNumber;
                output.result.cylinders = carInfoOutput.cylinders;
                output.result.logId = carInfoOutput.logId;
                output.result.majorColor = carInfoOutput.majorColor;
                output.result.modelYear = carInfoOutput.modelYear;
                output.result.ownerName = carInfoOutput.ownerName;
                output.result.plateNumber = carInfoOutput.plateNumber;
                output.result.plateText1 = carInfoOutput.plateText1;
                output.result.plateText2 = carInfoOutput.plateText2;
                output.result.plateText3 = carInfoOutput.plateText3;
                output.result.plateType = carInfoOutput.plateType;
                output.result.regPlace = carInfoOutput.regPlace;
                output.result.sequenceNumber = carInfoOutput.sequenceNumber;
                output.result.vehicleCapacity = carInfoOutput.vehicleCapacity;
                output.result.vehicleMakerCode = carInfoOutput.vehicleMakerCode;
                output.result.vehicleMaker = carInfoOutput.vehicleMaker;
                output.result.vehicleModel = carInfoOutput.vehicleModel;
                output.result.vehicleModelCode = carInfoOutput.vehicleModelCode;
                output.result.vehicleWeight = carInfoOutput.vehicleWeight;

                output.ErrorCode = YakeenVehicleOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.result = carInfoOutput;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    log.ServiceErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    log.ServiceErrorDescription = errorDetail.commonErrorObject.ErrorMessage;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenVehicleOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenVehicleOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        private VehicleYakeenInfoDto GetCarInfoBySequenceFromCache(VehicleYakeenRequestDto request)
        {
            VehicleYakeenInfoDto vehicleYakeenInfoDto = new VehicleYakeenInfoDto();
            Vehicle vehicleData = _vehicleRepository.Table.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault(v => !v.IsDeleted && v.VehicleIdTypeId == request.VehicleIdTypeId && v.SequenceNumber == request.VehicleId.ToString() && v.CarOwnerNIN == request.OwnerNin.ToString());
            if (vehicleData==null)
            {
                return null;
            }
            DateTime benchmarkDate = DateTime.Now.AddYears(-1);
            if (!vehicleData.CreatedDateTime.HasValue || vehicleData.CreatedDateTime < benchmarkDate)
            {
                return null;
            }

            vehicleYakeenInfoDto.Success = true;
            vehicleYakeenInfoDto.IsRegistered = true;
            if(vehicleData.Cylinders.HasValue)
                 vehicleYakeenInfoDto.Cylinders = vehicleData.Cylinders.Value;
            vehicleYakeenInfoDto.LicenseExpiryDate = vehicleData.LicenseExpiryDate;
            //vehicleYakeenInfoDto.LogId = vehicleData.logId;
            vehicleYakeenInfoDto.MajorColor = vehicleData.MajorColor;
            vehicleYakeenInfoDto.MinorColor = vehicleData.MinorColor;
            if(vehicleData.ModelYear.HasValue)
              vehicleYakeenInfoDto.ModelYear = vehicleData.ModelYear.Value;
            vehicleYakeenInfoDto.PlateTypeCode = vehicleData.PlateTypeCode;
            vehicleYakeenInfoDto.RegisterationPlace = vehicleData.RegisterationPlace;
            vehicleYakeenInfoDto.BodyCode = vehicleData.VehicleBodyCode;
            vehicleYakeenInfoDto.Weight = vehicleData.VehicleWeight;
            vehicleYakeenInfoDto.Load = vehicleData.VehicleLoad;
            if(vehicleData.VehicleMakerCode.HasValue)
                 vehicleYakeenInfoDto.MakerCode = vehicleData.VehicleMakerCode.Value;
            int modelCode = 0;
            if (vehicleData.VehicleModelCode.HasValue && int.TryParse(vehicleData.VehicleModelCode.Value.ToString(), out modelCode))
            {
                vehicleYakeenInfoDto.ModelCode = modelCode;
            }
            vehicleYakeenInfoDto.Maker = vehicleData.VehicleMaker;
            vehicleYakeenInfoDto.Model = vehicleData.VehicleModel;

            return vehicleYakeenInfoDto;
        }
        private VehiclePlateYakeenInfoDto GetCarPlateInfoFromCache(VehicleYakeenRequestDto request)
        {
            VehiclePlateYakeenInfoDto vehiclePlateYakeenInfoDto = new VehiclePlateYakeenInfoDto();
            Vehicle vehicleData = _vehicleRepository.Table.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault(v => !v.IsDeleted && v.VehicleIdTypeId == request.VehicleIdTypeId && v.SequenceNumber == request.VehicleId.ToString() && v.CarOwnerNIN == request.OwnerNin.ToString());
            if (vehicleData == null)
            {
                return null;
            }
            DateTime benchmarkDate = DateTime.Now.AddYears(-1);
            if (!vehicleData.CreatedDateTime.HasValue || vehicleData.CreatedDateTime < benchmarkDate)
            {
                return null;
            }
            vehiclePlateYakeenInfoDto.Success = true;
            //vehiclePlateYakeenInfoDto.LogId = response.logId;
            vehiclePlateYakeenInfoDto.ChassisNumber = vehicleData.ChassisNumber;
            vehiclePlateYakeenInfoDto.OwnerName = vehicleData.CarOwnerName;
            if(vehicleData.CarPlateNumber.HasValue)
                vehiclePlateYakeenInfoDto.PlateNumber = vehicleData.CarPlateNumber.Value;
            vehiclePlateYakeenInfoDto.PlateText1 = vehicleData.CarPlateText1;
            vehiclePlateYakeenInfoDto.PlateText2 = vehicleData.CarPlateText2;
            vehiclePlateYakeenInfoDto.PlateText3 = vehicleData.CarPlateText3;

            return vehiclePlateYakeenInfoDto;
        }

        private CustomerIdYakeenInfoDto GetDriverInfoFromCache(CustomerYakeenRequestDto request)
        {
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto();
            var driverInfo = _yakeenDriversRepository.TableNoTracking.Where(d => d.NIN == request.Nin.ToString()).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (driverInfo == null)
                return null;
            DateTime benchmarkDate = DateTime.Now.AddYears(-1);
            if (!driverInfo.CreatedDate.HasValue || driverInfo.CreatedDate < benchmarkDate)
            {
                return null;
            }
            if (!driverInfo.NationalityCode.HasValue)
            {
                return null;
            }
            if (driverInfo.NIN.StartsWith("2") && driverInfo.NationalityCode == 113)
            {
                return null;
            }
            result.Success = true;
            result.IsCitizen = true;
            result.FirstName = driverInfo.FirstName;
            result.SecondName = driverInfo.SecondName;
            result.ThirdName = driverInfo.ThirdName;
            result.LastName = driverInfo.LastName;
            result.EnglishFirstName = driverInfo.EnglishFirstName;
            result.EnglishSecondName = driverInfo.EnglishSecondName;
            result.EnglishThirdName = driverInfo.EnglishThirdName;
            result.EnglishLastName = driverInfo.EnglishLastName;
            result.SubtribeName = driverInfo.SubtribeName;
            if(driverInfo.GenderId.HasValue&& driverInfo.GenderId.Value==0)
                 result.Gender = EGender.M;
            else if (driverInfo.GenderId.HasValue && driverInfo.GenderId.Value == 1)
                result.Gender = EGender.F;
            else if (driverInfo.GenderId.HasValue && driverInfo.GenderId.Value == 2)
                result.Gender = EGender.U;

            if (driverInfo.LogId.HasValue)
                result.LogId = driverInfo.LogId.Value;
            if(driverInfo.DateOfBirthG.HasValue)
                result.DateOfBirthG = driverInfo.DateOfBirthG.Value;
           
            result.DateOfBirthH = driverInfo.DateOfBirthH;
            result.IdIssuePlace = driverInfo.IdIssuePlace;
            result.IdExpiryDate = driverInfo.IdExpiryDate;
           // result.NationalityCode = RepositoryConstants.SaudiNationalityCode;
            result.NationalityCode = driverInfo.NationalityCode.Value;
            result.SocialStatus = driverInfo.SocialStatus;
            result.OccupationCode = driverInfo.OccupationCode;
            var licenseListListField = new List<licenseList>();
            if (!string.IsNullOrEmpty(driverInfo.LicenseList))
            {
                result.licenseListListField = JsonConvert.DeserializeObject<licenseList[]>(driverInfo.LicenseList);
            }
            return result;
        }
        private List<YakeenAddressResult>  GetAddressFromCache(string nin)
        {
            var mainDriverAddress = addressService.GetAllAddressesByNin(nin);
            if (mainDriverAddress == null)
            {
                return null;
            }
            var benchmarkDate = DateTime.Now.AddYears(-1);
                var addressesWithinOneYears = mainDriverAddress.Where(a => a.CreatedDate > benchmarkDate).ToList();
                if (addressesWithinOneYears == null || !addressesWithinOneYears.Any())
                {
                    return null;
                }
            List<YakeenAddressResult> addressess = new List<YakeenAddressResult>();
            foreach (var address in addressesWithinOneYears)
            {
                YakeenAddressResult addressInfo = new YakeenAddressResult();
                int additionalNumber = 0;
                int buildingNumber = 0;
                int postCode = 0;
                int unitNumber = 0;
                bool isPrimaryAddress = false;

                if (int.TryParse(address.AdditionalNumber, out additionalNumber))
                    addressInfo.AdditionalNumber = additionalNumber;

                if (int.TryParse(address.BuildingNumber, out buildingNumber))
                    addressInfo.BuildingNumber = buildingNumber;

                addressInfo.City = address.City;
                addressInfo.District = address.District;
                addressInfo.LocationCoordinates = address.ObjLatLng;
                addressInfo.StreetName = address.Street;

                if (int.TryParse(address.PostCode, out postCode))
                    addressInfo.PostCode = postCode;
                if (int.TryParse(address.UnitNumber, out unitNumber))
                    addressInfo.UnitNumber = unitNumber;

                if (bool.TryParse(address.IsPrimaryAddress, out isPrimaryAddress))
                    addressInfo.IsPrimaryAddress = isPrimaryAddress;
                addressess.Add(addressInfo);
            }

            return addressess;
        }
        public YakeenMobileVerificationOutput YakeenMobileVerification(YakeenMobileVerificationDto request, string Language)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.DriverNin = request.NationalId;
            YakeenMobileVerificationOutput output = new YakeenMobileVerificationOutput();
            log.Method = "Yakeen-getYakeenMobileVerification";
            Utilities.GetAppSetting("");
            string Url = "https://yakeen-lite.api.elm.sa/api/v1/person/@id/owns-mobile/@mobilenumber";
            log.ServiceURL = Url;
            string serviceRequest = Url.Replace("@mobilenumber", Utilities.ValidatePhoneNumber(request.Phone));
            serviceRequest = serviceRequest.Replace("@id", request.NationalId);
            log.ServiceRequest = serviceRequest;
            try
            {
                if (request == null)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.NullRequest;
                    output.ErrorDescription = "request is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("APP-ID", "a56a2495");
                httpClient.DefaultRequestHeaders.Add("APP-KEY", "33bfd1bf76a0502f9ec2380dd7d53012");
                httpClient.DefaultRequestHeaders.Add("SERVICE_KEY", "265532d5-6330-49b0-b784-e5ea01c9b678");
                httpClient.DefaultRequestHeaders.Add("ORGANIZATION-NUMBER", "7001903785");
                var YakeenResponse = httpClient.GetAsync(serviceRequest).Result;
                string response = YakeenResponse.Content.ReadAsStringAsync().Result;

                DateTime dtBeforeCalling = DateTime.Now;
                //var yakeenMobileVerificationResponse = response;

                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response;
                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "yakeenMobileVerificationResponse is null";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "service response return null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(response);
                MobileVerificationModel responseObject = JsonConvert.DeserializeObject<MobileVerificationModel>(responseAfterDeserialize.ToString());
                if (responseObject == null)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "responseObject is null";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to deserialize object of MobileVerificationModel";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.mobileVerificationModel = responseObject;
                int responseCode = 0;
                int.TryParse(responseObject?.code.ToString(), out responseCode);
                if (responseCode == 100)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InternalError;
                    output.ErrorDescription = responseObject.message;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = responseCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "service response Internal Error : " + responseObject.message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseCode == 101)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidId;
                    output.ErrorDescription = responseObject.message;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = responseCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "service response Invalid Id: " + responseObject.message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseCode == 102)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileNumber;
                    output.ErrorDescription = responseObject.message;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = responseCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "service response Invalid Mobile Number: " + responseObject.message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseCode == 103)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidServiceKey;
                    output.ErrorDescription = responseObject.message;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = responseCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "service response Invalid Service Key: " + responseObject.message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!responseObject.isOwner)
                {
                    output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner;
                    output.ErrorDescription = "Phone is Not belong to national id";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = responseCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (System.ServiceModel.FaultException ex)
            {

                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    log.ServiceErrorCode = errorDetail.commonErrorObject.ErrorCode.ToString();
                    log.ServiceErrorDescription = errorDetail.commonErrorObject.ErrorMessage;
                    log.ServiceResponse = JsonConvert.SerializeObject(errorDetail);
                }
                output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.YakeenServiceException;
                output.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

        }
        #endregion
    }

}