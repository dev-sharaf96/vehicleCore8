using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Occupations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;

namespace Tameenk.Services.Inquiry.Components
{
    public class InquiryUtilities: IInquiryUtilities
    {
        private readonly IVehicleService vehicleService;
        private readonly IRepository<Driver> driverRepository;
        private readonly IHttpClient httpClient;
        private readonly ICustomerServices customerServices;
        private readonly IAddressService addressService;
        private readonly IYakeenVehicleServices yakeenVehicleServices;
        private readonly IOccupationService occupationService;
        private readonly IWebApiContext webApiContext;
        private readonly INajmService najmService;
        private readonly IRepository<NajmResponseEntity> najmResponseRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IRepository<YakeenVehicles> _yakeenVehiclesRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IRepository<QuotationRequest> quotationRequestRepository;
        private readonly HashSet<string> requiredFieldsInCustomOnly = new HashSet<string>
            {
                "VehicleLoad",
                "VehicleModel",
                "VehicleModelCode",
                "VehicleMajorColor",
                "VehicleBodyCode",
                "VehicleMaker",
                "VehicleMakerCode",
                "VehicleChassisNumber"
            };
        private readonly HashSet<string> FieldsAllowNullValue = new HashSet<string>
            {
                "AdditionDriver1ID",
                "AdditionDriver1DOB",
                "AdditionDriver2ID",
                "AdditionDriver2DOB"
            };
        public string AuthorizationToken { get; set; }
        public InquiryUtilities(IVehicleService vehicleService,
            IRepository<Driver> driverRepository, IHttpClient httpClient, ICustomerServices customerServices,
            IAddressService addressService,
            IYakeenVehicleServices yakeenVehicleServices,
            IOccupationService occupationService,
            IWebApiContext webApiContext,
            INajmService najmService,
            IRepository<NajmResponseEntity> NajmResponse,
            IYakeenClient yakeenClient,
            IRepository<YakeenDrivers> yakeenDriversRepository, IRepository<YakeenVehicles> yakeenVehiclesRepository, 
            IRepository<LicenseType> licenseTypeRepository, IRepository<QuotationRequest> quotationRequestRepository)
        {
           
            this.vehicleService = vehicleService;
            this.driverRepository = driverRepository;
            this.httpClient = httpClient;
            this.customerServices = customerServices;
            this.addressService = addressService;
            this.yakeenVehicleServices = yakeenVehicleServices;
            this.occupationService = occupationService;
            this.webApiContext = webApiContext;
            this.najmService = najmService;
            najmResponseRepository = NajmResponse;
            _yakeenClient = yakeenClient;
            _yakeenDriversRepository = yakeenDriversRepository;
            _yakeenVehiclesRepository = yakeenVehiclesRepository;
            _licenseTypeRepository = licenseTypeRepository;
            this.quotationRequestRepository = quotationRequestRepository;
        }
        public NajmResponse GetNajmResponse(NajmRequest request, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                //NajmResponseEntity entity;
                // Get only the valid request the are not expired within 29 days
                var VehicleId = request.VehicleId.ToString();
                var PolicyHolderNin = request.PolicyHolderNin.ToString();
                var IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered);
                var benchmarkDate = DateTime.Now.AddDays(-1);
                var entity = najmResponseRepository.Table
                                      .FirstOrDefault(x => x.VehicleId == VehicleId && x.IsDeleted == false &&
                                       x.PolicyHolderNin == PolicyHolderNin &&
                                       x.IsVehicleRegistered == IsVehicleRegistered
                                      );

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
                else
                {
                    var najmResponse = najmService.GetNajm(request, predefinedLogInfo);
                    if (najmResponse == null || !string.IsNullOrEmpty(najmResponse.ErrorMsg))
                    {
                        return najmResponse;
                    }

                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        najmResponseRepository.Update(entity);
                    }
                    entity = new NajmResponseEntity()
                    {
                        IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered),
                        VehicleId = request.VehicleId.ToString(),
                        PolicyHolderNin = request.PolicyHolderNin.ToString(),
                        NCDFreeYears = najmResponse.NCDFreeYears.Value,
                        NCDReference = najmResponse.NCDReference
                    };
                    najmResponseRepository.Insert(entity);
                    return najmResponse;
                }
            }
            catch (Exception exp)
            {
                NajmResponse najmResponse = new NajmResponse();
                najmResponse.StatusCode = 500;
                najmResponse.ErrorMsg = exp.ToString();
                return najmResponse;
            }
        }

        public VehicleYakeenModel GetVehicleYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var vehicleInfoRequest = new VehicleInfoRequestModel();
                vehicleInfoRequest.VehicleId = requestModel.Vehicle.VehicleId;
                vehicleInfoRequest.VehicleIdTypeId = requestModel.Vehicle.VehicleIdTypeId;
                vehicleInfoRequest.OwnerNin = requestModel.Vehicle.OwnerTransfer ? requestModel.OldOwnerNin.Value : long.Parse(requestModel.Insured.NationalId);
                vehicleInfoRequest.ModelYear = requestModel.Vehicle.ManufactureYear;
                int vehicleValue = 0;
                int.TryParse(requestModel.Vehicle.ApproximateValue.ToString(), out vehicleValue);
                vehicleInfoRequest.VehicleValue = vehicleValue;
                vehicleInfoRequest.IsUsedCommercially = requestModel.IsVehicleUsedCommercially;
                vehicleInfoRequest.IsOwnerTransfer = requestModel.Vehicle.OwnerTransfer;
                vehicleInfoRequest.BrakeSystemId = requestModel.Vehicle.BrakeSystemId;
                vehicleInfoRequest.CameraTypeId = requestModel.Vehicle.CameraTypeId;
                vehicleInfoRequest.CruiseControlTypeId = requestModel.Vehicle.CruiseControlTypeId;
                vehicleInfoRequest.CurrentMileageKM = requestModel.Vehicle.CurrentMileageKM;
                vehicleInfoRequest.HasAntiTheftAlarm = requestModel.Vehicle.HasAntiTheftAlarm;
                vehicleInfoRequest.HasFireExtinguisher = requestModel.Vehicle.HasFireExtinguisher;
                vehicleInfoRequest.ParkingSensorId = requestModel.Vehicle.ParkingSensorId;
                vehicleInfoRequest.TransmissionTypeId = requestModel.Vehicle.TransmissionTypeId;
                vehicleInfoRequest.ParkingLocationId = requestModel.Vehicle.ParkingLocationId;
                vehicleInfoRequest.HasModification = requestModel.Vehicle.HasModification;
                vehicleInfoRequest.Modification = requestModel.Vehicle.Modification;
                vehicleInfoRequest.MileageExpectedAnnualId = requestModel.Vehicle.MileageExpectedAnnualId;

                vehicleInfoRequest.UserId = predefinedLogInfo.UserID;
                vehicleInfoRequest.UserName = predefinedLogInfo.UserName;
                vehicleInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;

                Vehicle vehicleData = yakeenVehicleServices.GetVehicleEntity(vehicleInfoRequest.VehicleId, vehicleInfoRequest.VehicleIdTypeId, vehicleInfoRequest.IsOwnerTransfer, vehicleInfoRequest.OwnerNin.ToString());
                Integration.Dto.Yakeen.VehicleYakeenModel vehicle = null;

                if (vehicleData == null)
                {
                    var vehicleYakeenRequest = new VehicleYakeenRequestDto()
                    {
                        VehicleId = vehicleInfoRequest.VehicleId,
                        VehicleIdTypeId = vehicleInfoRequest.VehicleIdTypeId,
                        ModelYear = vehicleInfoRequest.ModelYear,
                        OwnerNin = vehicleInfoRequest.OwnerNin
                    };
                    var vehicleInfoFromYakeen = _yakeenClient.GetVehicleInfo(vehicleYakeenRequest, predefinedLogInfo);
                    if (vehicleInfoFromYakeen != null && vehicleInfoFromYakeen.Success)
                    {
                        VehiclePlateYakeenInfoDto vehiclePlateInfoFromYakeen = null;
                        if (vehicleInfoRequest.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
                        {
                            vehiclePlateInfoFromYakeen = _yakeenClient.GetVehiclePlateInfo(vehicleYakeenRequest, predefinedLogInfo);
                            if (vehiclePlateInfoFromYakeen == null || !vehiclePlateInfoFromYakeen.Success)
                            {
                                vehicle = new Integration.Dto.Yakeen.VehicleYakeenModel();
                                vehicle.Success = false;
                                vehicle.Error = vehicleInfoFromYakeen.Error.ToModel();
                            }
                        }

                        if (vehicle == null || vehicle.Error == null)
                        {
                            var vehicleColor = vehicleService.GetVehicleColor(vehicleInfoFromYakeen.MajorColor);
                            if (vehicleColor != null)
                            {
                                vehicleInfoRequest.MajorColor = vehicleColor.YakeenColor;
                                vehicleInfoRequest.ColorCode = vehicleColor.YakeenCode;
                            }
                            else
                            {
                                vehicleInfoRequest.MajorColor = vehicleInfoFromYakeen.MajorColor;
                                vehicleInfoRequest.ColorCode = 99;
                            }

                            vehicleData = yakeenVehicleServices.InsertVehicleInfoIntoDb(vehicleInfoRequest, vehicleInfoFromYakeen, vehiclePlateInfoFromYakeen);

                            // save yakeen vehicle data
                            YakeenVehicles yakeenVehicles = new YakeenVehicles();
                            yakeenVehicles.ID = vehicleData.ID;
                            if (vehiclePlateInfoFromYakeen != null && !string.IsNullOrEmpty(vehiclePlateInfoFromYakeen.ChassisNumber))
                            {
                                yakeenVehicles.ChassisNumber = vehiclePlateInfoFromYakeen.ChassisNumber;
                            }
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
                            if (vehicleInfoRequest.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
                            {
                                yakeenVehicles.SequenceNumber = vehicleYakeenRequest.VehicleId.ToString();
                            }
                            else
                            {
                                yakeenVehicles.CustomCardNumber = vehicleYakeenRequest.VehicleId.ToString();
                            }
                            yakeenVehicles.CreatedDate = DateTime.Now;
                            _yakeenVehiclesRepository.Insert(yakeenVehicles);
                            // end 

                            vehicle = vehicleData.ToVehicleYakeenModel();
                            vehicle.Success = true;
                        }
                    }
                    else
                    {
                        vehicle = new Integration.Dto.Yakeen.VehicleYakeenModel();
                        vehicle.Success = false;
                        if (vehicleInfoFromYakeen != null && vehicleInfoFromYakeen.Error != null)
                            vehicle.Error = vehicleInfoFromYakeen.Error.ToModel();
                    }
                }
                else
                {
                    Vehicle vehicleInfo = new Vehicle();
                    vehicleInfo.ID = Guid.NewGuid();
                    // //user inputs
                    vehicleInfo.OwnerTransfer = vehicleInfoRequest.IsOwnerTransfer;
                    vehicleInfo.VehicleValue = vehicleInfoRequest.VehicleValue;
                    vehicleInfo.IsUsedCommercially = vehicleInfoRequest.IsUsedCommercially;
                    vehicleInfo.TransmissionTypeId = vehicleInfoRequest.TransmissionTypeId;
                    vehicleInfo.ParkingLocationId = vehicleInfoRequest.ParkingLocationId;
                    vehicleInfo.HasModifications = vehicleInfoRequest.HasModification;
                    vehicleInfo.ModificationDetails = vehicleInfoRequest.Modification;
                    vehicleInfo.MileageExpectedAnnualId = vehicleInfoRequest.MileageExpectedAnnualId;
                    vehicleInfo.VehicleIdTypeId = vehicleInfoRequest.VehicleIdTypeId;
                    vehicleInfo.CarOwnerNIN = vehicleInfoRequest.OwnerNin.ToString();
                    vehicleInfo.BrakeSystemId = (BrakingSystem?)vehicleInfoRequest.BrakeSystemId;
                    vehicleInfo.CruiseControlTypeId = (CruiseControlType?)vehicleInfoRequest.CruiseControlTypeId;
                    vehicleInfo.ParkingSensorId = (ParkingSensors?)vehicleInfoRequest.ParkingSensorId;
                    vehicleInfo.CameraTypeId = (VehicleCameraType?)vehicleInfoRequest.CameraTypeId;
                    vehicleInfo.CurrentMileageKM = vehicleInfoRequest.CurrentMileageKM;
                    vehicleInfo.HasAntiTheftAlarm = vehicleInfoRequest.HasAntiTheftAlarm;
                    vehicleInfo.HasFireExtinguisher = vehicleInfoRequest.HasFireExtinguisher;
                    //end of user inputs 

                    vehicleInfo.Cylinders = vehicleData.Cylinders;

                    if (vehicleData.LicenseExpiryDate != null)
                    {
                        vehicleInfo.LicenseExpiryDate = Utilities.HandleHijriDate(vehicleData.LicenseExpiryDate);
                        vehicleInfo.LicenseExpiryDate = Utilities.FormatDateString(vehicleInfo.LicenseExpiryDate);
                    }

                    vehicleInfo.MinorColor = vehicleData.MinorColor;
                    var vehicleColor = vehicleService.GetVehicleColor(vehicleData.MajorColor);
                    if (vehicleColor != null)
                    {
                        vehicleInfo.MajorColor = vehicleColor.YakeenColor;
                        vehicleInfo.ColorCode = vehicleColor.YakeenCode;
                    }
                    else
                    {
                        vehicleInfo.MajorColor = vehicleData.MajorColor;
                        if (vehicleData.ColorCode.HasValue)
                            vehicleInfo.ColorCode = vehicleData.ColorCode.Value;
                        else
                            vehicleInfo.ColorCode = 99;

                    }

                    vehicleInfo.ModelYear = vehicleData.ModelYear;
                    vehicleInfo.PlateTypeCode = vehicleData.PlateTypeCode;
                    vehicleInfo.RegisterationPlace = vehicleData.RegisterationPlace;
                    vehicleInfo.VehicleBodyCode = vehicleData.VehicleBodyCode;
                    vehicleInfo.VehicleWeight = vehicleData.VehicleWeight;
                    vehicleInfo.VehicleLoad = vehicleData.VehicleLoad;
                    vehicleInfo.VehicleMaker = vehicleData.VehicleMaker;
                    vehicleInfo.VehicleModel = vehicleData.VehicleModel;
                    vehicleInfo.VehicleMakerCode = vehicleData.VehicleMakerCode;
                    vehicleInfo.VehicleModelCode = vehicleData.VehicleModelCode;
                    vehicleInfo.ChassisNumber = vehicleData.ChassisNumber;
                    vehicleInfo.SequenceNumber = vehicleData.SequenceNumber;
                    vehicleInfo.CustomCardNumber = vehicleData.CustomCardNumber;
                    vehicleInfo.CarPlateText1 = vehicleData.CarPlateText1;
                    vehicleInfo.CarPlateText2 = vehicleData.CarPlateText2;
                    vehicleInfo.CarPlateText3 = vehicleData.CarPlateText3;
                    vehicleInfo.CarPlateNumber = vehicleData.CarPlateNumber;
                    vehicleInfo.CarOwnerName = vehicleData.CarOwnerName;

                    string exception = string.Empty;
                    if (!yakeenVehicleServices.InsertVehicleIntoDb(vehicleInfo, out exception))
                    {
                        var vehicleYakeenModel = new VehicleYakeenModel();
                        vehicleYakeenModel.Error = new YakeenInfoErrorModel();
                        vehicleYakeenModel.Error.ErrorCode = "2";
                        vehicleYakeenModel.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                        vehicleYakeenModel.Error.ErrorDescription = exception;
                        return vehicleYakeenModel;
                    }
                    vehicle = vehicleInfo.ToVehicleYakeenModel();
                    vehicle.Success = true;
                }
                return vehicle;
            }
            catch (Exception ex)
            {
                var vehicleYakeenModel = new VehicleYakeenModel();
                vehicleYakeenModel.Error = new YakeenInfoErrorModel();
                vehicleYakeenModel.Error.ErrorCode = "0";
                vehicleYakeenModel.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                vehicleYakeenModel.Error.ErrorDescription = ex.ToString();
                return vehicleYakeenModel;
            }
        }

        public DriversOutput GetDriversData(List<DriverModel> drivers, InsuredModel insured, bool? isCustomerSpecialNeed, long mainDriverNin, ServiceRequestLog predefinedLogInfo)
        {
            DriversOutput output = new DriversOutput();
            try
            {
                output.AdditionalDrivers = new List<Driver>();
                foreach (var driver in drivers)
                {
                    if (driver.NationalId.StartsWith("7"))
                        continue;

                    var customerInfoRequest = new CustomerYakeenInfoRequestModel();
                    customerInfoRequest.Nin = long.Parse(driver.NationalId);
                    customerInfoRequest.BirthMonth = driver.BirthDateMonth;
                    customerInfoRequest.BirthYear = driver.BirthDateYear;
                    customerInfoRequest.MedicalConditionId = driver.MedicalConditionId;
                    customerInfoRequest.ViolationIds = driver.ViolationIds;
                    customerInfoRequest.NOALast5Years = driver.DriverNOALast5Years;
                    customerInfoRequest.EducationId = driver.EducationId;
                    customerInfoRequest.DrivingPercentage = driver.DrivingPercentage;
                    customerInfoRequest.ChildrenBelow16Years = driver.ChildrenBelow16Years;
                    customerInfoRequest.WorkCityId = driver.DriverWorkCityCode;
                    customerInfoRequest.CityId = driver.DriverHomeCityCode;
                    customerInfoRequest.CityName = driver.DriverHomeCity;
                    customerInfoRequest.WorkCityName = driver.DriverWorkCity;

                    if (driver.NationalId == mainDriverNin.ToString()) // main Driver
                    {
                        customerInfoRequest.IsSpecialNeed = isCustomerSpecialNeed.HasValue ? isCustomerSpecialNeed.Value : false;
                    }

                    if (driver.DriverExtraLicenses != null && driver.DriverExtraLicenses.Any())
                    {
                        customerInfoRequest.DriverExtraLicenses = driver.DriverExtraLicenses
                                            .Select(e => new Integration.Dto.Yakeen.DriverExtraLicenseModel
                                            {
                                                CountryId = e.CountryId,
                                                LicenseYearsId = e.LicenseYearsId
                                            }).ToList();

                    }

                    customerInfoRequest.UserId = predefinedLogInfo.UserID;
                    customerInfoRequest.UserName = predefinedLogInfo.UserName;
                    customerInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;

                    long NIN = 0;
                    long.TryParse(driver.NationalId, out NIN);
                    Driver customerData = customerServices.getDriverEntityFromNin(NIN);
                    predefinedLogInfo.DriverNin = NIN.ToString();
                    // Customer Yakeen Handle
                    Driver driverToInsert = null;
                    if (customerData == null)
                    {
                        var yakeenDriveroutput = GetDriverYakeenInfo(customerInfoRequest, predefinedLogInfo);

                        if (yakeenDriveroutput.ErrorCode == DriverOutput.ErrorCodes.ServiceException)
                        {
                            output.ErrorCode = DriversOutput.ErrorCodes.ServiceException;
                            output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                            output.LogErrorDescription = "GetCustomerYakeenInfo returned  " + yakeenDriveroutput.ErrorDescription;
                            return output;
                        }
                        if (yakeenDriveroutput.ErrorCode != DriverOutput.ErrorCodes.Success)
                        {
                            if (yakeenDriveroutput.Error != null && yakeenDriveroutput.Error.ErrorMessage != null
                                && ((yakeenDriveroutput.Error.Type == EErrorType.YakeenError ||
                                (yakeenDriveroutput.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))))
                            {
                                string ErrorMessage = SubmitInquiryResource.ResourceManager.GetString($"YakeenError_{yakeenDriveroutput.Error?.ErrorCode}", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                                var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                                output.ErrorCode = DriversOutput.ErrorCodes.ServiceException;
                                output.ErrorDescription = GenericErrorMessage;
                                output.LogErrorDescription = $"GetCustomerYakeenInfo returned code:{yakeenDriveroutput.Error?.ErrorCode} | message:{yakeenDriveroutput.Error.ErrorMessage} | description: {yakeenDriveroutput.Error.ErrorDescription}";
                            }
                            else
                            {
                                output.ErrorCode = DriversOutput.ErrorCodes.ServiceDown;
                                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                                output.LogErrorDescription = "GetCustomerYakeenInfo return error " + yakeenDriveroutput.ErrorDescription;
                            }
                            return output;
                        }
                        driverToInsert = yakeenDriveroutput.Driver;
                    }
                    else
                    {
                        Driver driverObj = new Driver();
                        driverObj.DriverId = Guid.NewGuid();
                        driverObj.IsSpecialNeed = customerInfoRequest.IsSpecialNeed;
                        driverObj.MedicalConditionId = customerInfoRequest.MedicalConditionId;
                        driverObj.EducationId = customerInfoRequest.EducationId;
                        driverObj.ChildrenBelow16Years = customerInfoRequest.ChildrenBelow16Years;
                        if (insured.NationalId.StartsWith("7"))
                        {
                            driverObj.DrivingPercentage = 100;
                        }
                        else
                        {
                            driverObj.DrivingPercentage = customerInfoRequest.DrivingPercentage;
                        }
                        driverObj.NOALast5Years = customerInfoRequest.NOALast5Years;
                        if (customerInfoRequest.DriverExtraLicenses != null && customerInfoRequest.DriverExtraLicenses.Any())
                        {
                            driverObj.ExtraLicenses = JsonConvert.SerializeObject(customerInfoRequest.DriverExtraLicenses);
                            driverObj.DriverExtraLicenses = new List<DriverExtraLicense>();
                            foreach (var l in customerInfoRequest.DriverExtraLicenses)
                            {
                                driverObj.DriverExtraLicenses.Add(new DriverExtraLicense
                                {
                                    CountryCode = l.CountryId,
                                    LicenseYearsId = l.LicenseYearsId,
                                    DriverId = driverObj.DriverId
                                });
                            }
                        }
                        if (customerInfoRequest.WorkCityId.HasValue)
                            driverObj.WorkCityId = customerInfoRequest.WorkCityId;

                        //driverObj.CityId = customerData.CityId;
                        driverObj.NIN = customerData.NIN;
                        driverObj.IsCitizen = customerData.IsCitizen;
                        driverObj.FirstName = customerData.FirstName;
                        driverObj.SecondName = customerData.SecondName;
                        driverObj.ThirdName = customerData.ThirdName;
                        driverObj.LastName = customerData.LastName;
                        driverObj.EnglishFirstName = customerData.EnglishFirstName;
                        driverObj.EnglishSecondName = customerData.EnglishSecondName;
                        driverObj.EnglishThirdName = customerData.EnglishThirdName;
                        driverObj.EnglishLastName = customerData.EnglishLastName;
                        driverObj.SubtribeName = customerData.SubtribeName;
                        driverObj.GenderId = customerData.GenderId > 2 ? 1 : customerData.GenderId;
                        driverObj.DateOfBirthG = customerData.DateOfBirthG;
                        driverObj.NationalityCode = customerData.NationalityCode;
                        driverObj.DateOfBirthH = Utilities.HandleHijriDate(customerData.DateOfBirthH);
                        driverObj.IdIssuePlace = customerData.IdIssuePlace;
                        driverObj.IdExpiryDate = customerData.IdExpiryDate;
                        driverObj.ResidentOccupation = customerData.ResidentOccupation;

                        driverObj.SocialStatusId = customerData.SocialStatusId;

                        if (driverObj.SocialStatusId.HasValue && string.IsNullOrEmpty(customerData.SocialStatusName))
                        {
                            driverObj.SocialStatusName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<SocialStatus>(customerData.SocialStatusId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
                        }
                        else
                        {
                            driverObj.SocialStatusName = customerData.SocialStatusName;
                        }

                        //driverObj.OccupationId = customerData.OccupationId;
                        if (!string.IsNullOrEmpty(customerData.OccupationCode) && customerData.OccupationCode == "G" && customerData.CreatedDateTime <= new DateTime(2020, 10, 13))
                        {
                            var yakeenResponse = YakeenRequestLogDataAccess.GetYakeenResponseByNIN(string.Empty, driverObj.NIN);

                            if (yakeenResponse != null && yakeenResponse.ServiceResponse != null)
                            {
                                if (driverObj.NIN.ToString().StartsWith("1"))
                                {
                                    var citizenIdInfo = JsonConvert.DeserializeObject<citizenIDInfoResult>(yakeenResponse.ServiceResponse);
                                    if (string.IsNullOrEmpty(citizenIdInfo.occupationCode))
                                    {
                                        driverObj.OccupationId = 2;
                                        driverObj.OccupationName = "غير ذالك";
                                        driverObj.OccupationCode = "O";
                                    }
                                }
                                else if (driverObj.NIN.ToString().StartsWith("2"))
                                {
                                    var alianInfo = JsonConvert.DeserializeObject<alienInfoByIqamaResult>(yakeenResponse.ServiceResponse);
                                    if (string.IsNullOrEmpty(alianInfo.occupationDesc))
                                    {
                                        if (driverObj.Gender == Gender.Female)
                                        {
                                            driverObj.OccupationId = 3849;
                                            driverObj.OccupationName = "موظفة ادارية";
                                            driverObj.OccupationCode = "31010";
                                        }
                                        else
                                        {
                                            driverObj.OccupationId = 270;
                                            driverObj.OccupationName = "موظف اداري";
                                            driverObj.OccupationCode = "31010";
                                        }
                                    }
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(customerData.OccupationCode))
                        {
                            driverObj.OccupationName = customerData.OccupationName;
                            driverObj.OccupationCode = customerData.OccupationCode;
                            driverObj.OccupationId = customerData.OccupationId;
                        }
                        else
                        {
                            var occupation = occupationService.GetOccupations().Where(x => x.ID == customerData.OccupationId).FirstOrDefault();
                            if (occupation != null)
                            {
                                driverObj.OccupationName = occupation?.NameAr;
                                driverObj.OccupationCode = occupation?.Code;
                            }
                            else if (driverObj.NIN.ToString().StartsWith("1"))
                            {
                                driverObj.OccupationId = 2;
                                driverObj.OccupationName = "غير ذالك";
                                driverObj.OccupationCode = "O";
                            }
                            else if (driverObj.NIN.ToString().StartsWith("2"))
                            {
                                if (driverObj.Gender == Gender.Female)
                                {
                                    driverObj.OccupationId = 3849;
                                    driverObj.OccupationName = "موظفة ادارية";
                                    driverObj.OccupationCode = "31010";
                                }
                                else
                                {
                                    driverObj.OccupationId = 270;
                                    driverObj.OccupationName = "موظف اداري";
                                    driverObj.OccupationCode = "31010";
                                }
                            }
                        }

                        if (driver.ViolationIds != null && driver.ViolationIds.Count > 0)
                        {
                            driverObj.Violations = JsonConvert.SerializeObject(driver.ViolationIds);
                        }
                        string exception = string.Empty;
                        var driverLicenses = customerServices.GetDriverlicensesByNin(customerData.NIN, out exception);
                        if (driverLicenses != null)
                        {
                            List<LicenseModel> licenseList = new List<LicenseModel>();
                            var liscenceTypes = _licenseTypeRepository.TableNoTracking.ToList();

                            var item = OrderingDriverLicensesByExpriyDate(driverLicenses);
                            if (item != null)
                            {
                                LicenseModel license = new LicenseModel();
                                license.DriverId = item.DriverId;
                                license.ExpiryDateH = Utilities.HandleHijriDate(item.ExpiryDateH);
                                license.IssueDateH = Utilities.HandleHijriDate(item.IssueDateH);
                                if (item.TypeDesc.HasValue)
                                {
                                    license.TypeDesc = item.TypeDesc;
                                }
                                else
                                {
                                    license.TypeDesc = 3;
                                    item.TypeDesc = 3;
                                }
                                if (item.TypeDesc.HasValue && item.TypeDesc.Value == 1
                                    && item.ExpiryDateH == "01-01-1449" && item.IssueDateH == "01-01-1441")
                                {
                                    license.TypeDesc = 3; // as per jira # 217
                                    item.TypeDesc = 3;
                                }

                                if (string.IsNullOrEmpty(item.licnsTypeDesc))
                                {
                                    item.licnsTypeDesc = liscenceTypes.Where(x => x.Code == item.TypeDesc).FirstOrDefault()?.ArabicDescription;
                                }
                                license.LicenseId = item.LicenseId;
                                licenseList.Add(license);
                                driverObj.DriverLicenses.Add(item);// this is for DB
                            }
                            int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                            driverObj.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                            driverObj.Licenses = JsonConvert.SerializeObject(licenseList);
                        }
                        else //this as per Jira #217 
                        {
                            int currentHijriYear = DateExtension.GetCurrenthijriYear();
                            int issueyear = currentHijriYear - 1;
                            DriverLicense driverLicense = new DriverLicense();
                            driverLicense.DriverId = driverObj.DriverId;
                            driverLicense.IssueDateH = "01-01-" + issueyear.ToString();
                            driverLicense.ExpiryDateH = "01-01-1449";
                            driverLicense.TypeDesc = 3;
                            driverObj.DriverLicenses.Add(driverLicense);
                            int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(driverLicense.IssueDateH).Year);
                            driverObj.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                            driverObj.Licenses = JsonConvert.SerializeObject(driverLicense);
                        }

                        driverObj.NOCLast5Years = customerData.NOCLast5Years;
                        driverObj.CreatedDateTime = DateTime.Now;
                        driverObj.IsDeleted = false;

                        var cities = addressService.GetAllCities();
                        if (driver.NationalId == mainDriverNin.ToString() && customerData.AddressId.HasValue)
                        {
                            driverObj.CityName = customerData.CityName;
                            driverObj.AddressId = customerData.AddressId;
                            driverObj.PostCode = customerData.PostCode;
                        }
                        if (driver.NationalId != mainDriverNin.ToString() && driver.DriverHomeCityCode.HasValue)
                        {
                            driverObj.CityId = driver.DriverHomeCityCode;
                            driverObj.CityName = cities.FirstOrDefault(c => c.Code == driverObj.CityId)?.ArabicDescription;
                        }

                        if (driver.NationalId != mainDriverNin.ToString() && driver.DriverWorkCityCode.HasValue)
                        {
                            driverObj.WorkCityId = driver.DriverWorkCityCode;
                            driverObj.WorkCityName = cities.FirstOrDefault(c => c.Code == driverObj.WorkCityId)?.ArabicDescription;
                        }

                        if (customerInfoRequest.EducationId > 0 && string.IsNullOrEmpty(customerData.EducationName))
                        {
                            driverObj.EducationName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<Education>(customerInfoRequest.EducationId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
                        }
                        else
                        {
                            driverObj.EducationName = customerData.EducationName;
                        }

                        driverToInsert = driverObj;
                    }

                    if (driverToInsert != null)
                    {
                        if (driver.NationalId == mainDriverNin.ToString()) // main Driver
                        {
                            if (!string.IsNullOrEmpty(driver.MobileNo))
                            {
                                driverToInsert.MobileNumber = driver.MobileNo;
                            }
                            output.MainDriver = driverToInsert;
                        }
                        else
                        {

                            driverToInsert.NCDFreeYears = 0;
                            driverToInsert.NCDReference = "0";
                            if (drivers.FirstOrDefault(a => a.NationalId == driverToInsert.NIN) != null &&
                                drivers.FirstOrDefault(a => a.NationalId == driverToInsert.NIN).RelationShipId > 0)
                            {
                                driverToInsert.RelationShipId = drivers.FirstOrDefault(a => a.NationalId == driverToInsert.NIN).RelationShipId.Value;
                            }
                            else
                            {
                                driverToInsert.RelationShipId = 0;
                            }
                            output.AdditionalDrivers.Add(driverToInsert);
                        }
                        driverRepository.Insert(driverToInsert);
                    }
                }

                output.ErrorCode = DriversOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = DriversOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogErrorDescription = exp.ToString();
                return output;
            }
        }

        public DriverOutput GetDriverYakeenInfo(CustomerYakeenInfoRequestModel customerInfoRequest, ServiceRequestLog predefinedLogInfo)
        {
            DriverOutput output = new DriverOutput();
            try
            {
                var customerYakeenRequest = new CustomerYakeenRequestDto()
                {
                    Nin = customerInfoRequest.Nin,
                    IsCitizen = customerInfoRequest.Nin.ToString().StartsWith("1"),
                    DateOfBirth = string.Format("{0}-{1}", customerInfoRequest.BirthMonth.ToString("00"), customerInfoRequest.BirthYear)
                };

                var customerIdInfo = _yakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedLogInfo);

                if (customerIdInfo.Success)
                {
                    Driver customerData = customerServices.PrepareDriverInfo(customerInfoRequest, customerIdInfo);
                    //keep yakeen info in a separate table 
                    YakeenDrivers yakeenDrivers = new YakeenDrivers();
                    yakeenDrivers.DateOfBirthG = customerIdInfo.DateOfBirthG;
                    yakeenDrivers.DateOfBirthH = customerIdInfo.DateOfBirthH;
                    yakeenDrivers.DriverId = customerData.DriverId;
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
                    yakeenDrivers.LicenseList = JsonConvert.SerializeObject(customerIdInfo.licenseListListField);
                    _yakeenDriversRepository.Insert(yakeenDrivers);
                    //end

                    output.Driver = customerData;
                    output.ErrorCode = DriverOutput.ErrorCodes.Success;
                    return output;
                }
                else
                {
                    output.Error = customerIdInfo.Error.ToModel();
                    output.ErrorCode = DriverOutput.ErrorCodes.ServiceFail;
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = DriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public DriverCityInfoOutput GetDriverCityInfo(Guid driverId, string driverNin, string vechileId, string channel, string birthDate, bool fromYakeen, string externalId)
        {
            DriverCityInfoOutput output = new DriverCityInfoOutput();
            DriverCityInfoModel driverCityInfoModel = new DriverCityInfoModel();
            try
            {
                var mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
                bool getAddressFromYakeen = false;
                var benchmarkDate = DateTime.Now.AddDays(-120);
                if (mainDriverAddress != null)
                {
                    var addressesWithin30Days = mainDriverAddress.Where(a => a.CreatedDate > benchmarkDate).ToList();
                    if (addressesWithin30Days == null || !addressesWithin30Days.Any())
                    {
                        getAddressFromYakeen = true;
                    }
                }
                if (mainDriverAddress == null || !mainDriverAddress.Any() || getAddressFromYakeen)
                {
                    var yakeenAddressOutput = GetYakeenAddress(driverId, driverNin, vechileId, channel, birthDate, externalId);
                    if (mainDriverAddress == null)
                    {
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                        {
                            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NoAddressFound;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse)
                        {
                            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoLookupForZipCode)
                        {
                            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NoLookupForZipCode;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.YakeenResultFailed;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            return output;
                        }
                        addressService.InsertAddresses(yakeenAddressOutput.DriverAddresses);
                        output.Addresses = yakeenAddressOutput.DriverAddresses;
                        mainDriverAddress = output.Addresses;
                    }
                    else if (mainDriverAddress != null && yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.Success)
                    {
                        string exception = string.Empty;
                        addressService.DeleteAllAddress(driverNin, benchmarkDate, out exception);  //mark old as deleted
                        addressService.InsertAddresses(yakeenAddressOutput.DriverAddresses);
                        output.Addresses = yakeenAddressOutput.DriverAddresses;
                        mainDriverAddress = output.Addresses;
                    }
                }
                if (mainDriverAddress == null || !mainDriverAddress.Any())
                {
                    output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NoAddressesFoundInSaudiPostOrInDB;
                    output.ErrorDescription = "No Addresses Found In SaudiPost Or In DB";
                    return output;
                }
                mainDriverAddress = mainDriverAddress.OrderByDescending(a => a.IsPrimaryAddress).ToList();
                int addressId = 0;
                string postCode = string.Empty;
                CityCenter nationalAddressCity = null;
                foreach (Address address in mainDriverAddress)
                {
                    nationalAddressCity = addressService.GetCityCenters().Where(a => a.CityId == address.CityId).FirstOrDefault();
                    if (nationalAddressCity != null && !string.IsNullOrEmpty(address.PostCode))
                    {
                        addressId = address.Id;
                        postCode = address.PostCode;
                        break;
                    }
                }
                //var nationalAddressCity = addressService.GetCityCenters().Where(a => a.CityId == mainDriverAddress.FirstOrDefault().CityId).FirstOrDefault();
                if (nationalAddressCity == null)
                {
                    output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NationalAddressCityIsNull;
                    output.ErrorDescription = "National Address City Is Null for the city id : " + mainDriverAddress.FirstOrDefault().CityId;
                    return output;
                }
                long cityId = 0;
                if (!long.TryParse(nationalAddressCity.ELM_Code, out cityId))
                {
                    output.ErrorCode = DriverCityInfoOutput.ErrorCodes.ParseElmCodeError;
                    output.ErrorDescription = "Failed to parse ELM_Code as we received  : " + nationalAddressCity.ELM_Code;
                    return output;
                }
                output.ErrorCode = DriverCityInfoOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                driverCityInfoModel.ElmCode = cityId;
                driverCityInfoModel.AddressId = addressId;
                driverCityInfoModel.PostCode = postCode;
                driverCityInfoModel.CityNameAr = nationalAddressCity.ArabicName;
                driverCityInfoModel.CityNameEn = nationalAddressCity.EnglishName;
                output.Output = driverCityInfoModel;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = DriverCityInfoOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Address exception: " + exp.ToString();
                return output;
            }
        }


        public YakeenAddressOutput GetYakeenAddress(Guid driverId, string driverNin, string vechileId, string channel, string birthDate, string externalId)
        {
            YakeenAddressOutput output = new YakeenAddressOutput();
            try
            {
                if (string.IsNullOrEmpty(driverNin))
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.DriverIsNull;
                    output.ErrorDescription = "Driver Nin Is Emty";
                    return output;
                }
                if (driverId == null)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.DriverIsNull;
                    output.ErrorDescription = "Driver Id Is Emty";
                    return output;
                }
                bool isCitizen = driverNin.StartsWith("1");
                output = _yakeenClient.GetYakeenAddress("0", driverNin, birthDate, "A", isCitizen, channel, vechileId, externalId);
                if (output.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = output.ErrorDescription;
                    return output;
                }
                if (output.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = output.ErrorDescription;
                    return output;
                }
                if (output.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = (YakeenAddressOutput.ErrorCodes)(int)output.ErrorCode;
                    output.ErrorDescription = output.ErrorDescription;
                    return output;
                }
                List<Address> addresses = new List<Address>();
                string zipcodes = string.Empty;
                foreach (var addressInfo in output.Addresses)
                {
                    zipcodes += addressInfo.PostCode + ",";
                    var yakeenCityCenter = addressService.GetYakeenCityCenterByZipCode(addressInfo.PostCode);
                    if (yakeenCityCenter == null)
                        continue;
                    Address address = new Address();
                    //address.Address1 = saudiPostAddress.Address1;
                    //address.Address2 = saudiPostAddress.Address2;
                    address.AdditionalNumber = addressInfo.AdditionalNumber.ToString();
                    address.BuildingNumber = addressInfo.BuildingNumber.ToString();
                    address.CityId = yakeenCityCenter.CityID.ToString();
                    address.City = addressInfo.City;
                    address.District = addressInfo.District;
                    address.DriverId = driverId;
                    address.IsPrimaryAddress = "True";
                    if (addressInfo.LocationCoordinates.Split(' ').Count() == 2)
                    {
                        address.Latitude = addressInfo.LocationCoordinates.Split(' ')[0];
                        address.Longitude = addressInfo.LocationCoordinates.Split(' ')[1];
                    }
                    address.ObjLatLng = addressInfo.LocationCoordinates;
                    //address.PKAddressID = saudiPostAddress.PKAddressID;
                    //address.PolygonString = saudiPostAddress.PolygonString?.ToString();
                    address.PostCode = addressInfo.PostCode.ToString();
                    address.RegionId = yakeenCityCenter.RegionID.ToString();
                    address.RegionName = yakeenCityCenter.RegionArabicName;
                    //address.Restriction = saudiPostAddress.Restriction;
                    address.Street = addressInfo.StreetName;
                    // address.Title = saudiPostAddress.Title?.ToString();
                    address.UnitNumber = addressInfo.UnitNumber.ToString();
                    address.NationalId = driverNin;
                    address.CreatedDate = DateTime.Now;
                    addresses.Add(address);
                }
                if (output.Addresses.Count() > 0 && addresses.Count == 0)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoLookupForZipCode;
                    output.ErrorDescription = "no lookup found for these zipcodes " + zipcodes;
                    return output;
                }
                output.DriverAddresses = addresses;
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = YakeenAddressOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }
        public VehicleModel ConvertVehicleYakeenToVehicle(VehicleYakeenModel vehicleYakeenModel)
        {
            VehicleModel vehicleModel = new VehicleModel();
            vehicleModel.CarPlateText1 = vehicleYakeenModel.CarPlateText1;
            vehicleModel.CarPlateText2 = vehicleYakeenModel.CarPlateText2;
            vehicleModel.CarPlateText3 = vehicleYakeenModel.CarPlateText3;
            vehicleModel.CarPlateNumber = vehicleYakeenModel.CarPlateNumber;
            vehicleModel.PlateTypeCode = vehicleYakeenModel.PlateTypeCode;
            vehicleModel.Model = vehicleYakeenModel.Model;
            vehicleModel.MinorColor = vehicleYakeenModel.MinorColor;
            vehicleModel.MajorColor = vehicleYakeenModel.MajorColor;
            vehicleModel.VehicleMaker = vehicleYakeenModel.Maker;
            vehicleModel.ModelYear = vehicleYakeenModel.ModelYear;
            vehicleModel.VehicleMakerCode = vehicleYakeenModel.MakerCode;
            vehicleModel.VehicleModelYear = vehicleYakeenModel.ModelYear;

            return vehicleModel;
        }
        public InquiryResponseModel CheckYakeenMissingFields(InquiryResponseModel result, QuotationRequestRequiredFieldsModel quotationRequiredFieldsModel, bool isCustomCard)
        {
            ValidationContext vc = new ValidationContext(quotationRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            result.IsValidInquiryRequest = Validator.TryValidateObject(quotationRequiredFieldsModel, vc, vResults, true);
            if (!result.IsValidInquiryRequest)
            {
                if (isCustomCard)
                {
                    HandleYakeenMissingFieldWithCustomCard(vResults);
                }
                result.YakeenMissingFields = GetYakeenMissingFields(vResults, quotationRequiredFieldsModel);
            }
            if (result.YakeenMissingFields == null || result.YakeenMissingFields.Count == 0)
            {
                result.IsValidInquiryRequest = true;
            }
            return result;
        }

        public IEnumerable<string> GetYakeenMissingPropertiesName(QuotationRequestRequiredFieldsModel quotationRequestRequiredFieldsModel, bool isCustomCard)
        {
            ValidationContext vc = new ValidationContext(quotationRequestRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            bool isValid = Validator.TryValidateObject(quotationRequestRequiredFieldsModel, vc, vResults, true);
            var result = vResults.Select(x => x.MemberNames.FirstOrDefault());
            if (isCustomCard)
            {
                List<string> resultForCustom = new List<string>();
                foreach (var item in result)
                {
                    if (requiredFieldsInCustomOnly.Contains(item))
                        resultForCustom.Add(item);
                }
                return resultForCustom;
            }
            return result;
        }

        public bool IsUserEnteredAllYakeenMissingFields(QuotationRequestRequiredFieldsModel model, IEnumerable<string> yakeenMissingFieldsNamesInDb)
        {
            foreach (var missingFieldName in yakeenMissingFieldsNamesInDb)
            {
                //get the property
                var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(missingFieldName);
                if (propertyInfo != null)
                {
                    //get the value of property
                    var value = propertyInfo.GetValue(model);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private ICollection<ValidationResult> HandleYakeenMissingFieldWithCustomCard(ICollection<ValidationResult> vResults)
        {
            foreach (var item in vResults.ToList())
            {
                var memName = item.MemberNames.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(memName))
                {
                    if (!requiredFieldsInCustomOnly.Contains(memName))
                        vResults.Remove(item);
                }
            }

            return vResults;
        }
        private List<YakeenMissingFieldBase> GetYakeenMissingFields(ICollection<ValidationResult> vResults, QuotationRequestRequiredFieldsModel model, string lang = "")
        {
            var result = new List<YakeenMissingFieldBase>();

            foreach (var validationResult in vResults)
            {
                var propertyName = validationResult.MemberNames.FirstOrDefault();
                if (propertyName != null)
                {
                    var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(propertyName);
                    var fieldDetailAttribute = propertyInfo.GetCustomAttributes(typeof(FieldDetailAttribute), true).FirstOrDefault() as FieldDetailAttribute;
                    if (fieldDetailAttribute != null)
                    {
                        YakeenMissingFieldBase yakeenField = CreateYakeenField(propertyName, fieldDetailAttribute, model, lang);
                        var existInfo = result.Where(a => a.Key == yakeenField.Key).FirstOrDefault();
                        if (existInfo == null)
                        {
                            result.Add(yakeenField);
                        }
                    }
                }
            }
            return result;
        }
        private YakeenMissingFieldBase CreateYakeenField(string propertyName, FieldDetailAttribute fieldDetailAttribute, QuotationRequestRequiredFieldsModel model, string lang = "")
        {
            if (propertyName == "VehicleMaker")
            {
                propertyName = "VehicleMakerCode";
            }
            if (propertyName == "VehicleModelCode")
            {
                propertyName = "VehicleModel";
            }
            YakeenMissingFieldBase yakeenField = null;
            switch (fieldDetailAttribute.ControlType)
            {
                case ControlType.Dropdown:
                    yakeenField = new DropdownField
                    {
                        Key = propertyName,
                        Label = propertyName,
                        Options = GetYakeenFieldDataSourceByName(fieldDetailAttribute.DataSourceName, model, lang),
                        Required = IsYakeenFieldRequired(propertyName)
                    };
                    break;
                case ControlType.DatePicker:
                    yakeenField = new DatePickerField
                    {
                        Key = propertyName,
                        Label = propertyName,
                        Required = IsYakeenFieldRequired(propertyName)
                    };
                    break;
                case ControlType.Toggle:
                case ControlType.Textbox:
                    yakeenField = new TextboxField
                    {
                        Key = propertyName,
                        Label = propertyName,
                        Required = IsYakeenFieldRequired(propertyName)
                    };
                    break;
                default:
                    break;
            }

            return yakeenField;
        }
        private bool IsYakeenFieldRequired(string propertyName)
        {
            var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(propertyName);
            return Attribute.IsDefined(propertyInfo, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute));
        }

        private List<IdNamePairModel> GetYakeenFieldDataSourceByName(string dataSourceName, QuotationRequestRequiredFieldsModel model, string lang = "")
        {
            if (string.IsNullOrEmpty(lang))
                lang = webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? "ar" : "en";

            var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(dataSourceName);
            var value = propertyInfo.GetValue(model) as List<IdNamePairModel>;
            if (value == null || value.Count == 0)
            {
                switch (dataSourceName)
                {
                    case "Cities":
                        value = addressService.GetCities().Select(e => new IdNamePairModel()
                        {
                            Id = Convert.ToInt32(e.Code),
                            Name = lang == "ar" ? e.ArabicDescription : e.EnglishDescription
                        }
                        ).ToList();

                        break;
                    case "SocialStatus":
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<SocialStatus>().Select(e => e.ToModel()).ToList();
                        break;
                    case "Occupations":
                        value = occupationService.GetOccupations().Select(e => new IdNamePairModel
                        {
                            Id = e.ID,
                            Name = lang == "ar" ? e.NameAr : e.NameEn
                        }).ToList();
                        break;
                    case "Genders":
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Gender>().Select(e => e.ToModel()).ToList();
                        break;
                    case "ModelYears":
                        for (int i = 1990; i < 2020; i++)
                        {
                            value.Add(new IdNamePairModel { Id = i, Name = i.ToString() });
                        }
                        break;
                    case "VehiclePlateTypes":
                        value = vehicleService.GetVehiclePlateTypes()
                                .Select(e => new IdNamePairModel
                                {
                                    Id = e.Code,
                                    Name = lang == "ar" ? e.ArabicDescription : e.EnglishDescription
                                })
                                .ToList();
                        break;
                    case "VehicleBodyTypes":
                        value = vehicleService.VehicleBodyTypes()
                                .Select(e => new IdNamePairModel
                                {
                                    Id = Convert.ToInt32(e.YakeenCode),
                                    Name = lang == "ar" ? e.ArabicDescription : e.EnglishDescription
                                })
                                .ToList();
                        break;
                    case "VehicleMakers":
                        value = vehicleService.VehicleMakers()
                             .Select(e => new IdNamePairModel
                             {
                                 Id = e.Code,
                                 Name = lang == "ar" ? e.ArabicDescription : e.EnglishDescription
                             })
                                .ToList();
                        break;
                    case "VehicleLoads":                        var itemsLIst = new List<IdNamePairModel>();                        for (int i = 1; i <= 15; i++)                        {                            itemsLIst.Add(new IdNamePairModel() { Id = i, Name = i.ToString() });                        }                        value = itemsLIst;                        break;
                    case "VehicleModels":
                        if (model.VehicleMakerCode.HasValue)
                        {
                            value = vehicleService.VehicleModels(model.VehicleMakerCode.Value).Select(e => new IdNamePairModel
                            {
                                Id = Convert.ToInt32(e.Code),
                                Name = lang == "ar" ? e.ArabicDescription : e.EnglishDescription
                            }).ToList();
                        }

                        break;
                    default:
                        break;
                }
            }
            return value;
        }

        public InquiryOutput HandleYakeenMissingFields(InquiryOutput output, string lang = "")
        {
            //get quotation request
            var quotationRequest = GetQuotationRequest(output.InquiryResponseModel.QuotationRequestExternalId);
            if (quotationRequest == null)
            {
                // throw new TameenkEntityNotFoundException("quotationExternalId", "There is no quotation request with the given external id.");
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "There is no quotation request with the given external id.";
                return output;
            }

            //convert to model to validate that all yakeen data are not missing
            var quotationRequiredFieldsModel = quotationRequest.ToQuotationRequestRequiredFieldsModel();

            if (quotationRequiredFieldsModel.VehicleModel == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleModel))
            {
                quotationRequiredFieldsModel.VehicleModel = null;
            }

            if (quotationRequiredFieldsModel.VehicleMaker == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleMaker))
            {
                quotationRequiredFieldsModel.VehicleMaker = null;
            }

            if (quotationRequiredFieldsModel.VehicleModelCode == null || quotationRequiredFieldsModel.VehicleModelCode.Value < 1)
            {
                quotationRequiredFieldsModel.VehicleModelCode = null;
            }

            if (quotationRequiredFieldsModel.VehicleMakerCode == null || quotationRequiredFieldsModel.VehicleMakerCode.Value < 1)
            {
                quotationRequiredFieldsModel.VehicleMakerCode = null;
            }
            if (quotationRequiredFieldsModel.VehicleBodyCode == null || quotationRequiredFieldsModel.VehicleBodyCode < 1 || quotationRequiredFieldsModel.VehicleBodyCode > 21)
            {
                quotationRequiredFieldsModel.VehicleBodyCode = null;
            }
            if (quotationRequiredFieldsModel.VehiclePlateTypeCode == null || quotationRequiredFieldsModel.VehiclePlateTypeCode < 1)
            {
                quotationRequiredFieldsModel.VehiclePlateTypeCode = null;
            }
            if (quotationRequiredFieldsModel.VehicleLoad == null || quotationRequiredFieldsModel.VehicleLoad < 1)
            {
                quotationRequiredFieldsModel.VehicleLoad = null;
            }


            ValidationContext vc = new ValidationContext(quotationRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            output.InquiryResponseModel.IsValidInquiryRequest = Validator.TryValidateObject(quotationRequiredFieldsModel, vc, vResults, true);
            if (!output.InquiryResponseModel.IsValidInquiryRequest)
            {
                if (quotationRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
                {
                    HandleYakeenMissingFieldWithCustomCard(vResults);
                }
                output.InquiryResponseModel.YakeenMissingFields = GetYakeenMissingFields(vResults, quotationRequiredFieldsModel, lang);
            }

            if (output.InquiryResponseModel.YakeenMissingFields == null || output.InquiryResponseModel.YakeenMissingFields.Count == 0)
            {
                output.InquiryResponseModel.IsValidInquiryRequest = true;
            }


            return output;
        }
        public QuotationRequest GetQuotationRequest(string quotationExternalId)
        {
            return quotationRequestRepository.Table
                .Include(e => e.Vehicle).Include(e => e.Driver)
                .FirstOrDefault(e => e.ExternalId == quotationExternalId);

        }
        public string GetNewRequestExternalId()
        {
            string qtExtrnlId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (quotationRequestRepository.Table.Any(q => q.ExternalId == qtExtrnlId))
                return GetNewRequestExternalId();
            return qtExtrnlId;
        }
        public void HandleNajmException(Exception ex, ref InquiryOutput inquiryOutput)
        {
            inquiryOutput.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
            switch (ex.Message)
            {
                case "E101":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E101;
                    break;

                case "E102":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E102;
                    break;

                case "E103":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E103;
                    break;

                case "E104":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E104;
                    break;

                case "E105":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E105;
                    break;

                case "E106":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E106;
                    break;
                case "E107":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E107;
                    break;
                case "E108":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E108;
                    break;
                case "E109":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E109;
                    break;
                case "E110":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E110;
                    break;
                case "E111":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E111;
                    break;
                case "E112":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E112;
                    break;
                case "E113":
                    inquiryOutput.ErrorDescription = NajmExceptionResource.E113;
                    break;
                default:
                    inquiryOutput.ErrorDescription = NajmExceptionResource.GeneralException;
                    break;
            }
        }

        public int CopyAdditionalDriversToNewQuotationRequest(int quotationId, int initialQuotationId, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "CopyAdditionalDriversToNewQuotationRequest";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter quotationIdParameter = new SqlParameter() { ParameterName = "quotationId", Value = quotationId };
                SqlParameter initialQuotationIdParameter = new SqlParameter() { ParameterName = "initialQuotationId", Value = initialQuotationId };
                command.Parameters.Add(quotationIdParameter);
                command.Parameters.Add(initialQuotationIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return -1;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        private DriverLicense OrderingDriverLicensesByExpriyDate(List<DriverLicense> licenses)
        {
            try
            {
                DriverLicense license = null;
                if (licenses != null && licenses.Count > 0)
                {
                    license = new DriverLicense();
                    license = licenses.OrderByDescending(a => DateExtension.ConvertHijriStringToDateTime(a.ExpiryDateH)).FirstOrDefault();
                }

                return license;
            }
            catch (Exception ex)
            {
                return licenses.FirstOrDefault();
            }
        }
    }
}
