using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Caching;
using Tameenk.Core.Domain.Enums;
using VehicleInsurance = Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Configuration;
using System.Threading.Tasks;
using QuotationIntegrationDTO = Tameenk.Integration.Dto.Quotation;
using Tameenk.Redis;
using NLog;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial  class QuotationService : Tameenk.Services.QuotationNew.Components.IQuotationService
    {
        #region Fields

        private readonly TameenkConfig _tameenkConfig;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<VehicleColor> _vehicleColorRepository;
        private readonly IRepository<VehicleInsurance.VehicleMaker> _vehicleMakerRepository;
        private readonly IRepository<VehicleInsurance.VehicleModel> _vehicleModelRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IRepository<InsuredExtraLicenses> _insuredExtraLicenses;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<DriverViolation> driverViolationRepository;
        private readonly IRepository<VehiclePlateText> _vehiclePlateTextRepository;
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<PriceType> _priceTypeRepository;
        private readonly IRepository<QuotationResponseCache> _quotationResponseCache;
        private readonly Logger logger;

        private readonly List<int> insuranceCompaniesExcluedFromSchemesQuotations = new List<int>()
        {
            (int)InsuranceCompanyEnum.ACIG,
            (int)InsuranceCompanyEnum.AICC,
            (int)InsuranceCompanyEnum.Tawuniya
        };

        public const string quotationResponseCach_Base_KEY = "QuOtAtIoN_cAcH";
        public const int quotationResponseCach_TiMe = 4 * 60 * 60;
        public const string quotationRequestCach_Base_KEY = "QuOtAtIoN_rEqUeSt_cAcH";
        public const string quotationRequestDetailsCach_Base_KEY = "QuOtAtIoN_rEqUeSt_DeTailS_cAcH";

        #endregion

        public QuotationService(TameenkConfig tameenkConfig, IRepository<QuotationResponse> quotationResponseRepository, IRepository<QuotationRequest> quotationRequestRepository
            , ICacheManager cacheManager, IRepository<InsuranceCompany> insuranceCompanyRepository, IRepository<City> cityRepository, IRepository<VehicleColor> vehicleColorRepository
            , IRepository<VehicleInsurance.VehicleMaker> vehicleMakerRepository, IRepository<VehicleInsurance.VehicleModel> vehicleModelRepository, IRepository<LicenseType> licenseTypeRepository
            , IRepository<InsuredExtraLicenses> insuredExtraLicenses, IRepository<Driver> driverRepository, IRepository<DriverViolation> _driverViolationRepository
            , IRepository<VehiclePlateText> vehiclePlateTextRepository, IRepository<Benefit> benefitRepository, IRepository<PriceType> priceTypeRepository, IRepository<QuotationResponseCache> quotationResponseCache
            )
        {
            _quotationResponseRepository = quotationResponseRepository;
            _quotationRequestRepository = quotationRequestRepository;
            _cacheManager = cacheManager;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _cityRepository = cityRepository;
            _vehicleColorRepository = vehicleColorRepository;
            _vehicleMakerRepository = vehicleMakerRepository;
            _vehicleModelRepository = vehicleModelRepository;
            _licenseTypeRepository = licenseTypeRepository;
            _insuredExtraLicenses = insuredExtraLicenses;
            _driverRepository = driverRepository;
            driverViolationRepository = _driverViolationRepository;
            _vehiclePlateTextRepository = vehiclePlateTextRepository;
            _benefitRepository = benefitRepository;
            _priceTypeRepository = priceTypeRepository;
            _tameenkConfig = tameenkConfig;
            _quotationResponseCache = quotationResponseCache;
            this.logger = LogManager.GetCurrentClassLogger();
        }
        public async  Task<QuotationOutPut> GetQuotation(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair, string currentUserId, string currentUserName, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false)
        {
         

            QuotationOutPut quotationOutPut = new QuotationOutPut();
            DateTime excutionStartDate = DateTime.Now;
            if (channel.ToLower() == "android".ToLower() && insuranceTypeCode == 2 && insuranceCompanyId == 6)
            {
                quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.NoprouductToShow;
                quotationOutPut.ErrorDescription = "No Product to show";
                return quotationOutPut;
            }
            QuotationRequestLog log = null;
            try
            {
                Guid selectedUserId = Guid.Empty;
                Guid.TryParse(currentUserId, out selectedUserId);
                if (insuranceTypeCode == 1 && insuranceCompanyId != 12)// as per Fayssal
                vehicleAgencyRepair = false;

                log =  ValidateRequest(insuranceCompanyId, qtRqstExtrnlId, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, currentUserId, currentUserName, deductibleValue, policyNo, policyExpiryDate, hashed, channel, OdQuotation);
                quotationOutPut.QuotationRequestLog = log;
                if (log.ErrorCode != (int)QuotationOutPut.ErrorCodes.Success)
                {
                    quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.NoprouductToShow;
                    quotationOutPut.ErrorDescription = "NO product To show ";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return quotationOutPut;
                }

                quotationOutPut.QuotationResponseModel = null;
                quotationOutPut.CacheExist = false;
                var quotationResponseCache = await GetFromQuotationResponseCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId);
                if (quotationResponseCache != null && quotationResponseCache.Products.Any())
                {
                    quotationOutPut.QuotationResponseModel = quotationResponseCache;
                    quotationOutPut.CacheExist = true;
                    quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.Success;
                    return quotationOutPut;
                }

                var quotationResponse = await  HandleGetQuote (insuranceCompanyId, qtRqstExtrnlId, log.Channel,
                        selectedUserId, currentUserName, log, excutionStartDate, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);

                if (quotationResponse == null || quotationResponse.ErrorCode != QuotationNewOutput.ErrorCodes.Success || quotationResponse.QuotationResponse == null|| quotationResponse.QuotationResponse.Products == null || quotationResponse.QuotationResponse.Products.Count() == 0)
                {
                    quotationOutPut.ErrorDescription = "No Product to show";
                    quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.NoprouductToShow;
                    return quotationOutPut;
                }
  

               // quotationOutPut = HandleProuduct_Terms_Price_payment(quotationResponse, log, insuranceCompanyId, insuranceTypeCode);


                quotationOutPut = FromatResponse(quotationOutPut, insuranceTypeCode, insuranceCompanyId, vehicleAgencyRepair, deductibleValue);

                //InsertQuotationResponseIntoInmemoryCache(insuranceCompanyId, insuranceTypeCode, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, selectedUserId, quotationOutPut.QuotationResponseModel);
                quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.Success;
                return quotationOutPut;
            }
            catch (Exception ex)
            {
                quotationOutPut.ErrorCode = QuotationOutPut.ErrorCodes.ServiceException;
                quotationOutPut.ErrorDescription = "NO product To show ";
                log.ErrorDescription = ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return quotationOutPut;
            }

        }

        public QuotationRequestLog ValidateRequest(int insuranceCompanyId, string qtRqstExtrnlId, Guid parentRequestId, int insuranceTypeCode, bool vehicleAgencyRepair,string currentUserId,string currentUserName, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, string channel = "Portal", bool OdQuotation = false)
        {
            QuotationRequestLog log = new QuotationRequestLog();
            try
            {
   
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.ExtrnlId = qtRqstExtrnlId;
                log.InsuranceTypeCode = insuranceTypeCode;
                log.CompanyId = insuranceCompanyId;
                log.Channel = channel;
                log.Referer = Utilities.GetFullUrlReferrer();
                log.ServiceRequest = $"insuranceCompanyId: {insuranceCompanyId}, qtRqstExtrnlId: {qtRqstExtrnlId}, parentRequestId: {parentRequestId}, insuranceTypeCode: {insuranceTypeCode}, vehicleAgencyRepair: {vehicleAgencyRepair}, deductibleValue: {deductibleValue}, policyNo: {policyNo}, policyExpiryDate: {policyExpiryDate}, hashed: {hashed}";
                Guid selectedUserId = Guid.Empty;
                Guid.TryParse(currentUserId, out selectedUserId);
                if (!string.IsNullOrEmpty(currentUserName))
                {
                    log.UserId = selectedUserId.ToString();
                    log.UserName = currentUserName;
                }
                if (insuranceTypeCode == 1 && insuranceCompanyId != 12)// as per Fayssal
                    vehicleAgencyRepair = false;
                log.ErrorCode = 1;
                return log;
            }
            catch (Exception ex)
            {
                log.ErrorCode = 3;
                log.ErrorDescription = "Validation Error " + ex.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return log;
            }

        }


        public async Task<QuotationIntegrationDTO.QuotationResponseModel> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId)
        {
            var cacheKey = $"{quotationResponseCach_Base_KEY}_{externalId}_{insuranceCompanyId}_{insuranceTypeCode}_{vehicleAgencyRepair}_{deductibleValue}";
            var cachedValue = await Tameenk.Redis.RedisCacheManager.Instance.GetAsync<QuotationIntegrationDTO.QuotationResponseModel>(cacheKey);
            if (cachedValue != null && cachedValue.Products.Any())
                return cachedValue;

            return null;
        }

        public async void InsertQuotationResponseIntoInmemoryCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId, QuotationIntegrationDTO.QuotationResponseModel quotation)
        {
            try
            {
                var cacheKey = $"{quotationResponseCach_Base_KEY}_{externalId}_{insuranceCompanyId}_{insuranceTypeCode}_{vehicleAgencyRepair}_{deductibleValue}";
                await RedisCacheManager.Instance.SetAsync(cacheKey, quotation, quotationResponseCach_TiMe);
            }
            catch (Exception ex)
            {
                //Log4NetManager.Instance.Log(nameof(InsertQuotationResponseIntoInmemoryCache), ex);
                logger.Error(nameof(InsertQuotationResponseIntoInmemoryCache), ex);
            }
        }

    }
}
