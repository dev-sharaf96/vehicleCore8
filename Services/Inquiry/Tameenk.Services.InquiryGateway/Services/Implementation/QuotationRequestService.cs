using Flurl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Resources.Inquiry;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Occupations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.InquiryGateway.CustomAttributes;
using Tameenk.Services.InquiryGateway.Extensions;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Models.ExceptionHandlerModel;
using Tameenk.Services.InquiryGateway.Models.YakeenMissingFields;
using Tameenk.Services.InquiryGateway.Repository;
using Tameenk.Services.InquiryGateway.Services.Core;
using Tameenk.Services.InquiryGateway.Services.Exceptions;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;

namespace Tameenk.Services.InquiryGateway.Services.Implementation
{
    public class QuotationRequestService : IQuotationRequestService
    {

        private readonly Tameenk.Services.Inquiry.Components.INajmService _najmService;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<NCDFreeYear> _NCDFreeYearRepository;
        private readonly IHttpClient _httpClient;
        private readonly IWebApiContext _webApiContext;
        private readonly string _yakeenServiceUrl;
        private readonly IAddressService _addressService;
        private readonly ILogger _logger;
        private readonly IVehicleService _vehicleService;
        private readonly IOccupationService _occupationService;
        private readonly IRepository<Insured> _insuredRepository;
        private readonly IRepository<PromotionProgramUser> _promotionProgramUserRepository;
        private readonly ICustomerServices customerServices;
        private readonly IYakeenVehicleServices yakeenVehicleServices;
        private readonly IDriverServices driverServices;
        private string _authorizationToken;
        private HashSet<string> requiredFieldsInCustomOnly = new HashSet<string>
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

        /// <summary>
        /// The Constructor
        /// </summary>
        /// <param name="najmService">Najm Service</param>
        /// <param name="quotationRequestRepository">quotation Request Repository</param>
        /// <param name="driverRepository">driver Repository</param>
        /// <param name="httpClient">http Client</param>
        /// <param name="addressService">address service</param>
        /// <param name="NCDFreeYearRepository">address service</param>
        /// <param name="webApiContext">address service</param>
        /// <param name="logger">logger</param>
        /// <param name="vehicleService">vehicle service</param>
        /// <param name="occupationService"></param>
        /// <param name="insuredRepository"></param>
        public QuotationRequestService(Tameenk.Services.Inquiry.Components.INajmService najmService, IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<Driver> driverRepository,
            IRepository<NCDFreeYear> NCDFreeYearRepository,
            IWebApiContext webApiContext,
            IHttpClient httpClient,
            IAddressService addressService,
            ILogger logger, IVehicleService vehicleService
            , IOccupationService occupationService,
            IRepository<Insured> insuredRepository
           , IRepository<PromotionProgramUser> promotionProgramUserRepository,
            ICustomerServices customerServices,
            IYakeenVehicleServices yakeenVehicleServices,
            IDriverServices driverServices
            )
        {
            _najmService = najmService ?? throw new TameenkArgumentNullException(nameof(Tameenk.Services.Inquiry.Components.INajmService));
            _quotationRequestRepository = quotationRequestRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<QuotationRequest>));
            _driverRepository = driverRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Driver>));
            _NCDFreeYearRepository = NCDFreeYearRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<NCDFreeYear>));
            _httpClient = httpClient ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
            _yakeenServiceUrl = ConfigurationManager.AppSettings["YakeenIntegrationApiUrl"];
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));
            _webApiContext = webApiContext ?? throw new TameenkArgumentNullException(nameof(IWebApiContext));
            _logger = logger;
            _vehicleService = vehicleService ?? throw new TameenkArgumentNullException(nameof(IVehicleService));
            _insuredRepository = insuredRepository;
            _promotionProgramUserRepository = promotionProgramUserRepository;
            this.customerServices = customerServices;
            this.yakeenVehicleServices = yakeenVehicleServices;
            this.driverServices = driverServices;
        }





        public InquiryResponseModel HandleQuotationRequest(InquiryRequestModel requestModel, string authorizationToken, ServiceRequestLog predefinedLogInfo)
        {
            string userId = string.Empty;
            if (predefinedLogInfo != null)
            {
                if (predefinedLogInfo.UserID.HasValue && predefinedLogInfo.UserID != Guid.Empty)
                {
                    userId = predefinedLogInfo.UserID.ToString();
                }
            }
            if (requestModel == null)
                throw new RequestModelIsNullException();

            InquiryResponseModel result = new InquiryResponseModel();
            try
            {
                //int totalDrivingPercentage = 0;
                var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                if (requestModel.Drivers.Count > 1 && mainDriver != null)
                {
                    int additionalDrivingPercentage = 0;

                    foreach (var d in requestModel.Drivers)
                    {
                        if (d.NationalId == requestModel.Insured.NationalId)
                            continue;
                        additionalDrivingPercentage += d.DrivingPercentage;
                    }
                    if (additionalDrivingPercentage > 100)
                    {
                        result.Errors.Add(SubmitInquiryResource.DrivingPercentageErrorInvalid);
                        return result;
                    }
                    int remainingPercentage = 100 - additionalDrivingPercentage;
                    requestModel.Drivers.Remove(mainDriver);
                    mainDriver.DrivingPercentage = remainingPercentage >= 0 ? remainingPercentage : 0;
                    requestModel.Drivers.Insert(0, mainDriver);
                }

                _authorizationToken = authorizationToken;
                //QuotationRequest quotationRequest = GetQuotationRequestByDetails(requestModel);

                //if (quotationRequest != null && ValidateUserPromotionProgram(userId, quotationRequest))
                //{
                //    result.QuotationRequestExternalId = quotationRequest.ExternalId;
                //    return result;
                //}

                predefinedLogInfo.DriverNin = requestModel?.Insured?.NationalId;
                predefinedLogInfo.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();

                var najmResponse = _najmService.GetNajm(new NajmRequest()
                {
                    IsVehicleRegistered = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                    PolicyHolderNin = long.Parse(requestModel.Insured.NationalId),
                    VehicleId = requestModel.Vehicle.VehicleId
                }, predefinedLogInfo);
                if (najmResponse == null)
                {
                    _logger.Log($"Inguiry Api -> QuotationRequestServices -> HandleQuotationRequest -> Najm response is null (request : {JsonConvert.SerializeObject(requestModel)}, user id : {userId})", LogLevel.Warning);
                    result.Errors.Add(NajmExceptionResource.GeneralException);
                    return result;
                }
                if (!string.IsNullOrEmpty(najmResponse.ErrorMsg))
                {
                    result.Errors.Add(najmResponse.ErrorMsg);
                    return result;
                }


                var customerYakeenInfo = GetCustomerYakeenInfo(requestModel, predefinedLogInfo);
                if (customerYakeenInfo == null)
                {
                    result.Errors.Add(WebResources.SerivceIsCurrentlyDown);
                    return result;
                }
                if (!customerYakeenInfo.Success)
                {
                    if (customerYakeenInfo.Error != null && customerYakeenInfo.Error.ErrorMessage != null)
                        result.Errors.Add(customerYakeenInfo.Error.ErrorMessage);
                    else
                        result.Errors.Add(string.Empty);
                    return result;
                }
                var insured = SaveInsured(customerYakeenInfo, requestModel);


                var vehicleYakeenInfo = GetVehicleYakeenInfo(requestModel, predefinedLogInfo);
                if (vehicleYakeenInfo == null)
                {
                    result.Errors.Add(WebResources.SerivceIsCurrentlyDown);
                    return result;
                }
                if (!vehicleYakeenInfo.Success)
                {
                    if (vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorMessage != null)
                        result.Errors.Add(vehicleYakeenInfo.Error.ErrorMessage);
                    else
                        result.Errors.Add(string.Empty);
                    return result;
                }

                IEnumerable<DriverYakeenInfoModel> additionalDriversYakeenInfo;
                if (requestModel.Drivers != null && requestModel.Drivers.Any())
                    additionalDriversYakeenInfo = GetAdditionalDriversYakeenInfo(requestModel, predefinedLogInfo);
                else
                    additionalDriversYakeenInfo = new List<DriverYakeenInfoModel>();

                if (additionalDriversYakeenInfo == null)
                {
                    result.Errors.Add(WebResources.SerivceIsCurrentlyDown);
                    return result;
                }
                if (additionalDriversYakeenInfo.Any(d => !d.Success))
                {
                    var addditionalDriversErrors = additionalDriversYakeenInfo.Where(d => !d.Success);
                    foreach (var additionalDriverError in addditionalDriversErrors)
                    {
                        if (additionalDriverError.Error != null && additionalDriverError.Error.ErrorMessage != null)
                            result.Errors.Add(additionalDriverError.Error.ErrorMessage);
                        else
                            result.Errors.Add(string.Empty);
                    }

                    return result;
                }
                // var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);

                var qtRqstExtrnlId = GetNewRequestExternalId();
                QuotationRequest quotationRequest = new QuotationRequest()
                {
                    ExternalId = qtRqstExtrnlId,
                    MainDriverId = customerYakeenInfo.TameenkId,
                    CityCode = requestModel.CityCode,
                    RequestPolicyEffectiveDate = requestModel.PolicyEffectiveDate,
                    VehicleId = vehicleYakeenInfo.TameenkId,
                    UserId = string.IsNullOrEmpty(userId) ? null : userId,
                    NajmNcdRefrence = najmResponse.NCDReference,
                    NajmNcdFreeYears = najmResponse.NCDFreeYears,
                    CreatedDateTime = DateTime.Now,
                    Insured = insured
                };

                if (additionalDriversYakeenInfo.Any())
                {
                    quotationRequest.Drivers = new List<Driver>();
                    foreach (var additionalDriver in additionalDriversYakeenInfo)
                    {
                        var dbDriver = _driverRepository.Table.FirstOrDefault(d => d.DriverId == additionalDriver.TameenkId);
                        if (dbDriver == null)
                        {
                            dbDriver = new Driver() { DriverId = additionalDriver.TameenkId };
                        }
                        var driver = GetDriverNajmResponse(dbDriver, requestModel.Vehicle.VehicleId, requestModel.Vehicle.VehicleIdTypeId, predefinedLogInfo);
                        driver.DrivingPercentage = requestModel.Drivers.FirstOrDefault(x => x.NationalId == additionalDriver.NIN).DrivingPercentage;
                        quotationRequest.Drivers.Add(driver);
                    }
                }

                _quotationRequestRepository.Insert(quotationRequest);
                var driverNCDFreeYears = najmResponse.NCDFreeYears.HasValue ? najmResponse.NCDFreeYears.Value : 0;

                if (quotationRequest != null && quotationRequest.Driver != null && quotationRequest.Driver.NCDFreeYears.HasValue)
                    driverNCDFreeYears = quotationRequest.Driver.NCDFreeYears.Value;

                var NCDObject = _NCDFreeYearRepository.Table.FirstOrDefault(x => x.Code == driverNCDFreeYears);
                result.QuotationRequestExternalId = qtRqstExtrnlId;
                result.Vehicle = ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);

                result.NajmNcdFreeYears = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? NCDObject.ArabicDescription : NCDObject.EnglishDescription;
                return result;
            }
            catch (NajmErrorException ex)
            {
                _logger.Log("Inquiry QuotationRequestServices -> HandleQuotationRequest", ex);
                InquiryOutputModel model = new InquiryOutputModel();
                HandleNajmException(ex, ref model);
                result.Errors.Add(model.Description);

                //result.Errors.Add(ex.GetBaseException().Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Log("Inquiry QuotationRequestServices -> HandleQuotationRequest", ex);
                InquiryOutputModel model = new InquiryOutputModel();
                HandleNajmException(ex, ref model);
                result.Errors.Add(model.Description);
                //result.Errors.Add(ex.GetBaseException().Message);
                return result;
            }
        }

        public InquiryResponseModel HandleQuotationRequestNew(InquiryRequestModel requestModel, string authorizationToken, ServiceRequestLog predefinedLogInfo)
        {
            //todo :: Naming Convention
            InquiryResponseModel result = new InquiryResponseModel();
            result.InquiryOutputModel = new InquiryOutputModel();

            string userId = string.Empty;
            if (predefinedLogInfo != null)
            {
                if (predefinedLogInfo.UserID.HasValue && predefinedLogInfo.UserID != Guid.Empty)
                {
                    userId = predefinedLogInfo.UserID.ToString();
                }
            }
            if (requestModel == null)
            {
                result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, SubmitInquiryResource.RequestModelIsNullException, "HandleQuotationRequestAfterRefactor", result.InquiryOutputModel);
                return result;
            }


            try
            {
                _authorizationToken = authorizationToken;
                QuotationRequest quotationRequest = GetQuotationRequestByDetails(requestModel);

                if (quotationRequest != null && ValidateUserPromotionProgram(userId, quotationRequest))
                {
                    result.QuotationRequestExternalId = quotationRequest.ExternalId;
                    return result;
                }

                predefinedLogInfo.DriverNin = requestModel?.Insured?.NationalId;
                predefinedLogInfo.VehicleId = requestModel?.Vehicle?.VehicleId.ToString();

                var najmResponse = _najmService.GetNajm(new NajmRequest()
                {
                    IsVehicleRegistered = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                    PolicyHolderNin = long.Parse(requestModel.Insured.NationalId),
                    VehicleId = requestModel.Vehicle.VehicleId
                }, predefinedLogInfo);
                if (najmResponse == null)
                {
                    _logger.Log($"Inguiry Api -> QuotationRequestServices -> HandleQuotationRequest -> Najm response is null (request : {JsonConvert.SerializeObject(requestModel)}, user id : {userId})", LogLevel.Warning);
                    result.Errors.Add(NajmExceptionResource.GeneralException);
                    result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, NajmExceptionResource.GeneralException, "HandleQuotationRequestAfterRefactor", result.InquiryOutputModel);
                    return result;

                }
                if (!string.IsNullOrEmpty(najmResponse.ErrorMsg))
                {
                    result.Errors.Add(najmResponse.ErrorMsg);
                    return result;
                }
                var customerYakeenInfo = GetCustomerYakeenInfo(requestModel, predefinedLogInfo);
                if (customerYakeenInfo == null)
                {
                    result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, WebResources.SerivceIsCurrentlyDown, "HandleQuotationRequestAfterRefactor", result.InquiryOutputModel);
                    result.Errors.Add(WebResources.SerivceIsCurrentlyDown);
                    return result;
                }
                if (!customerYakeenInfo.Success)
                {
                    if (customerYakeenInfo.Error != null && customerYakeenInfo.Error.ErrorMessage != null)
                    {
                        result.Errors.Add(customerYakeenInfo.Error.ErrorMessage);
                        result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, customerYakeenInfo.Error.ErrorMessage, "GetCustomerYakeenInfo", result.InquiryOutputModel);
                    }

                    else
                    {
                        result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, string.Empty, "GetCustomerYakeenInfo", result.InquiryOutputModel);
                        result.Errors.Add(string.Empty);
                    }
                    return result;
                }
                var insured = SaveInsured(customerYakeenInfo, requestModel);


                var vehicleYakeenInfo = GetVehicleYakeenInfo(requestModel, predefinedLogInfo);
                if (vehicleYakeenInfo == null)
                {
                    result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, WebResources.SerivceIsCurrentlyDown, "GetVehicleYakeenInfo", result.InquiryOutputModel);
                    return result;

                }
                if (!vehicleYakeenInfo.Success)
                {
                    if (vehicleYakeenInfo.Error != null && vehicleYakeenInfo.Error.ErrorMessage != null)
                    {
                        result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, vehicleYakeenInfo.Error.ErrorMessage, "GetVehicleYakeenInfo", result.InquiryOutputModel);

                        result.Errors.Add(vehicleYakeenInfo.Error.ErrorMessage);
                    }
                    else
                    {
                        result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, string.Empty, "GetVehicleYakeenInfo", result.InquiryOutputModel);
                        result.Errors.Add(string.Empty);
                    }
                    return result;
                }

                IEnumerable<DriverYakeenInfoModel> additionalDriversYakeenInfo;
                if (requestModel.Drivers != null && requestModel.Drivers.Any())
                    additionalDriversYakeenInfo = GetAdditionalDriversYakeenInfo(requestModel, predefinedLogInfo);
                else
                    additionalDriversYakeenInfo = new List<DriverYakeenInfoModel>();

                if (additionalDriversYakeenInfo == null)
                {
                    result.InquiryOutputModel = SetInquiryOutput((int)System.Net.HttpStatusCode.BadRequest, WebResources.SerivceIsCurrentlyDown, "GetAdditionalDriversYakeenInfo", result.InquiryOutputModel);

                    result.Errors.Add(WebResources.SerivceIsCurrentlyDown);
                    return result;
                }
                if (additionalDriversYakeenInfo.Any(d => !d.Success))
                {
                    var addditionalDriversErrors = additionalDriversYakeenInfo.Where(d => !d.Success);
                    foreach (var additionalDriverError in addditionalDriversErrors)
                    {
                        if (additionalDriverError.Error != null && additionalDriverError.Error.ErrorMessage != null)
                            result.Errors.Add(additionalDriverError.Error.ErrorMessage);
                        else
                            result.Errors.Add(string.Empty);
                    }

                    return result;
                }
                // var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);

                var qtRqstExtrnlId = GetNewRequestExternalId();
                quotationRequest = new QuotationRequest()
                {
                    ExternalId = qtRqstExtrnlId,
                    MainDriverId = customerYakeenInfo.TameenkId,
                    CityCode = requestModel.CityCode,
                    RequestPolicyEffectiveDate = requestModel.PolicyEffectiveDate,
                    VehicleId = vehicleYakeenInfo.TameenkId,
                    UserId = string.IsNullOrEmpty(userId) ? null : userId,
                    NajmNcdRefrence = najmResponse.NCDReference,
                    NajmNcdFreeYears = najmResponse.NCDFreeYears,
                    CreatedDateTime = DateTime.Now,
                    Insured = insured
                };

                if (additionalDriversYakeenInfo.Any())
                {
                    quotationRequest.Drivers = new List<Driver>();
                    foreach (var additionalDriver in additionalDriversYakeenInfo)
                    {
                        var dbDriver = _driverRepository.Table.FirstOrDefault(d => d.DriverId == additionalDriver.TameenkId);
                        if (dbDriver == null)
                        {
                            dbDriver = new Driver() { DriverId = additionalDriver.TameenkId };
                        }
                        var driver = GetDriverNajmResponse(dbDriver, requestModel.Vehicle.VehicleId, requestModel.Vehicle.VehicleIdTypeId, predefinedLogInfo);
                        driver.DrivingPercentage = requestModel.Drivers.FirstOrDefault(x => x.NationalId == additionalDriver.NIN).DrivingPercentage;
                        quotationRequest.Drivers.Add(driver);
                    }
                }

                _quotationRequestRepository.Insert(quotationRequest);
                var NCDObject = _NCDFreeYearRepository.TableNoTracking.FirstOrDefault(x => x.Code == quotationRequest.Driver.NCDFreeYears);
                result.QuotationRequestExternalId = qtRqstExtrnlId;
                result.Vehicle = ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);

                result.NajmNcdFreeYears = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? NCDObject.ArabicDescription : NCDObject.EnglishDescription;
                return result;
            }
            catch (NajmErrorException ex)
            {
                _logger.Log("Inquiry QuotationRequestServices -> HandleQuotationRequest", ex);
                result.Errors.Add(ex.GetBaseException().Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log("Inquiry QuotationRequestServices -> HandleQuotationRequest", ex);
                result.Errors.Add(ex.GetBaseException().Message);
                return result;
            }
        }

        public InitInquiryResponseModel InitInquiryRequest(InitInquiryRequestModel requestModel)
        {
            if (requestModel == null)
                throw new RequestModelIsNullException();

            InitInquiryResponseModel result = new InitInquiryResponseModel();
            try
            {
                _logger.Log($"QuotationRequestService -> InitQuotationRequest started.");
                result.PolicyEffectiveDate = requestModel.PolicyEffectiveDate;
                var vehicle = InitVehicleRequest(requestModel);
                result.IsCustomerCurrentOwner = !requestModel.OwnerTransfer;
                if (requestModel.OwnerTransfer)
                    result.OldOwnerNin = long.Parse(requestModel.OwnerNationalId);
                if (vehicle == null)
                {
                    result.IsVehicleExist = false;
                }
                else
                {
                    //add vehicle info into the model
                    result.Vehicle = vehicle.ToModel();
                    result.Vehicle.ApproximateValue = vehicle.VehicleValue ?? 30000;
                    //mark the vehcile as exist
                    result.IsVehicleExist = true;
                    result.IsVehicleUsedCommercially = vehicle.IsUsedCommercially ?? false;

                }

                var driver = InitDriverRequest(requestModel);
                if (driver == null)
                {
                    result.IsMainDriverExist = false;
                }
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
                        MedicalConditionId = driver.MedicalConditionId ?? 1
                    };

                    if (driver.DriverViolations != null && driver.DriverViolations.Any())
                        mainDriver.ViolationIds = driver.DriverViolations.Select(e => e.ViolationId).ToList();

                    result.IsCustomerSpecialNeed = driver.IsSpecialNeed;
                    var insured = _insuredRepository.TableNoTracking.FirstOrDefault(e => e.NationalId == requestModel.NationalId);
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

            }
            catch (Exception ex)
            {
                _logger.Log("QuotationRequestService->InitQuotationRequest An error happen while initiating inquery request", ex);
                result.Errors = new List<ErrorModel> { new ErrorModel(ex.GetBaseException().Message) };
            }

            return result;
        }

        public QuotationRequest GetQuotationRequest(string quotationExternalId)
        {
            return _quotationRequestRepository.Table
                .Include(e => e.Vehicle).Include(e => e.Driver)
                .FirstOrDefault(e => e.ExternalId == quotationExternalId);

        }

        public InquiryResponseModel UpdateQuotationRequestWithYakeenMissingFields(YakeenMissingInfoRequestModel model)
        {
            //if(model.YakeenMissingFields.VehicleMakerCode != null && model.YakeenMissingFields.VehicleMakerCode != 0)
            //{
            //    model.YakeenMissingFields.VehicleMaker = _vehicleService.GetMakerName(model.YakeenMissingFields.VehicleMakerCode ?? 0, "", _webApiContext.CurrentLanguage);

            //}
            //if (model.YakeenMissingFields.VehicleModelCode != null && model.YakeenMissingFields.VehicleModelCode != 0)
            //{
            //    model.YakeenMissingFields.VehicleModel = _vehicleService.GetModelName(model.YakeenMissingFields.VehicleModelCode ?? 0, (short)model.YakeenMissingFields.VehicleMakerCode ,  "", _webApiContext.CurrentLanguage);

            //}

            InquiryResponseModel result = new InquiryResponseModel
            {
                QuotationRequestExternalId = model.QuotationRequestExternalId
            };

            // 1- get the missing properties from db
            //get quotation request
            var quotationRequest = GetQuotationRequest(model.QuotationRequestExternalId);
            if (quotationRequest == null)
                throw new TameenkEntityNotFoundException("quotationExternalId", "There is no quotation request with the given external id.");

            //convert to model to 
            var quotationRequiredFieldsModel = quotationRequest.ToQuotationRequestRequiredFieldsModel();


            var missingPropertiesNames = GetYakeenMissingPropertiesName(quotationRequiredFieldsModel);

            if (quotationRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
            {
                missingPropertiesNames = GetYakeenMissingFieldNamesInCustomCard(missingPropertiesNames);
            }

            // 2- foreach missing info from yakeen check that it has value in the submited model
            // 3- if they exist then update the db with the values from submited model, else return error to user

            if (model.YakeenMissingFields.VehicleMakerCode.HasValue && model.YakeenMissingFields.VehicleMakerCode.Value > 0)
                model.YakeenMissingFields.VehicleMaker = model.YakeenMissingFields.VehicleMakerCode.Value.ToString();

            if (IsUserEnteredAllYakeenMissingFields(model.YakeenMissingFields, missingPropertiesNames))
            {
                UpdateQuotationRequestRequiredFieldsModel(quotationRequiredFieldsModel, model.YakeenMissingFields, missingPropertiesNames);
                var updatedQuotationRequest = quotationRequiredFieldsModel.ToEntity(quotationRequest);
                updatedQuotationRequest.ManualEntry = true;
                _quotationRequestRepository.Update(updatedQuotationRequest);
                result.IsValidInquiryRequest = true;
                return result;
            }
            else
            {
                return HandleYakeenMissingFields(result);
            }

        }

        /// <summary>
        /// Check if all info from yakeen are returned, if not then user should enter all the missing fields.
        /// </summary>
        /// <param name="result">Inquiry response model</param>
        /// <returns></returns>
        public InquiryResponseModel HandleYakeenMissingFields(InquiryResponseModel result)
        {
            //get quotation request
            var quotationRequest = GetQuotationRequest(result.QuotationRequestExternalId);
            if (quotationRequest == null)
                throw new TameenkEntityNotFoundException("quotationExternalId", SubmitInquiryResource.NoQuotation);



            //convert to model to validate that all yakeen data are not missing
            var quotationRequiredFieldsModel = quotationRequest.ToQuotationRequestRequiredFieldsModel();

            if (quotationRequiredFieldsModel.VehicleModel == "غير متوفر" && string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleModel))
            {
                quotationRequiredFieldsModel.VehicleModel = null;
            }

            if (quotationRequiredFieldsModel.VehicleMaker == "غير متوفر" && string.IsNullOrEmpty(quotationRequiredFieldsModel.VehicleMaker))
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
            if (quotationRequiredFieldsModel.VehicleBodyCode == null || quotationRequiredFieldsModel.VehicleBodyCode <1)
            {
                quotationRequiredFieldsModel.VehicleBodyCode = null;
            }


            ValidationContext vc = new ValidationContext(quotationRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            result.IsValidInquiryRequest = Validator.TryValidateObject(quotationRequiredFieldsModel, vc, vResults, true);
            if (!result.IsValidInquiryRequest)
            {
                if (quotationRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
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


        #region Private Methods

        private Insured SaveInsured(CustomerYakeenInfoModel customerYakeenInfo, InquiryRequestModel requestModel)
        {
            var insured = new Insured();
            insured.BirthDate = customerYakeenInfo.DateOfBirthG;
            insured.BirthDateH = customerYakeenInfo.DateOfBirthH;
            insured.NationalId = customerYakeenInfo.NIN;
            insured.CardIdTypeId = customerYakeenInfo.IsCitizen ? 1 : 2;
            insured.Gender = customerYakeenInfo.Gender;
            insured.NationalityCode = customerYakeenInfo.NationalityCode.GetValueOrDefault().ToString();
            insured.FirstNameAr = customerYakeenInfo.FirstName;
            insured.MiddleNameAr = customerYakeenInfo.SecondName;
            insured.LastNameAr = $"{customerYakeenInfo.ThirdName} {customerYakeenInfo.LastName}";
            insured.FirstNameEn = customerYakeenInfo.EnglishFirstName;
            insured.MiddleNameEn = customerYakeenInfo.EnglishSecondName;
            insured.LastNameEn = $"{customerYakeenInfo.EnglishThirdName} {customerYakeenInfo.EnglishLastName}";
            insured.CityId = requestModel.CityCode;
            insured.WorkCityId = requestModel.CityCode;
            insured.ChildrenBelow16Years = requestModel.Insured.ChildrenBelow16Years;
            insured.SocialStatusId = customerYakeenInfo.SocialStatusId.HasValue ? customerYakeenInfo.SocialStatusId.Value : 0;
            insured.OccupationId = customerYakeenInfo.OccupationId;
            insured.IdIssueCityId = _addressService.GetCityByName(_addressService.GetAllCities(),Utilities.Removemultiplespaces(customerYakeenInfo.IdIssuePlace))?.Code;
            insured.EducationId = requestModel.Insured.EducationId;
            insured.CreatedDateTime = DateTime.Now;
            insured.ModifiedDateTime = DateTime.Now;
            insured.UserSelectedCityId = requestModel.CityCode;
            insured.UserSelectedWorkCityId = requestModel.CityCode;

            _insuredRepository.Insert(insured);
            return insured;
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

        private IEnumerable<string> GetYakeenMissingFieldNamesInCustomCard(IEnumerable<string> missingPropertiesNames)
        {
            List<string> result = new List<string>();
            foreach (var item in missingPropertiesNames)
            {
                if (requiredFieldsInCustomOnly.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        private Models.VehicleModel GetVehicleDetails(Vehicle vehicle)
        {
            Models.VehicleModel vehicleModel = new Models.VehicleModel();
            // CarPlateInfo carPlateInfo = new CarPlateInfo(vehicle.CarPlateText1, vehicle.CarPlateText2, vehicle.CarPlateText3, vehicle.CarPlateNumber.HasValue ? vehicle.CarPlateNumber.Value : 0);
            vehicleModel.CarPlateText1 = vehicle.CarPlateText1;
            vehicleModel.CarPlateText2 = vehicle.CarPlateText2;
            vehicleModel.CarPlateText3 = vehicle.CarPlateText3;
            vehicleModel.CarPlateNumber = vehicle.CarPlateNumber;



            //vehicleModel.PlateTypeCode = vehicle.PlateTypeCode;

            /*vehicleModel.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
            vehicleModel.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
            vehicleModel.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
            vehicleModel.CarPlateTextEn = carPlateInfo.CarPlateTextEn;*/

            vehicleModel.PlateTypeCode = vehicle.PlateTypeCode;

            // vehicleModel.PlateColor = _vehicleService.GetPlateColor(vehicle.PlateTypeCode);
            vehicleModel.Model = vehicle.VehicleModel;
            vehicleModel.MinorColor = vehicle.MinorColor;
            vehicleModel.MajorColor = vehicle.MajorColor;
            vehicleModel.VehicleMaker = vehicle.VehicleMaker;
            vehicleModel.ModelYear = vehicle.ModelYear;
            vehicleModel.VehicleMakerCode = vehicle.VehicleMakerCode;
            vehicleModel.VehicleModelYear = vehicle.ModelYear;
            return vehicleModel;
        }

        private Models.VehicleModel ConvertVehicleYakeenToVehicle(VehicleYakeenModel vehicleYakeenModel)
        {
            Models.VehicleModel vehicleModel = new Models.VehicleModel();
            vehicleModel.CarPlateText1 = vehicleYakeenModel.CarPlateText1;
            vehicleModel.CarPlateText2 = vehicleYakeenModel.CarPlateText2;
            vehicleModel.CarPlateText3 = vehicleYakeenModel.CarPlateText3;
            vehicleModel.CarPlateNumber = vehicleYakeenModel.CarPlateNumber;
            vehicleModel.PlateTypeCode = vehicleYakeenModel.PlateTypeCode;
            /*CarPlateInfo carPlateInfo = new CarPlateInfo(vehicleYakeenModel.CarPlateText1, vehicleYakeenModel.CarPlateText2, vehicleYakeenModel.CarPlateText3, vehicleYakeenModel.CarPlateNumber.HasValue ? vehicleYakeenModel.CarPlateNumber.Value : 0);
            vehicleModel.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
            vehicleModel.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
            vehicleModel.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
            vehicleModel.CarPlateTextEn = carPlateInfo.CarPlateTextEn;
            vehicleModel.PlateColor = _vehicleService.GetPlateColor(vehicleYakeenModel.PlateTypeCode);
            */
            vehicleModel.Model = vehicleYakeenModel.Model;
            vehicleModel.MinorColor = vehicleYakeenModel.MinorColor;
            vehicleModel.MajorColor = vehicleYakeenModel.MajorColor;
            vehicleModel.VehicleMaker = vehicleYakeenModel.Maker;
            vehicleModel.ModelYear = vehicleYakeenModel.ModelYear;
            vehicleModel.VehicleMakerCode = vehicleYakeenModel.MakerCode;
            vehicleModel.VehicleModelYear = vehicleYakeenModel.ModelYear;

            return vehicleModel;
        }


        private string GetNewRequestExternalId()
        {
            string qtExtrnlId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (_quotationRequestRepository.TableNoTracking.Any(q => q.ExternalId == qtExtrnlId))
                return GetNewRequestExternalId();

            return qtExtrnlId;
        }

        private QuotationRequest GetQuotationRequestByDetails(InquiryRequestModel requestModel)
        {
            // Get only the valid request the are not expired within 16 hours window.
            var benchmarkDate = DateTime.Now.AddHours(-16);
            var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);

            foreach (var driver in requestModel.Drivers)
            {
                var driverInfo = _driverRepository.Table.Where(a => a.NIN == driver.NationalId && a.IsDeleted == false).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (driverInfo == null)
                    continue;
                if (driver.DrivingPercentage != driverInfo.DrivingPercentage)
                    return null;
                if (driver.ChildrenBelow16Years != driverInfo.ChildrenBelow16Years)
                    return null;
                if (driver.EducationId != driverInfo.EducationId)
                    return null;
                if (driver.MedicalConditionId != driverInfo.MedicalConditionId)
                    return null;
                if (driverInfo.DriverViolations == null && driver.ViolationIds != null)
                    return null;
                if (driverInfo.DriverViolations != null && driver.ViolationIds == null)
                    return null;
                if (driverInfo.DriverViolations != null && driver.ViolationIds != null)
                {
                    foreach (int v in driver.ViolationIds)
                    {
                        var violation = driverInfo.DriverViolations.FirstOrDefault(a => a.ViolationId == v && a.DriverId == driverInfo.DriverId);
                        if (violation == null)
                            return null;
                    }
                }
            }
            var query = _quotationRequestRepository.Table
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

            if (mainDriver.ViolationIds != null && mainDriver.ViolationIds.Any())
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

        private CustomerYakeenInfoModel GetCustomerYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var customerInfoRequest = new CustomerYakeenInfoRequestModel()
                {
                    Nin = long.Parse(requestModel.Insured.NationalId),
                    BirthMonth = requestModel.Insured.BirthDateMonth,
                    BirthYear = requestModel.Insured.BirthDateYear,
                    IsSpecialNeed = requestModel.IsCustomerSpecialNeed.HasValue
                               ? requestModel.IsCustomerSpecialNeed.Value : false
                               //,CityId = requestModel?.CityCode

                };
                var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                if (mainDriver != null)
                {
                    customerInfoRequest.MedicalConditionId = mainDriver.MedicalConditionId;
                    customerInfoRequest.EducationId = mainDriver.EducationId;
                    customerInfoRequest.ChildrenBelow16Years = mainDriver.ChildrenBelow16Years;
                    customerInfoRequest.DrivingPercentage = mainDriver.DrivingPercentage;
                    customerInfoRequest.ViolationIds = mainDriver.ViolationIds;
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
                    return CustomerYakeenInfoModel;
                }
                return customerYakeenInfoModel.CustomerYakeenInfoModel;
            }
            catch (Exception ex)
            {
                var customerYakeenInfoModel = new CustomerYakeenInfoModel();
                customerYakeenInfoModel.Error = new YakeenInfoErrorModel();
                customerYakeenInfoModel.Error.ErrorCode = "5";
                customerYakeenInfoModel.Error.ErrorMessage = ex.ToString();
                return customerYakeenInfoModel;
            }
            //    var queryUrl = _yakeenServiceUrl
            //        .AppendPathSegment(RepositoryConstants.YakeenCustomerEndpoint);
            //    var customerDataAsString = _httpClient.PostAsync(queryUrl, customerInfoRequest, _authorizationToken)
            //        .Result.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<CustomerYakeenInfoModel>(customerDataAsString);
            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("QuotationRequestService -> GetCustomerYakeenInfo : failed to get info from yakeen for object:" + JsonConvert.SerializeObject(requestModel), ex);
            //    return null;
            //}
        }

        private IEnumerable<DriverYakeenInfoModel> GetAdditionalDriversYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
        {
            var result = new List<DriverYakeenInfoModel>();
            try
            {

                foreach (var d in requestModel.Drivers)
                {

                    var driverInfoRequest = new DriverYakeenInfoRequestModel()
                    {
                        Nin = long.Parse(d.NationalId),
                        BirthMonth = d.BirthDateMonth,
                        BirthYear = d.BirthDateYear,
                        MedicalConditionId = d.MedicalConditionId,
                        ViolationIds = d.ViolationIds,

                        EducationId = d.EducationId,
                        ChildrenBelow16Years = d.ChildrenBelow16Years,
                        DrivingPercentage = d.DrivingPercentage
                    };

                    if (d.DriverExtraLicenses != null && d.DriverExtraLicenses.Any())
                    {
                        driverInfoRequest.DriverExtraLicenses = d.DriverExtraLicenses
                                         .Select(e => new Integration.Dto.Yakeen.DriverExtraLicenseModel
                                         {
                                             CountryId = e.CountryId,
                                             LicenseYearsId = e.LicenseYearsId
                                         }).ToList();
                    }
                    driverInfoRequest.UserId = predefinedLogInfo.UserID;
                    driverInfoRequest.UserName = predefinedLogInfo.UserName;
                    driverInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;

                    DriverBusiness _driverService = new DriverBusiness(driverServices, customerServices);
                    var driverYakeenInfoModel = _driverService.Post(driverInfoRequest,string.Empty,string.Empty);
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
            //        var queryUrl = _yakeenServiceUrl
            //            .AppendPathSegment(RepositoryConstants.YakeenDriverEndpoint);
            //        var driverDataAsString = _httpClient.PostAsync(queryUrl, driverInfoRequest, authorizationToken: _authorizationToken)
            //            .Result.Content.ReadAsStringAsync().Result;
            //        var driver = JsonConvert.DeserializeObject<DriverYakeenInfoModel>(driverDataAsString);
            //        result.Add(driver);
            //    }

            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("QuotationRequestService -> GetAdditionalDriversYakeenInfo : failed to get info from yakeen for object:" + JsonConvert.SerializeObject(requestModel), ex);
            //    return null;
            //}
        }

        private IEnumerable<DriverYakeenInfoModel> GetAdditionalDriversYakeenInfo(InquiryRequestModel requestModel, InquiryRequestLog log)
        {
            var result = new List<DriverYakeenInfoModel>();
            try
            {

                foreach (var d in requestModel.Drivers)
                {

                    var driverInfoRequest = new DriverYakeenInfoRequestModel()
                    {
                        Nin = long.Parse(d.NationalId),
                        BirthMonth = d.BirthDateMonth,
                        BirthYear = d.BirthDateYear,
                        MedicalConditionId = d.MedicalConditionId,
                        ViolationIds = d.ViolationIds
                    };
                    var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
                    if (mainDriver != null)
                    {
                        driverInfoRequest.MedicalConditionId = mainDriver.MedicalConditionId;
                        driverInfoRequest.EducationId = mainDriver.EducationId;
                        driverInfoRequest.ChildrenBelow16Years = mainDriver.ChildrenBelow16Years;
                        driverInfoRequest.DrivingPercentage = mainDriver.DrivingPercentage;
                    }

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
                    var driverYakeenInfoModel = _driverService.Post(driverInfoRequest,string.Empty,string.Empty);
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
        private VehicleYakeenModel GetVehicleYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo)
        {
            try
            {
                var vehicleInfoRequest = new VehicleInfoRequestModel()
                {
                    VehicleId = requestModel.Vehicle.VehicleId,
                    VehicleIdTypeId = requestModel.Vehicle.VehicleIdTypeId,
                    OwnerNin = requestModel.Vehicle.OwnerTransfer ? requestModel.OldOwnerNin.Value : long.Parse(requestModel.Insured.NationalId),
                    ModelYear = requestModel.Vehicle.ManufactureYear,
                    VehicleValue = requestModel.Vehicle.ApproximateValue,
                    IsUsedCommercially = requestModel.IsVehicleUsedCommercially,
                    IsOwnerTransfer = requestModel.Vehicle.OwnerTransfer,
                    BrakeSystemId = requestModel.Vehicle.BrakeSystemId,
                    CameraTypeId = requestModel.Vehicle.CameraTypeId,
                    CruiseControlTypeId = requestModel.Vehicle.CruiseControlTypeId,
                    CurrentMileageKM = requestModel.Vehicle.CurrentMileageKM,
                    HasAntiTheftAlarm = requestModel.Vehicle.HasAntiTheftAlarm,
                    HasFireExtinguisher = requestModel.Vehicle.HasFireExtinguisher,
                    ParkingSensorId = requestModel.Vehicle.ParkingSensorId,
                    TransmissionTypeId = requestModel.Vehicle.TransmissionTypeId,
                    ParkingLocationId = requestModel.Vehicle.ParkingLocationId,
                    HasModification = requestModel.Vehicle.HasModification,
                    Modification = requestModel.Vehicle.Modification,
                    MileageExpectedAnnualId = requestModel.Vehicle.MileageExpectedAnnualId

                };

                vehicleInfoRequest.UserId = predefinedLogInfo.UserID;
                vehicleInfoRequest.UserName = predefinedLogInfo.UserName;
                vehicleInfoRequest.ParentRequestId = predefinedLogInfo.RequestId;
                VehicleBusiness _vehicleBusiness = new VehicleBusiness(yakeenVehicleServices, null);
                var outputModel = _vehicleBusiness.GetBySequenceNumberOrCustomCardNumber(vehicleInfoRequest);
                return outputModel.VehicleYakeenModel;

            }
            catch (Exception ex)
            {
                var vehicleYakeenModel = new VehicleYakeenModel();
                vehicleYakeenModel.Error = new YakeenInfoErrorModel();
                vehicleYakeenModel.Error.ErrorCode = "5";
                vehicleYakeenModel.Error.ErrorMessage = ex.ToString();
                return vehicleYakeenModel;
            }
            //    var queryUrl = _yakeenServiceUrl
            //        .AppendPathSegment(RepositoryConstants.YakeenVehicleEndpoint);
            //    var vehicleDataAsString = _httpClient.PostAsync(queryUrl, vehicleInfoRequest, _authorizationToken)
            //                                         .Result.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<CommonResponseModel<Integration.Dto.Yakeen.VehicleYakeenModel>>(vehicleDataAsString).Data;

            //}
            //catch (Exception ex)
            //{
            //    _logger.Log("QuotationRequestService -> GetVehicleYakeenInfo : failed to get info from yakeen for object:" + JsonConvert.SerializeObject(requestModel), ex);
            //    return null;
            //}
        }

        private Driver GetDriverNajmResponse(Driver driver, long vehicleId, int vehicleIdTypeId, ServiceRequestLog predefinedLogInfo)
        {

            NajmRequest ngmResponse = new NajmRequest();

            ngmResponse.IsVehicleRegistered = vehicleIdTypeId == (int)VehicleIdType.SequenceNumber;

            long nin = 0;
            if (driver != null && driver.NIN != null)
            {
                long.TryParse(driver.NIN.ToString(), out nin);
            }

            ngmResponse.PolicyHolderNin = nin;// long.Parse(driver.NIN.ToString());
            ngmResponse.VehicleId = vehicleId;


            var najmResponse = _najmService.GetNajm(ngmResponse, predefinedLogInfo);
            if (najmResponse == null)
            {
                throw new NajmErrorException("There is an error occured please try gain later");
            }
            if (!string.IsNullOrWhiteSpace(najmResponse.ErrorMsg))
            {
                throw new NajmErrorException(najmResponse.ErrorMsg);
            }
            driver.NCDFreeYears = najmResponse.NCDFreeYears;
            driver.NCDReference = najmResponse.NCDReference;
            return driver;
        }


        private List<YakeenMissingFieldBase> GetYakeenMissingFields(ICollection<ValidationResult> vResults, QuotationRequestRequiredFieldsModel model)
        {
            var result = new List<YakeenMissingFieldBase>();
            var tempResult = new List<string>();

            foreach (var validationResult in vResults)
            {
                var propertyName = validationResult.MemberNames.FirstOrDefault();
                tempResult.Add(propertyName);



            }


            foreach (var validationResult in vResults)
            {
                var propertyName = validationResult.MemberNames.FirstOrDefault();
                if (propertyName != null)
                {
                    if (propertyName == "VehicleModelCode" && (!tempResult.Contains("VehicleModel")))
                    {
                        propertyName = "VehicleModel";
                    }
                    if (propertyName == "VehicleMakerCode" && (!tempResult.Contains("VehicleMaker")))
                    {
                        propertyName = "VehicleMaker";
                    }

                    if (propertyName == "VehicleModelCode" && tempResult.Contains("VehicleModel"))
                    {
                        continue;
                    }
                    if (propertyName == "VehicleMakerCode" && tempResult.Contains("VehicleMaker"))
                    {
                        continue;
                    }
                    if ((propertyName == "VehicleMakerCode" || propertyName == "VehicleMaker") && !(tempResult.Contains("VehicleModel") || tempResult.Contains("VehicleModelCode")))
                    {
                        var propertyNameTemp = "VehicleModel";
                        var propertyInfotemp = typeof(QuotationRequestRequiredFieldsModel).GetProperty(propertyNameTemp);
                        var fieldDetailAttributetemp = propertyInfotemp.GetCustomAttributes(typeof(FieldDetailAttribute), true).FirstOrDefault() as FieldDetailAttribute;
                        if (fieldDetailAttributetemp != null)
                        {
                            YakeenMissingFieldBase yakeenField = CreateYakeenField(propertyNameTemp, fieldDetailAttributetemp, model);
                            result.Add(yakeenField);
                        }

                    }

                    var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(propertyName);
                    var fieldDetailAttribute = propertyInfo.GetCustomAttributes(typeof(FieldDetailAttribute), true).FirstOrDefault() as FieldDetailAttribute;
                    if (fieldDetailAttribute != null)
                    {
                        YakeenMissingFieldBase yakeenField = CreateYakeenField(propertyName, fieldDetailAttribute, model);
                        result.Add(yakeenField);
                    }
                }
            }

            result = ReArrangeMissingFeild(result);

            return result;


        }

        private YakeenMissingFieldBase CreateYakeenField(string propertyName, FieldDetailAttribute fieldDetailAttribute, QuotationRequestRequiredFieldsModel model)
        {
            YakeenMissingFieldBase yakeenField = null;
            switch (fieldDetailAttribute.ControlType)
            {
                case Enums.ControlType.Dropdown:
                    yakeenField = new DropdownField
                    {
                        Key = propertyName,
                        Label = propertyName,
                        Options = GetYakeenFieldDataSourceByName(fieldDetailAttribute.DataSourceName, model),
                        Required = IsYakeenFieldRequired(propertyName)
                    };
                    break;
                case Enums.ControlType.DatePicker:
                    yakeenField = new DatePickerField
                    {
                        Key = propertyName,
                        Label = propertyName,
                        Required = IsYakeenFieldRequired(propertyName)
                    };
                    break;
                case Enums.ControlType.Toggle:
                case Enums.ControlType.Textbox:
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

        private List<IdNamePairModel> GetYakeenFieldDataSourceByName(string dataSourceName, QuotationRequestRequiredFieldsModel model)
        {
            var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(dataSourceName);
            var value = propertyInfo.GetValue(model) as List<IdNamePairModel>;
            if (value == null || value.Count == 0)
            {
                switch (dataSourceName)
                {
                    case "Cities":
                        value = _addressService.GetCities().Select(e => new IdNamePairModel()
                        {
                            Id = Convert.ToInt32(e.Code),
                            Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                        }
                        ).ToList();

                        break;
                    case "SocialStatus":
                        value = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<SocialStatus>().Select(e => e.ToModel()).ToList();
                        break;
                    case "Occupations":
                        value = _occupationService.GetOccupations().Select(e => new IdNamePairModel { Id = e.ID, Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.NameAr : e.NameEn }).ToList();
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
                        value = _vehicleService.GetVehiclePlateTypes()
                                .Select(e => new IdNamePairModel { Id = e.Code, Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription })
                                .ToList();
                        break;
                    case "VehicleBodyTypes":
                        value = _vehicleService.VehicleBodyTypes()
                                .Select(e => new IdNamePairModel { Id = int.Parse(e.YakeenCode), Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription })
                                .ToList();
                        break;
                    case "VehicleMakers":
                        value = _vehicleService.VehicleMakers()
                             .Select(e => new IdNamePairModel
                             {
                                 Id = e.Code,
                                 Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                             })
                                .ToList();
                        break;
                    case "VehicleLoads":
                        for (int i = 1; i <= 10; i++)
                        {
                            value.Add(new IdNamePairModel { Id = i, Name = i.ToString() });
                        }
                        break;
                    case "VehicleModels":
                        if (model.VehicleMakerCode.HasValue)
                        {
                            value = _vehicleService.VehicleModels(model.VehicleMakerCode.Value).Select(e => new IdNamePairModel
                            {
                                Id = Convert.ToInt32(e.Code),
                                Name = _webApiContext.CurrentLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar ? e.ArabicDescription : e.EnglishDescription
                            }).ToList();
                        }

                        break;
                    default:
                        break;
                }
            }
            return value;
        }

        private bool IsYakeenFieldRequired(string propertyName)
        {
            var propertyInfo = typeof(QuotationRequestRequiredFieldsModel).GetProperty(propertyName);
            return Attribute.IsDefined(propertyInfo, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute));
        }
        private IEnumerable<string> GetYakeenMissingPropertiesName(QuotationRequestRequiredFieldsModel quotationRequestRequiredFieldsModel)
        {
            if (quotationRequestRequiredFieldsModel.VehicleModel == "غير متوفر" || string.IsNullOrEmpty(quotationRequestRequiredFieldsModel.VehicleModel))
            {
                quotationRequestRequiredFieldsModel.VehicleModel = null;
            }

            if (quotationRequestRequiredFieldsModel.VehicleMaker == "غير متوفر" || string.IsNullOrEmpty(quotationRequestRequiredFieldsModel.VehicleMaker))
            {
                quotationRequestRequiredFieldsModel.VehicleMaker = null;
            }

            if (quotationRequestRequiredFieldsModel.VehicleModelCode == null || quotationRequestRequiredFieldsModel.VehicleModelCode.Value < 1)
            {
                quotationRequestRequiredFieldsModel.VehicleModelCode = null;
            }

            if (quotationRequestRequiredFieldsModel.VehicleMakerCode == null || quotationRequestRequiredFieldsModel.VehicleMakerCode.Value < 1)
            {
                quotationRequestRequiredFieldsModel.VehicleMakerCode = null;
            }
            if (quotationRequestRequiredFieldsModel.VehicleBodyCode == null || quotationRequestRequiredFieldsModel.VehicleBodyCode.Value < 1|| quotationRequestRequiredFieldsModel.VehicleBodyCode.Value > 21)
            {
                quotationRequestRequiredFieldsModel.VehicleBodyCode = null;
            }
            if (quotationRequestRequiredFieldsModel.VehicleLoad == null || quotationRequestRequiredFieldsModel.VehicleLoad.Value < 1 )
            {
                quotationRequestRequiredFieldsModel.VehicleLoad = null;
            }
            ValidationContext vc = new ValidationContext(quotationRequestRequiredFieldsModel);
            ICollection<ValidationResult> vResults = new List<ValidationResult>();
            //validate the model
            bool isValid = Validator.TryValidateObject(quotationRequestRequiredFieldsModel, vc, vResults, true);
            return vResults.Select(x => x.MemberNames.FirstOrDefault());

        }

        /// <summary>
        /// validate that all yakeen missing info in the db are set by the user model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="yakeenMissingFieldsNamesInDb"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Update Quotation Request Required Fields with the missing values entered by the user
        /// </summary>
        /// <param name="model">Model with values from db</param>
        /// <param name="userData">Model with values user entered</param>
        /// <param name="missingPropertiesNames">Missing properties names in dbs</param>
        private void UpdateQuotationRequestRequiredFieldsModel(QuotationRequestRequiredFieldsModel model, QuotationRequestRequiredFieldsModel userData, IEnumerable<string> missingPropertiesNames)
        {
            foreach (var propertyName in missingPropertiesNames)
            {
                var userValue = userData.GetType().GetProperty(propertyName).GetValue(userData);
                model.GetType().GetProperty(propertyName).SetValue(model, userValue);
            }
            HandleVehcileMakerMissingField(missingPropertiesNames, model, userData);
            HandleVehcileModelMissingField(missingPropertiesNames, model, userData);
           // HandleVehcileVehicleBodyCodeMissingField(missingPropertiesNames, model, userData);
        }

        private void HandleVehcileModelMissingField(IEnumerable<string> missingPropertiesNames, QuotationRequestRequiredFieldsModel model, QuotationRequestRequiredFieldsModel userSelectedData)
        {
            var vehicleModel = "VehicleModel";
            var vehicleModelCode = "VehicleModelCode";
            if (missingPropertiesNames.Contains(vehicleModel))
            {
                var userValue = userSelectedData.GetType().GetProperty(vehicleModel).GetValue(userSelectedData);
                int.TryParse(userValue as string, out int modelCode);
                var modelName = _vehicleService.GetModelName(modelCode, short.Parse(model.VehicleMakerCode.Value.ToString()), "");
                model.GetType().GetProperty(vehicleModel).SetValue(model, modelName);
            }

            else if (missingPropertiesNames.Contains(vehicleModelCode))
            {
                var userValue = userSelectedData.GetType().GetProperty(vehicleModelCode).GetValue(userSelectedData);
                int.TryParse(userValue.ToString(), out int modelCode);
                var modelName = _vehicleService.GetModelName(modelCode, short.Parse(model.VehicleMakerCode.Value.ToString()), "");
                model.GetType().GetProperty(vehicleModel).SetValue(model, modelName);
            }
        }

        private void HandleVehcileMakerMissingField(IEnumerable<string> missingPropertiesNames, QuotationRequestRequiredFieldsModel model, QuotationRequestRequiredFieldsModel userSelectedData)
        {
            var propertyName = "VehicleMaker";
            if (missingPropertiesNames.Contains(propertyName))
            {
                var userValue = userSelectedData.GetType().GetProperty(propertyName).GetValue(userSelectedData);
                int.TryParse(userValue.ToString(), out int makerCode);
                var makerName = _vehicleService.GetMakerName(makerCode, "");
                model.GetType().GetProperty(propertyName).SetValue(model, makerName);
            }
        }
        private void HandleVehcileVehicleBodyCodeMissingField(IEnumerable<string> missingPropertiesNames, QuotationRequestRequiredFieldsModel model, QuotationRequestRequiredFieldsModel userSelectedData)
        {
            var propertyName = "VehicleBodyCode";
            if (missingPropertiesNames.Contains(propertyName))
            {
                var userValue = userSelectedData.GetType().GetProperty(propertyName).GetValue(userSelectedData);
                int.TryParse(userValue as string, out int modelCode);
                var YakeenCode = _vehicleService.GetVehicleBodyType(modelCode).YakeenCode;
                model.GetType().GetProperty(propertyName).SetValue(model, YakeenCode);
            }
        }
        private void HandleVehcileModelCodeMissingField(IEnumerable<string> missingPropertiesNames, QuotationRequestRequiredFieldsModel model, QuotationRequestRequiredFieldsModel userSelectedData)
        {
            var propertyName = "VehicleModelCode";
            if (missingPropertiesNames.Contains(propertyName))
            {
                var userValue = userSelectedData.GetType().GetProperty(propertyName).GetValue(userSelectedData);
                int.TryParse(userValue as string, out int modelCode);
                var modelName = _vehicleService.GetModelName(modelCode, short.Parse(model.VehicleMakerCode.Value.ToString()), "");
                model.GetType().GetProperty(propertyName).SetValue(model, modelName);
            }
        }
       

        private void SetProperty(string compoundProperty, object target, object value)
        {
            string[] bits = compoundProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }
            PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
            propertyToSet.SetValue(target, value, null);
        }


        private Vehicle InitVehicleRequest(InitInquiryRequestModel requestModel)
        {
            if (requestModel.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
            {
                return _vehicleService.GetVehicleInfoBySequnceNumber(requestModel.SequenceNumber, requestModel.OwnerTransfer ? requestModel.OwnerNationalId : requestModel.NationalId);
            }
            else if (requestModel.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
            {
                return _vehicleService.GetVehicleInfoByCustomCardNumber(requestModel.SequenceNumber);
            }
            return null;
        }

        private Driver InitDriverRequest(InitInquiryRequestModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.NationalId))
                throw new TameenkArgumentNullException("NationalId", "National Id can not be null or empty.");

            return _driverRepository.Table.OrderByDescending(d => d.CreatedDateTime).Include(e => e.DriverViolations)
                .FirstOrDefault(d => d.NIN == requestModel.NationalId && !d.IsDeleted);
        }

        private bool ValidateUserPromotionProgram(string userId, QuotationRequest quotationRequest)
        {
            if (string.IsNullOrWhiteSpace(userId) || quotationRequest == null)
                return true;

            if (quotationRequest.UserId != userId)
                return false;
            else
            {
                var progUser = _promotionProgramUserRepository.Table.FirstOrDefault(e => e.UserId == userId);
                if (progUser != null && userId != quotationRequest.UserId)
                {
                    return false;
                }
                return true;
            }
        }

        private void LogInquiry(InquiryOutputModel model)
        {
            try
            {
                InquiryRequestLog inquiryRequestLog = new InquiryRequestLog()
                {
                    CreatedDate = DateTime.Now,
                    UserIP = HttpContext.Current.Request.UserHostAddress,
                    UserAgent = HttpContext.Current.Request.UserAgent,
                    //StatusCode = model.StatusCode,
                    //StatusDescription = model.Description,
                    //CalledUrl = HttpContext.Current.Request.Url.OriginalString,
                    MethodName = model.MethodName,

                };
                InquiryRequestLogDataAccess.AddInquiryRequestLog(inquiryRequestLog);

            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message);
            }
        }

        private InquiryOutputModel SetInquiryOutput(int statusCode, string description, string methodName, InquiryOutputModel inquiryOutputModel)
        {
            inquiryOutputModel.StatusCode = statusCode;
            inquiryOutputModel.Description = description;
            inquiryOutputModel.MethodName = methodName;
            LogInquiry(inquiryOutputModel);
            return inquiryOutputModel;
        }
        private void HandleNajmException(Exception ex, ref InquiryOutputModel inquiryOutput)
        {
            inquiryOutput.StatusCode = (int)HttpStatusCode.BadRequest;

            switch (ex.Message)
            {
                case "E101":
                    inquiryOutput.Description = NajmExceptionResource.E101;
                    break;

                case "E102":
                    inquiryOutput.Description = NajmExceptionResource.E102;
                    break;

                case "E103":
                    inquiryOutput.Description = NajmExceptionResource.E103;
                    break;

                case "E104":
                    inquiryOutput.Description = NajmExceptionResource.E104;
                    break;

                case "E105":
                    inquiryOutput.Description = NajmExceptionResource.E105;
                    break;

                case "E106":
                    inquiryOutput.Description = NajmExceptionResource.E106;
                    break;
                case "E107":
                    inquiryOutput.Description = NajmExceptionResource.E107;
                    break;
                case "E108":
                    inquiryOutput.Description = NajmExceptionResource.E108;
                    break;
                case "E109":
                    inquiryOutput.Description = NajmExceptionResource.E109;
                    break;
                case "E110":
                    inquiryOutput.Description = NajmExceptionResource.E110;
                    break;
                case "E111":
                    inquiryOutput.Description = NajmExceptionResource.E111;
                    break;
                case "E112":
                    inquiryOutput.Description = NajmExceptionResource.E112;
                    break;
                case "E113":
                    inquiryOutput.Description = NajmExceptionResource.E113;
                    break;
                default:
                    inquiryOutput.Description = NajmExceptionResource.GeneralException;
                    break;


            }


        }



        public InitInquiryResponseModel InitInquiryRequestNew(InitInquiryRequestModel requestModel)
        {
            InitInquiryResponseModel result = new InitInquiryResponseModel();
            result.InquiryOutputModel = new InquiryOutputModel();
            if (requestModel == null)
            {
                result.InquiryOutputModel = SetInquiryOutput((int)HttpStatusCode.BadRequest, SubmitInquiryResource.RequestModelIsNullException, "InitInquiryRequestNew", result.InquiryOutputModel);
                return result;
            }
            try
            {
                _logger.Log($"QuotationRequestService -> InitQuotationRequest started.");
                result.PolicyEffectiveDate = requestModel.PolicyEffectiveDate;
                var vehicle = InitVehicleRequest(requestModel);
                result.IsCustomerCurrentOwner = !requestModel.OwnerTransfer;
                if (requestModel.OwnerTransfer)
                    result.OldOwnerNin = long.Parse(requestModel.OwnerNationalId);
                if (vehicle == null)
                {
                    result.IsVehicleExist = false;
                }
                else
                {
                    //add vehicle info into the model
                    result.Vehicle = vehicle.ToModel();
                    result.Vehicle.ApproximateValue = vehicle.VehicleValue ?? 30000;
                    //mark the vehcile as exist
                    result.IsVehicleExist = true;
                    result.IsVehicleUsedCommercially = vehicle.IsUsedCommercially ?? false;

                }

                var driver = InitDriverRequest(requestModel);
                if (driver == null)
                {
                    result.IsMainDriverExist = false;
                }
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
                        MedicalConditionId = driver.MedicalConditionId ?? 1
                    };

                    if (driver.DriverViolations != null && driver.DriverViolations.Any())
                        mainDriver.ViolationIds = driver.DriverViolations.Select(e => e.ViolationId).ToList();

                    result.IsCustomerSpecialNeed = driver.IsSpecialNeed;
                    var insured = _insuredRepository.Table.FirstOrDefault(e => e.NationalId == requestModel.NationalId);
                    if (insured != null)
                    {
                        result.CityCode = insured.CityId ?? 1;
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

            }
            catch (Exception ex)
            {

                _logger.Log("QuotationRequestService->InitQuotationRequest An error happen while initiating inquery request", ex);
                result.InquiryOutputModel = SetInquiryOutput((int)HttpStatusCode.BadRequest, WebResources.SerivceIsCurrentlyDown, "InitInquiryRequestNew", result.InquiryOutputModel);
                result.Errors = new List<ErrorModel> { new ErrorModel(ex.GetBaseException().Message) };
            }

            return result;
        }


        private List<YakeenMissingFieldBase> ReArrangeMissingFeild(List<YakeenMissingFieldBase> yakeenMissingFields)
        {
            List<YakeenMissingFieldBase> result = new List<YakeenMissingFieldBase>();
            foreach (YakeenMissingFieldBase missingField in yakeenMissingFields)
            {
                if (missingField.Key == "VehicleModel" && yakeenMissingFields.Any(a => a.Key == "VehicleMaker"))
                {
                    var makerMissingField = yakeenMissingFields.FirstOrDefault(x => x.Key == "VehicleMaker");
                    result.Add(makerMissingField);
                }
                if (result.Any(a => a.Key == missingField.Key))
                {
                    continue;
                }

                result.Add(missingField);

            }

            return result;
        }


        #endregion
    }
}