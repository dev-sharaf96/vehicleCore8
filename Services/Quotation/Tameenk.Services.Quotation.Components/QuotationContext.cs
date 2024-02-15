using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Resources.WebResources;
using System.Globalization;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Settings;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Security.Extensions;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.InsuranceCompanies;
using DocumentFormat.OpenXml.Packaging;
using Tameenk.Integration.Core.Providers;
using System.IO;
using Tameenk.Core.Infrastructure;
using Tameenk.Core;
using Tameenk.Services.Core.Quotations;
using Newtonsoft.Json;
using Tameenk.Data;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core;
using Tameenk.Resources.Quotations;
using System.Net.Http;
using System.Text;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationContext : IQuotationContext
    {

        #region Fields

        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IAddressService _addressService;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<PromotionProgramUser> _promotionProgramUSerRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly TameenkConfig _config;
        private readonly IAuthorizationService _authorizationService;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ISettingService _settingService;
        private readonly IPromotionService _promotionService;
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<AutomatedTestIntegrationTransaction> _automatedTestIntegrationTransactionRepository;
        private readonly IRepository<PriceType> _priceTypeRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<InsuredExtraLicenses> _insuredExtraLicenses;
        private readonly IQuotationService _quotationService;
        private readonly IRepository<DriverViolation> driverViolationRepository;        private readonly IRepository<AutoleasingSelectedBenifits> _autoLeasingSelectedBenfitsRepository;        private readonly IRepository<Benefit> _benfitRepository;        private readonly IPolicyService _policyService;        private readonly IRepository<Bank> _bankRepository;        private readonly IRepository<BankNins> _bankNinsRepository;        private readonly IRepository<AutoleasingDepreciationSetting> _autoleasingDepreciationSettingRepository;        private readonly IRepository<AutoleasingAgencyRepair> _autoleasingRepairMethodRepository;        private readonly IRepository<AutoleasingMinimumPremium> _autoleasingMinimumPremiumRepository;        private const double VAT = 0.15;
        private readonly IRepository<Product_Benefit> _productBenefitRepository;        private readonly IRepository<AutoleasingDepreciationSettingHistory> _autoleasingDepreciationSettingHistoryRepository;        private readonly IRepository<AutoleasingAgencyRepairHistory> _autoleasingRepairMethodHistoryRepository;        private readonly IRepository<AutoleasingMinimumPremiumHistory> _autoleasingMinimumPremiumHistoryRepository;        private readonly IRepository<AutoleasingUser> _autoleasingUser;        private readonly IAutoleasingUserService _autoleasingUserService;        private readonly IBankService _bankService;
        private readonly IAutoleasingQuotationFormService _autoleasingQuotationFormService;
        private readonly IRepository<DriverExtraLicense> _driverExtraLicenses;
        private readonly IRepository<AutoleasingInitialQuotationCompanies> _autoleasingInitialQuotationCompaniesRepository;
        private readonly INotificationService _notificationService;
        private readonly IRepository<QuotationShares> _quotationShares;
        private readonly IRepository<PolicyModification> _policyModificationRepository;
        private readonly IRepository<PolicyAdditionalBenefit> _policyAdditionalBenefitRepository;
        private readonly IRepository<AutoleasingQuotationFormSettings> _autoleasingQuotationFormSettingsRepository;

        private readonly List<int> InsuranceCompaniesThatExcludeDriversAbove18 = new List<int>() { 19, 20 };

        #endregion

        public QuotationContext(IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            TameenkConfig config, IAuthorizationService authorizationService,
            ISettingService settingService, IPromotionService promotionService,
            IRepository<AutomatedTestIntegrationTransaction> automatedTestIntegrationTransactionRepository,
            IRepository<Benefit> benefitRepository, IInsuranceCompanyService insuranceCompanyService,
            IRepository<PriceType> priceTypeRepository,
            IAddressService addressService,
            IVehicleService vehicleService,
            IRepository<Driver> driverRepository,
            IRepository<PromotionProgramUser> promotionProgramUSerRepository,
            IRepository<InsuranceCompany> insuranceCompanyRepository, TameenkConfig tameenkConfig,
            IRepository<InsuredExtraLicenses> insuredExtraLicenses,
            IQuotationService quotationService, IRepository<DriverViolation> driverViolationRepository,
            IRepository<AutoleasingSelectedBenifits> autoLeasingSelectedBenfitsRepository, IRepository<Benefit> benfitRepository,
            IPolicyService policyService, IRepository<Bank> bankRepository, IRepository<BankNins> bankNinsRepository,
            IRepository<AutoleasingDepreciationSetting> autoleasingDepreciationSettingRepository,
            IRepository<AutoleasingAgencyRepair> autoleasingRepairMethodRepository, IRepository<AutoleasingMinimumPremium> autoleasingMinimumPremiumRepository,
            IRepository<Product_Benefit> productBenefitRepository, IRepository<AutoleasingDepreciationSettingHistory> autoleasingDepreciationSettingHistoryRepository,
            IRepository<AutoleasingAgencyRepairHistory> autoleasingRepairMethodHistoryRepository, IRepository<AutoleasingMinimumPremiumHistory> autoleasingMinimumPremiumHistoryRepository,
            IRepository<AutoleasingUser> autoleasingUser, IAutoleasingQuotationFormService autoleasingQuotationFormService,
            IAutoleasingUserService autoleasingUserService, IBankService bankService, IRepository<DriverExtraLicense> driverExtraLicenses,
            IRepository<AutoleasingInitialQuotationCompanies> autoleasingInitialQuotationCompaniesRepository
            , INotificationService notificationService, IRepository<QuotationShares> quotationShares,
           IRepository<PolicyModification> policyModificationRepository,
            IRepository<PolicyAdditionalBenefit> policyAdditionalBenefitRepository,
            IRepository<AutoleasingQuotationFormSettings> autoleasingQuotationFormSettingsRepository)
        {
            this._quotationRequestRepository = quotationRequestRepository;
            this._quotationResponseRepository = quotationResponseRepository;
            this._settingService = settingService;
            this._priceTypeRepository = priceTypeRepository;
            this._authorizationService = authorizationService;
            this._insuranceCompanyService = insuranceCompanyService;
            this._benefitRepository = benefitRepository;
            this._automatedTestIntegrationTransactionRepository = automatedTestIntegrationTransactionRepository;
            this._promotionService = promotionService;
            this._config = config;
            _addressService = addressService;
            _vehicleService = vehicleService;
            _driverRepository = driverRepository;
            _promotionProgramUSerRepository = promotionProgramUSerRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _tameenkConfig = tameenkConfig;
            _insuredExtraLicenses = insuredExtraLicenses;
            _quotationService = quotationService ?? throw new ArgumentNullException(nameof(quotationService));
            this.driverViolationRepository = driverViolationRepository;            _autoLeasingSelectedBenfitsRepository = autoLeasingSelectedBenfitsRepository;
            _benfitRepository = benfitRepository;
            _policyService = policyService;
            _bankRepository = bankRepository;
            _bankNinsRepository = bankNinsRepository;
            _autoleasingDepreciationSettingRepository = autoleasingDepreciationSettingRepository;
            _autoleasingRepairMethodRepository = autoleasingRepairMethodRepository;
            _autoleasingMinimumPremiumRepository = autoleasingMinimumPremiumRepository;
            _productBenefitRepository = productBenefitRepository;
            _autoleasingDepreciationSettingHistoryRepository = autoleasingDepreciationSettingHistoryRepository;
            _autoleasingRepairMethodHistoryRepository = autoleasingRepairMethodHistoryRepository;
            _autoleasingMinimumPremiumHistoryRepository = autoleasingMinimumPremiumHistoryRepository;
            _autoleasingUser = autoleasingUser;
            _autoleasingQuotationFormService = autoleasingQuotationFormService;
            _autoleasingUserService = autoleasingUserService;
            _bankService = bankService;
            _driverExtraLicenses = driverExtraLicenses;
            _autoleasingInitialQuotationCompaniesRepository = autoleasingInitialQuotationCompaniesRepository;
            _notificationService = notificationService;
            _quotationShares = quotationShares;
            _policyModificationRepository = policyModificationRepository;
            _policyAdditionalBenefitRepository = policyAdditionalBenefitRepository;
            _autoleasingQuotationFormSettingsRepository = autoleasingQuotationFormSettingsRepository;
        }



        public QuotationOutput GetQuote(int insuranceCompanyId, string qtRqstExtrnlId, string channel, Guid userId, string userName, QuotationRequestLog log, DateTime excutionStartDate, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, string policyNo = null, string policyExpiryDate = null, string hashed = null, bool OdQuotation = false)
        {
            QuotationOutput output = new QuotationOutput();
            output.QuotationResponse = new QuotationResponse();
            try
            {
               if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceCompanyId == 0)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
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
                        output.ErrorCode = QuotationOutput.ErrorCodes.InvalidODPolicyExpiryDate;
                        output.ErrorDescription = SubmitInquiryResource.InvalidODPolicyExpiryDate;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"Can't proceed OD request , as TPL policy expiry date is {oldPolicyExpiryDate.Value} and it's less than 90 days";
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }
                if (insuranceCompanyId == 22 && insuranceTypeCode == 9&&!OdQuotation)
                {
                    if (string.IsNullOrEmpty(hashed))
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
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
                        output.ErrorCode = QuotationOutput.ErrorCodes.HashedNotMatched;
                        output.ErrorDescription = SubmitInquiryResource.ErrorHashing;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                        log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                        QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                        return output;
                    }
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyId);
                log.CompanyName = insuranceCompany.Key;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 2 && !insuranceCompany.IsActiveComprehensive)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "Comprehensive products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Comprehensive products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceTypeCode == 1 && !insuranceCompany.IsActiveTPL)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.ComprehensiveIsNotAvailable;
                    output.ErrorDescription = "TPL products is not supported";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "TPL products is not supported";
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }

                if (insuranceCompany.Key.ToLower() == "tawuniya" || insuranceTypeCode == 1)
                {
                    deductibleValue = null;
                }
                else if (!deductibleValue.HasValue)
                {
                    deductibleValue = (int?)2000;
                }

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompanyId;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;
                predefinedLogInfo.Channel = channel.ToString();
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;
                //if (log.UserAgent.Contains("Tameenak/1"))
                //{
                //    predefinedLogInfo.Channel = "ios";
                //}
                //if (log.UserAgent.Contains("okhttp/"))
                //{
                //    predefinedLogInfo.Channel = "android";
                //}
                output = GetQuotationResponseDetails(insuranceCompany, qtRqstExtrnlId, predefinedLogInfo, log, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, policyNo: policyNo, policyExpiryDate: policyExpiryDate, OdQuotation: OdQuotation);
                //log.RefrenceId = output.QuotationResponse.ReferenceId;
                if (output.ErrorCode != QuotationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.LogDescription;
                    log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
               
                if (OdQuotation)                {                    output.QuotationResponse.InsuranceCompany.ShowQuotationToUser = false;                }
                if (insuranceCompany.AllowAnonymousRequest.HasValue && insuranceCompany.AllowAnonymousRequest.Value)
                {
                    output.QuotationResponse.CompanyAllowAnonymous = true;
                }
                if (userId != Guid.Empty)
                {
                    output.QuotationResponse.AnonymousRequest = false;
                }
                if (insuranceCompany.ShowQuotationToUser.HasValue && !insuranceCompany.ShowQuotationToUser.Value)
                {
                    output.QuotationResponse.Products = null;
                }
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

                //if (insuranceCompanyId == 14 && log.UserId != "1b4cdb65-9804-4ab4-86a8-af62bf7812d7" && log.UserId != "ebf4df2c-c9bb-4d7d-91fe-4b9208c1631a")
                //    output.QuotationResponse.Products = null;

                //if (insuranceCompanyId == 5 && insuranceTypeCode == 2 && log.UserId != "1b4cdb65-9804-4ab4-86a8-af62bf7812d7" && log.UserId != "ebf4df2c-c9bb-4d7d-91fe-4b9208c1631a") // As per Fayssal @ 22-03-2023 2:45 PM
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

                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                log.TotalResponseTimeInSeconds = DateTime.Now.Subtract(excutionStartDate).TotalSeconds;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }
        }


        public QuotationOutput GetQuotation(InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false, string policyNo = null, string policyExpiryDate = null, bool OdQuotation = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();

            QuotationOutput output = new QuotationOutput();
            DateTime startDateTime = DateTime.Now;
            string referenceId = string.Empty;
            //if (insuranceCompany.InsuranceCompanyID == 14)
            //    referenceId = CreateWataniyaReference();
            //else
                referenceId = getNewReferenceId();

            log.RefrenceId = referenceId;
            DateTime beforeCallingDB = DateTime.Now;
            var quoteRequest = _quotationService.GetQuotationRequestFromCachingWithExternalId(qtRqstExtrnlId);
            log.DabaseResponseTimeInSeconds = DateTime.Now.Subtract(beforeCallingDB).TotalSeconds;
            if (quoteRequest == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest is null";
                return output;
            }
            if (quoteRequest.Insured == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Insured is null";
                return output;
            }
            log.NIN = quoteRequest.Insured.NationalId;
            if (quoteRequest.Driver == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Driver is null";
                return output;
            }
            predefinedLogInfo.DriverNin = quoteRequest.Driver.NIN;
            if (quoteRequest.Vehicle == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Vehicle is null ";
                return output;
            }
            if (insuranceCompany.InsuranceCompanyID == 8 && 
                quoteRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard 
                && (quoteRequest.Vehicle.VehicleBodyCode == 1 || quoteRequest.Vehicle.VehicleBodyCode == 2 
                || quoteRequest.Vehicle.VehicleBodyCode == 3 || quoteRequest.Vehicle.VehicleBodyCode == 19 
                || quoteRequest.Vehicle.VehicleBodyCode == 20))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = "No supported product with medgulf with such information";
                output.LogDescription = "MedGulf Invalid Body Type with Custom Card body type is "+ quoteRequest.Vehicle.VehicleBodyCode;
                return output;
            }

            if (quoteRequest.Vehicle.Cylinders >= 0 && quoteRequest.Vehicle.Cylinders <= 4)
            {
                quoteRequest.Vehicle.EngineSizeId = 1;
            }
            else if (quoteRequest.Vehicle.Cylinders >= 5 && quoteRequest.Vehicle.Cylinders <= 7)
            {
                quoteRequest.Vehicle.EngineSizeId = 2;
            }
            else
            {
                quoteRequest.Vehicle.EngineSizeId = 3;
            }

            if (quoteRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
                predefinedLogInfo.VehicleId = quoteRequest.Vehicle.CustomCardNumber;
            else
                predefinedLogInfo.VehicleId = quoteRequest.Vehicle.SequenceNumber;
            log.VehicleId = predefinedLogInfo.VehicleId;
            if (quoteRequest.Insured.NationalId.StartsWith("7") && !quoteRequest.Vehicle.OwnerTransfer &&
                (insuranceCompany.InsuranceCompanyID == 12 || insuranceCompany.InsuranceCompanyID == 14))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for Tawuniya and Wataniya";
                return output;
            }
            if (quoteRequest.Insured.NationalId.StartsWith("7") && insuranceCompany.InsuranceCompanyID == 25) //AXA
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for AXA";
                return output;
            }
            //if (quoteRequest.RequestPolicyEffectiveDate.HasValue && quoteRequest.RequestPolicyEffectiveDate.Value.Date == DateTime.Now.Date)
            //{
            //    quoteRequest.RequestPolicyEffectiveDate = quoteRequest.RequestPolicyEffectiveDate.Value.AddDays(1);
            //    var quoteRequestInfo = _quotationRequestRepository.Table.Where(a => a.ExternalId == qtRqstExtrnlId);
            //    if (quoteRequestInfo != null)
            //    {
            //        _quotationRequestRepository.Update(quoteRequestInfo);
            //    }
            //}
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
            DateTime beforeGettingRequestMessage= DateTime.Now;
            var requestMessage = GetQuotationRequest(quoteRequest, output.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue, out promotionProgramCode, out promotionProgramId);
            log.RequestMessageResponseTimeInSeconds = DateTime.Now.Subtract(beforeGettingRequestMessage).TotalSeconds;
            if (insuranceCompany.InsuranceCompanyID == 21 && string.IsNullOrEmpty(requestMessage.PromoCode))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "PromoCode is null for Saico " ;
                return output;
            }
            if (insuranceCompany.Key.ToLower() == "malath")
            {
                if (insuranceTypeCode == 2)
                    requestMessage.DeductibleValue = null;
                else if (insuranceTypeCode == 9)
                {
                    //requestMessage.PolicyNo = policyNo;
                    //requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);
                    if (OdQuotation)                    {                        requestMessage.PolicyNo = "new";                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(DateTime.UtcNow.AddYears(1).ToString());                    }                    else                    {                        requestMessage.PolicyNo = policyNo;                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);                    }
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
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response is null due to errors, " + errors;
                return output;
            }
            if (response.Products == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products is null due to errors, " + errors;
                return output;
            }
            if (response.Products.Count() == 0)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
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
                if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode))
                    product.IsPromoted = true;
                product.ProviderId = insuranceCompany.InsuranceCompanyID;
                if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                    product.InsuranceTypeCode = insuranceTypeCode;

                if (product.Product_Benefits != null)
                {
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
                        if (pb.BenefitId == 7 && vehicleAgencyRepair == true&&insuranceTypeCode!=9)
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
            if (quoteRequest.Insured.City != null)
                output.QuotationResponse.CityId = quoteRequest.Insured.City.YakeenCode;
            output.QuotationResponse.ICQuoteReferenceNo = response.QuotationNo;
            _quotationResponseRepository.Insert(output.QuotationResponse);
            log.ProductResponseTimeInSeconds = DateTime.Now.Subtract(beforeHandlingProducts).TotalSeconds;
            output.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(output.QuotationResponse.Products).ToList();
            if(insuranceTypeCode==1&& insuranceCompany.InsuranceCompanyID != 14&& insuranceCompany.InsuranceCompanyID!=17 && insuranceCompany.InsuranceCompanyID != 9)
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

            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.LogDescription = "Success";
            return output;
        }
        private string getNewReferenceId()
        {
            string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            //if (_quotationResponseRepository.TableNoTracking.Any(q => q.ReferenceId == referenceId))
            //    return getNewReferenceId();
           if(_quotationService.IsQuotationResponseExist(referenceId))
                return getNewReferenceId();
            return referenceId;
        }

        private string getNewReferenceId(int referenceIdLength)
        {
            string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, referenceIdLength);
            if (_quotationService.IsQuotationResponseExist(referenceId))
                return getNewReferenceId(referenceIdLength);
            return referenceId;
        }

        private string CreateWataniyaReference()
        {
            Random random = new Random();
            string stringReference = string.Empty;
            for (int i = 1; i <= 10; i++)
                stringReference += random.Next(1, 9).ToString();

            if (_quotationService.IsQuotationResponseExist(stringReference))
                return CreateWataniyaReference();

            return stringReference;
        }

        private List<DriverDto> CreateInsuranceCompanyDriversFromDataRequest(QuotationRequest quotationRequest, List<City> cities,int insuranceCompanyId)
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

            // this is with Alamia
            if (insuranceCompanyId == 18)
                mainDriverDto.DriverZipCode = quotationRequest.PostCode;

            if (quotationRequest.Insured.Gender == Gender.Male)
                mainDriverDto.DriverGenderCode = "M";
            else if (quotationRequest.Insured.Gender == Gender.Female)
                mainDriverDto.DriverGenderCode = "F";
            else
                mainDriverDto.DriverGenderCode = "M";
            // mainDriverDto.DriverGenderCode = quotationRequest.Insured.Gender.GetCode();

            mainDriverDto.DriverNationalityCode = !string.IsNullOrEmpty(quotationRequest.Insured.NationalityCode) ? quotationRequest.Insured.NationalityCode : "113";

            //if ((insuranceCompanyId == 2 || insuranceCompanyId == 8
            //      || insuranceCompanyId == 11 || insuranceCompanyId == 13
            //      || insuranceCompanyId == 19) && mainDriverDto.DriverGenderCode == "M")
            //{
            //    mainDriverDto.DriverSocialStatusCode = "2";
            //}
            //else if ((insuranceCompanyId == 2 || insuranceCompanyId == 8
            //    || insuranceCompanyId == 11 || insuranceCompanyId == 13
            //    || insuranceCompanyId == 19) && mainDriverDto.DriverGenderCode == "F")
            //{
            //    mainDriverDto.DriverSocialStatusCode = "4";
            //}
            //else
            //{
                mainDriverDto.DriverSocialStatusCode = quotationRequest.Driver.SocialStatusId?.ToString();
            //}
            var mainDriverOccupation = quotationRequest.Insured.Occupation;
            if (mainDriverOccupation == null && mainDriverDto.DriverIdTypeCode == 1)
            {
                mainDriverDto.DriverOccupationCode = "O";
                mainDriverDto.DriverOccupation = "غير ذالك";
            }
            else if (mainDriverOccupation == null && mainDriverDto.DriverIdTypeCode == 2)
            {
                mainDriverDto.DriverOccupationCode = "31010";
                mainDriverDto.DriverOccupation = "موظف اداري";
            }
            else
            {
                if ((string.IsNullOrEmpty(mainDriverOccupation.Code) || mainDriverOccupation.Code == "o") && mainDriverDto.DriverIdTypeCode == 1)
                {
                    mainDriverDto.DriverOccupationCode = "O";
                    mainDriverDto.DriverOccupation = "غير ذالك";
                }
                else if ((string.IsNullOrEmpty(mainDriverOccupation.Code) || mainDriverOccupation.Code == "o") && mainDriverDto.DriverIdTypeCode == 2)
                {
                    mainDriverDto.DriverOccupationCode = "31010";
                    mainDriverDto.DriverOccupation = "موظف اداري";
                }
                else
                {
                    mainDriverDto.DriverOccupationCode = mainDriverOccupation.Code;
                    mainDriverDto.DriverOccupation = mainDriverOccupation.NameAr.Trim();
                }
            }
            if((!quotationRequest.Driver.DrivingPercentage.HasValue||quotationRequest.Driver.DrivingPercentage > 100 || quotationRequest.Driver.DrivingPercentage < 100) && quotationRequest.Drivers.Count == 1 )
            {
                mainDriverDto.DriverDrivingPercentage = 100;
            }
            else
            {
                mainDriverDto.DriverDrivingPercentage = quotationRequest.Driver.DrivingPercentage;
            }
            additionalDrivingPercentage = mainDriverDto.DriverDrivingPercentage.HasValue ? mainDriverDto.DriverDrivingPercentage.Value : 0; ;
            mainDriverDto.DriverEducationCode = quotationRequest.Insured.EducationId;
            if(!mainDriverDto.DriverEducationCode.HasValue || mainDriverDto.DriverEducationCode==0)
            {
                mainDriverDto.DriverEducationCode = 1;
            }
            mainDriverDto.DriverMedicalConditionCode = quotationRequest.Driver.MedicalConditionId;
            mainDriverDto.DriverChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
            mainDriverDto.DriverHomeCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.YakeenCode.ToString() : "";
            mainDriverDto.DriverHomeCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City.ArabicDescription : "";
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
                .FirstOrDefault(x => x.DriverId == quotationRequest.MainDriverId && x.IsDeleted == false)?
                .DriverLicenses;

            var LicenseDtos = new List<LicenseDto>();

            if (DriverLicenses != null && DriverLicenses.Count() > 0)
            {
                int licenseNumberYears;
                foreach (var item in DriverLicenses)
                {
                    int? _driverLicenseTypeCode = item.TypeDesc;
                    if (insuranceCompanyId == 14)
                        _driverLicenseTypeCode = _quotationService.GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

                    licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                    LicenseDtos.Add(new LicenseDto()
                    {
                        DriverLicenseExpiryDate =Utilities.HandleHijriDate(item.ExpiryDateH),
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
                .Where(d => d.InsuredId == quotationRequest.Insured.Id);

                // Main Driver Extra Licenses
                if (driversExtraLicenses != null && driversExtraLicenses.Any())
                {
                    var mainDriverExtraLicenses = driversExtraLicenses.Where(m => m.IsMainDriver == true);

                    if (mainDriverExtraLicenses != null && mainDriverExtraLicenses.Any())
                    {
                        LicenseDto licenseDto;
                        List<LicenseDto> license = new List<LicenseDto>() ;
                        foreach (var item in mainDriverExtraLicenses)
                        {
                            if (item.LicenseCountryCode < 1||item.LicenseCountryCode==113) //as jira 349
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
            var mainDriverViolations = driverViolationRepository.TableNoTracking
                              .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == quotationRequest.Driver.NIN).ToList();            if (mainDriverViolations != null && mainDriverViolations.Count > 0)            {
                mainDriverDto.DriverViolations = mainDriverViolations.Select(e => new ViolationDto()
                { ViolationCode = e.ViolationId }).ToList();            }
            //Add main driver to drivers list
            if (!quotationRequest.Insured.NationalId.StartsWith("7"))            {                if (insuranceCompanyId == 14)//Wataniya
                    HandleDriveAddressDetailsForWataniya(mainDriverDto);                drivers.Add(mainDriverDto);            }
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
                        DriverNCDFreeYears = 0,
                        DriverNCDReference = "0",
                        DriverRelationship = additionalDriver.RelationShipId ?? 0
                    };
                    if( insuranceCompanyId == 18) //Alamaya as per fayssal
                    {
                        driverDto.DriverRelationship = null;
                    }
                    if (quotationRequest.Insured.NationalId.StartsWith("7") && additionalDriver.NIN == additionalDrivers.ToList().FirstOrDefault().NIN)                    {                        driverDto.DriverTypeCode = 1;
                        driverDto.DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears;
                        driverDto.DriverNCDReference = quotationRequest.NajmNcdRefrence;                    }                    else
                    {
                        driverDto.DriverTypeCode = 2;
                    }
                    //driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                    if (additionalDriver.Gender == Gender.Male)
                        driverDto.DriverGenderCode = "M";
                    else if (additionalDriver.Gender == Gender.Female)
                        driverDto.DriverGenderCode = "F";
                    else
                        driverDto.DriverGenderCode = "M";

                    driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                            additionalDriver.NationalityCode.Value.ToString() : "113";
                    //driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                    //driverDto.DriverOccupation = additionalDriver.Occupation?.NameAr.Trim();

                   // var additionalDriverOccupation = quotationRequest.Insured.Occupation;
                    var additionalDriverOccupation = additionalDriver.Occupation;
                    if (additionalDriverOccupation == null && driverDto.DriverIdTypeCode == 1)
                    {
                        driverDto.DriverOccupationCode = "O";
                        driverDto.DriverOccupation = "غير ذالك";
                    }
                    else if(additionalDriverOccupation == null && driverDto.DriverIdTypeCode == 2)
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
                        driverDto.DriverHomeCityCode =mainDriverDto.DriverHomeCityCode;
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
                                _driverLicenseTypeCode = _quotationService.GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

                            licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                            additionalDriverLicenseDtos.Add(new LicenseDto()
                            {
                                DriverLicenseExpiryDate =Utilities.HandleHijriDate(item.ExpiryDateH),
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
                                     .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == additionalDriver.NIN);                    if (driverViolations != null && driverViolations.Count() > 0)                    {                        driverDto.DriverViolations = driverViolations                            .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();                    }

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

        private List<DriverDto> CreateInsuranceCompanyDriversFromDataRequestAutoleasing(QuotationRequest quotationRequest, List<City> cities, int insuranceCompanyId)
        {
            List<DriverDto> drivers = new List<DriverDto>();
            int additionalDrivingPercentage = 0;
            //Create main driver as first driver in the drivers list
            var mainDriverDto = new DriverDto();
            Occupation mainDriverOccupation;

            mainDriverDto = new DriverDto()
            {
                DriverTypeCode = 1,
                DriverId = long.Parse(quotationRequest.Driver.NIN),
                DriverIdTypeCode = quotationRequest.Driver.IsCitizen ? 1 : 2,
                DriverBirthDate = quotationRequest.Driver.DateOfBirthH,
                DriverBirthDateG = quotationRequest.Driver.DateOfBirthG,
                DriverFirstNameAr = quotationRequest.Driver.FirstName,
                DriverFirstNameEn = (string.IsNullOrWhiteSpace(quotationRequest.Driver.EnglishFirstName) ||
                    string.IsNullOrEmpty(quotationRequest.Driver.EnglishFirstName)) ? "-" : quotationRequest.Driver.EnglishFirstName,
                DriverMiddleNameAr = quotationRequest.Driver.SecondName,
                DriverMiddleNameEn = quotationRequest.Driver.EnglishSecondName,
                DriverLastNameAr = quotationRequest.Driver.LastName,
                DriverLastNameEn = (string.IsNullOrWhiteSpace(quotationRequest.Driver.EnglishLastName) ||
                    string.IsNullOrEmpty(quotationRequest.Driver.EnglishLastName)) ? "-" : quotationRequest.Driver.EnglishLastName,
                DriverNOALast5Years = quotationRequest.Driver.NOALast5Years,
                DriverNOCLast5Years = quotationRequest.Driver.NOCLast5Years,
                DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears,
                DriverNCDReference = quotationRequest.NajmNcdRefrence,
                DriverOccupation = quotationRequest.Driver.ResidentOccupation
            };

            // this is with MEdGulf & AICC
            if (insuranceCompanyId == 4 || insuranceCompanyId == 8)
                mainDriverDto.DriverZipCode = quotationRequest.Driver.PostCode;

            if (quotationRequest.Driver.Gender == Gender.Male)
                mainDriverDto.DriverGenderCode = "M";
            else if (quotationRequest.Driver.Gender == Gender.Female)
                mainDriverDto.DriverGenderCode = "F";
            else
                mainDriverDto.DriverGenderCode = "M";

            mainDriverDto.DriverNationalityCode = quotationRequest.Driver.NationalityCode.HasValue ? quotationRequest.Driver.NationalityCode.Value.ToString() : "113";
            mainDriverOccupation = quotationRequest.Driver.Occupation;
            mainDriverDto.DriverEducationCode = quotationRequest.Driver.EducationId;
            mainDriverDto.DriverChildrenBelow16Years = quotationRequest.Driver.ChildrenBelow16Years;

            if (quotationRequest.Driver.CityId.HasValue)
            {
                var city = cities.Where(c => c.Code == quotationRequest.Driver.CityId.Value).FirstOrDefault();
                mainDriverDto.DriverHomeCityCode = city?.YakeenCode.ToString();
                mainDriverDto.DriverHomeCity = city?.ArabicDescription;
            }

            if (quotationRequest.Driver.WorkCityId.HasValue)
            {
                var city = cities.Where(c => c.Code == quotationRequest.Driver.WorkCityId.Value).FirstOrDefault();
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

            // Get (Main & Additional Drivers Extra Licenses) // TO DO
            var mainDriverExtraLicenses = _driverExtraLicenses.TableNoTracking.Where(d => d.DriverId == quotationRequest.Driver.DriverId);

            // Main Driver Extra Licenses
            if (mainDriverExtraLicenses != null && mainDriverExtraLicenses.Any())
            {
                LicenseDto licenseDto;
                List<LicenseDto> license = new List<LicenseDto>();
                foreach (var item in mainDriverExtraLicenses)
                {
                    licenseDto = new LicenseDto();
                    licenseDto.LicenseNumberYears = item.LicenseYearsId;
                    licenseDto.LicenseCountryCode = item.CountryCode;
                    license.Add(licenseDto);

                }
                if (mainDriverDto.DriverLicenses != null)
                    mainDriverDto.DriverLicenses.AddRange(license);
                else
                    mainDriverDto.DriverLicenses = license;
            }

            var mainDriverViolations = driverViolationRepository.TableNoTracking
                          .Where(x => x.DriverId == quotationRequest.Driver.DriverId && x.NIN == quotationRequest.Driver.NIN).ToList();
            if (mainDriverViolations != null && mainDriverViolations.Count > 0)
            {
                mainDriverDto.DriverViolations = mainDriverViolations.Select(e => new ViolationDto()
                { ViolationCode = e.ViolationId }).ToList();

            }

            if (insuranceCompanyId == 14)
                mainDriverDto.MobileNo = quotationRequest.Driver.MobileNumber;

            mainDriverDto.DriverSocialStatusCode = quotationRequest.Driver.SocialStatusId?.ToString();

            if (mainDriverOccupation == null && mainDriverDto.DriverIdTypeCode == 1)
            {
                mainDriverDto.DriverOccupationCode = "O";
                mainDriverDto.DriverOccupation = "غير ذالك";
            }
            else if (mainDriverOccupation == null && mainDriverDto.DriverIdTypeCode == 2)
            {
                mainDriverDto.DriverOccupationCode = "31010";
                mainDriverDto.DriverOccupation = "موظف اداري";
            }
            else
            {
                if ((string.IsNullOrEmpty(mainDriverOccupation.Code) || mainDriverOccupation.Code == "o") && mainDriverDto.DriverIdTypeCode == 1)
                {
                    mainDriverDto.DriverOccupationCode = "O";
                    mainDriverDto.DriverOccupation = "غير ذالك";
                }
                else if ((string.IsNullOrEmpty(mainDriverOccupation.Code) || mainDriverOccupation.Code == "o") && mainDriverDto.DriverIdTypeCode == 2)
                {
                    mainDriverDto.DriverOccupationCode = "31010";
                    mainDriverDto.DriverOccupation = "موظف اداري";
                }
                else
                {
                    mainDriverDto.DriverOccupationCode = mainDriverOccupation.Code;
                    mainDriverDto.DriverOccupation = mainDriverOccupation.NameAr.Trim();
                }
            }

            if ((!quotationRequest.Driver.DrivingPercentage.HasValue || quotationRequest.Driver.DrivingPercentage > 100 || quotationRequest.Driver.DrivingPercentage < 100) && quotationRequest.Drivers.Count == 1)
            {
                mainDriverDto.DriverDrivingPercentage = 100;
            }
            else
            {
                mainDriverDto.DriverDrivingPercentage = quotationRequest.Driver.DrivingPercentage;
            }

            additionalDrivingPercentage = mainDriverDto.DriverDrivingPercentage.HasValue ? mainDriverDto.DriverDrivingPercentage.Value : 0; ;
            if (!mainDriverDto.DriverEducationCode.HasValue || mainDriverDto.DriverEducationCode == 0)
            {
                mainDriverDto.DriverEducationCode = 1;
            }
            mainDriverDto.DriverMedicalConditionCode = quotationRequest.Driver.MedicalConditionId;

            var DriverLicenses = _driverRepository.Table
                .Include(x => x.DriverLicenses)
                .FirstOrDefault(x => x.DriverId == quotationRequest.MainDriverId && x.IsDeleted == false)?
                .DriverLicenses;

            var LicenseDtos = new List<LicenseDto>();

            if (DriverLicenses != null && DriverLicenses.Count() > 0)
            {
                int licenseNumberYears;
                foreach (var item in DriverLicenses)
                {
                    licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                    LicenseDtos.Add(new LicenseDto()
                    {
                        DriverLicenseExpiryDate = Utilities.HandleHijriDate(item.ExpiryDateH),
                        DriverLicenseTypeCode = item.TypeDesc.ToString(),
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




            //Add main driver to drivers list
            //if (!quotationRequest.Insured.NationalId.StartsWith("7"))
            //{
            drivers.Add(mainDriverDto);
            //}
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
                        DriverNCDFreeYears = 0,
                        DriverNCDReference = "0"
                    };

                    if (insuranceCompanyId == 4) //AICC
                    {
                        driverDto.DriverRelationship = additionalDriver.RelationShipId ?? 0;
                    }

                    //if (quotationRequest.Insured.NationalId.StartsWith("7") && additionalDriver.NIN == additionalDrivers.ToList().FirstOrDefault().NIN)
                    //{
                    //    driverDto.DriverTypeCode = 1;
                    //    driverDto.DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears;
                    //    driverDto.DriverNCDReference = quotationRequest.NajmNcdRefrence;
                    //}
                    //else
                    //{
                    //    driverDto.DriverTypeCode = 2;
                    //}

                    //if (additionalDriver.AddressId.HasValue)
                    //{
                    //    var addressDetails = _addressService.GetAddressDetailsNoTracking(additionalDriver.AddressId.Value);
                    //    if (addressDetails != null)
                    //        driverDto.DriverHomeAddress = addressDetails.Address1;
                    //}

                    //driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                    if (additionalDriver.Gender == Gender.Male)
                        driverDto.DriverGenderCode = "M";
                    else if (additionalDriver.Gender == Gender.Female)
                        driverDto.DriverGenderCode = "F";
                    else
                        driverDto.DriverGenderCode = "M";

                    driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                    driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                            additionalDriver.NationalityCode.Value.ToString() : "113";
                    //driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                    //driverDto.DriverOccupation = additionalDriver.Occupation?.NameAr.Trim();

                    // var additionalDriverOccupation = quotationRequest.Insured.Occupation;
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
                            .FirstOrDefault(x => x.NIN == additionalDriver.NIN && x.IsDeleted == false)?
                            .DriverLicenses;

                    var additionalDriverLicenseDtos = new List<LicenseDto>();
                    if (additionalDriverLicenses != null && additionalDriverLicenses.Count() > 0)
                    {
                        int licenseNumberYears;
                        foreach (var item in additionalDriverLicenses)
                        {
                            licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(item.IssueDateH).Year);
                            additionalDriverLicenseDtos.Add(new LicenseDto()
                            {
                                DriverLicenseExpiryDate = Utilities.HandleHijriDate(item.ExpiryDateH),
                                DriverLicenseTypeCode = item.TypeDesc.ToString(),
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
                    var additionalDriverExtraLicenses = _driverExtraLicenses.TableNoTracking.Where(d => d.DriverId == additionalDriver.DriverId);
                    if (additionalDriverExtraLicenses != null && additionalDriverExtraLicenses.Any())
                    {
                        LicenseDto licenseDto;
                        List<LicenseDto> licenseAditional = new List<LicenseDto>();
                        foreach (var item in additionalDriverExtraLicenses)
                        {
                            licenseDto = new LicenseDto();
                            licenseDto.LicenseNumberYears = item.LicenseYearsId;
                            licenseDto.LicenseCountryCode = item.CountryCode;
                            licenseAditional.Add(licenseDto);

                        }
                        if (driverDto.DriverLicenses != null)
                            driverDto.DriverLicenses.AddRange(licenseAditional);
                        else
                            driverDto.DriverLicenses = licenseAditional;
                    }

                    var driverViolations = driverViolationRepository.TableNoTracking
                                     .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == additionalDriver.NIN);
                    if (driverViolations != null && driverViolations.Count() > 0)
                    {
                        driverDto.DriverViolations = driverViolations
                            .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();
                    }

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

        private List<Product> FilterOutProductsForMalath(List<Product> products, short deductibleValue)
        {
            List<Product> filteredProducts = new List<Product>();
            Product product = null;
            int delta = int.MaxValue;
            foreach (Product p in products)
            {
                if (p.DeductableValue == deductibleValue)
                    filteredProducts.Add(p);
                if (Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault()) < delta)
                {
                    if (!filteredProducts.Contains(p))
                        filteredProducts.Add(p);
                }
            }
            foreach (Product p in products)
            {
                if (p.DeductableValue == 0 && !filteredProducts.Contains(p))

                    filteredProducts.Add(p);
            }
            return filteredProducts;
        }



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
                if (promotionProgramCode != null)
                {
                    return promotionProgramCode;
                }
                dbContext.DatabaseInstance.Connection.Close();
                return promotionProgramCode;
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
            var vehiclesColors = _vehicleService.GetVehicleColors();
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

        private void GetAutoleaseVehicleColor(out string vehicleColor, out long vehicleColorCode, string vehicleMajorColor, int companyId)
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
            var vehiclesColors = _vehicleService.GetVehicleColors();
            var vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor || color.ArabicDescription == secondMajorCollor);
            if (vColor == null)
            {
                if (vehicleMajorColor.Contains(' '))
                {
                    vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor.Split(' ')[0] || color.ArabicDescription == secondMajorCollor.Split(' ')[0]);
                    if (vColor != null)
                    {
                        vehicleColor = vColor.YakeenColor;
                        vehicleColorCode = (companyId == 14) ? vColor.WataniyaAutoleaseCode.Value : vColor.YakeenCode;
                    }
                }
            }
            else
            {
                vehicleColor = vColor.YakeenColor;
                vehicleColorCode = (companyId == 14) ? vColor.WataniyaAutoleaseCode.Value : vColor.YakeenCode;
            }
        }


        private QuotationResponse GetQuotationResponse(int insuranceCompanyId, string qtRqstExtrnlId, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue)
        {
            var _16HoursBeforeNow = DateTime.Now.AddHours(-16);
            return _quotationResponseRepository.Table
                .Include(qr => qr.QuotationRequest)
                        .Include(qr => qr.Products)
                        .Include(qr => qr.Products.Select(p => p.PriceDetails.Select(pd => pd.PriceType)))
                        .Include(qr => qr.Products.Select(p => p.Product_Benefits))
                        .Include(qr => qr.Products.Select(p => p.Product_Benefits.Select(pb => pb.Benefit)))
                        .Include(qr => qr.QuotationRequest.Vehicle)
                        .Include(qr => qr.QuotationRequest.Driver)
                        .Where(
                        x => x.InsuranceCompanyId == insuranceCompanyId && x.QuotationRequest.ExternalId == qtRqstExtrnlId &&
                        x.InsuranceTypeCode == insuranceTypeCode &&
                        (
                            (x.VehicleAgencyRepair.HasValue && x.VehicleAgencyRepair.Value == vehicleAgencyRepair) ||
                            (!vehicleAgencyRepair && !x.VehicleAgencyRepair.HasValue)
                        ) &&
                        (
                            (!deductibleValue.HasValue && !x.DeductibleValue.HasValue) ||
                            (deductibleValue.HasValue && x.DeductibleValue.HasValue && x.DeductibleValue.Value == deductibleValue.Value)
                        ))
                        .FirstOrDefault(y => (_16HoursBeforeNow < y.CreateDateTime));
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


        private List<Product> FilterOutProducts(List<Product> products, short deductibleValue)
        {
            List<Product> filteredProducts = new List<Product>();
            Product product = null;
            int delta = int.MaxValue;
            foreach (Product p in products)
            {
                if (p.DeductableValue == deductibleValue)
                    filteredProducts.Add(p);
                if (Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault()) < delta)
                {
                    delta = Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault());
                    product = p;
                }
            }

            if (filteredProducts.Count == 0 && product != null)
            {

                if (!filteredProducts.Contains(product))
                    filteredProducts.Add(product);

            }

            foreach (Product p in products)
            {
                if (p.DeductableValue == 0 && !filteredProducts.Contains(p))
                    filteredProducts.Add(p);
            }

            return filteredProducts;
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

        public string ExportAutomatedTestResultToExcel(bool Quotation)
        {

            var dataToInsertInExcel = _automatedTestIntegrationTransactionRepository
                .Table.Where(x => x.Message.StartsWith(Quotation ? "Quotation" : "Policy") && !x.Retrieved)
                .ToList();

            dataToInsertInExcel.AsParallel().ForAll(x => x.Retrieved = true);

            _automatedTestIntegrationTransactionRepository.Update(dataToInsertInExcel);

            DateTime dt = DateTime.Now;
            string SPREADSHEET_NAME = null;

            if (Quotation)
                SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"QuotationAutomatedTest-{DateTime.Now.Ticks}-{dt.ToString("dd-MM-yyyy")}");
            else
                SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"PolicyAutomatedTest-{DateTime.Now.Ticks}-{dt.ToString("dd-MM-yyyy")}");

            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                {

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "CompamniesAutomatedTest" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<string> columns = new List<string>();
                    List<string> propNames = typeof(AutomatedTestIntegrationTransaction).GetProperties()
                        .Where(x => !string.Equals(x.Name, "Retrieved") && !string.Equals(x.Name, "Id") && !string.Equals(x.Name, "Date"))
                        .Select(x => x.Name).ToList();
                    propNames.Add("Status");

                    foreach (var prop in propNames)
                    {
                        columns.Add(prop);
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (var item in dataToInsertInExcel)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        foreach (String col in columns)
                        {
                            if (col == "Message")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Message); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InputParams")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InputParams); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "OutputParams")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InputParams); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "StatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StatusId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Status")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StatusId == 0 ? "Success" : "Failed"); //
                                //cell.StyleIndex = item.StatusId == 0 ? 1U : 2U;
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }
                }

                workbook.Close();
            }
            return SPREADSHEET_NAME;
        }


        public QuotationServiceRequest GetQuotationRequest(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue,out string promotionProgramCode,out int promotionProgramId)
        {
            var serviceRequestMessage = new QuotationServiceRequest();
            promotionProgramCode = string.Empty;
            promotionProgramId = 0;
            //Random r = new Random();
            var cities = _addressService.GetAllCities();
            long vehicleColorCode = 99;
            string vehicleColor;
            #region VehicleColor

            GetVehicleColor(out vehicleColor, out vehicleColorCode, quotationRequest.Vehicle.MajorColor, quotationResponse.InsuranceCompanyId);
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
            serviceRequestMessage.InsuredIdTypeCode = quotationRequest.Insured.CardIdTypeId;
            serviceRequestMessage.InsuredId = long.Parse(quotationRequest.Insured.NationalId);
            serviceRequestMessage.InsuredCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.ArabicDescription : "";
            serviceRequestMessage.InsuredCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.YakeenCode.ToString() : "";

            if (quotationRequest.Insured.NationalId.StartsWith("7")) //Company
            {                serviceRequestMessage.InsuredIdTypeCode = 3;                serviceRequestMessage.InsuredBirthDate = null;                serviceRequestMessage.InsuredBirthDateG = null;                serviceRequestMessage.InsuredBirthDateH = null;                serviceRequestMessage.InsuredGenderCode = null;                serviceRequestMessage.InsuredNationalityCode = null;                serviceRequestMessage.InsuredFirstNameEn = null;                serviceRequestMessage.InsuredMiddleNameEn = null;                serviceRequestMessage.InsuredLastNameEn = null;                serviceRequestMessage.InsuredFirstNameAr = quotationRequest.Insured.FirstNameAr; //Company Name
                serviceRequestMessage.InsuredMiddleNameAr = null;                serviceRequestMessage.InsuredLastNameAr = null;                serviceRequestMessage.InsuredSocialStatusCode = null;                serviceRequestMessage.InsuredEducationCode = null;                serviceRequestMessage.InsuredOccupation = null;                serviceRequestMessage.InsuredOccupationCode = null;                serviceRequestMessage.InsuredChildrenBelow16Years = null;                serviceRequestMessage.InsuredWorkCityCode = null;                serviceRequestMessage.InsuredWorkCity = null;                serviceRequestMessage.InsuredIdIssuePlaceCode = null;                serviceRequestMessage.InsuredIdIssuePlace = null;            }            else            {
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
                    serviceRequestMessage.InsuredGenderCode = "M";

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
                var insuredOccupation = quotationRequest.Insured.Occupation;
                if (insuredOccupation == null && serviceRequestMessage.InsuredIdTypeCode == 1)
                {
                    serviceRequestMessage.InsuredOccupationCode = "O";
                    serviceRequestMessage.InsuredOccupation = "غير ذالك";
                }
                else if (insuredOccupation == null && serviceRequestMessage.InsuredIdTypeCode == 2)
                {
                    serviceRequestMessage.InsuredOccupationCode = "31010";
                    serviceRequestMessage.InsuredOccupation = "موظف اداري";
                }
                else
                {
                    if ((string.IsNullOrEmpty(insuredOccupation.Code) || insuredOccupation.Code == "o") && serviceRequestMessage.InsuredIdTypeCode == 1)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "O";
                        serviceRequestMessage.InsuredOccupation = "غير ذالك";
                    }
                    else if ((string.IsNullOrEmpty(insuredOccupation.Code) || insuredOccupation.Code == "o") && serviceRequestMessage.InsuredIdTypeCode == 2)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "31010";
                        serviceRequestMessage.InsuredOccupation = "موظف اداري";
                    }
                    else
                    {
                        serviceRequestMessage.InsuredOccupationCode = insuredOccupation.Code;
                        serviceRequestMessage.InsuredOccupation = insuredOccupation.NameAr.Trim();
                    }
                }

                serviceRequestMessage.InsuredEducationCode = int.Parse(quotationRequest.Insured.Education.GetCode());
                if (!serviceRequestMessage.InsuredEducationCode.HasValue || serviceRequestMessage.InsuredEducationCode == 0)
                {
                    serviceRequestMessage.InsuredEducationCode = 1;
                }
                //end of mubark request
                serviceRequestMessage.InsuredChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
                serviceRequestMessage.InsuredIdIssuePlace = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.ArabicDescription : "";
                serviceRequestMessage.InsuredIdIssuePlaceCode = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.YakeenCode.ToString() : "";
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
            if (quotationRequest.Insured.NationalId.StartsWith("7")&& !quotationRequest.Vehicle.OwnerTransfer) //Company
            {                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.Insured.NationalId);            }            else            {
                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.Vehicle.CarOwnerNIN);
            }
            serviceRequestMessage.VehicleOwnerName = quotationRequest.Vehicle.CarOwnerName;
            serviceRequestMessage.VehiclePlateTypeCode = isVehicleRegistered ? quotationRequest.Vehicle.PlateTypeCode.ToString() : null;
            serviceRequestMessage.VehicleRegExpiryDate = isVehicleRegistered ? Utilities.HandleHijriDate(quotationRequest.Vehicle.LicenseExpiryDate) : null;

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
                            if (d < 10&&d>0)
                            {
                                day = "0" + day;
                            }
                            else if(d == 0)
                            {
                                day = "01";
                            }
                        }
                        if (int.TryParse(serviceRequestMessage.VehicleRegExpiryDate.Split('-')[1], out m))
                        {
                            if (m <10&& m>0)
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

            serviceRequestMessage.VehicleId = isVehicleRegistered ? long.Parse(quotationRequest.Vehicle.SequenceNumber) : long.Parse(quotationRequest.Vehicle.CustomCardNumber);
            serviceRequestMessage.VehicleModelYear = quotationRequest.Vehicle.ModelYear.Value;
            serviceRequestMessage.VehicleMaker = quotationRequest.Vehicle.VehicleMaker;
            serviceRequestMessage.VehicleMakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value.ToString() : "0";
            serviceRequestMessage.VehicleModel = quotationRequest.Vehicle.VehicleModel;
            serviceRequestMessage.VehicleModelCode = quotationRequest.Vehicle.VehicleModelCode.HasValue ? quotationRequest.Vehicle.VehicleModelCode.Value.ToString() : "0";

            if (quotationRequest.Vehicle.VehicleMakerCode.HasValue)
            {
                var makers = _vehicleService.VehicleMakers();
                if (makers != null)
                {
                    var vehicleMaker = makers.Where(e => e.Code == quotationRequest.Vehicle.VehicleMakerCode).FirstOrDefault();
                    if (vehicleMaker != null)
                    {
                        serviceRequestMessage.VehicleMaker = vehicleMaker.ArabicDescription;
                    }
                }
            }
            if (quotationRequest.Vehicle.VehicleModelCode.HasValue && quotationRequest.Vehicle.VehicleMakerCode.HasValue)
            {
                var models = _vehicleService.VehicleModels(quotationRequest.Vehicle.VehicleMakerCode.Value);
                if (models != null)
                {
                    var vehicleModel = models.Where(e => e.Code == quotationRequest.Vehicle.VehicleModelCode).FirstOrDefault();
                    if (vehicleModel != null)
                    {
                        serviceRequestMessage.VehicleModel = vehicleModel.ArabicDescription;
                    }
                }
            }
            serviceRequestMessage.VehicleMajorColor = vehicleColor;
            serviceRequestMessage.VehicleMajorColorCode = vehicleColorCode.ToString();
            serviceRequestMessage.VehicleBodyTypeCode = quotationRequest.Vehicle.VehicleBodyCode.ToString();

            serviceRequestMessage.VehicleRegPlace = quotationRequest.Vehicle.RegisterationPlace;
            if(string.IsNullOrEmpty(serviceRequestMessage.VehicleRegPlace))//as per mubark almutlak
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
            //if (quotationResponse.InsuranceCompanyId == 2 || quotationResponse.InsuranceCompanyId == 20 || quotationResponse.InsuranceCompanyId == 3 || quotationResponse.InsuranceCompanyId == 9 || quotationResponse.InsuranceCompanyId == 5 || quotationResponse.InsuranceCompanyId==11|| quotationResponse.InsuranceCompanyId == 6|| quotationResponse.InsuranceCompanyId == 7)//add GGI per rawabi
            //{
                if (serviceRequestMessage.VehicleUseCode == 2) {
                    serviceRequestMessage.VehicleAxleWeight = 1;
                    serviceRequestMessage.VehicleAxleWeightCode = 1;
                }    
           // }
            serviceRequestMessage.VehicleOvernightParkingLocationCode = int.Parse(quotationRequest.Vehicle?.ParkingLocation.GetCode());
            serviceRequestMessage.VehicleModification = quotationRequest.Vehicle.HasModifications;
            serviceRequestMessage.VehicleModificationDetails = string.IsNullOrEmpty(quotationRequest.Vehicle.ModificationDetails) ? "" : quotationRequest.Vehicle.ModificationDetails;
            if (quotationRequest.Vehicle.VehicleSpecifications != null && quotationRequest.Vehicle.VehicleSpecifications.Count > 0)
            {
                serviceRequestMessage.VehicleSpecifications = quotationRequest.Vehicle.VehicleSpecifications
                               .Select(e => new VehicleSpecificationDto() { VehicleSpecificationCode = e.Code }).ToList();
            }
            #endregion
            if (quotationRequest.Insured.NationalId.StartsWith("7"))            {                serviceRequestMessage.NCDFreeYears = 0;                serviceRequestMessage.NCDReference = "0";            }            else            {
                serviceRequestMessage.NCDFreeYears = quotationRequest.NajmNcdFreeYears.HasValue ? quotationRequest.NajmNcdFreeYears.Value : 0;
                serviceRequestMessage.NCDReference = quotationRequest.NajmNcdRefrence;
            }
            serviceRequestMessage.Drivers = CreateInsuranceCompanyDriversFromDataRequest(quotationRequest, cities, quotationResponse.InsuranceCompanyId);
            //if (!(string.IsNullOrWhiteSpace(userId) && string.IsNullOrEmpty(quotationRequest.Insured.NationalId)))
            //{
            var programcode = _promotionService.GetUserPromotionCodeInfo(userId, quotationRequest.Insured.NationalId, quotationResponse.InsuranceCompanyId, insuranceTypeCode == 2 ? 2 : 1);
            // as per Fayssal skip these emails 
            //mubarak.a @bcare.com.sa d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7
            //munera.s @bcare.com.sa a5fa9c14-61ed-44a1-a453-8db824a76a1e
            //mona.a @bcare.com.sa eb208f95-6b21-421c-be24-85f35ed017b5

            if (programcode!=null&&
                (userId== "d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7"
                || userId== "a5fa9c14-61ed-44a1-a453-8db824a76a1e"
                || userId=="eb208f95-6b21-421c-be24-85f35ed017b5"
                ||userId== "10c9e728-7459-4ef4-88d7-6321a41ead9c"))
            {
                promotionProgramCode = programcode.Code;
                promotionProgramId = programcode.PromotionProgramId;
                serviceRequestMessage.PromoCode = programcode.Code;
            }
            else if (programcode != null &&(string.IsNullOrEmpty(programcode.NationalId)||
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
            //}

            //else
            //{
            //    serviceRequestMessage.PromoCode = null;
            //}
            serviceRequestMessage.VehicleChassisNumber = quotationRequest.Vehicle.ChassisNumber;
            if ((quotationResponse.InsuranceCompanyId == 17 || quotationResponse.InsuranceCompanyId == 20 || quotationResponse.InsuranceCompanyId == 3 || quotationResponse.InsuranceCompanyId == 7 || quotationResponse.InsuranceCompanyId == 4 || quotationResponse.InsuranceCompanyId == 24 || quotationResponse.InsuranceCompanyId == 19) && quotationRequest.Driver != null)
            {
                serviceRequestMessage.PostalCode = quotationRequest.PostCode;
            }
            if (quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.TokioMarine) || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.MedGulf)
                || quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.ArabianShield))
            {
                if (quotationRequest.Vehicle.ManualEntry.HasValue && quotationRequest.Vehicle.ManualEntry.Value)
                    serviceRequestMessage.ManualEntry = ManualEntry.True;
                else
                    serviceRequestMessage.ManualEntry = ManualEntry.False;
            }
            if (quotationResponse.InsuranceCompanyId == 14)//Wataniya
            {
                serviceRequestMessage.IdExpiryDate = quotationRequest.Driver.IdExpiryDate;
                serviceRequestMessage.CameraTypeId = 3;
                serviceRequestMessage.BrakeSystemId = 3;
                serviceRequestMessage.HasAntiTheftAlarm = quotationRequest.Vehicle.HasAntiTheftAlarm;
                serviceRequestMessage.ParkingSensorId = 3;
                serviceRequestMessage.IsRenewal = false;
                serviceRequestMessage.IsUser = (!string.IsNullOrEmpty(userId)) ? true : false;
                serviceRequestMessage.HasFireExtinguisher = (quotationRequest.Vehicle.HasFireExtinguisher.HasValue &&
                                                        quotationRequest.Vehicle.HasFireExtinguisher.Value) ? true : false;

                if (!string.IsNullOrEmpty(quotationRequest.Insured.NationalId))
                {
                    var address = _addressService.GetAddressesByNin(quotationRequest.Insured.NationalId);
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

                var makerId = quotationRequest.Vehicle.VehicleMakerCode;
                var modelId = quotationRequest.Vehicle.VehicleModelCode;
                var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId.Value, modelId.Value);
                if (vehicleModel != null)
                {
                    if (vehicleModel.WataniyaMakerCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleMakerCode = vehicleModel.WataniyaMakerCode.Value.ToString();
                    if (vehicleModel.WataniyaModelCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleModelCode = vehicleModel.WataniyaModelCode.Value.ToString();
                }

                if (!string.IsNullOrEmpty(quotationRequest.Vehicle.CarPlateText1))
                    serviceRequestMessage.WataniyaFirstPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.Vehicle.CarPlateText1);
                if (!string.IsNullOrEmpty(quotationRequest.Vehicle.CarPlateText2))
                    serviceRequestMessage.WataniyaSecondPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.Vehicle.CarPlateText2);
                if (!string.IsNullOrEmpty(quotationRequest.Vehicle.CarPlateText3))
                    serviceRequestMessage.WataniyaThirdPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.Vehicle.CarPlateText3);
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
                (serviceRequestMessage.NCDFreeYears==0|| serviceRequestMessage.NCDFreeYears == 11
                || serviceRequestMessage.NCDFreeYears == 12 || serviceRequestMessage.NCDFreeYears == 13))
            {
                serviceRequestMessage.NoOfAccident = 0;
                serviceRequestMessage.ReferenceNo= "FIRSTHIT";
            }
            return serviceRequestMessage;
        }

        public QuotationServiceRequest GetAutoleasingQuotationRequest(QuotationRequest quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId)
        {
            var serviceRequestMessage = new QuotationServiceRequest();
            promotionProgramCode = string.Empty;
            promotionProgramId = 0;
            //Random r = new Random();
            var cities = _addressService.GetAllCities();
            long vehicleColorCode = 99;
            string vehicleColor;
            #region VehicleColor

            GetAutoleaseVehicleColor(out vehicleColor, out vehicleColorCode, quotationRequest.Vehicle.MajorColor, quotationResponse.InsuranceCompanyId);
            #endregion
            serviceRequestMessage.ReferenceId = quotationResponse.ReferenceId;
            serviceRequestMessage.ProductTypeCode = insuranceTypeCode;

            if (quotationRequest.RequestPolicyEffectiveDate.HasValue && quotationRequest.RequestPolicyEffectiveDate.Value <= DateTime.Now.Date)
            {
                DateTime effectiveDate = DateTime.Now.AddDays(1);
                serviceRequestMessage.PolicyEffectiveDate = new DateTime(effectiveDate.Year, effectiveDate.Month, effectiveDate.Day, effectiveDate.Hour, effectiveDate.Minute, effectiveDate.Second);

            }
            else
            {
                serviceRequestMessage.PolicyEffectiveDate = quotationRequest.RequestPolicyEffectiveDate.Value;
            }

            #region Insured
            serviceRequestMessage.InsuredIdTypeCode = quotationRequest.Insured.CardIdTypeId;
            serviceRequestMessage.InsuredId = long.Parse(quotationRequest.Insured.NationalId);
            serviceRequestMessage.InsuredCity = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.ArabicDescription : "";
            serviceRequestMessage.InsuredCityCode = quotationRequest.Insured.City != null ? quotationRequest.Insured.City?.YakeenCode.ToString() : "";

            if (quotationRequest.Insured.NationalId.StartsWith("7")) //Company
            {                serviceRequestMessage.InsuredBirthDate = null;                serviceRequestMessage.InsuredBirthDateG = null;                serviceRequestMessage.InsuredBirthDateH = null;                serviceRequestMessage.InsuredGenderCode = null;                serviceRequestMessage.InsuredNationalityCode = null;                serviceRequestMessage.InsuredFirstNameEn = null;
                serviceRequestMessage.InsuredMiddleNameEn = null;                serviceRequestMessage.InsuredLastNameEn = null;
                serviceRequestMessage.InsuredFirstNameAr = quotationRequest.Insured.FirstNameAr; //Company Name
                serviceRequestMessage.InsuredMiddleNameAr = null;                serviceRequestMessage.InsuredLastNameAr = null;
                serviceRequestMessage.InsuredSocialStatusCode = null;                serviceRequestMessage.InsuredEducationCode = null;                serviceRequestMessage.InsuredOccupation = null;                serviceRequestMessage.InsuredOccupationCode = null;                serviceRequestMessage.InsuredChildrenBelow16Years = null;                serviceRequestMessage.InsuredWorkCityCode = null;                serviceRequestMessage.InsuredWorkCity = null;                serviceRequestMessage.InsuredIdIssuePlaceCode = null;                serviceRequestMessage.InsuredIdIssuePlace = null;            }            else            {
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
                    serviceRequestMessage.InsuredGenderCode = "M";

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
                var insuredOccupation = quotationRequest.Insured.Occupation;
                if (insuredOccupation == null && serviceRequestMessage.InsuredIdTypeCode == 1)
                {
                    serviceRequestMessage.InsuredOccupationCode = "O";
                    serviceRequestMessage.InsuredOccupation = "غير ذالك";
                }
                else if (insuredOccupation == null && serviceRequestMessage.InsuredIdTypeCode == 2)
                {
                    serviceRequestMessage.InsuredOccupationCode = "31010";
                    serviceRequestMessage.InsuredOccupation = "موظف اداري";
                }
                else
                {
                    if ((string.IsNullOrEmpty(insuredOccupation.Code) || insuredOccupation.Code == "o") && serviceRequestMessage.InsuredIdTypeCode == 1)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "O";
                        serviceRequestMessage.InsuredOccupation = "غير ذالك";
                    }
                    else if ((string.IsNullOrEmpty(insuredOccupation.Code) || insuredOccupation.Code == "o") && serviceRequestMessage.InsuredIdTypeCode == 2)
                    {
                        serviceRequestMessage.InsuredOccupationCode = "31010";
                        serviceRequestMessage.InsuredOccupation = "موظف اداري";
                    }
                    else
                    {
                        serviceRequestMessage.InsuredOccupationCode = insuredOccupation.Code;
                        serviceRequestMessage.InsuredOccupation = insuredOccupation.NameAr.Trim();
                    }
                }

                serviceRequestMessage.InsuredEducationCode = int.Parse(quotationRequest.Insured.Education.GetCode());
                if (!serviceRequestMessage.InsuredEducationCode.HasValue || serviceRequestMessage.InsuredEducationCode == 0)
                {
                    serviceRequestMessage.InsuredEducationCode = 1;
                }
                //end of mubark request
                serviceRequestMessage.InsuredChildrenBelow16Years = quotationRequest.Insured.ChildrenBelow16Years;
                serviceRequestMessage.InsuredIdIssuePlace = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.ArabicDescription : "";
                serviceRequestMessage.InsuredIdIssuePlaceCode = quotationRequest.Insured.IdIssueCity != null ? quotationRequest.Insured.IdIssueCity.YakeenCode.ToString() : "";
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
            }
            #endregion
            if (quotationResponse.InsuranceCompanyId == 24)            {                serviceRequestMessage.PostalCode = quotationRequest.PostCode;            }
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
            if (quotationRequest.Insured.NationalId.StartsWith("7") && !quotationRequest.Vehicle.OwnerTransfer) //Company
            {                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.Insured.NationalId);            }            else            {
                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.Vehicle.CarOwnerNIN);
            }
            serviceRequestMessage.VehicleOwnerName = quotationRequest.Vehicle.CarOwnerName;
            serviceRequestMessage.VehiclePlateTypeCode = isVehicleRegistered ? quotationRequest.Vehicle.PlateTypeCode.ToString() : null;
            serviceRequestMessage.VehicleRegExpiryDate = isVehicleRegistered ? Utilities.HandleHijriDate(quotationRequest.Vehicle.LicenseExpiryDate) : null;

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

            serviceRequestMessage.VehicleModelYear = quotationRequest.Vehicle.ModelYear.Value;
            serviceRequestMessage.VehicleMaker = quotationRequest.Vehicle.VehicleMaker;
            serviceRequestMessage.VehicleMakerCode = quotationRequest.Vehicle.VehicleMakerCode.HasValue ? quotationRequest.Vehicle.VehicleMakerCode.Value.ToString() : "0";
            serviceRequestMessage.VehicleModel = quotationRequest.Vehicle.VehicleModel;
            serviceRequestMessage.VehicleModelCode = quotationRequest.Vehicle.VehicleModelCode.HasValue ? quotationRequest.Vehicle.VehicleModelCode.Value.ToString() : "0";

            if (quotationRequest.Vehicle.VehicleMakerCode.HasValue)
            {
                var makers = _vehicleService.VehicleMakers();
                if (makers != null)
                {
                    var vehicleMaker = makers.Where(e => e.Code == quotationRequest.Vehicle.VehicleMakerCode).FirstOrDefault();
                    if (vehicleMaker != null)
                    {
                        serviceRequestMessage.VehicleMaker = vehicleMaker.ArabicDescription;
                    }
                }
            }
            if (quotationRequest.Vehicle.VehicleModelCode.HasValue && quotationRequest.Vehicle.VehicleMakerCode.HasValue)
            {
                var models = _vehicleService.VehicleModels(quotationRequest.Vehicle.VehicleMakerCode.Value);
                if (models != null)
                {
                    var vehicleModel = models.Where(e => e.Code == quotationRequest.Vehicle.VehicleModelCode).FirstOrDefault();
                    if (vehicleModel != null)
                    {
                        serviceRequestMessage.VehicleModel = vehicleModel.ArabicDescription;
                    }
                }
            }

            serviceRequestMessage.VehicleMajorColor = vehicleColor;
            serviceRequestMessage.VehicleMajorColorCode = vehicleColorCode.ToString();
            serviceRequestMessage.VehicleOvernightParkingLocationCode = int.Parse(quotationRequest.Vehicle?.ParkingLocation.GetCode());

            if (!string.IsNullOrEmpty(quotationRequest.Vehicle.SequenceNumber) || !string.IsNullOrEmpty(quotationRequest.Vehicle.CustomCardNumber))
            {
                serviceRequestMessage.VehicleId = isVehicleRegistered ? long.Parse(quotationRequest.Vehicle.SequenceNumber) : long.Parse(quotationRequest.Vehicle.CustomCardNumber);
                serviceRequestMessage.VehicleRegPlace = quotationRequest.Vehicle.RegisterationPlace;
                if (string.IsNullOrEmpty(serviceRequestMessage.VehicleRegPlace))//as per mubark almutlak
                {
                    serviceRequestMessage.VehicleRegPlace = serviceRequestMessage.InsuredCity;
                }

                serviceRequestMessage.VehicleBodyTypeCode = quotationRequest.Vehicle.VehicleBodyCode.ToString();
                serviceRequestMessage.VehicleCapacity = quotationRequest.Vehicle.VehicleLoad; //@TODO: Validate this
                serviceRequestMessage.VehicleCylinders = int.Parse(quotationRequest.Vehicle.Cylinders?.ToString());
                serviceRequestMessage.VehicleWeight = quotationRequest.Vehicle.VehicleWeight;
                serviceRequestMessage.VehicleLoad = quotationRequest.Vehicle.VehicleLoad;
                serviceRequestMessage.VehicleOwnerTransfer = quotationRequest.Vehicle.OwnerTransfer;
                serviceRequestMessage.VehicleMileage = (int?)quotationRequest.Vehicle.CurrentMileageKM;

                if (quotationResponse.InsuranceCompanyId == 24)                {                    serviceRequestMessage.VehicleEngineSizeCode = int.Parse((bool)quotationRequest.Vehicle.EngineSize?.GetCode().Equals("0") ? "1" : quotationRequest.Vehicle.EngineSize?.GetCode());                }                else                {                    serviceRequestMessage.VehicleEngineSizeCode = int.Parse(quotationRequest.Vehicle.EngineSize?.GetCode());                }
                serviceRequestMessage.VehicleUseCode = int.Parse(quotationRequest.Vehicle.VehicleUse != null && quotationRequest.Vehicle.VehicleUse.GetCode().Equals("0") ? "1" : quotationRequest.Vehicle.VehicleUse.GetCode());
                serviceRequestMessage.VehicleUsingWorkPurposes = quotationRequest.Vehicle.IsUsedCommercially.HasValue ? quotationRequest.Vehicle.IsUsedCommercially.Value : false;
                if (serviceRequestMessage.VehicleUseCode == 2)
                {
                    serviceRequestMessage.VehicleAxleWeight = 1;
                    serviceRequestMessage.VehicleAxleWeightCode = 1;
                }
                serviceRequestMessage.VehicleModification = quotationRequest.Vehicle.HasModifications;
                serviceRequestMessage.VehicleModificationDetails = string.IsNullOrEmpty(quotationRequest.Vehicle.ModificationDetails) ? "" : quotationRequest.Vehicle.ModificationDetails;
                if (quotationRequest.Vehicle.VehicleSpecifications != null && quotationRequest.Vehicle.VehicleSpecifications.Count > 0)
                {
                    serviceRequestMessage.VehicleSpecifications = quotationRequest.Vehicle.VehicleSpecifications
                                   .Select(e => new VehicleSpecificationDto() { VehicleSpecificationCode = e.Code }).ToList();
                }
            }

            serviceRequestMessage.DriverDisabled = quotationRequest.Driver.IsSpecialNeed ?? false;
            serviceRequestMessage.VehicleAgencyRepair = vehicleAgencyRepair;
            serviceRequestMessage.VehicleValue = quotationRequest.Vehicle.VehicleValue;
            //if (quotationResponse.InsuranceCompanyId == 12)
            //    serviceRequestMessage.DeductibleValue = 2000;
            //else
            serviceRequestMessage.DeductibleValue = insuranceTypeCode == 1 ? null : (int?)(deductibleValue.HasValue ? deductibleValue.Value : 2000);

            serviceRequestMessage.VehicleTransmissionTypeCode = int.Parse(quotationRequest.Vehicle.TransmissionType?.GetCode());

            if (quotationRequest.Vehicle.MileageExpectedAnnual != null)
            {
                int MileageExpectedAnnualId = 0;
                int.TryParse(quotationRequest.Vehicle.MileageExpectedAnnual?.GetCode(), out MileageExpectedAnnualId);
                serviceRequestMessage.VehicleMileageExpectedAnnualCode = MileageExpectedAnnualId;
            }

            #endregion
            if (quotationRequest.Insured.NationalId.StartsWith("7"))            {                serviceRequestMessage.NCDFreeYears = 0;                serviceRequestMessage.NCDReference = "0";            }            else            {
                serviceRequestMessage.NCDFreeYears = quotationRequest.NajmNcdFreeYears.HasValue ? quotationRequest.NajmNcdFreeYears.Value : 0;
                serviceRequestMessage.NCDReference = quotationRequest.NajmNcdRefrence;
            }
            serviceRequestMessage.Drivers = CreateInsuranceCompanyDriversFromDataRequestAutoleasing(quotationRequest, cities, quotationResponse.InsuranceCompanyId);
            //if (!(string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(quotationRequest.Insured.NationalId)))
            //{
            //    var programcode = _promotionService.GetUserPromotionCodeInfo(userId, quotationRequest.Insured.NationalId, quotationResponse.InsuranceCompanyId, insuranceTypeCode);
            //    // as per Fayssal skip these emails 
            //    //mubarak.a @bcare.com.sa d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7
            //    //munera.s @bcare.com.sa a5fa9c14-61ed-44a1-a453-8db824a76a1e
            //    //mona.a @bcare.com.sa eb208f95-6b21-421c-be24-85f35ed017b5

            //    if (programcode != null &&
            //        (userId == "d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7"
            //        || userId == "a5fa9c14-61ed-44a1-a453-8db824a76a1e"
            //        || userId == "eb208f95-6b21-421c-be24-85f35ed017b5"))
            //    {
            //        promotionProgramCode = programcode.Code;
            //        promotionProgramId = programcode.PromotionProgramId;
            //        serviceRequestMessage.PromoCode = programcode.Code;
            //    }
            //    else if (programcode != null && (string.IsNullOrEmpty(programcode.NationalId) ||
            //         programcode.NationalId == serviceRequestMessage.InsuredId.ToString()))
            //    {
            //        promotionProgramCode = programcode.Code;
            //        promotionProgramId = programcode.PromotionProgramId;
            //        serviceRequestMessage.PromoCode = programcode.Code;
            //    }
            //    else
            //    {
            //        serviceRequestMessage.PromoCode = null;
            //    }
            //}

            //else
            //{
                serviceRequestMessage.PromoCode = null;
            //}
            serviceRequestMessage.VehicleChassisNumber = quotationRequest.Vehicle.ChassisNumber;
            if ((quotationResponse.InsuranceCompanyId == 20 || quotationResponse.InsuranceCompanyId == 3 || quotationResponse.InsuranceCompanyId == 7 || quotationResponse.InsuranceCompanyId == 21) && quotationRequest.Driver != null)
            {
                serviceRequestMessage.PostalCode = quotationRequest.PostCode;
            }
            if (quotationResponse.InsuranceCompanyId == 23)//Tokio Marine
            {
                if (quotationRequest.ManualEntry.HasValue && quotationRequest.ManualEntry.Value)
                    serviceRequestMessage.ManualEntry = "true";
                else
                    serviceRequestMessage.ManualEntry = "false";
            }
            if (quotationResponse.InsuranceCompanyId == 14)//Wataniya
            {
                serviceRequestMessage.IdExpiryDate = quotationRequest.Driver.IdExpiryDate;
                serviceRequestMessage.CameraTypeId = 3;
                serviceRequestMessage.BrakeSystemId = 3;
                serviceRequestMessage.HasAntiTheftAlarm = quotationRequest.Vehicle.HasAntiTheftAlarm;
                serviceRequestMessage.ParkingSensorId = 3;
                serviceRequestMessage.IsRenewal = false;
                serviceRequestMessage.IsUser = (!string.IsNullOrEmpty(userId)) ? true : false;
                serviceRequestMessage.HasFireExtinguisher = (quotationRequest.Vehicle.HasFireExtinguisher.HasValue &&
                                                        quotationRequest.Vehicle.HasFireExtinguisher.Value) ? true : false;

                if (quotationRequest.Insured.AddressId.HasValue)
                {
                    var addressDetails = _addressService.GetAddressDetailsNoTracking(quotationRequest.Insured.AddressId.Value);
                    if (addressDetails != null && !string.IsNullOrEmpty(addressDetails.RegionId))
                        serviceRequestMessage.InsuredAddressRegionID = int.Parse(addressDetails.RegionId);
                }

                serviceRequestMessage.IsRenewal = (quotationRequest.IsRenewal.HasValue) ? quotationRequest.IsRenewal.Value : false;

                if (quotationRequest.ManualEntry.HasValue && quotationRequest.ManualEntry.Value)
                {
                    serviceRequestMessage.ManualEntry = "true";
                    serviceRequestMessage.MissingFields = quotationRequest.MissingFields;
                }
                else
                    serviceRequestMessage.ManualEntry = "false";
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

        public int GetUserOffersCount(string id)
        {
            return _quotationRequestRepository.Table.Where(x => x.UserId == id).ToList().Where(y => GivenDateWithin16Hours(y.CreatedDateTime)).Count();
        }

        private bool GivenDateWithin16Hours(DateTime givenDate)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        }
        public IPagedList<QuotationRequest> GetQuotationRequestsByUserId(string userId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var _16HourBeforeNow = DateTime.Now.AddHours(-16);
            var query = _quotationRequestRepository.Table.Where(x => x.UserId == userId)
                .Include(x => x.Vehicle)
                .Include(x => x.Driver)
                .Include(x => x.City)
                .Where(y => DateTime.Compare(y.CreatedDateTime, _16HourBeforeNow) > 0).ToList();
            return new PagedList<QuotationRequest>(query, pageIndx, pageSize);
        }

        public List<Benefit> GetBenefits()
        {
          return  _benefitRepository.TableNoTracking.ToList();
        }
        public QuotationOutput GetTawuniyaQuotation(int quotationRequestId, string referenceId, Guid productInternalId, string qtRqstExtrnlId, InsuranceCompany insuranceCompany, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)        {            QuotationOutput output = new QuotationOutput();            output.QuotationResponse = new QuotationResponse();            try            {                if (string.IsNullOrEmpty(qtRqstExtrnlId))                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "qtRqstExtrnlId is null ";                    return output;                }                var quotationRequest = _quotationRequestRepository.Table               .Include(e => e.Insured)               .Include(e => e.Vehicle)               .Include(e => e.QuotationResponses)               .Include(e => e.Driver)               .Include(e => e.Insured.Occupation)               .Include(e => e.Drivers.Select(d => d.DriverViolations))               .Include(e => e.Driver.Occupation)               .Include(e => e.Insured.IdIssueCity)               .Include(e => e.Insured.City)               .FirstOrDefault(e => e.ID == quotationRequestId);                if (quotationRequest == null)                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "There is no quotation request with this id " + quotationRequestId;                    return output;                }                var quotationResponse = quotationRequest.QuotationResponses.FirstOrDefault(e => e.ReferenceId == referenceId);                string promotionProgramCode = string.Empty;                int promotionProgramId = 0;                var requestMessage = GetQuotationRequest(quotationRequest,                    quotationResponse, quotationResponse.InsuranceTypeCode.Value, quotationResponse.VehicleAgencyRepair.Value, userId.ToString(), quotationResponse.DeductibleValue, out promotionProgramCode, out promotionProgramId);

                if (string.IsNullOrEmpty(requestMessage.PromoCode) && !string.IsNullOrEmpty(quotationResponse.PromotionProgramCode))
                    requestMessage.PromoCode = quotationResponse.PromotionProgramCode;

                requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;                output.QuotationServiceRequest = requestMessage;                output.ErrorCode = QuotationOutput.ErrorCodes.Success;                return output;            }            catch (Exception exp)            {                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;                output.ErrorDescription = exp.ToString(); // SubmitInquiryResource.InvalidData;
                return output;            }        }

        public QuotationOutput GetTawuniyaAutoleasingQuotation(int quotationRequestId, string referenceId, Guid productInternalId, string qtRqstExtrnlId, InsuranceCompany insuranceCompany, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)        {            QuotationOutput output = new QuotationOutput();            output.QuotationResponse = new QuotationResponse();            try            {                if (string.IsNullOrEmpty(qtRqstExtrnlId))                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "qtRqstExtrnlId is null ";                    return output;                }                var quotationRequest = _quotationRequestRepository.Table               .Include(e => e.Insured)               .Include(e => e.Vehicle)               .Include(e => e.QuotationResponses)               .Include(e => e.Driver)               .Include(e => e.Insured.Occupation)               .Include(e => e.Drivers.Select(d => d.DriverViolations))               .Include(e => e.Driver.Occupation)               .Include(e => e.Insured.IdIssueCity)               .Include(e => e.Insured.City)               .FirstOrDefault(e => e.ID == quotationRequestId);                if (quotationRequest == null)                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "There is no quotation request with this id " + quotationRequestId;                    return output;                }                var quotationResponse = quotationRequest.QuotationResponses.FirstOrDefault(e => e.ReferenceId == referenceId);                string promotionProgramCode = string.Empty;                int promotionProgramId = 0;                var requestMessage = GetAutoleasingQuotationRequest(quotationRequest,                    quotationResponse, quotationResponse.InsuranceTypeCode.Value, false, userId.ToString(), quotationResponse.DeductibleValue, out promotionProgramCode, out promotionProgramId);                requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;                output.QuotationServiceRequest = requestMessage;                output.ErrorCode = QuotationOutput.ErrorCodes.Success;                return output;            }            catch (Exception exp)            {                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;                output.ErrorDescription = exp.ToString(); // SubmitInquiryResource.InvalidData;
                return output;            }        }


        public QuotationOutput GetNOAQuotationRequest(string qtRqstExtrnlId, int insuranceCompanyID, string userId, string channel, int insuranceTypeCode, bool vehicleAgencyRepair , int? deductibleValue,out string promotionProgramCode, out int promotionProgramId)        {            QuotationOutput output = new QuotationOutput();            output.QuotationResponse = new QuotationResponse();
             promotionProgramCode = string.Empty;
            promotionProgramId = 0;            try            {                DateTime startDateTime = DateTime.Now;
                var quoteRequest = _quotationRequestRepository.TableNoTracking
                    .Include(request => request.Vehicle)
                    .Include(request => request.Driver)
                    .Include(request => request.Insured)
                    .Include(request => request.Insured.Occupation)
                    .Include(request => request.Drivers.Select(d => d.DriverViolations))
                    .Include(request => request.Driver.Occupation)
                    .Include(e => e.Insured.IdIssueCity)
                    .Include(e => e.Insured.City)
                    .FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);
                if (quoteRequest == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
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
                    ReferenceId = getNewReferenceId(),
                    RequestId = quoteRequest.ID,
                    InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                    VehicleAgencyRepair = vehicleAgencyRepair,
                    DeductibleValue = deductibleValue,
                    CreateDateTime = startDateTime,
                    InsuranceCompanyId = insuranceCompanyID
                };
                //string promotionProgramCode = string.Empty;
                //int promotionProgramId = 0;
                var requestMessage = GetQuotationRequest(quoteRequest, output.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue, out promotionProgramCode, out promotionProgramId);
                output.QuotationServiceRequest = requestMessage;                output.ErrorCode = QuotationOutput.ErrorCodes.Success;                return output;
            }            catch (Exception exp)            {                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;                output.ErrorDescription = exp.ToString(); // SubmitInquiryResource.InvalidData;
                return output;            }        }

        #region Admin Add Benefit
        public AddVehicleBenefitOutput AddVehicleBenefit(AddVehicleBenefitModel model, string userId, string userName)
        {
            AddVehicleBenefitOutput output = new AddVehicleBenefitOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.Channel = model.Channel.ToString();
            log.UserId = userId;
            log.UserName = userName;
            log.RefrenceId = model.ReferenceId;
            log.PolicyNo = model?.PolicyNo;
            log.MethodName = "AddVehicleBenefit";
            try
            {
                model.BenefitStartDate = model.BenefitStartDate.AddDays(1);
                var validationOutput = ValidateAddVehicleBenefitData(model, log);
                if (validationOutput.ErrorCode != AddVehicleBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = validationOutput.ErrorCode;
                    output.ErrorDescription = validationOutput.ErrorDescription;
                    return output;
                }
                var policy = _policyService.GetPolicyByPolicyNoAndReferenceId(model.PolicyNo, model.ReferenceId, out string ex);
                if (policy == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "Policy Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy Not exist , ex = " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceCompanyId == 0)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (policy.InsuranceTypeCode == 0)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Insurance Type Code  is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
                log.RefrenceId = referenceId;
                log.InsuranceTypeCode = policy.InsuranceTypeCode;

                var insuranceCompany = _insuranceCompanyService.GetById(policy.InsuranceCompanyId);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany.Key;
                log.CompanyId = insuranceCompany.InsuranceCompanyID;
                Guid parsedUserId = Guid.Empty;
                Guid.TryParse(userId, out parsedUserId);
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                {
                    UserID = parsedUserId,
                    UserName = userName,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = model.ReferenceId
                };
                PolicyModification policyModification = new PolicyModification()
                {
                    Channel = model.Channel.ToString(),
                    CreatedDate = DateTime.Now,
                    UserIP = log.UserId,
                    CreatedBy = log.UserId,
                    InsuranceCompanyId = policy.InsuranceCompanyId,
                    InsuranceTypeCode = policy.InsuranceTypeCode,
                    MethodName = "AddVehicleBenefit",
                    PolicyNo = policy.PolicyNo,
                    ReferenceId = referenceId,
                    ServerIP = log.ServerIP,
                    UserAgent = log.UserAgent,
                    QuotationReferenceId = model.ReferenceId,
                    InvoiceNo = GetNewInvoiceNumber()
                };
                this._policyModificationRepository.Insert(policyModification);
                if (policyModification.Id < 1)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "Failed to insert policy modification Request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to insert policy modification Request";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var quotation = _quotationResponseRepository.TableNoTracking.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                AddVechileBenefitRequest request = new AddVechileBenefitRequest();
                request.ReferenceId = referenceId;
                request.PolicyNo = policy.PolicyNo;
                request.BenefitStartDate = model.BenefitStartDate.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                request.PolicyReferenceId = policy.ReferenceId;
                request.QuotationRequestId = quotation.RequestId;
                IInsuranceProvider provider = GetProvider(insuranceCompany, policy.InsuranceTypeCode);
                if (provider == null)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.AddVechileBenefit(request, predefinedLogInfo);
                if (results == null)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddVehicleBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                List<PolicyAdditionalBenefit> additionalBenefits = new List<PolicyAdditionalBenefit>();
                foreach (var benefit in results.Benefits)
                {
                    PolicyAdditionalBenefit policyAdditionalBenefit = new PolicyAdditionalBenefit();
                    policyAdditionalBenefit.ReferenceId = policyModification.ReferenceId;
                    policyAdditionalBenefit.InsuranceCompanyId = policyModification.InsuranceCompanyId;
                    policyAdditionalBenefit.InsuranceTypeCode = policyModification.InsuranceTypeCode;
                    policyAdditionalBenefit.QuotationReferenceId = policyModification.QuotationReferenceId;
                    policyAdditionalBenefit.BenefitCode = benefit.BenefitCode;
                    policyAdditionalBenefit.BenefitDescAr = benefit.BenefitDescAr;
                    policyAdditionalBenefit.BenefitDescEn = benefit.BenefitDescEn;
                    policyAdditionalBenefit.BenefitEffectiveDate = benefit.BenefitEffectiveDate;
                    policyAdditionalBenefit.BenefitExpiryDate = benefit.BenefitExpiryDate;
                    policyAdditionalBenefit.BenefitId = benefit.BenefitId;
                    policyAdditionalBenefit.BenefitNameAr = benefit.BenefitNameAr;
                    policyAdditionalBenefit.BenefitNameEn = benefit.BenefitNameEn;
                    policyAdditionalBenefit.BenefitPrice = benefit.BenefitPrice;
                    policyAdditionalBenefit.DeductibleValue = benefit.DeductibleValue != null ? (decimal)benefit.DeductibleValue : 0;
                    policyAdditionalBenefit.TaxableAmount = benefit.TaxableAmount != null ? (decimal)benefit.TaxableAmount : 0;
                    policyAdditionalBenefit.VATAmount = benefit.VATAmount != null ? (decimal)benefit.VATAmount : 0;
                    additionalBenefits.Add(policyAdditionalBenefit);
                }
                _policyAdditionalBenefitRepository.Insert(additionalBenefits);
                output.Benefits = results.Benefits;
                output.ReferenceId = referenceId;
                output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Add Driver Service";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private AddVehicleBenefitOutput ValidateAddVehicleBenefitData(AddVehicleBenefitModel model, PolicyModificationLog log)
        {
            AddVehicleBenefitOutput output = new AddVehicleBenefitOutput();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.RequestModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.PolicyNo))
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "PolicyNo is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "PolicyNo is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.BenefitStartDate == null)
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "BenefitStartDate is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "BenefitStartDate is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (model.BenefitStartDate < DateTime.Now.Date.AddDays(1))
                {
                    output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "AdditionStartDate must be in future";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "AdditionStartDate must be in future";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = AddVehicleBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = SubmitInquiryResource.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
        }
        private int GetNewInvoiceNumber()
        {
            Random rnd = new Random(System.Environment.TickCount);
            int invoiceNumber = rnd.Next(111111111, 999999999);

            if (_policyModificationRepository.Table.Any(i => i.InvoiceNo == invoiceNumber))
                return GetNewInvoiceNumber();

            return invoiceNumber;
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
        #region Autoleasing
        public QuotationsFormOutput GetQuotaionInsuranceProposalDetailsByExternalId(QuotationFormWithSelectedBenfitsViewModel model, out string exception)
        {
            exception = string.Empty;
            QuotationsFormOutput output = new QuotationsFormOutput();
            PdfGenerationLog log = new PdfGenerationLog();

            var langId = (model.lang.ToLower() == "en") ? 2 : 1;

            try
            {
                string exp = string.Empty;
                InsuranceProposalInfoModel quoteRequest = _quotationService.GetInsuranceProposalDetails(model.qtRqstExtrnlId,out exp);
                if (quoteRequest == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null and exception is:"+ exp;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (quoteRequest.DepreciationSettingHistory == null && quoteRequest.DepreciationSetting == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
                    output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quoteRequest.Bank.Id}, Maker: {quoteRequest.VehicleMakerCode}, Model: {quoteRequest.VehicleModelCode}";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (quoteRequest.RepairMethodeSetting == null && quoteRequest.RepairMethodeSettingHistory == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                    output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + quoteRequest.Bank.Id;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                QuotaionInsuranceProposalModel data = new QuotaionInsuranceProposalModel();
                AutoleasingDepreciationSetting depreciationSetting = null;

                #region Lessor Information

                data.CommercialRegistrationNo = quoteRequest.InsuredNationalId;
                data.LessorName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.Bank?.NameEn : quoteRequest.Bank?.NameAr;
                data.LessorPhoneNumber = quoteRequest.Bank?.PhoneNumber;
                data.LessorNationalAddress = quoteRequest.Bank?.NationalAddress;

                #endregion

                #region Lessee Information

                data.NationalId = quoteRequest.DriverNationalId;
                data.LesseeName = (langId == (int)LanguageTwoLetterIsoCode.En)
                    ? quoteRequest.MainDriverNameEn
                    : quoteRequest.MainDriverNameAr;
                data.LesseePhoneNumber = quoteRequest.MobileNumber;
                data.LesseeNationalAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.MainDriverAddressEn : quoteRequest.MainDriverAddressAr;

                #endregion

                #region Vehicle Details

                data.RegistrationPlateNo = (!string.IsNullOrEmpty(quoteRequest.VehicleId))
                    ? GetCarPlateInfo(quoteRequest.CarPlateText1, quoteRequest.CarPlateText2,
                    quoteRequest.CarPlateText3, (quoteRequest.CarPlateNumber.HasValue) ? quoteRequest.CarPlateNumber.Value : 0, model.lang) : null;
                data.ChassisNumber = quoteRequest.ChassisNumber;
                data.VehicleColor = (!string.IsNullOrEmpty(quoteRequest.VehicleId)) ? quoteRequest.MajorColor : null;
                data.VehicleRegistrationExpiryDate = quoteRequest.VehicleLicenseExpiryDate;
                data.BodyType = (!string.IsNullOrEmpty(quoteRequest.VehicleId)) ? ((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.BodyTypeEn : quoteRequest.BodyTypeAr) : null;

                data.VehicleIdTypeId = quoteRequest.VehicleBodyCode;
                data.VehicleId = quoteRequest.VehicleId;
                data.VehicleMaker = quoteRequest.VehicleMaker;
                data.VehicleModel = quoteRequest.VehicleModel;
                data.ManufactureYear = quoteRequest.VehicleYear;

                List<string> additionalDriversNames = new List<string>();
                if (!string.IsNullOrEmpty(quoteRequest.AdditionalDriverOneNameEn) && !string.IsNullOrEmpty(quoteRequest.AdditionalDriverOneNameAr))
                    additionalDriversNames.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.AdditionalDriverOneNameEn : quoteRequest.AdditionalDriverOneNameAr);

                if (!string.IsNullOrEmpty(quoteRequest.AdditionalDriverTwoNameEn) && !string.IsNullOrEmpty(quoteRequest.AdditionalDriverTwoNameAr))
                    additionalDriversNames.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.AdditionalDriverTwoNameEn : quoteRequest.AdditionalDriverTwoNameAr);

                if (additionalDriversNames != null && additionalDriversNames.Count > 0 && additionalDriversNames.Any(a => !string.IsNullOrWhiteSpace(a)))
                    data.NamesOfAuthorizedDrivers = string.Join(", ", additionalDriversNames);

                data.VehicleValue = quoteRequest.VehicleValue.Value;
                data.DeductibleValue = quoteRequest.QuotationResponses.OrderByDescending(a => a.Id).FirstOrDefault(a => a.Deductible.HasValue)?.Deductible;

                //var lastResponse = quoteRequest.QuotationResponses.OrderByDescending(a => a.Id).FirstOrDefault();
                var _bankRepairMethodSettings = quoteRequest.RepairMethodeSetting;
                if (_bankRepairMethodSettings != null)
                {
                    data.RepairMethod = _bankRepairMethodSettings.FirstYear;
                }
                //AutoleasingAgencyRepair bankRepairMethodSettings = null;
                //if (quoteRequest.RepairMethodeSettingHistory != null)
                //{
                //    var bankRepairMethodSettingsHistory = quoteRequest.RepairMethodeSettingHistory;
                //    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                //    {
                //        BankId = bankRepairMethodSettingsHistory.BankId,
                //        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                //        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                //        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                //        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                //        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                //    };
                //}
                //else
                //{
                //    var _bankRepairMethodSettings = quoteRequest.RepairMethodeSetting;
                //    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                //    {
                //        BankId = _bankRepairMethodSettings.BankId,
                //        FirstYear = _bankRepairMethodSettings.FirstYear,
                //        SecondYear = _bankRepairMethodSettings.SecondYear,
                //        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                //        FourthYear = _bankRepairMethodSettings.FourthYear,
                //        FifthYear = _bankRepairMethodSettings.FifthYear,
                //    };
                //}

                //List<string> repairMethodList = new List<string>();
                //var contractDuration = quoteRequest.ContractDuration.Value;
                //if (contractDuration >= 1)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                //}
                //if (contractDuration >= 2)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                //}
                //if (contractDuration >= 3)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                //}
                //if (contractDuration >= 4)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                //}
                //if (contractDuration >= 5)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FifthYear);
                //}

                //List<string> allRepairMethods = new List<string>();
                //foreach (var item in repairMethodList)
                //    allRepairMethods.Add(item.Substring(0, 1));

                //if (allRepairMethods != null && allRepairMethods.Count > 0)
                //    data.RepairMethod = string.Join(", ", allRepairMethods);

                #endregion

                #region Policy Details

                foreach (var qrResponse in quoteRequest.QuotationResponses)
                {
                    if (qrResponse.PolicyEffectiveDate.HasValue && qrResponse.PolicyExpiryDate.HasValue)
                    {
                        var policyEffectiveDateStart = (langId == (int)LanguageTwoLetterIsoCode.En)
                        ? $"{ qrResponse.PolicyEffectiveDate.Value.Day }-{ qrResponse.PolicyEffectiveDate.Value.Month }-{ qrResponse.PolicyEffectiveDate.Value.Year }"
                        : $"{ qrResponse.PolicyEffectiveDate.Value.Year }-{ qrResponse.PolicyEffectiveDate.Value.Month }-{ qrResponse.PolicyEffectiveDate.Value.Day}";

                        var policyEffectiveDateEnd = (langId == (int)LanguageTwoLetterIsoCode.En)
                            ? $"{ qrResponse.PolicyExpiryDate.Value.Day }-{ qrResponse.PolicyExpiryDate.Value.Month }-{ qrResponse.PolicyExpiryDate.Value.Year }"
                            : $"{ qrResponse.PolicyExpiryDate.Value.Year }-{ qrResponse.PolicyExpiryDate.Value.Month }-{ qrResponse.PolicyExpiryDate.Value.Day}";

                        data.Convergeperiod = (langId == (int)LanguageTwoLetterIsoCode.En)
                            ? $"{ policyEffectiveDateStart } { AutoLeasingQuotationResources.to } { policyEffectiveDateEnd }" : $"{ policyEffectiveDateEnd } { AutoLeasingQuotationResources.to } { policyEffectiveDateStart }";

                        break;
                    }
                }

                #endregion

                #region Deprecation Data

                if (quoteRequest.DepreciationSettingHistory != null)
                {
                    var deprecationHistory = quoteRequest.DepreciationSettingHistory;
                    depreciationSetting = new AutoleasingDepreciationSetting()
                    {
                        BankId = quoteRequest.Bank.Id,
                        MakerCode = deprecationHistory.MakerCode,
                        ModelCode = deprecationHistory.ModelCode,
                        MakerName = deprecationHistory.MakerName,
                        ModelName = deprecationHistory.ModelName,
                        Percentage = deprecationHistory.Percentage,
                        IsDynamic = deprecationHistory.IsDynamic,
                        FirstYear = deprecationHistory.FirstYear,
                        SecondYear = deprecationHistory.SecondYear,
                        ThirdYear = deprecationHistory.ThirdYear,
                        FourthYear = deprecationHistory.FourthYear,
                        FifthYear = deprecationHistory.FifthYear,
                        AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                    };
                }
                else
                {
                    var deprecationSetting = quoteRequest.DepreciationSetting;
                    depreciationSetting = new AutoleasingDepreciationSetting();
                    depreciationSetting.BankId = quoteRequest.Bank.Id;
                    depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSetting.MakerName = deprecationSetting.MakerName;
                    depreciationSetting.ModelName = deprecationSetting.ModelName;
                    depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;

                }

                if (depreciationSetting != null)
                {
                    data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;

                    List<string> annualPercentages = new List<string>();
                    if (!depreciationSetting.FirstYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FirstYear.ToString().Replace(".00", "") + "%");
                        data.Firstyear = depreciationSetting.FirstYear.ToString().Replace(".00", "") + "%";
                    }
                    if (!depreciationSetting.SecondYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.SecondYear.ToString().Replace(".00", "") + "%");
                        data.Secondyear = depreciationSetting.SecondYear.ToString().Replace(".00", "") + "%";
                    }
                    if (!depreciationSetting.ThirdYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Replace(".00", "") + "%");
                        data.Thirdyear = depreciationSetting.ThirdYear.ToString().Replace(".00", "") + "%";
                    }
                    if (!depreciationSetting.FourthYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FourthYear.ToString().Replace(".00", "") + "%");
                        data.Fourthyear = depreciationSetting.FourthYear.ToString().Replace(".00", "") + "%";
                    }
                    if (!depreciationSetting.FifthYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FifthYear.ToString().Replace(".00", "") + "%");
                        data.Fifthyear = depreciationSetting.FifthYear.ToString().Replace(".00", "") + "%";
                    }

                    data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Replace(".00", "") + "%";
                    data.AnnualDeprecationPercentage = data.Percentage;

                }

                #endregion

                var vehicleValue = (model.IsRenewal) ? model.VehiclOriginaleValue : quoteRequest.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }

                if (model.IsRenewal)
                {
                    exception = string.Empty;
                    RenewalOldDepreciationValuesModel oldDepreciationData = HandleRenewalOldDepreciationValues(model.qtRqstExtrnlId, model.VehiclOriginaleValue, depreciationSetting, model.PoliciesCount, out exception);
                    if (oldDepreciationData != null)
                    {
                        if (model.PoliciesCount >= 1)
                            DepreciationValue1 = oldDepreciationData.DepreciationValue1.Value;

                        if (model.PoliciesCount >= 2)
                            DepreciationValue2 = oldDepreciationData.DepreciationValue2.Value;

                        if (model.PoliciesCount >= 3)
                            DepreciationValue3 = oldDepreciationData.DepreciationValue3.Value;

                        if (model.PoliciesCount >= 4)
                            DepreciationValue4 = oldDepreciationData.DepreciationValue4.Value;

                        if (model.PoliciesCount >= 5)
                            DepreciationValue5 = oldDepreciationData.DepreciationValue5.Value;
                    }
                }

                data.Firstyear = Math.Round(DepreciationValue1.Value, 2).ToString().Replace(".00", "");
                data.Secondyear = Math.Round(DepreciationValue2.Value, 2).ToString().Replace(".00", "");
                data.Thirdyear = Math.Round(DepreciationValue3.Value, 2).ToString().Replace(".00", "");
                data.Fourthyear = Math.Round(DepreciationValue4.Value, 2).ToString().Replace(".00", "");
                data.Fifthyear = Math.Round(DepreciationValue5.Value, 2).ToString().Replace(".00", "");

                #region Selected Benfits

                #region Old Code

                //List<string> selectedAdditionalBenfitsString = new List<string>();
                //foreach (var qrResponse in quoteRequest.QuotationResponses)
                //{
                //    foreach (var product in qrResponse.Products)
                //    {
                //        List<short?> selectedBenfitsIds = new List<short?>();
                //        if (benfits != null)
                //        {
                //            selectedBenfitsIds = benfits.FirstOrDefault(a => a.ProductId == product.ProductID)?.BenfitIds;
                //            var productBenfits = product.Benfits;
                //            if (productBenfits != null)
                //            {
                //                foreach (var benfit in productBenfits)
                //                {
                //                    if (selectedBenfitsIds != null && selectedBenfitsIds.IndexOf(benfit.BenefitId) != -1)
                //                    {
                //                        selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //data.AdditionalBenefits = string.Join(", ", selectedAdditionalBenfitsString);

                #endregion

                List<short?> selectedBenfitsIds = new List<short?>();
                var finalProducts = new List<QuotationProductInfoModel>();
                List<ProductBenfitDetailsInfoModel> selectedProductBenfits = null;
                var responses = quoteRequest.QuotationResponses.Where(a => a.VehicleAgencyRepair == data.RepairMethod).ToList();
                foreach (var qrResponse in responses)
                {
                    foreach (var product in qrResponse.Products)
                    {
                        selectedProductBenfits = new List<ProductBenfitDetailsInfoModel>();
                        var productBenfits = product.Benfits;
                        //if (productBenfits == null)
                        //    continue;

                        selectedBenfitsIds = null;
                        selectedBenfitsIds = model.SelectedBenfits.FirstOrDefault(a => a.ProductId == product.ProductID)?.BenfitIds;
                        //if (selectedBenfitsIds == null)
                        //    continue;

                        decimal _selectedBenfitTotalPrice = 0;
                        foreach (var benfit in productBenfits)
                        {
                            if (selectedBenfitsIds != null && selectedBenfitsIds.IndexOf(benfit.BenefitId) != -1 && benfit.BenefitPrice.HasValue && benfit.BenefitPrice.Value > 0)
                            {
                                _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                selectedProductBenfits.Add(benfit);
                            }
                        }

                        product.Benfits = selectedProductBenfits;
                        product.ProductPrice += _selectedBenfitTotalPrice + _selectedBenfitTotalPrice * (decimal).15;
                        finalProducts.Add(product);
                    }
                }

                if (finalProducts != null && finalProducts.Count > 0)
                {
                    var decendingProducts = finalProducts.OrderBy(a => a.ProductPrice).ToList();
                    var lowestProductBenfits = decendingProducts.FirstOrDefault().Benfits;
                    if (lowestProductBenfits != null && lowestProductBenfits.Count > 0)
                    {
                        List<string> selectedAdditionalBenfitsString = new List<string>();
                        foreach (var benfit in lowestProductBenfits)
                            selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);

                        data.AdditionalBenefits = string.Join(", ", selectedAdditionalBenfitsString);
                    }
                }

                #endregion

                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;

                log.ServerIP = Utilities.GetInternalServerIP();
                //log.CompanyID = quoteResponse.InsuranceCompanyId;
                if (string.IsNullOrEmpty(log.Channel))
                    log.Channel = Channel.autoleasing.ToString();

                string policyDetailsJsonString = JsonConvert.SerializeObject(data);
                AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
                {
                    ReportType = "InsuranceProposalTemplate",
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
                log.ServiceResponse = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                return output;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();

                log.ServiceResponse = exception;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Exception Error";
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return null;
            }
        }

        private string GetCarPlateInfo(string plateText1, string plateText2, string plateText3, int plateNumber, string lang)
        {
            var carPlateInfo = new Tamkeen.bll.Model.CarPlateInfo(plateText1, plateText2, plateText3, plateNumber);
            if (lang == "en")
            {
                return carPlateInfo.CarPlateTextEn + " " + carPlateInfo.CarPlateNumberEn;
            }
            else
            {
                return carPlateInfo.CarPlateTextAr + " " + carPlateInfo.CarPlateNumberAr;
            }
        }

        public QuotationsFormOutput GetBulkQuotaionInsuranceProposalDetailsByExternalId(string externalId, string lang, out string exception)
        {
            exception = string.Empty;
            QuotationsFormOutput output = new QuotationsFormOutput();
            PdfGenerationLog log = new PdfGenerationLog();

            var langId = (lang.ToLower() == "en") ? 2 : 1;

            try
            {
                string exp = string.Empty;
                InsuranceProposalInfoModel quoteRequest = _quotationService.GetInsuranceProposalDetails(externalId, out exp);
                if (quoteRequest == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null and exp:"+exp;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (quoteRequest.DepreciationSettingHistory == null && quoteRequest.DepreciationSetting == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
                    output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quoteRequest.Bank.Id}, Maker: {quoteRequest.VehicleMakerCode}, Model: {quoteRequest.VehicleModelCode}";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                QuotaionInsuranceProposalModel data = new QuotaionInsuranceProposalModel();
                AutoleasingDepreciationSetting depreciationSetting = null;

                #region Lessor Information

                data.CommercialRegistrationNo = quoteRequest.InsuredNationalId;
                data.LessorName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.Bank?.NameEn : quoteRequest.Bank?.NameAr;
                data.LessorPhoneNumber = quoteRequest.Bank?.PhoneNumber;
                data.LessorNationalAddress = quoteRequest.Bank?.NationalAddress;

                #endregion

                #region Lessee Information

                data.NationalId = quoteRequest.DriverNationalId;
                data.LesseeName = (langId == (int)LanguageTwoLetterIsoCode.En)
                    ? quoteRequest.MainDriverNameEn
                    : quoteRequest.MainDriverNameAr;
                data.LesseePhoneNumber = quoteRequest.MobileNumber;
                data.LesseeNationalAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.MainDriverAddressEn : quoteRequest.MainDriverAddressAr;

                #endregion

                #region Vehicle Details

                data.RegistrationPlateNo = (!string.IsNullOrEmpty(quoteRequest.VehicleId))
                    ? GetCarPlateInfo(quoteRequest.CarPlateText1, quoteRequest.CarPlateText2,
                    quoteRequest.CarPlateText3, (quoteRequest.CarPlateNumber.HasValue) ? quoteRequest.CarPlateNumber.Value : 0, lang) : null;
                data.ChassisNumber = quoteRequest.ChassisNumber;
                data.VehicleColor = (!string.IsNullOrEmpty(quoteRequest.VehicleId)) ? quoteRequest.MajorColor : null;
                data.VehicleRegistrationExpiryDate = quoteRequest.VehicleLicenseExpiryDate;
                data.BodyType = (!string.IsNullOrEmpty(quoteRequest.VehicleId)) ? ((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.BodyTypeEn : quoteRequest.BodyTypeAr) : null;

                data.VehicleIdTypeId = quoteRequest.VehicleBodyCode;
                data.VehicleId = quoteRequest.VehicleId;
                data.VehicleMaker = quoteRequest.VehicleMaker;
                data.VehicleModel = quoteRequest.VehicleModel;
                data.ManufactureYear = quoteRequest.VehicleYear;

                List<string> additionalDriversNames = new List<string>();
                if (!string.IsNullOrEmpty(quoteRequest.AdditionalDriverOneNameEn) && !string.IsNullOrEmpty(quoteRequest.AdditionalDriverOneNameAr))
                    additionalDriversNames.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.AdditionalDriverOneNameEn : quoteRequest.AdditionalDriverOneNameAr);

                if (!string.IsNullOrEmpty(quoteRequest.AdditionalDriverTwoNameEn) && !string.IsNullOrEmpty(quoteRequest.AdditionalDriverTwoNameAr))
                    additionalDriversNames.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? quoteRequest.AdditionalDriverTwoNameEn : quoteRequest.AdditionalDriverTwoNameAr);

                if (additionalDriversNames != null && additionalDriversNames.Count > 0 && additionalDriversNames.Any(a => !string.IsNullOrWhiteSpace(a)))
                    data.NamesOfAuthorizedDrivers = string.Join(", ", additionalDriversNames);

                data.VehicleValue = quoteRequest.VehicleValue.Value;
                data.DeductibleValue = quoteRequest.QuotationResponses.OrderByDescending(a => a.Id).FirstOrDefault(a => a.Deductible.HasValue)?.Deductible;

                var lastResponse = quoteRequest.QuotationResponses.OrderByDescending(a => a.Id).FirstOrDefault();
                data.RepairMethod = lastResponse.VehicleAgencyRepair;

                //AutoleasingAgencyRepair bankRepairMethodSettings = null;
                //if (quoteRequest.RepairMethodeSettingHistory != null)
                //{
                //    var bankRepairMethodSettingsHistory = quoteRequest.RepairMethodeSettingHistory;
                //    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                //    {
                //        BankId = bankRepairMethodSettingsHistory.BankId,
                //        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                //        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                //        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                //        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                //        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                //    };
                //}
                //else
                //{
                //    var _bankRepairMethodSettings = quoteRequest.RepairMethodeSetting;
                //    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                //    {
                //        BankId = _bankRepairMethodSettings.BankId,
                //        FirstYear = _bankRepairMethodSettings.FirstYear,
                //        SecondYear = _bankRepairMethodSettings.SecondYear,
                //        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                //        FourthYear = _bankRepairMethodSettings.FourthYear,
                //        FifthYear = _bankRepairMethodSettings.FifthYear,
                //    };
                //}

                //List<string> repairMethodList = new List<string>();
                //var contractDuration = quoteRequest.ContractDuration.Value;
                //if (contractDuration >= 1)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                //}
                //if (contractDuration >= 2)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                //}
                //if (contractDuration >= 3)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                //}
                //if (contractDuration >= 4)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                //}
                //if (contractDuration >= 5)
                //{
                //    repairMethodList.Add(bankRepairMethodSettings.FifthYear);
                //}

                //List<string> allRepairMethods = new List<string>();
                //foreach (var item in repairMethodList)
                //    allRepairMethods.Add(item.Substring(0, 1));

                //if (allRepairMethods != null && allRepairMethods.Count > 0)
                //    data.RepairMethod = string.Join(", ", allRepairMethods);

                List<string> selectedBenfits = new List<string>();
                var quotationAutoLeasingSelectedBenfits = _autoLeasingSelectedBenfitsRepository.TableNoTracking.Where(a => a.ExternalId == externalId).ToList();
                if (quotationAutoLeasingSelectedBenfits.Any() && quotationAutoLeasingSelectedBenfits.Count > 0)
                {
                    foreach (var selectedBenfit in quotationAutoLeasingSelectedBenfits)
                    {
                        var benfit = _benfitRepository.TableNoTracking.FirstOrDefault(a => a.Code == selectedBenfit.BenifitId);
                        if (benfit != null)
                            selectedBenfits.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.EnglishDescription : benfit.ArabicDescription);
                    }

                    data.AdditionalBenefits = string.Join(", ", data.AdditionalBenefits);
                }

                #endregion

                #region Policy Details

                foreach (var qrResponse in quoteRequest.QuotationResponses)
                {
                    if (qrResponse.PolicyEffectiveDate.HasValue && qrResponse.PolicyExpiryDate.HasValue)
                    {
                        var policyEffectiveDateStart = (langId == (int)LanguageTwoLetterIsoCode.En)
                        ? $"{ qrResponse.PolicyEffectiveDate.Value.Day }-{ qrResponse.PolicyEffectiveDate.Value.Month }-{ qrResponse.PolicyEffectiveDate.Value.Year }"
                        : $"{ qrResponse.PolicyEffectiveDate.Value.Year }-{ qrResponse.PolicyEffectiveDate.Value.Month }-{ qrResponse.PolicyEffectiveDate.Value.Day}";

                        var policyEffectiveDateEnd = (langId == (int)LanguageTwoLetterIsoCode.En)
                            ? $"{ qrResponse.PolicyExpiryDate.Value.Day }-{ qrResponse.PolicyExpiryDate.Value.Month }-{ qrResponse.PolicyExpiryDate.Value.Year }"
                            : $"{ qrResponse.PolicyExpiryDate.Value.Year }-{ qrResponse.PolicyExpiryDate.Value.Month }-{ qrResponse.PolicyExpiryDate.Value.Day}";

                        data.Convergeperiod = (langId == (int)LanguageTwoLetterIsoCode.En)
                            ? $"{ policyEffectiveDateStart } { AutoLeasingQuotationResources.to } { policyEffectiveDateEnd }" : $"{ policyEffectiveDateEnd } { AutoLeasingQuotationResources.to } { policyEffectiveDateStart }";

                        break;
                    }
                }

                #endregion

                #region Deprecation Data

                if (quoteRequest.DepreciationSettingHistory != null)
                {
                    var deprecationHistory = quoteRequest.DepreciationSettingHistory;
                    depreciationSetting = new AutoleasingDepreciationSetting()
                    {
                        BankId = quoteRequest.Bank.Id,
                        MakerCode = deprecationHistory.MakerCode,
                        ModelCode = deprecationHistory.ModelCode,
                        MakerName = deprecationHistory.MakerName,
                        ModelName = deprecationHistory.ModelName,
                        Percentage = deprecationHistory.Percentage,
                        IsDynamic = deprecationHistory.IsDynamic,
                        FirstYear = deprecationHistory.FirstYear,
                        SecondYear = deprecationHistory.SecondYear,
                        ThirdYear = deprecationHistory.ThirdYear,
                        FourthYear = deprecationHistory.FourthYear,
                        FifthYear = deprecationHistory.FifthYear,
                        AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                    };
                }
                else
                {
                    var deprecationSetting = quoteRequest.DepreciationSetting;
                    depreciationSetting = new AutoleasingDepreciationSetting();
                    depreciationSetting.BankId = quoteRequest.Bank.Id;
                    depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSetting.MakerName = deprecationSetting.MakerName;
                    depreciationSetting.ModelName = deprecationSetting.ModelName;
                    depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
                }

                if (depreciationSetting != null)
                {
                    data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;

                    List<string> annualPercentages = new List<string>();
                    if (!depreciationSetting.FirstYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FirstYear.ToString().Split('.')[0] + "%");
                        data.Firstyear = depreciationSetting.FirstYear.ToString().Split('.')[0] + "%";
                    }
                    if (!depreciationSetting.SecondYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + "%");
                        data.Secondyear = depreciationSetting.SecondYear.ToString().Split('.')[0] + "%";
                    }
                    if (!depreciationSetting.ThirdYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + "%");
                        data.Thirdyear = depreciationSetting.ThirdYear.ToString() + "%";
                    }
                    if (!depreciationSetting.FourthYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + "%");
                        data.Fourthyear = depreciationSetting.FourthYear.ToString().Split('.')[0] + "%";
                    }
                    if (!depreciationSetting.FifthYear.Equals(null))
                    {
                        annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + "%");
                        data.Fifthyear = depreciationSetting.FifthYear.ToString().Split('.')[0] + "%";
                    }

                    data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + "%";
                    data.AnnualDeprecationPercentage = data.Percentage;
                }

                #endregion

                var vehicleValue = quoteRequest.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }

                data.Firstyear = Math.Round(DepreciationValue1.Value, 2).ToString().Replace(".00", "");
                data.Secondyear = Math.Round(DepreciationValue2.Value, 2).ToString().Replace(".00", "");
                data.Thirdyear = Math.Round(DepreciationValue3.Value, 2).ToString().Replace(".00", "");
                data.Fourthyear = Math.Round(DepreciationValue4.Value, 2).ToString().Replace(".00", "");
                data.Fifthyear = Math.Round(DepreciationValue5.Value, 2).ToString().Replace(".00", "");

                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;

                log.ServerIP = Utilities.GetInternalServerIP();
                //log.CompanyID = quoteResponse.InsuranceCompanyId;
                if (string.IsNullOrEmpty(log.Channel))
                    log.Channel = Channel.autoleasing.ToString();

                string policyDetailsJsonString = JsonConvert.SerializeObject(data);
                AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
                {
                    ReportType = "InsuranceProposalTemplate",
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
                log.ServiceResponse = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                return output;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();

                log.ServiceResponse = exception;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Exception Error";
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return null;
            }
        }

        public QuotationsFormOutput GetQuotaionsFormDetailsByExternalId(string externalId, string lang, out string exception)
        {
            exception = string.Empty;
            QuotationsFormOutput output = new QuotationsFormOutput();
            PdfGenerationLog log = new PdfGenerationLog();

            var langId = (lang.ToLower() == "en") ? 2 : 1;

            try
            {
                QuotationInfoModel quotationInfo = _quotationService.GetBulkQuotationsDetails(externalId);
                if (quotationInfo == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (quotationInfo.DepreciationSettingHistory == null && quotationInfo.DepreciationSetting == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
                    output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quotationInfo.Bank.Id}, Maker: {quotationInfo.VehicleMakerCode}, Model: {quotationInfo.VehicleModelCode}";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (quotationInfo.RepairMethodeSetting == null && quotationInfo.RepairMethodeSettingHistory == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                    output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + quotationInfo.Bank.Id;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                QuotationsFormTemplateViewModel data = new QuotationsFormTemplateViewModel();
                data.InsuredNationalId = quotationInfo.InsuredNationalId;
                data.DriverName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverNameEn : quotationInfo.MainDriverNameAr;
                data.DriverNationalId = quotationInfo.DriverNationalId;
                data.MainDriverAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverAddressEn : quotationInfo.MainDriverAddressAr;
                data.NCDFreeYears = (quotationInfo.NajmNcdFreeYears.HasValue) ? quotationInfo.NajmNcdFreeYears.Value.ToString() : "Not Eligible";
                data.VehicleId = quotationInfo.VehicleId;
                data.VehicleValue = quotationInfo.VehicleValue.Value.ToString();
                data.VehicleMaker = quotationInfo.VehicleMaker;
                data.VehicleModel = quotationInfo.VehicleModel;
                data.VehicleYear = quotationInfo.VehicleYear;
                data.Deductible = quotationInfo.Products?.FirstOrDefault(a => a.DeductableValue.HasValue)?.DeductableValue.ToString();
                data.VehicleOwnerName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.Bank.NameEn : quotationInfo.Bank.NameAr;

                AutoleasingAgencyRepair bankRepairMethodSettings = null;
                if (quotationInfo.RepairMethodeSettingHistory != null)
                {
                    var bankRepairMethodSettingsHistory = quotationInfo.RepairMethodeSettingHistory;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = bankRepairMethodSettingsHistory.BankId,
                        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                    };
                }
                else
                {
                    var _bankRepairMethodSettings = quotationInfo.RepairMethodeSetting;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = _bankRepairMethodSettings.BankId,
                        FirstYear = _bankRepairMethodSettings.FirstYear,
                        SecondYear = _bankRepairMethodSettings.SecondYear,
                        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                        FourthYear = _bankRepairMethodSettings.FourthYear,
                        FifthYear = _bankRepairMethodSettings.FifthYear,
                    };
                    bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id).FirstOrDefault();
                }

                var contractDuration = quotationInfo.ContractDuration.Value;

                string repairMethodString = string.Empty;
                var policiesNumbers = 0;// _checkoutService.GetPoliciesCount(quotationInfo.DriverNationalId, quotationInfo.VehicleId, quotationInfo.Bank.Id);
                if (policiesNumbers == 0)
                    repairMethodString = bankRepairMethodSettings.FirstYear;
                else if (policiesNumbers == 1)
                    repairMethodString = bankRepairMethodSettings.SecondYear;
                else if (policiesNumbers == 2)
                    repairMethodString = bankRepairMethodSettings.ThirdYear;
                else if (policiesNumbers == 3)
                    repairMethodString = bankRepairMethodSettings.FourthYear;
                else if (policiesNumbers == 4)
                    repairMethodString = bankRepairMethodSettings.FifthYear;

                data.Quotationlist = new List<QuotationsFormTemplateQuoteViewModel>();
                List<string> selectedAdditionalBenfitsString = new List<string>();
                foreach (var product in quotationInfo.Products.Where(q => q.VehicleRepairType == repairMethodString))
                {
                    var singleQuotation = new QuotationsFormTemplateQuoteViewModel();
                    singleQuotation.CompanyKey = product.CompanyKey;
                    //singleQuotation.ImageURL = product.CompanyKey;
                    singleQuotation.ProductName = "Comprehensive";

                    var productPriceDetails = product.PriceDetails;
                    if (productPriceDetails == null)
                        continue;

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 7))
                    {
                        var basicPremium = productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 7).PriceValue;
                        singleQuotation.TotalPremium = Math.Round(basicPremium, 2).ToString();
                        singleQuotation.InsurancePercentage = (quotationInfo.VehicleValue.Value / basicPremium).ToString();
                    }
                    else
                    {
                        singleQuotation.TotalPremium = "0.00";
                        singleQuotation.InsurancePercentage = "0.00";
                    }

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 4))
                        singleQuotation.ClaimLoading = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 4).PriceValue, 2).ToString();
                    else
                        singleQuotation.ClaimLoading = "0.00";

                    //singleQuotation.MinimumPremium = product.PriceDetails.FirstOrDefault(a => a.PriceTypeCode == )?.PriceValue.ToString();

                    //singleQuotation.InsurancePercentage = "0.00";
                    singleQuotation.MinimumPremium = "0.00";

                    decimal _shadowAmount = 0;
                    foreach (var price in productPriceDetails)
                    {
                        if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 ||
                                price.PriceTypeCode == 11 || price.PriceTypeCode == 12) // Discounts
                            _shadowAmount += price.PriceValue;
                    }

                    if (_shadowAmount > 0)
                        singleQuotation.ShadowAmount = Math.Round(_shadowAmount, 2).ToString();
                    else
                        singleQuotation.ShadowAmount = "0.00";

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 3))
                        singleQuotation.LoyalityAmount = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 3).PriceValue, 2).ToString();
                    else
                        singleQuotation.LoyalityAmount = "0.00";

                    List<string> selectedBenfits = new List<string>();
                    var quotationAutoLeasingSelectedBenfits = _autoLeasingSelectedBenfitsRepository.TableNoTracking.Where(a => a.ExternalId == externalId).ToList();

                    singleQuotation.Benefits = new List<QuotationsFormTemplateQuoteBenfitViewModel>();
                    var productBenfits = product.Benfits;
                    if (productBenfits != null)
                    {
                        decimal _selectedBenfitVat = 0;
                        decimal _selectedBenfitTotalPrice = 0;
                        foreach (var benfit in productBenfits)
                        {
                            if (benfit.IsReadOnly)
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = true
                                });
                                continue;
                            }
                            else if (quotationAutoLeasingSelectedBenfits != null && quotationAutoLeasingSelectedBenfits.Any(a => a.BenifitId == benfit.BenefitId.Value))
                            {
                                if (benfit.BenefitPrice.HasValue)
                                {
                                    _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
                                    _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                }

                                selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = true
                                });
                            }
                            else
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = false
                                });
                            }
                        }

                        if (productPriceDetails.Any(a => a.PriceTypeCode == 8))
                            singleQuotation.VAT = Math.Round((_selectedBenfitVat + productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 8).PriceValue), 2).ToString();
                        else if (_selectedBenfitVat > 0)
                            singleQuotation.VAT = (Math.Round(_selectedBenfitVat, 2)).ToString();
                        else
                            singleQuotation.VAT = "0.00";

                        singleQuotation.Total = (Math.Round(product.ProductPrice + _selectedBenfitVat + _selectedBenfitTotalPrice, 2)).ToString();
                    }

                    data.Quotationlist.Add(singleQuotation);
                }

                data.TotalQuotations = data.Quotationlist.Count;
                data.AdditionalBenfits = (selectedAdditionalBenfitsString != null && selectedAdditionalBenfitsString.Count > 0)
                    ? string.Join(", ", selectedAdditionalBenfitsString) : null;

                //data.RepairType = quotationInfo.VehicleRepairType;
                //data.Deductible = quotationInfo.DeductibleValue.ToString();

                AutoleasingDepreciationSetting depreciationSetting = null;
                #region Deprecation Data

                if (quotationInfo.DepreciationSettingHistory != null)
                {
                    var deprecationHistory = quotationInfo.DepreciationSettingHistory;
                    depreciationSetting = new AutoleasingDepreciationSetting()
                    {
                        BankId = quotationInfo.Bank.Id,
                        MakerCode = deprecationHistory.MakerCode,
                        ModelCode = deprecationHistory.ModelCode,
                        MakerName = deprecationHistory.MakerName,
                        ModelName = deprecationHistory.ModelName,
                        Percentage = deprecationHistory.Percentage,
                        IsDynamic = deprecationHistory.IsDynamic,
                        FirstYear = deprecationHistory.FirstYear,
                        SecondYear = deprecationHistory.SecondYear,
                        ThirdYear = deprecationHistory.ThirdYear,
                        FourthYear = deprecationHistory.FourthYear,
                        FifthYear = deprecationHistory.FifthYear,
                        AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                    };
                }
                else
                {
                    var deprecationSetting = quotationInfo.DepreciationSetting;
                    depreciationSetting = new AutoleasingDepreciationSetting();
                    depreciationSetting.BankId = quotationInfo.Bank.Id;
                    depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSetting.MakerName = deprecationSetting.MakerName;
                    depreciationSetting.ModelName = deprecationSetting.ModelName;
                    depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
                }

                data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;
                data.AnnualDeprecationPercentage = depreciationSetting.Percentage.ToString();

                List<string> annualPercentages = new List<string>();
                if (contractDuration >= 1)
                {
                    if (!depreciationSetting.FirstYear.Equals(null))
                    {
                        //data.FirstYear = depreciationSetting.FirstYear.ToString().Split('.')[0] + "%";
                        //annualPercentages.Add(depreciationSetting.FirstYear.ToString().Split('.')[0] + "%");
                        data.FirstYear = "0 %";
                        annualPercentages.Add("0 %");
                    }
                }

                if (contractDuration >= 2)
                {
                    if (!depreciationSetting.SecondYear.Equals(null))
                    {
                        data.SecondYear = depreciationSetting.SecondYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 3)
                {
                    if (!depreciationSetting.ThirdYear.Equals(null))
                    {
                        data.ThirdYear = depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 4)
                {
                    if (!depreciationSetting.FourthYear.Equals(null))
                    {
                        data.FourthYear = depreciationSetting.FourthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 5)
                {
                    if (!depreciationSetting.FifthYear.Equals(null))
                    {
                        data.FifthYear = depreciationSetting.FifthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + " %");
                    }
                }

                data.IsDynamic = depreciationSetting.IsDynamic;
                data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + " %";


                var vehicleValue = quotationInfo.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }
                List<PremiumReference> premiumReference = new List<PremiumReference>();
                List<string> repairMethodList = new List<string>();
                if (contractDuration >= 1)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                }
                if (contractDuration >= 2)
                {
                    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                }
                if (contractDuration >= 3)
                {
                    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                }
                if (contractDuration >= 4)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                }
                if (contractDuration >= 5)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FifthYear);
                }

                List<string> allRepairMethods = new List<string>();
                foreach (var item in repairMethodList)
                    allRepairMethods.Add(item.Substring(0, 1));

                if (allRepairMethods != null && allRepairMethods.Count > 0)
                    data.RepairType = string.Join(", ", allRepairMethods);

                List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();

                int countAgency = repairMethodList.Where(r => r == "Agency").Count();
                int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();

                if (repairMethodList.Count() == countAgency)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Agency").ToList();
                }
                else if (repairMethodList.Count() == countWorkShop)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Workshop").ToList();
                }
                else
                {
                    products = quotationInfo.Products;
                }

                foreach (var _product in products)
                {
                    //var product = quotation.Products.FirstOrDefault();
                    if (_product == null)
                        continue;
                    if (_product.PriceDetails == null)
                        continue;

                    decimal basicPremium = _product.PriceDetails.Where(p => p.PriceTypeCode == 7).FirstOrDefault().PriceValue;

                    decimal? otherTypeCodes = _product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                    || p.PriceTypeCode == 6 || p.PriceTypeCode == 8 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);


                    //var discounts = product.PriceDetails.Where(p => p.PriceTypeCode == 1 || p.PriceTypeCode == 2
                    //  || p.PriceTypeCode == 3 || p.PriceTypeCode == 10 || p.PriceTypeCode == 11 || p.PriceTypeCode == 12)?.Sum(p => p.PriceValue);

                    //List<short?> selectedBenfitsIds = new List<short?>();
                    //if (benfits != null)
                    //    selectedBenfitsIds = benfits.FirstOrDefault(a => a.ProductId == _product.ProductID)?.BenfitIds;

                    //decimal? benefits = 0;
                    //if (selectedBenfitsIds != null && benfits.Any())
                    //{
                    //    benefits = _product.Benfits.Where(b => b.BenefitId.HasValue && selectedBenfitsIds.Contains(b.BenefitId.Value))?.Sum(b => b.BenefitPrice.Value);
                    //}
                    decimal? benefits = 0;
                    var premium = basicPremium + otherTypeCodes + (benefits * 1.15M);// - discounts;
                    premiumReference.Add(new PremiumReference { Premium = premium, ReferenceId = _product.ReferenceId, BasicPremium = basicPremium });
                }

                var lowestPremium = premiumReference.OrderBy(p => p.Premium).FirstOrDefault();
                Decimal? InsurancePercentage1 = 0;
                Decimal? InsurancePercentage2 = 0;
                Decimal? InsurancePercentage3 = 0;
                Decimal? InsurancePercentage4 = 0;
                Decimal? InsurancePercentage5 = 0;

                Decimal? BasicPremium = lowestPremium.BasicPremium;

                AutoleasingMinimumPremium bankMinimumPremiumSettings = null;
                if (quotationInfo.MinimumPremiumSettingHistory != null)
                {
                    var minimumPremiumSettingHistory = quotationInfo.MinimumPremiumSettingHistory;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSettingHistory.FirstYear,
                        SecondYear = minimumPremiumSettingHistory.SecondYear,
                        ThirdYear = minimumPremiumSettingHistory.ThirdYear,
                        FourthYear = minimumPremiumSettingHistory.FourthYear,
                        FifthYear = minimumPremiumSettingHistory.FifthYear
                    };
                }
                else
                {
                    var minimumPremiumSetting = quotationInfo.MinimumPremiumSetting;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSetting.FirstYear,
                        SecondYear = minimumPremiumSetting.SecondYear,
                        ThirdYear = minimumPremiumSetting.ThirdYear,
                        FourthYear = minimumPremiumSetting.FourthYear,
                        FifthYear = minimumPremiumSetting.FifthYear
                    };
                }

                if (contractDuration >= 1)
                {
                    data.RepairMethod1 = bankRepairMethodSettings.FirstYear;
                    InsurancePercentage1 = (BasicPremium * 100) / vehicleValue;
                    data.InsurancePercentage1 = Math.Round(InsurancePercentage1.Value, 2).ToString();
                    data.Premium1 = Math.Round(lowestPremium.Premium.Value, 2).ToString();
                    data.Deductible1 = "1500";
                    data.MiuimumPremium1 = bankMinimumPremiumSettings.FirstYear.ToString().Split('.')[0];

                }
                if (contractDuration >= 2)
                {
                    data.RepairMethod2 = bankRepairMethodSettings.SecondYear;
                    InsurancePercentage2 = (BasicPremium * 100) / DepreciationValue2;
                    data.InsurancePercentage2 = Math.Round(InsurancePercentage2.Value, 2).ToString();
                    data.Premium2 = Math.Round((InsurancePercentage1.Value * DepreciationValue2.Value / 100), 2).ToString();
                    data.Deductible2 = "1500";
                    data.MiuimumPremium2 = bankMinimumPremiumSettings.SecondYear.ToString().Split('.')[0];

                }
                if (contractDuration >= 3)
                {
                    data.RepairMethod3 = bankRepairMethodSettings.ThirdYear;
                    InsurancePercentage3 = (BasicPremium * 100) / DepreciationValue3;
                    data.InsurancePercentage3 = Math.Round(InsurancePercentage3.Value, 2).ToString();
                    data.Premium3 = Math.Round((InsurancePercentage2.Value * DepreciationValue3.Value / 100), 2).ToString();
                    data.Deductible3 = "1000";
                    data.MiuimumPremium3 = bankMinimumPremiumSettings.ThirdYear.ToString().Split('.')[0];

                }
                if (contractDuration >= 4)
                {
                    data.RepairMethod4 = bankRepairMethodSettings.FourthYear;
                    InsurancePercentage4 = (BasicPremium * 100) / DepreciationValue4;
                    data.InsurancePercentage4 = Math.Round(InsurancePercentage4.Value, 2).ToString();
                    data.Premium4 = Math.Round((InsurancePercentage3.Value * DepreciationValue4.Value / 100), 2).ToString();
                    data.Deductible4 = "1000";
                    data.MiuimumPremium4 = bankMinimumPremiumSettings.FourthYear.ToString().Split('.')[0];

                }
                if (contractDuration >= 5)
                {
                    data.RepairMethod5 = bankRepairMethodSettings.FifthYear;
                    InsurancePercentage5 = (BasicPremium * 100) / DepreciationValue5;
                    data.InsurancePercentage5 = Math.Round(InsurancePercentage5.Value, 2).ToString();
                    data.Premium5 = Math.Round((InsurancePercentage4.Value * DepreciationValue5.Value / 100), 2).ToString();
                    data.Deductible5 = "1000";
                    data.MiuimumPremium5 = bankMinimumPremiumSettings.FifthYear.ToString().Split('.')[0];

                }
                #endregion

                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator/PostAutoLease";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;

                log.ServerIP = Utilities.GetInternalServerIP();
                //log.CompanyID = quoteResponse.InsuranceCompanyId;
                if (string.IsNullOrEmpty(log.Channel))
                    log.Channel = Channel.autoleasing.ToString();

                string policyDetailsJsonString = JsonConvert.SerializeObject(data);
                AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
                {
                    ReportType = "QuotationsFormTemplate",
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
                log.ServiceResponse = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                return output;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        public QuotationsFormOutput GetQuotaionsInfoByExternalId(QuotationRequestLog predefinedLog, QuotationFormWithSelectedBenfitsViewModel model, out string exception)
        {
            exception = string.Empty;
            var langId = (model.lang.ToLower() == "en") ? 2 : 1;

            QuotationsFormOutput output = new QuotationsFormOutput();
            //PdfGenerationLog log = new PdfGenerationLog();

            try
            {
                QuotationInfoModel quotationInfo = _quotationService.GetQuotationsDetails(model.qtRqstExtrnlId, model.AgencyRepair, model.deductible, out exception);
                if (!string.IsNullOrEmpty(exception))
                {

                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " db exception";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = exception;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;


                }
                if (quotationInfo == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "quotationInfo is null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (quotationInfo.DepreciationSettingHistory == null && quotationInfo.DepreciationSetting == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
                    output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quotationInfo.Bank.Id}, Maker: {quotationInfo.VehicleMakerCode}, Model: {quotationInfo.VehicleModelCode}";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (quotationInfo.RepairMethodeSetting == null && quotationInfo.RepairMethodeSettingHistory == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                    output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + quotationInfo.Bank.Id;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }

                // as per jira (https://bcare.atlassian.net/browse/ALP-110)
                if (model.SelectedCompany > 0)
                    quotationInfo.Products = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).ToList();

                QuotationsFormTemplateViewModel data = new QuotationsFormTemplateViewModel();
                data.ExternalId = model.qtRqstExtrnlId;
                data.InsuredNationalId = quotationInfo.InsuredNationalId;
                data.DriverName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverNameEn : quotationInfo.MainDriverNameAr;
                data.DriverNationalId = quotationInfo.DriverNationalId;
                data.MainDriverAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverAddressEn : quotationInfo.MainDriverAddressAr;
                data.NCDFreeYears = (quotationInfo.NajmNcdFreeYears.HasValue) ? quotationInfo.NajmNcdFreeYears.Value.ToString() : "Not Eligible";
                data.VehicleId = quotationInfo.VehicleId;
                data.VehicleValue = quotationInfo.VehicleValue.Value.ToString();
                data.VehicleMaker = quotationInfo.VehicleMaker;
                data.VehicleModel = quotationInfo.VehicleModel;
                data.VehicleYear = quotationInfo.VehicleYear;
                data.Deductible = model.deductible.ToString();
                data.VehicleOwnerName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.Bank.NameEn : quotationInfo.Bank.NameAr;

                AutoleasingAgencyRepair bankRepairMethodSettings = null;
                if (quotationInfo.RepairMethodeSettingHistory != null)
                {
                    var bankRepairMethodSettingsHistory = quotationInfo.RepairMethodeSettingHistory;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = bankRepairMethodSettingsHistory.BankId,
                        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                    };
                }
                else
                {
                    var _bankRepairMethodSettings = quotationInfo.RepairMethodeSetting;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = _bankRepairMethodSettings.BankId,
                        FirstYear = _bankRepairMethodSettings.FirstYear,
                        SecondYear = _bankRepairMethodSettings.SecondYear,
                        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                        FourthYear = _bankRepairMethodSettings.FourthYear,
                        FifthYear = _bankRepairMethodSettings.FifthYear,
                    };
                    bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id).FirstOrDefault();
                }

                var contractDuration = quotationInfo.ContractDuration.Value;

                string repairMethodString = string.Empty;
                var policiesNumbers = 0;// _checkoutService.GetPoliciesCount(quotationInfo.DriverNationalId, quotationInfo.VehicleId, quotationInfo.Bank.Id);
                if (policiesNumbers == 0)
                    repairMethodString = bankRepairMethodSettings.FirstYear;
                else if (policiesNumbers == 1)
                    repairMethodString = bankRepairMethodSettings.SecondYear;
                else if (policiesNumbers == 2)
                    repairMethodString = bankRepairMethodSettings.ThirdYear;
                else if (policiesNumbers == 3)
                    repairMethodString = bankRepairMethodSettings.FourthYear;
                else if (policiesNumbers == 4)
                    repairMethodString = bankRepairMethodSettings.FifthYear;

                data.Quotationlist = new List<QuotationsFormTemplateQuoteViewModel>();
                List<string> selectedAdditionalBenfitsString = new List<string>();
                foreach (var product in quotationInfo.Products.Where(q => q.VehicleRepairType == repairMethodString))
                {
                    var singleQuotation = new QuotationsFormTemplateQuoteViewModel();
                    singleQuotation.CompanyKey = product.CompanyKey;
                    //singleQuotation.ImageURL = product.CompanyKey;
                    singleQuotation.ProductName = "Comprehensive";
                    singleQuotation.DeductableValue = product.DeductableValue;

                    if (data.Quotationlist.Where(a => a.CompanyKey == singleQuotation.CompanyKey && a.DeductableValue == singleQuotation.DeductableValue).FirstOrDefault() != null)
                        continue;

                    var productPriceDetails = product.PriceDetails;
                    if (productPriceDetails == null)
                        continue;

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 7))
                    {
                        var basicPremium = productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 7).PriceValue;
                        singleQuotation.TotalPremium = Math.Round(basicPremium, 2).ToString();
                        //singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";  // (quotationInfo.VehicleValue.Value / basicPremium).ToString();
                    }
                    else
                    {
                        singleQuotation.TotalPremium = "0.00";
                        //singleQuotation.InsurancePercentage = "0.00";
                    }

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 4))
                        singleQuotation.ClaimLoading = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 4).PriceValue, 2).ToString();
                    else
                        singleQuotation.ClaimLoading = "0.00";

                    //singleQuotation.MinimumPremium = product.PriceDetails.FirstOrDefault(a => a.PriceTypeCode == )?.PriceValue.ToString();

                    singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";
                    singleQuotation.ShadowAmount = product.ShadowAmount.ToString() ?? "0.00";
                    singleQuotation.MinimumPremium = "0.00";

                    //decimal _shadowAmount = 0;
                    //foreach (var price in productPriceDetails)
                    //{
                    //    if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 ||
                    //            price.PriceTypeCode == 11 || price.PriceTypeCode == 12) // Discounts
                    //        _shadowAmount += price.PriceValue;
                    //}

                    //if (_shadowAmount > 0)
                    //    singleQuotation.ShadowAmount = Math.Round(_shadowAmount, 2).ToString();
                    //else
                    //    singleQuotation.ShadowAmount = "0.00";

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 3))
                        singleQuotation.LoyalityAmount = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 3).PriceValue, 2).ToString();
                    else
                        singleQuotation.LoyalityAmount = "0.00";

                    List<short?> selectedBenfitsIds = new List<short?>();
                    if (model.SelectedBenfits != null)
                        selectedBenfitsIds = model.SelectedBenfits.FirstOrDefault(a => a.ProductId == product.ProductID)?.BenfitIds;

                    if (quotationInfo.Bank.Id == 2) //alyusr
                    {
                        var preSelectedBenefits = product.Benfits.Where(p => p.IsReadOnly && p.IsSelected.HasValue && p.IsSelected.Value);
                        if (selectedBenfitsIds != null && selectedBenfitsIds.Any())
                        {
                            var clientSelectedBenefitsId = selectedBenfitsIds.Except(preSelectedBenefits.Select(b => b.BenefitId));
                            var clientSelectedBenefits = product.Benfits.Where(b => clientSelectedBenefitsId.Contains(b.BenefitId));
                            if (clientSelectedBenefits != null && clientSelectedBenefits.Any())
                            {
                                singleQuotation.InsurancePercentage = Decimal.Round((product.ProductPrice + clientSelectedBenefits.Sum(b => b.BenefitPrice.Value * 1.15M)) * 100 / quotationInfo.VehicleValue.Value, 2).ToString();
                            }
                        }
                       
                    }

                    singleQuotation.Benefits = new List<QuotationsFormTemplateQuoteBenfitViewModel>();
                    var productBenfits = product.Benfits;
                    if (productBenfits != null)
                    {
                        decimal _selectedBenfitVat = 0;
                        decimal _selectedBenfitTotalPrice = 0;
                        foreach (var benfit in productBenfits)
                        {
                            if (benfit.IsReadOnly)
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = true
                                });
                                continue;
                            }
                            else if (selectedBenfitsIds != null && selectedBenfitsIds.IndexOf(benfit.BenefitId) != -1)
                            {
                                if (benfit.BenefitPrice.HasValue)
                                {
                                    _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
                                    _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                }

                                selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = true
                                });
                            }
                            else
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    IsChecked = false
                                });
                            }
                        }

                        if (productPriceDetails.Any(a => a.PriceTypeCode == 8))
                            singleQuotation.VAT = Math.Round((_selectedBenfitVat + productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 8).PriceValue), 2).ToString();
                        else if (_selectedBenfitVat > 0)
                            singleQuotation.VAT = (Math.Round(_selectedBenfitVat, 2)).ToString();
                        else
                            singleQuotation.VAT = "0.00";

                        singleQuotation.Total = (Math.Round(product.ProductPrice + _selectedBenfitVat + _selectedBenfitTotalPrice, 2)).ToString();
                    }
                    data.Quotationlist.Add(singleQuotation);
                }

                data.TotalQuotations = data.Quotationlist.Count;
                data.AdditionalBenfits = string.Join(", ", selectedAdditionalBenfitsString);

                #region Deprecation Data

                AutoleasingDepreciationSetting depreciationSetting = null;
                if (quotationInfo.DepreciationSettingHistory != null)
                {
                    var deprecationHistory = quotationInfo.DepreciationSettingHistory;
                    depreciationSetting = new AutoleasingDepreciationSetting()
                    {
                        BankId = quotationInfo.Bank.Id,
                        MakerCode = deprecationHistory.MakerCode,
                        ModelCode = deprecationHistory.ModelCode,
                        MakerName = deprecationHistory.MakerName,
                        ModelName = deprecationHistory.ModelName,
                        Percentage = deprecationHistory.Percentage,
                        IsDynamic = deprecationHistory.IsDynamic,
                        FirstYear = deprecationHistory.FirstYear,
                        SecondYear = deprecationHistory.SecondYear,
                        ThirdYear = deprecationHistory.ThirdYear,
                        FourthYear = deprecationHistory.FourthYear,
                        FifthYear = deprecationHistory.FifthYear,
                        AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                    };
                }
                else
                {
                    var deprecationSetting = quotationInfo.DepreciationSetting;
                    depreciationSetting = new AutoleasingDepreciationSetting();
                    depreciationSetting.BankId = quotationInfo.Bank.Id;
                    depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSetting.MakerName = deprecationSetting.MakerName;
                    depreciationSetting.ModelName = deprecationSetting.ModelName;
                    depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
                }

                data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;
                data.AnnualDeprecationPercentage = depreciationSetting.Percentage.ToString();

                List<string> annualPercentages = new List<string>();

                if (contractDuration >= 1)
                {
                    if (!depreciationSetting.FirstYear.Equals(null))
                    {
                        //data.FirstYear = depreciationSetting.FirstYear.ToString().Split('.')[0] + "%";
                        //annualPercentages.Add(depreciationSetting.FirstYear.ToString().Split('.')[0] + "%");
                        data.FirstYear = "0 %";
                        annualPercentages.Add("0 %");
                    }
                }

                if (contractDuration >= 2)
                {
                    if (!depreciationSetting.SecondYear.Equals(null))
                    {
                        data.SecondYear = depreciationSetting.SecondYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 3)
                {
                    if (!depreciationSetting.ThirdYear.Equals(null))
                    {
                        data.ThirdYear = depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 4)
                {
                    if (!depreciationSetting.FourthYear.Equals(null))
                    {
                        data.FourthYear = depreciationSetting.FourthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 5)
                {
                    if (!depreciationSetting.FifthYear.Equals(null))
                    {
                        data.FifthYear = depreciationSetting.FifthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + " %");
                    }
                }

                data.IsDynamic = depreciationSetting.IsDynamic;
                data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + " %";


                var vehicleValue = quotationInfo.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }
                List<PremiumReference> premiumReference = new List<PremiumReference>();
                List<string> repairMethodList = new List<string>();
                if (contractDuration >= 1)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                }
                if (contractDuration >= 2)
                {
                    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                }
                if (contractDuration >= 3)
                {
                    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                }
                if (contractDuration >= 4)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                }
                if (contractDuration >= 5)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FifthYear);
                }

                List<string> allRepairMethods = new List<string>();
                foreach (var item in repairMethodList)
                    allRepairMethods.Add(item.Substring(0, 1));

                if (allRepairMethods != null && allRepairMethods.Count > 0)
                    data.RepairType = string.Join(", ", allRepairMethods);

                List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();

                int countAgency = repairMethodList.Where(r => r == "Agency").Count();
                int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();
                string RepairType = string.Empty;
                if (repairMethodList.Count() == countAgency)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Agency").ToList();
                    RepairType = "Agency";
                }
                else if (repairMethodList.Count() == countWorkShop)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Workshop").ToList();
                    RepairType = "Workshop";
                }
                else
                {
                    products = quotationInfo.Products;
                    RepairType = "Mixed";
                }

                Decimal? InsurancePercentageAgency = 0;
                Decimal? InsurancePercentageWorkshop = 0;

                foreach (var _product in products)
                {
                    //var product = quotation.Products.FirstOrDefault();
                    if (_product == null)
                        continue;
                    if (_product.PriceDetails == null)
                        continue;

                    //if (InsurancePercentageAgency == 0 && _product.VehicleRepairType == "Agency")
                    //{
                    //    InsurancePercentageAgency = _product.InsurancePercentage;
                    //}
                    //else if (InsurancePercentageWorkshop == 0 && _product.VehicleRepairType == "Workshop")
                    //{
                    //    InsurancePercentageWorkshop = _product.InsurancePercentage;
                    //}
                    decimal? basicPremium = _product.PriceDetails.Where(p => p.PriceTypeCode == 7)?.FirstOrDefault()?.PriceValue;

                    decimal? otherTypeCodes = _product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                    || p.PriceTypeCode == 6 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);

                    decimal? vat = _product.PriceDetails.Where(p => p.PriceTypeCode == 8)?.FirstOrDefault()?.PriceValue;
                    //var discounts = product.PriceDetails.Where(p => p.PriceTypeCode == 1 || p.PriceTypeCode == 2
                    //  || p.PriceTypeCode == 3 || p.PriceTypeCode == 10 || p.PriceTypeCode == 11 || p.PriceTypeCode == 12)?.Sum(p => p.PriceValue);

                    List<short?> selectedBenfitsIds = new List<short?>();
                    if (model.SelectedBenfits != null)
                        selectedBenfitsIds = model.SelectedBenfits.FirstOrDefault(a => a.ProductId == _product.ProductID)?.BenfitIds;

                    decimal? benefits = 0;
                    decimal? clientBenefitsAmount = 0;
                    decimal? clientBenefitsAmountWithVAT = 0;
                    if (selectedBenfitsIds != null && model.SelectedBenfits.Any())
                    {
                        benefits = _product.Benfits.Where(b => b.BenefitId.HasValue && selectedBenfitsIds.Contains(b.BenefitId.Value))?.Sum(b => b.BenefitPrice.Value);
                        clientBenefitsAmount = benefits;
                        clientBenefitsAmountWithVAT = benefits * 1.15M;
                        benefits *= 1.15M;
                    }

                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        //var MPAD = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MPAD")).FirstOrDefault();
                        //var MGAE = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MGAE")).FirstOrDefault();
                        //var MRCR = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MRCR")).FirstOrDefault();

                        //short MPAD_Id = 0;
                        //short MGAE_Id = 0;
                        //short MRCR_Id = 0;

                        //if (MPAD != null)
                        //{
                        //    MPAD_Id = MPAD.BenefitId.Value;
                        //}

                        //if (MGAE != null)
                        //{
                        //    MGAE_Id = MGAE.BenefitId.Value;
                        //}

                        //if (MRCR != null)
                        //{
                        //    MRCR_Id = MRCR.BenefitId.Value;
                        //}

                        foreach (var benefit in _product.Benfits)
                        {
                            if (selectedBenfitsIds != null && selectedBenfitsIds.Any() && selectedBenfitsIds.Contains(benefit.BenefitId))
                            {
                                continue;
                            }

                            if (benefit.IsReadOnly && benefit.IsSelected.HasValue && benefit.IsSelected.Value)
                            {
                                benefits += benefit.BenefitPrice.Value;
                            }

                            //if (benefit.BenefitId == 1
                            //    || benefit.BenefitId == 6
                            //    || benefit.BenefitId == 10)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MPAD_Id != 0 && benefit.BenefitId == MPAD_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MGAE_Id != 0 && benefit.BenefitId == MGAE_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MRCR_Id != 0 && benefit.BenefitId == MRCR_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                        }

                    }

                    var otherCodesAndBenifits = otherTypeCodes + benefits;
                   
                    var premium = basicPremium + vat + otherCodesAndBenifits;// - discounts;

                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        premiumReference.Add(
                        new PremiumReference
                        {
                            Premium = premium ?? 0,
                            ReferenceId = _product.ReferenceId,
                            BasicPremium = basicPremium ?? 0,
                            VehicleRepairType = _product.VehicleRepairType,
                            VAT = vat ?? 0,
                            InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / vehicleValue,
                            OtherCodesAndBenifits = otherTypeCodes + clientBenefitsAmount
                        });
                    }
                    else
                    {
                        premiumReference.Add(
                        new PremiumReference
                        {
                            Premium = premium ?? 0,
                            ReferenceId = _product.ReferenceId,
                            BasicPremium = basicPremium ?? 0,
                            VehicleRepairType = _product.VehicleRepairType,
                            VAT = vat ?? 0,
                            InsurancePercentage = _product.InsurancePercentage,
                            OtherCodesAndBenifits = otherCodesAndBenifits
                        });
                    }
                }

                var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType == "Agency").OrderBy(p => p.Premium).FirstOrDefault();
                var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType == "Workshop").OrderBy(p => p.Premium).FirstOrDefault();
                var lowestPremiumAlYusr = premiumReference.Where(p => p.VehicleRepairType == bankRepairMethodSettings.FirstYear).OrderBy(p => p.Premium).FirstOrDefault();

                //Decimal? BasicPremium = lowestPremium.BasicPremium;

                AutoleasingMinimumPremium bankMinimumPremiumSettings = null;
                if (quotationInfo.MinimumPremiumSettingHistory != null)
                {
                    var minimumPremiumSettingHistory = quotationInfo.MinimumPremiumSettingHistory;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSettingHistory.FirstYear,
                        SecondYear = minimumPremiumSettingHistory.SecondYear,
                        ThirdYear = minimumPremiumSettingHistory.ThirdYear,
                        FourthYear = minimumPremiumSettingHistory.FourthYear,
                        FifthYear = minimumPremiumSettingHistory.FifthYear
                    };
                }
                else
                {
                    var minimumPremiumSetting = quotationInfo.MinimumPremiumSetting;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSetting.FirstYear,
                        SecondYear = minimumPremiumSetting.SecondYear,
                        ThirdYear = minimumPremiumSetting.ThirdYear,
                        FourthYear = minimumPremiumSetting.FourthYear,
                        FifthYear = minimumPremiumSetting.FifthYear
                    };
                }

                Decimal? InsurancePercentage1 = 0;
                Decimal? InsurancePercentage2 = 0;
                Decimal? InsurancePercentage3 = 0;
                Decimal? InsurancePercentage4 = 0;
                Decimal? InsurancePercentage5 = 0;

                Decimal? Premium1 = 0;
                Decimal? Premium2 = 0;
                Decimal? Premium3 = 0;
                Decimal? Premium4 = 0;
                Decimal? Premium5 = 0;
                if (contractDuration >= 1)
                {
                    data.RepairMethod1 = bankRepairMethodSettings.FirstYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage1 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium1 = lowestPremiumAlYusr.Premium;
                        if (Premium1 < bankMinimumPremiumSettings.FirstYear)
                        {
                            Premium1 = bankMinimumPremiumSettings.FirstYear;
                        }
                    }
                    else if (bankRepairMethodSettings.FirstYear == "Agency")
                    {
                        InsurancePercentage1 = lowestPremiumAgency.InsurancePercentage;
                        Premium1 = lowestPremiumAgency.Premium;
                    }
                    else
                    {
                        InsurancePercentage1 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium1 = lowestPremiumWorkshop.Premium;
                    }
                    data.InsurancePercentage1 = Math.Round(InsurancePercentage1.Value, 2).ToString();
                    data.Premium1 = Math.Round(Premium1.Value, 2).ToString();
                    data.Deductible1 = data.Deductible;
                    data.MiuimumPremium1 = bankMinimumPremiumSettings.FirstYear.ToString().Replace(".00", "");
                }
                if (contractDuration >= 2)
                {
                    data.RepairMethod2 = bankRepairMethodSettings.SecondYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage2 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium2 = (InsurancePercentage2 * DepreciationValue2) / 100;
                        if (Premium2 < bankMinimumPremiumSettings.SecondYear)
                        {
                            Premium2 = bankMinimumPremiumSettings.SecondYear;
                        }
                    }
                    else if (bankRepairMethodSettings.SecondYear == "Agency")
                    {
                        InsurancePercentage2 = lowestPremiumAgency.InsurancePercentage;
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage2 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                    data.InsurancePercentage2 = Math.Round(InsurancePercentage2.Value, 2).ToString();
                    data.Premium2 = Math.Round(Premium2.Value, 2).ToString();
                    data.Deductible2 = data.Deductible;
                    data.MiuimumPremium2 = bankMinimumPremiumSettings.SecondYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 3)
                {
                    data.RepairMethod3 = bankRepairMethodSettings.ThirdYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage3 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium3 = (InsurancePercentage3 * DepreciationValue3) / 100;
                        if (Premium3 < bankMinimumPremiumSettings.ThirdYear)
                        {
                            Premium3 = bankMinimumPremiumSettings.ThirdYear;
                        }
                    }
                    else if (bankRepairMethodSettings.ThirdYear == "Agency")
                    {
                        InsurancePercentage3 = lowestPremiumAgency.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                    data.InsurancePercentage3 = Math.Round(InsurancePercentage3.Value, 2).ToString();
                    data.Premium3 = Math.Round(Premium3.Value, 2).ToString();
                    data.Deductible3 = data.Deductible;
                    data.MiuimumPremium3 = bankMinimumPremiumSettings.ThirdYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 4)
                {
                    data.RepairMethod4 = bankRepairMethodSettings.FourthYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage4 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium4 = (InsurancePercentage4 * DepreciationValue4) / 100;
                        if (Premium4 < bankMinimumPremiumSettings.FourthYear)
                        {
                            Premium4 = bankMinimumPremiumSettings.FourthYear;
                        }
                    }
                    else if (bankRepairMethodSettings.FourthYear == "Agency")
                    {
                        InsurancePercentage4 = lowestPremiumAgency.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                    data.InsurancePercentage4 = Math.Round(InsurancePercentage4.Value, 2).ToString();
                    data.Premium4 = Math.Round(Premium4.Value, 2).ToString();
                    data.Deductible4 = data.Deductible;
                    data.MiuimumPremium4 = bankMinimumPremiumSettings.FourthYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 5)
                {
                    data.RepairMethod5 = bankRepairMethodSettings.FifthYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage5 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium5 = (InsurancePercentage5 * DepreciationValue5) / 100;
                        if (Premium5 < bankMinimumPremiumSettings.FifthYear)
                        {
                            Premium5 = bankMinimumPremiumSettings.FifthYear;
                        }
                    }
                    else if (bankRepairMethodSettings.FifthYear == "Agency")
                    {
                        InsurancePercentage5 = lowestPremiumAgency.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                    }
                    else
                    {
                        InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                    }
                    data.InsurancePercentage5 = Math.Round(InsurancePercentage5.Value, 2).ToString();
                    data.Premium5 = Math.Round(Premium5.Value, 2).ToString();
                    data.Deductible5 = data.Deductible;
                    data.MiuimumPremium5 = bankMinimumPremiumSettings.FifthYear.ToString().Replace(".00", "");

                }
                data.Total5YearsPremium = Math.Round((Premium1 ?? 0) + (Premium2 ?? 0) + (Premium3 ?? 0) + (Premium4 ?? 0) + (Premium5 ?? 0), 2).ToString();
                #endregion

                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                //log.RetrievingMethod = "Generation";
                //log.ServiceURL = serviceURL;

                predefinedLog.ServerIP = Utilities.GetInternalServerIP();
                //log.CompanyID = quoteResponse.InsuranceCompanyId;
                if (string.IsNullOrEmpty(predefinedLog.Channel))
                    predefinedLog.Channel = Channel.autoleasing.ToString();

                string policyDetailsJsonString = JsonConvert.SerializeObject(data);
                AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
                {
                    ReportType = "IndividualQuotationsFormTemplate",
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                //log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                //log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                /* Save hits of download quotation form in db*/
                var autoleasingUser = _autoleasingUserService.GetUser(predefinedLog.UserId);
                var bank = _bankService.GetBank(autoleasingUser.BankId);
                if (autoleasingUser != null && bank != null)
                {
                    AutoleasingQuotationForm form = new AutoleasingQuotationForm();
                    form.ExternalId = model.qtRqstExtrnlId;
                    form.CreatedDate = DateTime.Now;
                    form.BankId = autoleasingUser.BankId;
                    form.BankName = bank.NameEn;
                    form.UserId = predefinedLog.UserId;
                    form.UserEmail = autoleasingUser.Email;
                    form.FilePath = Utilities.SaveQuotationFormFile(model.qtRqstExtrnlId, JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString), bank.NameEn);
                    _autoleasingQuotationFormService.Insert(form);
                }
                /* end  */

                /* Save selected company which download quotation form in db*/

                if (model.SelectedCompany > 0)
                {
                    var oldCompanies = _autoleasingInitialQuotationCompaniesRepository.Table.Where(a => a.ExternalId == model.qtRqstExtrnlId).ToList();
                    if (oldCompanies != null && oldCompanies.Count > 0)
                        _autoleasingInitialQuotationCompaniesRepository.Delete(oldCompanies);

                    AutoleasingInitialQuotationCompanies companyQuote = new AutoleasingInitialQuotationCompanies();
                    companyQuote.ExternalId = model.qtRqstExtrnlId;
                    companyQuote.CompanyId = model.SelectedCompany;
                    companyQuote.BankId = autoleasingUser.BankId;
                    companyQuote.UserId = predefinedLog.UserId;
                    companyQuote.CreatedDate = DateTime.Now;
                    _autoleasingInitialQuotationCompaniesRepository.Insert(companyQuote);
                }

                /* end  */

                output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
                //predefinedLog.ServiceResponse = "Success";
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = output.ErrorDescription;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);

                return output;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                //predefinedLog.ServiceResponse = exception;
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = "Exception Error";
                QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                return null;
            }
        }

        public List<IntialQuotationOutput> GetIntialQuotations(string nin,out string exception)
        {
            //totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetIntialQuotation";
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrWhiteSpace(nin))
                {
                    SqlParameter ninParameter = new SqlParameter() { ParameterName = "nin", Value = nin.Trim() };
                    command.Parameters.Add(ninParameter);
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<IntialQuotationOutput> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<IntialQuotationOutput>(reader).ToList();

                return filteredData;
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

        public Output<RepairMethodModel> GetRepairMethodFromBankSettings(int bankId, string externalId, int vehicleValue)
        {
            var bankRepairMethodSettings = _autoleasingRepairMethodHistoryRepository.TableNoTracking.FirstOrDefault(q => q.ExternalId == externalId);

            var output = new Output<RepairMethodModel>();
            if (bankRepairMethodSettings == null)
            {
                output.ErrorCode = Output<RepairMethodModel>.ErrorCodes.NullResult;
                output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                return output;
            }

            var model = new RepairMethodModel();
            model.RepairMethod = bankRepairMethodSettings.FirstYear;
            if (bankId == 1 && vehicleValue >= 200000) //Tajeer
            {
                model.IsReadOnly = true;
            }

            output.ErrorCode = Output<RepairMethodModel>.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.Result = model;

            return output;
        }

        public Output<bool> UpdateQuotationRepairMethodSettingsHistory(string externalId, string currentUserId, bool isAgency)
        {
            var output = new Output<bool>();
            QuotationRequestLog log = new QuotationRequestLog();
            log.Channel = Channel.autoleasing.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserId = currentUserId;
            log.ExtrnlId = externalId;

            //var quotationRequest = _quotationRequestRepository.TableNoTracking.FirstOrDefault(q=>q.ExternalId == externalId);
            //if (quotationRequest == null)
            //{
            //    output.ErrorCode = Output<bool>.ErrorCodes.NullResult;
            //    output.ErrorDescription = WebResources.ErrorGeneric;
            //    log.ErrorCode = (int)output.ErrorCode;
            //    log.ErrorDescription = $"UpdateQuotationRepareMethodSettingsHistory, The external id not found : {externalId}";
            //    return output;
            //}
            //if (quotationRequest.UserId != currentUserId)
            //{

            //}
            var quotationRepairMethodHistory = _autoleasingRepairMethodHistoryRepository.Table.FirstOrDefault(q => q.ExternalId == externalId);
            if (quotationRepairMethodHistory == null)
            {
                output.ErrorCode = Output<bool>.ErrorCodes.NullResult;
                output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodHistorySettingsNotFoundForThisExternalId;
                return output;
            }

            string repareMethod = isAgency ? "Agency" : "Workshop";
            quotationRepairMethodHistory.FirstYear = repareMethod;
            quotationRepairMethodHistory.SecondYear = repareMethod;
            quotationRepairMethodHistory.ThirdYear = repareMethod;
            quotationRepairMethodHistory.FourthYear = repareMethod;
            quotationRepairMethodHistory.FifthYear = repareMethod;

            _autoleasingRepairMethodHistoryRepository.Update(quotationRepairMethodHistory);
            output.ErrorCode = Output<bool>.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.Result = true;

            return output;
        }

        private List<QuotationCompanyOutput> GetQuotaionResponseCompanies(string ExternalId)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationInsuranceCompany";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter ExternalIdParameter = new SqlParameter() { ParameterName = "externalId", Value = ExternalId };
                command.Parameters.Add(ExternalIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<QuotationCompanyOutput> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationCompanyOutput>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }

        }

        public List<QuotationResponseModel> GetAllQuotationsByExternalId(string qtRqstExtrnlId, Channel channel, Guid userId, string userName, int insuranceTypeCode, bool vehicleAgencyRepair, int? deductibleValue)
        {
            List<QuotationResponseModel> output = new List<QuotationResponseModel>();
            QuotationRequestLog log = new QuotationRequestLog();
            QuotationResponseModel responseModel = null;
            List<QuotationCompanyOutput> quotationCompanies = new List<QuotationCompanyOutput>();
            quotationCompanies = GetQuotaionResponseCompanies(qtRqstExtrnlId);
            foreach (var company in quotationCompanies)
            {
                var quotationResponseCache = _quotationService.GetFromAutoleasingQuotationResponseCache(company.CompanyId, qtRqstExtrnlId, vehicleAgencyRepair, deductibleValue, userId);
                if (quotationResponseCache != null)
                {
                    responseModel = JsonConvert.DeserializeObject<QuotationResponseModel>(quotationResponseCache.QuotationResponse);
                }
                //else
                //{
                //    var quotationOutput = GetQuote(company.CompanyId, qtRqstExtrnlId, channel, userId, userName,Guid.NewGuid(), insuranceTypeCode,vehicleAgencyRepair,deductibleValue);
                //    if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
                //    {
                //        continue;
                //    }
                //    if (quotationOutput.QuotationResponse.Products == null)
                //    {
                //        continue;
                //    }
                //    responseModel = quotationOutput.QuotationResponse.ToModel();
                //    foreach (var product in responseModel.Products)
                //    {
                //        if (product.Product_Benefits == null)
                //            continue;
                //        foreach (var benfit in product.Product_Benefits)
                //        {
                //            if (benfit.BenefitId != 0)
                //                continue;
                //            var serviceProduct = quotationOutput.Products.Where(a => a.ProductId == product.ExternalProductId).FirstOrDefault();
                //            if (serviceProduct == null)
                //                continue;
                //            var serviceBenfitInfo = serviceProduct.Benefits.Where(a => a.BenefitId == benfit.BenefitExternalId).FirstOrDefault();
                //            if (serviceBenfitInfo == null)
                //                continue;
                //            benfit.Benefit.ArabicDescription = serviceBenfitInfo.BenefitNameAr;
                //            benfit.Benefit.EnglishDescription = serviceBenfitInfo.BenefitNameEn;
                //        }
                //    }
                //    if (company.CompanyId== 8 && insuranceTypeCode == 2)
                //    {
                //        foreach (var product in responseModel.Products)
                //        {
                //            if (product.DeductableValue == 0)
                //                product.DeductableValue = 2000;
                //        }
                //    }
                //    foreach (var product in responseModel.Products)
                //    {
                //        if (product.PriceDetails == null)
                //            continue;
                //        foreach (var price in product.PriceDetails)
                //        {
                //            if (price.PriceTypeCode == 1 || price.PriceTypeCode == 12)
                //            {
                //                price.PriceType.ArabicDescription = "خصم اليوم الوطني";
                //                price.PriceType.EnglishDescription = "National Day Discount";
                //            }
                //        }
                //    }
                //    if (insuranceTypeCode == 2)
                //    {
                //        responseModel.Products = responseModel.Products.OrderByDescending(x => x.DeductableValue).ToList();
                //    }
                //    QuotationResponseCache cache = new QuotationResponseCache();
                //    cache.InsuranceCompanyId = company.CompanyId;
                //    cache.ExternalId = qtRqstExtrnlId;
                //    cache.InsuranceTypeCode = insuranceTypeCode;
                //    cache.VehicleAgencyRepair = vehicleAgencyRepair;
                //    cache.DeductibleValue = deductibleValue;
                //    cache.UserId = userId;
                //    string jsonResponse = JsonConvert.SerializeObject(responseModel);
                //    cache.QuotationResponse = jsonResponse;
                //    string exception = string.Empty;
                //    _quotationService.InsertIntoQuotationResponseCache(cache, out exception);
                //}
                output.Add(responseModel);
            }
            return output;
            //return output.OrderBy(x => x.Products.Select(v => v.ProductPrice)).ToList();
        }

        public QuotationOutput GetQuoteAutoleasing(int insuranceCompanyId, string qtRqstExtrnlId, Channel channel, Guid userId, string userName, int bankId, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null)
        {
            QuotationOutput output = new QuotationOutput();
            QuotationRequestLog log = new QuotationRequestLog();
            log.Channel = channel.ToString();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.Channel = channel.ToString();
            log.UserId = userId.ToString();
            log.UserName = userName;
            log.ExtrnlId = qtRqstExtrnlId;
            log.InsuranceTypeCode = insuranceTypeCode;
            log.CompanyId = insuranceCompanyId;
            output.QuotationResponse = new QuotationResponse();
            try
            {
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if (insuranceCompanyId == 0)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company Id is required";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyId);
                log.CompanyName = insuranceCompany.Key;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = SubmitInquiryResource.insuranceCompanyId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "insurance Company is null";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }


                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompanyId;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;
                predefinedLogInfo.Channel = channel.ToString();
                predefinedLogInfo.ExternalId = qtRqstExtrnlId;

                output = GetAutoleasingQuotation(insuranceCompany, qtRqstExtrnlId, predefinedLogInfo, log, bankId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue);

                log.RefrenceId = output?.QuotationResponse?.ReferenceId;

                if (output.ErrorCode != QuotationOutput.ErrorCodes.Success )
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "failed to Get Quotation due to " + output.LogDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;

                }

                var bankRepairMethodHistorySettings = _autoleasingRepairMethodHistoryRepository.TableNoTracking.Where(r => r.ExternalId == qtRqstExtrnlId).FirstOrDefault();
                //Validate Bank Settings - to be in separate method
                if (bankRepairMethodHistorySettings == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                    output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + bankId;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                    return output;
                }
                if ((vehicleAgencyRepair && bankRepairMethodHistorySettings.FirstYear == "Workshop")
                    || (!vehicleAgencyRepair && bankRepairMethodHistorySettings.FirstYear == "Agency"))
                {
                    output.QuotationResponse.Products = null;
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Not Support product";
                    return output;
                }
                if (insuranceCompanyId == 5)
                {
                    output.QuotationResponse.Products = null;
                }

                //if (output.QuotationResponse.BankId == 2 && output.QuotationResponse.Products != null)                //{                //    foreach (var item in output.QuotationResponse.Products)                //    {                //        item.ProductPrice += item.Product_Benefits.Where(a => a.IsReadOnly == true && a.IsSelected == true).Sum(a => a.BenefitPrice).Value;                //    }                //}
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;

            }
            catch (Exception exp)
            {

                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString(); // SubmitInquiryResource.InvalidData;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
                return output;
            }

        }

        public QuotationOutput GetAutoleasingQuotation(InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int bankId, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();

            QuotationOutput output = new QuotationOutput();

            DateTime startDateTime = DateTime.Now;

            var quoteRequest = _quotationRequestRepository.TableNoTracking
                .Include(request => request.Vehicle)
                .Include(request => request.Driver)
                .Include(request => request.Insured)
                .Include(request => request.Insured.Occupation)
                .Include(request => request.Drivers.Select(d => d.DriverViolations))
                .Include(request => request.Driver.Occupation)
                .Include(e => e.Insured.IdIssueCity)
                .Include(e => e.Insured.City)
                .FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);
            if (quoteRequest == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                log.ErrorCode = (int)output.ErrorCode;
                output.LogDescription = "quoteRequest is null";
                return output;
            }
            var quoteRequestUserInfo = _autoleasingUser.TableNoTracking.FirstOrDefault(u => u.UserId == quoteRequest.UserId);
            var currentUserInfo = _autoleasingUser.TableNoTracking.FirstOrDefault(u => u.UserId == userId);

            if (quoteRequestUserInfo == null || currentUserInfo == null || quoteRequestUserInfo.BankId != currentUserInfo.BankId)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoDataWithThisUserIdAndExternalId;
                output.ErrorDescription = WebResources.NoDataWithThisUserIdAndExternalId;
                log.ErrorCode = (int)output.ErrorCode;
                output.LogDescription = $"No Data With This User Id And External Id, quoteRequest User Id: {quoteRequest.UserId}, Current User Id: {userId}, External Id: {qtRqstExtrnlId}";
                return output;
            }

            string referenceId = string.Empty;
            if (insuranceCompany.InsuranceCompanyID == 12)
            {
                referenceId = getNewReferenceId(13);
            }
            else
            {
                referenceId = getNewReferenceId();
            }
            log.RefrenceId = referenceId;

            if ((quoteRequest.AutoleasingInitialOption.HasValue && quoteRequest.AutoleasingInitialOption.Value)
                && (!quoteRequest.IsConverted.HasValue || quoteRequest.IsConverted.Value == false))
            {
                output.IsInitialQuotation = quoteRequest.AutoleasingInitialOption;
            }
            log.VehicleId = string.IsNullOrEmpty(quoteRequest.Vehicle.SequenceNumber) ? quoteRequest.Vehicle.CustomCardNumber : quoteRequest.Vehicle.SequenceNumber;
            log.NIN = quoteRequest.Driver.NIN;

            if (quoteRequest.Driver != null)
                predefinedLogInfo.DriverNin = quoteRequest.Driver.NIN;

            if (quoteRequest.Vehicle != null)
            {
                if (quoteRequest.Vehicle.VehicleIdType == VehicleIdType.CustomCard)
                    predefinedLogInfo.VehicleId = quoteRequest.Vehicle.CustomCardNumber;
                else
                    predefinedLogInfo.VehicleId = quoteRequest.Vehicle.SequenceNumber;

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

            output.VehicleValue = quoteRequest.Vehicle.VehicleValue ?? 0;
            QuotationResponse quotationResponseForInitial = null;
            string quotationNo = string.Empty;
            if (quoteRequest.AutoleasingInitialOption.HasValue && quoteRequest.AutoleasingInitialOption.Value)
            {
                quotationResponseForInitial = _quotationService.GetQuotationResponseByExternalAndCompanyId(quoteRequest.InitialExternalId, insuranceCompany.InsuranceCompanyID, vehicleAgencyRepair, deductibleValue.Value);
                if (quotationResponseForInitial != null && !string.IsNullOrEmpty(quotationResponseForInitial.ICQuoteReferenceNo))
                {
                    quotationNo = quotationResponseForInitial.ICQuoteReferenceNo;
                    if (insuranceCompany.InsuranceCompanyID == 12)
                    {
                        referenceId = quotationNo;
                    }
                }
            }
            log.RefrenceId = referenceId;
            output.QuotationResponse = new QuotationResponse()
            {
                ReferenceId = referenceId,
                RequestId = quoteRequest.ID,
                InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                VehicleAgencyRepair = vehicleAgencyRepair,
                DeductibleValue = deductibleValue,
                CreateDateTime = startDateTime,
                InsuranceCompanyId = insuranceCompany.InsuranceCompanyID,
                VehicleValue = quoteRequest.Vehicle.VehicleValue ?? 0,
                BankId = bankId
            };

            if (quoteRequest.AutoleasingInitialOption.HasValue && quoteRequest.AutoleasingInitialOption.Value)
            {
                // if (quoteRequest.IsConverted.HasValue && quoteRequest.IsConverted.Value)
                if (string.IsNullOrEmpty(predefinedLogInfo.VehicleId) || predefinedLogInfo.VehicleId == "0")
                    output.QuotationResponse.AutoleasingInitialOption = true;
                else
                    output.QuotationResponse.AutoleasingInitialOption = false;
            }
            else
                output.QuotationResponse.AutoleasingInitialOption = false;

            output.QuotationResponse.ArabicDriverName = quoteRequest.Driver.FirstName + " " + quoteRequest.Driver.SecondName
               + " " + quoteRequest.Driver.ThirdName + " " + quoteRequest.Driver.LastName;

            output.QuotationResponse.EnglishDriverName = quoteRequest.Driver.EnglishFirstName + " " + quoteRequest.Driver.EnglishSecondName
              + " " + quoteRequest.Driver.EnglishThirdName + " " + quoteRequest.Driver.EnglishLastName;

            string promotionProgramCode = string.Empty;
            int promotionProgramId = 0;
            var requestMessage = GetAutoleasingQuotationRequest(quoteRequest, output.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue, out promotionProgramCode, out promotionProgramId);
            string errors = "";

            requestMessage.NCDFreeYears = quoteRequest.NajmNcdFreeYears.HasValue ? quoteRequest.NajmNcdFreeYears.Value : 0;
            requestMessage.NCDReference = quoteRequest.NajmNcdRefrence;

            if (quoteRequest.AutoleasingInitialOption.HasValue && quoteRequest.AutoleasingInitialOption.Value) // Option No
            {
                requestMessage.VehicleRegExpiryDate = null;
                requestMessage.VehicleUseCode = 1;
                requestMessage.VehicleRegPlaceCode = null;
                requestMessage.NCDFreeYears = 0;
                requestMessage.NCDReference = "0";
                requestMessage.VehiclePlateTypeCode = "1";
                foreach (var item in requestMessage.Drivers)
                {
                    item.DriverNOALast5Years = 0;
                    item.DriverNCDFreeYears = 0;
                    item.DriverNCDReference = "0";
                }
            }
            else
            {
                if (requestMessage.VehicleIdTypeCode == (int)VehicleIdType.CustomCard)
                {
                    requestMessage.VehicleRegExpiryDate = null;
                }

                foreach (var item in requestMessage.Drivers)
                {
                    item.DriverNOALast5Years = 0;
                }
            }
            if(!string.IsNullOrEmpty(quotationNo))
            {
                requestMessage.QuotationNo = quotationNo;
            }

            requestMessage.BankId = _autoleasingUser.TableNoTracking.FirstOrDefault(b => b.Id == quoteRequest.UserId).BankId;
            var response = RequestQuotationProducts(requestMessage, output.QuotationResponse, insuranceCompany, predefinedLogInfo, automatedTest, out errors);

            if (response == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response is null due to errors, " + errors;
                return output;
            }
            if (response.Products == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products is null due to errors, " + errors;
                return output;
            }
            if (response.Products.Count() == 0)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products.Count() is null due to errors, " + errors;
                return output;
            }

            if (response != null && response.Products != null)
            {
                output.Products = response.Products;
                var products = new List<Product>();
                var allBenefitst = _benefitRepository.Table.ToList();
                var allPriceTypes = _priceTypeRepository.Table.ToList();
                foreach (var p in response.Products)
                {
                    var product = p.ToEntity();
                    if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode))
                        product.IsPromoted = true;
                    product.ProviderId = insuranceCompany.InsuranceCompanyID;
                    if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                        product.InsuranceTypeCode = insuranceTypeCode;

                    if (product.Product_Benefits != null)
                    {
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
                            if (pb.BenefitId == 7 && vehicleAgencyRepair == true)
                            {
                                pb.IsSelected = true;
                            }

                            // as per jira (https://bcare.atlassian.net/browse/ALP-110)
                            if ((bankId == 2 &&
                                        ((pb.BenefitId == 1 || pb.BenefitId == 10)
                                        ||
                                        (bankId == 2 && (pb.BenefitExternalId.Contains("MPAD") || pb.BenefitExternalId.Contains("MGAE") || pb.BenefitExternalId.Contains("MRCR")))))) // this for Wataniya benefits code
                            {
                                pb.IsSelected = true;
                                pb.IsReadOnly = true;
                                //HandleAutoleaseSpecialBenefits(product, pb);
                            }
                        }
                    }
                    product.CreateDateTime = DateTime.Now;
                    product.ReferenceId = output.QuotationResponse.ReferenceId;
                    product.ShadowAmount = 0;
                    decimal basicPremiumPrice = 0;
                    decimal additionalLoading = 0;
                    decimal additionalAgeContribution = 0;
                    // Load price details from database.
                    foreach (var pd in product.PriceDetails)
                    {
                        pd.IsCheckedOut = false;
                        pd.CreateDateTime = DateTime.Now;
                        pd.PriceType = allPriceTypes.FirstOrDefault(pt => pt.Code == pd.PriceTypeCode);

                        // Shadow Amount
                        if (pd.PriceTypeCode == 1 || pd.PriceTypeCode == 2 || pd.PriceTypeCode == 3 ||
                            pd.PriceTypeCode == 10 || pd.PriceTypeCode == 11 || pd.PriceTypeCode == 12) // Discounts
                        {
                            product.ShadowAmount += pd.PriceValue;
                        }
                        else if (pd.PriceTypeCode == 7) // Basic
                        {
                            basicPremiumPrice = pd.PriceValue;
                        }
                        else if (pd.PriceTypeCode == 4) // Additional Loading (Due to accidents)
                        {
                            additionalLoading = pd.PriceValue;
                        }
                        else if (pd.PriceTypeCode == 5) // Additional Age Contribution
                        {
                            additionalAgeContribution = pd.PriceValue;
                        }
                    }

                    //if (bankId == 2) //alyusr
                    //{
                    //    //var benifits = product.Product_Benefits.Where(b => b.IsSelected.HasValue && b.IsSelected.Value && b.IsReadOnly);
                    //    product.InsurancePercentage = Decimal.Round((product.ProductPrice / output.VehicleValue) * 100, 2);
                    //}
                    //else
                    //{
                    //    product.InsurancePercentage = Decimal.Round(((basicPremiumPrice + additionalLoading + additionalAgeContribution) / output.VehicleValue) * 100, 2);
                    //}
                    product.ProductPrice = Decimal.Round((p.ProductPrice), 2);
                    product.InsurancePercentage = Decimal.Round((product.ProductPrice / output.VehicleValue) * 100, 2);
                    product.QuotaionNo = response.QuotationNo;
                    products.Add(product);
                }
                output.QuotationResponse.Products = products;
                if (!string.IsNullOrEmpty(promotionProgramCode) && promotionProgramId != 0)
                {
                    output.QuotationResponse.PromotionProgramCode = promotionProgramCode;
                    output.QuotationResponse.PromotionProgramId = promotionProgramId;
                }
                if (quoteRequest.Insured.City != null)
                    output.QuotationResponse.CityId = quoteRequest.Insured.City.YakeenCode;
                output.QuotationResponse.ICQuoteReferenceNo = response.QuotationNo;
                _quotationResponseRepository.Insert(output.QuotationResponse);
            }
            //if (quoteRequest.AutoleasingBulkOption == true)
            //{
            //    output.QuotationResponse.Products = AutoleasingBulkHandleProductOrBenefitWithZeroPrice(output.QuotationResponse.Products, qtRqstExtrnlId).ToList();
            //}
            //else
            //{
            //    output.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(output.QuotationResponse.Products).ToList();
            //}

            output.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(output.QuotationResponse.Products).ToList();

            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";

            return output;
        }

        private IEnumerable<Product> AutoleasingBulkHandleProductOrBenefitWithZeroPrice(IEnumerable<Product> products, string externalId)
        {
            var quotationAutoLeasingSelectedBenfits = _autoLeasingSelectedBenfitsRepository.TableNoTracking.Where(a => a.ExternalId == externalId).Select(a => a.BenifitId).ToList();

            foreach (var product in products)
            {
                var productBenefits = new List<Product_Benefit>();
                if (product.Product_Benefits == null || !product.Product_Benefits.Any())
                    continue;

                foreach (var benefit in product.Product_Benefits)
                {
                    if (benefit.IsReadOnly && benefit.IsSelected.HasValue && benefit.IsSelected.Value)
                    {
                        productBenefits.Add(benefit);
                    }
                    else if (benefit.BenefitId.HasValue && quotationAutoLeasingSelectedBenfits.Contains(benefit.BenefitId.Value) && benefit.BenefitPrice > 0)
                    {
                        //benefit.IsReadOnly = true;
                        benefit.IsSelected = true;
                        productBenefits.Add(benefit);

                        product.ProductPrice += benefit.BenefitPrice.Value * 1.15M;
                    }
                }
                product.Product_Benefits = productBenefits;
            }

            return products.Where(x => x.ProductPrice > 0);
        }

        public QuotationOutput GetWataniyaQuotation(int quotationRequestId, string referenceId, Guid productInternalId, string qtRqstExtrnlId, InsuranceCompany insuranceCompany, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)        {            QuotationOutput output = new QuotationOutput();            output.QuotationResponse = new QuotationResponse();            try            {                if (string.IsNullOrEmpty(qtRqstExtrnlId))                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "qtRqstExtrnlId is null ";                    return output;                }                var quotationRequest = _quotationRequestRepository.Table               .Include(e => e.Insured)               .Include(e => e.Vehicle)               .Include(e => e.QuotationResponses)               .Include(e => e.Driver)               .Include(e => e.Driver.DriverLicenses)               .Include(e => e.Insured.Occupation)               .Include(e => e.Drivers.Select(d => d.DriverViolations))               .Include(e => e.Driver.Occupation)               .Include(e => e.Insured.IdIssueCity)               .Include(e => e.Insured.City)               .FirstOrDefault(e => e.ID == quotationRequestId);

                if (quotationRequest == null)                {                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;                    output.ErrorDescription = "There is no quotation request with this id " + quotationRequestId;                    return output;                }                var quotationResponse = quotationRequest.QuotationResponses.FirstOrDefault(e => e.ReferenceId == referenceId);

                //if (productInternalId == null || productInternalId == Guid.Empty)
                //{
                //    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                //    output.ErrorDescription = "productInternalId is null ";
                //    return output;
                //}
                bool quotResponseVehicleAgencyRepair = true;   
                if (channel == Channel.autoleasing.ToString().ToLower())
                {
                    AutoleasingAgencyRepair bankRepairMethodSettings = null;
                    var bankRepairMethodSettingsHistory = _autoleasingRepairMethodHistoryRepository.TableNoTracking.Where(c => c.ExternalId == qtRqstExtrnlId).FirstOrDefault();
                    if (bankRepairMethodSettingsHistory != null)
                    {
                        bankRepairMethodSettings = new AutoleasingAgencyRepair()
                        {
                            BankId = bankRepairMethodSettingsHistory.BankId,
                            FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                            SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                            ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                            FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                            FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                        };
                    }
                    int bankId =(int) _autoleasingUser.TableNoTracking.Where(x => x.UserId == quotationRequest.UserId).FirstOrDefault()?.BankId;
                    if (bankRepairMethodSettings == null)
                        bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(c => c.BankId == bankId).FirstOrDefault();
                    if (bankRepairMethodSettings == null)
                    {
                        output.ErrorCode = QuotationOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                        output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                        return output;
                    }
                    string vechileId = !string.IsNullOrEmpty(quotationRequest.Vehicle.SequenceNumber) ? quotationRequest.Vehicle.SequenceNumber : quotationRequest.Vehicle.CustomCardNumber;
                    int policiescount = GetVehiclePoliciesCount(vechileId, quotationRequest.Insured.NationalId, bankId);
                    switch (policiescount)
                    {
                        case 0:
                            quotResponseVehicleAgencyRepair = bankRepairMethodSettings.FirstYear == "Agency" ? true : false;
                            break;
                        case 1:
                            quotResponseVehicleAgencyRepair = bankRepairMethodSettings.SecondYear == "Agency" ? true : false;
                            break;
                        case 2:
                            quotResponseVehicleAgencyRepair = bankRepairMethodSettings.ThirdYear == "Agency" ? true : false;
                            break;
                        case 3:
                            quotResponseVehicleAgencyRepair = bankRepairMethodSettings.FourthYear == "Agency" ? true : false;
                            break;
                        case 4:
                            quotResponseVehicleAgencyRepair = bankRepairMethodSettings.FifthYear == "Agency" ? true : false;
                            break;
                    }
                }
                string promotionProgramCode = string.Empty;                int promotionProgramId = 0;                var requestMessage = GetAutoleasingQuotationRequest(quotationRequest,                    quotationResponse, quotationResponse.InsuranceTypeCode.Value, quotResponseVehicleAgencyRepair, userId.ToString(), quotationResponse.DeductibleValue, out promotionProgramCode, out promotionProgramId);                requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;

                output.QuotationServiceRequest = requestMessage;                output.ErrorCode = QuotationOutput.ErrorCodes.Success;                return output;            }            catch (Exception exp)            {                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;                output.ErrorDescription = exp.ToString(); // SubmitInquiryResource.InvalidData;
                return output;            }        }

        private int GetVehiclePoliciesCount(string vehicelId, string nin, int bankId)        {            int data = 0;            var dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetAutoleasingVehiclePoliciesCount";                command.CommandType = CommandType.StoredProcedure;                dbContext.DatabaseInstance.CommandTimeout = 120;                SqlParameter ninParameter = new SqlParameter() { ParameterName = "nin", Value = nin };                command.Parameters.Add(ninParameter);                SqlParameter vehicelIdParameter = new SqlParameter() { ParameterName = "vehicleId", Value = vehicelId };                command.Parameters.Add(vehicelIdParameter);                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };                command.Parameters.Add(bankIdParameter);                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();                dbContext.DatabaseInstance.Connection.Close();                return data;            }            catch (Exception exp)            {                File.WriteAllText(@"C:\inetpub\WataniyaLog\GetAutoleasingVehiclePoliciesCount" + vehicelId + "_error.txt", JsonConvert.SerializeObject(exp.ToString()));                dbContext.DatabaseInstance.Connection.Close();                return data;            }        }

        private void HandleDriveAddressDetailsForWataniya(DriverDto model)
        {
            if (model.DriverId > 0)
            {
                var address = _addressService.GetAddressesByNin(model.DriverId.ToString());
                if (address != null)
                    model.DriverHomeAddress = address.BuildingNumber + " " + address.AdditionalNumber + " " + address.PostCode + " " + address.City;
            }
        }

        private void HandleAutoleaseSpecialBenefits(Product product, Product_Benefit pb)
        {
            pb.IsSelected = true;
            pb.IsReadOnly = true;

            if (pb.BenefitPrice.HasValue && pb.BenefitPrice > 0)
                product.ProductPrice += pb.BenefitPrice.Value * 1.15M;

            var vat = product.PriceDetails.Where(a => a.PriceTypeCode == 8).FirstOrDefault();
            if (vat != null)
                vat.PriceValue += pb.BenefitPrice.Value * .15M;
        }

        public List<AutoleasingQuotationReportInfoModel> GetAutoleasingQuotaionReport(AutoleasingQuotationReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            try
            {
                var mainData = GetAutoleasingQuotaionReportMainData(filter, bankId, pageIndex, pageSize, out totalCount, out exception);
                if (!string.IsNullOrEmpty(exception) || mainData == null || !mainData.Any())
                {
                    return null;
                }
                
                if (string.IsNullOrEmpty(exception) && mainData != null && mainData.Any())
                {
                    var productIdsFromDB = mainData.Select(x => x.ProductId).ToList();
                    //var allPriceDetatils = _priceDetailRepository.TableNoTracking.Where(x => productIdsFromDB.Contains(x.ProductID)).ToList();
                    var allPriceDetatils = GetPriceDetailsByProductIdsList(productIdsFromDB, out exception);

                    foreach (var item in mainData)
                    {
                        var productPriceDetatils = allPriceDetatils.Where(x => x.ProductID == item.ProductId).ToList();
                        if (productPriceDetatils != null && productPriceDetatils.Any())
                        {
                            item.BasicPrimium = productPriceDetatils.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue;
                            item.VAT = productPriceDetatils.Where(x => x.PriceTypeCode == 8).FirstOrDefault()?.PriceValue;
                            item.BasicPrimiumWithVAT = item.BasicPrimium + item.VAT;

                            var ncdPriceDetails = productPriceDetatils.Where(x => x.PriceTypeCode == 2).FirstOrDefault();
                            item.NCDPercentage = ncdPriceDetails?.PercentageValue;
                            item.NoClaimsDiscountNCD = ncdPriceDetails?.PriceValue;
                        }
                    }
                }

                return mainData;
                
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                totalCount = 0;
                return null;
            }
        }
   


        public List<AutoleasingQuotationReportInfoModel> GetAutoleasingQuotaionReportMainData(AutoleasingQuotationReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingQuotaionReport";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (!string.IsNullOrEmpty(filter.QuotationNumber))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@quotationNumber", Value = filter.QuotationNumber });
                }

                if (filter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyId", Value = filter.InsuranceCompanyId.Value });
                }

                if (filter.Status.HasValue)
                {
                    bool status = (filter.Status.Value == 2) ? true : false; //true --> pruchased
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@status", Value = filter.Status.Value });
                }

                if (filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }

                if (filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }

                if (!string.IsNullOrEmpty(filter.NationalId))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@nationalId", Value = filter.NationalId });
                }

                if (!string.IsNullOrEmpty(filter.Email))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@email", Value = filter.Email });
                }

                if (!string.IsNullOrEmpty(filter.Mobile))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@mobile", Value = filter.Mobile });
                }

                if (!string.IsNullOrEmpty(filter.VehicleId))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@vehicleId", Value = filter.VehicleId });
                }

                if (!string.IsNullOrEmpty(filter.ChassisNo))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@chassisNo", Value = filter.ChassisNo });
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@bankId", Value = bankId });


                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = pageIndex });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = pageSize });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@isExcel", Value = filter.IsExcel ? 1 : 0 });
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleasingQuotationReportInfoModel> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingQuotationReportInfoModel>(reader).ToList();

                //get data count
                if (!filter.IsExcel)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        public List<AutoleasingProductPriceDetatils> GetPriceDetailsByProductIdsList(List<Guid> productIds, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                if (productIds == null || !productIds.Any())
                {
                    return null;
                }

                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPriceDetailsByProductIdsList";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                var table = new DataTable();
                table.Columns.Add("Item", typeof(string));

                foreach (var item in productIds)
                {
                    table.Rows.Add(item);
                }

                var pList = new SqlParameter("@productIdList", SqlDbType.Structured);
                pList.TypeName = "dbo.StringList";
                pList.Value = table;
                command.Parameters.Add(pList);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleasingProductPriceDetatils> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingProductPriceDetatils>(reader).ToList();

                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        public ShareQuotationOutput ShareQuotation(string phone, string email, string externalId, string userId, QuotationShareTypes shareType, string url, string channel,string lang)
        {
            ShareQuotationOutput output = new ShareQuotationOutput();
            QuotationShares quotationShares = new QuotationShares();
            quotationShares.Channel = channel;
            quotationShares.CreatedDate = DateTime.Now;
            quotationShares.UserId = userId;
            quotationShares.ShareType = shareType.ToString();
            quotationShares.ServerIP = Utilities.GetInternalServerIP();
            quotationShares.UserIP = Utilities.GetUserIPAddress();
            quotationShares.UserAgent = Utilities.GetUserAgent();
            try
            {
                if (string.IsNullOrEmpty(shareType.ToString()))
                {
                    output.ErrorCode = ShareQuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                    quotationShares.ErrorCode = (int)output.ErrorCode;
                    quotationShares.ErrorDescription = "ShareType is empty";
                    _quotationShares.Insert(quotationShares);
                    return output;
                }
                if ((string.IsNullOrEmpty(phone) || !Utilities.IsValidPhoneNo(phone)) && (shareType == QuotationShareTypes.SMS || shareType == QuotationShareTypes.WhatIsAPP))
                {
                    output.ErrorCode = ShareQuotationOutput.ErrorCodes.InvalidPhone;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorPhone", CultureInfo.GetCultureInfo(lang)); 

                    quotationShares.ErrorCode = (int)output.ErrorCode;
                    quotationShares.ErrorDescription = "invalid phone " + phone;
                    _quotationShares.Insert(quotationShares);
                    return output;
                }
                if (!string.IsNullOrEmpty(email))
                    quotationShares.Email = email;
                if (!string.IsNullOrEmpty(phone))
                    quotationShares.Phone = phone;

                if (string.IsNullOrEmpty(externalId))
                {
                    output.ErrorCode = ShareQuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("IncorrectData", CultureInfo.GetCultureInfo(lang));

                    quotationShares.ErrorCode = (int)output.ErrorCode;
                    quotationShares.ErrorDescription = "externalId is empty";
                    _quotationShares.Insert(quotationShares);
                    return output;
                }
                quotationShares.ExternalId = externalId;
                if (string.IsNullOrEmpty(url))
                {
                    output.ErrorCode = ShareQuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("IncorrectData", CultureInfo.GetCultureInfo(lang));

                    quotationShares.ErrorCode = (int)output.ErrorCode;
                    quotationShares.ErrorDescription = "url is empty";
                    _quotationShares.Insert(quotationShares);
                    return output;
                }
                quotationShares.Url = url;
                //check count 
                int count =_quotationService.GetcountFromQuotationSharesByExternalId(externalId, shareType.ToString());
                if(count>=2)
                {
                    output.ErrorCode = ShareQuotationOutput.ErrorCodes.MaxLimitExceeded;
                    output.ErrorDescription = GeneralMessages.ResourceManager.GetString("MaxLimitExceeded", CultureInfo.GetCultureInfo(lang));

                    quotationShares.ErrorCode = (int)output.ErrorCode;
                    quotationShares.ErrorDescription = "Max Limit Exceeded as count is "+count;
                    _quotationShares.Insert(quotationShares);
                    return output;
                }
                string exception = string.Empty;
                if (shareType == QuotationShareTypes.SMS)
                {
                    string smsBody = GeneralMessages.ResourceManager.GetString("QuotationShareSMSBody", CultureInfo.GetCultureInfo(lang)) + " " + url;
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = phone,
                        MessageBody = smsBody,
                        Method = SMSMethod.QuotationShare.ToString(),
                        Module = Module.Vehicle.ToString(),
                        Channel = channel
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode != 0)
                    {
                        output.ErrorCode = ShareQuotationOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                        quotationShares.ErrorCode = (int)output.ErrorCode;
                        quotationShares.ErrorDescription = "Failed to send sms due to " + smsOutput.ErrorDescription;
                        _quotationShares.Insert(quotationShares);
                        return output;
                    }
                }
                else if (shareType == QuotationShareTypes.WhatIsAPP)
                {
                    exception = string.Empty;
                    if (!_notificationService.SendWhatsAppMessageForShareQuoteAsync(phone, url, externalId,lang,out exception))
                    {
                        output.ErrorCode = ShareQuotationOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                        quotationShares.ErrorCode = (int)output.ErrorCode;
                        quotationShares.ErrorDescription = "Failed to send whatis app due to " + exception;
                        _quotationShares.Insert(quotationShares);
                        return output;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(email) || !Utilities.IsValidMail(email))
                    {
                        output.ErrorCode = ShareQuotationOutput.ErrorCodes.InvalidEmail;
                        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorEmail", CultureInfo.GetCultureInfo(lang));

                        quotationShares.ErrorCode = (int)output.ErrorCode;
                        quotationShares.ErrorDescription = "invalid email " + email;
                        _quotationShares.Insert(quotationShares);
                        return output;
                    }
                    string emailSubject = GeneralMessages.ResourceManager.GetString("QuotationShareEmailSubject", CultureInfo.GetCultureInfo(lang));
                    string emailBody = GeneralMessages.ResourceManager.GetString("QuotationShareEmailBody", CultureInfo.GetCultureInfo(lang)).Replace("{0}", url);
                    MessageBodyModel messageBodyModel = new MessageBodyModel();
                    messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/QuotationShare.png";
                    messageBodyModel.Language = lang;
                    messageBodyModel.MessageBody = emailBody;
                    
                    EmailModel emailModel = new EmailModel();
                    emailModel.To = new List<string>();
                    emailModel.To.Add(email);
                    emailModel.Subject = emailSubject;
                    emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                    emailModel.Module = "Vehicle";
                    emailModel.Method = "ShareQuotation";
                    emailModel.Channel = channel;
                    var sendMail = _notificationService.SendEmail(emailModel);
                    if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = ShareQuotationOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                        quotationShares.ErrorCode = (int)output.ErrorCode;
                        quotationShares.ErrorDescription = "Failed to send the email due to " + sendMail.ErrorDescription;
                        _quotationShares.Insert(quotationShares);
                        return output;
                    }
                }

                output.ErrorCode = ShareQuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = GeneralMessages.ResourceManager.GetString("Succcess", CultureInfo.GetCultureInfo(lang));
                quotationShares.ErrorCode = (int)output.ErrorCode;
                quotationShares.ErrorDescription = "Success";
                _quotationShares.Insert(quotationShares);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = ShareQuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = GeneralMessages.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));

                quotationShares.ErrorCode = (int)output.ErrorCode;
                quotationShares.ErrorDescription = exp.ToString();
                _quotationShares.Insert(quotationShares);
                return output;
            }
        }
        #endregion

        #region Renewal

        //public QuotationsFormOutput GetRenewalQuotaionsInfoByExternalId(QuotationRequestLog predefinedLog, QuotationFormWithSelectedBenfitsViewModel model, out string exception)
        //{
        //    QuotationsFormOutput output = new QuotationsFormOutput();
        //    var langId = (model.lang.ToLower() == "en") ? 2 : 1;
        //    exception = string.Empty;

        //    try
        //    {
        //        QuotationInfoModel quotationInfo = _quotationService.GetQuotationsDetails(model.qtRqstExtrnlId, model.AgencyRepair, model.deductible, out exception);
        //        if (!string.IsNullOrEmpty(exception))
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = " db exception";
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = exception;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        if (quotationInfo == null)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = "quotationInfo is null";
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = output.ErrorDescription;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        if (quotationInfo.DepreciationSettingHistory == null && quotationInfo.DepreciationSetting == null)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
        //            output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quotationInfo.Bank.Id}, Maker: {quotationInfo.VehicleMakerCode}, Model: {quotationInfo.VehicleModelCode}";
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        if (quotationInfo.RepairMethodeSetting == null && quotationInfo.RepairMethodeSettingHistory == null)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
        //            output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + quotationInfo.Bank.Id;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }

        //        // as per jira (https://bcare.atlassian.net/browse/ALP-110)
        //        if (model.SelectedCompany > 0)
        //            quotationInfo.Products = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).ToList();

        //        if (model.IsRenewal && quotationInfo.Products.Count > 3)
        //            quotationInfo.Products = quotationInfo.Products.Where(a => model.SelectedBenfits.Select(b => b.ProductId).Contains(a.ProductID)).ToList();

        //        QuotationsFormTemplateViewModel data = new QuotationsFormTemplateViewModel();
        //        data.ExternalId = model.qtRqstExtrnlId;
        //        data.InsuredNationalId = quotationInfo.InsuredNationalId;
        //        data.DriverName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverNameEn : quotationInfo.MainDriverNameAr;
        //        data.DriverNationalId = quotationInfo.DriverNationalId;
        //        data.MainDriverAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverAddressEn : quotationInfo.MainDriverAddressAr;
        //        data.NCDFreeYears = (quotationInfo.NajmNcdFreeYears.HasValue) ? quotationInfo.NajmNcdFreeYears.Value.ToString() : "Not Eligible";
        //        data.VehicleId = quotationInfo.VehicleId;
        //        data.VehicleValue = quotationInfo.VehicleValue.Value.ToString();
        //        data.VehicleMaker = quotationInfo.VehicleMaker;
        //        data.VehicleModel = quotationInfo.VehicleModel;
        //        data.VehicleYear = quotationInfo.VehicleYear;
        //        data.Deductible = model.deductible.ToString();
        //        data.VehicleOwnerName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.Bank.NameEn : quotationInfo.Bank.NameAr;

        //        AutoleasingAgencyRepair bankRepairMethodSettings = null;
        //        if (quotationInfo.RepairMethodeSettingHistory != null)
        //        {
        //            var bankRepairMethodSettingsHistory = quotationInfo.RepairMethodeSettingHistory;
        //            bankRepairMethodSettings = new AutoleasingAgencyRepair()
        //            {
        //                BankId = bankRepairMethodSettingsHistory.BankId,
        //                FirstYear = bankRepairMethodSettingsHistory.FirstYear,
        //                SecondYear = bankRepairMethodSettingsHistory.SecondYear,
        //                ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
        //                FourthYear = bankRepairMethodSettingsHistory.FourthYear,
        //                FifthYear = bankRepairMethodSettingsHistory.FifthYear,
        //            };
        //        }
        //        else
        //        {
        //            var _bankRepairMethodSettings = quotationInfo.RepairMethodeSetting;
        //            bankRepairMethodSettings = new AutoleasingAgencyRepair()
        //            {
        //                BankId = _bankRepairMethodSettings.BankId,
        //                FirstYear = _bankRepairMethodSettings.FirstYear,
        //                SecondYear = _bankRepairMethodSettings.SecondYear,
        //                ThirdYear = _bankRepairMethodSettings.ThirdYear,
        //                FourthYear = _bankRepairMethodSettings.FourthYear,
        //                FifthYear = _bankRepairMethodSettings.FifthYear,
        //            };
        //            bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id).FirstOrDefault();
        //        }

        //        var contractDuration = quotationInfo.ContractDuration.Value;

        //        string repairMethodString = string.Empty;
        //        var policiesNumbers = 0;// _checkoutService.GetPoliciesCount(quotationInfo.DriverNationalId, quotationInfo.VehicleId, quotationInfo.Bank.Id);
        //        if (policiesNumbers == 0)
        //            repairMethodString = bankRepairMethodSettings.FirstYear;
        //        else if (policiesNumbers == 1)
        //            repairMethodString = bankRepairMethodSettings.SecondYear;
        //        else if (policiesNumbers == 2)
        //            repairMethodString = bankRepairMethodSettings.ThirdYear;
        //        else if (policiesNumbers == 3)
        //            repairMethodString = bankRepairMethodSettings.FourthYear;
        //        else if (policiesNumbers == 4)
        //            repairMethodString = bankRepairMethodSettings.FifthYear;

        //        data.Quotationlist = new List<QuotationsFormTemplateQuoteViewModel>();
        //        List<string> selectedAdditionalBenfitsString = new List<string>();
        //        foreach (var product in quotationInfo.Products.Where(q => q.VehicleRepairType == repairMethodString))
        //        {
        //            if (product.InsuranceCompanyID == 5)
        //                continue;
        //            var singleQuotation = new QuotationsFormTemplateQuoteViewModel();
        //            singleQuotation.CompanyKey = product.CompanyKey;
        //            //singleQuotation.ImageURL = product.CompanyKey;
        //            singleQuotation.ProductName = "Comprehensive";
        //            singleQuotation.DeductableValue = product.DeductableValue;

        //            if (data.Quotationlist.Where(a => a.CompanyKey == singleQuotation.CompanyKey && a.DeductableValue == singleQuotation.DeductableValue).FirstOrDefault() != null)
        //                continue;

        //            var productPriceDetails = product.PriceDetails;
        //            if (productPriceDetails == null)
        //                continue;

        //            if (productPriceDetails.Any(a => a.PriceTypeCode == 7))
        //            {
        //                var basicPremium = productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 7).PriceValue;
        //                singleQuotation.TotalPremium = Math.Round(basicPremium, 2).ToString();
        //                //singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";  // (quotationInfo.VehicleValue.Value / basicPremium).ToString();
        //            }
        //            else
        //            {
        //                singleQuotation.TotalPremium = "0.00";
        //                //singleQuotation.InsurancePercentage = "0.00";
        //            }

        //            if (productPriceDetails.Any(a => a.PriceTypeCode == 4))
        //                singleQuotation.ClaimLoading = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 4).PriceValue, 2).ToString();
        //            else
        //                singleQuotation.ClaimLoading = "0.00";

        //            //singleQuotation.MinimumPremium = product.PriceDetails.FirstOrDefault(a => a.PriceTypeCode == )?.PriceValue.ToString();

        //            singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";
        //            singleQuotation.ShadowAmount = product.ShadowAmount.ToString() ?? "0.00";
        //            singleQuotation.MinimumPremium = "0.00";

        //            //decimal _shadowAmount = 0;
        //            //foreach (var price in productPriceDetails)
        //            //{
        //            //    if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 ||
        //            //            price.PriceTypeCode == 11 || price.PriceTypeCode == 12) // Discounts
        //            //        _shadowAmount += price.PriceValue;
        //            //}

        //            //if (_shadowAmount > 0)
        //            //    singleQuotation.ShadowAmount = Math.Round(_shadowAmount, 2).ToString();
        //            //else
        //            //    singleQuotation.ShadowAmount = "0.00";

        //            if (productPriceDetails.Any(a => a.PriceTypeCode == 3))
        //                singleQuotation.LoyalityAmount = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 3).PriceValue, 2).ToString();
        //            else
        //                singleQuotation.LoyalityAmount = "0.00";

        //            List<short?> selectedBenfitsIds = new List<short?>();
        //            List<string> selectedBenfitsExternalIds = new List<string>();
        //            bool useBenefitExternalId = false;
        //            if (model.SelectedBenfits != null)
        //            {
        //                var userBenefits = model.SelectedBenfits.Where(a => a.ProductId == product.ProductID).FirstOrDefault();
        //                if (userBenefits != null)
        //                {
        //                    selectedBenfitsIds = userBenefits.BenfitIds;
        //                    if (userBenefits.UseExternalId)
        //                    {
        //                        useBenefitExternalId = true;
        //                        selectedBenfitsExternalIds = userBenefits.BenfitExternalIds;
        //                    }
        //                }
        //            }

        //            if (quotationInfo.Bank.Id == 2) //alyusr
        //            {
        //                var preSelectedBenefits = product.Benfits.Where(p => p.IsReadOnly && p.IsSelected.HasValue && p.IsSelected.Value);
        //                if (useBenefitExternalId)
        //                {
        //                    var clientSelectedBenefitsExternalId = selectedBenfitsExternalIds.Except(preSelectedBenefits.Select(b => b.BenefitExternalId));
        //                    var clientSelectedBenefitsExternal = product.Benfits.Where(b => clientSelectedBenefitsExternalId.Contains(b.BenefitExternalId));
        //                    if (clientSelectedBenefitsExternal != null && clientSelectedBenefitsExternal.Any())
        //                    {
        //                        singleQuotation.InsurancePercentage = Decimal.Round((product.ProductPrice + clientSelectedBenefitsExternal.Sum(b => b.BenefitPrice.Value * 1.15M)) * 100 / quotationInfo.VehicleValue.Value, 2).ToString();
        //                    }
        //                }
        //                else if (selectedBenfitsIds != null && selectedBenfitsIds.Any())
        //                {
        //                    var clientSelectedBenefitsId = selectedBenfitsIds.Except(preSelectedBenefits.Select(b => b.BenefitId));
        //                    var clientSelectedBenefits = product.Benfits.Where(b => clientSelectedBenefitsId.Contains(b.BenefitId));
        //                    if (clientSelectedBenefits != null && clientSelectedBenefits.Any())
        //                    {
        //                        singleQuotation.InsurancePercentage = Decimal.Round((product.ProductPrice + clientSelectedBenefits.Sum(b => b.BenefitPrice.Value * 1.15M)) * 100 / quotationInfo.VehicleValue.Value, 2).ToString();
        //                    }
        //                }
        //            }

        //            singleQuotation.Benefits = new List<QuotationsFormTemplateQuoteBenfitViewModel>();
        //            var productBenfits = product.Benfits;
        //            if (productBenfits != null)
        //            {
        //                decimal _selectedBenfitVat = 0;
        //                decimal _selectedBenfitTotalPrice = 0;
        //                foreach (var benfit in productBenfits)
        //                {
        //                    if (benfit.IsReadOnly)
        //                    {
        //                        singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
        //                        {
        //                            BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
        //                            IsChecked = true
        //                        });
        //                        continue;
        //                    }
        //                    else if (useBenefitExternalId && selectedBenfitsExternalIds.IndexOf(benfit.BenefitExternalId) != -1)
        //                    {
        //                        if (benfit.BenefitPrice.HasValue)
        //                        {
        //                            _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
        //                            _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
        //                        }

        //                        selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
        //                        singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
        //                        {
        //                            BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
        //                            IsChecked = true
        //                        });
        //                    }
        //                    else if (selectedBenfitsIds != null && selectedBenfitsIds.IndexOf(benfit.BenefitId) != -1)
        //                    {
        //                        if (benfit.BenefitPrice.HasValue)
        //                        {
        //                            _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
        //                            _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
        //                        }

        //                        selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
        //                        singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
        //                        {
        //                            BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
        //                            IsChecked = true
        //                        });
        //                    }
        //                    else
        //                    {
        //                        singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
        //                        {
        //                            BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
        //                            IsChecked = false
        //                        });
        //                    }
        //                }

        //                if (productPriceDetails.Any(a => a.PriceTypeCode == 8))
        //                    singleQuotation.VAT = Math.Round((_selectedBenfitVat + productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 8).PriceValue), 2).ToString();
        //                else if (_selectedBenfitVat > 0)
        //                    singleQuotation.VAT = (Math.Round(_selectedBenfitVat, 2)).ToString();
        //                else
        //                    singleQuotation.VAT = "0.00";

        //                singleQuotation.Total = (Math.Round(product.ProductPrice + _selectedBenfitVat + _selectedBenfitTotalPrice, 2)).ToString();
        //            }
        //            data.Quotationlist.Add(singleQuotation);
        //        }

        //        data.TotalQuotations = data.Quotationlist.Count;
        //        data.AdditionalBenfits = string.Join(", ", selectedAdditionalBenfitsString);

        //        #region Deprecation Data

        //        AutoleasingDepreciationSetting depreciationSetting = null;
        //        if (quotationInfo.DepreciationSettingHistory != null)
        //        {
        //            var deprecationHistory = quotationInfo.DepreciationSettingHistory;
        //            depreciationSetting = new AutoleasingDepreciationSetting()
        //            {
        //                BankId = quotationInfo.Bank.Id,
        //                MakerCode = deprecationHistory.MakerCode,
        //                ModelCode = deprecationHistory.ModelCode,
        //                MakerName = deprecationHistory.MakerName,
        //                ModelName = deprecationHistory.ModelName,
        //                Percentage = deprecationHistory.Percentage,
        //                IsDynamic = deprecationHistory.IsDynamic,
        //                FirstYear = deprecationHistory.FirstYear,
        //                SecondYear = deprecationHistory.SecondYear,
        //                ThirdYear = deprecationHistory.ThirdYear,
        //                FourthYear = deprecationHistory.FourthYear,
        //                FifthYear = deprecationHistory.FifthYear,
        //                AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
        //            };
        //        }
        //        else
        //        {
        //            var deprecationSetting = quotationInfo.DepreciationSetting;
        //            depreciationSetting = new AutoleasingDepreciationSetting();
        //            depreciationSetting.BankId = quotationInfo.Bank.Id;
        //            depreciationSetting.MakerCode = deprecationSetting.MakerCode;
        //            depreciationSetting.ModelCode = deprecationSetting.ModelCode;
        //            depreciationSetting.MakerName = deprecationSetting.MakerName;
        //            depreciationSetting.ModelName = deprecationSetting.ModelName;
        //            depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
        //            depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
        //            depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
        //            depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
        //            depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
        //            depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
        //            depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
        //            depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
        //        }

        //        data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;
        //        data.AnnualDeprecationPercentage = depreciationSetting.Percentage.ToString();

        //        List<string> annualPercentages = new List<string>();

        //        if (contractDuration >= 1)
        //        {
        //            if (!depreciationSetting.FirstYear.Equals(null))
        //            {
        //                //data.FirstYear = depreciationSetting.FirstYear.ToString().Split('.')[0] + "%";
        //                //annualPercentages.Add(depreciationSetting.FirstYear.ToString().Split('.')[0] + "%");
        //                data.FirstYear = "0 %";
        //                annualPercentages.Add("0 %");
        //            }
        //        }

        //        if (contractDuration >= 2)
        //        {
        //            if (!depreciationSetting.SecondYear.Equals(null))
        //            {
        //                data.SecondYear = depreciationSetting.SecondYear.ToString().Split('.')[0] + " %";
        //                annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + " %");
        //            }
        //        }

        //        if (contractDuration >= 3)
        //        {
        //            if (!depreciationSetting.ThirdYear.Equals(null))
        //            {
        //                data.ThirdYear = depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %";
        //                annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %");
        //            }
        //        }

        //        if (contractDuration >= 4)
        //        {
        //            if (!depreciationSetting.FourthYear.Equals(null))
        //            {
        //                data.FourthYear = depreciationSetting.FourthYear.ToString().Split('.')[0] + " %";
        //                annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + " %");
        //            }
        //        }

        //        if (contractDuration >= 5)
        //        {
        //            if (!depreciationSetting.FifthYear.Equals(null))
        //            {
        //                data.FifthYear = depreciationSetting.FifthYear.ToString().Split('.')[0] + " %";
        //                annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + " %");
        //            }
        //        }

        //        data.IsDynamic = depreciationSetting.IsDynamic;
        //        data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + " %";


        //        var vehicleValue = quotationInfo.VehicleValue.Value;
        //        Decimal? DepreciationValue1 = 0;
        //        Decimal? DepreciationValue2 = 0;
        //        Decimal? DepreciationValue3 = 0;
        //        Decimal? DepreciationValue4 = 0;
        //        Decimal? DepreciationValue5 = 0;
        //        List<Decimal?> depreciationValues = new List<Decimal?>();
        //        var currentVehicleValue = vehicleValue;

        //        if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
        //        {
        //            if (depreciationSetting.IsDynamic)
        //            {
        //                DepreciationValue1 = vehicleValue;

        //                if (depreciationSetting.SecondYear != 0)
        //                    DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

        //                if (depreciationSetting.ThirdYear != 0)
        //                    DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

        //                if (depreciationSetting.FourthYear != 0)
        //                    DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

        //                if (depreciationSetting.FifthYear != 0)
        //                    DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
        //            }
        //            else
        //            {
        //                DepreciationValue1 = currentVehicleValue;
        //                DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
        //                DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
        //                DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
        //                DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
        //            }
        //        }
        //        else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
        //        {
        //            if (depreciationSetting.IsDynamic)
        //            {
        //                DepreciationValue1 = vehicleValue;

        //                if (depreciationSetting.SecondYear != 0)
        //                    DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

        //                if (depreciationSetting.ThirdYear != 0)
        //                    DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

        //                if (depreciationSetting.FourthYear != 0)
        //                    DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

        //                if (depreciationSetting.FifthYear != 0)
        //                    DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
        //            }
        //            else
        //            {
        //                DepreciationValue1 = vehicleValue;
        //                DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
        //                DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
        //                DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
        //                DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
        //            }
        //        }
        //        List<PremiumReference> premiumReference = new List<PremiumReference>();
        //        List<string> repairMethodList = new List<string>();
        //        if (contractDuration >= 1)
        //        {
        //            repairMethodList.Add(bankRepairMethodSettings.FirstYear);
        //        }
        //        if (contractDuration >= 2)
        //        {
        //            repairMethodList.Add(bankRepairMethodSettings.SecondYear);
        //        }
        //        if (contractDuration >= 3)
        //        {
        //            repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
        //        }
        //        if (contractDuration >= 4)
        //        {
        //            repairMethodList.Add(bankRepairMethodSettings.FourthYear);
        //        }
        //        if (contractDuration >= 5)
        //        {
        //            repairMethodList.Add(bankRepairMethodSettings.FifthYear);
        //        }

        //        List<string> allRepairMethods = new List<string>();
        //        foreach (var item in repairMethodList)
        //            allRepairMethods.Add(item.Substring(0, 1));

        //        if (allRepairMethods != null && allRepairMethods.Count > 0)
        //            data.RepairType = string.Join(", ", allRepairMethods);

        //        List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();

        //        int countAgency = repairMethodList.Where(r => r == "Agency").Count();
        //        int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();
        //        string RepairType = string.Empty;
        //        if (repairMethodList.Count() == countAgency)
        //        {
        //            products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Agency").ToList();
        //            RepairType = "Agency";
        //        }
        //        else if (repairMethodList.Count() == countWorkShop)
        //        {
        //            products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Workshop").ToList();
        //            RepairType = "Workshop";
        //        }
        //        else
        //        {
        //            products = quotationInfo.Products;
        //            RepairType = "Mixed";
        //        }

        //        if (model.IsRenewal)
        //            products = products.OrderBy(a => a.ProductPrice).Take(3).ToList();

        //        Decimal? InsurancePercentageAgency = 0;
        //        Decimal? InsurancePercentageWorkshop = 0;

        //        foreach (var _product in products)
        //        {
        //            if (_product.InsuranceCompanyID == 5)
        //                continue;
        //            //var product = quotation.Products.FirstOrDefault();
        //            if (_product == null)
        //                continue;
        //            if (_product.PriceDetails == null)
        //                continue;

        //            //if (InsurancePercentageAgency == 0 && _product.VehicleRepairType == "Agency")
        //            //{
        //            //    InsurancePercentageAgency = _product.InsurancePercentage;
        //            //}
        //            //else if (InsurancePercentageWorkshop == 0 && _product.VehicleRepairType == "Workshop")
        //            //{
        //            //    InsurancePercentageWorkshop = _product.InsurancePercentage;
        //            //}
        //            decimal? basicPremium = _product.PriceDetails.Where(p => p.PriceTypeCode == 7)?.FirstOrDefault()?.PriceValue;

        //            decimal? otherTypeCodes = _product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
        //            || p.PriceTypeCode == 6 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);

        //            decimal? vat = _product.PriceDetails.Where(p => p.PriceTypeCode == 8)?.FirstOrDefault()?.PriceValue;
        //            //var discounts = product.PriceDetails.Where(p => p.PriceTypeCode == 1 || p.PriceTypeCode == 2
        //            //  || p.PriceTypeCode == 3 || p.PriceTypeCode == 10 || p.PriceTypeCode == 11 || p.PriceTypeCode == 12)?.Sum(p => p.PriceValue);

        //            List<short?> selectedBenfitsIds = new List<short?>();
        //            List<string> selectedBenfitsExternalIds = new List<string>();
        //            bool useBenefitExternalId = false;
        //            if (model.SelectedBenfits != null)
        //            {
        //                var userBenefits = model.SelectedBenfits.Where(a => a.ProductId == _product.ProductID).FirstOrDefault();
        //                if (userBenefits != null)
        //                {
        //                    selectedBenfitsIds = userBenefits.BenfitIds;
        //                    if (userBenefits.UseExternalId)
        //                    {
        //                        useBenefitExternalId = true;
        //                        selectedBenfitsExternalIds = userBenefits.BenfitExternalIds;
        //                    }
        //                }
        //            }

        //            decimal? benefits = 0;
        //            decimal? clientBenefitsAmount = 0;
        //            decimal? clientBenefitsAmountWithVAT = 0;
        //            if (selectedBenfitsIds != null && model.SelectedBenfits.Any())
        //            {
        //                if (useBenefitExternalId)
        //                {
        //                    benefits = _product.Benfits.Where(b => !string.IsNullOrEmpty(b.BenefitExternalId) && selectedBenfitsExternalIds.Contains(b.BenefitExternalId))?.Sum(b => b.BenefitPrice.Value);
        //                    clientBenefitsAmount = benefits;
        //                    clientBenefitsAmountWithVAT = benefits * 1.15M;
        //                    benefits *= 1.15M;
        //                }
        //                else
        //                {
        //                    benefits = _product.Benfits.Where(b => b.BenefitId.HasValue && selectedBenfitsIds.Contains(b.BenefitId.Value))?.Sum(b => b.BenefitPrice.Value);
        //                    clientBenefitsAmount = benefits;
        //                    clientBenefitsAmountWithVAT = benefits * 1.15M;
        //                    benefits *= 1.15M;
        //                }
        //            }

        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                //var MPAD = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MPAD")).FirstOrDefault();
        //                //var MGAE = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MGAE")).FirstOrDefault();
        //                //var MRCR = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MRCR")).FirstOrDefault();

        //                //short MPAD_Id = 0;
        //                //short MGAE_Id = 0;
        //                //short MRCR_Id = 0;

        //                //if (MPAD != null)
        //                //{
        //                //    MPAD_Id = MPAD.BenefitId.Value;
        //                //}

        //                //if (MGAE != null)
        //                //{
        //                //    MGAE_Id = MGAE.BenefitId.Value;
        //                //}

        //                //if (MRCR != null)
        //                //{
        //                //    MRCR_Id = MRCR.BenefitId.Value;
        //                //}

        //                foreach (var benefit in _product.Benfits)
        //                {
        //                    if (selectedBenfitsIds != null && selectedBenfitsIds.Any() && selectedBenfitsIds.Contains(benefit.BenefitId))
        //                    {
        //                        continue;
        //                    }
        //                    if (selectedBenfitsIds != null && selectedBenfitsIds.Any() && selectedBenfitsIds.Contains(benefit.BenefitId))
        //                    {
        //                        continue;
        //                    }

        //                    if (benefit.IsReadOnly && benefit.IsSelected.HasValue && benefit.IsSelected.Value)
        //                    {
        //                        benefits += benefit.BenefitPrice.Value;
        //                    }

        //                    //if (benefit.BenefitId == 1
        //                    //    || benefit.BenefitId == 6
        //                    //    || benefit.BenefitId == 10)
        //                    //{
        //                    //    benefits += benefit.BenefitPrice.Value;
        //                    //}
        //                    //else if (MPAD_Id != 0 && benefit.BenefitId == MPAD_Id)
        //                    //{
        //                    //    benefits += benefit.BenefitPrice.Value;
        //                    //}
        //                    //else if (MGAE_Id != 0 && benefit.BenefitId == MGAE_Id)
        //                    //{
        //                    //    benefits += benefit.BenefitPrice.Value;
        //                    //}
        //                    //else if (MRCR_Id != 0 && benefit.BenefitId == MRCR_Id)
        //                    //{
        //                    //    benefits += benefit.BenefitPrice.Value;
        //                    //}
        //                }

        //            }

        //            var otherCodesAndBenifits = otherTypeCodes + benefits;

        //            var premium = basicPremium + vat + otherCodesAndBenifits;// - discounts;

        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                premiumReference.Add(
        //                new PremiumReference
        //                {
        //                    Premium = premium ?? 0,
        //                    ReferenceId = _product.ReferenceId,
        //                    BasicPremium = basicPremium ?? 0,
        //                    VehicleRepairType = _product.VehicleRepairType,
        //                    VAT = vat ?? 0,
        //                    InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / vehicleValue,
        //                    OtherCodesAndBenifits = otherTypeCodes + clientBenefitsAmount
        //                });
        //            }
        //            else
        //            {
        //                premiumReference.Add(
        //                new PremiumReference
        //                {
        //                    Premium = premium ?? 0,
        //                    ReferenceId = _product.ReferenceId,
        //                    BasicPremium = basicPremium ?? 0,
        //                    VehicleRepairType = _product.VehicleRepairType,
        //                    VAT = vat ?? 0,
        //                    InsurancePercentage = _product.InsurancePercentage,
        //                    OtherCodesAndBenifits = otherCodesAndBenifits
        //                });
        //            }
        //        }

        //        var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType == "Agency").OrderBy(p => p.Premium).FirstOrDefault();
        //        var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType == "Workshop").OrderBy(p => p.Premium).FirstOrDefault();
        //        var lowestPremiumAlYusr = premiumReference.Where(p => p.VehicleRepairType == bankRepairMethodSettings.FirstYear).OrderBy(p => p.Premium).FirstOrDefault();

        //        //Decimal? BasicPremium = lowestPremium.BasicPremium;

        //        AutoleasingMinimumPremium bankMinimumPremiumSettings = null;
        //        if (quotationInfo.MinimumPremiumSettingHistory != null)
        //        {
        //            var minimumPremiumSettingHistory = quotationInfo.MinimumPremiumSettingHistory;
        //            bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
        //            {
        //                BankId = quotationInfo.Bank.Id,
        //                FirstYear = minimumPremiumSettingHistory.FirstYear,
        //                SecondYear = minimumPremiumSettingHistory.SecondYear,
        //                ThirdYear = minimumPremiumSettingHistory.ThirdYear,
        //                FourthYear = minimumPremiumSettingHistory.FourthYear,
        //                FifthYear = minimumPremiumSettingHistory.FifthYear
        //            };
        //        }
        //        else
        //        {
        //            var minimumPremiumSetting = quotationInfo.MinimumPremiumSetting;
        //            bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
        //            {
        //                BankId = quotationInfo.Bank.Id,
        //                FirstYear = minimumPremiumSetting.FirstYear,
        //                SecondYear = minimumPremiumSetting.SecondYear,
        //                ThirdYear = minimumPremiumSetting.ThirdYear,
        //                FourthYear = minimumPremiumSetting.FourthYear,
        //                FifthYear = minimumPremiumSetting.FifthYear
        //            };
        //        }

        //        Decimal? InsurancePercentage1 = 0;
        //        Decimal? InsurancePercentage2 = 0;
        //        Decimal? InsurancePercentage3 = 0;
        //        Decimal? InsurancePercentage4 = 0;
        //        Decimal? InsurancePercentage5 = 0;

        //        Decimal? Premium1 = 0;
        //        Decimal? Premium2 = 0;
        //        Decimal? Premium3 = 0;
        //        Decimal? Premium4 = 0;
        //        Decimal? Premium5 = 0;
        //        if (contractDuration >= 1)
        //        {
        //            data.RepairMethod1 = bankRepairMethodSettings.FirstYear;
        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                InsurancePercentage1 = lowestPremiumAlYusr.InsurancePercentage;
        //                Premium1 = lowestPremiumAlYusr.Premium;
        //                if (Premium1 < bankMinimumPremiumSettings.FirstYear)
        //                {
        //                    Premium1 = bankMinimumPremiumSettings.FirstYear;
        //                }
        //            }
        //            else if (bankRepairMethodSettings.FirstYear == "Agency")
        //            {
        //                InsurancePercentage1 = lowestPremiumAgency.InsurancePercentage;
        //                Premium1 = lowestPremiumAgency.Premium;
        //            }
        //            else
        //            {
        //                InsurancePercentage1 = lowestPremiumWorkshop.InsurancePercentage;
        //                Premium1 = lowestPremiumWorkshop.Premium;
        //            }
        //            data.InsurancePercentage1 = Math.Round(InsurancePercentage1.Value, 2).ToString();
        //            data.Premium1 = Math.Round(Premium1.Value, 2).ToString();
        //            data.Deductible1 = data.Deductible;
        //            data.MiuimumPremium1 = bankMinimumPremiumSettings.FirstYear.ToString().Replace(".00", "");
        //        }
        //        if (contractDuration >= 2)
        //        {
        //            data.RepairMethod2 = bankRepairMethodSettings.SecondYear;
        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                InsurancePercentage2 = lowestPremiumAlYusr.InsurancePercentage;
        //                Premium2 = (InsurancePercentage2 * DepreciationValue2) / 100;
        //                if (Premium2 < bankMinimumPremiumSettings.SecondYear)
        //                {
        //                    Premium2 = bankMinimumPremiumSettings.SecondYear;
        //                }
        //            }
        //            else if (bankRepairMethodSettings.SecondYear == "Agency")
        //            {
        //                InsurancePercentage2 = lowestPremiumAgency.InsurancePercentage;
        //                Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            else
        //            {
        //                InsurancePercentage2 = lowestPremiumWorkshop.InsurancePercentage;
        //                Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            data.InsurancePercentage2 = Math.Round(InsurancePercentage2.Value, 2).ToString();
        //            data.Premium2 = Math.Round(Premium2.Value, 2).ToString();
        //            data.Deductible2 = data.Deductible;
        //            data.MiuimumPremium2 = bankMinimumPremiumSettings.SecondYear.ToString().Replace(".00", "");

        //        }
        //        if (contractDuration >= 3)
        //        {
        //            data.RepairMethod3 = bankRepairMethodSettings.ThirdYear;
        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                InsurancePercentage3 = lowestPremiumAlYusr.InsurancePercentage;
        //                Premium3 = (InsurancePercentage3 * DepreciationValue3) / 100;
        //                if (Premium3 < bankMinimumPremiumSettings.ThirdYear)
        //                {
        //                    Premium3 = bankMinimumPremiumSettings.ThirdYear;
        //                }
        //            }
        //            else if (bankRepairMethodSettings.ThirdYear == "Agency")
        //            {
        //                InsurancePercentage3 = lowestPremiumAgency.InsurancePercentage;
        //                Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            else
        //            {
        //                InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
        //                Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            data.InsurancePercentage3 = Math.Round(InsurancePercentage3.Value, 2).ToString();
        //            data.Premium3 = Math.Round(Premium3.Value, 2).ToString();
        //            data.Deductible3 = data.Deductible;
        //            data.MiuimumPremium3 = bankMinimumPremiumSettings.ThirdYear.ToString().Replace(".00", "");

        //        }
        //        if (contractDuration >= 4)
        //        {
        //            data.RepairMethod4 = bankRepairMethodSettings.FourthYear;
        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                InsurancePercentage4 = lowestPremiumAlYusr.InsurancePercentage;
        //                Premium4 = (InsurancePercentage4 * DepreciationValue4) / 100;
        //                if (Premium4 < bankMinimumPremiumSettings.FourthYear)
        //                {
        //                    Premium4 = bankMinimumPremiumSettings.FourthYear;
        //                }
        //            }
        //            else if (bankRepairMethodSettings.FourthYear == "Agency")
        //            {
        //                InsurancePercentage4 = lowestPremiumAgency.InsurancePercentage;
        //                Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            else
        //            {
        //                InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
        //                Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            data.InsurancePercentage4 = Math.Round(InsurancePercentage4.Value, 2).ToString();
        //            data.Premium4 = Math.Round(Premium4.Value, 2).ToString();
        //            data.Deductible4 = data.Deductible;
        //            data.MiuimumPremium4 = bankMinimumPremiumSettings.FourthYear.ToString().Replace(".00", "");

        //        }
        //        if (contractDuration >= 5)
        //        {
        //            data.RepairMethod5 = bankRepairMethodSettings.FifthYear;
        //            if (quotationInfo.Bank.Id == 2) //Yusr
        //            {
        //                InsurancePercentage5 = lowestPremiumAlYusr.InsurancePercentage;
        //                Premium5 = (InsurancePercentage5 * DepreciationValue5) / 100;
        //                if (Premium5 < bankMinimumPremiumSettings.FifthYear)
        //                {
        //                    Premium5 = bankMinimumPremiumSettings.FifthYear;
        //                }
        //            }
        //            else if (bankRepairMethodSettings.FifthYear == "Agency")
        //            {
        //                InsurancePercentage5 = lowestPremiumAgency.InsurancePercentage;
        //                Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            else
        //            {
        //                InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
        //                Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
        //            }
        //            data.InsurancePercentage5 = Math.Round(InsurancePercentage5.Value, 2).ToString();
        //            data.Premium5 = Math.Round(Premium5.Value, 2).ToString();
        //            data.Deductible5 = data.Deductible;
        //            data.MiuimumPremium5 = bankMinimumPremiumSettings.FifthYear.ToString().Replace(".00", "");

        //        }
        //        data.Total5YearsPremium = Math.Round((Premium1 ?? 0) + (Premium2 ?? 0) + (Premium3 ?? 0) + (Premium4 ?? 0) + (Premium5 ?? 0), 2).ToString();
        //        #endregion

        //        int index = 0;
        //        if (model.IsRenewal)
        //        {
        //            var previousSettings = _autoleasingQuotationFormSettingsRepository.TableNoTracking.Where(a => a.VehicleId == quotationInfo.VehicleId && a.IsPurchased).ToList();
        //            if (previousSettings != null && previousSettings.Count > 0)
        //            {
        //                for (int i = 0; i < previousSettings.Count; i++)
        //                {
        //                    index = i + 1;
        //                    HandleQuotationFormDynamicValues(data, previousSettings[i], index);
        //                    if (index == previousSettings.Count)
        //                        break;
        //                }
        //            }

        //            //exception = string.Empty;
        //            //HandleRenewalOldDepreciationValues(data, model.qtRqstExtrnlId, model.VehiclOriginaleValue, depreciationSetting, index, out exception);
        //        }

        //        var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
        //        //log.RetrievingMethod = "Generation";
        //        //log.ServiceURL = serviceURL;

        //        predefinedLog.ServerIP = Utilities.GetInternalServerIP();
        //        //log.CompanyID = quoteResponse.InsuranceCompanyId;
        //        if (string.IsNullOrEmpty(predefinedLog.Channel))
        //            predefinedLog.Channel = Channel.autoleasing.ToString();

        //        string policyDetailsJsonString = JsonConvert.SerializeObject(data);
        //        AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
        //        {
        //            ReportType = "IndividualQuotationsFormTemplate",
        //            ReportDataAsJsonString = policyDetailsJsonString
        //        };
        //        HttpClient client = new HttpClient();
        //        string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
        //        //log.ServiceRequest = reportGenerationModelAsJson;

        //        var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
        //        DateTime dtBeforeCalling = DateTime.Now;
        //        HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
        //        DateTime dtAfterCalling = DateTime.Now;
        //        //log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

        //        if (response == null)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = "Service return null";
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = output.ErrorDescription;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        if (response.Content == null)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = "Service response content return null";
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = output.ErrorDescription;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = "Service response content result return null";
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = output.ErrorDescription;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
        //            output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
        //            predefinedLog.ErrorCode = (int)output.ErrorCode;
        //            predefinedLog.ErrorDescription = output.ErrorDescription;
        //            QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //            return output;
        //        }
        //        /* Save hits of download quotation form in db*/
        //        var autoleasingUser = _autoleasingUserService.GetUser(predefinedLog.UserId);
        //        var bank = _bankService.GetBank(autoleasingUser.BankId);
        //        string filePath = string.Empty;
        //        if (autoleasingUser != null && bank != null)
        //        {
        //            AutoleasingQuotationForm form = new AutoleasingQuotationForm();
        //            form.ExternalId = model.qtRqstExtrnlId;
        //            form.CreatedDate = DateTime.Now;
        //            form.BankId = autoleasingUser.BankId;
        //            form.BankName = bank.NameEn;
        //            form.UserId = predefinedLog.UserId;
        //            form.UserEmail = autoleasingUser.Email;
        //            form.FilePath = Utilities.SaveQuotationFormFile(model.qtRqstExtrnlId, JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString), bank.NameEn);
        //            _autoleasingQuotationFormService.Insert(form);

        //            filePath = form.FilePath;
        //        }
        //        /* end  */

        //        /* Save selected company which download quotation form in db*/

        //        if (model.SelectedCompany > 0)
        //        {
        //            var oldCompanies = _autoleasingInitialQuotationCompaniesRepository.Table.Where(a => a.ExternalId == model.qtRqstExtrnlId).ToList();
        //            if (oldCompanies != null && oldCompanies.Count > 0)
        //                _autoleasingInitialQuotationCompaniesRepository.Delete(oldCompanies);

        //            AutoleasingInitialQuotationCompanies companyQuote = new AutoleasingInitialQuotationCompanies();
        //            companyQuote.ExternalId = model.qtRqstExtrnlId;
        //            companyQuote.CompanyId = model.SelectedCompany;
        //            companyQuote.BankId = autoleasingUser.BankId;
        //            companyQuote.UserId = predefinedLog.UserId;
        //            companyQuote.CreatedDate = DateTime.Now;
        //            _autoleasingInitialQuotationCompaniesRepository.Insert(companyQuote);
        //        }

        //        /* end  */

        //        /* Save current year settings in AutoleasingQuotationFormSettings */
        //        AutoleasingQuotationFormSettings autoleasingQuotationFormSettings = new AutoleasingQuotationFormSettings();
        //        autoleasingQuotationFormSettings.VehicleId = quotationInfo.VehicleId;
        //        autoleasingQuotationFormSettings.ExternalId = model.qtRqstExtrnlId;
        //        autoleasingQuotationFormSettings.BankId = quotationInfo.Bank.Id;
        //        autoleasingQuotationFormSettings.Depreciation = data.Percentage;
        //        autoleasingQuotationFormSettings.Total5YearsPremium = data.Total5YearsPremium;
        //        autoleasingQuotationFormSettings.UserId = predefinedLog.UserId;
        //        autoleasingQuotationFormSettings.IsPurchased = false;
        //        autoleasingQuotationFormSettings.FilePath = filePath;
        //        autoleasingQuotationFormSettings.CreateDate = DateTime.Now;
        //        autoleasingQuotationFormSettings.CreatedBy = predefinedLog.UserId;
        //        autoleasingQuotationFormSettings.SelectedInsuranceCompany = model.SelectedCompany;
        //        if (quotationInfo.Products.Any(a => a.InsuranceCompanyID == model.SelectedCompany))
        //            autoleasingQuotationFormSettings.SelectedInsuranceCompanyName = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).FirstOrDefault().CompanyKey;
        //        if (index > 0)
        //        {
        //            autoleasingQuotationFormSettings.InsurancePercentage = data.GetType().GetProperty("InsurancePercentage" + index).GetValue(data, null).ToString();
        //            autoleasingQuotationFormSettings.Premium = data.GetType().GetProperty("Premium" + index).GetValue(data, null).ToString();
        //            autoleasingQuotationFormSettings.RepairMethod = data.GetType().GetProperty("RepairMethod" + index).GetValue(data, null).ToString();
        //            autoleasingQuotationFormSettings.Deductible = data.GetType().GetProperty("Deductible" + index).GetValue(data, null).ToString();
        //            autoleasingQuotationFormSettings.MinimumPremium = data.GetType().GetProperty("MiuimumPremium" + index).GetValue(data, null).ToString();
        //        }
        //        else
        //        {
        //            autoleasingQuotationFormSettings.InsurancePercentage = data.InsurancePercentage1;
        //            autoleasingQuotationFormSettings.Premium = data.Premium1;
        //            autoleasingQuotationFormSettings.RepairMethod = data.RepairMethod1;
        //            autoleasingQuotationFormSettings.Deductible = data.Deductible1;
        //            autoleasingQuotationFormSettings.MinimumPremium = data.MiuimumPremium1;
        //        }

        //        if (_autoleasingQuotationFormSettingsRepository.TableNoTracking.Any(a => a.VehicleId == autoleasingQuotationFormSettings.VehicleId && a.ExternalId == autoleasingQuotationFormSettings.ExternalId))
        //        {
        //            var _quotationSettings = _autoleasingQuotationFormSettingsRepository.Table.Where(a => a.VehicleId == autoleasingQuotationFormSettings.VehicleId && a.ExternalId == autoleasingQuotationFormSettings.ExternalId).FirstOrDefault();
        //            _quotationSettings.InsurancePercentage = data.InsurancePercentage1;
        //            _quotationSettings.Premium = data.Premium1;
        //            _quotationSettings.RepairMethod = data.RepairMethod1;
        //            _quotationSettings.Deductible = data.Deductible1;
        //            _quotationSettings.MinimumPremium = data.MiuimumPremium1;
        //            _quotationSettings.Depreciation = data.Percentage;
        //            _quotationSettings.Total5YearsPremium = data.Total5YearsPremium;
        //            _quotationSettings.UserId = predefinedLog.UserId;
        //            _quotationSettings.FilePath = filePath;
        //            if (model.SelectedCompany > 0)
        //            {
        //                _quotationSettings.SelectedInsuranceCompany = model.SelectedCompany;
        //                if (quotationInfo.Products.Any(a => a.InsuranceCompanyID == model.SelectedCompany))
        //                    _quotationSettings.SelectedInsuranceCompanyName = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).FirstOrDefault().CompanyKey;
        //            }
        //            else
        //            {
        //                _quotationSettings.SelectedInsuranceCompany = null;
        //                _quotationSettings.SelectedInsuranceCompanyName = null;
        //            }

        //            _quotationSettings.ModifiedDate = DateTime.Now;
        //            _quotationSettings.ModifiedBy = predefinedLog.UserId;
        //            _autoleasingQuotationFormSettingsRepository.Update(_quotationSettings);
        //        }
        //        else
        //            _autoleasingQuotationFormSettingsRepository.Insert(autoleasingQuotationFormSettings);
        //        /* end  */

        //        output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
        //        output.ErrorDescription = "Success";
        //        output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
        //        //predefinedLog.ServiceResponse = "Success";
        //        predefinedLog.ErrorCode = (int)output.ErrorCode;
        //        predefinedLog.ErrorDescription = output.ErrorDescription;
        //        QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);

        //        return output;
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex.ToString();
        //        //predefinedLog.ServiceResponse = exception;
        //        predefinedLog.ErrorCode = (int)output.ErrorCode;
        //        predefinedLog.ErrorDescription = "Exception Error";
        //        QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
        //        return null;
        //    }
        //}

        public QuotationsFormOutput GetRenewalQuotaionsInfoByExternalId(QuotationRequestLog predefinedLog, QuotationFormWithSelectedBenfitsViewModel model, out string exception)
        {
            QuotationsFormOutput output = new QuotationsFormOutput();
            var langId = (model.lang.ToLower() == "en") ? 2 : 1;
            exception = string.Empty;

            try
            {
                QuotationInfoModel quotationInfo = _quotationService.GetQuotationsDetails(model.qtRqstExtrnlId, model.AgencyRepair, model.deductible, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = " db exception";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = exception;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (quotationInfo == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "quotationInfo is null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (quotationInfo.DepreciationSettingHistory == null && quotationInfo.DepreciationSetting == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.DepreciationSettingsNotFound;
                    output.ErrorDescription = AutoLeasingQuotationResources.DepreciationSettingsNotFound;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = $"DepreciationSetting return null for Bank Id: {quotationInfo.Bank.Id}, Maker: {quotationInfo.VehicleMakerCode}, Model: {quotationInfo.VehicleModelCode}";
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (quotationInfo.RepairMethodeSetting == null && quotationInfo.RepairMethodeSettingHistory == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.RepairMethodSettingsNotFoundForThisBank;
                    output.ErrorDescription = AutoLeasingQuotationResources.RepairMethodSettingsNotFoundForThisBank;
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = "Repair Method Settings Not Found For Bank Id : " + quotationInfo.Bank.Id;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }

                // as per jira (https://bcare.atlassian.net/browse/ALP-110)
                if (model.SelectedCompany > 0)
                    quotationInfo.Products = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).ToList();

                QuotationsFormTemplateViewModel data = new QuotationsFormTemplateViewModel();
                data.ExternalId = model.qtRqstExtrnlId;
                data.InsuredNationalId = quotationInfo.InsuredNationalId;
                data.DriverName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverNameEn : quotationInfo.MainDriverNameAr;
                data.DriverNationalId = quotationInfo.DriverNationalId;
                data.MainDriverAddress = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.MainDriverAddressEn : quotationInfo.MainDriverAddressAr;
                data.NCDFreeYears = (quotationInfo.NajmNcdFreeYears.HasValue) ? quotationInfo.NajmNcdFreeYears.Value.ToString() : "Not Eligible";
                data.VehicleId = quotationInfo.VehicleId;
                data.VehicleValue = quotationInfo.VehicleValue.Value.ToString();
                data.VehicleMaker = quotationInfo.VehicleMaker;
                data.VehicleModel = quotationInfo.VehicleModel;
                data.VehicleYear = quotationInfo.VehicleYear;
                data.Deductible = model.deductible.ToString();
                data.VehicleOwnerName = (langId == (int)LanguageTwoLetterIsoCode.En) ? quotationInfo.Bank.NameEn : quotationInfo.Bank.NameAr;

                AutoleasingAgencyRepair bankRepairMethodSettings = null;
                if (quotationInfo.RepairMethodeSettingHistory != null)
                {
                    var bankRepairMethodSettingsHistory = quotationInfo.RepairMethodeSettingHistory;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = bankRepairMethodSettingsHistory.BankId,
                        FirstYear = bankRepairMethodSettingsHistory.FirstYear,
                        SecondYear = bankRepairMethodSettingsHistory.SecondYear,
                        ThirdYear = bankRepairMethodSettingsHistory.ThirdYear,
                        FourthYear = bankRepairMethodSettingsHistory.FourthYear,
                        FifthYear = bankRepairMethodSettingsHistory.FifthYear,
                    };
                }
                else
                {
                    var _bankRepairMethodSettings = quotationInfo.RepairMethodeSetting;
                    bankRepairMethodSettings = new AutoleasingAgencyRepair()
                    {
                        BankId = _bankRepairMethodSettings.BankId,
                        FirstYear = _bankRepairMethodSettings.FirstYear,
                        SecondYear = _bankRepairMethodSettings.SecondYear,
                        ThirdYear = _bankRepairMethodSettings.ThirdYear,
                        FourthYear = _bankRepairMethodSettings.FourthYear,
                        FifthYear = _bankRepairMethodSettings.FifthYear,
                    };
                    bankRepairMethodSettings = _autoleasingRepairMethodRepository.TableNoTracking.Where(r => r.BankId == quotationInfo.Bank.Id).FirstOrDefault();
                }

                var contractDuration = quotationInfo.ContractDuration.Value;

                string repairMethodString = string.Empty;
                var policiesNumbers = 0;// _checkoutService.GetPoliciesCount(quotationInfo.DriverNationalId, quotationInfo.VehicleId, quotationInfo.Bank.Id);
                if (policiesNumbers == 0)
                    repairMethodString = bankRepairMethodSettings.FirstYear;
                else if (policiesNumbers == 1)
                    repairMethodString = bankRepairMethodSettings.SecondYear;
                else if (policiesNumbers == 2)
                    repairMethodString = bankRepairMethodSettings.ThirdYear;
                else if (policiesNumbers == 3)
                    repairMethodString = bankRepairMethodSettings.FourthYear;
                else if (policiesNumbers == 4)
                    repairMethodString = bankRepairMethodSettings.FifthYear;

                data.Quotationlist = new List<QuotationsFormTemplateQuoteViewModel>();
                List<string> selectedAdditionalBenfitsString = new List<string>();
                foreach (var product in quotationInfo.Products.Where(q => q.VehicleRepairType == repairMethodString))
                {
                    decimal basicPremium = 0;
                    var singleQuotation = new QuotationsFormTemplateQuoteViewModel();
                    singleQuotation.CompanyKey = product.CompanyKey;
                    //singleQuotation.ImageURL = product.CompanyKey;
                    singleQuotation.ProductName = "Comprehensive";
                    singleQuotation.DeductableValue = product.DeductableValue;

                    if (data.Quotationlist.Where(a => a.CompanyKey == singleQuotation.CompanyKey && a.DeductableValue == singleQuotation.DeductableValue).FirstOrDefault() != null)
                        continue;

                    var productPriceDetails = product.PriceDetails;
                    if (productPriceDetails == null)
                        continue;

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 7))
                    {
                     basicPremium = productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 7).PriceValue;
                        singleQuotation.TotalPremium = Math.Round(basicPremium, 2).ToString();
                        //singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";  // (quotationInfo.VehicleValue.Value / basicPremium).ToString();
                    }
                    else
                    {
                        singleQuotation.TotalPremium = "0.00";
                        //singleQuotation.InsurancePercentage = "0.00";
                    }

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 4))
                        singleQuotation.ClaimLoading = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 4).PriceValue, 2).ToString();
                    else
                        singleQuotation.ClaimLoading = "0.00";

                    //singleQuotation.MinimumPremium = product.PriceDetails.FirstOrDefault(a => a.PriceTypeCode == )?.PriceValue.ToString();

                    singleQuotation.InsurancePercentage = product.InsurancePercentage.ToString() ?? "0.00";
                    singleQuotation.ShadowAmount = product.ShadowAmount.ToString() ?? "0.00";
                    singleQuotation.MinimumPremium = "0.00";

                    //decimal _shadowAmount = 0;
                    //foreach (var price in productPriceDetails)
                    //{
                    //    if (price.PriceTypeCode == 1 || price.PriceTypeCode == 2 || price.PriceTypeCode == 3 || price.PriceTypeCode == 10 ||
                    //            price.PriceTypeCode == 11 || price.PriceTypeCode == 12) // Discounts
                    //        _shadowAmount += price.PriceValue;
                    //}

                    //if (_shadowAmount > 0)
                    //    singleQuotation.ShadowAmount = Math.Round(_shadowAmount, 2).ToString();
                    //else
                    //    singleQuotation.ShadowAmount = "0.00";

                    if (productPriceDetails.Any(a => a.PriceTypeCode == 3))
                        singleQuotation.LoyalityAmount = Math.Round(productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 3).PriceValue, 2).ToString();
                    else
                        singleQuotation.LoyalityAmount = "0.00";

                    List<short?> selectedBenfitsIds = new List<short?>();
                    List<string> selectedBenfitsExternalIds = new List<string>();
                    bool useBenefitExternalId = false;
                    if (model.SelectedBenfits != null)
                    {
                        var userBenefits = model.SelectedBenfits.Where(a => a.ProductId == product.ProductID).FirstOrDefault();
                        if (userBenefits != null)
                        {
                            selectedBenfitsIds = userBenefits.BenfitIds;
                            if (userBenefits.UseExternalId)
                            {
                                useBenefitExternalId = true;
                                selectedBenfitsExternalIds = userBenefits.BenfitExternalIds;
                            }
                        }
                    }

                    //if (quotationInfo.Bank.Id == 2) //alyusr
                    //{
                    //    var preSelectedBenefits = product.Benfits.Where(p => p.IsReadOnly && p.IsSelected.HasValue && p.IsSelected.Value);
                    //    if (useBenefitExternalId)
                    //    {
                    //        var clientSelectedBenefitsExternalId = selectedBenfitsExternalIds.Except(preSelectedBenefits.Select(b => b.BenefitExternalId));
                    //        var clientSelectedBenefitsExternal = product.Benfits.Where(b => clientSelectedBenefitsExternalId.Contains(b.BenefitExternalId));
                    //        if (clientSelectedBenefitsExternal != null && clientSelectedBenefitsExternal.Any())
                    //        {
                    //            singleQuotation.InsurancePercentage = Decimal.Round((product.ProductPrice + clientSelectedBenefitsExternal.Sum(b => b.BenefitPrice.Value)) * 100 / quotationInfo.VehicleValue.Value, 2).ToString();
                    //        }
                    //    }
                    //    else if (selectedBenfitsIds != null && selectedBenfitsIds.Any())
                    //    {
                    //        var clientSelectedBenefitsId = selectedBenfitsIds.Except(preSelectedBenefits.Select(b => b.BenefitId));
                    //        var clientSelectedBenefits = product.Benfits.Where(b => clientSelectedBenefitsId.Contains(b.BenefitId));
                    //        if (clientSelectedBenefits != null && clientSelectedBenefits.Any())
                    //        {
                    //            singleQuotation.InsurancePercentage = Decimal.Round((product.ProductPrice + clientSelectedBenefits.Sum(b => b.BenefitPrice.Value)) * 100 / quotationInfo.VehicleValue.Value, 2).ToString();
                    //        }
                    //    }

                    //}

                    singleQuotation.Benefits = new List<QuotationsFormTemplateQuoteBenfitViewModel>();
                    var productBenfits = product.Benfits;
                    decimal _selectedBenfitTotalPrice = 0;
                    decimal _selectedBenfitVat = 0;
                    if (productBenfits != null)
                    {
                       

                        foreach (var benfit in productBenfits)
                        {
                            if (benfit.IsReadOnly)
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    Bprice = benfit.BenefitPrice,

                                    IsChecked = true
                                });
                                if (benfit.BenefitPrice > 0)
                                {
                                    _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
                                    _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                }
                                continue;
                            }
                            else if (useBenefitExternalId && selectedBenfitsExternalIds.IndexOf(benfit.BenefitExternalId) != -1)
                            {
                                if (benfit.BenefitPrice.HasValue)
                                {
                                    _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
                                    _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                }

                                selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    Bprice = benfit.BenefitPrice,
                                    IsChecked = true
                                });
                            }
                            else if (selectedBenfitsIds != null && selectedBenfitsIds.IndexOf(benfit.BenefitId) != -1)
                            {
                                if (benfit.BenefitPrice.HasValue)
                                {
                                    _selectedBenfitVat += benfit.BenefitPrice.Value * (decimal).15;
                                    _selectedBenfitTotalPrice += benfit.BenefitPrice.Value;
                                }

                                selectedAdditionalBenfitsString.Add((langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr);
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    Bprice = benfit.BenefitPrice,
                                    IsChecked = true
                                });
                            }
                            else
                            {
                                singleQuotation.Benefits.Add(new QuotationsFormTemplateQuoteBenfitViewModel()
                                {
                                    BName = (langId == (int)LanguageTwoLetterIsoCode.En) ? benfit.BenefitNameEn : benfit.BenefitNameAr,
                                    Bprice = benfit.BenefitPrice,
                                    IsChecked = false
                                });
                            }
                        }

                        if (productPriceDetails.Any(a => a.PriceTypeCode == 8))
                            singleQuotation.VAT = Math.Round((_selectedBenfitVat + productPriceDetails.FirstOrDefault(a => a.PriceTypeCode == 8).PriceValue), 2).ToString();
                        else if (_selectedBenfitVat > 0)
                            singleQuotation.VAT = (Math.Round(_selectedBenfitVat, 2)).ToString();
                        else
                            singleQuotation.VAT = "0.00";

                        singleQuotation.Total = (Math.Round(product.ProductPrice + _selectedBenfitVat + _selectedBenfitTotalPrice, 2)).ToString();
                    }

                   decimal? otherTypeCodes = product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                  || p.PriceTypeCode == 6 || p.PriceTypeCode == 8 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);
                    var total = basicPremium + _selectedBenfitTotalPrice + _selectedBenfitVat + otherTypeCodes.Value;
                    singleQuotation.Total = (Math.Round(total, 2)).ToString();
                    singleQuotation.TotalBenfitPrice = _selectedBenfitTotalPrice;
                    singleQuotation.InsurancePercentage = Math.Round((total / quotationInfo.VehicleValue.Value) * 100, 2).ToString();
                    data.Quotationlist.Add(singleQuotation);
                }

                data.TotalQuotations = data.Quotationlist.Count;
                data.AdditionalBenfits = string.Join(", ", selectedAdditionalBenfitsString);

                #region Deprecation Data

                AutoleasingDepreciationSetting depreciationSetting = null;
                if (quotationInfo.DepreciationSettingHistory != null)
                {
                    var deprecationHistory = quotationInfo.DepreciationSettingHistory;
                    depreciationSetting = new AutoleasingDepreciationSetting()
                    {
                        BankId = quotationInfo.Bank.Id,
                        MakerCode = deprecationHistory.MakerCode,
                        ModelCode = deprecationHistory.ModelCode,
                        MakerName = deprecationHistory.MakerName,
                        ModelName = deprecationHistory.ModelName,
                        Percentage = deprecationHistory.Percentage,
                        IsDynamic = deprecationHistory.IsDynamic,
                        FirstYear = deprecationHistory.FirstYear,
                        SecondYear = deprecationHistory.SecondYear,
                        ThirdYear = deprecationHistory.ThirdYear,
                        FourthYear = deprecationHistory.FourthYear,
                        FifthYear = deprecationHistory.FifthYear,
                        AnnualDepreciationPercentage = deprecationHistory.AnnualDepreciationPercentage,
                    };
                }
                else
                {
                    var deprecationSetting = quotationInfo.DepreciationSetting;
                    depreciationSetting = new AutoleasingDepreciationSetting();
                    depreciationSetting.BankId = quotationInfo.Bank.Id;
                    depreciationSetting.MakerCode = deprecationSetting.MakerCode.Value;
                    depreciationSetting.ModelCode = deprecationSetting.ModelCode.Value;
                    depreciationSetting.MakerName = deprecationSetting.MakerName;
                    depreciationSetting.ModelName = deprecationSetting.ModelName;
                    depreciationSetting.Percentage = deprecationSetting.Percentage ?? 0;
                    depreciationSetting.IsDynamic = deprecationSetting.IsDynamic;
                    depreciationSetting.FirstYear = deprecationSetting.FirstYear ?? 0;
                    depreciationSetting.SecondYear = deprecationSetting.SecondYear ?? 0;
                    depreciationSetting.ThirdYear = deprecationSetting.ThirdYear ?? 0;
                    depreciationSetting.FourthYear = deprecationSetting.FourthYear ?? 0;
                    depreciationSetting.FifthYear = deprecationSetting.FifthYear ?? 0;
                    depreciationSetting.AnnualDepreciationPercentage = deprecationSetting.AnnualDepreciationPercentage;
                }

                data.AnnualDeprecationType = depreciationSetting.AnnualDepreciationPercentage;
                data.AnnualDeprecationPercentage = depreciationSetting.Percentage.ToString();

                List<string> annualPercentages = new List<string>();

                if (contractDuration >= 1)
                {
                    if (!depreciationSetting.FirstYear.Equals(null))
                    {
                        //data.FirstYear = depreciationSetting.FirstYear.ToString().Split('.')[0] + "%";
                        //annualPercentages.Add(depreciationSetting.FirstYear.ToString().Split('.')[0] + "%");
                        data.FirstYear = "0 %";
                        annualPercentages.Add("0 %");
                    }
                }

                if (contractDuration >= 2)
                {
                    if (!depreciationSetting.SecondYear.Equals(null))
                    {
                        data.SecondYear = depreciationSetting.SecondYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.SecondYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 3)
                {
                    if (!depreciationSetting.ThirdYear.Equals(null))
                    {
                        data.ThirdYear = depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 4)
                {
                    if (!depreciationSetting.FourthYear.Equals(null))
                    {
                        data.FourthYear = depreciationSetting.FourthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FourthYear.ToString().Split('.')[0] + " %");
                    }
                }

                if (contractDuration >= 5)
                {
                    if (!depreciationSetting.FifthYear.Equals(null))
                    {
                        data.FifthYear = depreciationSetting.FifthYear.ToString().Split('.')[0] + " %";
                        annualPercentages.Add(depreciationSetting.FifthYear.ToString().Split('.')[0] + " %");
                    }
                }

                data.IsDynamic = depreciationSetting.IsDynamic;
                data.Percentage = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Split('.')[0] + " %";


                var vehicleValue = quotationInfo.VehicleValue.Value;
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;
                List<Decimal?> depreciationValues = new List<Decimal?>();
                var currentVehicleValue = vehicleValue;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = currentVehicleValue;
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {
                        DepreciationValue1 = vehicleValue;

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue1 = vehicleValue;
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }
                List<PremiumReference> premiumReference = new List<PremiumReference>();
                List<string> repairMethodList = new List<string>();
                if (contractDuration >= 1)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FirstYear);
                }
                if (contractDuration >= 2)
                {
                    repairMethodList.Add(bankRepairMethodSettings.SecondYear);
                }
                if (contractDuration >= 3)
                {
                    repairMethodList.Add(bankRepairMethodSettings.ThirdYear);
                }
                if (contractDuration >= 4)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FourthYear);
                }
                if (contractDuration >= 5)
                {
                    repairMethodList.Add(bankRepairMethodSettings.FifthYear);
                }

                List<string> allRepairMethods = new List<string>();
                foreach (var item in repairMethodList)
                    allRepairMethods.Add(item.Substring(0, 1));

                if (allRepairMethods != null && allRepairMethods.Count > 0)
                    data.RepairType = string.Join(", ", allRepairMethods);

                List<QuotationProductInfoModel> products = new List<QuotationProductInfoModel>();

                int countAgency = repairMethodList.Where(r => r == "Agency").Count();
                int countWorkShop = repairMethodList.Where(r => r == "Workshop").Count();
                string RepairType = string.Empty;
                if (repairMethodList.Count() == countAgency)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Agency").ToList();
                    RepairType = "Agency";
                }
                else if (repairMethodList.Count() == countWorkShop)
                {
                    products = quotationInfo.Products.Where(q => q.VehicleRepairType == "Workshop").ToList();
                    RepairType = "Workshop";
                }
                else
                {
                    products = quotationInfo.Products;
                    RepairType = "Mixed";
                }

                Decimal? InsurancePercentageAgency = 0;
                Decimal? InsurancePercentageWorkshop = 0;

                foreach (var _product in products)
                {
                    //var product = quotation.Products.FirstOrDefault();
                    if (_product == null)
                        continue;
                    if (_product.PriceDetails == null)
                        continue;

                    //if (InsurancePercentageAgency == 0 && _product.VehicleRepairType == "Agency")
                    //{
                    //    InsurancePercentageAgency = _product.InsurancePercentage;
                    //}
                    //else if (InsurancePercentageWorkshop == 0 && _product.VehicleRepairType == "Workshop")
                    //{
                    //    InsurancePercentageWorkshop = _product.InsurancePercentage;
                    //}
                    decimal? basicPremium = _product.PriceDetails.Where(p => p.PriceTypeCode == 7)?.FirstOrDefault()?.PriceValue;

                    decimal? otherTypeCodes = _product.PriceDetails.Where(p => p.PriceTypeCode == 4 || p.PriceTypeCode == 5
                    || p.PriceTypeCode == 6 || p.PriceTypeCode == 9).Sum(p => p.PriceValue);

                    decimal? vat = _product.PriceDetails.Where(p => p.PriceTypeCode == 8)?.FirstOrDefault()?.PriceValue;
                    //var discounts = product.PriceDetails.Where(p => p.PriceTypeCode == 1 || p.PriceTypeCode == 2
                    //  || p.PriceTypeCode == 3 || p.PriceTypeCode == 10 || p.PriceTypeCode == 11 || p.PriceTypeCode == 12)?.Sum(p => p.PriceValue);

                    List<short?> selectedBenfitsIds = new List<short?>();
                    List<string> selectedBenfitsExternalIds = new List<string>();
                    bool useBenefitExternalId = false;
                    if (model.SelectedBenfits != null)
                    {
                        var userBenefits = model.SelectedBenfits.Where(a => a.ProductId == _product.ProductID).FirstOrDefault();
                        if (userBenefits != null)
                        {
                            selectedBenfitsIds = userBenefits.BenfitIds;
                            if (userBenefits.UseExternalId)
                            {
                                useBenefitExternalId = true;
                                selectedBenfitsExternalIds = userBenefits.BenfitExternalIds;
                            }
                        }
                    }

                    decimal? benefits = 0;
                    decimal? clientBenefitsAmount = 0;
                    decimal? clientBenefitsAmountWithVAT = 0;
                    if (selectedBenfitsIds != null && model.SelectedBenfits.Any())
                    {
                        if (useBenefitExternalId)
                        {
                            benefits = _product.Benfits.Where(b => !string.IsNullOrEmpty(b.BenefitExternalId) && selectedBenfitsExternalIds.Contains(b.BenefitExternalId))?.Sum(b => b.BenefitPrice.Value);
                            clientBenefitsAmount = benefits;
                            clientBenefitsAmountWithVAT = benefits * 1.15M;
                            //  benefits *= 1.15M;
                        }
                        else
                        {
                            benefits = _product.Benfits.Where(b => b.BenefitId.HasValue && selectedBenfitsIds.Contains(b.BenefitId.Value))?.Sum(b => b.BenefitPrice.Value);
                            clientBenefitsAmount = benefits;
                            clientBenefitsAmountWithVAT = benefits * 1.15M;
                            benefits *= 1.15M;
                        }
                    }

                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        //var MPAD = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MPAD")).FirstOrDefault();
                        //var MGAE = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MGAE")).FirstOrDefault();
                        //var MRCR = _product.Benfits.Where(a => a.BenefitExternalId.Contains("MRCR")).FirstOrDefault();

                        //short MPAD_Id = 0;
                        //short MGAE_Id = 0;
                        //short MRCR_Id = 0;

                        //if (MPAD != null)
                        //{
                        //    MPAD_Id = MPAD.BenefitId.Value;
                        //}

                        //if (MGAE != null)
                        //{
                        //    MGAE_Id = MGAE.BenefitId.Value;
                        //}

                        //if (MRCR != null)
                        //{
                        //    MRCR_Id = MRCR.BenefitId.Value;
                        //}

                        foreach (var benefit in _product.Benfits)
                        {
                            if (selectedBenfitsIds != null && selectedBenfitsIds.Any() && selectedBenfitsIds.Contains(benefit.BenefitId))
                            {
                                continue;
                            }
                            else if (selectedBenfitsExternalIds != null && selectedBenfitsExternalIds.Any() && selectedBenfitsExternalIds.Contains(benefit.BenefitExternalId))
                            {
                                continue;
                            }

                            if (benefit.IsReadOnly && benefit.IsSelected.HasValue && benefit.IsSelected.Value)
                            {
                                benefits += benefit.BenefitPrice.Value;
                            }

                            //if (benefit.BenefitId == 1
                            //    || benefit.BenefitId == 6
                            //    || benefit.BenefitId == 10)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MPAD_Id != 0 && benefit.BenefitId == MPAD_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MGAE_Id != 0 && benefit.BenefitId == MGAE_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                            //else if (MRCR_Id != 0 && benefit.BenefitId == MRCR_Id)
                            //{
                            //    benefits += benefit.BenefitPrice.Value;
                            //}
                        }

                    }
               
                    var otherCodesAndBenifits = otherTypeCodes + benefits;

                    var premium = basicPremium + vat + otherCodesAndBenifits;// - discounts;

                    //if (quotationInfo.Bank.Id == 2) //Yusr
                    //{
                    //    premiumReference.Add(
                    //    new PremiumReference
                    //    {
                    //        Premium = premium ?? 0,
                    //        ReferenceId = _product.ReferenceId,
                    //        BasicPremium = basicPremium ?? 0,
                    //        VehicleRepairType = _product.VehicleRepairType,
                    //        VAT = vat ?? 0,
                    //        //  InsurancePercentage = (_product.ProductPrice + clientBenefitsAmountWithVAT) * 100 / vehicleValue,
                    //        InsurancePercentage = _product.InsurancePercentage,
                    //        OtherCodesAndBenifits = otherCodesAndBenifits

                    //        // OtherCodesAndBenifits = otherTypeCodes + clientBenefitsAmount
                    //    });
                    //}
                    //else
                    //{
                    //    premiumReference.Add(
                    //    new PremiumReference
                    //    {
                    //        Premium = premium ?? 0,
                    //        ReferenceId = _product.ReferenceId,
                    //        BasicPremium = basicPremium ?? 0,
                    //        VehicleRepairType = _product.VehicleRepairType,
                    //        VAT = vat ?? 0,
                    //        InsurancePercentage = _product.InsurancePercentage,
                    //        OtherCodesAndBenifits = otherCodesAndBenifits
                    //    });
                    //}
                    premiumReference.Add(
                   new PremiumReference
                   {
                       Premium = premium ?? 0,
                       ReferenceId = _product.ReferenceId,
                       BasicPremium = basicPremium ?? 0,
                       VehicleRepairType = _product.VehicleRepairType,
                       VAT = vat ?? 0,
                       InsurancePercentage = (premium / vehicleValue) * 100,
                       OtherCodesAndBenifits = otherCodesAndBenifits
                   });
                }

                var lowestPremiumAgency = premiumReference.Where(p => p.VehicleRepairType == "Agency").OrderBy(p => p.InsurancePercentage).FirstOrDefault();
                var lowestPremiumWorkshop = premiumReference.Where(p => p.VehicleRepairType == "Workshop").OrderBy(p => p.InsurancePercentage).FirstOrDefault();
                var lowestPremiumAlYusr = premiumReference.Where(p => p.VehicleRepairType == bankRepairMethodSettings.FirstYear).OrderBy(p => p.Premium).FirstOrDefault();

                //Decimal? BasicPremium = lowestPremium.BasicPremium;

                AutoleasingMinimumPremium bankMinimumPremiumSettings = null;
                if (quotationInfo.MinimumPremiumSettingHistory != null)
                {
                    var minimumPremiumSettingHistory = quotationInfo.MinimumPremiumSettingHistory;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSettingHistory.FirstYear,
                        SecondYear = minimumPremiumSettingHistory.SecondYear,
                        ThirdYear = minimumPremiumSettingHistory.ThirdYear,
                        FourthYear = minimumPremiumSettingHistory.FourthYear,
                        FifthYear = minimumPremiumSettingHistory.FifthYear
                    };
                }
                else
                {
                    var minimumPremiumSetting = quotationInfo.MinimumPremiumSetting;
                    bankMinimumPremiumSettings = new AutoleasingMinimumPremium()
                    {
                        BankId = quotationInfo.Bank.Id,
                        FirstYear = minimumPremiumSetting.FirstYear,
                        SecondYear = minimumPremiumSetting.SecondYear,
                        ThirdYear = minimumPremiumSetting.ThirdYear,
                        FourthYear = minimumPremiumSetting.FourthYear,
                        FifthYear = minimumPremiumSetting.FifthYear
                    };
                }

                Decimal? InsurancePercentage1 = 0;
                Decimal? InsurancePercentage2 = 0;
                Decimal? InsurancePercentage3 = 0;
                Decimal? InsurancePercentage4 = 0;
                Decimal? InsurancePercentage5 = 0;

                Decimal? Premium1 = 0;
                Decimal? Premium2 = 0;
                Decimal? Premium3 = 0;
                Decimal? Premium4 = 0;
                Decimal? Premium5 = 0;
                if (contractDuration >= 1)
                {
                    data.RepairMethod1 = bankRepairMethodSettings.FirstYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage1 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium1 = lowestPremiumAlYusr.Premium;
                        if (Premium1 < bankMinimumPremiumSettings.FirstYear)
                        {
                            Premium1 = bankMinimumPremiumSettings.FirstYear;
                        }
                    }
                    else
                    {
                        if (bankRepairMethodSettings.FirstYear == "Agency")
                        {
                            InsurancePercentage1 = lowestPremiumAgency.InsurancePercentage;
                            Premium1 = lowestPremiumAgency.Premium;
                        }
                        else
                        {
                            InsurancePercentage1 = lowestPremiumWorkshop.InsurancePercentage;
                            Premium1 = lowestPremiumWorkshop.Premium;
                        }
                    }
                   
                    data.InsurancePercentage1 = Math.Round(InsurancePercentage1.Value, 2).ToString();
                    data.Premium1 = Math.Round(Premium1.Value, 2).ToString();
                    data.Deductible1 = data.Deductible;
                    data.MiuimumPremium1 = bankMinimumPremiumSettings.FirstYear.ToString().Replace(".00", "");
                }
                if (contractDuration >= 2)
                {
                    data.RepairMethod2 = bankRepairMethodSettings.SecondYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage2 = lowestPremiumAlYusr.InsurancePercentage;
                        Premium2 = (InsurancePercentage2 * DepreciationValue2) / 100;
                        if (Premium2 < bankMinimumPremiumSettings.SecondYear)
                        {
                            Premium2 = bankMinimumPremiumSettings.SecondYear;
                        }
                    }
                    else
                    {
                        if (bankRepairMethodSettings.SecondYear == "Agency")
                        {
                            InsurancePercentage2 = lowestPremiumAgency.InsurancePercentage;
                            Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                        }
                        else
                        {
                            InsurancePercentage2 = lowestPremiumWorkshop.InsurancePercentage;
                            Premium2 = (((InsurancePercentage2 * DepreciationValue2) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                        }
                    }
                   
                    data.InsurancePercentage2 = Math.Round(InsurancePercentage2.Value, 2).ToString();
                    data.Premium2 = Math.Round(Premium2.Value, 2).ToString();
                    data.Deductible2 = data.Deductible;
                    data.MiuimumPremium2 = bankMinimumPremiumSettings.SecondYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 3)
                {
                    data.RepairMethod3 = bankRepairMethodSettings.ThirdYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium3 = (InsurancePercentage3 * DepreciationValue3) / 100;
                        if (Premium3 < bankMinimumPremiumSettings.ThirdYear)
                        {
                            Premium3 = bankMinimumPremiumSettings.ThirdYear;
                        }
                    }
                    else
                    {
                        if (bankRepairMethodSettings.ThirdYear == "Agency")
                        {
                            InsurancePercentage3 = lowestPremiumAgency.InsurancePercentage;
                            Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                        }
                        else
                        {
                            InsurancePercentage3 = lowestPremiumWorkshop.InsurancePercentage;
                            Premium3 = (((InsurancePercentage3 * DepreciationValue3) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                        }
                    }
              
                    data.InsurancePercentage3 = Math.Round(InsurancePercentage3.Value, 2).ToString();
                    data.Premium3 = Math.Round(Premium3.Value, 2).ToString();
                    data.Deductible3 = data.Deductible;
                    data.MiuimumPremium3 = bankMinimumPremiumSettings.ThirdYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 4)
                {
                    data.RepairMethod4 = bankRepairMethodSettings.FourthYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium4 = (InsurancePercentage4 * DepreciationValue4) / 100;
                        if (Premium4 < bankMinimumPremiumSettings.FourthYear)
                        {
                            Premium4 = bankMinimumPremiumSettings.FourthYear;
                        }
                    }
                    else {
                        if (bankRepairMethodSettings.FourthYear == "Agency")
                        {
                            InsurancePercentage4 = lowestPremiumAgency.InsurancePercentage;
                            Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                        }
                        else
                        {
                            InsurancePercentage4 = lowestPremiumWorkshop.InsurancePercentage;
                            Premium4 = (((InsurancePercentage4 * DepreciationValue4) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                        }
                    }
                    data.InsurancePercentage4 = Math.Round(InsurancePercentage4.Value, 2).ToString();
                    data.Premium4 = Math.Round(Premium4.Value, 2).ToString();
                    data.Deductible4 = data.Deductible;
                    data.MiuimumPremium4 = bankMinimumPremiumSettings.FourthYear.ToString().Replace(".00", "");

                }
                if (contractDuration >= 5)
                {
                    data.RepairMethod5 = bankRepairMethodSettings.FifthYear;
                    if (quotationInfo.Bank.Id == 2) //Yusr
                    {
                        InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
                        Premium5 = (InsurancePercentage5 * DepreciationValue5) / 100;
                        if (Premium5 < bankMinimumPremiumSettings.FifthYear)
                        {
                            Premium5 = bankMinimumPremiumSettings.FifthYear;
                        }
                    }
                    else
                    {
                        if (bankRepairMethodSettings.FifthYear == "Agency")
                        {
                            InsurancePercentage5 = lowestPremiumAgency.InsurancePercentage;
                            Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumAgency.OtherCodesAndBenifits) * 1.15M;
                        }
                        else
                        {
                            InsurancePercentage5 = lowestPremiumWorkshop.InsurancePercentage;
                            Premium5 = (((InsurancePercentage5 * DepreciationValue5) / 100) + lowestPremiumWorkshop.OtherCodesAndBenifits) * 1.15M;
                        }
                    }
                    data.InsurancePercentage5 = Math.Round(InsurancePercentage5.Value, 2).ToString();
                    data.Premium5 = Math.Round(Premium5.Value, 2).ToString();
                    data.Deductible5 = data.Deductible;
                    data.MiuimumPremium5 = bankMinimumPremiumSettings.FifthYear.ToString().Replace(".00", "");

                }
                data.Total5YearsPremium = Math.Round((Premium1 ?? 0) + (Premium2 ?? 0) + (Premium3 ?? 0) + (Premium4 ?? 0) + (Premium5 ?? 0), 2).ToString();
                #endregion

                AutoleasingQuotationFormSettings autoleasingQuotationFormSettings = null;
                if (model.IsRenewal)
                {
                    var previousSettings = _autoleasingQuotationFormSettingsRepository.TableNoTracking.Where(a => a.VehicleId == quotationInfo.VehicleId).ToList();
                    if (previousSettings != null && previousSettings.Count > 0)
                    {
                        int index = 0;
                        for (int i = 0; i < previousSettings.Count; i++)
                        {
                            index = i + 1;
                            HandleQuotationFormDynamicValues(data, previousSettings[i], index);
                            if (index == previousSettings.Count)
                            {
                                autoleasingQuotationFormSettings = new AutoleasingQuotationFormSettings();
                                autoleasingQuotationFormSettings.VehicleId = quotationInfo.VehicleId;
                                autoleasingQuotationFormSettings.ExternalId = model.qtRqstExtrnlId;
                                autoleasingQuotationFormSettings.BankId = quotationInfo.Bank.Id;
                                autoleasingQuotationFormSettings.SelectedInsuranceCompany = model.SelectedCompany;
                                if (quotationInfo.Products.Any(a => a.InsuranceCompanyID == model.SelectedCompany))
                                    autoleasingQuotationFormSettings.SelectedInsuranceCompanyName = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).FirstOrDefault().CompanyKey;
                                autoleasingQuotationFormSettings.Depreciation = data.Percentage;
                                autoleasingQuotationFormSettings.Total5YearsPremium = data.Total5YearsPremium;
                                autoleasingQuotationFormSettings.InsurancePercentage = data.GetType().GetProperty("InsurancePercentage" + index).GetValue(data, null).ToString();
                                autoleasingQuotationFormSettings.Premium = data.GetType().GetProperty("Premium" + index).GetValue(data, null).ToString();
                                autoleasingQuotationFormSettings.RepairMethod = data.GetType().GetProperty("RepairMethod" + index).GetValue(data, null).ToString();
                                autoleasingQuotationFormSettings.Deductible = data.GetType().GetProperty("Deductible" + index).GetValue(data, null).ToString();
                                autoleasingQuotationFormSettings.MinimumPremium = data.GetType().GetProperty("MinimumPremium" + index).GetValue(data, null).ToString();
                                autoleasingQuotationFormSettings.UserId = predefinedLog.UserId;
                                autoleasingQuotationFormSettings.IsPurchased = false;
                                //autoleasingQuotationFormSettings.FilePath = 
                                autoleasingQuotationFormSettings.CreateDate = DateTime.Now;
                                autoleasingQuotationFormSettings.CreatedBy = predefinedLog.UserId;
                                //_autoleasingQuotationFormSettingsRepository.Insert(autoleasingQuotationFormSettings);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    autoleasingQuotationFormSettings = new AutoleasingQuotationFormSettings();
                    autoleasingQuotationFormSettings.VehicleId = quotationInfo.VehicleId;
                    autoleasingQuotationFormSettings.ExternalId = model.qtRqstExtrnlId;
                    autoleasingQuotationFormSettings.BankId = quotationInfo.Bank.Id;
                    autoleasingQuotationFormSettings.SelectedInsuranceCompany = model.SelectedCompany;
                    if (quotationInfo.Products.Any(a => a.InsuranceCompanyID == model.SelectedCompany))
                        autoleasingQuotationFormSettings.SelectedInsuranceCompanyName = quotationInfo.Products.Where(a => a.InsuranceCompanyID == model.SelectedCompany).FirstOrDefault().CompanyKey;
                    autoleasingQuotationFormSettings.Depreciation = data.Percentage;
                    autoleasingQuotationFormSettings.Total5YearsPremium = data.Total5YearsPremium;
                    autoleasingQuotationFormSettings.InsurancePercentage = data.GetType().GetProperty("InsurancePercentage" + 1).GetValue(data, null).ToString();

                    decimal Premium1Value = 0;
                    decimal Premium2Value = 0;
                    decimal Premium3Value = 0;
                    decimal Premium4Value = 0;
                    decimal Premium5Value = 0;
                    decimal.TryParse(data.GetType().GetProperty("Premium" + 1).GetValue(data, null)?.ToString(), out Premium1Value);
                    decimal.TryParse(data.GetType().GetProperty("Premium" + 2).GetValue(data, null)?.ToString(), out Premium2Value);
                    decimal.TryParse(data.GetType().GetProperty("Premium" + 3).GetValue(data, null)?.ToString(), out Premium3Value);
                    decimal.TryParse(data.GetType().GetProperty("Premium" + 4).GetValue(data, null)?.ToString(), out Premium4Value);
                    decimal.TryParse(data.GetType().GetProperty("Premium" + 5).GetValue(data, null)?.ToString(), out Premium5Value);
                    autoleasingQuotationFormSettings.Premium1 = Premium1Value;
                    autoleasingQuotationFormSettings.Premium2 = Premium2Value;
                    autoleasingQuotationFormSettings.Premium3 = Premium3Value;
                    autoleasingQuotationFormSettings.Premium4 = Premium4Value;
                    autoleasingQuotationFormSettings.Premium5 = Premium5Value;
                    autoleasingQuotationFormSettings.RepairMethod = data.GetType().GetProperty("RepairMethod" + 1).GetValue(data, null).ToString();
                    autoleasingQuotationFormSettings.Deductible = data.GetType().GetProperty("Deductible" + 1).GetValue(data, null).ToString();
                    autoleasingQuotationFormSettings.UserId = predefinedLog.UserId;
                    autoleasingQuotationFormSettings.IsPurchased = false;
                    autoleasingQuotationFormSettings.CreateDate = DateTime.Now;
                    autoleasingQuotationFormSettings.CreatedBy = predefinedLog.UserId;
                }

                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                //log.RetrievingMethod = "Generation";
                //log.ServiceURL = serviceURL;

                predefinedLog.ServerIP = Utilities.GetInternalServerIP();
                //log.CompanyID = quoteResponse.InsuranceCompanyId;
                if (string.IsNullOrEmpty(predefinedLog.Channel))
                    predefinedLog.Channel = Channel.autoleasing.ToString();

                string policyDetailsJsonString = JsonConvert.SerializeObject(data);
                AutoLeaseReportGenerationModel reportGenerationModel = new AutoLeaseReportGenerationModel
                {
                    ReportType = "IndividualQuotationsFormTemplate",
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                //log.ServiceRequest = reportGenerationModelAsJson;

                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                HttpResponseMessage response = client.PostAsync(serviceURL, httpContent).Result;
                DateTime dtAfterCalling = DateTime.Now;
                //log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = QuotationsFormOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();
                    predefinedLog.ErrorCode = (int)output.ErrorCode;
                    predefinedLog.ErrorDescription = output.ErrorDescription;
                    QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                    return output;
                }
                /* Save hits of download quotation form in db*/
                var autoleasingUser = _autoleasingUserService.GetUser(predefinedLog.UserId);
                var bank = _bankService.GetBank(autoleasingUser.BankId);
                string filePath = string.Empty;
                if (autoleasingUser != null && bank != null)
                {
                    AutoleasingQuotationForm form = new AutoleasingQuotationForm();
                    form.ExternalId = model.qtRqstExtrnlId;
                    form.CreatedDate = DateTime.Now;
                    form.BankId = autoleasingUser.BankId;
                    form.BankName = bank.NameEn;
                    form.UserId = predefinedLog.UserId;
                    form.UserEmail = autoleasingUser.Email;
                    form.FilePath = Utilities.SaveQuotationFormFile(model.qtRqstExtrnlId, JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString), bank.NameEn);
                    _autoleasingQuotationFormService.Insert(form);

                    filePath = form.FilePath;
                }
                /* end  */

                /* Save selected company which download quotation form in db*/

                if (model.SelectedCompany > 0)
                {
                    var oldCompanies = _autoleasingInitialQuotationCompaniesRepository.Table.Where(a => a.ExternalId == model.qtRqstExtrnlId).ToList();
                    if (oldCompanies != null && oldCompanies.Count > 0)
                        _autoleasingInitialQuotationCompaniesRepository.Delete(oldCompanies);

                    AutoleasingInitialQuotationCompanies companyQuote = new AutoleasingInitialQuotationCompanies();
                    companyQuote.ExternalId = model.qtRqstExtrnlId;
                    companyQuote.CompanyId = model.SelectedCompany;
                    companyQuote.BankId = autoleasingUser.BankId;
                    companyQuote.UserId = predefinedLog.UserId;
                    companyQuote.CreatedDate = DateTime.Now;
                    _autoleasingInitialQuotationCompaniesRepository.Insert(companyQuote);
                }

                /* end  */

                /* Save current year settings in AutoleasingQuotationFormSettings */
                //if (model.IsRenewal && autoleasingQuotationFormSettings != null)
                //{
                //    autoleasingQuotationFormSettings.FilePath = filePath;
                //    _autoleasingQuotationFormSettingsRepository.Insert(autoleasingQuotationFormSettings);
                //}
                if (autoleasingQuotationFormSettings != null)
                {
                    autoleasingQuotationFormSettings.FilePath = filePath;
                    _autoleasingQuotationFormSettingsRepository.Insert(autoleasingQuotationFormSettings);
                }
                /* end  */

                output.ErrorCode = QuotationsFormOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.File = JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResponseString);
                //predefinedLog.ServiceResponse = "Success";
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = output.ErrorDescription;
                QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);

                return output;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                //predefinedLog.ServiceResponse = exception;
                predefinedLog.ErrorCode = (int)output.ErrorCode;
                predefinedLog.ErrorDescription = "Exception Error";
                QuotationRequestLogDataAccess.AddQuotationRequestLog(predefinedLog);
                return null;
            }
        }

        private void HandleQuotationFormDynamicValues(QuotationsFormTemplateViewModel data, AutoleasingQuotationFormSettings previousSettings, int index)
        {
            if (!string.IsNullOrEmpty(previousSettings.InsurancePercentage) && previousSettings.InsurancePercentage != "0")
                data.GetType().GetProperty("InsurancePercentage" + index).SetValue(data, previousSettings.InsurancePercentage, null);

            if (!string.IsNullOrEmpty(previousSettings.Premium) && previousSettings.Premium != "0")
                data.GetType().GetProperty("Premium" + index).SetValue(data, previousSettings.Premium, null);

            if (!string.IsNullOrEmpty(previousSettings.RepairMethod))
                data.GetType().GetProperty("RepairMethod" + index).SetValue(data, previousSettings.RepairMethod, null);

            if (!string.IsNullOrEmpty(previousSettings.Deductible) && previousSettings.Deductible != "0")
                data.GetType().GetProperty("Deductible" + index).SetValue(data, previousSettings.Deductible, null);

            if (!string.IsNullOrEmpty(previousSettings.MinimumPremium) && previousSettings.MinimumPremium != "0")
                data.GetType().GetProperty("MiuimumPremium" + index).SetValue(data, previousSettings.MinimumPremium, null);
        }

        private RenewalOldDepreciationValuesModel HandleRenewalOldDepreciationValues(string externalId, decimal vehicleValue, AutoleasingDepreciationSetting depreciationSetting, int currentYear, out string exception)
        {
            exception = string.Empty;
            RenewalOldDepreciationValuesModel oldDepreciatopns = new RenewalOldDepreciationValuesModel();
            try
            {
                Decimal? DepreciationValue1 = 0;
                Decimal? DepreciationValue2 = 0;
                Decimal? DepreciationValue3 = 0;
                Decimal? DepreciationValue4 = 0;
                Decimal? DepreciationValue5 = 0;

                if (depreciationSetting.AnnualDepreciationPercentage == "Reducing Balance")
                {
                    DepreciationValue1 = vehicleValue;

                    if (depreciationSetting.IsDynamic)
                    {
                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue2 = DepreciationValue1 - (DepreciationValue1 * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = DepreciationValue2 - (DepreciationValue2 * depreciationSetting.Percentage / 100);
                        DepreciationValue4 = DepreciationValue3 - (DepreciationValue3 * depreciationSetting.Percentage / 100);
                        DepreciationValue5 = DepreciationValue4 - (DepreciationValue4 * depreciationSetting.Percentage / 100);
                    }
                }
                else if (depreciationSetting.AnnualDepreciationPercentage == "Straight Line")
                {
                    if (depreciationSetting.IsDynamic)
                    {

                        if (depreciationSetting.SecondYear != 0)
                            DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.SecondYear / 100);

                        if (depreciationSetting.ThirdYear != 0)
                            DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.ThirdYear / 100);

                        if (depreciationSetting.FourthYear != 0)
                            DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.FourthYear / 100);

                        if (depreciationSetting.FifthYear != 0)
                            DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.FifthYear / 100);
                    }
                    else
                    {
                        DepreciationValue2 = vehicleValue - (vehicleValue * depreciationSetting.Percentage / 100);
                        DepreciationValue3 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 2 / 100);
                        DepreciationValue4 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 3 / 100);
                        DepreciationValue5 = vehicleValue - (vehicleValue * depreciationSetting.Percentage * 4 / 100);
                    }
                }


                if (currentYear >= 1)
                    oldDepreciatopns.DepreciationValue1 = DepreciationValue1.Value;

                if (currentYear >= 2)
                    oldDepreciatopns.DepreciationValue2 = DepreciationValue2.Value;

                if (currentYear >= 3)
                    oldDepreciatopns.DepreciationValue3 = DepreciationValue3.Value;

                if (currentYear >= 4)
                    oldDepreciatopns.DepreciationValue4 = DepreciationValue4.Value;

                if (currentYear >= 5)
                    oldDepreciatopns.DepreciationValue5 = DepreciationValue5.Value;

                return oldDepreciatopns;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleRenewalVehicelValue_" + externalId + "_error.txt", JsonConvert.SerializeObject(ex.ToString()));
                return null;
            }
        }

        #endregion

        public QuotationRequestDetails GetQuotationRequestDetailsByExternalId(string externalId, out string exception)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestDetailsByExternalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(nationalIDParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                QuotationRequestDetails requests = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequestDetails>(reader).FirstOrDefault();
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
                dbContext.DatabaseInstance.Connection.Close();
                return requests;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public QuotationServiceRequest GetQuotationRequestMessage(QuotationRequestDetails quotationRequest, QuotationResponse quotationResponse, int insuranceTypeCode, bool vehicleAgencyRepair, string userId, int? deductibleValue, out string promotionProgramCode, out int promotionProgramId)
        {
            var serviceRequestMessage = new QuotationServiceRequest();
            promotionProgramCode = string.Empty;
            promotionProgramId = 0;
            //Random r = new Random();
            var cities = _addressService.GetAllCities();
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
            {                serviceRequestMessage.InsuredIdTypeCode = 3;                serviceRequestMessage.InsuredBirthDate = null;                serviceRequestMessage.InsuredBirthDateG = null;                serviceRequestMessage.InsuredBirthDateH = null;                serviceRequestMessage.InsuredGenderCode = null;                serviceRequestMessage.InsuredNationalityCode = null;                serviceRequestMessage.InsuredFirstNameEn = null;                serviceRequestMessage.InsuredMiddleNameEn = null;                serviceRequestMessage.InsuredLastNameEn = null;                serviceRequestMessage.InsuredFirstNameAr = quotationRequest.InsuredFirstNameAr; //Company Name
                serviceRequestMessage.InsuredMiddleNameAr = null;                serviceRequestMessage.InsuredLastNameAr = null;                serviceRequestMessage.InsuredSocialStatusCode = null;                serviceRequestMessage.InsuredEducationCode = null;                serviceRequestMessage.InsuredOccupation = null;                serviceRequestMessage.InsuredOccupationCode = null;                serviceRequestMessage.InsuredChildrenBelow16Years = null;                serviceRequestMessage.InsuredWorkCityCode = null;                serviceRequestMessage.InsuredWorkCity = null;                serviceRequestMessage.InsuredIdIssuePlaceCode = null;                serviceRequestMessage.InsuredIdIssuePlace = null;            }            else            {
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
                var info = _addressService.GetCityByName(cities, Utilities.RemoveWhiteSpaces(quotationRequest.RegisterationPlace));
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
            {                serviceRequestMessage.VehicleOwnerId = long.Parse(quotationRequest.NationalId);            }            else            {
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
                var makers = _vehicleService.VehicleMakers();
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
                var models = _vehicleService.VehicleModels(quotationRequest.VehicleMakerCode.Value);
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
                || ((insuranceTypeCode == 2 || insuranceTypeCode == 13) && quotationResponse.InsuranceCompanyId == Convert.ToInt32(InsuranceCompanyEnum.Allianz)))
            {
                serviceRequestMessage.HasTrailer = quotationRequest.HasTrailer;
                serviceRequestMessage.TrailerSumInsured = (quotationRequest.HasTrailer) ? quotationRequest.TrailerSumInsured : (int?)null;// quotationRequest.TrailerSumInsured;
                //serviceRequestMessage.HasTrailer = (quotationRequest.Vehicle.HasTrailer) ? quotationRequest.Vehicle.HasTrailer : (bool?)null;
                //serviceRequestMessage.TrailerSumInsured = (quotationRequest.Vehicle.HasTrailer) ? quotationRequest.Vehicle.TrailerSumInsured : (int?)null;

                serviceRequestMessage.OtherUses = (quotationRequest.OtherUses) ? quotationRequest.OtherUses : (bool?)null;
            }

            #endregion
            if (quotationRequest.NationalId.StartsWith("7"))            {                serviceRequestMessage.NCDFreeYears = 0;                serviceRequestMessage.NCDReference = "0";            }            else            {
                serviceRequestMessage.NCDFreeYears = quotationRequest.NajmNcdFreeYears.HasValue ? quotationRequest.NajmNcdFreeYears.Value : 0;
                serviceRequestMessage.NCDReference = quotationRequest.NajmNcdRefrence;
            }

            bool excludeDriversAbove18 = false;
            //if (insuranceTypeCode == 1 && InsuranceCompaniesThatExcludeDriversAbove18.Contains(quotationResponse.InsuranceCompanyId))
            //    excludeDriversAbove18 = true;

            serviceRequestMessage.Drivers = CreateDriversRequestMessage(quotationRequest, cities, quotationResponse.InsuranceCompanyId, excludeDriversAbove18);
            var programcode = _promotionService.GetUserPromotionCodeInfo(userId, quotationRequest.NationalId, quotationResponse.InsuranceCompanyId, insuranceTypeCode == 2 ? 2 : 1);
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
                    var address = _addressService.GetAddressesByNin(quotationRequest.NationalId);
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
                var vehicleModel = _vehicleService.GetVehicleModelByMakerCodeAndModelCode(makerId.Value, modelId.Value);
                if (vehicleModel != null)
                {
                    if (vehicleModel.WataniyaMakerCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleMakerCode = vehicleModel.WataniyaMakerCode.Value.ToString();
                    if (vehicleModel.WataniyaModelCode.HasValue)
                        serviceRequestMessage.WataniyaVehicleModelCode = vehicleModel.WataniyaModelCode.Value.ToString();
                }

                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText1))
                    serviceRequestMessage.WataniyaFirstPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.CarPlateText1);
                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText2))
                    serviceRequestMessage.WataniyaSecondPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.CarPlateText2);
                if (!string.IsNullOrEmpty(quotationRequest.CarPlateText3))
                    serviceRequestMessage.WataniyaThirdPlateLetterID = _vehicleService.GetWataiyaPlateLetterId(quotationRequest.CarPlateText3);
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

        private List<DriverDto> CreateDriversRequestMessage(QuotationRequestDetails quotationRequest, List<City> cities, int insuranceCompanyId, bool excludeDriversAbove18)
        {
            List<DriverDto> drivers = new List<DriverDto>();
            int additionalDrivingPercentage = 0;
            //Create main driver as first driver in the drivers list
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
            mainDriverDto.DriverHomeCityCode =!string.IsNullOrEmpty(quotationRequest.InsuredCityYakeenCode.ToString()) ? quotationRequest.InsuredCityYakeenCode.ToString() : "";
            mainDriverDto.DriverHomeCity = !string.IsNullOrEmpty(quotationRequest.InsuredCityArabicDescription) ? quotationRequest.InsuredCityArabicDescription: "";
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
                        _driverLicenseTypeCode = _quotationService.GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

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
            //                  .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == quotationRequest.MainDriverNin).ToList();            if (quotationRequest.MainDriverViolation != null && quotationRequest.MainDriverViolation.Count > 0)            {
                mainDriverDto.DriverViolations = quotationRequest.MainDriverViolation.Select(e => new ViolationDto()
                { ViolationCode = e.ViolationId }).ToList();            }
            //Add main driver to drivers list
            if (!quotationRequest.NationalId.StartsWith("7"))            {                if (insuranceCompanyId == 14)//Wataniya
                    HandleDriveAddressDetailsForWataniya(mainDriverDto);                if (excludeDriversAbove18 && (quotationRequest.AdditionalDrivers != null && quotationRequest.AdditionalDrivers.Count >= 1))
                    quotationRequest.AdditionalDrivers = HandleDriversAbove18Years(quotationRequest.AdditionalDrivers, mainDriverDto);
                drivers.Add(mainDriverDto);            }
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
                    if (quotationRequest.NationalId.StartsWith("7") && additionalDriver.NIN == additionalDrivers.ToList().FirstOrDefault().NIN)                    {                        driverDto.DriverTypeCode = 1;
                        driverDto.DriverNCDFreeYears = quotationRequest.NajmNcdFreeYears;
                        driverDto.DriverNCDReference = quotationRequest.NajmNcdRefrence;                    }                    else
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
                                _driverLicenseTypeCode = _quotationService.GetWataniyaDriverLicenseType(item.TypeDesc.ToString())?.WataniyaCode.Value;

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
                                     .Where(x => x.InsuredId == quotationRequest.InsuredId && x.NIN == additionalDriver.NIN);                    if (driverViolations != null && driverViolations.Count() > 0)                    {                        driverDto.DriverViolations = driverViolations                            .Select(e => new ViolationDto() { ViolationCode = e.ViolationId }).ToList();                    }

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

        public QuotationOutput GetQuotationResponseDetails(InsuranceCompany insuranceCompany, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, QuotationRequestLog log, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false, string policyNo = null, string policyExpiryDate = null, bool OdQuotation = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();

            QuotationOutput output = new QuotationOutput();
            DateTime startDateTime = DateTime.Now;
            string referenceId = string.Empty;
            referenceId = getNewReferenceId();
            log.RefrenceId = referenceId;
            DateTime beforeCallingDB = DateTime.Now;
            string exception = string.Empty;
            var quoteRequest = GetQuotationRequestDetailsByExternalId(qtRqstExtrnlId,out exception);
            log.DabaseResponseTimeInSeconds = DateTime.Now.Subtract(beforeCallingDB).TotalSeconds;
            if (!string.IsNullOrEmpty(exception))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "failed to get Quotation Request from DB due to:" + exception;
                return output;
            }
            if (quoteRequest == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest is null";
                return output;
            }
            if (string.IsNullOrEmpty(quoteRequest.NationalId))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Insured is null or empty";
                return output;
            }
            log.NIN = quoteRequest.NationalId;
            if (string.IsNullOrEmpty(quoteRequest.MainDriverNin))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Driver is null";
                return output;
            }
            predefinedLogInfo.DriverNin = quoteRequest.MainDriverNin;
            if (string.IsNullOrEmpty(quoteRequest.VehicleId.ToString()))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "quoteRequest.Vehicle is null ";
                return output;
            }
            if (insuranceCompany.InsuranceCompanyID == 8 &&
                quoteRequest.VehicleIdType == VehicleIdType.CustomCard
                && (quoteRequest.VehicleBodyCode == 1 || quoteRequest.VehicleBodyCode == 2
                || quoteRequest.VehicleBodyCode == 3 || quoteRequest.VehicleBodyCode == 19
                || quoteRequest.VehicleBodyCode == 20))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = "No supported product with medgulf with such information";
                output.LogDescription = "MedGulf Invalid Body Type with Custom Card body type is " + quoteRequest.VehicleBodyCode;
                return output;
            }

            if (quoteRequest.Cylinders >= 0 && quoteRequest.Cylinders <= 4)
            {
                quoteRequest.EngineSizeId = 1;
            }
            else if (quoteRequest.Cylinders >= 5 && quoteRequest.Cylinders <= 7)
            {
                quoteRequest.EngineSizeId = 2;
            }
            else
            {
                quoteRequest.EngineSizeId = 3;
            }

            if (quoteRequest.VehicleIdType == VehicleIdType.CustomCard)
                predefinedLogInfo.VehicleId = quoteRequest.CustomCardNumber;
            else
                predefinedLogInfo.VehicleId = quoteRequest.SequenceNumber;
            log.VehicleId = predefinedLogInfo.VehicleId;
            if (quoteRequest.NationalId.StartsWith("7") && !quoteRequest.OwnerTransfer &&
                (insuranceCompany.InsuranceCompanyID == 12 || insuranceCompany.InsuranceCompanyID == 14))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success as no Quote for 700 for Tawuniya and Wataniya";
                return output;
            }
            if (quoteRequest.NationalId.StartsWith("7") && insuranceCompany.InsuranceCompanyID == 25) //AXA
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
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
            if (insuranceCompany.InsuranceCompanyID == 21 && string.IsNullOrEmpty(requestMessage.PromoCode))
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "PromoCode is null for Saico ";
                return output;
            }
            if (insuranceCompany.InsuranceCompanyID == 6&& requestMessage.VehicleUseCode == 2)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.CommercialProductNotSupported;
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
                    if (OdQuotation)                    {                        requestMessage.PolicyNo = "new";                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(DateTime.UtcNow.AddYears(1).ToString());                    }                    else                    {                        requestMessage.PolicyNo = policyNo;                        requestMessage.PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(policyExpiryDate);                    }
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
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response is null due to errors, " + errors;
                return output;
            }
            if (response.Products == null)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                output.ErrorDescription = WebResources.SerivceIsCurrentlyDown;
                output.LogDescription = "response.Products is null due to errors, " + errors;
                return output;
            }
            if (response.Products.Count() == 0)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
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
                if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode))
                    product.IsPromoted = true;
                product.ProviderId = insuranceCompany.InsuranceCompanyID;
                if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                    product.InsuranceTypeCode = insuranceTypeCode;

                if (product.Product_Benefits != null)
                {
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
            if (insuranceTypeCode == 1 && insuranceCompany.InsuranceCompanyID != 14 && insuranceCompany.InsuranceCompanyID != 17 && insuranceCompany.InsuranceCompanyID != 9)
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

            if (quoteRequest.IsRenewal.HasValue && quoteRequest.IsRenewal.Value)
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

            output.ErrorCode = QuotationOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            output.LogDescription = "Success";
            return output;
        }

        public QuotationResponseModelForProfile GetVehicleInfo(string qtRqstExtrnlId, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool saveQuotResponseWithOldDate = false, string lang = "ar")
        {
           try
            {
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                    return null;
                var vehicleInfo = _quotationService.GetQuotationRequestAndVehicleInfo(qtRqstExtrnlId);
                if (vehicleInfo == null)
                {
                    return null;
                }
                var result = new QuotationResponseModelForProfile();
                result.QtRqstExtrnlId = vehicleInfo.ExternalId;
                result.IsRenewal = (vehicleInfo.IsRenewal.HasValue && vehicleInfo.IsRenewal.Value) ? true : false;
                result.RenewalReferenceId = (!string.IsNullOrEmpty(vehicleInfo.PreviousReferenceId)) ? vehicleInfo.PreviousReferenceId : null;
                result.Vehicle = new VehicleModelForProfile();

                result.Vehicle.Id = vehicleInfo.ID;
                result.Vehicle.Maker = vehicleInfo.VehicleMaker;
                result.Vehicle.MakerCode = vehicleInfo.VehicleMakerCode.HasValue ? vehicleInfo.VehicleMakerCode.Value : default(short);
                result.Vehicle.FormatedMakerCode = vehicleInfo.VehicleMakerCode.HasValue ? vehicleInfo.VehicleMakerCode.Value.ToString("0000") : default(string);
                result.Vehicle.Model = vehicleInfo.VehicleModel;
                result.Vehicle.ModelYear = vehicleInfo.ModelYear;
                result.Vehicle.PlateTypeCode = vehicleInfo.PlateTypeCode;
                if (vehicleInfo.PlateTypeCode.HasValue)
                    result.Vehicle.PlateColor = Tameenk.Core.Utilities.CarPlateUtils.GetCarPlateColorByCode((int)vehicleInfo.PlateTypeCode);
                if (vehicleInfo.VehicleIdTypeId == 2 && !string.IsNullOrEmpty(vehicleInfo.CustomCardNumber))
                    result.Vehicle.CustomCardNumber = vehicleInfo.CustomCardNumber;
                result.Vehicle.CarPlate = new Tameenk.Integration.Dto.Quotation.CarPlateInfo(vehicleInfo.CarPlateText1,
                vehicleInfo.CarPlateText2, vehicleInfo.CarPlateText3,
                vehicleInfo.CarPlateNumber.HasValue ? vehicleInfo.CarPlateNumber.Value : 0);

                var nCDFreeYearsInfo = _quotationService.GetNCDFreeYearsInfo(vehicleInfo.NajmNcdFreeYears.Value);
                if (nCDFreeYearsInfo != null)
                {
                    result.NCDFreeYearsEn = nCDFreeYearsInfo.EnglishDescription;
                    result.NCDFreeYearsAr = nCDFreeYearsInfo.ArabicDescription;
                    result.NCDFreeYears = lang.ToUpper().StartsWith("EN") ? nCDFreeYearsInfo.EnglishDescription : nCDFreeYearsInfo.ArabicDescription;
                }
                result.TypeOfInsurance = insuranceTypeCode;

                result.TypeOfInsuranceText = insuranceTypeCode == 1 ? GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo(lang)) : GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo(lang));
                if (insuranceTypeCode == 1)
                {
                    result.TypeOfInsuranceTextAr = GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo("Ar"));
                    result.TypeOfInsuranceTextEn = GenderResource.ResourceManager.GetString(GenderResource.Tpl_txt.ToString(), CultureInfo.GetCultureInfo("En"));
                }
                if (insuranceTypeCode == 2)
                {
                    result.TypeOfInsuranceTextAr = GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo("Ar"));
                    result.TypeOfInsuranceTextEn = GenderResource.ResourceManager.GetString(GenderResource.Comprehensive_txt.ToString(), CultureInfo.GetCultureInfo("En"));
                }
                result.DeductibleValue = deductibleValue;
                result.VehicleAgencyRepair = vehicleAgencyRepair;

                return result;
            }
            catch (Exception ex)
            {
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
    }
}
