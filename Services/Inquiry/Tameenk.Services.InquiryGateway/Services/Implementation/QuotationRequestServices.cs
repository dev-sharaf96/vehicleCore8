using Flurl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Repository;
using Tameenk.Services.InquiryGateway.Services.Core;
using Tameenk.Services.InquiryGateway.Services.Exceptions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.InquiryGateway.Services.Implementation
{
    public class QuotationRequestServices : IQuotationRequestServices
    {
        private readonly INajmService _najmService;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IHttpClient _httpClient;
        private readonly string _yakeenServiceUrl;
        private readonly IAddressService _addressService;
        private readonly ILogger _logger;
        private readonly IVehicleService _vehicleService;

        /// <summary>
        /// The Constructor
        /// </summary>
        /// <param name="najmService">Najm Service</param>
        /// <param name="quotationRequestRepository">quotation Request Repository</param>
        /// <param name="driverRepository">driver Repository</param>
        /// <param name="httpClient">http Client</param>
        /// <param name="addressService">address service</param>
        /// <param name="logger">logger</param>
        /// <param name="vehicleService">vehicle service</param>
        public QuotationRequestServices(INajmService najmService, IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<Driver> driverRepository,
            IHttpClient httpClient,
            IAddressService addressService,
            ILogger logger, IVehicleService vehicleService)
        {
            _najmService = najmService ?? throw new TameenkArgumentNullException(nameof(INajmService));
            _quotationRequestRepository = quotationRequestRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<QuotationRequest>));
            _driverRepository = driverRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Driver>));
            _httpClient = httpClient ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
            _yakeenServiceUrl = ConfigurationManager.AppSettings["YakeenIntegrationApiUrl"];
            _addressService = addressService ?? throw new TameenkArgumentNullException(nameof(IAddressService));
            _logger = logger;
            _vehicleService = vehicleService ?? throw new TameenkArgumentNullException(nameof(IVehicleService));
        }





        public InquiryResponseModel HandleQuotationRequest(InquiryRequestModel requestModel, string userId = null)
        {
            if (requestModel == null)
                throw new RequestModelIsNullException();

            InquiryResponseModel result = new InquiryResponseModel();
            try
            {
                QuotationRequest quotationRequest = GetQuotationRequestByDetails(
                        requestModel.Vehicle.VehicleId.ToString(),
                        requestModel.Insured.NationalId,
                        requestModel.Vehicle.ApproximateValue,
                        requestModel.Drivers == null ? null : requestModel.Drivers.Select(d => d.NationalId));

                if (quotationRequest != null)
                {
                    result.QuotationRequestId = quotationRequest.ExternalId;
                    return result;
                }
                var najmResponse = _najmService.GetNajm(new NajmRequest()
                {
                    IsVehicleRegistered = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                    PolicyHolderNin = long.Parse(requestModel.Insured.NationalId),
                    VehicleId = requestModel.Vehicle.VehicleId
                });
                if (najmResponse == null)
                {
                    _logger.Log($"Inguiry Api -> QuotationRequestServices -> HandleQuotationRequest -> Najm response is null (request : {JsonConvert.SerializeObject(requestModel)}, user id : {userId})", LogLevel.Warning);
                    result.Errors.Add("Najm service does not return response.");
                    return result;
                }
                if (!string.IsNullOrEmpty(najmResponse.ErrorMsg))
                {
                    result.Errors.Add(najmResponse.ErrorMsg);
                    return result;
                }
                var customerYakeenInfo = GetCustomerYakeenInfo(requestModel);
                if (!customerYakeenInfo.Success)
                {
                    if (customerYakeenInfo.Error != null && customerYakeenInfo.Error.ErrorMessage != null)
                        result.Errors.Add(customerYakeenInfo.Error.ErrorMessage);
                    else
                        result.Errors.Add(string.Empty);
                    return result;
                }

                var vehicleYakeenInfo = GetVehicleYakeenInfo(requestModel);
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
                    additionalDriversYakeenInfo = GetAdditionalDriversYakeenInfo(requestModel);
                else
                    additionalDriversYakeenInfo = new List<DriverYakeenInfoModel>();
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

                var insured = new Insured
                {
                    BirthDate = customerYakeenInfo.DateOfBirthG,
                    BirthDateH = customerYakeenInfo.DateOfBirthH,
                    NationalId = customerYakeenInfo.NIN,
                    CardIdTypeId = customerYakeenInfo.IsCitizen ? 1 : 2,
                    Gender = customerYakeenInfo.Gender,
                    NationalityCode = customerYakeenInfo.NationalityCode.GetValueOrDefault().ToString(),
                    FirstNameAr = customerYakeenInfo.FirstName,
                    MiddleNameAr = customerYakeenInfo.SecondName,
                    LastNameAr = $"{customerYakeenInfo.ThirdName} {customerYakeenInfo.LastName}",
                    FirstNameEn = customerYakeenInfo.EnglishFirstName,
                    MiddleNameEn = customerYakeenInfo.EnglishSecondName,
                    LastNameEn = $"{customerYakeenInfo.EnglishThirdName} {customerYakeenInfo.EnglishLastName}",
                    CityId = requestModel.CityCode,
                    WorkCityId = requestModel.CityCode,
                    ChildrenBelow16Years = requestModel.Insured.ChildrenBelow16Years
                    //  ChildrenBelow16Years = requestModel.Drivers.Count()==1 ? mainDriver.ChildrenBelow16Years : requestModel.Insured.ChildrenBelow16Years
                };
                insured.IdIssueCityId = _addressService.GetCityByArabicName(customerYakeenInfo.IdIssuePlace).Code;

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
                    CreatedDateTimeUtc = DateTime.UtcNow,
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
                        var driver = GetDriverNajmResponse(dbDriver, requestModel.Vehicle.VehicleId, requestModel.Vehicle.VehicleIdTypeId);
                        quotationRequest.Drivers.Add(driver);
                    }
                }

                _quotationRequestRepository.Insert(quotationRequest);

                result.QuotationRequestId = qtRqstExtrnlId;
                result.Vehicle = ConvertVehicleYakeenToVehicle(vehicleYakeenInfo);
                result.NajmNcdFreeYears = quotationRequest.Driver.NCDFreeYears;
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

        #region Private Methods

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
            vehicleModel.VehicleMakerCode = vehicle.VehicleMakerCode.ToString();
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
            vehicleModel.VehicleMakerCode = vehicleYakeenModel.MakerCode.ToString();
            vehicleModel.VehicleModelYear = vehicleYakeenModel.ModelYear;

            return vehicleModel;
        }


        private string GetNewRequestExternalId()
        {
            string qtExtrnlId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (_quotationRequestRepository.Table.Any(q => q.ExternalId == qtExtrnlId))
                return GetNewRequestExternalId();

            return qtExtrnlId;
        }

        private QuotationRequest GetQuotationRequestByDetails(string vehicleId, string mainDriverNin, decimal vehicleValue, IEnumerable<string> additionalDriversNins)
        {
            // Get only the valid request the are not expired within 16 hours window.
            var benchmarkDate = DateTime.UtcNow.AddHours(-16);

            var query = _quotationRequestRepository.Table.Where(q =>
                            q.CreatedDateTimeUtc > benchmarkDate &&
                            q.Vehicle != null &&
                            (
                                (q.Vehicle.CustomCardNumber == vehicleId && q.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber) ||
                                (q.Vehicle.SequenceNumber == vehicleId && q.Vehicle.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                            ) &&
                            q.Vehicle.VehicleValue.HasValue && q.Vehicle.VehicleValue == vehicleValue &&
                            q.Driver != null && q.Driver.NIN == mainDriverNin);

            if (additionalDriversNins == null || !additionalDriversNins.Any())
                return query.FirstOrDefault(q => !q.Drivers.Any());

            var quotationRequests = query.AsEnumerable();
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

        private CustomerYakeenInfoModel GetCustomerYakeenInfo(InquiryRequestModel requestModel)
        {
            var customerInfoRequest = new CustomerYakeenInfoRequestModel()
            {
                Nin = long.Parse(requestModel.Insured.NationalId),
                BirthMonth = requestModel.Insured.BirthDateMonth,
                BirthYear = requestModel.Insured.BirthDateYear,
                IsSpecialNeed = requestModel.IsCustomerSpecialNeed.HasValue
                           ? requestModel.IsCustomerSpecialNeed.Value : false,
            };
            var mainDriver = requestModel.Drivers.FirstOrDefault(e => e.NationalId == requestModel.Insured.NationalId);
            if (mainDriver != null)
            {
                customerInfoRequest.MedicalConditionId = mainDriver.MedicalConditionId;
                customerInfoRequest.EducationId = mainDriver.EducationId;
                customerInfoRequest.ChildrenBelow16Years = mainDriver.ChildrenBelow16Years;
                customerInfoRequest.DrivingPercentage = mainDriver.DrivingPercentage;
            }
            var queryUrl = _yakeenServiceUrl
                .AppendPathSegment(RepositoryConstants.YakeenCustomerEndpoint)
                .SetQueryParams(customerInfoRequest);
            var customerDataAsString = _httpClient.GetStringAsync(queryUrl, true).Result;
            return JsonConvert.DeserializeObject<CustomerYakeenInfoModel>(customerDataAsString);
        }

        private IEnumerable<DriverYakeenInfoModel> GetAdditionalDriversYakeenInfo(InquiryRequestModel requestModel)
        {
            var result = new List<DriverYakeenInfoModel>();
            foreach (var d in requestModel.Drivers)
            {
                var driverInfoRequest = new DriverYakeenInfoRequestModel()
                {
                    Nin = long.Parse(d.NationalId),
                    LicenseExpiryMonth = d.LicenseExpiryMonth,
                    LicenseExpiryYear = d.LicenseExpiryYear
                };
                var queryUrl = _yakeenServiceUrl
                    .AppendPathSegment(RepositoryConstants.YakeenDriverEndpoint)
                    .SetQueryParams(driverInfoRequest);
                var driverDataAsString = _httpClient.GetStringAsync(queryUrl).Result;
                var driver = JsonConvert.DeserializeObject<DriverYakeenInfoModel>(driverDataAsString);
                result.Add(driver);
            }

            return result;
        }

        private VehicleYakeenModel GetVehicleYakeenInfo(InquiryRequestModel requestModel)
        {
            var vehicleInfoRequest = new VehicleInfoRequestModel()
            {
                VehicleId = requestModel.Vehicle.VehicleId,
                VehicleIdTypeId = requestModel.Vehicle.VehicleIdTypeId,
                OwnerNin = requestModel.IsCustomerCurrentOwner ? long.Parse(requestModel.Insured.NationalId) : requestModel.OldOwnerNin.Value,
                ModelYear = requestModel.Vehicle.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber ? requestModel.Vehicle.ManufactureYear : null,
                VehicleValue = requestModel.Vehicle.ApproximateValue,
                IsUsedCommercially = requestModel.IsVehicleUsedCommercially
            };
            var queryUrl = _yakeenServiceUrl
                .AppendPathSegment(RepositoryConstants.YakeenVehicleEndpoint)
                .SetQueryParams(vehicleInfoRequest);
            var vehicleDataAsString = _httpClient.GetStringAsync(queryUrl, true).Result;
            return JsonConvert.DeserializeObject<CommonResponseModel<Integration.Dto.Yakeen.VehicleYakeenModel>>(vehicleDataAsString).Data;
        }

        private Driver GetDriverNajmResponse(Driver driver, long vehicleId, int vehicleIdTypeId)
        {
            var najmResponse = _najmService.GetNajm(new NajmRequest()
            {
                IsVehicleRegistered = vehicleIdTypeId == (int)VehicleIdType.SequenceNumber,
                PolicyHolderNin = long.Parse(driver.NIN.ToString()),
                VehicleId = vehicleId
            });
            if (!string.IsNullOrWhiteSpace(najmResponse.ErrorMsg))
            {
                throw new NajmErrorException(najmResponse.ErrorMsg);
            }
            driver.NCDFreeYears = najmResponse.NCDFreeYears;
            driver.NCDReference = najmResponse.NCDReference;
            return driver;
        }

        #endregion  
    }
}