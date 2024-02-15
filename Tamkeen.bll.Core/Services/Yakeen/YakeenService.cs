using System;
using System.Globalization;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Services.Core.Vehicles;
using TameenkDAL.Store;
using TameenkDAL.UoW;
using Tamkeen.bll.Business;
using Tamkeen.bll.Services.Yakeen.Models;
using Tamkeen.bll.YakeenBCareService;

namespace Tamkeen.bll.Services.Yakeen
{
    public class YakeenService : IYakeenService
    {
        //@TODO: move out of here later
        public static class Authorized
        {
            public static string UserName { get { return "Bcare_PROD_USR"; } }
            public static string Password { get { return "Bcare@9143"; } }
            public static string ChargeCode { get { return "PROD"; } }
        }

        private string _token = "yakeen_token";
        private Yakeen4BcareClient _clint = new Yakeen4BcareClient();

        private readonly ITameenkUoW _tameenkUoW;
        private readonly IVehicleService _vehicleService;
        private readonly DriverBusiness _driverBusiness;

        public YakeenService(IVehicleService vehicleService, ITameenkUoW tameenkUoW)
        {
            _tameenkUoW = tameenkUoW;
            _vehicleService = vehicleService;
            _driverBusiness = new DriverBusiness(_tameenkUoW);
        }

        private bool TokenVerification(string token)
        {
            if (!string.IsNullOrEmpty(token) && token == _token)
                return true;

            return false;
        }

        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,DateOfBirth)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public CustomerIdInfoResult GetCustomerIdInfo(YakeenRequest req)
        {
            CustomerIdInfoResult res = new CustomerIdInfoResult
            {
                Success = false
            };

            if (req == null)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (!TokenVerification(req.Token))
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "invalid token.";
                return res;
            }

            // if user is citizen so get citizen else get alien
            Driver driver = _driverBusiness.GetDriverInfoByNIN(req.NIN);
            if (req.IsCitizen)
            {
                // get citizen req 
                if (driver == null)
                {
                    res = GetCitizenIdInfo(req, res);
                    if (res.Success)
                    {
                        res.InternalIdentifier = _driverBusiness.CreateDriverEntityFromCustomerIdInfo(res, req.NIN.ToString(), req.IsCitizen, req.IsDriverSpecialNeed);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(driver.IdIssuePlace) || string.IsNullOrEmpty(driver.IdExpiryDate))
                    {
                        res = GetCitizenIdInfo(req, res);
                        if (res.Success)
                        {
                            res.InternalIdentifier = _driverBusiness.UpdateDriverEntityFromCustomerIdInfo(res, driver, req.IsDriverSpecialNeed);
                        }
                    }
                    else
                    {
                        res = getCustomerIdInfoByDriverEntity(driver);
                    }
                }
            }
            else
            {
                // get alien req
                if (driver == null)
                {
                    // get drliver info
                    res = GetAlienInfoByIqama(req, res);
                    if (res.Success)
                    {
                        res.InternalIdentifier = _driverBusiness.CreateDriverEntityFromCustomerIdInfo(res, req.NIN.ToString(), req.IsCitizen, req.IsDriverSpecialNeed);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(driver.IdIssuePlace) || string.IsNullOrEmpty(driver.IdExpiryDate))
                    {
                        res = GetAlienInfoByIqama(req, res);
                        if (res.Success)
                        {
                            res.InternalIdentifier = _driverBusiness.UpdateDriverEntityFromCustomerIdInfo(res, driver, req.IsDriverSpecialNeed);
                        }
                    }
                    else
                    {
                        res = getCustomerIdInfoByDriverEntity(driver);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,DateOfBirth)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        public CustomerNameInfoResult GetCustomerNameInfo(YakeenRequest req)
        {
            CustomerNameInfoResult res = new CustomerNameInfoResult
            {
                Success = false
            };

            if (req == null)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (!TokenVerification(req.Token))
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "invalid token.";
                return res;
            }

            Driver driver = _driverBusiness.GetDriverInfoByNIN(req.NIN);
            if (driver != null &&
                !string.IsNullOrEmpty(driver.FirstName) &&
                !string.IsNullOrEmpty(driver.EnglishFirstName) &&
                !string.IsNullOrEmpty(driver.SecondName) &&
                !string.IsNullOrEmpty(driver.EnglishSecondName) &&
                !string.IsNullOrEmpty(driver.ThirdName) &&
                !string.IsNullOrEmpty(driver.EnglishThirdName) &&
                !string.IsNullOrEmpty(driver.LastName) &&
                !string.IsNullOrEmpty(driver.EnglishLastName))
            {
                res = new CustomerNameInfoResult()
                {
                    Success = true,
                    IsCitizen = driver.IsCitizen ? 1 : 2,
                    EnglishFirstName = driver.EnglishFirstName,
                    EnglishLastName = driver.EnglishLastName,
                    EnglishSecondName = driver.EnglishSecondName,
                    EnglishThirdName = driver.EnglishThirdName,
                    FirstName = driver.FirstName,
                    SecondName = driver.SecondName,
                    LastName = driver.LastName,
                    ThirdName = driver.ThirdName,
                    SubtribeName = driver.SubtribeName
                };

                return res;
            }

            // if user is citizen so get citizen else get alien 
            if (req.IsCitizen)
            {
                // get citizen name
                res = GetCitizenNameInfo(req, res);
            }

            else
            {
                // get alien name
                res = GetAlienNameInfoByIqama(req, res);
            }

            _driverBusiness.UpdateDriverFromCustomerNameInfoResult(driver, res);
            return res;

        }

        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,licExpiryDate)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        public DriverInfoResult GetDriverInfo(YakeenRequest req)
        {
            DriverInfoResult res = new DriverInfoResult
            {
                Success = false
            };

            if (req == null)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (!TokenVerification(req.Token))
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "invalid token.";
                return res;
            }

            // if user is citizen so get citizen else get alien 
            Driver driver = _driverBusiness.GetDriverInfoByNIN(req.NIN);
            if (req.IsCitizen)
            {
                if (driver == null)
                {
                    // get driver info 
                    res = GetCitizenDriverInfo(req, res);
                    if (res.Success)
                    {
                        res.InternalIdentifier = _driverBusiness.CreateDriverEntityFromDriverInfo(res, req.NIN.ToString(), req.IsCitizen);
                    }
                }
                else
                {
                    if (driver.DriverLicenses != null && driver.DriverLicenses.Any())
                    {
                        res = getDriverInfoByDriverEntity(driver);
                    }
                    else
                    {
                        res = GetCitizenDriverInfo(req, res);
                        if (res.Success)
                        {
                            res.InternalIdentifier = _driverBusiness.UpdateDriverEntityFromDriverInfo(res, driver);
                        }
                    }
                }

            }
            else
            {
                if (driver == null)
                {
                    // get drliver info
                    res = GetAlienDriverInfoByIqama(req, res);
                    if (res.Success)
                    {
                        res.InternalIdentifier = _driverBusiness.CreateDriverEntityFromDriverInfo(res, req.NIN.ToString(), req.IsCitizen);
                    }
                }
                else
                {
                    if (driver.DriverLicenses != null && driver.DriverLicenses.Any())
                    {
                        res = getDriverInfoByDriverEntity(driver);
                    }
                    else
                    {
                        res = GetAlienDriverInfoByIqama(req, res);
                        if (res.Success)
                        {
                            res.InternalIdentifier = _driverBusiness.UpdateDriverEntityFromDriverInfo(res, driver);
                        }
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// data needed in req : registered : (IsCarRegistered=true,UserName,Password,ChargeCode,ReferenceNumber,CarOwnerId,CarSequenceNumber)
        /// not registered : (IsCarRegistered=false,UserName,Password,ChargeCode,ReferenceNumber,CarModelYear,CustomCarCardNumber)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        public CarInfoResult GetCarInfo(YakeenRequest req)
        {
            CarInfoResult res = new CarInfoResult
            {
                Success = false
            };

            if (req == null)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (!TokenVerification(req.Token))
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "invalid token.";
                return res;
            }

            // if car is registered get by seq else get by custom
            if (req.IsCarRegistered)
            {
                var vehicle = _vehicleService.GetVehicleInfoBySequnceNumber(req.CarSequenceNumber.ToString());
                if (vehicle == null)
                {
                    res = GetCarInfoBySequence(req, res);
                    if (res.PlateTypeCode == 0)
                        res.PlateTypeCode = 11; // Change from unknown to temp
                    if (res.Success)
                    {
                        vehicle = CreateVehicleEntityFromCarInfo(res,
                            (req.CarSequenceNumber == default(int) ? null : req.CarSequenceNumber.ToString()),
                            (req.CustomCarCardNumber == default(long) ? null : req.CustomCarCardNumber.ToString()),
                            req.IsVehicleUsedCommercially, req.VehicleValue);
                        res.InternalIdentifier = _vehicleService.AddVehicle(vehicle).ID;
                    }
                }
                else
                {
                    res = getCarInfoByVehicleEntity(vehicle);
                }
            }
            else
            {
                var vehicle = _vehicleService.GetVehicleInfoByCustomCardNumber(req.CustomCarCardNumber.ToString());
                if (vehicle == null)
                {

                    res = GetCarInfoByCustom(req, res);
                    if (res.PlateTypeCode == 0)
                        res.PlateTypeCode = 11; // Change from unknown to temp
                    if (res.Success)
                    {
                        vehicle = CreateVehicleEntityFromCarInfo(res,
                            (req.CarSequenceNumber == default(int) ? null : req.CarSequenceNumber.ToString()),
                            (req.CustomCarCardNumber == default(long) ? null : req.CustomCarCardNumber.ToString()),
                            req.IsVehicleUsedCommercially, req.VehicleValue);
                        res.InternalIdentifier = _vehicleService.AddVehicle(vehicle).ID;
                    }
                }
                else
                {
                    res = getCarInfoByVehicleEntity(vehicle);
                }
            }

            return res;

        }



        /// <summary>
        /// data needed in req : (IsCarRegistered=true,UserName,Password,ChargeCode,ReferenceNumber,CarOwnerId,CarSequenceNumber)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public CarPlateResult GetCarPlateInfo(YakeenRequest req)
        {
            CarPlateResult res = new CarPlateResult
            {
                Success = false
            };

            if (req == null)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (!TokenVerification(req.Token))
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "invalid token.";
                return res;
            }

            if (!req.IsCarRegistered)
            {
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "Car is not Registered.";
                return res;
            }

            var vehicle = _vehicleService.GetVehicleInfoBySequnceNumber(req.CarSequenceNumber.ToString());

            if (vehicle != null &&
                !string.IsNullOrEmpty(vehicle.CarPlateText1) &&
                !string.IsNullOrEmpty(vehicle.CarPlateText2) &&
                !string.IsNullOrEmpty(vehicle.CarPlateText3) &&
                !string.IsNullOrEmpty(vehicle.CarOwnerNIN) &&
                !string.IsNullOrEmpty(vehicle.CarOwnerName) &&
                vehicle.CarPlateNumber.HasValue)
            {
                res = new CarPlateResult()
                {
                    Success = true,
                    ChassisNumber = vehicle.ChassisNumber,
                    OwnerName = vehicle.CarOwnerName,
                    PlateNumber = vehicle.CarPlateNumber.Value,
                    PlateText1 = vehicle.CarPlateText1,
                    PlateText2 = vehicle.CarPlateText2,
                    PlateText3 = vehicle.CarPlateText3
                };
            }
            else
            {
                res = GetCarPlateInfoBySequence(req, res);
                vehicle.CarOwnerNIN = req.CarOwnerId.ToString();
                if (res.Success)
                {
                    vehicle.CarOwnerName = res.OwnerName;
                    vehicle.CarPlateNumber = res.PlateNumber;
                    vehicle.CarPlateText1 = res.PlateText1;
                    vehicle.CarPlateText2 = res.PlateText2;
                    vehicle.CarPlateText3 = res.PlateText3;
                    vehicle.ChassisNumber = res.ChassisNumber;
                    _vehicleService.UpdateVehicle(vehicle);
                }

            }
            return res;

        }


        private CustomerIdInfoResult GetCitizenIdInfo(YakeenRequest req, CustomerIdInfoResult res)
        {
            if (res == null)
            {
                res = new CustomerIdInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable obj";
                return res;
            }
            try
            {
                getCitizenIDInfo citizenId = new getCitizenIDInfo();
                citizenId.CitizenIDInfoRequest = new citizenIDInfoRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    nin = req.NIN.ToString(),
                    dateOfBirth = req.DateOfBirth
                };
                var result = _clint.getCitizenIDInfo(citizenId.CitizenIDInfoRequest);

                // add to res
                res.Success = true;
                res.IsCitizen = 1;
                res.Gender = result.gender;
                res.LogId = result.logId;
                res.DateOfBirthG = DateTime.ParseExact(result.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                res.DateOfBirthH = result.dateOfBirthH;
                res.IdIssuePlace = result.idIssuePlace;
                res.IdExpiryDate = result.idExpiryDate;
                res.NationalityCode = 113;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CustomerNameInfoResult GetCitizenNameInfo(YakeenRequest req, CustomerNameInfoResult res)
        {
            if (res == null)
            {
                res = new CustomerNameInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }


            try
            {
                getCitizenNameInfo citizenName = new getCitizenNameInfo();
                citizenName.CitizenNameInfoRequest = new citizenNameInfoRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    nin = req.NIN.ToString(),
                    dateOfBirth = req.DateOfBirth
                };
                var result = _clint.getCitizenNameInfo(citizenName.CitizenNameInfoRequest);

                // add to res
                res.Success = true;
                res.IsCitizen = 1;
                res.LogId = result.logId;
                res.FirstName = result.firstName;
                res.SecondName = result.fatherName;
                res.ThirdName = result.grandFatherName;
                res.LastName = result.familyName;
                res.EnglishFirstName = result.englishFirstName;
                res.EnglishSecondName = result.englishSecondName;
                res.EnglishThirdName = result.englishThirdName;
                res.EnglishLastName = result.englishLastName;
                res.SubtribeName = result.subtribeName;

            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CustomerIdInfoResult GetAlienInfoByIqama(YakeenRequest req, CustomerIdInfoResult res)
        {
            if (res == null)
            {
                res = new CustomerIdInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getAlienInfoByIqama alienId = new getAlienInfoByIqama();
                alienId.AlienInfoByIqamaRequest = new alienInfoByIqamaRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    iqamaNumber = req.NIN.ToString(),
                    dateOfBirth = req.DateOfBirth
                };
                var result = _clint.getAlienInfoByIqama(alienId.AlienInfoByIqamaRequest);
                // add to res
                res.Success = true;
                res.IsCitizen = 2;
                res.NationalityCode = result.nationalityCode;
                res.Gender = result.gender;
                res.LogId = result.logId;
                res.DateOfBirthG = DateTime.ParseExact(result.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                res.DateOfBirthH = result.dateOfBirthH;
                res.IdIssuePlace = result.iqamaIssuePlaceDesc;
                res.IdExpiryDate = result.iqamaExpiryDateH;

            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CustomerNameInfoResult GetAlienNameInfoByIqama(YakeenRequest req, CustomerNameInfoResult res)
        {
            if (res == null)
            {
                res = new CustomerNameInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            try
            {
                getAlienNameInfoByIqama alienName = new getAlienNameInfoByIqama();
                alienName.AlienNameInfoByIqamaRequest = new alienNameInfoByIqamaRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    iqamaNumber = req.NIN.ToString(),
                    dateOfBirth = req.DateOfBirth
                };
                var result = _clint.getAlienNameInfoByIqama(alienName.AlienNameInfoByIqamaRequest);

                // add to res
                res.Success = true;
                res.IsCitizen = 2;
                res.LogId = result.logId;
                res.FirstName = result.firstName;
                res.SecondName = result.secondName;
                res.ThirdName = result.thirdName;
                res.LastName = result.lastName;
                res.EnglishFirstName = result.englishFirstName;
                res.EnglishSecondName = result.englishSecondName;
                res.EnglishThirdName = result.englishThirdName;
                res.EnglishLastName = result.englishLastName;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private DriverInfoResult GetCitizenDriverInfo(YakeenRequest req, DriverInfoResult res)
        {
            if (res == null)
            {
                res = new DriverInfoResult()
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getCitizenDriverInfo citizenId = new getCitizenDriverInfo();
                citizenId.CitizenDriverInfoRequest = new citizenDriverInfoRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    nin = req.NIN.ToString(),
                    licExpiryDate = req.LicenseExpiryDate
                };
                var result = _clint.getCitizenDriverInfo(citizenId.CitizenDriverInfoRequest);
                // add to res
                res.Success = true;
                res.IsCitizen = 1;
                res.Gender = result.gender;
                res.LogId = result.logId;
                res.FirstName = result.firstName;
                res.SecondName = result.fatherName;
                res.ThirdName = result.grandFatherName;
                res.LastName = result.familyName;
                res.SubtribeName = result.subtribeName;
                res.EnglishFirstName = result.englishFirstName;
                res.EnglishSecondName = result.englishSecondName;
                res.EnglishThirdName = result.englishThirdName;
                res.EnglishLastName = result.englishLastName;
                res.DateOfBirthG = DateTime.ParseExact(result.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                res.DateOfBirthH = result.dateOfBirthH;
                res.NationalityCode = 113;
                foreach (var lic in result.licenseListList)
                {
                    res.LicensesList.Add(new Models.DriverLicense
                    {
                        TypeDesc = lic.licnsTypeCode,
                        ExpiryDateH = lic.licssExpiryDateH
                    });
                }
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private DriverInfoResult GetAlienDriverInfoByIqama(YakeenRequest req, DriverInfoResult res)
        {
            if (res == null)
            {
                res = new DriverInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getAlienDriverInfoByIqama alienId = new getAlienDriverInfoByIqama();
                alienId.AlienDriverInfoByIqamaRequest = new alienDriverInfoByIqamaRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    iqamaNumber = req.NIN.ToString(),
                    licExpiryDate = req.LicenseExpiryDate
                };
                var result = _clint.getAlienDriverInfoByIqama(alienId.AlienDriverInfoByIqamaRequest);
                // add to res
                res.Success = true;
                res.IsCitizen = 2;
                res.Gender = result.gender;
                res.NationalityCode = result.nationalityCode;
                res.LogId = result.logId;
                res.FirstName = result.firstName;
                res.SecondName = result.secondName;
                res.ThirdName = result.thirdName;
                res.LastName = result.lastName;
                res.EnglishFirstName = result.englishFirstName;
                res.EnglishSecondName = result.englishSecondName;
                res.EnglishThirdName = result.englishThirdName;
                res.EnglishLastName = result.englishLastName;
                res.DateOfBirthG = DateTime.ParseExact(result.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                res.DateOfBirthH = result.dateOfBirthH;
                foreach (var lic in result.licensesListList)
                {
                    res.LicensesList.Add(new Models.DriverLicense
                    {
                        TypeDesc = lic.licnsTypeCode,
                        ExpiryDateH = lic.licssExpiryDateH
                    });
                }
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CarInfoResult GetCarInfoBySequence(YakeenRequest req, CarInfoResult res)
        {
            if (res == null)
            {
                res = new CarInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getCarInfoBySequence carSequence = new getCarInfoBySequence();
                carSequence.CarInfoBySequenceRequest = new carInfoBySequenceRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    ownerID = req.CarOwnerId,
                    sequenceNumber = req.CarSequenceNumber
                };
                var result = _clint.getCarInfoBySequence(carSequence.CarInfoBySequenceRequest);
                // add to res
                res.Success = true;
                res.IsRegistered = true;
                res.Cylinders = result.cylinders;
                res.LicenseExpiryDate = result.licenseExpiryDate;
                res.LogId = result.logId;
                res.MajorColor = result.majorColor;
                res.MinorColor = result.minorColor;
                res.ModelYear = result.modelYear;
                res.PlateTypeCode = result.plateTypeCode;
                res.Regplace = result.regplace;
                res.VehicleBodyCode = result.vehicleBodyCode;
                res.VehicleWeight = result.vehicleWeight;
                res.VehicleLoad = result.vehicleLoad;
                res.VehicleMakerCode = (short)result.vehicleMakerCode;
                res.VehicleModelCode = int.Parse(result.vehicleModelCode);
                res.VehicleMaker = result.vehicleMaker;
                res.VehicleModel = result.vehicleModel;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CarPlateResult GetCarPlateInfoBySequence(YakeenRequest req, CarPlateResult res)
        {
            if (res == null)
            {
                res = new CarPlateResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getCarPlateInfoBySequence carPlate = new getCarPlateInfoBySequence();
                carPlate.CarPlateInfoBySequenceRequest = new carPlateInfoBySequenceRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    ownerID = req.CarOwnerId,
                    sequenceNumber = req.CarSequenceNumber
                };
                var result = _clint.getCarPlateInfoBySequence(carPlate.CarPlateInfoBySequenceRequest);
                // add to res
                res.Success = true;
                res.LogId = result.logId;
                res.ChassisNumber = result.chassisNumber;
                res.OwnerName = result.ownerName;
                res.PlateNumber = result.plateNumber;
                res.PlateText1 = result.plateText1;
                res.PlateText2 = result.plateText2;
                res.PlateText3 = result.plateText3;

            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CarInfoResult GetCarInfoByCustom(YakeenRequest req, CarInfoResult res)
        {
            if (res == null)
            {
                res = new CarInfoResult
                {
                    Success = false
                };
            }

            if (req == null)
            {
                res.Success = false;
                res.Error.Type = ErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getCarInfoByCustom carSequence = new getCarInfoByCustom();
                carSequence.CarInfoByCustomRequest = new carInfoByCustomRequest()
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    modelYear = req.CarModelYear,
                    customCardNumber = req.CustomCarCardNumber.ToString()
                };
                var result = _clint.getCarInfoByCustom(carSequence.CarInfoByCustomRequest);
                // add to res
                res.Success = true;
                res.IsRegistered = false;
                res.Cylinders = result.cylinders;
                res.LogId = result.logId;
                res.MajorColor = result.majorColor;
                res.MinorColor = result.minorColor;
                res.ModelYear = result.modelYear;
                res.VehicleBodyCode = result.vehicleBodyCode;
                res.VehicleWeight = result.vehicleWeight;
                res.VehicleLoad = result.vehicleLoad;
                res.VehicleMakerCode = (short)result.vehicleMakerCode;
                res.VehicleModelCode = int.Parse(result.vehicleModelCode);
                res.VehicleMaker = result.vehicleMaker;
                res.VehicleModel = result.vehicleModel;
                res.ChassisNumber = result.chassisNumber;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = ErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }


        public string GetChassisNumberByCustom(YakeenRequest req)
        {
            if (req == null)
            {
                return null;
            }
            try
            {
                getCarChassisNumberInfoByCustom carChassis = new getCarChassisNumberInfoByCustom();
                carChassis.CarChassisNumberInfoByCustomRequest = new carChassisNumberInfoByCustomRequest
                {
                    userName = Authorized.UserName,
                    password = Authorized.Password,
                    chargeCode = Authorized.ChargeCode,
                    referenceNumber = req.ReferenceNumber,
                    customCardNumber = req.CustomCarCardNumber.ToString(),
                    modelYear = req.CarModelYear
                };


                var result = _clint.getCarChassisNumberInfoByCustom(carChassis.CarChassisNumberInfoByCustomRequest);
                return result.chassisNumber;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                //if (msgFault.HasDetail)
                //{
                //    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                //    res.Success = false;
                //    res.Error.Type = ErrorType.YakeenError;
                //    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                //}

                return null;
            }
        }

        //public CitizenIDInfoResult GetCitizenIDInfoResponse(string id)
        //{
        //    // mock for now
        //    return new CitizenIDInfoResult
        //    {
        //        dateOfBirthG = "1-1-1412",
        //        dateOfBirthH = "1-1-1990",
        //        gender = "1",
        //        idExpiryDate = DateTime.Now.ToShortDateString(),
        //        idIssuePlace = "1"
        //    };
        //}

        private CarInfoResult getCarInfoByVehicleEntity(Tameenk.Core.Domain.Entities.VehicleInsurance.Vehicle vehicle)
        {
            var res = new CarInfoResult()
            {
                Success = true,
                InternalIdentifier = vehicle.ID,
                IsRegistered = vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                Cylinders = vehicle.Cylinders.HasValue ? vehicle.Cylinders.Value : default(short),
                LicenseExpiryDate = vehicle.LicenseExpiryDate,
                MajorColor = vehicle.MajorColor,
                MinorColor = vehicle.MinorColor,
                ModelYear = (vehicle.ModelYear.HasValue ? vehicle.ModelYear.Value : default(short)),
                PlateTypeCode = vehicle.PlateTypeCode,
                Regplace = vehicle.RegisterationPlace,
                VehicleBodyCode = vehicle.VehicleBodyCode,
                VehicleWeight = vehicle.VehicleWeight,
                VehicleLoad = vehicle.VehicleLoad,
                VehicleModel = vehicle.VehicleModel,
                VehicleMaker = vehicle.VehicleMaker,
                ChassisNumber = vehicle.ChassisNumber,
                VehicleMakerCode = vehicle.VehicleMakerCode ?? 0,
                VehicleModelCode = vehicle.VehicleModelCode.HasValue ? (int)vehicle.VehicleModelCode : 0
            };

            return res;
        }



        private DriverInfoResult getDriverInfoByDriverEntity(Driver driver)
        {
            var res = new DriverInfoResult()
            {
                Success = true,
                EnglishFirstName = driver.EnglishFirstName,
                EnglishLastName = driver.EnglishLastName,
                EnglishSecondName = driver.EnglishSecondName,
                EnglishThirdName = driver.EnglishThirdName,
                LastName = driver.LastName,
                SecondName = driver.SecondName,
                FirstName = driver.FirstName,
                ThirdName = driver.ThirdName,
                SubtribeName = driver.SubtribeName,
                DateOfBirthG = driver.DateOfBirthG,
                DateOfBirthH = driver.DateOfBirthH,
                NationalityCode = driver.NationalityCode.HasValue ? driver.NationalityCode.Value : default(short),
                IsCitizen = driver.IsCitizen ? 1 : 2,
                InternalIdentifier = driver.DriverId,

            };

            res.Gender = Extensions.FromCode<gender>(driver.Gender.GetCode());

            if (driver.DriverLicenses != null && driver.DriverLicenses.Any())
            {
                foreach (var lic in driver.DriverLicenses)
                {
                    res.LicensesList.Add(new Models.DriverLicense()
                    {
                        TypeDesc = lic.TypeDesc,
                        ExpiryDateH = lic.ExpiryDateH
                    });
                }
            }
            return res;
        }

        private CustomerIdInfoResult getCustomerIdInfoByDriverEntity(Driver driver)
        {
            var res = new CustomerIdInfoResult()
            {
                Success = true,
                DateOfBirthG = driver.DateOfBirthG,
                DateOfBirthH = driver.DateOfBirthH,
                NationalityCode = driver.NationalityCode.HasValue ? driver.NationalityCode.Value : default(short),
                IsCitizen = driver.IsCitizen ? 1 : 2,
                IdExpiryDate = driver.IdExpiryDate,
                IdIssuePlace = driver.IdIssuePlace,
                InternalIdentifier = driver.DriverId,

            };

            switch (driver.Gender.GetCode())
            {
                case "F":
                    res.Gender = gender.F;
                    break;
                case "M":
                    res.Gender = gender.M;
                    break;
                case "U":
                    res.Gender = gender.U;
                    break;
            }

            return res;
        }

        public Vehicle CreateVehicleEntityFromCarInfo(CarInfoResult carInfo, string sequenceNo, string customCardNo, bool isVehicleUsedCommercially, int vehicleValue)
        {
            return new Vehicle()
            {
                ID = Guid.NewGuid(),
                SequenceNumber = sequenceNo,
                CustomCardNumber = customCardNo,
                VehicleIdTypeId = carInfo.IsRegistered ? 1 : 2,
                Cylinders = byte.Parse(carInfo.Cylinders.ToString()),
                LicenseExpiryDate = carInfo.LicenseExpiryDate,
                MajorColor = carInfo.MajorColor,
                MinorColor = carInfo.MinorColor,
                ModelYear = short.Parse(carInfo.ModelYear.ToString()),
                PlateTypeCode = byte.Parse(carInfo.PlateTypeCode.ToString()),
                RegisterationPlace = carInfo.Regplace,
                VehicleBodyCode = byte.Parse(carInfo.VehicleBodyCode.ToString()),
                VehicleWeight = carInfo.VehicleWeight,
                VehicleLoad = carInfo.VehicleLoad,
                VehicleModel = carInfo.VehicleModel,
                VehicleMaker = carInfo.VehicleMaker,
                ChassisNumber = carInfo.ChassisNumber,
                VehicleMakerCode = (short)carInfo.VehicleMakerCode,
                VehicleModelCode = carInfo.VehicleModelCode,
                VehicleValue = vehicleValue,
                IsUsedCommercially = isVehicleUsedCommercially,
                IsDeleted = false,
                CreatedDateTime = DateTime.Now,
            };
        }
    }
}
