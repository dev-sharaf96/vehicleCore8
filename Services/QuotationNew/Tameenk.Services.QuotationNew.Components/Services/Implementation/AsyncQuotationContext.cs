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

namespace Tameenk.Services.QuotationNew.Components
{
    public class AsyncQuotationContext : IAsyncQuotationContext
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

        private const string quotationResponseCach_Base_KEY = "QuOtAtIoN_cAcH";
        private const int quotationResponseCach_TiMe = 4 * 60 * 60;
        private const string quotationRequestCach_Base_KEY = "QuOtAtIoN_rEqUeSt_cAcH";
        private const string quotationRequestDetailsCach_Base_KEY = "QuOtAtIoN_rEqUeSt_DeTailS_cAcH";

        #endregion

        public AsyncQuotationContext(TameenkConfig tameenkConfig, IRepository<QuotationResponse> quotationResponseRepository, IRepository<QuotationRequest> quotationRequestRepository
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


        #region Public Methods

        //public async Task<string> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId)
        //{

        //    var cacheKey = $"{quotationResponseCach_Base_KEY}_{externalId}_{insuranceCompanyId}_{insuranceTypeCode}_{vehicleAgencyRepair}_{deductibleValue}";

        //    //return _cacheManager.Get(cacheKey, 30, () => { return string.Empty; });
        //    return _cacheManager.GetWithoutSetValue(cacheKey, () => { return string.Empty; });

        //}

        /// <summary>
        /// New Caching With REDIS
        /// </summary>
        /// <param name="insuranceCompanyId"></param>
        /// <param name="insuranceTypeCode"></param>
        /// <param name="externalId"></param>
        /// <param name="vehicleAgencyRepair"></param>
        /// <param name="deductibleValue"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<QuotationIntegrationDTO.QuotationResponseModel> GetFromQuotationResponseCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId)
        {
            var cacheKey = $"{quotationResponseCach_Base_KEY}_{externalId}_{insuranceCompanyId}_{insuranceTypeCode}_{vehicleAgencyRepair}_{deductibleValue}";
            var cachedValue = await RedisCacheManager.Instance.GetAsync<QuotationIntegrationDTO.QuotationResponseModel>(cacheKey);
            if (cachedValue != null && cachedValue.Products.Any())
                return cachedValue;

            return null;
        }

        public async Task<QuotationNewOutput> HandleGetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            var task = Task.Run(() =>
            {
                return GetQuote(insuranceCompanyId, qtRqstExtrnlId, channel, userId, userName, log, excutionStartDate, parentRequestId
                            , insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo, policyExpiryDate, hashed, OdQuotation);
            });

            return task.Result;
        }

        //public async void InsertQuotationResponseIntoInmemoryCache(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId, string jsonResponse)
        //{
        //    try
        //    {
        //        //QuotationResponseCache quotationResponseCache = new QuotationResponseCache()
        //        //{
        //        //    InsuranceCompanyId = insuranceCompanyId,
        //        //    ExternalId = externalId,
        //        //    InsuranceTypeCode = insuranceTypeCode,
        //        //    VehicleAgencyRepair = vehicleAgencyRepair,
        //        //    DeductibleValue = deductibleValue,
        //        //    UserId = userId,
        //        //    QuotationResponse = jsonResponse,
        //        //    CreateDateTime = DateTime.Now
        //        //};
        //        //_quotationResponseCache.Insert(quotationResponseCache);

        //        var cacheKey = $"{quotationResponseCach_Base_KEY}_{externalId}_{insuranceCompanyId}_{insuranceTypeCode}_{vehicleAgencyRepair}_{deductibleValue}";
        //        _cacheManager.Set(cacheKey, jsonResponse, 30);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\InsertQuotationResponseIntoInmemoryCache_" + externalId + "_" + insuranceCompanyId + "_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());
        //    }
        //}

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

        #endregion


        #region Private Methods

        //By Niaz Upgrade-Assistant todo
        //private QuotationResponseCache GetFromQuotationResponseCacheDB(int insuranceCompanyId, int insuranceTypeCode, string externalId, bool vehicleAgencyRepair, int? deductibleValue, Guid userId)
        //{
        //    QuotationResponseCache responseCache = null;
        //    IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

        //    try
        //    {
        //        dbContext.DatabaseInstance.CommandTimeout = 60;
        //        var command = dbContext.DatabaseInstance.Connection.CreateCommand();
        //        command.CommandText = "GetFromQuotationResponseCache";
        //        command.CommandType = CommandType.StoredProcedure;
        //        SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
        //        SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };
        //        SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
        //        SqlParameter vehicleAgencyRepairParam = new SqlParameter() { ParameterName = "vehicleAgencyRepair", Value = vehicleAgencyRepair };
        //        SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
        //        SqlParameter deductibleValueParam = new SqlParameter("deductibleValue", SqlDbType.Int);
        //        if (deductibleValue.HasValue)
        //            deductibleValueParam.Value = deductibleValue.Value;
        //        else
        //            deductibleValueParam.Value = (object)DBNull.Value;

        //        command.Parameters.Add(insuranceCompanyIdParam);
        //        command.Parameters.Add(insuranceTypeCodeParam);
        //        command.Parameters.Add(externalIdParam);
        //        command.Parameters.Add(vehicleAgencyRepairParam);
        //        command.Parameters.Add(deductibleValueParam);
        //        command.Parameters.Add(userIdParam);

        //        dbContext.DatabaseInstance.Connection.Open();
        //        var reader = command.ExecuteReader();
        //        responseCache = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationResponseCache>(reader).FirstOrDefault();
        //        dbContext.DatabaseInstance.Connection.Close();
        //        if (responseCache != null)
        //        {
        //            return responseCache;
        //        }
        //        return responseCache;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
        //            dbContext.DatabaseInstance.Connection.Close();
        //    }
        //}

        private async Task<QuotationNewOutput> GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            QuotationNewOutput output = new QuotationNewOutput();
            output.QuotationResponse = new QuotationResponse();
            try
            {
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceCompanyId == 0)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 9 && !string.IsNullOrEmpty(policyExpiryDate))
                {
                    var oldPolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                    if (oldPolicyExpiryDate.HasValue && oldPolicyExpiryDate.Value.CompareTo(DateTime.Now.AddDays(90)) <= 0)
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.InvalidODPolicyExpiryDate;
                        output.ErrorDescription = SubmitInquiryResource.InvalidODPolicyExpiryDate;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"Can't proceed OD request , as TPL policy expiry date is {oldPolicyExpiryDate.Value} and it's less than 90 days";
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }
                if (insuranceCompanyId == 22 && insuranceTypeCode == 9 && !OdQuotation)
                {
                    if (string.IsNullOrEmpty(hashed))
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = SubmitInquiryResource.ErrorHashing;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Hashed value is empty";
                        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }

                    string clearText = string.Format("{0}_{1}_{2}_{3}", true, policyNo, Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate), SecurityUtilities.HashKey);
                    if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.HashedNotMatched;
                        output.ErrorDescription = SubmitInquiryResource.ErrorHashing;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }

                var insuranceCompany = GetById(insuranceCompanyId);
                log.CompanyName = insuranceCompany.Key;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 2 && !insuranceCompany.IsActiveComprehensive)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "Comprehensive products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Comprehensive products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 1 && !insuranceCompany.IsActiveTPL)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "TPL products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TPL products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 13 && !insuranceCompany.IsActiveMotorPlus)
                {
                    output.ErrorCode = QuotationNewOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "Motor Plus products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Motor Plus products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }

                if (insuranceCompany.Key.ToLower() == "tawuniya" || insuranceTypeCode == 1)
                    deductibleValue = null;
                else if (!deductibleValue.HasValue)
                    deductibleValue = (int?)2000;

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompanyId;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;
                predefinedLogInfo.Channel = channel.ToString();
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;

                var quotationNewRequestDetails = await RedisCacheManager.Instance.GetAsync<QuotationRequestDetailsCachingModel>($"{quotationRequestCach_Base_KEY}_{qtRqstExtrnlId}");
                QuotationNewRequestDetails requestDetails = null;
                if (quotationNewRequestDetails != null && quotationNewRequestDetails.QuotationDetails != null)
                {
                    requestDetails = quotationNewRequestDetails.QuotationDetails.ToModel();
                }
                else
                {
                    requestDetails = await GetQuotationRequestDetailsByExternalId(qtRqstExtrnlId);
                    if (requestDetails == null)
                    {
                        output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                        output.LogDescription = "failed to get Quotation Request from DB, please check the log file";
                        return output;
                    }
                }

                output = GetQuotationResponseDetails(requestDetails, insuranceCompany, qtRqstExtrnlId, predefinedLogInfo, log, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo: policyNo, policyExpiryDate: policyExpiryDate, OdQuotation: OdQuotation);
                //log.RefrenceId = output.QuotationResponse.ReferenceId;
                if (output == null)
                    return null;
                if (output.ErrorCode != QuotationNewOutput.ErrorCodes.Success)
                {
                    //output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
                    //output.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.LogDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return (output.ErrorCode == QuotationNewOutput.ErrorCodes.QuotationExpired) ? output : null;
                }
                if (output.QuotationResponse == null || output.QuotationResponse.Products == null || output.QuotationResponse.Products.Count == 0)
                    return null;

                if (OdQuotation)
                    output.QuotationResponse.InsuranceCompany.ShowQuotationToUser = false;
                if (insuranceCompany.AllowAnonymousRequest.HasValue && insuranceCompany.AllowAnonymousRequest.Value)
                    output.QuotationResponse.CompanyAllowAnonymous = true;
                if (userId != Guid.Empty)
                    output.QuotationResponse.AnonymousRequest = false;
                if (insuranceCompany.ShowQuotationToUser.HasValue && !insuranceCompany.ShowQuotationToUser.Value)
                    output.QuotationResponse.Products = null;
                if (insuranceCompany.HasDiscount.HasValue && output.QuotationResponse.Products != null)
                {
                    if (insuranceCompany.HasDiscount.Value && DateTime.Now < insuranceCompany.DiscountEndDate)
                    {
                        insuranceCompany.HasDiscount = true;
                        output.QuotationResponse.HasDiscount = true;
                        output.QuotationResponse.DiscountText = insuranceCompany.DiscountText;
                    }
                    else
                    {
                        insuranceCompany.HasDiscount = false;
                    }
                }
                else
                {
                    insuranceCompany.HasDiscount = false;
                }
                if (insuranceTypeCode == 1)
                    output.ShowTabby = insuranceCompany.ActiveTabbyTPL;
                else if (insuranceTypeCode == 2)
                    output.ShowTabby = insuranceCompany.ActiveTabbyComp;
                else if (insuranceTypeCode == 7)
                    output.ShowTabby = insuranceCompany.ActiveTabbySanadPlus;
                else if (insuranceTypeCode == 8)
                    output.ShowTabby = insuranceCompany.ActiveTabbyWafiSmart;
                else if (insuranceTypeCode == 13)
                    output.ShowTabby = insuranceCompany.ActiveTabbyMotorPlus;
                else
                    output.ShowTabby = false;
                //output.ActiveTabbyComp = insuranceCompany.ActiveTabbyComp;
                //output.ActiveTabbyTPL = insuranceCompany.ActiveTabbyTPL;
                //output.ActiveTabbySanadPlus = insuranceCompany.ActiveTabbySanadPlus;
                //output.ActiveTabbyWafiSmart = insuranceCompany.ActiveTabbyWafiSmart;

                // As per Moneera @19/3/2023 (https://bcare.atlassian.net/browse/MLI-14)
                //// As per @Moneera request @12-1-2023
                //if (insuranceCompanyId != 22)
                //{
                output.TermsAndConditionsFilePath = insuranceCompany.TermsAndConditionsFilePath;
                output.TermsAndConditionsFilePathComp = insuranceCompany.TermsAndConditionsFilePathComp;
                output.TermsAndConditionsFilePathSanadPlus = insuranceCompany.TermsAndConditionsFilePathSanadPlus;
                //}

                //if (insuranceCompanyId == 14 && log.UserId != "4e4e4605-bd3a-4288-bf56-d32658466a93")
                //    output.QuotationResponse.Products = null;

                //if (insuranceCompanyId == 5 && insuranceTypeCode == 2 && log.UserId != "1b4cdb65-9804-4ab4-86a8-af62bf7812d7" && log.UserId != "ebf4df2c-c9bb-4d7d-91fe-4b9208c1631a") // As per Fayssal @ 22-03-2023 2:45 PM
                //    output.QuotationResponse.Products = null;

                //if (insuranceCompanyId == 21 && log.UserId != "1b4cdb65-9804-4ab4-86a8-af62bf7812d7" && log.UserId != "ebf4df2c-c9bb-4d7d-91fe-4b9208c1631a") // As per Mubarak @ 18-01-2024 12:23 PM
                //    output.QuotationResponse.Products = null;

                ////
                /// First time (5000) --> As per Fayssal @ 06-03-2023 3:45 PM
                /// Second time (8000) --> As per Moneera @ 28-03-2023 3:45 PM (email Hide Quotations)
                /// Second time (20.000) --> As per Moneera @ 29-03-2023 2:57 PM (email Hide Quotations)
                /// Second time (5000) --> As per Moneera @ 30-03-2023 1:51 PM (Hide Quotations VW-769)
                if (insuranceTypeCode == 1 && output.QuotationResponse.Products != null && output.QuotationResponse.Products.Any(a => a.ProductPrice >= 5000))
                {
                    var productsLessThan5000 = output.QuotationResponse.Products.Where(a => a.ProductPrice <= 4999).ToList();
                    output.QuotationResponse.Products = productsLessThan5000;
                }

                //////
                ///// exclude products with price type (7 & 8) if agencyRepair is true
                ///// As per Al-Majed && Rawan
                //if (vehicleAgencyRepair && output.QuotationResponse.Products != null && output.QuotationResponse.Products.Any(a => a.InsuranceTypeCode == 7 || a.InsuranceTypeCode == 8))
                //{
                //    var filteredProducts = output.QuotationResponse.Products.Where(a => a.InsuranceTypeCode != 7 && a.InsuranceTypeCode != 8).ToList();
                //    output.QuotationResponse.Products = filteredProducts;
                //}

                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }
        }

        private QuotationNewOutput GetQuotationResponseDetails(QuotationNewRequestDetails quoteRequest, InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false, string policyNo = null, string policyExpiryDate = null, bool OdQuotation = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();

            QuotationNewOutput output = new QuotationNewOutput();
            DateTime startDateTime = DateTime.Now;
            string referenceId = string.Empty;
            referenceId = getNewReferenceId();
            log.RefrenceId = referenceId;
            DateTime beforeCallingDB = DateTime.Now;
            string exception = string.Empty;
            if (quoteRequest == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest is null";
                return output;
            }

            ////
            /// validate for quotation expiration as per Khaled@27-11-2023 2:30 PM
            if (DateTime.Now.AddHours(-16) > quoteRequest.QuotationCreatedDate)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.QuotationExpired;
                output.ErrorDescription = WebResources.quotations_is_expired;
                output.LogDescription = "quoteRequest is expired";
                return output;
            }

            if (string.IsNullOrEmpty(quoteRequest.NationalId))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Insured is null or empty";
                return output;
            }
            log.NIN = quoteRequest.NationalId;
            if (string.IsNullOrEmpty(quoteRequest.MainDriverNin))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Driver is null";
                return output;
            }
            predefinedLogInfo.DriverNin = quoteRequest.MainDriverNin;

            if (string.IsNullOrEmpty(quoteRequest.VehicleId.ToString()))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Vehicle is null ";
                return output;
            }
            if (insuranceCompany.InsuranceCompanyID == 8 && quoteRequest.VehicleIdType == VehicleIdType.CustomCard
                && (quoteRequest.VehicleBodyCode == 1 || quoteRequest.VehicleBodyCode == 2 || quoteRequest.VehicleBodyCode == 3 || quoteRequest.VehicleBodyCode == 19 || quoteRequest.VehicleBodyCode == 20))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = "No supported product with medgulf with such information";
                output.LogDescription = "MedGulf Invalid Body Type with Custom Card body type is " + quoteRequest.VehicleBodyCode;
                return output;
            }

            if (quoteRequest.Cylinders >= 0 && quoteRequest.Cylinders <= 4)
                quoteRequest.EngineSizeId = 1;
            else if (quoteRequest.Cylinders >= 5 && quoteRequest.Cylinders <= 7)
                quoteRequest.EngineSizeId = 2;
            else
                quoteRequest.EngineSizeId = 3;

            if (quoteRequest.VehicleIdType == VehicleIdType.CustomCard)
                predefinedLogInfo.VehicleId = quoteRequest.CustomCardNumber;
            else
                predefinedLogInfo.VehicleId = quoteRequest.SequenceNumber;
            log.VehicleId = predefinedLogInfo.VehicleId;
            if (quoteRequest.NationalId.StartsWith("7") && !quoteRequest.OwnerTransfer && (insuranceCompany.InsuranceCompanyID == 12 || insuranceCompany.InsuranceCompanyID == 14))
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for Tawuniya and Wataniya";
                return output;
            }
            if (quoteRequest.NationalId.StartsWith("7") && insuranceCompany.InsuranceCompanyID == 25) //AXA
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for AXA";
                return output;
            }
            if (quoteRequest.RequestPolicyEffectiveDate.HasValue && quoteRequest.RequestPolicyEffectiveDate.Value <= DateTime.Now.Date)
            {
                DateTime effectiveDate = DateTime.Now.AddDays(1);
                quoteRequest.RequestPolicyEffectiveDate = new DateTime(effectiveDate.Year, effectiveDate.Month, effectiveDate.Day, effectiveDate.Hour, effectiveDate.Minute, effectiveDate.Second);
                var quoteRequestInfo = _quotationRequestRepository.Table.Where(a => a.ExternalId == qtRqstExtrnlId).FirstOrDefault();
                if (quoteRequestInfo != null)
                {
                    quoteRequestInfo.RequestPolicyEffectiveDate = quoteRequest.RequestPolicyEffectiveDate;
                    _quotationRequestRepository.Update(quoteRequestInfo);
                }
            }

            output.QuotationResponse = new QuotationResponse()
            {
                ReferenceId = referenceId,
                RequestId = quoteRequest.ID,
                InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                VehicleAgencyRepair = vehicleAgencyRepair,
                DeductibleValue = deductibleValue,
                CreateDateTime = startDateTime,
                InsuranceCompanyId = insuranceCompany.InsuranceCompanyID
            };
            string promotionProgramCode = string.Empty;
            int promotionProgramId = 0;
            DateTime beforeGettingRequestMessage = DateTime.Now;
            var requestMessage = GetQuotationRequestMessage(quoteRequest, output.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue, out promotionProgramCode, out promotionProgramId);
            log.RequestMessageResponseTimeInSeconds = DateTime.Now.Subtract(beforeGettingRequestMessage).TotalSeconds;
            //if (insuranceCompany.InsuranceCompanyID == 21 && string.IsNullOrEmpty(requestMessage.PromoCode))
            //{
            //    output.ErrorCode = QuotationNewOutput.ErrorCodes.ServiceDown;
            //    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
            //    output.LogDescription = "PromoCode is null for Saico ";
            //    return output;
            //}
            if (insuranceCompany.InsuranceCompanyID == 6 && requestMessage.VehicleUseCode == 2)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.CommercialProductNotSupported;
                output.ErrorDescription = "Commercial product is not supported";
                output.LogDescription = "Commercial product is not supported";
                return output;
            }
            if (insuranceCompany.Key.ToLower() == "malath")
            {
                if (insuranceTypeCode == 2)
                    requestMessage.DeductibleValue = null;
                else if (insuranceTypeCode == 9)
                {
                    if (OdQuotation)
                    {
                        requestMessage.PolicyNo = "new";
                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(DateTime.UtcNow.AddYears(1).ToString());
                    }
                    else
                    {
                        requestMessage.PolicyNo = policyNo;
                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                    }
                }
            }

            string errors = string.Empty;
            DateTime beforeCallingQuoteService = DateTime.Now;
            predefinedLogInfo.VehicleAgencyRepair = requestMessage.VehicleAgencyRepair;
            predefinedLogInfo.City = requestMessage.InsuredCity;
            predefinedLogInfo.ChassisNumber = requestMessage.VehicleChassisNumber;
            var response = RequestQuotationProducts(requestMessage, output.QuotationResponse, insuranceCompany, predefinedLogInfo, automatedTest, out errors);
            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(beforeCallingQuoteService).TotalSeconds;
            if (response == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response is null due to errors, " + errors;
                return output;
            }
            if (response.Products == null)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products is null due to errors, " + errors;
                return output;
            }
            if (response.Products.Count() == 0)
            {
                output.ErrorCode = QuotationNewOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products.Count() is null due to errors, " + errors;
                return output;
            }
            output.Products = response.Products;
            var products = new List<Product>();
            DateTime beforeHandlingProducts = DateTime.Now;
            var allBenefitst = _benefitRepository.Table.ToList();
            var allPriceTypes = _priceTypeRepository.Table.ToList();
            foreach (var p in response.Products)
            {
                var product = p.ToEntity();
                if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode)
                    && (insuranceCompaniesExcluedFromSchemesQuotations != null && !insuranceCompaniesExcluedFromSchemesQuotations.Contains(insuranceCompany.InsuranceCompanyID)))
                    product.IsPromoted = true;
                product.ProviderId = insuranceCompany.InsuranceCompanyID;
                if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                    product.InsuranceTypeCode = insuranceTypeCode;

                if (product.Product_Benefits != null)
                {
                    if (insuranceCompany.InsuranceCompanyID == Convert.ToInt32(InsuranceCompanyEnum.SAICO))
                    {
                        var removedbenefit = product.Product_Benefits.FirstOrDefault(a => a.BenefitId == 15 || a.BenefitExternalId == "15");
                        if (removedbenefit!=null)
                            product.Product_Benefits.Remove(removedbenefit);
                    }
                    var ben = product.Product_Benefits.Select(a => a.BenefitId == 15 || a.BenefitExternalId == "15");
                    foreach (var pb in product.Product_Benefits)
                    {
                      
                        pb.Benefit = allBenefitst.FirstOrDefault(bf => pb.BenefitId.HasValue && bf.Code == pb.BenefitId.Value);
                        if (pb.BenefitId == 0)
                        {
                            var serviceBenfitInfo = p.Benefits.Where(a => a.BenefitId == pb.BenefitExternalId).FirstOrDefault();
                            if (serviceBenfitInfo != null)
                            {
                                pb.BenefitNameAr = serviceBenfitInfo.BenefitNameAr;
                                pb.BenefitNameEn = serviceBenfitInfo.BenefitNameEn;
                            }
                        }
                        else
                        {
                            pb.BenefitNameAr = pb.Benefit.ArabicDescription;
                            pb.BenefitNameEn = pb.Benefit.EnglishDescription;
                        }
                        if (pb.BenefitId == 7 && vehicleAgencyRepair == true && insuranceTypeCode != 9)
                        {
                            pb.IsSelected = true;
                        }
                    }

                
                }
                product.CreateDateTime = DateTime.Now;
                product.ReferenceId = output.QuotationResponse.ReferenceId;

                // Load price details from database.
                foreach (var pd in product.PriceDetails)
                {
                    pd.IsCheckedOut = false;
                    pd.CreateDateTime = DateTime.Now;
                    pd.PriceType = allPriceTypes.FirstOrDefault(pt => pt.Code == pd.PriceTypeCode);
                }
                product.QuotaionNo = response.QuotationNo;
                products.Add(product);
            }
            output.QuotationResponse.Products = products;
            if (!string.IsNullOrEmpty(promotionProgramCode) && promotionProgramId != 0)
            {
                output.QuotationResponse.PromotionProgramCode = promotionProgramCode;
                output.QuotationResponse.PromotionProgramId = promotionProgramId;
            }
            if (!string.IsNullOrEmpty(quoteRequest.InsuredCityYakeenCode.ToString()))
                output.QuotationResponse.CityId = quoteRequest.InsuredCityYakeenCode;
            output.QuotationResponse.ICQuoteReferenceNo = response.QuotationNo;
            _quotationResponseRepository.Insert(output.QuotationResponse);
            log.ProductResponseTimeInSeconds = DateTime.Now.Subtract(beforeHandlingProducts).TotalSeconds;
            output.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(output.QuotationResponse.Products).ToList();
            if (insuranceTypeCode == 1 && insuranceCompany.InsuranceCompanyID != Convert.ToInt32(InsuranceCompanyEnum.Wataniya)
              && insuranceCompany.InsuranceCompanyID != Convert.ToInt32(InsuranceCompanyEnum.UCA) && insuranceCompany.InsuranceCompanyID != Convert.ToInt32(InsuranceCompanyEnum.ArabianShield) && insuranceCompany.InsuranceCompanyID != Convert.ToInt32(InsuranceCompanyEnum.SAICO))
            {
                var tplbenefit = allBenefitst.Where(a => a.Code == 14).FirstOrDefault();
                if (tplbenefit != null)
                {
                    Product_Benefit prodBenefit = new Product_Benefit();
                    prodBenefit.Benefit = tplbenefit;
                    prodBenefit.BenefitNameAr = tplbenefit.ArabicDescription;
                    prodBenefit.BenefitNameEn = tplbenefit.EnglishDescription;
                    prodBenefit.BenefitId = tplbenefit.Code;
                    prodBenefit.BenefitExternalId = tplbenefit.Code.ToString();
                    prodBenefit.IsSelected = true;
                    prodBenefit.IsReadOnly = true;
                    output.QuotationResponse.Products.FirstOrDefault()?.Product_Benefits?.Add(prodBenefit);
                }

            }

            if (quoteRequest.IsRenewal.HasValue && quoteRequest.IsRenewal.Value && insuranceCompany.InsuranceCompanyID != 13)
            {
                output.ActiveTabbyComp = true;
                output.ActiveTabbyTPL = true;
                output.ActiveTabbySanadPlus = true;
                output.ActiveTabbyWafiSmart = true;
                output.ActiveTabbyMotorPlus = true;
                output.IsRenewal = true;
            }
            else
            {
                output.ActiveTabbyComp = insuranceCompany.ActiveTabbyComp;
                output.ActiveTabbyTPL = insuranceCompany.ActiveTabbyTPL;
                output.ActiveTabbySanadPlus = insuranceCompany.ActiveTabbySanadPlus;
                output.ActiveTabbyWafiSmart = insuranceCompany.ActiveTabbyWafiSmart;
                output.ActiveTabbyMotorPlus = insuranceCompany.ActiveTabbyMotorPlus;
            }

            output.ErrorCode = QuotationNewOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.LogDescription = "Success";
            return output;
        }

        private string getNewReferenceId()
        {
            string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (_quotationResponseRepository.TableNoTracking.Any(a => a.ReferenceId == referenceId))
                return getNewReferenceId();
            return referenceId;
        }

        private async Task<QuotationNewRequestDetails> GetQuotationRequestDetailsByExternalId(string externalId)
        {
            //QuotationNewRequestDetails requests = await _redisCacheManager.GetAsync<QuotationNewRequestDetails>($"{quotationRequestDetailsCach_Base_KEY}_{externalId}");
            //if (requests != null)
            //    return requests;

            //var scope = EngineContext.Current.ContainerManager.Scope();
            //var providerType = Type.GetType("TameenkObjectContext, Tameenk.Data");
            //IDbContext _dbContext = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope) as IDbContext;
            //var scheduleTaskService = EngineContext.Current.ContainerManager.Resolve<IDbContext>("", scope);

            //By Niaz Upgrade-Assistant todo
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                // dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestDetailsByExternalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(nationalIDParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                QuotationNewRequestDetails requests = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationNewRequestDetails>(reader).FirstOrDefault();
                if (requests != null)
                {
                    requests.AdditionalDrivers = new List<Driver>();
                    requests.MainDriverViolation = new List<DriverViolation>();
                    requests.MainDriverLicenses = new List<DriverLicense>();
                    reader.NextResult();
                    requests.AdditionalDrivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Driver>(reader).ToList();
                    reader.NextResult();
                    requests.MainDriverViolation = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                    reader.NextResult();
                    requests.MainDriverLicenses = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                }

                //RedisCacheManager redisCacheManager = RedisCacheManager.Instance;
                await RedisCacheManager.Instance.SetAsync($"{quotationRequestDetailsCach_Base_KEY}_{externalId}", requests, quotationResponseCach_TiMe);
                return requests;
            }
            catch (Exception exp)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\GetQuotationRequestDetailsByExternalId_exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

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
    
            if (quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.UCA) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.AlRajhi)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Solidarity) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Walaa)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.AICC) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Allianz)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.GulfUnion) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.TUIC))
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

        private QuotationServiceResponse RequestQuotationProducts(QuotationServiceRequest requestMessage, QuotationResponse quotationResponse, InsuranceCompany insuranceCompany, ServiceRequestLog predefinedLogInfo, bool automatedTest, out string errors)
        {
            errors = string.Empty;
            try
            {
                requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

                QuotationServiceResponse results = null;
                IInsuranceProvider provider = null;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName + quotationResponse.InsuranceTypeCode);
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
                            //not resolved
                            instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                        }
                        provider = instance as IInsuranceProvider;
                    }
                    if (provider == null)
                    {
                        throw new Exception("Unable to find provider.");
                    }
                    if (insuranceCompany.Key != "Tawuniya")
                        Utilities.AddValueToCache("instance_" + providerFullTypeName + quotationResponse.InsuranceTypeCode, instance, 1440);

                    if (provider != null)
                    {
                        if (predefinedLogInfo.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                        {
                            results = provider.GetQuotationAutoleasing(requestMessage, predefinedLogInfo);
                        }
                        else
                        {
                            results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                        }
                    }
                    scope.Dispose();
                }
                else
                {
                    if (provider != null)
                    {
                        if (predefinedLogInfo.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                        {
                            results = provider.GetQuotationAutoleasing(requestMessage, predefinedLogInfo);
                        }
                        else
                        {
                            results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                        }
                    }
                }
                // Remove products if price is zero
                if (results != null && results.Products != null)
                {

                    results.Products = results.Products.Where(e => e.ProductPrice > 0).ToList();

                    var showZeroPremium = _tameenkConfig.Quotatoin.showZeroPremium;

                    if (showZeroPremium)
                    {
                        // Remove products if basic perineum equal zero
                        results.Products = results.Products.Where(e => e.PriceDetails.Any(p => p.PriceTypeCode == 7 && p.PriceValue > 0)).ToList();

                    }
                    // Remove benefits if price is zero
                    foreach (var prod in results.Products)
                    {
                        if (prod.Benefits != null && prod.Benefits.Count() > 0)
                        {
                            prod.Benefits = prod.Benefits.Where(e => e.BenefitPrice > 0 || (e.IsReadOnly && e.IsSelected == true)).ToList();
                        }
                    }
                }

                return results;
            }
            catch (Exception exp)
            {
                errors = exp.ToString();
                return null;
            }

        }

        private void GetVehicleColor(out string vehicleColor, out long vehicleColorCode, string vehicleMajorColor, int companyId)
        {
            vehicleColor = vehicleMajorColor; //default value
            vehicleColorCode = 99;//default value
            var secondMajorCollor = string.Empty;
            if (vehicleMajorColor[0] == 'ا')
                secondMajorCollor = 'أ' + vehicleMajorColor.Substring(1);
            else if (vehicleMajorColor[0] == 'أ')
                secondMajorCollor = 'ا' + vehicleMajorColor.Substring(1);
            else
                secondMajorCollor = vehicleMajorColor;
            var vehiclesColors = GetVehicleColors();
            var vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor || color.ArabicDescription == secondMajorCollor);
            if (vColor == null)
            {
                if (vehicleMajorColor.Contains(' '))
                {
                    vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor.Split(' ')[0] || color.ArabicDescription == secondMajorCollor.Split(' ')[0]);
                    if (vColor != null)
                    {
                        vehicleColor = vColor.YakeenColor;
                        vehicleColorCode = (companyId == 12) ? vColor.TawuniyaCode.Value : (companyId == 14) ? vColor.WataniyaCode.Value : vColor.YakeenCode;
                    }
                }
            }
            else
            {
                vehicleColor = vColor.YakeenColor;
                vehicleColorCode = (companyId == 12) ? vColor.TawuniyaCode.Value : (companyId == 14) ? vColor.WataniyaCode.Value : vColor.YakeenCode;
            }
        }

        private City GetCityByName(List<City> Citites, string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                City _city = Citites.FirstOrDefault(c => c.ArabicDescription == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_city == null)
                    _city = Citites.FirstOrDefault(c => c.EnglishDescription == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_city == null)
                {
                    if (Name.Trim().Contains("ه"))
                        _city = Citites.FirstOrDefault(c => c.ArabicDescription == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ه", "ة")));
                    else if (_city == null && Name.Trim().Contains("ة"))
                        _city = Citites.FirstOrDefault(c => c.EnglishDescription == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ة", "ه")));
                }
                if (_city != null)
                    return _city;
                else
                    return null;
            }
            return null;
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

        public VehicleInsurance.VehicleModel GetVehicleModelByMakerCodeAndModelCode(short vehicleMakerId, long vehicleModelId)
        {
            var vehicleModel = _vehicleModelRepository.TableNoTracking.Where(a => a.VehicleMakerCode == vehicleMakerId && a.Code == vehicleModelId).FirstOrDefault();
            return vehicleModel;
        }

        public int GetWataiyaPlateLetterId(string letter)
        {
            int letterId = 0;
            var letterData = _vehiclePlateTextRepository.TableNoTracking.Where(a => a.ArabicDescription == letter).FirstOrDefault();
            if (letterData != null && letterData.WataniyaCode.HasValue)
                letterId = letterData.WataniyaCode.Value;

            return letterId;
        }

        private IEnumerable<Product> ExcludeProductOrBenefitWithZeroPrice(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                var productBenefits = new List<Product_Benefit>();
                productBenefits.AddRange(product.Product_Benefits.Where(x => x.BenefitPrice > 0 || (x.IsReadOnly && x.IsSelected.HasValue && x.IsSelected == true)));
                product.Product_Benefits = productBenefits;
            }

            return products.Where(x => x.ProductPrice > 0);
        }

        #endregion


        #region Get From Caching

        private InsuranceCompany GetById(int insuranceCompanyId)
        {
            return GetAllinsuranceCompany().FirstOrDefault(ic => ic.InsuranceCompanyID == insuranceCompanyId);
        }

        private IEnumerable<InsuranceCompany> GetAllinsuranceCompany()
        {
            return _cacheManager.Get("tameenk.insurance.companies.all", 20, () =>
            {
                return _insuranceCompanyRepository.TableNoTracking.Include(c => c.Contact).ToList();
            });
        }

        private List<City> GetAllCities(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("_CITY__aLl_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _cityRepository.TableNoTracking.ToList();
            });
        }

        private IList<VehicleColor> GetVehicleColors()
        {
            return _cacheManager.Get("tameenk.vehiclColor.all", () =>
            {
                return _vehicleColorRepository.Table.ToList();
            });
        }

        private IPagedList<VehicleInsurance.VehicleMaker> VehicleMakers(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("tameenk.vehiclMaker.all.{0}.{1}", pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleInsurance.VehicleMaker>(_vehicleMakerRepository.Table.OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }

        private IPagedList<VehicleInsurance.VehicleModel> VehicleModels(int vehicleMakerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string vehicleMakerCode = vehicleMakerId.ToString();
            return _cacheManager.Get(string.Format("tameenk.vehiclMaker.all.{0}.{1}.{2}", vehicleMakerId, pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleInsurance.VehicleModel>(_vehicleModelRepository.Table.Where(e => e.VehicleMakerCode == vehicleMakerId).OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }

        private List<LicenseType> GetAllLicenseType(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("_License___typE_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _licenseTypeRepository.TableNoTracking.ToList();
            });
        }

        #endregion
    }
}
