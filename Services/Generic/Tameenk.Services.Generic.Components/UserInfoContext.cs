using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Generic.Component;
using Tameenk.Services.Generic.Components.Models;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Core.Data;
using Tameenk.Services.Generic.Components.Output;
using Tameenk.Resources.WebResources;
using Tameenk.Common.Utilities;
using Tameenk.Services.YakeenIntegration.Business.Extensions;
using Tameenk.Core;
using Newtonsoft.Json;
using Tameenk.Services.Core.Notifications;

namespace Tameenk.Services.Generic.Components
{
    public class UserInfoContext : IUserInfoContext
    {
        private readonly IYakeenClient _yakeenClient;
        private readonly ICustomerServices _customerServices;
        private readonly IGenericContext _genericContext;
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IYakeenVehicleServices _yakeenVehicleServices;
        private readonly IRepository<YakeenVehicles> _yakeenVehiclesRepository;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly INotificationService _notificationService;
        private readonly IRepository<BcareWithdrawalWinner> _bcareWithdrawalWinnerRepository;

        public UserInfoContext(IYakeenClient yakeenClient,
                               ICustomerServices yakeenIntegration,
                               IRepository<YakeenDrivers> yakeenDriversRepository,
                               IGenericContext genericContext,
                               IVehicleService vehicleService,
                               IYakeenVehicleServices yakeenVehicleServices,
                               IRepository<YakeenVehicles> yakeenVehiclesRepository,
                               IRepository<UserInfo> userInfoRepository,
                               IRepository<Driver> driverRepository,
                               IRepository<Vehicle> vehicleRepository,
                               INotificationService notificationService,
                               IRepository<BcareWithdrawalWinner> bcareWithdrawalWinnerRepository
                               )
        {
            _yakeenClient = yakeenClient;
            _customerServices = yakeenIntegration;
            _yakeenDriversRepository = yakeenDriversRepository;
            _genericContext = genericContext;
            _vehicleService = vehicleService;
            _yakeenVehicleServices = yakeenVehicleServices;
            _yakeenVehiclesRepository = yakeenVehiclesRepository;
            _userInfoRepository = userInfoRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _notificationService = notificationService;
            _bcareWithdrawalWinnerRepository = bcareWithdrawalWinnerRepository;
        }

        public UserInfoOutput AddUserInfo(UserInfoModel model,Guid userID,string userName)
        {
            var output = new UserInfoOutput();
            CompetitionRequestLog log = new CompetitionRequestLog();
            log.Channel = model.Channel.ToString();
            log.UserId = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.Method = "AdduserInfo";
            try
            {
                if (model == null)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model data id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NinIdIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                log.Nin = model.NationalId;
                if (string.IsNullOrEmpty(model.SequenceNumber))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SequenceIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sequence Number is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                log.VehicleId = model.SequenceNumber;
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("PhoneNmuberIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Phone Number is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (!Utilities.IsValidPhoneNo(model.PhoneNumber))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.InvalidPhoneNumber;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidPhoneNumber", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "incorrect phone format as phone is " + model.PhoneNumber;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                var userData= _userInfoRepository.Table.Where(g => g.NationalId == model.NationalId && g.SequenceNumber == model.SequenceNumber).FirstOrDefault();
                if (userData != null && userData.IsVerified.HasValue && userData.IsVerified.Value == true)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NinAndSequenceAlreadyExists", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "The Nid and sequence number is already registered before";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (userData != null && !userData.DriverId.HasValue)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.MissingData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("YakeenDriverData", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Driver Data is missing";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                Random rnd = new Random();
                int Otp = rnd.Next(1000, 9999);
                string smsMessage = WebResources.ResourceManager.GetString("OTPUserInfo", System.Globalization.CultureInfo.GetCultureInfo(model.Language)) + Otp.ToString();
                var smsModel = new Implementation.SMSModel()
                {
                    PhoneNumber = model.PhoneNumber,
                    MessageBody = smsMessage,
                    Method = SMSMethod.WebsiteOTP.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = model.Channel.ToString(),
                };
                if (userData != null && !userData.IsVerified.HasValue && (userData.IsVerified == null || userData.IsVerified.Value == false))
                {
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if(smsOutput.ErrorCode!=0)
                    {
                        output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to send otp due to:"+ smsOutput.ErrorDescription;
                        CompetitionDataAccess.AddToCompetition(log);
                        return output;
                    }
                    userData.OTP = Otp;
                    userData.OTPCreatedDate = DateTime.Now;
                    userData.Hashed = SecurityUtilities.HashData($"{model.NationalId}_{model.SequenceNumber}_{Otp}_{SecurityUtilities.HashKey}", null);
                    _userInfoRepository.Update(userData);

                    output.ErrorCode = UserInfoOutput.ErrorCodes.Success;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                UserInfo userInfo = new UserInfo();
                userInfo.IsActive = true;
                userInfo.NationalId = model.NationalId;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.SequenceNumber = model.SequenceNumber;
                userInfo.BirthDateMonth = model.BirthDateMonth;
                userInfo.BirthDateYear = model.BirthDateYear;
                userInfo.CreatedDateTime = DateTime.UtcNow;

                var driverInfo= _driverRepository.TableNoTracking.Where(d => d.NIN == model.NationalId && d.IsDeleted != true).OrderByDescending(I => I.CreatedDateTime).FirstOrDefault();
                if(driverInfo!=null)
                {
                    userInfo.IsDriverExist = true;
                    userInfo.DriverId = driverInfo.DriverId;
                    userInfo.BirthDateYear = driverInfo.DateOfBirthG.Year;
                    userInfo.BirthDateMonth = driverInfo.DateOfBirthG.Month;
                }
                else
                {
                    userInfo.IsDriverExist = false;
                    userInfo.DriverId = null;
                }
                var vehicleInfo  =_vehicleRepository.TableNoTracking.Where(I => I.SequenceNumber == model.SequenceNumber && I.IsDeleted != true).OrderByDescending(I => I.CreatedDateTime).FirstOrDefault();
                if (vehicleInfo == null)//get data from yakeen
                {
                    long nin = 0;
                    long sequenceNumber = 0;
                    long.TryParse(model.NationalId, out nin);
                    long.TryParse(model.SequenceNumber, out sequenceNumber);
                    Guid? vehicleId = GetVechileInfo(nin, sequenceNumber, model.Channel.ToString(), userID, userName);
                    if (!vehicleId.HasValue)
                    {
                        output.ErrorCode = UserInfoOutput.ErrorCodes.GetYakeenInfoError;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("YakeenVehicleError", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Unable to get the Vehicle data";
                        CompetitionDataAccess.AddToCompetition(log);
                        return output;
                    }
                    userInfo.VechileId = vehicleId;
                    userInfo.IsVechileExist = false;
                }
                else
                {
                    userInfo.VechileId = vehicleInfo.ID;
                    userInfo.IsVechileExist = true;
                }
                _userInfoRepository.Insert(userInfo);
              
                if (!userInfo.DriverId.HasValue)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.MissingData;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("YakeenDriverData", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Driver Data is missing";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                var smsStatus = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                if (smsStatus.ErrorCode != 0)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to send otp due to:" + smsStatus.ErrorDescription;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                userInfo.OTP = Otp;
                userInfo.OTPCreatedDate = DateTime.Now;
                userInfo.Hashed = SecurityUtilities.HashData($"{model.NationalId}_{model.SequenceNumber}_{Otp}_{SecurityUtilities.HashKey}", null);
                _userInfoRepository.Update(userInfo);

                output.ErrorCode = UserInfoOutput.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UserInfoOutput.ErrorCodes.Failed;
                output.ErrorDescription = WebResources.ResourceManager.GetString("Unknown", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
        }

        public UserInfoOutput AddUserInfoMissingFields(UserInfoModel model, Guid userID, string userName)
        {
            var output = new UserInfoOutput();
            CompetitionRequestLog log = new CompetitionRequestLog();
            log.Channel = model.Channel.ToString();
            log.UserId = userID.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.Method = "AddMissingInfo";

            try
            {
                if (model == null)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model data id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NinIdIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                log.Nin = model.NationalId;
                if (string.IsNullOrEmpty(model.SequenceNumber))
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SequenceIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sequence Number is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                log.VehicleId = model.SequenceNumber;
                if (model.BirthDateMonth == 0)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Birth month is  empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (model.BirthDateYear == 0)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Birth year is  empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                Random rnd = new Random();
                int Otp = rnd.Next(1000, 9999);
                string smsMessage = WebResources.ResourceManager.GetString("OTPUserInfo", System.Globalization.CultureInfo.GetCultureInfo(model.Language)) + Otp.ToString();
                var smsModel = new Implementation.SMSModel()
                {
                    PhoneNumber = model.PhoneNumber,
                    MessageBody = smsMessage,
                    Method = SMSMethod.WebsiteOTP.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = model.Channel.ToString(),
                };

                var userData = _userInfoRepository.Table.Where(g => g.NationalId == model.NationalId && g.SequenceNumber == model.SequenceNumber).FirstOrDefault();
                if (userData == null)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "No data exist with Nin: " + model.NationalId + " and sequence: " + model.SequenceNumber;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (userData.IsVerified.HasValue && userData.IsVerified.Value == true)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NinAndSequenceAlreadyExists", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "The Nid and sequence number is already registered before";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (userData.DriverId != null && userData.VechileId != null && (!userData.IsVerified.HasValue || userData.IsVerified.Value == false))
                {
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode != 0)
                    {
                        output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "failed to send otp due to:" + smsOutput.ErrorDescription;
                        CompetitionDataAccess.AddToCompetition(log);
                        return output;
                    }
                    userData.OTP = Otp;
                    userData.OTPCreatedDate = DateTime.Now;
                    userData.Hashed = SecurityUtilities.HashData($"{model.NationalId}_{model.SequenceNumber}_{Otp}_{SecurityUtilities.HashKey}", null);
                    _userInfoRepository.Update(userData);

                    output.ErrorCode = UserInfoOutput.ErrorCodes.Success;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                long nin;
                int birthMonth;
                int birthYear;
                long.TryParse(model.NationalId, out nin);
                int.TryParse(model.BirthDateMonth.ToString(), out birthMonth);
                int.TryParse(model.BirthDateYear.ToString(), out birthYear);

                Guid? driverId = GetDriverInfo(nin, birthMonth, birthYear, model.Channel.ToString(), userID, userName);
                if (!driverId.HasValue)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.GetYakeenInfoError;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("YakeenVehicleError", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Unable to get the driver data from yakeen";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                userData.IsVechileExist = false;
                userData.DriverId = driverId;
                userData.BirthDateYear = birthMonth;
                userData.BirthDateMonth = birthYear;

                var smsStatus = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                if (smsStatus.ErrorCode != 0)
                {
                    output.ErrorCode = UserInfoOutput.ErrorCodes.AlreadyExits;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to send otp due to:" + smsStatus.ErrorDescription;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                userData.OTP = Otp;
                userData.OTPCreatedDate = DateTime.Now;
                userData.Hashed = SecurityUtilities.HashData($"{model.NationalId}_{model.SequenceNumber}_{Otp}_{SecurityUtilities.HashKey}", null);
                _userInfoRepository.Update(userData);

                output.ErrorCode = UserInfoOutput.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UserInfoOutput.ErrorCodes.Failed;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
        }

        public VerifyOTPOutput VerifyOTP(VerifyOTPModel model, Guid userID, string userName)
        {
            var output = new VerifyOTPOutput();
            CompetitionRequestLog log = new CompetitionRequestLog();
            log.Channel = model.Channel.ToString();
            log.UserId = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.Method = "VerifyUserInfoOTP";
            try
            {
                if (model == null)
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model data id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NinIdIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "National id is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.SequenceNumber))
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SequenceIsNull", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sequence Number is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (model.otp == 0)
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.EmptyParams;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyOTP", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OTP is empty";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                var userData = _userInfoRepository.Table.Where(g => g.NationalId == model.NationalId && g.SequenceNumber == model.SequenceNumber).FirstOrDefault();
                if (userData == null)
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.NotFound;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("NoDataWithThisUserIdAndExternalId", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User Info Not Found ";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                string hashed = userData.Hashed;
                string clearText = string.Format("{0}_{1}_{2}_{3}", model.NationalId, model.SequenceNumber, model.otp, SecurityUtilities.HashKey);
                if (!SecurityUtilities.VerifyHashedData(userData.Hashed, clearText))
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.InvalidOTP;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("InvalidOTP", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "InvalidOTP";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                if (DateTime.Now > userData.OTPCreatedDate.Value.AddMinutes(15))
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.OTPExpired;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("OtpIsExpired", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OTP is expired";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                userData.IsVerified = true;
                userData.VerifiedAt = DateTime.Now;
                _userInfoRepository.Update(userData);
                if (userData == null)
                {
                    output.ErrorCode = VerifyOTPOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Unable to update user info data";
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                output.ErrorCode = VerifyOTPOutput.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("UserDataSaved", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Data has been stored successfully ";
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = VerifyOTPOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(model.Language));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
        }

        #region Private Methods

        private Guid? GetVechileInfo(long nin, long vehicleId, string channel, Guid userId, string userName)
        {
            VehicleYakeenModel vehicleYakeenModel = new VehicleYakeenModel();
            VehiclePlateYakeenInfoDto vehiclePlateInfoFromYakeen = null;
            var vehicleYakeenRequest = new VehicleYakeenRequestDto()
            {
                VehicleId = vehicleId,
                VehicleIdTypeId = 1,
                OwnerNin = nin
            };
            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
            {
                UserID = userId,
                UserName = userName,
                Channel = Channel.autoleasing.ToString(),
                ServerIP = Utilities.GetInternalServerIP()
            };
            var vehicleInfoFromYakeen = _yakeenClient.GetVehicleInfo(vehicleYakeenRequest, predefinedLogInfo);
            YakeenVehicles yakeenVehicles = new YakeenVehicles();
            if (vehicleInfoFromYakeen != null && vehicleInfoFromYakeen.Success)
            {
                vehiclePlateInfoFromYakeen = _yakeenClient.GetVehiclePlateInfo(vehicleYakeenRequest, predefinedLogInfo);
                if (vehiclePlateInfoFromYakeen != null && !string.IsNullOrEmpty(vehiclePlateInfoFromYakeen.ChassisNumber))
                {
                    yakeenVehicles.ChassisNumber = vehiclePlateInfoFromYakeen.ChassisNumber;
                }
                yakeenVehicles.ID = Guid.NewGuid();

                yakeenVehicles.IsRegistered = vehicleInfoFromYakeen.IsRegistered;
                yakeenVehicles.RegisterationPlace = vehicleInfoFromYakeen.RegisterationPlace;
                yakeenVehicles.Cylinders = vehicleInfoFromYakeen.Cylinders;
                yakeenVehicles.LicenseExpiryDate = vehicleInfoFromYakeen.LicenseExpiryDate;
                yakeenVehicles.LogId = vehicleInfoFromYakeen.LogId;
                yakeenVehicles.MajorColor = vehicleInfoFromYakeen.MajorColor;
                yakeenVehicles.MinorColor = vehicleInfoFromYakeen.MinorColor;
                yakeenVehicles.ModelYear = vehicleInfoFromYakeen.ModelYear;
                yakeenVehicles.PlateTypeCode = vehicleInfoFromYakeen.PlateTypeCode;
                yakeenVehicles.VehicleBodyCode = vehicleInfoFromYakeen.BodyCode;
                yakeenVehicles.VehicleWeight = vehicleInfoFromYakeen.Weight;
                yakeenVehicles.VehicleLoad = vehicleInfoFromYakeen.Load;
                yakeenVehicles.VehicleMakerCode = vehicleInfoFromYakeen.MakerCode;
                yakeenVehicles.VehicleModelCode = vehicleInfoFromYakeen.ModelCode;
                yakeenVehicles.VehicleMaker = vehicleInfoFromYakeen.Maker;
                yakeenVehicles.VehicleModel = vehicleInfoFromYakeen.Model;
                yakeenVehicles.SequenceNumber = vehicleYakeenRequest.VehicleId.ToString();
                yakeenVehicles.CreatedDate = DateTime.Now;
                _yakeenVehiclesRepository.Insert(yakeenVehicles);
                return yakeenVehicles.ID;
            }
            return null;
        }

        private Guid? GetDriverInfo(long nin, int birthMonth, int birthYear, string channel, Guid userId, string userName)
        {
            YakeenDrivers yakeenDrivers = new YakeenDrivers();
            var customerYakeenRequest = new CustomerYakeenRequestDto()
            {
                Nin = nin,
                IsCitizen = nin.ToString().StartsWith("1"),
                DateOfBirth = string.Format("{0}-{1}", birthMonth.ToString("00"), birthYear.ToString())
            };
            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
            {
                UserID = userId,
                UserName = userName,
                Channel = Channel.autoleasing.ToString(),
                ServerIP = Utilities.GetInternalServerIP()
            };
            var customerIdInfo = _yakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedLogInfo);
            if (!customerIdInfo.Success)
                return null;

            yakeenDrivers.DateOfBirthG = customerIdInfo.DateOfBirthG;
            yakeenDrivers.DateOfBirthH = customerIdInfo.DateOfBirthH;
            yakeenDrivers.DriverId = Guid.NewGuid();
            yakeenDrivers.EnglishFirstName = customerIdInfo.EnglishFirstName;
            yakeenDrivers.EnglishLastName = customerIdInfo.EnglishLastName;
            yakeenDrivers.EnglishSecondName = customerIdInfo.EnglishSecondName;
            yakeenDrivers.EnglishThirdName = customerIdInfo.EnglishThirdName;
            yakeenDrivers.FirstName = customerIdInfo.FirstName;
            yakeenDrivers.SecondName = customerIdInfo.SecondName;
            yakeenDrivers.ThirdName = customerIdInfo.ThirdName;
            yakeenDrivers.LastName = customerIdInfo.LastName;
            yakeenDrivers.GenderId = (int)customerIdInfo.Gender;
            yakeenDrivers.IdExpiryDate = customerIdInfo.IdExpiryDate;
            yakeenDrivers.IdIssuePlace = customerIdInfo.IdIssuePlace;
            yakeenDrivers.LogId = customerIdInfo.LogId;
            yakeenDrivers.NationalityCode = customerIdInfo.NationalityCode;
            yakeenDrivers.NIN = customerYakeenRequest.Nin.ToString();
            yakeenDrivers.OccupationCode = customerIdInfo.OccupationCode;
            yakeenDrivers.OccupationDesc = customerIdInfo.OccupationDesc;
            yakeenDrivers.SocialStatus = customerIdInfo.SocialStatus;
            yakeenDrivers.SubtribeName = customerIdInfo.SubtribeName;
            yakeenDrivers.CreatedDate = DateTime.Now;
            yakeenDrivers.LicenseList = Newtonsoft.Json.JsonConvert.SerializeObject(customerIdInfo.licenseListListField);
            _yakeenDriversRepository.Insert(yakeenDrivers);
            return yakeenDrivers.DriverId;
        }

        #region Winners

        public WinnersOutput<WinnersModel> GetAllWinners(int? weekNumber, string lang, string channel, string userId)
        {
            var output = new WinnersOutput<WinnersModel>();
            CompetitionRequestLog log = new CompetitionRequestLog();
            log.Channel = channel;
            log.UserId = userId;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.ServiceRequest = JsonConvert.SerializeObject($"weekNumber: {weekNumber}, lang: {lang}, channel: {channel}, userId: {userId}");
            log.Method = "GetWinnersDataForSite";
            try
            {
                if (weekNumber.HasValue && weekNumber.Value <= 0)
                {
                    output.ErrorCode = WinnersOutput<WinnersModel>.ErrorCodes.InvalidInput;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Week number is invalid, as the value passed is:" + weekNumber;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }

                WinnersModel winnersCategoriesList = new WinnersModel();
                winnersCategoriesList.CurrentWeekNumber = HandleCurrentWeekNumberForBcareCompetition();

                if (weekNumber == null || !weekNumber.HasValue || weekNumber.Value == 0)
                    weekNumber = winnersCategoriesList.CurrentWeekNumber;

                List<BcareWithdrawalWinner> winners = _bcareWithdrawalWinnerRepository.TableNoTracking.Where(a => a.WeekNumber == weekNumber && !a.IsDeleted).OrderBy(a => a.Id).ToList();
                if (winners == null || winners.Count <= 0)
                {
                    output.Result = new WinnersModel();
                    output.Result = winnersCategoriesList;
                    output.ErrorCode = WinnersOutput<WinnersModel>.ErrorCodes.NullResult;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("CompetitionWeekNoData", System.Globalization.CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "NO data found for the week number: " + weekNumber;
                    CompetitionDataAccess.AddToCompetition(log);
                    return output;
                }
                
                var tplList = winners.Where(a => a.ProductType == 1).ToList();
                if (tplList != null && tplList.Count > 0)
                {
                    winnersCategoriesList.TPLList = new List<WinnerCategory>();
                    for (int i = 0; i < tplList.Count; i++)
                    {
                        winnersCategoriesList.TPLList.Add(new WinnerCategory()
                        {
                            Order = i + 1,
                            ArabicName = tplList[i].ArabicName,
                            EnglishName = tplList[i].EnglishName,
                            PrizeType = tplList[i].PrizeNumber
                        });
                    }
                }

                var compList = winners.Where(a => a.ProductType == 2).ToList();
                if (compList != null && compList.Count > 0)
                {
                    winnersCategoriesList.CompList = new List<WinnerCategory>();
                    for (int i = 0; i < compList.Count; i++)
                    {
                        winnersCategoriesList.CompList.Add(new WinnerCategory()
                        {
                            Order = i + 1,
                            ArabicName = compList[i].ArabicName,
                            EnglishName = compList[i].EnglishName,
                            PrizeType = compList[i].PrizeNumber
                        });
                    }
                }

                var registerList = winners.Where(a => a.ProductType == 0).ToList();
                if (registerList != null && registerList.Count > 0)
                {
                    winnersCategoriesList.RegisterList = new List<WinnerCategory>();
                    for (int i = 0; i < registerList.Count; i++)
                    {
                        winnersCategoriesList.RegisterList.Add(new WinnerCategory()
                        {
                            Order = i + 1,
                            ArabicName = registerList[i].ArabicName,
                            EnglishName = registerList[i].EnglishName,
                            PrizeType = registerList[i].PrizeNumber
                        });
                    }
                }

                output.Result = new WinnersModel();
                output.Result = winnersCategoriesList;
                output.ErrorCode = WinnersOutput<WinnersModel>.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("Success", System.Globalization.CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = WinnersOutput<WinnersModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", System.Globalization.CultureInfo.GetCultureInfo(lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CompetitionDataAccess.AddToCompetition(log);
                return output;
            }
        }

        private int HandleCurrentWeekNumberForBcareCompetition()
        {
            int weekNumber = 0;
            DateTime toDay = DateTime.Now.Date;
            if (toDay >= new DateTime(2022, 03, 17) && toDay <= new DateTime(2022, 03, 23))
                weekNumber = 1;
            else if (toDay >= new DateTime(2022, 03, 24) && toDay <= new DateTime(2022, 03, 30))
                weekNumber = 2;
            else if (toDay >= new DateTime(2022, 03, 31) && toDay <= new DateTime(2022, 04, 06))
                weekNumber = 3;
            else if (toDay >= new DateTime(2022, 04, 07) && toDay <= new DateTime(2022, 04, 13))
                weekNumber = 4;
            else if (toDay >= new DateTime(2022, 04, 14) && toDay <= new DateTime(2022, 04, 20))
                weekNumber = 5;
            else if (toDay >= new DateTime(2022, 04, 21) && toDay <= new DateTime(2022, 04, 27))
                weekNumber = 6;
            else if (toDay >= new DateTime(2022, 05, 05) && toDay <= new DateTime(2022, 05, 11))
                weekNumber = 7;
            else if (toDay >= new DateTime(2022, 05, 12) && toDay <= new DateTime(2022, 05, 25))
                weekNumber = 8;

            return weekNumber;
        }

        #endregion

        #endregion
    }
}
