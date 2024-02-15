using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;

using Tameenk.Resources.Inquiry;
using Tameenk.Services.Core.Vehicles;
using System.Data.Entity;
using System.Data.Linq;
using Tameenk.Services.Core.Http;
using Tameenk.Loggin.DAL;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Services.Core.Occupations;
using Tameenk.Api.Core.Context;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Services.Extensions;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.YakeenIntegration.Business.YakeenBCareService;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Security.Services;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Core.Providers;
using Tameenk.Core.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data;
using Tameenk.Data;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Implementation;
using Tameenk.Services.Core.Wathq;
using Tameenk.Loggin.DAL.Entities.ServiceRequestLogs;
using Tameenk.Services.Implementation.Wathq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tameenk.Services.Inquiry.Components
{
    public class InquiryContext : IInquiryContext
    {
        private readonly IQuotationService _quotationService;
        private readonly TameenkConfig _config;
        private readonly IRepository<Insured> insuredRepository;
        private readonly IVehicleService vehicleService;
        private readonly IRepository<Driver> driverRepository;
        private readonly IHttpClient httpClient;
        private readonly ICustomerServices customerServices;
        private readonly IAddressService addressService;
        private readonly IRepository<PromotionProgramUser> promotionProgramUserRepository;
        private readonly IRepository<QuotationRequest> quotationRequestRepository;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IYakeenVehicleServices yakeenVehicleServices;
        private readonly IDriverServices driverServices;
        private readonly IOccupationService occupationService;
        private readonly IWebApiContext webApiContext;
        private readonly IRepository<NCDFreeYear> nCDFreeYearRepository;
        private readonly INajmService najmService;
        private readonly IRepository<NajmResponseEntity> najmResponseRepository;
        private readonly IRepository<DriverViolation> driverViolationRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IRepository<YakeenVehicles> _yakeenVehiclesRepository;
        private readonly IRepository<NajmAccidentResponse> _najmAccidentResponseRepository;
        private readonly IRepository<Occupation> _occupationRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IInsuredService _insuredService;
        private readonly ICheckoutsService _checkoutsService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRepository<CustomCardInfo> _customCardInfoRepository;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IRepository<PolicyModification> _policyModificationrepository;
        private readonly IRepository<QuotationBlockedNins> _quotationBlockedNins;
        private readonly IWathqService _wathqService;
        //private readonly IRepository<WathqInfo> _wathqRepo;

        private readonly HashSet<string> requiredFieldsInCustomOnly = new HashSet<string>
            {
                "VehicleLoad",
                "VehicleModel",
                "VehicleModelCode",
                "VehicleMajorColor",
                "VehicleBodyCode",
                "VehicleMaker",
                "VehicleMakerCode",
                "VehicleChassisNumber",
                "MainDriverSocialStatusCode",
                "AdditionalDriverOneSocialStatusCode",
                "AdditionalDriverTwoSocialStatusCode"
            };

        private readonly HashSet<string> ExcludedVehicleIds = new HashSet<string>
        {
            "702757510",
            "52553900",
            "471702910",
            //"86209800",
            "447534410",
            "354976900",
            "182797510",
            "9211610"
        };

        public string AuthorizationToken { get; set; }
        public InquiryContext(IRepository<Insured> insuredRepository, IVehicleService vehicleService, IQuotationService quotationService,
            IRepository<Driver> driverRepository, IHttpClient httpClient, ICustomerServices customerServices,
            IAddressService addressService,
            IRepository<PromotionProgramUser> promotionProgramUserRepository,
            IRepository<QuotationRequest> quotationRequestRepository,
            IYakeenVehicleServices yakeenVehicleServices,
            IDriverServices driverServices,
            IOccupationService occupationService,
            IWebApiContext webApiContext,
            IRepository<NCDFreeYear> NCDFreeYearRepository,
            INajmService najmService,
            IRepository<NajmResponseEntity> NajmResponse,
            IRepository<DriverViolation> driverViolationRepository,
            IYakeenClient yakeenClient, TameenkConfig tameenkConfig, IRepository<Policy> policyRepository,
            IRepository<YakeenDrivers> yakeenDriversRepository, IRepository<YakeenVehicles> yakeenVehiclesRepository
            , IRepository<NajmAccidentResponse> najmAccidentResponseRepository,
            IRepository<Occupation> occupationRepository,
            IRepository<LicenseType> licenseTypeRepository, IInsuredService insuredService,
            ICheckoutsService checkoutsService, IAuthorizationService authorizationService,
            IRepository<CustomCardInfo> customCardInfoRepository,
            IInsuranceCompanyService insuranceCompanyService,
            IRepository<PolicyModification> policyModificationrepository,
              IRepository<QuotationBlockedNins> quotationBlockedNins,
              IWathqService wathqService
            //  IRepository<WathqInfo> wathqRepo
            )
        {
            this.insuredRepository = insuredRepository;
            this.vehicleService = vehicleService;
            this.driverRepository = driverRepository;
            this.httpClient = httpClient;
            this.customerServices = customerServices;
            this.addressService = addressService;
            this.promotionProgramUserRepository = promotionProgramUserRepository;
            this.quotationRequestRepository = quotationRequestRepository;
            this.yakeenVehicleServices = yakeenVehicleServices;
            this.driverServices = driverServices;
            this.occupationService = occupationService;
            this.webApiContext = webApiContext;
            nCDFreeYearRepository = NCDFreeYearRepository;
            this.najmService = najmService;
            najmResponseRepository = NajmResponse;
            this.driverViolationRepository = driverViolationRepository;
            _yakeenClient = yakeenClient;
            _config = tameenkConfig;
            _policyRepository = policyRepository;
            _yakeenDriversRepository = yakeenDriversRepository;
            _yakeenVehiclesRepository = yakeenVehiclesRepository;
            _najmAccidentResponseRepository = najmAccidentResponseRepository;
            _occupationRepository = occupationRepository;
            _licenseTypeRepository = licenseTypeRepository;
            _insuredService = insuredService;
            _checkoutsService = checkoutsService;
            _authorizationService = authorizationService;
            _customCardInfoRepository = customCardInfoRepository;
            _insuranceCompanyService = insuranceCompanyService;
            _policyModificationrepository = policyModificationrepository;
            _quotationService = quotationService;
            _quotationBlockedNins = quotationBlockedNins;
           _wathqService = wathqService;
           // _wathqRepo = wathqRepo;
        }

        public InquiryOutput InitInquiryRequest(InitInquiryRequestModel model,InquiryRequestLog log)
        {
            InquiryOutput output = new InquiryOutput();
            try
            {

                if ((model.Channel == Channel.android || model.Channel == Channel.ios
                    ||model.Channel.ToString().ToLower() == "ios"||model.Channel.ToString().ToLower()== "android")
                    && string.IsNullOrEmpty(model.MobileVersion))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.OldMobileVersion;
                    output.ErrorDescription = SubmitInquiryResource.OldMobileVersion;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Old Mobile Version";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.NationalIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.NationalIdRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "NationalId is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                List<string> NationalIds = new List<string>();                NationalIds.Add(model.NationalId);                var blockUsers = CheckBlockedNin(NationalIds);                if (blockUsers != null && blockUsers.Count > 0)                {                    output.ErrorCode = InquiryOutput.ErrorCodes.BlockedNationalId;                    output.ErrorDescription = SubmitInquiryResource.BlockedNin.Replace("{0}", model.NationalId);                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "this nin"+ model.NationalId + " is Blocked To Get Quotation";                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);                    return output;                }
                if (string.IsNullOrEmpty(model.SequenceNumber))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SequenceNumberIsNull;
                    output.ErrorDescription = SubmitInquiryResource.SequenceNumberRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sequence Number is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (model.VehicleIdTypeId == 0)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleIdTypeIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleIdTypeIdRequired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Vehicle Id type is Zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (model.OwnerTransfer && model.OwnerNationalId == model.NationalId)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.OwnerNationalIdAndNationalIdAreEqual;
                    output.ErrorDescription = SubmitInquiryResource.InvaildOldOwnerNin;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Vehicle.OwnerNationalId and Insured.NationalId are the same";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (model.OwnerTransfer && string.IsNullOrEmpty(model.OwnerNationalId)&& model.Channel == Channel.Portal)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleOwnerNinIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleOwnerNinIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Owner National Id is empty";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (model.PolicyEffectiveDate < DateTime.Now.Date.AddDays(1) || model.PolicyEffectiveDate > DateTime.Now.AddDays(29))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.InvalidPolicyEffectiveDate;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy effective date should be within 29 days starts from toworrow as user enter "+ model.PolicyEffectiveDate.ToString();
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                //DateTime endDate = new DateTime(2020, 6, 30);
                //if (DateTime.Now.Date <= endDate.Date)
                //{
                //    if (model.PolicyEffectiveDate.Date > endDate.Date)
                //    {
                //        output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                //        output.ErrorDescription = SubmitInquiryResource.InvalidPolicyEffectiveDateToEndOfJune;
                //        log.ErrorCode = (int)output.ErrorCode;
                //        log.ErrorDescription = "Policy effective date should starts from toworrow and should not exceed june 30th,2020 " + model.PolicyEffectiveDate.ToString();
                //        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                //        return output;
                //    }
                //}
                // var result = InitInquiryRequest(model);
                InitInquiryResponseModel responseModel = new InitInquiryResponseModel();
                responseModel.PolicyEffectiveDate = model.PolicyEffectiveDate;
                responseModel.IsCustomerCurrentOwner = !model.OwnerTransfer;
                if (model.OwnerTransfer)
                {
                    long oldOwnerNin = 0;
                    long.TryParse(model.OwnerNationalId, out oldOwnerNin);
                    responseModel.OldOwnerNin = oldOwnerNin;
                }
                if (model.OwnerTransfer && (!responseModel.OldOwnerNin.HasValue || responseModel.OldOwnerNin == 0) && model.Channel == Channel.Portal)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.VehicleOwnerNinIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleOwnerNinIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and OldOwnerNin is zero";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var vehicle = GetVehicleInfo(model);
                if (vehicle != null)
                {
                    //add vehicle info into the model
                    responseModel.Vehicle = vehicle.ToModel();
                    responseModel.Vehicle.OwnerTransfer = model.OwnerTransfer;
                    responseModel.Vehicle.ApproximateValue = vehicle.VehicleValue ?? 30000;
                    responseModel.Vehicle.Modification = string.Empty;
                    responseModel.Vehicle.HasModification = false;
                    responseModel.Vehicle.HasTrailer = false;
                    responseModel.Vehicle.ApproximateTrailerSumInsured = 0;
                    responseModel.Vehicle.OtherUses = false;
                    //mark the vehcile as exist
                    responseModel.IsVehicleExist = true;
                    responseModel.IsVehicleUsedCommercially = vehicle.IsUsedCommercially ?? false;
                }
                else
                {
                    responseModel.IsVehicleExist = false;
                }
                var driver = GetDriverInfo(model);
                if (driver != null)
                {
                    //driver exist case
                    responseModel.IsMainDriverExist = true;
                    DriverModel mainDriver = new DriverModel
                    {
                        NationalId = driver.NIN,
                        ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0,
                        EducationId = driver.EducationId,
                        DrivingPercentage = 100,
                        MedicalConditionId = driver.MedicalConditionId ?? 1,
                        DriverNOALast5Years = driver.NOALast5Years ?? 0,
                        RelationShipId = driver.RelationShipId ?? 0
                    };

                    responseModel.IsCustomerSpecialNeed = driver.IsSpecialNeed;
                    // var insured = insuredRepository.TableNoTracking.Where(e => e.NationalId == model.NationalId).OrderByDescending(x => x.Id).FirstOrDefault();
                    var insured = _insuredService.GetIInsuredByNationalId(model.NationalId);
                    if (insured != null)
                    {
                        responseModel.CityCode = insured.CityId ?? 1;
                    }
                    else
                    {
                        responseModel.CityCode = 1;
                    }
                    responseModel.Insured = new InsuredModel
                    {
                        NationalId = driver.NIN,
                        ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0,
                        EducationId = driver.EducationId
                    };
                    if (driver.IsCitizen)
                    {
                        if (!string.IsNullOrWhiteSpace(driver.DateOfBirthH))
                        {
                            var dateH = driver.DateOfBirthH.Split('-');
                            responseModel.Insured.BirthDateMonth = Convert.ToByte(dateH[1]);
                            responseModel.Insured.BirthDateYear = short.Parse(dateH[2]);
                            mainDriver.BirthDateMonth = Convert.ToByte(dateH[1]);
                            mainDriver.BirthDateYear = short.Parse(dateH[2]);
                        }
                    }
                    else
                    {
                        var dateG = driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        responseModel.Insured.BirthDateMonth = Convert.ToByte(dateG[1]);
                        responseModel.Insured.BirthDateYear = short.Parse(dateG[2]);
                        mainDriver.BirthDateMonth = Convert.ToByte(dateG[1]);
                        mainDriver.BirthDateYear = short.Parse(dateG[2]);
                    }
                    responseModel.Drivers = new List<DriverModel> { mainDriver };
                }
                else
                {
                    responseModel.IsMainDriverExist = false;
                }

                output.InitInquiryResponseModel = responseModel;
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                //log.ErrorCode = (int)output.ErrorCode;
                //log.ErrorDescription = output.ErrorDescription;
                //InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }


        public InquiryOutput InitInquiryRequest(InitInquiryRequestModel requestModel)
        {
            InitInquiryResponseModel result = new InitInquiryResponseModel();
            InquiryOutput output = new InquiryOutput();
            try
            {
                if (requestModel == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    return output;
                }
                result.PolicyEffectiveDate = requestModel.PolicyEffectiveDate;
                var vehicle = GetVehicleInfo(requestModel);
                result.IsCustomerCurrentOwner = !requestModel.OwnerTransfer;

                if (requestModel.OwnerTransfer)
                {
                    long.TryParse(requestModel.OwnerNationalId, out var oldOwnerNin);
                    result.OldOwnerNin = oldOwnerNin;
                }
                if (vehicle == null)
                    result.IsVehicleExist = false;
                else
                {
                    //add vehicle info into the model
                    result.Vehicle = vehicle.ToModel();
                    result.Vehicle.OwnerTransfer = requestModel.OwnerTransfer;
                    result.Vehicle.ApproximateValue = vehicle.VehicleValue ?? 30000;
                    //mark the vehcile as exist
                    result.IsVehicleExist = true;
                    result.IsVehicleUsedCommercially = vehicle.IsUsedCommercially ?? false;
                }

                var driver = GetDriverInfo(requestModel);
                if (driver == null)
                    result.IsMainDriverExist = false;
                else
                {
                    //driver exist case
                    result.IsMainDriverExist = true;
                    DriverModel mainDriver = new DriverModel
                    {
                        NationalId = driver.NIN,
                        ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0,
                        EducationId = driver.EducationId,
                        DrivingPercentage = 100,
                        MedicalConditionId = driver.MedicalConditionId ?? 1,
                        DriverNOALast5Years = driver.NOALast5Years ?? 0

                    };

                    //if (driver.DriverViolations != null && driver.DriverViolations.Any())
                    //    mainDriver.ViolationIds = driver.DriverViolations.Select(e => e.ViolationId).ToList();

                    result.IsCustomerSpecialNeed = driver.IsSpecialNeed;
                    //var insured = insuredRepository.TableNoTracking.OrderByDescending(x => x.Id).FirstOrDefault(e => e.NationalId == requestModel.NationalId);
                    var insured = _insuredService.GetIInsuredByNationalId(requestModel.NationalId);
                    if (insured != null)
                    {
                        result.CityCode = insured.CityId ?? 1;
                    }
                    else
                    {
                        result.CityCode = 1;
                    }
                    result.Insured = new InsuredModel
                    {
                        NationalId = driver.NIN,
                        ChildrenBelow16Years = driver.ChildrenBelow16Years ?? 0,
                        EducationId = driver.EducationId
                    };
                    if (driver.IsCitizen)
                    {
                        if (!string.IsNullOrWhiteSpace(driver.DateOfBirthH))
                        {
                            var dateH = driver.DateOfBirthH.Split('-');
                            result.Insured.BirthDateMonth = Convert.ToByte(dateH[1]);
                            result.Insured.BirthDateYear = short.Parse(dateH[2]);
                            mainDriver.BirthDateMonth = Convert.ToByte(dateH[1]);
                            mainDriver.BirthDateYear = short.Parse(dateH[2]);
                        }
                    }
                    else
                    {
                        var dateG = driver.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        result.Insured.BirthDateMonth = Convert.ToByte(dateG[1]);
                        result.Insured.BirthDateYear = short.Parse(dateG[2]);
                        mainDriver.BirthDateMonth = Convert.ToByte(dateG[1]);
                        mainDriver.BirthDateYear = short.Parse(dateG[2]);
                    }
                    result.Drivers = new List<DriverModel> { mainDriver };
                }
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.InitInquiryResponseModel = result;
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public ValidationOutput ValidateData(InquiryRequestModel requestModel, InquiryRequestLog log, string channel)
        {
            ValidationOutput output = new ValidationOutput();
            output.Log = log;
            try
            {
                if (requestModel == null)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    output.Log = log;
                    return output;
                }
                //if (string.IsNullOrEmpty(requestModel.MobileVersion) && (log.Channel.ToLower() == "ios" || log.Channel.ToLower() == "android"))
                //{
                //    output.ErrorCode = ValidationOutput.ErrorCodes.OldMobileVersion;
                //    output.ErrorDescription = SubmitInquiryResource.OldMobileVersion;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "Old Mobile Version";
                //    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                //    return output;
                //}
                if (requestModel.Vehicle.TransmissionTypeId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.TransmissionTypeIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.TransmissionTypeIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TransmissionTypeId is zero"; ;
                    output.Log = log;
                    return output;

                }
                if (!requestModel.Vehicle.MileageExpectedAnnualId.HasValue || requestModel.Vehicle.MileageExpectedAnnualId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.MileageExpectedAnnualIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.MileageExpectedAnnualIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "MileageExpectedAnnualId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.ParkingLocationId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.ParkingLocationIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ParkingLocationId is zero"; ;
                    output.Log = log;
                    return output;
                }
                List<string> NationalIds = new List<string>();
                foreach (var driver in requestModel.Drivers)
                {
                    NationalIds.Add(driver.NationalId);
                }                var blockedNins = CheckBlockedNin(NationalIds);                if (blockedNins != null && blockedNins.Count > 0)                {                    output.ErrorCode = ValidationOutput.ErrorCodes.BlockedNationalId;                    output.ErrorDescription = SubmitInquiryResource.BlockedNin.Replace("{0}", blockedNins[0]);                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "this nin" + blockedNins[0] + " is Blocked To Get Quotation";                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);                    return output;                }
                int vehicleValue = 0;
                string comingValue = requestModel.Vehicle.ApproximateValue.ToString();
                if (requestModel.Vehicle.ApproximateValue.ToString().Contains(".") && channel.ToLower() != "portal")
                {
                    string value = requestModel.Vehicle.ApproximateValue.ToString().Split('.')[0];
                    int.TryParse(value, out vehicleValue);
                    requestModel.Vehicle.ApproximateValue = vehicleValue;
                }
                if (requestModel.Vehicle.ApproximateValue <= 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.VehicleValueZero;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Vehicle value can't be zero requestModel.Vehicle.ApproximateValue is 0 as we recive " + comingValue;
                    output.Log = log;
                    return output;
                }
                if (!int.TryParse(requestModel.Vehicle.ApproximateValue.ToString(), out vehicleValue))
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.VehicleValueInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel.Vehicle.ApproximateValue is invalid as we recive " + requestModel.Vehicle.ApproximateValue.ToString();
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.ApproximateValue < 10000) //this condition addded by Mubark and Felwa and ticket # VW-376 
                {
                    requestModel.Vehicle.ApproximateValue = 10000;
                    //output.ErrorCode = ValidationOutput.ErrorCodes.VehicleValueLessThan10K;
                    //output.ErrorDescription = SubmitInquiryResource.VehicleValueLessThan10K;
                    //log.ErrorCode = (int)output.ErrorCode;
                    //log.ErrorDescription = "Vehicle value less than 10000 as we recived " + requestModel.Vehicle.ApproximateValue;
                    //output.Log = log;
                    //return output;
                }
                if (requestModel.Vehicle.OwnerTransfer && requestModel.Vehicle.OwnerNationalId == requestModel.Insured.NationalId)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.OwnerNationalIdAndNationalIdAreEqual;
                    output.ErrorDescription = SubmitInquiryResource.InvaildOldOwnerNin;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and Vehicle.OwnerNationalId and Insured.NationalId are the same";
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.OwnerTransfer && !requestModel.OldOwnerNin.HasValue && channel.ToLower() == "portal")
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.VehicleOwnerNinIsNull;
                    output.ErrorDescription = SubmitInquiryResource.VehicleOwnerNinIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "OwnerTransfer and OldOwnerNin is null";
                    output.Log = log;
                    return output;
                }
                if (requestModel.Vehicle.HasTrailer) // this condition added by Fayssal & Mubark and ticket #VW-707
                {
                    if (requestModel.Vehicle.ApproximateTrailerSumInsured <= 0)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.TrailerSumInsuredValueZero;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "TrailerSumInsured value can't be zero requestModel.Vehicle.TrailerSumInsured is 0 as we recive " + requestModel.Vehicle.ApproximateTrailerSumInsured;
                        output.Log = log;
                        return output;
                    }

                    var trailerSumInsuredValue = 0;
                    if (!int.TryParse(requestModel.Vehicle.ApproximateTrailerSumInsured.ToString(), out trailerSumInsuredValue))
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.TrailerSumInsuredValueInvalid;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "requestModel.Vehicle.TrailerSumInsured is invalid as we recive " + requestModel.Vehicle.ApproximateTrailerSumInsured.ToString();
                        output.Log = log;
                        return output;
                    }
                    else if (trailerSumInsuredValue < 1000 || trailerSumInsuredValue > 999999)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.TrailerSumInsuredValueOutOfRange;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "TrailerSumInsured value must be between 1000 and 999,999";
                        output.Log = log;
                        return output;
                    }
                }
                else
                {
                    requestModel.Vehicle.ApproximateTrailerSumInsured = 0;
                }

                if (requestModel.IsRenewalRequest && !string.IsNullOrEmpty(requestModel.PreviousReferenceId))
                {
                    requestModel.Vehicle.OwnerTransfer = false;
                    long oldownerNin = 0;
                    long.TryParse(requestModel.Insured.NationalId, out oldownerNin);
                    requestModel.OldOwnerNin = oldownerNin;
                    requestModel.Vehicle.OwnerNationalId = requestModel.Insured.NationalId;
                    var oldPolicy = _policyRepository.TableNoTracking.Where(q => q.CheckOutDetailsId == requestModel.PreviousReferenceId).FirstOrDefault();
                    if (oldPolicy != null)
                    {
                        if ((string.IsNullOrEmpty(log.UserId) || log.UserId == "00000000-0000-0000-0000-000000000000") && log.Channel.ToLower() != "ivr")
                        {
                            var checkoutDetails = _checkoutsService.GetFromCheckoutDeatilsbyReferenceId(requestModel.PreviousReferenceId);
                            if (checkoutDetails != null)
                            {
                                var user = _authorizationService.GetUserDBByID(checkoutDetails.UserId);
                                if (user != null)
                                {
                                    string exception = string.Empty;
                                    bool value = SetUserAuthenticationCookies(user, out exception);
                                    if (!value)
                                    {
                                        output.ErrorCode = ValidationOutput.ErrorCodes.AccessTokenResultNull;
                                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                                        log.ErrorCode = (int)output.ErrorCode;
                                        log.ErrorDescription = "SetUserAuthenticationCookies return false";
                                        output.Log = log;
                                    }

                                    var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                                    if (accessTokenResult == null)
                                    {
                                        output.ErrorCode = ValidationOutput.ErrorCodes.AccessTokenResultNull;
                                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                                        log.ErrorCode = (int)output.ErrorCode;
                                        log.ErrorDescription = "accessTokenResult is null";
                                        output.Log = log;
                                    }

                                    if (string.IsNullOrEmpty(accessTokenResult.access_token))
                                    {
                                        output.ErrorCode = ValidationOutput.ErrorCodes.AccessTokenResultNull;
                                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo("ar"));
                                        log.ErrorCode = (int)output.ErrorCode;
                                        log.ErrorDescription = "accessTokenResult.access_token is null";
                                        output.Log = log;
                                    }
                                    output.UserName = user.Email;
                                    output.UserId = user.Id;
                                    output.AccessToken = accessTokenResult.access_token;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(requestModel.Vehicle.CustomCardNumber))
                        {
                            var customCardIno = _customCardInfoRepository.TableNoTracking.Where(a => a.CustomCardNumber == requestModel.Vehicle.CustomCardNumber).FirstOrDefault();
                            if (customCardIno != null)
                            {
                                Vehicle vehicleInfo = new Vehicle();
                                vehicleInfo.ID = Guid.NewGuid();
                                vehicleInfo.OwnerTransfer = requestModel.Vehicle.OwnerTransfer;

                                int vehicelValue = 0;
                                int.TryParse(requestModel.Vehicle.ApproximateValue.ToString(), out vehicelValue);
                                vehicleInfo.VehicleValue = vehicelValue;
                                vehicleInfo.IsUsedCommercially = requestModel.IsVehicleUsedCommercially;
                                vehicleInfo.TransmissionTypeId = requestModel.Vehicle.TransmissionTypeId;
                                vehicleInfo.ParkingLocationId = requestModel.Vehicle.ParkingLocationId;
                                vehicleInfo.HasModifications = requestModel.Vehicle.HasModification;
                                vehicleInfo.ModificationDetails = requestModel.Vehicle.Modification;
                                vehicleInfo.MileageExpectedAnnualId = requestModel.Vehicle.MileageExpectedAnnualId;
                                vehicleInfo.VehicleIdTypeId = requestModel.Vehicle.VehicleIdTypeId;
                                vehicleInfo.CarOwnerNIN = requestModel.Vehicle.OwnerNationalId;
                                vehicleInfo.BrakeSystemId = (BrakingSystem?)requestModel.Vehicle.BrakeSystemId;
                                vehicleInfo.CruiseControlTypeId = (CruiseControlType?)requestModel.Vehicle.CruiseControlTypeId;
                                vehicleInfo.ParkingSensorId = (ParkingSensors?)requestModel.Vehicle.ParkingSensorId;
                                vehicleInfo.CameraTypeId = (VehicleCameraType?)requestModel.Vehicle.CameraTypeId;
                                vehicleInfo.CurrentMileageKM = requestModel.Vehicle.CurrentMileageKM;
                                vehicleInfo.HasAntiTheftAlarm = requestModel.Vehicle.HasAntiTheftAlarm;
                                vehicleInfo.HasFireExtinguisher = requestModel.Vehicle.HasFireExtinguisher;

                                byte cylinders = 0;
                                byte.TryParse(customCardIno.Cylinders.ToString(), out cylinders);
                                vehicleInfo.Cylinders = cylinders;
                                if (requestModel.Vehicle.LicenseExpiryDate != null)
                                {
                                    vehicleInfo.LicenseExpiryDate = Utilities.HandleHijriDate(requestModel.Vehicle.LicenseExpiryDate);
                                    vehicleInfo.LicenseExpiryDate = Utilities.FormatDateString(vehicleInfo.LicenseExpiryDate);
                                }
                                vehicleInfo.MinorColor = requestModel.Vehicle.MinorColor;
                                var vehicleColor = vehicleService.GetVehicleColor(customCardIno.MajorColor);
                                if (vehicleColor != null)
                                {
                                    vehicleInfo.MajorColor = vehicleColor.YakeenColor;
                                    vehicleInfo.ColorCode = vehicleColor.YakeenCode;
                                }
                                else
                                {
                                    vehicleInfo.MajorColor = customCardIno.MajorColor;
                                    if (requestModel.Vehicle.ColorCode.HasValue)
                                        vehicleInfo.ColorCode = requestModel.Vehicle.ColorCode.Value;
                                    else
                                        vehicleInfo.ColorCode = 99;

                                }
                                byte platetypeCode = 0;
                                byte.TryParse(customCardIno.PlateTypeCode.ToString(), out platetypeCode);
                                vehicleInfo.PlateTypeCode = platetypeCode;
                                vehicleInfo.ModelYear = customCardIno.ModelYear;
                                vehicleInfo.RegisterationPlace = customCardIno.RegisterationPlace;
                                vehicleInfo.VehicleBodyCode = requestModel.Vehicle.VehicleBodyCode;
                                vehicleInfo.VehicleWeight = requestModel.Vehicle.VehicleWeight;
                                vehicleInfo.VehicleLoad = requestModel.Vehicle.VehicleLoad;
                                vehicleInfo.VehicleMaker = customCardIno.VehicleMaker;
                                vehicleInfo.VehicleModel = customCardIno.VehicleModel;

                                short makerCode = 0;
                                short.TryParse(customCardIno.VehicleMakerCode.ToString(), out makerCode);
                                vehicleInfo.VehicleMakerCode = makerCode;
                                vehicleInfo.VehicleModelCode = customCardIno.VehicleModelCode;
                                vehicleInfo.ChassisNumber = customCardIno.ChassisNumber;
                                vehicleInfo.SequenceNumber = customCardIno.SequenceNumber.ToString();
                                vehicleInfo.CarPlateText1 = customCardIno.CarPlateText1;
                                vehicleInfo.CarPlateText2 = customCardIno.CarPlateText2;
                                vehicleInfo.CarPlateText3 = customCardIno.CarPlateText3;
                                vehicleInfo.CarPlateNumber = customCardIno.CarPlateNumber;
                                vehicleInfo.CarOwnerName = customCardIno.CarOwnerName;
                                if (customCardIno.PlateTypeCode == (int)VehicleIdType.CustomCard || requestModel.Insured.NationalId.StartsWith("7"))
                                {
                                    vehicleInfo.VehicleUseId = (requestModel.IsVehicleUsedCommercially) ? (int)VehicleUse.Commercial : (int)VehicleUse.Private;
                                }
                                else
                                {
                                    var allVehicleUsages = vehicleService.GetVehicleUsage();
                                    if (allVehicleUsages != null && allVehicleUsages.Count > 0)
                                    {
                                        var vehicleUsage = allVehicleUsages.Where(a => a.PlateTypeCode == customCardIno.PlateTypeCode).FirstOrDefault();
                                        vehicleInfo.VehicleUseId = (vehicleUsage != null) ? vehicleUsage.VehicleUseCode.Value : (int)VehicleUse.Private;
                                    }
                                    else
                                        vehicleInfo.VehicleUseId = (int)VehicleUse.Private;
                                }

                                yakeenVehicleServices.InsertVehicleIntoDb(vehicleInfo, out string exception);
                                requestModel.Vehicle.ID = vehicleInfo.ID;
                                requestModel.Vehicle.VehicleIdTypeId = (int)VehicleIdType.SequenceNumber;
                                requestModel.Vehicle.SequenceNumber = customCardIno.SequenceNumber.ToString();
                                requestModel.Vehicle.ChassisNumber = customCardIno.ChassisNumber;
                                requestModel.Vehicle.VehicleId = customCardIno.SequenceNumber.Value;
                                requestModel.Vehicle.Cylinders = cylinders;
                                requestModel.Vehicle.MajorColor = customCardIno.MajorColor;
                                requestModel.Vehicle.ModelYear = customCardIno.ModelYear;
                                requestModel.Vehicle.VehicleModelYear = customCardIno.ModelYear;
                                requestModel.Vehicle.CarPlateNumber = customCardIno.CarPlateNumber;
                                requestModel.Vehicle.CarPlateText1 = customCardIno.CarPlateText1;
                                requestModel.Vehicle.CarPlateText2 = customCardIno.CarPlateText2;
                                requestModel.Vehicle.CarPlateText3 = customCardIno.CarPlateText3;
                                requestModel.Vehicle.PlateTypeCode = (int)VehicleUse.Private;
                                requestModel.Vehicle.VehicleMaker = customCardIno.VehicleMaker;
                                requestModel.Vehicle.VehicleMakerCode = makerCode;
                                requestModel.Vehicle.Model = customCardIno.VehicleModel;
                                requestModel.Vehicle.VehicleModelCode = customCardIno.VehicleModelCode;
                                requestModel.Vehicle.VehicleWeight = (customCardIno.VehicleWeight.HasValue) ? customCardIno.VehicleWeight.Value : requestModel.Vehicle.VehicleWeight;
                            }
                        }
                        //if (DateTime.Now.Date <= new DateTime(2021, 8, 5) && oldPolicy.PolicyIssueDate >= new DateTime(2019, 5, 6, 0, 0, 0) && oldPolicy.PolicyIssueDate <= new DateTime(2020, 6, 6, 0, 0, 0))
                        //{
                        //    oldPolicy.PolicyExpiryDate = oldPolicy.PolicyExpiryDate.Value.AddMonths(2).AddDays(-1);
                        //}
                        if (oldPolicy.PolicyExpiryDate.HasValue && oldPolicy.PolicyExpiryDate.Value.AddDays(-30).Date <= DateTime.Now.Date)
                        {

                            if (oldPolicy.PolicyExpiryDate <= DateTime.Now)
                            {
                                requestModel.PolicyEffectiveDate = DateTime.Now.Date.AddDays(1);
                            }
                            else
                            {
                                requestModel.PolicyEffectiveDate = oldPolicy.PolicyExpiryDate.Value.AddDays(1);
                            }
                            //string date = requestModel.PolicyEffectiveDate.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));
                            //DateTime dt = new DateTime();
                            //DateTime.TryParse(date, out dt);
                            //requestModel.PolicyEffectiveDate = dt;
                        }
                    }
                }
                if (requestModel.PolicyEffectiveDate < DateTime.Now.Date.AddDays(1) || requestModel.PolicyEffectiveDate > DateTime.Now.AddDays(30))
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.InvalidPolicyEffectiveDate;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy effective date should be within 30 days starts from toworrow as we received " + requestModel.PolicyEffectiveDate + " IsRenewalRequest:" + requestModel.IsRenewalRequest.ToString();
                    output.Log = log;
                    return output;
                }
                long mainDriverNin = 0;
                //Driver Validations 
                DriverModel mainDriver = null;
                if (requestModel.Insured.NationalId.StartsWith("7"))
                {
                    if (log.Channel.ToLower() == "portal" && !requestModel.IsRenewalRequest)
                    {
                        mainDriver = requestModel.Drivers.Where(a => a.IsCompanyMainDriver).FirstOrDefault();
                    }
                    else
                    {
                        mainDriver = requestModel.Drivers.Where(a => a.NationalId != requestModel.Insured.NationalId).FirstOrDefault();
                    }
                    mainDriverNin = long.Parse(mainDriver.NationalId);
                }
                else
                {
                    mainDriverNin = long.Parse(requestModel.Insured.NationalId);
                    mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == mainDriverNin.ToString());
                }
                if (mainDriver == null)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Main Driver Not Exist";
                    output.Log = log;
                    return output;
                }
                if (requestModel.Insured.NationalId.StartsWith("7")&& mainDriverNin ==1011282652 )
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.InvalidMainDriverNinWith700;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "national ID 1011282652 is not allowed for purchased with 700 as per Mr Khalid";
                    output.Log = log;
                    return output;
                }
                if (requestModel.Insured.NationalId.StartsWith("7"))
                {
                    string exception = string.Empty;
                    //List<InsuredPolicyInfo> userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesDetails(mainDriverNin.ToString(), out exception);
                    CorporateModel userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesDetailsForCorprate(mainDriverNin.ToString(), out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "There is an error while checking for main driver policies, and the error is: " + exception;
                        output.Log = log;
                        return output;
                    }

                    List<InsuredPolicyInfo> companyPolicies = null;
                    if (userSuccessPoliciesDetails.InsuredPolicies != null && userSuccessPoliciesDetails.InsuredPolicies.Count > 0)
                        companyPolicies = userSuccessPoliciesDetails.InsuredPolicies.Where(a => a.IsCompany == true).ToList();
                    
                    if (companyPolicies != null && companyPolicies.Count >= 1)
                    {
                        if (companyPolicies.Count >= 5)
                        {
                            output.ErrorCode = ValidationOutput.ErrorCodes.CompanyDriverExceedsInsuranceNumberLimitPerYear;
                            output.ErrorDescription = SubmitInquiryResource.CompanyDriverExceedsInsuranceNumberLimitPerYear;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Company Driver " + mainDriverNin + " Exceeds Insurance Number Limit Per Year: 5";
                            output.Log = log;
                            return output;
                        }
                        if (!companyPolicies.Any(a => a.NationalId == requestModel.Insured.NationalId))
                        {
                            output.ErrorCode = ValidationOutput.ErrorCodes.CompanyDriverIsAssignedToAnotherCompany;
                            output.ErrorDescription = SubmitInquiryResource.CompanyDriverIsAssignedToAnotherCompany;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Company driver is assigned to another company, and the current 700 is not one of them";
                            output.Log = log;
                            return output;
                        }
                    }

                    if (userSuccessPoliciesDetails.EdaatRequest != null)
                    {
                        if (!ValidateEdaatWithCompanies(userSuccessPoliciesDetails.EdaatRequest, companyPolicies, requestModel.Insured.NationalId))
                        {
                            var lang = !string.IsNullOrEmpty(requestModel.Language) ? requestModel.Language : "ar";
                            var remainingEdaatRequestHours = userSuccessPoliciesDetails.EdaatRequest.ExpiryDate.Value - DateTime.Now;

                            output.ErrorCode = ValidationOutput.ErrorCodes.CompanyDriverIsAssignedToAnotherCompany;
                            output.ErrorDescription = string.Format(SubmitInquiryResource.ResourceManager.GetString("DriverHasEdaatValidRequest", CultureInfo.GetCultureInfo(lang)), $"{remainingEdaatRequestHours.Hours.ToString("00")}:{remainingEdaatRequestHours.Minutes.ToString("00")}:{remainingEdaatRequestHours.Seconds.ToString("00")}");
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Driver with nin: " + mainDriverNin + " has active edaat request with insured id: " + userSuccessPoliciesDetails.EdaatRequest.InsuredNationalId;
                            log.ErrorCode = (int)output.ErrorCode;
                            output.Log = log;
                            return output;
                        }
                    }

                    requestModel.Insured.BirthDateMonth = mainDriver.BirthDateMonth;
                    requestModel.Insured.BirthDateYear = mainDriver.BirthDateYear;
                }
                if (requestModel.Insured.BirthDateMonth == 0 || requestModel.Insured.BirthDateYear == 0)
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BirthDate can't be zero month:" + mainDriver.BirthDateMonth + " Year:" + mainDriver.BirthDateYear;
                    output.Log = log;
                    return output;
                }
                output.DriverNin = mainDriverNin;
                output.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();
                //validation added as per fayssal 
                if (mainDriver.EducationId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.EducationIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.EducationIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver EducationId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (mainDriver.MedicalConditionId == 0)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.MedicalConditionIdIsNull;
                    output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "main driver MedicalConditionId is zero"; ;
                    output.Log = log;
                    return output;
                }
                if (!mainDriver.DriverNOALast5Years.HasValue)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DriverNOALast5YearsIsNull;
                    output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DriverNOALast5Years is zero"; ;
                    output.Log = log;
                    return output;
                }
                int additionalDrivingPercentage = 0;
                foreach (var driver in requestModel.Drivers)
                {
                    if (driver.NationalId.StartsWith("7"))
                        continue;
                    if (driver.NationalId == mainDriver.NationalId)
                        continue;
                    if (driver.EducationId == 0)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.EducationIdIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.EducationIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver EducationId is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    if (driver.MedicalConditionId == 0)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.MedicalConditionIdIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver MedicalConditionId is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    if (!driver.DriverNOALast5Years.HasValue)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.DriverNOALast5YearsIsNullAdditionalDriver;
                        output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Additional Driver DriverNOALast5Years is zero for Additional Driver " + driver.NationalId;
                        output.Log = log;
                        return output;
                    }
                    additionalDrivingPercentage += driver.DrivingPercentage;

                }
                if (additionalDrivingPercentage > 100)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DrivingPercentageMoreThan100;
                    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Driving Percentage exceeded 100% for Additional Drivers as it's with total " + additionalDrivingPercentage;
                    output.Log = log;
                    return output;
                }
                int remainingPercentage = 100 - additionalDrivingPercentage;
                requestModel.Drivers.Remove(mainDriver);
                mainDriver.DrivingPercentage = remainingPercentage >= 0 ? remainingPercentage : 0;
                requestModel.Drivers.Insert(0, mainDriver);

                if (additionalDrivingPercentage + mainDriver.DrivingPercentage != 100)
                {
                    output.ErrorCode = ValidationOutput.ErrorCodes.DrivingPercentageMoreThan100;
                    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DrivingPercentage is not 100% as additionalDrivingPercentage is " + additionalDrivingPercentage + " and main is " + mainDriver.DrivingPercentage;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                if (!ExcludedVehicleIds.Contains(requestModel.Vehicle.VehicleId.ToString()))
                {
                    bool vehicleExceededLimit = vehicleService.CheckIfvehicleExceededLimit(requestModel.Vehicle.VehicleId.ToString());
                    if (vehicleExceededLimit)
                    {
                        output.ErrorCode = ValidationOutput.ErrorCodes.VehicleExceededLimit;
                        output.ErrorDescription = SubmitInquiryResource.VehicleExceededLimit;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Vehicle exceeded the maximum limit";
                        output.Log = log;
                        return output;
                    }
                }

                output.ErrorCode = ValidationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Log = log;
                output.RequestModel = requestModel;
                return output;
            }
            catch(Exception exp)
            {
                output.ErrorCode = ValidationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                output.Log = log;
                return output;
            }
        }

        public DriversOutput GetDriversData(List<DriverModel> drivers, InsuredModel insured, bool? isCustomerSpecialNeed, long mainDriverNin, ServiceRequestLog predefinedLogInfo, int? insuranceTypeCode = null, string externalId = null)
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
                        customerInfoRequest.DriverExtraLicenses = driver.DriverExtraLicenses.Where(e => e.CountryId > 0 && e.CountryId != 113) //as jira 349
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
                    Driver customerData = null;
                    if (insuranceTypeCode.HasValue && insuranceTypeCode.Value == 9 && !string.IsNullOrEmpty(externalId))
                    {
                        var oldTplQuote = _quotationService.GetQuotaionDriversForODByExternailId(externalId);
                        if (oldTplQuote != null && (oldTplQuote.Drivers != null && oldTplQuote.Drivers.Count > 0) && oldTplQuote.Drivers.Any(a => a.NIN == NIN.ToString()))
                            customerData = oldTplQuote.Drivers.Where(a => a.NIN == NIN.ToString()).FirstOrDefault();
                    }
                    else
                        customerData = customerServices.getDriverEntityFromNin(NIN);

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
                                if (l.LicenseYearsId > 0 && l.CountryId > 0)
                                {
                                    driverObj.DriverExtraLicenses.Add(new DriverExtraLicense
                                    {
                                        CountryCode = l.CountryId,
                                        LicenseYearsId = l.LicenseYearsId,
                                        DriverId = driverObj.DriverId
                                    });
                                }
                                else if (l.LicenseYearsId <= 0)
                                {
                                    output.ErrorCode = DriversOutput.ErrorCodes.DriverDriverExtraLicenseNumberOfYearsError;
                                    output.ErrorDescription = WebResources.DriverDriverExtraLicenseNumberOfYearsError;
                                    return output;
                                }
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
                        if (!string.IsNullOrEmpty(customerData.OccupationCode) && customerData.OccupationCode == "G" && customerData.CreatedDateTime <= new DateTime(2020,10,13))
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
                            bool _isMale = (driverObj.Gender == Gender.Female) ? false : true;
                            var occupation = occupationService.GetOccupations().Where(x => x.Code == customerData.OccupationCode && x.IsMale == _isMale).FirstOrDefault();
                            if (occupation != null)
                            {
                                driverObj.OccupationName = occupation?.NameAr;
                                driverObj.OccupationCode = occupation?.Code;
                                driverObj.OccupationId = occupation.ID;
                            }
                            else
                            {
                                driverObj.OccupationName = customerData.OccupationName;
                                driverObj.OccupationCode = customerData.OccupationCode;
                                driverObj.OccupationId = customerData.OccupationId;
                            }
                        }
                        else
                        {
                            var occupation = occupationService.GetOccupations().Where(x => x.ID == customerData.OccupationId).FirstOrDefault();
                            if (occupation != null)
                            {
                                driverObj.OccupationName = occupation?.NameAr;
                                driverObj.OccupationCode = occupation?.Code;
                                driverObj.OccupationId = occupation?.ID;
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
                                if(item.TypeDesc.HasValue)
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
                        if (driverToInsert.IsCitizen && string.IsNullOrEmpty(driverToInsert.DateOfBirthH)) //add birthDate manual if yaqeen response HijriDate is NULL
                        {
                            driverToInsert.DateOfBirthH = string.Format("01-{0}-{1}", driver.BirthDateMonth.ToString("00"), driver.BirthDateYear);
                        }

                        if (driver.NationalId == mainDriverNin.ToString()) // main Driver
                        {
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
            catch(Exception exp)
            {
                output.ErrorCode = DriversOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogErrorDescription = exp.ToString();
                return output;
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

        public InquiryOutput SubmitInquiryRequest(InquiryRequestModel requestModel, string channel, Guid userId, string userName, Guid? parentRequestId = null)
        {
            requestModel.InquiryOutputModel = new InquiryOutputModel();
            InquiryOutput output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.Channel = channel.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = channel;
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "SubmitInquiryRequest";
            log.NIN = requestModel.Insured.NationalId;
            log.VehicleId = requestModel.Vehicle.VehicleId.ToString();
            log.RequestId = requestModel.ParentRequestId;
            log.ServiceRequest = JsonConvert.SerializeObject(requestModel);
            log.MobileVersion = requestModel.MobileVersion;
            try
            {
                var validationOutput = ValidateData(requestModel, log, channel);
                if (validationOutput.ErrorCode != ValidationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(validationOutput.Log);
                    return output;
                }
                requestModel = validationOutput.RequestModel;

                if (requestModel.ODInsuranceTypeCode.HasValue && requestModel.ODInsuranceTypeCode.Value == 9 && requestModel.ODPolicyExpiryDate.HasValue)
                {
                    if (requestModel.ODPolicyExpiryDate.HasValue && requestModel.ODPolicyExpiryDate.Value.Date.CompareTo(DateTime.Now.AddDays(90).Date) <= 0)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.InvalidODPolicyExpiryDate;
                        output.ErrorDescription = SubmitInquiryResource.InvalidODPolicyExpiryDate;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"Can't proceed OD request , as TPL policy expiry date is {requestModel.ODPolicyExpiryDate.Value} and it's less than 90 days";
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                }

                var result = HandleQuotationRequest(requestModel, validationOutput.DriverNin, ref log);

                output.InquiryResponseModel = result.InquiryResponseModel;
                if (!(string.IsNullOrEmpty(result.ErrorDescription)) && result.ErrorCode != InquiryOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = result.ErrorCode;
                    output.ErrorDescription = result.ErrorDescription;
                    //log.ErrorCode = (int)output.ErrorCode;
                    //log.ErrorDescription = output.ErrorDescription;
                    //InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                //get quotation request
                var quotationRequest = GetQuotationRequestInfo(output.InquiryResponseModel.QuotationRequestExternalId);
                if (quotationRequest == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "There is no quotation request with the given external id.";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var yakeenOutput = HandleYakeenMissingFields(output, quotationRequest, requestModel.Language);
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                if (output.InquiryResponseModel!=null&& output.InquiryResponseModel.YakeenMissingFields != null && output.InquiryResponseModel.YakeenMissingFields.Count > 0)
                {
                    List<string> missingKeys =new List<string>();
                    foreach(var missingField in output.InquiryResponseModel.YakeenMissingFields)
                    {
                        missingKeys.Add(missingField.Key);
                    }
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success with missing feilds:" +JsonConvert.SerializeObject(missingKeys);
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                }
                //log.ErrorCode = (int)output.ErrorCode;
                //log.ErrorDescription = output.ErrorDescription;
                //InquiryRequestLogDataAccess.AddInquiryRequestLog(log);

                //VehicleModel for mobile
                if (output.InquiryResponseModel.Vehicle != null)
                {
                    var CarPlate = new Tamkeen.bll.Model.CarPlateInfo(output.InquiryResponseModel.Vehicle.CarPlateText1,
                            output.InquiryResponseModel.Vehicle.CarPlateText2, output.InquiryResponseModel.Vehicle.CarPlateText3,
                            output.InquiryResponseModel.Vehicle.CarPlateNumber.HasValue ? output.InquiryResponseModel.Vehicle.CarPlateNumber.Value : 0);
                    output.InquiryResponseModel.Vehicle.CarPlateNumberAr = CarPlate.CarPlateNumberAr;
                    output.InquiryResponseModel.Vehicle.CarPlateNumberEn = CarPlate.CarPlateNumberEn;
                    output.InquiryResponseModel.Vehicle.CarPlateTextAr = CarPlate.CarPlateTextAr;
                    output.InquiryResponseModel.Vehicle.CarPlateTextEn = CarPlate.CarPlateTextEn;
                }
                string exception = string.Empty;
                bool ret = vehicleService.InsertIntoVehicleRequests(requestModel.Vehicle.VehicleId.ToString(), requestModel.Insured.NationalId, requestModel.CityCode, out exception);
                if(!ret)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.FailedToSaveInVehicleRequests;
                    output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to Save to VehicleRequests due to "+exception;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                }
                if (!string.IsNullOrEmpty(validationOutput.AccessToken))
                {
                    output.InquiryResponseModel.UserId = validationOutput.UserId;
                    output.InquiryResponseModel.UserName = validationOutput.UserName;
                    output.InquiryResponseModel.AccessToken = validationOutput.AccessToken;
                }
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }

        public InquiryOutput HandleQuotationRequest(InquiryRequestModel requestModel, long mainDriverNin, ref InquiryRequestLog log)
        {
            string exception = string.Empty;
            InquiryOutput output = new InquiryOutput
            {
                InquiryResponseModel = new InquiryResponseModel()
            };
            try
            {
                Guid userID = Guid.Empty;
                Guid.TryParse(log.UserId, out userID);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userID,
                    UserName = log.UserName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP
                };
                predefinedLogInfo.RequestId = log.RequestId;
                predefinedLogInfo.DriverNin = mainDriverNin.ToString();
                predefinedLogInfo.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();

                var qtRqstExtrnlId = GetNewRequestExternalId();
                log.ExternalId = qtRqstExtrnlId;
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;

                //1) Vehicle
                var vehicleYakeenInfo = GetVehicleYakeenInfo(requestModel, predefinedLogInfo);

                if (vehicleYakeenInfo == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "GetVehicleYakeenInfo returned null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (vehicleYakeenInfo != null && vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorCode == "0")
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "GetVehicleYakeenInfo returned " + vehicleYakeenInfo.Error.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (!vehicleYakeenInfo.Success)
                {
               
                    if (vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorMessage != null
                        && ((vehicleYakeenInfo.Error.Type == EErrorType.YakeenError ||
                        (vehicleYakeenInfo.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))))
                    {
                        string ErrorMessage = SubmitInquiryResource.ResourceManager.GetString($"YakeenError_{vehicleYakeenInfo.Error?.ErrorCode}", CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));
                        var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                        output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = GenericErrorMessage;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"GetVehicleYakeenInfo returned code:{vehicleYakeenInfo.Error?.ErrorCode} | message:{vehicleYakeenInfo.Error.ErrorMessage} | description: {vehicleYakeenInfo.Error.ErrorDescription}";
                    }
                    else
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = "";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Service Exception for GetVehicleYakeenInfo";
                    }
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }

                //2) Drivers
                var driversoutput = GetDriversData(requestModel.Drivers, requestModel.Insured, requestModel.IsCustomerSpecialNeed, mainDriverNin, predefinedLogInfo, requestModel.ODInsuranceTypeCode, requestModel.ODTPLExternalId);
                if (driversoutput.ErrorCode != DriversOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = driversoutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driversoutput.LogErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var customerYakeenInfo = driversoutput.MainDriver.ToCustomerModel(); //-- to be handeled
                //3)Validate Address
                int addressId = 0;
                string postCode = string.Empty;
                #region Validate Address
                string birthDate = string.Format("{0}-{1}", requestModel.Insured.BirthDateMonth.ToString("00"), requestModel.Insured.BirthDateYear);
                bool isODProduct = (requestModel.ODInsuranceTypeCode.HasValue && requestModel.ODInsuranceTypeCode.Value == 9) ? true : false;
                var driverCityInfoOutput = GetDriverCityInfo(driversoutput.MainDriver.DriverId, mainDriverNin.ToString(), log.VehicleId, log.Channel, birthDate, true, qtRqstExtrnlId, isODProduct, requestModel.ODTPLExternalId);
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.SaudiPostNoResultReturned
                    || driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.NoAddressFound)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostNoResultReturned;
                    output.ErrorDescription = SubmitInquiryResource.MissingAddress;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostNullResponse;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostError;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode == DriverCityInfoOutput.ErrorCodes.InvalidPublicID)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostInvalidPublicID.Replace("{0}", mainDriverNin.ToString());
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                if (driverCityInfoOutput.ErrorCode != DriverCityInfoOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.SaudiPostError;
                    output.ErrorDescription = SubmitInquiryResource.SaudiPostError;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverCityInfoOutput.ErrorDescription;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                var cities = addressService.GetAllCities();
                long elmCode = driverCityInfoOutput.Output.ElmCode;
                addressId = driverCityInfoOutput.Output.AddressId;
                postCode = driverCityInfoOutput.Output.PostCode;

                requestModel.CityCode = elmCode;
                //Main Driver
                var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == mainDriverNin.ToString());
                var mainDriverDB = driverRepository.Table.Where(d => d.DriverId == driversoutput.MainDriver.DriverId).FirstOrDefault();

                mainDriverDB.CityId = elmCode;
                mainDriverDB.CityName = driverCityInfoOutput.Output.CityNameAr;
                mainDriverDB.AddressId = addressId;
                mainDriverDB.PostCode = postCode;
                if (mainDriver.DriverWorkCityCode.HasValue && !requestModel.Insured.InsuredWorkCityCode.HasValue)
                {
                    mainDriverDB.WorkCityName = mainDriver.DriverWorkCity;
                    mainDriverDB.WorkCityId = mainDriver.DriverWorkCityCode;
                }
                else if (requestModel.Insured.InsuredWorkCityCode.HasValue)
                {
                    mainDriverDB.WorkCityId = requestModel.Insured.InsuredWorkCityCode;
                    mainDriverDB.WorkCityName = cities.FirstOrDefault(c => c.Code == requestModel.Insured.InsuredWorkCityCode)?.ArabicDescription;
                }
                else
                {
                    mainDriverDB.WorkCityName = driverCityInfoOutput.Output.CityNameAr;
                    mainDriverDB.WorkCityId = elmCode;
                }

                if (driversoutput.AdditionalDrivers != null && driversoutput.AdditionalDrivers.Any())
                {
                    foreach (var additionalDriver in driversoutput.AdditionalDrivers)
                    {
                        var additionalDriverDB = driverRepository.Table.Where(d => d.DriverId == additionalDriver.DriverId).FirstOrDefault();

                        if (!additionalDriver.CityId.HasValue)
                        {
                            int cityId = 0;
                            int.TryParse(driversoutput.MainDriver.CityId?.ToString(), out cityId);
                            additionalDriver.CityName = driversoutput.MainDriver.CityName;
                            additionalDriver.CityId = cityId;
                        }
                        if (!additionalDriver.WorkCityId.HasValue)
                        {
                            int workCtyId = 0;
                            int.TryParse(driversoutput.MainDriver.WorkCityId?.ToString(), out workCtyId);
                            additionalDriver.WorkCityName = driversoutput.MainDriver.WorkCityName;
                            additionalDriver.WorkCityId = workCtyId;
                        }

                        driverRepository.Update(additionalDriverDB);
                    }
                }
                #endregion

                NajmResponse najmResponse = null;
                // NCDFreeYears will be 0 for Insured 700 As per Khaled & Mubarak & M.Al Majed (https://bcare.atlassian.net/browse/VW-701)
                if (requestModel.Insured.NationalId.StartsWith("7"))
                {
                    mainDriverDB.NCDFreeYears = 0;
                    mainDriverDB.NCDReference = null;
                }
                else
                {
                    //GetNajmResponse
                    NajmRequest najmRequest = new NajmRequest()
                    {
                        IsVehicleRegistered = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                        PolicyHolderNin = mainDriverNin,
                        VehicleId = requestModel.Vehicle.VehicleId
                    };
                    najmResponse = GetNajmResponse(najmRequest, predefinedLogInfo, isODProduct, requestModel.ODTPLExternalId);

                    if (najmResponse == null || najmResponse.StatusCode != (int)NajmOutput.ErrorCodes.Success)
                    {
                        driverRepository.Update(mainDriverDB);
                    }
                    if (najmResponse == null)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.GeneralError;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Najm Response Is null";
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.CorporatePolicyHolderIDIsNOTAllowed)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.CorporatePolicyHolderIDIsNOTAllowed;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.InvalidPolicyholderID)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.InvalidPolicyholderID;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode == (int)NajmOutput.ErrorCodes.InvalidVehicleID)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.InvalidVehicleID;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }
                    if (najmResponse.StatusCode != (int)NajmOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = InquiryOutput.ErrorCodes.NajmInsuredResponseError;
                        output.ErrorDescription = NajmExceptionResource.GeneralError;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "GetNajmResponse return " + najmResponse.ErrorMsg;
                        InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                        return output;
                    }

                    mainDriverDB.NCDFreeYears = najmResponse.NCDFreeYears;
                    mainDriverDB.NCDReference = najmResponse.NCDReference;

                    log.NajmNcdRefrence = najmResponse.NCDReference;
                    log.NajmNcdFreeYears = najmResponse.NCDFreeYears;
                }
                
                driverRepository.Update(mainDriverDB);

                // Insured
                if (requestModel.Insured.NationalId.StartsWith("7")) //Company
                {
                    customerYakeenInfo.NIN = requestModel.Insured.NationalId;
                    if (requestModel.Vehicle.OwnerTransfer || requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                    {
                        WathqInfo wathqInfo = _wathqService.GetTreeFromWathqResponseCache(requestModel.Insured.NationalId, out exception);
                        if (wathqInfo != null && string.IsNullOrEmpty(exception))
                        {
                            customerYakeenInfo.FirstName = wathqInfo.CrName;
                            customerYakeenInfo.EnglishFirstName = wathqInfo.CrName;
                            customerYakeenInfo.SecondName = string.Empty;
                            customerYakeenInfo.ThirdName = string.Empty;
                            customerYakeenInfo.LastName = string.Empty;
                            customerYakeenInfo.EnglishSecondName = string.Empty;
                            customerYakeenInfo.EnglishThirdName = string.Empty;
                            customerYakeenInfo.EnglishLastName = string.Empty;
                        }
                        else
                        {
                            ServiceRequestLog wathqServiceRequestLog = new ServiceRequestLog();
                            wathqServiceRequestLog.UserID = predefinedLogInfo.UserID;
                            wathqServiceRequestLog.Channel = predefinedLogInfo.Channel;
                            wathqServiceRequestLog.VehicleId = predefinedLogInfo.VehicleId;
                            wathqServiceRequestLog.DriverNin = predefinedLogInfo.DriverNin;

                            var wathqResponse = _wathqService.GetTreeFromWathqResponse(requestModel.Insured.NationalId, wathqServiceRequestLog);
                            if (wathqResponse != null && wathqResponse.ErrorCode == WathqOutput.ErrorCodes.Success)
                            {
                                customerYakeenInfo.FirstName = wathqResponse.WathqResponseModel.CrName;
                                customerYakeenInfo.EnglishFirstName = wathqResponse.WathqResponseModel.CrName;
                                customerYakeenInfo.SecondName = string.Empty;
                                customerYakeenInfo.ThirdName = string.Empty;
                                customerYakeenInfo.LastName = string.Empty;
                                customerYakeenInfo.EnglishSecondName = string.Empty;
                                customerYakeenInfo.EnglishThirdName = string.Empty;
                                customerYakeenInfo.EnglishLastName = string.Empty;
                            }
                            else
                            {
                                log.ErrorCode = (int)InquiryOutput.ErrorCodes.WathqError;
                                log.ErrorDescription = "Error Get Wathq Response " + exception;
                                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                            }
                        }
                    }

                }
                var insured = SaveInsured(customerYakeenInfo, requestModel, addressId);

                if (userID == Guid.Empty && log.UserName.ToLower().Trim() == "anonymous")
                {
                    userID = Guid.Empty;
                }
                DateTime dtNow = DateTime.Now;
                var RequestPolicyEffectiveDate = new DateTime(requestModel.PolicyEffectiveDate.Year,
                    requestModel.PolicyEffectiveDate.Month, requestModel.PolicyEffectiveDate.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                if (dtNow.Hour==23&&dtNow.Minute>=45&& RequestPolicyEffectiveDate.Date == dtNow.Date.AddDays(1))
                {
                    RequestPolicyEffectiveDate = new DateTime(requestModel.PolicyEffectiveDate.Year,
                    requestModel.PolicyEffectiveDate.Month, requestModel.PolicyEffectiveDate.Day, 0, 0, 0).AddDays(1);
                }
                var quotationRequest = new QuotationRequest()
                {
                    ExternalId = qtRqstExtrnlId,
                    MainDriverId = customerYakeenInfo.TameenkId,
                    CityCode = requestModel.CityCode,
                    RequestPolicyEffectiveDate =new DateTime(RequestPolicyEffectiveDate.Year, RequestPolicyEffectiveDate.Month, RequestPolicyEffectiveDate.Day, RequestPolicyEffectiveDate.Hour, RequestPolicyEffectiveDate.Minute, RequestPolicyEffectiveDate.Second),
                    VehicleId = vehicleYakeenInfo.TameenkId,
                    UserId = userID == Guid.Empty ? null : userID.ToString(),
                    NajmNcdRefrence = mainDriverDB.NCDReference, //najmResponse.NCDReference,
                    NajmNcdFreeYears = mainDriverDB.NCDFreeYears, //najmResponse.NCDFreeYears,
                    CreatedDateTime = DateTime.Now,
                    Insured = insured,
                    IsRenewal = requestModel.IsRenewalRequest,
                    PostCode = postCode,
                    PreviousReferenceId = requestModel.PreviousReferenceId
                };
                //get cached NOA
                var benchmarkDate = DateTime.Now.AddDays(-45);
                var najmNoA = _najmAccidentResponseRepository.TableNoTracking.Where(d => d.DriverNin == predefinedLogInfo.DriverNin && d.IsDeleted == false && d.CreatedDate > benchmarkDate).FirstOrDefault();
                if (najmNoA != null)
                {
                    NajmOutput NumberOfAccidentResponse = new NajmOutput();
                    ResponseData response = JsonConvert.DeserializeObject<ResponseData>(najmNoA.NajmResponse);
                    NumberOfAccidentResponse.NajmDriverCaseResponse = response;
                    quotationRequest.NoOfAccident = (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID != "101")
                                             ? NumberOfAccidentResponse.NajmDriverCaseResponse.CaseDetails.Noofaccident
                                             : 0;
                    quotationRequest.NajmResponse = (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID != "101")
                                                        ? JsonConvert.SerializeObject(NumberOfAccidentResponse.NajmDriverCaseResponse.CaseDetails.CaseDetail)
                                                        : NumberOfAccidentResponse.NajmDriverCaseResponse.ReferenceNo;
                }

                log.CityCode = Convert.ToInt32(quotationRequest.CityCode);
                log.PolicyEffectiveDate = requestModel.PolicyEffectiveDate;

                //Add Main Driver to additionalDriversYakeenInfo
                quotationRequest.Drivers = new List<Driver>();

                //driversoutput.MainDriver.NCDFreeYears = najmResponse.NCDFreeYears;
                //driversoutput.MainDriver.NCDReference = najmResponse.NCDReference;
                //driversoutput.MainDriver.Addresses = driverCityInfoOutput.Addresses;
                quotationRequest.Drivers.Add(driversoutput.MainDriver);
                int index = 0;
                if (driversoutput.AdditionalDrivers != null && driversoutput.AdditionalDrivers.Any())
                {
                    foreach (var additionalDriver in driversoutput.AdditionalDrivers)
                    {
                        index++;
                        if (index == 1)
                        {
                            quotationRequest.AdditionalDriverIdOne = additionalDriver.DriverId;
                        }
                        else if (index == 2)
                        {
                            quotationRequest.AdditionalDriverIdTwo = additionalDriver.DriverId;
                        }
                        else if (index == 3)
                        {
                            quotationRequest.AdditionalDriverIdThree = additionalDriver.DriverId;
                        }
                        else if (index == 4)
                        {
                            quotationRequest.AdditionalDriverIdFour = additionalDriver.DriverId;
                        }
                        quotationRequest.Drivers.Add(additionalDriver);
                    }
                }
                quotationRequestRepository.Insert(quotationRequest);
                // add violations for main driver
                if (mainDriver.ViolationIds != null && mainDriver.ViolationIds.Count() > 0)
                {
                    List<DriverViolation> mainDriverviolations = new List<DriverViolation>();
                    foreach (var violationId in mainDriver.ViolationIds.Distinct())
                    {
                        DriverViolation violation = new DriverViolation();

                        violation.DriverId = quotationRequest.MainDriverId;
                        violation.InsuredId = insured.Id;
                        violation.NIN = mainDriver.NationalId;
                        violation.ViolationId = violationId;
                        mainDriverviolations.Add(violation);
                    }
                    if (mainDriverviolations.Count() > 0)
                        driverViolationRepository.Insert(mainDriverviolations);
                }
                // add violations for additional drivers
                var drivers = requestModel.Drivers.Where(x => x.NationalId != mainDriver.NationalId);
                if (drivers != null && drivers.Count() > 0)
                {
                    List<DriverViolation> additionlDriverviolations = new List<DriverViolation>();
                    foreach (var driver in drivers)
                    {
                        if (driver.ViolationIds != null && driver.ViolationIds.Count() > 0)
                        {
                            foreach (var violationId in driver.ViolationIds.Distinct())
                            {
                                var additionalDriver = driversoutput.AdditionalDrivers.Where(x => x.NIN == driver.NationalId).FirstOrDefault();
                                if (additionalDriver == null)
                                    continue;
                                DriverViolation violation = new DriverViolation();
                                violation.DriverId = additionalDriver.DriverId;
                                violation.InsuredId = insured.Id;
                                violation.NIN = driver.NationalId;
                                violation.ViolationId = violationId;
                                additionlDriverviolations.Add(violation);
                            }
                        }
                    }
                    if (additionlDriverviolations.Count() > 0)
                        driverViolationRepository.Insert(additionlDriverviolations);
                }
                var NCDObject = nCDFreeYearRepository.Table.FirstOrDefault(x => x.Code == quotationRequest.Driver.NCDFreeYears);
                output.InquiryResponseModel.QuotationRequestExternalId = qtRqstExtrnlId;
                output.InquiryResponseModel.Vehicle = ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);
                output.InquiryResponseModel.NajmNcdFreeYears = webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? NCDObject?.ArabicDescription : NCDObject?.EnglishDescription;
                output.InquiryResponseModel.NajmNcd = NCDObject;
                return output;
            }
            catch (NajmErrorException ex)
            {
                HandleNajmException(ex, ref output);
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                HandleNajmException(ex, ref output);
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }
        /// <summary>
        /// Check if all info from yakeen are returned, if not then user should enter all the missing fields.
        /// </summary>
        /// <param name="result">Inquiry response model</param>
        /// <returns></returns>
        public InquiryOutput HandleYakeenMissingFields(InquiryOutput output, QuotationRequest quotationRequest, string lang = "")
        {
            //convert to model to validate that all yakeen data are not missing
    
            Driver addtionallDriverOne = new Driver();
            Driver addtionallDriverTwo = new Driver();

            if (quotationRequest.AdditionalDriverIdOne != null)
            {
                addtionallDriverOne = driverRepository.TableNoTracking.FirstOrDefault(a => a.DriverId == quotationRequest.AdditionalDriverIdOne);
            }
            if (quotationRequest.AdditionalDriverIdTwo != null)
            {
                addtionallDriverTwo = driverRepository.TableNoTracking.FirstOrDefault(a => a.DriverId == quotationRequest.AdditionalDriverIdTwo);
            }
        
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
            if (quotationRequiredFieldsModel.VehicleMajorColor == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleMajorColor))
            {
                quotationRequiredFieldsModel.VehicleMajorColor = null;
            }
            if (quotationRequiredFieldsModel.MainDriverSocialStatusCode == null || quotationRequiredFieldsModel.MainDriverSocialStatusCode < 1)
            {
                quotationRequiredFieldsModel.MainDriverSocialStatusCode = null;
                quotationRequiredFieldsModel.MainDriverNationalId = quotationRequest.Driver.NIN;
            }
            if ((quotationRequest.AdditionalDriverIdOne != null) && (addtionallDriverOne.SocialStatusId < 1 || addtionallDriverOne.SocialStatusId == null))
            {
                quotationRequiredFieldsModel.AdditionalDriverOneSocialStatusCode = null;
                quotationRequiredFieldsModel.AdditionalDriverOneNationalId = addtionallDriverOne.NIN;
            }
            if ((quotationRequest.AdditionalDriverIdTwo != null) && (addtionallDriverTwo.SocialStatusId < 1 || addtionallDriverTwo.SocialStatusId == null))
            {
                quotationRequiredFieldsModel.AdditionalDriverTwoSocialStatusCode = null;
                quotationRequiredFieldsModel.AdditionalDriverTwoNationalId = addtionallDriverTwo.NIN;
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

        private Vehicle GetVehicleInfo(InitInquiryRequestModel requestModel)
        {
            if (requestModel.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
            {
                return vehicleService.GetVehicleInfoBySequnceNumber(requestModel.SequenceNumber, requestModel.OwnerTransfer ? requestModel.OwnerNationalId : requestModel.NationalId);
            }
            else if (requestModel.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
            {
                return vehicleService.GetVehicleInfoByCustomCardNumber(requestModel.SequenceNumber);
            }
            return null;
        }
  private Driver GetDriverInfo(InitInquiryRequestModel requestModel)
        {
            return driverRepository.TableNoTracking.OrderByDescending(d => d.CreatedDateTime)
                .FirstOrDefault(d => d.NIN == requestModel.NationalId && !d.IsDeleted);
        }

        #region Private Methods
        private async Task<bool> ValidateCaptcha(string token, string input)
        {
            try
            {
                var requestModel = new ValidateCaptchaModel
                {
                    Token = token,
                    Input = input
                };

                var validateTask = httpClient.PostAsync($"{_config.Identity.Url}api/identity/captcha/validate/", requestModel, authorizationToken: AuthorizationToken);
                validateTask.Wait();
                var result = validateTask.Result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var response = JsonConvert.DeserializeObject<CommonResponseModel<bool>>(result);
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                // _logger.Log($"Inquiry controller -> ValidateCaptcha (token : {token}, input {input})", ex);
            }
            return false;
        }

        private bool ValidPolicyEffectiveDate(DateTime policyEffectiveDate)
        {
            if (policyEffectiveDate < DateTime.Now.Date.AddDays(1) || policyEffectiveDate > DateTime.Now.AddDays(29))
            {
                return false;
            }
            return true;
        }


        private InquiryRequestModel MapToInquiryRequestModel(InitInquiryRequestModel model)
        {
            try
            {
                InquiryRequestModel inquiryRequestModel = new InquiryRequestModel();
                if (model != null)
                {
                    inquiryRequestModel.Vehicle = new VehicleModel();
                    inquiryRequestModel.Insured = new InsuredModel();
                    inquiryRequestModel.Vehicle.SequenceNumber = model.SequenceNumber;
                    inquiryRequestModel.Vehicle.OwnerNationalId = model.OwnerNationalId;
                    inquiryRequestModel.Vehicle.OwnerTransfer = model.OwnerTransfer;
                    inquiryRequestModel.Insured.NationalId = model.NationalId;
                    inquiryRequestModel.PolicyEffectiveDate = model.PolicyEffectiveDate;
                    inquiryRequestModel.Vehicle.VehicleIdTypeId = model.VehicleIdTypeId;

                }
                return inquiryRequestModel;
            }
            catch (Exception ex)
            {
                // _logger.Log("Issue on Mapping" + ex.Message);
                throw;
            }
        }


        private CustomerYakeenInfoModel GetCustomerYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var customerInfoRequest = new CustomerYakeenInfoRequestModel()
                {
                    //Nin = long.Parse(requestModel.Insured.NationalId),
                    Nin = (requestModel.Insured.NationalId.StartsWith("7")) ?
                                    long.Parse(requestModel.Drivers.Where(a => a.NationalId != requestModel.Insured.NationalId).FirstOrDefault()?.NationalId) : long.Parse(requestModel.Insured.NationalId),
                    BirthMonth = requestModel.Insured.BirthDateMonth,
                    BirthYear = requestModel.Insured.BirthDateYear,
                    IsSpecialNeed = requestModel.IsCustomerSpecialNeed.HasValue
                               ? requestModel.IsCustomerSpecialNeed.Value : false

                };
                //var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                DriverModel mainDriver = (requestModel.Insured.NationalId.StartsWith("7"))
                                        ? mainDriver = requestModel.Drivers.Where(a => a.NationalId != requestModel.Insured.NationalId).ToList().FirstOrDefault()
                                        : requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                if (mainDriver != null)
                {
                    customerInfoRequest.MedicalConditionId = mainDriver.MedicalConditionId;
                    customerInfoRequest.EducationId = mainDriver.EducationId;
                    customerInfoRequest.ChildrenBelow16Years = mainDriver.ChildrenBelow16Years;
                    customerInfoRequest.DrivingPercentage = mainDriver.DrivingPercentage;
                    customerInfoRequest.ViolationIds = mainDriver.ViolationIds;
                    customerInfoRequest.NOALast5Years = mainDriver.DriverNOALast5Years;
                }
                customerInfoRequest.UserId = predefinedLogInfo.UserID;
                customerInfoRequest.UserName = predefinedLogInfo.UserName;
                customerInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;
                if (mainDriver.DriverExtraLicenses != null && mainDriver.DriverExtraLicenses.Any())
                {
                    customerInfoRequest.DriverExtraLicenses = mainDriver.DriverExtraLicenses.Select(e => new Integration.Dto.Yakeen.DriverExtraLicenseModel
                    {
                        LicenseYearsId = e.LicenseYearsId,
                        CountryId = e.CountryId
                    }).ToList();
                }

                CustomerBusiness _customerService = new CustomerBusiness(customerServices);
                var customerYakeenInfoModel = _customerService.CustomerYakeenInfoModel(customerInfoRequest);
                if (customerYakeenInfoModel.CustomerYakeenInfoModel == null)
                {
                    var CustomerYakeenInfoModel = new CustomerYakeenInfoModel();
                    CustomerYakeenInfoModel.Error = new YakeenInfoErrorModel();
                    CustomerYakeenInfoModel.Error.ErrorCode = customerYakeenInfoModel.StatusCode.ToString();
                    CustomerYakeenInfoModel.Error.ErrorMessage = customerYakeenInfoModel.Description;
                    CustomerYakeenInfoModel.Error.ErrorDescription = customerYakeenInfoModel.ErrorDescription;
                    return CustomerYakeenInfoModel;
                }
                return customerYakeenInfoModel.CustomerYakeenInfoModel;
            }
            catch (Exception ex)
            {
                var customerYakeenInfoModel = new CustomerYakeenInfoModel();
                customerYakeenInfoModel.Error = new YakeenInfoErrorModel();
                customerYakeenInfoModel.Error.ErrorCode = "5";
                customerYakeenInfoModel.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                customerYakeenInfoModel.Error.ErrorDescription = ex.ToString();
                return customerYakeenInfoModel;
            }
        }


        private Insured SaveInsured(CustomerYakeenInfoModel customerYakeenInfo, InquiryRequestModel requestModel,int addressId)
        {
            var insured = new Insured();
            if(string.IsNullOrEmpty(customerYakeenInfo.DateOfBirthH.ToString()))
            {
                string month = requestModel.Insured.BirthDateMonth.ToString();
                if (month.Length == 1)
                    month = "0" + month;
                insured.BirthDateH = "01-" + month + "-" + requestModel.Insured.BirthDateYear;
            }
            else
            {
                insured.BirthDateH = customerYakeenInfo.DateOfBirthH;
            }
            insured.BirthDate = customerYakeenInfo.DateOfBirthG;
            insured.NationalId = customerYakeenInfo.NIN;
            insured.CardIdTypeId = customerYakeenInfo.IsCitizen ? 1 : 2;

            int genderId = (int)customerYakeenInfo.Gender;
            if (genderId == 1 || genderId == 2)
            {
                insured.Gender = customerYakeenInfo.Gender;
            }
            else
            {
                insured.Gender = Gender.Male;
            }

            insured.NationalityCode = customerYakeenInfo.NationalityCode.GetValueOrDefault().ToString();
            insured.FirstNameAr = customerYakeenInfo.FirstName;
            insured.MiddleNameAr = customerYakeenInfo.SecondName;
            insured.LastNameAr = $"{customerYakeenInfo.ThirdName} {customerYakeenInfo.LastName}";
            insured.FirstNameEn = customerYakeenInfo.EnglishFirstName;
            insured.MiddleNameEn = customerYakeenInfo.EnglishSecondName;
            insured.LastNameEn = $"{customerYakeenInfo.EnglishThirdName} {customerYakeenInfo.EnglishLastName}";
            insured.CityId = requestModel.CityCode;
            if (requestModel.Insured.InsuredWorkCityCode.HasValue)
            {
                insured.WorkCityId = requestModel.Insured.InsuredWorkCityCode;
            }
            else
            {
                insured.WorkCityId = requestModel.CityCode;
            }
            insured.ChildrenBelow16Years = requestModel.Insured.ChildrenBelow16Years;
            insured.SocialStatusId = customerYakeenInfo.SocialStatusId.HasValue ? customerYakeenInfo.SocialStatusId.Value : 0;

            insured.OccupationId = customerYakeenInfo.OccupationId;
            if (customerYakeenInfo.OccupationId.HasValue)
            {
                var occupation = occupationService.GetOccupations().Where(x => x.ID == customerYakeenInfo.OccupationId).FirstOrDefault();
                if (occupation != null)
                {
                    insured.OccupationName = occupation?.NameAr;
                    insured.OccupationCode = occupation?.Code;
                }
            }

            insured.IdIssueCityId = addressService.GetCityByName(addressService.GetAllCities(), Utilities.Removemultiplespaces(customerYakeenInfo.IdIssuePlace))?.Code;
            insured.EducationId = requestModel.Insured.EducationId > 0 ? requestModel.Insured.EducationId : 1;
            insured.CreatedDateTime = DateTime.Now;
            insured.ModifiedDateTime = DateTime.Now;
            insured.UserSelectedCityId = requestModel.CityCode;
            insured.AddressId = addressId;
            if (requestModel.Insured.InsuredWorkCityCode.HasValue)
            {
                insured.UserSelectedWorkCityId = requestModel.Insured.InsuredWorkCityCode;
            }
            else
            {
                insured.UserSelectedWorkCityId = requestModel.CityCode;
            }

            foreach (var driver in requestModel.Drivers)
            {
                var driverExtraLicenses = driver.DriverExtraLicenses;
                if (driverExtraLicenses != null && driverExtraLicenses.Any())
                {
                    InsuredExtraLicenses insuredExtraLicenses;
                    foreach (var item in driverExtraLicenses)
                    {
                        if (item.CountryId == 0)
                            continue;
                        insuredExtraLicenses = new InsuredExtraLicenses();
                        insuredExtraLicenses.DriverNin = driver.NationalId;
                        insuredExtraLicenses.IsMainDriver = (driver.NationalId == requestModel.Insured.NationalId) ? true : false;
                        insuredExtraLicenses.LicenseCountryCode = item.CountryId;
                        insuredExtraLicenses.LicenseNumberYears = item.LicenseYearsId;
                        insuredExtraLicenses.CreatedDate = DateTime.Now;
                        insured.InsuredExtraLicenses.Add(insuredExtraLicenses);
                    }
                }
            }

            insuredRepository.Insert(insured);
            return insured;
        }

        private bool ValidateUserPromotionProgram(string userId, QuotationRequest quotationRequest)
        {
            if (string.IsNullOrWhiteSpace(userId) || quotationRequest == null)
                return true;

            if (quotationRequest.UserId != userId)
                return false;
            else
            {
                var progUser = promotionProgramUserRepository.Table.FirstOrDefault(e => e.UserId == userId);
                if (progUser != null && userId != quotationRequest.UserId)
                {
                    return false;
                }
                return true;
            }

        }

        private QuotationRequest GetQuotationRequestByDetails(InquiryRequestModel requestModel)
        {
            // Get only the valid request the are not expired within 16 hours window.
            var benchmarkDate = DateTime.Now.AddHours(-16);
            var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);

            var query = quotationRequestRepository.Table
                .Include(x => x.Drivers.Select(e => e.DriverViolations))
                .Include(x => x.Driver.DriverViolations)
                .Include(x => x.Vehicle)
                .Include(x => x.Insured)
                .Where(q =>
                            q.CreatedDateTime > benchmarkDate &&
                            q.RequestPolicyEffectiveDate.HasValue && q.RequestPolicyEffectiveDate.Value == requestModel.PolicyEffectiveDate &&
                            q.Vehicle != null &&
                            (
                                (q.Vehicle.CustomCardNumber == requestModel.Vehicle.VehicleId.ToString() && q.Vehicle.VehicleIdTypeId == (int)VehicleIdType.CustomCard) ||
                                (q.Vehicle.SequenceNumber == requestModel.Vehicle.VehicleId.ToString() && q.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
                            ) &&
                            q.Vehicle.VehicleValue.HasValue && q.Vehicle.VehicleValue == requestModel.Vehicle.ApproximateValue &&
                            q.Vehicle.TransmissionTypeId.HasValue && q.Vehicle.TransmissionTypeId.Value == requestModel.Vehicle.TransmissionTypeId &&
                            q.Vehicle.ParkingLocationId.HasValue && q.Vehicle.ParkingLocationId.Value == requestModel.Vehicle.ParkingLocationId &&
                            q.Vehicle.ModificationDetails == requestModel.Vehicle.Modification &&
                           (q.Insured != null && q.Insured.EducationId == requestModel.Insured.EducationId
                           && q.Insured.ChildrenBelow16Years.HasValue && q.Insured.ChildrenBelow16Years.Value == requestModel.Insured.ChildrenBelow16Years
                           && q.Insured.CityId.HasValue && q.Insured.CityId == requestModel.CityCode) &&
                            q.Driver != null && q.Driver.NIN == requestModel.Insured.NationalId
                            && q.Driver.MedicalConditionId.HasValue && q.Driver.MedicalConditionId.Value == mainDriver.MedicalConditionId);

            var quotationRequests = query.AsEnumerable();

            if (mainDriver != null && (mainDriver.ViolationIds != null && mainDriver.ViolationIds.Any()))
            {
                quotationRequests = quotationRequests.Where(e => e.Driver.DriverViolations.Select(v => v.ViolationId).All(mainDriver.ViolationIds.Contains)
                && e.Driver.DriverViolations.Count == mainDriver.ViolationIds.Count);
            }

            IEnumerable<string> additionalDriversNins = requestModel.Drivers?.Select(d => d.NationalId);
            if (additionalDriversNins == null || !additionalDriversNins.Any())
                return query.FirstOrDefault(q => !q.Drivers.Any());

            foreach (var quotationRequest in quotationRequests)
            {
                if (quotationRequest.Drivers != null && quotationRequest.Drivers.Any())
                {
                    var q = from dataNin in quotationRequest.Drivers.Select(d => d.NIN)
                            join nin in additionalDriversNins
                            on dataNin equals nin
                            select dataNin;
                    if (q.Count() == additionalDriversNins.Count() && q.Count() == quotationRequest.Drivers.Count)
                        return quotationRequest;
                }
            }

            return null;
        }

        public NajmResponse GetNajmResponse(NajmRequest request, ServiceRequestLog predefinedLogInfo, bool isODProduct = false, string tplExternalId = null)
        {
            try
            {
                //NajmResponseEntity entity;
                // Get only the valid request the are not expired within 29 days
                var VehicleId = request.VehicleId.ToString();
                var PolicyHolderNin = request.PolicyHolderNin.ToString();
                var IsVehicleRegistered = Convert.ToInt16(request.IsVehicleRegistered);

                // for od product --> to get tpl request driver (NCDFreeYears, NCDReference) data
                if (isODProduct && !string.IsNullOrEmpty(tplExternalId))
                {
                    var oldTplQuote = _quotationService.GetQuotaionDriversForODByExternailId(tplExternalId);
                    if (oldTplQuote != null && oldTplQuote.Driver != null && (oldTplQuote.Driver.NCDFreeYears.HasValue && !string.IsNullOrEmpty(oldTplQuote.Driver.NCDReference)))
                    {
                        NajmOutput najmResponse = new NajmOutput();
                        najmResponse.Output = new NajmResponse()
                        {
                            NCDReference = oldTplQuote.Driver.NCDReference,
                            NCDFreeYearsText = oldTplQuote.Driver.NCDFreeYears.Value.ToString(),
                            StatusCode = (int)NajmOutput.ErrorCodes.Success
                        };
                        return najmResponse.Output;
                    }
                }

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
                        benchmarkDate = DateTime.Now.AddMonths(-6);
                        entity = najmResponseRepository.Table.Where(x => x.VehicleId == VehicleId && x.PolicyHolderNin == PolicyHolderNin && x.IsVehicleRegistered == IsVehicleRegistered)
                                                       .OrderByDescending(a => a.CreatedAt).FirstOrDefault();
                        if (entity != null && entity.CreatedAt > benchmarkDate)
                        {
                            najmResponse = new NajmResponse();
                            najmResponse.StatusCode = (int)NajmOutput.ErrorCodes.Success;
                            najmResponse.NCDReference = entity.NCDReference;
                            najmResponse.NCDFreeYearsText = entity.NCDFreeYears.ToString();
                        }
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
            catch(Exception exp)
            {
                NajmResponse najmResponse = new NajmResponse();
                najmResponse.StatusCode =500;
                najmResponse.ErrorMsg = exp.ToString();
                return najmResponse;
            }
        }

        private VehicleYakeenModel GetVehicleYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
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
                vehicleInfoRequest.HasTrailer = requestModel.Vehicle.HasTrailer;
                int trailerSumInsured = 0;
                int.TryParse(requestModel.Vehicle.ApproximateTrailerSumInsured.ToString(), out trailerSumInsured);
                vehicleInfoRequest.TrailerSumInsured = trailerSumInsured;
                vehicleInfoRequest.OtherUses = requestModel.Vehicle.OtherUses;

                                           //vehicleInfoRequest.HasModification = requestModel.Vehicle.HasModification;
                                                                                                                                                                                                                                                              //vehicleInfoRequest.Modification = requestModel.Vehicle.Modification;
                    if (requestModel.Vehicle.HasModification && string.IsNullOrEmpty(requestModel.Vehicle.Modification))
                {
                    vehicleInfoRequest.HasModification = false;
                    vehicleInfoRequest.Modification = string.Empty;
                }
                else if (!requestModel.Vehicle.HasModification && !string.IsNullOrEmpty(requestModel.Vehicle.Modification))
                {
                    vehicleInfoRequest.HasModification = true;
                    vehicleInfoRequest.Modification = requestModel.Vehicle.Modification;
                }
                else
                {
                    vehicleInfoRequest.HasModification = requestModel.Vehicle.HasModification;
                    vehicleInfoRequest.Modification = requestModel.Vehicle.Modification;
                }

                vehicleInfoRequest.MileageExpectedAnnualId = requestModel.Vehicle.MileageExpectedAnnualId;

                vehicleInfoRequest.UserId = predefinedLogInfo.UserID;
                vehicleInfoRequest.UserName = predefinedLogInfo.UserName;
                vehicleInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;

                Vehicle vehicleData = null;
                if (requestModel.ODInsuranceTypeCode.HasValue && requestModel.ODInsuranceTypeCode.Value == 9 && !string.IsNullOrEmpty(requestModel.ODTPLExternalId))
                    vehicleData = vehicleService.GetVehicleInfoByExternalId(requestModel.ODTPLExternalId, vehicleInfoRequest.OwnerNin, out string exception);
                else
                    vehicleData = yakeenVehicleServices.GetVehicleEntity(vehicleInfoRequest.VehicleId, vehicleInfoRequest.VehicleIdTypeId, vehicleInfoRequest.IsOwnerTransfer, vehicleInfoRequest.OwnerNin.ToString());

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
                    vehicleInfo.HasTrailer = vehicleInfoRequest.HasTrailer;
                    vehicleInfo.TrailerSumInsured = vehicleInfoRequest.TrailerSumInsured;
                    vehicleInfo.OtherUses = vehicleInfoRequest.OtherUses;
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
                        if(vehicleData.ColorCode.HasValue)
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
                    vehicleInfo.ManualEntry = vehicleData.ManualEntry;
                    vehicleInfo.MissingFields = vehicleData.MissingFields;

                    if (requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.CustomCard || requestModel.Insured.NationalId.StartsWith("7"))
                    {
                        vehicleInfo.VehicleUseId = (requestModel.IsVehicleUsedCommercially) ? (int)VehicleUse.Commercial : (int)VehicleUse.Private;
                    }
                    else
                    {
                        var allVehicleUsages = vehicleService.GetVehicleUsage();
                        if (allVehicleUsages != null && allVehicleUsages.Count > 0)
                        {
                            var vehicleUsage = allVehicleUsages.Where(a => a.PlateTypeCode == vehicleData.PlateTypeCode).FirstOrDefault();
                            vehicleInfo.VehicleUseId = (vehicleUsage != null) ? vehicleUsage.VehicleUseCode.Value : (int)VehicleUse.Private;
                        }
                        else
                            vehicleInfo.VehicleUseId = (int)VehicleUse.Private;
                    }
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

        private CustomerYakeenInfoModel GetCustomerYakeenInfo(CustomerYakeenInfoRequestModel customerInfoRequest, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var customerYakeenRequest = new CustomerYakeenRequestDto()
                {
                    Nin = customerInfoRequest.Nin,
                    IsCitizen = customerInfoRequest.Nin.ToString().StartsWith("1"),
                    DateOfBirth = string.Format("{0}-{1}", customerInfoRequest.BirthMonth.ToString("00"), customerInfoRequest.BirthYear)
                };

                var customerIdInfo = _yakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedLogInfo);

                CustomerYakeenInfoModel customer = null;

                if (customerIdInfo.Success)
                {
                    Driver customerData = customerServices.InsertDriverInfoIntoDb(customerInfoRequest, customerIdInfo, customerIdInfo);
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
                    _yakeenDriversRepository.Insert(yakeenDrivers);
                    //end

                    customer = customerData.ToCustomerModel();
                    customer.Success = true;
                }
                else
                {
                    customer = new CustomerYakeenInfoModel();
                    customer.Success = false;
                    customer.Error = customerIdInfo.Error.ToModel();
                }

                return customer;
            }
            catch (Exception ex)
            {
                var customerYakeenInfoModel = new CustomerYakeenInfoModel();
                customerYakeenInfoModel.Error = new YakeenInfoErrorModel();
                customerYakeenInfoModel.Error.ErrorCode = "0";
                customerYakeenInfoModel.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                customerYakeenInfoModel.Error.ErrorDescription = ex.ToString();
                customerYakeenInfoModel.Success = false;
                return customerYakeenInfoModel;
            }
        }

        private string GetNewRequestExternalId()
        {
            string qtExtrnlId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (quotationRequestRepository.Table.Any(q => q.ExternalId == qtExtrnlId))
                return GetNewRequestExternalId();
            return qtExtrnlId;
        }

        private Driver GetDriverNajmResponse(Driver driver, long vehicleId, int vehicleIdTypeId)
        {

            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
            var najmResponse = najmService.GetNajm(new NajmRequest()
            {
                IsVehicleRegistered = vehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                PolicyHolderNin = long.Parse(driver.NIN.ToString()),
                VehicleId = vehicleId
            }, predefinedLogInfo);
            if (!string.IsNullOrWhiteSpace(najmResponse.ErrorMsg))
            {
                throw new NajmErrorException(najmResponse.ErrorMsg);
            }
            driver.NCDFreeYears = najmResponse.NCDFreeYears;
            driver.NCDReference = najmResponse.NCDReference;
            return driver;
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
                        var existInfo=result.Where(a => a.Key == yakeenField.Key).FirstOrDefault();
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
            if(propertyName== "VehicleMaker")
            {
                propertyName = "VehicleMakerCode";
            }
            if (propertyName == "VehicleModelCode")
            {
                propertyName = "VehicleModel";
            }
            string newLable = string.Empty;
            if (propertyName == "MainDriverSocialStatusCode" || propertyName == "AdditionalDriverOneSocialStatusCode" || propertyName == "AdditionalDriverTwoSocialStatusCode")
            {
                newLable = "MainDriverSocialStatusCodeDescription";
            }
            else
            {
                newLable = propertyName;
            }
            YakeenMissingFieldBase yakeenField = null;
            switch (fieldDetailAttribute.ControlType)
            {
                case ControlType.Dropdown:
                    var Nin = string.Empty;                    if (propertyName == "MainDriverSocialStatusCode")                    {                        Nin = model.MainDriverNationalId;                    }                    if (propertyName == "AdditionalDriverOneSocialStatusCode")                    {                        Nin = model.AdditionalDriverOneNationalId;                    }                    if (propertyName == "AdditionalDriverTwoSocialStatusCode")                    {                        Nin = model.AdditionalDriverTwoNationalId;                    }
                    yakeenField = new DropdownField
                    {
                        Key = propertyName,
                        Label = newLable,
                        Options = GetYakeenFieldDataSourceByName(fieldDetailAttribute.DataSourceName, model, lang),
                        Required = IsYakeenFieldRequired(propertyName),
                        NationalId = Nin
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

        private IEnumerable<DriverYakeenInfoModel> GetAdditionalDriversYakeenInfo(InquiryRequestModel requestModel, DriverModel mainDriver, InquiryRequestLog log)
        {

            var result = new List<DriverYakeenInfoModel>();
            try
            {

                foreach (var d in requestModel.Drivers)
                {
                    //var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                    if (d.NationalId == requestModel.Insured.NationalId)
                        continue;
                    if (requestModel.Insured.NationalId.StartsWith("7") &&
                        d.NationalId == mainDriver.NationalId) // for compaines  
                        continue;

                    var driverInfoRequest = new DriverYakeenInfoRequestModel()
                    {
                        Nin = long.Parse(d.NationalId),
                        BirthMonth = d.BirthDateMonth,
                        BirthYear = d.BirthDateYear,
                        MedicalConditionId = d.MedicalConditionId,
                        ViolationIds = d.ViolationIds,
                        DriverNOALast5Years = d.DriverNOALast5Years,
                        EducationId = d.EducationId,
                        DrivingPercentage=d.DrivingPercentage,
                        ChildrenBelow16Years=d.ChildrenBelow16Years
                    };

                    driverInfoRequest.DriverWorkCityCode = d.DriverWorkCityCode;
                    driverInfoRequest.DriverHomeCityCode = d.DriverHomeCityCode;

                    driverInfoRequest.CityName = d.DriverHomeCity;
                    driverInfoRequest.WorkCityName = d.DriverWorkCity;

                    if (d.DriverExtraLicenses != null && d.DriverExtraLicenses.Any())
                    {
                        driverInfoRequest.DriverExtraLicenses = d.DriverExtraLicenses
                                         .Select(e => new Integration.Dto.Yakeen.DriverExtraLicenseModel
                                         {
                                             CountryId = e.CountryId,
                                             LicenseYearsId = e.LicenseYearsId
                                         }).ToList();

                    }
                    Guid userId = Guid.Empty;
                    Guid.TryParse(log.UserId, out userId);
                    driverInfoRequest.UserId = userId;
                    driverInfoRequest.UserName = log.UserName;
                    // driverInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;

                    DriverBusiness _driverService = new DriverBusiness(driverServices, customerServices);
                    var driverYakeenInfoModel = _driverService.Post(driverInfoRequest,log.VehicleId,log.ExternalId);
                    result.Add(driverYakeenInfoModel.DriverYakeenInfoModel);
                }

                return result;
            }
            catch (Exception ex)
            {
                //_logger.Log("QuotationRequestService -> GetAdditionalDriversYakeenInfo : failed to get info from yakeen for object:" + JsonConvert.SerializeObject(requestModel), ex);
                //SetInquiryOutput((int)HttpStatusCode.BadRequest, ex.GetBaseException().Message, "GetAdditionalDriversYakeenInfo", requestModel.InquiryOutputModel);
                return null;
            }
        }

        private VehicleModel ConvertVehicleYakeenToVehicle(VehicleYakeenModel vehicleYakeenModel)
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
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<SocialStatus>().Select(e => e.ToModel()).Where(a=>a.Id!= 0).ToList();
                        break;
                    case "AdditionalDriverOneSocialStatus":
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<SocialStatus>().Select(e => e.ToModel()).Where(a => a.Id != 0).ToList();
                        break;
                    case "AdditionalDriverTwoSocialStatus":
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<SocialStatus>().Select(e => e.ToModel()).Where(a => a.Id != 0).ToList();
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
                                    Id =Convert.ToInt32(e.YakeenCode),
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
                    case "VehicleLoads":
                        var itemsLIst = new List<IdNamePairModel>();
                        for (int i = 1; i <= 15; i++)
                        {
                            itemsLIst.Add(new IdNamePairModel() { Id = i, Name = i.ToString() });
                        }
                        value = itemsLIst;
                        break;
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
                    case "VehicleColors":
                        value = vehicleService.GetVehicleColors().Select(e => new IdNamePairModel
                        {
                            Id = e.Code,
                            Name = lang == "ar" ? e.ArabicDescription : $"{e.EnglishDescription} - {e.ArabicDescription}"
                        }).ToList();
                        break;

                    default:
                        break;
                }
            }
            return value;
        }

        private void HandleNajmException(Exception ex, ref InquiryOutput inquiryOutput)
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

        public QuotationRequest GetQuotationRequest(string quotationExternalId)
        {
            return quotationRequestRepository.Table
                .Include(e => e.Vehicle).Include(e => e.Driver).Include(e => e.Insured)
                .FirstOrDefault(e => e.ExternalId == quotationExternalId);
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

        public SaudiPostOutput_Inquiry GetSaudiPostAddress(Guid driverId, string driverNin, string externalId)
        {
            SaudiPostOutput_Inquiry output = new SaudiPostOutput_Inquiry();
            try
            {
                if (string.IsNullOrEmpty(driverNin))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.DriverIsNull;
                    output.ErrorDescription = "Driver Nin Is Emty";
                    return output;
                }
                if (driverId == null)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.DriverIsNull;
                    output.ErrorDescription = "Driver Id Is Emty";
                    return output;
                }
                var data = GetAddressesFromSaudiPost(driverNin, externalId);
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode != SaudiPostOutput_Inquiry.ErrorCodes.Success)
                {
                    //String exeption = string.Empty;
                    //List<SaudiPostAddressDB> saudiPostAddressDBList = addressService.GetSaudiPostAddressDB(driverNin, out exeption);
                    //if (saudiPostAddressDBList != null && saudiPostAddressDBList.Any())
                    //{
                    //    data.Output = new SaudiPostApiResult_Inquiry();
                    //    data.Output.Addresses = new List<SaudiPostAddress_Inquiry>();
                    //    foreach (var saudiPostAddressDB in saudiPostAddressDBList)
                    //    {
                    //        SaudiPostAddress_Inquiry saudiPostAddress = new SaudiPostAddress_Inquiry()
                    //        {
                    //            Address1 = saudiPostAddressDB.Address1,
                    //            Address2 = saudiPostAddressDB.Address2,
                    //            AdditionalNumber = saudiPostAddressDB.AdditionalNumber,
                    //            BuildingNumber = saudiPostAddressDB.BuildingNumber,
                    //            CityId = saudiPostAddressDB.CityId,
                    //            City = saudiPostAddressDB.City,
                    //            District = saudiPostAddressDB.District,
                    //            IsPrimaryAddress = saudiPostAddressDB.IsPrimaryAddress,
                    //            Latitude = saudiPostAddressDB.Latitude,
                    //            Longitude = saudiPostAddressDB.Longitude,
                    //            ObjLatLng = saudiPostAddressDB.ObjLatLng,
                    //            PKAddressID = saudiPostAddressDB.PKAddressID,
                    //            PolygonString = saudiPostAddressDB.PolygonString,
                    //            PostCode = saudiPostAddressDB.PostCode,
                    //            RegionId = saudiPostAddressDB.RegionId,
                    //            RegionName = saudiPostAddressDB.RegionName,
                    //            Restriction = saudiPostAddressDB.Restriction,
                    //            Street = saudiPostAddressDB.Street,
                    //            Title = saudiPostAddressDB.Title,
                    //            UnitNumber = saudiPostAddressDB.UnitNumber
                    //        };
                    //        data.Output.Addresses.Add(saudiPostAddress);
                    //    }
                    //    data.Output.success = true;
                    //    data.Output.totalSearchResults = "1";
                    //    data.Output.statusdescription = "success";
                    //}
                    //else
                    //{
                    output.ErrorCode = (SaudiPostOutput_Inquiry.ErrorCodes)(int)data.ErrorCode;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                    //}
                }
                SaudiPostApiResult_Inquiry response = data.Output;
                List<Address> addresses = new List<Address>();
                foreach (var saudiPostAddress in response.Addresses)
                {
                    var address = new Address
                    {
                        Address1 = saudiPostAddress.Address1,
                        Address2 = saudiPostAddress.Address2,
                        AdditionalNumber = saudiPostAddress.AdditionalNumber,
                        BuildingNumber = saudiPostAddress.BuildingNumber,
                        CityId = saudiPostAddress.CityId,
                        City = saudiPostAddress.City,
                        District = saudiPostAddress.District,
                        DriverId = driverId,
                        IsPrimaryAddress = saudiPostAddress.IsPrimaryAddress,
                        Latitude = saudiPostAddress.Latitude,
                        Longitude = saudiPostAddress.Longitude,
                        ObjLatLng = saudiPostAddress.ObjLatLng,
                        PKAddressID = saudiPostAddress.PKAddressID,
                        PolygonString = saudiPostAddress.PolygonString?.ToString(),
                        PostCode = saudiPostAddress.PostCode,
                        RegionId = saudiPostAddress.RegionId,
                        RegionName = saudiPostAddress.RegionName,
                        Restriction = saudiPostAddress.Restriction,
                        Street = saudiPostAddress.Street,
                        Title = saudiPostAddress.Title?.ToString(),
                        UnitNumber = saudiPostAddress.UnitNumber,
                        NationalId=driverNin,
                        CreatedDate=DateTime.Now
                    };
                    addresses.Add(address);
                }
                addressService.InsertAddresses(addresses);
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public SaudiPostOutput_Inquiry GetAddressesFromSaudiPost(string id,string externalId)
        {
            SaudiPostOutput_Inquiry output = new SaudiPostOutput_Inquiry();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "saudiPost";
            log.ServiceURL = _config.SaudiPost.Url;
            log.DriverNin = id;
            log.ExternalId = externalId;
            try
            {
                string apiUrl = $"{_config.SaudiPost.Url}AddressByID/national-id?language=A&format=JSON&iqama={id}&api_key={_config.SaudiPost.ApiKey}";
                log.ServiceRequest = apiUrl;
                DateTime dtBeforeCalling = DateTime.Now;
                var task = httpClient.GetStringAsync(apiUrl);
                task.Wait();
                var response = task.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                SaudiPostApiResult_Inquiry saudiPostApiResult = null;

                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response;
                saudiPostApiResult = JsonConvert.DeserializeObject<SaudiPostApiResult_Inquiry>(response);
                if (saudiPostApiResult == null)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = "saudiPostApiResult is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.totalSearchResults == "0")
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = "Total Search Results is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.statusdescription.ToLower().Contains("invalid public id"))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = "statusdescription is " + saudiPostApiResult.statusdescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.statusdescription.ToLower() != "SUCCESS".ToLower())
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.StatusdescriptionIsNotSuccess;
                    output.ErrorDescription = "statusdescription is " + saudiPostApiResult.statusdescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!saudiPostApiResult.success)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.StatusdescriptionIsNotSuccess;
                    output.ErrorDescription = "saudiPostApiResult.success is false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.Addresses.Count == 0)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = "saudiPostApiResult.Addresses.Count is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = saudiPostApiResult;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

        }

        //private DriverCityInfoOutput GetDriverCityInfo(Guid driverId, string driverNin)
        //{
        //    DriverCityInfoOutput output = new DriverCityInfoOutput();
        //    DriverCityInfoModel driverCityInfoModel = new DriverCityInfoModel();
        //    var mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
        //    if (mainDriverAddress == null || !mainDriverAddress.Any())
        //    {
        //        var saudiPostOutput = GetSaudiPostAddress(driverId, driverNin);
        //        if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero)
        //        {
        //            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.SaudiPostNoResultReturned;
        //            output.ErrorDescription = saudiPostOutput.ErrorDescription;
        //            return output;
        //        }

        //        if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.NullResponse)
        //        {
        //            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = saudiPostOutput.ErrorDescription;
        //            return output;
        //        }
        //        if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID)
        //        {
        //            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.InvalidPublicID;
        //            output.ErrorDescription = saudiPostOutput.ErrorDescription;
        //            return output;
        //        }
        //        if (saudiPostOutput.ErrorCode != SaudiPostOutput_Inquiry.ErrorCodes.Success)
        //        {
        //            output.ErrorCode = DriverCityInfoOutput.ErrorCodes.SaudiPostResultFailed;
        //            output.ErrorDescription = saudiPostOutput.ErrorDescription;
        //            return output;
        //        }

        //        mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
        //    }
        //    if (mainDriverAddress == null || !mainDriverAddress.Any())
        //    {
        //        output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NoAddressesFoundInSaudiPostOrInDB;
        //        output.ErrorDescription = "No Addresses Found In SaudiPost Or In DB";
        //        return output;
        //    }
        //    int addressId = 0;
        //    CityCenter nationalAddressCity = null;
        //    foreach (Address address in mainDriverAddress)
        //    {
        //        nationalAddressCity = addressService.GetCityCenters().Where(a => a.CityId == address.CityId).FirstOrDefault();
        //        if (nationalAddressCity != null)
        //        {
        //            addressId = address.Id;
        //            break;
        //        }
        //    }
        //    //var nationalAddressCity = addressService.GetCityCenters().Where(a => a.CityId == mainDriverAddress.FirstOrDefault().CityId).FirstOrDefault();
        //    if (nationalAddressCity == null)
        //    {
        //        output.ErrorCode = DriverCityInfoOutput.ErrorCodes.NationalAddressCityIsNull;
        //        output.ErrorDescription = "National Address City Is Null for the city id : " + mainDriverAddress.FirstOrDefault().CityId;
        //        return output;
        //    }
        //    long cityId = 0;
        //    if(!long.TryParse(nationalAddressCity.ELM_Code, out cityId))
        //    {
        //        output.ErrorCode = DriverCityInfoOutput.ErrorCodes.ParseElmCodeError;
        //        output.ErrorDescription = "Failed to parse ELM_Code as we received  : " + nationalAddressCity.ELM_Code;
        //        return output;
        //    }
        //    output.ErrorCode = DriverCityInfoOutput.ErrorCodes.Success;
        //    output.ErrorDescription = "Success";
        //    driverCityInfoModel.ElmCode = cityId;
        //    driverCityInfoModel.AddressId = addressId;
        //    output.Output = driverCityInfoModel;
        //    return output;
        //}



        /////////////////////Yakeeeeen//////////////////////////
        private DriverCityInfoOutput GetDriverCityInfo(Guid driverId, string driverNin, string vechileId, string channel, string birthDate, bool fromYakeen, string externalId, bool isODProduct = false, string tplExternalId = null)
        {
            DriverCityInfoOutput output = new DriverCityInfoOutput();
            DriverCityInfoModel driverCityInfoModel = new DriverCityInfoModel();
            try
            {
                // for od product --> to get tpl request driver (cityId, addressId, postCode) data
                if (isODProduct && !string.IsNullOrEmpty(tplExternalId))
                {
                    var oldTplQuote = _quotationService.GetQuotaionDriversForODByExternailId(tplExternalId);
                    if (oldTplQuote != null && oldTplQuote.Driver != null && (oldTplQuote.Driver.AddressId.HasValue && oldTplQuote.Driver.CityId.HasValue && !string.IsNullOrEmpty(oldTplQuote.Driver.PostCode)))
                    {
                        var doNationalAddressCity = addressService.GetCityCenters().Where(a => a.CityId == oldTplQuote.Driver.CityId.Value.ToString()).FirstOrDefault();
                        output.ErrorCode = DriverCityInfoOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        driverCityInfoModel.ElmCode = oldTplQuote.Driver.CityId.Value;
                        driverCityInfoModel.AddressId = oldTplQuote.Driver.AddressId.Value;
                        driverCityInfoModel.PostCode = oldTplQuote.Driver.PostCode;
                        driverCityInfoModel.CityNameAr = doNationalAddressCity.ArabicName;
                        driverCityInfoModel.CityNameEn = doNationalAddressCity.EnglishName;
                        output.Output = driverCityInfoModel;
                        return output;
                    }
                }

                var mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
                bool getAddressFromYakeen = false;
                var benchmarkDate = DateTime.Now.AddYears(-1);
                if (mainDriverAddress!=null)
                {
                    var addressesWithin30Days = mainDriverAddress.Where(a => a.CreatedDate >benchmarkDate).ToList();
                    if(addressesWithin30Days == null || !addressesWithin30Days.Any())
                    {
                        getAddressFromYakeen=true;
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
                    else if(mainDriverAddress!=null && yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.Success)
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


        public YakeenAddressOutput GetYakeenAddress(Guid driverId, string driverNin, string vechileId, string channel, string birthDate,string externalId)
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
                output = _yakeenClient.GetYakeenAddress("0", driverNin, birthDate, "A", isCitizen, channel, vechileId,externalId);
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
                    zipcodes +=addressInfo.PostCode+ ",";
                    var yakeenCityCenterList = addressService.GetYakeenCityCenterByPostCode(addressInfo.PostCode);
                    if (yakeenCityCenterList == null|| yakeenCityCenterList.Count()==0)
                        continue;

                    var yakeenCityCenter = yakeenCityCenterList.Where(a => a.CityName == addressInfo.City|| a.EnglishName== addressInfo.City).FirstOrDefault();
                    if (yakeenCityCenter == null)
                    {
                        yakeenCityCenter = yakeenCityCenterList.FirstOrDefault();
                    }
                    Address address = new Address();
                    //address.Address1 = saudiPostAddress.Address1;
                    //address.Address2 = saudiPostAddress.Address2;
                    address.AdditionalNumber = addressInfo.AdditionalNumber.ToString();
                    address.BuildingNumber = addressInfo.BuildingNumber.ToString();
                    address.CityId = yakeenCityCenter.CityID.ToString();
                    address.City = addressInfo.City;
                    address.District = addressInfo.District;
                    address.DriverId = driverId;
                    address.IsPrimaryAddress = addressInfo.IsPrimaryAddress.ToString();
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
                    address.ModifiedDate = DateTime.Now;
                    address.IsDeleted = false;
                    addresses.Add(address);
                }
                if(output.Addresses.Count()>0&& addresses.Count==0)
                {
                    output.ErrorCode = YakeenAddressOutput.ErrorCodes.NoLookupForZipCode;
                    output.ErrorDescription = "no lookup found for these zipcodes "+ zipcodes;
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

        public NationalAddressOutput GetNationalAddress(string driverNin, string birthDate, string channel, bool fromYakeen)
        {
            NationalAddressOutput output = new NationalAddressOutput();
            var mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
            if (mainDriverAddress == null || !mainDriverAddress.Any())
            {
                if (!fromYakeen)
                {
                    var saudiPostOutput = GetSaudiPostNationalAddress(driverNin, channel);
                    if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.SaudiPostNoResultReturned;
                        output.ErrorDescription = saudiPostOutput.ErrorDescription;
                        return output;
                    }

                    if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.NullResponse)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = saudiPostOutput.ErrorDescription;
                        return output;
                    }
                    if (saudiPostOutput.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.InvalidPublicID;
                        output.ErrorDescription = saudiPostOutput.ErrorDescription;
                        return output;
                    }
                    if (saudiPostOutput.ErrorCode != SaudiPostOutput_Inquiry.ErrorCodes.Success)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.SaudiPostResultFailed;
                        output.ErrorDescription = saudiPostOutput.ErrorDescription;
                        return output;
                    }
                }
                if (fromYakeen)//yakeeeen
                {
                    var yakeenAddressOutput = GetYakeenNationalAddress(driverNin, channel, birthDate);
                    if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.NoAddressFound;
                        output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                        return output;
                    }
                    if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.NullResponse;
                        output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                        return output;
                    }
                    if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoLookupForZipCode)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.NoLookupForZipCode;
                        output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                        return output;
                    }
                    if (yakeenAddressOutput.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = NationalAddressOutput.ErrorCodes.YakeenResultFailed;
                        output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                        return output;
                    }
                }
                mainDriverAddress = addressService.GetAllAddressesByNin(driverNin);
            }
            if (mainDriverAddress == null || !mainDriverAddress.Any())
            {
                output.ErrorCode = NationalAddressOutput.ErrorCodes.NoAddressesFoundInSaudiPostOrInDB;
                output.ErrorDescription = "No Addresses Found In SaudiPost Or In DB";
                return output;
            }

            try
            {
                addressService.InsertAddresses(mainDriverAddress);

                output.ErrorCode = NationalAddressOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Addresses = mainDriverAddress;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = NationalAddressOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public YakeenAddressOutput GetYakeenNationalAddress(string driverNin, string birthDate, string channel)
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
                bool isCitizen = driverNin.StartsWith("1");
                output = _yakeenClient.GetYakeenAddress("0", driverNin, birthDate, "A", isCitizen, channel, "0", "0");
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
                    //address.DriverId = driverId;
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
                addressService.InsertAddresses(addresses);
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

        public SaudiPostOutput_Inquiry GetSaudiPostNationalAddress(string driverNin, string channel)
        {
            SaudiPostOutput_Inquiry output = new SaudiPostOutput_Inquiry();
            try
            {
                if (string.IsNullOrEmpty(driverNin))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.DriverIsNull;
                    output.ErrorDescription = "Driver Nin Is Emty";
                    return output;
                }
                var data = GetNationalAddressesFromSaudiPost(driverNin, channel);
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode == SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                if (data.ErrorCode != SaudiPostOutput_Inquiry.ErrorCodes.Success)
                {
                    output.ErrorCode = (SaudiPostOutput_Inquiry.ErrorCodes)(int)data.ErrorCode;
                    output.ErrorDescription = data.ErrorDescription;
                    return output;
                }
                SaudiPostApiResult_Inquiry response = data.Output;
                List<Address> addresses = new List<Address>();
                foreach (var saudiPostAddress in response.Addresses)
                {
                    var address = new Address
                    {
                        Address1 = saudiPostAddress.Address1,
                        Address2 = saudiPostAddress.Address2,
                        AdditionalNumber = saudiPostAddress.AdditionalNumber,
                        BuildingNumber = saudiPostAddress.BuildingNumber,
                        CityId = saudiPostAddress.CityId,
                        City = saudiPostAddress.City,
                        District = saudiPostAddress.District,
                        //DriverId = driverId,
                        IsPrimaryAddress = saudiPostAddress.IsPrimaryAddress,
                        Latitude = saudiPostAddress.Latitude,
                        Longitude = saudiPostAddress.Longitude,
                        ObjLatLng = saudiPostAddress.ObjLatLng,
                        PKAddressID = saudiPostAddress.PKAddressID,
                        PolygonString = saudiPostAddress.PolygonString?.ToString(),
                        PostCode = saudiPostAddress.PostCode,
                        RegionId = saudiPostAddress.RegionId,
                        RegionName = saudiPostAddress.RegionName,
                        Restriction = saudiPostAddress.Restriction,
                        Street = saudiPostAddress.Street,
                        Title = saudiPostAddress.Title?.ToString(),
                        UnitNumber = saudiPostAddress.UnitNumber,
                        NationalId = driverNin,
                        CreatedDate = DateTime.Now
                    };
                    addresses.Add(address);
                }
                addressService.InsertAddresses(addresses);
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public SaudiPostOutput_Inquiry GetNationalAddressesFromSaudiPost(string id, string channel)
        {
            SaudiPostOutput_Inquiry output = new SaudiPostOutput_Inquiry();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "saudiPost";
            log.ServiceURL = _config.SaudiPost.Url;
            log.DriverNin = id;
            try
            {
                string apiUrl = $"{_config.SaudiPost.Url}AddressByID/national-id?language=A&format=JSON&iqama={id}&api_key={_config.SaudiPost.ApiKey}";
                log.ServiceRequest = apiUrl;
                DateTime dtBeforeCalling = DateTime.Now;
                var task = httpClient.GetStringAsync(apiUrl);
                task.Wait();
                var response = task.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                SaudiPostApiResult_Inquiry saudiPostApiResult = null;

                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response;
                saudiPostApiResult = JsonConvert.DeserializeObject<SaudiPostApiResult_Inquiry>(response);
                if (saudiPostApiResult == null)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.NullResponse;
                    output.ErrorDescription = "saudiPostApiResult is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.totalSearchResults == "0")
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = "Total Search Results is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.statusdescription.ToLower().Contains("invalid public id"))
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.InvalidPublicID;
                    output.ErrorDescription = "statusdescription is " + saudiPostApiResult.statusdescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.statusdescription.ToLower() != "SUCCESS".ToLower())
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.StatusdescriptionIsNotSuccess;
                    output.ErrorDescription = "statusdescription is " + saudiPostApiResult.statusdescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!saudiPostApiResult.success)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.StatusdescriptionIsNotSuccess;
                    output.ErrorDescription = "saudiPostApiResult.success is false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.Addresses.Count == 0)
                {
                    output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.TotalSearchResultsIsZero;
                    output.ErrorDescription = "saudiPostApiResult.Addresses.Count is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = saudiPostApiResult;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SaudiPostOutput_Inquiry.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

        }

        private DriverOutput GetDriverYakeenInfo(CustomerYakeenInfoRequestModel customerInfoRequest, ServiceRequestLog predefinedLogInfo)
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
                    yakeenDrivers.LicenseList= JsonConvert.SerializeObject(customerIdInfo.licenseListListField);
                    _yakeenDriversRepository.Insert(yakeenDrivers);
                    //end

                    output.Driver = customerData;
                    output.ErrorCode = DriverOutput.ErrorCodes.Success;
                    return output;
                }
                else
                {
                    /// as per @(khaled & Mubarak)
                    /// to get data from cahce back to (1 year (delete or not deleted)) if yakeen return error @12-12-2023
                    DateTime dateFrom = DateTime.Now.AddYears(-1);
                    var driverData = driverRepository.TableNoTracking.Where(d => d.NIN == customerInfoRequest.Nin.ToString()).OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
                    if (driverData != null && driverData.CreatedDateTime.HasValue && driverData.CreatedDateTime > dateFrom)
                    {
                        driverData.DriverId = Guid.NewGuid();
                        driverData.CreatedDateTime = DateTime.Now;
                        driverData.IsDeleted = false;
                        output.Driver = driverData;
                        output.ErrorCode = DriverOutput.ErrorCodes.Success;
                        return output;
                    }
                    
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
        public InquiryOutput SubmitMissingFeilds(YakeenMissingInfoRequestModel model,string userId,string userName)
        {
            InquiryResponseModel result = new InquiryResponseModel
            {
                QuotationRequestExternalId = model.QuotationRequestExternalId
            };
            InquiryOutput output = new InquiryOutput();
            InquiryRequestLog log = new InquiryRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            if (log.Headers["User-Agent"].ToString().Contains("Tameenak/1"))
            {
                log.Channel = "ios";
            }
            else if (log.Headers["User-Agent"].ToString().Contains("okhttp/"))
            {
                log.Channel = "android";
            }
            else
            {
                log.Channel = "Portal";
            }
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.MethodName = "SubmitMissingFields";
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            // log.MobileVersion = requestModel.MobileVersion;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    result.IsValidInquiryRequest = false;
                    output.InquiryResponseModel = result;
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    return output;
                }
                log.ExternalId = model.QuotationRequestExternalId;
                if (model.YakeenMissingFields.VehicleMakerCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleMaker) && model.YakeenMissingFields.VehicleMaker != "غير متوفر")))
                {
                    model.YakeenMissingFields.VehicleMakerCode = int.Parse(model.YakeenMissingFields.VehicleMaker);
                }
                if (model.YakeenMissingFields.VehicleModelCode == null && ((!string.IsNullOrEmpty(model.YakeenMissingFields.VehicleModel) && model.YakeenMissingFields.VehicleModel != "غير متوفر")))
                {
                    model.YakeenMissingFields.VehicleModelCode = int.Parse(model.YakeenMissingFields.VehicleModel);
                }
                if (model.YakeenMissingFields.VehicleMakerCode.HasValue && model.YakeenMissingFields.VehicleMakerCode.Value > 0)
                {
                    model.YakeenMissingFields.VehicleMaker = model.YakeenMissingFields.VehicleMakerCode.Value.ToString();
                }
                var quotationRequest = GetQuotationRequest(model.QuotationRequestExternalId);
                Driver addtionallDriverOne = new Driver();
                Driver addtionallDriverTwo = new Driver();
                if (quotationRequest.AdditionalDriverIdOne != null)
                {
                    addtionallDriverOne = driverRepository.TableNoTracking.FirstOrDefault(a => a.DriverId == quotationRequest.AdditionalDriverIdOne);
                }
                if (quotationRequest.AdditionalDriverIdTwo != null)
                {
                    addtionallDriverTwo = driverRepository.TableNoTracking.FirstOrDefault(a => a.DriverId == quotationRequest.AdditionalDriverIdTwo);
                }


                if (quotationRequest == null)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result.IsValidInquiryRequest = false;
                    output.InquiryResponseModel = result;
                    return output;
                }
                log.NIN = quotationRequest.Driver.NIN;
                bool isCustomCard = false;
                if (quotationRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
                {
                    isCustomCard = true;
                    log.VehicleId = quotationRequest.Vehicle.CustomCardNumber;

                }
                else
                {
                    log.VehicleId = quotationRequest.Vehicle.SequenceNumber;
                }

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
                if (quotationRequiredFieldsModel.VehicleBodyCode == null || quotationRequiredFieldsModel.VehicleBodyCode.Value < 1 || quotationRequiredFieldsModel.VehicleBodyCode.Value > 21)
                {
                    quotationRequiredFieldsModel.VehicleBodyCode = null;
                }
                if (quotationRequiredFieldsModel.VehicleLoad == null || quotationRequiredFieldsModel.VehicleLoad.Value < 1)
                {
                    quotationRequiredFieldsModel.VehicleLoad = null;
                }
                if (quotationRequiredFieldsModel.VehicleMajorColor == "غير متوفر" || string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleMajorColor))
                {
                    quotationRequiredFieldsModel.VehicleMajorColor = null;
                }
                if (quotationRequiredFieldsModel.MainDriverSocialStatusCode == null || quotationRequiredFieldsModel.MainDriverSocialStatusCode < 1)
                {
                    quotationRequiredFieldsModel.MainDriverSocialStatusCode = null;
                }
                //if (quotationRequest.AdditionalDriverIdOne != null && (quotationRequiredFieldsModel.AdditionalDriverOneSocialStatusCode == null || quotationRequiredFieldsModel.AdditionalDriverOneSocialStatusCode < 1))
                //{
                //    quotationRequiredFieldsModel.AdditionalDriverOneSocialStatusCode = null;
                //}
                //if (quotationRequest.AdditionalDriverIdTwo != null && (quotationRequiredFieldsModel.AdditionalDriverTwoSocialStatusCode == null || quotationRequiredFieldsModel.AdditionalDriverTwoSocialStatusCode < 1))
                //{
                //    quotationRequiredFieldsModel.AdditionalDriverTwoSocialStatusCode = null;
                //}

                if ((quotationRequest.AdditionalDriverIdOne != null) && (addtionallDriverOne.SocialStatusId < 1 || addtionallDriverOne.SocialStatusId == null))
                {
                    quotationRequiredFieldsModel.AdditionalDriverOneSocialStatusCode = null;
                }
                if ((quotationRequest.AdditionalDriverIdTwo != null) && (addtionallDriverTwo.SocialStatusId < 1 || addtionallDriverTwo.SocialStatusId == null))
                {
                    quotationRequiredFieldsModel.AdditionalDriverTwoSocialStatusCode = null;
                }
                var missingPropertiesNames = GetYakeenMissingPropertiesName(quotationRequiredFieldsModel,isCustomCard);
                if (missingPropertiesNames == null || missingPropertiesNames.Count() == 0)
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result.IsValidInquiryRequest = true;
                    output.InquiryResponseModel = result;
                    return output;
                }
                if (!IsUserEnteredAllYakeenMissingFields(model.YakeenMissingFields, missingPropertiesNames))
                {
                    output.ErrorCode = InquiryOutput.ErrorCodes.StillMissed;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User didn't entered all required data";
                    InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                    result = CheckYakeenMissingFields(result, quotationRequiredFieldsModel, isCustomCard); ;
                    output.InquiryResponseModel = result;
                    return output;
                }
                var userData = model.YakeenMissingFields;
                List<YakeenMissingField> missingFields = new List<YakeenMissingField>();
                string vehicleModel = string.Empty;
                string vehicleMaker = string.Empty;
                int socialStatus = 0;
                int AdditionalDriverOneSocialStatus = 0;
                int AdditionalDriverTwoSocialStatus = 0;
                foreach (var propertyName in missingPropertiesNames)
                {
                    YakeenMissingField field = new YakeenMissingField();
                    var userValue = userData.GetType().GetProperty(propertyName).GetValue(userData);
                    string value = userValue.ToString();
                    if (propertyName == "VehicleMaker")
                    {
                        int.TryParse(userValue.ToString(), out int makerCode);
                        var makerName = vehicleService.GetMakerName(makerCode, "");
                        value = makerName;
                        vehicleMaker = makerName;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, makerName);
                    }
                    else if(propertyName == "VehicleModel")
                    {
                        int makerCode = 0;
                        if(!quotationRequiredFieldsModel.VehicleMakerCode.HasValue|| quotationRequiredFieldsModel.VehicleMakerCode.Value<1)
                        {
                            var maker = userData.GetType().GetProperty("VehicleMaker").GetValue(userData);
                            int.TryParse(maker.ToString(), out makerCode);
                        }
                        else
                        {
                            makerCode = quotationRequiredFieldsModel.VehicleMakerCode.Value;
                        }
                        //userValue = userData.GetType().GetProperty(propertyName).GetValue(userData);
                        int modelCode = 0;
                        int.TryParse(userValue as string, out modelCode);
                        var modelName = vehicleService.GetModelName(modelCode, short.Parse(makerCode.ToString()), "");
                        value = modelName;
                        vehicleModel = modelName;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, modelName);
                    }
                    else if (propertyName == "VehicleMajorColor")
                    {
                        int.TryParse(userValue.ToString(), out int majorColorCode);
                        var color = vehicleService.GetVehicleColorBycode(majorColorCode, 99);
                        value = color.ArabicDescription;

                        quotationRequest.Vehicle.ColorCode = color.Code;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, color.ArabicDescription);
                    }
                    else if (propertyName == "MainDriverSocialStatusCode")
                    {
                        int.TryParse(userValue.ToString(), out socialStatus);
                        value = socialStatus.ToString();
                        string socialStatusName = Enum.GetName(typeof(SocialStatus), socialStatus);
                        quotationRequest.Driver.SocialStatusId = socialStatus;
                        quotationRequest.Driver.SocialStatusName = socialStatusName;
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, socialStatus);
                    }
                    if (propertyName == "AdditionalDriverOneSocialStatusCode")                    {                        int.TryParse(userValue.ToString(), out AdditionalDriverOneSocialStatus);                        value = AdditionalDriverOneSocialStatus.ToString();                        string socialStatusName = Enum.GetName(typeof(SocialStatus), AdditionalDriverOneSocialStatus);                        quotationRequest.Driver.SocialStatusId = AdditionalDriverOneSocialStatus;                        quotationRequest.Driver.SocialStatusName = socialStatusName;                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, AdditionalDriverOneSocialStatus);                    }                    if (propertyName == "AdditionalDriverTwoSocialStatusCode")                    {                        int.TryParse(userValue.ToString(), out AdditionalDriverTwoSocialStatus);                        value = AdditionalDriverTwoSocialStatus.ToString();                        string socialStatusName = Enum.GetName(typeof(SocialStatus), AdditionalDriverTwoSocialStatus);                        quotationRequest.Driver.SocialStatusId = AdditionalDriverTwoSocialStatus;                        quotationRequest.Driver.SocialStatusName = socialStatusName;                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, AdditionalDriverTwoSocialStatus);                    }
                    else
                    {
                        quotationRequiredFieldsModel.GetType().GetProperty(propertyName).SetValue(quotationRequiredFieldsModel, userValue);
                    }
                    field.Key = propertyName;
                    field.Value = value;
                    missingFields.Add(field);
                }
                var updatedQuotationRequest = quotationRequiredFieldsModel.ToEntity(quotationRequest);
                if(updatedQuotationRequest.Vehicle!=null)
                {
                    if (!string.IsNullOrEmpty(vehicleModel))
                        updatedQuotationRequest.Vehicle.VehicleModel = vehicleModel;

                    if (!string.IsNullOrEmpty(vehicleMaker))
                        updatedQuotationRequest.Vehicle.VehicleMaker = vehicleMaker;
                }
                if (quotationRequest.Insured != null && socialStatus > 0)
                    quotationRequest.Insured.SocialStatusId = socialStatus;

                updatedQuotationRequest.ManualEntry = true;
                updatedQuotationRequest.Vehicle.ManualEntry = true;
                if (missingFields.Count > 0)
                {
                    updatedQuotationRequest.Vehicle.MissingFields = JsonConvert.SerializeObject(missingFields);
                    updatedQuotationRequest.MissingFields = JsonConvert.SerializeObject(missingFields);
                }
                quotationRequestRepository.Update(updatedQuotationRequest);
                // Handel Addtional Drivers social Status
                if (quotationRequest.AdditionalDriverIdOne != null && model.YakeenMissingFields.AdditionalDriverOneSocialStatusCode > 0)                {
                    var AddtionaldriverOne = driverRepository.Table.Where(a => a.DriverId == quotationRequest.AdditionalDriverIdOne).FirstOrDefault();                    AddtionaldriverOne.SocialStatusId = model.YakeenMissingFields.AdditionalDriverOneSocialStatusCode;                    driverRepository.Update(AddtionaldriverOne);                }                if (quotationRequest.AdditionalDriverIdTwo != null && model.YakeenMissingFields.AdditionalDriverOneSocialStatusCode > 0)                {                    var AddtionaldriverTwo = driverRepository.Table.Where(a => a.DriverId == quotationRequest.AdditionalDriverIdTwo).FirstOrDefault();                    AddtionaldriverTwo.SocialStatusId = model.YakeenMissingFields.AdditionalDriverTwoSocialStatusCode;                    driverRepository.Update(AddtionaldriverTwo);                }
                result.IsValidInquiryRequest = true;
                output.ErrorCode = InquiryOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                result.IsValidInquiryRequest = true;
                output.InquiryResponseModel = result;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = InquiryOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                InquiryRequestLogDataAccess.AddInquiryRequestLog(log);
                return output;
            }
        }

        public InquiryResponseModel CheckYakeenMissingFields(InquiryResponseModel result, QuotationRequestRequiredFieldsModel quotationRequiredFieldsModel,bool isCustomCard)
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

        private IEnumerable<string> GetYakeenMissingPropertiesName(QuotationRequestRequiredFieldsModel quotationRequestRequiredFieldsModel,bool isCustomCard)
        {
            ValidationContext vc = new ValidationContext(quotationRequestRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            bool isValid = Validator.TryValidateObject(quotationRequestRequiredFieldsModel, vc, vResults, true);
            var result= vResults.Select(x => x.MemberNames.FirstOrDefault());
            if(isCustomCard)
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

        private bool IsUserEnteredAllYakeenMissingFields(QuotationRequestRequiredFieldsModel model, IEnumerable<string> yakeenMissingFieldsNamesInDb)
        {
            foreach (var missingFieldName in yakeenMissingFieldsNamesInDb)
            {
                //get the property
                var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(missingFieldName);
                if (propertyInfo != null)
                {
                    //get the value of property
                    var value = propertyInfo.GetValue(model);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.ToString()=="0")
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        public bool SetUserAuthenticationCookies(AspNetUser userObject, out string exception)
        {
            exception = string.Empty;
            try
            {
                string userTicketData = string.Empty;

                #region Build User Ticket Data String
                userTicketData = "UserID=" + userObject.Id + ";"
                + "Email=" + userObject.Email + ";"
                + "CreatedDate=" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + ";"
                + "Key=" + Guid.NewGuid().ToString();

                #endregion
                #region Set main first ticket (For Non-SSL Mode) object
                System.Web.HttpCookie cookieMain = new System.Web.HttpCookie("_authCookie");
                cookieMain.HttpOnly = true;
                cookieMain.Expires = DateTime.Now.AddDays(1);
                //Create a new FormsAuthenticationTicket that includes Custom User Data
                System.Web.Security.FormsAuthenticationTicket firstTicketUserData = new System.Web.Security.FormsAuthenticationTicket(1, userObject.Id, DateTime.Now
                    , cookieMain.Expires, false, userTicketData);
                //add cookie with the new ticket value
                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                                cookieMain.Value = System.Web.Security.FormsAuthentication.Encrypt(firstTicketUserData);
                cookieMain.Secure = true;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookieMain);
                return true;
                #endregion


            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        #region Admin Add Vechile Driver 
        public Tameenk.Services.Inquiry.Components.AddDriverOutput AddVechileDriver(AddDriverModel model, string UserId, string userName, bool automatedTest = false)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.UserId = UserId.ToString();
            log.UserName = userName;
            log.NIN = model?.Driver?.NationalId;
            log.PolicyNo = model?.PolicyNo;
            try
            {
                var validationOutput = ValidateAddVechileDriverData(model, log);
                if (validationOutput.ErrorCode != AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = validationOutput.ErrorCode;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    return output;
                }
                var policy = GetPolicyByPolicyNo(model.PolicyNo, model.ReferenceId, out string ex);
                if (policy == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Policy Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy Not exist , ex = " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.Drivers.Count == 4)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Can't Add  new driver as the policy  already reached max number which is 4";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceCompanyId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceTypeCode == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Insurance Type Code  is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var quotationRequest = _quotationService.GetQuotationResponseByReferenceId(model.ReferenceId);
                var AdditoinalCount = quotationRequest.QuotationRequest.Drivers.Where(a => a.NIN != (quotationRequest.QuotationRequest.InsuredId).ToString());
                if (quotationRequest != null && AdditoinalCount.Count() > 2)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.NajmAdditionalResponseError;
                    output.ErrorDescription = "You Have Max Additional Driver 2";
                    return output;
                }
                else
                {
                    string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
                    log.RefrenceId = referenceId;
                    log.InsuranceTypeCode = policy.InsuranceTypeCode;
                    bool isExist = policy.Drivers.Any(x => x.Nin == model.Driver.NationalId);
                    if (isExist)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "Driver Already Exist on this policy";
                        log.ErrorDescription = "Driver with nin" + model.Driver.NationalId + " Already Exist on this policy ";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    var insuranceCompany = _insuranceCompanyService.GetById(policy.InsuranceCompanyId);
                    if (insuranceCompany == null)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "insurance Company is null";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    log.CompanyName = insuranceCompany.Key;
                    log.CompanyId = insuranceCompany.InsuranceCompanyID;
                    Guid userId = Guid.Empty;
                    Guid.TryParse(UserId, out userId);
                    ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                    {
                        UserID = userId,
                        UserName = userName,
                        Channel = log.Channel,
                        ServerIP = log.ServerIP
                    };
                    predefinedLogInfo.DriverNin = model.Driver.NationalId.ToString();
                    predefinedLogInfo.CompanyID = policy.InsuranceCompanyId;
                    predefinedLogInfo.PolicyNo = policy.PolicyNo;
                    var mainDriver = policy.Drivers.FirstOrDefault(x => x.IsMainDriver == 1);
                    var drivers = new List<DriverModel>
                {
                    model.Driver
                };
                    var yakeenOutput = GetDriversData(drivers, policy.Insured, false, Convert.ToInt64(mainDriver.Nin), predefinedLogInfo);
                    if (yakeenOutput.ErrorCode != DriversOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                        output.ErrorDescription = yakeenOutput.ErrorDescription;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = yakeenOutput.LogErrorDescription;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    PolicyModification policyModification = new PolicyModification()
                    {
                        Channel = model.Channel.ToString(),
                        CreatedDate = DateTime.Now,
                        UserIP = log.UserId,
                        CreatedBy = log.UserId,
                        InsuranceCompanyId = policy.InsuranceCompanyId,
                        InsuranceTypeCode = policy.InsuranceTypeCode,
                        MethodName = "AddVehicleDriver",
                        Nin = model.Driver.NationalId,
                        PolicyNo = policy.PolicyNo,
                        ReferenceId = referenceId,
                        ServerIP = log.ServerIP,
                        UserAgent = log.Headers["User-Agent"].ToString()
                    };
                    this._policyModificationrepository.Insert(policyModification);
                    if (policyModification.Id < 1)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                        output.ErrorDescription = "Failed to insert policy modification Request";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed to insert policy modification Request";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }

                    var driver = yakeenOutput.AdditionalDrivers.FirstOrDefault();
                    if (driver == null)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.DriverDataError;
                        output.ErrorDescription = "driver is null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "driver is null";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    AddDriverRequest request = new AddDriverRequest();
                    request.AdditionStartDate = model.AdditionStartDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    request.DriverBirthDate = driver.DateOfBirthH;
                    request.DriverBirthDateG = driver.DateOfBirthG;
                    request.DriverChildrenBelow16Years = driver.ChildrenBelow16Years.Value;
                    request.DriverDrivingPercentage = driver.DrivingPercentage.Value;
                    request.DriverEducationCode = driver.EducationId;
                    request.DriverFirstNameAr = driver.FirstName;
                    request.DriverFirstNameEn = driver.EnglishFirstName;
                    if (driver.GenderId == (int)Gender.Male)
                        request.DriverGenderCode = "M";
                    else if (driver.GenderId == (int)Gender.Female)
                        request.DriverGenderCode = "F";
                    else
                        request.DriverGenderCode = "M";
                    request.DriverHomeCity = driver.CityName;
                    request.DriverHomeCityCode = driver.CityId?.ToString();
                    request.DriverId = Convert.ToInt64(driver.NIN);
                    request.DriverIdTypeCode = driver.IsCitizen ? 1 : 2;
                    request.DriverLastNameAr = driver.LastName;
                    request.DriverLastNameEn = driver.EnglishLastName;
                    request.DriverMedicalConditionCode = driver.MedicalConditionId.Value;
                    request.DriverMiddleNameAr = driver.SecondName;
                    request.DriverMiddleNameEn = driver.EnglishSecondName;
                    request.DriverNationalityCode = driver.NationalityCode.HasValue ? driver.NationalityCode.Value.ToString() : "113";
                    request.DriverNOALast5Years = driver.NOALast5Years;
                    request.DriverNOCLast5Years = driver.NOCLast5Years;
                    var additionalDriverOccupation = driver.Occupation;
                    if (additionalDriverOccupation == null && request.DriverIdTypeCode == 1)
                    {
                        request.DriverOccupationCode = "O";
                        request.DriverOccupation = "غير ذالك";
                    }
                    else if (additionalDriverOccupation == null && request.DriverIdTypeCode == 2)
                    {
                        request.DriverOccupationCode = "31010";
                        request.DriverOccupation = "موظف اداري";
                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && request.DriverIdTypeCode == 1)
                        {
                            request.DriverOccupationCode = "O";
                            request.DriverOccupation = "غير ذالك";
                        }
                        else if ((string.IsNullOrEmpty(additionalDriverOccupation.Code) || additionalDriverOccupation.Code == "o") && request.DriverIdTypeCode == 2)
                        {
                            request.DriverOccupationCode = "31010";
                            request.DriverOccupation = "موظف اداري";
                        }
                        else
                        {
                            request.DriverOccupationCode = additionalDriverOccupation.Code;
                            request.DriverOccupation = additionalDriverOccupation.NameAr.Trim();
                        }
                    }
                    request.DriverRelationship = driver.RelationShipId.Value;
                    request.DriverSocialStatusCode = driver.SocialStatusId?.ToString();
                    request.DriverWorkCity = driver.WorkCityName;
                    request.DriverWorkCityCode = driver.WorkCityId?.ToString();
                    request.PolicyNo = policy.PolicyNo;
                    request.ReferenceId = referenceId;
                    var LicenseDtos = new List<LicenseDto>();
                    if (driver.DriverLicenses != null && driver.DriverLicenses.Count() > 0)
                    {
                        foreach (var item in driver.DriverLicenses)
                        {
                            LicenseDtos.Add(new LicenseDto()
                            {
                                DriverLicenseExpiryDate = item.ExpiryDateH,
                                DriverLicenseTypeCode = item.TypeDesc.ToString(),
                                LicenseCountryCode = 113,
                                LicenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year)
                            });
                        }
                    }
                    request.DriverLicenses = LicenseDtos;
                    request.DriverViolations = new List<ViolationDto>();
                    if (driver.DriverViolations != null && driver.DriverViolations.Count > 0)
                    {
                        request.DriverViolations = driver.DriverViolations.Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();
                    }
                    IInsuranceProvider provider = GetProvider(insuranceCompany, policy.InsuranceTypeCode);
                    if (provider == null)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                        output.ErrorDescription = "provider is null";
                        log.ErrorDescription = "provider is null";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    var results = provider.AddVechileDriver(request, predefinedLogInfo, automatedTest);
                    if (results == null)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = "Service Return Null ";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Service Return Null ";
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                        PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                        return output;
                    }
                    policyModification.TotalAmount = results.TotalAmount;
                    policyModification.TaxableAmount = results.TaxableAmount;
                    policyModification.VATAmount = results.VATAmount;
                    _policyModificationrepository.Update(policyModification);
                    output.TotalAmount = results.TotalAmount;
                    output.TaxableAmount = results.TaxableAmount;
                    output.VATAmount = results.VATAmount;
                    output.ReferenceId = referenceId;
                    output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private AddDriverOutput ValidateAddVechileDriverData(AddDriverModel model, PolicyModificationLog log)
        {
            AddDriverOutput output = new AddDriverOutput();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.PolicyNo))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "PolicyNo is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "PolicyNo is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.AdditionStartDate == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "AdditionStartDate is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.AdditionStartDate < DateTime.Now.Date.AddDays(1))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "AdditionStartDate must be in future";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate must be in future";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                DriverModel driver = null;
                driver = model.Driver;
                if (driver.BirthDateMonth == 0 || driver.BirthDateYear == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "BirthDate can't be zero month:" + driver.BirthDateMonth + " Year:" + driver.BirthDateYear;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BirthDate can't be zero month:" + driver.BirthDateMonth + " Year:" + driver.BirthDateYear;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (!driver.DriverNOALast5Years.HasValue)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverNOALast5YearsIsNull;
                    output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DriverNOALast5Years is zero"; ;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (driver.EducationId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EducationIdIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.EducationIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver EducationId is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (driver.MedicalConditionId == 0)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.MedicalConditionIdIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.MedicalConditionIdIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver MedicalConditionId is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (!driver.DriverNOALast5Years.HasValue)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.DriverNOALast5YearsIsNullAdditionalDriver;
                    output.ErrorDescription = SubmitInquiryResource.DriverNOALast5YearsIsNullAdditionalDriver.Replace("{0}", driver.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Additional Driver DriverNOALast5Years is zero for Additional Driver " + driver.NationalId;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                //int additionalDrivingPercentage = driver.DrivingPercentage;
                //foreach (var d in oldDrivers)
                //{                    
                //    additionalDrivingPercentage += d.DrivingPercentage;
                //}                
                //if (additionalDrivingPercentage > 100)
                //{
                //    output.ErrorCode = AddDriverOutput.ErrorCodes.DrivingPercentageMoreThan100;
                //    output.ErrorDescription = SubmitInquiryResource.DrivingPercentageErrorInvalid;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "Driving Percentage exceeded 100% for Additional Drivers as it's with total " + additionalDrivingPercentage;
                //    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                //    return output;
                //}
                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        public AddDriverOutput PurchaseVechileDriver(PurchaseDriverModel model, string UserId, string userName)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            if (!string.IsNullOrEmpty(userName))
            {
                log.UserId = UserId.ToString();
                log.UserName = userName;
            }
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }

                var request = _policyModificationrepository.Table.FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
                if (request == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(request.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid userId = Guid.Empty;
                Guid.TryParse(UserId, out userId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = userId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP
                };
                predefinedLogInfo.DriverNin = request.Nin;

                PurchaseDriverRequest serviceRequest = new PurchaseDriverRequest();
                serviceRequest.PolicyNo = request.PolicyNo;
                serviceRequest.ReferenceId = request.ReferenceId;
                serviceRequest.PaymentAmount = model.PaymentAmount;
                serviceRequest.PaymentBillNumber = request.InvoiceNo.ToString();
                IInsuranceProvider provider = GetProvider(insuranceCompany, request.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.PurchaseDriver(serviceRequest, predefinedLogInfo);
                if (results == null)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddDriverOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                output.ErrorCode = AddDriverOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddDriverOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private Models.PolicyModel GetPolicyByPolicyNo(string policyNo, string referenceId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter refNoParameter = new SqlParameter()
                {
                    ParameterName = "referenceId",
                    Value = referenceId
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(refNoParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.PolicyModel>(reader).FirstOrDefault();
                reader.NextResult();
                data.Drivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Models.AdditionalDriver>(reader).ToList();
                if (data.Drivers.Any())
                {
                    data.Insured = new InsuredModel
                    {
                        NationalId = data.Drivers.FirstOrDefault().Insured
                    };
                }
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }
        private IInsuranceProvider GetProvider(InsuranceCompany insuranceCompany, int type)
        {
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + type);
            if (instance != null && insuranceCompany.Key != "Tawuniya")
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);

                if (providerType != null)
                {
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    throw new Exception("Unable to find provider.");
                }
                if (insuranceCompany.Key != "Tawuniya")
                    Utilities.AddValueToCache("instance_" + providerFullTypeName + type, instance, 1440);

                scope.Dispose();
            }
            return provider;
        }
        #endregion

        public QuotationRequest GetQuotationRequestInfo(string quotationExternalId)
        {
            return quotationRequestRepository.TableNoTracking
                .Include(e => e.Vehicle).Include(e => e.Driver).Include(e => e.Insured)
                .FirstOrDefault(e => e.ExternalId == quotationExternalId);

        }
      
        public AddressInfoOutput GetAddressByNationalId(string nationalId,string birthDate,string externalId, string channel)
        {
            DateTime startTime = DateTime.Now;
            AddressInfoOutput output = new AddressInfoOutput();
            AddressRequestLog log = new AddressRequestLog();
            log.NationalId = nationalId;
            log.ExternalId = externalId;
            log.ServiceRequest = $"nationalId: {nationalId}, birthDate: {birthDate}, externalId: {externalId}, channel: {channel}";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                var addresses = addressService.GetAllAddressesByNin(nationalId);
                bool getAddressFromYakeen = false;
                var benchmarkDate = DateTime.Now.AddDays(-120);
                Guid driverId = Guid.Empty;
                if (addresses != null)
                {
                    addresses = addresses.OrderByDescending(x => x.CreatedDate).ToList();
                    var address = addresses.FirstOrDefault();
                    if(address!=null&&address.DriverId.HasValue)
                    {
                        driverId = address.DriverId.Value;
                    }
                    var addressesWithin30Days = addresses.Where(a => a.CreatedDate > benchmarkDate).ToList();
                    if (addressesWithin30Days == null || !addressesWithin30Days.Any())
                    {
                        getAddressFromYakeen = true;
                    }
                }
                if(driverId==Guid.Empty)
                {
                    driverId = Guid.NewGuid();
                    long nin = 0;
                    long.TryParse(nationalId, out nin);
                    var customerData = customerServices.getDriverEntityFromNin(nin);
                    if(customerData==null)
                    {
                        Driver driver = new Driver();
                        driver.DriverId = driverId;
                        driver.IsCitizen = nin.ToString().StartsWith("1");
                        driver.NIN = nin.ToString();
                        driver.GenderId = 0;
                        driver.IsDeleted = true;
                        driver.DateOfBirthG = DateTime.Now;
                        driver.CreatedDateTime = DateTime.Now;
                        driverRepository.Insert(driver);
                    }
                }
                if (addresses == null || !addresses.Any() || getAddressFromYakeen)
                {
                    var yakeenAddressOutput = GetYakeenAddress(driverId, nationalId, string.Empty, channel, birthDate, externalId);
                    log.ServiceResponse = JsonConvert.SerializeObject(yakeenAddressOutput);
                    if (addresses == null || !addresses.Any())
                    {
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                        {
                            output.ErrorCode = AddressInfoOutput.ErrorCodes.NoAddressFound;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse)
                        {
                            output.ErrorCode = AddressInfoOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.NoLookupForZipCode)
                        {
                            output.ErrorCode = AddressInfoOutput.ErrorCodes.NoLookupForZipCode;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                            return output;
                        }
                        if (yakeenAddressOutput.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = AddressInfoOutput.ErrorCodes.YakeenResultFailed;
                            output.ErrorDescription = yakeenAddressOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                            return output;
                        }
                        addressService.InsertAddresses(yakeenAddressOutput.DriverAddresses);
                        addresses = yakeenAddressOutput.DriverAddresses.OrderByDescending(a => a.IsPrimaryAddress).ToList();
                    }
                    else if (addresses != null && yakeenAddressOutput.ErrorCode == YakeenAddressOutput.ErrorCodes.Success)
                    {
                        string exception = string.Empty;
                        addressService.DeleteAllAddress(nationalId, benchmarkDate, out exception);  //mark old as deleted
                        addressService.InsertAddresses(yakeenAddressOutput.DriverAddresses);
                        addresses = yakeenAddressOutput.DriverAddresses.OrderByDescending(a => a.IsPrimaryAddress).ToList();
                    }
                }
                if (addresses == null || !addresses.Any())
                {
                    output.ErrorCode = AddressInfoOutput.ErrorCodes.NoAddressesFoundInSaudiPostOrInDB;
                    output.ErrorDescription = "No Addresses Found In SaudiPost Or In DB";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                    return output;
                }
                List<AddressInfo> addressesList = new List<AddressInfo>();
                foreach (var address in addresses)
                {
                    AddressInfo addressInfo = new AddressInfo();
                    addressInfo.AdditionalNumber = address.AdditionalNumber;
                    addressInfo.Address1 = address.Address1;
                    addressInfo.Address2 = address.Address2;
                    addressInfo.AddressLoction = address.AddressLoction;
                    addressInfo.BuildingNumber = address.BuildingNumber;
                    addressInfo.City = address.City;
                    addressInfo.CityId = address.CityId;
                    addressInfo.CreatedDate = address.CreatedDate;
                    addressInfo.District = address.District;
                    addressInfo.DriverId = address.DriverId;
                    addressInfo.IsDeleted = address.IsDeleted;
                    addressInfo.IsPrimaryAddress = address.IsPrimaryAddress;
                    addressInfo.Latitude = address.Latitude;
                    addressInfo.Longitude = address.Longitude;
                    addressInfo.ModifiedDate = address.ModifiedDate;
                    addressInfo.NationalId = address.NationalId;
                    addressInfo.ObjLatLng = address.ObjLatLng;
                    addressInfo.PKAddressID = address.PKAddressID;
                    addressInfo.PolygonString = address.PolygonString;
                    addressInfo.PostCode = address.PostCode;
                    addressInfo.RegionId = address.RegionId;
                    addressInfo.RegionName = address.RegionName;
                    addressInfo.Restriction = address.Restriction;
                    addressInfo.Street = address.Street;
                    addressInfo.Title = address.Title;
                    addressInfo.UnitNumber = address.UnitNumber;
                    addressesList.Add(addressInfo);
                }
                output.Addresses= addressesList;
                output.ErrorCode = AddressInfoOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponse = JsonConvert.SerializeObject(output);
                AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = AddressInfoOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Address exception: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                AddressRequestLogDataAccess.AddtoAddressRequestLogs(log);
                return output;
            }
        }
        public List<string> CheckBlockedNin(List<string> NationalIds)        {            try            {
                List<string> blockedNins = new List<string>();
                foreach (var NationalId in NationalIds)
                {
                    var blockedNin = _quotationBlockedNins.TableNoTracking.Where(a => a.NationalId == NationalId).FirstOrDefault();
                    if (blockedNin != null)
                    {
                        blockedNins.Add(blockedNin.NationalId);
                    }
                }                return blockedNins;            }            catch            {                return null;            }
        }

        private bool ValidateEdaatWithCompanies(EdaatRequestModel edaatRequest, List<InsuredPolicyInfo> companyPolicies, string nationalId)
        {
            /// cases
            /// 1- same 700 --> true
            /// 2- response exist
            ///     1- success --> true
            ///     2- fail && request expire (> 16 hours) --> true
            ///     3- fail && request not expire --> false
            /// 
            /// 3- response not exist
            ///     1- nin is not the same as last request --> false
            ///     2- request not expire --> false
            ///     3- 

            if (edaatRequest.InsuredNationalId == nationalId)
                return true;

            if (edaatRequest.EdaatResponseId > 0)
            {
                if (edaatRequest.IsEdaatResponseSuccess || DateTime.Now >= edaatRequest.ExpiryDate)
                    return true;
                else
                    return false;
            }
            else
            {
                if (edaatRequest.InsuredNationalId != nationalId || edaatRequest.ExpiryDate >= DateTime.Now)
                    return false;
                else
                    return true;
            }
        }
    }
}
