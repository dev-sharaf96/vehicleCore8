using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;


namespace Tameenk.Services.QuotationApi.Services
{
    public class RequestInitializer : IRequestInitializer
    {
        private readonly IAddressService _addressService;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<PromotionProgramUser> _promotionProgramUSerRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly TameenkConfig _config;

        public RequestInitializer(IAddressService addressService,
            IVehicleService vehicleService,
            IRepository<Driver> driverRepository,
            IRepository<PromotionProgramUser> promotionProgramUSerRepository
            , TameenkConfig config)
        {
            _addressService = addressService;
            _vehicleService = vehicleService;
            _promotionProgramUSerRepository = promotionProgramUSerRepository;
            _config = config;

            _driverRepository = driverRepository;
           
        }

        public QuotationServiceRequest GetQuotationRequestData(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue)
        {
            var serviceRequestMessage = new QuotationServiceRequest();

            //Random r = new Random();
            var cities = _addressService.GetAllCities();
            string vehicleColorCode = "99";
            string vehicleColor;
            //var sakakaDbCode = 9999;
            //var rightSakakaCode = 38;
            #region VehicleColor

            GetVehicleColor(out vehicleColor, out vehicleColorCode, quotationRequest.Vehicle.MajorColor);
            #endregion



            if (quotationRequest.Vehicle != null && !string.IsNullOrEmpty(quotationRequest.Vehicle.RegisterationPlace))
            {
                var info = _addressService.GetCityByName(cities,Utilities.RemoveWhiteSpaces(quotationRequest.Vehicle.RegisterationPlace));
                if (info != null)
                {
                    serviceRequestMessage.VehicleRegPlaceCode = info?.YakeenCode.ToString();
                }
                else
                {
                    serviceRequestMessage.VehicleRegPlaceCode = null;
                }
            }
            else
            {
                serviceRequestMessage.VehicleRegPlaceCode = null;
            }

            serviceRequestMessage.ReferenceId = quotationResponse.ReferenceId;
            serviceRequestMessage.ProductTypeCode = insuranceTypeCode;

            if (quotationRequest.RequestPolicyEffectiveDate.HasValue && quotationRequest.RequestPolicyEffectiveDate.Value <= DateTime.Now.Date)
            {
                serviceRequestMessage.PolicyEffectiveDate = DateTime.Now.Date.AddDays(1);

            }
            else
            {
                serviceRequestMessage.PolicyEffectiveDate = quotationRequest.RequestPolicyEffectiveDate.Value;
            }

            #region Insured
            serviceRequestMessage.InsuredIdTypeCode = quotationRequest.Insured.CardIdTypeId;
            serviceRequestMessage.InsuredId = long.Parse(quotationRequest.Insured.NationalId);
            serviceRequestMessage.InsuredBirthDate = quotationRequest.Insured.CardIdType == CardIdType.Citizen
                ? quotationRequest.Insured.BirthDateH
                : quotationRequest.Insured.BirthDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

            // Add two lines for medGulf Company Only 
            serviceRequestMessage.InsuredBirthDateG = quotationRequest.Insured.BirthDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
            serviceRequestMessage.InsuredBirthDateH = quotationRequest.Insured.BirthDateH;


            serviceRequestMessage.InsuredGenderCode = quotationRequest.Insured.Gender.GetCode();
            serviceRequestMessage.InsuredNationalityCode = quotationRequest.Insured.NationalityCode;
            serviceRequestMessage.InsuredFirstNameAr = quotationRequest.Insured.FirstNameAr;
            serviceRequestMessage.InsuredMiddleNameAr = $"{quotationRequest.Insured.MiddleNameAr}";
            serviceRequestMessage.InsuredLastNameAr = quotationRequest.Insured.LastNameAr;
            serviceRequestMessage.InsuredFirstNameEn = (string.IsNullOrEmpty(quotationRequest.Insured.FirstNameEn)
                || string.IsNullOrWhiteSpace(quotationRequest.Insured.FirstNameEn)) ? "-" : quotationRequest.Insured.FirstNameEn;
            serviceRequestMessage.InsuredMiddleNameEn = $"{quotationRequest.Insured.MiddleNameEn}";
            serviceRequestMessage.InsuredLastNameEn = (string.IsNullOrEmpty(quotationRequest.Insured.LastNameEn)
                || string.IsNullOrWhiteSpace(quotationRequest.Insured.LastNameEn)) ? "-" : quotationRequest.Insured.LastNameEn;
            serviceRequestMessage.InsuredSocialStatusCode = quotationRequest.Insured.SocialStatus?.GetCode();
            serviceRequestMessage.InsuredOccupationCode = quotationRequest.Insured.Occupation?.Code;
            serviceRequestMessage.InsuredOccupation = quotationRequest.Insured.Occupation?.NameAr.Trim();
            serviceRequestMessage.InsuredEducationCode = int.Parse(quotationRequest.Insured.Education.GetCode());
            serviceRequestMessage.InsuredChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
            serviceRequestMessage.InsuredWorkCity = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.ArabicDescription : "";
            serviceRequestMessage.InsuredWorkCityCode = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.YakeenCode.ToString() : "";
            serviceRequestMessage.InsuredIdIssuePlace = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.ArabicDescription : "";
            serviceRequestMessage.InsuredIdIssuePlaceCode = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.YakeenCode.ToString() : "";
            serviceRequestMessage.InsuredCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.ArabicDescription : "";
            serviceRequestMessage.InsuredCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.YakeenCode.ToString() : "";
            if (serviceRequestMessage.InsuredIdIssuePlaceCode == null&& serviceRequestMessage.InsuredCityCode!=null)
            {
                serviceRequestMessage.InsuredIdIssuePlaceCode = serviceRequestMessage.InsuredCityCode;
            }
            if (serviceRequestMessage.InsuredIdIssuePlace == null && serviceRequestMessage.InsuredCity != null)
            {
                serviceRequestMessage.InsuredIdIssuePlace = serviceRequestMessage.InsuredCity;
            }

            #endregion

            #region  Vehicle
            var isVehicleRegistered = quotationRequest.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber;
            if (isVehicleRegistered)
            {
                serviceRequestMessage.VehiclePlateNumber = quotationRequest.Vehicle.CarPlateNumber.HasValue ? quotationRequest.Vehicle.CarPlateNumber.Value : 0;
                serviceRequestMessage.VehiclePlateText1 = quotationRequest.Vehicle.CarPlateText1;
                serviceRequestMessage.VehiclePlateText2 = quotationRequest.Vehicle.CarPlateText2;
                serviceRequestMessage.VehiclePlateText3 = quotationRequest.Vehicle.CarPlateText3;
            }
            else
            {
                serviceRequestMessage.VehiclePlateNumber = null;
                serviceRequestMessage.VehiclePlateText1 = null;
                serviceRequestMessage.VehiclePlateText2 = null;
                serviceRequestMessage.VehiclePlateText3 = null;
            }

            //#endif


            serviceRequestMessage.VehicleIdTypeCode = quotationRequest.Vehicle.VehicleIdTypeId;
            //serviceRequestMessage.VehicleRegPlaceCode = regPlaceCode;
            serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.Vehicle.CarOwnerNIN);
            serviceRequestMessage.VehicleOwnerName = quotationRequest.Vehicle.CarOwnerName;
            serviceRequestMessage.VehiclePlateTypeCode = isVehicleRegistered ? quotationRequest.Vehicle.PlateTypeCode.ToString() : null;
            serviceRequestMessage.VehicleRegExpiryDate = isVehicleRegistered ? quotationRequest.Vehicle.LicenseExpiryDate : null;

            if (serviceRequestMessage.VehicleRegExpiryDate != null)
            {
                try
                {
                    if (serviceRequestMessage.VehicleRegExpiryDate?.Length < 10 && serviceRequestMessage.VehicleRegExpiryDate.Contains("-"))
                    {
                        var day = serviceRequestMessage.VehicleRegExpiryDate.Split('-')[0];
                        var month = serviceRequestMessage.VehicleRegExpiryDate.Split('-')[1];
                        var year = serviceRequestMessage.VehicleRegExpiryDate.Split('-')[2];
                        int d = 0;
                        int m = 0;
                        if (int.TryParse(serviceRequestMessage.VehicleRegExpiryDate.Split('-')[0], out d))
                        {
                            if (d < 10)
                            {
                                day = "0" + day;
                            }
                        }
                        if (int.TryParse(serviceRequestMessage.VehicleRegExpiryDate.Split('-')[1], out m))
                        {
                            if (m < 10)
                            {
                                month = "0" + month;
                            }
                        }
                        serviceRequestMessage.VehicleRegExpiryDate = day + "-" + month + "-" + year;
                    }
                }
                catch
                {

                }
            }
            if (serviceRequestMessage.VehicleRegExpiryDate == null)
            {
                try
                {
                    System.Globalization.DateTimeFormatInfo HijriDTFI;
                    HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                    HijriDTFI.Calendar = new System.Globalization.UmAlQuraCalendar();
                    HijriDTFI.ShortDatePattern = "dd-MM-yyyy";
                    DateTime dt = DateTime.Now;
                    serviceRequestMessage.VehicleRegExpiryDate = dt.ToString("dd-MM-yyyy", HijriDTFI);
                }
            catch
            {

            }
        }

            serviceRequestMessage.VehicleId = isVehicleRegistered ? long.Parse(quotationRequest.Vehicle.SequenceNumber) : long.Parse(quotationRequest.Vehicle.CustomCardNumber);
            serviceRequestMessage.VehicleModelYear = quotationRequest.Vehicle.ModelYear.Value;
            serviceRequestMessage.VehicleMaker = quotationRequest.Vehicle.VehicleMaker;
            serviceRequestMessage.VehicleMakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value.ToString() : "0";
            serviceRequestMessage.VehicleModel = quotationRequest.Vehicle.VehicleModel;
            serviceRequestMessage.VehicleModelCode = quotationRequest.Vehicle.VehicleModelCode.HasValue ? quotationRequest.Vehicle.VehicleModelCode.Value.ToString() : "0";


            serviceRequestMessage.VehicleMajorColor = vehicleColor;
            serviceRequestMessage.VehicleMajorColorCode = vehicleColorCode;
            serviceRequestMessage.VehicleBodyTypeCode = quotationRequest.Vehicle.VehicleBodyCode.ToString();

            serviceRequestMessage.VehicleRegPlace = quotationRequest.Vehicle.RegisterationPlace;
            serviceRequestMessage.VehicleCapacity = quotationRequest.Vehicle.VehicleLoad; //@TODO: Validate this
            serviceRequestMessage.VehicleCylinders = int.Parse(quotationRequest.Vehicle.Cylinders.Value.ToString());
            serviceRequestMessage.VehicleWeight = quotationRequest.Vehicle.VehicleWeight;
            serviceRequestMessage.VehicleLoad = quotationRequest.Vehicle.VehicleLoad;
            serviceRequestMessage.VehicleOwnerTransfer = quotationRequest.Vehicle.OwnerTransfer;
            serviceRequestMessage.DriverDisabled = quotationRequest.Driver.IsSpecialNeed ?? false;
            serviceRequestMessage.VehicleUsingWorkPurposes = quotationRequest.Vehicle.IsUsedCommercially.HasValue ? quotationRequest.Vehicle.IsUsedCommercially.Value : false;

            serviceRequestMessage.VehicleAgencyRepair = vehicleAgencyRepair;
            serviceRequestMessage.VehicleValue = quotationRequest.Vehicle.VehicleValue;
            serviceRequestMessage.DeductibleValue = insuranceTypeCode == 1 ? null : (int?)(deductibleValue.HasValue ? deductibleValue.Value : 1500);

            serviceRequestMessage.VehicleEngineSizeCode = int.Parse(quotationRequest.Vehicle.EngineSize?.GetCode());
            serviceRequestMessage.VehicleUseCode = int.Parse(quotationRequest.Vehicle.VehicleUse != null && quotationRequest.Vehicle.VehicleUse.GetCode().Equals("0") ? "1" : quotationRequest.Vehicle.VehicleUse.GetCode());
            serviceRequestMessage.VehicleMileage = (int?)quotationRequest.Vehicle.CurrentMileageKM;
            serviceRequestMessage.VehicleTransmissionTypeCode = int.Parse(quotationRequest.Vehicle.TransmissionType?.GetCode());

            if (quotationRequest.Vehicle.MileageExpectedAnnual != null)
            {
                int MileageExpectedAnnualId = 0;
                int.TryParse(quotationRequest.Vehicle.MileageExpectedAnnual?.GetCode(), out MileageExpectedAnnualId);
                serviceRequestMessage.VehicleMileageExpectedAnnualCode = MileageExpectedAnnualId;
            }
            serviceRequestMessage.VehicleAxleWeightCode = quotationRequest.Vehicle.AxleWeightId;
            serviceRequestMessage.VehicleAxleWeight = quotationRequest.Vehicle.AxleWeightId;
            serviceRequestMessage.VehicleOvernightParkingLocationCode = int.Parse(quotationRequest.Vehicle?.ParkingLocation.GetCode());
            serviceRequestMessage.VehicleModification = quotationRequest.Vehicle.HasModifications;
            serviceRequestMessage.VehicleModificationDetails = string.IsNullOrEmpty(quotationRequest.Vehicle.ModificationDetails) ? "" : quotationRequest.Vehicle.ModificationDetails;
            if (quotationRequest.Vehicle.VehicleSpecifications != null && quotationRequest.Vehicle.VehicleSpecifications.Count > 0)
            {
                serviceRequestMessage.VehicleSpecifications = quotationRequest.Vehicle.VehicleSpecifications
                               .Select(e => new VehicleSpecificationDto() { VehicleSpecificationCode = e.Code }).ToList();
            }


            #endregion

            serviceRequestMessage.NCDFreeYears = quotationRequest.NajmNcdFreeYears.HasValue ? quotationRequest.NajmNcdFreeYears.Value : 0;
            serviceRequestMessage.NCDReference = quotationRequest.NajmNcdRefrence;
            serviceRequestMessage.Drivers = CreateInsuranceCompanyDriversFromDataRequest(quotationRequest);


            serviceRequestMessage.PromoCode = string.IsNullOrWhiteSpace(userId) ? null : GetUserPromotionCode(userId, quotationResponse.InsuranceCompanyId, insuranceTypeCode);
            serviceRequestMessage.VehicleChassisNumber = quotationRequest.Vehicle.ChassisNumber;
            return serviceRequestMessage;
        }
        private List<DriverDto> CreateInsuranceCompanyDriversFromDataRequest(QuotationRequest quotationRequest)
        {
            List<DriverDto> drivers = new List<DriverDto>();
            int additionalDrivingPercentage = 0;
            //Create main driver as first driver in the drivers list
            var mainDriverDto = new DriverDto()
            {
                DriverTypeCode = 1,
                DriverId = long.Parse(quotationRequest.Insured.NationalId),
                DriverIdTypeCode = quotationRequest.Insured.CardIdTypeId,
                DriverBirthDate = quotationRequest.Insured.BirthDateH,
                DriverBirthDateG = quotationRequest.Insured.BirthDate,
                DriverFirstNameAr = quotationRequest.Insured.FirstNameAr,
                DriverFirstNameEn = (string.IsNullOrWhiteSpace(quotationRequest.Insured.FirstNameEn) ||
                string.IsNullOrEmpty(quotationRequest.Insured.FirstNameEn)) ? "-" : quotationRequest.Insured.FirstNameEn,
                DriverMiddleNameAr = quotationRequest.Insured.MiddleNameAr,
                DriverMiddleNameEn = quotationRequest.Insured.MiddleNameEn,
                DriverLastNameAr = quotationRequest.Insured.LastNameAr,
                DriverLastNameEn = (string.IsNullOrWhiteSpace(quotationRequest.Insured.LastNameEn) ||
                string.IsNullOrEmpty(quotationRequest.Insured.LastNameEn)) ? "-" : quotationRequest.Insured.LastNameEn,
                DriverNOALast5Years = quotationRequest.Driver.NOALast5Years,
                DriverNOCLast5Years = quotationRequest.Driver.NOCLast5Years,
                DriverNCDFreeYears = quotationRequest.Driver.NCDFreeYears,
                DriverNCDReference = quotationRequest.Driver.NCDReference
            };

            mainDriverDto.DriverGenderCode = quotationRequest.Insured.Gender.GetCode();
            mainDriverDto.DriverNationalityCode = !string.IsNullOrEmpty(quotationRequest.Insured.NationalityCode) ? quotationRequest.Insured.NationalityCode : "113";
            mainDriverDto.DriverSocialStatusCode = quotationRequest.Insured.SocialStatusId?.ToString();
            mainDriverDto.DriverOccupationCode = quotationRequest.Insured.Occupation?.Code;
            mainDriverDto.DriverOccupation = quotationRequest.Insured.Occupation?.NameAr.Trim();
            if (quotationRequest.Driver.DrivingPercentage < 100 && quotationRequest.Drivers == null && !quotationRequest.Drivers.Any())
            {
                mainDriverDto.DriverDrivingPercentage = 100;
            }
            else
            {
                mainDriverDto.DriverDrivingPercentage = quotationRequest.Driver.DrivingPercentage;
            }
            additionalDrivingPercentage = mainDriverDto.DriverDrivingPercentage.HasValue ? mainDriverDto.DriverDrivingPercentage.Value : 0; ;
            mainDriverDto.DriverEducationCode = quotationRequest.Insured.EducationId;
            mainDriverDto.DriverMedicalConditionCode = quotationRequest.Driver.MedicalConditionId;
            mainDriverDto.DriverChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
            mainDriverDto.DriverHomeCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.YakeenCode.ToString() : "";
            mainDriverDto.DriverHomeCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.ArabicDescription : "";
            mainDriverDto.DriverWorkCityCode = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.YakeenCode.ToString() : "";
            mainDriverDto.DriverWorkCity = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.ArabicDescription : "";

            var DriverLicenses = _driverRepository.Table
                .Include(x => x.DriverLicenses)
                .FirstOrDefault(x => x.NIN == quotationRequest.Insured.NationalId)?
                .DriverLicenses;

            var LicenseDtos = new List<LicenseDto>();

            if (DriverLicenses != null && DriverLicenses.Count() > 0)
            {
                foreach (var item in DriverLicenses)
                {
                    LicenseDtos.Add(new LicenseDto()
                    {
                        DriverLicenseExpiryDate = item.ExpiryDateH,
                        DriverLicenseTypeCode = item.TypeDesc.ToString(),
                        LicenseCountryCode = 113,
                        LicenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year)
                    });
                }
                mainDriverDto.DriverLicenses = LicenseDtos; //from tameenk
            }
            else
            {
                mainDriverDto.DriverLicenses = null; //from tameenk
            }

            if (quotationRequest.Driver.DriverViolations != null && quotationRequest.Driver.DriverViolations.Count > 0)
            {
                mainDriverDto.DriverViolations = quotationRequest.Driver.DriverViolations
                    .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();

            }
            //Add main driver to drivers list
            drivers.Add(mainDriverDto);
            //check if there are additional drivers, if yes then add them to drivers list
            if (quotationRequest.Drivers != null && quotationRequest.Drivers.Any())
            {
                var additionalDrivers = quotationRequest.Drivers.Where(e => e.NIN != mainDriverDto.DriverId.ToString());
                
                foreach (var additionalDriver in additionalDrivers)
                {
                    var driverDto = new DriverDto()
                    {
                        DriverTypeCode = 2,
                        DriverId = long.Parse(additionalDriver.NIN),
                        DriverIdTypeCode = additionalDriver.IsCitizen ? 1 : 2,
                        DriverBirthDate = additionalDriver.DateOfBirthH,
                        DriverBirthDateG = additionalDriver.DateOfBirthG,
                        DriverFirstNameAr = additionalDriver.FirstName,
                        DriverFirstNameEn = (string.IsNullOrEmpty(additionalDriver.EnglishFirstName)
                        || string.IsNullOrWhiteSpace(additionalDriver.EnglishFirstName)) ? "-" : additionalDriver.EnglishFirstName,
                        DriverMiddleNameAr = additionalDriver.SecondName,
                        DriverMiddleNameEn = additionalDriver.EnglishSecondName,
                        DriverLastNameAr = additionalDriver.LastName,
                        DriverLastNameEn = (string.IsNullOrEmpty(additionalDriver.EnglishLastName)
                        || string.IsNullOrWhiteSpace(additionalDriver.EnglishLastName)) ? "-" : additionalDriver.EnglishLastName,
                        DriverOccupation = additionalDriver.ResidentOccupation,
                        DriverNOALast5Years = additionalDriver.NOALast5Years,
                        DriverNOCLast5Years = additionalDriver.NOCLast5Years,
                        DriverNCDFreeYears = additionalDriver.NCDFreeYears,
                        DriverNCDReference = additionalDriver.NCDReference,
                    };
                    driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                    driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                            additionalDriver.NationalityCode.Value.ToString() : "113";
                    driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                    driverDto.DriverOccupation = additionalDriver.Occupation?.NameAr.Trim();
                    driverDto.DriverDrivingPercentage = additionalDriver.DrivingPercentage; // from tameenk
                    additionalDrivingPercentage += additionalDriver.DrivingPercentage.HasValue?additionalDriver.DrivingPercentage.Value:0;
                    driverDto.DriverEducationCode = additionalDriver.EducationId;
                    driverDto.DriverMedicalConditionCode = additionalDriver.MedicalConditionId;
                    driverDto.DriverChildrenBelow16Years = additionalDriver.ChildrenBelow16Years;
                    driverDto.DriverHomeCityCode = additionalDriver.City?.YakeenCode.ToString();
                    driverDto.DriverHomeCity = additionalDriver.City?.ArabicDescription;
                    driverDto.DriverWorkCityCode = additionalDriver.WorkCity?.YakeenCode.ToString();
                    driverDto.DriverWorkCity = additionalDriver.WorkCity?.ArabicDescription;
                    var additionalDriverLicenses = _driverRepository.Table
                            .Include(x => x.DriverLicenses)
                            .FirstOrDefault(x => x.NIN == additionalDriver.NIN)?
                            .DriverLicenses;

                    var additionalDriverLicenseDtos = new List<LicenseDto>();
                    if (additionalDriverLicenses != null && additionalDriverLicenses.Count() > 0)
                    {
                        foreach (var item in additionalDriverLicenses)
                        {
                            additionalDriverLicenseDtos.Add(new LicenseDto()
                            {
                                DriverLicenseExpiryDate = item.ExpiryDateH,
                                DriverLicenseTypeCode = item.TypeDesc.ToString(),
                                LicenseCountryCode = 113,
                                LicenseNumberYears = (DateTime.Now.Year - DateTime.Parse(item.IssueDateH).Year)
                            });
                        }
                        driverDto.DriverLicenses = additionalDriverLicenseDtos; //from tameenk
                    }
                    else
                    {
                        driverDto.DriverLicenses = null;
                    }

                    if (additionalDriver.DriverViolations != null && additionalDriver.DriverViolations.Count > 0)
                    {
                        driverDto.DriverViolations = additionalDriver.DriverViolations
                            .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();
                    }

                    drivers.Add(driverDto);
                }
            }
            if (additionalDrivingPercentage > 100 && drivers.Count() > 1)
            {
                int numberOfDriver = drivers.Count();
                if (drivers.Count() > 4)
                    numberOfDriver = 4;
                int percentage = 0;
                int mainPercentage = 0;

                if (numberOfDriver == 4)
                {
                    percentage = mainPercentage = 25;
                }
                else if (numberOfDriver == 3)
                {
                    percentage = 25;
                    mainPercentage = 50;
                }
                else if (numberOfDriver == 2)
                {
                    percentage = mainPercentage= 50;
                }
                foreach (var d in drivers)
                {
                    if (d.DriverTypeCode == 1)
                        d.DriverDrivingPercentage = mainPercentage;
                    else
                        d.DriverDrivingPercentage = percentage;
                }
            }
            return drivers;
        }

        private string GetUserPromotionCode(string userId, int insuranceCompanyId, int insuranceTypeCode)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User id can't be empty.");
            if (insuranceCompanyId < 1)
                throw new TameenkArgumentNullException(nameof(insuranceCompanyId), "Insurance company id can't be less than 1.");

            var progUser = _promotionProgramUSerRepository.Table
                .Include(e => e.PromotionProgram.PromotionProgramCodes)
                .FirstOrDefault(e => e.UserId == userId && e.EmailVerified == true
                                && e.PromotionProgram.PromotionProgramCodes.Any(c => c.InsuranceCompanyId == insuranceCompanyId && c.InsuranceTypeCode == insuranceTypeCode));
            if (progUser == null)
                return string.Empty;

            return progUser.PromotionProgram.PromotionProgramCodes.First(e => e.InsuranceCompanyId == insuranceCompanyId && e.InsuranceTypeCode == insuranceTypeCode).Code;
        }

        private void GetVehicleColor(out string vehicleColor, out string vehicleColorCode, string vehicleMajorColor)
        {
            vehicleColor = vehicleMajorColor;
            vehicleColorCode = "99";
            var vehiclesColors = _vehicleService.GetVehicleColors();

            if (vehicleMajorColor.Contains("رصاصي") || vehicleMajorColor == "رصاصي" || vehicleMajorColor == "رصاصي غامق" || vehicleMajorColor == "رصاصي فاتح")
            {
                vehicleColor = "رمادي";
                var vecColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == "رمادي");
                vehicleColorCode = vecColor == null ? "99" : vecColor.Code.ToString();
                return;
            }
            string diffColor;

            if (vehicleMajorColor[0] == 'ا')
                diffColor = 'أ' + vehicleMajorColor.Substring(1);
            else if (vehicleMajorColor[0] == 'أ')
                diffColor = 'ا' + vehicleMajorColor.Substring(1);
            else
                diffColor = vehicleMajorColor;

            var vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor || color.ArabicDescription == diffColor);
            if (vColor == null)
            {
                if (vehicleMajorColor.Contains(' '))
                {
                    vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor.Split(' ')[0] || color.ArabicDescription == diffColor.Split(' ')[0]);
                    if (vColor != null)
                    {
                        vehicleColor = vehicleMajorColor.Split(' ')[0];
                        vehicleColorCode = vColor.Code.ToString();
                    }
                }
                else
                    vehicleColorCode = "99";
            }
            else
            {
                vehicleColorCode = vColor.Code.ToString();
            }
        }
    }
}
