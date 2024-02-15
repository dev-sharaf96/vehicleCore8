using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net.Http;
using System.Text;
using Tameenk.Core.Domain.Entities;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Common.Utilities;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Core.Data;
using Tameenk.Resources.WebResources;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Core.Caching;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core;
using VehicleInsurance = Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Extensions;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Configuration;
using System.Threading.Tasks;
using QuotationIntegrationDTO = Tameenk.Integration.Dto.Quotation;
using Tameenk.Redis;
using Tameenk.Core.Domain;
using NLog;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService
    {
        private QuotationServiceRequest GetQuotationRequestMessage(QuotationNewRequestDetails quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId)
        {
            var serviceRequestMessage = new QuotationServiceRequest();
            promotionProgramCode = string.Empty;
            promotionProgramId = 0;
            var cities = GetAllCities();
            long vehicleColorCode = 99;
            string vehicleColor;

            #region VehicleColor

            GetVehicleColor(out vehicleColor, out vehicleColorCode, quotationRequest.MajorColor, quotationResponse.InsuranceCompanyId);

            #endregion

            serviceRequestMessage.ReferenceId = quotationResponse.ReferenceId;
            serviceRequestMessage.ProductTypeCode = insuranceTypeCode;
            if (quotationRequest.RequestPolicyEffectiveDate.HasValue && quotationRequest.RequestPolicyEffectiveDate.Value.Date <= DateTime.Now.Date)
            {
                DateTime effectiveDate = DateTime.Now.AddDays(1);
                serviceRequestMessage.PolicyEffectiveDate = new DateTime(effectiveDate.Year, effectiveDate.Month, effectiveDate.Day, effectiveDate.Hour, effectiveDate.Minute, effectiveDate.Second);
            }
            else
            {
                serviceRequestMessage.PolicyEffectiveDate = quotationRequest.RequestPolicyEffectiveDate.Value;
            }

            #region Insured
            serviceRequestMessage.InsuredIdTypeCode = quotationRequest.CardIdTypeId;
            serviceRequestMessage.InsuredId = long.Parse(quotationRequest.NationalId);
            serviceRequestMessage.InsuredCity = !string.IsNullOrEmpty(quotationRequest.InsuredCityArabicDescription) ? quotationRequest.InsuredCityArabicDescription : "";
            serviceRequestMessage.InsuredCityCode = !string.IsNullOrEmpty(quotationRequest.InsuredCityYakeenCode.ToString()) ? quotationRequest.InsuredCityYakeenCode.ToString() : "";

            if (quotationRequest.NationalId.StartsWith("7")) //Company
            {
                serviceRequestMessage.InsuredIdTypeCode = 3;
                serviceRequestMessage.InsuredBirthDate = null;
                serviceRequestMessage.InsuredBirthDateG = null;
                serviceRequestMessage.InsuredBirthDateH = null;
                serviceRequestMessage.InsuredGenderCode = null;
                serviceRequestMessage.InsuredNationalityCode = null;
                serviceRequestMessage.InsuredFirstNameEn = null;
                serviceRequestMessage.InsuredMiddleNameEn = null;
                serviceRequestMessage.InsuredLastNameEn = null;
                serviceRequestMessage.InsuredFirstNameAr = quotationRequest.InsuredFirstNameAr; //Company Name
                serviceRequestMessage.InsuredMiddleNameAr = null;
                serviceRequestMessage.InsuredLastNameAr = null;
                serviceRequestMessage.InsuredSocialStatusCode = null;
                serviceRequestMessage.InsuredEducationCode = null;
                serviceRequestMessage.InsuredOccupation = null;
                serviceRequestMessage.InsuredOccupationCode = null;
                serviceRequestMessage.InsuredChildrenBelow16Years = null;
                serviceRequestMessage.InsuredWorkCityCode = null;
                serviceRequestMessage.InsuredWorkCity = null;
                serviceRequestMessage.InsuredIdIssuePlaceCode = null;
                serviceRequestMessage.InsuredIdIssuePlace = null;
            }
            else
            {
                serviceRequestMessage.InsuredBirthDate = quotationRequest.CardIdType == CardIdType.Citizen
                ? quotationRequest.InsuredBirthDateH
                : quotationRequest.InsuredBirthDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                // Add two lines for medGulf Company Only 
                serviceRequestMessage.InsuredBirthDateG = quotationRequest.InsuredBirthDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                serviceRequestMessage.InsuredBirthDateH = quotationRequest.InsuredBirthDateH;

                if (quotationRequest.InsuredGender == Gender.Male)
                    serviceRequestMessage.InsuredGenderCode = "M";
                else if (quotationRequest.InsuredGender == Gender.Female)
                    serviceRequestMessage.InsuredGenderCode = "F";
                else
                    serviceRequestMessage.InsuredGenderCode = "M";

                serviceRequestMessage.InsuredNationalityCode = quotationRequest.NationalityCode;
                serviceRequestMessage.InsuredFirstNameAr = quotationRequest.InsuredFirstNameAr;
                serviceRequestMessage.InsuredMiddleNameAr = $"{quotationRequest.InsuredMiddleNameAr}";
                serviceRequestMessage.InsuredLastNameAr = quotationRequest.InsuredLastNameAr;
                serviceRequestMessage.InsuredFirstNameEn = (string.IsNullOrEmpty(quotationRequest.InsuredFirstNameEn)
                    || string.IsNullOrWhiteSpace(quotationRequest.InsuredFirstNameEn)) ? "-" : quotationRequest.InsuredFirstNameEn;
                serviceRequestMessage.InsuredMiddleNameEn = $"{quotationRequest.InsuredMiddleNameEn}";
                serviceRequestMessage.InsuredLastNameEn = (string.IsNullOrEmpty(quotationRequest.InsuredLastNameEn)
                    || string.IsNullOrWhiteSpace(quotationRequest.InsuredLastNameEn)) ? "-" : quotationRequest.InsuredLastNameEn;


                serviceRequestMessage.InsuredSocialStatusCode = quotationRequest.SocialStatus?.GetCode();
                if (string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) && serviceRequestMessage.InsuredIdTypeCode == 1)
                {
                    serviceRequestMessage.InsuredOccupationCode = "O";
                    serviceRequestMessage.InsuredOccupation = "غير ذالك";
                }
                else if (string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) && serviceRequestMessage.InsuredIdTypeCode == 2)
                {
                    serviceRequestMessage.InsuredOccupationCode = "31010";
                    serviceRequestMessage.InsuredOccupation = "موظف اداري";
                }
                else
                {
                    if ((string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) || quotationRequest.InsuredOccupationCode == "o") && serviceRequestMessage.InsuredIdTypeCode == 1)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "O";
                        serviceRequestMessage.InsuredOccupation = "غير ذالك";
                    }
                    else if ((string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) || quotationRequest.InsuredOccupationCode == "o") && serviceRequestMessage.InsuredIdTypeCode == 2)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "31010";
                        serviceRequestMessage.InsuredOccupation = "موظف اداري";
                    }
                    else
                    {
                        serviceRequestMessage.InsuredOccupationCode = quotationRequest.InsuredOccupationCode;
                        serviceRequestMessage.InsuredOccupation = quotationRequest.InsuredOccupationNameAr.Trim();
                    }
                }

                serviceRequestMessage.InsuredEducationCode = int.Parse(quotationRequest.InsuredEducation.GetCode());
                if (!serviceRequestMessage.InsuredEducationCode.HasValue || serviceRequestMessage.InsuredEducationCode == 0)
                {
                    serviceRequestMessage.InsuredEducationCode = 1;
                }
                //end of mubark request
                serviceRequestMessage.InsuredChildrenBelow16Years = quotationRequest.ChildrenBelow16Years;
                serviceRequestMessage.InsuredIdIssuePlace = !string.IsNullOrEmpty(quotationRequest.IdIssueCityArabicDescription) ? quotationRequest.IdIssueCityArabicDescription : "";
                serviceRequestMessage.InsuredIdIssuePlaceCode = !string.IsNullOrEmpty(quotationRequest.IdIssueCityYakeenCode.ToString()) ? quotationRequest.IdIssueCityYakeenCode.ToString() : "";
                if (string.IsNullOrEmpty(serviceRequestMessage.InsuredIdIssuePlaceCode) && !string.IsNullOrEmpty(serviceRequestMessage.InsuredCityCode))
                {
                    serviceRequestMessage.InsuredIdIssuePlaceCode = serviceRequestMessage.InsuredCityCode;
                }
                if (string.IsNullOrEmpty(serviceRequestMessage.InsuredIdIssuePlace) && !string.IsNullOrEmpty(serviceRequestMessage.InsuredCity))
                {
                    serviceRequestMessage.InsuredIdIssuePlace = serviceRequestMessage.InsuredCity;
                }
                if (quotationRequest.WorkCityId.HasValue)
                {
                    var city = cities.Where(c => c.Code == quotationRequest.WorkCityId.Value).FirstOrDefault();
                    if (city == null)
                    {
                        serviceRequestMessage.InsuredWorkCity = serviceRequestMessage.InsuredCity;
                        serviceRequestMessage.InsuredWorkCityCode = serviceRequestMessage.InsuredCityCode;
                    }
                    else
                    {
                        serviceRequestMessage.InsuredWorkCity = city.ArabicDescription;
                        serviceRequestMessage.InsuredWorkCityCode = city.YakeenCode.ToString();
                    }
                }
                else
                {
                    serviceRequestMessage.InsuredWorkCity = serviceRequestMessage.InsuredCity;
                    serviceRequestMessage.InsuredWorkCityCode = serviceRequestMessage.InsuredCityCode;
                }
            }
            #endregion

            #region  Vehicle

            if (!string.IsNullOrEmpty(quotationRequest.RegisterationPlace))
            {
                var info = GetCityByName(cities, Utilities.RemoveWhiteSpaces(quotationRequest.RegisterationPlace));
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
            if (string.IsNullOrEmpty(serviceRequestMessage.VehicleRegPlaceCode))//as per mubark almutlak
            {
                serviceRequestMessage.VehicleRegPlaceCode = serviceRequestMessage.InsuredCityCode;
            }
            var isVehicleRegistered = quotationRequest.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber;
            if (isVehicleRegistered)
            {
                serviceRequestMessage.VehiclePlateNumber = quotationRequest.CarPlateNumber.HasValue ? quotationRequest.CarPlateNumber.Value : 0;
                serviceRequestMessage.VehiclePlateText1 = quotationRequest.CarPlateText1;
                serviceRequestMessage.VehiclePlateText2 = quotationRequest.CarPlateText2;
                serviceRequestMessage.VehiclePlateText3 = quotationRequest.CarPlateText3;
            }
            else
            {
                serviceRequestMessage.VehiclePlateNumber = null;
                serviceRequestMessage.VehiclePlateText1 = null;
                serviceRequestMessage.VehiclePlateText2 = null;
                serviceRequestMessage.VehiclePlateText3 = null;
            }

            //#endif
            serviceRequestMessage.VehicleIdTypeCode = quotationRequest.VehicleIdTypeId;
            if (quotationRequest.NationalId.StartsWith("7") && !quotationRequest.OwnerTransfer) //Company
            {
                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.NationalId);
            }
            else
            {
                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.CarOwnerNIN);
            }
            serviceRequestMessage.VehicleOwnerName = quotationRequest.CarOwnerName;
            serviceRequestMessage.VehiclePlateTypeCode = isVehicleRegistered ? quotationRequest.PlateTypeCode.ToString() : null;
            serviceRequestMessage.VehicleRegExpiryDate = isVehicleRegistered ? Utilities.HandleHijriDate(quotationRequest.LicenseExpiryDate) : null;

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
                            if (d < 10 && d > 0)
                            {
                                day = "0" + day;
                            }
                            else if (d == 0)
                            {
                                day = "01";
                            }
                        }
                        if (int.TryParse(serviceRequestMessage.VehicleRegExpiryDate.Split('-')[1], out m))
                        {
                            if (m < 10 && m > 0)
                            {
                                month = "0" + month;
                            }
                            else if (m == 0)
                            {
                                month = "01";
                            }
                        }
                        serviceRequestMessage.VehicleRegExpiryDate = day + "-" + month + "-" + year;
                    }
                }
                catch
                {

                }
            }
            if (string.IsNullOrEmpty(serviceRequestMessage.VehicleRegExpiryDate))
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

            serviceRequestMessage.VehicleId = isVehicleRegistered ? long.Parse(quotationRequest.SequenceNumber) : long.Parse(quotationRequest.CustomCardNumber);
            serviceRequestMessage.VehicleModelYear = quotationRequest.ModelYear.Value;
            serviceRequestMessage.VehicleMaker = quotationRequest.VehicleMaker;
            serviceRequestMessage.VehicleMakerCode = quotationRequest.VehicleMakerCode.HasValue ? quotationRequest.VehicleMakerCode.Value.ToString() : "0";
            serviceRequestMessage.VehicleModel = quotationRequest.VehicleModel;
            serviceRequestMessage.VehicleModelCode = quotationRequest.VehicleModelCode.HasValue ? quotationRequest.VehicleModelCode.Value.ToString() : "0";

            if (quotationRequest.VehicleMakerCode.HasValue)
            {
                var makers = VehicleMakers();
                if (makers != null)
                {
                    var vehicleMaker = makers.Where(e => e.Code == quotationRequest.VehicleMakerCode).FirstOrDefault();
                    if (vehicleMaker != null)
                    {
                        serviceRequestMessage.VehicleMaker = vehicleMaker.ArabicDescription;
                    }
                }
            }
            if (quotationRequest.VehicleModelCode.HasValue && quotationRequest.VehicleMakerCode.HasValue)
            {
                var models = VehicleModels(quotationRequest.VehicleMakerCode.Value);
                if (models != null)
                {
                    var vehicleModel = models.Where(e => e.Code == quotationRequest.VehicleModelCode).FirstOrDefault();
                    if (vehicleModel != null)
                    {
                        serviceRequestMessage.VehicleModel = vehicleModel.ArabicDescription;
                    }
                }
            }
            serviceRequestMessage.VehicleMajorColor = vehicleColor;
            serviceRequestMessage.VehicleMajorColorCode = vehicleColorCode.ToString();
            serviceRequestMessage.VehicleBodyTypeCode = quotationRequest.VehicleBodyCode.ToString();

            serviceRequestMessage.VehicleRegPlace = quotationRequest.RegisterationPlace;
            if (string.IsNullOrEmpty(serviceRequestMessage.VehicleRegPlace))//as per mubark almutlak
            {
                serviceRequestMessage.VehicleRegPlace = serviceRequestMessage.InsuredCity;
            }
            serviceRequestMessage.VehicleCapacity = quotationRequest.VehicleLoad; //@TODO: Validate this
            serviceRequestMessage.VehicleCylinders = int.Parse(quotationRequest.Cylinders.Value.ToString());
            serviceRequestMessage.VehicleWeight = quotationRequest.VehicleWeight;
            serviceRequestMessage.VehicleLoad = quotationRequest.VehicleLoad;
            serviceRequestMessage.VehicleOwnerTransfer = quotationRequest.OwnerTransfer;
            serviceRequestMessage.DriverDisabled = quotationRequest.IsSpecialNeed ?? false;
            serviceRequestMessage.VehicleUsingWorkPurposes = quotationRequest.IsUsedCommercially.HasValue ? quotationRequest.IsUsedCommercially.Value : false;

            serviceRequestMessage.VehicleAgencyRepair = vehicleAgencyRepair;
            serviceRequestMessage.VehicleValue = quotationRequest.VehicleValue;
            serviceRequestMessage.DeductibleValue = insuranceTypeCode == 1 ? null : (int?)(deductibleValue.HasValue ? deductibleValue.Value : 1500);

            serviceRequestMessage.VehicleEngineSizeCode = int.Parse(quotationRequest.EngineSize?.GetCode());
            serviceRequestMessage.VehicleUseCode = int.Parse(quotationRequest.VehicleUse != null && quotationRequest.VehicleUse.GetCode().Equals("0") ? "1" : quotationRequest.VehicleUse.GetCode());
            serviceRequestMessage.VehicleMileage = (int?)quotationRequest.CurrentMileageKM;
            serviceRequestMessage.VehicleTransmissionTypeCode = int.Parse(quotationRequest.TransmissionType?.GetCode());

            if (quotationRequest.MileageExpectedAnnual != null)
            {
                int MileageExpectedAnnualId = 0;
                int.TryParse(quotationRequest.MileageExpectedAnnual?.GetCode(), out MileageExpectedAnnualId);
                serviceRequestMessage.VehicleMileageExpectedAnnualCode = MileageExpectedAnnualId;
            }
            serviceRequestMessage.VehicleAxleWeightCode = quotationRequest.AxleWeightId;
            serviceRequestMessage.VehicleAxleWeight = quotationRequest.AxleWeightId;
            if (serviceRequestMessage.VehicleUseCode == 2)
            {
                serviceRequestMessage.VehicleAxleWeight = 1;
                serviceRequestMessage.VehicleAxleWeightCode = 1;
            }
            serviceRequestMessage.VehicleOvernightParkingLocationCode = int.Parse(quotationRequest?.ParkingLocation.GetCode());
            serviceRequestMessage.VehicleModification = quotationRequest.HasModifications;
            serviceRequestMessage.VehicleModificationDetails = string.IsNullOrEmpty(quotationRequest.ModificationDetails) ? "" : quotationRequest.ModificationDetails;

            if (quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.MedGulf) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.GGI)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.AlRajhi) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Malath)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.GIG) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Buruj)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.TokioMarine) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Walaa)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.ArabianShield) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Amana)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.AICC)
                || ((insuranceTypeCode == 2 || insuranceTypeCode == 13) && quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Allianz))
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.TUIC) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.UCA))
            {
                serviceRequestMessage.HasTrailer = quotationRequest.HasTrailer;
                serviceRequestMessage.TrailerSumInsured = (quotationRequest.HasTrailer) ? quotationRequest.TrailerSumInsured : (int?)null;// quotationRequest.TrailerSumInsured;
                //serviceRequestMessage.HasTrailer = (quotationRequest.Vehicle.HasTrailer) ? quotationRequest.Vehicle.HasTrailer : (bool?)null;
                //serviceRequestMessage.TrailerSumInsured = (quotationRequest.Vehicle.HasTrailer) ? quotationRequest.Vehicle.TrailerSumInsured : (int?)null;

                serviceRequestMessage.OtherUses = (quotationRequest.OtherUses) ? quotationRequest.OtherUses : (bool?)null;
            }

            #endregion
            if (quotationRequest.NationalId.StartsWith("7"))
            {
                serviceRequestMessage.NCDFreeYears = 0;
                serviceRequestMessage.NCDReference = "0";
            }
            else
            {
                serviceRequestMessage.NCDFreeYears = quotationRequest.NajmNcdFreeYears.HasValue ? quotationRequest.NajmNcdFreeYears.Value : 0;
                serviceRequestMessage.NCDReference = quotationRequest.NajmNcdRefrence;
            }

            bool excludeDriversAbove18 = false;
            //if (insuranceTypeCode == 1 && InsuranceCompaniesThatExcludeDriversAbove18.Contains(quotationResponse.InsuranceCompanyId))
            //    excludeDriversAbove18 = true;

            serviceRequestMessage.Drivers = CreateDriversRequestMessage(quotationRequest, cities, quotationResponse.InsuranceCompanyId, excludeDriversAbove18);
            var programcode = GetUserPromotionCodeInfo(userId, quotationRequest.NationalId, quotationResponse.InsuranceCompanyId, insuranceTypeCode == 2 ? 2 : 1);
            // as per Fayssal skip these emails 
            //mubarak.a @bcare.com.sa d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7
            //munera.s @bcare.com.sa a5fa9c14-61ed-44a1-a453-8db824a76a1e
            //mona.a @bcare.com.sa eb208f95-6b21-421c-be24-85f35ed017b5

            if (programcode != null &&
                (userId == "d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7"
                || userId == "a5fa9c14-61ed-44a1-a453-8db824a76a1e"
                || userId == "eb208f95-6b21-421c-be24-85f35ed017b5"
                || userId == "10c9e728-7459-4ef4-88d7-6321a41ead9c"))
            {
                promotionProgramCode = programcode.Code;
                promotionProgramId = programcode.PromotionProgramId;
                serviceRequestMessage.PromoCode = programcode.Code;
            }
            else if (programcode != null && (string.IsNullOrEmpty(programcode.NationalId) ||
                 programcode.NationalId == serviceRequestMessage.InsuredId.ToString()))
            {
                promotionProgramCode = programcode.Code;
                promotionProgramId = programcode.PromotionProgramId;
                serviceRequestMessage.PromoCode = programcode.Code;
            }
            else
            {
                serviceRequestMessage.PromoCode = null;
            }
            serviceRequestMessage.VehicleChassisNumber = quotationRequest.ChassisNumber;
            if ((quotationResponse.InsuranceCompanyId == 17 || quotationResponse.InsuranceCompanyId == 20 || quotationResponse.InsuranceCompanyId == 3 || quotationResponse.InsuranceCompanyId == 7 || quotationResponse.InsuranceCompanyId == 4 || quotationResponse.InsuranceCompanyId == 24 || quotationResponse.InsuranceCompanyId == 19) && quotationRequest.AdditionalDrivers != null)
            {
                serviceRequestMessage.PostalCode = quotationRequest.PostCode;
            }
            if (quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.TokioMarine)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.MedGulf))
            {
                if (quotationRequest.ManualEntry.HasValue && quotationRequest.ManualEntry.Value)
                    serviceRequestMessage.ManualEntry = ManualEntry.True;
                else
                    serviceRequestMessage.ManualEntry = ManualEntry.False;
            }
            if (quotationResponse.InsuranceCompanyId == 14)//Wataniya
            {
                serviceRequestMessage.IdExpiryDate = quotationRequest.IdExpiryDate;
                serviceRequestMessage.CameraTypeId = 3;
                serviceRequestMessage.BrakeSystemId = 3;
                serviceRequestMessage.HasAntiTheftAlarm = quotationRequest.HasAntiTheftAlarm;
                serviceRequestMessage.ParkingSensorId = 3;
                serviceRequestMessage.IsRenewal = false;
                serviceRequestMessage.IsUser = (!string.IsNullOrEmpty(userId)) ? true : false;
                serviceRequestMessage.HasFireExtinguisher = (quotationRequest.HasFireExtinguisher.HasValue &&
                                                        quotationRequest.HasFireExtinguisher.Value) ? true : false;

                if (!string.IsNullOrEmpty(quotationRequest.NationalId))
                {
                    var address = GetAddressesByNin(quotationRequest.NationalId);
                    if (address != null)
                    {
                        serviceRequestMessage.Street = string.IsNullOrEmpty(address.Street) ? "" : address.Street;
                        serviceRequestMessage.District = string.IsNullOrEmpty(address.District) ? "" : address.District;
                        serviceRequestMessage.City = string.IsNullOrEmpty(address.City) ? "" : address.City;

                        if (!string.IsNullOrEmpty(address.BuildingNumber))
                        {
                            int buildingNumber = 0;
                            int.TryParse(address.BuildingNumber, out buildingNumber);
                            serviceRequestMessage.BuildingNumber = buildingNumber;
                        }
                        if (!string.IsNullOrEmpty(address.PostCode))
                        {
                            int postCode = 0;
                            int.TryParse(address.PostCode, out postCode);
                            serviceRequestMessage.ZipCode = postCode;
                        }
                        if (!string.IsNullOrEmpty(address.AdditionalNumber))
                        {
                            int additionalNumber = 0;
                            int.TryParse(address.AdditionalNumber, out additionalNumber);
                            serviceRequestMessage.AdditionalNumber = additionalNumber;
                        }
                        if (!string.IsNullOrEmpty(address.RegionId))
                        {
                            int addressRegionID = 0;
                            int.TryParse(address.RegionId, out addressRegionID);
                            serviceRequestMessage.InsuredAddressRegionID = addressRegionID;
                        }
                    }
                }

                serviceRequestMessage.IsRenewal = (quotationRequest.IsRenewal.HasValue) ? quotationRequest.IsRenewal.Value : false;

                if (quotationRequest.ManualEntry.HasValue && quotationRequest.ManualEntry.Value)
                {
                    serviceRequestMessage.ManualEntry = "true";
                    serviceRequestMessage.MissingFields = quotationRequest.MissingFields;
                }
                else
                    serviceRequestMessage.ManualEntry = "false";

                var makerId = quotationRequest.VehicleMakerCode;
                var modelId = quotationRequest.VehicleModelCode;
                var vehicleModel = GetVehicleModelByMakerCodeAndModelCode(makerId.Value, modelId.Value);
                if (vehicleModel != null)
                {
                    if (vehicleModel.WataniyaMakerCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleMakerCode = vehicleModel.WataniyaMakerCode.Value.ToString();
                    if (vehicleModel.WataniyaModelCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleModelCode = vehicleModel.WataniyaModelCode.Value.ToString();
                }

                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText1))
                    serviceRequestMessage.WataniyaFirstPlateLetterID = GetWataiyaPlateLetterId(quotationRequest.CarPlateText1);
                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText2))
                    serviceRequestMessage.WataniyaSecondPlateLetterID = GetWataiyaPlateLetterId(quotationRequest.CarPlateText2);
                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText3))
                    serviceRequestMessage.WataniyaThirdPlateLetterID = GetWataiyaPlateLetterId(quotationRequest.CarPlateText3);
            }

            if (quotationResponse.InsuranceCompanyId == 5 || quotationResponse.InsuranceCompanyId == 7 || quotationResponse.InsuranceCompanyId == 18)
            {
                serviceRequestMessage.NoOfAccident = quotationRequest.NoOfAccident;
                if (serviceRequestMessage.NoOfAccident.HasValue && serviceRequestMessage.NoOfAccident.Value == 0 && !string.IsNullOrEmpty(quotationRequest.NajmResponse))
                {
                    serviceRequestMessage.ReferenceNo = quotationRequest.NajmResponse;
                }
                else if (!string.IsNullOrEmpty(quotationRequest.NajmResponse))
                {
                    serviceRequestMessage.Accidents = JsonConvert.DeserializeObject<List<Accident>>(quotationRequest.NajmResponse);
                }
                serviceRequestMessage.IsUseNumberOfAccident = true;
            }
            if (quotationResponse.InsuranceCompanyId == 7 && string.IsNullOrEmpty(quotationRequest.NajmResponse) &&
                (serviceRequestMessage.NCDFreeYears == 0 || serviceRequestMessage.NCDFreeYears == 11
                || serviceRequestMessage.NCDFreeYears == 12 || serviceRequestMessage.NCDFreeYears == 13))
            {
                serviceRequestMessage.NoOfAccident = 0;
                serviceRequestMessage.ReferenceNo = "FIRSTHIT";
            }
            return serviceRequestMessage;
        }

        private List<DriverDto> CreateDriversRequestMessage(QuotationNewRequestDetails quotationRequest, List<City> cities, int insuranceCompanyId, bool excludeDriversAbove18)
        {
            List<DriverDto> drivers = new List<DriverDto>();
            int additionalDrivingPercentage = 0;
            var mainDriverDto = new DriverDto()
            {
                DriverTypeCode = 1,
                DriverId = long.Parse(quotationRequest.NationalId),
                DriverIdTypeCode = quotationRequest.CardIdTypeId,
                DriverBirthDate = quotationRequest.InsuredBirthDateH,
                DriverBirthDateG = quotationRequest.InsuredBirthDate,
                DriverFirstNameAr = quotationRequest.InsuredFirstNameAr,
                DriverFirstNameEn = (string.IsNullOrWhiteSpace(quotationRequest.InsuredFirstNameEn) ||
                string.IsNullOrEmpty(quotationRequest.InsuredFirstNameEn)) ? "-" : quotationRequest.InsuredFirstNameEn,
                DriverMiddleNameAr = quotationRequest.InsuredMiddleNameAr,
                DriverMiddleNameEn = quotationRequest.InsuredMiddleNameEn,
                DriverLastNameAr = quotationRequest.InsuredLastNameAr,
                DriverLastNameEn = (string.IsNullOrWhiteSpace(quotationRequest.InsuredLastNameEn) ||
                string.IsNullOrEmpty(quotationRequest.InsuredLastNameEn)) ? "-" : quotationRequest.InsuredLastNameEn,
                DriverNOALast5Years = quotationRequest.NOALast5Years,
                DriverNOCLast5Years = quotationRequest.NOCLast5Years,
                DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears,
                DriverNCDReference = quotationRequest.NajmNcdRefrence
            };

            // this is with Alamia
            if (insuranceCompanyId == 18)
                mainDriverDto.DriverZipCode = quotationRequest.PostCode;

            if (quotationRequest.InsuredGender == Gender.Male)
                mainDriverDto.DriverGenderCode = "M";
            else if (quotationRequest.InsuredGender == Gender.Female)
                mainDriverDto.DriverGenderCode = "F";
            else
                mainDriverDto.DriverGenderCode = "M";

            mainDriverDto.DriverNationalityCode = !string.IsNullOrEmpty(quotationRequest.NationalityCode) ? quotationRequest.NationalityCode : "113";
            mainDriverDto.DriverSocialStatusCode = quotationRequest.MainDriverSocialStatusId?.ToString();
            if (string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) && mainDriverDto.DriverIdTypeCode == 1)
            {
                mainDriverDto.DriverOccupationCode = "O";
                mainDriverDto.DriverOccupation = "غير ذالك";
            }
            else if (string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) && mainDriverDto.DriverIdTypeCode == 2)
            {
                mainDriverDto.DriverOccupationCode = "31010";
                mainDriverDto.DriverOccupation = "موظف اداري";
            }
            else
            {
                if ((string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) || quotationRequest.InsuredOccupationCode == "o") && mainDriverDto.DriverIdTypeCode == 1)
                {
                    mainDriverDto.DriverOccupationCode = "O";
                    mainDriverDto.DriverOccupation = "غير ذالك";
                }
                else if ((string.IsNullOrEmpty(quotationRequest.InsuredOccupationCode) || quotationRequest.InsuredOccupationCode == "o") && mainDriverDto.DriverIdTypeCode == 2)
                {
                    mainDriverDto.DriverOccupationCode = "31010";
                    mainDriverDto.DriverOccupation = "موظف اداري";
                }
                else
                {
                    mainDriverDto.DriverOccupationCode = quotationRequest.InsuredOccupationCode;
                    mainDriverDto.DriverOccupation = quotationRequest.InsuredOccupationNameAr.Trim();
                }
            }
            if ((!quotationRequest.DrivingPercentage.HasValue || quotationRequest.DrivingPercentage > 100 || quotationRequest.DrivingPercentage < 100) && quotationRequest.AdditionalDrivers.Count == 1)
            {
                mainDriverDto.DriverDrivingPercentage = 100;
            }
            else
            {
                mainDriverDto.DriverDrivingPercentage = quotationRequest.DrivingPercentage;
            }
            additionalDrivingPercentage = mainDriverDto.DriverDrivingPercentage.HasValue ? mainDriverDto.DriverDrivingPercentage.Value : 0; ;
            mainDriverDto.DriverEducationCode = quotationRequest.EducationId;
            if (!mainDriverDto.DriverEducationCode.HasValue || mainDriverDto.DriverEducationCode == 0)
            {
                mainDriverDto.DriverEducationCode = 1;
            }
            mainDriverDto.DriverMedicalConditionCode = quotationRequest.MedicalConditionId;
            mainDriverDto.DriverChildrenBelow16Years = quotationRequest.ChildrenBelow16Years;
            mainDriverDto.DriverHomeCityCode = !string.IsNullOrEmpty(quotationRequest.InsuredCityYakeenCode.ToString()) ? quotationRequest.InsuredCityYakeenCode.ToString() : "";
            mainDriverDto.DriverHomeCity = !string.IsNullOrEmpty(quotationRequest.InsuredCityArabicDescription) ? quotationRequest.InsuredCityArabicDescription : "";
            if (quotationRequest.WorkCityId.HasValue)
            {
                var city = cities.Where(c => c.Code == quotationRequest.WorkCityId.Value).FirstOrDefault();
                if (city == null)
                {
                    mainDriverDto.DriverWorkCity = mainDriverDto.DriverHomeCity;
                    mainDriverDto.DriverWorkCityCode = mainDriverDto.DriverHomeCityCode;
                }
                else
                {
                    mainDriverDto.DriverWorkCity = city.ArabicDescription;
                    mainDriverDto.DriverWorkCityCode = city.YakeenCode.ToString();
                }
            }
            else
            {
                mainDriverDto.DriverWorkCity = mainDriverDto.DriverHomeCity;
                mainDriverDto.DriverWorkCityCode = mainDriverDto.DriverHomeCityCode;
            }

            //var DriverLicenses = _driverRepository.Table
            //    .Include(x => x.DriverLicenses)
            //    .FirstOrDefault(x => x.DriverId == quotationRequest.MainDriverId && x.IsDeleted == false)?
            //    .DriverLicenses;

            var LicenseDtos = new List<LicenseDto>();

            if (quotationRequest.MainDriverLicenses != null && quotationRequest.MainDriverLicenses.Count() > 0)
            {
                int licenseNumberYears;
                foreach (var item in quotationRequest.MainDriverLicenses)
                {
                    int? _driverLicenseTypeCode = item.TypeDesc;
                    if (insuranceCompanyId == 14)
                        _driverLicenseTypeCode = GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

                    licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                    LicenseDtos.Add(new LicenseDto()
                    {
                        DriverLicenseExpiryDate = Utilities.HandleHijriDate(item.ExpiryDateH),
                        DriverLicenseTypeCode = _driverLicenseTypeCode.ToString(),
                        LicenseCountryCode = 113,
                        LicenseNumberYears = licenseNumberYears == 0 ? 1 : licenseNumberYears
                    });
                }
                mainDriverDto.DriverLicenses = LicenseDtos; //from tameenk
            }
            else
            {
                mainDriverDto.DriverLicenses = null; //from tameenk
            }
            // Get (Main & Additional Drivers Extra Licenses)
            var driversExtraLicenses = _insuredExtraLicenses.TableNoTracking
                .Where(d => d.InsuredId == quotationRequest.InsuredId);

            // Main Driver Extra Licenses
            if (driversExtraLicenses != null && driversExtraLicenses.Any())
            {
                var mainDriverExtraLicenses = driversExtraLicenses.Where(m => m.IsMainDriver == true);

                if (mainDriverExtraLicenses != null && mainDriverExtraLicenses.Any())
                {
                    LicenseDto licenseDto;
                    List<LicenseDto> license = new List<LicenseDto>();
                    foreach (var item in mainDriverExtraLicenses)
                    {
                        if (item.LicenseCountryCode < 1 || item.LicenseCountryCode == 113) //as jira 349
                            continue;

                        licenseDto = new LicenseDto();
                        licenseDto.LicenseNumberYears = item.LicenseNumberYears;
                        licenseDto.LicenseCountryCode = item.LicenseCountryCode;
                        license.Add(licenseDto);

                    }
                    if (mainDriverDto.DriverLicenses != null)
                        mainDriverDto.DriverLicenses.AddRange(license);
                    else
                        mainDriverDto.DriverLicenses = license;
                }
            }
            //var mainDriverViolations = driverViolationRepository.TableNoTracking
            //                  .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == quotationRequest.MainDriverNin).ToList();

            if (quotationRequest.MainDriverViolation != null && quotationRequest.MainDriverViolation.Count > 0)
            {
                mainDriverDto.DriverViolations = quotationRequest.MainDriverViolation.Select(e => new ViolationDto()
                { ViolationCode = e.ViolationId }).ToList();

            }
            //Add main driver to drivers list
            if (!quotationRequest.NationalId.StartsWith("7"))
            {
                if (insuranceCompanyId == 14)//Wataniya
                    HandleDriveAddressDetailsForWataniya(mainDriverDto);

                if (excludeDriversAbove18 && (quotationRequest.AdditionalDrivers != null && quotationRequest.AdditionalDrivers.Count >= 1))
                    quotationRequest.AdditionalDrivers = HandleDriversAbove18Years(quotationRequest.AdditionalDrivers, mainDriverDto);

                drivers.Add(mainDriverDto);
            }
            //check if there are additional drivers, if yes then add them to drivers list
            if (quotationRequest.AdditionalDrivers != null && quotationRequest.AdditionalDrivers.Any())
            {
                var additionalDrivers = quotationRequest.AdditionalDrivers.Where(e => e.NIN != mainDriverDto.DriverId.ToString());
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
                        DriverNCDFreeYears = 0,
                        DriverNCDReference = "0",
                        DriverRelationship = additionalDriver.RelationShipId ?? 0
                    };
                    if (insuranceCompanyId == 18) //Alamaya as per fayssal
                    {
                        driverDto.DriverRelationship = null;
                    }
                    if (quotationRequest.NationalId.StartsWith("7") && additionalDriver.NIN == additionalDrivers.ToList().FirstOrDefault().NIN)
                    {
                        driverDto.DriverTypeCode = 1;
                        driverDto.DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears;
                        driverDto.DriverNCDReference = quotationRequest.NajmNcdRefrence;
                    }
                    else
                    {
                        driverDto.DriverTypeCode = 2;
                    }
                    if (additionalDriver.Gender == Gender.Male)
                        driverDto.DriverGenderCode = "M";
                    else if (additionalDriver.Gender == Gender.Female)
                        driverDto.DriverGenderCode = "F";
                    else
                        driverDto.DriverGenderCode = "M";

                    driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                            additionalDriver.NationalityCode.Value.ToString() : "113";
                    var additionalDriverOccupation = additionalDriver.Occupation;
                    if (additionalDriverOccupation == null && driverDto.DriverIdTypeCode == 1)
                    {
                        driverDto.DriverOccupationCode = "O";
                        driverDto.DriverOccupation = "غير ذالك";
                    }
                    else if (additionalDriverOccupation == null && driverDto.DriverIdTypeCode == 2)
                    {
                        driverDto.DriverOccupationCode = "31010";
                        driverDto.DriverOccupation = "موظف اداري";
                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && driverDto.DriverIdTypeCode == 1)
                        {
                            driverDto.DriverOccupationCode = "O";
                            driverDto.DriverOccupation = "غير ذالك";
                        }

                        else if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && driverDto.DriverIdTypeCode == 2)
                        {
                            driverDto.DriverOccupationCode = "31010";
                            driverDto.DriverOccupation = "موظف اداري";
                        }
                        else
                        {
                            driverDto.DriverOccupationCode = additionalDriverOccupation.Code;
                            driverDto.DriverOccupation = additionalDriverOccupation.NameAr.Trim();
                        }
                    }
                    driverDto.DriverDrivingPercentage = additionalDriver.DrivingPercentage; // from tameenk
                    additionalDrivingPercentage += additionalDriver.DrivingPercentage.HasValue ? additionalDriver.DrivingPercentage.Value : 0;
                    driverDto.DriverEducationCode = additionalDriver.EducationId;
                    if (!driverDto.DriverEducationCode.HasValue || driverDto.DriverEducationCode == 0)
                    {
                        driverDto.DriverEducationCode = 1;
                    }
                    driverDto.DriverMedicalConditionCode = additionalDriver.MedicalConditionId;
                    driverDto.DriverChildrenBelow16Years = additionalDriver.ChildrenBelow16Years;
                    if (additionalDriver.CityId.HasValue)
                    {
                        var city = cities.Where(c => c.Code == additionalDriver.CityId.Value).FirstOrDefault();
                        if (city == null)
                        {
                            driverDto.DriverHomeCity = mainDriverDto.DriverHomeCity;
                            driverDto.DriverHomeCityCode = mainDriverDto.DriverHomeCityCode;
                        }
                        else
                        {
                            driverDto.DriverHomeCity = city.ArabicDescription;
                            driverDto.DriverHomeCityCode = city.YakeenCode.ToString();
                        }
                    }
                    else
                    {
                        driverDto.DriverHomeCity = mainDriverDto.DriverHomeCity;
                        driverDto.DriverHomeCityCode = mainDriverDto.DriverHomeCityCode;
                    }
                    if (additionalDriver.WorkCityId.HasValue)
                    {
                        var city = cities.Where(c => c.Code == additionalDriver.WorkCityId.Value).FirstOrDefault();
                        if (city == null)
                        {
                            driverDto.DriverWorkCity = driverDto.DriverHomeCity;
                            driverDto.DriverWorkCityCode = driverDto.DriverHomeCityCode;

                        }
                        else
                        {
                            driverDto.DriverWorkCity = city.ArabicDescription;
                            driverDto.DriverWorkCityCode = city.YakeenCode.ToString();
                        }
                    }
                    else
                    {
                        driverDto.DriverWorkCity = driverDto.DriverHomeCity;
                        driverDto.DriverWorkCityCode = driverDto.DriverHomeCityCode;
                    }

                    var additionalDriverLicenses = _driverRepository.Table
                            .Include(x => x.DriverLicenses)
                            .FirstOrDefault(x => x.DriverId == additionalDriver.DriverId && x.IsDeleted == false)?
                            .DriverLicenses;

                    var additionalDriverLicenseDtos = new List<LicenseDto>();
                    if (additionalDriverLicenses != null && additionalDriverLicenses.Count() > 0)
                    {
                        int licenseNumberYears;
                        foreach (var item in additionalDriverLicenses)
                        {
                            int? _driverLicenseTypeCode = item.TypeDesc;
                            if (insuranceCompanyId == 14)
                                _driverLicenseTypeCode = GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

                            licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                            additionalDriverLicenseDtos.Add(new LicenseDto()
                            {
                                DriverLicenseExpiryDate = Utilities.HandleHijriDate(item.ExpiryDateH),
                                DriverLicenseTypeCode = _driverLicenseTypeCode.ToString(),
                                LicenseCountryCode = 113,
                                LicenseNumberYears = licenseNumberYears == 0 ? 1 : licenseNumberYears
                            });
                        }
                        driverDto.DriverLicenses = additionalDriverLicenseDtos; //from tameenk
                    }
                    else
                    {
                        driverDto.DriverLicenses = null;
                    }
                    // Aditional Driver Extra Licenses
                    if (driversExtraLicenses != null && driversExtraLicenses.Any())
                    {
                        var additionalDriversExtraLicenses = driversExtraLicenses.Where(m => m.IsMainDriver == false && m.DriverNin == additionalDriver.NIN);

                        if (additionalDriversExtraLicenses != null && additionalDriversExtraLicenses.Any())
                        {
                            LicenseDto licenseDto;
                            List<LicenseDto> licenseAditional = new List<LicenseDto>();
                            foreach (var item in additionalDriversExtraLicenses)
                            {
                                if (item.LicenseCountryCode < 1 || item.LicenseCountryCode == 113)  //as jira 349
                                    continue;

                                licenseDto = new LicenseDto();
                                licenseDto.LicenseNumberYears = item.LicenseNumberYears;
                                licenseDto.LicenseCountryCode = item.LicenseCountryCode;
                                licenseAditional.Add(licenseDto);

                            }
                            if (driverDto.DriverLicenses != null)
                                driverDto.DriverLicenses.AddRange(licenseAditional);
                            else
                                driverDto.DriverLicenses = licenseAditional;
                        }
                    }
                    var driverViolations = driverViolationRepository.TableNoTracking
                                     .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == additionalDriver.NIN);
                    if (driverViolations != null && driverViolations.Count() > 0)
                    {
                        driverDto.DriverViolations = driverViolations
                            .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();
                    }

                    // this for additional drivers
                    if (insuranceCompanyId == 14)//Wataniya
                        HandleDriveAddressDetailsForWataniya(driverDto);

                    drivers.Add(driverDto);
                }
            }
            if (additionalDrivingPercentage != 100 && drivers.Count() > 1)
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
                    percentage = mainPercentage = 50;
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

        private LicenseType GetWataniyaDriverLicenseType(string licenseType)
        {
            LicenseType license = null;

            short typeCode = 0;
            short.TryParse(licenseType, out typeCode);
            if (typeCode > 0)
                license = GetAllLicenseType().Where(a => a.Code == typeCode).FirstOrDefault();

            return license;
        }

        private void HandleDriveAddressDetailsForWataniya(DriverDto model)
        {
            if (model.DriverId > 0)
            {
                var address = GetAddressesByNin(model.DriverId.ToString());
                if (address != null)
                    model.DriverHomeAddress = address.BuildingNumber + " " + address.AdditionalNumber + " " + address.PostCode + " " + address.City;
            }
        }

        private Address GetAddressesByNin(string driverNin)
        {
            Address address = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAddress";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                command.Parameters.Add(driverNinParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return address;
            }
            catch (Exception ex)
            {
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public PromotionProgramUserModel GetUserPromotionCodeInfo(string userId, string nationalId, int insuranceCompanyId, int insuranceTypeCode)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                if (insuranceCompanyId < 1)
                    throw new TameenkArgumentNullException(nameof(insuranceCompanyId), "Insurance company id can't be less than 1.");
                PromotionProgramUserModel promotionProgramUserInfo = null;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserPromotionProgramInfo";
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(userId) && userId != Guid.Empty.ToString())
                {
                    SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                    command.Parameters.Add(userIdParam);
                }
                SqlParameter nationalIdParam = new SqlParameter() { ParameterName = "nationalId", Value = nationalId };
                command.Parameters.Add(nationalIdParam);

                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };

                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(insuranceTypeCodeParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                promotionProgramUserInfo = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramUserModel>(reader).FirstOrDefault();
                if (promotionProgramUserInfo == null)
                {
                    reader.NextResult();
                    promotionProgramUserInfo = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramUserModel>(reader).FirstOrDefault();
                }

                return promotionProgramUserInfo;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public int GetWataiyaPlateLetterId(string letter)
        {
            int letterId = 0;
            var letterData = _vehiclePlateTextRepository.TableNoTracking.Where(a => a.ArabicDescription == letter).FirstOrDefault();
            if (letterData != null && letterData.WataniyaCode.HasValue)
                letterId = letterData.WataniyaCode.Value;

            return letterId;
        }

        private List<Driver> HandleDriversAbove18Years(List<Driver> drivers, DriverDto mainDriverDto)
        {
            var additionalDrivers = drivers.Where(a => a.NIN != mainDriverDto.DriverId.ToString()).ToList();
            if (additionalDrivers == null || additionalDrivers.Count == 0)
                return drivers;
            if (additionalDrivers.Count > 1)
                return drivers;

            var currentYear = DateTime.Today.Year;
            var additionalDriver = additionalDrivers.FirstOrDefault();
            var driverAge = currentYear - additionalDriver.DateOfBirthG.Year;

            if (driverAge < 18)
                return drivers;

            mainDriverDto.DriverDrivingPercentage += additionalDriver.DrivingPercentage;
            drivers.Remove(additionalDriver);
            return drivers;
        }

    }
}
