using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos.Edaat;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.AutoleasingWallet;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Payments.Tabby;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Integration.Dto.Payment;
using Tameenk.Integration.Dto.Payment.Edaat;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Payment.Esal.Component;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.Payment;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Payments;
using Tameenk.Services.Implementation.Payments.Tabby;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.Orders;
using Tameenk.Services.Quotation.Components;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tamkeen.bll.Model;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;
using Image = System.Drawing.Image;

namespace Tameenk.Services.Checkout.Components
{
    public class CheckoutContext : ICheckoutContext
    {
        #region Fields


        private readonly IShoppingCartService _shoppingCartService;
        private readonly IQuotationService _quotationService;
        private readonly IOrderService _orderService;
        private readonly IDriverService _driverService;
        private readonly Random _rnd;
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly ISadadPaymentService _sadadPaymentService;

        private readonly IAuthorizationService _authorizationService;
        private readonly IRepository<TawuniyaProposal> _tawuniyaProposalRepository;
        private readonly INajmService najmService;
        private readonly IAddressService addressService;
        private readonly IHyperpayPaymentService hyperpayPaymentService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly ICheckoutsService _checkoutsService;

        private readonly IQuotationContext _quotationContext;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<NajmAccidentResponse> _najmAccidentResponseRepository;
        private readonly int UserInsuranceNumberLimitPerYear = 5;
        private const string Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<PriceType> _priceTypeRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IBankService _bankService;
        private readonly IAutoleasingUserService _autoleasingUserService;
        private readonly IRepository<IbanHistory> _ibanHistory;
        private readonly string TestMode;

        private readonly IRepository<EdaatRequest> _edaatRepository;
        private readonly IRepository<EdaatResponse> _edaatResponseRepository;
        private readonly IRepository<CompanyBankAccounts> _companyBankAccountRepository;

        private readonly IRepository<AutoleasingQuotationResponseCache> _autoleasingQuotationResponseCache;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IRepository<CorporateAccount> _corporateAccountRepository;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;
        private readonly IRepository<CorporateWalletHistory> _corporateWalletHistoryRepository;
        private readonly IRepository<AutoleasingWalletHistory> _autoleasingWalletHistoryRepository;

        private readonly IRepository<RenewalDiscount> _renewalDiscountRepository;
        private readonly IRepository<PolicyModification> _policyModificationRepository;
        private readonly IRepository<OrderItemBenefit> _orderItemBenefitRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailrepository;
        private readonly IRepository<PolicyAdditionalBenefit> _policyAdditionalBenefitRepository;
        private readonly IRepository<Endorsment> _endormentRepository;
        private readonly IRepository<EndorsmentBenefit> _endormentBenefitRepository;
        private readonly IRepository<VehicleDiscounts> _vehicleDiscountsRepository;
        private readonly IRepository<AutoleasingQuotationFormSettings> _autoleasingQuotationFormSettingsRepository;
        private readonly IRepository<ProductType> _productTypeRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompany;

        private readonly ITabbyPaymentService _tabbyPaymentService;
        private readonly IRepository<TabbyRequest> _tabbyRequest;
        private readonly IRepository<TabbyRequestDetails> _tabbyRequestDetails;
        private readonly IRepository<TabbyResponse> _tabbyResponse;
        private readonly IRepository<TabbyResponseDetail> _tabbyResponseDetail;
        private readonly IRepository<ProductType> _productTypeRep;        private readonly IRepository<LeasingUser> _leasingUserRepository;
        private readonly IRepository<AutoleasingPortalLinkProcessingQueue> _autoleasingPortalLinkProcessingQueueQueueRepository;
        private readonly IRepository<Product_Benefit> _productBenefitRepository;        private readonly IRepository<Quotation_Product_Benefit> _quotationProductBenefitRepository;
        private readonly IRepository<AutoleasingRenewalPolicyStatistics> _autoleasingRenewalPolicyStatistics;
        private readonly IIVRService _iVRService;
        private readonly IYakeenClient _iYakeenClient;
        private readonly IRepository<CheckoutMobileVerification> _checkoutMobileVerificationRepository;

        private const string IVR_SHARED_KEY = "Tameenk_IVR_2022_SharedKey_@$";
        #endregion

        #region Constructor

        public CheckoutContext(IShoppingCartService shoppingCartService, IQuotationService quotationService,
                                IOrderService orderService, IDriverService driverService, IHttpClient httpClient, IInsuranceCompanyService insuranceCompanyService,
                                ISadadPaymentService sadadPaymentService,
                                TameenkConfig tameenkConfig,

                                IAuthorizationService authorizationService,
                                IRepository<TawuniyaProposal> tawuniyaProposalRepository,
                                INajmService najmService,
                                IAddressService addressService,
                                IHyperpayPaymentService hyperpayPaymentService
                                , IRepository<CheckoutDetail> checkoutDetailRepository
                                , INotificationService notificationService,
                                IRepository<Benefit> benefitRepository,
                                ICheckoutsService checkoutsService,
                                IQuotationContext quotationContext,
                                IRepository<Product> productRepository,
                                IRepository<EdaatRequest> edaatRepository,
                                IRepository<EdaatResponse> edaatResponseRepository,
                                IRepository<NajmAccidentResponse> najmAccidentResponseRepository,
                                IRepository<QuotationResponse> quotationResponseRepository, IRepository<PriceType> priceTypeRepository,
                                IPolicyProcessingService policyProcessingService, IBankService bankService, IAutoleasingUserService autoleasingUserService, IRepository<IbanHistory> ibanHistory,
                                IRepository<AutoleasingQuotationResponseCache> autoleasingQuotationResponseCache,
                                IRepository<QuotationRequest> quotationRequestRepository,
                                IRepository<CompanyBankAccounts> companyBankAccountRepository,
                                IPaymentMethodService paymentMethodService,
                                IRepository<CorporateAccount> corporateAccountRepository,
                                IRepository<CorporateUsers> corporateUsersRepository,
                                IRepository<CorporateWalletHistory> corporateWalletHistoryRepository,
                                IRepository<RenewalDiscount> renewalDiscountRepository,
                                IRepository<AutoleasingWalletHistory> autoleasingWalletHistoryRepository,
                                IRepository<PolicyModification> policyModificationRepository,
                                IRepository<OrderItemBenefit> orderItemBenefitRepository,
                                IRepository<OrderItem> orderItemRepository,
                                IRepository<CheckoutDetail> checkoutDetailrepository,
                                IRepository<PolicyAdditionalBenefit> policyAdditionalBenefitRepository,
                                IRepository<Endorsment> endormentRepository,
                                IRepository<EndorsmentBenefit> endormentBenefitRepository,
                                IRepository<VehicleDiscounts> vehicleDiscountsRepository,
                                IRepository<AutoleasingQuotationFormSettings> autoleasingQuotationFormSettingsRepository,
                                IRepository<ProductType> productTypeRepository,
                                IRepository<InsuranceCompany> insuranceCompany,
                                ITabbyPaymentService tabbyPaymentService,
                                IRepository<TabbyRequest> tabbyRequest,
                                IRepository<TabbyRequestDetails> tabbyRequestDetails,
                                IRepository<TabbyResponse> tabbyResponse,
                                IRepository<TabbyResponseDetail> tabbyResponseDetail,
                                IRepository<ProductType> productTypeRep,
                                IRepository<LeasingUser> leasingUserRepository,
                                IRepository<AutoleasingPortalLinkProcessingQueue> autoleasingPortalLinkProcessingQueueQueueRepository,
                                IRepository<Product_Benefit> productBenefitRepository,
                                IRepository<Quotation_Product_Benefit> quotationProductBenefitRepository,
                                IRepository<AutoleasingRenewalPolicyStatistics> autoleasingRenewalPolicyStatistics,
                                IIVRService iVRService, IYakeenClient iYakeenClient,
                                IRepository<CheckoutMobileVerification> checkoutMobileVerificationRepository)
        {

            _shoppingCartService = shoppingCartService;
            _quotationService = quotationService;
            _orderService = orderService;
            _driverService = driverService;
            _rnd = new Random(System.Environment.TickCount);
            _httpClient = httpClient;
            _config = tameenkConfig;
            _insuranceCompanyService = insuranceCompanyService;
            _sadadPaymentService = sadadPaymentService;

            _authorizationService = authorizationService;
            _tawuniyaProposalRepository = tawuniyaProposalRepository;
            this.najmService = najmService;
            this.addressService = addressService;
            this.hyperpayPaymentService = hyperpayPaymentService;
            _notificationService = notificationService;
            this._benefitRepository = benefitRepository;
            _checkoutsService = checkoutsService;
            _quotationContext = quotationContext;
            _productRepository = productRepository;
            _najmAccidentResponseRepository = najmAccidentResponseRepository;
            this._quotationResponseRepository = quotationResponseRepository;
            this._priceTypeRepository = priceTypeRepository;
            _policyProcessingService = policyProcessingService;
            _bankService = bankService;
            _autoleasingUserService = autoleasingUserService;
            _ibanHistory = ibanHistory;
            TestMode = Utilities.GetAppSetting("TestMode");
            this._edaatRepository = edaatRepository;
            this._edaatResponseRepository = edaatResponseRepository;
            _companyBankAccountRepository = companyBankAccountRepository;
            _autoleasingQuotationResponseCache = autoleasingQuotationResponseCache;
            _quotationRequestRepository = quotationRequestRepository;
            _paymentMethodService = paymentMethodService;
            _corporateAccountRepository = corporateAccountRepository;
            _corporateUsersRepository = corporateUsersRepository;
            _corporateWalletHistoryRepository = corporateWalletHistoryRepository;
            _renewalDiscountRepository = renewalDiscountRepository;
            _autoleasingWalletHistoryRepository = autoleasingWalletHistoryRepository;
            _policyModificationRepository = policyModificationRepository;
            _orderItemBenefitRepository = orderItemBenefitRepository;
            _orderItemRepository = orderItemRepository;
            _checkoutDetailrepository = checkoutDetailRepository;
            _policyAdditionalBenefitRepository = policyAdditionalBenefitRepository;
            _endormentRepository = endormentRepository;
            _endormentBenefitRepository = endormentBenefitRepository;
            _vehicleDiscountsRepository = vehicleDiscountsRepository;
            _autoleasingQuotationFormSettingsRepository = autoleasingQuotationFormSettingsRepository;
            _productTypeRepository = productTypeRepository;
            _insuranceCompany = insuranceCompany;
            _tabbyPaymentService = tabbyPaymentService;
            _tabbyRequest = tabbyRequest;
            _tabbyRequestDetails = tabbyRequestDetails;
            _tabbyResponse = tabbyResponse;
            _tabbyResponseDetail = tabbyResponseDetail;
            _productTypeRep = productTypeRep;
            _leasingUserRepository = leasingUserRepository;
            _autoleasingPortalLinkProcessingQueueQueueRepository = autoleasingPortalLinkProcessingQueueQueueRepository;
            _productBenefitRepository = productBenefitRepository;
            _quotationProductBenefitRepository = quotationProductBenefitRepository;
            _autoleasingRenewalPolicyStatistics = autoleasingRenewalPolicyStatistics;
            _iVRService = iVRService;
            _iYakeenClient = iYakeenClient;
            _checkoutMobileVerificationRepository = checkoutMobileVerificationRepository;
        }
        #endregion

        public CheckoutOutput CheckoutDetails(string ReferenceId, string QtRqstExtrnlId, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang, string productId, string selectedProductBenfitId, string hashed)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            log.ReferenceId = ReferenceId;
            log.MethodName = "CheckoutDetails";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"ReferenceId: {ReferenceId}, lang: {lang}, QtRqstExtrnlId: {QtRqstExtrnlId}, lang: {lang}, productId: {productId}, selectedProductBenfitId: {selectedProductBenfitId}, hashed: {hashed}";
            try
            {
                var userManager = _authorizationService.GetUser(log.UserId.ToString());
                log.UserName = userManager.Result?.UserName;
                if (userManager == null || userManager.Result == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userManager is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (userManager.Result.LockoutEnabled && userManager.Result.LockoutEndDateUtc >= DateTime.UtcNow)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserLockedOut;
                    output.ErrorDescription = CheckoutResources.AccountLocked;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is locked";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.IsUserPhoneConfirmed = userManager.Result.PhoneNumberConfirmed;
                if (string.IsNullOrEmpty(ReferenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(QtRqstExtrnlId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QtsRqsExtrnlIds is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string clearText = ReferenceId + "_" + QtRqstExtrnlId + "_" + productId;
                if (string.IsNullOrEmpty(selectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;
                if (!SecurityUtilities.VerifyHashedData(hashed, clearText) && log.Channel.ToLower() == "portal")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched as we received clearText:" + clearText + " and hash is" + SecurityUtilities.HashData(clearText, null) + " and reciving hashed:" + hashed;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), ReferenceId);

                if (shoppingCartItem == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (shoppingCartItem.ProductId == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item product is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                log.CompanyId = shoppingCartItem.InsuranceCompanyID;
                log.CompanyName = shoppingCartItem.InsuranceCompanyKey;

                if (DateTime.Now.AddHours(-16) > shoppingCartItem.QuotationResponseCreateDateTime)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart quotation response is not within 16hr.";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                _quotationService.SetQuotationRequestUser(QtRqstExtrnlId, log.UserId.ToString());
                string exception = string.Empty;
                var quotationRequest = _quotationService.GetQuotationRequestAndDriversInfo(ReferenceId, QtRqstExtrnlId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null due to:" + exception;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quotationRequest == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quotationRequest.RequestPolicyEffectiveDate < DateTime.Now.AddHours(-16))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.Checkout_QuotationRequest_RequestPolicyEffectiveDateIsNotValid;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Request policy effective date is less than 16hr.";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(quotationRequest.DriverNIN))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "There is no driver in Quotation Request";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (quotationRequest.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    log.VehicleId = quotationRequest.CustomCardNumber;
                else
                    log.VehicleId = quotationRequest.SequenceNumber;

                if (!string.IsNullOrEmpty(quotationRequest.NationalId))
                {
                    log.DriverNin = quotationRequest.NationalId;
                }
                else
                {
                    log.DriverNin = quotationRequest.DriverNIN;
                }
                var mainDriverHasAddress = false;
                if (quotationRequest.AddressId.HasValue)
                {
                    mainDriverHasAddress = true;
                }
                string mainDriverCity = string.Empty;
                var mainDriverAddress = addressService.GetAddressesByNin(quotationRequest.DriverNIN);

                if (!mainDriverHasAddress)//try to get address using nin
                {
                    if (mainDriverAddress != null)
                    {
                        mainDriverCity = mainDriverAddress.City;
                        mainDriverHasAddress = true;
                    }
                }
                var checkoutModel = PrepareCheckoutModel(shoppingCartItem, shoppingCartItem.QuotationReferenceId, log.CompanyName, log.CompanyId.Value);

                bool showWalletPaymentOption = false;
                if (userManager.Result.IsCorporateUser)
                {
                    var corporateUser = _corporateUsersRepository.TableNoTracking.FirstOrDefault(u => u.UserId == userManager.Result.Id && u.IsActive);
                    if (corporateUser != null)
                    {
                        var corporateAccount = _corporateAccountRepository.TableNoTracking.FirstOrDefault(c => c.Id == corporateUser.CorporateAccountId && c.IsActive == true);

                        if (corporateAccount != null && corporateAccount.Balance.HasValue && corporateAccount.Balance >= Math.Round(checkoutModel.PaymentAmount, 2))
                        {
                            showWalletPaymentOption = true;
                        }
                    }
                }

                checkoutModel.ShowWalletPaymentOption = showWalletPaymentOption;

                checkoutModel.BankCodes = _orderService.GetBankCodes().Select(bc =>
                    new Tameenk.Core.Domain.Dtos.Lookup
                    {
                        Id = bc.Code.ToString(),
                        Name = lang == LanguageTwoLetterIsoCode.En ? bc.EnglishDescription : bc.ArabicDescription
                    }).ToList();
                checkoutModel.UserId = log.UserId.ToString();

                //Get checkout details for the latest pruchased policy by insured nin
                string exp = string.Empty;
                var lastPruchasedCheckoutDetails = _checkoutsService.GetLastPruchasedCheckoutDetailsByNIN(quotationRequest.DriverNIN, out exp);
                if (!string.IsNullOrEmpty(exp))
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"GetLastPruchasedCheckoutDetailsByNIN exception : {exp}";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                }

                if (lastPruchasedCheckoutDetails != null)
                {
                    checkoutModel.IBAN = lastPruchasedCheckoutDetails.IBAN;
                    checkoutModel.Email = lastPruchasedCheckoutDetails.Email;
                    checkoutModel.Phone = lastPruchasedCheckoutDetails.Phone;
                }

                checkoutModel.QtRqstExtrnlId = QtRqstExtrnlId;
                checkoutModel.ErrorValidatingUserData = !mainDriverHasAddress;
                if (shoppingCartItem.InsuranceCompanyKey == "Tawuniya" && shoppingCartItem.ProductInsuranceTypeCode == 7)
                {
                    checkoutModel.IsSanadPlus = true;
                }
                else if (shoppingCartItem.InsuranceCompanyKey == "Tawuniya" && shoppingCartItem.ProductInsuranceTypeCode.HasValue && shoppingCartItem.ProductInsuranceTypeCode == 2)
                {
                    checkoutModel.TypeOfInsurance = 2;// to not draw the images for tawuniya 
                }
                else if (shoppingCartItem.InsuranceCompanyKey == "Tawuniya" && (checkoutModel.TypeOfInsurance == 1 || shoppingCartItem.ProductInsuranceTypeCode == 1))
                {
                    checkoutModel.TypeOfInsurance = 1;
                    checkoutModel.IsSanadPlus = false;
                }
                checkoutModel.Hashed = hashed;
                checkoutModel.ProductId = productId;
                checkoutModel.SelectedProductBenfitId = selectedProductBenfitId;
                checkoutModel.VehicleIdType = quotationRequest.VehicleIdType;
                checkoutModel.Vehicle = new Tamkeen.bll.Model.VehicleModel();
                checkoutModel.Vehicle.Maker = quotationRequest.VehicleMaker;
                checkoutModel.Vehicle.MakerCode = quotationRequest.VehicleMakerCode.HasValue ? quotationRequest.VehicleMakerCode.Value : default(short);
                checkoutModel.Vehicle.Model = quotationRequest.VehicleModel;
                checkoutModel.Vehicle.ModelYear = quotationRequest.ModelYear;
                checkoutModel.Vehicle.PlateTypeCode = quotationRequest.PlateTypeCode;
                checkoutModel.Vehicle.PlateColor = GetCarPlateColorByCode((int?)quotationRequest.PlateTypeCode);
                checkoutModel.Vehicle.CarPlate = new Tamkeen.bll.Model.CarPlateInfo(quotationRequest.CarPlateText1,
                quotationRequest.CarPlateText2, quotationRequest.CarPlateText3,
                quotationRequest.CarPlateNumber.HasValue ? quotationRequest.CarPlateNumber.Value : 0);

                checkoutModel.Driver = new DriverData();
                checkoutModel.Driver.FirstName = quotationRequest.DriverFirstName;
                checkoutModel.Driver.SecondName = quotationRequest.DriverSecondName;
                checkoutModel.Driver.LastName = quotationRequest.DriverLastName;
                checkoutModel.Driver.EnglishFirstName = quotationRequest.DriverEnglishFirstName;
                checkoutModel.Driver.EnglishSecondName = quotationRequest.DriverEnglishSecondName;
                checkoutModel.Driver.EnglishLastName = quotationRequest.DriverEnglishLastName;
                checkoutModel.Driver.NIN = quotationRequest.DriverNIN;
                checkoutModel.EducationLevel = (lang == LanguageTwoLetterIsoCode.En) ? quotationRequest.EducationLevelEn : quotationRequest.EducationLevelAr;
                checkoutModel.MileageExpectedAnnualName = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<Mileage>().FirstOrDefault(a => a.Id == quotationRequest.MileageExpectedAnnualId).Name;
                checkoutModel.ParkingLocationName = Tameenk.Core.Domain.Enums.Extensions.GetAsKeyValuePair<ParkingLocation>().FirstOrDefault(a => a.Id == quotationRequest.ParkingLocationId).Name;

                if (mainDriverAddress!=null && mainDriverAddress.City!=null)
                {
                    checkoutModel.Driver.NationalAddress = mainDriverAddress.City;
                }
                else if (quotationRequest.InsuredCityId.HasValue)
                {
                    checkoutModel.Driver.NationalAddress = lang == LanguageTwoLetterIsoCode.En ? quotationRequest.CityEnglishDescription : quotationRequest.CityArabicDescription;
                }
                if (quotationRequest.RequestPolicyEffectiveDate.HasValue)
                {
                    checkoutModel.PolicyEffectiveDate = quotationRequest.RequestPolicyEffectiveDate;
                    checkoutModel.PolicyEndDate = quotationRequest.RequestPolicyEffectiveDate.Value.AddYears(1).AddDays(-1);
                }
                if (shoppingCartItem.ProductInsuranceTypeCode == 9)                {
                    exp = string.Empty;
                    var result = _checkoutsService.GetOLdTplPolicyData(quotationRequest.DriverNIN, quotationRequest.SequenceNumber, out exp);                    if (result != null && result.PolicyExpiryDate.HasValue && string.IsNullOrEmpty(exp))                    {
                        //log.ErrorCode = (int)output.ErrorCode;
                        //log.ErrorDescription = $"Cant Get Old Tpl Policy  : {exp}";
                        //dtAfterCalling = DateTime.Now;
                        //log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        //CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        //return output;
                        checkoutModel.PolicyEndDate = result.PolicyExpiryDate;                    }                }
                string insuredNIN = quotationRequest.NationalId;
                //if (insuredNIN.StartsWith("7"))
                //{
                //    checkoutModel.IsCompany = true;
                //    checkoutModel.Company = new CompanyModel();
                //    checkoutModel.Company.NIN = insuredNIN;
                //    checkoutModel.Company.CompanyNameAr = quotationRequest.InsuredFirstNameAr;
                //}
                checkoutModel.CanEditPhoneNumper = true;
                if (quotationRequest.NationalId.StartsWith("7") || quotationRequest.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                {
                    checkoutModel.IsCompany = true;
                    checkoutModel.Company = new CompanyModel();
                    checkoutModel.Company.NIN = quotationRequest.NationalId;
                    checkoutModel.Company.CompanyNameAr = quotationRequest.InsuredFirstNameAr;
                    if (quotationRequest.NajmNcdFreeYears == 4 || quotationRequest.NajmNcdFreeYears == 5)
                    {
                        exception = string.Empty;
                        var latestActivePolicy = _checkoutsService.GetUserActivePoliciesByNin(quotationRequest.DriverNIN, out exception);
                        if (!string.IsNullOrEmpty(exception))
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                            output.ErrorDescription = CheckoutResources.ErrorGenericException;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Error GetUserActivePoliciesByNin due to:" + exception;
                            dtAfterCalling = DateTime.Now;
                            log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                            return output;
                        }
                        if (latestActivePolicy != null)
                        {
                            checkoutModel.CanEditPhoneNumper = false;
                        }
                    }
                }

                if (checkoutModel.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    checkoutModel.SequenceNumber = quotationRequest.CustomCardNumber;
                else
                    checkoutModel.SequenceNumber = quotationRequest.SequenceNumber;

                var additionalDrivires = quotationRequest.AdditionalDriverList;
                if (additionalDrivires != null && additionalDrivires.Any())
                {
                    checkoutModel.FirstAdditionalDriverFirstNameAr = additionalDrivires[0].FirstName;
                    checkoutModel.FirstAdditionalDriverFirstNameEn = additionalDrivires[0].EnglishFirstName;

                    checkoutModel.FirstAdditionalDriverNameAr = additionalDrivires[0].FirstName + " " + additionalDrivires[0].SecondName + " " + additionalDrivires[0].LastName;
                    checkoutModel.FirstAdditionalDriverNameEn = additionalDrivires[0].EnglishFirstName + " " + additionalDrivires[0].EnglishSecondName + " " + additionalDrivires[0].LastName;


                    if (additionalDrivires.Count() > 1)
                    {
                        checkoutModel.SecondAdditionalDriverFirstNameAr = additionalDrivires[1].FirstName;
                        checkoutModel.SecondAdditionalDriverFirstNameEn = additionalDrivires[1].EnglishFirstName;
                        checkoutModel.SecondAdditionalDriverNameAr = additionalDrivires[1].FirstName + " " + additionalDrivires[1].SecondName + " " + additionalDrivires[1].LastName;
                        checkoutModel.SecondAdditionalDriverNameEn = additionalDrivires[1].EnglishFirstName + " " + additionalDrivires[1].EnglishSecondName + " " + additionalDrivires[1].EnglishLastName;
                    }
                }
               
                checkoutModel.ShowTabby = false;
                if (quotationRequest.IsRenewal.HasValue && quotationRequest.IsRenewal.Value && shoppingCartItem.InsuranceCompanyID != 13)
                {
                    checkoutModel.ShowTabby = true;
                }
                else
                {
                    if (shoppingCartItem.ProductInsuranceTypeCode == 1 && quotationRequest.ActiveTabbyTPL)
                        checkoutModel.ShowTabby = quotationRequest.ActiveTabbyTPL;
                    if (shoppingCartItem.ProductInsuranceTypeCode == 2 && quotationRequest.ActiveTabbyComp)
                        checkoutModel.ShowTabby = quotationRequest.ActiveTabbyComp;
                    if (shoppingCartItem.ProductInsuranceTypeCode == 7 && quotationRequest.ActiveTabbySanadPlus)
                        checkoutModel.ShowTabby = quotationRequest.ActiveTabbySanadPlus;
                    if (shoppingCartItem.ProductInsuranceTypeCode == 8 && quotationRequest.ActiveTabbyWafiSmart)
                        checkoutModel.ShowTabby = quotationRequest.ActiveTabbyWafiSmart;
                    if (shoppingCartItem.ProductInsuranceTypeCode == 13 && quotationRequest.ActiveTabbyMotorPlus)
                        checkoutModel.ShowTabby = quotationRequest.ActiveTabbyMotorPlus;
                }

                if (checkoutModel.TypeOfInsurance == 2 || checkoutModel.TypeOfInsurance == 9 || checkoutModel.TypeOfInsurance == 13)
                {
                    if (shoppingCartItem.VehicleLimitValue > 0)
                        checkoutModel.Vehicle.VehicleValue = shoppingCartItem.VehicleLimitValue;
                    else
                        checkoutModel.Vehicle.VehicleValue = quotationRequest.VehicleValue;

                    output.DeductibleValue = shoppingCartItem.DeductibleValue;
                    // if (quotationRequest.QuotationResponses != null)
                    {
                        output.VehicleAgencyRepair = quotationRequest.VehicleAgencyRepair;
                        if (!output.DeductibleValue.HasValue)// || output.DeductibleValue == 0)
                        {
                            output.DeductibleValue = quotationRequest.DeductibleValue;
                        }
                    }
                    var agencyRepairBenefit = checkoutModel.SelectedProduct?.Benefits?.Where(a => a.BenefitCode == 7).FirstOrDefault();
                    if (agencyRepairBenefit != null)
                    {
                        output.VehicleAgencyRepair = true;
                    }

                    if (checkoutModel.InsuranceCompanyKey == "AlRajhi" 
                        && (shoppingCartItem.ProductInsuranceTypeCode == 8 || (shoppingCartItem.ProductInsuranceTypeCode == 2 && checkoutModel.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard))) // Wafi Smart or (comp and custom)
                    {
                        checkoutModel.UsePhoneCamera = false;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = false;
                    }
                    else if (checkoutModel.InsuranceCompanyKey == "AlRajhi" &&
                        (quotationRequest.NajmNcdFreeYears == 3 || quotationRequest.NajmNcdFreeYears == 4 || quotationRequest.NajmNcdFreeYears == 5))
                    {
                        checkoutModel.UsePhoneCamera = true;
                        checkoutModel.ShowGallery = true;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = true;
                    }

                    //// for ArabianShild old business was depend on (NajmNcdFreeYears)
                    /// (quotationRequest.NajmNcdFreeYears == 3 || quotationRequest.NajmNcdFreeYears == 4 || quotationRequest.NajmNcdFreeYears == 5)
                    /// new business as in Jira (https://bcare.atlassian.net/browse/VW-881)
                    else if (shoppingCartItem.InsuranceCompanyKey == "ArabianShield")
                    {
                        if (quotationRequest.VehicleIdType == VehicleIdType.CustomCard && quotationRequest.ModelYear.HasValue && quotationRequest.ModelYear >= DateTime.Now.Year)
                        {
                            checkoutModel.ImageRequired = false;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = false;
                        }
                        else
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                    }

                    else if (checkoutModel.InsuranceCompanyKey == "AICC")
                    {
                        checkoutModel.UsePhoneCamera = true;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = true;
                        checkoutModel.ImageRequired = true;
                    }
                    else if (shoppingCartItem.InsuranceCompanyKey == "Wataniya")
                    {
                        if (quotationRequest.VehicleIdType == VehicleIdType.SequenceNumber)
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                        else
                        {
                            checkoutModel.ImageRequired = false;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = false;
                        }
                    }
                    else if (checkoutModel.InsuranceCompanyKey == "Tawuniya" || checkoutModel.InsuranceCompanyKey == "UCA")
                    {
                        checkoutModel.UsePhoneCamera = false;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = false;
                    }
                    else if (checkoutModel.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    {
                        if (checkoutModel.InsuranceCompanyKey == "TUIC" && checkoutModel.TypeOfInsurance == 13)
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                        else if (checkoutModel.InsuranceCompanyKey == "ACIG" && checkoutModel.TypeOfInsurance == 2)
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                        else if (checkoutModel.InsuranceCompanyKey == "TUIC" && (checkoutModel.Vehicle.ModelYear == null || checkoutModel.Vehicle.ModelYear < DateTime.Now.Year - 1))//AribanSheild or TUIC
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = false;
                            if (string.IsNullOrEmpty(TestMode) || TestMode.ToLower() == "false")
                            {
                                checkoutModel.PurchaseByMobile = true;
                            }
                            checkoutModel.ImageRequired = true;
                        }
                        else
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                    }
                    else if (shoppingCartItem.ProductInsuranceTypeCode == 13)
                    {
                        if (checkoutModel.InsuranceCompanyKey == "TUIC")
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = true;
                        }
                        else if (checkoutModel.InsuranceCompanyKey == "Allianz")
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                    }
                    else
                    {
                        if (shoppingCartItem.UsePhoneCamera)
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = false;
                            if (string.IsNullOrEmpty(TestMode) || TestMode.ToLower() == "false")
                            {
                                checkoutModel.PurchaseByMobile = true;
                            }
                            checkoutModel.ImageRequired = true;
                        }
                        else
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = true;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = true;
                        }
                    }
                    if (Utilities.IsMobileBrowser()
                        || log.UserId == "d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7"
                        || log.UserId == "9ee7c02f-c1d2-488c-abc9-64c3957dccec"
                        || log.UserId == "ccba903e-b2d4-4370-8689-3e199e23a085"
                        || log.UserId == "a7988f6b-5131-46e8-9e09-7ba76e8d56dc"
                        || log.UserId == "8650fce9-c7a3-4a19-9049-5260b543e750"
                        || log.UserId == "eb208f95-6b21-421c-be24-85f35ed017b5"
                        || log.UserId == "dd740d28-e459-4d26-9ae8-aaf184a362da")// as per mubark fal7 and other accounts open all comp product for all companies to mubarak.a@bcare.com.sa
                    {
                        checkoutModel.IsMobileBrowser = true;
                    }
                }

                if (checkoutModel.InsuranceCompanyKey == "Tawuniya" && checkoutModel.ProductInsuranceTypeCode == 7)
                    checkoutModel.BcareCommission = 8;
                else if (checkoutModel.InsuranceCompanyKey == "AlRajhi" && checkoutModel.ProductInsuranceTypeCode == 8)
                    checkoutModel.BcareCommission = 4;
                else if (checkoutModel.TypeOfInsurance == 9)
                    checkoutModel.BcareCommission = 15;
                else if (checkoutModel.TypeOfInsurance == 2)
                {
                    if (checkoutModel.InsuranceCompanyKey == "Solidarity" || checkoutModel.InsuranceCompanyKey == "GulfUnion" || checkoutModel.InsuranceCompanyKey == "Buruj" || checkoutModel.InsuranceCompanyKey == "Allianz")
                        checkoutModel.BcareCommission = null;
                    else if (checkoutModel.InsuranceCompanyKey == "AlRajhi" || checkoutModel.InsuranceCompanyKey == "Tawuniya")
                        checkoutModel.BcareCommission = 10;
                    else
                        checkoutModel.BcareCommission = 15;
                }
                else
                    checkoutModel.BcareCommission = 2;


                exception = string.Empty;                if (checkoutModel.TypeOfInsurance == 1)                {                    var odResponse = _quotationService.GetODResponseDetailsByExternalId(QtRqstExtrnlId, out exception);
                    if (odResponse != null)                    {                        var odProducts = _shoppingCartService.GetODProductDetailsByReferenceAndQuotaionNo(odResponse.ReferenceId, odResponse.ICQuoteReferenceNo);
                        if (odProducts != null && odProducts.Count > 0)                        {                            List<Product> products = new List<Product>();                            foreach (var odProduct in odProducts)
                            {
                                if (odProduct.Product_Benefits != null && odProduct.Product_Benefits.Count > 1)
                                    odProduct.Product_Benefits = odProduct.Product_Benefits.OrderByDescending(x => x.IsSelected).ToList();

                                products.Add(odProduct);
                            }                            odResponse.Products = products;
                            checkoutModel.ODQuotationDetails = odResponse.ToModel();                            var terms = _insuranceCompany.TableNoTracking.Where(a => a.InsuranceCompanyID == 22).Select(a => a.TermsAndConditionsFilePath).FirstOrDefault();
                            var termsAr = string.Empty;                            var termsEn = string.Empty; ;                            if (!string.IsNullOrEmpty(terms))                            {
                                termsAr = terms.Replace("_TPL", "_OD").Replace("_en", "_ar"); ;
                                termsEn = terms.Replace("_TPL", "_OD").Replace("_ar", "_en");                            }
                            foreach (var item in checkoutModel.ODQuotationDetails.Products)                            {                                item.TermsFilePathAr = termsAr;                                item.TermsFilePathEn = termsEn;                            }                        }                    }                }

                exception = string.Empty;
                output.CheckoutModel = checkoutModel;
                if (log.Channel == "2" || log.Channel.ToLower() == "mobile")
                {
                    HyperpayRequest hyperpayRequest = new HyperpayRequest();
                    hyperpayRequest.Amount = Math.Round(output.CheckoutModel.PaymentAmount, 2);
                    hyperpayRequest.UserId = log.UserId.ToString();
                    hyperpayRequest.CreatedDate = DateTime.Now;
                    hyperpayRequest.UserEmail = checkoutModel.Email;
                    hyperpayRequest.ReferenceId = ReferenceId;

                    hyperpayRequest = hyperpayPaymentService.RequestHyperpayUrl(hyperpayRequest, out exception);
                    output.HyperPayCheckoutId = hyperpayRequest.ResponseId;
                    output.HyperpayRequestId = hyperpayRequest.Id;
                }
                if (checkoutModel.InsuranceCompanyKey == "Buruj")
                {
                    output.CheckoutModel.TermsAndConditions = CheckoutResources.BurujTermsAndConditions;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = CheckoutResources.Success;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = JsonConvert.SerializeObject(output);
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                return output;
            }
            catch (TameenkEntityNotFoundException tameenkEx)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;// string.Format(CheckoutResources.Tameenk_ArgumentNullException_Quotation, QtRqstExtrnlId);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = tameenkEx.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {

                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }

        }

        public CheckoutOutput SubmitCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            log.ReferenceId = model.ReferenceId;
            log.MethodName = "SubmitCheckoutDetails";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.PaymentMethod = model.PaymentMethodCode?.ToString();
            try
            {
                var userManager = _authorizationService.GetUser(log.UserId.ToString());
                log.UserName = userManager.Result?.UserName;
                if (userManager == null || userManager.Result == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userManager is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (userManager.Result.LockoutEnabled && userManager.Result.LockoutEndDateUtc >= DateTime.UtcNow)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserLockedOut;
                    output.ErrorDescription = CheckoutResources.AccountLocked;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is locked";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                ////
                /// the ble condition to prevent purchasment from staging
                if (!string.IsNullOrEmpty(log.RequesterUrl) && log.RequesterUrl.Contains("staging") && (!log.UserName.Contains("bcare.com") && !log.UserName.Contains("neomtech.com")))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.AuthorizationFailed;
                    output.ErrorDescription = "Cannot Purchase as this is staging site not production, please go to (bcare.com.sa)";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User try to purchase from Staging";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout Model is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                string clearText = model.ReferenceId + "_" + model.QtRqstExtrnlId + "_" + model.ProductId;
                if (string.IsNullOrEmpty(model.SelectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += model.SelectedProductBenfitId + SecurityUtilities.HashKey;
                if (!SecurityUtilities.VerifyHashedData(model.Hashed, clearText) && log.Channel.ToLower() == "portal")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                Guid userId = Guid.Empty;
                Guid.TryParse(model.UserId, out userId);
                if (userId == Guid.Empty)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User ID is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Phone))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Phone number is empty";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IBAN == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IBAN.ToLower().Replace("sa", string.Empty).Trim().Length < 22)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN length is less than 22 char as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IBAN.ToLower().Replace("sa", string.Empty).Trim().Length > 22)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.InvalidIBANlength;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN length is more than 22 char as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!Regex.IsMatch(model.IBAN.ToLower().Replace("sa", string.Empty).Trim(), "^[0-9]*[A-Za-z]?[0-9]*$", RegexOptions.IgnoreCase))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid IBAN as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!Utilities.IsValidMail(model.Email))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = CheckoutResources.checkout_error_email;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "incorrect email format as email is " + model.Email;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!Utilities.IsValidPhoneNo(model.Phone))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPhone;
                    output.ErrorDescription = CheckoutResources.checkout_error_phone;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "incorrect phone format as phone is " + model.Phone;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!model.IBAN.ToLower().StartsWith("sa"))
                {
                    model.IBAN = "sa" + model.IBAN;
                    model.IBAN = model.IBAN.Replace(" ", "").Trim();
                }
                //check if payment method is active 
                var paymentMethod = _paymentMethodService.GetPaymentMethodsByCode(model.PaymentMethodCode.GetValueOrDefault());
                if (paymentMethod == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid Payment Methods " + model.PaymentMethodCode.GetValueOrDefault();
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (log.Channel.ToLower() == "ios" && paymentMethod.IosEnabled == false && log.UserId.ToString() != "06501157-46af-4639-a85e-f29060e21fce")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ios Invalid Payment Methods " + model.PaymentMethodCode.GetValueOrDefault();
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (log.Channel.ToLower() == "android" && paymentMethod.AndroidEnabled == false && log.UserId.ToString() != "06501157-46af-4639-a85e-f29060e21fce")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "android Invalid Payment Methods " + model.PaymentMethodCode.GetValueOrDefault();
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                // create checkout detail object.
                //var checkoutDetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(model.ReferenceId);
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ReferenceId);
                if (shoppingCartItem == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (shoppingCartItem.ProductId == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item product is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (log.Channel.ToLower() == "portal" && shoppingCartItem.ProductId.ToString().ToLower() != model.ProductId.ToLower())
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ConfilictProduct;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "conflict product as DB return " + shoppingCartItem.ProductId.ToString() + " and user select " + model.ProductId;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                log.CompanyId = shoppingCartItem.InsuranceCompanyID;
                log.CompanyName = shoppingCartItem.InsuranceCompanyKey;
                if (shoppingCartItem.InsuranceCompanyKey == "SAICO" && (shoppingCartItem.ShoppingCartItemBenefits == null || shoppingCartItem.ShoppingCartItemBenefits.Count() == 0))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ShoppingCartItemBenefitsIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ShoppingCartItemBenefits is null for " + shoppingCartItem.ProductId;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                QuotationResponseDBModel quoteResponse = null;
                quoteResponse = _quotationService.GetQuotationResponseByReferenceIdDB(model.ReferenceId, shoppingCartItem.ProductId.ToString());
                if (quoteResponse == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QuotationvResponse is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    log.VehicleId = quoteResponse.CustomCardNumber;
                else
                    log.VehicleId = quoteResponse.SequenceNumber;
                log.DriverNin = quoteResponse.InsuredNationalId;

                string exception = string.Empty;
                if (string.IsNullOrEmpty(quoteResponse.InsuredNationalId)&& (quoteResponse.NajmNcdFreeYears == 4 || quoteResponse.NajmNcdFreeYears == 5) && (quoteResponse.InsuredNationalId.StartsWith("7") || quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard))
                {
                    var latestActivePolicy = _checkoutsService.GetUserActivePoliciesByNin(quoteResponse.NIN, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = CheckoutResources.ErrorGenericException;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Error GetUserActivePoliciesByNin due to:" + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if(latestActivePolicy!=null&&Utilities.ValidatePhoneNumber(latestActivePolicy.Phone) != Utilities.ValidatePhoneNumber(model.Phone))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPhone;
                        output.ErrorDescription = CheckoutResources.checkout_error_phone;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "its not allowed to edit phone number as last active policy phone is:"+ Utilities.ValidatePhoneNumber(latestActivePolicy.Phone) + " and new entered one is:"+ Utilities.ValidatePhoneNumber(model.Phone);
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                short insuranceTypeCode = 0;
                if (quoteResponse.ProductInsuranceTypeCode.HasValue)
                {
                    short.TryParse(quoteResponse.ProductInsuranceTypeCode.Value.ToString(), out insuranceTypeCode);
                    quoteResponse.InsuranceTypeCode = insuranceTypeCode;
                }

                // adding od product to shopping cart item to get it when sending OTP message to include OD product price @ 10-1-2023
                if (quoteResponse.InsuranceTypeCode == 1 && model.ODDetails != null && !string.IsNullOrEmpty(model.ODDetails.ReferenceId) && !string.IsNullOrEmpty(model.ODDetails.ProductId))
                {
                    if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Edaat
                       || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Wallet
                       || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "invalid payment method wit OD" + model.PaymentMethodCode;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }                    exception = string.Empty;
                    _shoppingCartService.EmptyShoppingCart(log.UserId.ToString(), model.ODDetails.ReferenceId);
                    bool result = _shoppingCartService.AddItemToCart(log.UserId.ToString(), model.ODDetails.ReferenceId, Guid.Parse(model.ODDetails.ProductId), model.ODDetails.SelectedProductBenfitId?.Select(b => new Product_Benefit
                    {
                        Id = b,
                        IsSelected = true
                    }).ToList(), out exception);
                    if (!result || !string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToAddODItemToCart;
                        output.ErrorDescription = CheckoutResources.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed To AddItemToCart for OD Product for reference:" + model.ODDetails.ReferenceId + " due to:" + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                var driverInfo = ValidateRequest(userId.ToString(), model, log.Channel, quoteResponse,lang);
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.EmailAlreadyUsed)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmailAlreadyUsed;
                    output.ErrorDescription = CheckoutResources.EmailUsedWithAnotherDriver;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.PhoneAlreadyUsed)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneAlreadyUsed;
                    output.ErrorDescription = CheckoutResources.PhoneNoWithAnotherDriver;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver;
                    output.ErrorDescription = CheckoutResources.IBANUsedForOtherDriver;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.PhoneIsNotVerified)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneIsNotVerified;
                    output.ErrorDescription = CheckoutResources.CheckoutModelIsNullException;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear || driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.CompanyDriverExceedsInsuranceNumberLimitPerYear)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear;
                    output.ErrorDescription = CheckoutResources.UserExceedsInsuranceNumberLimitCheckoutPopup;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.IBANValidationError || driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.InvalidIBAN)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.IBANValidationError;
                    output.ErrorDescription = CheckoutResources.IBANValidationError;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invalid iban as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.EdaatNumberReachedToMaximum)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EdaatNumberReachedToMaximum;
                    output.ErrorDescription = CheckoutResources.EdaatNumberReachedToMaximum.Replace("{0}", driverInfo.FirstExpiryDateForEdaat);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.QuotationRequestExpired)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                //if (driverInfo.ErrorCode == CheckoutOutput.ErrorCodes.YakeenMobileVerificationError)
                //{
                //    output.ErrorCode = driverInfo.ErrorCode;
                //    output.ErrorDescription = driverInfo.ErrorDescription;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = driverInfo.LogDescription;
                //    dtAfterCalling = DateTime.Now;
                //    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                //    return output;
                //}
                if (driverInfo.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = driverInfo.ErrorCode;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = driverInfo.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                
                exception = string.Empty;
                bool isEmailVerified = false;
                if (model.Email == userManager.Result.Email && userManager.Result.EmailConfirmed)
                {
                    isEmailVerified = true;
                }
                else
                {
                    string returnedException = string.Empty;
                    int count = _checkoutsService.GetVerifiedEmailCheckoutDetail(quoteResponse.NIN, model.Email, out returnedException);
                    if (!string.IsNullOrEmpty(returnedException))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = CheckoutResources.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed to Get Verified Email Checkout Detail due to " + returnedException;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if (count > 0)
                    {
                        isEmailVerified = true;
                    }
                }

                if (!shoppingCartItem.ActiveTabbyTPL && !shoppingCartItem.ActiveTabbyComp
                   &&!shoppingCartItem.ActiveTabbySanadPlus && !shoppingCartItem.ActiveTabbyWafiSmart
                    && model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Taby is not valid for this product "+ insuranceTypeCode +" or Company:"+ quoteResponse.CompanyKey;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string discountCode = string.Empty;
                int discountType = 0;
                decimal? discountPercentage = 0;
                decimal? discountValue = 0;
                if (!string.IsNullOrEmpty(model.DiscountCode))
                {
                    var vehicleDiscountsInfo = _vehicleDiscountsRepository.Table.Where(a => (a.SequenceNumber == log.VehicleId || a.CustomCardNumber == log.VehicleId) && a.DiscountCode == model.DiscountCode).FirstOrDefault();
                    if (vehicleDiscountsInfo != null && vehicleDiscountsInfo.IsUsed.HasValue && !vehicleDiscountsInfo.IsUsed.Value)
                    {
                        var dateNow = DateTime.Now;
                        var renewalDiscount = _renewalDiscountRepository.TableNoTracking.Where(a => a.Code == model.DiscountCode && a.StartDate <= dateNow && a.EndDate >= dateNow).OrderByDescending(a => a.Id).FirstOrDefault();
                        if (renewalDiscount != null)
                        {
                            discountCode = renewalDiscount.Code;
                            discountType = renewalDiscount.DiscountType;
                            discountPercentage = renewalDiscount.Percentage;
                            discountValue = renewalDiscount.Amount;
                            vehicleDiscountsInfo.VehicleId = quoteResponse.VehicleId;
                            vehicleDiscountsInfo.ModifiedDate = DateTime.Now;
                            _vehicleDiscountsRepository.Update(vehicleDiscountsInfo);
                        }
                    }
                }
                // need to generic the blew discount
                ////
                /// 1- BCare discount 5% As per Mubarak 18-12-2022
                /// 2- https://bcare.atlassian.net/browse/VW-859
                else if ((quoteResponse.InsuranceTypeCode == 2 || quoteResponse.InsuranceTypeCode == 13) && DateTime.Now.Date >= new DateTime(2022, 12, 21))
                {
                    if (shoppingCartItem.ProductPrice >= 1350 && shoppingCartItem.ProductPrice <= 3999)
                    {
                        discountCode = "BCare_Comp_200SR_Discount";
                        discountType = 1; // 2 mean discount by value
                        discountValue = 200;
                    }
                    else
                    {
                        discountCode = "BCare_Comp_5%_Discount";
                        discountType = 2; // 2 mean discount by percentage
                        discountPercentage = 5;
                    }
                }
                int? oldPaymentMethodId = 0;
                decimal totalInvoiceAmount = 0;
                CheckoutDetail odCheckoutDetails = null;                string odReferenceId = null;                bool isrequestContainsOD = false;                if (model.ODDetails != null && !string.IsNullOrEmpty(model.ODDetails.ReferenceId) && !string.IsNullOrEmpty(model.ODDetails.ProductId))                {
                    isrequestContainsOD = true;
                    odReferenceId = model.ODDetails.ReferenceId;                    log.ServiceRequest = JsonConvert.SerializeObject(model.ODDetails);                    
                    var odShoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ODDetails.ReferenceId);
                    if (odShoppingCartItem == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ShoppingCartItemIsNull;
                        output.ErrorDescription = CheckoutResources.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "odShoppingCartItem is null for:" + model.ODDetails.ReferenceId;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    var odQuoteResponse = _quotationService.GetQuotationResponseByReferenceIdDB(model.ODDetails.ReferenceId, odShoppingCartItem.ProductId.ToString());
                    if (odQuoteResponse == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                        output.ErrorDescription = CheckoutResources.QuotationExpired;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "odQuoteResponse is null for:" + model.ODDetails.ReferenceId;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }

                    // create OD checkout details
                    odCheckoutDetails = AddChekoutDetails(model, model.ODDetails, true, odQuoteResponse, odShoppingCartItem, isEmailVerified, discountCode, discountPercentage, discountType, discountValue, log, lang);
                    if (odCheckoutDetails == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                        output.ErrorDescription = CheckoutResources.QuotationExpired;

                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "odCheckoutDetails is null for:" + model.ODDetails.ReferenceId;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }

                    // set OD response to be checkedout
                    _quotationService.UpdateQuotationResponseToBeCheckedout(odQuoteResponse.QuotationResponseId, odShoppingCartItem.ProductId);
                    // create OD invoice

                    var odInvoice = _orderService.GetInvoiceByRefrenceId(model.ODDetails.ReferenceId);
                    if (odInvoice != null)
                    {
                        // cancel invoice
                        if (!_orderService.DeleteInvoiceByRefrenceId(model.ODDetails.ReferenceId, log.UserId.ToString(), out exception))
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToDeleteOldInvoice;
                            output.ErrorDescription = CheckoutResources.FailedToDeleteOldInvoice;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Failed To Delete previous OD Invoice with InvoiceNo " + odInvoice.InvoiceNo + " and InvoiceId " + odInvoice.Id + " due to " + exception;
                            dtAfterCalling = DateTime.Now;
                            log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                            return output;
                        }
                        odInvoice = _orderService.CreateInvoice(model.ODDetails.ReferenceId, odQuoteResponse.InsuranceTypeCode.Value, odQuoteResponse.CompanyID, odInvoice.InvoiceNo);
                    }
                    else
                    {
                        odInvoice = _orderService.CreateInvoice(model.ODDetails.ReferenceId, odQuoteResponse.InsuranceTypeCode.Value, odQuoteResponse.CompanyID);
                    }
                    totalInvoiceAmount = odInvoice.TotalPrice.Value;
                }
                if (isrequestContainsOD && model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Taby is not valid for OD";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                CheckoutDetail checkoutDetails = AddChekoutDetails(model, model.ODDetails, false, quoteResponse, shoppingCartItem, isEmailVerified, discountCode, discountPercentage, discountType, discountValue, log, lang);
                oldPaymentMethodId = checkoutDetails.PaymentMethodId;
                _quotationService.UpdateQuotationResponseToBeCheckedout(quoteResponse.QuotationResponseId, shoppingCartItem.ProductId);

                if (quoteResponse.RequestPolicyEffectiveDate.HasValue
                  && DateTime.Now.Date >= quoteResponse.RequestPolicyEffectiveDate.Value.Date
                  && log.CompanyName == "SAICO")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.RequestPolicyEffectiveDateIsNotValidForSAICO;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "policy effective date is equal To Today Date For SAICO. as date for toay is " + DateTime.Now.Date + " and RequestPolicyEffectiveDate is " + quoteResponse.RequestPolicyEffectiveDate.Value.Date;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (shoppingCartItem.InsuranceCompanyKey == "SAICO" && checkoutDetails.OrderItems.FirstOrDefault().OrderItemBenefits == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ShoppingCartItemBenefitsIsNull;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkoutDetails.OrderItems.FirstOrDefault().OrderItemBenefits for " + shoppingCartItem.ProductId;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                /*
                  this code only for checkoutApi because hyperpayRequest inserted before create
                  checkout details so we should add CheckoutDetails to hyperpayRequest
                */
                #region checkoutApi
                if (model.HyperpayRequestId.HasValue && log.Channel.ToLower() != "ios" && log.Channel.ToLower() != "android")
                {
                    var hyperpayRequest = hyperpayPaymentService.GetById(model.HyperpayRequestId.Value);
                    if (hyperpayRequest.CheckoutDetails.Count == 0)
                    {
                        hyperpayRequest.CheckoutDetails.Add(checkoutDetails);
                        hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                    }
                }
                #endregion


                if (shoppingCartItem.IsAddressValidationEnabled)
                {
                    var mainDriverAddress = addressService.GetAllAddressesByNin(quoteResponse.NIN);
                    bool cityMatched = false;
                    foreach (var address in mainDriverAddress)
                    {
                        var city = addressService.GetCityCenterById(address.CityId.Trim());
                        if (city == null)
                            continue;
                        if (city.IsActive && city.ELM_Code == quoteResponse.InsuredCityId.ToString() && city.ELM_Code == quoteResponse.QuotationResponseCityId.ToString())
                        {
                            cityMatched = true;
                            break;
                        }
                    }
                    if (!cityMatched)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidCity;
                        output.ErrorDescription = CheckoutResources.InsuredCityDoesNotMatchAddressCity;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "The national address city does not match the driving city. as driverid is "
                            + quoteResponse.DriverId + " and quoteResponse.QuotationResponseCityId is "
                            + quoteResponse.QuotationResponseCityId.ToString()
                            + " and quoteResponse.InsuredCityId.ToString()" + quoteResponse.InsuredCityId.ToString();
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }
                // check if invoice already exists
                var invoice = _orderService.GetInvoiceByRefrenceId(model.ReferenceId);
                if (invoice != null)
                {
                    // cancel invoice
                    if (!_orderService.DeleteInvoiceByRefrenceId(model.ReferenceId, log.UserId.ToString(), out exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToDeleteOldInvoice;
                        output.ErrorDescription = CheckoutResources.FailedToDeleteOldInvoice;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed To Delete previous Invoice with InvoiceNo " + invoice.InvoiceNo + " and InvoiceId " + invoice.Id + " due to " + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    invoice = _orderService.CreateInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID, invoice.InvoiceNo, odReferenceId);
                }
                else
                {
                    invoice = _orderService.CreateInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID, 0, odReferenceId);
                }
                totalInvoiceAmount += invoice.TotalPrice.Value;
                if(invoice.TotalBCareDiscount.HasValue)
                    totalInvoiceAmount -= invoice.TotalBCareDiscount.Value;
                
                log.Amount = totalInvoiceAmount;

                ////
                ///tabby limit last update @30-3-2023 as per (https://bcare.atlassian.net/browse/VW-797)
                if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby 
                    &&(((quoteResponse.InsuranceTypeCode == 1) && totalInvoiceAmount > 5000) 
                    || ((quoteResponse.InsuranceTypeCode == 8 || quoteResponse.InsuranceTypeCode == 7||quoteResponse.InsuranceTypeCode==2)&&totalInvoiceAmount > 10000))
                    || ((quoteResponse.InsuranceTypeCode == 13) && totalInvoiceAmount > 10000))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invoice exceeded 10000 as comprehanssive and 5000 as TPL as its:" + totalInvoiceAmount + " can not use Taby";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                bool skipImageValidation = false;
                if ((quoteResponse.CompanyKey == "AlRajhi" || quoteResponse.CompanyKey == "ArabianShield") && (quoteResponse.NajmNcdFreeYears == 3 ||
                    quoteResponse.NajmNcdFreeYears == 4 || quoteResponse.NajmNcdFreeYears == 5))
                {
                    skipImageValidation = true;
                }
                if (quoteResponse.CompanyKey == "Tawuniya" || quoteResponse.CompanyKey == "UCA")
                {
                    skipImageValidation = true;
                }
                if (!skipImageValidation && quoteResponse.InsuranceTypeCode == 2)
                {
                    bool validateCustomCardImages = false;
                    if (quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    {
                        if ((quoteResponse.CompanyKey == "ACIG" || quoteResponse.CompanyKey == "TUIC" || quoteResponse.CompanyKey == "ArabianShield")//Arabian Shield && TUIC
                        && (quoteResponse.VehicleModelYear == null || quoteResponse.VehicleModelYear < DateTime.Now.Year - 1))//ACIG or AribanSheild or TUIC
                        {
                            validateCustomCardImages = true;
                        }
                    }

                    if ((validateCustomCardImages || quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber) && !checkoutDetails.ImageBackId.HasValue)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.MissingImages;
                        output.ErrorDescription = CheckoutResources.MissingCarImages;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Image Back is missing";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if ((validateCustomCardImages || quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber) && !checkoutDetails.ImageBodyId.HasValue)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.MissingImages;
                        output.ErrorDescription = CheckoutResources.MissingCarImages;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Image Body is missing";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if ((validateCustomCardImages || quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber) && !checkoutDetails.ImageFrontId.HasValue)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.MissingImages;
                        output.ErrorDescription = CheckoutResources.MissingCarImages;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Image Front Id is missing";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if ((validateCustomCardImages || quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber) && !checkoutDetails.ImageRightId.HasValue)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.MissingImages;
                        output.ErrorDescription = CheckoutResources.MissingCarImages;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Image Right Id is missing";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if ((validateCustomCardImages || quoteResponse.VehicleIdTypeId.Value == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber) && !checkoutDetails.ImageLeftId.HasValue)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.MissingImages;
                        output.ErrorDescription = CheckoutResources.MissingCarImages;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Image Left Id is missing";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }
                //refresh Cache
                exception = string.Empty;
                bool retVal = _quotationService.GetQuotationResponseCacheAndDelete(quoteResponse.CompanyID, quoteResponse.InsuranceTypeCode.Value, quoteResponse.ExternalId, out exception);
                if (!retVal && !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to delete from cache due to " + exception;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string customerName = string.Format("{0} {1} {2}", quoteResponse.EnglishFirstName, quoteResponse.EnglishSecondName, quoteResponse.EnglishLastName);
                if (string.IsNullOrWhiteSpace(quoteResponse.EnglishFirstName.Replace("-", string.Empty)) &&
                    string.IsNullOrWhiteSpace(quoteResponse.EnglishSecondName.Replace("-", string.Empty)) &&
                    string.IsNullOrWhiteSpace(quoteResponse.EnglishLastName.Replace("-", string.Empty)))
                {
                    customerName = "BCare";
                }
                var paymentRequestModel = new PaymentRequestModel()
                {
                    UserId = checkoutDetails.UserId,
                    ReferenceId = checkoutDetails.ReferenceId,
                    UserEmail = model.Email,
                    CustomerNameAr = string.Format("{0} {1} {2}", quoteResponse.FirstName, quoteResponse.SecondName, quoteResponse.LastName),
                    CustomerNameEn = customerName,
                    //PaymentAmount = (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0) ? totalInvoiceAmount - invoice.TotalBCareDiscount.Value : totalInvoiceAmount,
                    PaymentAmount = totalInvoiceAmount,
                    InvoiceNumber = invoice.InvoiceNo
                };
                paymentRequestModel.RequestId = "03-" + paymentRequestModel.InvoiceNumber.ToString() + "-" + _rnd.Next(111111, 999999);
                if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Hyperpay
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Mada
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.AMEX
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                {

                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Mada)
                        log.PaymentMethod = "Hyperpay-Mada";
                    else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.AMEX)
                        log.PaymentMethod = "Hyperpay-AMEX";
                    else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                    {
                        log.PaymentMethod = "ApplePay";
                        model.PaymentAmount = paymentRequestModel.PaymentAmount;
                    }
                    else
                        log.PaymentMethod = "Hyperpay";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    model.IsCheckoutEmailVerified = isEmailVerified;
                    output.IsCheckoutEmailVerified = isEmailVerified;
                    output.CheckoutModel = model;
                    if ((log.Channel.ToLower() == "ios" || log.Channel.ToLower() == "android") && model.PaymentMethodCode.GetValueOrDefault() != (int)PaymentMethodCode.ApplePay)
                    {
                        HyperpayRequest hyperpayRequest = new HyperpayRequest();
                        hyperpayRequest.Amount = Math.Round(paymentRequestModel.PaymentAmount, 2);
                        hyperpayRequest.UserId = log.UserId.ToString();
                        hyperpayRequest.CreatedDate = DateTime.Now;
                        hyperpayRequest.UserEmail = paymentRequestModel.UserEmail;
                        hyperpayRequest.ReferenceId = paymentRequestModel.ReferenceId;
                        var hyperpayOutput = hyperpayPaymentService.RequestHyperpayUrlWithSplitOption(hyperpayRequest, log.CompanyId.Value, log.CompanyName, log.Channel, checkoutDetails.MerchantTransactionId.Value, out exception);
                        if (hyperpayOutput.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                            output.ErrorDescription = CheckoutResources.InvalidPayment;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "RequestHyperpayUrlWithSplitOption return an error " + hyperpayOutput.ErrorDescription;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                            return output;
                        }
                        hyperpayRequest = hyperpayOutput.HyperpayRequest;
                        output.HyperPayCheckoutId = hyperpayRequest.ResponseId;
                        output.HyperpayRequestId = hyperpayRequest.Id;
                        hyperpayRequest = hyperpayPaymentService.GetById(output.HyperpayRequestId);
                        if (hyperpayRequest.CheckoutDetails == null || hyperpayRequest.CheckoutDetails.Count == 0)
                        {
                            hyperpayRequest.CheckoutDetails.Add(checkoutDetails);
                            hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                        }
                    }
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Edaat)
                {
                    log.PaymentMethod = "Edaat";
                    var edaatoutput = ExecuteEdaatPayment(log.Channel, checkoutDetails, invoice, model.QtRqstExtrnlId, quoteResponse);
                    if (edaatoutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                        output.ErrorDescription = CheckoutResources.Checkout_Sadadpayment_instruction_error;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "edaat return an error " + edaatoutput.ErrorDescription;
                        log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    else
                    {
                        LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                        LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;

                        var companyName = culture == LanguageTwoLetterIsoCode.Ar ? shoppingCartItem.InsuranceCompanyNameAR : shoppingCartItem.InsuranceCompanyNameEN;

                        var amount = Math.Round(paymentRequestModel.PaymentAmount, 2);
                        var message = string.Format(Tameenk.Resources.WebResources.WebResources.SadadSMSMessage,
                             companyName, amount, edaatoutput.EdaatPaymentResponseModel.InvoiceNo);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = checkoutDetails.Phone,
                            MessageBody = message,
                            Method = SMSMethod.SadadInvoice.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = log.Channel,
                            ReferenceId = model.ReferenceId
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);

                        output.EdaatPaymentResponseModel = edaatoutput.EdaatPaymentResponseModel;
                        output.EdaatPaymentResponseModel.PremiumAmount = amount.ToString();
                        output.EdaatPaymentResponseModel.CompanyName = companyName;
                        output.EdaatPaymentResponseModel.IsCheckoutEmailVerified = isEmailVerified;
                        output.EdaatPaymentResponseModel.CheckoutEmail = model.Email;
                        output.EdaatPaymentResponseModel.CheckoutReferenceId = model.ReferenceId;
                        output.EdaatPaymentResponseModel.ReferenceId = paymentRequestModel.ReferenceId;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        model.IsCheckoutEmailVerified = isEmailVerified;
                        output.IsCheckoutEmailVerified = isEmailVerified;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    output.CheckoutModel = model;
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                {
                    log.PaymentMethod = "Tabby";
                    var tabbyoutput = ExecuteTabbyPayment(log.Channel, checkoutDetails, invoice, model, quoteResponse);
                    if (tabbyoutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                        output.ErrorDescription = CheckoutResources.Checkout_Tabbbypayment_instruction_error1;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Tabby return an error " + tabbyoutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    else
                    {
                        output.tabbyResponseHandlerModel = new TabbyResponseHandler();
                        output.tabbyResponseHandlerModel.Status = tabbyoutput.tabbyResponseHandlerModel.Status;
                        output.tabbyResponseHandlerModel.TabbyUrl = tabbyoutput.tabbyResponseHandlerModel.ResponseBody.configuration.available_products.installments[0].web_url;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        model.IsCheckoutEmailVerified = isEmailVerified;
                        output.IsCheckoutEmailVerified = isEmailVerified;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    output.CheckoutModel = model;
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Wallet)
                {
                    if (!userManager.Result.IsCorporateUser)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The user {userId} is not Corporate";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }

                    var corporateUser = _corporateUsersRepository.TableNoTracking.FirstOrDefault(u => u.UserId == userManager.Result.Id && u.IsActive);
                    if (corporateUser == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The user {userId} doesn't exist in Corporate users";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }

                    var corporateAccount = _corporateAccountRepository.Table.FirstOrDefault(c => c.Id == corporateUser.CorporateAccountId && c.IsActive == true);
                    if (corporateAccount == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.AccountIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The corprate user {userId} doesn't have in Corporate Account";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }

                    var totalAmount = CalculateTotalPaymentAmount(shoppingCartItem);
                    var balance = corporateAccount.Balance.HasValue ? corporateAccount.Balance.Value : 0;
                    if (totalAmount > balance)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.ThereIsNotEnoughBalance;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The corprate user {userId}, corporate account {corporateUser.CorporateAccountId} doesn't have enough balance {balance}, amount to pay {totalAmount}";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }



                    CorporateWalletHistory corporateWalletHistory = new CorporateWalletHistory();
                    corporateWalletHistory.CorporateAccountId = corporateUser.CorporateAccountId;
                    corporateWalletHistory.ReferenceId = checkoutDetails.ReferenceId;
                    corporateWalletHistory.Amount = -Math.Round(paymentRequestModel.PaymentAmount, 2);
                    corporateWalletHistory.MethodName = "SubmitCheckoutDetails";
                    corporateWalletHistory.CreatedDate = DateTime.Now;
                    corporateWalletHistory.CreatedBy = corporateUser.UserName;

                    _corporateWalletHistoryRepository.Insert(corporateWalletHistory);

                    //Update Balance
                    corporateAccount.Balance -= Math.Round(paymentRequestModel.PaymentAmount, 2);
                    _corporateAccountRepository.Update(corporateAccount);


                    var checkoutDetailsToUpdate = _checkoutsService.GetCheckoutDetails(checkoutDetails.ReferenceId);
                    checkoutDetailsToUpdate.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                    checkoutDetailsToUpdate.CorporateAccountId = corporateUser.CorporateAccountId;
                    _orderService.UpdateCheckout(checkoutDetailsToUpdate);

                    _policyProcessingService.InsertPolicyProcessingQueue(model.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, checkoutDetails.InsuranceCompanyName, model.Channel);
                    var updateCreateOutput = WalletCreateOrder(checkoutDetails);
                    //if (updateCreateOutput.ErrorCode != WalletPaymentOutput.ErrorCodes.Success)
                    //{
                    //    output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToWalletUpdateOrder;
                    //    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    //    log.ErrorCode = (int)output.ErrorCode;
                    //    log.ErrorDescription = $"Failed To Update Order, {updateCreateOutput.ErrorDescription}";
                    //    dtAfterCalling = DateTime.Now;
                    //    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    //    return output;
                    //}
                    output.WalletPaymentResponseModel = new WalletPaymentResponseModel();
                    output.WalletPaymentResponseModel.ReferenceId = checkoutDetails.ReferenceId;
                    output.WalletPaymentResponseModel.Status = "Succeeded";
                    output.WalletPaymentResponseModel.ErrorMessage = "Succeeded";
                    output.WalletPaymentResponseModel.NewBalance = corporateAccount.Balance?.ToString();
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                    output.CheckoutModel = model;
                    return output;
                }
                else
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invalid payment method as we received  " + model.PaymentMethodCode;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }
        public CheckoutOutput AddItemToCart(Tameenk.Core.Domain.Dtos.AddItemToCartModel model, CheckoutRequestLog log, string lang)
        {
            DateTime dtStart = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            log.ReferenceId = model.ReferenceId;
            log.MethodName = "AddItemToCart";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                string exception = string.Empty;
                _shoppingCartService.EmptyShoppingCart(log.UserId.ToString(), model.ReferenceId);
                bool result = _shoppingCartService.AddItemToCart(log.UserId.ToString(), model.ReferenceId, Guid.Parse(model.ProductId), model.SelectedProductBenfitId?.Select(b => new Product_Benefit
                {
                    Id = b,
                    IsSelected = true
                }).ToList(), out exception);

                if (!result)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToAddItemToCart;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(lang));
                    output.qtRqstExtrnlId = model.QuotaionRequestExternalId;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = " _shoppingCartService.AddItemToCart is return:" + exception;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ReferenceId);
                // var quotationRequest = _quotationService.GetQuotationRequestDriversInfo(model.QuotaionRequestExternalId);
                exception = string.Empty;
                var quotationRequest = _quotationService.GetQuotationRequestAndDriversInfo(model.ReferenceId, model.QuotaionRequestExternalId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null due to:" + exception;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quotationRequest == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidRetrunObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotationRequest is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (shoppingCartItem == null || DateTime.Now.AddHours(-16) > shoppingCartItem.QuotationResponseCreateDateTime)
                {
                    output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    output.InvalidQoutation = true;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidQuotation;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("RefreshQuotation", new CultureInfo(model.lang));
                    output.qtRqstExtrnlId = model.QuotaionRequestExternalId;
                    output.TypeOfInsurance = quotationRequest.InsuranceTypeCode.GetValueOrDefault(1);
                    output.VehicleAgencyRepair = quotationRequest.VehicleAgencyRepair;
                    output.DeductibleValue = quotationRequest.DeductibleValue;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shoppingCartItem is null or expired";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string quotaionRequestExternalId = quotationRequest?.ExternalId;
                short typeOfInsurance = shoppingCartItem.InsuranceTypeCode.GetValueOrDefault(1);
                bool? vehicleAgencyRepair = shoppingCartItem.VehicleAgencyRepair;
                int? deductibleValue = shoppingCartItem.DeductibleValue;
                if (!string.IsNullOrEmpty(quotationRequest.NationalId))
                {
                    log.DriverNin = quotationRequest.NationalId;
                }
                else
                {
                    log.DriverNin = quotationRequest.DriverNIN;
                }
                log.VehicleId = quotationRequest.SequenceNumber != null ? quotationRequest.SequenceNumber : quotationRequest.CustomCardNumber;

                if (shoppingCartItem.InsuranceCompanyId == 12) //Tawuniya
                {
                    var selectedBenefits = shoppingCartItem.ShoppingCartItemBenefits?.Select(a => a.BenefitExternalId).ToList();
                    var quotationOutput = ValidateTawuniyaQuotation(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
                        shoppingCartItem.InsuranceCompanyId, log.Channel, new Guid(log.UserId), log.UserName, selectedBenefits);
                    if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("RefreshQuotation", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;

                        log.ErrorCode = (int)quotationOutput.ErrorCode;
                        log.ErrorDescription = "Tawuniya Quotation returned " + quotationOutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }
                if (shoppingCartItem.InsuranceCompanyId == 14 && typeOfInsurance == 2) //Wataniya
                {
                    var draftpolicyOutput = SendWataniyaDraftpolicy(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
                        shoppingCartItem.InsuranceCompanyId, Channel.Portal.ToString(), new Guid(log.UserId), log.UserName);
                    if (draftpolicyOutput.ErrorCode != PolicyOutput.ErrorCodes.Success)
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("WataniyaDraftPolicyError", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;

                        log.ErrorCode = (int)draftpolicyOutput.ErrorCode;
                        log.ErrorDescription = "Wataniya Draft policy returned " + draftpolicyOutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }
                if (shoppingCartItem.InsuranceCompanyId == 18 || shoppingCartItem.InsuranceCompanyId == 7
                    || shoppingCartItem.InsuranceCompanyId == 5) //Alalamiya or Wlaa or TUIC
                {
                    NumberOfAccidentOutput numberOfAccidentOutput = ValidateNumberOfAccident(log.UserId, log.UserName, shoppingCartItem, quotationRequest, log.Channel, model.SelectedProductBenfitId);
                    if (numberOfAccidentOutput.ErrorCode == NumberOfAccidentOutput.ErrorCodes.HaveAccidents ||
                        numberOfAccidentOutput.ErrorCode == NumberOfAccidentOutput.ErrorCodes.NotHaveAccidents)
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity; //we set this error code to keep same journy on app as it depend on this code to refrech quote;
                        if (numberOfAccidentOutput.ErrorCode == NumberOfAccidentOutput.ErrorCodes.HaveAccidents)
                        {
                            log.ErrorDescription = "Insured Have Accidents";
                            output.ErrorDescription = InsuranceProvidersResource.ResourceManager.GetString("UpdatePriceDueToAccident", new CultureInfo(model.lang));
                        }
                        else
                        {
                            log.ErrorDescription = "Insured Have No Accidents";
                            output.ErrorDescription = InsuranceProvidersResource.ResourceManager.GetString("NoAccidentMsg", new CultureInfo(model.lang));
                        }
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;
                        log.ErrorCode = (int)CheckoutOutput.ErrorCodes.HaveAccidents;

                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }
                    if (numberOfAccidentOutput.ErrorCode != NumberOfAccidentOutput.ErrorCodes.Success)
                    {
                        _shoppingCartService.EmptyShoppingCart(log.UserId.ToString(), model.ReferenceId);
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("NumberOfAccidentVlidationError", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;

                        log.ErrorCode = (int)numberOfAccidentOutput.ErrorCode;
                        log.ErrorDescription = "Validate Number Of Accident returned error: " + numberOfAccidentOutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if (numberOfAccidentOutput.ErrorCode == NumberOfAccidentOutput.ErrorCodes.Success && model.ReferenceId != numberOfAccidentOutput.NewReferenceId
                        && log.Channel.ToLower() != "portal")
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity; //we set this error code to keep same journy on app as it depend on this code to refrech quote;
                        log.ErrorDescription = "Insured Have No Accidents";
                        output.ErrorDescription = InsuranceProvidersResource.ResourceManager.GetString("NoAccidentMsg", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;
                        log.ErrorCode = (int)CheckoutOutput.ErrorCodes.HaveAccidents;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }
                    model.ReferenceId = numberOfAccidentOutput.NewReferenceId;
                    model.ProductId = numberOfAccidentOutput.newProductId;
                }
                string clearText = model.ReferenceId + "_" + model.QuotaionRequestExternalId + "_" + model.ProductId;
                string selectedProductBenfitId = string.Empty;
                if (model.SelectedProductBenfitId == null || model.SelectedProductBenfitId.Count() == 0)
                    clearText += SecurityUtilities.HashKey;
                else
                {
                    selectedProductBenfitId = string.Join(",", model.SelectedProductBenfitId);
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;
                }

                CheckoutModel checkoutModel = new CheckoutModel();
                if (typeOfInsurance == 2 || typeOfInsurance == 13)
                {
                    checkoutModel.TypeOfInsurance = typeOfInsurance;

                    if (shoppingCartItem.InsuranceCompanyKey == "AlRajhi" 
                        && (shoppingCartItem.ProductInsuranceTypeCode == 8 || (shoppingCartItem.ProductInsuranceTypeCode == 2 && quotationRequest.VehicleIdType == VehicleIdType.CustomCard))) // Wafi Smart or (comp and custom)
                    {
                        checkoutModel.UsePhoneCamera = false;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = false;
                    }
                    else if (shoppingCartItem.InsuranceCompanyKey == "AlRajhi" &&
                        (quotationRequest.NajmNcdFreeYears == 3 || quotationRequest.NajmNcdFreeYears == 4 || quotationRequest.NajmNcdFreeYears == 5))
                    {
                        checkoutModel.UsePhoneCamera = true;
                        checkoutModel.ShowGallery = true;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = true;
                    }

                    //// for ArabianShild old business was depend on (NajmNcdFreeYears)
                    /// (quotationRequest.NajmNcdFreeYears == 3 || quotationRequest.NajmNcdFreeYears == 4 || quotationRequest.NajmNcdFreeYears == 5)
                    /// new business as in Jira (https://bcare.atlassian.net/browse/VW-881)
                    else if (shoppingCartItem.InsuranceCompanyKey == "ArabianShield")
                    {
                        if (quotationRequest.VehicleIdType == VehicleIdType.CustomCard && quotationRequest.ModelYear.HasValue && quotationRequest.ModelYear >= DateTime.Now.Year)
                        {
                            checkoutModel.ImageRequired = false;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = false;
                        }
                        else
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                    }
                    else if (shoppingCartItem.InsuranceCompanyKey == "AICC")
                    {
                        checkoutModel.UsePhoneCamera = true;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = true;
                        checkoutModel.ImageRequired = true;
                    }


                    else if (shoppingCartItem.InsuranceCompanyKey == "Wataniya")
                    {
                        if (quotationRequest.VehicleIdType == VehicleIdType.SequenceNumber)
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                        else
                        {
                            checkoutModel.ImageRequired = false;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = false;
                        }
                    }
                    else if (shoppingCartItem.InsuranceCompanyKey == "Tawuniya" || checkoutModel.InsuranceCompanyKey == "UCA")
                    {
                        checkoutModel.UsePhoneCamera = false;
                        checkoutModel.ShowGallery = false;
                        checkoutModel.PurchaseByMobile = false;
                        checkoutModel.ImageRequired = false;
                    }
                    else if (quotationRequest.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    {
                        if (quotationRequest.CompanyKey == "TUIC" && typeOfInsurance == 13)
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                        else if (quotationRequest.CompanyKey == "ACIG" && typeOfInsurance == 2)
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                        else if (quotationRequest.CompanyKey == "TUIC" && (quotationRequest.ModelYear == null || quotationRequest.ModelYear < DateTime.Now.Year - 1))//AribanSheild or TUIC
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = false;
                            if (string.IsNullOrEmpty(TestMode) || TestMode.ToLower() == "false")
                            {
                                checkoutModel.PurchaseByMobile = true;
                            }
                            checkoutModel.ImageRequired = true;
                        }
                        else
                        {
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.ShowGallery = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = false;
                        }
                    }
                    else if (typeOfInsurance == 13)
                    {
                        if (shoppingCartItem.InsuranceCompanyKey == "TUIC")
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = false;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ShowGallery = true;
                        }
                        else if (shoppingCartItem.InsuranceCompanyKey == "Allianz")
                        {
                            checkoutModel.ImageRequired = true;
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.PurchaseByMobile = true;
                            checkoutModel.ShowGallery = false;
                        }
                    }
                    else
                    {
                        if (shoppingCartItem.UsePhoneCamera)
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = false;
                            if (string.IsNullOrEmpty(TestMode) || TestMode.ToLower() == "false")
                            {
                                checkoutModel.PurchaseByMobile = true;
                            }
                            checkoutModel.ImageRequired = true;
                        }
                        else
                        {
                            checkoutModel.UsePhoneCamera = true;
                            checkoutModel.ShowGallery = true;
                            checkoutModel.PurchaseByMobile = false;
                            checkoutModel.ImageRequired = true;
                        }
                    }
                    if (Utilities.IsMobileBrowser()
                        || log.UserId == "d21e49e3-4d56-4eb6-b9e0-f7e6c32540c7"
                        || log.UserId == "9ee7c02f-c1d2-488c-abc9-64c3957dccec"
                        || log.UserId == "ccba903e-b2d4-4370-8689-3e199e23a085"
                        || log.UserId == "a7988f6b-5131-46e8-9e09-7ba76e8d56dc"
                        || log.UserId == "8650fce9-c7a3-4a19-9049-5260b543e750"
                        || log.UserId == "eb208f95-6b21-421c-be24-85f35ed017b5"
                        || log.UserId == "dd740d28-e459-4d26-9ae8-aaf184a362da")// as per mubark fal7 and other accounts open all comp product for all companies to mubarak.a@bcare.com.sa
                    {
                        checkoutModel.IsMobileBrowser = true;
                    }
                    output.CheckoutModel = checkoutModel;
                }

                string hashed = SecurityUtilities.HashData(clearText, null);
                output._hValue = hashed;
                //output._c = clearText;

                output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.qtRqstExtrnlId = model.QuotaionRequestExternalId;
                output.ReferenceId = model.ReferenceId;
                output.ProductId = model.ProductId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                log.ResponseTimeInSeconds = output.ResponseTimeInSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public void SubmitMissingTransactions()
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.Channel = "PaymentByRetrialMechanism";
            log.MethodName = "HyperpayProcessPaymentByRetrialMechanism";
            log.ServerIP = Utilities.GetInternalServerIP();
            DateTime startDate = DateTime.Now.AddDays(-4);
            DateTime endDate = DateTime.Now.AddMinutes(-10);
            log.ServiceRequest = $"startDate: {startDate}, endDate: {endDate}";
            try
            {
                string exception = string.Empty;
                var requests = hyperpayPaymentService.GetAllMissingTransactionsFromHyperPayRequest(startDate, endDate, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorDescription = exception;
                    log.ErrorCode = 3;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return;
                }
                if (requests == null || requests.Count() == 0)
                {
                    log.ErrorDescription = "count is zero for for start date: " + startDate.ToString() + "; enddate: " + endDate;
                    log.ErrorCode = 2;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return;
                }
                foreach (HyperpayRequest request in requests)
                {
                    if (string.IsNullOrEmpty(request.ReferenceId))
                        continue;
                    Guid userId = Guid.Empty;
                    if (!string.IsNullOrEmpty(request.UserId))
                        Guid.TryParse(request.UserId, out userId);
                    var output = HyperpayProcessPaymentByRetrialMechanism(request.Id, request.ReferenceId, "PaymentRetrialMechanism", userId, (int)PaymentMethodCode.Hyperpay, request.MerchantTransactionId);
                    //if (output.PaymentMethodId == (int)PaymentMethodCode.ApplePay)
                    //{
                    //    var hyperPayUpdateOrderOutput = HyperpayUpdateOrder(output.CheckoutDetail, output.HyperpayResponse, output.PaymentId, output.PaymentSucceded);
                    //}
                }
                log.ErrorDescription = "Success and count is " + requests.Count();
                log.ErrorCode = 1;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return;
            }
            catch (Exception exp)
            {
                log.ErrorCode = 500;
                log.ErrorDescription = exp.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
            }
        }




        #region Private Methods
        private CheckoutModel PrepareCheckoutModel(ShoppingCartItemDB item, string referenceId, string insuranceCompanyKey, int insuranceCompanyId)
        {
            CheckoutModel result = new CheckoutModel();
            result.ReferenceId = referenceId;
            result.SelectedProduct = PrepareProductModel(item);
            result.InsuranceCompanyKey = insuranceCompanyKey;//_quotationService.GetQuotationResponseByReferenceId(referenceId).InsuranceCompany.Key;
            result.InsuranceCompanyId = insuranceCompanyId;// _quotationService.GetQuotationResponseByReferenceId(referenceId).InsuranceCompanyId;
            result.QuotationResponseId = item.QuotationResponseId;
            result.TypeOfInsurance = item.InsuranceTypeCode ?? 1;
            result.ProductInsuranceTypeCode = item.ProductInsuranceTypeCode.HasValue ? item.ProductInsuranceTypeCode.Value : result.TypeOfInsurance;

            result.PaymentAmount = _shoppingCartService.CalculateShoppingCartTotal(item);

            if ((result.ProductInsuranceTypeCode == 2 || result.ProductInsuranceTypeCode == 13) && DateTime.Now.Date >= new DateTime(2022, 12, 21))
            {
                decimal discount = (item.ProductPrice >= 1350 && item.ProductPrice <= 3999) ? 200 : Math.Round((item.ProductPrice * 5) / 100, 2);
                if (item.ProductPrice >= 1350 && item.ProductPrice <= 3999)
                {
                    result.BCareDiscount = new CheckoutBCareDiscountModel()
                    {
                        DiscountValue = discount,
                        DiscountType = 1
                    };
                }
                else
                {
                    result.BCareDiscount = new CheckoutBCareDiscountModel()
                    {
                        DiscountValue = discount,
                        DiscountType = 2
                    };
                }

                result.PaymentAmount -= discount;
            }

            return result;
        }

        private ProductModel PrepareProductModel(ShoppingCartItemDB shoppingCartItem)
        {
            var productModel = new ProductModel()
            {
                ProductNameAr = shoppingCartItem.ProductNameAr,
                ProductNameEn = shoppingCartItem.ProductNameEn
            };

            productModel.PriceDetails = new List<PriceModel>();
            // Get all price details without VAT
            DateTime start = new DateTime(2020, 9, 20, 0, 0, 0);
            DateTime end = new DateTime(2020, 9, 30, 23, 59, 59);
            if (shoppingCartItem.InsuranceCompanyKey == "UCA")
            {
                end = new DateTime(2020, 10, 5, 23, 59, 59);
            }
            foreach (var price in shoppingCartItem.PriceDetails.Where(pd => pd.PriceTypeCode != 8))
            {
                if (price.PriceValue <= 0)
                    continue;
                if (price.PriceTypeCode == 1 && DateTime.Now.Date <= new DateTime(2021, 09, 30))
                {
                    productModel.PriceDetails.Add(new PriceModel()
                    {
                        PriceNameAr = "خصم اليوم الوطني",
                        PriceNameEn = "National Day Discount",
                        PriceValue = price.PriceValue,
                        PriceTypeCode = int.Parse(price.PriceTypeCode.ToString())
                    });
                }
                else
                {
                    productModel.PriceDetails.Add(new PriceModel()
                    {
                        PriceNameAr = price.PriceTypeArabicDescription,
                        PriceNameEn = price.PriceTypeEnglishDescription,
                        PriceValue = price.PriceValue,
                        PriceTypeCode = int.Parse(price.PriceTypeCode.ToString())
                    });
                }
            }
            var vatPrice = shoppingCartItem.PriceDetails.FirstOrDefault(pd => pd.PriceTypeCode == 8);
            if (vatPrice != null)
            {
                productModel.PriceDetails.Add(new PriceModel()
                {
                    PriceNameAr = vatPrice.PriceTypeArabicDescription,
                    PriceNameEn = vatPrice.PriceTypeEnglishDescription,
                    PriceValue = _shoppingCartService.GetVatTotal(shoppingCartItem),
                    PriceTypeCode = 8
                });
            }

            productModel.Benefits = new List<BenefitModel>();
            foreach (var benefit in shoppingCartItem.ShoppingCartItemBenefits)
            {
                productModel.Benefits.Add(new BenefitModel()
                {
                    BenefitCode = benefit.BenefitCode,
                    BenefitId = benefit.BenefitId.HasValue ? benefit.BenefitId.Value.ToString() : "",
                    BenefitNameAr = benefit.BenefitArabicDescription,
                    BenefitNameEn = benefit.BenefitEnglishDescription,
                    BenefitDescAr = benefit.BenefitArabicDescription,
                    BenefitDescEn = benefit.BenefitEnglishDescription,
                    BenefitPrice = benefit.BenefitPrice,
                    ProductBenefitId = benefit.Id,
                    IsSelected = true
                });
            }
            return productModel;

        }

        byte[] GetFileByte(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        private CheckoutDetail AssignImagesToCheckOutAndAddWaterMark(CheckoutDetail checkoutDetails, CheckoutModel model)
        {
            Image Logo = Image.FromFile(@"C:\inetpub\wwwroot\Tameenk_Website\resources\imgs\app_logo.png", true);
            Bitmap bitmaplogo = (Bitmap)Logo;
            string date = DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("en-US"));
            string time = DateTime.Now.ToString("HH:mm:ss", new CultureInfo("en-US"));
            if (model.ImageBack != null)
            {
                var imageBack = new CheckoutCarImage() { ImageData = Utilities.AddWaterMarkOverImage(model.ImageBack, date, bitmaplogo, time) };
                checkoutDetails.ImageBack = imageBack;
                checkoutDetails.ImageBackId = imageBack.ID;
            }
            if (model.ImageBody != null)
            {
                var imageBody = new CheckoutCarImage() { ImageData = Utilities.AddWaterMarkOverImage(model.ImageBody, date, bitmaplogo, time) };
                checkoutDetails.ImageBody = imageBody;
                checkoutDetails.ImageBodyId = imageBody.ID;
            }
            if (model.ImageFront != null)
            {
                var imageFront = new CheckoutCarImage() { ImageData = Utilities.AddWaterMarkOverImage(model.ImageFront, date, bitmaplogo, time) };
                checkoutDetails.ImageFront = imageFront;
                checkoutDetails.ImageFrontId = imageFront.ID;
            }
            if (model.ImageLeft != null)
            {
                var imageLeft = new CheckoutCarImage() { ImageData = Utilities.AddWaterMarkOverImage(model.ImageLeft, date, bitmaplogo, time) };
                checkoutDetails.ImageLeft = imageLeft;
                checkoutDetails.ImageLeftId = imageLeft.ID;
            }
            if (model.ImageRight != null)
            {
                var imageRight = new CheckoutCarImage() { ImageData = Utilities.AddWaterMarkOverImage(model.ImageRight, date, bitmaplogo, time) };
                checkoutDetails.ImageRight = imageRight;
                checkoutDetails.ImageRightId = imageRight.ID;
            }

            return checkoutDetails;
        }
        private CheckoutDetail AssignImagesToCheckOut(CheckoutDetail checkoutDetails, CheckoutModel model)
        {
            if (model.ImageBack != null)
            {
                var imageBack = new CheckoutCarImage() { ImageData = model.ImageBack };
                checkoutDetails.ImageBack = imageBack;
                checkoutDetails.ImageBackId = imageBack.ID;
            }
            if (model.ImageBody != null)
            {
                var imageBody = new CheckoutCarImage() { ImageData = model.ImageBody };
                checkoutDetails.ImageBody = imageBody;
                checkoutDetails.ImageBodyId = imageBody.ID;
            }
            if (model.ImageFront != null)
            {
                var imageFront = new CheckoutCarImage() { ImageData = model.ImageFront };
                checkoutDetails.ImageFront = imageFront;
                checkoutDetails.ImageFrontId = imageFront.ID;
            }
            if (model.ImageLeft != null)
            {
                var imageLeft = new CheckoutCarImage() { ImageData = model.ImageLeft };
                checkoutDetails.ImageLeft = imageLeft;
                checkoutDetails.ImageLeftId = imageLeft.ID;
            }
            if (model.ImageRight != null)
            {
                var imageRight = new CheckoutCarImage() { ImageData = model.ImageRight };
                checkoutDetails.ImageRight = imageRight;
                checkoutDetails.ImageRightId = imageRight.ID;
            }

            return checkoutDetails;
        }
        private CheckoutOutput PaymentUsingSadad(PaymentRequestModel paymentRequestModel, int companyId, string companyName, string referenceId, bool isActive, string externalId)
        {
            CheckoutOutput output = new CheckoutOutput();

            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            SadadPaymentResponseModel sadadResponseModel = new SadadPaymentResponseModel();

            try
            {
                CultureInfo cultureEnglish = CultureInfo.GetCultureInfo("ar-EG");

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("En");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("En");

                var convertDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", cultureEnglish.DateTimeFormat);
                var format = "dd/MM/yyyy HH:mm:ss";
                var requestDateTime = DateTime.ParseExact(convertDate, format, cultureEnglish);

                SadadRequest sadadRequest = new SadadRequest
                {
                    BillerId = _config.Sadad.BillerId,
                    ExactFlag = _config.Sadad.ExactFlag,
                    CustomerAccountNumber = $"03{paymentRequestModel.InvoiceNumber}",
                    CustomerAccountName = paymentRequestModel.CustomerNameEn,
                    BillAmount = Math.Round(paymentRequestModel.PaymentAmount, 2, MidpointRounding.AwayFromZero),
                    BillOpenDate = requestDateTime,
                    BillDueDate = requestDateTime,
                    BillExpiryDate = requestDateTime.AddDays(1),
                    BillCloseDate = requestDateTime.AddHours(40),
                    CompanyId = companyId,
                    CompanyName = companyName,
                    ReferenceId = referenceId
                };

                var sadadResponse = _sadadPaymentService.ExecuteSadadPayment(sadadRequest, isActive, companyId, companyName, referenceId, externalId);

                if (sadadResponse != null)
                {
                    if (sadadResponse.ErrorCode == 0)
                    {
                        sadadResponseModel.Status = "Succeeded";
                        sadadResponseModel.ErrorMessage = null;
                        sadadResponseModel.ReferenceNumber = string.Format("{0}{1}{2}", sadadRequest.BillerId, sadadRequest.ExactFlag, sadadRequest.CustomerAccountNumber);
                        sadadResponseModel.BillDueDate = sadadRequest.BillExpiryDate;
                        // _shoppingCartService.EmptyShoppingCart(paymentRequestModel.UserId, paymentRequestModel.ReferenceId);

                        output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        //log.ErrorCode = (int)output.ErrorCode;
                        //log.ErrorDescription = output.ErrorDescription;
                        //CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        output.SadadPaymentResponseModel = sadadResponseModel;
                    }
                    else
                    {
                        sadadResponseModel.Status = "Failed";
                        sadadResponseModel.ErrorMessage = sadadResponse.Description;
                        sadadResponseModel.ReferenceNumber = null;

                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = sadadResponse.Description;
                        //log.ErrorCode = (int)output.ErrorCode;
                        //log.ErrorDescription = output.ErrorDescription;
                        //CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        output.SadadPaymentResponseModel = sadadResponseModel;
                    }
                }
                else
                {
                    sadadResponseModel.Status = "Failed";
                    sadadResponseModel.ErrorMessage = "Failed to read/parse response from RyadBank/Sadad service";
                    sadadResponseModel.ReferenceNumber = null;

                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = sadadResponseModel.ErrorMessage;
                    //log.ErrorCode = (int)output.ErrorCode;
                    //log.ErrorDescription = output.ErrorDescription;
                    //CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    output.SadadPaymentResponseModel = sadadResponseModel;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                //log.ErrorCode = (int)output.ErrorCode;
                //log.ErrorDescription = ex.ToString();
                //CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(name);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(name);
            }

            return output;
        }
        #endregion
        public CheckoutOutput ValidateRequest(string userId, CheckoutModel model, string channel, QuotationResponseDBModel quoteResponse,LanguageTwoLetterIsoCode lang)
        {
            CheckoutOutput output = new CheckoutOutput();
            if (!string.IsNullOrEmpty(TestMode) && TestMode.ToLower() == "true")
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            string phoneNumber = model.Phone.Trim();
            string email = model.Email.Trim();
            string iban = model.IBAN.Trim();
            try
            {
                string exception = string.Empty;
                string driverNin = quoteResponse.InsuredNationalId.StartsWith("7")? quoteResponse.NIN : quoteResponse.InsuredNationalId;

                if (string.IsNullOrEmpty(model.ReferenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId null ";
                    return output;
                }
                if (!Utilities.IsValidPhoneNo(phoneNumber))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPhone;
                    output.ErrorDescription = "phoneNumber is not valid user enter " + phoneNumber;
                    return output;
                }
                if (string.IsNullOrEmpty(iban))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    return output;
                }

                //YakeenMobileVerificationOutput yakeenMobileVerificationOutput = VerifyMobileFromYakeen(userId, driverNin, phoneNumber, lang.ToString(), out exception);
                //if (yakeenMobileVerificationOutput == null
                //    || yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success
                //    || yakeenMobileVerificationOutput.mobileVerificationModel == null
                //    || !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner)
                //{
                //    var logErrorMessage = (yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                //                            ? exception
                //                            : yakeenMobileVerificationOutput.mobileVerificationModel == null ? "yakeenMobileVerificationOutput.mobileVerificationModel return null"
                //                            : !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner ? $"the nationalId {driverNin} is not the owner of this mobile {phoneNumber}"
                //                            : "yakeenMobileVerificationOutput return null";

                //    output.ErrorCode = CheckoutOutput.ErrorCodes.YakeenMobileVerificationError;
                //    output.ErrorDescription = yakeenMobileVerificationOutput.ErrorDescription;
                //    output.LogDescription = logErrorMessage;
                //    return output;
                //}

                if (iban.ToLower().Replace("sa", string.Empty).Trim().Length < 22)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    return output;
                }
                if (!Regex.IsMatch(iban.ToLower().Replace("sa", string.Empty).Trim(), "^[0-9]*[A-Za-z]?[0-9]*$", RegexOptions.IgnoreCase))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.IBAN_Validation;
                    return output;
                }
                if (!iban.ToLower().StartsWith("sa"))
                    iban = "sa" + iban;
                iban = iban.Replace(" ", "").Trim();
                Guid driverId = quoteResponse.DriverId;
                
                if (DateTime.Now.AddHours(-16) > quoteResponse.QuotationResponseCreatedDate)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = "Quotation Expired";
                    return output;
                }
                var userManager = _authorizationService.GetUser(userId);
                if (!Utilities.IsValidMail(email))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = "email is not valid user enter " + email;
                    return output;
                }
                //if (!quoteResponse.InsuredNationalId.StartsWith("7")) // Not Company
                //{

                ////
                /// validate for corporate users also
                ///  As per jira (https://bcare.atlassian.net/browse/VW-785)
                //if (!userManager.Result.IsCorporateUser)
                {
                    exception = string.Empty;

                    #region Old Code

                    //User Insurance Number Limit Per Year = 5 Polices per year
                    List<InsuredPolicyInfo> userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesDetails(driverNin, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = "There is an error occured due to " + exception;
                        return output;
                    }
                    int userSuccessPolicies = 0;
                    if (userSuccessPoliciesDetails != null)
                    {
                        if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                        {
                            userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true).Sum(a => a.TotalPolicyCount);
                        }
                        else
                        {
                            userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false).Sum(a => a.TotalPolicyCount);
                        }
                    }
                    if (userSuccessPolicies >= UserInsuranceNumberLimitPerYear)
                    {
                        if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.CompanyDriverExceedsInsuranceNumberLimitPerYear;
                            output.ErrorDescription = "Company Driver " + driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                        }
                        else
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear;
                            output.ErrorDescription = "Nin Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                        }
                        return output;
                    }

                    #endregion

                    #region New logic for check user policies

                    //List<InsuredPolicyDetails> userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesInfo(driverNin, out exception);
                    //if (!string.IsNullOrEmpty(exception))
                    //{
                    //    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    //    output.ErrorDescription = "There is an error occured due to " + exception;
                    //    return output;
                    //}
                    //int userSuccessPolicies = 0;
                    //int pendingPolicies = 0;
                    //if (userSuccessPoliciesDetails != null)
                    //{
                    //    if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                    //    {
                    //        userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId == 4).ToList().Count;
                    //        pendingPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId != 4).ToList().Count;
                    //    }
                    //    else
                    //    {
                    //        userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId == 4).ToList().Count;
                    //        pendingPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId != 4).ToList().Count;
                    //    }
                    //}
                    //if (userSuccessPolicies >= UserInsuranceNumberLimitPerYear || (userSuccessPolicies + pendingPolicies) > UserInsuranceNumberLimitPerYear)//5 per year
                    //{
                    //    if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                    //    {
                    //        output.ErrorCode = CheckoutOutput.ErrorCodes.CompanyDriverExceedsInsuranceNumberLimitPerYear;
                    //        output.ErrorDescription = "Company Driver " + driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                    //    }
                    //    else
                    //    {
                    //        output.ErrorCode = CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear;
                    //        output.ErrorDescription = driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                    //    }

                    //    return output;
                    //}

                    #endregion

                    //if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Edaat)
                    {
                        DateTime startDate = DateTime.Now.AddDays(-1);
                        DateTime endDate = DateTime.Now;
                        exception = string.Empty;
                        var prevEdaatRequests = GetEdaatRequestsByNationalID(driverNin, startDate, endDate, out exception);
                        if (!string.IsNullOrEmpty(exception))
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                            output.ErrorDescription = "There is an error occured while getting edaat requests due to " + exception;
                            return output;
                        }

                        int edaatRequestCount = 0;
                        if (prevEdaatRequests != null)
                        {
                            if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                            {
                                edaatRequestCount = prevEdaatRequests.Where(a => a.InsuredNationalId != null && a.InsuredNationalId.StartsWith("7")).Count();
                            }
                            else
                            {
                                edaatRequestCount = prevEdaatRequests.Where(a => a.InsuredNationalId != null && !a.InsuredNationalId.StartsWith("7")).Count();
                            }
                        }

                        if (prevEdaatRequests != null && edaatRequestCount + userSuccessPolicies >= UserInsuranceNumberLimitPerYear)
                        {
                            if (lang == LanguageTwoLetterIsoCode.En)
                                output.FirstExpiryDateForEdaat = prevEdaatRequests.FirstOrDefault()?.CreatedDate?.AddDays(1).ToString("dd MMMM yyyy, HH:mm:ss", new CultureInfo("en-US"));
                            else
                                output.FirstExpiryDateForEdaat = prevEdaatRequests.FirstOrDefault()?.CreatedDate?.AddDays(1).ToString("dd MMMM yyyy, HH:mm:ss", new CultureInfo("ar-EG"));

                            output.ErrorCode = CheckoutOutput.ErrorCodes.EdaatNumberReachedToMaximum;
                            output.ErrorDescription = "user reached the maximum number of edaat requests as he have " + userSuccessPolicies + " success policies and he requested " + prevEdaatRequests.Count() + " requests from edaat";
                            return output;
                        }
                    }
                }
                //end of check 
                if (userId != "0733f674-92c2-4900-978a-0a325f182cec")//albesherm@bcare.com.sa as per mubark skip this condition from mohamed el bishr
                {
                    //validate iban,phone,email
                    bool skipPhoneValidation = false;
                    bool skipEmailValidation = false;
                    bool skipIbanValidation = false;
                    var checkoutDriverInfo = _checkoutsService.GetCheckoutDriverInfo(driverNin, phoneNumber, email, iban);
                    if (checkoutDriverInfo != null)
                    {
                        string phoneWithout966 = string.Empty;
                        if (phoneNumber.StartsWith("966"))
                        {
                            phoneWithout966 = phoneNumber.Substring("966".Length);
                            phoneWithout966 = "0" + phoneWithout966;
                        }
                        if (checkoutDriverInfo.Phone != null && (checkoutDriverInfo.Phone == phoneNumber || checkoutDriverInfo.Phone == Utilities.ValidatePhoneNumber(phoneNumber)
                            || checkoutDriverInfo.Phone == phoneWithout966))
                            skipPhoneValidation = true;
                        if (checkoutDriverInfo.Email != null && checkoutDriverInfo.Email.ToLower() == email.ToLower())
                            skipEmailValidation = true;
                        if (checkoutDriverInfo.IBAN != null && checkoutDriverInfo.IBAN.ToLower() == iban.ToLower())
                            skipIbanValidation = true;
                    }
                    var emailInfo = _orderService.CheckIfEmailAlreadyUsed(email, driverNin);
                    if (emailInfo != null && !skipEmailValidation)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.EmailAlreadyUsed;
                        output.ErrorDescription = "email " + email + " already used with another driver " + emailInfo.NIN + " and current driver id is " + driverNin;
                        return output;
                    }
                    bool isVerfiedByYaqeen = false;
                    var phoneInfo = _orderService.CheckIfPhoneAlreadyUsed(Utilities.ValidatePhoneNumber(phoneNumber), driverNin);
                    if (phoneInfo != null && !skipPhoneValidation)
                    {
                        var isVerifiedByYakeen = _orderService.IsUserHaveVerifiedPhoneByYakeen(driverNin, Utilities.ValidatePhoneNumber(phoneNumber));
                        if (!isVerifiedByYakeen)
                        {
                            var yakeenMobileVerificationDto = new YakeenMobileVerificationDto()
                            {
                                NationalId = quoteResponse.InsuredNationalId,
                                Phone = model.Phone
                            };
                            CheckoutRequestLog checkoutRequestLog = new CheckoutRequestLog();
                            var startTime = DateTime.Now;
                            YakeenMobileVerificationOutput yakeenMobileVerification = _iYakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, "en");
                            if (yakeenMobileVerification != null)
                            {
                                ServiceRequestLog yakeenServiceRequestLog = new ServiceRequestLog();
                                yakeenServiceRequestLog.Method = "Yakeen-yakeenMobileVerification";
                                yakeenServiceRequestLog.ServiceRequest = JsonConvert.SerializeObject(yakeenMobileVerificationDto);
                                yakeenServiceRequestLog.ServiceResponse = JsonConvert.SerializeObject(yakeenMobileVerification);
                                yakeenServiceRequestLog.DriverNin = quoteResponse.InsuredNationalId;
                                yakeenServiceRequestLog.ReferenceId = quoteResponse.ReferenceId;
                                yakeenServiceRequestLog.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(yakeenServiceRequestLog);

                                if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success || yakeenMobileVerification.mobileVerificationModel == null || !yakeenMobileVerification.mobileVerificationModel.isOwner)
                                {
                                    output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneAlreadyUsed;
                                    output.ErrorDescription = "phone Number " + phoneNumber + " already used with another driver " + phoneInfo.NIN + " and current driver id is " + driverNin;
                                    return output;
                                }
                                //else
                                //{
                                //    //if (phoneInfo.PolicyStatusId==4)
                                //    //{
                                //      //  UpdateCheckoutPhoneNumberDBToNull(phoneInfo.Phone);
                                //        CheckYaqeenMobile = true;
                                //    //}
                                //}

                                isVerfiedByYaqeen = true;
                            }
                        }
                    }
                    var ibanInfo = _orderService.CheckIfIbanAlreadyUsed(iban, driverNin);
                    if (ibanInfo != null && !skipIbanValidation)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.IBANUsedForOtherDriver;
                        output.ErrorDescription = "iban " + iban + " already used with another driver " + ibanInfo.NIN + " and current driver id is " + driverNin;
                        return output;
                    }
                    //}
                    //else
                    //{
                    //    driverNin = quoteResponse.NIN;
                    //}

                    string sendSMSTo = phoneNumber;
                    if (userManager.Result.IsCorporateUser)
                    {
                        sendSMSTo = userManager.Result.PhoneNumber;
                    }
                    string vehicleId = string.Empty;
                    bool userPhoneNumbers = false;
                    if (quoteResponse.InsuredNationalId.StartsWith("7") || quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    {
                        userPhoneNumbers = _orderService.IsUserHaveVerifiedPhoneNumbersByNIN(driverNin, Utilities.ValidatePhoneNumber(sendSMSTo.Trim()), model.ReferenceId);
                    }
                    else
                    {
                        userPhoneNumbers = _orderService.IsUserHaveVerifiedPhoneNumbersByNINAndSequenceNumber(driverNin, Utilities.ValidatePhoneNumber(sendSMSTo.Trim()), quoteResponse.SequenceNumber);
                    }
                    if (!userPhoneNumbers)  //|| CheckYaqeenMobile
                    {
                        #region Get Policy Price
                        var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(userId, model.ReferenceId);
                        if (shoppingCartItem == null)
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                            output.ErrorDescription = "shoppingCartItem is null";
                            return output;
                        }
                        decimal policyPrice = _shoppingCartService.CalculateShoppingCartTotal(shoppingCartItem);
                        #endregion
                        if ((quoteResponse.InsuranceTypeCode == 2 || quoteResponse.InsuranceTypeCode == 13) && DateTime.Now.Date >= new DateTime(2022, 12, 21)) // BCare discount for National day As per Mubarak 15-9-2022
                        {
                            var discount = (policyPrice * 5) / 100;
                            if (shoppingCartItem.ProductPrice >= 1350 && shoppingCartItem.ProductPrice <= 3999)
                                discount = 200;

                            policyPrice = policyPrice - discount;
                        }

                        if (quoteResponse.InsuranceTypeCode == 1 && model.ODDetails != null && !string.IsNullOrEmpty(model.ODDetails.ReferenceId) && !string.IsNullOrEmpty(model.ODDetails.ProductId))
                        {
                            var odShoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(userId, model.ODDetails.ReferenceId);
                            if (odShoppingCartItem == null)
                            {
                                output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                                output.ErrorDescription = "OD ShoppingCartItem is null";
                                return output;
                            }
                            policyPrice += _shoppingCartService.CalculateShoppingCartTotal(odShoppingCartItem);
                        }

                        Random rnd = new Random();
                        int verifyCode = rnd.Next(1000, 9999);
                        _orderService.CreateCheckoutUser(new CheckoutUsers()
                        {
                            UserId = Guid.Parse(userId),
                            PhoneNumber = Utilities.ValidatePhoneNumber(sendSMSTo.Trim()),
                            VerificationCode = verifyCode,
                            UserEmail = email,
                            ReferenceId = model.ReferenceId,
                            CreatedDate = DateTime.Now,
                            Nin = driverNin,
                            CustomCardNumber = quoteResponse.CustomCardNumber,
                            SequenceNumber = quoteResponse.SequenceNumber,
                            //VerfiedByYaqeen = isVerfiedByYaqeen,
                            IsMobileVerifiedbyYakeen = isVerfiedByYaqeen
                        });
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = sendSMSTo,
                            MessageBody = string.Format(CheckoutResources.VerificationCodeMessage, verifyCode) + string.Format(" {0}: {1} {2}", CheckoutResources.PolicyPrice, policyPrice.ToString("0.00"), CheckoutResources.SAR),
                            Method = SMSMethod.WebsiteOTP.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = channel,
                            ReferenceId = model.ReferenceId
                        };
                        var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);

                        output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneIsNotVerified;
                        output.ErrorDescription = "Phone number is not verified";
                        return output;
                    }
                }
                if (quoteResponse.CompanyID == 24) //Allianz only //Alalamiya removed as per fayssal request on Novamber 30 @ 9:44 am
                {
                    var ibanValidationResult = ValidateIBAN(iban, quoteResponse.InsuredNationalId, model.ReferenceId, channel);
                    if (ibanValidationResult.IsValid == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                        output.ErrorDescription = "ibanValidationResult.IsValid is null";
                        return output;
                    }
                    if (ibanValidationResult.IsValid == false)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                        output.ErrorDescription = "invaid iban " + iban;
                        return output;
                    }
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public bool ManagePhoneVerification(Guid userId, string phoneNumber, string code, string channel)
        {
            phoneNumber = Utilities.ValidatePhoneNumber(phoneNumber.Trim());
            DateTime dtBeforeCalling = DateTime.Now;
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.MethodName = "PhoneVerification";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.Channel = channel;
            checkoutlog.ServiceRequest = JsonConvert.SerializeObject(new { userId, phoneNumber, code, channel });
            try
            {
                var user = _authorizationService.GetUserDBByID(userId.ToString());
                if (user == null)
                {
                    checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.AnonymousUser;
                    checkoutlog.ErrorDescription = "user is null";
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return false;
                }
                var checkoutUser = _orderService.GetByUserIdAndPhoneNumberWithCodeNotVerified(userId, phoneNumber);
                if (checkoutUser == null)
                {
                    checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceDown;
                    checkoutlog.ErrorDescription = "checkoutUser is null";
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return false;
                }
                checkoutlog.DriverNin = checkoutUser.Nin;
                checkoutlog.ReferenceId = checkoutUser.ReferenceId;
                if (checkoutUser.VerificationCode != int.Parse(code))
                {
                    checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.InvalidVerificationCode;
                    checkoutlog.ErrorDescription = "Invalid Verification Code as correct code is " + checkoutUser.VerificationCode + " and received code is " + code;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return false;
                }
                if (user.PhoneNumber == phoneNumber && !user.PhoneNumberConfirmed)
                {
                    var result = _authorizationService.ConfirmUserPhoneNumberDB(userId.ToString());
                }

                checkoutUser.IsCodeVerified = true;
                _orderService.UpdateCheckoutUser(checkoutUser);
                return true;
            }
            catch (Exception exp)
            {
                checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                checkoutlog.ErrorDescription = exp.ToString();
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return false;
            }
        }
        public PolicyOutput GetUserPolicies(string nin, string sequenceNumber, string customCardNumber, out string exception)
        {
            exception = "";
            PolicyOutput output = new PolicyOutput();
            try
            {
                if (string.IsNullOrEmpty(nin))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "nin is null ";
                    return output;
                }
                if (string.IsNullOrEmpty(sequenceNumber) && string.IsNullOrEmpty(customCardNumber))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "sequenceNumber and customCardNumber is null";
                    return output;
                }
                var userPolicies = _checkoutsService.GetUserPolicies(nin, sequenceNumber, customCardNumber, out exception);
                var userFailedPolicies = _checkoutsService.GetUserFailedPolicies(nin, sequenceNumber, customCardNumber, null, out exception);
                output.Policies = userPolicies;
                output.FailedPolicies = userFailedPolicies;
                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public PolicyOutput GetUserFailedPoliciesByReference(string referenceId, out string exception)
        {
            exception = "";
            PolicyOutput output = new PolicyOutput();
            try
            {
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId is null ";
                    return output;
                }

                var userFailedPolicies = _checkoutsService.GetUserFailedPolicies(null, null, null, referenceId, out exception);
                output.FailedPolicies = userFailedPolicies;
                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public QuotationOutput ValidateTawuniyaQuotation(QuotationRequestDriverModel quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, List<string> selectedBenefits = null, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            QuotationOutput output = new QuotationOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel.ToString();
            checkoutlog.MethodName = "ValidateTawuniyaQuotation";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ReferenceId = referenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                //if (quotationRequest == null)
                //{
                //    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                //    output.ErrorDescription = "quotationRequest is null";
                //    checkoutlog.ErrorCode = (int)output.ErrorCode;
                //    checkoutlog.ErrorDescription = output.ErrorDescription;
                //    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                //    return output;
                //}
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (productInternalId == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "productInternalId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (insuranceCompanyID == 0)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "insuranceCompanyID = " + insuranceCompanyID;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyID);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoCompanyReturned;
                    output.ErrorDescription = "No Company Returned from database";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                checkoutlog.CompanyName = insuranceCompany.Key;
                checkoutlog.CompanyId = insuranceCompany.InsuranceCompanyID;

                var tawuniyaQuotation = _quotationContext.GetTawuniyaQuotation(quotationRequest.QuotationRequestId, referenceId, productInternalId, qtRqstExtrnlId, insuranceCompany, channel, userId, userName, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, automatedTest);
                if (tawuniyaQuotation.ErrorCode != QuotationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "No Returned Quotation due to " + tawuniyaQuotation.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                IInsuranceProvider provider = null;
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType != null)
                {
                    object instance = null;
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "provider is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                Product currentProduct = _productRepository.Table.Include(p => p.Product_Benefits).Include(p => p.PriceDetails).FirstOrDefault(p => p.Id == productInternalId);
                if (currentProduct == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "currentProduct is null " + productInternalId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                var tawuniyaProposal = _tawuniyaProposalRepository.Table.FirstOrDefault(e => e.ReferenceId == referenceId && e.ProposalTypeCode == currentProduct.InsuranceTypeCode);
                if (tawuniyaProposal == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "There is no proposal for this reference id " + referenceId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;
                predefinedLogInfo.VehicleId = tawuniyaQuotation.QuotationServiceRequest.VehicleId.ToString();
                predefinedLogInfo.DriverNin = tawuniyaQuotation.QuotationServiceRequest.InsuredId.ToString();
                predefinedLogInfo.ExternalId = quotationRequest.ExternalId;

                if (currentProduct.DeductableValue.HasValue)
                    tawuniyaQuotation.QuotationServiceRequest.DeductibleValue = currentProduct.DeductableValue.Value;
                if (currentProduct.InsuranceTypeCode == 2 && currentProduct.VehicleLimitValue.HasValue)
                    tawuniyaQuotation.QuotationServiceRequest.VehicleValue = currentProduct.VehicleLimitValue.Value;
                ServiceOutput results = provider.GetQuotationServiceResponse(tawuniyaQuotation.QuotationServiceRequest, currentProduct, tawuniyaProposal.ProposalNumber, predefinedLogInfo, selectedBenefits);
                if (results.ErrorCode != ServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "No Returned Quotation due to " + results.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                if (results.QuotationServiceResponse == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "results.QuotationServiceResponse is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                if (results.QuotationServiceResponse.Products == null || results.QuotationServiceResponse.Products.Count() == 0)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "No Returned Quotation Products";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                //var resultProduct = results.Products.First().ToEntity();
                var resultProduct = results.QuotationServiceResponse.Products.FirstOrDefault();
                if (resultProduct == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "resultProduct is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                decimal selectedBenefitsPrice = 0;
                decimal selectedBenefitsVat = 0;
                if (currentProduct.Product_Benefits != null && currentProduct.Product_Benefits.Any(a => selectedBenefits.Contains(a.BenefitExternalId)))
                {
                    selectedBenefitsPrice = currentProduct.Product_Benefits.Where(a => selectedBenefits.Contains(a.BenefitExternalId)).Sum(a => a.BenefitPrice.Value);
                    selectedBenefitsVat = selectedBenefitsPrice * (decimal)0.15;
                }

                resultProduct.ProductPrice = resultProduct.ProductPrice - selectedBenefitsPrice - selectedBenefitsVat;
                decimal proposalPrice = currentProduct.ProductPrice;
                if (resultProduct.ProductPrice > currentProduct.ProductPrice)
                {
                    currentProduct.ProductPrice = resultProduct.ProductPrice;
                    currentProduct.QuotaionNo = results.QuotationServiceResponse.QuotationNo;
                    if (resultProduct.PriceDetails != null)
                    {
                        foreach (var item in resultProduct.PriceDetails)
                        {
                            if (item.PriceTypeCode == 8) // vat code
                                continue;

                            if (currentProduct.PriceDetails.FirstOrDefault(p => p.PriceTypeCode == item.PriceTypeCode) != null)
                            {
                                currentProduct.PriceDetails.FirstOrDefault(p => p.PriceTypeCode == item.PriceTypeCode).PriceValue = item.PriceValue;

                                if (item.PercentageValue.HasValue) // Update Percentage
                                {
                                    currentProduct.PriceDetails.FirstOrDefault(p => p.PriceTypeCode == item.PriceTypeCode).PercentageValue = item.PercentageValue.Value;
                                }
                            }
                        }
                    }
                    output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Different Product Price as Proposal price is " + proposalPrice + " And Quotation price is " + resultProduct.ProductPrice;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    _productRepository.Update(currentProduct);
                    scope.Dispose();
                    return output;
                }
                currentProduct.QuotaionNo = results.QuotationServiceResponse.QuotationNo;
                _productRepository.Update(currentProduct);
                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                scope.Dispose();
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public PolicyOutput SendWataniyaDraftpolicy(QuotationRequestDriverModel quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            PolicyOutput output = new PolicyOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel.ToString();
            checkoutlog.MethodName = "SendWataniyaDraftpolicy";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ReferenceId = referenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (quotationRequest == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "quotationRequest is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (productInternalId == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "productInternalId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (insuranceCompanyID == 0)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "insuranceCompanyID = " + insuranceCompanyID;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyID);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = "No Company Returned from database";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                checkoutlog.CompanyName = insuranceCompany.Key;
                checkoutlog.CompanyId = insuranceCompany.InsuranceCompanyID;

                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                IInsuranceProvider provider = null;
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType != null)
                {
                    object instance = null;
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "provider is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                var wataniyaQuotationDetails = _quotationService.GetWataniyaQuotationResponseByReferenceId(referenceId);
                if (wataniyaQuotationDetails == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "No response from Wataniya witn this reference " + referenceId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                Product currentProduct = wataniyaQuotationDetails.Products.FirstOrDefault(p => p.Id == productInternalId);
                if (currentProduct == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "currentProduct is null " + productInternalId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                PolicyRequest draftPolicyRequest = new PolicyRequest();
                draftPolicyRequest.ReferenceId = referenceId;
                draftPolicyRequest.QuotationNo = wataniyaQuotationDetails.ICQuoteReferenceNo;
                //draftPolicyRequest.QuotationNo = currentProduct.QuotaionNo;
                draftPolicyRequest.DeductibleAmount = (currentProduct.DeductableValue.HasValue)
                                ? currentProduct.DeductableValue.Value.ToString()
                                : (wataniyaQuotationDetails.DeductibleValue.HasValue) ? wataniyaQuotationDetails.DeductibleValue.Value.ToString()
                                : "";
                draftPolicyRequest.PolicyPremium = currentProduct.PolicyPremium?.ToString();

                List<BenefitRequest> selectedBenefits = new List<BenefitRequest>();
                var shoppingCartItemBenefits = _shoppingCartService.GetUserShoppingCartItemBenefitsByUserIdAndReferenceId(userId.ToString(), referenceId);
                var ProductselectedBenefits = currentProduct.Product_Benefits.Where(a => shoppingCartItemBenefits.Contains(a.Id)).ToList();
                selectedBenefits = ProductselectedBenefits.Select(b =>
                        new BenefitRequest()
                        {
                            BenefitId = b.BenefitExternalId,
                            BenefitPrice = b.BenefitPrice
                        }).ToList();

                draftPolicyRequest.Benefits = selectedBenefits;

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;

                ServiceOutput results = provider.GetWataniyaMotorDraftpolicy(draftPolicyRequest, predefinedLogInfo);
                if (results.ErrorCode != ServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.NoReturnedDraftPolicy;
                    output.ErrorDescription = "No Returned Quotation From provider.GetWataniyaMotorDraftpolicy due to " + results.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                scope.Dispose();
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public static string GetCarPlateColorByCode(int? carPlateCode)
        {
            string resultColor = "";
            switch (carPlateCode)
            {
                case 1:
                case 10:
                    resultColor = "white";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 7:
                    resultColor = "blue";
                    break;
                case 6:
                    resultColor = "yellow";
                    break;
                case 8:
                case 11:
                    resultColor = "black";
                    break;
                case 9:
                    resultColor = "green";
                    break;
            }

            return resultColor;
        }

        public CheckoutOutput ActivateUserEmailToReceivePolicy(string referenceId, string email, string userId, string channel, out string exception)
        {
            CheckoutOutput output = new CheckoutOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel;
            checkoutlog.MethodName = "ActivateUserEmailToReceivePolicy";
            checkoutlog.UserId = userId;
            checkoutlog.ReferenceId = referenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.ServiceRequest = $"referenceId: {referenceId}, email: {email}, userId: {userId}, channel: {channel}";
            try
            {
                _orderService.ConfirmCheckoutDetailEmail(referenceId, userId, email, out exception);
                if (exception == "NoRecord")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "No Record found email is " + email + " user id is  " + userId + " and referenceId is " + referenceId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (!string.IsNullOrEmpty(exception) && exception != "NoRecord")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "exception " + exception + " email is " + email + " user id is  " + userId + " and referenceId is " + referenceId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                exception = string.Empty;
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public CheckoutDetail GetCheckoutDetailByReferenceIdAndUserId(string referenceId, string userId)
        {
            return _checkoutsService.GetCheckoutDetailByReferenceIdAndUserId(referenceId, userId);
        }

        public bool SendActivationEmail(string referenceId, string userId, out string exception)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.MethodName = "SendActivationEmail";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.ServiceRequest = JsonConvert.SerializeObject(new { userId, referenceId });
            exception = string.Empty;
            var checkoutDetails = GetCheckoutDetailByReferenceIdAndUserId(referenceId, userId);
            if (checkoutDetails == null)
            {
                exception = "checkoutDetails is null and referenceId=" + referenceId + " and userId=" + userId;
                checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.CheckoutDetailsNotFound;
                checkoutlog.ErrorDescription = exception;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return false;
            }
            string checkoutEmail = checkoutDetails.Email;
            try
            {
                ActivationEmailToReceivePolicyModel model = new ActivationEmailToReceivePolicyModel()
                {
                    UserId = userId,
                    RequestedDate = DateTime.Now,
                    Email = checkoutEmail,
                    ReferenceId = referenceId
                };
                var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);
                var emailSubject = CheckoutResources.ActivateEmailToReceivePolicySubject;
                string url = $"{Utilities.SiteURL}/ActivateUserEmailToReceivePolicy?token={HttpUtility.UrlEncode(token)}";
                string emailBody = string.Format(CheckoutResources.EmailActivationBodyToReceivePolicy, url);
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/ActivateEmailToReceivePolicy.png";
                messageBodyModel.Language = CultureInfo.CurrentCulture.Name;
                messageBodyModel.MessageBody = emailBody;

                List<string> to = new List<string>();
                to.Add(checkoutDetails.Email);
                EmailModel emailModel = new EmailModel();
                emailModel.Method = "CheckoutActivationEmail";
                emailModel.Module = Module.Vehicle.ToString(); ;
                emailModel.Channel = checkoutDetails.Channel;
                emailModel.To = to;
                emailModel.Subject = emailSubject;
                emailModel.EmailBody = emailBody.ToString();
                emailModel.ReferenceId = checkoutDetails.ReferenceId;
                var emailOutput = _notificationService.SendEmail(emailModel);
                if (emailOutput.ErrorCode != EmailOutput.ErrorCodes.Success)                {                    checkoutlog.ErrorCode = 5;                    checkoutlog.ErrorDescription = "Failed to send checkout activation email due to " + emailOutput.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);                    return false;                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                checkoutlog.ErrorDescription = exception;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return false;
            }
        }
        public IbanValidationOutput ValidateIBAN(string iban, string driverNin, string referenceId, string chanel)
        {
            IbanValidationOutput output = new IbanValidationOutput();
            DateTime startDate = DateTime.Now.AddHours(-1);
            var ibanInfo = _ibanHistory.TableNoTracking.Where(a => a.IBAN == iban.Trim() && a.CreatedDate >= startDate).FirstOrDefault();
            if (ibanInfo != null)
            {
                output.ErrorCode = IbanValidationOutput.StatusCode.Success;
                output.ErrorDescription = "Success";
                output.IsValid = true;
                return output;
            }
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = chanel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "ValidateIBAN";
            log.ServiceURL = Utilities.GetAppSetting("IbanValidationUrl");
            log.DriverNin = driverNin;
            log.ReferenceId = referenceId;
            try
            {
                iban = iban?.Trim();
                if (string.IsNullOrEmpty(iban))
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = "An IBAN must be provided.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                string apiUrl = $"{Utilities.GetAppSetting("IbanValidationUrl").TrimEnd('/')}/?"
                               + $"format=json"
                               + $"&api_key={Utilities.GetAppSetting("IbanValidationApiKey")}"
                               + $"&iban={iban}";
                log.ServiceRequest = $"{Utilities.GetAppSetting("IbanValidationUrl").TrimEnd('/')}/?"
                                       + $"format=json"
                                       + $"&iban={iban}";
                DateTime dtBeforeCalling = DateTime.Now;
                var task = _httpClient.GetStringAsync(apiUrl);
                task.Wait();
                var response = task.Result;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                log.ServiceResponse = response;
                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = "response return null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var responseBody = JsonConvert.DeserializeObject<IbanValidationServiceResult>(response);
                if (responseBody == null)
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = "Empty response was returned from the IBAN validation service.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseBody.Errors != null && responseBody.Errors.Any())
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = JsonConvert.SerializeObject(responseBody.Errors);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "responseBody contains Errors";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseBody.Validations == null)
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = "Empty validation object was returned from the IBAN validation service.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (responseBody.Validations.FailureValidationCodes.Any())
                {
                    output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                    output.ErrorDescription = "Validation errors were returned by the IBAN validation service.";
                    output.IsValid = false;
                    output.SuccessValidationMessages = (from code in responseBody.Validations.SuccessValidationCodes
                                                        select new ValidationMessage
                                                        {
                                                            Code = code,
                                                            Message = IBANValidationnDictionaries.ValidationsSuccessDictionary[code]
                                                        })
                                                    .ToList();
                    output.FailureValidationMessages = (from code in responseBody.Validations.FailureValidationCodes
                                                        select new ValidationMessage
                                                        {
                                                            Code = code,
                                                            Message = IBANValidationnDictionaries.ValidationsFailureDictionary[code]
                                                        })
                                                    .ToList();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                IbanHistory ibanDetails = new IbanHistory();
                ibanDetails.ReferenceId = referenceId;
                ibanDetails.IBAN = iban;
                ibanDetails.Account = responseBody.BankData.Account;
                ibanDetails.Address = responseBody.BankData.Address;
                ibanDetails.Bank = responseBody.BankData.Bank;
                ibanDetails.BankCode = responseBody.BankData.BankCode;
                ibanDetails.Bic = responseBody.BankData.Bic;
                ibanDetails.Branch = responseBody.BankData.Branch;
                ibanDetails.BranchCode = responseBody.BankData.BranchCode;
                ibanDetails.City = responseBody.BankData.City;
                ibanDetails.Country = responseBody.BankData.Country;
                ibanDetails.CountryIso = responseBody.BankData.CountryIso;
                ibanDetails.Email = responseBody.BankData.Email;
                ibanDetails.Fax = responseBody.BankData.Fax;
                ibanDetails.Phone = responseBody.BankData.Phone;
                ibanDetails.State = responseBody.BankData.State;
                ibanDetails.Www = responseBody.BankData.Www;
                ibanDetails.Zip = responseBody.BankData.Zip;

                ibanDetails.SepaDataB2B = responseBody.SepaData.B2B;
                ibanDetails.SepaDataCOR1 = responseBody.SepaData.Cor1;
                ibanDetails.SepaDataSCC = responseBody.SepaData.Scc;
                ibanDetails.SepaDataSCT = responseBody.SepaData.Sct;
                ibanDetails.SepaDataSDD = responseBody.SepaData.Sdd;
                ibanDetails.ResponseJson = response;
                ibanDetails.CreatedDate = DateTime.Now;
                _ibanHistory.Insert(ibanDetails);

                output.ErrorCode = IbanValidationOutput.StatusCode.Success;
                output.ErrorDescription = "Success";
                output.IsValid = true;
                output.SuccessValidationMessages = (from code in responseBody.Validations.SuccessValidationCodes
                                                    select new ValidationMessage
                                                    {
                                                        Code = code,
                                                        Message = IBANValidationnDictionaries.ValidationsSuccessDictionary[code]
                                                    })
                                                   .ToList();

                output.FailureValidationMessages = (from code in responseBody.Validations.FailureValidationCodes
                                                    select new ValidationMessage
                                                    {
                                                        Code = code,
                                                        Message = IBANValidationnDictionaries.ValidationsFailureDictionary[code]
                                                    })
                                             .ToList();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IbanValidationOutput.StatusCode.Failure;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public NumberOfAccidentOutput ValidateNumberOfAccident(string userId, string userName, ShoppingCartItemDB item, QuotationRequestDriverModel quotationRequest, string channel, List<long> selectedProductBenfitId)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            NumberOfAccidentOutput output = new NumberOfAccidentOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel;
            checkoutlog.MethodName = "ValidateNumberOfAccident";
            checkoutlog.UserId = userId;
            checkoutlog.UserName = userName;
            checkoutlog.ReferenceId = item.QuotationReferenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.VehicleId = quotationRequest.SequenceNumber != null ? quotationRequest.SequenceNumber : quotationRequest.CustomCardNumber;
            checkoutlog.DriverNin = quotationRequest.DriverNIN;
            try
            {
                string exception = string.Empty;
                var insuranceCompany = _insuranceCompanyService.GetById(item.InsuranceCompanyId);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NoCompanyReturned;
                    output.ErrorDescription = "No Company Returned from database with this id " + item.InsuranceCompanyId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                checkoutlog.CompanyName = insuranceCompany.Key;
                checkoutlog.CompanyId = insuranceCompany.InsuranceCompanyID;
                output.NewReferenceId = item.ReferenceId; // as defualt value
                output.newProductId = item.ProductId.ToString(); // as defualt value
                if (!insuranceCompany.IsUseNumberOfAccident.HasValue || insuranceCompany.IsUseNumberOfAccident == false)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }
                var ncds = insuranceCompany.NajmNcdFreeYearsToUseNumberOfAccident.Split(',').ToList();
                if (ncds.Count() == 0)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NcdFreeYearsIsNull;
                    output.ErrorDescription = "NajmNcdFreeYearsToUseNumberOfAccident is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (DateTime.Now.AddHours(-16) > quotationRequest.QuotationResponseCreatedDate)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.QuotationExpired;
                    output.ErrorDescription = "Quotation Expired";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (quotationRequest.NoOfAccident.HasValue)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }
                if (!quotationRequest.NajmNcdFreeYears.HasValue)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NcdFreeYearsIsNull;
                    output.ErrorDescription = "Ncd Free Years IsNull";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (!ncds.Contains(quotationRequest.NajmNcdFreeYears.Value.ToString()))
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    return output;
                }
                NajmDriverCaseRequest request = new NajmDriverCaseRequest();
                request.InsuranceId = 35;
                request.DriverId = quotationRequest.DriverNIN;

                var benchmarkDate = DateTime.Now.AddDays(-45);
                var najmResponse = _najmAccidentResponseRepository.TableNoTracking.Where(d => d.VehicleId == checkoutlog.VehicleId && d.IsDeleted == false && d.CreatedDate > benchmarkDate).FirstOrDefault();
                NajmOutput NumberOfAccidentResponse = new NajmOutput();
                if (najmResponse == null)
                {
                    NumberOfAccidentResponse = najmService.GetDriverCaseDetailV2(request, channel, checkoutlog.ReferenceId, checkoutlog.VehicleId, quotationRequest.ExternalId, new Guid(checkoutlog.UserId), checkoutlog.UserName, checkoutlog.CompanyId.Value, checkoutlog.CompanyName);
                    if (NumberOfAccidentResponse.ErrorCode != NajmOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NcdFreeYearsIsNull;
                        output.ErrorDescription = " GetDriverCaseDetailV2 returned error which is " + NumberOfAccidentResponse.ErrorDescription;
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        dtAfterCalling = DateTime.Now;
                        checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        return output;
                    }
                    NajmAccidentResponse njm = new NajmAccidentResponse();
                    njm.ReferenceId = item.QuotationReferenceId;
                    njm.ReferenceNo = NumberOfAccidentResponse.NajmDriverCaseResponse.ReferenceNo;
                    njm.VehicleId = checkoutlog.VehicleId;
                    njm.DriverNin = checkoutlog.DriverNin;
                    njm.MessageID = NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID;
                    njm.IsDeleted = false;
                    if ((NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID != "101"))
                    {
                        njm.NoOfAccident = NumberOfAccidentResponse.NajmDriverCaseResponse.CaseDetails.Noofaccident;
                    }
                    njm.NajmResponse = JsonConvert.SerializeObject(NumberOfAccidentResponse.NajmDriverCaseResponse);
                    njm.ExternalId = quotationRequest.ExternalId;
                    _najmAccidentResponseRepository.Insert(njm);
                }
                else
                {
                    ResponseData response = JsonConvert.DeserializeObject<ResponseData>(najmResponse.NajmResponse);
                    NumberOfAccidentResponse.NajmDriverCaseResponse = response;
                }
                // var quotationRequestDB = _quotationService.GetQuotationRequestData(quotationRequest.ExternalId);
                int noOfAccident = (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID != "101")
                                                ? NumberOfAccidentResponse.NajmDriverCaseResponse.CaseDetails.Noofaccident
                                                : 0;
                string najmResponseValue = (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID != "101")
                                                    ? JsonConvert.SerializeObject(NumberOfAccidentResponse.NajmDriverCaseResponse.CaseDetails.CaseDetail)
                                                    : NumberOfAccidentResponse.NajmDriverCaseResponse.ReferenceNo;

                _quotationService.UpdateQuotationRequestWithNOA(quotationRequest.QuotationRequestId, quotationRequest.InsuredId, noOfAccident, najmResponseValue);

                bool matchedPrice = false;
                if (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID == "101" && channel.ToLower() == "portal".ToLower())
                {
                    //As per Fayssal and Khalid get another Quote and compare price
                    Product currentProduct = _productRepository.TableNoTracking.Where(p => p.Id == item.ProductId).FirstOrDefault();
                    if (currentProduct == null)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.CurrentProductIsNull;
                        output.ErrorDescription = "currentProduct is null " + item.ProductId;
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        return output;
                    }

                    string promotionProgramCode = string.Empty;
                    int promotionProgramId = 0;
                    var newQuotation = _quotationContext.GetNOAQuotationRequest(quotationRequest.ExternalId, quotationRequest.InsuranceCompanyID, userId, channel,
                       quotationRequest.InsuranceTypeCode.Value, quotationRequest.VehicleAgencyRepair.Value, quotationRequest.DeductibleValue, out promotionProgramCode, out promotionProgramId);
                    if (newQuotation.ErrorCode != QuotationOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NoReturnedQuotation;
                        output.ErrorDescription = "No Returned Quotation Request Message due to " + newQuotation.ErrorDescription;
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        return output;
                    }
                    var providerFullTypeName = string.Empty;
                    providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

                    ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                    predefinedLogInfo.UserID = new Guid(userId);
                    predefinedLogInfo.UserName = userName;
                    predefinedLogInfo.RequestId = Guid.NewGuid();
                    predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
                    predefinedLogInfo.InsuranceTypeCode = quotationRequest.InsuranceTypeCode;

                    predefinedLogInfo.DriverNin = checkoutlog.DriverNin;
                    predefinedLogInfo.VehicleId = checkoutlog.VehicleId;
                    predefinedLogInfo.Channel = channel;

                    QuotationServiceResponse results = null;

                    IInsuranceProvider provider = null;
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);
                    if (providerType != null)
                    {
                        object instance = null;
                        if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                        {
                            //not resolved
                            instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                        }
                        provider = instance as IInsuranceProvider;
                    }
                    if (provider == null)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = "provider is null";
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        scope.Dispose();
                        return output;
                    }
                    results = provider.GetQuotation(newQuotation.QuotationServiceRequest, predefinedLogInfo, false);

                    if (results == null)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NoReturnedQuotation;
                        output.ErrorDescription = "results.QuotationServiceResponse is null";
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        scope.Dispose();
                        return output;
                    }
                    if (results.Products == null || results.Products.Count() == 0)
                    {
                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NoReturnedQuotation;
                        output.ErrorDescription = "No Returned Quotation Products";
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription;
                        checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        scope.Dispose();
                        return output;
                    }
                    var products = new List<Product>();
                    var allBenefitst = _benefitRepository.Table.ToList();
                    var allPriceTypes = _priceTypeRepository.Table.ToList();
                    foreach (var p in results.Products)
                    {
                        var product = p.ToEntity();
                        if (newQuotation.QuotationServiceRequest != null && !string.IsNullOrEmpty(newQuotation.QuotationServiceRequest.PromoCode))
                            product.IsPromoted = true;
                        product.ProviderId = insuranceCompany.InsuranceCompanyID;
                        if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                            product.InsuranceTypeCode = quotationRequest.InsuranceTypeCode;

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
                                if (pb.BenefitId == 7 && quotationRequest.VehicleAgencyRepair == true)
                                {
                                    pb.IsSelected = true;
                                }
                            }
                        }
                        product.CreateDateTime = DateTime.Now;
                        product.ReferenceId = newQuotation.QuotationResponse.ReferenceId;

                        // Load price details from database.
                        foreach (var pd in product.PriceDetails)
                        {
                            pd.IsCheckedOut = false;
                            pd.CreateDateTime = DateTime.Now;
                            pd.PriceType = allPriceTypes.FirstOrDefault(pt => pt.Code == pd.PriceTypeCode);
                        }
                        product.QuotaionNo = results.QuotationNo;
                        products.Add(product);
                    }
                    newQuotation.QuotationResponse.Products = products;
                    if (!string.IsNullOrEmpty(promotionProgramCode) && promotionProgramId != 0)
                    {
                        newQuotation.QuotationResponse.PromotionProgramCode = promotionProgramCode;
                        newQuotation.QuotationResponse.PromotionProgramId = promotionProgramId;
                    }
                    if (quotationRequest.CityYakeenCode.HasValue)
                        newQuotation.QuotationResponse.CityId = quotationRequest.CityYakeenCode;
                    newQuotation.QuotationResponse.ICQuoteReferenceNo = results.QuotationNo;
                    _quotationResponseRepository.Insert(newQuotation.QuotationResponse);

                    Product newProduct = null;
                    foreach (var product in newQuotation.QuotationResponse.Products)
                    {
                        newProduct = product;
                        if (product.DeductableValue != currentProduct.DeductableValue)
                            continue;
                        if (product.ExternalProductId != currentProduct.ExternalProductId)
                            continue;
                        if (product.ProductPrice == currentProduct.ProductPrice)
                        {
                            matchedPrice = true;
                            break;
                        }
                    }

                    if (matchedPrice)
                    {
                        _shoppingCartService.EmptyShoppingCart(userId, item.QuotationReferenceId);
                        bool retValue = _shoppingCartService.AddItemToCart(userId, newQuotation.QuotationResponse.ReferenceId, newProduct.Id, selectedProductBenfitId?.Select(b => new Product_Benefit
                        {
                            Id = b,
                            IsSelected = true
                        }).ToList(), out exception);
                        if (!retValue)
                        {
                            output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.ServiceException;
                            output.ErrorDescription = "Failed to add new shopping cart item due to:" + exception;
                            checkoutlog.ErrorCode = (int)output.ErrorCode;
                            checkoutlog.ErrorDescription = output.ErrorDescription;
                            dtAfterCalling = DateTime.Now;
                            checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                            scope.Dispose();
                            return output;
                        }

                        output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.Success;
                        output.ErrorDescription = NumberOfAccidentResponse.NajmDriverCaseResponse.Message;
                        output.NewReferenceId = newQuotation.QuotationResponse.ReferenceId;
                        output.newProductId = newProduct.Id.ToString();
                        checkoutlog.ErrorCode = (int)output.ErrorCode;
                        checkoutlog.ErrorDescription = output.ErrorDescription
                            + " old Reference " + item.QuotationReferenceId + " Old product " + currentProduct.Id
                            + " new ReferenceId " + newQuotation.QuotationResponse.ReferenceId + " new Product " + newProduct.Id.ToString();
                        dtAfterCalling = DateTime.Now;
                        checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                        scope.Dispose();
                        return output;
                    }
                    output.ErrorDescription = "product price are not the same as old product price is " + currentProduct.ProductPrice;
                    output.ErrorDescription += " and old external is " + currentProduct.ExternalProductId;
                    output.ErrorDescription += " and old deducatble is " + currentProduct.DeductableValue;
                    output.ErrorDescription += " and new price is " + newProduct.ProductPrice;
                    output.ErrorDescription += " and new external is " + newProduct.ExternalProductId;
                    output.ErrorDescription += " and new deducatble is " + newProduct.DeductableValue;
                }
                _quotationService.ExpireQuotationResponses(quotationRequest.QuotationRequestId, quotationRequest.InsuredId);
                _shoppingCartService.EmptyShoppingCart(userId, item.QuotationReferenceId);
                exception = string.Empty;
                bool retVal = _quotationService.DeleteFromQuotationResponseCache(quotationRequest.ExternalId, out exception);
                if (!retVal && !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Failed to delete from cache due to " + exception;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.HaveAccidents;
                if (NumberOfAccidentResponse.NajmDriverCaseResponse.MessageID == "101" && !matchedPrice)
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.NotHaveAccidents;
                    output.ErrorDescription = "No Accidents";
                }
                else
                {
                    output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.HaveAccidents;
                    output.ErrorDescription = "Have Accidents";
                }
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                dtAfterCalling = DateTime.Now;
                checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = NumberOfAccidentOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                dtAfterCalling = DateTime.Now;
                checkoutlog.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public string GetCarPlateInfo(string plateText1, string plateText2, string plateText3, int plateNumber, string lang)
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

        public List<RenewalPolicyInfo> GetRenewalPolicies(DateTime start, DateTime end, int notificationNo, out string exception)
        {
            return _checkoutsService.GetRenewalPolicies(start, end, notificationNo, out exception);
        }

        public CheckoutOutput UserHasActivePolicy(string nin)
        {
            var dateBeforeCalling = DateTime.Now;
            var log = new CheckoutRequestLog()
            {
                MethodName = "UserHasActivePolicy",
                ServerIP = Utilities.GetInternalServerIP(),
                UserAgent = Utilities.GetUserAgent(),
                UserIP = Utilities.GetUserIPAddress(),
                ServiceRequest = $"nin: {nin}",
                DriverNin = nin
            };

            try
            {
                string errorMessage;
                var result = _checkoutsService.UserHasActivePolicy(nin, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                    log.ErrorDescription = errorMessage;
                    log.ResponseTimeInSeconds = (DateTime.Now - dateBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return new CheckoutOutput
                    {
                        ErrorCode = CheckoutOutput.ErrorCodes.ServiceException,
                        ErrorDescription = CheckoutResources.ErrorGeneric
                    };
                }

                log.ErrorCode = (int)CheckoutOutput.ErrorCodes.Success;
                log.ErrorDescription = "SUCCESS";
                log.ResponseTimeInSeconds = (DateTime.Now - dateBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return new CheckoutOutput
                {
                    ErrorCode = CheckoutOutput.ErrorCodes.Success,
                    ErrorDescription = "SUCCESS",
                    ActivePolicyData = result
                };
            }
            catch (Exception ex)
            {
                log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                log.ResponseTimeInSeconds = (DateTime.Now - dateBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return new CheckoutOutput
                {
                    ErrorCode = CheckoutOutput.ErrorCodes.ServiceException,
                    ErrorDescription = CheckoutResources.ErrorGeneric
                };
            }
        }

        public List<PolicyInformation> GetPolicyInformationForRoadAssistance()
        {
            string exception = string.Empty;
            try
            {

                List<PolicyInformation> policyInformation = _checkoutsService.GetPolicyInformationForRoadAssistance(out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    string filename = @"C:\inetpub\wwwroot\ScheduledTask\logs\GetPolicyInformationForRoadAssistance_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".txt";
                    System.IO.File.WriteAllText(filename, exception);
                }
                else if (policyInformation == null || policyInformation.Count == 0)
                {
                    string filename = @"C:\inetpub\wwwroot\ScheduledTask\logs\GetPolicyInformationForRoadAssistance_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".txt";
                    System.IO.File.WriteAllText(filename, "policyInformation is null or count is zero");
                }

                return policyInformation;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetACheckOutTemp_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return null;
            }

        }


        public List<FailedMorniRequests> GetAllFailedMorniRequests()
        {
            string exception = string.Empty;
            try
            {

                List<FailedMorniRequests> policyInformation = _checkoutsService.GetAllFailedMorniRequests(out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    string filename = @"C:\inetpub\wwwroot\ScheduledTask\logs\GetPolicyInformationForRoadAssistance_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".txt";
                    System.IO.File.WriteAllText(filename, exception);
                }
                else if (policyInformation == null || policyInformation.Count == 0)
                {
                    string filename = @"C:\inetpub\wwwroot\ScheduledTask\logs\GetPolicyInformationForRoadAssistance_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".txt";
                    System.IO.File.WriteAllText(filename, "policyInformation is null or count is zero");
                }

                return policyInformation;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetACheckOutTemp_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return null;
            }

        }

        public CheckoutOutput SubmitAutoleasingCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            log.ReferenceId = model.ReferenceId;
            log.MethodName = "AutoleasingSubmitCheckoutDetails";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(Enum.GetName(typeof(LanguageTwoLetterIsoCode), lang).ToLower());
            CheckoutResources.Culture = cultureInfo;
            try
            {
                model.Channel = Channel.autoleasing.ToString().ToLower();

                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout Model is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                Guid userId = Guid.Empty;
                Guid.TryParse(log.UserId, out userId);
                if (userId == Guid.Empty)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User ID is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IsRenewal && log.UserId != "ff4d6e21-5a48-47ff-b076-d13bfeae8813")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Moneera's account only can purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (log.UserId == "6fd351a8-cff8-4745-838d-b2153c5f7a68")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Mubark Account is blocked from purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var userManager = _autoleasingUserService.GetUser(log.UserId.ToString());
                log.UserName = userManager?.UserName;
                if (userManager == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userManager is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (userManager.LockoutEnabled.HasValue && userManager.LockoutEndDateUtc >= DateTime.UtcNow)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserLockedOut;
                    output.ErrorDescription = CheckoutResources.AccountLocked;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is locked";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!userManager.CanPurchase)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "this user is from alyousr and is blocked from purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var bank = _bankService.GetBank(userManager.BankId);
                if (bank == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidBank;
                    output.ErrorDescription = "Invalid Bank Id:" + bank.Id;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid Bank Id:" + bank.Id;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                model.Email = userManager.Email;//bank.Email;
                model.IBAN = bank.IBAN;

                int bankCode = 0;
                Int32.TryParse(model.IBAN.ToLower().Replace("sa", "").Substring(2, 2), out bankCode);
                model.BankCode = bankCode;

                if (model.IBAN == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IBAN.ToLower().Replace("sa", string.Empty).Trim().Length < 22)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN length is less than 22 char as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (model.IBAN.ToLower().Replace("sa", string.Empty).Trim().Length > 22)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.InvalidIBANlength;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "IBAN length is more than 22 char as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!Regex.IsMatch(model.IBAN.ToLower().Replace("sa", string.Empty).Trim(), "^[0-9]*[A-Za-z]?[0-9]*$", RegexOptions.IgnoreCase))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidIBAN;
                    output.ErrorDescription = CheckoutResources.Checkout_InvalidIBAN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invalid IBAN as we received " + model.IBAN;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!model.IBAN.ToLower().StartsWith("sa"))
                {
                    model.IBAN = "sa" + model.IBAN;
                    model.IBAN = model.IBAN.Replace(" ", "").Trim();
                }
                // create checkout detail object.
                var checkoutDetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(model.ReferenceId);
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ReferenceId);
                if (shoppingCartItem == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (shoppingCartItem.ProductId == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item product is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                //if (shoppingCartItem.ProductId.ToString().ToLower() != model.ProductId.ToLower())
                //{
                //    output.ErrorCode = CheckoutOutput.ErrorCodes.ConfilictProduct;
                //    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "conflict product as DB return " + shoppingCartItem.ProductId.ToString() + " and user select " + model.ProductId;
                //    dtAfterCalling = DateTime.Now;
                //    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                //    return output;
                //}

                log.CompanyId = shoppingCartItem.InsuranceCompanyID;
                log.CompanyName = shoppingCartItem.InsuranceCompanyKey;

                QuotationResponseDBModel quoteResponse = null;
                quoteResponse = _quotationService.GetAutoleasingQuotationResponseByReferenceIdDB(model.ReferenceId, shoppingCartItem.ProductId.ToString());
                if (quoteResponse == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QuotationvResponse is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    log.VehicleId = quoteResponse.CustomCardNumber;
                else
                    log.VehicleId = quoteResponse.SequenceNumber;

                log.DriverNin = quoteResponse.NIN;

                if (quoteResponse.ReferenceId != model.ReferenceId)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.initialOptionPurchasedError;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId in quotation Response not match refrenceId";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quoteResponse.AutoleasingInitialOptionResponce.HasValue && quoteResponse.AutoleasingInitialOptionResponce == true)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.initialOptionPurchasedError;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "initial Option can't be purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                string exception = string.Empty;
                if (string.IsNullOrEmpty(TestMode) || TestMode.ToLower() == "false")
                {
                    var numberOfActivePolicies = _checkoutsService.GetActivePoliciesByNinAndVehicleId(log.DriverNin, log.VehicleId, out exception);
                    if (numberOfActivePolicies == -1)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = CheckoutResources.ErrorGeneric;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Exception due to: " + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    else if (numberOfActivePolicies > 0)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.HasActivePolicies;
                        output.ErrorDescription = string.Format(CheckoutResources.DriverNinWithVehicleIdHasActivePolicies, log.DriverNin, log.VehicleId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The DriverNin: {log.DriverNin}, VehicleId: {log.VehicleId} , has {numberOfActivePolicies} Active Policies";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                short insuranceTypeCode = 0;
                if (quoteResponse.ProductInsuranceTypeCode.HasValue)
                {
                    short.TryParse(quoteResponse.ProductInsuranceTypeCode.Value.ToString(), out insuranceTypeCode);
                    quoteResponse.InsuranceTypeCode = insuranceTypeCode;
                }
                //string exception = string.Empty;
                string driverPhone = _driverService.GetDriver(quoteResponse.DriverId.ToString())?.MobileNumber;


                if (!Utilities.IsValidPhoneNo(driverPhone))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPhone;
                    output.ErrorDescription = CheckoutResources.checkout_error_phone;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "incorrect phone format as phone is " + driverPhone;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (checkoutDetails == null)
                {
                    checkoutDetails = new CheckoutDetail
                    {
                        ReferenceId = model.ReferenceId,
                        BankCodeId = model.BankCode,
                        Email = model.Email,
                        IBAN = model.IBAN.Replace(" ", "").Trim().ToLower(),
                        Phone = Utilities.ValidatePhoneNumber(driverPhone),
                        UserId = model.UserId,
                        MainDriverId = quoteResponse.DriverId,
                        PolicyStatusId = (int)EPolicyStatus.PaymentSuccess,
                        VehicleId = quoteResponse.VehicleId,
                        CreatedDateTime = DateTime.Now,
                        SelectedInsuranceTypeCode = quoteResponse.InsuranceTypeCode/*(short)model.TypeOfInsurance*/,
                        SelectedLanguage = lang,
                        InsuranceCompanyId = quoteResponse.CompanyID,
                        InsuranceCompanyName = quoteResponse.CompanyKey,
                        Channel = model.Channel,
                        IsEmailVerified = true,
                        SelectedProductId = shoppingCartItem.ProductId,
                        BankId = bank.Id,
                        InsuredId = quoteResponse.InsuredTableRowId,
                        ExternalId = quoteResponse.ExternalId
                    };
                    if (bank.HasWallet && bank.IsAcitveWallet)
                    {
                        checkoutDetails.IsAutoleasingWallet = true;
                        checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.AutoleasingWallet;
                    }
                    else
                    {
                        checkoutDetails.IsAutoleasingWallet = false;
                        checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.AutoLeasing;
                    }
                    checkoutDetails.OrderItems = _orderService.CreateOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId);
                    checkoutDetails = AssignImagesToCheckOut(checkoutDetails, model);
                    if (quoteResponse.AdditionalDriverList != null && quoteResponse.AdditionalDriverList.Count > 1)
                    {
                        int index = 0;
                        foreach (var driverId in quoteResponse.AdditionalDriverList)
                        {

                            checkoutDetails.CheckoutAdditionalDrivers.Add(new CheckoutAdditionalDriver()
                            {
                                CheckoutDetailsId = checkoutDetails.ReferenceId,
                                DriverId = driverId
                            });
                            if (driverId.ToString().ToLower() != quoteResponse.DriverId.ToString().ToLower())//not main driver 
                            {
                                index++;
                                if (index == 1)
                                {
                                    checkoutDetails.AdditionalDriverIdOne = driverId;
                                }
                                if (index == 2)
                                {
                                    checkoutDetails.AdditionalDriverIdTwo = driverId;
                                }
                                if (index == 3)
                                {
                                    checkoutDetails.AdditionalDriverIdThree = driverId;
                                }
                                if (index == 4)
                                {
                                    checkoutDetails.AdditionalDriverIdFour = driverId;
                                }
                            }
                        }
                    }
                    // save checkout details on database.
                    _orderService.CreateCheckoutDetails(checkoutDetails);
                }
                else
                {

                    if (_orderService.DeleteOrderItemByRefrenceId(model.ReferenceId, out exception))
                        _orderService.SaveOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId, out exception);
                    checkoutDetails = AssignImagesToCheckOut(checkoutDetails, model);
                    checkoutDetails.UserId = model.UserId;
                    checkoutDetails.SelectedLanguage = lang;
                    checkoutDetails.Email = model.Email;
                    checkoutDetails.IBAN = model.IBAN.Replace(" ", "").Trim().ToLower();
                    checkoutDetails.Phone = Utilities.ValidatePhoneNumber(driverPhone);
                    checkoutDetails.BankCodeId = model.BankCode;
                    checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.AutoLeasing;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    checkoutDetails.Channel = log.Channel;
                    checkoutDetails.SelectedProductId = shoppingCartItem.ProductId;
                    checkoutDetails.InsuredId = quoteResponse.InsuredTableRowId;
                    checkoutDetails.ExternalId = quoteResponse.ExternalId;
                    _orderService.UpdateCheckout(checkoutDetails);
                }
                _quotationService.UpdateQuotationResponseToBeCheckedout(quoteResponse.QuotationResponseId, shoppingCartItem.ProductId);
                // check if invoice already exists
                var invoice = _orderService.GetInvoiceByRefrenceId(model.ReferenceId);
                if (invoice != null)
                {
                    // cancel invoice
                    if (!_orderService.DeleteInvoiceByRefrenceId(model.ReferenceId, log.UserId.ToString(), out exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToDeleteOldInvoice;
                        output.ErrorDescription = CheckoutResources.FailedToDeleteOldInvoice;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed To Delete previous Invoice with InvoiceNo " + invoice.InvoiceNo + " and InvoiceId " + invoice.Id + " due to " + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PendingPayment;
                        _orderService.UpdateCheckout(checkoutDetails);

                        return output;
                    }
                    invoice = _orderService.CreateInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID, invoice.InvoiceNo);
                }
                else
                {
                    invoice = _orderService.CreateInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID);
                }
                log.Amount = invoice.TotalPrice.Value;
                output.DriverNin = quoteResponse.NIN;
                output.SequenceNumber = quoteResponse.SequenceNumber;
                //refresh Cache
                exception = string.Empty;
                bool retVal = _quotationService.GetAutoleaseQuotationResponseCacheAndDelete(quoteResponse.CompanyID, quoteResponse.ExternalId, quoteResponse.InitialExternalId, out exception);
                if (!retVal && !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to delete from cache due to " + exception;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PendingPayment;
                    _orderService.UpdateCheckout(checkoutDetails);
                    return output;
                }
                //AutoleasingWallet
                if (checkoutDetails.PaymentMethodId == (int)PaymentMethodCode.AutoleasingWallet)
                {
                    var totalAmount = CalculateTotalPaymentAmount(shoppingCartItem);
                    var balance = bank.Balance;
                    if (totalAmount > balance && !bank.PurchaseByNegative)// canot purchase by negative balance and has not enough balance
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.ThereIsNotEnoughBalance;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The bank {bank.NameEn}, doesn't have enough balance {balance}, amount to pay {totalAmount}";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        var checkoutDetailsToUpdate = _checkoutsService.GetCheckoutDetails(checkoutDetails.ReferenceId);
                        checkoutDetailsToUpdate.PolicyStatusId = (int)EPolicyStatus.PaymentFailure;
                        _orderService.UpdateCheckout(checkoutDetailsToUpdate);
                        return output;
                    }
                    bank.Balance -= Math.Round(totalAmount, 2);
                    _bankService.EditBank(bank);
                    AutoleasingWalletHistory autoleasingWalletHistory = new AutoleasingWalletHistory();
                    autoleasingWalletHistory.BankId = bank.Id;
                    autoleasingWalletHistory.ReferenceId = checkoutDetails.ReferenceId;
                    autoleasingWalletHistory.Amount = -Math.Round(totalAmount, 2);
                    autoleasingWalletHistory.Method = "AutoleasingSubmitCheckoutDetails";
                    autoleasingWalletHistory.CreatedDate = DateTime.Now;
                    autoleasingWalletHistory.CreatedBy = bank.NameEn;
                    autoleasingWalletHistory.CompanyId = checkoutDetails.InsuranceCompanyId;
                    autoleasingWalletHistory.CompanyKey = _insuranceCompanyService.GetById((int)checkoutDetails.InsuranceCompanyId)?.Key;
                    autoleasingWalletHistory.RemainingBalance = bank.Balance;
                    _autoleasingWalletHistoryRepository.Insert(autoleasingWalletHistory);

                    var updateCreateOutput = AutoleasingWalletCreateOrder(checkoutDetails);
                    output.WalletPaymentResponseModel = new WalletPaymentResponseModel();
                    output.WalletPaymentResponseModel.ReferenceId = checkoutDetails.ReferenceId;
                    output.WalletPaymentResponseModel.Status = "Succeeded";
                    output.WalletPaymentResponseModel.ErrorMessage = "Succeeded";
                    output.WalletPaymentResponseModel.NewBalance = bank.Balance.ToString();
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                }
                _policyProcessingService.InsertPolicyProcessingQueue(model.ReferenceId, model.InsuranceCompanyId, checkoutDetails.InsuranceCompanyName, model.Channel);

                /*Convert intial quotation isConverted = true  */
                if (!string.IsNullOrEmpty(quoteResponse.InitialExternalId))
                {
                    var quotationRequest = _quotationService.GetQuotationRequestData(quoteResponse.InitialExternalId);
                    if (quotationRequest == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"quotation request is null for the external: {quoteResponse.InitialExternalId}";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    if (quotationRequest.AutoleasingInitialOption == true)
                    {
                        quotationRequest.IsConverted = true;
                        _quotationService.UpdateQuotationRequest(quotationRequest);
                    }
                }
                /*end of convert intial quotation isconverted=1*/

                /* update quotation form history sitting as isPurchased */
                string vehicleId = (!string.IsNullOrEmpty(quoteResponse.CustomCardNumber)) ? quoteResponse.CustomCardNumber : quoteResponse.SequenceNumber;
                var autoleasingQuotationFormSetting = _autoleasingQuotationFormSettingsRepository.Table.Where(a => a.VehicleId == vehicleId && a.ExternalId == quoteResponse.ExternalId).OrderByDescending(a => a.CreateDate).FirstOrDefault();
                if (autoleasingQuotationFormSetting != null)
                {
                    autoleasingQuotationFormSetting.IsPurchased = true;
                    autoleasingQuotationFormSetting.ModifiedBy = log.UserId;
                    autoleasingQuotationFormSetting.ModifiedDate = DateTime.Now;
                    _autoleasingQuotationFormSettingsRepository.Update(autoleasingQuotationFormSetting);
                }
                /* end of update quotation form history sitting as isPurchased */

                /* handle leasing user data */

                var autoleasingPortalLinkModel = new Tameenk.Services.Core.Leasing.Models.AutoleasingPortalLinkModel()
                {
                    BankId = bank.Id,
                    BankKey = bank.BankKey,
                    CheckOutUserId = checkoutDetails.UserId,
                    ReferenceId = checkoutDetails.ReferenceId,
                    MainDriverId = checkoutDetails.MainDriverId,
                    VehicleId = checkoutDetails.VehicleId,
                    VehicleSequenceOrCustom = (quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard) ? quoteResponse.CustomCardNumber : quoteResponse.SequenceNumber,
                    Phone = checkoutDetails.Phone,
                    CompanyID = checkoutDetails.InsuranceCompanyId,
                    CompanyName = checkoutDetails.InsuranceCompanyName,
                    InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode,
                    CheckoutSelectedlang = checkoutDetails.SelectedLanguage ?? LanguageTwoLetterIsoCode.Ar
                };

                var response = HandleLeasingUserAccount(autoleasingPortalLinkModel);

                /* end of handle leasing user data */

                if (model.IsRenewal)
                {
                    var PolicyStatistics = _autoleasingRenewalPolicyStatistics.TableNoTracking.Where(a => a.SequenceNumber == quoteResponse.SequenceNumber).ToList();//1
                    int index = 1;
                    if (PolicyStatistics != null && PolicyStatistics.Count > 0)
                    {
                        index += PolicyStatistics.Count;
                    }

                    var data = _quotationService.GetFristYearPolicyDetails(quoteResponse.SequenceNumber);
                    AutoleasingRenewalPolicyStatistics item = new AutoleasingRenewalPolicyStatistics();
                    if (data != null)
                    {
                        item.ParentExternalId = data.ExternalId;
                        item.ParentReferenceId = data.ReferenceId;
                        item.ExternalId = model.QtRqstExtrnlId;
                        item.PaymentAmount = invoice.ExtraPremiumPrice + invoice.Vat + invoice.TotalBenefitPrice;
                        item.SequenceNumber = quoteResponse.SequenceNumber;
                        item.ReferenceId = quoteResponse.ReferenceId;
                        item.Year = index;

                        _autoleasingRenewalPolicyStatistics.Insert(item);
                    }
                }

                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                output.CheckoutModel = model;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public CheckoutOutput AddAutoleasingItemToCart(Tameenk.Core.Domain.Dtos.AddItemToCartModel model, CheckoutRequestLog log, string lang, bool autoRenewal = false)
        {
            DateTime dtStart = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            log.ReferenceId = model.ReferenceId;
            log.MethodName = "AddAutoleasingItemToCart";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            try
            {
                var user = _autoleasingUserService.GetUser(log.UserId);
                if (user == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "user not found with this is " + log.UserId;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!user.CanPurchase)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "this user is from alyousr and is blocked from purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (log.UserId == "6fd351a8-cff8-4745-838d-b2153c5f7a68")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Mubark Account is blocked from purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!autoRenewal)
                {
                    AutoleasingQuotationFormSettings quotationForm = _autoleasingQuotationFormSettingsRepository.TableNoTracking.Where(a => a.ExternalId == model.QuotaionRequestExternalId).FirstOrDefault();
                    if (quotationForm == null)
                    {
                        var initialExternalId = _quotationRequestRepository.TableNoTracking.FirstOrDefault(a => a.ExternalId == model.QuotaionRequestExternalId)?.InitialExternalId;
                        quotationForm = _autoleasingQuotationFormSettingsRepository.TableNoTracking.Where(a => a.ExternalId == initialExternalId).FirstOrDefault();
                        if (quotationForm == null)
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.NoQuotationForm;
                            output.ErrorDescription = CheckoutResources.QuotationFormNotFound;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "there is No Quotatoin Form For this External";
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                            return output;
                        }
                    }
                }

                var quotationRequest = _quotationService.GetQuotationRequestDriversInfo(model.QuotaionRequestExternalId);
                if (quotationRequest == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Quotation request is null";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var quotationResponce = quotationRequest.QuotationResponses.Where(x => x.ReferenceId == model.ReferenceId).FirstOrDefault();
                if (quotationResponce == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.initialOptionPurchasedError;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId in quotation Response not match refrenceId";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quotationResponce.AutoleasingInitialOption)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.initialOptionPurchasedError;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "initial Option can't be purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                _shoppingCartService.EmptyShoppingCart(log.UserId.ToString(), model.ReferenceId);
                bool result = false;
                if (model.Channel.ToLower() == Channel.autoleasing.ToString().ToLower() && model.IsBulk)
                {
                    result = _shoppingCartService.AddItemToCartBulkAutoleasing(log.UserId.ToString(), model.ReferenceId, Guid.Parse(model.ProductId), model.QuotaionRequestExternalId);
                }
                else
                {
                    result = _shoppingCartService.AddItemToCartIndividualAutoleasing(log.UserId.ToString(), model.ReferenceId, Guid.Parse(model.ProductId), model.SelectedProductBenfitId?.Select(b => new Product_Benefit
                    {
                        Id = b,
                        IsSelected = true
                    }).ToList());
                }
                if (!result)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToAddItemToCart;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(lang)); //ResourceManager.GetString("AccidentBenefits", CultureInfo.GetCultureInfo(lang)); //
                    output.qtRqstExtrnlId = model.QuotaionRequestExternalId;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = " _shoppingCartService.AddItemToCart is return false";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ReferenceId);
                if (shoppingCartItem == null || DateTime.Now.AddDays(-60) > shoppingCartItem.QuotationResponseCreateDateTime)
                {
                    output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    output.InvalidQoutation = true;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidQuotation;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("RefreshQuotation", new CultureInfo(model.lang));
                    output.qtRqstExtrnlId = model.QuotaionRequestExternalId;
                    output.TypeOfInsurance = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).InsuranceTypeCode.GetValueOrDefault(1);
                    output.VehicleAgencyRepair = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).VehicleAgencyRepair;
                    output.DeductibleValue = quotationRequest.QuotationResponses.FirstOrDefault(a => a.ReferenceId == model.ReferenceId).DeductibleValue;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shoppingCartItem is null or expired";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string quotaionRequestExternalId = quotationRequest?.ExternalId;
                short typeOfInsurance = shoppingCartItem.InsuranceTypeCode.GetValueOrDefault(1);
                bool? vehicleAgencyRepair = shoppingCartItem.VehicleAgencyRepair;
                int? deductibleValue = shoppingCartItem.DeductibleValue;
                log.DriverNin = quotationRequest.Driver.NIN;
                log.VehicleId = quotationRequest.Vehicle.SequenceNumber != null ? quotationRequest.Vehicle.SequenceNumber : quotationRequest.Vehicle.CustomCardNumber;

                if (shoppingCartItem.InsuranceCompanyId == 12) //Tawuniya
                {
                    var quotationOutput = ValidateTawuniyaAutoleasingQuotation(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
                        shoppingCartItem.InsuranceCompanyId, log.Channel, new Guid(log.UserId), log.UserName);
                    if (quotationOutput.ErrorCode != QuotationOutput.ErrorCodes.Success)
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("RefreshQuotation", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;

                        log.ErrorCode = (int)quotationOutput.ErrorCode;
                        log.ErrorDescription = "Tawuniya Quotation returned " + quotationOutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                if (shoppingCartItem.InsuranceCompanyId == 14) //Tawuniya
                {
                    var draftpolicyOutput = WataniyaAutoleasingDraftPolicy(quotationRequest, shoppingCartItem.ReferenceId, new Guid(model.ProductId), quotaionRequestExternalId,
                        shoppingCartItem.InsuranceCompanyId, log.Channel, new Guid(log.UserId), log.UserName);
                    if (draftpolicyOutput.ErrorCode != PolicyOutput.ErrorCodes.Success)
                    {
                        output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                        output.InvalidQoutation = true;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InsuredCityDoesNotMatchAddressCity;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("WataniyaDraftPolicyError", new CultureInfo(model.lang));
                        output.qtRqstExtrnlId = quotaionRequestExternalId;
                        output.TypeOfInsurance = typeOfInsurance;
                        output.VehicleAgencyRepair = vehicleAgencyRepair;
                        output.DeductibleValue = deductibleValue;

                        log.ErrorCode = (int)draftpolicyOutput.ErrorCode;
                        log.ErrorDescription = "Wataniya Draft Policy returned " + draftpolicyOutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.qtRqstExtrnlId = model.QuotaionRequestExternalId;
                output.ReferenceId = model.ReferenceId;
                output.ProductId = model.ProductId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                log.ResponseTimeInSeconds = output.ResponseTimeInSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public QuotationOutput ValidateTawuniyaAutoleasingQuotation(QuotationRequest quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, int? deductibleValue = null, bool automatedTest = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            QuotationOutput output = new QuotationOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel.ToString();
            checkoutlog.MethodName = "ValidateTawuniyaAutoleasingQuotation";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ReferenceId = referenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (quotationRequest == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "quotationRequest is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (productInternalId == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "productInternalId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (insuranceCompanyID == 0)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "insuranceCompanyID = " + insuranceCompanyID;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyID);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoCompanyReturned;
                    output.ErrorDescription = "No Company Returned from database";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                checkoutlog.CompanyName = insuranceCompany.Key;
                checkoutlog.CompanyId = insuranceCompany.InsuranceCompanyID;

                var tawuniyaQuotation = _quotationContext.GetTawuniyaAutoleasingQuotation(quotationRequest.ID, referenceId, productInternalId, qtRqstExtrnlId, insuranceCompany, channel, userId, userName, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, automatedTest);
                if (tawuniyaQuotation.ErrorCode != QuotationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "quotationContext.GetTawuniyaQuotation: No Returned Quotation due to " + tawuniyaQuotation.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                IInsuranceProvider provider = null;
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType != null)
                {
                    object instance = null;
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "provider is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                Product currentProduct = _productRepository.Table.Include(p => p.PriceDetails).FirstOrDefault(p => p.Id == productInternalId);
                if (currentProduct == null)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "currentProduct is null " + productInternalId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;

                tawuniyaQuotation.QuotationServiceRequest.QuotationNo = currentProduct.QuotaionNo;
                ServiceOutput results = provider.GetTawuniyaAutoleasingQuotation(tawuniyaQuotation.QuotationServiceRequest, predefinedLogInfo);

                if (results.ErrorCode != ServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = QuotationOutput.ErrorCodes.NoReturnedQuotation;
                    output.ErrorDescription = "provider.GetTawuniyaAutoleasingQuotation: No Returned Quotation due to " + results.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                output.ErrorCode = QuotationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                scope.Dispose();
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = QuotationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public bool GetCheckOutDetailsTemp3(string externalId)
        {
            try
            {
                var autoleasingQuotationResponseCache = _autoleasingQuotationResponseCache.TableNoTracking.Where(c => c.ExternalId == externalId);
                if (autoleasingQuotationResponseCache != null && autoleasingQuotationResponseCache.Any()) //May be working on it in background
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCheckOutDetailsTemp3_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " External May be working on it in background :" + externalId);
                    return false;
                }

                var quotationRequest = _quotationRequestRepository.TableNoTracking.Include(q => q.QuotationResponses).Where(q => q.ExternalId == externalId).FirstOrDefault();
                if (quotationRequest == null || quotationRequest.QuotationResponses == null || quotationRequest.QuotationResponses.Count == 0)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCheckOutDetailsTemp3_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " quotationRequest is empty for external: " + externalId);
                    return false;
                }
                foreach (var qResponse in quotationRequest.QuotationResponses)
                {
                    var quotationResponse = _quotationResponseRepository.TableNoTracking.Include(q => q.InsuranceCompany).Where(q => q.ReferenceId == qResponse.ReferenceId).FirstOrDefault();
                    var productDetails = _productRepository.TableNoTracking.Include(q => q.PriceDetails).Include(q => q.Product_Benefits).Where(q => q.ReferenceId == qResponse.ReferenceId);

                    if (quotationResponse == null || productDetails == null)
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCheckOutDetailsTemp3_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt",
                        $" quotationResponse is empty for external: {externalId}, ReferenceId : {qResponse.ReferenceId}");
                        return false;
                    }
                    Tameenk.Integration.Dto.Quotation.QuotationResponseModel responseModel = new Tameenk.Integration.Dto.Quotation.QuotationResponseModel();
                    // responseModel.Id = quotationResponse.Id;
                    responseModel.CompanyAllowAnonymous = false;
                    responseModel.AnonymousRequest = false;
                    responseModel.HasDiscount = false;
                    responseModel.RequestId = quotationResponse.RequestId;
                    responseModel.InsuranceTypeCode = 2;
                    responseModel.CreateDateTime = DateTime.Now;
                    responseModel.VehicleAgencyRepair = quotationResponse.VehicleAgencyRepair;
                    responseModel.DeductibleValue = quotationResponse.DeductibleValue;
                    responseModel.ReferenceId = quotationResponse.ReferenceId;
                    responseModel.InsuranceCompanyId = quotationResponse.InsuranceCompanyId;
                    responseModel.insuranceCompanyKey = quotationResponse.InsuranceCompany.Key;
                    responseModel.ArabicDriverName = string.Empty;
                    responseModel.EnglishDriverName = string.Empty;

                    responseModel.Products = new List<Tameenk.Integration.Dto.Quotation.ProductModel>();
                    foreach (var product in productDetails)
                    {
                        Tameenk.Integration.Dto.Quotation.ProductModel productModel = new Integration.Dto.Quotation.ProductModel();
                        productModel.Id = product.Id;
                        productModel.ExternalProductId = product.ExternalProductId;
                        productModel.QuotaionNo = product.QuotaionNo;
                        productModel.QuotationDate = product.QuotationDate;
                        productModel.QuotationExpiryDate = product.QuotationExpiryDate;
                        productModel.ProviderId = product.ProviderId;
                        productModel.ProductNameAr = product.ProductNameAr;
                        productModel.ProductNameEn = product.ProductNameEn;
                        productModel.ProductDescAr = product.ProductDescAr;
                        productModel.ProductDescEn = product.ProductDescEn;
                        productModel.ProductPrice = product.ProductPrice;
                        productModel.DeductableValue = product.DeductableValue;
                        productModel.VehicleLimitValue = product.VehicleLimitValue;
                        // productModel.QuotationResponseId = product.QuotationResponseId;
                        productModel.ProductImage = product.ProductImage;
                        productModel.InsuranceTypeCode = 2;
                        productModel.ShadowAmount = product.ShadowAmount;
                        productModel.InsurancePercentage = product.InsurancePercentage;

                        productModel.PriceDetails = new List<Tameenk.Integration.Dto.Quotation.PriceDetailModel>();
                        Tameenk.Integration.Dto.Quotation.PriceDetailModel priceDetailModel;
                        foreach (var priceDetails in product.PriceDetails)
                        {
                            priceDetailModel = new Integration.Dto.Quotation.PriceDetailModel();
                            priceDetailModel.DetailId = priceDetails.DetailId;
                            priceDetailModel.ProductID = priceDetails.ProductID;
                            priceDetailModel.PriceTypeCode = priceDetails.PriceTypeCode;
                            priceDetailModel.PriceValue = priceDetails.PriceValue;
                            priceDetailModel.PercentageValue = priceDetails.PercentageValue;

                            priceDetailModel.PriceType = new Integration.Dto.Quotation.PriceTypeModel();
                            priceDetailModel.PriceType.Code = priceDetails.PriceType.Code;
                            priceDetailModel.PriceType.EnglishDescription = priceDetails.PriceType.EnglishDescription;
                            priceDetailModel.PriceType.ArabicDescription = priceDetails.PriceType.ArabicDescription;
                            priceDetailModel.PriceType.Order = priceDetails.PriceType.Order;

                            productModel.PriceDetails.Add(priceDetailModel);
                        }

                        if (product.Product_Benefits != null && product.Product_Benefits.Any())
                        {
                            productModel.Product_Benefits = new List<Tameenk.Integration.Dto.Quotation.ProductBenefitModel>();
                            Tameenk.Integration.Dto.Quotation.ProductBenefitModel productBenefitModel;
                            foreach (var benefit in product.Product_Benefits)
                            {
                                productBenefitModel = new Integration.Dto.Quotation.ProductBenefitModel();
                                // productBenefitModel.Id = benefit.Id;
                                productBenefitModel.ProductId = benefit.ProductId;
                                productBenefitModel.BenefitId = benefit.BenefitId;
                                productBenefitModel.IsSelected = benefit.IsSelected;
                                productBenefitModel.BenefitPrice = benefit.BenefitPrice;
                                productBenefitModel.BenefitExternalId = benefit.BenefitExternalId;
                                productBenefitModel.IsReadOnly = benefit.IsReadOnly;

                                productBenefitModel.Benefit = new Integration.Dto.Quotation.BenefitModel();
                                productBenefitModel.Benefit.Code = benefit.Benefit.Code;
                                productBenefitModel.Benefit.EnglishDescription = benefit.Benefit.EnglishDescription;
                                productBenefitModel.Benefit.ArabicDescription = benefit.Benefit.ArabicDescription;

                                productModel.Product_Benefits.Add(productBenefitModel);
                            }
                        }

                        responseModel.Products.Add(productModel);
                    }

                    AutoleasingQuotationResponseCache cache = new AutoleasingQuotationResponseCache();
                    cache.InsuranceCompanyId = responseModel.InsuranceCompanyId;
                    cache.ExternalId = quotationResponse.QuotationRequest.ExternalId;
                    cache.VehicleAgencyRepair = responseModel.VehicleAgencyRepair;
                    cache.DeductibleValue = responseModel.DeductibleValue;

                    Guid userId = Guid.Empty;
                    Guid.TryParse(quotationResponse.QuotationRequest.UserId, out userId);
                    cache.UserId = userId;
                    string jsonResponse = JsonConvert.SerializeObject(responseModel);
                    cache.QuotationResponse = jsonResponse;
                    string exception = string.Empty;
                    _quotationService.InsertIntoAutoleasingQuotationResponseCache(cache, out exception);
                }
                return true;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetCheckOutDetailsTemp3_EXP_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exp.ToString());
                return false;
            }
        }


        #region Edaat
        public CheckoutOutput ExecuteEdaatPayment(string Channel, CheckoutDetail details, Tameenk.Core.Domain.Entities.Invoice invoice, string externalId, QuotationResponseDBModel quoteResponse)
        {
            var output = new CheckoutOutput();
            string EdaatRequestJson = string.Empty;
            try
            {
                EdaatConfig setting = _config.Edaat;
                if (setting == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "setting is null";
                    return output;
                }
                if (!quoteResponse.CreatedDate.HasValue)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "CreatedDate is null";
                    return output;
                }
                var tokenOutput = GetEdaatToken(setting, details, Channel, externalId, quoteResponse.NIN);
                if (tokenOutput.ErrorCode != EdaatOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.AuthorizationFailed;
                    output.ErrorDescription = "GetEdaatToken returned " + tokenOutput.ErrorDescription;
                    return output;
                }
                var companyAccountDetails = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyAccountDetails == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "company Account Not found for " + invoice.InsuranceCompanyId;
                    return output;
                }
                var subBillerOuttput = GetEdaatSubBillerRegisterationNo(setting, details, Channel, externalId, companyAccountDetails.CrNumber, quoteResponse.NIN, tokenOutput.Token);
                if (subBillerOuttput.ErrorCode != EdaatOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "GetEdaatSubBillerRegisterationNo returned " + subBillerOuttput.ErrorDescription;
                    return output;
                }
                var edaatResponseModel = new EdaatPaymentResponseModel();
                string fromDurationTime = string.Empty;
                string toDurationTime = string.Empty;
                DateTime toDurationDate = quoteResponse.CreatedDate.Value.AddHours(16);
                fromDurationTime = quoteResponse.CreatedDate.Value.Hour + ":" + quoteResponse.CreatedDate.Value.Minute;
                toDurationTime = toDurationDate.Hour + ":" + toDurationDate.Minute;
                EdaatRequest request = new EdaatRequest
                {
                    RegistrationNo = companyAccountDetails.CrNumber,
                    NationalID = quoteResponse.InsuredNationalId,
                    InsuredNationalId = quoteResponse.InsuredNationalId,
                    ReferenceId = details.ReferenceId,
                    UserId = details.UserId,
                    Conditions = "",
                    ExportToSadad = true,
                    HasValidityPeriod = true,
                    DueDate = quoteResponse.CreatedDate.Value,
                    FromDurationTime = fromDurationTime,
                    ToDurationTime = toDurationTime,
                    InternalCode = details.ReferenceId,
                    IsClientEnterpise = false,
                    TotalAmount = (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0) ? invoice.TotalPrice.Value - invoice.TotalBCareDiscount.Value : invoice.TotalPrice.Value,
                    CompanyId = details.InsuranceCompanyId.Value,
                    CompanyName = details.InsuranceCompanyName,
                    IssueDate = DateTime.Now,
                    ExpiryDate = quoteResponse.CreatedDate.Value.AddHours(16).Date,
                    SubBillerRegistrationNo = subBillerOuttput.SubBillerRegistrationNo,
                    SubBillerShareAmount = invoice.TotalCompanyAmount,
                    SubBillerSharePercentage = null
                };
                request.Products = new List<EdaatProduct>();
                var product = new EdaatProduct
                {
                    Qty = 1,
                    Price = (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0) ? (double)invoice.TotalPrice.Value - (double)invoice.TotalBCareDiscount.Value : (double)invoice.TotalPrice.Value,
                    UserId = details.UserId
                };
                if (details.SelectedInsuranceTypeCode == 1 || details.SelectedInsuranceTypeCode == 8)
                {
                    product.ProductId = setting.TPlProductId;
                }
                else if (details.SelectedInsuranceTypeCode == 2)
                {
                    product.ProductId = setting.ComprehensiveProductId;
                }
                else if (details.SelectedInsuranceTypeCode == 7)
                {
                    product.ProductId = setting.SanadPlusProductId;
                }
                else if (details.SelectedInsuranceTypeCode == 9)
                {
                    product.ProductId = setting.ODProductId;
                }
                else if (details.SelectedInsuranceTypeCode == 13)
                {
                    product.ProductId = setting.MotorPlusProductId;
                }
                request.Products.Add(product);
                request.NationalID = quoteResponse.NIN;
                request.Customers = new List<EdaatCustomer>();
                var customer = new EdaatCustomer();
                customer.DateOfBirth = quoteResponse.DateOfBirthG;
                customer.DateOfBirthHijri = quoteResponse.DateOfBirthH;
                customer.NationalID = quoteResponse.NIN;
                customer.Email = details.Email;
                customer.FirstNameAr = quoteResponse.FirstName;
                customer.FatherNameAr = quoteResponse.SecondName;
                customer.GrandFatherNameAr = quoteResponse.ThirdName;
                customer.LastNameAr = quoteResponse.LastName;
                customer.FirstNameEn = quoteResponse.EnglishFirstName;
                customer.FatherNameEn = quoteResponse.EnglishSecondName;
                customer.GrandFatherNameEn = quoteResponse.EnglishThirdName;
                customer.LastNameEn = quoteResponse.EnglishLastName;
                customer.MobileNo = Utilities.Remove966FromMobileNumber(details.Phone);
                customer.UserId = details.UserId;
                customer.CustomerRefNumber = details.ReferenceId;
                request.Customers.Add(customer);

                EdaatRequestJson = JsonConvert.SerializeObject(request);
                _edaatRepository.Insert(request);
                if (request.Id < 1)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Failed to save Edaat request";
                    return output;
                }
                var result = SubmitEdaatRequest(request, setting, tokenOutput.Token, details, Channel, externalId, quoteResponse.NIN);
                if (result == null)
                {
                    edaatResponseModel.Status = "Failed";
                    edaatResponseModel.ErrorMessage = "Failed to read/parse response from Edaat service";
                    edaatResponseModel.InvoiceNo = null;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Edaat Result is null";
                    return output;
                }
                if (result.Result == null || result.Result.Status == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = result.ErrorDescription;
                    edaatResponseModel.Status = "Failed";
                    edaatResponseModel.ErrorMessage = output.ErrorDescription;
                    edaatResponseModel.InvoiceNo = null;
                    output.EdaatPaymentResponseModel = edaatResponseModel;
                    return output;
                }
                if (!result.Result.Status.Success)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = result.ErrorDescription;
                    edaatResponseModel.Status = "Failed";
                    foreach (var msg in result.Result.Status.Message)
                        output.ErrorDescription += msg + ",";
                    edaatResponseModel.ErrorMessage = output.ErrorDescription;
                    edaatResponseModel.InvoiceNo = null;
                    output.EdaatPaymentResponseModel = edaatResponseModel;
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                edaatResponseModel.Status = "Succeeded";
                edaatResponseModel.ErrorMessage = "Succeeded";
                edaatResponseModel.InvoiceNo = result.Result.Body.InvoiceNo;
                edaatResponseModel.BillDueDate = request.ExpiryDate;
                output.EdaatPaymentResponseModel = edaatResponseModel;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Exceute Edaat failed duo to " + ex.ToString() + " , and EdaatRequestJson =" + EdaatRequestJson;
                return output;
            }
        }
        private Output<EdaatResponseDto> SubmitEdaatRequest(EdaatRequest edaatrequest, EdaatConfig edaatsetting, string token,
            CheckoutDetail checkoutDetail, string channel, string externalId, string nin)
        {
            Output<EdaatResponseDto> output = new Output<EdaatResponseDto>();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "SubmitEdaatRequest";
            log.ReferenceId = checkoutDetail.ReferenceId;
            Guid userId = Guid.Empty;
            Guid.TryParse(checkoutDetail.UserId, out userId);
            log.UserID = userId;
            log.VehicleId = checkoutDetail.VehicleId.ToString();
            log.DriverNin = nin;
            log.CompanyID = checkoutDetail.InsuranceCompanyId;
            log.CompanyName = checkoutDetail.InsuranceCompanyName;
            log.ExternalId = externalId;
            DateTime date = DateTime.Now;
            try
            {
                EdaatRequestDto edaatRequestDto = new EdaatRequestDto();
                edaatRequestDto.DueDate = edaatrequest.DueDate.ToString("yyyy-MM-dd", new CultureInfo("En"));
                edaatRequestDto.ExpiryDate = edaatrequest.ExpiryDate.ToString("yyyy-MM-dd", new CultureInfo("En"));
                edaatRequestDto.ExportToSadad = edaatrequest.ExportToSadad;
                edaatRequestDto.FromDurationTime = edaatrequest.FromDurationTime;
                edaatRequestDto.HasValidityPeriod = edaatrequest.HasValidityPeriod;
                edaatRequestDto.InternalCode = checkoutDetail.ReferenceId;
                edaatRequestDto.IsClientEnterpise = edaatrequest.IsClientEnterpise;
                edaatRequestDto.IssueDate = edaatrequest.IssueDate.ToString("yyyy-MM-dd", new CultureInfo("En"));
                edaatRequestDto.NationalID = edaatrequest.NationalID;
                edaatRequestDto.RegistrationNo = edaatrequest.RegistrationNo;
                edaatRequestDto.SubBillerRegistrationNo = edaatrequest.SubBillerRegistrationNo;
                edaatRequestDto.SubBillerShareAmount = edaatrequest.SubBillerShareAmount.Value;
                edaatRequestDto.SubBillerSharePercentage = edaatrequest.SubBillerSharePercentage;
                edaatRequestDto.ToDurationTime = edaatrequest.ToDurationTime;
                edaatRequestDto.TotalAmount = edaatrequest.TotalAmount;
                edaatRequestDto.Products = new List<EdaatProductDto>();
                var product = new EdaatProductDto
                {
                    Price = Convert.ToDouble(edaatRequestDto.TotalAmount),
                    Qty = 1,
                    ProductId = edaatrequest.Products[0].ProductId
                };
                edaatRequestDto.Products.Add(product);
                if (edaatrequest.Companys.Any())
                {
                    edaatRequestDto.Company = new EdaatCompanyDto();
                    edaatRequestDto.Company.RegistrationNo = edaatrequest.Companys[0].RegistrationNo;
                    edaatRequestDto.Company.CommissionerEmail = edaatrequest.Companys[0].CommissionerEmail;
                    edaatRequestDto.Company.CommissionerMobileNo = edaatrequest.Companys[0].CommissionerMobileNo;
                    edaatRequestDto.Company.CommissionerName = edaatrequest.Companys[0].CommissionerName;
                    edaatRequestDto.Company.CommissionerNationalId = edaatrequest.Companys[0].CommissionerNationalId;
                    edaatRequestDto.Company.CustomerRefNumber = edaatrequest.Companys[0].CustomerRefNumber;
                    edaatRequestDto.Company.NameAr = edaatrequest.Companys[0].NameAr;
                    edaatRequestDto.Company.NameEn = edaatrequest.Companys[0].NameEn;
                }
                if (edaatrequest.Customers.Any())
                {
                    edaatRequestDto.Customer = new EdaatCustomerDto();
                    edaatRequestDto.Customer.CustomerRefNumber = edaatrequest.Customers[0].CustomerRefNumber;
                    edaatRequestDto.Customer.DateOfBirth = edaatrequest.Customers[0].DateOfBirth.ToString("yyyy-MM-dd", new CultureInfo("En"));
                    edaatRequestDto.Customer.DateOfBirthHijri = edaatrequest.Customers[0].DateOfBirthHijri;
                    edaatRequestDto.Customer.Email = edaatrequest.Customers[0].Email;
                    edaatRequestDto.Customer.FatherNameAr = edaatrequest.Customers[0].FatherNameAr;
                    edaatRequestDto.Customer.FatherNameEn = edaatrequest.Customers[0].FatherNameEn;
                    edaatRequestDto.Customer.FirstNameAr = edaatrequest.Customers[0].FirstNameAr;
                    edaatRequestDto.Customer.FirstNameEn = edaatrequest.Customers[0].FirstNameEn;
                    edaatRequestDto.Customer.GrandFatherNameAr = edaatrequest.Customers[0].GrandFatherNameAr;
                    edaatRequestDto.Customer.GrandFatherNameEn = edaatrequest.Customers[0].GrandFatherNameEn;
                    edaatRequestDto.Customer.LastNameAr = edaatrequest.Customers[0].LastNameAr;
                    edaatRequestDto.Customer.LastNameEn = edaatrequest.Customers[0].LastNameEn;
                    edaatRequestDto.Customer.MobileNo = edaatrequest.Customers[0].MobileNo;
                    edaatRequestDto.Customer.NationalID = edaatrequest.Customers[0].NationalID;
                }
                var jonRequest = JsonConvert.SerializeObject(edaatRequestDto);
                log.ServiceRequest = jonRequest;
                log.ServiceURL = edaatsetting.Url;
                HttpStatusCode httpStatusCode = new HttpStatusCode();
                var response = Utilities.SendRequestJson(edaatsetting.Url, jonRequest, out httpStatusCode, token);
                log.ServiceResponse = response;
                if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.NotAuthorized;
                    output.ErrorDescription = "HttpStatusCode is Unauthorized ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.InternalServerError)
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is Internal Server Error";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode == HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.ServiceException;
                    output.ErrorDescription = " HttpStatusCode is BadRequest";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "HttpStatusCode  is not Ok as they return " + httpStatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response == null)
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.NullResult;
                    output.ErrorDescription = "Service Response is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic responseAfterDeserialize = JsonConvert.DeserializeObject(response);
                if (responseAfterDeserialize?.Status?.Success == "false")
                {
                    output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.ServiceDown;
                    output.ErrorDescription = responseAfterDeserialize?.Status?.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.Result = new EdaatResponseDto
                {
                    Body = new Body
                    {
                        InternalCode = responseAfterDeserialize?.Body?.InternalCode,
                        InvoiceNo = responseAfterDeserialize?.Body?.InvoiceNo,
                    },
                    Status = new Tameenk.Core.Domain.Dtos.Edaat.Status
                    {
                        Code = responseAfterDeserialize?.Status?.Code,
                        Success = responseAfterDeserialize?.Status?.Success,
                        Message = new List<string>()
                    }
                };
                output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                EdaatResponse res = new EdaatResponse
                {
                    Code = responseAfterDeserialize.Status?.Code,
                    InternalCode = responseAfterDeserialize.Body?.InternalCode,
                    InvoiceNo = responseAfterDeserialize.Body?.InvoiceNo,
                    UserId = checkoutDetail.UserId,
                    Success = responseAfterDeserialize.Status?.Success,
                    EdaatRequestId = edaatrequest.Id,
                    ReferenceId = checkoutDetail.ReferenceId
                };
                if (!output.Result.Status.Success)
                {
                    foreach (var item in responseAfterDeserialize.Status.Message)
                    {
                        res.Message += item + ", ";
                        output.Result.Status.Message += item + ", ";
                    }
                }
                else
                {
                    res.Message = responseAfterDeserialize.Status.Message;
                    output.Result.Status.Message.Add((string)responseAfterDeserialize.Status.Message);
                }
                _edaatResponseRepository.Insert(res);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<EdaatResponseDto>.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                var edaatResponse = new EdaatResponse
                {
                    Success = false,
                    Code = "500",
                    Message = ex.ToString(),
                    EdaatRequestId = edaatrequest.Id,
                    ReferenceId = checkoutDetail.ReferenceId
                };
                _edaatResponseRepository.Insert(edaatResponse);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        private EdaatOutput GetEdaatToken(EdaatConfig setting, CheckoutDetail details, string channel, string externalId, string Nin)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "GetEdaatToken";
            log.ReferenceId = details.ReferenceId;
            Guid userId = Guid.Empty;
            Guid.TryParse(details.UserId, out userId);
            log.UserID = userId;
            log.VehicleId = details.VehicleId.ToString();
            log.DriverNin = Nin;
            log.CompanyID = details.InsuranceCompanyId;
            log.CompanyName = details.InsuranceCompanyName;
            log.ExternalId = externalId;
            DateTime date = DateTime.Now;
            EdaatOutput output = new EdaatOutput();
            try
            {
                string userName = setting.UserName;
                string password = setting.Password;
                string LoginURL = setting.LoginUrl;
                string grantType = "password";
                log.ServiceURL = LoginURL;
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "credential is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(LoginURL))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "EdaatLoginURL is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var formParamters = new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password },
                    { "grant_type",grantType }
                };
                var client = new HttpClient();
                var req = new HttpRequestMessage(HttpMethod.Post, LoginURL) { Content = new FormUrlEncodedContent(formParamters) };
                log.ServiceRequest = JsonConvert.SerializeObject(req);
                var res = client.SendAsync(req).Result;
                if (res == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (res.Content == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(res.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content.ReadAsStringAsync().Result is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var response = res.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = response;
                if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service returned bad request.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic outputInfo = JsonConvert.DeserializeObject(response);
                if (outputInfo == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "output is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty((string)outputInfo.access_token))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "token is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ErrorCode = EdaatOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)EsalOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.Token = (string)outputInfo.access_token;
                return output;
            }
            catch (Exception e)
            {
                output.ErrorCode = EdaatOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = e.ToString();
                log.ErrorCode = (int)EsalOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = e.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }
        private EdaatOutput GetEdaatSubBillerRegisterationNo(EdaatConfig setting, CheckoutDetail details, string channel, string externalId, string crNumber, string nin, string token)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "GetEdaatSubBillerRegisterationNo";
            log.ReferenceId = details.ReferenceId;
            Guid userId = Guid.Empty;
            Guid.TryParse(details.UserId, out userId);
            log.UserID = userId;
            log.VehicleId = details.VehicleId.ToString();
            log.DriverNin = nin;
            log.CompanyID = details.InsuranceCompanyId;
            log.CompanyName = details.InsuranceCompanyName;
            log.ExternalId = externalId;
            DateTime date = DateTime.Now;
            var output = new EdaatOutput();
            try
            {
                string SubBillerUrl = setting.SubBillerUrl + crNumber;
                log.ServiceURL = SubBillerUrl;
                if (string.IsNullOrEmpty(SubBillerUrl))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = " SubBillerUrl is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var client = new HttpClient();
                var req = new HttpRequestMessage(HttpMethod.Get, SubBillerUrl);
                req.Headers.Add("Authorization", "Bearer " + token);
                log.ServiceRequest = JsonConvert.SerializeObject(req);
                var res = client.SendAsync(req).Result;
                if (res == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (res.Content == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(res.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response.Content.ReadAsStringAsync().Result is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var response = res.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = response;
                if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service returned bad request.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                dynamic outputInfo = JsonConvert.DeserializeObject(response);
                if (outputInfo == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "output is null";
                    log.ErrorCode = (int)EdaatOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (outputInfo.Body == null)
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "SubBiller Info is null";
                    log.ErrorCode = (int)EdaatOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty((string)outputInfo.Body.RegistrationNo))
                {
                    output.ErrorCode = EdaatOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "RegistrationNo is null";
                    log.ErrorCode = (int)EsalOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.ErrorCode = EdaatOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)EsalOutput.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.SubBillerRegistrationNo = (string)outputInfo.Body.RegistrationNo;
                return output;
            }
            catch (Exception e)
            {
                output.ErrorCode = EdaatOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = e.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = e.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        #endregion

        public PolicyOutput WataniyaAutoleasingDraftPolicy(QuotationRequest quotationRequest, string referenceId, Guid productInternalId, string qtRqstExtrnlId, int insuranceCompanyID, string channel, Guid userId, string userName, Guid? parentRequestId = null, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool automatedTest = false)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            PolicyOutput output = new PolicyOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = channel.ToString();
            checkoutlog.MethodName = "AutoleasingDraftPolicy";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ReferenceId = referenceId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (quotationRequest == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "quotationRequest is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(qtRqstExtrnlId))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "qtRqstExtrnlId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(referenceId))
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "referenceId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (productInternalId == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "productInternalId is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (insuranceCompanyID == 0)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "insuranceCompanyID = " + insuranceCompanyID;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyID);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = "No Company Returned from database";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                checkoutlog.CompanyName = insuranceCompany.Key;
                checkoutlog.CompanyId = insuranceCompany.InsuranceCompanyID;

                var wataniyaQuotation = _quotationContext.GetWataniyaQuotation(quotationRequest.ID, referenceId, productInternalId, qtRqstExtrnlId, insuranceCompany, channel, userId, userName, parentRequestId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue, automatedTest);
                if (wataniyaQuotation.ErrorCode != QuotationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "No Returned Quotation due to " + wataniyaQuotation.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var providerFullTypeName = string.Empty;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                IInsuranceProvider provider = null;
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType != null)
                {
                    object instance = null;
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "provider is null";
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }
                //Product currentProduct = _productRepository.Table.Include(p => p.PriceDetails).FirstOrDefault(p => p.Id == productInternalId);
                Product currentProduct = _productRepository.Table.Include(e => e.Product_Benefits).Include(e => e.PriceDetails).Where(p => p.Id == productInternalId).FirstOrDefault();

                if (currentProduct == null)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "currentProduct is null " + productInternalId;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                predefinedLogInfo.UserID = userId;
                predefinedLogInfo.UserName = userName;
                predefinedLogInfo.RequestId = parentRequestId;
                predefinedLogInfo.CompanyID = insuranceCompany.InsuranceCompanyID;
                predefinedLogInfo.InsuranceTypeCode = insuranceTypeCode;

                wataniyaQuotation.QuotationServiceRequest.QuotationNo = currentProduct.QuotaionNo;

                List<Product_Benefit> selectedBenefits = new List<Product_Benefit>();
                var includedBenefits = currentProduct.Product_Benefits.Where(a => a.IsSelected == true).ToList();
                selectedBenefits.AddRange(includedBenefits);

                var shoppingCartItemBenefits = _shoppingCartService.GetUserShoppingCartItemBenefitsByUserIdAndReferenceId(userId.ToString(), referenceId);
                if (shoppingCartItemBenefits != null && shoppingCartItemBenefits.Count > 0)
                {
                    var slectedBenefitsFromSite = currentProduct.Product_Benefits.Where(a => shoppingCartItemBenefits.Contains(a.Id)).ToList();
                    foreach (var item in slectedBenefitsFromSite)
                        item.IsSelected = true;

                    selectedBenefits.AddRange(slectedBenefitsFromSite);
                }

                currentProduct.Product_Benefits = selectedBenefits;

                ServiceOutput results = provider.GetWataniyaAutoleasingDraftpolicy(wataniyaQuotation.QuotationServiceRequest, currentProduct, predefinedLogInfo);
                if (results.ErrorCode != ServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PolicyOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "No Returned Draft Policy due to " + results.ErrorDescription;
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    scope.Dispose();
                    return output;
                }

                output.ErrorCode = PolicyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                scope.Dispose();
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = PolicyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = output.ErrorDescription;
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public List<EdaatNotificationOutput> GetAllEdaatNotificationPayment(EdaatFilter filter, out int count, out string exception, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            count = 0;
            exception = string.Empty;
            int commandTimeout = 60 * 60 * 60;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetEdaatNotificationData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                if (!string.IsNullOrWhiteSpace(filter.ReferenceId))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "referenceId", Value = filter.ReferenceId.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "invoiceNo", Value = filter.InvoiceNumber.Trim() };
                    command.Parameters.Add(InvoiceNoParameter);
                }
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter exportParameter = new SqlParameter() { ParameterName = "export", Value = filter.Export == null ? 0 : 1 };
                command.Parameters.Add(exportParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<EdaatNotificationOutput>(reader).ToList();
                reader.NextResult();
                count = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                foreach (var item in data)
                {
                    item.FullName = item.FirstNameAr + " " + item.FatherNameAr + " " + item.GrandFatherNameAr + " " + item.LastNameAr;
                }
                return data;
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

        public CheckoutOutput PaymentUsingHyperpay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId)
        {
            CheckoutOutput output = new CheckoutOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.ReferenceId = referenceId;
            log.MethodName = "PaymentUsingHyperpay";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            if (paymentMethodCode == (int)PaymentMethodCode.Mada)
                log.PaymentMethod = "Hyperpay-Mada";
            else if (paymentMethodCode == (int)PaymentMethodCode.AMEX)
                log.PaymentMethod = "Hyperpay-AMEX";
            else if (paymentMethodCode == (int)PaymentMethodCode.ApplePay)
                log.PaymentMethod = "Hyperpay-ApplePay";
            else
                log.PaymentMethod = "Hyperpay";
            log.Channel = channel;
            log.UserId = userId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"ReferenceId: {referenceId}, QtRqstExtrnlId: {QtRqstExtrnlId}, productId: {productId}, selectedProductBenfitId: {selectedProductBenfitId}, hashed: {hashed}, paymentMethodCode: {paymentMethodCode}, channel: {channel}, lang: {lang}, userId: {userId}";
            if (!string.IsNullOrEmpty(log.UserId))
            {
                var userManager = _authorizationService.GetUser(log.UserId);
                log.UserName = userManager.Result?.UserName;
            }
            string exception = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userid is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(referenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string clearText = referenceId + "_" + QtRqstExtrnlId + "_" + productId;
                if (string.IsNullOrEmpty(selectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;

                if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var checkoutDetail = hyperpayPaymentService.GetHyperpayRequestInfo(referenceId, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = exception;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (checkoutDetail == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!string.IsNullOrEmpty(checkoutDetail.CustomCardNumber))
                    log.VehicleId = checkoutDetail.CustomCardNumber;
                else
                    log.VehicleId = checkoutDetail.SequenceNumber;

                log.DriverNin = checkoutDetail.NationalId;
                log.CompanyId = checkoutDetail.InsuranceCompanyId;
                log.CompanyName = checkoutDetail.InsuranceCompanyName;

                // Check if checkout detail is already paid.
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentSuccess)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.PaidBefore;
                    output.ErrorDescription = CheckoutResources.AlreadyPaidBefore;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkoutDetail PolicyStatusId is PaymentSuccess";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var hyperpayRequest = new HyperpayRequest();
                hyperpayRequest.AccessToken = _config.Hyperpay.AccessToken;
                hyperpayRequest.EntityId = _config.Hyperpay.EntityId;
                hyperpayRequest.ReferenceId = referenceId;
                decimal amount = Math.Round(checkoutDetail.OrderItemPrice + checkoutDetail.OrderItemBenefit.Sum(oib => oib.Price * ((decimal)1.15)), 2);

                if (checkoutDetail.SelectedInsuranceTypeCode.HasValue
                    && checkoutDetail.SelectedInsuranceTypeCode == 1
                    && !string.IsNullOrEmpty(checkoutDetail.ODReference))
                {
                    var odDetails = hyperpayPaymentService.GetHyperpayRequestInfo(checkoutDetail.ODReference, out exception);
                    amount += Math.Round(odDetails.OrderItemPrice + odDetails.OrderItemBenefit.Sum(oib => oib.Price * ((decimal)1.15)), 2);
                }
                hyperpayRequest.Amount = amount;
                hyperpayRequest.CreatedDate = DateTime.Now;
                hyperpayRequest.UserId = log.UserId.ToString();
                hyperpayRequest.UserEmail = checkoutDetail.Email;
                hyperpayRequest.NationalId = checkoutDetail.NationalId;
                log.Amount = hyperpayRequest.Amount;

                //hyperpayRequest.CheckoutDetails.Add(checkoutDetail);
                HyperpayRequest response = null;
                var hyperpayOutput = hyperpayPaymentService.RequestHyperpayUrlWithSplitOption(hyperpayRequest, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.InsuranceCompanyName, checkoutDetail.Channel, checkoutDetail.MerchantTransactionId.Value, out exception);
                if (hyperpayOutput.ErrorCode == HyperSplitOutput.ErrorCodes.Success && hyperpayOutput.HyperpayRequest != null && hyperpayOutput.HyperpayRequest.ResponseCode == "000.200.100")
                {
                    response = hyperpayOutput.HyperpayRequest;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = CheckoutResources.Success;

                    output.HyperPayCheckoutId = response.ResponseId;
                    output.ReferenceId = response.ReferenceId;
                    output.CheckoutModel = new CheckoutModel();
                    output.CheckoutModel.Email = response.UserEmail;
                    output.IsCheckoutEmailVerified = checkoutDetail.IsEmailVerified.HasValue ? checkoutDetail.IsEmailVerified.Value : false;
                    output.CheckoutModel.PaymentMethodCode = paymentMethodCode;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sucess";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = CheckoutResources.InvalidPayment;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "response.ResponseCode is not success it return " + response?.ResponseCode + " sent data is " + exception;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidPayment;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " exception: " + exception;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }

        }

        public decimal CalculateTotalPaymentAmount(ShoppingCartItemDB orderItem)
        {
            return orderItem.ProductPrice + orderItem.ShoppingCartItemBenefits.Sum(oib => oib.BenefitPrice.Value * ((decimal)1.15));
        }

        public HyperPayOutput ProcessHyperpayPayment(string id, string lang, string channel, Guid userId, int paymentMethodId)
        {
            HyperPayOutput output = new HyperPayOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = userId.ToString();
            log.Channel = channel;
            log.MethodName = "HyperpayProcessPayment";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"id: {id}, lang: {lang}, channel: {channel}, userId: {userId}, paymentMethodId: {paymentMethodId}";
            string ReferenceId = string.Empty;
            string json = string.Empty;
            string exception = string.Empty;
            var paymentSucceded = false;
            string errorDescription = string.Empty;
            string merchantTransactionId = string.Empty;
            try
            {
                var response = hyperpayPaymentService.RequestHyperpayToValidResponse(id, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.HyperpayToValidResponseServiceException;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "There is an error occured due to " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperpayResponse hyperpayResponse = new HyperpayResponse();
                LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                          LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;
                json = JsonConvert.SerializeObject(response, Formatting.Indented);
                hyperpayResponse.ServiceResponse = json;

                if (Regex.IsMatch(response["result"]["code"], "^(000.000.|000.100.1|000.[36])", RegexOptions.IgnoreCase) || Regex.IsMatch(response["result"]["code"], "(000.400.0|000.400.100)"))
                {
                    paymentSucceded = true;
                }
                if (response.ContainsKey("merchantTransactionId"))
                {
                    merchantTransactionId = response["merchantTransactionId"];
                }
                if (response.ContainsKey("customParameters"))
                {
                    hyperpayResponse.ReferenceId = response["customParameters"]["bill_number"];
                    log.ReferenceId = hyperpayResponse.ReferenceId;
                    ReferenceId = hyperpayResponse.ReferenceId;
                }
                if (response.ContainsKey("result"))
                {
                    hyperpayResponse.ResponseCode = response["result"]["code"];
                    hyperpayResponse.Message = response["result"]["description"];
                    output.HyperpayResponseCode = hyperpayResponse.ResponseCode;
                }
                if (response.ContainsKey("amount"))
                {
                    hyperpayResponse.Amount = decimal.Parse(response["amount"]);
                }

                if (response.ContainsKey("buildNumber"))
                {
                    hyperpayResponse.BuildNumber = response["buildNumber"];
                }
                if (response.ContainsKey("timestamp"))
                {
                    hyperpayResponse.Timestamp = response["timestamp"];
                }
                if (response.ContainsKey("ndc"))
                {
                    hyperpayResponse.Ndc = response["ndc"];
                }
                if (response.ContainsKey("card"))
                {
                    hyperpayResponse.Last4Digits = response["card"]["last4Digits"];
                    //hyperpayResponse.Holder = response["card"]["holder"];
                    hyperpayResponse.CardBin = response["card"]["bin"];
                    hyperpayResponse.ExpiryMonth = response["card"]["expiryMonth"];
                    hyperpayResponse.ExpiryYear = response["card"]["expiryYear"];
                }
                if (response.ContainsKey("descriptor"))
                {
                    hyperpayResponse.Descriptor = response["descriptor"];
                }
                if (response.ContainsKey("id"))
                {
                    hyperpayResponse.HyperpayResponseId = response["id"];
                }
                if (response.ContainsKey("paymentBrand"))
                {
                    hyperpayResponse.PaymentBrand = response["paymentBrand"];
                }
                hyperpayResponse.CreatedDate = DateTime.Now;

                //output.HyperpayResponse = hyperpayResponse;
                output.PaymentSucceded = paymentSucceded;
                exception = string.Empty;
                if (string.IsNullOrEmpty(merchantTransactionId))
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.MerchantTransactionIdIsNullOrEmpty;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "merchantTransactionId is null or empty and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var hyperpayRequest = hyperpayPaymentService.GetHyperpayRequestByMerchantTransactionId(merchantTransactionId, out exception);
                if (hyperpayRequest == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.HyperpayRequestIsNull;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "There is no hyperpayRequest and paymentSucceded " + paymentSucceded.ToString() + "; exception:" + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                //_shoppingCartService.EmptyShoppingCart(hyperpayRequest.UserId.ToString(), hyperpayRequest.ReferenceId);

                if (paymentSucceded && hyperpayRequest.Amount != hyperpayResponse.Amount)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvalidAmount", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "amount sent in the request " + hyperpayRequest.Amount + " not like the received " + hyperpayResponse.Amount + " and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;

                }
                var checkoutDetail = _checkoutsService.GetFromCheckoutDetailsByReferenceId(hyperpayResponse.ReferenceId, out exception);
                if (checkoutDetail == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.CheckoutIsNull;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "There is no checkoutDetail";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                output.CheckoutDetail = checkoutDetail;

                int paymentId = _paymentMethodService.GetPaymentMethodIdByBrand(hyperpayResponse.PaymentBrand?.ToLower());
                if (paymentId <= 0)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidBrand;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "Invalid brand name as brand name is " + hyperpayResponse.PaymentBrand + " and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.PaymentId = paymentId;
                if (checkoutDetail.PaymentMethodId == (int)PaymentMethodCode.ApplePay)
                {
                    log.PaymentMethod = "ApplePay";
                    output.PaymentMethodId = (int)PaymentMethodCode.ApplePay;
                }
                else
                {
                    output.PaymentMethodId = checkoutDetail.PaymentMethodId.Value;
                }

                ////for apple pay calculate fees again 
                //if (paymentMethodId == 0 && checkoutDetail.PaymentMethodId.HasValue)
                //{
                //    paymentMethodId = checkoutDetail.PaymentMethodId.Value;
                //}
                if (checkoutDetail.IsCancelled)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.IsCancelled;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("RequestIsCancelled", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "Policy is cancelled and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.Available)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("PolicyPurchasedBefore", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "Success Before and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment && checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPolicyStatus;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvalidRequestStatus", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "policy status is not PendingPayment it's " + checkoutDetail.PolicyStatusId + " and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (paymentSucceded && !string.IsNullOrEmpty(checkoutDetail.DiscountCode)) //mark discount code as consumed
                {
                    _orderService.UpdateDiscountCodeToBeConsumed(checkoutDetail.VehicleId, checkoutDetail.DiscountCode, checkoutDetail.ReferenceId);
                }
                if (!string.IsNullOrEmpty(hyperpayResponse.ResponseCode) && !paymentSucceded)
                {
                    errorDescription = GetHyperpayErrorMessage(hyperpayResponse.ResponseCode, lang);
                }
                var hyperpayResponses = hyperpayPaymentService.GetFromHyperpayResponseSuccessTransaction(hyperpayRequest.Id, hyperpayResponse.HyperpayResponseId, hyperpayResponse.Amount);
                if (hyperpayResponses != null)
                {
                    if (hyperpayPaymentService.UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                    {
                        hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                        if (checkoutDetail.ODReference != null && checkoutDetail.ReferenceId != checkoutDetail.ODReference)
                        {
                            var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(checkoutDetail.ODReference, out exception);
                            if (odCheckout != null)
                            {
                                hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, paymentSucceded, channel, paymentMethodId);
                            }
                        }
                    }
                    if (paymentSucceded)
                    {
                        
                        output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("SuccessPayment", CultureInfo.GetCultureInfo(lang));
                        log.ErrorDescription = "Success";

                    }
                    else
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = errorDescription;
                        log.ErrorDescription = "Invalid Payment";
                    }
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (paymentSucceded)
                {
                    try
                    {
                        _shoppingCartService.EmptyShoppingCart(hyperpayRequest.UserId.ToString(), hyperpayRequest.ReferenceId);
                        lang = culture == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";

                        exception = string.Empty;
                        var isRenewalRequest = _checkoutsService.CheckIfQuotationIsRenewalByReferenceId(hyperpayResponse.ReferenceId, out exception);

                        var product = _productRepository.TableNoTracking.Where(a => a.Id == checkoutDetail.SelectedProductId).FirstOrDefault();
                        //var companyName = checkoutDetail.InsuranceCompanyId.HasValue
                        //            ? _insuranceCompanyService.GetInsuranceCompanyName(checkoutDetail.InsuranceCompanyId.Value, culture)
                        //            : string.Empty;
                        var companyName = checkoutDetail.InsuranceCompanyId.HasValue 
                                            ? WebResources.ResourceManager.GetString($"InsuranceCompany_{checkoutDetail.InsuranceCompanyId.Value}", CultureInfo.GetCultureInfo(lang))
                                            : string.Empty; // _insuranceCompanyService.GetInsuranceCompanyName(product.ProviderId.Value, culture);

                        string productType = string.Empty;
                        //if (checkoutDetail.InsuranceCompanyId == 12) // tawuniya
                        //{
                        //    if (product != null)
                        //    {
                        //        if (product.InsuranceTypeCode == 1)
                        //        {
                        //            productType = CheckoutResources.ResourceManager.GetString("TPL", CultureInfo.GetCultureInfo(lang));
                        //        }
                        //        else if (product.InsuranceTypeCode == 2)
                        //        {
                        //            productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
                        //            if (checkoutDetail.InsuranceCompanyId == 2)
                        //                productType = CheckoutResources.ResourceManager.GetString("COMP_ACIG", CultureInfo.GetCultureInfo(lang));
                        //            else if (checkoutDetail.InsuranceCompanyId == 17)
                        //                productType = CheckoutResources.ResourceManager.GetString("COMP_UCA", CultureInfo.GetCultureInfo(lang));
                        //            else if (checkoutDetail.InsuranceCompanyId == 5)
                        //                productType = CheckoutResources.ResourceManager.GetString("COMP_TUIC", CultureInfo.GetCultureInfo(lang));
                        //        }
                        //        else if (product.InsuranceTypeCode == 7)
                        //        {
                        //            productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
                        //        }
                        //    }
                        //}
                        //else if (product != null && checkoutDetail.InsuranceCompanyId == 20 && product.InsuranceTypeCode == 8) // Al Rajhi & Wafi Smart
                        //{
                        //    productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
                        //}
                        //else
                        //{
                        //    var productTypeData = _productTypeRepository.TableNoTracking.Where(p => p.Code == (product.InsuranceTypeCode.HasValue ? product.InsuranceTypeCode.Value : checkoutDetail.SelectedInsuranceTypeCode.Value)).FirstOrDefault();
                        //    productType = productTypeData == null
                        //            ? string.Empty
                        //            : culture == LanguageTwoLetterIsoCode.Ar ? productTypeData.ArabicDescription : productTypeData.EnglishDescription;
                        //}

                        if (product.InsuranceTypeCode == 2)
                        {
                            if (checkoutDetail.InsuranceCompanyId.Value == 2)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_ACIG", CultureInfo.GetCultureInfo(lang));
                            else if (checkoutDetail.InsuranceCompanyId.Value == 5)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_TUIC", CultureInfo.GetCultureInfo(lang));
                            else if (checkoutDetail.InsuranceCompanyId.Value == 17)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_UCA", CultureInfo.GetCultureInfo(lang));
                            //else if (checkoutDetail.InsuranceCompanyId.Value == 23)
                            //    productType = CheckoutResources.ResourceManager.GetString("COMP_TokioMarine", CultureInfo.GetCultureInfo(lang));
                            else
                                productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
                        }
                        else if (product.InsuranceTypeCode == 7)
                            productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
                        else if (product.InsuranceTypeCode == 8)
                            productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
                        //else if (product.InsuranceTypeCode == 9)
                        //    productType = CheckoutResources.ResourceManager.GetString("", CultureInfo.GetCultureInfo(lang));
                        else if (product.InsuranceTypeCode == 13)
                            productType = CheckoutResources.ResourceManager.GetString("MotorPlus", CultureInfo.GetCultureInfo(lang));
                        else
                            productType = CheckoutResources.ResourceManager.GetString("TPL_SMS", CultureInfo.GetCultureInfo(lang));
                        
                        var phoneNumber = checkoutDetail.Phone;
                        if (!isRenewalRequest)
                        {
                            var userAccount = _authorizationService.GetUserDBByID(checkoutDetail.UserId);
                            if (userAccount != null && !string.IsNullOrEmpty(userAccount.PhoneNumber))
                                phoneNumber = userAccount.PhoneNumber;
                        }

                        var amount = Math.Round(hyperpayResponse.Amount, 2);
                        var message = string.Format(WebResources.ResourceManager.GetString("ProcessPayment_SendingSMS", CultureInfo.GetCultureInfo(lang)), productType, companyName, amount);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = phoneNumber,
                            MessageBody = message,
                            Method = SMSMethod.HyperPayPaymentNotification.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = channel
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    }
                    catch
                    {

                    }
                }
                if (hyperpayPaymentService.UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                {
                    hyperpayResponse.HyperpayRequestId = hyperpayRequest.Id;
                    hyperpayPaymentService.InsertHyperpayResponse(hyperpayResponse);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    if (checkoutDetail.ODReference != null && checkoutDetail.ReferenceId != checkoutDetail.ODReference)
                    {
                        var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(checkoutDetail.ODReference, out exception);
                        if (odCheckout != null)
                        {
                            hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, paymentSucceded, channel, paymentMethodId);
                        }
                    }
                    if (paymentSucceded)
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("SuccessPayment", CultureInfo.GetCultureInfo(lang));
                        log.ErrorDescription = "Success";
                    }
                    else
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = errorDescription;
                        log.ErrorDescription = "Invalid Payment";
                    }
                    output.ReferenceId = hyperpayResponse.ReferenceId;

                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                else
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.FailedToUpdateCheckoutStatus;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = "Failed to Update Checkout Payment Status and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                if (!string.IsNullOrEmpty(ReferenceId))
                    log.ReferenceId = ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " and paymentSucceded " + paymentSucceded.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public HyperPayOutput HyperpayUpdateOrder(CheckoutDetail checkoutDetail, HyperpayResponse hyperpayResponse, int? paymentId, bool? paymentSucceded)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            HyperPayOutput output = new HyperPayOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = checkoutDetail.UserId;
            log.Channel = checkoutDetail.Channel;
            log.MethodName = "HyperpayUpdateOrder";
            log.ReferenceId = checkoutDetail?.ReferenceId;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (checkoutDetail == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "checkoutDetail Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.CompanyId = checkoutDetail.InsuranceCompanyId;
                log.CompanyName = checkoutDetail.InsuranceCompanyName;
                log.PaymentMethod = hyperpayResponse.PaymentBrand;

                if (hyperpayResponse == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "hyperpayResponse Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!paymentId.HasValue)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "paymentId Is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!paymentSucceded.HasValue)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "paymentSucceded Is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var invoice = _orderService.UpdateInvoiceCommissionsAndFees(checkoutDetail.ReferenceId, checkoutDetail.SelectedInsuranceTypeCode.Value, checkoutDetail.InsuranceCompanyId.Value, paymentId.Value);
                if (invoice == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = "Invoice is Null and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperPayNotificationInfo request = new HyperPayNotificationInfo();
                request.ReferenceId = checkoutDetail?.ReferenceId;
                request.Channel = checkoutDetail?.Channel;
                request.InsuranceCompanyId = checkoutDetail?.InsuranceCompanyId;
                request.InsuranceCompanyName = checkoutDetail?.InsuranceCompanyName;
                var loginInfo = hyperpayPaymentService.SendSplitLoginRequest(request);
                if (loginInfo.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidLogin;
                    output.ErrorDescription = "failed to login due to " + loginInfo.ErrorDescription + " and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var bcareBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == 15);
                if (bcareBankAccount == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.BcareBankAccountIsNull;
                    output.ErrorDescription = "bcareBankAccount is null and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.CompanyBankAccountIsNull;
                    output.ErrorDescription = "companyBankAccount is null and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperpayOrderModel bcareOrderModel = new HyperpayOrderModel();
                bcareOrderModel.Amount = (Math.Round(invoice.TotalBCareCommission ?? 0 + invoice.TotalBCareFees ?? 0, 2)).ToString();
                bcareOrderModel.UniqueId = hyperpayResponse.HyperpayResponseId;
                bcareOrderModel.BeneficiaryAccountId = bcareBankAccount.IBAN;
                bcareOrderModel.ConfigId = Utilities.GetAppSetting("HyperpaykeyConfigId");
                var bcareOrderOutput = hyperpayPaymentService.UpdateOrder(bcareOrderModel, hyperpayResponse.ReferenceId, checkoutDetail.Channel, loginInfo.AccessToken, hyperpayResponse.PaymentBrand?.ToLower(), true, checkoutDetail);
                if (bcareOrderOutput.ErrorCode != HyperpayUpdateOrderOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.FailedToUpdateBCareAmount;
                    output.ErrorDescription = "Failed to update bcare amount due to " + bcareOrderOutput.ErrorDescription + " and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperpayOrderModel companyOrderModel = new HyperpayOrderModel();
                companyOrderModel.Amount = Math.Round(invoice.TotalCompanyAmount ?? 0, 2).ToString();
                companyOrderModel.UniqueId = hyperpayResponse.HyperpayResponseId;
                companyOrderModel.BeneficiaryAccountId = companyBankAccount.IBAN;
                companyOrderModel.ConfigId = Utilities.GetAppSetting("HyperpaykeyConfigId");
                var companyOrderOutput = hyperpayPaymentService.UpdateOrder(companyOrderModel, hyperpayResponse.ReferenceId, checkoutDetail.Channel, loginInfo.AccessToken, hyperpayResponse.PaymentBrand?.ToLower(), false, checkoutDetail);
                if (companyOrderOutput.ErrorCode != HyperpayUpdateOrderOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.FailedToUpdateCompanyAmount;
                    output.ErrorDescription = "Failed to update company amount due to " + companyOrderOutput.ErrorDescription + " and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidData;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " and paymentSucceded " + paymentSucceded.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public WalletPaymentOutput WalletCreateOrder(CheckoutDetail checkoutDetail)
        {
            WalletPaymentOutput output = new WalletPaymentOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = checkoutDetail.UserId;
            log.Channel = checkoutDetail.Channel;
            log.MethodName = "WalletUpdateOrder";
            log.ReferenceId = checkoutDetail?.ReferenceId;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (checkoutDetail == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "checkoutDetail Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var invoice = _orderService.UpdateInvoiceCommissionsAndFees(checkoutDetail.ReferenceId, checkoutDetail.SelectedInsuranceTypeCode.Value, checkoutDetail.InsuranceCompanyId.Value, (int)PaymentMethodCode.Hyperpay);
                if (invoice == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = "Invoice is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperPayNotificationInfo request = new HyperPayNotificationInfo();
                request.ReferenceId = checkoutDetail?.ReferenceId;
                request.Channel = checkoutDetail?.Channel;
                request.InsuranceCompanyId = checkoutDetail?.InsuranceCompanyId;
                request.InsuranceCompanyName = checkoutDetail?.InsuranceCompanyName;
                var loginInfo = hyperpayPaymentService.SendSplitLoginRequest(request);
                if (loginInfo.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.InvalidLogin;
                    output.ErrorDescription = "failed to login due to " + loginInfo.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.CompanyBankAccountIsNull;
                    output.ErrorDescription = "companyBankAccount is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                HyperpayCreateOrderModel companyOrderModel = new HyperpayCreateOrderModel();
                companyOrderModel.MerchantTransactionId = checkoutDetail.ReferenceId;
                companyOrderModel.TransferOption = "7";
                companyOrderModel.Period = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                companyOrderModel.BatchDescription = $"Transfer fund to beneficiary - {companyBankAccount.IBAN}";
                companyOrderModel.ConfigId = Utilities.GetAppSetting("HyperpaykeyConfigId");

                var companyBeneficiary = new BeneficiaryRequest();
                companyBeneficiary.Name = companyBankAccount.BeneficiaryName;
                companyBeneficiary.AccountId = companyBankAccount.IBAN;
                companyBeneficiary.PayoutBeneficiaryAddress1 = companyBankAccount.Address1;
                companyBeneficiary.PayoutBeneficiaryAddress2 = companyBankAccount.Address2;
                companyBeneficiary.PayoutBeneficiaryAddress3 = companyBankAccount.Address3;
                companyBeneficiary.BankIdBIC = companyBankAccount.SWIFTCODE;
                companyBeneficiary.DebitCurrency = "SAR";
                companyBeneficiary.TransferAmount = invoice.TotalCompanyAmount?.ToString();
                companyBeneficiary.TransferCurrency = "SAR";
                companyOrderModel.Beneficiary = new List<BeneficiaryRequest>() { companyBeneficiary };

                var companyOrderOutput = hyperpayPaymentService.CreateOrder(companyOrderModel, checkoutDetail.ReferenceId, checkoutDetail.Channel, loginInfo.AccessToken, "visa");
                if (companyOrderOutput.ErrorCode != HyperpayCreateOrderOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.FailedToUpdateCompanyAmount;
                    output.ErrorDescription = "Failed to update company amount due to " + companyOrderOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                output.ErrorCode = WalletPaymentOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = WalletPaymentOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidData;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }
        public WalletPaymentOutput AutoleasingWalletCreateOrder(CheckoutDetail checkoutDetail)
        {
            WalletPaymentOutput output = new WalletPaymentOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = checkoutDetail.UserId;
            log.Channel = checkoutDetail.Channel;
            log.MethodName = "AutoleasingWalletCreateOrder";
            log.ReferenceId = checkoutDetail?.ReferenceId;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();

            try
            {
                if (checkoutDetail == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "checkoutDetail Is Null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var invoice = _orderService.UpdateInvoiceCommissionsAndFees(checkoutDetail.ReferenceId, checkoutDetail.SelectedInsuranceTypeCode.Value, checkoutDetail.InsuranceCompanyId.Value, (int)PaymentMethodCode.Hyperpay);
                if (invoice == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = "Invoice is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperPayNotificationInfo request = new HyperPayNotificationInfo();
                request.ReferenceId = checkoutDetail?.ReferenceId;
                request.Channel = checkoutDetail?.Channel;
                request.InsuranceCompanyId = checkoutDetail?.InsuranceCompanyId;
                request.InsuranceCompanyName = checkoutDetail?.InsuranceCompanyName;
                var loginInfo = hyperpayPaymentService.SendSplitLoginRequest(request);
                if (loginInfo.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.InvalidLogin;
                    output.ErrorDescription = "failed to login due to " + loginInfo.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var companyBankAccount = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyBankAccount == null)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.CompanyBankAccountIsNull;
                    output.ErrorDescription = "companyBankAccount is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                HyperpayCreateOrderModel companyOrderModel = new HyperpayCreateOrderModel();
                companyOrderModel.MerchantTransactionId = checkoutDetail.ReferenceId;
                companyOrderModel.TransferOption = "7";
                companyOrderModel.Period = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd", new CultureInfo("en-US"));
                companyOrderModel.BatchDescription = $"Transfer fund to beneficiary - {companyBankAccount.IBAN}";
                companyOrderModel.ConfigId = Utilities.GetAppSetting("HyperpaykeyConfigId");

                var companyBeneficiary = new BeneficiaryRequest();
                companyBeneficiary.Name = companyBankAccount.BeneficiaryName;
                companyBeneficiary.AccountId = companyBankAccount.IBAN;
                companyBeneficiary.PayoutBeneficiaryAddress1 = companyBankAccount.Address1;
                companyBeneficiary.PayoutBeneficiaryAddress2 = companyBankAccount.Address2;
                companyBeneficiary.PayoutBeneficiaryAddress3 = companyBankAccount.Address3;
                companyBeneficiary.BankIdBIC = companyBankAccount.SWIFTCODE;
                companyBeneficiary.DebitCurrency = "SAR";
                companyBeneficiary.TransferAmount = invoice.TotalCompanyAmount?.ToString();
                companyBeneficiary.TransferCurrency = "SAR";
                companyOrderModel.Beneficiary = new List<BeneficiaryRequest>() { companyBeneficiary };

                var companyOrderOutput = hyperpayPaymentService.CreateOrder(companyOrderModel, checkoutDetail.ReferenceId, checkoutDetail.Channel, loginInfo.AccessToken, "visa");
                if (companyOrderOutput.ErrorCode != HyperpayCreateOrderOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = WalletPaymentOutput.ErrorCodes.FailedToUpdateCompanyAmount;
                    output.ErrorDescription = "Failed to update company amount due to " + companyOrderOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                output.ErrorCode = WalletPaymentOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = WalletPaymentOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidData;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public ApplePayOutput PaymentUsingApplePay(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId)
        {
            ApplePayOutput output = new ApplePayOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.ReferenceId = referenceId;
            log.MethodName = "PaymentUsingApplePay";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.PaymentMethod = "ApplePay";
            log.Channel = channel;
            log.UserId = userId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"ReferenceId: {referenceId}, QtRqstExtrnlId: {QtRqstExtrnlId}, productId: {productId}, selectedProductBenfitId: {selectedProductBenfitId}, hashed: {hashed}, paymentMethodCode: {paymentMethodCode}, channel: {channel}, lang: {lang}, userId: {userId}";
            if (!string.IsNullOrEmpty(log.UserId))
            {
                var userManager = _authorizationService.GetUser(log.UserId);
                log.UserName = userManager.Result?.UserName;
            }
            string exception = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userid is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(referenceId))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string clearText = referenceId + "_" + QtRqstExtrnlId + "_" + productId;
                if (string.IsNullOrEmpty(selectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;

                if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var checkoutDetail = hyperpayPaymentService.GetHyperpayRequestInfo(referenceId, out exception);
                if (checkoutDetail == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.checkoutDetailIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (!string.IsNullOrEmpty(checkoutDetail.CustomCardNumber))
                    log.VehicleId = checkoutDetail.CustomCardNumber;
                else
                    log.VehicleId = checkoutDetail.SequenceNumber;

                log.DriverNin = checkoutDetail.NationalId;
                log.CompanyId = checkoutDetail.InsuranceCompanyId;
                log.CompanyName = checkoutDetail.InsuranceCompanyName;

                // Check if checkout detail is already paid.
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentSuccess)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.PaidBefore;
                    output.ErrorDescription = CheckoutResources.AlreadyPaidBefore;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkoutDetail PolicyStatusId is PaymentSuccess";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                decimal amount = Math.Round(checkoutDetail.OrderItemPrice + checkoutDetail.OrderItemBenefit.Sum(oib => oib.Price * ((decimal)1.15)), 2);
                if (checkoutDetail.DiscountValue.HasValue || checkoutDetail.DiscountPercentage.HasValue)
                {
                    var invoice = _orderService.GetInvoiceByRefrenceId(referenceId);
                    if (invoice != null && invoice.TotalBCareDiscount.HasValue)
                    {
                        amount = amount - invoice.TotalBCareDiscount.Value;
                    }
                }
                if (checkoutDetail.SelectedInsuranceTypeCode.HasValue
                    && checkoutDetail.SelectedInsuranceTypeCode == 1
                    && !string.IsNullOrEmpty(checkoutDetail.ODReference))
                {
                    var odDetails = hyperpayPaymentService.GetHyperpayRequestInfo(checkoutDetail.ODReference, out exception);
                    amount += Math.Round(odDetails.OrderItemPrice + odDetails.OrderItemBenefit.Sum(oib => oib.Price * ((decimal)1.15)), 2);
                    var odinvoice = _orderService.GetInvoiceByRefrenceId(checkoutDetail.ODReference);
                    if (odinvoice != null && odinvoice.TotalBCareDiscount.HasValue)
                    {
                        amount = amount - odinvoice.TotalBCareDiscount.Value;
                    }
                }
                var hyperpayRequest = new HyperpayRequest();
                hyperpayRequest.EntityId = _config.Hyperpay.EntityId;
                hyperpayRequest.ReferenceId = referenceId;
                hyperpayRequest.Amount = amount;
                hyperpayRequest.CreatedDate = DateTime.Now;
                hyperpayRequest.UserId = log.UserId.ToString();
                hyperpayRequest.UserEmail = checkoutDetail.Email;
                hyperpayRequest.NationalId = checkoutDetail.NationalId;
                log.Amount = hyperpayRequest.Amount;

                //hyperpayRequest.CheckoutDetails.Add(checkoutDetail);
                //HyperpayRequest response = null;
                var applePayOutput = hyperpayPaymentService.StartApplePaySession(checkoutDetail.ReferenceId, checkoutDetail.InsuranceCompanyName, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.Channel);
                if (applePayOutput.ErrorCode != ApplePayOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = CheckoutResources.InvalidPayment;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "applepay session failed to start due to " + applePayOutput.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                hyperpayPaymentService.InsertHyperpayReuest(hyperpayRequest);
                output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                output.ErrorDescription = CheckoutResources.Success;
                output.Result = applePayOutput.Result;
                output.ReferenceId = checkoutDetail.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Sucess";
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidPayment;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " exception: " + exception;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }

        }
        public ApplePayOutput ApplePayProcessPayment(string referenceId, string paymentToken, string lang, string userId)
        {
            ApplePayOutput output = new ApplePayOutput();
            output.ReferenceId = referenceId;
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.ReferenceId = referenceId;
            log.UserId = userId;
            log.MethodName = "ApplePayProcessPayment";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"referenceId: {referenceId},PaymentToken: {paymentToken}";
            string json = string.Empty;
            string exception = string.Empty;
            var paymentSucceded = false;
            string errorDescription = string.Empty;
            try
            {
                var requestInfo = hyperpayPaymentService.GetRequestInfoForApplePay(referenceId, out exception);
                if (requestInfo == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.CheckoutIsNull;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("CheckoutIsNull", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "There is no requestInfo " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.Channel = requestInfo.Channel;
                if (requestInfo.IsCancelled)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.IsCancelled;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("IsCancelled", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "checkout is cancelled";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (requestInfo.PolicyStatusId == (int)EPolicyStatus.Available)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("PaymentSuccess", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "Policy already Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (requestInfo.PolicyStatusId != (int)EPolicyStatus.PendingPayment && requestInfo.PolicyStatusId != (int)EPolicyStatus.PaymentFailure)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidPolicyStatus;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "policy status is not PendingPayment it's " + requestInfo.PolicyStatusId;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (requestInfo.TotalCompanyAmount <= 0)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.TotalCompanyAmountIsLessZero;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invoice.TotalCompanyAmount<=0 as it's " + requestInfo.TotalCompanyAmount;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if ((requestInfo.HyperpayRequestAmount + requestInfo.TotalBCareDiscount) != requestInfo.InvoiceAmount)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "HyperpayRequestAmount and InvoiceAmount are not the same";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var paymentTokenInfo = JsonConvert.DeserializeObject<ApplePayPaymnetTokenModel>(paymentToken);
                if (paymentTokenInfo == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "paymentTokenInfo is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (paymentTokenInfo.PaymentData == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "paymentTokenInfo.PaymentToken.PaymentData is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(paymentTokenInfo.PaymentData.Data))
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("InvaildRequest", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "paymentTokenInfo.PaymentToken.PaymentData.Data is null or empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                int paymentId = _paymentMethodService.GetPaymentMethodIdByBrand(paymentTokenInfo.PaymentMethod.Network?.ToLower());
                if (paymentId <= 0)
                {
                    paymentId = _paymentMethodService.GetPaymentMethodIdByBrand(paymentTokenInfo.PaymentMethod.DisplayName?.ToLower());
                }
                if (paymentId <= 0)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidBrand;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang)); ;
                    log.ErrorDescription = "Invalid brand name as Network is " + paymentTokenInfo.PaymentMethod.Network + "; and DisplayName is:" + paymentTokenInfo.PaymentMethod.DisplayName;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.PaymentMethodId = paymentId;
                var invoice = _orderService.UpdateInvoiceCommissionsAndFees(requestInfo.ReferenceId, requestInfo.SelectedInsuranceTypeCode.Value, requestInfo.InsuranceCompanyId.Value, paymentId);
                if (invoice == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvoiceIsNull;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "falied to update Invoice and it's Null";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var ApplePayPaymentOutput = hyperpayPaymentService.ApplePayPayment(requestInfo, paymentTokenInfo);
                if (ApplePayPaymentOutput.ErrorCode != ApplePayOutput.ErrorCodes.Success || ApplePayPaymentOutput.PaymnetResponseModel == null)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = ApplePayPaymentOutput.ErrorDescription;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                HyperpayResponse hyperpayResponse = new HyperpayResponse();
                hyperpayResponse.ReferenceId = referenceId;
                LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                            LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;
                json = JsonConvert.SerializeObject(ApplePayPaymentOutput.PaymnetResponseModel, Formatting.Indented);
                hyperpayResponse.ServiceResponse = json;

                if (Regex.IsMatch(ApplePayPaymentOutput.PaymnetResponseModel.Result.Code, "^(000.000.|000.100.1|000.[36])", RegexOptions.IgnoreCase) || Regex.IsMatch(ApplePayPaymentOutput.PaymnetResponseModel.Result.Code, "(000.400.0|000.400.100)"))
                {
                    paymentSucceded = true;
                }
                if (paymentSucceded && !string.IsNullOrEmpty(requestInfo.DiscountCode)) //mark discount code as consumed
                {
                    _orderService.UpdateDiscountCodeToBeConsumed(requestInfo.VehicleId, requestInfo.DiscountCode, requestInfo.ReferenceId);
                }
                output.PaymentSucceded = paymentSucceded;
                hyperpayResponse.ResponseCode = ApplePayPaymentOutput.PaymnetResponseModel.Result.Code;
                output.HyperpayResponseCode = hyperpayResponse.ResponseCode;
                hyperpayResponse.Message = ApplePayPaymentOutput.PaymnetResponseModel.Result.Description;
                hyperpayResponse.Amount = decimal.Parse(ApplePayPaymentOutput.PaymnetResponseModel.Amount);
                hyperpayResponse.BuildNumber = ApplePayPaymentOutput.PaymnetResponseModel.BuildNumber;
                hyperpayResponse.Timestamp = ApplePayPaymentOutput.PaymnetResponseModel.Timestamp;
                hyperpayResponse.Ndc = ApplePayPaymentOutput.PaymnetResponseModel.Ndc;
                if (ApplePayPaymentOutput.PaymnetResponseModel.Card != null)
                {
                    hyperpayResponse.Last4Digits = ApplePayPaymentOutput.PaymnetResponseModel.Card.Last4Digits;
                    hyperpayResponse.CardBin = ApplePayPaymentOutput.PaymnetResponseModel.Card.Bin;
                    hyperpayResponse.ExpiryMonth = ApplePayPaymentOutput.PaymnetResponseModel.Card.ExpiryMonth;
                    hyperpayResponse.ExpiryYear = ApplePayPaymentOutput.PaymnetResponseModel.Card.ExpiryYear;
                }
                hyperpayResponse.Descriptor = ApplePayPaymentOutput.PaymnetResponseModel.Descriptor;
                hyperpayResponse.HyperpayResponseId = ApplePayPaymentOutput.PaymnetResponseModel.Id;
                if (!string.IsNullOrEmpty(paymentTokenInfo.PaymentMethod.Network))
                {
                    hyperpayResponse.PaymentBrand = paymentTokenInfo.PaymentMethod.Network;
                }
                else if (!string.IsNullOrEmpty(paymentTokenInfo.PaymentMethod.DisplayName))
                {
                    hyperpayResponse.PaymentBrand = paymentTokenInfo.PaymentMethod.DisplayName;
                }
                else
                {
                    hyperpayResponse.PaymentBrand = ApplePayPaymentOutput.PaymnetResponseModel.PaymentBrand;
                }
                hyperpayResponse.CreatedDate = DateTime.Now;
                exception = string.Empty;
                //  _shoppingCartService.EmptyShoppingCart(requestInfo.UserId, referenceId);
                if (paymentSucceded && requestInfo.HyperpayRequestAmount != hyperpayResponse.Amount)
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorSecurity", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "Payment response was not the same amount as request and paymentSucceded " + paymentSucceded.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;

                }
                if (!string.IsNullOrEmpty(hyperpayResponse.ResponseCode) && !paymentSucceded)
                {
                    errorDescription = GetHyperpayErrorMessage(hyperpayResponse.ResponseCode, culture.ToString());
                }
                var hyperpayResponses = hyperpayPaymentService.GetFromHyperpayResponseSuccessTransaction(requestInfo.HyperpayRequestId, hyperpayResponse.HyperpayResponseId, hyperpayResponse.Amount);
                if (hyperpayResponses != null)
                {
                    if (hyperpayPaymentService.UpdateCheckoutWithPaymentStatus(referenceId, requestInfo.PolicyStatusId.Value, requestInfo.InsuranceCompanyId.Value, requestInfo.InsuranceCompanyName, paymentSucceded, requestInfo.Channel, requestInfo.PaymentMethodId.Value))
                    {
                        hyperpayPaymentService.UpdateHyperpayRequestStatus(requestInfo.HyperpayRequestId, hyperpayResponse.ReferenceId);
                    }
                    if (paymentSucceded)
                    {
                        output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("PaymentSuccess", CultureInfo.GetCultureInfo(lang)); ;
                        log.ErrorDescription = "Success";
                    }
                    else
                    {
                        output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = errorDescription;// CheckoutResources.ResourceManager.GetString("InvalidPayment", CultureInfo.GetCultureInfo(lang));
                        log.ErrorDescription = "Invalid Payment";
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (paymentSucceded)
                {
                    try
                    {
                        _shoppingCartService.EmptyShoppingCart(requestInfo.UserId, referenceId);

                        exception = string.Empty;
                        var isRenewalRequest = _checkoutsService.CheckIfQuotationIsRenewalByReferenceId(hyperpayResponse.ReferenceId, out exception);

                        lang = culture == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";
                        //var companyName = lang == "en" ? requestInfo.NameEN : requestInfo.NameAR;
                        var companyName = requestInfo.InsuranceCompanyId.HasValue
                                            ? WebResources.ResourceManager.GetString($"InsuranceCompany_{requestInfo.InsuranceCompanyId}", CultureInfo.GetCultureInfo(lang))
                                            : string.Empty;
                        
                        string productType = string.Empty;
                        if (requestInfo.ProductInsuranceTypeCode == 2)
                        {
                            if (requestInfo.InsuranceCompanyId.Value == 2)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_ACIG", CultureInfo.GetCultureInfo(lang));
                            else if (requestInfo.InsuranceCompanyId.Value == 5)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_TUIC", CultureInfo.GetCultureInfo(lang));
                            else if (requestInfo.InsuranceCompanyId.Value == 17)
                                productType = CheckoutResources.ResourceManager.GetString("COMP_UCA", CultureInfo.GetCultureInfo(lang));
                            //else if (requestInfo.InsuranceCompanyId.Value == 23)
                            //    productType = CheckoutResources.ResourceManager.GetString("COMP_TokioMarine", CultureInfo.GetCultureInfo(lang));
                            else
                                productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
                        }
                        else if (requestInfo.ProductInsuranceTypeCode == 7)
                            productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
                        else if (requestInfo.ProductInsuranceTypeCode == 8)
                            productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
                        //else if (requestInfo.ProductInsuranceTypeCode == 9)
                        //    productType = CheckoutResources.ResourceManager.GetString("", CultureInfo.GetCultureInfo(lang));
                        else if (requestInfo.ProductInsuranceTypeCode == 13)
                            productType = CheckoutResources.ResourceManager.GetString("MotorPlus", CultureInfo.GetCultureInfo(lang));
                        else
                            productType = CheckoutResources.ResourceManager.GetString("TPL_SMS", CultureInfo.GetCultureInfo(lang));
                        
                        var phoneNumber = requestInfo.Phone;
                        if (!isRenewalRequest)
                        {
                            var userAccount = _authorizationService.GetUserDBByID(requestInfo.UserId);
                            if (userAccount != null && !string.IsNullOrEmpty(userAccount.PhoneNumber))
                                phoneNumber = userAccount.PhoneNumber;
                        }

                        var amount = Math.Round(hyperpayResponse.Amount, 2);
                        var message = string.Format(WebResources.ResourceManager.GetString("ProcessPayment_SendingSMS", CultureInfo.GetCultureInfo(lang)), productType, companyName, amount);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = phoneNumber,
                            MessageBody = message,
                            Method = SMSMethod.ApplePayPaymentNotification.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = requestInfo.Channel
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    }
                    catch
                    {

                    }
                }
                if (hyperpayPaymentService.UpdateCheckoutWithPaymentStatus(referenceId, requestInfo.PolicyStatusId.Value, requestInfo.InsuranceCompanyId.Value, requestInfo.InsuranceCompanyName, paymentSucceded, requestInfo.Channel, requestInfo.PaymentMethodId.Value))
                {
                    hyperpayResponse.HyperpayRequestId = requestInfo.HyperpayRequestId;
                    hyperpayPaymentService.InsertHyperpayResponse(hyperpayResponse);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(requestInfo.HyperpayRequestId, hyperpayResponse.ReferenceId);
                    if (requestInfo.ODReference != null && requestInfo.ReferenceId != requestInfo.ODReference)
                    {
                        var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(requestInfo.ODReference, out exception);
                        if (odCheckout != null)
                        {
                            hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, paymentSucceded, requestInfo.Channel, requestInfo.PaymentMethodId.Value);
                        }
                    }
                    if (paymentSucceded)
                    {
                        output.ErrorCode = ApplePayOutput.ErrorCodes.Success;
                        output.ErrorDescription = CheckoutResources.ResourceManager.GetString("PaymentSuccess", CultureInfo.GetCultureInfo(lang));
                        log.ErrorDescription = "Success";
                    }
                    else
                    {
                        output.ErrorCode = ApplePayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = errorDescription;//CheckoutResources.ResourceManager.GetString("InvalidPayment", CultureInfo.GetCultureInfo(lang));
                        log.ErrorDescription = "Invalid Payment";
                    }

                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                else
                {
                    output.ErrorCode = ApplePayOutput.ErrorCodes.FailedToUpdateCheckoutStatus;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                    log.ErrorDescription = "Failed to Update Checkout Payment Status and paymentSucceded " + paymentSucceded.ToString(); ;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = ApplePayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                log.ReferenceId = referenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " and paymentSucceded " + paymentSucceded.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        string GetHyperpayErrorMessage(string responseCode, string lang)
        {
            string errorDescription = string.Empty;
            try
            {
                string errorMessageResourceName = "Error_" + responseCode.Replace(".", string.Empty).Trim();
                errorDescription = HyperPayResource.ResourceManager.GetString(errorMessageResourceName, CultureInfo.GetCultureInfo(lang));
                if (string.IsNullOrEmpty(errorDescription))
                {
                    errorDescription = HyperPayResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                }
                return errorDescription;
            }
            catch
            {
                return HyperPayResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
            }
        }
        public CheckoutOutput ResendVerifyCode(string userId, string phoneNumber, string lang)
        {
            CheckoutOutput output = new CheckoutOutput();
            phoneNumber = Utilities.ValidatePhoneNumber(phoneNumber.Trim());
            DateTime dtBeforeCalling = DateTime.Now;
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.MethodName = "ResendVerifyCode";
            checkoutlog.UserId = userId.ToString();
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.ServiceRequest = JsonConvert.SerializeObject(new { userId, phoneNumber, lang });
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;
                    output.ErrorDescription = "user is null or empty";

                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;
                    output.ErrorDescription = "phoneNumber is null";

                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var user = _authorizationService.GetUserDBByID(userId.ToString());
                if (user == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.AnonymousUser;
                    output.ErrorDescription = "user is null";

                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var checkoutUser = _orderService.GetByUserIdAndPhoneNumber(Guid.Parse(userId), phoneNumber);
                if (checkoutUser == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "checkoutUser is null";

                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = output.ErrorDescription;
                    checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(userId, checkoutUser.ReferenceId);
                if (shoppingCartItem == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = "shoppingCartItem is null";
                    return output;
                }
                decimal policyPrice = _shoppingCartService.CalculateShoppingCartTotal(shoppingCartItem);

                Random rnd = new Random();
                int verifyCode = rnd.Next(1000, 9999);
                _orderService.CreateCheckoutUser(new CheckoutUsers()
                {
                    UserId = Guid.Parse(userId),
                    PhoneNumber = Utilities.ValidatePhoneNumber(phoneNumber.Trim()),
                    VerificationCode = verifyCode,
                    UserEmail = checkoutUser.UserEmail,
                    ReferenceId = checkoutUser.ReferenceId,
                    CreatedDate = DateTime.Now,
                    Nin = checkoutUser.Nin
                });

                checkoutlog.DriverNin = checkoutUser.Nin;
                checkoutlog.ReferenceId = checkoutUser.ReferenceId;
                checkoutlog.UserName = checkoutUser.UserEmail;
                if (string.IsNullOrEmpty(lang) || lang.ToLower() == "undefined")
                    lang = "ar";
                string smsMessage = string.Format(CheckoutResources.VerificationCodeMessage, verifyCode) + string.Format(" {0}: {1} {2}", CheckoutResources.PolicyPrice, policyPrice.ToString("0.00"), CheckoutResources.SAR);
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneNumber,
                    MessageBody = smsMessage,
                    Method = SMSMethod.ResendOTP.ToString(),
                    Module = Module.Vehicle.ToString()
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);

                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";

                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "an error has occured";

                checkoutlog.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                checkoutlog.ErrorDescription = exp.ToString();
                checkoutlog.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }

        public List<EdaatRequest> GetEdaatRequestsByNationalID(string nationalID, DateTime startDate, DateTime endDate, out string exception)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetEdaatRequestsByNationalID";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "nationalID", Value = nationalID };
                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = startDate };
                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = endDate };
                command.Parameters.Add(nationalIDParameter);
                command.Parameters.Add(startDateParameter);
                command.Parameters.Add(endDateParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<EdaatRequest> requests = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<EdaatRequest>(reader).ToList();
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
        public HyperPayOutput HyperpayProcessPaymentByRetrialMechanism(int id, string referenceId, string channel, Guid userId, int paymentMethodId, Guid? merchantTransaction)
        {
            HyperPayOutput output = new HyperPayOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = userId.ToString();
            log.Channel = channel;
            log.MethodName = "HyperpayProcessPaymentByRetrialMechanism";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"id: {id}, referenceId: {referenceId}, channel: {channel}, userId: {userId}";
            string ReferenceId = string.Empty;
            string json = string.Empty;
            string exception = string.Empty;
            var paymentSucceded = false;
            log.ReferenceId = ReferenceId;
            string merchantTransactionId = string.Empty;
            try
            {
                var hyperpayRequest = hyperpayPaymentService.GetById(id);
                if (hyperpayRequest == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.HyperpayRequestIsNull;
                    output.ErrorDescription = "hyperpayRequest is null";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var response = hyperpayPaymentService.Search(merchantTransaction.HasValue ? merchantTransaction.Value.ToString() : referenceId, channel, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "There is an error occured due to " + exception;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (response == null || response.Count() == 0)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "response is null or count is 0";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                json = JsonConvert.SerializeObject(response, Formatting.Indented);
                if (response["result"]["code"] == "700.400.580")
                {
                    log.ReferenceId = referenceId;
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPayment;
                    output.ErrorDescription = "transaction serach service return error " + response["result"]["description"];
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                    hyperpayRequest.StatusDescription = response["result"]["description"];
                    hyperpayRequest.StatusJsonResponse = json;
                    hyperpayRequest.ModifiedDate = DateTime.Now;
                    hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                    return output;
                }
                HyperpayResponse hyperpayResponse = new HyperpayResponse();
                hyperpayResponse.ServiceResponse = json;
                if (Regex.IsMatch(response["payments"][0]["result"]["code"], "^(000.000.|000.100.1|000.[36])", RegexOptions.IgnoreCase) || Regex.IsMatch(response["result"]["code"], "(000.400.0|000.400.100)"))
                {
                    paymentSucceded = true;
                }
                if (response["payments"][0].ContainsKey("merchantTransactionId"))
                {
                    merchantTransactionId = response["payments"][0]["merchantTransactionId"];
                }
                if (response["payments"][0].ContainsKey("customParameters"))
                {
                    hyperpayResponse.ReferenceId = response["payments"][0]["customParameters"]["bill_number"];
                    log.ReferenceId = hyperpayResponse.ReferenceId;
                    ReferenceId = hyperpayResponse.ReferenceId;
                }
                if (response["payments"][0].ContainsKey("result"))
                {
                    hyperpayResponse.ResponseCode = response["payments"][0]["result"]["code"];
                    hyperpayResponse.Message = response["payments"][0]["result"]["description"];
                }
                if (response["payments"][0].ContainsKey("amount"))
                {
                    hyperpayResponse.Amount = decimal.Parse(response["payments"][0]["amount"]);
                }
                if (response.ContainsKey("buildNumber"))
                {
                    hyperpayResponse.BuildNumber = response["buildNumber"];
                }
                if (response.ContainsKey("timestamp"))
                {
                    hyperpayResponse.Timestamp = response["timestamp"];
                }
                if (response.ContainsKey("ndc"))
                {
                    hyperpayResponse.Ndc = response["ndc"];
                }
                if (response["payments"][0].ContainsKey("card"))
                {
                    if (response["payments"][0]["card"].ContainsKey("last4Digits"))
                    {
                        hyperpayResponse.Last4Digits = response["payments"][0]["card"]["last4Digits"];
                    }
                    if (response["payments"][0]["card"].ContainsKey("holder"))
                    {
                        hyperpayResponse.Holder = response["payments"][0]["card"]["holder"];
                    }
                    if (response["payments"][0]["card"].ContainsKey("bin"))
                    {
                        hyperpayResponse.CardBin = response["payments"][0]["card"]["bin"];
                    }
                    if (response["payments"][0]["card"].ContainsKey("expiryMonth"))
                    {
                        hyperpayResponse.ExpiryMonth = response["payments"][0]["card"]["expiryMonth"];
                    }
                    if (response["payments"][0]["card"].ContainsKey("expiryYear"))
                    {
                        hyperpayResponse.ExpiryYear = response["payments"][0]["card"]["expiryYear"];
                    }
                }
                if (response["payments"][0].ContainsKey("descriptor"))
                {
                    hyperpayResponse.Descriptor = response["payments"][0]["descriptor"];
                }
                if (response["payments"][0].ContainsKey("id"))
                {
                    hyperpayResponse.HyperpayResponseId = response["payments"][0]["id"];
                }
                if (response["payments"][0].ContainsKey("paymentBrand"))
                {
                    hyperpayResponse.PaymentBrand = response["payments"][0]["paymentBrand"];
                }
                hyperpayResponse.CreatedDate = DateTime.Now;
                output.PaymentSucceded = paymentSucceded;
                if (string.IsNullOrEmpty(hyperpayResponse.ReferenceId))
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "ReferenceId is null or empty";
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                    hyperpayRequest.StatusDescription = "ReferenceId is null or empty";
                    hyperpayRequest.StatusJsonResponse = json;
                    hyperpayRequest.ModifiedDate = DateTime.Now;
                    hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                    return output;
                }
                if (merchantTransaction != hyperpayRequest.MerchantTransactionId)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidValue;
                    output.ErrorDescription = "BCare MerchantTransactionId:" + hyperpayRequest.MerchantTransactionId + " is not equal hyperpay merchantTransaction:" + merchantTransaction;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayRequest.StatusDescription = "invalid MerchantTransactionId";
                    hyperpayRequest.StatusJsonResponse = json;
                    hyperpayRequest.ModifiedDate = DateTime.Now;
                    hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                    return output;
                }
                _shoppingCartService.EmptyShoppingCart(hyperpayRequest.UserId.ToString(), hyperpayRequest.ReferenceId);
                exception = string.Empty;
                var checkoutDetail = _checkoutsService.GetFromCheckoutDetailsByReferenceId(hyperpayResponse.ReferenceId, out exception);
                if (checkoutDetail == null)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.CheckoutIsNull;
                    output.ErrorDescription = "checkoutDetail is null";
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayRequest.StatusDescription = "checkoutDetail is null";
                    hyperpayRequest.StatusJsonResponse = json;
                    hyperpayRequest.ModifiedDate = DateTime.Now;
                    hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                    return output;
                }
                output.CheckoutDetail = checkoutDetail;
                if (checkoutDetail.IsCancelled)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.IsCancelled;
                    output.ErrorDescription = "Policy is cancelled and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    return output;
                }
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.Available)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success Before and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    return output;
                }
                if (checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment && checkoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPolicyStatus;
                    output.ErrorDescription = "policy status is not PendingPayment it's " + checkoutDetail.PolicyStatusId + " and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (paymentSucceded && hyperpayRequest.Amount != hyperpayResponse.Amount)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = "amount not the same:" + hyperpayRequest.Amount + " and:" + hyperpayResponse.Amount;
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    return output;
                }
                int paymentId = _paymentMethodService.GetPaymentMethodIdByBrand(hyperpayResponse.PaymentBrand?.ToLower());
                if (paymentId <= 0)
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidBrand;
                    output.ErrorDescription = "Invalid brand name:" + hyperpayResponse.PaymentBrand + " paymentSucceded:" + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    return output;
                }
                output.PaymentId = paymentId;
                if (checkoutDetail.PaymentMethodId == (int)PaymentMethodCode.ApplePay)
                {
                    log.PaymentMethod = "ApplePay";
                    output.PaymentMethodId = (int)PaymentMethodCode.ApplePay;
                }
                else
                {
                    output.PaymentMethodId = checkoutDetail.PaymentMethodId.Value;
                }
                if (paymentSucceded && !string.IsNullOrEmpty(checkoutDetail.DiscountCode)) //mark discount code as consumed
                {
                    _orderService.UpdateDiscountCodeToBeConsumed(checkoutDetail.VehicleId, checkoutDetail.DiscountCode, checkoutDetail.ReferenceId);
                }
                var hyperpayResponses = hyperpayPaymentService.GetFromHyperpayResponseSuccessTransaction(id, hyperpayResponse.HyperpayResponseId, hyperpayResponse.Amount);
                if (hyperpayResponses != null)
                {
                    if (hyperpayPaymentService.UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                    {
                        hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                        if (checkoutDetail.ODReference != null && checkoutDetail.ReferenceId != checkoutDetail.ODReference)
                        {
                            var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(checkoutDetail.ODReference, out exception);
                            if (odCheckout != null)
                            {
                                hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, paymentSucceded, channel, paymentMethodId);
                            }
                        }
                    }
                    if (paymentSucceded)
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                    }
                    else
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = "Invalid Payment";
                    }
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (paymentSucceded)
                {
                    try
                    {
                        string lang = checkoutDetail.SelectedLanguage == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";
                        var culture = checkoutDetail.SelectedLanguage.HasValue ? checkoutDetail.SelectedLanguage.Value : LanguageTwoLetterIsoCode.Ar;
                        var product = _productRepository.TableNoTracking.Where(a => a.Id == checkoutDetail.SelectedProductId).FirstOrDefault();
                        if (product != null)
                        {
                            exception = string.Empty;
                            var isRenewalRequest = _checkoutsService.CheckIfQuotationIsRenewalByReferenceId(hyperpayResponse.ReferenceId, out exception);

                            var companyName = WebResources.ResourceManager.GetString($"InsuranceCompany_{product.ProviderId.Value}", CultureInfo.GetCultureInfo(lang)); // _insuranceCompanyService.GetInsuranceCompanyName(product.ProviderId.Value, culture);
                            string productType = string.Empty;
                            if (product.InsuranceTypeCode == 2)
                            {
                                if (checkoutDetail.InsuranceCompanyId.Value == 2)
                                    productType = CheckoutResources.ResourceManager.GetString("COMP_ACIG", CultureInfo.GetCultureInfo(lang));
                                else if (checkoutDetail.InsuranceCompanyId.Value == 5)
                                    productType = CheckoutResources.ResourceManager.GetString("COMP_TUIC", CultureInfo.GetCultureInfo(lang));
                                else if (checkoutDetail.InsuranceCompanyId.Value == 17)
                                    productType = CheckoutResources.ResourceManager.GetString("COMP_UCA", CultureInfo.GetCultureInfo(lang));
                                //else if (checkoutDetail.InsuranceCompanyId.Value == 23)
                                //    productType = CheckoutResources.ResourceManager.GetString("COMP_TokioMarine", CultureInfo.GetCultureInfo(lang));
                                else
                                    productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
                            }
                            else if (product.InsuranceTypeCode == 7)
                                productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
                            else if (product.InsuranceTypeCode == 8)
                                productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
                            //else if (product.InsuranceTypeCode == 9)
                            //    productType = CheckoutResources.ResourceManager.GetString("", CultureInfo.GetCultureInfo(lang));
                            else if (product.InsuranceTypeCode == 13)
                                productType = CheckoutResources.ResourceManager.GetString("MotorPlus", CultureInfo.GetCultureInfo(lang));
                            else
                                productType = CheckoutResources.ResourceManager.GetString("TPL_SMS", CultureInfo.GetCultureInfo(lang));

                            var phoneNumber = checkoutDetail.Phone;
                            if (!isRenewalRequest)
                            {
                                var userAccount = _authorizationService.GetUserDBByID(checkoutDetail.UserId);
                                if (userAccount != null && !string.IsNullOrEmpty(userAccount.PhoneNumber))
                                    phoneNumber = userAccount.PhoneNumber;
                            }

                            var amount = Math.Round(hyperpayResponse.Amount, 2);
                            var message = string.Format(WebResources.ResourceManager.GetString("ProcessPayment_SendingSMS", CultureInfo.GetCultureInfo(lang)), productType, companyName, amount);
                            var smsModel = new SMSModel()
                            {
                                PhoneNumber = phoneNumber,
                                MessageBody = message,
                                Method = SMSMethod.HyperPayPaymentNotification.ToString(),
                                Module = Module.Vehicle.ToString(),
                                Channel = channel
                            };
                            _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        }
                    }
                    catch
                    {

                    }
                }
                if (hyperpayPaymentService.UpdateCheckoutPaymentStatus(checkoutDetail, paymentSucceded, channel, paymentMethodId))
                {
                    hyperpayResponse.HyperpayRequestId = id;
                    hyperpayPaymentService.InsertHyperpayResponse(hyperpayResponse);
                    hyperpayPaymentService.UpdateHyperpayRequestStatus(hyperpayRequest.Id, hyperpayResponse.ReferenceId);
                    if (checkoutDetail.ODReference != null && checkoutDetail.ReferenceId != checkoutDetail.ODReference)
                    {
                        var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(checkoutDetail.ODReference, out exception);
                        if (odCheckout != null)
                        {
                            hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, paymentSucceded, channel, paymentMethodId);
                        }
                    }
                    if (paymentSucceded)
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                    }
                    else
                    {
                        output.ErrorCode = HyperPayOutput.ErrorCodes.InvalidPayment;
                        output.ErrorDescription = "Invalid Payment";
                    }
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                else
                {
                    output.ErrorCode = HyperPayOutput.ErrorCodes.FailedToUpdateCheckoutStatus;
                    output.ErrorDescription = "Failed to Update Checkout Payment Status and paymentSucceded " + paymentSucceded.ToString();
                    output.ReferenceId = hyperpayResponse.ReferenceId;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = HyperPayOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidData;
                if (!string.IsNullOrEmpty(ReferenceId))
                    log.ReferenceId = ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        #region Admin Purchase Benefit
        public AddBenefitOutput PurchaseVechileBenefit(Models.Checkout.PurchaseBenefitModel model, string UserId, string userName)
        {
            AddBenefitOutput output = new AddBenefitOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.Channel = model?.Channel.ToString();
            log.MethodName = "PurchaseVechileBenefit";
            if (!string.IsNullOrEmpty(userName))
            {
                log.UserId = UserId;
                log.UserName = userName;
            }
            try
            {
                if (model == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.InvaildRequest;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "requestModel Is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.ReferenceId))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ParkingLocationIdIsNull;
                    output.ErrorDescription = "ReferenceId is required";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is required";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var request = _policyModificationRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
                if (request == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "request  Not exist ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var insuranceCompany = _insuranceCompanyService.GetById(request.InsuranceCompanyId.Value);
                if (insuranceCompany == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Insurance company Id is null";
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
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = request.QuotationReferenceId
                };
                //Calculate total ammount of benefits 
                CheckoutDetail c = _checkoutDetailrepository.Table.FirstOrDefault(x => x.ReferenceId == request.QuotationReferenceId);
                var orderItems = _orderItemRepository.TableNoTracking.Where(x => x.CheckoutDetailReferenceId == request.QuotationReferenceId).FirstOrDefault();
                decimal totalAmmount = 0;
                if (orderItems != null)
                {
                    var orderBenefits = _orderItemBenefitRepository.TableNoTracking.Where(x => x.OrderItemId == orderItems.Id);
                    foreach (var benefit in model.Benefits)
                    {
                        if (orderBenefits != null && orderBenefits.Count() != 0)
                        {
                            if (orderBenefits.Any(x => x.BenefitExternalId == benefit.BenefitId))
                            {
                                model.Benefits.Remove(benefit);
                            }
                            else
                            {
                                totalAmmount += _policyAdditionalBenefitRepository.TableNoTracking.Where(x => x.BenefitId == benefit.BenefitId).Select(x => x.BenefitPrice).FirstOrDefault().Value * 1.15M;
                            }
                        }
                    }

                }
                PurchaseBenefitRequest serviceRequest = new PurchaseBenefitRequest();
                serviceRequest.PolicyNo = request.PolicyNo;
                serviceRequest.ReferenceId = request.ReferenceId;
                serviceRequest.PaymentAmount = (double)totalAmmount;
                serviceRequest.PaymentBillNumber = request.InvoiceNo.ToString();
                serviceRequest.Benefits = model.Benefits;
                IInsuranceProvider provider = GetProvider(insuranceCompany, request.InsuranceTypeCode.Value);
                if (provider == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "provider is null";
                    log.ErrorDescription = "provider is null";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                var results = provider.PurchaseVechileBenefit(serviceRequest, predefinedLogInfo);
                if (results == null)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Service Return Null ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Service Return Null ";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (results.StatusCode != (int)AddBenefitOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceDown;
                    output.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to get Response as status code =  " + results.StatusCode;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                string fileURL = results.EndorsementFileUrl;
                fileURL = fileURL.Replace(@"\\", @"//");
                fileURL = fileURL.Replace(@"\", @"/");
                byte[] bytes = null;
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    bytes = client.DownloadData(fileURL);
                }

                var isAutoleasingPolicy = c.Channel == Channel.autoleasing.ToString().ToLower() ? true : false;
                string filePath = Utilities.SaveCompanyFileFromDashboard(c.ReferenceId, bytes, insuranceCompany.Key, false,
                        _config.RemoteServerInfo.UseNetworkDownload,
                        _config.RemoteServerInfo.DomainName,
                        _config.RemoteServerInfo.ServerIP,
                        _config.RemoteServerInfo.ServerUserName,
                        _config.RemoteServerInfo.ServerPassword,
                        isAutoleasingPolicy,
                        out string exception);
                if (!string.IsNullOrWhiteSpace(exception))
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "failed to save file";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to save file duo to exception : " + exception;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                Endorsment endorsment = new Endorsment()
                {
                    FilePath = filePath,
                    InsurranceCompanyId = insuranceCompany.InsuranceCompanyID,
                    PolicyModificationRequestId = request.Id,
                    ReferenceId = request.ReferenceId,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    UserAgent = log.UserAgent,
                    UserIP = log.UserIP,
                    CreatedDate = DateTime.Now,
                    QuotationReferenceId = request.QuotationReferenceId
                };
                _endormentRepository.Insert(endorsment);
                if (endorsment.Id < 1)
                {
                    output.ErrorCode = AddBenefitOutput.ErrorCodes.DriverDataError;
                    output.ErrorDescription = "Failed to insert Endorsment request";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to insert Endorsment request";
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                List<EndorsmentBenefit> endorsmentBenefits = new List<EndorsmentBenefit>();
                foreach (var benefit in model.Benefits)
                {
                    EndorsmentBenefit endorsmentBenefit = new EndorsmentBenefit()
                    {
                        BenefitId = benefit.BenefitId,
                        CreatedDate = DateTime.Now,
                        EndorsmentId = endorsment.Id,
                        QuotationReferenceId = endorsment.QuotationReferenceId,
                        ReferenceId = endorsment.ReferenceId
                    };
                    endorsmentBenefits.Add(endorsmentBenefit);
                }
                _endormentBenefitRepository.Insert(endorsmentBenefits);
                output.ErrorCode = AddBenefitOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = AddBenefitOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Failed to Request Purchase benefits";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                return output;
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

        #region admin purchase vechile driver 
        public AddDriverOutput PurchaseVechileDriver(Models.Checkout.PurchaseDriverModel model, string UserId, string userName)
        {
            AddDriverOutput output = new AddDriverOutput();
            PolicyModificationLog log = new PolicyModificationLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
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
                    output.ErrorDescription = "Request Model is Null ";
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
                var request = _policyModificationRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.ReferenceId == model.ReferenceId);
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
                    output.ErrorDescription = "Insurance company Id is null";
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
                    ServerIP = log.ServerIP,
                    CompanyID = log.CompanyId,
                    CompanyName = log.CompanyName,
                    ReferenceId = request.QuotationReferenceId
                };
                predefinedLogInfo.DriverNin = request.Nin;
                PurchaseDriverRequest serviceRequest = new PurchaseDriverRequest();
                serviceRequest.PolicyNo = request.PolicyNo;
                serviceRequest.ReferenceId = request.ReferenceId;
                serviceRequest.PaymentAmount = request.TotalAmount.Value;
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
                var results = provider.PurchaseVechileDriver(serviceRequest, predefinedLogInfo);
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
                QuotationRequest quotationRequest = GetQuotationRequest(request.PolicyNo, request.InsuranceCompanyId, out string ex);
                if (quotationRequest == null || !string.IsNullOrEmpty(ex))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "quotation request  Not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "quotation request  Not exist " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                if (quotationRequest.AdditionalDriverIdOne == null)
                {
                    quotationRequest.AdditionalDriverIdOne = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdTwo == null)
                {
                    quotationRequest.AdditionalDriverIdTwo = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdThree == null)
                {
                    quotationRequest.AdditionalDriverIdThree = request.DriverId;
                }
                else if (quotationRequest.AdditionalDriverIdFour == null)
                {
                    quotationRequest.AdditionalDriverIdFour = request.DriverId;
                }
                _quotationRequestRepository.Update(quotationRequest);
                CheckoutDetail c = _checkoutDetailrepository.Table.FirstOrDefault(x => x.ReferenceId == request.QuotationReferenceId);
                if (c.AdditionalDriverIdOne == null)
                {
                    c.AdditionalDriverIdOne = request.DriverId;
                }
                else if (c.AdditionalDriverIdTwo == null)
                {
                    c.AdditionalDriverIdTwo = request.DriverId;
                }
                else if (c.AdditionalDriverIdThree == null)
                {
                    c.AdditionalDriverIdThree = request.DriverId;
                }
                else if (c.AdditionalDriverIdFour == null)
                {
                    c.AdditionalDriverIdFour = request.DriverId;
                }
                //CheckoutAdditionalDriver additionalDriver = new CheckoutAdditionalDriver
                //{
                //    CheckoutDetailsId = c.ReferenceId,
                //    DriverId = request.DriverId.Value,
                //};
                //_checkoutAdditionalDriverrepository.Insert(additionalDriver);
                string fileURL = results.EndorsementFileUrl;
                fileURL = fileURL.Replace(@"\\", @"//");
                fileURL = fileURL.Replace(@"\", @"/");
                byte[] bytes = null;
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    bytes = client.DownloadData(fileURL);
                }
                
                var isAutoleasingPolicy = c.Channel == Channel.autoleasing.ToString().ToLower() ? true : false;
                string filePath = Utilities.SaveCompanyFileFromDashboard(c.ReferenceId, bytes, insuranceCompany.Key, false,
                        _config.RemoteServerInfo.UseNetworkDownload,
                        _config.RemoteServerInfo.DomainName,
                        _config.RemoteServerInfo.ServerIP,
                        _config.RemoteServerInfo.ServerUserName,
                        _config.RemoteServerInfo.ServerPassword,
                        isAutoleasingPolicy,
                        out string exception);
                if (!string.IsNullOrWhiteSpace(exception))
                {
                    output.ErrorCode = AddDriverOutput.ErrorCodes.InvalidData;
                    output.ErrorDescription = "failed to save file";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to save file duo to exception : " + ex;
                    PolicyModificationLogDataAccess.AddPolicyModificationLog(log);
                    return output;
                }
                Endorsment driverFile = new Endorsment
                {
                    FilePath = filePath,
                    InsurranceCompanyId = insuranceCompany.InsuranceCompanyID,
                    PolicyModificationRequestId = request.Id,
                    ReferenceId = c.ReferenceId,
                    Channel = log.Channel,
                    ServerIP = log.ServerIP,
                    UserAgent = log.UserAgent,
                    UserIP = log.UserIP,
                    CreatedDate = DateTime.Now,
                    QuotationReferenceId = request.QuotationReferenceId
                };
                _endormentRepository.Insert(driverFile);
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
        private QuotationRequest GetQuotationRequest(string policyNo, int? insuranceCompanyId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GeQuotationByPolicyNo";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter companyIdParameter = new SqlParameter()
                {
                    ParameterName = "companyId",
                    Value = insuranceCompanyId.Value
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(companyIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();
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
        #endregion

        #region CheckOut discount

        public CheckoutDiscountOutput CheckDiscountCode(Tameenk.Core.Domain.Dtos.CheckotDiscountModel model, string userId)
        {
            CheckoutDiscountOutput output = new CheckoutDiscountOutput();
            CheckoutRequestLog checkoutlog = new CheckoutRequestLog();
            checkoutlog.Channel = model.Channel;
            checkoutlog.MethodName = "CheckDiscountCode";
            checkoutlog.UserId = userId;
            checkoutlog.ServerIP = Utilities.GetInternalServerIP();
            checkoutlog.UserAgent = Utilities.GetUserAgent();
            checkoutlog.UserIP = Utilities.GetUserIPAddress();
            checkoutlog.RequesterUrl = Utilities.GetUrlReferrer();
            checkoutlog.ReferenceId = model.ReferenceId;
            checkoutlog.VehicleId = model.VehicleId;
            checkoutlog.DriverNin = model.DriverNin;
            checkoutlog.ServiceRequest = JsonConvert.SerializeObject(model);
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Code))
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "model is null or code is null";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                
                var dateNow = DateTime.Now;
                var renewalDiscount = _renewalDiscountRepository.TableNoTracking.Where(a => a.Code == model.Code).OrderByDescending(a => a.Id).FirstOrDefault();
                if (renewalDiscount == null)
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.InvalidCode;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("DiscountCodeNotExist", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " not exist in the database";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (!renewalDiscount.IsActive)
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.InvalidCode;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("DiscountCodeNotExist", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " is not active";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (DateTime.Compare(dateNow.Date, renewalDiscount.StartDate.Value.Date) < 0 && DateTime.Compare(dateNow.Date, renewalDiscount.EndDate.Value.Date) > 0) // means that dateNow before startDate or after endDate
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.CodeExpired;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("DiscountCodeExpired", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " is expired";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                var vehicleDiscountsInfo = _vehicleDiscountsRepository.TableNoTracking.Where(a => (a.SequenceNumber == model.VehicleId || a.CustomCardNumber == model.VehicleId) && a.DiscountCode == model.Code).FirstOrDefault();
                if (vehicleDiscountsInfo == null)
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.InvalidCode;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("YouDoNotDeserveTheDiscount", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " not exist in the database";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (vehicleDiscountsInfo.IsUsed.HasValue && vehicleDiscountsInfo.IsUsed.Value)
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.InvalidCode;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("CodeConsumedBefore", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " already consumed before";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }
                if (DateTime.Compare(dateNow.Date, vehicleDiscountsInfo.CreatedDate.Value.AddHours(48).Date) > 0) // means that dateNow after createdDate
                {
                    output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.CodeExpired;
                    output.ErrorDescription = CheckoutResources.ResourceManager.GetString("DiscountCodeExpired", CultureInfo.GetCultureInfo(model.Lang));
                    checkoutlog.ErrorCode = (int)output.ErrorCode;
                    checkoutlog.ErrorDescription = "this code " + model.Code + " is expired, as it has passed for more than 48 hours";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                    return output;
                }

                output.DiscountData = renewalDiscount;
                output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.Success;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = "Success";
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = CheckoutDiscountOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                checkoutlog.ErrorCode = (int)output.ErrorCode;
                checkoutlog.ErrorDescription = exp.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(checkoutlog);
                return output;
            }
        }
        #endregion

        public int UpdateCheckoutWithPolicyStatus(CheckoutDetail checkoutDetail, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateCheckoutWithPolicyStatus";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;
                SqlParameter referenceIdParam = new SqlParameter() { ParameterName = "referenceId", Value = checkoutDetail.ReferenceId };
                command.Parameters.Add(referenceIdParam);

                SqlParameter policyStatusIdParam = new SqlParameter() { ParameterName = "policyStatusId", Value = checkoutDetail.PolicyStatusId };
                command.Parameters.Add(policyStatusIdParam);

                SqlParameter modifiedDateParam = new SqlParameter() { ParameterName = "modifiedDate", Value = checkoutDetail.ModifiedDate };
                command.Parameters.Add(modifiedDateParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }            catch (Exception ex)            {
                dbContext.DatabaseInstance.Connection.Close();
                var checkoutdetatils = _checkoutDetailrepository.Table.Where(c => c.ReferenceId == checkoutDetail.ReferenceId && c.IsCancelled == false).FirstOrDefault();
                if (checkoutdetatils != null)
                {
                    checkoutdetatils.PolicyStatusId = checkoutDetail.PolicyStatusId;
                    checkoutdetatils.ModifiedDate = checkoutDetail.ModifiedDate;
                    _checkoutDetailrepository.Update(checkoutdetatils);
                    return 1;
                }

                exception = ex.ToString();
                return -1;
            }
        }
        public CheckoutDetail AddChekoutDetails(CheckoutModel model, ODCheckoutDetails ODModel, bool isODCheckout, QuotationResponseDBModel quoteResponse, ShoppingCartItemDB shoppingCartItem, bool? isEmailVerified, string discountCode, decimal? discountPercentage, int? discountType, decimal? discountValue, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang)        {            var referenceId = isODCheckout ? ODModel.ReferenceId : model.ReferenceId;            var checkoutDetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(referenceId);
            if (checkoutDetails == null)
            {
                checkoutDetails = new CheckoutDetail
                {
                    ReferenceId = referenceId,
                    BankCodeId = model.BankCode,
                    Email = model.Email,
                    IBAN = model.IBAN.Replace(" ", "").Trim().ToLower(),
                    Phone = Utilities.ValidatePhoneNumber(model.Phone),
                    PaymentMethodId = model.PaymentMethodCode,
                    UserId = model.UserId,
                    MainDriverId = quoteResponse.DriverId,
                    PolicyStatusId = (int)EPolicyStatus.PendingPayment,
                    VehicleId = quoteResponse.VehicleId,
                    CreatedDateTime = DateTime.Now,
                    SelectedInsuranceTypeCode = quoteResponse.InsuranceTypeCode/*(short)model.TypeOfInsurance*/,
                    SelectedLanguage = lang,
                    InsuranceCompanyId = quoteResponse.CompanyID,
                    InsuranceCompanyName = quoteResponse.CompanyKey,
                    Channel = log.Channel,
                    IsEmailVerified = isEmailVerified,
                    SelectedProductId = shoppingCartItem.ProductId,
                    MerchantTransactionId = Guid.NewGuid(),
                    DiscountCode = discountCode == string.Empty ? null : discountCode,
                    DiscountPercentage = discountPercentage,
                    DiscountType = discountType,
                    DiscountValue = discountValue,
                    ODReference = isODCheckout ? null : model.ODDetails?.ReferenceId,
                    InsuredId = quoteResponse.InsuredTableRowId,
                    ExternalId = quoteResponse.ExternalId

                };
                checkoutDetails.OrderItems = _orderService.CreateOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId);
                if (quoteResponse.InsuranceTypeCode != 1)
                {
                    checkoutDetails = AssignImagesToCheckOutAndAddWaterMark(checkoutDetails, model); // as per Moneera Al-Saqr(mail --> Comprehensive Images)
                }
                if (quoteResponse.AdditionalDriverList != null && quoteResponse.AdditionalDriverList.Count > 1)
                {
                    int index = 0;
                    foreach (var driverId in quoteResponse.AdditionalDriverList)
                    {

                        checkoutDetails.CheckoutAdditionalDrivers.Add(new CheckoutAdditionalDriver()
                        {
                            CheckoutDetailsId = checkoutDetails.ReferenceId,
                            DriverId = driverId
                        });
                        if (driverId.ToString().ToLower() != quoteResponse.DriverId.ToString().ToLower())//not main driver 
                        {
                            index++;
                            if (index == 1)
                            {
                                checkoutDetails.AdditionalDriverIdOne = driverId;
                            }
                            if (index == 2)
                            {
                                checkoutDetails.AdditionalDriverIdTwo = driverId;
                            }
                            if (index == 3)
                            {
                                checkoutDetails.AdditionalDriverIdThree = driverId;
                            }
                            if (index == 4)
                            {
                                checkoutDetails.AdditionalDriverIdFour = driverId;
                            }
                        }
                    }
                }
                // save checkout details on database.
                _orderService.CreateCheckoutDetails(checkoutDetails);
            }
            else
            {
                string exception = string.Empty;
                if (_orderService.DeleteOrderItemByRefrenceId(referenceId, out exception))
                    _orderService.SaveOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId, out exception);
                if (quoteResponse.InsuranceTypeCode != 1)
                {
                    checkoutDetails = AssignImagesToCheckOutAndAddWaterMark(checkoutDetails, model);
                }
                checkoutDetails.UserId = model.UserId;
                checkoutDetails.SelectedLanguage = lang;
                checkoutDetails.Email = model.Email;
                checkoutDetails.IBAN = model.IBAN.Replace(" ", "").Trim().ToLower();
                checkoutDetails.Phone = Utilities.ValidatePhoneNumber(model.Phone);
                checkoutDetails.BankCodeId = model.BankCode;
                checkoutDetails.PaymentMethodId = model.PaymentMethodCode;
                checkoutDetails.ModifiedDate = DateTime.Now;
                checkoutDetails.Channel = log.Channel;
                checkoutDetails.SelectedProductId = shoppingCartItem.ProductId;
                checkoutDetails.MerchantTransactionId = Guid.NewGuid();
                checkoutDetails.DiscountCode = discountCode == string.Empty ? null : discountCode;
                checkoutDetails.DiscountPercentage = discountPercentage;
                checkoutDetails.DiscountType = discountType;
                checkoutDetails.DiscountValue = discountValue;
                checkoutDetails.ODReference = isODCheckout ? null : model.ODDetails?.ReferenceId;
                checkoutDetails.InsuredId = quoteResponse.InsuredTableRowId;
                checkoutDetails.ExternalId = quoteResponse.ExternalId;
                _orderService.UpdateCheckout(checkoutDetails);
            }
            return checkoutDetails;        }
        #region Tabby
        public CheckoutOutput ExecuteTabbyPayment(string Channel, CheckoutDetail details, Tameenk.Core.Domain.Entities.Invoice invoice, CheckoutModel model, QuotationResponseDBModel quoteResponse)
        {
            var output = new CheckoutOutput();
            string TabbyRequestJson = string.Empty;
            try
            {
                TabbyConfig setting = _config.Tabby; // need to add to config

                if (setting == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "setting is null";
                    return output;
                }
                if (!quoteResponse.CreatedDate.HasValue)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "CreatedDate is null";
                    return output;
                }
                var companyAccountDetails = _companyBankAccountRepository.TableNoTracking.FirstOrDefault(a => a.CompanyId == invoice.InsuranceCompanyId);
                if (companyAccountDetails == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "company Account Not found for " + invoice.InsuranceCompanyId;
                    return output;
                }
                // get tabby items 
                var tabbyitems = _tabbyPaymentService.GetTabbyItemsByUserId(invoice.UserId);

                var tabbyResponseModel = new TabbyResponseModel();
                // create tabby request
                var tabbyPayment = new TabbyPayment();
                var value = (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0) ? (invoice.TotalPrice.Value - invoice.TotalBCareDiscount.Value).ToString() : invoice.TotalPrice.Value.ToString();
                tabbyPayment.amount = Math.Round(double.Parse(value), 2).ToString();
                tabbyPayment.currency = "SAR";
                tabbyPayment.description = "Pay Policy From BCare";
                tabbyPayment.order = new TabbyOrder();
                tabbyPayment.order.reference_id = details.ReferenceId;
                tabbyPayment.order.items = new List<OrderItems>();
                OrderItems orderItems = new OrderItems();
                orderItems.category = "Vehicle";
                orderItems.unit_price = Math.Round(double.Parse(value), 2).ToString();
                if (details.SelectedInsuranceTypeCode.HasValue)
                {
                    orderItems.title = _productTypeRep.Table.FirstOrDefault(x => x.Code == details.SelectedInsuranceTypeCode)?.EnglishDescription;
                }
                orderItems.category = "Vehicle";
                tabbyPayment.order.items.Add(orderItems);
                tabbyPayment.meta = new { ic_iban = companyAccountDetails.IBAN, ic_amount = invoice.TotalCompanyAmount };
                tabbyPayment.buyer = new Buyer();
                tabbyPayment.buyer.email = details.Email; //tabbyitems.Buyer.Email;
                tabbyPayment.buyer.name = tabbyitems.Buyer.UserName;
                tabbyPayment.buyer.phone = details.Phone;// tabbyitems.Buyer.PhoneNumber;
                tabbyPayment.buyer_history = new BuyerHistory();
                tabbyPayment.buyer_history.registered_since = tabbyitems.Buyer.RegisterDate.ToString("yyyy-MM-ddTHH:mm:ssZ",new CultureInfo("en-US"));

                tabbyPayment.buyer_history.loyalty_level = tabbyitems.TotalPolicies;

                //get address 
                ShippingAddress shippingAddress = new ShippingAddress();
                if(string.IsNullOrEmpty(quoteResponse.AddressCityName))
                {
                    var address = addressService.GetAddressesByNin(quoteResponse.NIN);
                    if(address!=null)
                    {
                        quoteResponse.AddressCityName = address.City;
                        quoteResponse.BuildingNumber = address.BuildingNumber;
                        quoteResponse.District = address.District;
                        quoteResponse.PostCode = address.PostCode;
                    }
                }
                shippingAddress.address = $"{quoteResponse?.BuildingNumber} - {quoteResponse?.District}";
                shippingAddress.city = quoteResponse.AddressCityName;
                shippingAddress.zip = quoteResponse.PostCode;
                tabbyPayment.shipping_address = shippingAddress;


                InsuranceDetails insurance_Details = new InsuranceDetails();
                insurance_Details.Pnr = "";
                insurance_Details.Policy_type = new InsurancePolicyType();
                insurance_Details.Policy_type.Car_brand = quoteResponse.VehicleMaker;
                insurance_Details.Policy_type.Car_model = quoteResponse.VehicleModel;
                insurance_Details.Policy_type.Car_manufacture_year = quoteResponse.VehicleModelYear.ToString();
                insurance_Details.Policy_type.Duration = 364;
                insurance_Details.Policy_type.Refundable = true;
                insurance_Details.Policy_type.Type = (details.SelectedInsuranceTypeCode == 1) ? "Third party Vehicle Insurance" : "Comprehensive Vehicle Insurance";
                tabbyPayment.Insurance_details = insurance_Details;

                if (tabbyitems.BuyerHistoryData != null && tabbyitems.BuyerHistoryData.Count > 0)
                {
                    tabbyPayment.order_history = new List<OrderHistory>();
                    foreach (var item in tabbyitems.BuyerHistoryData)
                    {
                        var Order = new OrderHistory();
                        Order.amount = item.TotalPrice.ToString();
                        Order.purchased_at = item.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        if (item.StatusName.ToLower().Contains("pending"))
                        {
                            Order.status = "processing";
                        }
                        else if (item.StatusName.ToLower().Contains("failure") || item.StatusName.ToLower().Contains("fail"))
                        {
                            Order.status = "canceled";
                        }
                        else if (item.StatusName.ToLower().Contains("success"))
                        {
                            Order.status = "processing";
                        }
                        else if (item.StatusName.ToLower().Contains("available"))
                        {
                            Order.status = "complete";
                        }
                        else if (item.StatusName.ToLower().Contains("pending"))
                        {
                            Order.status = "processing";
                        }
                        else
                        {
                            Order.status = "unknown";
                        }
                        tabbyPayment.order_history.Add(Order);
                    }
                }
                // get all benifit ids
                var benifitArray = model.SelectedProductBenfitId?.Split(',');
                string spbIds = "";
                if (benifitArray != null && benifitArray.Length > 0)
                {
                    foreach (var item in benifitArray)
                    {
                        spbIds += "&spbIds=" + item;
                    }
                }
                string FailURL = setting.FailUrl + "?re=" + details.ReferenceId + "&ext=" + model.QtRqstExtrnlId + "&pid=" + model.ProductId + "&hashed=" + model.Hashed + spbIds; //"https://test.bcare.com.sa/checkout?re="
                string CancelURL = setting.CancelUrl + "?re=" + details.ReferenceId + "&ext=" + model.QtRqstExtrnlId + "&pid=" + model.ProductId + "&hashed=" + model.Hashed + spbIds; //"https://test.bcare.com.sa/checkout?re="
                var merchantURLS = new MerchantUrls();
                merchantURLS.success = setting.SuccessUrl;
                merchantURLS.failure = FailURL;
                merchantURLS.cancel = CancelURL;
                TabbyRequestModel request = new TabbyRequestModel
                {
                    payment = tabbyPayment,
                    lang = "en",
                    merchant_code =(details.SelectedInsuranceTypeCode == 1) ? "bcaretpl" : setting.MerchantCode,
                    merchant_urls = merchantURLS
                };
                TabbyRequest tabbyRequestEntity = new TabbyRequest();
                tabbyRequestEntity.UserId = details.UserId;
                tabbyRequestEntity.RefrenceId = details.ReferenceId;
                tabbyRequestEntity.Channel = details.Channel;
                tabbyRequestEntity.CreatedDate = DateTime.Now;
                tabbyRequestEntity.Amount = Math.Round(double.Parse(value), 2);
                tabbyRequestEntity.Currency = "SAR";
                tabbyRequestEntity.CompanyId = details.InsuranceCompanyId;
                tabbyRequestEntity.CompanyName = details.InsuranceCompanyName;
                tabbyRequestEntity.InsuranceCompanyAmount = double.Parse(invoice.TotalCompanyAmount.Value.ToString());

                // insert tabby request entity
                _tabbyRequest.Insert(tabbyRequestEntity);
                Guid parseResult = Guid.Empty;
                var isParsed = Guid.TryParse(tabbyRequestEntity.Id.ToString(), out parseResult);
                if (!isParsed || parseResult == Guid.Empty)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Failed to save Tabby request";
                    return output;
                }
                // insert details
                TabbyRequestDetails tabbyRequestDetails = new TabbyRequestDetails();

                tabbyRequestDetails.TabbyRequestId = tabbyRequestEntity.Id;
                tabbyRequestDetails.CreatedDate = DateTime.Now;
                tabbyRequestDetails.Attachment = JsonConvert.SerializeObject(request.payment.attachment);
                tabbyRequestDetails.Buyer = JsonConvert.SerializeObject(request.payment.buyer);
                tabbyRequestDetails.buyerHistory = JsonConvert.SerializeObject(request.payment.buyer_history);
                tabbyRequestDetails.Order = JsonConvert.SerializeObject(request.payment.order);
                tabbyRequestDetails.OrderHistory = JsonConvert.SerializeObject(request.payment.order_history);
                tabbyRequestDetails.ShippingAddress = JsonConvert.SerializeObject(request.payment.shipping_address);
                tabbyRequestDetails.Meta = JsonConvert.SerializeObject(request.payment.meta);
                tabbyRequestDetails.MerchantCode = request.merchant_code;
                tabbyRequestDetails.MerchantURL = JsonConvert.SerializeObject(request.merchant_urls);
                tabbyRequestDetails.Lang = request.lang;

                _tabbyRequestDetails.Insert(tabbyRequestDetails);
                if (tabbyRequestDetails.Id < 0)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Failed to save Tabby request Details";
                    return output;
                }
                TabbyRequestJson = JsonConvert.SerializeObject(request);
                //// send request to tabby
                var tabbyRsponseHandler = new TabbyResponseHandler();
                var result = _tabbyPaymentService.SubmitTabbyRequest(request, setting, details, Channel, model.QtRqstExtrnlId, quoteResponse.NIN);
                if (result.ErrorCode != TabbyOutput.ErrorCodes.Success)
                {
                    tabbyRsponseHandler.Status.IsErrors = true;
                    tabbyRsponseHandler.Status.status = "Failed";
                    tabbyRsponseHandler.Status.error = "SubmitTabbyRequest returned error:" + result.ErrorDescription;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Tabby Result is null";
                    output.tabbyResponseHandlerModel = tabbyRsponseHandler;
                    return output;
                }
                if (result.TabbyResponseHandler == null || result.TabbyResponseHandler.Status == null)
                {
                    tabbyRsponseHandler.Status.IsErrors = true;
                    tabbyRsponseHandler.Status.status = "Failed";
                    tabbyRsponseHandler.Status.error = "Failed to read/parse response from Tabby service";
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "TabbyResponseHandler is null";
                    output.tabbyResponseHandlerModel = tabbyRsponseHandler;
                    return output;
                }
                if (result.TabbyResponseHandler.Status.status == "error")
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = result.ErrorDescription;
                    tabbyRsponseHandler.Status.status = "Failed";

                    foreach (var item in tabbyRsponseHandler.Status.errors)
                    {
                        output.ErrorDescription += item + ",";
                    }
                    output.ErrorDescription = tabbyRsponseHandler.Status.error;
                    output.tabbyResponseHandlerModel = tabbyRsponseHandler;

                    return output;
                }
                //save response entity
                TabbyResponse tabbyResponseEntity = new TabbyResponse();
                tabbyResponseEntity.TabbyRequestId = tabbyRequestEntity.Id;
                tabbyResponseEntity.ReferenceId = details.ReferenceId;
                tabbyResponseEntity.CreatedDate = DateTime.Now;
                tabbyResponseEntity.TotalAmount = double.Parse(result.TabbyResponseHandler.ResponseBody.payment.amount);
                tabbyResponseEntity.AmountRemaining = double.Parse(result.TabbyResponseHandler.ResponseBody.configuration.available_products.installments[0].amount_to_pay);
                tabbyResponseEntity.AmountPerInstallment = double.Parse(result.TabbyResponseHandler.ResponseBody.configuration.available_products.installments[0].pay_per_installment);
                tabbyResponseEntity.Channel = details.Channel;
                tabbyResponseEntity.InstallmentCount = result.TabbyResponseHandler.ResponseBody.configuration.available_products.installments[0].installments_count;
                tabbyResponseEntity.PaymentId = result.TabbyResponseHandler.ResponseBody.payment.id;
                tabbyResponseEntity.IsWarning = result.TabbyResponseHandler.ResponseBody.warnings != null ? true : false;
                tabbyResponseEntity.UserId = details.UserId;

                _tabbyResponse.Insert(tabbyResponseEntity);

                if (tabbyResponseEntity.Id < 1)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Failed to save Tabby Response";
                    return output;
                }
                // save response details
                TabbyResponseDetail tabbyResponseDetailEntity = new TabbyResponseDetail();
                tabbyResponseDetailEntity.TabbyResponseId = tabbyResponseEntity.Id;
                tabbyResponseDetailEntity.CreatedDate = DateTime.Now;
                tabbyResponseDetailEntity.ApiUrl = result.TabbyResponseHandler.ResponseBody.api_url;
                tabbyResponseDetailEntity.Configuration = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.configuration);
                tabbyResponseDetailEntity.Customer = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.customer);
                tabbyResponseDetailEntity.Flow = result.TabbyResponseHandler.ResponseBody.flow;
                tabbyResponseDetailEntity.JuicyScore = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.juicyscore);
                tabbyResponseDetailEntity.Lang = result.TabbyResponseHandler.ResponseBody.lang;
                tabbyResponseDetailEntity.Merchant = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.merchant);
                tabbyResponseDetailEntity.MerchantCode = result.TabbyResponseHandler.ResponseBody.merchant_code;
                tabbyResponseDetailEntity.MerchantURL = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.merchant_urls);
                tabbyResponseDetailEntity.PaymentAmount = double.Parse(result.TabbyResponseHandler.ResponseBody.payment?.amount);
                tabbyResponseDetailEntity.PaymentBuyer = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.buyer);
                tabbyResponseDetailEntity.PaymentBuyerHistory = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.buyer_history);
                tabbyResponseDetailEntity.PaymentCancelable = result.TabbyResponseHandler.ResponseBody.payment?.cancelable;
                tabbyResponseDetailEntity.PaymentCaptures = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.captures);
                tabbyResponseDetailEntity.PaymentCreatedAt = result.TabbyResponseHandler.ResponseBody.payment?.created_at;
                tabbyResponseDetailEntity.PaymentExpireAt = result.TabbyResponseHandler.ResponseBody.payment?.expires_at;
                tabbyResponseDetailEntity.PaymentCurrency = result.TabbyResponseHandler.ResponseBody.payment?.currency;
                tabbyResponseDetailEntity.PaymentDescription = result.TabbyResponseHandler.ResponseBody.payment?.description;
                tabbyResponseDetailEntity.PaymentID = result.TabbyResponseHandler.ResponseBody.payment?.id;
                tabbyResponseDetailEntity.PaymentIsExpire = result.TabbyResponseHandler.ResponseBody.payment?.is_expired;
                tabbyResponseDetailEntity.PaymentMeta = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.meta);
                tabbyResponseDetailEntity.PaymentOrder = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.order);
                tabbyResponseDetailEntity.PaymentOrderHistory = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.order_history);
                tabbyResponseDetailEntity.PaymentProduct = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.product);
                tabbyResponseDetailEntity.PaymentRefunds = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.refunds);
                tabbyResponseDetailEntity.PaymentShippingAddress = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.payment?.shipping_address);
                tabbyResponseDetailEntity.PaymentStatus = result.TabbyResponseHandler.ResponseBody.payment?.status;
                tabbyResponseDetailEntity.PaymentTest = result.TabbyResponseHandler.ResponseBody.payment?.is_test;
                tabbyResponseDetailEntity.ProductType = result.TabbyResponseHandler.ResponseBody.product_type;
                tabbyResponseDetailEntity.SiftSessionId = result.TabbyResponseHandler.ResponseBody.sift_session_id;
                tabbyResponseDetailEntity.Status = result.TabbyResponseHandler.ResponseBody.status;
                tabbyResponseDetailEntity.TermsAccepted = result.TabbyResponseHandler.ResponseBody.terms_accepted;
                tabbyResponseDetailEntity.Token = result.TabbyResponseHandler.ResponseBody.token;
                tabbyResponseDetailEntity.Warnings = JsonConvert.SerializeObject(result.TabbyResponseHandler.ResponseBody.warnings);


                _tabbyResponseDetail.Insert(tabbyResponseDetailEntity);
                if (tabbyResponseDetailEntity.Id < 1)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                    output.ErrorDescription = "Failed to save Tabby Response Details";
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                tabbyRsponseHandler.Status.status = "Succeeded";

                tabbyRsponseHandler.ResponseBody = result.TabbyResponseHandler.ResponseBody;
                output.tabbyResponseHandlerModel = tabbyRsponseHandler;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = "Exceute Tabbly failed duo to " + ex.ToString();
                return output;
            }
        }

        public TabbyOutput TabbyPaymentNotification(string PaymentId, string channel, string userId)
        {
            DateTime date = DateTime.Now;
            TabbyOutput output = new TabbyOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.MethodName = "TabbyPaymentNotification";
            log.UserId = userId;
            log.Channel = channel;
            log.UserId = userId;
            try
            {
                if (string.IsNullOrEmpty(PaymentId))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.EmptyPaymentId;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorDescription = "PaymentId is Null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var userManager = _authorizationService.GetUser(log.UserId.ToString());
                log.UserName = userManager.Result?.UserName;
                if (userManager == null || userManager.Result == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userManager is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (userManager.Result.LockoutEnabled && userManager.Result.LockoutEndDateUtc >= DateTime.UtcNow)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.UserLockedOut;
                    output.ErrorDescription = CheckoutResources.AccountLocked;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is locked";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var tabbyResponseObject = _tabbyResponse.TableNoTracking.Include(x => x.TabbyRequest).Where(x => x.PaymentId == PaymentId).FirstOrDefault();
                if (tabbyResponseObject == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorDescription = "No Response for this PaymentId = " + PaymentId;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.ReferenceId = tabbyResponseObject.ReferenceId;
                log.Channel = tabbyResponseObject.Channel;
                log.CompanyId = tabbyResponseObject.TabbyRequest.CompanyId;
                log.CompanyName = tabbyResponseObject.TabbyRequest.CompanyName;
                TabbyConfig tabbyConfig = _config.Tabby;
                var notificationOutput = _tabbyPaymentService.SubmitTabbyNotification(PaymentId, tabbyResponseObject, tabbyConfig);
                if (notificationOutput.ErrorCode != TabbyOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorDescription = "Failed to send taby notificatiion due to:" + notificationOutput.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var captureOutput = _tabbyPaymentService.SubmitTabbyCaptureRequest(tabbyConfig, tabbyResponseObject, PaymentId);
                if (captureOutput.ErrorCode != TabbyOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorDescription = "Failed to send taby capture request due to:" + notificationOutput.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                TabbyNotification tabbyNotificationEntity = new TabbyNotification();
                tabbyNotificationEntity.Channel = tabbyResponseObject.Channel;
                tabbyNotificationEntity.ReferenceID = tabbyResponseObject.ReferenceId;
                tabbyNotificationEntity.PaymentId = PaymentId;
                tabbyNotificationEntity.CreatedDate = DateTime.Now;
                tabbyNotificationEntity.UserId = tabbyResponseObject.UserId;
                tabbyNotificationEntity.TabbyRequestId = tabbyResponseObject.TabbyRequest.Id;

                tabbyNotificationEntity.PaymentId = notificationOutput.TabbyNotificationResponseModel?.id;
                tabbyNotificationEntity.Amount = double.Parse(notificationOutput.TabbyNotificationResponseModel?.amount);
                tabbyNotificationEntity.Cancelable = notificationOutput.TabbyNotificationResponseModel?.cancelable;
                tabbyNotificationEntity.CreatedAT = notificationOutput.TabbyNotificationResponseModel?.created_at;
                tabbyNotificationEntity.ExpiresAt = notificationOutput.TabbyNotificationResponseModel?.expires_at;
                tabbyNotificationEntity.CreatedDate = DateTime.Now;
                tabbyNotificationEntity.Currency = notificationOutput.TabbyNotificationResponseModel?.currency;
                tabbyNotificationEntity.Description = notificationOutput.TabbyNotificationResponseModel?.description;
                tabbyNotificationEntity.IsTest = notificationOutput.TabbyNotificationResponseModel?.is_test;
                tabbyNotificationEntity.Meta = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel?.meta);
                tabbyNotificationEntity.ReferenceID = notificationOutput.TabbyNotificationResponseModel.order?.reference_id;
                tabbyNotificationEntity.Status = notificationOutput.TabbyNotificationResponseModel.status;
                tabbyNotificationEntity.UserId = tabbyResponseObject.UserId;
                tabbyNotificationEntity.TabbyRequestId = tabbyResponseObject.TabbyRequest.Id;


                if (tabbyNotificationEntity.Status.ToLower() != "authorized" && tabbyNotificationEntity.Status.ToLower() != "closed")
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvalidTabbyNotificationStatus;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyNotificationEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyNotificationEntity.ErrorDescription = output.ErrorDescription;
                    _tabbyPaymentService.InsertIntoTabbyNotification(tabbyNotificationEntity);
                    log.ErrorDescription = "Error Payment Status = " + tabbyNotificationEntity.Status;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var checkoutDetails = _checkoutDetailrepository.Table
                      .Include(x => x.OrderItems.Select(y => y.Product.InsuranceCompany))
                      .Include(x => x.OrderItems.Select(y => y.Product.QuotationResponse.ProductType))
                      .FirstOrDefault(c => c.ReferenceId == tabbyNotificationEntity.ReferenceID);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.checkoutDetailsIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyNotificationEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyNotificationEntity.ErrorDescription = "checkoutDetails is null for referenceID=" + tabbyNotificationEntity.ReferenceID;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var invoice = _orderService.GetInvoiceByRefrenceId(tabbyNotificationEntity.ReferenceID);
                if (invoice == null)
                {
                    invoice = _orderService.CreateInvoice(tabbyNotificationEntity.ReferenceID, checkoutDetails.SelectedInsuranceTypeCode.Value,
                        checkoutDetails.InsuranceCompanyId.Value);
                }
                if (invoice == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvoiceIsNulll;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    tabbyNotificationEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyNotificationEntity.ErrorDescription = output.ErrorDescription;
                    _tabbyPaymentService.InsertIntoTabbyNotification(tabbyNotificationEntity);
                    log.ErrorDescription = "invoice is null for referenceID=" + tabbyNotificationEntity.ReferenceID;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (invoice.TotalPrice != decimal.Parse(tabbyNotificationEntity.Amount.ToString()))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyNotificationEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyNotificationEntity.ErrorDescription = "PaymentAmount is not valid as we recived " + tabbyNotificationEntity.Amount
                        + " and invoice amount is " + invoice.TotalPrice;
                    _tabbyPaymentService.InsertIntoTabbyNotification(tabbyNotificationEntity);
                    log.ErrorDescription = output.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                tabbyNotificationEntity.InvoiceNo = invoice.InvoiceNo.ToString();

                if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PendingPayment ||
                        checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
                {
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.Tabby;
                    string companyName = string.Empty;
                    if (checkoutDetails.InsuranceCompany != null && !string.IsNullOrEmpty(checkoutDetails.InsuranceCompany.Key))
                    {
                        companyName = checkoutDetails.InsuranceCompany.Key;
                    }
                    else
                    {
                        companyName = checkoutDetails.InsuranceCompanyName;
                    }
                    _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, companyName, checkoutDetails.Channel);
                    _checkoutDetailrepository.Update(checkoutDetails);
                    _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId, checkoutDetails.ReferenceId);
                    if (!string.IsNullOrEmpty(checkoutDetails.DiscountCode)) //mark discount code as consumed
                    {
                        _orderService.UpdateDiscountCodeToBeConsumed(checkoutDetails.VehicleId, checkoutDetails.DiscountCode, checkoutDetails.ReferenceId);
                    }

                }
                tabbyNotificationEntity.ErrorCode = (int)TabbyOutput.ErrorCodes.Success;
                tabbyNotificationEntity.ErrorDescription = "Success";
                _tabbyPaymentService.InsertIntoTabbyNotification(tabbyNotificationEntity);

                TabbyNotificationDetails tabbyNotificationDetails = new TabbyNotificationDetails();
                tabbyNotificationDetails.TabbyNotificationId = tabbyNotificationEntity.Id;
                tabbyNotificationDetails.CreatedDate = DateTime.Now;
                tabbyNotificationDetails.Order = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.order);
                tabbyNotificationDetails.Buyer = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.buyer);
                tabbyNotificationDetails.BuyerHistory = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.buyer_history);
                tabbyNotificationDetails.Captures = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.captures);
                tabbyNotificationDetails.OrderHistory = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.order_history);
                tabbyNotificationDetails.Product = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.product);
                tabbyNotificationDetails.Refunds = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.refunds);
                tabbyNotificationDetails.ShippingAddress = JsonConvert.SerializeObject(notificationOutput.TabbyNotificationResponseModel.shipping_address);

                _tabbyPaymentService.InsertIntoTabbyNotificationDetails(tabbyNotificationDetails);

                TabbyCaptureRequest tabbyCaptureRequestEntity = new TabbyCaptureRequest();

                tabbyCaptureRequestEntity.Amount = tabbyResponseObject.TabbyRequest.Amount;
                tabbyCaptureRequestEntity.Channel = tabbyResponseObject.Channel;
                tabbyCaptureRequestEntity.TabbyRequestId = tabbyResponseObject.TabbyRequest.Id;
                tabbyCaptureRequestEntity.CreatedDate = DateTime.Now;
                tabbyCaptureRequestEntity.RefrenceId = tabbyResponseObject.ReferenceId;
                tabbyCaptureRequestEntity.CreatedAt = DateTime.Now.ToString();
                tabbyCaptureRequestEntity.DiscountAmount = 0;
                tabbyCaptureRequestEntity.TaxAmount = 0;
                tabbyCaptureRequestEntity.UserId = tabbyResponseObject.UserId;
                tabbyCaptureRequestEntity.ShippingAmount = 0;
                tabbyCaptureRequestEntity.Items = null;

                _tabbyPaymentService.InsertIntoTabbyCaptureRequest(tabbyCaptureRequestEntity);
                var tabbyCaptureResponseEntity = new TabbyCaptureResponse();

                tabbyCaptureResponseEntity.Amount = double.Parse(captureOutput.TabbyCaptureResponseViewModel.amount);
                tabbyCaptureResponseEntity.Cancelable = captureOutput.TabbyCaptureResponseViewModel.cancelable;
                tabbyCaptureResponseEntity.Currency = captureOutput.TabbyCaptureResponseViewModel.currency;
                tabbyCaptureResponseEntity.IsExpired = captureOutput.TabbyCaptureResponseViewModel.is_expired;
                tabbyCaptureResponseEntity.Status = captureOutput.TabbyCaptureResponseViewModel.status;
                tabbyCaptureResponseEntity.Test = captureOutput.TabbyCaptureResponseViewModel.test;
                tabbyCaptureResponseEntity.UserId = tabbyResponseObject.UserId;
                tabbyCaptureResponseEntity.Channel = tabbyResponseObject.Channel;
                tabbyCaptureResponseEntity.TabbyCaptureRequestId = tabbyCaptureRequestEntity.Id;
                tabbyCaptureResponseEntity.CaptureId = captureOutput.TabbyCaptureResponseViewModel.captures[0]?.id;
                tabbyCaptureResponseEntity.OrderRefrenceId = captureOutput.TabbyCaptureResponseViewModel.order?.reference_id;
                tabbyCaptureResponseEntity.RefrenceId = tabbyResponseObject.ReferenceId;
                tabbyCaptureResponseEntity.CreatedDate = DateTime.Now;
                tabbyCaptureResponseEntity.CreatedAt = captureOutput.TabbyCaptureResponseViewModel.created_at;
                tabbyCaptureResponseEntity.ExpiresAt = captureOutput.TabbyCaptureResponseViewModel.expires_at;
                _tabbyPaymentService.InsertIntoTabbyCaptureResponse(tabbyCaptureResponseEntity);

                var tabbyCaptureResponseDetailsEntity = new TabbyCaptureResponseDetails();

                tabbyCaptureResponseDetailsEntity.CreatedDate = DateTime.Now;
                tabbyCaptureResponseDetailsEntity.TabbyCaptureResponseId = tabbyCaptureResponseEntity.Id;
                tabbyCaptureResponseDetailsEntity.Buyer = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.buyer);
                tabbyCaptureResponseDetailsEntity.BuyerHistory = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.buyer_history);
                tabbyCaptureResponseDetailsEntity.Captures = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.captures);
                tabbyCaptureResponseDetailsEntity.Meta = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.meta);
                tabbyCaptureResponseDetailsEntity.Order = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.order);
                tabbyCaptureResponseDetailsEntity.OrderHistory = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.order_history);
                tabbyCaptureResponseDetailsEntity.Product = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.product);
                tabbyCaptureResponseDetailsEntity.Refund = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.refunds);
                tabbyCaptureResponseDetailsEntity.ShippingAddress = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.shipping_address);

                _tabbyPaymentService.InsertIntoTabbyCaptureResponseDetails(tabbyCaptureResponseDetailsEntity);

                output.ErrorCode = TabbyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorDescription = output.ErrorDescription;
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }
        public TabbyOutput TabbyPaymentWebhook(TabbyWebhookModel tabbyWebHookModel)
        {
            DateTime date = DateTime.Now;
            TabbyOutput output = new TabbyOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.MethodName = "TabbyPaymentWebHook";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.PaymentMethod = PaymentMethodCode.Tabby.ToString();
            try
            {
                if (tabbyWebHookModel == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.tabbyWebHookModel;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.tabbyWebHookModel;
                    log.ErrorDescription = "WebHookModel is Null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(tabbyWebHookModel);
                //if (Utilities.GetUserIPAddress() != "34.93.76.191")
                //{
                //    output.ErrorCode = TabbyOutput.ErrorCodes.Unauthorized;
                //    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                //    log.ErrorCode = (int)TabbyOutput.ErrorCodes.tabbyWebHookModel;
                //    log.ErrorDescription = "Unauthorized IP";
                //    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                //    return output;
                //}
                if (string.IsNullOrEmpty(tabbyWebHookModel.id))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.EmptyPaymentId;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.EmptyPaymentId;
                    log.ErrorDescription = "PaymentId is Null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (tabbyWebHookModel.status.ToLower() != "authorized")
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.WebhookNotAuthorized;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.WebhookNotAuthorized;
                    log.ErrorDescription = "Webhook Not Authorized , we recieved = " + tabbyWebHookModel.status;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var tabbyResponseObject = _tabbyResponse.TableNoTracking.Include(x => x.TabbyRequest).Where(x => x.PaymentId == tabbyWebHookModel.id).FirstOrDefault();
                if (tabbyResponseObject == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    log.ErrorDescription = "No Response for this PaymentId = " + tabbyWebHookModel.id;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.UserId = tabbyResponseObject.UserId;
                log.ReferenceId = tabbyResponseObject.ReferenceId;
                log.Channel = tabbyResponseObject.Channel;
                log.CompanyId = tabbyResponseObject.TabbyRequest.CompanyId;
                log.CompanyName = tabbyResponseObject.TabbyRequest.CompanyName;
                decimal amount = 0;
                decimal.TryParse(tabbyWebHookModel.amount, out amount);
                log.Amount = amount;
                TabbyConfig tabbyConfig = _config.Tabby;

                if (tabbyResponseObject.TotalAmount != double.Parse(tabbyWebHookModel.amount))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.InvalidAmount;
                    log.ErrorDescription = "Amount are not the same as we recived " + tabbyWebHookModel.amount + "; and correct one is:" + tabbyResponseObject.TotalAmount;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
        
                TabbyWebHook tabbyWebHookEntity = new TabbyWebHook();
                tabbyWebHookEntity.PaymentId = tabbyWebHookModel.id;
                tabbyWebHookEntity.Amount = double.Parse(tabbyWebHookModel.amount);
                tabbyWebHookEntity.Cancelable = tabbyWebHookModel.cancelable;
                tabbyWebHookEntity.ClosedAt = tabbyWebHookModel.closed_at;
                tabbyWebHookEntity.CreatedAt = tabbyWebHookModel.created_at;
                tabbyWebHookEntity.CreatedDate = DateTime.Now;
                tabbyWebHookEntity.Currency = tabbyWebHookModel.currency;
                tabbyWebHookEntity.UserId = tabbyWebHookModel.customer_id;
                tabbyWebHookEntity.Description = tabbyWebHookModel.description;
                tabbyWebHookEntity.ExpiresAt = tabbyWebHookModel.expires_at;
                tabbyWebHookEntity.IsTest = tabbyWebHookModel.is_test;
                tabbyWebHookEntity.MerchantId = tabbyWebHookModel.merchant_id;
                tabbyWebHookEntity.ProductOptionId = tabbyWebHookModel.product_option_id;
                tabbyWebHookEntity.RefrenceId = tabbyWebHookModel.order.reference_id;
                tabbyWebHookEntity.status = tabbyWebHookModel.status;
                tabbyWebHookEntity.TabbyRequestId = tabbyResponseObject.TabbyRequestId;

                var checkoutDetails = _checkoutDetailrepository.Table
                      .Include(x => x.OrderItems.Select(y => y.Product.InsuranceCompany))
                      .Include(x => x.OrderItems.Select(y => y.Product.QuotationResponse.ProductType))
                      .FirstOrDefault(c => c.ReferenceId == tabbyWebHookEntity.RefrenceId);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.checkoutDetailsIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyWebHookEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyWebHookEntity.ErrorDescription = "checkoutDetails is null for referenceID=" + tabbyWebHookEntity.RefrenceId;
                    _tabbyPaymentService.InsertIntoTabbyWebhook(tabbyWebHookEntity);
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.checkoutDetailsIsNull;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var invoice = _orderService.GetInvoiceByRefrenceId(tabbyWebHookEntity.RefrenceId);
                if (invoice == null)
                {
                    invoice = _orderService.CreateInvoice(tabbyWebHookEntity.RefrenceId, checkoutDetails.SelectedInsuranceTypeCode.Value,
                        checkoutDetails.InsuranceCompanyId.Value);
                }
                if (invoice == null)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvoiceIsNulll;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyWebHookEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyWebHookEntity.ErrorDescription = "invoice is null for referenceID=" + tabbyWebHookEntity.RefrenceId;
                    _tabbyPaymentService.InsertIntoTabbyWebhook(tabbyWebHookEntity);
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.InvoiceIsNulll;
                    log.ErrorDescription = "invoice is null for referenceID=" + tabbyWebHookEntity.RefrenceId;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                tabbyWebHookEntity.InvoiceNo = invoice.InvoiceNo.ToString();
                if (invoice.TotalPrice-invoice.TotalBCareDiscount != decimal.Parse(tabbyWebHookEntity.Amount.ToString()))
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.InvalidAmount;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    tabbyWebHookEntity.ErrorCode = (int)output.ErrorCode;
                    tabbyWebHookEntity.ErrorDescription = "PaymentAmount is not valid as we recived " + tabbyWebHookEntity.Amount + " and invoice amount is " + invoice.TotalPrice;
                    _tabbyPaymentService.InsertIntoTabbyWebhook(tabbyWebHookEntity);
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.InvalidAmount;
                    log.ErrorDescription = "PaymentAmount is not valid as we recived " + tabbyWebHookEntity.Amount + " and invoice amount is " + invoice.TotalPrice;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PendingPayment ||
                        checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
                {
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.Tabby;
                    string companyName = string.Empty;
                    if (checkoutDetails.InsuranceCompany != null && !string.IsNullOrEmpty(checkoutDetails.InsuranceCompany.Key))
                    {
                        companyName = checkoutDetails.InsuranceCompany.Key;
                    }
                    else
                    {
                        companyName = checkoutDetails.InsuranceCompanyName;
                    }
                    _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, companyName, checkoutDetails.Channel);
                    _checkoutDetailrepository.Update(checkoutDetails);
                    _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId, checkoutDetails.ReferenceId);
                    if (!string.IsNullOrEmpty(checkoutDetails.DiscountCode)) //mark discount code as consumed
                    {
                        _orderService.UpdateDiscountCodeToBeConsumed(checkoutDetails.VehicleId, checkoutDetails.DiscountCode, checkoutDetails.ReferenceId);
                    }
                    if (checkoutDetails.ODReference != null && checkoutDetails.ReferenceId != checkoutDetails.ODReference)
                    {
                        string exception = string.Empty;
                        var odCheckout = _checkoutsService.GetFromCheckoutDetailsByReferenceId(checkoutDetails.ODReference, out exception);
                        if (odCheckout != null)
                        {
                            hyperpayPaymentService.UpdateCheckoutPaymentStatus(odCheckout, true, odCheckout.Channel, (int)PaymentMethodCode.Tabby);
                        }
                    }
                }
                tabbyWebHookEntity.ErrorCode = (int)TabbyOutput.ErrorCodes.Success;
                tabbyWebHookEntity.ErrorDescription = "Success";
                _tabbyPaymentService.InsertIntoTabbyWebhook(tabbyWebHookEntity);

                TabbyWebHookDetails tabbyWebHookDetailsEntity = new TabbyWebHookDetails();
                tabbyWebHookDetailsEntity.TabbyWebHookId = tabbyWebHookEntity.Id;
                tabbyWebHookDetailsEntity.CreatedDate = DateTime.Now;
                tabbyWebHookDetailsEntity.Order = JsonConvert.SerializeObject(tabbyWebHookModel.order);
                tabbyWebHookDetailsEntity.Buyer = JsonConvert.SerializeObject(tabbyWebHookModel.buyer);
                tabbyWebHookDetailsEntity.BuyerHistory = JsonConvert.SerializeObject(tabbyWebHookModel.buyer_history);
                tabbyWebHookDetailsEntity.Captures = JsonConvert.SerializeObject(tabbyWebHookModel.captures);
                tabbyWebHookDetailsEntity.OrderHistory = JsonConvert.SerializeObject(tabbyWebHookModel.order_history);
                tabbyWebHookDetailsEntity.Refunds = JsonConvert.SerializeObject(tabbyWebHookModel.refunds);
                tabbyWebHookDetailsEntity.ShippingAddress = JsonConvert.SerializeObject(tabbyWebHookModel.shipping_address);
                tabbyWebHookDetailsEntity.Meta = JsonConvert.SerializeObject(tabbyWebHookModel.meta);

                _tabbyPaymentService.InsertIntoTabbyWebhookDetails(tabbyWebHookDetailsEntity);

                TabbyCaptureRequest tabbyCaptureRequestEntity = new TabbyCaptureRequest();

                tabbyCaptureRequestEntity.Amount = tabbyResponseObject.TabbyRequest.Amount;
                tabbyCaptureRequestEntity.Channel = tabbyResponseObject.Channel;
                tabbyCaptureRequestEntity.TabbyRequestId = tabbyResponseObject.TabbyRequest.Id;
                tabbyCaptureRequestEntity.CreatedDate = DateTime.Now;
                tabbyCaptureRequestEntity.RefrenceId = tabbyResponseObject.ReferenceId;
                tabbyCaptureRequestEntity.CreatedAt = DateTime.Now.ToString();
                tabbyCaptureRequestEntity.DiscountAmount = 0;
                tabbyCaptureRequestEntity.TaxAmount = 0;
                tabbyCaptureRequestEntity.UserId = tabbyResponseObject.UserId;
                tabbyCaptureRequestEntity.ShippingAmount = 0;
                tabbyCaptureRequestEntity.Items = null;

                _tabbyPaymentService.InsertIntoTabbyCaptureRequest(tabbyCaptureRequestEntity);



                var captureOutput = _tabbyPaymentService.SubmitTabbyCaptureRequest(tabbyConfig, tabbyResponseObject, tabbyWebHookModel.id);
                if (captureOutput.ErrorCode != TabbyOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)TabbyOutput.ErrorCodes.TabbyResponseObjectIsNull;
                    log.ErrorDescription = "Failed to send taby capture request due to:" + captureOutput.ErrorDescription;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var tabbyCaptureResponseEntity = new TabbyCaptureResponse();
                tabbyCaptureResponseEntity.Amount = double.Parse(captureOutput.TabbyCaptureResponseViewModel.amount);
                tabbyCaptureResponseEntity.Cancelable = captureOutput.TabbyCaptureResponseViewModel.cancelable;
                tabbyCaptureResponseEntity.Currency = captureOutput.TabbyCaptureResponseViewModel.currency;
                tabbyCaptureResponseEntity.IsExpired = captureOutput.TabbyCaptureResponseViewModel.is_expired;
                tabbyCaptureResponseEntity.Status = captureOutput.TabbyCaptureResponseViewModel.status;
                tabbyCaptureResponseEntity.Test = captureOutput.TabbyCaptureResponseViewModel.test;
                tabbyCaptureResponseEntity.UserId = tabbyResponseObject.UserId;
                tabbyCaptureResponseEntity.Channel = tabbyResponseObject.Channel;
                tabbyCaptureResponseEntity.TabbyCaptureRequestId = tabbyCaptureRequestEntity.Id;
                tabbyCaptureResponseEntity.CaptureId = captureOutput.TabbyCaptureResponseViewModel.captures[0]?.id;
                tabbyCaptureResponseEntity.OrderRefrenceId = captureOutput.TabbyCaptureResponseViewModel.order?.reference_id;
                tabbyCaptureResponseEntity.RefrenceId = tabbyResponseObject.ReferenceId;
                tabbyCaptureResponseEntity.CreatedDate = DateTime.Now;
                tabbyCaptureResponseEntity.CreatedAt = captureOutput.TabbyCaptureResponseViewModel.created_at;
                tabbyCaptureResponseEntity.ExpiresAt = captureOutput.TabbyCaptureResponseViewModel.expires_at;
                _tabbyPaymentService.InsertIntoTabbyCaptureResponse(tabbyCaptureResponseEntity);
                var tabbyCaptureResponseDetailsEntity = new TabbyCaptureResponseDetails();
                tabbyCaptureResponseDetailsEntity.CreatedDate = DateTime.Now;
                tabbyCaptureResponseDetailsEntity.TabbyCaptureResponseId = tabbyCaptureResponseEntity.Id;
                tabbyCaptureResponseDetailsEntity.Buyer = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.buyer);
                tabbyCaptureResponseDetailsEntity.BuyerHistory = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.buyer_history);
                tabbyCaptureResponseDetailsEntity.Captures = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.captures);
                tabbyCaptureResponseDetailsEntity.Meta = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.meta);
                tabbyCaptureResponseDetailsEntity.Order = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.order);
                tabbyCaptureResponseDetailsEntity.OrderHistory = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.order_history);
                tabbyCaptureResponseDetailsEntity.Product = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.product);
                tabbyCaptureResponseDetailsEntity.Refund = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.refunds);
                tabbyCaptureResponseDetailsEntity.ShippingAddress = JsonConvert.SerializeObject(captureOutput.TabbyCaptureResponseViewModel.shipping_address);
                _tabbyPaymentService.InsertIntoTabbyCaptureResponseDetails(tabbyCaptureResponseDetailsEntity);
                output.ErrorCode = TabbyOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorDescription = output.ErrorDescription;
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(date).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = TabbyOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)CheckoutOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }
        public string LogApplePayError(string userId, string referenceId, string errorDescription)
        {
            CheckoutOutput output = new CheckoutOutput();
            ApplepayErrorLog log = new ApplepayErrorLog();
            log.UserID = userId.ToString();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ReferenceId = referenceId;
            log.ErrorDescription = errorDescription;
            ApplepayErrorLogDataAccess.AddtoApplepayErrorLogs(log);
            return CheckoutResources.ErrorGenericApplePay;
        }




        #endregion
        public CheckoutOutput PrePaymentChecks(string userId, PrePaymentCheckModel model)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.UserId = userId;
            log.Channel = model.Channel;
            log.MethodName = "PrePaymentChecks";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            if (!string.IsNullOrEmpty(TestMode) && TestMode.ToLower() == "true")
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            try
            {
                var userManager = _authorizationService.GetUser(userId);
                if (userManager.Result.IsCorporateUser)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success Corporate User";
                    return output;
                }
                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model us null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.ReferenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "referenceId is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.ProductId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ProductId is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.ReferenceId = model.ReferenceId;
                QuotationResponseDBModel quoteResponse = null;
                quoteResponse = _quotationService.GetQuotationResponseByReferenceIdDB(model.ReferenceId, model.ProductId.ToString());
                if (quoteResponse == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QuotationvResponse is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.CompanyId = quoteResponse.CompanyID;
                log.CompanyName = quoteResponse.CompanyKey;
                if (!string.IsNullOrEmpty(quoteResponse.SequenceNumber))
                  log.VehicleId = quoteResponse.SequenceNumber;
                else
                 log.VehicleId = quoteResponse.CustomCardNumber;

                string driverNin = quoteResponse.InsuredNationalId;
                if (DateTime.Now.AddHours(-16) > quoteResponse.QuotationResponseCreatedDate)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Quotation Expired";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                {
                    driverNin = quoteResponse.NIN;
                }
                log.DriverNin = driverNin;
               
                //User Insurance Number Limit Per Year = 5 Polices per year
                string exception = string.Empty;
                List<InsuredPolicyInfo> userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesDetails(driverNin, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "There is an error occured due to " + exception;
                    return output;
                }
                int userSuccessPolicies = 0;
                if (userSuccessPoliciesDetails != null)
                {
                    if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                    {
                        userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true).Sum(a => a.TotalPolicyCount);
                    }
                    else
                    {
                        userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false).Sum(a => a.TotalPolicyCount);
                    }
                }
                if (userSuccessPolicies >= UserInsuranceNumberLimitPerYear)
                {
                    if (quoteResponse.InsuredNationalId.StartsWith("7")) // Company
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.CompanyDriverExceedsInsuranceNumberLimitPerYear;
                        log.ErrorDescription = "Company Driver " + driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                    }
                    else
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserExceedsInsuranceNumberLimitPerYear;
                        log.ErrorDescription = "Nin Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                    }
                    output.ErrorDescription = CheckoutResources.UserExceedsInsuranceNumberLimitCheckoutPopup;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        #region Leasing

        private async Task<LeasingUser> HandleLeasingUserAccount(Tameenk.Services.Core.Leasing.Models.AutoleasingPortalLinkModel portalLinkModel)
        {
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.MethodName = "SubmitAtoleasingCheckoutAddLeasingUser";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            //log.RequesterUrl = Utilities.GetCurrentURL;
            log.UserId = portalLinkModel.CheckOutUserId;

            var email = $"{portalLinkModel.Phone}@{portalLinkModel.BankKey}.com";
            var password = $"P@$$w0rd_{portalLinkModel.Phone}";

            try
            {
                //var aspNet_User = _authorizationService.GetUserByEmail(email);
                var aspNet_User = _authorizationService.GetUserDBByID(portalLinkModel.CheckOutUserId);
                if (aspNet_User == null)
                {
                    var aspNetUser = new AspNetUser();
                    aspNetUser.CreatedDate = DateTime.Now;
                    aspNetUser.LastModifiedDate = DateTime.Now;
                    aspNetUser.LastLoginDate = DateTime.Now;
                    aspNetUser.LockoutEndDateUtc = DateTime.UtcNow;
                    aspNetUser.LockoutEnabled = false;
                    aspNetUser.DeviceToken = "";
                    aspNetUser.Email = email;
                    aspNetUser.EmailConfirmed = true; //TODO
                    aspNetUser.RoleId = Guid.Parse("DB5159FA-D585-4FEE-87B1-D9290D515DFB"); //_db.Roles.ToList()[0].ID,
                    aspNetUser.LanguageId = Guid.Parse("5046A00B-D915-48A1-8CCF-5E5DFAB934FB"); //_db.Languages.Where(l => l.isDefault).Select(l => l.Id).SingleOrDefault(),
                    aspNetUser.PhoneNumber = portalLinkModel.Phone;
                    aspNetUser.PhoneNumberConfirmed = true; //TODO
                    aspNetUser.UserName = email;
                    aspNetUser.FullName = email;
                    aspNetUser.TwoFactorEnabled = false;
                    aspNetUser.Channel = Channel.autoleasing.ToString();
                    var result = await _authorizationService.CreateUser(aspNetUser, password);

                    if (result.Any())
                    {
                        log.ErrorCode = (int)Output<bool>.ErrorCodes.CanNotCreate;
                        log.ErrorDescription = "can not create user due to " + string.Join(",", result);
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return null;
                    }

                    aspNet_User = _authorizationService.GetUserByEmail(email);
                }

                if (string.IsNullOrEmpty(portalLinkModel.DriverNin))
                    portalLinkModel.DriverNin = _driverService.GetDriver(portalLinkModel.MainDriverId.Value.ToString()).NIN;

                string hashedPassword = SecurityUtilities.HashData(password, null);
                var leasUser = new LeasingUser();
                leasUser.CreatedDate = DateTime.Now;
                leasUser.LastModifiedDate = DateTime.Now;
                leasUser.LockoutEndDateUtc = DateTime.UtcNow.AddDays(-1);
                leasUser.LockoutEnabled = false;
                leasUser.Email = email;
                leasUser.PhoneNumber = portalLinkModel.Phone;
                leasUser.PasswordHash = hashedPassword;
                leasUser.UserName = email;
                leasUser.FullName = email;
                leasUser.BankId = portalLinkModel.BankId;
                leasUser.BankName = portalLinkModel.BankKey;
                leasUser.CreatedBy = portalLinkModel.CheckOutUserId;
                leasUser.UserId = aspNet_User.Id;
                leasUser.IsDeleted = false;
                leasUser.ReferenceId = portalLinkModel.ReferenceId;
                leasUser.DriverId = portalLinkModel.MainDriverId.Value.ToString();
                leasUser.VehicleId = portalLinkModel.VehicleId.ToString();
                leasUser.DriverNin = portalLinkModel.DriverNin;
                leasUser.VehicleSequenceOrCustom = portalLinkModel.VehicleSequenceOrCustom;
                _leasingUserRepository.Insert(leasUser);

                AutoleasingPortalLinkProcessingQueue processingQueueLink = new AutoleasingPortalLinkProcessingQueue()
                {
                    ReferenceId = portalLinkModel.ReferenceId,
                    CheckOutUserId = aspNet_User.Id,
                    Phone = portalLinkModel.Phone,
                    MainDriverId = portalLinkModel.MainDriverId ?? new Guid(),
                    NIN = portalLinkModel.DriverNin,
                    VehicleId = portalLinkModel.VehicleId,
                    VehicleSequenceOrCustom = portalLinkModel.VehicleSequenceOrCustom,
                    CompanyID = portalLinkModel.CompanyID,
                    CompanyName = portalLinkModel.CompanyName,
                    InsuranceTypeCode = portalLinkModel.InsuranceTypeCode,
                    ProcessingTries = 0,
                    CreatedDate = DateTime.Now,
                    SelectedLanguage = (int)portalLinkModel.CheckoutSelectedlang,
                    BankId = portalLinkModel.BankId,
                    BankKey = portalLinkModel.BankKey,
                    IsLocked = false,
                    IsDone = false
                };
                _autoleasingPortalLinkProcessingQueueQueueRepository.Insert(processingQueueLink);

                return leasUser;
            }
            catch (Exception ex)
            {
                log.ErrorCode = (int)Output<bool>.ErrorCodes.ExceptionError;
                log.ErrorDescription = "Exception error happend while inserting leasing user, and the error is: " + ex.ToString();
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return null;
            }
        }

        public CheckoutOutput AddLeasingItemToCart(Tameenk.Core.Domain.Dtos.AddILeasingtemToCartModel model, CheckoutRequestLog log, string lang)
        {
            DateTime dtStart = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();
            output.qtRqstExtrnlId = model.QuotaionRequestExternalId;

            try
            {
                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout Model is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                CheckoutProviderServicesCodes LeasingServiceId = (CheckoutProviderServicesCodes)model.LeasingServiceId;
                PolicyModification policyModification = _policyModificationRepository.TableNoTracking
                    .FirstOrDefault(p => p.QuotationReferenceId == model.ReferenceId
                                    && p.IsLeasing
                                    && p.ProviderServiceId.Value == LeasingServiceId
                                    && !p.IsCheckedkOut
                                    && !p.IsDeleted);

                if (policyModification == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorNoRecordFound", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"No record found in policyModification with this reference: {model.ReferenceId}";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(policyModification.ReferenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorNoRecordFound", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "policyModification.ReferenceId is null";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                model.ReferenceId = policyModification.ReferenceId;
                log.ReferenceId = model.ReferenceId;

                string exception = string.Empty;
                _shoppingCartService.EmptyLeasingShoppingCart(log.UserId.ToString(), model.ReferenceId);
                bool result = _shoppingCartService.AddLeasingItemToCart(out exception, log.UserId.ToString(), model.ReferenceId, Guid.Parse(model.ProductId)
                    , model.SelectedProductBenfitId?.Select(b => new Product_Benefit { Id = b, IsSelected = true }).ToList()
                    , model.SelectedProductDriverId?.Select(d => new Product_Driver { Id = d }).ToList());

                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToAddItemToCart;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"Error happend while add leaisng item to cart, and the error is: {exception}";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!result)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToAddItemToCart;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("SerivceIsCurrentlyDown", CultureInfo.GetCultureInfo(lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = " _shoppingCartService.AddLeasingItemToCart is return false";
                    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                string clearText = model.ReferenceId + "_" + model.QuotaionRequestExternalId + "_" + model.ProductId;
                string selectedProductBenfitId = string.Empty;
                if (model.SelectedProductBenfitId == null || model.SelectedProductBenfitId.Count() == 0)
                    clearText += SecurityUtilities.HashKey;
                else
                {
                    selectedProductBenfitId = string.Join(",", model.SelectedProductBenfitId);
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;
                }
                string hashed = SecurityUtilities.HashData(clearText, null);
                output._hValue = hashed;
                output.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.ReferenceId = model.ReferenceId;
                output.ProductId = model.ProductId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                log.ResponseTimeInSeconds = output.ResponseTimeInSeconds;
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtStart).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public CheckoutOutput SubmitLeasingCheckoutDetails(CheckoutModel model, CheckoutRequestLog log, LanguageTwoLetterIsoCode lang)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();

            log.ReferenceId = model.ReferenceId;
            log.PaymentMethod = model.PaymentMethodCode?.ToString();

            try
            {
                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout Model is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.ReferenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = " model.ReferenceId is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                CheckoutProviderServicesCodes LeasingServiceId = (CheckoutProviderServicesCodes)model.LeasingServiceId;
                PolicyModification policyModification = _policyModificationRepository.Table
                    .FirstOrDefault(p => p.ReferenceId == model.ReferenceId
                                    && p.IsLeasing
                                    && p.ProviderServiceId.Value == LeasingServiceId
                                    && !p.IsCheckedkOut
                                    && !p.IsDeleted);

                if (policyModification == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.ErrorNoRecordFound;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"No record found in policyModification with this reference: {model.ReferenceId}";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                var userManager = _authorizationService.GetUser(log.UserId);
                log.UserName = userManager.Result?.UserName;
                if (userManager == null || userManager.Result == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserNotFound;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userManager is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (userManager.Result.LockoutEnabled && userManager.Result.LockoutEndDateUtc >= DateTime.UtcNow)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.UserLockedOut;
                    output.ErrorDescription = CheckoutResources.AccountLocked;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User is locked";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                Guid userId = Guid.Empty;
                Guid.TryParse(model.UserId, out userId);
                if (userId == Guid.Empty)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorAnonymous;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "User ID is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                string clearText = model.ReferenceId + "_" + model.QtRqstExtrnlId + "_" + model.ProductId;
                if (string.IsNullOrEmpty(model.SelectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += model.SelectedProductBenfitId + SecurityUtilities.HashKey;
                if (!SecurityUtilities.VerifyHashedData(model.Hashed, clearText))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Phone))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Phone number is empty";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (!Utilities.IsValidPhoneNo(model.Phone))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPhone;
                    output.ErrorDescription = CheckoutResources.checkout_error_phone;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "incorrect phone format as phone is " + model.Phone;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                //check if payment method is active 
                if (!model.IsLeasingPaymentMethod)
                {
                    var paymentMethod = _paymentMethodService.GetPaymentMethodsByCode(model.PaymentMethodCode.GetValueOrDefault());
                    if (paymentMethod == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Invalid Payment Methods " + model.PaymentMethodCode.GetValueOrDefault();
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                }

                // create checkout detail object.
                // here need to get additional drivers as well as like benefits
                var shoppingCartItem = _shoppingCartService.GetLeasingUserShoppingCartItemDBByUserIdAndReferenceId(log.UserId.ToString(), model.ReferenceId);
                if (shoppingCartItem == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (shoppingCartItem.ProductId == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.Checkout_ShoppingCartItemIsNull;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item product is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (shoppingCartItem.ProductId.ToString().ToLower() != model.ProductId.ToLower())
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ConfilictProduct;
                    output.ErrorDescription = CheckoutResources.ErrorSecurity;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "conflict product as DB return " + shoppingCartItem.ProductId.ToString() + " and user select " + model.ProductId;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.CompanyId = shoppingCartItem.InsuranceCompanyID;
                log.CompanyName = shoppingCartItem.InsuranceCompanyKey;

                if (shoppingCartItem.ShoppingCartItemBenefits != null && shoppingCartItem.ShoppingCartItemBenefits.Count > 0)
                {
                    var oldBenefits = _productBenefitRepository.Table.Where(a => a.ProductId == shoppingCartItem.ProductId).ToList();
                    if (oldBenefits != null && oldBenefits.Count > 0)
                        _productBenefitRepository.Delete(oldBenefits);

                    List<Product_Benefit> product_Benefits = new List<Product_Benefit>();
                    foreach (var benefit in shoppingCartItem.ShoppingCartItemBenefits)
                    {
                        var quotation_product_benefit = _quotationProductBenefitRepository.TableNoTracking
                                                            .Include(a => a.Product).Include(a => a.Benefit)
                                                            .Where(a => a.Id == benefit.Id).FirstOrDefault();
                        if (quotation_product_benefit == null)
                            continue;

                        product_Benefits.Add(new Product_Benefit()
                        {
                            ProductId = quotation_product_benefit.ProductId,
                            BenefitId = quotation_product_benefit.BenefitId,
                            IsSelected = true,
                            IsReadOnly = quotation_product_benefit.IsReadOnly,
                            BenefitPrice = quotation_product_benefit.BenefitPrice,
                            BenefitExternalId = quotation_product_benefit.BenefitExternalId,
                            BenefitNameAr = quotation_product_benefit.BenefitNameAr,
                            BenefitNameEn = quotation_product_benefit.BenefitNameEn,
                            AveragePremium = quotation_product_benefit.AveragePremium,
                            CoveredCountry = quotation_product_benefit.CoveredCountry,
                        });
                    }

                    //var product = quotation_product_benefit.Product;
                    //product.Quotation_Product_Benefits = null;
                    //product_Benefit.Product = product;

                    //var _benefit = quotation_product_benefit.Benefit;
                    //_benefit.ArabicDescription = product_Benefit.BenefitNameAr;
                    //_benefit.EnglishDescription = product_Benefit.BenefitNameEn;
                    //_benefit.Quotation_Product_Benefits = null;
                    //product_Benefit.Benefit = _benefit;

                    if (product_Benefits != null && product_Benefits.Count > 0)
                        _productBenefitRepository.Insert(product_Benefits);
                }

                QuotationResponseDBModel quoteResponse = null;
                // here need to get added drivers in (Add driver service) to add them in checkout
                quoteResponse = _quotationService.GetAutoleasingQuotationResponseByReferenceIdDB(policyModification.QuotationReferenceId, shoppingCartItem.ProductId.ToString());
                if (quoteResponse == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.QuotationRequestExpired;
                    output.ErrorDescription = CheckoutResources.QuotationExpired;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QuotationvResponse is null";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (quoteResponse.AutoleasingInitialOptionResponce.HasValue && quoteResponse.AutoleasingInitialOptionResponce == true)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.initialOptionPurchasedError;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "initial Option can't be purchased";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                log.DriverNin = quoteResponse.NIN;
                log.VehicleId = (quoteResponse.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard) ? quoteResponse.CustomCardNumber : quoteResponse.SequenceNumber;

                //var phoneInfo = _orderService.CheckIfPhoneAlreadyUsed(Utilities.ValidatePhoneNumber(model.Phone), quoteResponse.NIN);
                //if (phoneInfo != null)
                //{
                //    output.ErrorCode = CheckoutOutput.ErrorCodes.PhoneAlreadyUsed;
                //    output.ErrorDescription = CheckoutResources.PhoneNoWithAnotherDriver;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "phone Number " + model.Phone + " already used with another driver " + phoneInfo.NIN + " and current driver id is " + quoteResponse.NIN;
                //    log.ResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                //    return output;
                //}

                string exception = string.Empty;
                bool isEmailVerified = true;
                short insuranceTypeCode = 0;
                if (quoteResponse.ProductInsuranceTypeCode.HasValue)
                {
                    short.TryParse(quoteResponse.ProductInsuranceTypeCode.Value.ToString(), out insuranceTypeCode);
                    quoteResponse.InsuranceTypeCode = insuranceTypeCode;
                }

                int? oldPaymentMethodId = 0;
                var checkoutDetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(model.ReferenceId);
                if (checkoutDetails == null)
                {
                    checkoutDetails = new CheckoutDetail
                    {
                        ReferenceId = model.ReferenceId,
                        BankCodeId = model.BankCode,
                        Email = model.Email,
                        //IBAN = model.IBAN.Replace(" ", "").Trim().ToLower(),
                        Phone = Utilities.ValidatePhoneNumber(model.Phone),
                        PaymentMethodId = model.PaymentMethodCode,
                        UserId = model.UserId,
                        MainDriverId = quoteResponse.DriverId,
                        PolicyStatusId = model.IsLeasingPaymentMethod ? (int)EPolicyStatus.Pending : (int)EPolicyStatus.PendingPayment,
                        VehicleId = quoteResponse.VehicleId,
                        CreatedDateTime = DateTime.Now,
                        SelectedInsuranceTypeCode = quoteResponse.InsuranceTypeCode,
                        SelectedLanguage = lang,
                        InsuranceCompanyId = quoteResponse.CompanyID,
                        InsuranceCompanyName = quoteResponse.CompanyKey,
                        Channel = log.Channel,
                        IsEmailVerified = isEmailVerified,
                        SelectedProductId = shoppingCartItem.ProductId,
                        MerchantTransactionId = Guid.NewGuid(),
                        MainPolicyReferenceId = policyModification.QuotationReferenceId,
                        Isleasing = true,
                        ProviderServiceId = LeasingServiceId,
                        InsuredId = quoteResponse.InsuredTableRowId,
                        ExternalId = quoteResponse.ExternalId
                    };

                    var oldCheckoutDetails = _checkoutDetailrepository.TableNoTracking
                        .Where(c => c.MainPolicyReferenceId == policyModification.QuotationReferenceId)
                        .OrderByDescending(x => x.CreatedDateTime)
                        .FirstOrDefault();
                    if (oldCheckoutDetails != null)
                    {
                        checkoutDetails.AdditionalDriverIdOne = oldCheckoutDetails.AdditionalDriverIdOne;
                        checkoutDetails.AdditionalDriverIdTwo = oldCheckoutDetails.AdditionalDriverIdTwo;
                    }

                    checkoutDetails.OrderItems = _orderService.CreateLeasingOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId, out exception);
                    if (shoppingCartItem.ShoppingCartItemDrivers != null && shoppingCartItem.ShoppingCartItemDrivers.Count > 0)
                    {
                        foreach (var driver in shoppingCartItem.ShoppingCartItemDrivers)
                        {
                            checkoutDetails.CheckoutAdditionalDrivers.Add(new CheckoutAdditionalDriver()
                            {
                                CheckoutDetailsId = checkoutDetails.ReferenceId,
                                DriverId = driver.DriverId
                            });
                            if (driver.DriverId.ToString().ToLower() == quoteResponse.DriverId.ToString().ToLower())//driver is main driver
                                continue;

                            if (!checkoutDetails.AdditionalDriverIdOne.HasValue)
                                checkoutDetails.AdditionalDriverIdOne = driver.DriverId;
                            else if (!checkoutDetails.AdditionalDriverIdTwo.HasValue)
                                checkoutDetails.AdditionalDriverIdTwo = driver.DriverId;
                        }
                    }

                    _orderService.CreateCheckoutDetails(checkoutDetails);
                }
                else
                {
                    oldPaymentMethodId = checkoutDetails.PaymentMethodId;
                    if (_orderService.DeleteOrderItemByRefrenceId(model.ReferenceId, out exception))
                        _orderService.SaveOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId, out exception);

                    checkoutDetails.UserId = model.UserId;
                    checkoutDetails.SelectedLanguage = lang;
                    checkoutDetails.Email = model.Email;
                    //checkoutDetails.IBAN = model.IBAN.Replace(" ", "").Trim().ToLower();
                    checkoutDetails.Phone = Utilities.ValidatePhoneNumber(model.Phone);
                    checkoutDetails.BankCodeId = model.BankCode;
                    checkoutDetails.PaymentMethodId = model.PaymentMethodCode;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    checkoutDetails.Channel = log.Channel;
                    checkoutDetails.SelectedProductId = shoppingCartItem.ProductId;
                    checkoutDetails.MerchantTransactionId = Guid.NewGuid();
                    checkoutDetails.MainPolicyReferenceId = policyModification.QuotationReferenceId;
                    checkoutDetails.Isleasing = true;
                    checkoutDetails.ProviderServiceId = LeasingServiceId;
                    checkoutDetails.InsuredId = quoteResponse.InsuredTableRowId;
                    checkoutDetails.ExternalId = quoteResponse.ExternalId;
                    _orderService.UpdateCheckout(checkoutDetails);
                }
                //_quotationService.UpdateQuotationResponseToBeCheckedout(quoteResponse.QuotationResponseId, shoppingCartItem.ProductId);

                policyModification.IsCheckedkOut = true;
                _policyModificationRepository.Update(policyModification);

                // check if invoice already exists
                var invoice = _orderService.GetInvoiceByRefrenceId(model.ReferenceId);
                if (invoice != null)
                {
                    // cancel invoice
                    if (!_orderService.DeleteInvoiceByRefrenceId(model.ReferenceId, log.UserId.ToString(), out exception))
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.FailedToDeleteOldInvoice;
                        output.ErrorDescription = CheckoutResources.FailedToDeleteOldInvoice;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed To Delete previous Invoice with InvoiceNo " + invoice.InvoiceNo + " and InvoiceId " + invoice.Id + " due to " + exception;
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }
                    invoice = _orderService.CreateLeasingInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID, invoice.InvoiceNo);
                }
                else
                {
                    invoice = _orderService.CreateLeasingInvoice(model.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID);
                }

                log.Amount = invoice.TotalPrice.Value;
                if (log.Amount > 5000 && model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Invoice exceeded 5000 as its:" + log.Amount + " can not use Taby";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                ////refresh Cache
                //exception = string.Empty;
                //bool retVal = _quotationService.GetQuotationResponseCacheAndDelete(quoteResponse.CompanyID, quoteResponse.InsuranceTypeCode.Value, quoteResponse.ExternalId, out exception);
                //if (!retVal && !string.IsNullOrEmpty(exception))
                //{
                //    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                //    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "Failed to delete from cache due to " + exception;
                //    dtAfterCalling = DateTime.Now;
                //    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                //    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                //    return output;
                //}

                if (model.IsLeasingPaymentMethod)
                {
                    _policyProcessingService.InsertPolicyProcessingQueue(model.ReferenceId, model.InsuranceCompanyId, checkoutDetails.InsuranceCompanyName, model.Channel);

                    log.PaymentMethod = "Leasing";
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    output.CheckoutModel = model;
                    return output;
                }

                string customerName = string.Format("{0} {1} {2}", quoteResponse.EnglishFirstName, quoteResponse.EnglishSecondName, quoteResponse.EnglishLastName);
                if (string.IsNullOrWhiteSpace(quoteResponse.EnglishFirstName.Replace("-", string.Empty)) &&
                    string.IsNullOrWhiteSpace(quoteResponse.EnglishSecondName.Replace("-", string.Empty)) &&
                    string.IsNullOrWhiteSpace(quoteResponse.EnglishLastName.Replace("-", string.Empty)))
                {
                    customerName = "BCare";
                }
                var paymentRequestModel = new PaymentRequestModel()
                {
                    UserId = checkoutDetails.UserId,
                    ReferenceId = checkoutDetails.ReferenceId,
                    UserEmail = model.Email,
                    CustomerNameAr = string.Format("{0} {1} {2}", quoteResponse.FirstName, quoteResponse.SecondName, quoteResponse.LastName),
                    CustomerNameEn = customerName,
                    PaymentAmount = _orderService.CalculateCheckoutDetailTotal(checkoutDetails),
                    InvoiceNumber = invoice.InvoiceNo
                };

                paymentRequestModel.RequestId = "03-" + paymentRequestModel.InvoiceNumber.ToString() + "-" + _rnd.Next(111111, 999999);
                if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Hyperpay
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Mada
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.AMEX
                      || model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Mada)
                        log.PaymentMethod = "Hyperpay-Mada";
                    else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.AMEX)
                        log.PaymentMethod = "Hyperpay-AMEX";
                    else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.ApplePay)
                    {
                        log.PaymentMethod = "ApplePay";
                        model.PaymentAmount = paymentRequestModel.PaymentAmount;
                    }
                    else
                        log.PaymentMethod = "Hyperpay";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    model.IsCheckoutEmailVerified = isEmailVerified;
                    output.IsCheckoutEmailVerified = isEmailVerified;
                    output.CheckoutModel = model;
                    if (log.Channel.ToLower() == "ios" || log.Channel.ToLower() == "android")
                    {
                        HyperpayRequest hyperpayRequest = new HyperpayRequest();
                        hyperpayRequest.Amount = Math.Round(paymentRequestModel.PaymentAmount, 2);
                        hyperpayRequest.UserId = log.UserId.ToString();
                        hyperpayRequest.CreatedDate = DateTime.Now;
                        hyperpayRequest.UserEmail = paymentRequestModel.UserEmail;
                        hyperpayRequest.ReferenceId = paymentRequestModel.ReferenceId;
                        var hyperpayOutput = hyperpayPaymentService.RequestHyperpayForLeasing(hyperpayRequest, log.CompanyId.Value, log.CompanyName, log.Channel, checkoutDetails.MerchantTransactionId.Value, out exception);
                        if (hyperpayOutput.ErrorCode != HyperSplitOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                            output.ErrorDescription = CheckoutResources.InvalidPayment;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "RequestHyperpayUrlWithSplitOption return an error " + hyperpayOutput.ErrorDescription;
                            CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                            return output;
                        }
                        hyperpayRequest = hyperpayOutput.HyperpayRequest;
                        output.HyperPayCheckoutId = hyperpayRequest.ResponseId;
                        output.HyperpayRequestId = hyperpayRequest.Id;
                        hyperpayRequest = hyperpayPaymentService.GetById(output.HyperpayRequestId);
                        if (hyperpayRequest.CheckoutDetails == null || hyperpayRequest.CheckoutDetails.Count == 0)
                        {
                            ////error
                            hyperpayRequest.CheckoutDetails.Add(checkoutDetails);
                            hyperpayPaymentService.UpdateHyperRequest(hyperpayRequest);
                        }
                    }
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Tabby)
                {
                    File.WriteAllText(@"C:\inetpub\WataniyaLog\Tabby.txt", "ENter tabby Condition");
                    log.PaymentMethod = "Tabby";
                    var tabbyoutput = ExecuteTabbyPayment(log.Channel, checkoutDetails, invoice, model, quoteResponse);
                    if (tabbyoutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                        output.ErrorDescription = CheckoutResources.Checkout_Sadadpayment_instruction_error;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Tabby return an error " + tabbyoutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    else
                    {
                        LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                        LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;

                        var companyName = culture == LanguageTwoLetterIsoCode.Ar ? shoppingCartItem.InsuranceCompanyNameAR : shoppingCartItem.InsuranceCompanyNameEN;

                        var amount = Math.Round(paymentRequestModel.PaymentAmount, 2);
                        var message = string.Format(Tameenk.Resources.WebResources.WebResources.SadadSMSMessage,
                             companyName, amount, tabbyoutput.tabbyResponseHandlerModel.ResponseBody.payment?.id);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = checkoutDetails.Phone,
                            MessageBody = message,
                            Method = SMSMethod.SadadInvoice.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = log.Channel,
                            ReferenceId = model.ReferenceId
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);

                        output.tabbyResponseHandlerModel = tabbyoutput.tabbyResponseHandlerModel;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        model.IsCheckoutEmailVerified = isEmailVerified;
                        output.IsCheckoutEmailVerified = isEmailVerified;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    output.CheckoutModel = model;
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Edaat)
                {
                    log.PaymentMethod = "Edaat";
                    var edaatoutput = ExecuteEdaatPayment(log.Channel, checkoutDetails, invoice, model.QtRqstExtrnlId, quoteResponse);
                    if (edaatoutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Failed;
                        output.ErrorDescription = CheckoutResources.Checkout_Sadadpayment_instruction_error;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "edaat return an error " + edaatoutput.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    else
                    {
                        LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                        LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;

                        var companyName = culture == LanguageTwoLetterIsoCode.Ar ? shoppingCartItem.InsuranceCompanyNameAR : shoppingCartItem.InsuranceCompanyNameEN;

                        var amount = Math.Round(paymentRequestModel.PaymentAmount, 2);
                        var message = string.Format(Tameenk.Resources.WebResources.WebResources.SadadSMSMessage,
                             companyName, amount, edaatoutput.EdaatPaymentResponseModel.InvoiceNo);
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = checkoutDetails.Phone,
                            MessageBody = message,
                            Method = SMSMethod.SadadInvoice.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = log.Channel,
                            ReferenceId = model.ReferenceId
                        };
                        _notificationService.SendSmsBySMSProviderSettings(smsModel);

                        output.EdaatPaymentResponseModel = edaatoutput.EdaatPaymentResponseModel;
                        output.EdaatPaymentResponseModel.PremiumAmount = amount.ToString();
                        output.EdaatPaymentResponseModel.CompanyName = companyName;
                        output.EdaatPaymentResponseModel.IsCheckoutEmailVerified = isEmailVerified;
                        output.EdaatPaymentResponseModel.CheckoutEmail = model.Email;
                        output.EdaatPaymentResponseModel.CheckoutReferenceId = model.ReferenceId;
                        output.EdaatPaymentResponseModel.ReferenceId = paymentRequestModel.ReferenceId;
                        output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        model.IsCheckoutEmailVerified = isEmailVerified;
                        output.IsCheckoutEmailVerified = isEmailVerified;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    }
                    output.CheckoutModel = model;
                    return output;
                }
                else if (model.PaymentMethodCode.GetValueOrDefault() == (int)PaymentMethodCode.Wallet)
                {
                    if (!userManager.Result.IsCorporateUser)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The user {userId} is not Corporate";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }

                    var corporateUser = _corporateUsersRepository.TableNoTracking.FirstOrDefault(u => u.UserId == userManager.Result.Id && u.IsActive);
                    if (corporateUser == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The user {userId} doesn't exist in Corporate users";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }

                    var corporateAccount = _corporateAccountRepository.Table.FirstOrDefault(c => c.Id == corporateUser.CorporateAccountId && c.IsActive == true);
                    if (corporateAccount == null)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.AccountIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.InvalidPaymentMethod;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The corprate user {userId} doesn't have in Corporate Account";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;

                    }

                    var totalAmount = CalculateTotalPaymentAmount(shoppingCartItem);
                    var balance = corporateAccount.Balance.HasValue ? corporateAccount.Balance.Value : 0;
                    if (totalAmount > balance)
                    {
                        output.ErrorCode = CheckoutOutput.ErrorCodes.UserIsNotCorporate;
                        output.ErrorDescription = CheckoutResources.ThereIsNotEnoughBalance;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"The corprate user {userId}, corporate account {corporateUser.CorporateAccountId} doesn't have enough balance {balance}, amount to pay {totalAmount}";
                        dtAfterCalling = DateTime.Now;
                        log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                        CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                        return output;
                    }



                    CorporateWalletHistory corporateWalletHistory = new CorporateWalletHistory();
                    corporateWalletHistory.CorporateAccountId = corporateUser.CorporateAccountId;
                    corporateWalletHistory.ReferenceId = checkoutDetails.ReferenceId;
                    corporateWalletHistory.Amount = -Math.Round(paymentRequestModel.PaymentAmount, 2);
                    corporateWalletHistory.MethodName = "SubmitCheckoutDetails";
                    corporateWalletHistory.CreatedDate = DateTime.Now;
                    corporateWalletHistory.CreatedBy = corporateUser.UserName;

                    _corporateWalletHistoryRepository.Insert(corporateWalletHistory);

                    //Update Balance
                    corporateAccount.Balance -= Math.Round(paymentRequestModel.PaymentAmount, 2);
                    _corporateAccountRepository.Update(corporateAccount);


                    var checkoutDetailsToUpdate = _checkoutsService.GetCheckoutDetails(checkoutDetails.ReferenceId);
                    checkoutDetailsToUpdate.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                    checkoutDetailsToUpdate.CorporateAccountId = corporateUser.CorporateAccountId;
                    _orderService.UpdateCheckout(checkoutDetailsToUpdate);

                    _policyProcessingService.InsertPolicyProcessingQueue(model.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, checkoutDetails.InsuranceCompanyName, model.Channel);
                    //var updateCreateOutput = WalletCreateOrder(checkoutDetails);
                    output.WalletPaymentResponseModel = new WalletPaymentResponseModel();
                    output.WalletPaymentResponseModel.ReferenceId = checkoutDetails.ReferenceId;
                    output.WalletPaymentResponseModel.Status = "Succeeded";
                    output.WalletPaymentResponseModel.ErrorMessage = "Succeeded";
                    output.WalletPaymentResponseModel.NewBalance = corporateAccount.Balance?.ToString();
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);

                    output.CheckoutModel = model;
                    return output;
                }
                else
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.InvalidPaymentMethods;
                    output.ErrorDescription = CheckoutResources.InvalidPaymentMethods;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invalid payment method as we received  " + model.PaymentMethodCode;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                dtAfterCalling = DateTime.Now;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
        }

        public CheckoutOutput PaymentUsingHyperpayLeasing(string referenceId, string QtRqstExtrnlId, string productId, string selectedProductBenfitId, string hashed, int paymentMethodCode, string channel, string lang, string userId)
        {
            CheckoutOutput output = new CheckoutOutput();
            DateTime dtBeforeCalling = DateTime.Now;
            DateTime dtAfterCalling = DateTime.Now;
            CheckoutRequestLog log = new CheckoutRequestLog();
            log.ReferenceId = referenceId;
            log.MethodName = "PaymentUsingHyperpay";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            if (paymentMethodCode == (int)PaymentMethodCode.Mada)
                log.PaymentMethod = "Hyperpay-Mada";
            else if (paymentMethodCode == (int)PaymentMethodCode.AMEX)
                log.PaymentMethod = "Hyperpay-AMEX";
            else if (paymentMethodCode == (int)PaymentMethodCode.ApplePay)
                log.PaymentMethod = "Hyperpay-ApplePay";
            else
                log.PaymentMethod = "Hyperpay";
            log.Channel = channel;
            log.UserId = userId;
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = $"ReferenceId: {referenceId}, QtRqstExtrnlId: {QtRqstExtrnlId}, productId: {productId}, selectedProductBenfitId: {selectedProductBenfitId}, hashed: {hashed}, paymentMethodCode: {paymentMethodCode}, channel: {channel}, lang: {lang}, userId: {userId}";
            if (!string.IsNullOrEmpty(log.UserId))
            {
                var userManager = _authorizationService.GetUser(log.UserId);
                log.UserName = userManager.Result?.UserName;
            }
            string exception = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "userid is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(referenceId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ReferenceId is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                string clearText = referenceId + "_" + QtRqstExtrnlId + "_" + productId;
                if (string.IsNullOrEmpty(selectedProductBenfitId))
                    clearText += SecurityUtilities.HashKey;
                else
                    clearText += selectedProductBenfitId + SecurityUtilities.HashKey;

                if (!SecurityUtilities.VerifyHashedData(hashed, clearText))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.HashedNotMatched;
                    output.ErrorDescription = CheckoutResources.ErrorHashing;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Hashed Not Matched as clear text is:" + clearText + " and hashed is:" + hashed;
                    dtAfterCalling = DateTime.Now;
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var checkoutDetail = _orderService.GetCheckoutDetailByReferenceId(referenceId);
                if (checkoutDetail == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyReturnObject;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Checkout is null";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }

                if (checkoutDetail.Vehicle?.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                    log.VehicleId = checkoutDetail.Vehicle?.CustomCardNumber;
                else
                    log.VehicleId = checkoutDetail.Vehicle?.SequenceNumber;

                log.DriverNin = checkoutDetail.Driver?.NIN;
                log.CompanyId = checkoutDetail.InsuranceCompanyId.Value;
                log.CompanyName = checkoutDetail.InsuranceCompany.Key;

                // Check if checkout detail is already paid.
                if (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentSuccess)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.PaidBefore;
                    output.ErrorDescription = CheckoutResources.AlreadyPaidBefore;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkoutDetail PolicyStatusId is PaymentSuccess";
                    log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                var hyperpayRequest = new HyperpayRequest();
                hyperpayRequest.AccessToken = _config.Hyperpay.AccessToken;
                hyperpayRequest.EntityId = _config.Hyperpay.EntityId;
                hyperpayRequest.ReferenceId = referenceId;
                //hyperpayRequest.Amount = Math.Round(_orderService.CalculateCheckoutDetailTotal(checkoutDetail), 2);
                hyperpayRequest.Amount = (checkoutDetail.Isleasing) ? Math.Round(_orderService.CalculateLeasingCheckoutDetailTotal(checkoutDetail), 2) : Math.Round(_orderService.CalculateCheckoutDetailTotal(checkoutDetail), 2);
                hyperpayRequest.CreatedDate = DateTime.Now;
                hyperpayRequest.UserId = log.UserId.ToString();
                hyperpayRequest.UserEmail = checkoutDetail.Email;
                log.Amount = hyperpayRequest.Amount;
                hyperpayRequest.CheckoutDetails.Add(checkoutDetail);
                HyperpayRequest response = null;
                var hyperpayOutput = hyperpayPaymentService.RequestHyperpayForLeasing(hyperpayRequest, checkoutDetail.InsuranceCompanyId.Value, checkoutDetail.InsuranceCompanyName, checkoutDetail.Channel, checkoutDetail.MerchantTransactionId.Value, out exception);
                if (hyperpayOutput.ErrorCode == HyperSplitOutput.ErrorCodes.Success && hyperpayOutput.HyperpayRequest != null && hyperpayOutput.HyperpayRequest.ResponseCode == "000.200.100")
                {
                    response = hyperpayOutput.HyperpayRequest;
                    output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                    output.ErrorDescription = CheckoutResources.Success;

                    output.HyperPayCheckoutId = response.ResponseId;
                    output.ReferenceId = response.ReferenceId;
                    output.CheckoutModel = new CheckoutModel();
                    output.CheckoutModel.Email = response.UserEmail;
                    output.IsCheckoutEmailVerified = checkoutDetail.IsEmailVerified.HasValue ? checkoutDetail.IsEmailVerified.Value : false;
                    output.CheckoutModel.PaymentMethodCode = paymentMethodCode;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Sucess";
                    CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                    return output;
                }
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceDown;
                output.ErrorDescription = CheckoutResources.InvalidPayment;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "response.ResponseCode is not success it return " + response?.ResponseCode + " sent data is " + exception;
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.InvalidPayment;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString() + " exception: " + exception;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                log.ResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                CheckoutRequestLogDataAccess.AddCheckoutRequestLog(log);
                return output;
            }

        }

        public bool LeasingHandleAutoleasingPoliciesToSendSmsPortalLink(Tameenk.Services.Core.Leasing.Models.AutoleasingPortalLinkModel portalLinkModel, out string exception)
        {
            exception = string.Empty;
            try
            {
                string smsBody = string.Empty;
                if (portalLinkModel.CheckoutSelectedlang == LanguageTwoLetterIsoCode.En)
                    smsBody += $"The Insurance Policy has been issued successfully. You can find policy details and after sales services via the below link: ";
                else
                    smsBody += "تم إصدار وثيقة التأمين بنجاح. يمكنك الاطلاع على تفاصيل وثيقة التأمين وخدمات ما بعد البيع عن طريق الرابط التالي: ";

                smsBody += $"https://Lease.bcare.com.sa/login/{portalLinkModel.BankKey}";
                var smsModel = new SMSModel()
                {
                    PhoneNumber = portalLinkModel.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.Leasing.ToString(),
                    Module = Module.Autoleasing.ToString(),
                    Channel = Channel.Leasing.ToString(),
                    ReferenceId = portalLinkModel.ReferenceId
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        #endregion

        #region IVR

        public CheckoutOutput AddIVRItemToChart(RenewalAddItemToChartModel checkOutModel, IVRServicesLog log)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            CheckoutOutput output = new CheckoutOutput();

            try
            {
                if (checkOutModel == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Model data model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(checkOutModel.Token))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "checkOutModel.Token value is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var decryptedToken = AESEncryption.DecryptString(Utilities.GetDecodeUrl(checkOutModel.Token), IVR_SHARED_KEY);
                var model = JsonConvert.DeserializeObject<RenewalAddItemToChartUserModel>(decryptedToken);
                if (model == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model after DeserializeObject is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(model);

                if (!model.IsIVR)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Can not proceed request, as model.IsIVR = false";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.VehicleId))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.VehicleId is empty";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (_quotationService.GetQuotationResponseByReferenceId(model.ReferenceId) == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "_quotationService.GetQuotationResponseByReferenceId return null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                string exception;
                IVRTicketPolicyDetails oldPolicyDetails = _iVRService.GetLastPolicyBySequenceOrCustomCardNumber(model.VehicleId, out exception);
                if (!string.IsNullOrEmpty(exception) || oldPolicyDetails == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = !string.IsNullOrEmpty(exception) ? "Error happend while renewal process" : "There is no old policy data for the provided vehicleId " + model.VehicleId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "oldPolicyDetails is null for the provided vehilceId " + model.VehicleId;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (!oldPolicyDetails.PolicyExpiryDate.HasValue || oldPolicyDetails.PolicyExpiryDate.Value > DateTime.Now.AddDays(28))
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = !oldPolicyDetails.PolicyExpiryDate.HasValue ? "Error happend while renewal process" : "Can not renew your policy as it's expiration is > 28 days";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !oldPolicyDetails.PolicyExpiryDate.HasValue ? "oldPolicyDetails.PolicyExpiryDate value is null" : "Can not renew your policy as it's expiration is > 28 days";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                // 1- make user logged in
                var loginResult = IVRLogin(oldPolicyDetails.CheckoutUserId, out exception);
                if (!string.IsNullOrEmpty(exception) || loginResult == null)
                {
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while renewal process";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "loginResult is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output = HandleIVRAddItemToCart(model, log, loginResult.UserId, dtBeforeCalling);
                if (output.ErrorCode != Services.Checkout.Components.Output.CheckoutOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                // return model.selectedProductBeneftis
                if (model.SelectedProductBenfitId != null && model.SelectedProductBenfitId.Count > 0)
                    output.SelectedProductBenfitId = model.SelectedProductBenfitId;

                // return logged in email
                output.LoginResult = loginResult;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"AddIVRItemToChart Exception, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> HandleIVRSendSadadNumber(RenewalAddItemToChartUserModel lowestProduct, bool sendToCallerPhone, string phoneNumber, IVRServicesLog log)
        {
            string exception = string.Empty;
            DateTime dtBeforeCalling = DateTime.Now;
            IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel> output = new IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>();
            output.Result = new RenewalSendLowestLinkSMSResponseModel() { IsSuccess = false };

            try
            {
                var addItemToCartOutput = HandleIVRAddItemToCart(lowestProduct, log, lowestProduct.OldPolicyUserId, dtBeforeCalling);
                if (addItemToCartOutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var shoppingCartItem = _shoppingCartService.GetUserShoppingCartItemDBByUserIdAndReferenceId(lowestProduct.OldPolicyUserId, lowestProduct.ReferenceId);
                if (shoppingCartItem == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (shoppingCartItem.ProductId == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "shopping cart item product is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (shoppingCartItem.ProductId.ToString().ToLower() != lowestProduct.ProductId.ToString().ToLower())
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "conflict product as DB return " + shoppingCartItem.ProductId.ToString() + " and user select " + lowestProduct.ProductId;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (shoppingCartItem.InsuranceCompanyKey == "SAICO" && (shoppingCartItem.ShoppingCartItemBenefits == null || shoppingCartItem.ShoppingCartItemBenefits.Count() == 0))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    output.ErrorDescription = CheckoutResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "InsuranceCompanyKey is SAICO and shoppingCartItem.ShoppingCartItemBenefits is null for " + shoppingCartItem.ProductId;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var quoteResponse = _quotationService.GetQuotationResponseByReferenceIdDB(lowestProduct.ReferenceId, lowestProduct.ProductId.ToString());
                if (quoteResponse == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "QuotationvResponse is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var checkActivePoliciesResult = ValidateIVRRequest(quoteResponse.InsuredNationalId, quoteResponse.NIN, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"CheckForUserActivePolicies return {exception}";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var checkoutDetails = HandleIVRCheckoutANdInvoice(lowestProduct, shoppingCartItem, quoteResponse, out exception);
                if (checkoutDetails == null || !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "checkoutDetails is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var invoice = _orderService.GetInvoiceByRefrenceId(lowestProduct.ReferenceId);
                if (invoice == null)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invoice is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                _quotationService.UpdateQuotationResponseToBeCheckedout(quoteResponse.QuotationResponseId, shoppingCartItem.ProductId);
                _quotationService.GetQuotationResponseCacheAndDelete(quoteResponse.CompanyID, quoteResponse.InsuranceTypeCode.Value, quoteResponse.ExternalId, out exception);

                var edaatoutput = ExecuteEdaatPayment("IVR", checkoutDetails, invoice, lowestProduct.ExternalId, quoteResponse);
                if (edaatoutput.ErrorCode != CheckoutOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "edaat return an error " + edaatoutput.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase)
                                            ? LanguageTwoLetterIsoCode.Ar
                                            : LanguageTwoLetterIsoCode.En;

                var paymentAmount = _orderService.CalculateCheckoutDetailTotal(checkoutDetails);
                if (paymentAmount <= 0)
                {
                    output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while send sned sadad number";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "paymentAmount <= 0";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var amount = Math.Round(paymentAmount, 2);
                var companyName = culture == LanguageTwoLetterIsoCode.Ar ? shoppingCartItem.InsuranceCompanyNameAR : shoppingCartItem.InsuranceCompanyNameEN;
                var message = string.Format(Tameenk.Resources.WebResources.WebResources.SadadSMSMessage, companyName, amount, edaatoutput.EdaatPaymentResponseModel.InvoiceNo);
                
                string phoneToSendSMS = sendToCallerPhone ? phoneNumber : checkoutDetails.Phone;
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneToSendSMS,
                    MessageBody = message,
                    Method = SMSMethod.SadadInvoice.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = "IVR",
                    ReferenceId = lowestProduct.ReferenceId
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);

                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result.IsSuccess = true;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error happend while send sned sadad number";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Error happend while send sned sadad number, and error is: {ex.ToString()}";
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        #region IVR Private Methods

        private RenewalLoginResponseModel IVRLogin(string userId, out string exception)
        {
            exception = string.Empty;
            RenewalLoginResponseModel output = null;

            try
            {
                var user = _authorizationService.GetUserDBByID(userId);
                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    exception = "user or user.Id is null";
                    return output;
                }

                bool value = SetUserAuthenticationCookies(user, out exception);
                if (!value)
                {
                    exception = "SetUserAuthenticationCookies return false";
                    return output;
                }

                var accessTokenResult = _authorizationService.GetAccessToken(user.Id);
                if (accessTokenResult == null)
                {
                    exception = "accessTokenResult is null";
                    return output;
                }
                if (string.IsNullOrEmpty(accessTokenResult.access_token))
                {
                    exception = "accessTokenResult.access_token is null";
                    return output;
                }

                output = new RenewalLoginResponseModel();
                output.UserName = user.Email;
                output.UserId = user.Id;
                output.AccessToken = accessTokenResult.access_token;
                MigrateUser(user.Id);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }

            return output;
        }

        private CheckoutOutput HandleIVRAddItemToCart(RenewalAddItemToChartUserModel model, IVRServicesLog log, string userId, DateTime dtBeforeCalling)
        {
            CheckoutOutput output = new CheckoutOutput();

            try
            {
                var benefitList = model.SelectedProductBenfitId.Select(a => (long)a).ToList();
                Tameenk.Core.Domain.Dtos.AddItemToCartModel addItemToCartModel = new Tameenk.Core.Domain.Dtos.AddItemToCartModel()
                {
                    IsIVRRequest = true,
                    QuotaionRequestExternalId = model.ExternalId,
                    ProductId = model.ProductId.ToString(),
                    ReferenceId = model.ReferenceId,
                    SelectedProductBenfitId = benefitList,
                    Channel = "IVR",
                    lang = "en"
                };
                CheckoutRequestLog addItemToCartLog = new CheckoutRequestLog()
                {
                    UserId = userId,
                    Channel = addItemToCartModel.Channel
                };

                output = AddItemToCart(addItemToCartModel, addItemToCartLog, addItemToCartModel.lang);
                if (output.ErrorCode != Services.Checkout.Components.Output.CheckoutOutput.ErrorCodes.Success)
                {
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while renewal process";
                    return output;
                }

                output.ErrorCode = CheckoutOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = CheckoutOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = CheckoutResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"HandleIVRAddItemToCart Exception, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        private bool ValidateIVRRequest(string insuredId, string driverNin, out string exception)
        {
            exception = string.Empty;
            try
            {
                var userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesInfo(driverNin, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    exception = $"Failed to get UserSuccessPoliciesDetails to check count of insured policies due to {exception}";
                    return false;
                }

                int pendingPolicies = 0;
                int userSuccessPolicies = 0;
                if (userSuccessPoliciesDetails != null)
                {
                    userSuccessPolicies = (insuredId.StartsWith("7"))
                                            ? userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId == 4).ToList().Count
                                            : userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId == 4).ToList().Count;

                    pendingPolicies = (insuredId.StartsWith("7"))
                                            ? userSuccessPoliciesDetails.Where(a => a.IsCompany == true && (a.PolicyStatusId != 4 && a.PolicyStatusId != 10)).ToList().Count
                                            : userSuccessPoliciesDetails.Where(a => a.IsCompany == false && (a.PolicyStatusId != 4 && a.PolicyStatusId != 10)).ToList().Count;
                }

                if (userSuccessPolicies >= UserInsuranceNumberLimitPerYear)//5 per year
                {
                    if (insuredId.StartsWith("7"))
                        exception = $"Company Driver {driverNin} exceeds insurance number limit per year: {UserInsuranceNumberLimitPerYear}";
                    else
                        exception = $"{driverNin} exceeds insurance number limit per year: {UserInsuranceNumberLimitPerYear}";

                    return false;
                }
                else if ((userSuccessPolicies + pendingPolicies) >= UserInsuranceNumberLimitPerYear)
                {
                    if (insuredId.StartsWith("7"))
                        exception = $"Company Driver {driverNin} have {pendingPolicies} pending policies which will exceeds insurance number limit per year: {UserInsuranceNumberLimitPerYear} if issued";
                    else
                        exception = $"{driverNin} have {pendingPolicies} pending policies which will exceeds insurance number limit per year: {UserInsuranceNumberLimitPerYear} if issued";

                    return false;
                }

                DateTime startDate = DateTime.Now.AddDays(-1);
                DateTime endDate = DateTime.Now;
                exception = string.Empty;
                var prevEdaatRequests = GetEdaatRequestsByNationalID(insuredId, startDate, endDate, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    exception = "GetEdaatRequestsByNationalID return " + (!string.IsNullOrEmpty(exception) ? exception : "null");
                    return false;
                }
                if (prevEdaatRequests != null && (prevEdaatRequests.Count() + userSuccessPolicies) >= UserInsuranceNumberLimitPerYear)
                {
                    exception = "user reached the maximum number of edaat requests as he have " + userSuccessPolicies + " success policies and he requested " + prevEdaatRequests.Count() + " requests from edaat";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private CheckoutDetail HandleIVRCheckoutANdInvoice(RenewalAddItemToChartUserModel lowestProduct, ShoppingCartItemDB shoppingCartItem, QuotationResponseDBModel quoteResponse, out string exception)
        {
            exception = string.Empty;
            try
            {
                var oldCheckoutdetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(lowestProduct.OldPolicyReferenceId);
                if (oldCheckoutdetails == null)
                {
                    exception = "oldCheckoutdetails is null";
                    return null;
                }

                var checkoutDetails = _orderService.GetFromCheckoutDeatilsbyReferenceId(lowestProduct.ReferenceId);
                if (checkoutDetails == null)
                {
                    checkoutDetails = new CheckoutDetail
                    {
                        ReferenceId = lowestProduct.ReferenceId,
                        BankCodeId = oldCheckoutdetails.BankId,
                        Email = oldCheckoutdetails.Email,
                        IBAN = oldCheckoutdetails.IBAN.Replace(" ", "").Trim().ToLower(),
                        Phone = Utilities.ValidatePhoneNumber(oldCheckoutdetails.Phone),
                        PaymentMethodId = oldCheckoutdetails.PaymentMethodId,
                        UserId = oldCheckoutdetails.UserId,
                        MainDriverId = quoteResponse.DriverId,
                        PolicyStatusId = (int)EPolicyStatus.PendingPayment,
                        VehicleId = quoteResponse.VehicleId,
                        CreatedDateTime = DateTime.Now,
                        SelectedInsuranceTypeCode = (short)lowestProduct.SelectedInsuranceTypeCode/*(short)model.TypeOfInsurance*/,
                        SelectedLanguage = oldCheckoutdetails.SelectedLanguage,
                        InsuranceCompanyId = lowestProduct.InsuranceCompanyId,
                        InsuranceCompanyName = lowestProduct.InsuranceCompanyName,
                        Channel = "IVR",
                        IsEmailVerified = oldCheckoutdetails.IsEmailVerified,
                        SelectedProductId = lowestProduct.ProductId,
                        MerchantTransactionId = Guid.NewGuid(),
                        InsuredId = quoteResponse.InsuredTableRowId,
                        ExternalId = quoteResponse.ExternalId
                    };
                    checkoutDetails.OrderItems = _orderService.CreateOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId);
                    _orderService.CreateCheckoutDetails(checkoutDetails);
                }
                else
                {
                    if (_orderService.DeleteOrderItemByRefrenceId(lowestProduct.ReferenceId, out exception))
                        _orderService.SaveOrderItems(new List<ShoppingCartItemDB>() { shoppingCartItem }, checkoutDetails.ReferenceId, out exception);

                    checkoutDetails.UserId = checkoutDetails.UserId;
                    checkoutDetails.SelectedLanguage = checkoutDetails.SelectedLanguage;
                    checkoutDetails.Email = checkoutDetails.Email;
                    checkoutDetails.IBAN = checkoutDetails.IBAN.Replace(" ", "").Trim().ToLower();
                    checkoutDetails.Phone = Utilities.ValidatePhoneNumber(checkoutDetails.Phone);
                    checkoutDetails.BankCodeId = checkoutDetails.BankId;
                    checkoutDetails.PaymentMethodId = checkoutDetails.PaymentMethodId;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    checkoutDetails.Channel = "IVR";
                    checkoutDetails.SelectedProductId = shoppingCartItem.ProductId;
                    checkoutDetails.MerchantTransactionId = Guid.NewGuid();
                    checkoutDetails.InsuredId = quoteResponse.InsuredTableRowId;
                    checkoutDetails.ExternalId = quoteResponse.ExternalId;
                    _orderService.UpdateCheckout(checkoutDetails);
                }

                var invoice = _orderService.GetInvoiceByRefrenceId(lowestProduct.ReferenceId);
                if (invoice != null)
                {
                    exception = string.Empty;
                    if (!_orderService.DeleteInvoiceByRefrenceId(lowestProduct.ReferenceId, lowestProduct.OldPolicyUserId, out exception))
                    {
                        exception = "Failed To Delete previous Invoice with InvoiceNo " + invoice.InvoiceNo + " and InvoiceId " + invoice.Id + " due to " + exception;
                        return null;
                    }
                    invoice = _orderService.CreateInvoice(lowestProduct.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID, invoice.InvoiceNo);
                }
                else
                {
                    invoice = _orderService.CreateInvoice(lowestProduct.ReferenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.CompanyID);
                }

                return checkoutDetails;
            }
            catch (Exception ex)
            {
                exception = "HandleIVRCheckout Exception, " + ex.ToString();
                return null;
            }
        }

        private bool SetUserAuthenticationCookies(AspNetUser userObject, out string exception)
        {
            exception = string.Empty;
            try
            {
                string userTicketData = string.Empty;

                #region Build User Ticket Data String
                userTicketData = "UserID=" + userObject.Id + ";"
                + "Email=" + userObject.Email + ";"
                + "CreatedDate=" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + ";"
                + "Key=" + Guid.NewGuid().ToString();// + ";"

                #endregion
                #region Set main first ticket (For Non-SSL Mode) object
                HttpCookie cookieMain = new HttpCookie("_authCookie");
                cookieMain.HttpOnly = true;
                cookieMain.Expires = DateTime.Now.AddDays(1);
                //Create a new FormsAuthenticationTicket that includes Custom User Data
                FormsAuthenticationTicket firstTicketUserData = new FormsAuthenticationTicket(1, userObject.Id, DateTime.Now
                    , cookieMain.Expires, false, userTicketData);
                //add cookie with the new ticket value
                cookieMain.Value = FormsAuthentication.Encrypt(firstTicketUserData);
                cookieMain.Secure = true;
                HttpContext.Current.Response.Cookies.Add(cookieMain);
                return true;
                #endregion
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private void MigrateUser(string userId)
        {
            var anonymousId = HttpContext.Current.Request.AnonymousID;
            // var anonymousId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.Anonymous)?.Value;
            if (string.IsNullOrWhiteSpace(anonymousId)) return;

            if (string.IsNullOrWhiteSpace(userId)) return;

            _shoppingCartService.EmptyShoppingCart(userId, string.Empty);

            _shoppingCartService.MigrateShoppingCart(anonymousId, userId);
        }

        #endregion

        #endregion

        //private YakeenMobileVerificationOutput VerifyMobileFromYakeen(string userId, string nationalId, string mobile, string lang, out string exception)
        //{
        //    YakeenMobileVerificationOutput yakeenMobileVerification = null;
        //    exception = string.Empty;

        //    try
        //    {
        //        var mobileVerification = _checkoutMobileVerificationRepository.TableNoTracking.Where(a => a.Phone == mobile).FirstOrDefault();
        //        if (mobileVerification != null)
        //        {
        //            yakeenMobileVerification = new YakeenMobileVerificationOutput()
        //            {
        //                ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.Success,
        //                ErrorDescription = WebResources.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(lang)),
        //                mobileVerificationModel = new MobileVerificationModel()
        //                {
        //                    mobile = mobileVerification.Phone,
        //                    id = mobileVerification.NationalId,
        //                    isOwner = mobileVerification.IsYakeenMobileVerified,
        //                    referenceNumber = mobileVerification.ReferenceNumber
        //                }
        //            };

        //            if (!mobileVerification.IsYakeenMobileVerified)
        //            {
        //                yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner;
        //                yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(lang));
        //                exception = "national id:" + nationalId + " is not the owner of mobile phone:" + mobile;
        //            }

        //            return yakeenMobileVerification;
        //        }

        //        var yakeenMobileVerificationDto = new YakeenMobileVerificationDto()
        //        {
        //            NationalId = nationalId,
        //            Phone = mobile
        //        };
        //        yakeenMobileVerification = _iYakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, lang);
        //        mobileVerification = new CheckoutMobileVerification()
        //        {
        //            UserId = userId,
        //            NationalId = nationalId,
        //            Phone = mobile,
        //            IsYakeenMobileVerified = yakeenMobileVerification.mobileVerificationModel.isOwner,
        //            ReferenceNumber = yakeenMobileVerification.mobileVerificationModel.referenceNumber,
        //            CreatedDate = DateTime.Now
        //        };
        //        _checkoutMobileVerificationRepository.Insert(mobileVerification);

        //        if (yakeenMobileVerification.ErrorCode == YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner)
        //        {
        //            yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner;
        //            yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(lang));
        //            exception = yakeenMobileVerification.ErrorDescription;
        //            return yakeenMobileVerification;
        //        }
        //        if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
        //        {
        //            yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceError;
        //            yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang));
        //            exception = yakeenMobileVerification.ErrorDescription;
        //            return yakeenMobileVerification;
        //        }

        //        return yakeenMobileVerification;
        //    }
        //    catch (Exception ex)
        //    {
        //        yakeenMobileVerification = new YakeenMobileVerificationOutput()
        //        {
        //            ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceException,
        //            ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang)),
        //            mobileVerificationModel = new MobileVerificationModel()
        //        };
        //        exception = ex.ToString();
        //        return yakeenMobileVerification;
        //    }
        //}

        public int UpdateCheckoutPhoneNumberDBToNull(string phone)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            dbContext.DatabaseInstance.CommandTimeout = 60;
            var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "UpdateCheckoutPhoneNumberDB";
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter userIdParam = new SqlParameter() { ParameterName = "phone", Value = phone };
            command.Parameters.Add(userIdParam);
            dbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
            dbContext.DatabaseInstance.Connection.Close();
            return result;
        }
    }
}