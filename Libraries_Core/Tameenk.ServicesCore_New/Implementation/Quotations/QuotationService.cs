using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
//using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Addresses;
//using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Common.Utilities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Core.Caching;
//using Tameenk.Core;
using Tameenk.Services.Core.BlockNins;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Implementation.Quotations
{

    public class QuotationService : IQuotationService
    {
        #region Fields
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<NCDFreeYear> _NCDFreeYearRepository;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IAddressService _addressService;
        private readonly TameenkConfig _config;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<PromotionProgramUser> _promotionProgramUSerRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<QuotationResponseCache> _quotationResponseCache;
        private readonly IRepository<InsuredExtraLicenses> _insuredExtraLicenses;
        private readonly IRepository<AutoleasingQuotationResponseCache> _autoleasingQuotationResponseCache;
        private readonly IRepository<WataniyaDraftPolicy> _wataniyaDraftPolicyRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IRepository<WataniyaMotorPolicyInfo> _wataniyaMotorPolicyInfoRepository;
        private readonly IRepository<QuotationShares> _quotationSharesRepository;
        private readonly ICacheManager _cacheManger;
        private readonly IRepository<QuotationBlockedNins> _quotationBlockedNins;

        #endregion

        #region Ctor

        public QuotationService(IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<NCDFreeYear> NCDFreeYearRepository,
            IRepository<Product> productRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IAddressService addressService,
            TameenkConfig config,
            IRepository<Driver> driverRepository,
            IRepository<PromotionProgramUser> promotionProgramUSerRepository,
            IVehicleService vehicleService, IRepository<QuotationResponseCache> quotationResponseCache,
             IRepository<InsuredExtraLicenses> insuredExtraLicenses,
           IRepository<AutoleasingQuotationResponseCache> autoleasequotationResponseCache,
           IRepository<WataniyaDraftPolicy> wataniyaDraftPolicyRepository, IRepository<LicenseType> licenseTypeRepository,
           IRepository<WataniyaMotorPolicyInfo> wataniyaMotorPolicyInfoRepository,
           IRepository<QuotationShares> quotationSharesRepository,
          IRepository<QuotationBlockedNins> quotationBlockedNins
           ,ICacheManager cacheManger)
        {
            _quotationResponseRepository = quotationResponseRepository;
            _quotationRequestRepository = quotationRequestRepository;
            _NCDFreeYearRepository = NCDFreeYearRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _productRepository = productRepository ?? throw new TameenkArgumentNullException(nameof(productRepository));
            _checkoutDetailRepository = checkoutDetailRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _addressService = addressService;
            _config = config;
            _driverRepository = driverRepository;
            _promotionProgramUSerRepository = promotionProgramUSerRepository;
            _vehicleService = vehicleService;
            _quotationResponseCache = quotationResponseCache;
            _insuredExtraLicenses = insuredExtraLicenses;
            _autoleasingQuotationResponseCache = autoleasequotationResponseCache;
            _wataniyaDraftPolicyRepository = wataniyaDraftPolicyRepository;
            _licenseTypeRepository = licenseTypeRepository;
            _wataniyaMotorPolicyInfoRepository = wataniyaMotorPolicyInfoRepository;
            _quotationSharesRepository = quotationSharesRepository;
            _quotationBlockedNins = quotationBlockedNins;
            _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(ICacheManager));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get quotation Request by external id 
        /// </summary>
        /// <param name="externalId">external Id</param>
        /// <returns></returns>
        public QuotationRequest GetQuotationRequest(string externalId)
        {

            var query = _quotationRequestRepository.Table.Include(q => q.Vehicle)
                  .Include(q => q.Driver.DriverViolations)
                  .Include(q => q.Insured)
                  .Include(q => q.Drivers.Select(e => e.DriverViolations))
                  .Include(q => q.Vehicle)
                  .FirstOrDefault(q => q.ExternalId == externalId);
            return query;
        }
        public QuotationRequest GetQuotationRequestByExternal(string externalId)
        {
            var query = _quotationRequestRepository.Table.Include(q => q.Vehicle)
                  .Include(q => q.Driver.DriverViolations)
                  .Include(q => q.Insured)
                  .Include(q => q.Insured.InsuredExtraLicenses)
                  .Include(q => q.Drivers.Select(e => e.DriverViolations))
                  .Include(q => q.Vehicle)
                  .FirstOrDefault(q => q.ExternalId == externalId);
            return query;
        }
        public RenewalPolicesData GetQuotationRequestByExternalNew(string externalId, out string exception)
        {

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                exception = string.Empty;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetRenewalPoliciesForQuotation";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;
                SqlParameter quotationExternalId = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(quotationExternalId);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalPolicesData>(reader).FirstOrDefault();
                if (info != null)
                {
                    return info;
                }
                return info;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        //public CheckoutDetail GetQuotationRequestByExternal(string externalId)
        //{

        //    var query = _checkoutDetailRepository.Table.Include(q => q.Vehicle)
        //          .Include(q => q.Driver.DriverViolations)
        //          .Include(q => q.Insured)
        //          .Include(q => q.Insured.InsuredExtraLicenses)
        //          .Include(q => q.Drivers.Select(e => e.DriverViolations))
        //          .Include(q => q.Vehicle)
        //          .FirstOrDefault(q => q.ExternalId == externalId);



        //    return query;
        //}

        public CheckoutDetail GetQuotationRequestByExternal(string externalId, out string exception)
        {
            exception = string.Empty;
          return  _checkoutDetailRepository.TableNoTracking.FirstOrDefault(a=>a.ExternalId==externalId);
            //IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            //try
            //{
            //    exception = string.Empty;
            //    dbContext.DatabaseInstance.CommandTimeout = 60;
            //    var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            //    command.CommandText = "GetRenewalPoliciesForQuotation";
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.CommandTimeout = 60;
            //    SqlParameter quotationExternalId = new SqlParameter() { ParameterName = "externalId", Value = externalId };
            //    command.Parameters.Add(quotationExternalId);
            //    dbContext.DatabaseInstance.Connection.Open();
            //    var reader = command.ExecuteReader();
            //    var info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
            //    if (info != null)
            //    {
            //        return info;
            //    }
            //    return info;
            //}
            //catch (Exception exp)
            //{
            //    exception = exp.ToString();
            //    return null;
            //}
            //finally
            //{
            //    if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
            //        dbContext.DatabaseInstance.Connection.Close();
            //}
        }
        /// <summary>
        /// get number of offers fro specific user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        public int GetUserOffersCount(string id)
        {
            return _quotationRequestRepository.Table.Where(x => x.UserId == id).ToList().Where(y => GivenDateWithin16Hours(y.CreatedDateTime)).Count();
        }

        private bool GivenDateWithin16Hours(DateTime givenDate)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        }

        /// <summary>
        /// Get quotation response by reference identifier.
        /// </summary>
        /// <param name="referenceId">The reference identifier.</param>
        /// <returns></returns>
        public QuotationResponse GetQuotationResponseByReferenceId(string referenceId)
        {
            return _quotationResponseRepository.TableNoTracking.Include(q => q.QuotationRequest)
                    .Include(q => q.QuotationRequest.Driver)
                    .Include(q => q.QuotationRequest.Drivers)
                    .Include(q => q.Products)
                    .Include(q => q.InsuranceCompany)
                    .Include(q => q.QuotationRequest.Insured)
                    .Include(q => q.QuotationRequest.Vehicle)
                    .FirstOrDefault(q => q.ReferenceId == referenceId);
        }
        public QuotationResponseDBModel GetQuotationResponseByReferenceIdDB(string ReferenceId, string ProductId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                QuotationResponseDBModel quotationResponseModel = null;
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationResponse";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter productIdParameter = new SqlParameter() { ParameterName = "ReferenceId", Value = ReferenceId };
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "productId", Value = ProductId };
                command.Parameters.Add(productIdParameter);
                command.Parameters.Add(referenceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                quotationResponseModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationResponseDBModel>(reader).FirstOrDefault();
                if (quotationResponseModel != null)
                {
                    reader.NextResult();
                    List<Guid> additionalDriverList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Guid>(reader).ToList();
                    quotationResponseModel.AdditionalDriverList = new List<Guid>();
                    quotationResponseModel.AdditionalDriverList.AddRange(additionalDriverList);
                }
                dbContext.DatabaseInstance.Connection.Close();


                return quotationResponseModel;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public QuotationRequest SetQuotationRequestUser(string qtRqstExtrnlId, string userId)
        {
            var quotationRequest = _quotationRequestRepository.Table.FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);

            if (quotationRequest == null)
            {
                throw new TameenkEntityNotFoundException($"The quotation request with external identifier {qtRqstExtrnlId} not found.");
            }
            if (string.IsNullOrEmpty(quotationRequest.UserId))
            {
                quotationRequest.UserId = userId;
                _quotationRequestRepository.Update(quotationRequest);
            }
            return quotationRequest;
        }


        public QuotationRequest GetQuotationRequestDrivers(string qtRqstExtrnlId)
        {
            var quotationRequest = _quotationRequestRepository.Table
                .Include(qr => qr.Driver.Addresses)
                .Include(qr => qr.Drivers.Select(d => d.Addresses))
                .Include(qr => qr.Insured.City.Region)
                .Include(qr => qr.Vehicle)
                .Include(qr => qr.QuotationResponses)
                .FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);

            if (quotationRequest == null)
            {
                throw new TameenkEntityNotFoundException($"The quotation request with external identifier {qtRqstExtrnlId} not found.");
            }

            return quotationRequest;
        }
        public QuotationRequest GetQuotationRequestDriversByExternalAndRef(string qtRqstExtrnlId, string referenceId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                QuotationRequest quotationRequest = null;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLowestProductByPrice";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "qtRqstExtrnlId", Value = qtRqstExtrnlId };
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "ReferenceId", Value = referenceId };

                command.Parameters.Add(externalIdParam);
                command.Parameters.Add(referenceIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                quotationRequest = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (quotationRequest != null)
                {
                    return quotationRequest;
                }
                return quotationRequest;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }

        }
        /// <summary>
        /// Pormotion From TPL To Comperhensive for specific external Id
        /// </summary>
        /// <param name="qtRqstExtrnlId"></param>
        /// <param name="vehicleAgencyRepair"></param>
        /// <param name="deductibleValue"></param>
        /// <param name="productTypeId">product type id</param>
        /// <returns></returns>
        public Product GetLowestProductByPrice(string externalId, bool vehicleAgencyRepair = false, int? deductibleValue = 2000, int productTypeId = 2)
        {
            //if (string.IsNullOrWhiteSpace(qtRqstExtrnlId))
            //    throw new TameenkArgumentNullException("Quotation external id is missing ", nameof(qtRqstExtrnlId));

            //DateTime lastAvailabletime = DateTime.Now.AddHours(-16);


            //return (from rqst in _quotationRequestRepository.Table
            //        join res in _quotationResponseRepository.Table
            //        on rqst.ID equals res.RequestId
            //        join pro in _productRepository.Table
            //        on res.Id equals pro.QuotationResponseId
            //        where res.VehicleAgencyRepair == vehicleAgencyRepair
            //        && res.DeductibleValue == deductibleValue
            //        && res.CreateDateTime > lastAvailabletime
            //        && rqst.ExternalId == qtRqstExtrnlId
            //        && res.InsuranceTypeCode == productTypeId
            //        && pro.ProductPrice > 0
            //        select pro).OrderBy(p => p.ProductPrice).FirstOrDefault();
            Product product = null;

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLowestProductByPrice";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "qtRqstExtrnlId", Value = externalId };
                SqlParameter vehicleAgencyRepairParam = new SqlParameter() { ParameterName = "vehicleAgencyRepair", Value = vehicleAgencyRepair };
                SqlParameter deductibleValueParam = new SqlParameter("deductibleValue", SqlDbType.Int);
                SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "productTypeId", Value = productTypeId };
                if (deductibleValue.HasValue)
                    deductibleValueParam.Value = deductibleValue.Value;
                else
                    deductibleValueParam.Value = (object)DBNull.Value;

                command.Parameters.Add(insuranceTypeCodeParam);
                command.Parameters.Add(externalIdParam);
                command.Parameters.Add(vehicleAgencyRepairParam);
                command.Parameters.Add(deductibleValueParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                product = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (product != null)
                {
                    return product;
                }
                return product;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public bool IsQuotationStillValid(string quotationExternalId)
        {
            if (string.IsNullOrWhiteSpace(quotationExternalId))
                throw new TameenkArgumentNullException(nameof(quotationExternalId), "Quoation external id can't be empty.");

            var quotRequest = _quotationRequestRepository.Table.SingleOrDefault(e => e.ExternalId == quotationExternalId);
            if (quotRequest == null)
                throw new TameenkEntityNotFoundException(nameof(quotationExternalId), "There is no quotation request with the given external id.");

            if (quotRequest.CreatedDateTime.GivenDateWithinGivenHours(16))
                return true;

            return false;
        }

        public string GetNCDFreeYearsDescription(int code, string lang)
        {
            return lang.ToUpper().StartsWith("EN") ? _NCDFreeYearRepository.Table.FirstOrDefault(x => x.Code == code).EnglishDescription : _NCDFreeYearRepository.Table.FirstOrDefault(x => x.Code == code).ArabicDescription;
        }


        public QuotationRequest UpdateQuotationRequest(QuotationRequest quotationRequest)
        {
            if (quotationRequest == null)
                throw new TameenkArgumentNullException("quotationRequest", "Can't update null quoutation request");

            _quotationRequestRepository.Update(quotationRequest);

            return quotationRequest;
        }

        public bool ValidateUniqueIBAN(string referenceId, string strIBAN,Guid mainDriverId)
        {
            strIBAN = strIBAN.ToLower().Trim();
            return !_checkoutDetailRepository.TableNoTracking.Any(x => x.IBAN.ToLower().Trim() == strIBAN && x.MainDriverId.HasValue && x.MainDriverId.Value != mainDriverId);
        }
        public QuotationServiceRequest GetQuotationRequestData(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue)
        {
            var serviceRequestMessage = new QuotationServiceRequest();
            string promotionProgramCode = string.Empty;
            int promotionProgramId = 0;
            //Random r = new Random();
            var cities = _addressService.GetAllCities();
            string vehicleColorCode = "99";
            string vehicleColor;
            //var sakakaDbCode = 9999;
            //var rightSakakaCode = 38;
            #region VehicleColor

            GetVehicleColor(out vehicleColor, out vehicleColorCode, quotationRequest.Vehicle.MajorColor);
            #endregion




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

            if (quotationRequest.Insured.Gender == Gender.Male)
                serviceRequestMessage.InsuredGenderCode = "M";
            else if (quotationRequest.Insured.Gender == Gender.Female)
                serviceRequestMessage.InsuredGenderCode = "F";
            else
                serviceRequestMessage.InsuredGenderCode = "N";

            //serviceRequestMessage.InsuredGenderCode = quotationRequest.Insured.Gender.GetCode();
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
            //as per mubark almutlkk request 
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredOccupationCode) && serviceRequestMessage.InsuredIdTypeCode == 1)
            {
                serviceRequestMessage.InsuredOccupationCode = "31010";
            }
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredOccupationCode) && serviceRequestMessage.InsuredIdTypeCode == 2)
            {
                serviceRequestMessage.InsuredOccupationCode = "o";
            }
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredOccupation) && serviceRequestMessage.InsuredIdTypeCode == 1)
            {
                serviceRequestMessage.InsuredOccupation = "31010";
            }
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredOccupation) && serviceRequestMessage.InsuredIdTypeCode == 2)
            {
                serviceRequestMessage.InsuredOccupation = "o";
            }
            serviceRequestMessage.InsuredEducationCode = int.Parse(quotationRequest.Insured.Education.GetCode());
            if (!serviceRequestMessage.InsuredEducationCode.HasValue || serviceRequestMessage.InsuredEducationCode == 0)
            {
                serviceRequestMessage.InsuredEducationCode = 1;
            }
            //end of mubark request
            serviceRequestMessage.InsuredChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
            //serviceRequestMessage.InsuredWorkCity = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.ArabicDescription : "";
            //serviceRequestMessage.InsuredWorkCityCode = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.YakeenCode.ToString() : "";
            serviceRequestMessage.InsuredIdIssuePlace = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.ArabicDescription : "";
            serviceRequestMessage.InsuredIdIssuePlaceCode = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.YakeenCode.ToString() : "";
            serviceRequestMessage.InsuredCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.ArabicDescription : "";
            serviceRequestMessage.InsuredCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.YakeenCode.ToString() : "";
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredIdIssuePlaceCode) && !string.IsNullOrEmpty(serviceRequestMessage.InsuredCityCode))
            {
                serviceRequestMessage.InsuredIdIssuePlaceCode = serviceRequestMessage.InsuredCityCode;
            }
            if (string.IsNullOrEmpty(serviceRequestMessage.InsuredIdIssuePlace) && !string.IsNullOrEmpty(serviceRequestMessage.InsuredCity))
            {
                serviceRequestMessage.InsuredIdIssuePlace = serviceRequestMessage.InsuredCity;
            }
            if (quotationRequest.Insured.WorkCityId.HasValue)
            {
                var city = cities.Where(c => c.Code == quotationRequest.Insured.WorkCityId.Value).FirstOrDefault();
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
            #endregion

            #region  Vehicle

            if (quotationRequest.Vehicle != null && !string.IsNullOrEmpty(quotationRequest.Vehicle.RegisterationPlace))
            {
                var info = _addressService.GetCityByName(cities, Utilities.RemoveWhiteSpaces(quotationRequest.Vehicle.RegisterationPlace));
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
            if (string.IsNullOrEmpty(serviceRequestMessage.VehicleRegPlace))//as per mubark almutlak
            {
                serviceRequestMessage.VehicleRegPlace = serviceRequestMessage.InsuredCity;
            }
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
            serviceRequestMessage.Drivers = CreateInsuranceCompanyDriversFromDataRequest(quotationRequest, cities);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var programcode = GetUserPromotionCode(userId, quotationResponse.InsuranceCompanyId, insuranceTypeCode);
                if (programcode != null)
                {
                    promotionProgramCode = programcode.Code;
                    promotionProgramId = programcode.PromotionProgramId;
                    serviceRequestMessage.PromoCode = programcode.Code;
                }
                else
                    serviceRequestMessage.PromoCode = null;
            }
            else
            {
                serviceRequestMessage.PromoCode = null;
            }
            serviceRequestMessage.VehicleChassisNumber = quotationRequest.Vehicle.ChassisNumber;

            return serviceRequestMessage;
        }


        private void GetVehicleColor(out string vehicleColor, out string vehicleColorCode, string vehicleMajorColor)
        {
            vehicleColor = vehicleMajorColor;
            vehicleColorCode = "99";
            var vehiclesColors = _vehicleService.GetVehicleColors();

            if (vehicleMajorColor == "رصاصي" || vehicleMajorColor == "رصاصي غامق")
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

        private List<DriverDto> CreateInsuranceCompanyDriversFromDataRequest(QuotationRequest quotationRequest, List<City> cities)
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
                DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears,
                DriverNCDReference = quotationRequest.NajmNcdRefrence
            };
            if (quotationRequest.Insured.Gender == Gender.Male)
                mainDriverDto.DriverGenderCode = "M";
            else if (quotationRequest.Insured.Gender == Gender.Female)
                mainDriverDto.DriverGenderCode = "F";
            else
                mainDriverDto.DriverGenderCode = "N";
            // mainDriverDto.DriverGenderCode = quotationRequest.Insured.Gender.GetCode();

            mainDriverDto.DriverNationalityCode = !string.IsNullOrEmpty(quotationRequest.Insured.NationalityCode) ? quotationRequest.Insured.NationalityCode : "113";
            mainDriverDto.DriverSocialStatusCode = quotationRequest.Insured.SocialStatusId?.ToString();
            mainDriverDto.DriverOccupationCode = quotationRequest.Insured.Occupation?.Code;
            mainDriverDto.DriverOccupation = quotationRequest.Insured.Occupation?.NameAr.Trim();

            if (string.IsNullOrEmpty(mainDriverDto.DriverOccupationCode) && mainDriverDto.DriverIdTypeCode == 1)
            {
                mainDriverDto.DriverOccupationCode = "31010";
            }
            if (string.IsNullOrEmpty(mainDriverDto.DriverOccupationCode) && mainDriverDto.DriverIdTypeCode == 2)
            {
                mainDriverDto.DriverOccupationCode = "o";
            }
            if (string.IsNullOrEmpty(mainDriverDto.DriverOccupation) && mainDriverDto.DriverIdTypeCode == 1)
            {
                mainDriverDto.DriverOccupation = "31010";
            }
            if (string.IsNullOrEmpty(mainDriverDto.DriverOccupation) && mainDriverDto.DriverIdTypeCode == 2)
            {
                mainDriverDto.DriverOccupation = "o";
            }
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
            if (!mainDriverDto.DriverEducationCode.HasValue || mainDriverDto.DriverEducationCode == 0)
            {
                mainDriverDto.DriverEducationCode = 1;
            }
            mainDriverDto.DriverMedicalConditionCode = quotationRequest.Driver.MedicalConditionId;
            mainDriverDto.DriverChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
            mainDriverDto.DriverHomeCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.YakeenCode.ToString() : "";
            mainDriverDto.DriverHomeCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.ArabicDescription : "";
            //mainDriverDto.DriverWorkCityCode = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.YakeenCode.ToString() : "";
            //mainDriverDto.DriverWorkCity = quotationRequest.Insured.WorkCity != null ? quotationRequest.Insured.WorkCity.ArabicDescription : "";
            if (quotationRequest.Insured.WorkCityId.HasValue)
            {
                var city = cities.Where(c => c.Code == quotationRequest.Insured.WorkCityId.Value).FirstOrDefault();
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
            // Get (Main & Additional Drivers Extra Licenses)
            var driversExtraLicenses = _insuredExtraLicenses.TableNoTracking
                .Where(d => d.InsuredId == quotationRequest.Insured.Id);

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
                    //driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                    if (additionalDriver.Gender == Gender.Male)
                        driverDto.DriverGenderCode = "M";
                    else if (additionalDriver.Gender == Gender.Female)
                        driverDto.DriverGenderCode = "F";
                    else
                        driverDto.DriverGenderCode = "N";

                    driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                            additionalDriver.NationalityCode.Value.ToString() : "113";
                    driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                    driverDto.DriverOccupation = additionalDriver.Occupation?.NameAr.Trim();
                    if (string.IsNullOrEmpty(driverDto.DriverOccupationCode) && driverDto.DriverIdTypeCode == 1)
                    {
                        driverDto.DriverOccupationCode = "31010";
                    }
                    if (string.IsNullOrEmpty(driverDto.DriverOccupationCode) && driverDto.DriverIdTypeCode == 2)
                    {
                        driverDto.DriverOccupationCode = "o";
                    }
                    if (string.IsNullOrEmpty(driverDto.DriverOccupation) && driverDto.DriverIdTypeCode == 1)
                    {
                        driverDto.DriverOccupation = "31010";
                    }
                    if (string.IsNullOrEmpty(driverDto.DriverOccupation) && driverDto.DriverIdTypeCode == 2)
                    {
                        driverDto.DriverOccupation = "o";
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
                    //driverDto.DriverHomeCityCode = additionalDriver.City?.YakeenCode.ToString();
                    //driverDto.DriverHomeCity = additionalDriver.City?.ArabicDescription;
                    //driverDto.DriverWorkCityCode = additionalDriver.WorkCity?.YakeenCode.ToString();
                    //driverDto.DriverWorkCity = additionalDriver.WorkCity?.ArabicDescription;
                    if (additionalDriver.CityId.HasValue)
                    {
                        var city = cities.Where(c => c.Code == additionalDriver.CityId.Value).FirstOrDefault();
                        if (city == null)
                        {
                            driverDto.DriverHomeCity = "";
                            driverDto.DriverHomeCityCode = "";
                        }
                        else
                        {
                            driverDto.DriverHomeCity = city.ArabicDescription;
                            driverDto.DriverHomeCityCode = city.YakeenCode.ToString();
                        }
                    }
                    else
                    {
                        driverDto.DriverHomeCity = "";
                        driverDto.DriverHomeCityCode = "";
                    }
                    if (additionalDriver.WorkCityId.HasValue)
                    {
                        var city = cities.Where(c => c.Code == additionalDriver.WorkCityId.Value).FirstOrDefault();
                        if (city == null)
                        {
                            driverDto.DriverWorkCity = city.ArabicDescription;
                            driverDto.DriverWorkCityCode = city.YakeenCode.ToString();
                        }
                        else
                        {
                            driverDto.DriverWorkCity = driverDto.DriverHomeCity;
                            driverDto.DriverWorkCityCode = driverDto.DriverHomeCityCode;
                        }
                    }
                    else
                    {
                        driverDto.DriverWorkCity = driverDto.DriverHomeCity;
                        driverDto.DriverWorkCityCode = driverDto.DriverHomeCityCode;
                    }

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
                                LicenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year)
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



        //private PromotionProgramCode GetUserPromotionCode(string userId, int insuranceCompanyId, int insuranceTypeCode)
        //{
        //    if (string.IsNullOrWhiteSpace(userId))
        //        throw new TameenkArgumentNullException(nameof(userId), "User id can't be empty.");
        //    if (insuranceCompanyId < 1)
        //        throw new TameenkArgumentNullException(nameof(insuranceCompanyId), "Insurance company id can't be less than 1.");

        //    var progUser = _promotionProgramUSerRepository.Table
        //        .Include(e => e.PromotionProgram.PromotionProgramCodes)
        //        .FirstOrDefault(e => e.UserId == userId && e.EmailVerified == true
        //                        && e.PromotionProgram.PromotionProgramCodes.Any(c => c.InsuranceCompanyId == insuranceCompanyId && c.InsuranceTypeCode == insuranceTypeCode));
        //    if (progUser == null) // || IdentityCustomExtensions.CheckUserAllowedGettingPromotionCodeAsync(userId, _promotionService, _authorizationService, _settingService).Result
        //        return null;

        //    return progUser.PromotionProgram.PromotionProgramCodes.FirstOrDefault(e => e.InsuranceCompanyId == insuranceCompanyId && e.InsuranceTypeCode == insuranceTypeCode);
        //}
        private PromotionProgramCode GetUserPromotionCode(string userId, int insuranceCompanyId, int insuranceTypeCode)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new TameenkArgumentNullException(nameof(userId), "User id can't be empty.");
                if (insuranceCompanyId < 1)
                    throw new TameenkArgumentNullException(nameof(insuranceCompanyId), "Insurance company id can't be less than 1.");
                PromotionProgramCode promotionProgramCode = null;
              
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserPromotionProgram";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };

                command.Parameters.Add(userIdParam);
                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(insuranceTypeCodeParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                promotionProgramCode = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramCode>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (promotionProgramCode != null)
                {
                    return promotionProgramCode;
                }
              
                return promotionProgramCode;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public void InvalidateUserQuotationResponses(string userId)
        {
            var _16HoursBeforeNow = DateTime.Now.AddHours(-16);
            var quotationResponses = _quotationResponseRepository.Table.Where(e => e.QuotationRequest.UserId == userId&& (_16HoursBeforeNow <e.CreateDateTime)).ToList();
            if (quotationResponses != null && quotationResponses.Count > 0)
            {
                foreach (var item in quotationResponses)
                {
                    item.CreateDateTime = DateTime.Now.AddDays(-5);
                    _quotationResponseRepository.Update(item);
                }
            }
        }
        public Guid GetDriverIdByReferenceId(string referenceId)
        {
            Guid driverId = Guid.Empty;
            var info= (from res  in _quotationResponseRepository.TableNoTracking
                    join rqst in _quotationRequestRepository.TableNoTracking
                    on res.RequestId equals rqst.ID
                    where res.ReferenceId == referenceId
                    select rqst.MainDriverId).FirstOrDefault();
            if (info != null)
                driverId = info;
            return driverId;
        }

        public QuotationDetailsModel GetQuotationDetailsByReferenceId(string referenceId)
        {
            //return _quotationResponseRepository.TableNoTracking.Include(q => q.QuotationRequest)
            //        .Include(q=>q.QuotationRequest.Insured)
            //        .Include(q => q.QuotationRequest.Driver)
            //        .Include(q=>q.InsuranceCompany)
            //        .FirstOrDefault(q => q.ReferenceId == referenceId);

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationDetailsByReferenceId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                QuotationDetailsModel quotationDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationDetailsModel>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (quotationDetails != null)
                {
                    return quotationDetails;
                }
                return null;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public bool InsertIntoQuotationResponseCache(QuotationResponseCache quotationResponseCache,out string exception)
        {
            try
            {
                exception = string.Empty;
                quotationResponseCache.CreateDateTime = DateTime.Now;
                _quotationResponseCache.Insert(quotationResponseCache);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        //public QuotationResponseCache GetFromQuotationResponseCache(int insuranceCompanyId,int insuranceTypeCode, string externalId, bool vehicleAgencyRepair,int? deductibleValue)
        //{
        //    var _16HoursBeforeNow = DateTime.Now.AddHours(-16);
        //    return _quotationResponseCache.TableNoTracking.Where(x => x.InsuranceCompanyId == insuranceCompanyId && x.ExternalId == externalId &&
        //                x.InsuranceTypeCode == insuranceTypeCode &&
        //                (
        //                    (x.VehicleAgencyRepair.HasValue && x.VehicleAgencyRepair.Value == vehicleAgencyRepair) ||
        //                    (!vehicleAgencyRepair && !x.VehicleAgencyRepair.HasValue)
        //                ) &&
        //                (
        //                    (!deductibleValue.HasValue && !x.DeductibleValue.HasValue) ||
        //                    (deductibleValue.HasValue && x.DeductibleValue.HasValue && x.DeductibleValue.Value == deductibleValue.Value)
        //                ))
        //                .FirstOrDefault(y => (_16HoursBeforeNow < y.CreateDateTime));
            
        //}
        public bool GetQuotationResponseCacheAndDelete(int insuranceCompanyId, int insuranceTypeCode, string externalId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var response = _quotationResponseCache.Table.Where(x => x.InsuranceCompanyId == insuranceCompanyId
                 && x.InsuranceTypeCode == insuranceTypeCode && x.ExternalId == externalId).FirstOrDefault();
                if (response != null)
                {
                    _quotationResponseCache.Delete(response);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public bool DeleteFromQuotationResponseCache(string externalId, out string exception)
        {
            try
            {
                exception = string.Empty;
                List<QuotationResponseCache> responses = _quotationResponseCache.Table.Where(x => x.ExternalId == externalId).ToList();
                if (responses != null)
                {
                    _quotationResponseCache.Delete(responses);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public QuotationResponseCache GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue,Guid userId)
        {
            QuotationResponseCache responseCache = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFromQuotationResponseCache";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                SqlParameter vehicleAgencyRepairParam = new SqlParameter() { ParameterName = "vehicleAgencyRepair", Value = vehicleAgencyRepair };
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter deductibleValueParam = new SqlParameter("deductibleValue", SqlDbType.Int);
                if (deductibleValue.HasValue)
                    deductibleValueParam.Value = deductibleValue.Value;
                else
                    deductibleValueParam.Value = (object)DBNull.Value;

                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(insuranceTypeCodeParam);
                command.Parameters.Add(externalIdParam);
                command.Parameters.Add(vehicleAgencyRepairParam);
                command.Parameters.Add(deductibleValueParam);
                command.Parameters.Add(userIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                responseCache = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationResponseCache>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (responseCache != null)
                {
                    return responseCache;
                }
                return responseCache;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public int UpdateQuotationResponseToBeCheckedout(long quotationResponseId,Guid productid)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateQuotationResponseToBeCheckedout";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter quotationResponseIdParam = new SqlParameter() { ParameterName = "quotationResponseId", Value = quotationResponseId };
                SqlParameter productidParam = new SqlParameter() { ParameterName = "productid", Value = productid };
                command.Parameters.Add(quotationResponseIdParam);
                command.Parameters.Add(productidParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }

        public bool IsQuotationResponseExist(string referenceId)
        {
            string value = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFromQuotationResponse";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "ReferenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                value = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<string>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (!string.IsNullOrEmpty(value))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public QuotationResponse GetInvoiceDataFromQuotationResponseByReferenceId(string referenceId)
        {
            return _quotationResponseRepository.TableNoTracking.Include(q => q.QuotationRequest)
                    .Include(q => q.InsuranceCompany)
                    .Include(q => q.QuotationRequest.Insured)
                     .Include(e => e.QuotationRequest.Vehicle)
                    .FirstOrDefault(q => q.ReferenceId == referenceId);
        }
        public QuotationResponse GetQuotationByReference(string referenceId)
        {
            return _quotationResponseRepository.TableNoTracking.Include(q => q.QuotationRequest)
                    .FirstOrDefault(q => q.ReferenceId == referenceId);
        }

        public QuotationResponseDBModel GetAutoleasingQuotationResponseByReferenceIdDB(string ReferenceId, string ProductId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                QuotationResponseDBModel quotationResponseModel = null;
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingQuotationResponse";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter productIdParameter = new SqlParameter() { ParameterName = "ReferenceId", Value = ReferenceId };
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "productId", Value = ProductId };
                command.Parameters.Add(productIdParameter);
                command.Parameters.Add(referenceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                quotationResponseModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationResponseDBModel>(reader).FirstOrDefault();
                if (quotationResponseModel != null)
                {
                    reader.NextResult();
                    List<Guid> additionalDriverList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Guid>(reader).ToList();
                    quotationResponseModel.AdditionalDriverList = new List<Guid>();
                    quotationResponseModel.AdditionalDriverList.AddRange(additionalDriverList);
                }
                dbContext.DatabaseInstance.Connection.Close();


                return quotationResponseModel;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public QuotationRequest GetQuotationRequestData(string externalId)
        {
            try
            {
                var query = _quotationRequestRepository.Table.FirstOrDefault(q => q.ExternalId == externalId);
                return query;
            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion

        #region Quotations Form Form Stored Procedures

        public QuotationInfoModel GetQuotationsDetails(string externalId, bool agencyRepair, int deductible, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationsDetails";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);

                //SqlParameter agencyRepairParam = new SqlParameter() { ParameterName = "agencyRepair", Value = (agencyRepair) ? 1 : 0 };
                //command.Parameters.Add(agencyRepairParam);

                SqlParameter deductibleParam = new SqlParameter() { ParameterName = "deductible", Value = deductible };
                command.Parameters.Add(deductibleParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationInfoModel>(reader).FirstOrDefault();

                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                reader.NextResult();
                data.Products = new List<QuotationProductInfoModel>();
                List<QuotationProductInfoModel> productsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationProductInfoModel>(reader).ToList();
                data.Products = productsData;

                //List<QuotationProductInfoModel> _productsDataIEnumerable = productsData.GroupBy(a => new { a.InsuranceCompanyID, a.VehicleRepairType, a.DeductableValue })
                //                                                                       .Select(a => a.FirstOrDefault()).ToList();
                //data.Products = _productsDataIEnumerable;

                reader.NextResult();
                List<ProductPriceDetailsInfoModel> PriceDetailsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductPriceDetailsInfoModel>(reader).ToList();

                reader.NextResult();
                List<ProductBenfitDetailsInfoModel> BenfitsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductBenfitDetailsInfoModel>(reader).ToList();

                if (data.Products != null && data.Products.Count > 0)
                {
                    foreach (var product in data.Products)
                    {
                        product.PriceDetails = new List<ProductPriceDetailsInfoModel>();
                        product.PriceDetails = PriceDetailsData.Where(a => a.ProductID == product.ProductID).ToList();

                        product.Benfits = new List<ProductBenfitDetailsInfoModel>();
                        product.Benfits = BenfitsData.Where(a => a.ProductId == product.ProductID).ToList();
                    }
                }

                reader.NextResult();
                BankData bankData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BankData>(reader).FirstOrDefault();
                data.Bank = bankData;

                reader.NextResult();
                DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();
                data.DepreciationSettingHistory = depreciationSettingHistoryData;

                reader.NextResult();
                DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();
                data.DepreciationSetting = depreciationSettingData;

                reader.NextResult();
                RepairMethodeSettingHistory repairMethodeSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSettingHistory>(reader).FirstOrDefault();
                data.RepairMethodeSettingHistory = repairMethodeSettingHistoryData;

                reader.NextResult();
                RepairMethodeSetting repairMethodeSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSetting>(reader).FirstOrDefault();
                data.RepairMethodeSetting = repairMethodeSettingData;

                reader.NextResult();
                MinimumPremiumSettingHistory minimumPremiumSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSettingHistory>(reader).FirstOrDefault();
                data.MinimumPremiumSettingHistory = minimumPremiumSettingHistoryData;

                reader.NextResult();
                MinimumPremiumSetting MinimumPremiumSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSetting>(reader).FirstOrDefault();
                data.MinimumPremiumSetting = MinimumPremiumSettingData;

                /* update isShow = 1 in case if AutoleasingInitialOption = 1 */
                var quotationRequest = _quotationRequestRepository.Table.Include(q => q.Vehicle).Where(x => x.ExternalId == externalId).FirstOrDefault();
                if (quotationRequest.AutoleasingInitialOption == true && string.IsNullOrEmpty(quotationRequest.Vehicle.SequenceNumber) && string.IsNullOrEmpty(quotationRequest.Vehicle.CustomCardNumber))
                {
                    quotationRequest.ShowInitial = true;
                    _quotationRequestRepository.Update(quotationRequest);
                }
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public QuotationInfoModel GetBulkQuotationsDetails(string externalId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetBulkQuotationsDetails";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationInfoModel>(reader).FirstOrDefault();

                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                reader.NextResult();
                data.Products = new List<QuotationProductInfoModel>();
                List<QuotationProductInfoModel> productsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationProductInfoModel>(reader).ToList();
                data.Products = productsData;

                reader.NextResult();
                List<ProductPriceDetailsInfoModel> PriceDetailsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductPriceDetailsInfoModel>(reader).ToList();

                reader.NextResult();
                List<ProductBenfitDetailsInfoModel> BenfitsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductBenfitDetailsInfoModel>(reader).ToList();

                if (data.Products != null && data.Products.Count > 0)
                {
                    foreach (var product in data.Products)
                    {
                        product.PriceDetails = new List<ProductPriceDetailsInfoModel>();
                        product.PriceDetails = PriceDetailsData.Where(a => a.ProductID == product.ProductID).ToList();

                        product.Benfits = new List<ProductBenfitDetailsInfoModel>();
                        product.Benfits = BenfitsData.Where(a => a.ProductId == product.ProductID).ToList();
                    }
                }

                reader.NextResult();
                BankData bankData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BankData>(reader).FirstOrDefault();
                data.Bank = bankData;

                reader.NextResult();
                DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();
                data.DepreciationSettingHistory = depreciationSettingHistoryData;

                reader.NextResult();
                DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();
                data.DepreciationSetting = depreciationSettingData;

                reader.NextResult();
                RepairMethodeSettingHistory repairMethodeSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSettingHistory>(reader).FirstOrDefault();
                data.RepairMethodeSettingHistory = repairMethodeSettingHistoryData;

                reader.NextResult();
                RepairMethodeSetting repairMethodeSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSetting>(reader).FirstOrDefault();
                data.RepairMethodeSetting = repairMethodeSettingData;

                reader.NextResult();
                MinimumPremiumSettingHistory minimumPremiumSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSettingHistory>(reader).FirstOrDefault();
                data.MinimumPremiumSettingHistory = minimumPremiumSettingHistoryData;

                reader.NextResult();
                MinimumPremiumSetting MinimumPremiumSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSetting>(reader).FirstOrDefault();
                data.MinimumPremiumSetting = MinimumPremiumSettingData;

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception)
            {

                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        #endregion

        #region Insurance Proposal From Stored Procedures

        public InsuranceProposalInfoModel GetInsuranceProposalDetails(string externalId,out string exception)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetInsuranceProposalDetails";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<InsuranceProposalInfoModel>(reader).FirstOrDefault();

                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                reader.NextResult();
                List<InsuranceProposalQuotationResponsesInfoModel> responsesData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<InsuranceProposalQuotationResponsesInfoModel>(reader).ToList();
                data.QuotationResponses = responsesData;
                if (responsesData != null && responsesData.Count > 0)
                {
                    reader.NextResult();
                    List<QuotationProductInfoModel> productsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationProductInfoModel>(reader).ToList();

                    reader.NextResult();
                    List<ProductPriceDetailsInfoModel> PriceDetailsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductPriceDetailsInfoModel>(reader).ToList();

                    reader.NextResult();
                    List<ProductBenfitDetailsInfoModel> BenfitsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductBenfitDetailsInfoModel>(reader).ToList();

                    foreach (var qtResponse in responsesData)
                    {
                        qtResponse.Products = new List<QuotationProductInfoModel>();
                        qtResponse.Products = productsData.Where(a => a.QuotationResponseId == qtResponse.Id).ToList();
                        if (qtResponse.Products != null && qtResponse.Products.Count > 0)
                        {
                            foreach (var product in qtResponse.Products)
                            {
                                product.PriceDetails = new List<ProductPriceDetailsInfoModel>();
                                product.PriceDetails = PriceDetailsData.Where(a => a.ProductID == product.ProductID).ToList();

                                product.Benfits = new List<ProductBenfitDetailsInfoModel>();
                                product.Benfits = BenfitsData.Where(a => a.ProductId == product.ProductID).ToList();
                            }
                        }

                        //qtResponse.Products = productsData;
                    }
                }

                reader.NextResult();
                BankData bankData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BankData>(reader).FirstOrDefault();
                data.Bank = bankData;

                reader.NextResult();
                DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();
                data.DepreciationSettingHistory = depreciationSettingHistoryData;

                reader.NextResult();
                DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();
                data.DepreciationSetting = depreciationSettingData;

                reader.NextResult();
                RepairMethodeSettingHistory repairMethodeSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSettingHistory>(reader).FirstOrDefault();
                data.RepairMethodeSettingHistory = repairMethodeSettingHistoryData;

                reader.NextResult();
                RepairMethodeSetting repairMethodeSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSetting>(reader).FirstOrDefault();
                data.RepairMethodeSetting = repairMethodeSettingData;

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        #endregion

        #region Company Quotation Response For Initial Quote

        public QuotationResponse GetQuotationResponseByExternalAndCompanyId(string externalId, int companyId, bool agencyRepair, int deductible)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetCompanyQuotationResponseForInitialQuote";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter ExternalIdParam = new SqlParameter() { ParameterName = "ExternalId", Value = externalId };
                command.Parameters.Add(ExternalIdParam);

                SqlParameter InsuranceCompanyIdParam = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = companyId };
                command.Parameters.Add(InsuranceCompanyIdParam);

                SqlParameter AgencyRepairParam = new SqlParameter() { ParameterName = "AgencyRepair", Value = agencyRepair };
                command.Parameters.Add(AgencyRepairParam);

                SqlParameter DeductibleParam = new SqlParameter() { ParameterName = "Deductible", Value = deductible };
                command.Parameters.Add(DeductibleParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationResponse>(reader).FirstOrDefault();
                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception)
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        #endregion

        public bool GetAutoleaseQuotationResponseCacheAndDelete(int insuranceCompanyId, string externalId, string initialExternalId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var response = _autoleasingQuotationResponseCache.Table.Where(x => x.InsuranceCompanyId == insuranceCompanyId
                && (x.ExternalId == externalId || x.ExternalId == initialExternalId));
                if (response != null && response.Any())
                {
                    _autoleasingQuotationResponseCache.Delete(response);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public AutoleasingQuotationResponseCache GetFromAutoleasingQuotationResponseCache(int insuranceCompanyId, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId)
        {
            AutoleasingQuotationResponseCache responseCache = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFromAutoleasingQuotationResponseCache";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                SqlParameter vehicleAgencyRepairParam = new SqlParameter() { ParameterName = "vehicleAgencyRepair", Value = vehicleAgencyRepair ? 1 : 0 };
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter deductibleValueParam = new SqlParameter("deductibleValue", SqlDbType.Int);
                if (deductibleValue.HasValue)
                    deductibleValueParam.Value = deductibleValue.Value;
                else
                    deductibleValueParam.Value = (object)DBNull.Value;

                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(externalIdParam);
                command.Parameters.Add(vehicleAgencyRepairParam);
                command.Parameters.Add(deductibleValueParam);
                command.Parameters.Add(userIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                responseCache = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationResponseCache>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (responseCache != null)
                {
                    return responseCache;
                }

                return responseCache;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool InsertIntoAutoleasingQuotationResponseCache(AutoleasingQuotationResponseCache autoleasingQuotationResponseCache, out string exception)
        {
            try
            {
                exception = string.Empty;
                autoleasingQuotationResponseCache.CreateDateTime = DateTime.Now;
                _autoleasingQuotationResponseCache.Insert(autoleasingQuotationResponseCache);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public WataniyaDraftPolicy GetWataniyaDraftPolicyInitialData(string referenceId)
        {
            var data = _wataniyaDraftPolicyRepository.TableNoTracking.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            return data;
        }

        public QuotationResponse GetWataniyaQuotationResponseByReferenceId(string referenceId)
        {
            return _quotationResponseRepository.TableNoTracking
                    .Include(q => q.QuotationRequest) // can be neglect if it not used
                    .Include(q => q.Products)
                    .Include(q => q.Products.Select(a => a.Product_Benefits))
                    .FirstOrDefault(q => q.ReferenceId == referenceId);
        }

        public LicenseType GetWataniyaDriverLicenseType(string licenseType)
        {
            LicenseType license = null;

            short typeCode = 0;
            short.TryParse(licenseType, out typeCode);
            if (typeCode > 0)
                license = GetAllLicenseType().Where(a => a.Code == typeCode).FirstOrDefault();

            return license;
        }

        public void InsertOrupdateWataniyaMotorPolicyInfo(WataniyaMotorPolicyInfo initialPolicyInfo, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (initialPolicyInfo.Id > 0)
                    _wataniyaMotorPolicyInfoRepository.Update(initialPolicyInfo);
                else
                    _wataniyaMotorPolicyInfoRepository.Insert(initialPolicyInfo);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }

        public WataniyaMotorPolicyInfo GetWataniyaMotorPolicyInfoByReference(string reference)
        {
            var initialPolicyInfo = _wataniyaMotorPolicyInfoRepository.Table.Where(a => a.ReferenceId == reference).FirstOrDefault();
            return initialPolicyInfo;
        }

        public void InsertOrupdateWataniyaAutoleasePolicyInfo(WataniyaDraftPolicy initialPolicyInfo, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (initialPolicyInfo.Id > 0)
                    _wataniyaDraftPolicyRepository.Update(initialPolicyInfo);
                else
                    _wataniyaDraftPolicyRepository.Insert(initialPolicyInfo);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }

        public WataniyaDraftPolicy GetWataniyaAutoleasePolicyInfoByReference(string reference)
        {
            var initialPolicyInfo = _wataniyaDraftPolicyRepository.Table.Where(a => a.ReferenceId == reference).FirstOrDefault();
            return initialPolicyInfo;
        }

        public QuotationRequest GetQuotationRequestVehicleInfo(string qtRqstExtrnlId)
        {
            var quotationRequest = _quotationRequestRepository.TableNoTracking
                .Include(qr => qr.Vehicle).FirstOrDefault(qr => qr.ExternalId == qtRqstExtrnlId);

            if (quotationRequest == null)
            {
                throw new TameenkEntityNotFoundException($"The quotation request with external identifier {qtRqstExtrnlId} not found.");
            }

            return quotationRequest;
        }
        public NCDFreeYear GetNCDFreeYearsInfo(int code)
        {
            return  _NCDFreeYearRepository.Table.FirstOrDefault(x => x.Code == code);
        }
        public int GetcountFromQuotationSharesByExternalId(string externalId,string shareType)
        {
            return _quotationSharesRepository.TableNoTracking.Where(x => x.ExternalId == externalId&&x.ShareType== shareType&&x.ErrorCode==1).Count();
        }
        public int RemoveUserIdFromQuotationRequest(string userId, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "RemoveUserIdFromQuotationRequest";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                command.Parameters.Add(userIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }
        public QuotationRequest GetQuotationRequesByPreviousReferenceId(string previousReferenceId)
        {
            var quotationRequest = _quotationRequestRepository.TableNoTracking
                .Where(qr => qr.PreviousReferenceId== previousReferenceId).FirstOrDefault();
            return quotationRequest;
        }

        public QuotationInfoModel GetAutoleasingRenewalQuotationsDetailsForHistorySettings(string externalId, bool agencyRepair, int deductible, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingRenewalQuotationsDetailsForHistorySettings";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);

                //SqlParameter agencyRepairParam = new SqlParameter() { ParameterName = "agencyRepair", Value = (agencyRepair) ? 1 : 0 };
                //command.Parameters.Add(agencyRepairParam);

                SqlParameter deductibleParam = new SqlParameter() { ParameterName = "deductible", Value = deductible };
                command.Parameters.Add(deductibleParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationInfoModel>(reader).FirstOrDefault();

                if (data == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    return null;
                }

                reader.NextResult();
                data.Products = new List<QuotationProductInfoModel>();
                List<QuotationProductInfoModel> productsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationProductInfoModel>(reader).ToList();
                data.Products = productsData;

                //List<QuotationProductInfoModel> _productsDataIEnumerable = productsData.GroupBy(a => new { a.InsuranceCompanyID, a.VehicleRepairType, a.DeductableValue })
                //                                                                       .Select(a => a.FirstOrDefault()).ToList();
                //data.Products = _productsDataIEnumerable;

                reader.NextResult();
                List<ProductPriceDetailsInfoModel> PriceDetailsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductPriceDetailsInfoModel>(reader).ToList();

                reader.NextResult();
                List<ProductBenfitDetailsInfoModel> BenfitsData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProductBenfitDetailsInfoModel>(reader).ToList();

                if (data.Products != null && data.Products.Count > 0)
                {
                    foreach (var product in data.Products)
                    {
                        product.PriceDetails = new List<ProductPriceDetailsInfoModel>();
                        product.PriceDetails = PriceDetailsData.Where(a => a.ProductID == product.ProductID).ToList();

                        product.Benfits = new List<ProductBenfitDetailsInfoModel>();
                        product.Benfits = BenfitsData.Where(a => a.ProductId == product.ProductID).ToList();
                    }
                }

                reader.NextResult();
                BankData bankData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BankData>(reader).FirstOrDefault();
                data.Bank = bankData;

                reader.NextResult();
                DepreciationSettingHistory depreciationSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSettingHistory>(reader).FirstOrDefault();
                data.DepreciationSettingHistory = depreciationSettingHistoryData;

                reader.NextResult();
                DepreciationSetting depreciationSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DepreciationSetting>(reader).FirstOrDefault();
                data.DepreciationSetting = depreciationSettingData;

                reader.NextResult();
                RepairMethodeSettingHistory repairMethodeSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSettingHistory>(reader).FirstOrDefault();
                data.RepairMethodeSettingHistory = repairMethodeSettingHistoryData;

                reader.NextResult();
                RepairMethodeSetting repairMethodeSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RepairMethodeSetting>(reader).FirstOrDefault();
                data.RepairMethodeSetting = repairMethodeSettingData;

                reader.NextResult();
                MinimumPremiumSettingHistory minimumPremiumSettingHistoryData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSettingHistory>(reader).FirstOrDefault();
                data.MinimumPremiumSettingHistory = minimumPremiumSettingHistoryData;

                reader.NextResult();
                MinimumPremiumSetting MinimumPremiumSettingData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MinimumPremiumSetting>(reader).FirstOrDefault();
                data.MinimumPremiumSetting = MinimumPremiumSettingData;

                /* update isShow = 1 in case if AutoleasingInitialOption = 1 */
                var quotationRequest = _quotationRequestRepository.Table.Include(q => q.Vehicle).Where(x => x.ExternalId == externalId).FirstOrDefault();
                if (quotationRequest.AutoleasingInitialOption == true && string.IsNullOrEmpty(quotationRequest.Vehicle.SequenceNumber) && string.IsNullOrEmpty(quotationRequest.Vehicle.CustomCardNumber))
                {
                    quotationRequest.ShowInitial = true;
                    _quotationRequestRepository.Update(quotationRequest);
                }
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public QuotationRequest GetQuotaionDriversForODByExternailId(string externalId)
        {
            return _quotationRequestRepository.TableNoTracking.Where(a => a.ExternalId == externalId)
                                              .Include(a => a.Driver)
                                              .Include(a => a.Drivers)
                                              .FirstOrDefault();
        }
        public QuotationRequest GetQuotationRequestDriversInfo(string qtRqstExtrnlId)
        {
            var quotationRequest = _quotationRequestRepository.TableNoTracking
                .Include(qr => qr.Driver.Addresses)
                .Include(qr => qr.Drivers.Select(d => d.Addresses))
                .Include(qr => qr.Insured.City.Region)
                .Include(qr => qr.Vehicle)
                .Include(qr => qr.QuotationResponses)
                .FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);

            if (quotationRequest == null)
            {
                throw new TameenkEntityNotFoundException($"The quotation request with external identifier {qtRqstExtrnlId} not found.");
            }

            return quotationRequest;
        }
        public QuotationRequestDriverModel GetQuotationRequestAndDriversInfo (string ReferenceId, string externalId, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                QuotationRequestDriverModel quotationResponseModel = null;
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestAndDriversInfo";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "ReferenceId", Value = ReferenceId };
                SqlParameter ExternalIdParameter = new SqlParameter() { ParameterName = "ExternalId", Value = externalId };
                command.Parameters.Add(referenceIdParameter);
                command.Parameters.Add(ExternalIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                quotationResponseModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequestDriverModel>(reader).FirstOrDefault();
                if (quotationResponseModel != null)
                {
                    reader.NextResult();
                    List<DriverData> additionalDriverList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverData>(reader).ToList();
                    quotationResponseModel.AdditionalDriverList = new List<DriverData>();
                    quotationResponseModel.AdditionalDriverList.AddRange(additionalDriverList);
                }
                dbContext.DatabaseInstance.Connection.Close();
                return quotationResponseModel;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public int UpdateQuotationRequestWithNOA (int quotationRequestId,int insuredId,int noOfAccident,string najmResponse)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateQuotationRequestWithNOA";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter quotationRequestIdParam = new SqlParameter() { ParameterName = "Id", Value = quotationRequestId };
                SqlParameter insuredIdParam = new SqlParameter() { ParameterName = "InsuredId", Value = insuredId };
                SqlParameter noOfAccidentParam = new SqlParameter() { ParameterName = "NoOfAccident", Value = noOfAccident };
                SqlParameter najmResponseParam = new SqlParameter() { ParameterName = "NajmResponse", Value = najmResponse };
                command.Parameters.Add(quotationRequestIdParam);
                command.Parameters.Add(insuredIdParam);
                command.Parameters.Add(noOfAccidentParam);
                command.Parameters.Add(najmResponseParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }

        public int ExpireQuotationResponses(int quotationRequestId, int insuredId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "ExpireQuotationResponses";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter quotationRequestIdParam = new SqlParameter() { ParameterName = "RequestId", Value = quotationRequestId };
                SqlParameter insuredIdParam = new SqlParameter() { ParameterName = "InsuredId", Value = insuredId };
                command.Parameters.Add(quotationRequestIdParam);
                command.Parameters.Add(insuredIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }

        public QuotationRequest GetQuotationRequestFromCachingWithExternalId(string externalId)
        {
            string cachingKey = $"quotation_{externalId}";

            try
            {
                object quoteRequestFromCache = Utilities.GetValueFromCache(cachingKey);
                if (quoteRequestFromCache != null)
                    return (QuotationRequest)quoteRequestFromCache;

                var quoteRequest = _quotationRequestRepository.TableNoTracking
                                        .Include(request => request.Vehicle)
                                        .Include(request => request.Driver)
                                        .Include(request => request.Insured)
                                        .Include(request => request.Insured.Occupation)
                                        .Include(request => request.Drivers.Select(d => d.DriverViolations))
                                        .Include(request => request.Driver.Occupation)
                                        .Include(e => e.Insured.IdIssueCity)
                                        .Include(e => e.Insured.City)
                                        .FirstOrDefault(q => q.ExternalId == externalId);
                if (quoteRequest != null)
                    Utilities.AddValueToCache(cachingKey, quoteRequest, 3);
                return quoteRequest;
            }
            catch (Exception)
            {
                var quoteRequest = _quotationRequestRepository.TableNoTracking
                                        .Include(request => request.Vehicle)
                                        .Include(request => request.Driver)
                                        .Include(request => request.Insured)
                                        .Include(request => request.Insured.Occupation)
                                        .Include(request => request.Drivers.Select(d => d.DriverViolations))
                                        .Include(request => request.Driver.Occupation)
                                        .Include(e => e.Insured.IdIssueCity)
                                        .Include(e => e.Insured.City)
                                        .FirstOrDefault(q => q.ExternalId == externalId);
                if (quoteRequest != null)
                    Utilities.AddValueToCache(cachingKey, quoteRequest, 3);
                return quoteRequest;
            }
        }

        public List<OldQuotationDetails> GetOldQuotationDetails(OldQuotationDetailsFilter oldQuotationDetailsFilter, out int totalcount, out string exception)
        {
            totalcount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetOldQuotationLogDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                if (!string.IsNullOrEmpty(oldQuotationDetailsFilter.NationalId))
                {
                    SqlParameter NationalIdParamter = new SqlParameter() { ParameterName = "NationalId", Value = oldQuotationDetailsFilter.NationalId };
                    command.Parameters.Add(NationalIdParamter);
                }
                if (!string.IsNullOrEmpty(oldQuotationDetailsFilter.SequenceNumber))
                {
                    SqlParameter SequenceNumberParamter = new SqlParameter() { ParameterName = "SequenceNumber", Value = oldQuotationDetailsFilter.SequenceNumber };
                    command.Parameters.Add(SequenceNumberParamter);
                }
                if (!string.IsNullOrEmpty(oldQuotationDetailsFilter.referenceNo))
                {
                    SqlParameter ReferenceNumberParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = oldQuotationDetailsFilter.referenceNo };
                    command.Parameters.Add(ReferenceNumberParameter);
                }
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = oldQuotationDetailsFilter.PageNumber > 0 ? oldQuotationDetailsFilter.PageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = oldQuotationDetailsFilter.PageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter IsExportParameter = new SqlParameter() { ParameterName = "IsExport", Value = oldQuotationDetailsFilter.isExport };
                command.Parameters.Add(IsExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<OldQuotationDetails> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<OldQuotationDetails>(reader).ToList();

                if (filteredData != null && filteredData.Count > 0)
                    totalcount = filteredData.Count;

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public QuotationResponse GetODResponseDetailsByExternalId(string externalId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var quotation = _quotationRequestRepository.TableNoTracking.Include(a => a.QuotationResponses).Where(a => a.ExternalId == externalId).FirstOrDefault();
                if (quotation == null || (quotation.QuotationResponses == null || quotation.QuotationResponses.Count < 1) || !quotation.QuotationResponses.Any(a => a.InsuranceCompanyId == 22 && a.InsuranceTypeCode == 9))
                    return null;

                return quotation.QuotationResponses.Where(a => a.InsuranceCompanyId == 22 && a.InsuranceTypeCode == 9).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }
        public List<LicenseType> GetAllLicenseType(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format("_License___typE_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _licenseTypeRepository.TableNoTracking.ToList();
            });
        }

        public QuoteRequestVehicleInfo GetQuotationRequestAndVehicleInfo(string externalId)
        {

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestVehicleInfo";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuoteRequestVehicleInfo>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public bool AddBlockedUser(AddBlockedNinModel addBlockedUser, string userName, out string exception)
        {
            exception = string.Empty;
            try
            {
                var entityExist = _quotationBlockedNins.Table.Where(a => a.NationalId == addBlockedUser.NationalId.Trim()).FirstOrDefault();

                if (entityExist == null)
                {
                    QuotationBlockedNins blockedUser = new QuotationBlockedNins();
                    blockedUser.NationalId = addBlockedUser.NationalId.Trim();
                    blockedUser.CreatedBy = userName;
                    blockedUser.CreatedDate = DateTime.Now;
                    blockedUser.BlockReason = addBlockedUser.BlockReason;
                    _quotationBlockedNins.Insert(blockedUser);
                    return true;
                }
                else
                {

                    exception = "Nin " + entityExist.NationalId + " Is Already Blocked ";
                    return false;

                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return false;
            }
        }

        public List<BlockedUsersDTO> GetQuotationBlockFilterService(BlockedNinFilter filter, out int totalcount, out string exception)
        {
            totalcount = 0;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationBlockedUserDetails";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                dbContext.DatabaseInstance.Connection.Open();

                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@NationalId",
                    Value = (!string.IsNullOrEmpty(filter.NationalId)) ? filter.NationalId : null
                });

                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@IsExport",
                    Value = (filter.IsExport) ? 1 : 0
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PageNumber",
                    Value = filter.PageIndex
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PageSize",
                    Value = filter.PageSize
                });
                var reader = command.ExecuteReader();
                List<BlockedUsersDTO> data = new List<BlockedUsersDTO>();
                if (filter.IsExport)
                {
                    ////get excel data
                    data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BlockedUsersDTO>(reader).ToList();
                }
                else
                {
                    // get blocked users paginated data
                    data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BlockedUsersDTO>(reader).ToList();

                    //get data count
                    reader.NextResult();
                    totalcount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }
                return data;
            }
            catch (Exception ex)
            {
                totalcount = 0;
                exception = ex.ToString();
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool RemoveBlockNin(int Id, out string nin, out string exception)
        {
            nin = string.Empty;
            try
            {
                exception = string.Empty;
                var blockedNin = _quotationBlockedNins.Table.FirstOrDefault(n => n.Id == Id);
                if (blockedNin != null)
                {
                    nin = blockedNin.NationalId;
                    _quotationBlockedNins.Delete(blockedNin);
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public AutoleasingQuotationFormSettingModel GetFristYearPolicyDetails(string SequenceNumber)
        {
            AutoleasingQuotationFormSettingModel AutoleasingQuotationFormSettings = new AutoleasingQuotationFormSettingModel();
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetFirstYearPolicyStatistics";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter vehileId = new SqlParameter() { ParameterName = "SequenceNumber", Value = SequenceNumber };
                command.Parameters.Add(vehileId);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                AutoleasingQuotationFormSettings = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationFormSettingModel>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (AutoleasingQuotationFormSettings != null)
                {
                    return AutoleasingQuotationFormSettings;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
    }
}
