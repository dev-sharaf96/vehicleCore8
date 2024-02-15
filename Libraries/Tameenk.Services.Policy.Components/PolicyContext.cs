using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Quotations;
using Tameenk.Resources.Vehicles;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Policies.Renewal;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Checkouts;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.Logging;
using Tameenk.Services.Notifications;
using Tameenk.Services.Orders;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using AddBenefitOutput = Tameenk.Services.Inquiry.Components.AddBenefitOutput;
using AddDriverOutput = Tameenk.Services.Checkout.Components.AddDriverOutput;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyContext : IPolicyContext
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepository;
        private readonly IRepository<PolicyFile> _policyFileRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueue;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IPolicyEmailService _policyEmailService;
        private readonly ILogger _logger;
        private readonly IQuotationService _quotationService;
        private readonly IAddressService _addressService;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IRepository<SadadRequest> _sadadRequest;
        private readonly INotificationServiceProvider _notification;
        private readonly TameenkConfig _tameenkConfig;
        private readonly ICheckoutContext _checkoutContext;
        private readonly IPromotionService _promotionService;
        private readonly IExcelService _excelService;
        private readonly IRepository<MorniSMS> _morniSMSRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<MorniRequest> _morniRequestRepository;
        private readonly IRepository<BankNins> _bankNinRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<AutoleasingDepreciationSettingHistory> _autoleasingDepreciationSettingHistoryRepository;
        private readonly ICustomCardQueueService _customCardQueueService;
        private readonly IRepository<RenewalPolicies> _renewalPolicies;
        private readonly IRepository<Driver> _driver;
        private readonly IPolicyNotificationContext _policyNotificationContext;
        private readonly IRepository<SMSSkippedNumbers> _smsSkippedNumbers;
        public RemoteServerInfo remoteServerInfo { get; set; }
        private readonly IYakeenClient _yakeenClient;        private readonly IPolicyFileContext _policyFileContext;
        private readonly IRepository<VehicleDiscounts> _vehicleDiscountsRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IRepository<RenewalDiscount> _renewalDiscount;
        private readonly IRepository<CustomCardInfo> _customCardInfo;
        private readonly ICheckoutsService _checkoutsService;
        private readonly int UserInsuranceNumberLimitPerYear = 5;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAutoleasingInquiryContext _autoleasingInquiryContext;
        private readonly IPolicyModificationContext _policyModificationContext;
        private readonly IRepository<AutoleasingPortalLinkProcessingQueue> _autoleasingPortalLinkProcessingQueueQueueRepository;
        private readonly IRepository<MissingPolicyPolicyProcessingQueue> _missingPolicyPolicyProcessingQueue;


        //private readonly IRepository<CommissionsAndFees> _commissionsAndFees;
        //private readonly IRepository<Invoice_1> _Invoice_1;
        //private readonly IRepository<Invoice_2> _Invoice_2;

        List<string> AppNotificationChannels = new List<string>()
        {
            Channel.ios.ToString().ToLower(),
            Channel.android.ToString().ToLower()
        };

        public PolicyContext(IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepository,
            IInvoiceService invoiceService, IPolicyEmailService policyEmailService,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<PolicyFile> policyFileRepository,
            IRepository<PolicyProcessingQueue> policyProcessingQueue,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            ILogger logger, IQuotationService quotationService, IAddressService addressService,
            IInsuranceCompanyService insuranceCompanyService, INotificationService notificationService,
            IOrderService orderService,
            IRepository<SadadRequest> sadadRequest, INotificationServiceProvider notification,
            TameenkConfig tameenkConfig, ICheckoutContext checkoutContext,
            IPromotionService promotionService, IExcelService excelService, IRepository<MorniSMS> morniSMSRepository,
            IVehicleService vehicleService, IRepository<MorniRequest> morniRequestRepository,
            IRepository<BankNins> bankNinRepository, IRepository<Bank> bankRepository,
            IRepository<AutoleasingDepreciationSettingHistory> autoleasingDepreciationSettingHistoryRepository
            , IRepository<CustomCardQueue> customCardQueueRepository
               , ICustomCardQueueService customCardQueueService, IRepository<RenewalPolicies> renewalPolicies
            , IRepository<Driver> driver, IPolicyNotificationContext policyNotificationContext,
            IRepository<SMSSkippedNumbers> smsSkippedNumbers
            ,IYakeenClient yakeenClient, IPolicyFileContext policyFileContext, IRepository<VehicleDiscounts> vehicleDiscountsRepository
            ,IPolicyProcessingService policyProcessingService
            , IRepository<RenewalDiscount> renewalDiscount
            , IRepository<CustomCardInfo> customCardInfo
            ,ICheckoutsService checkoutsService
            ,IAuthorizationService authorizationService
            , IAutoleasingInquiryContext autoleasingInquiryContext
            ,IPolicyModificationContext policyModificationContext
            , IRepository<AutoleasingPortalLinkProcessingQueue> autoleasingPortalLinkProcessingQueueQueueRepository
            , IRepository<MissingPolicyPolicyProcessingQueue> missingPolicyPolicyProcessingQueue
            //, IRepository<CommissionsAndFees> commissionsAndFeesRepository, IRepository<Invoice_1> _Invoice_1Repository, IRepository<Invoice_2> _Invoice_2Repository
            )
        {
            _invoiceService = invoiceService;
            _policyRepository = policyRepository;
            _policyEmailService = policyEmailService;
            _checkoutDetailRepository = checkoutDetailRepository;
            _orderItemRepository = orderItemRepository;
            _policyFileRepository = policyFileRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _invoiceRepository = invoiceRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _logger = logger;
            _policyProcessingQueue = policyProcessingQueue;
            _quotationService = quotationService;
            _addressService = addressService;
            _insuranceCompanyService = insuranceCompanyService;
            _notificationService = notificationService;
            _orderService = orderService;
            _sadadRequest = sadadRequest;
            _notification = notification;
            _tameenkConfig = tameenkConfig;
            _checkoutContext = checkoutContext;
            _promotionService = promotionService;
            _excelService = excelService;
            _morniSMSRepository = morniSMSRepository;
            _vehicleService = vehicleService;
            _morniRequestRepository = morniRequestRepository;
            _bankNinRepository = bankNinRepository;
            _bankRepository = bankRepository;
            _autoleasingDepreciationSettingHistoryRepository = autoleasingDepreciationSettingHistoryRepository;
            _customCardQueueService = customCardQueueService;
            _renewalPolicies = renewalPolicies;
            _driver = driver;
            _policyNotificationContext = policyNotificationContext;
            _smsSkippedNumbers = smsSkippedNumbers;
            _yakeenClient = yakeenClient;
            _policyFileContext = policyFileContext;
            _vehicleDiscountsRepository = vehicleDiscountsRepository;
            _policyProcessingService = policyProcessingService;
            _renewalDiscount = renewalDiscount;            _customCardInfo = customCardInfo;
            _checkoutsService = checkoutsService;
            _authorizationService = authorizationService;            _autoleasingInquiryContext = autoleasingInquiryContext;
            _policyModificationContext = policyModificationContext;            _autoleasingPortalLinkProcessingQueueQueueRepository = autoleasingPortalLinkProcessingQueueQueueRepository;            _missingPolicyPolicyProcessingQueue = missingPolicyPolicyProcessingQueue;            //_commissionsAndFees = commissionsAndFeesRepository;
            //_Invoice_1 = _Invoice_1Repository;
            //_Invoice_2 = _Invoice_2Repository;        }
        public PolicyOutput SubmitPolicy(string referenceId, LanguageTwoLetterIsoCode userLanguage, string serverIP, string channel)
        {
            string exception = string.Empty;
            if (string.IsNullOrEmpty(serverIP))
                serverIP = Utilities.GetAppSetting("FrontEndServerIP");

            PolicyOutput output = new PolicyOutput();
            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
            bool showErrors = false;
            bool.TryParse(Utilities.GetAppSetting("ShowInsuranceCompanyErrors"), out showErrors);
            try
            {
                var checkoutDetails = _checkoutDetailRepository.Table.Include(c => c.Driver)
                            .Include(c => c.Driver.Addresses)
                            .Include(c => c.OrderItems.Select(oi => oi.Product.PriceDetails))
                            .Include(c => c.OrderItems.Select(oi => oi.OrderItemBenefits.Select(oib => oib.Benefit)))
                            .Include(c => c.BankCode)
                            .Include(c => c.Vehicle)
                            .FirstOrDefault(c => c.ReferenceId == referenceId
                                                && c.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                                                && c.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                                                && c.PolicyStatusId != (int)EPolicyStatus.Available);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = "Checkout Details Is Null";
                    return output;
                }
                channel = checkoutDetails.Channel;
                var policy = _policyRepository.TableNoTracking.FirstOrDefault(a => a.CheckOutDetailsId == referenceId);
                if (policy != null)
                {
                    if (policy.PolicyFileId != null)
                    {
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Available;
                        output.ErrorCode = 1;
                        output.ErrorDescription = "Success";
                    }
                    else
                    {
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PolicyFileGeneraionFailure;
                        output.ErrorDescription = "Failed to generate or download the pdf";
                        output.ErrorCode = 20;
                    }
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    _checkoutDetailRepository.Update(checkoutDetails);
                    return output;
                }
                Guid selectedUserId = Guid.Empty;
                Guid.TryParse(checkoutDetails.UserId, out selectedUserId);
                predefinedLogInfo.UserID = selectedUserId;
                if (checkoutDetails.Driver != null)
                    predefinedLogInfo.DriverNin = checkoutDetails.Driver.NIN;
                if (checkoutDetails.Vehicle != null)
                {
                    if (checkoutDetails.Vehicle.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.CustomCardNumber;
                    else
                        predefinedLogInfo.VehicleId = checkoutDetails.Vehicle.SequenceNumber;
                }

                var quoteResponse = _quotationResponseRepository.TableNoTracking
                   .Include(e => e.QuotationRequest)
                    .Include(e => e.QuotationRequest.Insured)
                    .Include(e => e.QuotationRequest.Insured.City)
                   .Include(e => e.QuotationRequest.Vehicle)
                   .Include(e => e.QuotationRequest.Driver)
                   .Include(e => e.InsuranceCompany)
                   .FirstOrDefault(q => q.ReferenceId == referenceId);

                if (quoteResponse == null)
                {
                    output.ErrorCode = 3; ;
                    output.ErrorDescription = "Quotation Response Is Null";
                    return output;
                }
                //var userManager = _authorizationService.GetUser(checkoutDetails.UserId);
                //if (!userManager.Result.IsCorporateUser&&string.IsNullOrEmpty(checkoutDetails.ODReference))
                {
                    string driverNin = quoteResponse.QuotationRequest.Insured.NationalId;
                    if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                    {
                        driverNin = quoteResponse.QuotationRequest.Driver.NIN;
                    }
                    //User Insurance Number Limit Per Year = 5 Polices per year
                    exception = string.Empty;
                    List<InsuredPolicyDetails> userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesInfo(driverNin, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorCode = 500;
                        output.ErrorDescription = "Failed to get UserSuccessPoliciesDetails to check count of insured policies due to " + exception;
                        return output;
                    }
                    int userSuccessPolicies = 0;
                    int pendingPolicies = 0;
                    List<InsuredPolicyDetails> userPendingPoliciesDetails = null;
                    if (userSuccessPoliciesDetails != null)
                    {
                        //if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                        //{
                        //    userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && (a.PolicyStatusId != 5 && a.PolicyStatusId != 10)).ToList().Count;
                        //    pendingPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId == 10).ToList().Count;
                        //}
                        //else
                        //{
                        //    userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && (a.PolicyStatusId != 5 && a.PolicyStatusId != 10)).ToList().Count;
                        //    pendingPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId == 10).ToList().Count;
                        //}

                        if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                        {
                            userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId == 4).ToList().Count;
                            //userPendingPoliciesDetails = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && (a.PolicyStatusId != 4 && a.PolicyStatusId != 10)).OrderBy(a => a.CreatedDateTime).ToList();
                            userPendingPoliciesDetails = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId != 4).OrderBy(a => a.CreatedDateTime).ToList();
                            pendingPolicies = userPendingPoliciesDetails.Count;
                        }
                        else
                        {
                            userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId == 4).ToList().Count;
                            //userPendingPoliciesDetails = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && (a.PolicyStatusId != 4 && a.PolicyStatusId != 10)).OrderBy(a => a.CreatedDateTime).ToList();
                            userPendingPoliciesDetails = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId != 4).OrderBy(a => a.CreatedDateTime).ToList();
                            pendingPolicies = userPendingPoliciesDetails.Count;
                        }
                    }

                    if (userSuccessPolicies >= UserInsuranceNumberLimitPerYear)//5 per year
                    {
                        output.DriverNin = driverNin;
                        output.VehicleId = (quoteResponse.QuotationRequest.Vehicle.VehicleIdTypeId == 1) ? quoteResponse.QuotationRequest.Vehicle.SequenceNumber : quoteResponse.QuotationRequest.Vehicle.CustomCardNumber;
                     
                        if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                        {
                            output.ErrorCode = 57;//CompanyDriverExceedsInsuranceNumberLimitPerYear;
                            output.ErrorDescription = "Company Driver " + driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                        }
                        else
                        {
                            output.ErrorCode = 58;//UserExceedsInsuranceNumberLimitPerYear;
                            output.ErrorDescription =driverNin + " Exceeds Insurance Number Limit Per Year :" + UserInsuranceNumberLimitPerYear;
                        }
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PolicyYearlyMaximumPurchase;
                        checkoutDetails.ModifiedDate = DateTime.Now;
                        _checkoutDetailRepository.Update(checkoutDetails);
                        return output;
                    }
                    else if ((userSuccessPolicies + pendingPolicies) > UserInsuranceNumberLimitPerYear)
                    {
                        output.DriverNin = driverNin;
                        output.VehicleId = (quoteResponse.QuotationRequest.Vehicle.VehicleIdTypeId == 1) ? quoteResponse.QuotationRequest.Vehicle.SequenceNumber : quoteResponse.QuotationRequest.Vehicle.CustomCardNumber;

                        var lastIndexToProcess = (UserInsuranceNumberLimitPerYear - userSuccessPolicies) - 1; // (-1) as index in list start from 0
                        var currentPendingIndex = userPendingPoliciesDetails?.FindIndex(a => a.ReferenceId == referenceId);
                        if ((currentPendingIndex.HasValue && currentPendingIndex.Value >= 0) && currentPendingIndex > lastIndexToProcess)
                        {
                            if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                            {
                                output.ErrorCode = 57;//CompanyDriverExceedsInsuranceNumberLimitPerYear;
                                output.ErrorDescription = "Company Driver " + driverNin + " Exceeds Insurance Number Limit Per Year: " + UserInsuranceNumberLimitPerYear + ", as success = " + userSuccessPolicies 
                                                        + " and pending = " + pendingPolicies + ", policies ref = [ " + String.Join(",", userSuccessPoliciesDetails.Select(a => a.ReferenceId).ToList()) + " ]";
                            }
                            else
                            {
                                output.ErrorCode = 58;//UserExceedsInsuranceNumberLimitPerYear;
                                output.ErrorDescription = driverNin + " Exceeds Insurance Number Limit Per Year: " + UserInsuranceNumberLimitPerYear + ", as success = " + userSuccessPolicies 
                                                        + " and pending = " + pendingPolicies + ", policies ref = [ " + String.Join(",", userSuccessPoliciesDetails.Select(a => a.ReferenceId).ToList()) + " ]";
                            }
                        }

                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PolicyYearlyMaximumPurchase;
                        checkoutDetails.ModifiedDate = DateTime.Now;
                        _checkoutDetailRepository.Update(checkoutDetails);
                        return output;
                    }
                }
                var companyId = quoteResponse.InsuranceCompanyId;
                var insuranceCompany = quoteResponse.InsuranceCompany;
                if (insuranceCompany == null)
                {
                    output.ErrorCode = 4;
                    output.ErrorDescription = "CompanyId  Is Null";
                    return output;
                }
                if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentSuccess && checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Pending && checkoutDetails.PolicyStatusId != (int)EPolicyStatus.ComprehensiveImagesFailure)
                {
                    output.ErrorCode = 5;
                    output.ErrorDescription = "Invalid Policy Status id as id is " + checkoutDetails.PolicyStatusId;
                    return output;
                }
                Invoice invoice = null;
                invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                if (invoice == null)
                {
                    //output.ErrorCode = 6;
                    //output.ErrorDescription = "Invoice Is Null";
                    //return output;
                    if (checkoutDetails.PaymentMethodId == 2)
                    {
                        var sadad = _sadadRequest.TableNoTracking.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
                        if (sadad != null)
                        {
                            int invoiceNumber = 0;
                            int.TryParse(sadad.CustomerAccountNumber.Substring(2, sadad.CustomerAccountNumber.Length - 2), out invoiceNumber);
                            _orderService.CreateInvoice(referenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.InsuranceCompanyId, invoiceNumber);
                        }
                        else
                        {
                            _orderService.CreateInvoice(referenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.InsuranceCompanyId);
                        }
                    }
                    else
                    {
                        _orderService.CreateInvoice(referenceId, quoteResponse.InsuranceTypeCode.Value, quoteResponse.InsuranceCompanyId);
                    }
                    invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                }
                if (invoice == null)
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "Invoice Is Null";
                    return output;
                }
                var orderItem = checkoutDetails.OrderItems.FirstOrDefault();
                if (orderItem == null)
                {
                    output.ErrorCode = 7;
                    output.ErrorDescription = "Order Item Is Null";
                    return output;
                }

                List<BenefitRequest> selectedBenefits = new List<BenefitRequest>();
                if (insuranceCompany.Key == "Wataniya")
                {
                    selectedBenefits = orderItem.OrderItemBenefits.Select(b =>
                        new BenefitRequest()
                        {
                            BenefitId = b.BenefitExternalId,
                            BenefitPrice = b.Price
                        }).ToList();
                }

                else
                {
                    selectedBenefits = orderItem.OrderItemBenefits.Select(b =>
                        new BenefitRequest()
                        {
                            BenefitId = b.BenefitExternalId,
                        }).ToList();
                }

                int quotationRequestId = 0;
                if (quoteResponse != null && quoteResponse.QuotationRequest != null)
                    quotationRequestId = quoteResponse.QuotationRequest.ID;
                var policyRequestMessage = new PolicyRequest();
                // {
                long insuredId = 0;
                int insuredBuildingNo = 0;
                int insuredZipCode = 0;
                int insuredAdditionalNumber = 0;
                int insuredUnitNo = 0;
                //var address = checkoutDetails.Driver?.Addresses?.FirstOrDefault();
                Address address = null;
                if (quoteResponse.QuotationRequest.Insured != null && quoteResponse.QuotationRequest.Insured.AddressId.HasValue)
                {
                    // address = checkoutDetails.Driver.Addresses.Where(a=>a.Id== quoteResponse.QuotationRequest.Insured.AddressId).FirstOrDefault();
                    address = _addressService.GetAddressDetailsNoTracking(quoteResponse.QuotationRequest.Insured.AddressId.Value);
                    if (address == null)
                    {
                        address = _addressService.GetAddressesByNin(checkoutDetails.Driver?.NIN);
                    }
                }
                else
                {
                    address = _addressService.GetAddressesByNin(checkoutDetails.Driver?.NIN);
                }
                long.TryParse(checkoutDetails.Driver?.NIN, out insuredId);
                int.TryParse(address?.BuildingNumber, out insuredBuildingNo);
                int.TryParse(address?.PostCode, out insuredZipCode);
                int.TryParse(address?.AdditionalNumber, out insuredAdditionalNumber);
                int.TryParse(address?.UnitNumber, out insuredUnitNo);
                string insuredCity = "غير معروف";
                string insuredDistrict = "غير معروف";
                string insuredStreet = "غير معروف";
                //var address = checkoutDetails.Driver.Addresses.FirstOrDefault();
                if (address != null && !string.IsNullOrEmpty(address.City))
                {
                    insuredCity = address.City;
                }
                if (address != null && !string.IsNullOrEmpty(address.District))
                {
                    insuredDistrict = address.District;
                }
                if (address != null && !string.IsNullOrEmpty(address.Street))
                {
                    insuredStreet = address.Street;
                }
                policyRequestMessage.ReferenceId = referenceId;
                policyRequestMessage.QuotationNo = orderItem.Product.QuotaionNo;
                policyRequestMessage.ProductId = orderItem.Product.ExternalProductId;
                policyRequestMessage.Benefits = selectedBenefits;
                if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                    long.TryParse(quoteResponse.QuotationRequest?.Insured?.NationalId, out insuredId);
                else if (quoteResponse.QuotationRequest?.Insured?.NationalId.StartsWith("7") == true)
                    long.TryParse(quoteResponse.QuotationRequest?.Insured?.NationalId, out insuredId);
                else
                    long.TryParse(checkoutDetails.Driver?.NIN, out insuredId);

                policyRequestMessage.InsuredId = insuredId;// long.Parse(checkoutDetails.Driver.NIN);
                policyRequestMessage.InsuredMobileNumber = Utilities.Remove966FromMobileNumber(checkoutDetails.Phone);
                policyRequestMessage.InsuredEmail = checkoutDetails.Email;
                policyRequestMessage.InsuredBuildingNo = insuredBuildingNo;// int.Parse(checkoutDetails.Driver.Addresses.First().BuildingNumber);
                policyRequestMessage.InsuredZipCode = insuredZipCode;// int.Parse(checkoutDetails.Driver.Addresses.First().PostCode);
                policyRequestMessage.InsuredAdditionalNumber = insuredAdditionalNumber;// int.Parse(checkoutDetails.Driver.Addresses.First().AdditionalNumber);
                policyRequestMessage.InsuredUnitNo = insuredUnitNo;// int.Parse(checkoutDetails.Driver.Addresses.First().UnitNumber);
                policyRequestMessage.InsuredCity = insuredCity;//string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.FirstOrDefault()?.City) ? "غير معروف"
                                                               //: checkoutDetails.Driver.Addresses.First().City;
                if (insuranceCompany.Key == "SAICO" || insuranceCompany.Key == "Alalamiya")
                {
                    var city = _addressService.GetCityCenterById(address.CityId);
                    if (city != null)
                    {
                        policyRequestMessage.InsuredCityCode = city.ELM_Code;
                    }
                }
                if (insuranceCompany.Key == "Wataniya")
                {
                    policyRequestMessage.DeductibleAmount = (orderItem.Product.DeductableValue.HasValue) ? orderItem.Product.DeductableValue.Value.ToString() : "0";
                    policyRequestMessage.PolicyPremium = invoice.TotalCompanyAmount.Value.ToString();

                    predefinedLogInfo.InsuranceTypeCode = orderItem.Product.InsuranceTypeCode;
                }

                if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                {
                    var vehicleData = quoteResponse?.QuotationRequest?.Vehicle;
                    if (vehicleData != null)
                    {
                        policyRequestMessage.VehicleRegExpiryDate = string.IsNullOrEmpty(vehicleData.LicenseExpiryDate) ? string.Empty : vehicleData.LicenseExpiryDate;
                        if (insuranceCompany.Key == "MedGulf" || insuranceCompany.Key == "AICC")
                        {
                            policyRequestMessage.VehicleIdTypeCode = vehicleData.VehicleIdTypeId;
                            policyRequestMessage.VehicleId = (vehicleData.VehicleIdTypeId == 1) ? vehicleData.SequenceNumber : vehicleData.CustomCardNumber;
                            policyRequestMessage.VehiclePlateTypeCode = vehicleData.PlateTypeCode.ToString();
                            policyRequestMessage.VehicleMajorColor = vehicleData.MajorColor;
                            policyRequestMessage.VehicleMajorColorCode = vehicleData.ColorCode.ToString();
                            policyRequestMessage.VehicleBodyTypeCode = vehicleData.VehicleBodyCode.ToString();
                            policyRequestMessage.VehicleCylinders = vehicleData.Cylinders;
                            policyRequestMessage.VehicleWeight = vehicleData.VehicleWeight;
                            policyRequestMessage.VehicleLoad = vehicleData.VehicleLoad;
                            policyRequestMessage.NCDFreeYears = quoteResponse.QuotationRequest?.NajmNcdFreeYears;
                            policyRequestMessage.NCDReference = quoteResponse.QuotationRequest?.NajmNcdRefrence;
                            policyRequestMessage.VehicleChassisNumber = vehicleData.ChassisNumber;
                            policyRequestMessage.VehiclePlateNumber = vehicleData.CarPlateNumber.HasValue ? vehicleData.CarPlateNumber.Value : 0;
                            policyRequestMessage.VehiclePlateText1 = string.IsNullOrEmpty(vehicleData.CarPlateText1) ? string.Empty : vehicleData.CarPlateText1;
                            policyRequestMessage.VehiclePlateText2 = string.IsNullOrEmpty(vehicleData.CarPlateText2) ? string.Empty : vehicleData.CarPlateText2;
                            policyRequestMessage.VehiclePlateText3 = string.IsNullOrEmpty(vehicleData.CarPlateText3) ? string.Empty : vehicleData.CarPlateText3;
                            policyRequestMessage.VehicleOvernightParkingLocationCode = vehicleData.ParkingLocationId;
                            policyRequestMessage.VehicleRegPlace = string.IsNullOrEmpty(vehicleData.RegisterationPlace) ? string.Empty : vehicleData.RegisterationPlace;
                            policyRequestMessage.VehicleRegPlaceCode = "";
                            if (!string.IsNullOrEmpty(vehicleData.RegisterationPlace))
                            {
                                var info = _addressService.GetFromCityByArabicName(Utilities.RemoveWhiteSpaces(vehicleData.RegisterationPlace));
                                if (info != null)
                                    policyRequestMessage.VehicleRegPlaceCode = info.YakeenCode.ToString();
                                else
                                    policyRequestMessage.VehicleRegPlaceCode = "";
                            }
                        }
                    }
                }
                policyRequestMessage.InsuredDistrict = insuredDistrict;// string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.FirstOrDefault()?.District) ? "غير معروف"
                policyRequestMessage.InsuredStreet = insuredStreet;// string.IsNullOrEmpty(checkoutDetails.Driver.Addresses.FirstOrDefault()?.Street) ? "غير معروف"

                if (checkoutDetails.PaymentMethodId == 1 || checkoutDetails.PaymentMethodId == 3 || checkoutDetails.PaymentMethodId == 4 || checkoutDetails.PaymentMethodId == 10 || checkoutDetails.PaymentMethodId == 13 || checkoutDetails.PaymentMethodId == 16)
                {
                    policyRequestMessage.PaymentMethodCode = 1;
                }
                else if (checkoutDetails.PaymentMethodId == 5 || checkoutDetails.PaymentMethodId == 2 || checkoutDetails.PaymentMethodId == 12)
                {
                    policyRequestMessage.PaymentMethodCode = 2;

                }
                else if (checkoutDetails.PaymentMethodId == 6 || checkoutDetails.PaymentMethodId == 8 || checkoutDetails.PaymentMethodId == 9)
                {
                    policyRequestMessage.PaymentMethodCode = 6;
                }
                else if (checkoutDetails.PaymentMethodId == 7)
                {
                    policyRequestMessage.PaymentMethodCode = 7;
                }
                else if (checkoutDetails.PaymentMethodId == 14)
                {
                    policyRequestMessage.PaymentMethodCode = 1;
                }
                else
                {
                    policyRequestMessage.PaymentMethodCode = checkoutDetails.PaymentMethodId.GetValueOrDefault();
                }

                if (companyId == 18 && policyRequestMessage.PaymentMethodCode == 1)
                {
                    policyRequestMessage.PaymentMethodCode = 4;
                }

                if (companyId == 12)
                {
                    policyRequestMessage.Language = checkoutDetails.SelectedLanguage.ToString();
                }

                policyRequestMessage.PaymentAmount = invoice.TotalPrice.Value;
                policyRequestMessage.PaymentBillNumber = invoice.InvoiceNo.ToString();
                policyRequestMessage.InsuredBankCode = checkoutDetails.BankCode?.Code;
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredBankCode) && checkoutDetails.IBAN?.Length > 4)
                    policyRequestMessage.InsuredBankCode = checkoutDetails.IBAN?.Substring(4, 2);
                policyRequestMessage.PaymentUsername = "UNKNOWN";
                policyRequestMessage.InsuredIBAN = checkoutDetails.IBAN.ToUpper();
                policyRequestMessage.PolicyEffectiveDate = quoteResponse?.QuotationRequest?.RequestPolicyEffectiveDate;
                policyRequestMessage.QuotationRequestId = quotationRequestId;
                policyRequestMessage.ProductInternalId = orderItem.Product.Id;
                //};
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredDistrict?.Trim()))
                    policyRequestMessage.InsuredDistrict = "غير معروف";
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredCity?.Trim()))
                    policyRequestMessage.InsuredCity = "غير معروف";
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredStreet?.Trim()))
                    policyRequestMessage.InsuredStreet = "غير معروف";
                PolicyResponse policyResponseMessage = null;
                predefinedLogInfo.CompanyID = companyId;
                predefinedLogInfo.ExternalId = quoteResponse?.QuotationRequest?.ExternalId;
                predefinedLogInfo.VehicleAgencyRepair = quoteResponse?.VehicleAgencyRepair;
                predefinedLogInfo.City = quoteResponse?.QuotationRequest.Insured?.City?.ArabicDescription;
                predefinedLogInfo.ChassisNumber = quoteResponse?.QuotationRequest?.Vehicle?.ChassisNumber;
                var providerFullTypeName = string.Empty;
                IInsuranceProvider provider = null;
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
                if (instance != null)
                {
                    provider = instance as IInsuranceProvider;
                }

                if (instance == null)
                {
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);
                    if (providerType == null)
                    {
                        output.ErrorCode = 8;
                        output.ErrorDescription = "provider Type is Null";
                        return output;
                    }

                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                    Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

                }
                if (provider == null)
                {
                    output.ErrorCode = 9;
                    output.ErrorDescription = "provider is Null";
                    return output;
                }
                if (string.IsNullOrEmpty(policyRequestMessage.InsuredStreet) || string.IsNullOrWhiteSpace(policyRequestMessage.InsuredStreet.Trim()) || policyRequestMessage.InsuredStreet == "0")
                {
                    policyRequestMessage.InsuredStreet = "غير معروف";//WebResources.Unknown;
                }

                /* code to call SAICO to send them the comprehansive images */
                if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Pending && insuranceCompany.Key == "SAICO" && checkoutDetails.SelectedInsuranceTypeCode == 2 && checkoutDetails.Channel != Channel.autoleasing.ToString() && checkoutDetails.BankId == null)
                {

                    ComprehensiveImagesRequest comprehensiveImageRequest = new ComprehensiveImagesRequest();
                    comprehensiveImageRequest.ReferenceId = referenceId;
                    comprehensiveImageRequest.Nin = checkoutDetails.Driver.NIN;
                    List<ComprehensiveImage> attachmentsModel = new List<ComprehensiveImage>();
                    var checkoutDetail = _checkoutDetailRepository.TableNoTracking
                                    .Include(e => e.ImageBack)
                                    .Include(e => e.ImageBody)
                                    .Include(e => e.ImageFront)
                                    .Include(e => e.ImageLeft)
                                    .Include(e => e.ImageRight)
                                    .Where(x => x.ReferenceId == referenceId).FirstOrDefault();

                    if (checkoutDetail == null)
                    {
                        exception = string.Empty;
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.ComprehensiveImagesFailure;
                        checkoutDetails.ModifiedDate = DateTime.Now;
                        //_checkoutDetailRepository.Update(checkoutDetails);
                        _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);
                        output.ErrorCode = 20;
                        output.ErrorDescription = "There is no checkout details with this referenceId " + referenceId;
                        return output;
                    }
                    if (checkoutDetail.ImageFront != null)
                        attachmentsModel.Add(new ComprehensiveImage { AttachmentFile = checkoutDetail.ImageFront.ImageData, AttachmentCode = (int)VehicleImageType.VehicleFront });

                    if (checkoutDetail.ImageBack != null)
                        attachmentsModel.Add(new ComprehensiveImage { AttachmentFile = checkoutDetail.ImageBack.ImageData, AttachmentCode = (int)VehicleImageType.VehicleBack });

                    if (checkoutDetail.ImageRight != null)
                        attachmentsModel.Add(new ComprehensiveImage { AttachmentFile = checkoutDetail.ImageRight.ImageData, AttachmentCode = (int)VehicleImageType.VehicleRight });

                    if (checkoutDetail.ImageLeft != null)
                        attachmentsModel.Add(new ComprehensiveImage { AttachmentFile = checkoutDetail.ImageLeft.ImageData, AttachmentCode = (int)VehicleImageType.VehicleLeft });

                    if (checkoutDetail.ImageBody != null)
                        attachmentsModel.Add(new ComprehensiveImage { AttachmentFile = checkoutDetail.ImageBody.ImageData, AttachmentCode = (int)VehicleImageType.VehicleChassis });

                    comprehensiveImageRequest.Attachments = attachmentsModel;

                    predefinedLogInfo.Channel = channel;
                    var comprehensiveImagesResponse = provider.UploadComprehansiveImages(comprehensiveImageRequest, predefinedLogInfo);
                    if (comprehensiveImagesResponse.ErrorCode != ComprehensiveImagesOutput.ErrorCodes.Success)
                    {
                        exception = string.Empty;
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.ComprehensiveImagesFailure;
                        checkoutDetails.ModifiedDate = DateTime.Now;
                        //_checkoutDetailRepository.Update(checkoutDetails);
                        _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);
                        output.ErrorCode = 21;
                        output.ErrorDescription = "failed to upload comprehansive images for the company due to this response " + comprehensiveImagesResponse.ErrorDescription;
                        return output;
                    }
                }
                /* end of the code */
                SendPolicyViaMailDto emailInfo = new SendPolicyViaMailDto();
                bool isAutoleasingPolicy = false;
                if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                {
                    isAutoleasingPolicy = true;
                    policyResponseMessage = provider.GetAutoleasingPolicy(policyRequestMessage, predefinedLogInfo);
                    emailInfo.Module = Module.Autoleasing.ToString();
                }
                else
                {
                    isAutoleasingPolicy = false;
                    policyResponseMessage = provider.GetPolicy(policyRequestMessage, predefinedLogInfo);
                    emailInfo.Module = Module.Vehicle.ToString();
                }
                emailInfo.PolicyResponseMessage = policyResponseMessage;
                emailInfo.ReceiverEmailAddress = checkoutDetails.Email;
                emailInfo.ReferenceId = referenceId;
                emailInfo.UserLanguage = userLanguage;
                emailInfo.Method = "PolicyFile";
                emailInfo.Channel = checkoutDetails.Channel;
                emailInfo.InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode;

                if (policyResponseMessage == null || (policyResponseMessage.Errors != null && policyResponseMessage.Errors.Count() > 0) || policyResponseMessage.StatusCode == 2)
                {
                    UpdatePolicyStatus(checkoutDetails, EPolicyStatus.Pending);

                    emailInfo.PolicyFileByteArray = null;
                    emailInfo.InvoiceFileByteArray = null;
                    emailInfo.IsPolicyGenerated = false;
                    emailInfo.IsShowErrors = showErrors;

                    var TrialNumbersOfGeneratingPlicy = _policyProcessingQueue.TableNoTracking
                        .Where(x => x.ReferenceId == referenceId)
                        .Select(x => x.ProcessingTries)
                        .FirstOrDefault();
                    if (TrialNumbersOfGeneratingPlicy == 0 && checkoutDetails.IsEmailVerified.HasValue && checkoutDetails.IsEmailVerified.Value)
                    {
                        var result = _policyEmailService.SendPolicyByMail(emailInfo, insuranceCompany.Key);
                    }
                    output.ErrorCode = 10;
                    return output;
                }
                if (string.IsNullOrEmpty(policyResponseMessage?.PolicyDetails?.VehicleCapacity))
                {
                    if (policyResponseMessage.PolicyDetails != null && checkoutDetails.Vehicle != null && checkoutDetails.Vehicle.VehicleLoad > 0)
                        policyResponseMessage.PolicyDetails.VehicleCapacity = checkoutDetails.Vehicle.VehicleLoad.ToString();
                }

                exception = string.Empty;
                policyResponseMessage.ReferenceId = referenceId;
                byte[] invoiceFilePdf = null;
                Invoice invoiceInfo = _invoiceService.GenerateAndSaveInvoicePdf(policyRequestMessage,
                    policyResponseMessage, serverIP, insuranceCompany.NameAR, insuranceCompany.NameEN,
                    insuranceCompany.Key, quoteResponse.QuotationRequest.Insured, insuranceCompany.VAT, quoteResponse.QuotationRequest.Vehicle, isAutoleasingPolicy, out invoiceFilePdf, out exception);
                //invoiceFilePdf == null ? null : invoiceFilePdf;
                if (string.IsNullOrEmpty(policyResponseMessage.PolicyNo))
                {
                    output.ErrorCode = 11;
                    output.ErrorDescription = "Failed To Save Policy to database because policy no is empty";
                    return output;
                }
                var policyId = SavePolicy(policyResponseMessage, companyId);
                if (!policyId.HasValue)
                {
                    output.ErrorCode = 11;
                    output.ErrorDescription = "Failed To Save Policy to database or may policy already exist";
                    return output;
                }
                invoice.PolicyId = policyId;
                _invoiceRepository.Update(invoice);
                if (!string.IsNullOrEmpty(checkoutDetails.Vehicle.SequenceNumber))
                {
                    string exp = string.Empty;
                    AddToPolicyRenewal(quoteResponse.QuotationRequest?.Insured?.NationalId, checkoutDetails.Vehicle.SequenceNumber, checkoutDetails, policyResponseMessage, invoice.TotalPrice.Value, out exp);
                }

                if (!string.IsNullOrEmpty(checkoutDetails.Vehicle.CustomCardNumber) && 
                    (
                    ((checkoutDetails.InsuranceCompanyId == 7 || checkoutDetails.InsuranceCompanyId == 23) && checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower()) 
                    || 
                    ((checkoutDetails.InsuranceCompanyId == 9 || checkoutDetails.InsuranceCompanyId == 23 || checkoutDetails.InsuranceCompanyId == 18) && checkoutDetails.Channel.ToLower() != Channel.autoleasing.ToString().ToLower()))) //walaa
                {
                    _customCardQueueService.AddCustomCardQueue(checkoutDetails, policyResponseMessage.PolicyNo);
                }

                PdfGenerationLog log = new PdfGenerationLog();
                log.Channel = channel;

                bool generateTemplateFromOurSide = true;
                // stop generating GGI TPL from company side as per https://bcare.atlassian.net/browse/VW-817  @11-4-2023
                //if (checkoutDetails.InsuranceCompanyId == 11 && checkoutDetails.SelectedInsuranceTypeCode == 1)
                //    generateTemplateFromOurSide = false;

                var policyStatus = _policyFileContext.GeneratePolicyPdfFile(policyResponseMessage, companyId, checkoutDetails.Channel, userLanguage, generateTemplateFromOurSide);

                if (policyStatus.ErrorCode !=PolicyGenerationOutput.ErrorCodes.Success)
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = (int)policyStatus.EPolicyStatus;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    emailInfo.PolicyFileByteArray = policyResponseMessage.PolicyFile;
                    emailInfo.InvoiceFileByteArray = invoiceFilePdf;
                    emailInfo.IsPolicyGenerated = false;
                    emailInfo.IsShowErrors = showErrors;
               
                    if (checkoutDetails.IsEmailVerified.HasValue && checkoutDetails.IsEmailVerified.Value)
                    {
                        _policyEmailService.SendPolicyByMail(emailInfo, insuranceCompany.Key);
                    }
                    output.ErrorCode = 13;
                    output.ErrorDescription = "Failed to generate or download the pdf file due to " + policyStatus.ErrorDescription;
                    return output;
                }
                string filePath = Utilities.SaveCompanyFile(referenceId, policyResponseMessage.PolicyFile, insuranceCompany.Key, true, isAutoleasingPolicy);
                if (string.IsNullOrEmpty(filePath))
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = 15;
                    output.ErrorDescription = "Failed to save pdf file on server";
                    return output;
                }
                if (insuranceCompany.Key == "GGI" && !IsValidPdfFile(filePath))//validate if file is corrupted
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = 15;
                    output.ErrorDescription = "it's not valid pdf file Lenght is to short it's " + policyResponseMessage.PolicyFile.Length;
                    return output;
                }
                Guid fileId = SavePolicyFile(policyResponseMessage, filePath, serverIP, out exception);
                if (fileId == Guid.Empty)
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = 16;
                    output.ErrorDescription = "Failed to save pdf file on database due to " + exception;
                    return output;
                }
               
                emailInfo.PolicyFileByteArray = policyResponseMessage.PolicyFile;
                emailInfo.InvoiceFileByteArray = invoiceFilePdf;
                emailInfo.IsPolicyGenerated = true;
                emailInfo.IsShowErrors = showErrors;
                emailInfo.TawuniyaFileUrl = insuranceCompany.Key == "Tawuniya" ? policyResponseMessage.PolicyFileUrl : "";

                exception = string.Empty;
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Available;
                checkoutDetails.ModifiedDate = DateTime.Now;
                //_checkoutDetailRepository.Update(checkoutDetails);
                _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                if (insuranceCompany.Key == "TokioMarine")
                {
                    emailInfo.ReceiverEmailAddressBCC.Add("marketing@atmc.com.sa");
                }
                if (insuranceCompany.Key == "SAICO")
                {
                    emailInfo.ReceiverEmailAddressBCC.Add("info@saico.com.sa");
                }
                if (checkoutDetails.IsEmailVerified.HasValue && checkoutDetails.IsEmailVerified.Value)
                {
                    var emailOutput = _policyEmailService.SendPolicyByMail(emailInfo, insuranceCompany.Key);
                    if (emailOutput.ErrorCode!=EmailOutput.ErrorCodes.Success)
                    {
                        output.ErrorDescription = "Partial Success Failed to send email to client due to "+emailOutput.ErrorDescription;
                    }
                    else
                    {
                        output.ErrorDescription = "Success";
                    }
                }
                else
                {
                    output.ErrorDescription = "Success Email not Verified";
                }
                output.ErrorCode = 1;
                string smsBody = string.Empty;
                string whatsAppBody = string.Empty;
                string policyFileUrl = "https://bcare.com.sa/Identityapi/api/u/p?r=" + policyResponseMessage.ReferenceId;
                if (insuranceCompany.Key == "Tawuniya")
                    policyFileUrl = policyResponseMessage.PolicyFileUrl;
                string shortUrl = Utilities.GetShortUrl(policyFileUrl);
                if (!string.IsNullOrEmpty(shortUrl))
                    policyFileUrl = shortUrl;
                string emo = DecodeEncodedNonAsciiCharacters("\uD83E\uDD73");
                string emo_heart = DecodeEncodedNonAsciiCharacters("\uD83D\uDC99");
                string emo_flower = DecodeEncodedNonAsciiCharacters("\uD83C\uDF39");
                string emo_store = DecodeEncodedNonAsciiCharacters("\uD83D\uDCF2");
                //string emo_postal = DecodeEncodedNonAsciiCharacters("\uD83D\uDCE9");

                if (userLanguage == LanguageTwoLetterIsoCode.Ar)
                {
                    smsBody = "وثيقتك جاهزة" + "!" + " " + emo;
                    smsBody += " " + policyFileUrl;

                    whatsAppBody = "هلا بك في بي كير " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "وثيقتك جاهزة! " + emo;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "يرجى تحميل تطبيق بي كير لإرسال اشعار التجديد قبل انتهاء الوثيقة وتجديد التأمين بطريقة سهلة " + emo_store;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "http://bcareksa.app.link/7zDuwosCf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "ولأنك تستاهل " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "🏷وفّرنا لك عروض لأكثر من 100 جهة";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "تقدر تستفيد من العروض خلال فترة سريان وثيقتك بزيارتك الرابط التالي: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/wareef";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "ملف التعريفي لبرنامج وريف: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/WareefAR.pdf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "بي كير تتمني لك قيادة آمنة " + emo_heart;
                }
                else
                {
                    smsBody = "Policy is ready! " + emo + " ";
                    smsBody += policyFileUrl;/// smsBody.Replace("[%PolicyFileUrl%]", policyFileUrl);

                    whatsAppBody = "Welcome to BCare " + emo_heart;
                    whatsAppBody = DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Your policy is READY " + emo + " !";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Please download BCare application to receive renewal notifications before the expiry of the policy and easily renew it " + emo_store;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "http://bcareksa.app.link/7zDuwosCf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Because you are valuable " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "🏷We are providing you with offers from more than 100 sectors";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "You can exploit the offers during your policy period. Please visit the below link for more information: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/wareef";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Wareef User Guide: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/WareefEN.pdf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "BCare wishes you a safe drive " + emo_heart;

                }

                //_notificationService.SendSmsAsync(checkoutDetails.Phone, smsBody);
                var smsModel = new SMSModel()
                {
                    PhoneNumber = checkoutDetails.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.PolicyFile.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = channel,
                    ReferenceId = checkoutDetails.ReferenceId
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                _notification.SendFireBaseNotification(checkoutDetails.UserId, "بي كير - Bcare", "وثيقتك جاهزة 😍 الرجاء الذهاب إلى الملف الشخصي \"وثائق التأمين\" لتحميل الوثيقة","PolicyFile", checkoutDetails.ReferenceId, checkoutDetails.Channel);
                _notificationService.SendWhatsAppMessageAsync(checkoutDetails.Phone, whatsAppBody, SMSMethod.PolicyFile.ToString(), referenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), checkoutDetails.SelectedLanguage).ToLower());

                //update promotion with national id as per Fayssal 
                if (!string.IsNullOrEmpty(quoteResponse.PromotionProgramCode)
                    && quoteResponse.PromotionProgramId > 0
                    && insuranceCompany.Key == "ArabianShield")
                {
                    exception = string.Empty;
                    _promotionService.UpdateUserPromotionProgramWithNationalId(
                        checkoutDetails.UserId, quoteResponse.PromotionProgramCode,
                        quoteResponse.PromotionProgramId, quoteResponse.InsuranceCompanyId,
                        insuredId.ToString(), out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorDescription = " Success but failed to update promo due to " + exception;
                    }

                }
                
                return output;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                output.ErrorCode = 12;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }
        private PdfGenerationOutput HandleInsurancePolicyFileResult(PolicyResponse policy, int iCompanyId, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            log.ServerIP = ServicesUtilities.GetServerIP();
            PdfGenerationOutput output = new PdfGenerationOutput();
            try
            {
                if (policy == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Policy is sent to method null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    output.IsPdfGeneratedByBcare = true;
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy is sent to method null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.ReferenceId = policy?.ReferenceId;
                log.PolicyNo = policy?.PolicyNo;
                log.ServiceRequest = JsonConvert.SerializeObject(policy);

                if (iCompanyId == 0)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Company is is sent to method 0";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    output.IsPdfGeneratedByBcare = true;
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.CompanyID = iCompanyId;
                var insuranceCompany = _insuranceCompanyService.GetById(iCompanyId);

                if (insuranceCompany == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    output.ErrorDescription = "Insurance Company Is Null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    output.IsPdfGeneratedByBcare = true;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany?.NameEN;
                if (!string.IsNullOrEmpty(insuranceCompany.ReportTemplateName)
                 || (!string.IsNullOrEmpty(insuranceCompany.AutoleaseReportTemplateName) && log.Channel.ToLower() == Channel.autoleasing.ToString().ToLower()))
                {
                    output.IsPdfGeneratedByBcare = true; ;
                    var policyFile = GeneratePolicyFileFromPolicyDetails(policy, iCompanyId, selectedLanguage, log);
                    if (policyFile.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = policyFile.ErrorCode;
                        output.ErrorDescription = policyFile.ErrorDescription;
                        output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                        return output;
                    }
                    policy.PolicyFile = policyFile.File;
                    output.File = policyFile.File;
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.EPolicyStatus = EPolicyStatus.Available;
                    return output;
                }
                output.IsPdfGeneratedByBcare = false;
                byte[] policyFileByteArray = null;
                if (!string.IsNullOrEmpty(policy.PolicyFileUrl))
                {
                    log.RetrievingMethod = "FileUrl";
                    string fileURL = policy.PolicyFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    log.ServiceURL = fileURL;
                    if (insuranceCompany.Key == "GGI")
                    {
                        log.RetrievingMethod = "FileUrlWebRequest";
                        string responseData = string.Empty;
                        var request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fileURL);
                        request.Timeout = 30000;
                        var response = (System.Net.HttpWebResponse)request.GetResponse();
                        Stream s = response.GetResponseStream();
                        using (var streamReader = new MemoryStream())
                        {
                            s.CopyTo(streamReader);
                            policyFileByteArray = streamReader.ToArray();
                        }
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        if (policyFileByteArray == null)
                        {
                            output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = "policy FileByte Array is returned null";
                            output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }

                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.EPolicyStatus = EPolicyStatus.Available;
                        output.File = policyFileByteArray;

                        policy.PolicyFile = policyFileByteArray;
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                    else
                    {
                        if (insuranceCompany.Key == "Malath" && fileURL.ToLower().StartsWith("http://"))
                        {
                            fileURL = fileURL.Replace("http://", "https://");
                        }
                        using (System.Net.WebClient client = new System.Net.WebClient())
                        {

                            policyFileByteArray = client.DownloadData(fileURL);
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            if (policyFileByteArray == null)
                            {
                                output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                                output.ErrorDescription = "policy FileByte Array is returned null";
                                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = output.ErrorDescription;
                                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                                return output;
                            }

                            output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.EPolicyStatus = EPolicyStatus.Available;
                            output.File = policyFileByteArray;

                            policy.PolicyFile = policyFileByteArray;
                            log.ServiceResponse = "Success";
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }
                    }
                }
                if (policy.PolicyFile != null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.File = policy.PolicyFile;
                    output.EPolicyStatus = EPolicyStatus.Available;

                    log.RetrievingMethod = "Bytes";
                    log.ServiceResponse = "Success";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                IInsuranceProvider provider = null;
                var providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
                if (instance != null)
                {
                    provider = instance as IInsuranceProvider;
                }
                if (instance == null)
                {
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);
                    if (providerType == null)
                    {
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ProviderTypeIsNull;
                        output.ErrorDescription = "providerType Is Null";
                        output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                        log.ErrorDescription = output.ErrorDescription;
                        log.ErrorCode = (int)output.ErrorCode;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }

                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                    Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

                }
                if (provider == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ProviderIsNull;
                    output.ErrorDescription = "provider Is Null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var policyScheduleOutput = provider.PolicySchedule(policy.PolicyNo, policy.ReferenceId);
                if (policyScheduleOutput.ErrorCode != FileServiceOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                    output.ErrorDescription = policyScheduleOutput.ServiceResponse;
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;

                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.RetrievingMethod = "PolicySchedule";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = policyScheduleOutput.ErrorDescription;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (policyScheduleOutput.IsSchedule)
                {
                    log.ServiceURL = policyScheduleOutput.ServiceUrl;
                    log.ServiceResponseTimeInSeconds = policyScheduleOutput.ServiceResponseTimeInSeconds;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;

                    if (!string.IsNullOrEmpty(policyScheduleOutput.PolicyFileUrl))
                    {
                        policy.PolicyFileUrl = policyScheduleOutput.PolicyFileUrl;

                        log.RetrievingMethod = "PolicyScheduleFileUrl";
                        string fileURL = policy.PolicyFileUrl;
                        fileURL = fileURL.Replace(@"\\", @"//");
                        fileURL = fileURL.Replace(@"\", @"/");
                        log.ServiceURL = fileURL;
                        using (System.Net.WebClient client = new System.Net.WebClient())
                        {

                            policyFileByteArray = client.DownloadData(fileURL);
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            if (policyFileByteArray == null)
                            {
                                output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                                output.ErrorDescription = "policy FileByte Array is returned null";
                                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = output.ErrorDescription;
                                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                                return output;
                            }

                            output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                            output.ErrorDescription = "Success";
                            output.EPolicyStatus = EPolicyStatus.Available;
                            output.File = policyFileByteArray;

                            policy.PolicyFile = policyFileByteArray;
                            log.ServiceResponse = "Success";
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = output.ErrorDescription;
                            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                            return output;
                        }
                    }
                    if (policyScheduleOutput.PolicyFile != null)
                    {
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.File = policyScheduleOutput.PolicyFile;
                        output.EPolicyStatus = EPolicyStatus.Available;

                        policyFileByteArray = policyScheduleOutput.PolicyFile;
                        policy.PolicyFile = policyScheduleOutput.PolicyFile;
                        log.RetrievingMethod = "PolicyScheduleBytes";
                        log.ServiceResponse = "Success";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                        return output;
                    }
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                    output.ErrorDescription = policyScheduleOutput.ErrorDescription;
                    output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                    log.RetrievingMethod = "PolicySchedule";
                    log.ErrorCode = (int)PdfGenerationOutput.ErrorCodes.PolicyScheduleError;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponse = policyScheduleOutput.ServiceResponse;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
                output.ErrorDescription = "No File Url or file data in company response";
                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;

            }
            catch (Exception exp)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                output.IsPdfGeneratedByBcare = true;

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
        }

        public PdfGenerationOutput GeneratePolicyFileFromPolicyDetails(PolicyResponse policy, int iCompanyId, LanguageTwoLetterIsoCode selectedLanguage, PdfGenerationLog log)
        {
            PdfGenerationOutput output = new PdfGenerationOutput();
            try
            {
                var serviceURL = Utilities.GetAppSetting("PolicyPDFGeneratorAPIURL") + "api/PolicyPdfGenerator";
                log.RetrievingMethod = "Generation";
                log.ServiceURL = serviceURL;

                log.ServerIP = ServicesUtilities.GetServerIP();
                log.CompanyID = iCompanyId;
                if (string.IsNullOrEmpty(log.Channel))
                    log.Channel = Channel.Portal.ToString();
                log.ServiceRequest = JsonConvert.SerializeObject(policy);

                if (policy == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Policy is sent to method null";
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    output.IsPdfGeneratedByBcare = true;
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Policy is sent to method null";
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.ReferenceId = policy.ReferenceId;
                log.PolicyNo = policy.PolicyNo;

                var modelOutput = GetPolicyTemplateGenerationModel(policy, selectedLanguage);
                if (modelOutput.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = modelOutput.ErrorCode;
                    output.ErrorDescription = modelOutput.ErrorDescription;

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }

                var policyTemplateModel = modelOutput.Output;
                string policyDetailsJsonString = JsonConvert.SerializeObject(policyTemplateModel);
                log.ServiceRequest = policyDetailsJsonString;

                var insuranceCompany = _insuranceCompanyService.GetById(iCompanyId);

                if (insuranceCompany == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    output.ErrorDescription = "Insurance Company Is Null";

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.CompanyName = insuranceCompany?.Key;
                if (string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) && string.IsNullOrEmpty(insuranceCompany.AutoleaseReportTemplateName))
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.InsuranceCompanyIsNull;
                    output.ErrorDescription = "report template name is null";

                    log.ErrorDescription = output.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                ReportGenerationModel reportGenerationModel = new ReportGenerationModel
                {
                    ReportType = insuranceCompany?.ReportTemplateName,
                    ReportDataAsJsonString = policyDetailsJsonString
                };
                if (log.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                {
                    reportGenerationModel.ReportType = insuranceCompany?.AutoleaseReportTemplateName;
                }
                reportGenerationModel.ReportType = reportGenerationModel.ReportType.Replace("#ProductType", policyTemplateModel.ProductType);
                reportGenerationModel.ReportType = reportGenerationModel.ReportType.Replace("#RoadSideAssistant", policyTemplateModel.HasRoadsideAssistanceBenefit ? "WithRoadsideAssistance" : "WithoutRoadsideAssistance");
                //if (log.Channel.ToLower() != Channel.autoleasing.ToString().ToLower())
                //{
                //    if (policyTemplateModel.DriverCovered && policyTemplateModel.PassengerCovered && insuranceCompany.Key == "Sagr")
                //    {
                //        reportGenerationModel.ReportType += "_Driver&Passenger";
                //    }
                //    else if (policyTemplateModel.DriverCovered && insuranceCompany.Key == "Sagr")
                //    {
                //        reportGenerationModel.ReportType += "_Driver_Only";
                //    }
                //    else if (policyTemplateModel.PassengerCovered && insuranceCompany.Key == "Sagr")
                //    {
                //        reportGenerationModel.ReportType += "Passenger_Only";
                //    }

                //}
                if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                    reportGenerationModel.ReportType += "_En";
                else
                    reportGenerationModel.ReportType += "_Ar";

                if (!string.IsNullOrEmpty(policyTemplateModel.ProductDescription) && !string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) && insuranceCompany.Key == "AlRajhi")
                {
                    reportGenerationModel.ReportType = policyTemplateModel.ProductDescription;
                }
                if (!string.IsNullOrEmpty(policyTemplateModel.ProductDescription) && !string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) &&
                    (policyTemplateModel.ProductDescription.ToLower() == "Wafi Comprehensive".ToLower() || policyTemplateModel.ProductDescription.ToLower() == "Wafi Smart".ToLower() || policyTemplateModel.ProductDescription.ToLower() == "Wafi Basic".ToLower()) && insuranceCompany.Key == "AlRajhi")
                {
                    if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                        reportGenerationModel.ReportType += "_En";
                    else
                        reportGenerationModel.ReportType += "_Ar";
                }
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
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (response.Content == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Service response content result return null";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                var pdfGeneratorResponseString = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Service http status code is not ok it returned " + response.ToString();


                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
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
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                return output;
            }
        }
        public PdfGenerationOutput GetFailedPolicyFile(string referenceId, string serverIP, string channel = "RetrialMechanism")
        {
            if (string.IsNullOrEmpty(serverIP))
                serverIP = Utilities.GetAppSetting("FrontEndServerIP");
            PdfGenerationOutput output = new PdfGenerationOutput();
            PdfGenerationLog log = new PdfGenerationLog();
            log.Channel = channel;
            log.ReferenceId = referenceId;
            log.ServerIP = serverIP;
            DateTime dtBefore = DateTime.Now;
            string exception;

            try
            {
                ServiceRequestLog policyRequestLog = ServiceRequestLogDataAccess.GetPolicyByRefernceId(referenceId);
                if (policyRequestLog == null)
                {
                    var item = _policyProcessingQueue.TableNoTracking.FirstOrDefault(a => a.ReferenceId == referenceId);
                    if (item != null)
                    {
                        policyRequestLog = new ServiceRequestLog();
                        policyRequestLog.ServiceRequest = item.ServiceRequest;
                        policyRequestLog.ServiceResponse = item.ServiceResponse;
                        policyRequestLog.CompanyName = item.CompanyName;
                        policyRequestLog.CompanyID = item.CompanyID;
                        policyRequestLog.DriverNin = item.DriverNin;
                        policyRequestLog.VehicleId = item.VehicleId;
                        policyRequestLog.Channel = item.Chanel;
                    }
                }
                if (policyRequestLog == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "policyRequestLog is null";
                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)output.ErrorCode;

                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                log.ServiceRequest = policyRequestLog.ServiceRequest;
                log.CompanyName = policyRequestLog.CompanyName;
                log.CompanyID = policyRequestLog.CompanyID;
                log.DriverNin = policyRequestLog.DriverNin;
                log.VehicleId = policyRequestLog.VehicleId;

                PolicyResponse policyResponseMessage = null;
                if (policyRequestLog.CompanyName.ToLower() == "MedGulf".ToLower())
                {
                    var responseValue = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(policyRequestLog.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        Errors = responseValue.Errors,
                        PolicyFileUrl = responseValue.PolicyFileUrl,
                        PolicyNo = responseValue.PolicyNo,
                        ReferenceId = responseValue.ReferenceId,
                        StatusCode = responseValue.StatusCode,
                        PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyEffectiveDate),
                        PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyExpiryDate),
                        PolicyIssuanceDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyIssuanceDate)
                    };
                }
                else if (policyRequestLog.CompanyName.ToLower() == "Tawuniya".ToLower() && (policyRequestLog.Channel != null && policyRequestLog.Channel != Channel.autoleasing.ToString()))
                {
                    var result = JsonConvert.DeserializeObject<Integration.Providers.Tawuniya.Dtos.PolicyResponseDto>(policyRequestLog.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        StatusCode = 1,
                        ReferenceId = referenceId,
                        PolicyNo = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyNumber,
                        PolicyFileUrl = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyDocPrintingLink,
                        PolicyExpiryDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyExpiryDate, "yyyy-MM-dd", new CultureInfo("en-US")),
                        PolicyIssuanceDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyInceptionDate, "yyyy-MM-dd", new CultureInfo("en-US"))
                    };

                }
                else if (policyRequestLog.CompanyName.ToLower() == "tawuniya".ToLower() && (policyRequestLog.Channel != null && policyRequestLog.Channel == Channel.autoleasing.ToString()))
                {
                    var result = JsonConvert.DeserializeObject<Integration.Providers.Tawuniya.Dtos.AutoLeasingPolicyResponse>(policyRequestLog.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        StatusCode = 1,
                        ReferenceId = referenceId,
                        PolicyNo = result.PolicyNumber,
                        PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromAllianz(result.PolicyEffectiveDate + " 00:00:00"),
                        PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromAllianz(result.PolicyExpiryDate + " 23:59:59"),
                        PolicyIssuanceDate = DateTime.Now
                    };
                }
                else if (policyRequestLog.CompanyName.ToLower() == "alalamiya".ToLower() && (policyRequestLog.Channel != null && policyRequestLog.Channel != Channel.autoleasing.ToString()))
                {
                    var deserializeResult = JsonConvert.DeserializeObject<AlamiaPolicyResponse>(policyRequestLog.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        ReferenceId = deserializeResult.ReferenceId,
                        StatusCode = deserializeResult.StatusCode,
                        PolicyNo = deserializeResult.PolicyNo,
                        PolicyIssuanceDate = (deserializeResult.PolicyIssuanceDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyIssuanceDate)
                                            : null,
                        PolicyEffectiveDate = (deserializeResult.PolicyEffectiveDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyEffectiveDate)
                                            : null,
                        PolicyExpiryDate = (deserializeResult.PolicyExpiryDate != null)
                                            ? Utilities.ConvertStringToDateTimeFromAllianz(deserializeResult.PolicyExpiryDate)
                                            : null,
                        Errors = deserializeResult.Errors
                    };
                }
                else
                {
                    try
                    {
                        policyResponseMessage = JsonConvert.DeserializeObject<PolicyResponse>(policyRequestLog.ServiceResponse);
                    }
                    catch
                    {
                        policyResponseMessage = new PolicyResponse
                        {
                            StatusCode = 1,
                            ReferenceId = referenceId,
                        };
                    }
                }
                if (string.IsNullOrEmpty(policyResponseMessage.ReferenceId))
                    policyResponseMessage.ReferenceId = referenceId;
                var checkoutDetails = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == referenceId);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "checkoutDetails is null";

                    log.ServiceResponse = "Failed";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                    return output;
                }
                channel = checkoutDetails.Channel;
                log.Channel = channel;
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }

                bool generateTemplateFromOurSide = true;
                //if (checkoutDetails.InsuranceCompanyId == 11 && checkoutDetails.SelectedInsuranceTypeCode == 1)
                //    generateTemplateFromOurSide = false;

                //if (channel.ToLower() == "dashboard")
                //{

                // the blew conditin to handle special failed policy which are made by operations form dashboard (re-generate)
                var policy = _policyRepository.TableNoTracking.OrderByDescending(e => e.Id).FirstOrDefault(i => i.CheckOutDetailsId == referenceId);
                if (policy == null)
                {
                    var policyId = SavePolicy(policyResponseMessage, policyRequestLog.CompanyID.Value);
                    if (!policyId.HasValue)
                    {
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "GetFailedPolicyFile: Failed To Save Policy to database or may policy already exist";
                        return output;
                    }
                    var invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                    if (invoice == null)
                    {
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "GetFailedPolicyFile: invoice is null";
                        return output;
                    }

                    invoice.PolicyId = policyId;
                    _invoiceRepository.Update(invoice);
                }

                var policyStatus= _policyFileContext.GeneratePolicyPdfFile(policyResponseMessage, policyRequestLog.CompanyID.Value, checkoutDetails.Channel, lang, generateTemplateFromOurSide);
                //}
                //else
                //{
                //    policyStatus = HandleInsurancePolicyFileResult(policyResponseMessage, policyRequestLog.CompanyID.Value, lang, log);
                //}
                if (policyStatus.ErrorCode != PolicyGenerationOutput.ErrorCodes.Success)
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = (int)policyStatus.EPolicyStatus;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "GetFailedPolicyFile Failed to generate or download the pdf file due to " + policyStatus.ErrorDescription;
                    return output;
                }

                bool isAutoleasingPolicy = (channel.ToLower() == Channel.autoleasing.ToString().ToLower()) ? true : false;
                string filePath = Utilities.SaveCompanyFile(referenceId, policyResponseMessage.PolicyFile, policyRequestLog.CompanyName, true, isAutoleasingPolicy);
                if (string.IsNullOrEmpty(filePath))
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Failed to save pdf file on server";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                    return output;
                }
                if (policyRequestLog.CompanyName == "GGI" && !IsValidPdfFile(filePath))// validate if file is corrupted
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "it's not valid pdf file Lenght is to short it's " + policyResponseMessage.PolicyFile.Length;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
                }
                exception = string.Empty;
                Guid fileId = SavePolicyFile(policyResponseMessage, filePath, serverIP, out exception);
                if (fileId == Guid.Empty)
                {
                    exception = string.Empty;
                    checkoutDetails.PolicyStatusId = policyStatus.IsPdfGeneratedByBcare ? (int)EPolicyStatus.PolicyFileGeneraionFailure : (int)EPolicyStatus.PolicyFileDownloadFailure;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    //_checkoutDetailRepository.Update(checkoutDetails);
                    _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.DatabaseException;
                    output.ErrorDescription = "Failed to save pdf file on database due to " + exception;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                    return output;
                }
                if (channel.ToLower() == Channel.Dashboard.ToString().ToLower())
                {
                    var policyProcessingQueue = _policyProcessingQueue.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                    if (policyProcessingQueue != null)
                    {
                        policyProcessingQueue.ProcessedOn = DateTime.Now;
                        policyProcessingQueue.Chanel = channel;
                        DateTime dtAfter = DateTime.Now;
                        policyProcessingQueue.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                        _policyProcessingQueue.Update(policyProcessingQueue);
                    }
                }

                SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                {
                    Channel= checkoutDetails.Channel,
                    Module= checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower()?Module.Autoleasing.ToString() : Module.Vehicle.ToString(),
                    Method = "PolicyFile",
                    PolicyResponseMessage = policyResponseMessage,
                    ReceiverEmailAddress = checkoutDetails.Email,
                    ReferenceId = policyResponseMessage.ReferenceId,
                    UserLanguage = lang,
                    PolicyFileByteArray = policyResponseMessage.PolicyFile,
                    IsPolicyGenerated = true,
                    IsShowErrors = false,
                    TawuniyaFileUrl = policyRequestLog.CompanyName == "Tawuniya" ? policyResponseMessage.PolicyFileUrl : "",
                    InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode
                };
                var emailOutput = _policyEmailService.SendPolicyByMail(sendPolicyViaMailDto, policyRequestLog.CompanyName);
                if (emailOutput.ErrorCode!=EmailOutput.ErrorCodes.Success)
                {
                    output.ErrorDescription = "Partial Success Failed to send email to client due to "+emailOutput.ErrorDescription;
                }
                else
                {
                    output.ErrorDescription = "Success";
                }

                exception = string.Empty;
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Available;
                checkoutDetails.ModifiedDate = DateTime.Now;
                //_checkoutDetailRepository.Update(checkoutDetails);
                _checkoutContext.UpdateCheckoutWithPolicyStatus(checkoutDetails, out exception);

                string smsBody = string.Empty;
                string whatsAppBody = string.Empty;
                string policyFileUrl = "https://bcare.com.sa/Identityapi/api/u/p?r=" + policyResponseMessage.ReferenceId;
                if (policyRequestLog.CompanyName.ToLower() == "Tawuniya".ToLower())
                    policyFileUrl = policyResponseMessage.PolicyFileUrl;
                string shortUrl = Utilities.GetShortUrl(policyFileUrl);
                if (!string.IsNullOrEmpty(shortUrl))
                    policyFileUrl = shortUrl;
                string emo = DecodeEncodedNonAsciiCharacters("\uD83E\uDD73");
                string emo_heart = DecodeEncodedNonAsciiCharacters("\uD83D\uDC99");
                string emo_flower = DecodeEncodedNonAsciiCharacters("\uD83C\uDF39");
                string emo_store = DecodeEncodedNonAsciiCharacters("\uD83D\uDCF2");
                //string emo_postal = DecodeEncodedNonAsciiCharacters("\uD83D\uDCE9");
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    smsBody = "Policy is ready! " + emo + " ";
                    smsBody += policyFileUrl;/// smsBody.Replace("[%PolicyFileUrl%]", policyFileUrl);

                    whatsAppBody = "Welcome to BCare " + emo_heart;
                    whatsAppBody = DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Your policy is READY " + emo + " !";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Please download BCare application to receive renewal notifications before the expiry of the policy and easily renew it " + emo_store;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "http://bcareksa.app.link/7zDuwosCf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Because you are valuable " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "🏷We are providing you with offers from more than 100 sectors";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "You can exploit the offers during your policy period. Please visit the below link for more information: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/wareef";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "Wareef User Guide: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/WareefEN.pdf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "BCare wishes you a safe drive " + emo_heart;
                }
                else
                {
                    smsBody = "وثيقتك جاهزة" + "!" + " " + emo;
                    smsBody += " " + policyFileUrl;

                    whatsAppBody = "هلا بك في بي كير " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "وثيقتك جاهزة! " + emo;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "يرجى تحميل تطبيق بي كير لإرسال اشعار التجديد قبل انتهاء الوثيقة وتجديد التأمين بطريقة سهلة " + emo_store;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "http://bcareksa.app.link/7zDuwosCf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "ولأنك تستاهل " + emo_heart;
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "🏷وفّرنا لك عروض لأكثر من 100 جهة";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "تقدر تستفيد من العروض خلال فترة سريان وثيقتك بزيارتك الرابط التالي: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/wareef";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "ملف التعريفي لبرنامج وريف: ";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "https://bcare.com.sa/WareefAR.pdf";
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    whatsAppBody += "بي كير تتمني لك قيادة آمنة " + emo_heart;
                }
                var smsModel = new SMSModel()
                {
                    PhoneNumber = checkoutDetails.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.PolicyFile.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = channel,
                    ReferenceId = checkoutDetails.ReferenceId
                };
                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                _notification.SendFireBaseNotification(checkoutDetails.UserId, "بي كير - Bcare", "وثيقتك جاهزة برجاءالذهاب إلى الملف الشخصى","FailedPolicyFile", checkoutDetails.ReferenceId, checkoutDetails.Channel);
                _notificationService.SendWhatsAppMessageAsync(checkoutDetails.Phone, whatsAppBody, SMSMethod.PolicyFile.ToString(), referenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), checkoutDetails.SelectedLanguage).ToLower());

                try
                {
                    DateTime endDate = new DateTime(2020, 12, 31, 23, 59, 59);
                    if (DateTime.Now <= endDate)
                    {
                        MorniContext morniContext = new MorniContext(_morniRequestRepository);
                        if (morniContext.CreateMorniMembership(referenceId, checkoutDetails.Channel))
                        {
                            smsBody = string.Empty;
                            if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.Ar)
                            {
                                smsBody = "تم تفعيل خدمة المساعدة على الطريق مجانًا يرجى تفعيل العضوية عبر تطبيق مرنى http://bit.ly/MorniRS :وقم بإدخال رقم الهيكل للمركبة [%CHASSISNUMBER%]:لتفاصيل أكثر ";
                                smsBody += " " + "https://bcare.com.sa/rsa.pdf";

                                Vehicle vehicle = _vehicleService.GetVehicle(checkoutDetails.VehicleId.ToString());
                                if (vehicle != null)
                                {
                                    smsBody = smsBody.Replace("[%CHASSISNUMBER%]", vehicle.ChassisNumber);
                                }
                            }
                            else
                            {
                                smsBody = "The roadside assistance service has been activated for free " +
                                      "Please activate the membership via the Morni app: http://bit.ly/MorniRS " +
                                      "and enter Vehicle Chassis Number:[%CHASSISNUMBER%] for more details https://bcare.com.sa/rsa.pdf";
                                Vehicle vehicle = _vehicleService.GetVehicle(checkoutDetails.VehicleId.ToString());
                                if (vehicle != null)
                                {
                                    smsBody = smsBody.Replace("[%CHASSISNUMBER%]", vehicle.ChassisNumber);
                                }
                            }
                            smsModel = new SMSModel()
                            {
                                PhoneNumber = checkoutDetails.Phone,
                                MessageBody = smsBody,
                                Method = SMSMethod.MorniSMS.ToString(),
                                Module = Module.Vehicle.ToString(),
                                Channel = channel
                            };
                            _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        }
                    }
                }
                catch (Exception excep)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\morni2.txt", excep.ToString());

                }
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);

                return output;
            }
        }

        public int? SavePolicy(PolicyResponse policy, int? companyId)
        {
            int? result = null;
            Tameenk.Core.Domain.Entities.Policy policyData = null;
            try
            {
                var isExist = _policyRepository.Table.Any(a => a.CheckOutDetailsId == policy.ReferenceId);
                if (!isExist)
                {
                    if (companyId != 27 && companyId != 4) // I add this because company chanage stauscode to status
                    {
                        if (policy.StatusCode != 1)
                            return result;
                        if (string.IsNullOrEmpty(policy.PolicyNo))
                            return result;
                    }


                    if (companyId == 21) // SAICO
                    {
                        policy.PolicyEffectiveDate = HandleDateFormate(policy.PolicyEffectiveDate.ToString());
                        policy.PolicyExpiryDate = HandleDateFormate(policy.PolicyExpiryDate.ToString());
                        policy.PolicyIssuanceDate = HandleDateFormate(policy.PolicyIssuanceDate.ToString());
                    }

                    policyData = new Tameenk.Core.Domain.Entities.Policy
                    {
                        CheckOutDetailsId = policy.ReferenceId,
                        PolicyEffectiveDate = policy.PolicyEffectiveDate,
                        PolicyExpiryDate = policy.PolicyExpiryDate,
                        PolicyIssueDate = policy.PolicyIssuanceDate,
                        PolicyNo = policy.PolicyNo,
                        StatusCode = Convert.ToByte(policy.StatusCode),
                        InsuranceCompanyID = companyId,
                        IsCancelled = false,
                        CreatedDate = DateTime.Now,
                        NotificationNo = 0
                    };

                  

                    var jsonSerializer = new JavaScriptSerializer();


                    PolicyDetail policyDetail = null;
                    if (policy.PolicyDetails != null)
                    {


                        policyDetail = new PolicyDetail()
                        {
                            Id = policyData.Id,
                            Policy = policyData,
                            DocumentSerialNo = policy.PolicyDetails.DocumentSerialNo,
                            PolicyNo = policy.PolicyDetails.PolicyNo,
                            InsuranceStartDate = policy.PolicyDetails.InsuranceStartDate,
                            InsuranceEndDate = policy.PolicyDetails.InsuranceEndDate,
                            PolicyCoverTypeEn = policy.PolicyDetails.PolicyCoverTypeEn,
                            PolicyCoverTypeAr = policy.PolicyDetails.PolicyCoverTypeAr,
                            PolicyCoverAgeLimitEn = jsonSerializer.Serialize(policy.PolicyDetails.PolicyCoverAgeLimitEn),
                            PolicyCoverAgeLimitAr = jsonSerializer.Serialize(policy.PolicyDetails.PolicyCoverAgeLimitAr),
                            PolicyAdditionalCoversEn = jsonSerializer.Serialize(policy.PolicyDetails.PolicyAdditionalCoversEn),
                            PolicyAdditionalCoversAr = jsonSerializer.Serialize(policy.PolicyDetails.PolicyAdditionalCoversAr),
                            PolicyGeographicalAreaEn = policy.PolicyDetails.PolicyGeographicalAreaEn,
                            PolicyGeographicalAreaAr = policy.PolicyDetails.PolicyGeographicalAreaAr,
                            InsuredNameEn = policy.PolicyDetails.InsuredNameEn,
                            InsuredNameAr = policy.PolicyDetails.InsuredNameAr,
                            InsuredMobile = policy.PolicyDetails.InsuredMobile,
                            InsuredID = policy.PolicyDetails.InsuredID,
                            InsuredCity = policy.PolicyDetails.InsuredCity,
                            InsuredDisctrict = policy.PolicyDetails.InsuredDisctrict,
                            InsuredStreet = policy.PolicyDetails.InsuredStreet,
                            InsuredBuildingNo = policy.PolicyDetails.InsuredBuildingNo,
                            InsuredZipCode = policy.PolicyDetails.InsuredZipCode,
                            InsuredAdditionalNo = policy.PolicyDetails.InsuredAdditionalNo,
                            VehicleMakeEn = policy.PolicyDetails.VehicleMakeEn,
                            VehicleMakeAr = policy.PolicyDetails.VehicleMakeAr,
                            VehicleModelEn = policy.PolicyDetails.VehicleModelEn,
                            VehicleModelAr = policy.PolicyDetails.VehicleModelAr,
                            VehiclePlateTypeEn = policy.PolicyDetails.VehiclePlateTypeEn,
                            VehiclePlateTypeAr = policy.PolicyDetails.VehiclePlateTypeAr,
                            VehiclePlateNoEn = policy.PolicyDetails.VehiclePlateNoEn,
                            VehiclePlateNoAr = policy.PolicyDetails.VehiclePlateNoAr,
                            VehicleChassis = policy.PolicyDetails.VehicleChassis,
                            VehicleBodyEn = policy.PolicyDetails.VehicleBodyEn,
                            VehicleBodyAr = policy.PolicyDetails.VehicleBodyAr,
                            VehicleYear = policy.PolicyDetails.VehicleYear,
                            VehicleColorEn = policy.PolicyDetails.VehicleColorEn,
                            VehicleColorAr = policy.PolicyDetails.VehicleColorAr,
                            VehicleCapacity = policy.PolicyDetails.VehicleCapacity,
                            VehicleSequenceNo = policy.PolicyDetails.VehicleSequenceNo,
                            VehicleCustomNo = policy.PolicyDetails.VehicleCustomNo,
                            VehicleOwnerName = policy.PolicyDetails.VehicleOwnerName,
                            VehicleOwnerID = policy.PolicyDetails.VehicleOwnerID,
                            VehicleUsingPurposesEn = policy.PolicyDetails.VehicleUsingPurposesEn,
                            VehicleUsingPurposesAr = policy.PolicyDetails.VehicleUsingPurposesAr,
                            VehicleRegistrationExpiryDate = policy.PolicyDetails.VehicleRegistrationExpiryDate,
                            VehicleValue = policy.PolicyDetails.VehicleValue,
                            OfficePremium = policy.PolicyDetails.OfficePremium,
                            PACover = policy.PolicyDetails.PACover,
                            ValueExcess = policy.PolicyDetails.ValueExcess,
                            TotalPremium = policy.PolicyDetails.TotalPremium,
                            NCDPercentage = policy.PolicyDetails.NCDPercentage,
                            NCDAmount = policy.PolicyDetails.NCDAmount,
                            VATPercentage = policy.PolicyDetails.VATPercentage,
                            VATAmount = policy.PolicyDetails.VATAmount,
                            CommissionPaid = policy.PolicyDetails.CommissionPaid,
                            PolicyFees = policy.PolicyDetails.PolicyFees,
                            ClalmLoadingPercentage = policy.PolicyDetails.ClalmLoadingPercentage,
                            ClalmLoadingAmount = policy.PolicyDetails.ClalmLoadingAmount,
                            AgeLoadingAmount = policy.PolicyDetails.AgeLoadingAmount,
                            AgeLoadingPercentage = policy.PolicyDetails.AgeLoadingPercentage,
                            DeductibleValue = policy.PolicyDetails.DeductibleValue,
                            InsuranceEndDateH = policy.PolicyDetails.InsuranceEndDateH,
                            InsuranceStartDateH = policy.PolicyDetails.InsuranceStartDateH,
                            InsuredAge = policy.PolicyDetails.InsuredAge,
                            NCDFreeYears = policy.PolicyDetails.NCDFreeYears,
                            AccidentNo = policy.PolicyDetails.AccidentNo,
                            AccidentLoadingAmount = policy.PolicyDetails.AccidentLoadingAmount,
                            AccidentLoadingPercentage = policy.PolicyDetails.AccidentLoadingPercentage,
                            PolicyIssueDate = policy.PolicyDetails.PolicyIssueDate,
                            PolicyIssueTime = policy.PolicyDetails.PolicyIssueTime
                        };
                        policyData.PolicyDetail = policyDetail;
                    }

                    _policyRepository.Insert(policyData);
                    result = policyData.Id;
                }

                return result;
            }
            catch (DbUpdateException ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_DbUpdateException_" + policy.ReferenceId + "_Exception.txt", ex.ToString());
                try
                {
                    policyData.Invoices = null;
                    if (policyData.PolicyDetail!=null && policyData.PolicyDetail.Policy!=null)
                    {
                        policyData.PolicyDetail.Policy.Invoices = null;
                    }
                    _policyRepository.Insert(policyData);
                }
                catch (Exception exc)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_DbUpdateException_" + policy.ReferenceId + "_EXCeption.txt", exc.ToString());
                }
          
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_DbUpdateException_" + policy.ReferenceId + "_Id.txt", $"{policyData.Id}");
                return policyData.Id;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_" + policy.ReferenceId + "_Exception.txt", ex.ToString());
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_" + policy.ReferenceId + "_policy.txt", JsonConvert.SerializeObject(policy));
                //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SavePolicy_" + policy.ReferenceId + "_policyData.txt", JsonConvert.SerializeObject(policyData));
                return result;
            }
        }

        #region MyRegion

     

        private DateTime? HandleDateFormate(string strValue)
        {
            var value = strValue;
            var date = strValue.Split(' ');
            var dateComponents = date[0].Split('/');

            string day = dateComponents[0];
            string month = dateComponents[1];
            string year = HandleDateYear(dateComponents[2]);
            value = $"{HandleTwoDigitDayAndMonth(day)}/{HandleTwoDigitDayAndMonth(month)}/{year} {date[1]} {date[2]}";

            DateTime dt;
            if (DateTime.TryParse(value, out dt))
                return dt;
            else if (DateTime.TryParse(strValue, out dt))
                return dt;
            return null;
        }

        private DateTime? HandleDateFormateNew(DateTime dateTimeValue)
        {
            if (dateTimeValue.Year > 0 && dateTimeValue.Year<2000)
            {
                var year = dateTimeValue.Year + 2000;
                var month = dateTimeValue.Month;
                var day = dateTimeValue.Day;
                var newdate = new DateTime(year, month, day);
                return newdate;
            }
            return dateTimeValue;
        }


            static string HandleDateYear(string dateYear)
        {
            var currentYear = DateTime.Now.Date.Year.ToString();
            if (dateYear.StartsWith("00"))
                dateYear = dateYear.Replace((dateYear.Substring(0, 2)), (currentYear.Substring(0, 2)));

            return dateYear;
        }

        private string HandleTwoDigitDayAndMonth(string value)
        {
            if (value.Length == 2)
                return value;

            var newValue = ConvertToTwoDigitString(value);
            return newValue;
        }

        private string ConvertToTwoDigitString(string input)
        {
            if (int.TryParse(input, out int number))
                return number.ToString("D2");

            return input;
        }

        #endregion

        private void UpdatePolicyStatus(CheckoutDetail checkoutDetail, EPolicyStatus status)
        {
            //update policy status 
            checkoutDetail.PolicyStatusId = (int)status;
            //save the updates into db
            _checkoutDetailRepository.Update(checkoutDetail);
        }

        private Guid SavePolicyFile(PolicyResponse policy, string filePath, string serverIP, out string exception)
        {
            try
            {
                Guid policyFileId = Guid.NewGuid();
                PolicyFile policyFile = new PolicyFile
                {
                    ID = policyFileId,
                    //PolicyFileByte = policy.PolicyFile,
                    FilePath = filePath,
                    ServerIP = serverIP
                };
                _policyFileRepository.Insert(policyFile);
                //get policy entity 
                Tameenk.Core.Domain.Entities.Policy policyEntity = _policyRepository.Table.FirstOrDefault(p => p.CheckOutDetailsId == policy.ReferenceId);
                //update policy fileId 
                policyEntity.PolicyFileId = policyFileId;
                //save the updates into db
                _policyRepository.Update(policyEntity);
                exception = string.Empty;
                return policyFileId;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return Guid.Empty;
            }
        }
        private PdfGenerationOutput GetPolicyTemplateGenerationModel(PolicyResponse policy, LanguageTwoLetterIsoCode selectedLanguage)
        {
            var output = new PdfGenerationOutput();
            try
            {
                var stringLang = Enum.GetName(typeof(LanguageTwoLetterIsoCode), selectedLanguage).ToLower();
                var quotationResponseQry = _quotationResponseRepository.TableNoTracking
                    .Include(e => e.QuotationRequest.Driver.Addresses)
                    .Include(e => e.QuotationRequest.Vehicle.VehicleBodyType)
                    .Include(e => e.QuotationRequest.Vehicle.VehiclePlateType)
                    .Include(e => e.QuotationRequest.Insured)
                    .Include(e => e.QuotationRequest.Drivers.Select(d => d.Addresses))
                    .Include(e => e.QuotationRequest.Drivers.Select(d => d.DriverLicenses))
                     .Include(e => e.QuotationRequest.Driver.DriverLicenses)
                    .Include(e => e.ProductType)
                    .Where(e => e.ReferenceId == policy.ReferenceId);

                var quotationResponse = quotationResponseQry.FirstOrDefault();
                if (quotationResponse == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.QuotationResponseIsNull;
                    output.ErrorDescription = "Quotation Response object is null";
                    return output;
                }
                var checkoutDetails = _checkoutDetailRepository.TableNoTracking.Include(c => c.BankCode).Where(e => e.ReferenceId == policy.ReferenceId).FirstOrDefault();
                if (checkoutDetails == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.CheckoutDetailsIsNull;
                    output.ErrorDescription = "checkout Details object is null";
                    return output;
                }
                var lang = Enum.GetName(typeof(LanguageTwoLetterIsoCode), checkoutDetails.SelectedLanguage).ToLower();

                var orderItem = _orderItemRepository.TableNoTracking.Include(x => x.OrderItemBenefits.Select(y => y.Benefit)).Include(x => x.Product.PriceDetails).Where(e => e.CheckoutDetailReferenceId == policy.ReferenceId).FirstOrDefault();
                if (orderItem == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.OrderItemIsNull;
                    output.ErrorDescription = "order Item object is null";
                    return output;
                }
                var invoice = _invoiceRepository.TableNoTracking.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == policy.ReferenceId && i.PolicyId != null);
                decimal? totalPrice = null;
                decimal? vat = null;
                if (invoice != null)
                {
                    totalPrice = invoice.TotalPrice;
                    vat = invoice.Vat;
                }
                PolicyTemplateGenerationModel policyTemplateModel = null;
                if (quotationResponse != null)
                {
                    if (!policy.PolicyEffectiveDate.HasValue || !policy.PolicyExpiryDate.HasValue || !policy.PolicyIssuanceDate.HasValue)
                    {
                        var info = _policyRepository.TableNoTracking.Where(a => a.CheckOutDetailsId == policy.ReferenceId).FirstOrDefault();
                        if (info != null)
                        {
                            policy.PolicyEffectiveDate = info.PolicyEffectiveDate;
                            policy.PolicyIssuanceDate = info.PolicyIssueDate;
                            policy.PolicyExpiryDate = info.PolicyExpiryDate;
                            policy.PolicyNo = info.PolicyNo;
                        }
                    }
                    if (policy.PolicyDetails == null)
                        policy.PolicyDetails = new PolicyDetails();

                    try
                    {
                        policyTemplateModel = policy.PolicyDetails.ToTemplateModel();
                    }
                    catch (Exception ex)
                    {
                        // To skip the mapping error
                        policyTemplateModel = new PolicyTemplateGenerationModel();
                        policyTemplateModel.PolicyNo = policy.PolicyDetails.PolicyNo;
                        policyTemplateModel.ReferenceNo = policy.PolicyDetails.ReferenceNo;
                    }

                    var insured = quotationResponse.QuotationRequest.Insured;
                    var mainDriver = quotationResponse.QuotationRequest.Driver;
                    var vehicle = quotationResponse.QuotationRequest.Vehicle;
                    List<Driver> additionalDrivers = new List<Driver>();
                    if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                        additionalDrivers = quotationResponse.QuotationRequest.Drivers.Where(x => x.NIN.ToUpper() != mainDriver.NIN.ToUpper()).ToList();
                    else
                        additionalDrivers = quotationResponse.QuotationRequest.Drivers.Where(x => x.NIN.ToUpper() != insured.NationalId.ToUpper()).ToList();
                    //var additionalDrivers = quotationResponse.QuotationRequest.Drivers.Where(x => x.NIN.ToUpper() != insured.NationalId.ToUpper()).ToList();
                    Driver secondDriver = null;
                    Driver thirdDriver = null;
                    Driver fourthDriver = null;
                    Driver fifthDriver = null;

                    if (additionalDrivers != null && additionalDrivers.Count > 0)
                        secondDriver = additionalDrivers[0];
                    if (additionalDrivers != null && additionalDrivers.Count > 1)
                        thirdDriver = additionalDrivers[1];
                    if (additionalDrivers != null && additionalDrivers.Count > 2)
                        fourthDriver = additionalDrivers[2];
                    if (additionalDrivers != null && additionalDrivers.Count > 3)
                        fifthDriver = additionalDrivers[3];
                    decimal discount1 = 0;
                    decimal vatPrice = 0;
                    decimal fees = 0;
                    decimal extraPremiumPrice = 0;
                    decimal discount = 0;
                    decimal bcareCommission = 0;
                    decimal clalmLoadingAmount = 0;
                    decimal additionAgeContribution = 0;
                    decimal loyalDiscount = 0;

                    Address mainDriverAddress = null;
                    if (insured.AddressId.HasValue)
                    {
                        mainDriverAddress = _addressService.GetAddressDetailsNoTracking(insured.AddressId.Value);
                    }
                    if (mainDriverAddress == null)
                    {
                        mainDriverAddress = _addressService.GetAddressesByNin(insured.NationalId);
                    }
                    if (orderItem.Product != null && orderItem.Product.PriceDetails != null)
                    {
                        var priceDetail = orderItem.Product.PriceDetails.Where(a => a.PriceTypeCode == 4).FirstOrDefault();
                        if (priceDetail != null)
                        {
                            policyTemplateModel.ClalmLoadingAmount = priceDetail.PriceValue.ToString();
                        }
                        foreach (var price in orderItem.Product.PriceDetails)
                        {
                            switch (price.PriceTypeCode)
                            {
                                case 1: discount1 += price.PriceValue; break;
                                case 2: discount += price.PriceValue; break;
                                case 3: loyalDiscount += price.PriceValue; break;
                                case 4: clalmLoadingAmount += price.PriceValue; break;
                                case 5: additionAgeContribution += price.PriceValue; break;
                                case 6: fees += price.PriceValue; break;
                                case 7: extraPremiumPrice += price.PriceValue; break;
                                case 8: vatPrice += price.PriceValue; break;
                                case 9: bcareCommission += price.PriceValue; break;
                            }
                        }
                    }

                    decimal totalBenfitPrice = 0;
                    if (orderItem.Product != null && orderItem.OrderItemBenefits != null)
                    {
                        decimal benefitsPrice = orderItem.OrderItemBenefits
                                        .Select(x => x.Price)
                                         .Sum();
                        //decimal vatValue = vatPrice + orderItem.OrderItemBenefits
                        //                   .Select(x => x.Price * (decimal)0.15)
                        //                   .Sum();
                        totalBenfitPrice = benefitsPrice;
                        if (benefitsPrice > 0)
                            policyTemplateModel.BenfitPrice = Math.Round(totalBenfitPrice, 2).ToString();
                        else
                            policyTemplateModel.BenfitPrice = "0.00";


                    }

                    policyTemplateModel.SpecialDiscount = discount1.ToString("#.##");
                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredAge) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredAge.Trim()))
                        policyTemplateModel.InsuredAge = insured.BirthDate.GetUserAge().ToString();
                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredBuildingNo) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredBuildingNo.Trim()))
                        policyTemplateModel.InsuredBuildingNo = mainDriverAddress?.BuildingNumber;

                    policyTemplateModel.ProductTypeAr = quotationResponse.ProductType?.ArabicDescription;
                    policyTemplateModel.ProductType = quotationResponse.ProductType?.EnglishDescription;
                    policyTemplateModel.PrintingDate = DateTime.Now.ToString("dd/MM/yyyy");

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleCapacity) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleCapacity.Trim()))
                        policyTemplateModel.VehicleCapacity = policy.PolicyDetails?.VehicleCapacity;

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyNo) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyNo.Trim()))
                        policyTemplateModel.PolicyNo = policy.PolicyNo;

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueDate) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueDate.Trim()))
                        policyTemplateModel.PolicyIssueDate = policy.PolicyIssuanceDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueTime) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueTime.Trim()))
                    {
                        if (policy.PolicyIssuanceDate != null)
                            policyTemplateModel.PolicyIssueTime = policy.PolicyIssuanceDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                        else
                            policyTemplateModel.PolicyIssueTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyExpireTime) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyExpireTime.Trim()))
                    {
                        if (policy.PolicyExpiryDate != null)
                            policyTemplateModel.PolicyExpireTime = policy.PolicyExpiryDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                        else
                            policyTemplateModel.PolicyExpireTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.PolicyEffectiveTime) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyEffectiveTime.Trim()))
                    {
                        if (policy.PolicyExpiryDate != null)
                            policyTemplateModel.PolicyEffectiveTime = policy.PolicyEffectiveDate?.ToString("HH:mm:tt", new CultureInfo("en-US"));
                        else
                            policyTemplateModel.PolicyEffectiveTime = DateTime.Now.ToString("HH:mm:tt", new CultureInfo("en-US"));
                    }


                    if (string.IsNullOrEmpty(policyTemplateModel.InsuranceStartDate) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceStartDate.Trim()))
                    {
                        policyTemplateModel.InsuranceStartDate = policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")); ;
                        policyTemplateModel.InsuranceStartDateDay = policy.PolicyEffectiveDate?.ToString("dddd");
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.InsuranceEndDate) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceEndDate.Trim()))
                    {
                        //policyTemplateModel.InsuranceEndDate = policy.PolicyEffectiveDate?.AddYears(1).AddDays(-1).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        //policyTemplateModel.InsuranceEndDateDay = policy.PolicyEffectiveDate?.AddYears(1).AddDays(-1).ToString("dddd");
                        policyTemplateModel.InsuranceEndDate = policy.PolicyExpiryDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.InsuranceEndDateDay = policy.PolicyExpiryDate?.ToString("dddd");
                    }
                    DateTime policyEffectiveDateH;
                    DateTime policyIssuanceDate;
                    DateTime PolicyExpiryDateH;
                    System.Globalization.DateTimeFormatInfo HijriDTFI;
                    HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                    HijriDTFI.Calendar = new System.Globalization.UmAlQuraCalendar();
                    HijriDTFI.ShortDatePattern = "dd/MM/yyyy";

                    if (policy.PolicyEffectiveDate.HasValue)
                    {
                        policyEffectiveDateH = policy.PolicyEffectiveDate.Value;
                        if (string.IsNullOrEmpty(policyTemplateModel.InsuranceStartDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceStartDateH.Trim()))
                            policyTemplateModel.InsuranceStartDateH = policyEffectiveDateH.ToString("dd-MM-yyyy", HijriDTFI); //policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")); ;
                        if (policy.PolicyExpiryDate.HasValue)
                        {
                            PolicyExpiryDateH = policy.PolicyExpiryDate.Value;
                            if (string.IsNullOrEmpty(policyTemplateModel.InsuranceEndDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuranceEndDateH.Trim()))
                                policyTemplateModel.InsuranceEndDateH = PolicyExpiryDateH.ToString("dd-MM-yyyy", HijriDTFI);//policy.PolicyEffectiveDate?.AddYears(1).AddDays(-1).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        }
                    }

                    if (policy.PolicyIssuanceDate.HasValue)
                    {
                        policyIssuanceDate = policy.PolicyIssuanceDate.Value;
                        if (string.IsNullOrEmpty(policyTemplateModel.PolicyIssueDateH) || string.IsNullOrWhiteSpace(policyTemplateModel.PolicyIssueDateH.Trim()))
                            policyTemplateModel.PolicyIssueDateH = policyIssuanceDate.ToString("dd-MM-yyyy", HijriDTFI); //policy.PolicyIssuanceDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                    }

                    if (policyTemplateModel.PolicyAdditionalCoversAr == null || policyTemplateModel.PolicyAdditionalCoversAr.Count < 1)
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {

                                if (benefit != null && benefit.Benefit != null)
                                {
                                    policyTemplateModel.PolicyAdditionalCoversAr.Add(benefit.Benefit.ArabicDescription);
                                }
                            }
                        }
                    }

                    if (policyTemplateModel.PolicyAdditionalCoversEn == null || policyTemplateModel.PolicyAdditionalCoversEn.Count < 1)
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null)
                                    policyTemplateModel.PolicyAdditionalCoversEn.Add(benefit.Benefit.EnglishDescription);
                            }
                        }
                    }

                    decimal _pACoverForDriverOnly = 0;
                    if (string.IsNullOrEmpty(policyTemplateModel.PACoverForDriverOnly) || string.IsNullOrWhiteSpace(policyTemplateModel.PACoverForDriverOnly.Trim()))
                    {
                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                            policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                        else
                            policyTemplateModel.PACoverForDriverOnly = "Not Included";

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null && (benefit.Benefit.Code == 1 || benefit.Benefit.Code == 2))
                                {
                                    _pACoverForDriverOnly = benefit.Price;

                                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                        policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    else
                                        policyTemplateModel.PACoverForDriverOnly = "Included";

                                    break;
                                }
                            }
                        }
                    }

                    if ((string.IsNullOrEmpty(policyTemplateModel.PACoverForDriverAndPassenger) || string.IsNullOrWhiteSpace(policyTemplateModel.PACoverForDriverAndPassenger.Trim())) && quotationResponse.InsuranceCompanyId != 4)
                    {
                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                            policyTemplateModel.PACoverForDriverAndPassenger = "غير مشمولة";
                        else
                            policyTemplateModel.PACoverForDriverAndPassenger = "Not Included";

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 2)
                                {
                                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                        policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                    else
                                        policyTemplateModel.PACoverForDriverAndPassenger = "Included";

                                    break;
                                }
                            }
                        }
                    }

                    decimal _passengerIncluded = 0;
                    if (string.IsNullOrEmpty(policyTemplateModel.PassengerIncluded) || string.IsNullOrWhiteSpace(policyTemplateModel.PassengerIncluded.Trim()))
                    {
                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                            policyTemplateModel.PassengerIncluded = "غير مشمولة";
                        else
                            policyTemplateModel.PassengerIncluded = "Not Included";

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 8)
                                {
                                    _passengerIncluded = benefit.Price;

                                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                        policyTemplateModel.PassengerIncluded = (_passengerIncluded > 0) ? _passengerIncluded.ToString() : "مشمولة";
                                    else
                                        policyTemplateModel.PassengerIncluded = (_passengerIncluded > 0) ? _passengerIncluded.ToString() : "Included";

                                    break;
                                }
                            }
                        }
                    }

                    if (vehicle != null)
                    {
                        policyTemplateModel.HasTrailerAr = "لا";
                        policyTemplateModel.HasTrailerEn = "No";
                        if (vehicle.HasTrailer)
                        {
                            policyTemplateModel.HasTrailerAr = "نعم";
                            policyTemplateModel.HasTrailerEn = "Yes";
                        }

                        policyTemplateModel.OtherUsesAr = "لا";
                        policyTemplateModel.OtherUsesEn = "No";
                        if (vehicle.OtherUses)
                        {
                            policyTemplateModel.OtherUsesAr = "نعم";
                            policyTemplateModel.OtherUsesEn = "Yes";
                        }

                        policyTemplateModel.VehicleModificationAr = "لا";
                        policyTemplateModel.VehicleModificationEn = "No";
                        if (vehicle.HasModifications)
                        {
                            policyTemplateModel.VehicleModificationAr = "نعم";
                            policyTemplateModel.VehicleModificationEn = "Yes";
                        }
                    }

                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                        policyTemplateModel.CoverforDriverBelow21Years = "غير مشمولة";
                    else
                        policyTemplateModel.CoverforDriverBelow21Years = "Not Included";

                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit != null && benefit.Benefit != null)
                            {
                                policyTemplateModel.SelectedBenfits += benefit.BenefitExternalId + ",";
                            }
                            if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 0 && benefit.BenefitExternalId == "22")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.CoverforDriverBelow21Years = "مشمولة";
                                else
                                    policyTemplateModel.CoverforDriverBelow21Years = "Included";
                                break;
                            }
                        }
                    }

                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                        policyTemplateModel.HireCarBenefit = "غير مشمولة";
                    else
                        policyTemplateModel.HireCarBenefit = "Not Included";

                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "6")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.HireCarBenefit = "مشمولة";
                                else
                                    policyTemplateModel.HireCarBenefit = "Included";
                                break;
                            }
                        }
                    }

                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                        policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                    else
                        policyTemplateModel.GCCCoverBenefit = "Not Included";

                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 0 && benefit.BenefitExternalId == "19")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                else
                                    policyTemplateModel.GCCCoverBenefit = "Included";
                                break;
                            }
                        }
                    }

                    if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                        policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                    else
                        policyTemplateModel.RoadsideAssistanceBenefit = "Not Included";

                    policyTemplateModel.HasRoadsideAssistanceBenefit = false;
                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 5)
                            {
                                policyTemplateModel.HasRoadsideAssistanceBenefit = true;
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Included";

                                break;
                            }
                        }
                    }
                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 1)
                            {
                                policyTemplateModel.DriverCovered = true;
                                continue;
                            }
                            if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 2)
                            {
                                policyTemplateModel.PassengerCovered = true;
                                continue;
                            }
                        }
                    }

                    var benefitList = new List<BenfitSummaryModel>();
                    var PAacidantList = new List<BenfitSummaryModel>();
                    var CarEndorsment120 = new List<BenfitSummaryModel>();
                    var CarEndorsment150 = new List<BenfitSummaryModel>();
                    var CarEndorsment180 = new List<BenfitSummaryModel>();
                    var RoadAssistanttList = new List<BenfitSummaryModel>();
                    var AccidentOutsideTerritory = new List<BenfitSummaryModel>();
                    var DeathPhysicalInjuriesList = new List<BenfitSummaryModel>();
                    var DeathPhysicalInjuriesListForPassengers = new List<BenfitSummaryModel>();
                    var CarEndorsmentList = new List<BenfitSummaryModel>();

                    // new benefits added by Mubarak & Felwa @27-12-2022
                    if (orderItem != null && orderItem.OrderItemBenefits != null)
                    {
                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit == null && benefit.Benefit == null)
                                continue;

                            BenfitSummaryModel benefitObject = new BenfitSummaryModel();

                            policyTemplateModel.DriverRelatedToInsured_Above21Ar = "غير مشمولة";
                            policyTemplateModel.DriverRelatedToInsured_Above21En = "Not Included";
                            policyTemplateModel.HireCar_1500_Max15DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_1500_Max15DaysEn = "Not Included";
                            policyTemplateModel.DriverRelatedToInsured_FamilyCoverAr = "غير مشمولة";
                            policyTemplateModel.DriverRelatedToInsured_FamilyCoverEn = "Not Included";
                            policyTemplateModel.HireCar_120_Max15DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_120_Max15DaysEn = "Not Included";
                            policyTemplateModel.HireCar_150_Max15DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_150_Max15DaysEn = "Not Included";
                            policyTemplateModel.HireCar_180_Max15DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_180_Max15DaysEn = "Not Included";
                            policyTemplateModel.HireCar_2000_Max20DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_2000_Max20DaysEn = "Not Included";
                            policyTemplateModel.HireCar_4000_Max20DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_4000_Max20DaysEn = "Not Included";
                            policyTemplateModel.HireCar_5000_Max20DaysAr = "غير مشمولة";
                            policyTemplateModel.HireCar_5000_Max20DaysEn = "Not Included";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "غير مشمولة";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Not Included";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersAr = "غير مشمولة";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersEn = "Not Included";
                            policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAAr = "غير مشمولة";
                            policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAEn = "Not Included";

                            if (benefit.Benefit.Code == 28)
                            {
                                policyTemplateModel.DriverRelatedToInsured_Above21Ar = "مشمولة";
                                policyTemplateModel.DriverRelatedToInsured_Above21En = "Included";
                            }
                            if (benefit.Benefit.Code == 29)
                            {
                                policyTemplateModel.HireCar_1500_Max15DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_1500_Max15DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 30)
                            {
                                policyTemplateModel.DriverRelatedToInsured_FamilyCoverAr = "مشمولة";
                                policyTemplateModel.DriverRelatedToInsured_FamilyCoverEn = "Included";
                            }
                            if (benefit.Benefit.Code == 31)
                            {
                                policyTemplateModel.HireCar_120_Max15DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_120_Max15DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 32)
                            {
                                policyTemplateModel.HireCar_150_Max15DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_150_Max15DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 33)
                            {
                                policyTemplateModel.HireCar_180_Max15DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_180_Max15DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 34)
                            {
                                policyTemplateModel.HireCar_2000_Max20DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_2000_Max20DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 35)
                            {
                                policyTemplateModel.HireCar_4000_Max20DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_4000_Max20DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 36)
                            {
                                policyTemplateModel.HireCar_5000_Max20DaysAr = "مشمولة";
                                policyTemplateModel.HireCar_5000_Max20DaysEn = "Included";
                            }
                            if (benefit.Benefit.Code == 37)
                            {
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "مشمولة";
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Included";
                            }
                            if (benefit.Benefit.Code == 38)
                            {
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersAr = "مشمولة";
                                policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForPassengersEn = "Included";
                            }
                            if (benefit.Benefit.Code == 39)
                            {
                                policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAAr = "مشمولة";
                                policyTemplateModel.CoverageForAccidentsOccurringOutsideTheTerritoryOfSAEn = "Included";
                            }

                            if (benefit.Benefit.Code == 24001)
                            {
                                policyTemplateModel.GE_Jordan_Lebanon_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.Benefit.Code == 24002)
                            {
                                policyTemplateModel.GCC_12Month = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.Benefit.Code == 24003)
                            {
                                policyTemplateModel.GE_Egypt_Sudan_Turky_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }
                            if (benefit.Benefit.Code == 24004)
                            {
                                policyTemplateModel.GCC_Jordan_Lebanon_Egypt_Sudan_12months = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Included" : "مشمولة";
                            }

                            if (quotationResponse.InsuranceCompanyId == 20)
                            {
                                if (benefit.BenefitExternalId == "1")
                                {
                                    benefitObject.BenefitDescriptionAr = "المسؤولية المدنية تجاه الغير بحد أقصى 10,000,000 ريال في كل حالة";
                                    benefitObject.BenefitDescriptionEn = "Third Party Liability Up to SR 10 min per one occurrence";
                                }
                                else if (benefit.BenefitExternalId == "2")
                                {
                                    benefitObject.BenefitDescriptionAr = "إعفاء من نسبة استهلاك قطع الغيار";
                                    benefitObject.BenefitDescriptionEn = "Zero depriciation spare parts";
                                }
                                else if (benefit.BenefitExternalId == "3")
                                {
                                    benefitObject.BenefitDescriptionAr = "تكاليف حالات الطوارئ الطبية";
                                    benefitObject.BenefitDescriptionEn = "Emergency Medical expenses";
                                }
                                if (benefit.BenefitExternalId == "4" && orderItem?.Product?.ProductNameEn == "Wafi Comprehensive")
                                {
                                    benefitObject.BenefitDescriptionAr = "التعویض بحسب قیمة المركبة السوقیة على أن لا تتعدى القیمة التامینة";
                                    benefitObject.BenefitDescriptionEn = "Market value not exceeding declared Sum Insured";
                                }
                                else if (benefit.BenefitExternalId == "5" && orderItem?.Product?.ProductNameEn == "Wafi Comprehensive")
                                {
                                    benefitObject.BenefitDescriptionAr = "تغطية الخسارة الكلية أو الجزئية للمركبة";
                                    benefitObject.BenefitDescriptionEn = "Coverage for total or partial loss of Vehicle";
                                }
                                else if (benefit.BenefitExternalId == "6")
                                {
                                    benefitObject.BenefitDescriptionAr = "الكوارث الطبيعية, الأمطار والبرد , والعواصف";
                                    benefitObject.BenefitDescriptionEn = "Natural Hazards rain, hail & flood";
                                }
                                else if (benefit.BenefitExternalId == "7")
                                {
                                    benefitObject.BenefitDescriptionAr = "تغطية الحوادث الشخصية للسائق و الركاب";
                                    benefitObject.BenefitDescriptionEn = "Personal Accident (Driver and Passengers)";
                                }
                                else if (benefit.BenefitExternalId == "8" && orderItem?.Product?.ProductNameEn == "Wafi Comprehensive")
                                {
                                    benefitObject.BenefitDescriptionAr = "المساعدة على الطريق";
                                    benefitObject.BenefitDescriptionEn = "Road Side Assistance";
                                }
                                else if (benefit.BenefitExternalId == "9")
                                {
                                    benefitObject.BenefitDescriptionAr = "التغطية سارية في دول الخليج";
                                    benefitObject.BenefitDescriptionEn = "Valid in GCC Countries";
                                }
                                else if (benefit.BenefitExternalId == "10")
                                {
                                    benefitObject.BenefitDescriptionAr = "استأجر سيارة";
                                    benefitObject.BenefitDescriptionEn = "Hire car";
                                }
                                else if (benefit.BenefitExternalId == "11")
                                {
                                    benefitObject.BenefitDescriptionAr = "تغطية السائق ذو صلة قرابة بالمؤمن له ( الأب, الأم, الزوج, الزوجة, الأبن, الابنة, الأخ, الأخت) أو السائق الذي يكون تحت كفالة المؤمن له, أو يعمل لدى المؤمن له بموجب عقد عمل";
                                    benefitObject.BenefitDescriptionEn = "Family cover - the driver related to the insured, such as parents, spouse, sons, daughters, brother and sister or the insured's domestic worker or someone who work for the insured based on a labor law";
                                }
                                
                                if (!string.IsNullOrEmpty(benefitObject.BenefitDescriptionAr) || !string.IsNullOrEmpty(benefitObject.BenefitDescriptionEn))
                                {
                                    benefitList.Add(benefitObject);
                                }
                            }

                            if (quotationResponse.InsuranceCompanyId == 27)
                            {
                                if (benefit.BenefitId == 34 || benefit.BenefitExternalId == "34")
                                {
                                    benefitObject.BenefitDescriptionAr = "استئجار سيارة بحد أقصى 2,000 ريال سعودي لكل مركبة، بحد اقصى 100 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يوم";
                                    benefitObject.BenefitDescriptionEn = "Hire car facility Up to Maximum limit of SR.2,000/- per vehicle at the rate of SR.100 per day, Maximum 20 days";
                                }
                                else if (benefit.BenefitId == 35 || benefit.BenefitExternalId == "35")
                                {
                                    benefitObject.BenefitDescriptionAr = " استئجار سيارة بحد أقصى 4,000 ريال سعودي لكل مركبة، بحد اقصى 200 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يو";
                                    benefitObject.BenefitDescriptionEn = " Hire car facility Up to Maximum limit of SR.4,000 / -per vehicle at the rate of SR.200 per day, Maximum 20 days";
                                }
                                else if (benefit.BenefitId == 36 || benefit.BenefitExternalId == "36")
                                {
                                    benefitObject.BenefitDescriptionAr = "استئجار سيارة بحد أقصى 5,000 ريال سعودي لكل مركبة، بحد اقصى 250 ريال سعودي في اليوم، وبمدة لا تتجاوز 20 يوم";
                                    benefitObject.BenefitDescriptionEn = "Hire car facility Up to Maximum limit of SR.5,000/- per vehicle at the rate of SR.250 per day, Maximum 20 days";
                                }

                                if (benefitObject != null && (!string.IsNullOrEmpty(benefitObject.BenefitDescriptionAr) || !string.IsNullOrEmpty(benefitObject.BenefitDescriptionEn)))
                                    CarEndorsmentList.Add(benefitObject);

                                if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "10")
                                    AccidentOutsideTerritory.Add(new BenfitSummaryModel());

                                if (benefit.BenefitId == 37 || benefit.BenefitExternalId == "37")
                                    DeathPhysicalInjuriesList.Add(new BenfitSummaryModel());

                                if (benefit.BenefitId == 38 || benefit.BenefitExternalId == "38")
                                    DeathPhysicalInjuriesListForPassengers.Add(new BenfitSummaryModel());

                                if (RoadAssistanttList.Count < 1 && (benefit.BenefitId == 34 || benefit.BenefitId == 35 || benefit.BenefitId == 36))
                                    RoadAssistanttList.Add(new BenfitSummaryModel());
                            }

                            if (quotationResponse.InsuranceCompanyId == 13)
                            {
                                policyTemplateModel.PersonalAccidentForDriverPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.PassengersPersonalAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.TowingWithoutAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.TowingWithAccidentPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.AuthorizedRepairLimitPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.PersonalBelongingsPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.ReplacementOfKeysPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.NonApplicationOfDepreciationInCaseOfTotalLossPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.NaturalHazardsBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.TheftFirePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.LiabilitiesToThirdPartiesPrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.RoadsideAssistanceBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.DeathInjuryMedic = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.GCCCoverBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";
                                policyTemplateModel.UnNamedDriver = (selectedLanguage == LanguageTwoLetterIsoCode.En) ? "Not Included" : "غير مشمولة";

                                if (orderItem != null && orderItem.OrderItemBenefits != null)
                                {
                                    decimal totalBenefitPrice = 0;
                                    foreach (var item in orderItem.OrderItemBenefits)
                                    {
                                        if (item.BenefitExternalId == "1")//1
                                        {
                                            policyTemplateModel.PersonalAccidentForDriverPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "2")//2
                                        {
                                            policyTemplateModel.PassengersPersonalAccidentPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "4")//4
                                        {
                                            policyTemplateModel.TowingWithoutAccidentPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "5")//5
                                        {
                                            policyTemplateModel.TowingWithAccidentPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "6")//6
                                        {
                                            policyTemplateModel.AuthorizedRepairLimitPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "7")//7
                                        {
                                            policyTemplateModel.PersonalBelongingsPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "8")//8
                                        {
                                            policyTemplateModel.ReplacementOfKeysPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "9")//9
                                        {
                                            policyTemplateModel.NonApplicationOfDepreciationInCaseOfTotalLossPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "23")//23
                                        {
                                            policyTemplateModel.ProvisionOfReplacementVehiclePrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "107")//107
                                        {
                                            policyTemplateModel.NaturalHazardsBenefit = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "109")//109
                                        {
                                            policyTemplateModel.TheftFirePrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "110")//110
                                        {
                                            policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "111")//111
                                        {
                                            policyTemplateModel.LiabilitiesToThirdPartiesPrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        //if (item.BenefitExternalId == "112")//108
                                        //{
                                        //    policyTemplateModel.ExpansionOfGeographicalScopeFrom = policy.PolicyEffectiveDate?.ToString("dd-MM-yyyy");
                                        //    policyTemplateModel.ExpansionOfGeographicalScopeTo = policy.PolicyEffectiveDate?.Date.AddMonths(1).ToString("dd-MM-yyyy");
                                        //    totalBenefitPrice += item.Price;
                                        //}
                                        if (item.BenefitExternalId == "135")//111
                                        {
                                            policyTemplateModel.ProvisionOfReplacementVehiclePrice = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "136")//111
                                        {
                                            policyTemplateModel.RoadsideAssistanceBenefit = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "137")//111
                                        {
                                            policyTemplateModel.DeathInjuryMedic = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "138")//111
                                        {
                                            policyTemplateModel.GCCCoverBenefit = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                        if (item.BenefitExternalId == "139")//111
                                        {
                                            policyTemplateModel.UnNamedDriver = item.Price.ToString("#.##");
                                            totalBenefitPrice += item.Price;
                                        }
                                    }
                                    policyTemplateModel.SumAdditionalBenefitPremiumtPrice = totalBenefitPrice.ToString("#.##");
                                }
                            }

                            if (quotationResponse.InsuranceCompanyId == 5)
                            {
                                policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                                policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                                policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Included";
                                policyTemplateModel.CarReplacement_150 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                                policyTemplateModel.CarReplacement_200 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";
                                policyTemplateModel.CarReplacement_75 = selectedLanguage == LanguageTwoLetterIsoCode.Ar ? "غير مشمولة" : "Not Included";

                                if (benefit.BenefitId == 1)
                                {
                                    //policyTemplateModel.PersonalAccidentBenefit = "مشمولة";
                                    //policyTemplateModel.PersonalAccidentBenefitEn = "Covered";
                                    policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                                }
                                else if (benefit.BenefitId == 9)
                                {
                                    policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                                }
                                else if (benefit.BenefitId == 10)
                                {
                                    policyTemplateModel.GCCCoverBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                                }

                                if (benefit.BenefitExternalId == "CARREP-150")
                                {
                                    policyTemplateModel.CarReplacement_150 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                                }
                                else if (benefit.BenefitExternalId == "CARREP-200")
                                {
                                    policyTemplateModel.CarReplacement_200 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                                }
                                else if (benefit.BenefitExternalId == "CARREP-75")
                                {
                                    policyTemplateModel.CarReplacement_75 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";
                                }
                            }
                        }

                        if (benefitList != null && benefitList.Count >= 1)
                            policyTemplateModel.BenefitSummary = benefitList;
                    }

                    //saico
                    if (quotationResponse.InsuranceCompanyId == 21)// saico additonal benefits
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null)
                                {
                                    if (benefit.BenefitId == 1)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "الحوادث الشخصية للسائق";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "Personal accidents of the driver";
                                    }
                                    if (benefit.BenefitId == 3)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "الاخطار الطبيعية";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "NATURAL PERIL";
                                    }
                                    if (benefit.BenefitId == 4)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "سرقة وحرائق";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "THIFT AND FIRE";
                                    }
                                    if (benefit.BenefitId == 7)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "اصلاح وكالة";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "AGENCY REPAIR";
                                    }
                                    if (benefit.BenefitId == 8)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "الحوادث الشخصية للركاب";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "Personal accidents of passengers";
                                    }
                                    if (benefit.BenefitId == 14)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "تغطية ضد الغير";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "Third Party coverage";
                                    }
                                    if (benefit.BenefitId == 16)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "تغطية شامل";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "Comprehensive Coverage";
                                    }
                                    if (benefit.BenefitId == 17)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "ضرر كامل";
                                        else
                                            policyTemplateModel.VehicleAgencyRepairBenfitEn += " - " + "TOTAL LOSS";
                                    }
                                    if (benefit.BenefitId == 18)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "ضرر جزئي";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "PARTIAL LOSS";
                                    }
                                    if (benefit.BenefitId == 19)
                                    {
                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                            policyTemplateModel.BenefitsList += " - " + "مصاريف طبية";
                                        else
                                            policyTemplateModel.BenefitsList += " - " + "MEDICAL EXPINCES";
                                    }
                                }
                            }
                        }
                    }

                    if (quotationResponse.InsuranceCompanyId == 23)// Tokio Marine
                    {                        if (orderItem != null && orderItem.OrderItemBenefits != null)                        {                            foreach (var benefit in orderItem.OrderItemBenefits)                            {                                if (benefit != null && benefit.Benefit != null)                                {                                    if (benefit.Benefit.Code == 6)                                    {                                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                            policyTemplateModel.AlternativeCarAr = "سيارة بديلة";                                        else                                            policyTemplateModel.AlternativeCarEn = "Hire Car";                                    }                                    else if (benefit.Benefit.Code == 0)                                    {                                        if (benefit.BenefitExternalId == "2101")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2101");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.GCC_1MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(1 شهر)";                                            else                                                policyTemplateModel.GCC_1MonthEn = "Geographical Extension-GCC (1Month)";                                        }                                        else if (benefit.BenefitExternalId == "2102")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2102");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.GCC_3MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(3 أشهر)";                                            else                                                policyTemplateModel.GCC_3MonthEn = "Geographical Extension-GCC (3Month)";                                        }                                        else if (benefit.BenefitExternalId == "2103")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2103");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.GCC_6MonthAr = "التغطية الجغرافية لدول مجلس التعاون الخليجي مع التحمل مضاعف(6 أشهر)";                                            else                                                policyTemplateModel.GCC_6MonthEn = "Geographical Extension-GCC (6Month)";                                        }                                        else if (benefit.BenefitExternalId == "2104")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2104");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.NONGCC_1MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (1 شهر)";                                            else                                                policyTemplateModel.NONGCC_1MonthEn = "Geographical Extension-Non GCC (1Month)";                                        }                                        else if (benefit.BenefitExternalId == "2105")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2105");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.NONGCC_3MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (3 أشهر)";                                            else                                                policyTemplateModel.NONGCC_3MonthEn = "Geographical Extension-Non GCC (3Month)";                                        }                                        else if (benefit.BenefitExternalId == "2106")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2106");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.NONGCC_6MonthAr = "التغطية الجغرافية لدول (مصر,السودان,الأردن) مع تحمل مضاعف (6 أشهر)";                                            else                                                policyTemplateModel.NONGCC_6MonthEn = "Geographical Extension-Non GCC (6Month)";                                        }                                        else if (benefit.BenefitExternalId == "2096")                                        {
                                            //var product_benfit = orderItem?.Product?.Product_Benefits.FirstOrDefault(a => a.BenefitId == 0 && a.BenefitExternalId == "2096");

                                            if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)                                                policyTemplateModel.NONDEPRECIATIONCOVERAr = "عدم تطبيق شرط نسبة الاستهلاك في الخسارة الكلية";                                            else                                                policyTemplateModel.NONDEPRECIATIONCOVEREn = "NON DEPRECIATION COVER";                                        }                                    }                                }                            }                        }                    }

                    if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                    {
                        if (quotationResponse.InsuranceCompanyId == 12)
                        {
                            if (invoice.TotalPrice.HasValue && invoice.Vat.HasValue)
                                policyTemplateModel.VatableAmount = Math.Round(invoice.TotalPrice.Value - invoice.Vat.Value, 2).ToString();

                            if (orderItem != null && orderItem.OrderItemBenefits != null)
                            {
                                List<string> _selectedBenefits = new List<string>();
                                foreach (var benefit in orderItem.OrderItemBenefits)
                                {
                                    if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "2")
                                        _selectedBenefits.Add("1");
                                    else if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "6")
                                        _selectedBenefits.Add("3");
                                    else if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "10")
                                        _selectedBenefits.Add("2");
                                }

                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join(",", _selectedBenefits);
                            }
                        }
                        else if (quotationResponse.InsuranceCompanyId == 14)
                        {
                            if (orderItem != null && orderItem.OrderItemBenefits != null)
                            {
                                List<string> _selectedBenefits = new List<string>();
                                foreach (var benefit in orderItem.OrderItemBenefits)
                                {
                                    if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "MNHS")
                                        _selectedBenefits.Add("Natural Hazards");
                                    else if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "MME")
                                        _selectedBenefits.Add("Medical Expenses");
                                    else if (benefit != null && benefit.Benefit != null && benefit.BenefitExternalId == "MTB")
                                        _selectedBenefits.Add("Towing Benefit");
                                }

                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join("\n", _selectedBenefits);
                            }
                        }
                        else if (quotationResponse.InsuranceCompanyId == 23) //Tokio
                        {
                            if (orderItem != null && orderItem.OrderItemBenefits != null)
                            {
                                List<string> _selectedBenefits = new List<string>();
                                List<string> _geoGraphicalBenefits = new List<string>();
                                foreach (var benefit in orderItem.OrderItemBenefits)
                                {
                                    if (benefit != null && benefit.Benefit != null)
                                    {
                                        _selectedBenefits.Add(benefit.Benefit.ArabicDescription);

                                        if (benefit.BenefitId == 9 || benefit.BenefitId == 10 || benefit.BenefitId == 11) // Geo-graphical benefits
                                        {
                                            _geoGraphicalBenefits.Add(benefit.Benefit.ArabicDescription);
                                        }
                                    }
                                }

                                _selectedBenefits.Sort((x, y) => string.Compare(x, y));
                                policyTemplateModel.SelectedBenfits = string.Join(", ", _selectedBenefits);
                                policyTemplateModel.GeoGraphicalBenefits = string.Join(", ", _geoGraphicalBenefits);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.TotalPremium) || string.IsNullOrWhiteSpace(policyTemplateModel.TotalPremium.Trim()))
                    {
                        // policyTemplateModel.TotalPremium = orderItem?.Price.ToString("#.##");
                        policyTemplateModel.TotalPremium = totalPrice.HasValue ? totalPrice.Value.ToString("#.##") : "0.00";
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.OfficePremium) || string.IsNullOrWhiteSpace(policyTemplateModel.OfficePremium.Trim()))
                        policyTemplateModel.OfficePremium = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.VATAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.VATAmount.Trim()))
                    {
                        //policyTemplateModel.VATAmount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 8).FirstOrDefault()?.PriceValue.ToString("#.##");
                        policyTemplateModel.VATAmount = vat.HasValue ? vat.Value.ToString("#.##") : "0.00";
                    }

                    if (!string.IsNullOrEmpty(policyTemplateModel.OfficePremium))
                    {
                        var _vat = (vat.HasValue) ? vat.Value : 0;
                        decimal officePremium = 0;
                        if (decimal.TryParse(policyTemplateModel.OfficePremium, out officePremium))
                        {
                            decimal officePremiumWithoutVat = officePremium - _vat;
                            policyTemplateModel.OfficePremiumWithoutVAT = officePremiumWithoutVat.ToString("#.##");
                        }
                        else
                        {
                            policyTemplateModel.OfficePremiumWithoutVAT = "0.00";

                        }
                        if (!string.IsNullOrEmpty(policyTemplateModel.TotalPremium) && totalPrice.HasValue)
                        {
                            decimal totalPremiumWithoutVAT = totalPrice.Value - _vat;
                            policyTemplateModel.TotalPremiumWithoutVAT = totalPremiumWithoutVAT.ToString("#.##");
                        }
                        else
                        {
                            policyTemplateModel.TotalPremiumWithoutVAT = "0.00";

                        }

                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VATPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.VATPercentage.Trim()))
                        policyTemplateModel.VATPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 8).FirstOrDefault()?.PercentageValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SpecialDiscount.Trim()))
                    {
                        policyTemplateModel.SpecialDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue.ToString("#.##");
                        policyTemplateModel.SpecialDiscountPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PercentageValue?.ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2) || string.IsNullOrWhiteSpace(policyTemplateModel.SpecialDiscount2.Trim()))
                    {
                        policyTemplateModel.SpecialDiscount2 = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 12).FirstOrDefault()?.PriceValue.ToString("#.##");
                        policyTemplateModel.SpecialDiscount2Percentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 12).FirstOrDefault()?.PercentageValue?.ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.SchemesDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SchemesDiscount.Trim()))//Voucher Discount 
                    {
                        policyTemplateModel.SchemesDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 11).FirstOrDefault()?.PriceValue.ToString("#.##");
                        policyTemplateModel.SchemesDiscountPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 11).FirstOrDefault()?.PercentageValue?.ToString();
                    }

                    var _benfitCode2 = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault();
                    if (string.IsNullOrEmpty(policyTemplateModel.NoClaimDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.NoClaimDiscount.Trim()))
                        policyTemplateModel.NoClaimDiscount = (_benfitCode2 != null && _benfitCode2.PercentageValue > 0) ? _benfitCode2.PercentageValue.Value.ToString("#.##") : "0.00";

                    if (string.IsNullOrEmpty(policyTemplateModel.NCDAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDAmount.Trim()))
                        policyTemplateModel.NCDAmount = (_benfitCode2 != null && _benfitCode2.PriceValue > 0) ? _benfitCode2.PriceValue.ToString("#.##") : "0.00";

                    if (string.IsNullOrEmpty(policyTemplateModel.NCDPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDPercentage.Trim()))
                    {
                        policyTemplateModel.NCDPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString()?.Replace(".00", string.Empty).Trim();
                        if (string.IsNullOrEmpty(policyTemplateModel.NCDPercentage))
                        {
                            policyTemplateModel.NCDPercentage = "0";
                        }
                    }

                    var _benfitCode3 = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault();
                    if (string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.LoyaltyDiscount.Trim()))
                        policyTemplateModel.LoyaltyDiscount = (_benfitCode3 != null && _benfitCode3.PriceValue > 0) ? _benfitCode3.PriceValue.ToString("#.##") : "0.00";
                    //policyTemplateModel.LoyaltyDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscountPercentage) || string.IsNullOrWhiteSpace(policyTemplateModel.LoyaltyDiscountPercentage.Trim()))
                        policyTemplateModel.LoyaltyDiscountPercentage = (_benfitCode3 != null && _benfitCode3.PercentageValue > 0) ? _benfitCode3.PercentageValue.ToString() : "0.00";
                    //policyTemplateModel.LoyaltyDiscountPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PercentageValue?.ToString("#.##");

                    if (string.IsNullOrEmpty(policyTemplateModel.TPLCommissionAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.TPLCommissionAmount.Trim())
                        || string.IsNullOrEmpty(policyTemplateModel.ComprehesiveCommissionAmount) || string.IsNullOrWhiteSpace(policyTemplateModel.ComprehesiveCommissionAmount.Trim()))
                    {
                        var specialDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue;
                        var noClaimDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue;
                        var loyaltyDiscount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue;
                        var additionaLoading = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 4).FirstOrDefault()?.PriceValue;
                        var additionalAgeContribution = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 5).FirstOrDefault()?.PriceValue;
                        var adminFees = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 6).FirstOrDefault()?.PriceValue;
                        var basicPremium = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue;

                        var productPriceWithouVaT = basicPremium != null && basicPremium.HasValue ? basicPremium.Value : 0
                            + adminFees != null && adminFees.HasValue ? adminFees.Value : 0
                            + additionalAgeContribution != null && additionalAgeContribution.HasValue ? additionalAgeContribution.Value : 0
                            + additionaLoading != null && additionaLoading.HasValue ? additionaLoading.Value : 0
                            - loyaltyDiscount != null && loyaltyDiscount.HasValue ? loyaltyDiscount.Value : 0
                            - noClaimDiscount != null && noClaimDiscount.HasValue ? noClaimDiscount.Value : 0
                            - specialDiscount != null && specialDiscount.HasValue ? specialDiscount.Value : 0;

                        decimal totalPremium = 0;
                        decimal.TryParse(policyTemplateModel.TotalPremium, out totalPremium);

                        policyTemplateModel.TPLCommissionAmount = (totalPremium * 2 / 100).ToString("#.##");
                        policyTemplateModel.ComprehesiveCommissionAmount = (totalPremium * 15 / 100).ToString("#.##");
                        if (orderItem?.Product?.QuotationResponse?.InsuranceTypeCode != null)
                        {
                            if (orderItem?.Product?.QuotationResponse?.InsuranceTypeCode.Value == 1)
                                policyTemplateModel.CommissionPaid = policyTemplateModel.TPLCommissionAmount;
                            else
                                policyTemplateModel.CommissionPaid = policyTemplateModel.ComprehesiveCommissionAmount;

                        }
                        policyTemplateModel.TPLTotalSubscriptionPremium = ((totalPremium * 2 / 100) + totalPremium).ToString();
                        policyTemplateModel.ComprehensiveTotalSubscriptionPremium = ((totalPremium * 15 / 100) + totalPremium).ToString();
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredCity) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredCity.Trim()))
                        policyTemplateModel.InsuredCity = mainDriverAddress?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredDisctrict) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredDisctrict.Trim()))
                        policyTemplateModel.InsuredDisctrict = mainDriverAddress?.District;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredID) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredID.Trim()))
                        policyTemplateModel.InsuredID = insured.NationalId;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredMobile) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredMobile.Trim()))
                        policyTemplateModel.InsuredMobile = checkoutDetails.Phone;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredIbanNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredIbanNumber.Trim()))
                        policyTemplateModel.InsuredIbanNumber = checkoutDetails.IBAN;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredBankName) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredBankName.Trim()))
                        policyTemplateModel.InsuredBankName = checkoutDetails.BankCode?.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameAr) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameAr.Trim()))
                        policyTemplateModel.InsuredNameAr = $"{insured.FirstNameAr} {insured.MiddleNameAr} {insured.LastNameAr}";

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredNameEn) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNameEn.Trim()))
                        policyTemplateModel.InsuredNameEn = $"{insured.FirstNameEn} {insured.MiddleNameEn} {insured.LastNameEn}";

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredStreet) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredStreet.Trim()))
                        policyTemplateModel.InsuredStreet = mainDriverAddress?.Street;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredZipCode) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredZipCode.Trim()))
                        policyTemplateModel.InsuredZipCode = mainDriverAddress?.PostCode;

                    if (string.IsNullOrEmpty(policyTemplateModel.InsuredNationalAddress) || string.IsNullOrWhiteSpace(policyTemplateModel.InsuredNationalAddress.Trim()))
                    {
                        policyTemplateModel.InsuredNationalAddress = string.Empty;
                        policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredBuildingNo) ? policyTemplateModel.InsuredBuildingNo + " " : string.Empty;
                        policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredStreet) ? policyTemplateModel.InsuredStreet + " " : string.Empty;
                        policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredDisctrict) ? policyTemplateModel.InsuredDisctrict + " " : string.Empty;
                        policyTemplateModel.InsuredNationalAddress += !string.IsNullOrEmpty(policyTemplateModel.InsuredCity) ? policyTemplateModel.InsuredCity : string.Empty;
                        policyTemplateModel.InsuredNationalAddress.TrimEnd();
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleBodyAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleBodyAr.Trim()))
                        policyTemplateModel.VehicleBodyAr = vehicle.VehicleBodyType.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleBodyEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleBodyEn.Trim()))
                        policyTemplateModel.VehicleBodyEn = vehicle.VehicleBodyType.EnglishDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleChassis) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleChassis.Trim()))
                        policyTemplateModel.VehicleChassis = vehicle.ChassisNumber;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleColorAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleColorAr.Trim()))
                        policyTemplateModel.VehicleColorAr = vehicle.MajorColor;
                    // this code get arabic color value only
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleColorEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleColorEn.Trim()))
                        policyTemplateModel.VehicleColorEn = vehicle.MajorColor;

                    if (quotationResponse.InsuranceCompanyId == 4 || quotationResponse.InsuranceCompanyId == 24)
                    {
                        string vColor = string.Empty;
                        GetVehicleColor(out vColor, vehicle.MajorColor, quotationResponse.InsuranceCompanyId);
                        policyTemplateModel.VehicleColorEn = vColor;
                    }


                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleCustomNo) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleCustomNo.Trim()))
                        policyTemplateModel.VehicleCustomNo = vehicle.CustomCardNumber;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleMakeAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleMakeAr.Trim()))
                        policyTemplateModel.VehicleMakeAr = vehicle.VehicleMaker;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleMakeEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleMakeEn.Trim()))
                        policyTemplateModel.VehicleMakeEn = vehicle.VehicleMaker;

                    // this code get arabic VehicleModel value only
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModelAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModelAr.Trim()))
                        policyTemplateModel.VehicleModelAr = vehicle.VehicleModel;
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModelEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModelEn.Trim()))
                        policyTemplateModel.VehicleModelEn = vehicle.VehicleModel;

                    if (quotationResponse.InsuranceCompanyId == 4 && quotationResponse.QuotationRequest.Vehicle.VehicleMakerCode > 0 && quotationResponse.QuotationRequest.Vehicle.VehicleBodyCode > 0)
                    {
                        var vehicle_maker = _vehicleService.GetMakerDetails((int)quotationResponse.QuotationRequest.Vehicle?.VehicleMakerCode);
                        if (vehicle_maker != null)
                        {
                            policyTemplateModel.VehicleMakeAr = vehicle_maker.ArabicDescription;
                            policyTemplateModel.VehicleMakeEn = vehicle_maker.EnglishDescription;
                        }

                        if (quotationResponse.QuotationRequest.Vehicle.VehicleModelCode > 0)
                        {
                            var vehicle_model = _vehicleService.GetVehicleModelByMakerCodeAndModelCode((short)(quotationResponse.QuotationRequest.Vehicle?.VehicleMakerCode), (int)quotationResponse.QuotationRequest.Vehicle.VehicleModelCode);
                            if (vehicle_model != null)
                            {
                                policyTemplateModel.VehicleModelAr = vehicle_model.ArabicDescription;
                                policyTemplateModel.VehicleModelEn = vehicle_model.EnglishDescription;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleWeight) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleWeight.Trim()))
                        policyTemplateModel.VehicleWeight = vehicle.VehicleWeight.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleLoad) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleLoad.Trim()))
                        policyTemplateModel.VehicleLoad = vehicle.VehicleLoad.ToString();

                    var vehicleUseId = vehicle.VehicleUseId;
                    var vehicleUseEnumKey = (vehicleUseId > 0) ? Enum.GetName(typeof(VehicleUse), vehicleUseId) : String.Empty;
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleUsingPurposesAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleUsingPurposesAr.Trim()))
                    {
                        //policyTemplateModel.VehicleUsingPurposesAr = vehicle.VehicleUse.ToString();
                        policyTemplateModel.VehicleUsingPurposesAr = (!string.IsNullOrEmpty(vehicleUseEnumKey)) ? VehicleUseResource.ResourceManager.GetString(vehicleUseEnumKey, CultureInfo.GetCultureInfo("ar-SA")) : "";
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleUsingPurposesEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleUsingPurposesEn.Trim()))
                    {
                        //policyTemplateModel.VehicleUsingPurposesEn = vehicle.VehicleUse.ToString();
                        policyTemplateModel.VehicleUsingPurposesEn = (!string.IsNullOrEmpty(vehicleUseEnumKey)) ? VehicleUseResource.ResourceManager.GetString(vehicleUseEnumKey, CultureInfo.GetCultureInfo("en-SA")) : "";
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleEngineSize) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleEngineSize.Trim()))
                        policyTemplateModel.VehicleEngineSize = vehicle.EngineSize?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOdometerReading) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOdometerReading.Trim()))
                        policyTemplateModel.VehicleOdometerReading = vehicle.CurrentMileageKM?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleModificationDetails.Trim()))
                        policyTemplateModel.VehicleModificationDetails = vehicle.ModificationDetails;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOwnerID) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOwnerID.Trim()))
                        policyTemplateModel.VehicleOwnerID = vehicle.CarOwnerNIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOwnerName) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOwnerName.Trim()))
                        policyTemplateModel.VehicleOwnerName = vehicle.CarOwnerName;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoAr.Trim()))
                        policyTemplateModel.VehiclePlateNoAr = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoEn.Trim()))
                        policyTemplateModel.VehiclePlateNoEn = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateText) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateText.Trim()))
                    {
                        if (!string.IsNullOrEmpty(vehicle.CarPlateText3))
                        {
                            policyTemplateModel.VehiclePlateText = vehicle.CarPlateText3 + " " + vehicle.CarPlateText2 + " " + vehicle.CarPlateText1;
                        }
                        else
                        {
                            policyTemplateModel.VehiclePlateText = vehicle.CarPlateText2 + " " + vehicle.CarPlateText1;
                        }
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateNoEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateNoEn.Trim()))
                        policyTemplateModel.VehiclePlateNoEn = vehicle.CarPlateNumber?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateTypeAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateTypeAr.Trim()))
                        policyTemplateModel.VehiclePlateTypeAr = vehicle?.VehiclePlateType?.ArabicDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehiclePlateTypeEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehiclePlateTypeEn.Trim()))
                        policyTemplateModel.VehiclePlateTypeEn = vehicle?.VehiclePlateType?.EnglishDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRegistrationExpiryDate) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRegistrationExpiryDate.Trim()))
                        policyTemplateModel.VehicleRegistrationExpiryDate = vehicle?.LicenseExpiryDate;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleSequenceNo) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleSequenceNo.Trim()))
                        policyTemplateModel.VehicleSequenceNo = vehicle?.SequenceNumber;

                    ////
                    /// as per Rawan https://bcare.atlassian.net/browse/VW-853
                    //if (string.IsNullOrEmpty(policyTemplateModel.VehicleValue) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleValue.Trim()))
                    policyTemplateModel.VehicleValue = vehicle.VehicleValue?.ToString();
                    policyTemplateModel.VehicleLimitValue = (orderItem.Product?.VehicleLimitValue.HasValue == true && orderItem.Product?.VehicleLimitValue.Value > 0)
                                                                ? orderItem.Product?.VehicleLimitValue.Value.ToString()
                                                                : vehicle.VehicleValue?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRegistrationType) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRegistrationType.Trim()))
                        policyTemplateModel.VehicleRegistrationType = vehicle?.VehiclePlateType?.ArabicDescription;
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRegistrationTypeEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRegistrationTypeEn.Trim()))
                        policyTemplateModel.VehicleRegistrationTypeEn = vehicle?.VehiclePlateType?.EnglishDescription;

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleYear) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleYear.Trim()))
                        policyTemplateModel.VehicleYear = vehicle?.ModelYear?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.DeductibleValue) || string.IsNullOrWhiteSpace(policyTemplateModel.DeductibleValue.Trim()))
                    {
                        if (orderItem.Product != null && orderItem.Product.DeductableValue.HasValue && orderItem.Product.DeductableValue.Value == 0 && quotationResponse.InsuranceCompanyId==13)
                        {
                            policyTemplateModel.DeductibleValue = "0"; //quotationResponse?.DeductibleValue?.ToString();
                        }
                        else if (orderItem.Product != null && orderItem.Product.DeductableValue.HasValue && orderItem.Product.DeductableValue.Value > 0)
                        {
                            policyTemplateModel.DeductibleValue = orderItem.Product.DeductableValue.Value.ToString(); //quotationResponse?.DeductibleValue?.ToString();
                        }
                        else
                        {
                            policyTemplateModel.DeductibleValue = quotationResponse?.DeductibleValue?.ToString();

                        }
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleAgencyRepairEn) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleAgencyRepairEn.Trim()))
                    {
                        if (quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true)
                        {
                            policyTemplateModel.VehicleAgencyRepairEn = "Agency Repair";

                            policyTemplateModel.VehicleAgencyRepairBenfitEn = "Included";
                            policyTemplateModel.VehicleAgencyRepairValue = "1500 SR";
                        }
                        else
                        {
                            policyTemplateModel.VehicleAgencyRepairBenfitEn = "Not Included";
                            policyTemplateModel.VehicleAgencyRepairEn = "Workshop repair approved by the Company";
                            policyTemplateModel.VehicleAgencyRepairValue = "750 SR";
                        }

                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleAgencyRepairAr) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleAgencyRepairAr.Trim()))
                    {
                        if (quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true)
                        {
                            policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                            policyTemplateModel.VehicleAgencyRepairAr = "لدى الوكالة";
                            policyTemplateModel.VehicleAgencyRepairValue = "1500 ريال";
                        }
                        else
                        {
                            policyTemplateModel.VehicleAgencyRepairBenfit = "غير مشمولة";
                            policyTemplateModel.VehicleAgencyRepairAr = "لدى الورش المعتمدة من قبل الشركة";
                            policyTemplateModel.VehicleAgencyRepairValue = "750 ريال";
                        }

                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleRepairMethod) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleRepairMethod.Trim()))
                    {
                        if (quotationResponse.VehicleAgencyRepair.HasValue)
                        {
                            policyTemplateModel.VehicleRepairMethod = quotationResponse.VehicleAgencyRepair.Value ? "Agency" : "Workshop";
                        }
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.VehicleOvernightParkingLocationCode) || string.IsNullOrWhiteSpace(policyTemplateModel.VehicleOvernightParkingLocationCode.Trim()))
                    {
                        var parkingLocationId = vehicle.ParkingLocationId.Value;
                        var parkingLocationEnumKey = (parkingLocationId > 0) ? Enum.GetName(typeof(ParkingLocation), parkingLocationId) : String.Empty;
                        policyTemplateModel.VehicleOvernightParkingLocationCode = (!string.IsNullOrEmpty(parkingLocationEnumKey)) ? ParkingLocationResource.ResourceManager.GetString(parkingLocationEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.TransmissionType) || string.IsNullOrWhiteSpace(policyTemplateModel.TransmissionType.Trim()))
                    {
                        var transmissionTypeId = vehicle.TransmissionTypeId.Value;
                        var transmissionTypeEnumKey = (transmissionTypeId > 0) ? Enum.GetName(typeof(TransmissionType), transmissionTypeId) : String.Empty;
                        policyTemplateModel.TransmissionType = (!string.IsNullOrEmpty(transmissionTypeEnumKey)) ? TransmissionTypeResource.ResourceManager.GetString(transmissionTypeEnumKey, CultureInfo.GetCultureInfo(stringLang)) : "";
                    }

                    if (quotationResponse.InsuranceCompanyId == 20)
                    {
                        if (quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true)
                            policyTemplateModel.VehicleAgencyRepairAr = "تغطية شامل اصلاح داخل و خارج الوكالة";
                        else
                            policyTemplateModel.VehicleAgencyRepairAr = "تغطية شامل اصلاح خارج الوكالة";

                        if (insured.NationalId.StartsWith("7"))
                            policyTemplateModel.OwnerAsDriver = insured.NationalId;
                        else if (insured.NationalId == mainDriver.NIN)
                            policyTemplateModel.OwnerAsDriver = "YES";
                        else
                            policyTemplateModel.OwnerAsDriver = "NO";

                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.NCDFreeYears) || string.IsNullOrWhiteSpace(policyTemplateModel.NCDFreeYears.Trim()))
                        policyTemplateModel.NCDFreeYears = quotationResponse.QuotationRequest.NajmNcdFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverName.Trim()))
                        policyTemplateModel.MainDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? mainDriver?.FullArabicName : mainDriver?.FullEnglishName;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverIDNumber.Trim()))
                        policyTemplateModel.MainDriverIDNumber = mainDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverGender.Trim()))
                        policyTemplateModel.MainDriverGender = mainDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverDateofBirth.Trim()))
                    {
                        policyTemplateModel.MainDriverDateofBirth = mainDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.MainDriverDateofBirthH = mainDriver?.DateOfBirthG.ToString("dd-MM-yyyy", HijriDTFI);
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.MainDriverNumberofyearseligiblefor = mainDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.MainDriverNoClaimsDiscount = mainDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.MainDriverResidentialAddressCity = mainDriver?.Addresses?.FirstOrDefault()?.City;
                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverResidentialAddressCity))
                    {
                        policyTemplateModel.MainDriverResidentialAddressCity = mainDriverAddress.City;
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.MainDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.MainDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.MainDriverFrequencyofDrivingVehicle = mainDriver?.DrivingPercentage?.ToString();

                    if (!policyTemplateModel.EducationCode.HasValue)
                        policyTemplateModel.EducationCode = mainDriver?.EducationId;

                    if (string.IsNullOrEmpty(policyTemplateModel.Education) || string.IsNullOrWhiteSpace(policyTemplateModel.Education.Trim()))
                        policyTemplateModel.Education = mainDriver?.Education.ToString();

                    if (!policyTemplateModel.OccupationCode.HasValue)
                        policyTemplateModel.OccupationCode = mainDriver?.OccupationId;

                    if (string.IsNullOrEmpty(policyTemplateModel.Occupation) || string.IsNullOrWhiteSpace(policyTemplateModel.Occupation.Trim()))
                    {
                        //policyTemplateModel.Occupation = mainDriver?.Occupation?.NameAr;
                        policyTemplateModel.Occupation = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? mainDriver?.OccupationName : mainDriver.Occupation.NameEn;
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.SocialStatus) || string.IsNullOrWhiteSpace(policyTemplateModel.SocialStatus.Trim()))
                    {
                        //policyTemplateModel.SocialStatus = mainDriver?.SocialStatus.ToString();
                        var socialStatusCode = mainDriver?.SocialStatusId;
                        var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                        policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo(lang));
                    }

                    if (!policyTemplateModel.SocialStatusCode.HasValue)
                        policyTemplateModel.SocialStatusCode = mainDriver?.SocialStatusId;

                    if (string.IsNullOrEmpty(policyTemplateModel.AdditionalNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.AdditionalNumber.Trim()))
                        policyTemplateModel.AdditionalNumber = mainDriver?.Addresses.FirstOrDefault()?.AdditionalNumber;
                    if (string.IsNullOrEmpty(policyTemplateModel.AdditionalNumber))
                    {
                        policyTemplateModel.AdditionalNumber = mainDriverAddress.AdditionalNumber;
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.UnitNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.UnitNumber.Trim()))
                        policyTemplateModel.UnitNumber = mainDriver?.Addresses.FirstOrDefault()?.UnitNumber;
                    if (string.IsNullOrEmpty(policyTemplateModel.UnitNumber))
                    {
                        policyTemplateModel.UnitNumber = mainDriverAddress.UnitNumber;
                    }

                    if (string.IsNullOrEmpty(policyTemplateModel.District) || string.IsNullOrWhiteSpace(policyTemplateModel.District.Trim()))
                        policyTemplateModel.District = mainDriver?.Addresses.FirstOrDefault()?.District;
                    if (string.IsNullOrEmpty(policyTemplateModel.District))
                    {
                        policyTemplateModel.District = mainDriverAddress.District;
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.RegionName) || string.IsNullOrWhiteSpace(policyTemplateModel.RegionName.Trim()))
                        policyTemplateModel.RegionName = mainDriver?.Addresses.FirstOrDefault()?.RegionName;
                    if (string.IsNullOrEmpty(policyTemplateModel.RegionName))
                    {
                        policyTemplateModel.RegionName = mainDriverAddress.RegionName;
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.Email) || string.IsNullOrWhiteSpace(policyTemplateModel.Email.Trim()))
                        policyTemplateModel.Email = checkoutDetails?.Email;
                    if (!policyTemplateModel.NationalityId.HasValue)
                        policyTemplateModel.NationalityId = mainDriver?.NationalityCode;
                    if (string.IsNullOrEmpty(policyTemplateModel.Nationality) || string.IsNullOrWhiteSpace(policyTemplateModel.Nationality.Trim()))
                    {
                        if (mainDriver.NationalityCode.HasValue)
                        {
                            var nationality = _addressService.GetNationality(mainDriver.NationalityCode.Value);
                            if (nationality != null)
                            {
                                policyTemplateModel.Nationality = checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En ? nationality.EnglishDescription : nationality.ArabicDescription;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(mainDriver?.MobileNumber) && (string.IsNullOrEmpty(policyTemplateModel.LesseeMobileNo) || string.IsNullOrWhiteSpace(policyTemplateModel.LesseeMobileNo.Trim())))
                        policyTemplateModel.LesseeMobileNo = Utilities.ValidatePhoneNumber(mainDriver?.MobileNumber);

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverName.Trim()))
                        policyTemplateModel.SecondDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? secondDriver?.FullArabicName : secondDriver?.FullEnglishName;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverIDNumber.Trim()))
                        policyTemplateModel.SecondDriverIDNumber = secondDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverGender.Trim()))
                        policyTemplateModel.SecondDriverGender = secondDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverDateofBirth.Trim()))
                    {
                        policyTemplateModel.SecondDriverDateofBirth = secondDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.SecondDriverAge = secondDriver?.DateOfBirthG.GetUserAge().ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.SecondDriverNumberofyearseligiblefor = secondDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.SecondDriverNoClaimsDiscount = secondDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.SecondDriverResidentialAddressCity = secondDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.SecondDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.SecondDriverFrequencyofDrivingVehicle = secondDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverName.Trim()))
                        policyTemplateModel.ThirdDriverName = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? thirdDriver?.FullArabicName : thirdDriver?.FullEnglishName;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverIDNumber.Trim()))
                        policyTemplateModel.ThirdDriverIDNumber = thirdDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverGender.Trim()))
                        policyTemplateModel.ThirdDriverGender = thirdDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverDateofBirth.Trim()))
                    {
                        policyTemplateModel.ThirdDriverDateofBirth = thirdDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.ThirdDriverAge = thirdDriver?.DateOfBirthG.GetUserAge().ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.ThirdDriverNumberofyearseligiblefor = thirdDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.ThirdDriverNoClaimsDiscount = thirdDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.ThirdDriverResidentialAddressCity = thirdDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.ThirdDriverFrequencyofDrivingVehicle = thirdDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverName.Trim()))
                        policyTemplateModel.FourthDriverName = fourthDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverIDNumber.Trim()))
                        policyTemplateModel.FourthDriverIDNumber = fourthDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverGender.Trim()))
                        policyTemplateModel.FourthDriverGender = fourthDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverDateofBirth.Trim()))
                    {
                        policyTemplateModel.FourthDriverDateofBirth = fourthDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.FourthDriverAge = fourthDriver?.DateOfBirthG.GetUserAge().ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.FourthDriverNumberofyearseligiblefor = fourthDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.FourthDriverNoClaimsDiscount = fourthDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.FourthDriverResidentialAddressCity = fourthDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.FourthDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.FourthDriverFrequencyofDrivingVehicle = fourthDriver?.DrivingPercentage?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverName) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverName.Trim()))
                        policyTemplateModel.FifthDriverName = fifthDriver?.FullArabicName;

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverIDNumber) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverIDNumber.Trim()))
                        policyTemplateModel.FifthDriverIDNumber = fifthDriver?.NIN;

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverGender) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverGender.Trim()))
                        policyTemplateModel.FifthDriverGender = fifthDriver?.Gender.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverDateofBirth) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverDateofBirth.Trim()))
                    {
                        policyTemplateModel.FifthDriverDateofBirth = fifthDriver?.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                        policyTemplateModel.FifthDriverAge = fifthDriver?.DateOfBirthG.GetUserAge().ToString();
                    }
                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverNumberofyearseligiblefor) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverNumberofyearseligiblefor.Trim()))
                        policyTemplateModel.FifthDriverNumberofyearseligiblefor = fifthDriver?.NCDFreeYears?.ToString();

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverNoClaimsDiscount) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverNoClaimsDiscount.Trim()))
                        policyTemplateModel.FifthDriverNoClaimsDiscount = fifthDriver?.NCDReference;

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverResidentialAddressCity) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverResidentialAddressCity.Trim()))
                        policyTemplateModel.FifthDriverResidentialAddressCity = fifthDriver?.Addresses?.FirstOrDefault()?.City;

                    if (string.IsNullOrEmpty(policyTemplateModel.FifthDriverFrequencyofDrivingVehicle) || string.IsNullOrWhiteSpace(policyTemplateModel.FifthDriverFrequencyofDrivingVehicle.Trim()))
                        policyTemplateModel.FifthDriverFrequencyofDrivingVehicle = fifthDriver?.DrivingPercentage?.ToString();

                    if (!policyTemplateModel.ChildrenBelow16Years.HasValue)
                        policyTemplateModel.ChildrenBelow16Years = mainDriver.ChildrenBelow16Years.HasValue ? mainDriver.ChildrenBelow16Years.Value : 0;
                    //for allianz

                    policyTemplateModel.BasicPremium = extraPremiumPrice.ToString();

                    //policyTemplateModel.DriverLicenseTypeCode = quotationResponse.QuotationRequest.Driver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc.ToString();
                    var licenseTypeCode = quotationResponse.QuotationRequest.Driver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                    var LicenseTypeEnumKey = (licenseTypeCode.HasValue && licenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                    policyTemplateModel.DriverLicenseTypeCode = (!string.IsNullOrEmpty(LicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(LicenseTypeEnumKey, CultureInfo.GetCultureInfo(lang)) : "";

                    if (quotationResponse.InsuranceCompanyId == 4) // AICC
                    {
                        policyTemplateModel.PACoverForDriverOnly = "0.00";
                        policyTemplateModel.PACoverForDriverOnlyEn = "0.00";
                        policyTemplateModel.PACoverForDriverAndPassenger = "0.00";
                        policyTemplateModel.PACoverForDriverAndPassengerEn = "0.00";
                        policyTemplateModel.ProvisionOfReplacementVehiclePrice = "0.00";
                        policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "0.00";
                        policyTemplateModel.RoadsideAssistanceBenefit = "0.00";
                        policyTemplateModel.RoadsideAssistanceBenefitEn = "0.00";
                        policyTemplateModel.GCCCoverBenefit = "0.00";
                        policyTemplateModel.GCCCoverBenefitEn = "0.00";
                        policyTemplateModel.DeathInjuryMedic = "0.00";
                        policyTemplateModel.DeathInjuryMedicEn = "0.00";
                        policyTemplateModel.UnNamedDriver = "0.00";
                        policyTemplateModel.UnNamedDriverEn = "0.00";
                        policyTemplateModel.GeographicalCoverageAr = "0.00";
                        policyTemplateModel.GeographicalCoverageEn = "0.00";
                        //policyTemplateModel.SpecialDiscount = discount1.ToString("#.##");
                        policyTemplateModel.SpecialDiscount = (discount1 > 0) ? discount1.ToString() : "0.00";
                        policyTemplateModel.Discount = (discount > 0) ? discount.ToString("#.##") : "0.00";
                        policyTemplateModel.SubTotal = HandlePolicySubTotal(orderItem.Product.PriceDetails, policyTemplateModel, totalBenfitPrice);
                        if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                        {
                            policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                            policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                        }

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit == null && benefit.Benefit == null)
                                    continue;

                                if (benefit.BenefitExternalId == "4")
                                    policyTemplateModel.HireCarBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Included";

                                if (benefit.BenefitId == 1)
                                {
                                    policyTemplateModel.PACoverForDriverOnly = benefit.Price.ToString("F");
                                    policyTemplateModel.PACoverForDriverOnlyEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 2)
                                {
                                    policyTemplateModel.PACoverForDriverAndPassenger = benefit.Price.ToString("F");
                                    policyTemplateModel.PACoverForDriverAndPassengerEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 5)
                                {
                                    policyTemplateModel.RoadsideAssistanceBenefit = benefit.Price.ToString("F");
                                    policyTemplateModel.RoadsideAssistanceBenefitEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 10)
                                {
                                    policyTemplateModel.GCCCoverBenefit = benefit.Price.ToString("F");
                                    policyTemplateModel.GCCCoverBenefitEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 6)
                                {
                                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = benefit.Price.ToString("F");
                                    policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 30)
                                {
                                    policyTemplateModel.UnNamedDriver = benefit.Price.ToString("F");
                                    policyTemplateModel.UnNamedDriverEn = benefit.Price.ToString("F");
                                }
                                else if (benefit.BenefitId == 37)
                                {
                                    policyTemplateModel.DeathInjuryMedic = benefit.Price.ToString("F");
                                    policyTemplateModel.DeathInjuryMedicEn = benefit.Price.ToString("F");
                                }
                                else if ((benefit.BenefitId == 57 || benefit.BenefitExternalId == "57") || (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58")) // assgin one of them on GeographicalCoverageAr
                                {
                                    policyTemplateModel.GeographicalCoverageAr = benefit.Price.ToString("F");
                                    policyTemplateModel.GeographicalCoverageEn = benefit.Price.ToString("F");
                                }
                            }
                        }

                        var bankNin = _bankNinRepository.TableNoTracking.Where(a => a.NIN == insured.NationalId).FirstOrDefault();
                        if (bankNin != null)
                        {
                            var bankData = _bankRepository.TableNoTracking.Where(a => a.Id == bankNin.BankId).FirstOrDefault();
                            if (bankData != null)
                                policyTemplateModel.BankNationalAddress = bankData.NationalAddress;
                        }

                        if (secondDriver != null)
                        {
                            var AddtionalOnelicenseTypeCode = secondDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            var AddtionalOneLicenseTypeEnumKey = (AddtionalOnelicenseTypeCode.HasValue && AddtionalOnelicenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                            policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.FullEnglishName : secondDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            }
                        }
                        else
                            policyTemplateModel.SecondDriverRelationship = string.Empty;

                        if (thirdDriver != null)
                        {
                            policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.ThirdDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? thirdDriver.FullEnglishName : thirdDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverTwoOccupation = thirdDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverTwoOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverTwoOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                            }
                        }
                        else
                            policyTemplateModel.ThirdDriverRelationship = string.Empty;
                    }
                    if (quotationResponse.InsuranceCompanyId == 27) //Buruj
                    {
                        policyTemplateModel.VehicleUsingPurposesAr = VehicleUseResource.ResourceManager.GetString(vehicle.VehicleUse.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        if (secondDriver != null)
                            policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        if (thirdDriver != null)
                            policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));

                        policyTemplateModel.SubTotal = HandlePolicySubTotal(orderItem.Product.PriceDetails, policyTemplateModel, totalBenfitPrice);
                    }
                    if (quotationResponse.InsuranceCompanyId == 8) // MedGulf
                    {
                        policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                        policyTemplateModel.PACoverForDriverOnlyEn = "Not Included";
                        policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                        policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Included";
                        policyTemplateModel.HireCarBenefit = "غير مشمولة";
                        policyTemplateModel.HireCarBenefitEn = "Not Included";
                        policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                        policyTemplateModel.GCCCoverBenefitEn = "Not Included";
                        policyTemplateModel.UnNamedDriver = "غير مشمولة";
                        policyTemplateModel.UnNamedDriverEn = "Not Included";
                        policyTemplateModel.DeathInjuryMedic = "غير مشمولة";
                        policyTemplateModel.DeathInjuryMedicEn = "Not Covered";

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit == null || benefit.Benefit == null)
                                    continue;

                                if (benefit.BenefitId == 1 || benefit.BenefitExternalId == "2")
                                {
                                    policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    policyTemplateModel.PACoverForDriverOnlyEn = "Included";
                                    continue;
                                }
                                if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "6")
                                {
                                    policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                    policyTemplateModel.GCCCoverBenefitEn = "Included";
                                    continue;

                                }
                                if (benefit.BenefitId == 5 || benefit.BenefitExternalId == "12")
                                {
                                    policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                    policyTemplateModel.RoadsideAssistanceBenefitEn = "Included";
                                    continue;
                                }
                                if (benefit.BenefitId == 6 || benefit.BenefitExternalId == "1")
                                {
                                    policyTemplateModel.HireCarBenefit = "مشمولة";
                                    policyTemplateModel.HireCarBenefitEn = "Included";
                                    continue;
                                }
                                if (benefit.BenefitId == 30)
                                {
                                    policyTemplateModel.UnNamedDriver = "مشمولة";
                                    policyTemplateModel.UnNamedDriverEn = "Included";
                                    continue;
                                }
                                if (benefit.BenefitId == 37)
                                {
                                    policyTemplateModel.DeathInjuryMedic = "مشمولة";
                                    policyTemplateModel.DeathInjuryMedicEn = "Included";
                                    continue;
                                }
                            }

                            if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                            {
                                policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                                policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                            }
                        }
                    }

                    if (quotationResponse.InsuranceCompanyId == 26) // Amana
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            policyTemplateModel.ProvisionOfReplacementVehiclePrice = "غير مشمولة";
                            policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Not Covered";
                            policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                            policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                            policyTemplateModel.PACoverForDriverAndPassenger = "غير مشمولة";
                            policyTemplateModel.PACoverForDriverAndPassengerEn = "Not Covered";
                            policyTemplateModel.HireCarBenefit = "غير مشمولة";
                            policyTemplateModel.HireCarBenefitEn = "Not Covered";
                            policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                            policyTemplateModel.GCCCoverBenefitEn = "Not Covered";
                            policyTemplateModel.GeographicalCoverageAr = "غير مشمولة";
                            policyTemplateModel.GeographicalCoverageEn = "Not Covered";
                            policyTemplateModel.UnNamedDriver = "غير مشمولة";
                            policyTemplateModel.UnNamedDriverEn = "Not Covered";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "غير مشمولة";
                            policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Not Covered";
                            policyTemplateModel.PassengerIncluded = "غير مشمولة";
                            policyTemplateModel.PassengerIncludedEn = "Not Covered";
                            policyTemplateModel.DriverRelatedToInsured_Above21Ar = "غير مشمولة";
                            policyTemplateModel.DriverRelatedToInsured_Above21En = "Not Covered";
                            policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                            policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Covered";
                            policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                            policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                            policyTemplateModel.HireLimit2000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                            policyTemplateModel.HireLimit3000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "غير مشمولة" : "Not Covered";
                            policyTemplateModel.CarReplacement_100_10DaysAr = "غير مشمولة";
                            policyTemplateModel.CarReplacement_100_10DaysEn = "Not Covered";
                            policyTemplateModel.CarReplacement_200_10DaysAr = "غير مشمولة";
                            policyTemplateModel.CarReplacement_200_10DaysEn = "Not Covered";
                            policyTemplateModel.CarReplacement_300_10DaysAr = "غير مشمولة";
                            policyTemplateModel.CarReplacement_300_10DaysEn = "Not Covered";
                            policyTemplateModel.Benefit_57_Ar = "غير مشمولة";
                            policyTemplateModel.Benefit_57_En = "Not Covered";
                            policyTemplateModel.Benefit_58_Ar = "غير مشمولة";
                            policyTemplateModel.Benefit_58_En = "Not Covered";

                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit == null || benefit.Benefit == null)
                                    continue;

                                if (benefit.BenefitId == 1 || benefit.BenefitExternalId == "1")
                                {
                                    policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                                }
                                else if (benefit.BenefitId == 2 || benefit.BenefitExternalId == "2")
                                {
                                    policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                    policyTemplateModel.PACoverForDriverAndPassengerEn = "Covered";
                                }
                                else if (benefit.BenefitId == 5 || benefit.BenefitExternalId == "5")
                                {
                                    policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                    policyTemplateModel.RoadsideAssistanceBenefitEn = "Covered";

                                }
                                else if (benefit.BenefitId == 6 || benefit.BenefitExternalId == "6")
                                {
                                    policyTemplateModel.HireCarBenefit = "مشمولة";
                                    policyTemplateModel.HireCarBenefitEn = "Covered";
                                }
                                else if (benefit.BenefitId == 8 || benefit.BenefitExternalId == "8")
                                {
                                    policyTemplateModel.PassengerIncluded = "مشمولة";
                                    policyTemplateModel.PassengerIncludedEn = "Covered";
                                }
                                else if (benefit.BenefitId == 9 || benefit.BenefitExternalId == "9")
                                {
                                    policyTemplateModel.GeographicalExtensionBahrain = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                                }
                                else if (benefit.BenefitId == 10 || benefit.BenefitExternalId == "10")
                                {
                                    policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                    policyTemplateModel.GCCCoverBenefitEn = "Covered";
                                }
                                else if (benefit.BenefitId == 11 || benefit.BenefitExternalId == "11")
                                {
                                    policyTemplateModel.GeographicalCoverageAr = "مشمولة";
                                    policyTemplateModel.GeographicalCoverageEn = "Covered";
                                }
                                else if (benefit.BenefitId == 28 || benefit.BenefitExternalId == "28")
                                {
                                    policyTemplateModel.DriverRelatedToInsured_Above21Ar = "مشمولة";
                                    policyTemplateModel.DriverRelatedToInsured_Above21En = "Covered";

                                }
                                else if (benefit.BenefitId == 30 || benefit.BenefitExternalId == "30")
                                {
                                    policyTemplateModel.UnNamedDriver = "مشمولة";
                                    policyTemplateModel.UnNamedDriverEn = "Covered";
                                }
                                else if (benefit.BenefitId == 37 || benefit.BenefitExternalId == "37")
                                {
                                    policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverAr = "مشمولة";
                                    policyTemplateModel.DeathAndphysicalInjuriesAndMedicalExpensesForInsuredOrNamedDriverEn = "Covered";
                                }
                                else if (benefit.BenefitId == 45 || benefit.BenefitExternalId == "45")
                                {
                                    policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                                }
                                else if (benefit.BenefitId == 54 || benefit.BenefitExternalId == "54")
                                {
                                    policyTemplateModel.CarReplacement_100_10DaysAr = "مشمولة";
                                    policyTemplateModel.CarReplacement_100_10DaysEn = "Covered";

                                }
                                else if (benefit.BenefitId == 55 || benefit.BenefitExternalId == "55")
                                {
                                    policyTemplateModel.CarReplacement_200_10DaysAr = "مشمولة";
                                    policyTemplateModel.CarReplacement_200_10DaysEn = "Covered";
                                }
                                else if (benefit.BenefitId == 56 || benefit.BenefitExternalId == "56")
                                {
                                    policyTemplateModel.CarReplacement_300_10DaysAr = "مشمولة";
                                    policyTemplateModel.CarReplacement_300_10DaysEn = "Covered";
                                }
                                else if (benefit.BenefitId == 57 || benefit.BenefitExternalId == "57")
                                {
                                    policyTemplateModel.Benefit_57_Ar = "مشمولة";
                                    policyTemplateModel.Benefit_57_En = "Covered";
                                }
                                else if (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58")
                                {
                                    policyTemplateModel.Benefit_58_Ar = "مشمولة";
                                    policyTemplateModel.Benefit_58_En = "Covered";
                                }
                                else if (benefit.BenefitId == 6200)
                                {
                                    policyTemplateModel.HireLimit2000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                                }
                                else if (benefit.BenefitId == 6300)
                                {
                                    policyTemplateModel.HireLimit3000 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Covered";
                                }

                                if ((benefit.BenefitId == 54 || benefit.BenefitExternalId == "54") || (benefit.BenefitId == 55 || benefit.BenefitExternalId == "55") || (benefit.BenefitId == 56 || benefit.BenefitExternalId == "56"))
                                {
                                    policyTemplateModel.ProvisionOfReplacementVehiclePrice = "مشمولة";
                                    policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Covered";
                                }
                                if ((benefit.BenefitId == 57 || benefit.BenefitExternalId == "57") || (benefit.BenefitId == 58 || benefit.BenefitExternalId == "58"))
                                {
                                    policyTemplateModel.GeographicalCoverageAr = "مشمولة";
                                    policyTemplateModel.GeographicalCoverageEn = "Covered";
                                }
                            }
                        }

                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                        if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                        {
                            policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                            policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                        }
                        if (quotationResponse.VehicleAgencyRepair.HasValue && quotationResponse.VehicleAgencyRepair.Value == true)
                        {
                            policyTemplateModel.VehicleAgencyRepairBenfit = "مشمولة";
                            policyTemplateModel.VehicleAgencyRepairBenfitEn = "Covered";
                        }

                        var socialStatusCode = policyTemplateModel.SocialStatusCode;
                        var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                        policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo(lang));

                        if (vehicle.TransmissionTypeId.HasValue && vehicle.TransmissionTypeId.Value > 0)
                        {
                            var TransmissionTypeId = vehicle.TransmissionTypeId.Value;
                            var TransmissionTypeEnumKey = (TransmissionTypeId > 0) ? Enum.GetName(typeof(TransmissionType), TransmissionTypeId) : String.Empty;
                            policyTemplateModel.TransmissionType = (!string.IsNullOrEmpty(TransmissionTypeEnumKey)) ? TransmissionTypeResource.ResourceManager.GetString(TransmissionTypeEnumKey, CultureInfo.GetCultureInfo(selectedLanguage.ToString())) : "";
                        }
                        if (vehicle.ParkingLocationId.HasValue || vehicle.ParkingLocationId.Value > 0)
                        {
                            var parkingLocationId = vehicle.ParkingLocationId.Value;
                            var parkingLocationEnumKey = (parkingLocationId > 0) ? Enum.GetName(typeof(ParkingLocation), parkingLocationId) : String.Empty;
                            policyTemplateModel.VehicleOvernightParkingLocationCode = (!string.IsNullOrEmpty(parkingLocationEnumKey)) ? ParkingLocationResource.ResourceManager.GetString(parkingLocationEnumKey, CultureInfo.GetCultureInfo(selectedLanguage.ToString())) : "";
                        }
                        if (!string.IsNullOrEmpty(policyTemplateModel.Occupation) || !string.IsNullOrWhiteSpace(policyTemplateModel.Occupation.Trim()))
                        {
                            policyTemplateModel.Occupation = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ?  mainDriver?.OccupationName : mainDriver.Occupation.NameEn;
                        }
                    }

                    if (quotationResponse.InsuranceCompanyId == 11) // GGI
                    {
                        //if (!string.IsNullOrEmpty(policyTemplateModel.InsuredNameAr))
                        //    policyTemplateModel.InsuredNameAr = (policyTemplateModel.InsuredNameAr.Contains("تأجير")) ? "شركة التأجير التمويلى" : policyTemplateModel.InsuredNameAr;

                        policyTemplateModel.SubTotal = HandlePolicySubTotal(orderItem.Product.PriceDetails, policyTemplateModel, totalBenfitPrice);
                        policyTemplateModel.SpecialDiscount = (discount1 > 0) ? discount1.ToString() : "0";
                        policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                        policyTemplateModel.TransmissionType = (vehicle.TransmissionType.HasValue) ? Enum.GetName(typeof(TransmissionType), vehicle.TransmissionTypeId.Value) : "";

                        var OfficePremium = extraPremiumPrice + totalBenfitPrice;
                        policyTemplateModel.OfficePremium = OfficePremium.ToString();
                        policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";
                        policyTemplateModel.LoyaltyDiscountPercentage = (_benfitCode3 != null && _benfitCode3.PercentageValue > 0) ? _benfitCode3.PercentageValue.ToString() : "0.00";
                        policyTemplateModel.NCDAmount = discount > 0 ? string.Format("{0:N}", discount) : "0.00";

                        policyTemplateModel.ProvisionOfReplacementVehiclePrice = "غير مشمولة";
                        policyTemplateModel.HireCarBenefit = "غير مشمولة";
                        policyTemplateModel.UnNamedDriver = "غير مشمولة";
                        policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                        policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                        policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                        policyTemplateModel.PersonalAccidentBenefit = "غير مشمولة";
                        policyTemplateModel.HireCarBenefitEn = "Not Covered";
                        policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Not Covered";
                        policyTemplateModel.UnNamedDriverEn = "Not Covered";
                        policyTemplateModel.RoadsideAssistanceBenefitEn = "Not Covered";
                        policyTemplateModel.GCCCoverBenefitEn = "Not Covered";
                        policyTemplateModel.PACoverForDriverOnlyEn = "Not Covered";
                        policyTemplateModel.PersonalAccidentBenefitEn = "Not Covered";

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 2)
                                {
                                    policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                    policyTemplateModel.PACoverForDriverAndPassengerEn = "Included";
                                    continue;
                                }

                                if (quotationResponse.InsuranceTypeCode == 2)
                                {
                                    if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 5))
                                    {
                                        policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                        policyTemplateModel.RoadsideAssistanceBenefitEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 23))
                                    {
                                        policyTemplateModel.ProvisionOfReplacementVehiclePrice = "مشمولة";
                                        policyTemplateModel.ProvisionOfReplacementVehiclePriceEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 6))
                                    {
                                        policyTemplateModel.HireCarBenefit = "مشمولة";
                                        policyTemplateModel.HireCarBenefitEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 1))
                                    {
                                        policyTemplateModel.PersonalAccidentBenefit = "مشمولة";
                                        policyTemplateModel.PersonalAccidentBenefitEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 30))
                                    {
                                        policyTemplateModel.UnNamedDriver = "مشمولة";
                                        policyTemplateModel.UnNamedDriverEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 8))
                                    {
                                        policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                        policyTemplateModel.PACoverForDriverOnlyEn = "Covered";
                                    }
                                    else if (orderItem.OrderItemBenefits.Any(a => a.BenefitId == 10))
                                    {
                                        policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                        policyTemplateModel.GCCCoverBenefitEn = "Covered";
                                    }
                                }
                            }
                        }
                    }

                    if (quotationResponse.InsuranceCompanyId == 20) // Alrajhi
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "4"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "مشمولة";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "8") && orderItem?.Product?.ProductNameEn == "Wafi Smart")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                else
                                    policyTemplateModel.GCCCoverBenefit = "Inclusded";
                            }
                            else if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "9") && orderItem?.Product?.ProductNameEn == "Wafi Comprehensive")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.GCCCoverBenefit = "مشمولة";
                                else
                                    policyTemplateModel.GCCCoverBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.GCCCoverBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.GCCCoverBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "3"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.PersonalAccidentBenefit = "مشمولة";
                                else
                                    policyTemplateModel.PersonalAccidentBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.PersonalAccidentBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.PersonalAccidentBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "6"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.NaturalHazardsBenefit = "مشمولة";
                                else
                                    policyTemplateModel.NaturalHazardsBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.NaturalHazardsBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.NaturalHazardsBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "5"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.WindscreenFireTheftBenefit = "مشمولة";
                                else
                                    policyTemplateModel.WindscreenFireTheftBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.WindscreenFireTheftBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.WindscreenFireTheftBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "10"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.HandbagCoverReimbursementBenefit = "مشمولة";
                                else
                                    policyTemplateModel.HandbagCoverReimbursementBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.HandbagCoverReimbursementBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.HandbagCoverReimbursementBenefit = "Not Included";
                            }
                        }
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.BenefitExternalId == "11"))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.ChildSeatCoverReimbursementBenefit = "مشمولة";
                                else
                                    policyTemplateModel.ChildSeatCoverReimbursementBenefit = "Inclusded";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.ChildSeatCoverReimbursementBenefit = "غير مشمولة";
                                else
                                    policyTemplateModel.ChildSeatCoverReimbursementBenefit = "Not Included";
                            }
                        }
                        if (orderItem?.Product?.ProductNameEn == "Wafi Smart")
                        {
                            var totalPremium = double.Parse(policyTemplateModel.TotalPremium) * 0.04;
                            policyTemplateModel.CommissionAmmount = totalPremium.ToString("#.##");
                        }

                        if (orderItem?.Product?.ProductNameEn == "Wafi Basic")
                        {
                            var totalPremium = double.Parse(policyTemplateModel.TotalPremium) * 0.02;
                            policyTemplateModel.CommissionAmmount = totalPremium.ToString("#.##");
                        }
                        if (orderItem?.Product?.ProductNameEn == "Wafi Comprehensive")
                        {
                            var totalPremium = double.Parse(policyTemplateModel.TotalPremium) * 0.1;
                            policyTemplateModel.CommissionAmmount = totalPremium.ToString("#.##");
                        }

                        ////
                        /// delete any percentage multiplication as company return the coorect percentage  @12-4-2023 as per Rawan
                        var ncdPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue;// * 100;
                        policyTemplateModel.NCDPercentage = ncdPercentage.HasValue ? ncdPercentage.ToString() : "0";
                        var specialDiscountPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PercentageValue;// * 100;
                        policyTemplateModel.SpecialDiscountPercentage = specialDiscountPercentage.HasValue ? specialDiscountPercentage.ToString() : "0";
                        var loyaltyDis = (_benfitCode3 != null && _benfitCode3.PercentageValue > 0) ? _benfitCode3.PercentageValue : 0; // * 100
                        policyTemplateModel.LoyaltyDiscountPercentage = loyaltyDis.ToString();
                        
                        decimal benefitPrice = 0;
                        foreach (var benefit in orderItem?.OrderItemBenefits)
                        {
                            benefitPrice += benefit.Price;
                        }
                        var OfficePremium = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 7).FirstOrDefault()?.PriceValue + benefitPrice;
                        policyTemplateModel.OfficePremium = OfficePremium.ToString();
                        policyTemplateModel.ActualAmount = (OfficePremium + vat).ToString();
                    }
                    if (quotationResponse.InsuranceCompanyId == 18) // Alamia (Liva)
                    {
                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                        if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                        {
                            policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                            policyTemplateModel.VehicleModificationDetailsEn = "Not Available";
                        }

                        if (secondDriver != null)
                        {
                            var AddtionalOnelicenseTypeCode = secondDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            var AddtionalOneLicenseTypeEnumKey = (AddtionalOnelicenseTypeCode.HasValue && AddtionalOnelicenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                            policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.FullEnglishName : secondDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver.SocialStatusName; //RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            }
                        }
                        if (thirdDriver != null)
                        {
                            var AddtionalTwolicenseTypeCode = thirdDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            policyTemplateModel.AddtionalDriverTwoLicenseTypeCode = (AddtionalTwolicenseTypeCode.HasValue && AddtionalTwolicenseTypeCode > 0) ? LicenseTypeResource.ResourceManager.GetString(Enum.GetName(typeof(LicenseTypeEnum), AddtionalTwolicenseTypeCode), CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.ThirdDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? thirdDriver.FullEnglishName : thirdDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverTwoSocialStatus = thirdDriver.SocialStatusName; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), thirdDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.AddtionalDriverTwoOccupation = thirdDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverTwoOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverTwoOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                            }
                        }

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.Benefit.Code == 5))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "نعم";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Yes";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "لا";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "No";
                            }
                        }
                    }
                    if (quotationResponse.InsuranceCompanyId == 6) // Saqr
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.Benefit.Code == 5))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "المساعدة على الطريق";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Roadside Assistance";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = string.Empty;
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = string.Empty;
                            }
                        }
                    }
                    if (quotationResponse.InsuranceCompanyId == 27)
                    {
                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            if (orderItem.OrderItemBenefits.Any(a => a.Benefit != null && a.Benefit.Code == 5))
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = "المساعدة على الطريق";
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = "Roadside Assistance";
                            }
                            else
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                    policyTemplateModel.RoadsideAssistanceBenefit = string.Empty;
                                else
                                    policyTemplateModel.RoadsideAssistanceBenefit = string.Empty;
                            }
                        }
                    }
                    
                    if (quotationResponse.InsuranceCompanyId == 14)
                    {
                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        policyTemplateModel.SubTotal = HandlePolicySubTotal(orderItem.Product.PriceDetails, policyTemplateModel, totalBenfitPrice);

                        foreach (var benefit in orderItem.OrderItemBenefits)
                        {
                            if (benefit == null || benefit.Benefit == null)
                                continue;

                            if (benefit.BenefitId == 117 || benefit.BenefitExternalId == "117")
                            {
                                policyTemplateModel.PassengerIncluded = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 118 || benefit.BenefitExternalId == "118")
                            {
                                policyTemplateModel.PACoverForDriverOnly = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 119 || benefit.BenefitExternalId == "119")
                            {
                                policyTemplateModel.GCCCoverBenefit = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 120 || benefit.BenefitExternalId == "120")
                            {
                                policyTemplateModel.ProvisionOfReplacementVehiclePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 121 || benefit.BenefitExternalId == "121")
                            {
                                policyTemplateModel.Towing = Math.Round(benefit.Price, 2).ToString();
                            }
                            //else if (benefit.BenefitId == 122 || benefit.BenefitExternalId == "122")
                            //{
                            //    policyTemplateModel.EmergencyMedical = benefit.Price.ToString();
                            //}
                            else if (benefit.BenefitId == 259 || benefit.BenefitExternalId == "259")
                            {
                                policyTemplateModel.LiabilitiesToThirdPartiesPrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 260 || benefit.BenefitExternalId == "260")
                            {
                                policyTemplateModel.CoverageForTotalOrPartialLossOfVehiclePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 261 || benefit.BenefitExternalId == "261")
                            {
                                policyTemplateModel.NaturalDisasters = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 262 || benefit.BenefitExternalId == "262")
                            {
                                policyTemplateModel.TheftFirePrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 265 || benefit.BenefitExternalId == "265")
                            {
                                policyTemplateModel.TowingWithAccidentPrice = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 266 || benefit.BenefitExternalId == "266")
                            {
                                policyTemplateModel.EmergencyMedical = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 489 || benefit.BenefitExternalId == "489")
                            {
                                policyTemplateModel.RoadsideAssistanceBenefit = Math.Round(benefit.Price, 2).ToString();
                            }
                            else if (benefit.BenefitId == 490 || benefit.BenefitExternalId == "490")
                            {
                                policyTemplateModel.UnNamedDriver = Math.Round(benefit.Price, 2).ToString();
                            }
                        }
                    }

                    var _policyFees = 0;
                    if (quotationResponse.InsuranceCompanyId == 24)//Alianz 
                    {
                        var _annualPremiumIncludingLoadingsCharges = extraPremiumPrice + additionAgeContribution + fees;
                        policyTemplateModel.AnnualPremiumIncludingLoadingsCharges = _annualPremiumIncludingLoadingsCharges.ToString();

                        policyTemplateModel.Charges = fees.ToString();
                        var _benfitCode4 = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 4).FirstOrDefault();
                        policyTemplateModel.RiskPremiumTrafficViolationLoading = (_benfitCode4 != null && _benfitCode4.PriceValue > 0) ? _benfitCode4.PriceValue.ToString("#.##") : "0.00";

                        var _annualPremiumbeforeNoClaimsDiscount = _annualPremiumIncludingLoadingsCharges + _passengerIncluded + _pACoverForDriverOnly + _benfitCode4?.PriceValue;
                        policyTemplateModel.AnnualPremiumbeforeNoClaimsDiscount = _annualPremiumbeforeNoClaimsDiscount.ToString();

                        policyTemplateModel.TotalAnnualPremiumAfterNoClaimsDiscount = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue).ToString();

                        var _loyaltyDiscountValue = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 3).FirstOrDefault()?.PriceValue;
                        policyTemplateModel.AnnualPremiumAfterLoyaltyDiscount = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue).ToString();

                        var _bcareCommission = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue) * 2 / 100;
                        policyTemplateModel.BcareCommission = Math.Round((double)_bcareCommission, 2).ToString();

                        policyTemplateModel.PolicyFees = _policyFees.ToString();
                        policyTemplateModel.TotalAnnualPremiumWithoutVAT = (_annualPremiumbeforeNoClaimsDiscount - _benfitCode2.PriceValue - _loyaltyDiscountValue + _policyFees).ToString();
                        policyTemplateModel.DeathInjuryMedicEn = "Not Included";
                        policyTemplateModel.DeathInjuryMedic = "غير مشمولة";
                        policyTemplateModel.DeathInjuryMedicPrice = "0.00";

                        //policyTemplateModel.SubTotal = HandlePolicySubTotal(policyTemplateModel);
                        policyTemplateModel.SubTotal = HandlePolicySubTotal(orderItem.Product.PriceDetails, policyTemplateModel, totalBenfitPrice);
                        policyTemplateModel.SpecialDiscount = (orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue > 0) ? orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue.ToString("#.##") : "0.00";

                        if (string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2))
                            policyTemplateModel.SpecialDiscount2 = "0.00";

                        if (string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2Percentage))
                            policyTemplateModel.SpecialDiscount2Percentage = "0.00";

                        if (string.IsNullOrEmpty(policyTemplateModel.VehicleModificationDetails))
                            policyTemplateModel.VehicleModificationDetails = "لا يوجد";
                        foreach (var item in orderItem.OrderItemBenefits)
                        {
                            if (item.BenefitExternalId == "PADRV")
                            {
                                policyTemplateModel.PersonalAccidentBenefit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.PersonalAccidentForDriverPrice = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "PAPSG")
                            {
                                policyTemplateModel.PassengerIncluded = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";

                                policyTemplateModel.PassengersPersonalAccidentPrice = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "CARREP-150")
                            {
                                policyTemplateModel.CarReplacement_150 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.HireCarBenefit = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "CARREP-200")
                            {
                                policyTemplateModel.CarReplacement_200 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.HireCarBenefit = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "CARREP-75")
                            {
                                policyTemplateModel.CarReplacement_75 = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.HireCarBenefit = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "GEXT-2")
                            {
                                policyTemplateModel.GCC_12Month = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "GEXT-3")
                            {
                                policyTemplateModel.GeographicalCoverageAr = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "GEXT-4")
                            {
                                policyTemplateModel.GE_Jordan_Lebanon_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "GEXT-5")
                            {
                                policyTemplateModel.GE_Egypt_Sudan_Turky_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "GEXT-6")
                            {
                                policyTemplateModel.GCC_Jordan_Lebanon_Egypt_Sudan_12months = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                policyTemplateModel.GeoGraphicalBenefits = item.Price.ToString();
                            }
                            else if (item.BenefitExternalId == "24008")
                            {
                                policyTemplateModel.MaxLimit = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                            else if (item.BenefitExternalId == "24009")
                            {
                                policyTemplateModel.OwnDamage = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                            else if (item.BenefitExternalId == "3")
                            {
                                policyTemplateModel.NaturalDisasters = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                            else if (item.BenefitExternalId == "4")
                            {
                                if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                                {
                                    policyTemplateModel.TheftFire = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                                }
                            }
                            else if (item.BenefitExternalId == "24")
                            {
                                policyTemplateModel.Towing = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                            else if (item.BenefitExternalId == "19")
                            {
                                policyTemplateModel.EmergencyMedical = (selectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "مشمولة" : "Not Included";
                            }
                            else if (item.BenefitId == 37 || item.BenefitExternalId == "37")
                            {
                                policyTemplateModel.DeathInjuryMedicPrice = (item.Price > 0) ? Math.Round(item.Price, 2).ToString() : "0.00";
                                policyTemplateModel.DeathInjuryMedic = "مشمولة";
                                policyTemplateModel.DeathInjuryMedicEn = "Included";
                                DeathPhysicalInjuriesList.Add(new BenfitSummaryModel());
                            }
                        }
                    }

                    else if (quotationResponse.InsuranceCompanyId == 10)//Ahlia
                    {
                        policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0";
                        policyTemplateModel.ClalmLoadingAmount = (clalmLoadingAmount > 0) ? clalmLoadingAmount.ToString() : "0";
                        policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                        policyTemplateModel.AccidentLoadingAmount = "0";

                        policyTemplateModel.GeographicalCoverageAr = "غير مشمولة";
                        policyTemplateModel.GeographicalCoverageEn = "Not Included";

                        policyTemplateModel.DriverAgeCoverageBlew21Ar = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageBlew21En = "Not Included";
                        policyTemplateModel.DriverAgeCoverageFrom18To20Ar = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom18To20En = "Not Included";
                        policyTemplateModel.DriverAgeCoverageFrom21To24Ar = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom21To24En = "Not Included";
                        policyTemplateModel.DriverAgeCoverageAbove24Ar = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageAbove24En = "Not Included";
                        policyTemplateModel.DriverAgeCoverageFrom17To21Ar_Tpl = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom17To21En_Tpl = "Not Included";
                        policyTemplateModel.DriverAgeCoverageFrom21To24Ar_Tpl = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom21To24En_Tpl = "Not Included";
                        policyTemplateModel.DriverAgeCoverageFrom24To29Ar_Tpl = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageFrom24To29En_Tpl = "Not Included";
                        policyTemplateModel.DriverAgeCoverageAbove29Ar_Tpl = "غير مشمولة";
                        policyTemplateModel.DriverAgeCoverageAbove29En_Tpl = "Not Included";
                        policyTemplateModel.PACoverForDriverOnly = "غير مشمولة";
                        policyTemplateModel.PACoverForDriverOnlyEn = "Not Included";
                        policyTemplateModel.PACoverForDriverAndPassenger = "غير مشمولة";
                        policyTemplateModel.PACoverForDriverAndPassengerEn = "Not Included";

                        var insuredAge = insured.BirthDate.GetUserAge();
                        // comp prop
                        if (insuredAge < 21)
                        {
                            policyTemplateModel.DriverAgeCoverageBlew21Ar = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageBlew21En = "Included";

                            if (insuredAge <= 20 && insuredAge >= 18)
                            {
                                policyTemplateModel.DriverAgeCoverageFrom18To20Ar = "مشمولة";
                                policyTemplateModel.DriverAgeCoverageFrom18To20En = "Included";
                            }
                        }
                        else if (insuredAge <= 24 && insuredAge >= 21)
                        {
                            policyTemplateModel.DriverAgeCoverageFrom21To24Ar = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageFrom21To24En = "Included";
                        }
                        else
                        {
                            policyTemplateModel.DriverAgeCoverageAbove24Ar = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageAbove24En = "Included";
                        }

                        // tpl prop
                        if (insuredAge <= 21 && insuredAge >= 17)
                        {
                            policyTemplateModel.DriverAgeCoverageFrom17To21Ar_Tpl = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageFrom17To21En_Tpl = "Included";
                        }
                        else if (insuredAge <= 24 && insuredAge > 21)
                        {
                            policyTemplateModel.DriverAgeCoverageFrom21To24Ar_Tpl = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageFrom21To24En_Tpl = "Included";
                        }
                        else if (insuredAge <= 29 && insuredAge > 24)
                        {
                            policyTemplateModel.DriverAgeCoverageFrom24To29Ar_Tpl = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageFrom24To29En_Tpl = "Included";
                        }
                        else
                        {
                            policyTemplateModel.DriverAgeCoverageAbove29Ar_Tpl = "مشمولة";
                            policyTemplateModel.DriverAgeCoverageAbove29En_Tpl = "Included";
                        }

                        if (orderItem != null && orderItem.OrderItemBenefits != null)
                        {
                            foreach (var benefit in orderItem.OrderItemBenefits)
                            {
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 1)
                                {
                                    policyTemplateModel.PACoverForDriverOnly = "مشمولة";
                                    policyTemplateModel.PACoverForDriverOnlyEn = "Included";
                                    continue;
                                }
                                if (benefit != null && benefit.Benefit != null && benefit.Benefit.Code == 2)
                                {
                                    policyTemplateModel.PACoverForDriverAndPassenger = "مشمولة";
                                    policyTemplateModel.PACoverForDriverAndPassengerEn = "Included";
                                    continue;
                                }
                            }
                        }
                    }
                    else if (quotationResponse.InsuranceCompanyId == 13) // Salama
                    {
                        var socialStatusCode = policyTemplateModel.SocialStatusCode;
                        var socialStatus = (socialStatusCode > 0 && socialStatusCode < 8) ? Enum.GetName(typeof(SocialStatus), socialStatusCode) : String.Empty;
                        policyTemplateModel.SocialStatus = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo("ar-SA"));
                        policyTemplateModel.SocialStatusEn = SocialStatusResource.ResourceManager.GetString(socialStatus, CultureInfo.GetCultureInfo("en-US"));

                        //policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                        policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                        policyTemplateModel.MainDriverGenderEn = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("en-US"));

                        if (secondDriver != null)
                        {
                            var AddtionalOnelicenseTypeCode = secondDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            var AddtionalOneLicenseTypeEnumKey = (AddtionalOnelicenseTypeCode.HasValue && AddtionalOnelicenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                            policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.SecondDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? secondDriver.FullEnglishName : secondDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver.SocialStatusName; //RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), secondDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.AddtionalDriverOneOccupation = secondDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverOneOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverOneOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverOneOccupation = "Other";
                            }
                        }
                        if (thirdDriver != null)
                        {
                            var AddtionalTwolicenseTypeCode = thirdDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            policyTemplateModel.AddtionalDriverTwoLicenseTypeCode = (AddtionalTwolicenseTypeCode.HasValue && AddtionalTwolicenseTypeCode > 0) ? LicenseTypeResource.ResourceManager.GetString(Enum.GetName(typeof(LicenseTypeEnum), AddtionalTwolicenseTypeCode), CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.ThirdDriverGender = GenderResource.ResourceManager.GetString(thirdDriver?.Gender.ToString(), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.ThirdDriverName = selectedLanguage == LanguageTwoLetterIsoCode.En ? thirdDriver.FullEnglishName : thirdDriver.FullArabicName;
                            policyTemplateModel.AddtionalDriverTwoSocialStatus = thirdDriver.SocialStatusName; // RelationShipResource.ResourceManager.GetString(Enum.GetName(typeof(RelationShip), thirdDriver.SocialStatusId), CultureInfo.GetCultureInfo(lang));
                            policyTemplateModel.AddtionalDriverTwoOccupation = thirdDriver.OccupationName;
                            if (selectedLanguage == LanguageTwoLetterIsoCode.En)
                            {
                                if (policyTemplateModel.AddtionalDriverTwoOccupation == "حكومي")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Government";
                                else if (policyTemplateModel.AddtionalDriverTwoOccupation == "غير ذلك")
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                                else
                                    policyTemplateModel.AddtionalDriverTwoOccupation = "Other";
                            }
                        }
                    }
                    else
                    {
                        policyTemplateModel.AnnualPremiumIncludingLoadingsCharges = "";
                        policyTemplateModel.BcareCommission = bcareCommission.ToString();
                        policyTemplateModel.Charges = "";
                        policyTemplateModel.RiskPremiumTrafficViolationLoading = "N/A";
                        policyTemplateModel.TotalAnnualPremiumAfterNoClaimsDiscount = (extraPremiumPrice - discount).ToString();
                        policyTemplateModel.AnnualPremiumAfterLoyaltyDiscount = (extraPremiumPrice - loyalDiscount).ToString();
                        policyTemplateModel.PolicyFees = fees.ToString();
                        policyTemplateModel.TotalAnnualPremiumWithoutVAT = (extraPremiumPrice - vatPrice).ToString();
                        policyTemplateModel.AnnualPremiumbeforeNoClaimsDiscount = "";
                    }

                    if (quotationResponse.InsuranceCompanyId == 8 || quotationResponse.InsuranceCompanyId == 13 || quotationResponse.InsuranceCompanyId == 14)
                    {
                        if (secondDriver != null)
                        {
                            var AddtionalOnelicenseTypeCode = secondDriver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.TypeDesc;
                            var AddtionalOneLicenseTypeEnumKey = (AddtionalOnelicenseTypeCode.HasValue && AddtionalOnelicenseTypeCode > 0) ? Enum.GetName(typeof(LicenseTypeEnum), licenseTypeCode.Value) : String.Empty;
                            policyTemplateModel.AddtionalDriverOneLicenseTypeCode = (!string.IsNullOrEmpty(AddtionalOneLicenseTypeEnumKey)) ? LicenseTypeResource.ResourceManager.GetString(AddtionalOneLicenseTypeEnumKey, CultureInfo.GetCultureInfo(lang)) : "";
                            policyTemplateModel.AddtionalDriverOneOccupation = secondDriver?.OccupationName;
                            policyTemplateModel.SecondDriverGender = GenderResource.ResourceManager.GetString(secondDriver.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));
                            if (!string.IsNullOrEmpty(secondDriver.SocialStatusName))
                                policyTemplateModel.AddtionalDriverOneSocialStatus = secondDriver.SocialStatusName;
                        }
                    }

                    policyTemplateModel.CarEndorsmentList = CarEndorsmentList;
                    policyTemplateModel.AccidentOutsideTerritory = AccidentOutsideTerritory;
                    policyTemplateModel.DeathPhysicalInjuriesList = DeathPhysicalInjuriesList;
                    policyTemplateModel.DeathPhysicalInjuriesListForPassengers = DeathPhysicalInjuriesListForPassengers;
                    policyTemplateModel.RoadAssistanttList = RoadAssistanttList;

                    policyTemplateModel.AdditionalAgeContribution = additionAgeContribution.ToString();
                    policyTemplateModel.AdditionalPremium = (clalmLoadingAmount + additionAgeContribution) > 0 ? (clalmLoadingAmount + additionAgeContribution).ToString("#.##") : "0.00";

                    policyTemplateModel.MainDriverLicenseExpiryDate = quotationResponse.QuotationRequest.Driver.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;
                    policyTemplateModel.SecondDriverLicenseExpiryDate = secondDriver?.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;
                    policyTemplateModel.ThirdDriverLicenseExpiryDate = thirdDriver?.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;
                    policyTemplateModel.FourthDriverLicenseExpiryDate = fourthDriver?.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;
                    policyTemplateModel.FifthDriverLicenseExpiryDate = fifthDriver?.DriverLicenses.OrderByDescending(x => x.LicenseId).FirstOrDefault()?.ExpiryDateH;

                    if (policy.PolicyEffectiveDate.HasValue)
                    {
                        if (selectedLanguage == LanguageTwoLetterIsoCode.Ar)
                        {
                            policyTemplateModel.InsuranceStartDay = policy.PolicyEffectiveDate?.ToString("dddd", new CultureInfo("ar-SA"));
                            policyTemplateModel.InsuranceEndDay = policy.PolicyExpiryDate?.ToString("dddd", new CultureInfo("ar-SA"));
                        }
                        else
                        {
                            policyTemplateModel.InsuranceStartDay = policy.PolicyEffectiveDate?.ToString("dddd", new CultureInfo("en-US"));
                            policyTemplateModel.InsuranceEndDay = policy.PolicyExpiryDate?.ToString("dddd", new CultureInfo("en-US"));
                        }
                    }

                    if (policy.PolicyEffectiveDate.HasValue && vehicle.ModelYear.HasValue)
                    {
                        int yearsDifference = policy.PolicyEffectiveDate.Value.Year - vehicle.ModelYear.Value;
                        if (yearsDifference > 5)
                        {
                            policyTemplateModel.PartialLossClaimsEn = "- In case of Partial Loss claims: deduct 15% from the cost of new parts.";
                            policyTemplateModel.PartialLossClaimsAr = "في حالة الخسارة الجزئية: يخصم 15 % من قطع الغيار المستبدلة .";

                            policyTemplateModel.TotalLossClaimsEn = "- In case of Total Loss claims: deduct 20% from the insured value or market value whichever is less.";
                            policyTemplateModel.TotalLossClaimsAr = "في حالة الخسارة الكلية: يخصم 20 % من القيمة المؤمنة او القيمة السوقية أيهما أقل.";
                        }
                        else
                        {
                            policyTemplateModel.PartialLossClaimsEn = "- In case of Partial Loss claims: Nil.";
                            policyTemplateModel.PartialLossClaimsAr = "في حالة الخسارة الجزئية : لا يوجد .";

                            policyTemplateModel.TotalLossClaimsEn = "- In case of Total Loss claims: deduct 20% from the insured value or market value whichever is less.";
                            policyTemplateModel.TotalLossClaimsAr = "في حالة الخسارة الكلية: يخصم 20 % من القيمة المؤمنة او القيمة السوقية أيهما أقل.";
                        }
                    }
                    policyTemplateModel.Drivers = new List<DriverPloicyGenerationDto>();
                    foreach (var additionalDriver in additionalDrivers)
                    {
                        var driverDto = new DriverPloicyGenerationDto();
                        driverDto.DriverTypeCode = 2;
                        long driverId = 0;
                        long.TryParse(additionalDriver?.NIN, out driverId);
                        driverDto.DriverId = driverId;
                        driverDto.DriverIdTypeCode = additionalDriver.IsCitizen ? 1 : 2;
                        driverDto.DriverBirthDate = additionalDriver.DateOfBirthH;
                        driverDto.DriverBirthDateG = additionalDriver.DateOfBirthG;
                        driverDto.DriverFirstNameAr = additionalDriver.FirstName;
                        driverDto.DriverFirstNameEn = additionalDriver.EnglishFirstName;
                        driverDto.DriverMiddleNameAr = additionalDriver.SecondName;
                        driverDto.DriverMiddleNameEn = additionalDriver.EnglishSecondName;
                        driverDto.DriverLastNameAr = additionalDriver.LastName;
                        driverDto.DriverLastNameEn = additionalDriver.EnglishLastName;
                        driverDto.DriverFullNameAr = $"{additionalDriver.FullArabicName}";
                        driverDto.DriverFullNameEn = $"{additionalDriver.FullEnglishName}";
                        driverDto.IsAgeLessThen21YearsAR = DateTime.Now.Year - additionalDriver.DateOfBirthG.Year < 21 ? "نعم" : "لا";
                        driverDto.IsAgeLessThen21YearsEN = DateTime.Now.Year - additionalDriver.DateOfBirthG.Year < 21 ? "Yes" : "No";
                        driverDto.DriverOccupation = additionalDriver.ResidentOccupation;
                        driverDto.DriverNOALast5Years = additionalDriver.NOALast5Years;
                        driverDto.DriverNOCLast5Years = additionalDriver.NOCLast5Years;
                        driverDto.DriverNCDFreeYears = additionalDriver.NCDFreeYears;
                        driverDto.DriverNCDReference = additionalDriver.NCDReference;
                        driverDto.DriverGenderCode = additionalDriver.Gender.GetCode();
                        driverDto.DriverSocialStatusCode = additionalDriver.SocialStatusId?.ToString();
                        driverDto.DriverNationalityCode = additionalDriver.NationalityCode.HasValue ?
                                additionalDriver.NationalityCode.Value.ToString() : "113";
                        driverDto.DriverOccupationCode = additionalDriver.Occupation?.Code;
                        driverDto.DriverDrivingPercentage = additionalDriver.DrivingPercentage;
                        driverDto.DriverEducationCode = additionalDriver.EducationId;
                        driverDto.DriverMedicalConditionCode = additionalDriver.MedicalConditionId;
                        driverDto.DriverChildrenBelow16Years = additionalDriver.ChildrenBelow16Years;
                        driverDto.DriverHomeCityCode = additionalDriver.City?.Code.ToString();
                        driverDto.DriverHomeCity = additionalDriver.City?.ArabicDescription;
                        driverDto.DriverWorkCityCode = additionalDriver.WorkCity?.Code.ToString();
                        driverDto.DriverWorkCity = additionalDriver.WorkCity?.ArabicDescription;
                        driverDto.NCDAmount = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PriceValue.ToString();
                        driverDto.NCDPercentage = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 2).FirstOrDefault()?.PercentageValue?.ToString();

                        policyTemplateModel.Drivers.Add(driverDto);
                    }
                    if (insured != null)
                    {
                        if (insured.CardIdTypeId == 1 && quotationResponse.InsuranceCompanyId != 20)
                        {
                            policyTemplateModel.InsuredIdTypeCode = insured.CardIdTypeId.ToString();
                            policyTemplateModel.InsuredIdTypeEnglishDescription = "Citizen";
                            policyTemplateModel.InsuredIdTypeArabicDescription = "مواطن";
                        }
                        if (insured.CardIdTypeId == 2 && quotationResponse.InsuranceCompanyId != 20)
                        {
                            policyTemplateModel.InsuredIdTypeCode = insured.CardIdTypeId.ToString();
                            policyTemplateModel.InsuredIdTypeEnglishDescription = "Resident";
                            policyTemplateModel.InsuredIdTypeArabicDescription = "مقيم";
                        }
                        if ((insured.CardIdTypeId == 1 || insured.CardIdTypeId == 2) && quotationResponse.InsuranceCompanyId == 20)//AlRajhi
                        {
                            policyTemplateModel.InsuredIdTypeCode = insured.CardIdTypeId.ToString();
                            policyTemplateModel.InsuredIdTypeEnglishDescription = "Individual";
                            policyTemplateModel.InsuredIdTypeArabicDescription = "أفراد";
                        }
                        if (insured.CardIdTypeId == 1 && insured.NationalId.StartsWith("7"))
                        {
                            policyTemplateModel.InsuredIdTypeCode = insured.CardIdTypeId.ToString();
                            policyTemplateModel.InsuredIdTypeEnglishDescription = "Companies";
                            policyTemplateModel.InsuredIdTypeArabicDescription = "شركات";
                        }

                    }
                    if (quotationResponse.InsuranceCompanyId == 11 || quotationResponse.InsuranceCompanyId == 20|| quotationResponse.InsuranceCompanyId == 23)
                    {
                        //policyTemplateModel.TotalAnnualPremiumWithoutVAT = extraPremiumPrice.ToString("#.##");
                        //policyTemplateModel.VehicleLimitValue = vehicle.VehicleValue?.ToString();

                        //if (quotationResponse.ProductType?.Code == 1) // tpl --> vahicle value from vehicle it self (VehicleValue)
                        //{
                        //    policyTemplateModel.VehicleValue = vehicle.VehicleValue?.ToString();
                        //    policyTemplateModel.VehicleLimitValue = vehicle.VehicleValue?.ToString();
                        //}
                        //else // comp --> if product returned with (VehicleLimitValue) take this value else take it from vehicle it self (VehicleValue)
                        //{
                        //    policyTemplateModel.VehicleValue = (orderItem.Product?.VehicleLimitValue.HasValue == true && orderItem.Product?.VehicleLimitValue.Value > 0)
                        //                                        ? orderItem.Product?.VehicleLimitValue.Value.ToString()
                        //                                        : vehicle.VehicleValue?.ToString();
                        //    policyTemplateModel.VehicleLimitValue = (orderItem.Product?.VehicleLimitValue.HasValue == true && orderItem.Product?.VehicleLimitValue.Value > 0)
                        //                                        ? orderItem.Product?.VehicleLimitValue.Value.ToString()
                        //                                        : vehicle.VehicleValue?.ToString();
                        //}
                        if (quotationResponse.InsuranceCompanyId == 20)
                        {
                            policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";

                            var _benfitCode9 = orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 9).FirstOrDefault();
                            policyTemplateModel.BcareCommission = (_benfitCode9 != null) ? _benfitCode9?.PercentageValue.ToString() : "0";
                            policyTemplateModel.BcareCommissionValue = (_benfitCode9 != null) ? _benfitCode9.PriceValue.ToString() : "0.00";
                        }
                        if (quotationResponse.InsuranceCompanyId == 11) // GGI
                        {
                            policyTemplateModel.AgeLoadingAmount = (additionAgeContribution > 0) ? additionAgeContribution.ToString() : "0";
                            policyTemplateModel.PolicyFees = (fees > 0) ? fees.ToString() : "0.00";
                            policyTemplateModel.SpecialDiscount = (orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue > 0) ? orderItem?.Product?.PriceDetails?.Where(x => x.PriceTypeCode == 1).FirstOrDefault()?.PriceValue.ToString("#.##") : "0.00";
                            policyTemplateModel.MainDriverGender = GenderResource.ResourceManager.GetString(mainDriver?.Gender.ToString(), CultureInfo.GetCultureInfo("ar-SA"));

                            //policyTemplateModel.VehicleOvernightParkingLocationCode = (vehicle.ParkingLocationId.HasValue) ?
                            //ParkingLocationResource.ResourceManager.GetString(vehicle.ParkingLocationId.Value.ToString(), CultureInfo.GetCultureInfo(stringLang)) : "";

                            //policyTemplateModel.TransmissionType = (vehicle.TransmissionType.HasValue) ?
                            //TransmissionTypeResource.ResourceManager.GetString(vehicle.TransmissionType.HasValue.ToString(), CultureInfo.GetCultureInfo(stringLang)) : "";
                        }
                    }
                    if (orderItem.Product != null)
                    {
                        policyTemplateModel.ProductDescription = orderItem.Product.ProductNameEn;
                    }

                    if (checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower())
                    {
                        var bankNin = _bankNinRepository.TableNoTracking.Where(a => a.NIN == insured.NationalId).FirstOrDefault();
                        if (bankNin != null)
                        {
                            var bankData = _bankRepository.TableNoTracking.Where(a => a.Id == bankNin.BankId).FirstOrDefault();
                            if (bankData != null)
                            {
                                policyTemplateModel.BankNationalAddress = bankData.NationalAddress;
                                policyTemplateModel.LessorMobileNo = Utilities.ValidatePhoneNumber(bankData.PhoneNumber);
                            }
                        }

                        #region Deprecation Data
                        var depreciationSetting = _autoleasingDepreciationSettingHistoryRepository.TableNoTracking.FirstOrDefault(d => d.ExternalId == quotationResponse.QuotationRequest.ExternalId);
                        if (depreciationSetting != null)
                        {
                            List<string> annualPercentages = new List<string>();
                            if (!depreciationSetting.FirstYear.Equals(null))
                            {
                                annualPercentages.Add(depreciationSetting.FirstYear.ToString().Replace(".00", "") + "%");
                            }
                            if (!depreciationSetting.SecondYear.Equals(null))
                            {
                                annualPercentages.Add(depreciationSetting.SecondYear.ToString().Replace(".00", "") + "%");
                            }
                            if (!depreciationSetting.ThirdYear.Equals(null))
                            {
                                annualPercentages.Add(depreciationSetting.ThirdYear.ToString().Replace(".00", "") + "%");
                            }
                            if (!depreciationSetting.FourthYear.Equals(null))
                            {
                                annualPercentages.Add(depreciationSetting.FourthYear.ToString().Replace(".00", "") + "%");
                            }
                            if (!depreciationSetting.FifthYear.Equals(null))
                            {
                                annualPercentages.Add(depreciationSetting.FifthYear.ToString().Replace(".00", "") + "%");
                            }

                            policyTemplateModel.AnnualDeprecationPercentages = (depreciationSetting.IsDynamic) ? string.Join(", ", annualPercentages) : depreciationSetting.Percentage.ToString().Replace(".00", "") + "%";

                            var vehicleValue = vehicle.VehicleValue;
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

                            policyTemplateModel.FirstYearDepreciation = Math.Round(DepreciationValue1.Value, 2).ToString().Replace(".00", "");
                            policyTemplateModel.SecondYearDepreciation = Math.Round(DepreciationValue2.Value, 2).ToString().Replace(".00", "");
                            policyTemplateModel.ThirdYearDepreciation = Math.Round(DepreciationValue3.Value, 2).ToString().Replace(".00", "");
                            policyTemplateModel.FourthYearDepreciation = Math.Round(DepreciationValue4.Value, 2).ToString().Replace(".00", "");
                            policyTemplateModel.FifthYearDepreciation = Math.Round(DepreciationValue5.Value, 2).ToString().Replace(".00", "");
                        }
                        #endregion
                    }
                }

                output.Output = policyTemplateModel;
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }
        public PolicyOutput GeneratePolicyManually(PolicyData policyInfo, string serverIP, string channel, string userName, bool isPdfServer, string domain, string remoteServerIP, string adminUsername, string adminPassword)
        {
            PolicyOutput output = new PolicyOutput();
            DateTime dtBefore = DateTime.Now;
            string exception = string.Empty;
            try
            {
                if (policyInfo == null)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = "policyInfo is null";
                    return output;
                }
                if (string.IsNullOrEmpty(policyInfo.PolicyNo))
                {
                    output.ErrorCode = 3;
                    output.ErrorDescription = "PolicyNo is empty";
                    return output;
                }
                if (string.IsNullOrEmpty(policyInfo.ReferenceId))
                {
                    output.ErrorCode = 4;
                    output.ErrorDescription = "ReferenceId is empty";
                    return output;
                }
                if (!policyInfo.PolicyIssuanceDate.HasValue)
                {
                    output.ErrorCode = 5;
                    output.ErrorDescription = "PolicyIssuanceDate is empty";
                    return output;
                }
                if (!policyInfo.PolicyEffectiveDate.HasValue)
                {
                    output.ErrorCode = 6;
                    output.ErrorDescription = "PolicyEffectiveDate is empty";
                    return output;
                }
                if (!policyInfo.PolicyExpiryDate.HasValue)
                {
                    output.ErrorCode = 7;
                    output.ErrorDescription = "PolicyExpiryDate is empty";
                    return output;
                }
                if (policyInfo.PolicyFile == null)
                {
                    output.ErrorCode = 8;
                    output.ErrorDescription = "PolicyFile is empty";
                    return output;
                }
                var checkoutDetails = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == policyInfo.ReferenceId && c.IsCancelled == false);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = 9;
                    output.ErrorDescription = "checkoutDetails is null";
                    return output;
                }
                if (checkoutDetails.PolicyStatusId == 4)
                {
                    output.ErrorCode = 10;
                    output.ErrorDescription = "Policy is already success";
                    return output;
                }
                PolicyResponse policy = new PolicyResponse();
                policy.ReferenceId = policyInfo.ReferenceId;
                policy.PolicyNo = policyInfo.PolicyNo;
                policy.PolicyEffectiveDate = policyInfo.PolicyEffectiveDate;
                policy.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                policy.PolicyFile = policyInfo.PolicyFile;
                policy.PolicyIssuanceDate = policyInfo.PolicyIssuanceDate;
                policy.PolicyFileUrl = policyInfo.PolicyFileUrl;
                policy.StatusCode = 1;
                var policyDetails = _policyRepository.Table.FirstOrDefault(a => a.CheckOutDetailsId == policy.ReferenceId && a.IsCancelled == false);

                if (policyDetails == null)
                {
                    var policyId = SavePolicy(policy, policyInfo.CompanyID);
                    if (!policyId.HasValue)
                    {
                        output.ErrorCode = 11;
                        output.ErrorDescription = "failed to save policy as it may be exist";
                        return output;
                    }
                    var invoice = _invoiceRepository.Table.FirstOrDefault(c => c.ReferenceId == policy.ReferenceId && c.IsCancelled == false);
                    if (invoice != null)
                    {
                        invoice.PolicyId = policyId;
                        _invoiceRepository.Update(invoice);
                    }
                    exception = string.Empty;
                    var vehicleInfo = _vehicleService.GetVehicleInfoById(checkoutDetails.VehicleId, out exception);
                    var driverInfo = _driver.TableNoTracking.Where(a => a.DriverId == checkoutDetails.MainDriverId).FirstOrDefault();
                    if (vehicleInfo != null && driverInfo != null)
                    {
                        string exp = string.Empty;
                        AddToPolicyRenewal(driverInfo.NIN, vehicleInfo.SequenceNumber, checkoutDetails, policy, invoice.TotalPrice.Value, out exp);
                    }
                }
                if (policyInfo.IsPolicyGeneratedByBCare)
                {
                    output.ErrorCode = 1;
                    output.ErrorDescription = "Success";
                    checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PolicyFileGeneraionFailure;
                    checkoutDetails.Channel = channel;
                    checkoutDetails.ModifiedDate = DateTime.Now;
                    _checkoutDetailRepository.Update(checkoutDetails);
                    return output;
                }
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                //PdfGenerationLog log = new PdfGenerationLog();
                //log.Channel = policyInfo.Channel;
                var insuranceCompany = _insuranceCompanyRepository.TableNoTracking.FirstOrDefault(
                  i => i.InsuranceCompanyID == policyInfo.CompanyID);

                var isAutoleasingPolicy = checkoutDetails.Channel == Channel.autoleasing.ToString().ToLower() ? true : false;
                string filePath = Utilities.SaveCompanyFileFromDashboard(policyInfo.ReferenceId, policyInfo.PolicyFile, insuranceCompany.Key, true, isPdfServer, domain, remoteServerIP, adminUsername, adminPassword, isAutoleasingPolicy, out exception);
                if (string.IsNullOrEmpty(filePath))
                {
                    output.ErrorCode = 15;
                    output.ErrorDescription = "Failed to save pdf file on server " + exception;
                    return output;
                }
                exception = string.Empty;
                Guid fileId = SavePolicyFile(policy, filePath, serverIP, out exception);
                if (fileId == Guid.Empty)
                {
                    output.ErrorCode = 16;
                    output.ErrorDescription = "Failed to save pdf file on database due to " + exception;
                    return output;
                }
                //SavePolicyFile(policy);
                var policyProcessingQueue = _policyProcessingQueue.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == policy.ReferenceId);
                if (policyProcessingQueue != null)
                {
                    policyProcessingQueue.ProcessedOn = DateTime.Now;
                    policyProcessingQueue.ModifiedDate = DateTime.Now;
                    policyProcessingQueue.ErrorDescription = "Success";
                    policyProcessingQueue.Chanel = channel;
                    policyProcessingQueue.UserName = userName;
                    DateTime dtAfter = DateTime.Now;
                    policyProcessingQueue.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    _policyProcessingQueue.Update(policyProcessingQueue);
                }
                SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                {
                    Channel = checkoutDetails.Channel,
                    Module = checkoutDetails.Channel.ToLower() == Channel.autoleasing.ToString().ToLower() ? Module.Autoleasing.ToString() : Module.Vehicle.ToString(),
                    Method = "PolicyFile",
                    PolicyResponseMessage = policy,
                    ReceiverEmailAddress = checkoutDetails.Email,
                    ReferenceId = policy.ReferenceId,
                    UserLanguage = lang,
                    PolicyFileByteArray = policy.PolicyFile,
                    IsPolicyGenerated = true,
                    IsShowErrors = false,
                    InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode
                };
                _policyEmailService.SendPolicyByMail(sendPolicyViaMailDto, insuranceCompany.Key);
                output.ErrorCode = 1;
                output.ErrorDescription = "Success";
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Available;
                // checkoutDetails.Channel = channel;
                checkoutDetails.ModifiedDate = DateTime.Now;
                _checkoutDetailRepository.Update(checkoutDetails);
                return output;
            }

            catch (Exception exp)
            {
                output.ErrorCode = 12;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public bool GenerateInvoicePdf(string referenceId, out string exception)
        {
            exception = string.Empty;
            try
            {
                bool isAutoleasingPolicy = false;
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(i => i.ReferenceId == referenceId);
                if (invoice != null)
                {
                    var quotation = _quotationService.GetInvoiceDataFromQuotationResponseByReferenceId(invoice.ReferenceId);
                    if (quotation != null)
                    {
                        var policy = _policyRepository.TableNoTracking.FirstOrDefault(a => a.CheckOutDetailsId == referenceId);
                        if (policy == null)
                        {
                            var checkout = _checkoutDetailRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == referenceId && a.PolicyStatusId != 1 && a.PolicyStatusId != 3 && a.IsCancelled == false);
                            if (checkout != null)
                            {
                                isAutoleasingPolicy = checkout.Channel.ToString().ToLower() == Channel.autoleasing.ToString().ToLower() ? true : false;
                                policy = new Tameenk.Core.Domain.Entities.Policy();
                                policy.CheckoutDetail = new CheckoutDetail();
                                policy.CheckoutDetail.Email = checkout.Email;
                                policy.CheckoutDetail.Phone = checkout.Phone;
                                policy.PolicyNo = "-";
                                policy.PolicyEffectiveDate = quotation.QuotationRequest?.RequestPolicyEffectiveDate?.Date;
                                policy.PolicyExpiryDate = quotation.QuotationRequest?.RequestPolicyEffectiveDate?.AddYears(1).AddDays(-1).Date;
                                policy.PolicyIssueDate = quotation.QuotationRequest.CreatedDateTime.Date;
                            }
                        }
                        if (policy != null)
                        {
                            var policyRequestMessage = new PolicyRequest();
                            policyRequestMessage.ReferenceId = invoice.ReferenceId;
                            policyRequestMessage.PaymentBillNumber = invoice.InvoiceNo.ToString();
                            policyRequestMessage.InsuredEmail = policy.CheckoutDetail?.Email;
                            policyRequestMessage.InsuredMobileNumber = policy.CheckoutDetail?.Phone;
                            PolicyResponse policyResponseMessage = new PolicyResponse();
                            policyResponseMessage.PolicyNo = policy.PolicyNo;
                            policyResponseMessage.PolicyEffectiveDate = policy.PolicyEffectiveDate;
                            policyResponseMessage.PolicyExpiryDate = policy.PolicyExpiryDate;
                            policyResponseMessage.PolicyIssuanceDate = policy.PolicyIssueDate;
                            byte[] invoiceFileAsByteArray = null;
                            Invoice generatedInvoice = _invoiceService.GenerateAndSaveInvoicePdf(policyRequestMessage, policyResponseMessage, Utilities.GetInternalServerIP(),
                                quotation.InsuranceCompany.NameAR,
                                quotation.InsuranceCompany.NameEN,
                                quotation.InsuranceCompany.Key,
                                quotation.QuotationRequest.Insured,
                                quotation.InsuranceCompany.VAT,
                                 quotation.QuotationRequest.Vehicle,
                                 isAutoleasingPolicy,
                                out invoiceFileAsByteArray, out exception);
                            return generatedInvoice != null ? true : false;
                            //SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
                            //{
                            //    PolicyResponseMessage = policyResponseMessage,
                            //    ReceiverEmailAddress = policy.CheckoutDetail?.Email,
                            //    ReferenceId = invoice.ReferenceId,
                            //    UserLanguage = LanguageTwoLetterIsoCode.Ar,
                            //    PolicyFileByteArray = null,
                            //    InvoiceFileByteArray = invoiceFileAsByteArray,
                            //    IsPolicyGenerated = true,
                            //    IsShowErrors = false,
                            //    TawuniyaFileUrl = "",
                            //    InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode
                            //};
                            //_policyEmailService.SendPolicyByMail(sendPolicyViaMailDto);
                            //return true;
                        }
                        else
                        {
                            exception = "policy is null";
                        }
                    }
                    else
                    {
                        exception = "quotation is null";
                    }
                }
                else
                {
                    exception = "invoice is null";
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }
        string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        public bool SendSmsRenewalNotifications(DateTime start, DateTime end, int notificationNo, string taskName)
        {
            SMSNotification log = new SMSNotification();
            log.StartDate = start;
            log.EndDate = end;
            log.NotificationNo = notificationNo;
            log.TaskName = taskName;
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                string exception = string.Empty;
                var policies = _checkoutContext.GetRenewalPolicies(start, end,notificationNo, out exception);
      //List<Implementation.Checkouts.RenewalPolicyInfo> policies = new List<Implementation.Checkouts.RenewalPolicyInfo>();
      //          Implementation.Checkouts.RenewalPolicyInfo policyinfo1 = new Implementation.Checkouts.RenewalPolicyInfo();
      //          policyinfo1.CarPlateNumber = 5723;
      //          policyinfo1.CarPlateText1 = "";
      //          policyinfo1.CarPlateText2 = "";
      //          policyinfo1.CarPlateText3 = "";
      //          policyinfo1.EnglishFirstName = "IBRAHIM";
      //          policyinfo1.ExternalId = "a84c79c28d23477";
      //          policyinfo1.FirstName = "ابراهيم";
      //          policyinfo1.MakerDescAr = "مرسيدس";
      //          policyinfo1.MakerDescEn = "Mercedes";
      //          policyinfo1.ModelDescAr = "اس اي ال560";
      //          policyinfo1.ModelDescEn = "اس اي ال560";
      //          policyinfo1.Phone = "966552660771";
      //          policyinfo1.PolicyEffectiveDate = new DateTime(2021, 06, 12, 20, 17, 24);
      //          policyinfo1.PolicyExpiryDate = new DateTime(2022, 06, 12, 20, 17, 24);
      //          policyinfo1.ReferenceId = "2a81542e8df1428";
      //          policyinfo1.SelectedLanguage = 1;
      //          policyinfo1.SequenceNumber = "490189600";
      //          policyinfo1.ModelYear = 2019;
      //          policyinfo1.Channel = "Portal";
      //          policies.Add(policyinfo1);

      //          Implementation.Checkouts.RenewalPolicyInfo policyinfo2 = new Implementation.Checkouts.RenewalPolicyInfo();
      //          policyinfo2.CarPlateNumber = 5723;
      //          policyinfo2.CarPlateText1 = "ه";
      //          policyinfo2.CarPlateText2 = "ك";
      //          policyinfo2.CarPlateText3 = "ح";
      //          policyinfo2.EnglishFirstName = "Mubark";
      //          policyinfo2.ExternalId = "7020d8e33c3e48a";
      //          policyinfo2.FirstName = "مبارك";
      //          policyinfo2.MakerDescAr = "هونداي";
      //          policyinfo2.MakerDescEn = "Hyundai";
      //          policyinfo2.ModelDescAr = "سوناتا";
      //          policyinfo2.ModelDescEn = "Sonata";
      //          policyinfo2.Phone = "966552660771";
      //          policyinfo2.PolicyEffectiveDate = new DateTime(2021, 06, 13, 00, 00, 00);
      //          policyinfo2.PolicyExpiryDate = new DateTime(2022, 06, 12, 00, 00, 00);
      //          policyinfo2.ReferenceId = "e157af8b4623451";
      //          policyinfo2.SelectedLanguage = 1;
      //          policyinfo2.SequenceNumber = "841066710";
      //          policyinfo2.Channel = "Portal";

      //          Implementation.Checkouts.RenewalPolicyInfo policyinfo3 = new Implementation.Checkouts.RenewalPolicyInfo();
      //          policyinfo3.CarPlateNumber = 5723;
      //          policyinfo3.CarPlateText1 = "ه";
      //          policyinfo3.CarPlateText2 = "ك";
      //          policyinfo3.CarPlateText3 = "ح";
      //          policyinfo3.EnglishFirstName = "Mubark";
      //          policyinfo3.ExternalId = "260d317b111143a";
      //          policyinfo3.FirstName = "مبارك";
      //          policyinfo3.MakerDescAr = "هونداي";
      //          policyinfo3.MakerDescEn = "Hyundai";
      //          policyinfo3.ModelDescAr = "سوناتا";
      //          policyinfo3.ModelDescEn = "Sonata";
      //          policyinfo3.Phone = "966552660771";
      //          policyinfo3.PolicyEffectiveDate = new DateTime(2021, 06, 13, 00, 00, 00);
      //          policyinfo3.PolicyExpiryDate = new DateTime(2022, 06, 12, 00, 00, 00);
      //          policyinfo3.ReferenceId = "dd19942d6a3843d";
      //          policyinfo3.SelectedLanguage = 1;
      //          policyinfo3.SequenceNumber = "589604210";
      //          policyinfo3.Channel = "Portal";

      //          policies.Add(policyinfo1);
      //          policies.Add(policyinfo2);
      //          policies.Add(policyinfo3);

                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "failed to GetRenewalPolicies due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                if (policies.Count == 0)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "GetRenewalPolicies count is 0";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                var skippedNumbers = _smsSkippedNumbers.TableNoTracking.ToList();
                var discountCode = _renewalDiscount.TableNoTracking
                                                   .Where(a => a.IsActive && a.MessageType == notificationNo && a.CodeType == 2 && (a.EndDate.HasValue && a.EndDate.Value >= end.Date))
                                                   .OrderByDescending(a => a.Id).FirstOrDefault();

                foreach (var p in policies)
                {
                    dtBeforeCalling = DateTime.Now;
                    if (p.Channel.ToLower() == "autoleasing")
                        continue;
                    if (skippedNumbers.Where(a => a.PhoneNo == p.Phone).FirstOrDefault() != null)
                        continue;
                    log = new SMSNotification();
                    log.TaskName = taskName;
                    log.StartDate = start;
                    log.EndDate = end;
                    log.NotificationNo = notificationNo;
                    string smsBody = string.Empty;
                    log.MobileNumber = p.Phone;
                    log.ReferenceId = p.ReferenceId;
                    exception = string.Empty;
                    string vehicleId = p.SequenceNumber == null ? p.CustomCardNumber : p.SequenceNumber;
                    log.SequenceNumber = p.SequenceNumber;
                    log.CustomCard = p.CustomCardNumber;
                    var policy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == p.ReferenceId).FirstOrDefault();
                    if (policy == null)
                    {
                        log.ErrorCode = 8;
                        log.ErrorDescription = "policy is null";
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    var info = SMSNotificationDataAccess.GetFromMSNotification(p.Phone, vehicleId, notificationNo, p.ReferenceId);
                    if (info != null && info.Count() > 0)
                    {
                        log.ErrorCode = 8;
                        log.ErrorDescription = "this referenceId: " + p.ReferenceId + "already sent before with total count " + info.Count() + " and notificationNo is " + notificationNo;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);
                        continue;
                    }
                    string make = string.Empty;
                    string model = string.Empty;
                    string VechilePlatInfo = string.Empty;
                    string url = string.Empty;
                    bool isCustomCardConverted = false;
                    if (!string.IsNullOrEmpty(p.CustomCardNumber))
                    {
                        CustomCardQueue customCardInfo = new CustomCardQueue();
                        customCardInfo.UserId = p.UserId;
                        customCardInfo.CustomCardNumber = p.CustomCardNumber;
                        customCardInfo.ModelYear = p.ModelYear;
                        customCardInfo.Channel = p.Channel;
                        customCardInfo.CompanyID = p.InsuranceCompanyId;
                        customCardInfo.CompanyName = p.InsuranceCompanyName;
                        customCardInfo.VehicleId = p.VehicleId;
                        customCardInfo.ReferenceId = p.ReferenceId;
                        customCardInfo.PolicyNo = p.PolicyNo;
                        var customCardOutput = _policyNotificationContext.GetCustomCardInfo(customCardInfo);
                        if (customCardOutput.ErrorCode != UpdateCustomCardOutput.ErrorCodes.Success)
                        {
                            log.ErrorCode = 500;
                            log.ErrorDescription = "falied to get custom card info due to " + customCardOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                        }
                        isCustomCardConverted = true;

                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText1))
                            p.CarPlateText1 = customCardOutput.CarPlateText1;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText2))
                            p.CarPlateText2 = customCardOutput.CarPlateText2;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText3))
                            p.CarPlateText3 = customCardOutput.CarPlateText3;
                        if (customCardOutput.CarPlateNumber.HasValue)
                            p.CarPlateNumber = customCardOutput.CarPlateNumber;

                        if (!string.IsNullOrEmpty(customCardOutput.SequenceNumber))
                        {
                            vehicleId = customCardOutput.SequenceNumber;
                            log.SequenceNumber = customCardOutput.SequenceNumber;
                        }
                    }
                    if (!string.IsNullOrEmpty(p.SequenceNumber) || isCustomCardConverted)
                    {
                        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                        {
                            Channel = p.Channel,
                            ServerIP = Utilities.GetInternalServerIP()
                        };
                        if (!string.IsNullOrEmpty(p.UserId))
                        {
                            Guid userIdGUID = Guid.Empty;
                            Guid.TryParse(p.UserId, out userIdGUID);
                            predefinedLogInfo.UserID = userIdGUID;
                        }
                        predefinedLogInfo.VehicleId = vehicleId;
                        predefinedLogInfo.DriverNin = p.CarOwnerNIN;
                        predefinedLogInfo.ReferenceId = p.ReferenceId;

                        var vehicleYakeenRequest = new VehicleYakeenRequestDto()
                        {
                            VehicleId = Convert.ToInt64(vehicleId),
                            VehicleIdTypeId = 1,
                            OwnerNin = p.OwnerTransfer ? Convert.ToInt64(p.NationalId) : Convert.ToInt64(p.CarOwnerNIN)
                        };
                        var driverInfo = _yakeenClient.GetCarInfoBySequenceInfo(vehicleYakeenRequest, predefinedLogInfo);
                        if (driverInfo.ErrorCode != YakeenOutput.ErrorCodes.Success)
                        {
                            policy.RenewalNotificationStatus = "SoldOut";
                            policy.NotificationNo = notificationNo;
                            _policyRepository.Update(policy);

                            log.ErrorCode = 15;
                            log.ErrorDescription = "can't send sms due to " + driverInfo.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                    }
                    DateTime beforeCallingDatabase = DateTime.Now;
                    var policyInfo = _vehicleService.GetVehiclePolicyDetails(vehicleId, out exception);
                    DateTime afterCallingDatabase = DateTime.Now;
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "GetVehiclePolicy returned exp: " + exception+"; time consumed for DB:"+ afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (policyInfo == null && isCustomCardConverted)
                    {
                        policyInfo = _vehicleService.GetVehiclePolicyDetails(p.CustomCardNumber, out exception);
                    }
                    if (policyInfo == null)
                    {
                        log.ErrorCode = 4;
                        log.ErrorDescription = "policyInfo is null" + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (!policyInfo.PolicyExpiryDate.HasValue)
                    {
                        policy.RenewalNotificationStatus = "PolicyExpiryDate is null";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 5;
                        log.ErrorDescription = "PolicyExpiryDate is null and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    if (policyInfo.PolicyExpiryDate.HasValue
                        && policyInfo.PolicyExpiryDate.Value.Date.Year > end.Year)
                    {
                        policy.IsRenewed = true;
                        policy.RenewalNotificationStatus = "AlreadyRenewed";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 6;
                        log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (policyInfo.PolicyExpiryDate.HasValue
                       && policyInfo.PolicyExpiryDate.Value.Date > end.AddMonths(5).Date)
                    {
                        policy.IsRenewed = true;
                        policy.RenewalNotificationStatus = "StillValid";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 16;
                        log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    bool sendSms = true;
                    if (p.PolicyExpiryDate < start)
                    {
                        sendSms = false;
                    }
                    if (p.PolicyExpiryDate > end)
                    {
                        sendSms = false;
                    }
                    log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    if (!sendSms)
                    {
                        log.ErrorCode = 7;
                        log.ErrorDescription = "don't send sms due to as p.PolicyExpiryDate is " + p.PolicyExpiryDate + " and start is " + start + " and enddate is " + end + " and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    var discountMsg = string.Empty;
                    if (discountCode != null)
                    {
                        VehicleDiscounts VehicleDiscountmodel = new VehicleDiscounts()
                        {
                            CreatedDate = DateTime.Now,
                            DiscountCode = discountCode.Code,
                            IsUsed = false,
                            SequenceNumber = p.SequenceNumber,
                            CustomCardNumber = p.CustomCardNumber,
                            Nin = p.CarOwnerNIN
                        };
                        string ex = string.Empty;
                        var CodeWithVech = LinkVechWithDiscountCode(VehicleDiscountmodel, out ex);
                        if (!string.IsNullOrEmpty(ex))
                        {
                            log.ErrorCode = 5;
                            log.ErrorDescription = "Error happend while insert Data Vehcile Discount , and the error is " + ex;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                        if (CodeWithVech != null && !string.IsNullOrEmpty(CodeWithVech.DiscountCode))
                        {
                            if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                            {
                                discountMsg = "ولأنك من عائلة بي كير نقدم لك كود خصم {0} صالح لمده 48 ساعة  يمكنك استخدامه في مرحلة الدفع لمرة واحدة";
                            }
                            else
                            {
                                discountMsg = "Because you are a member of BCare family, we are providing you with coupon discount {0} valid for 48 hours you can only use it once at the payment step";
                            }
                            discountMsg = discountMsg.Replace("{0}", CodeWithVech.DiscountCode);
                        }
                    }

                    string emailPlainText = string.Empty;

                    string emo_heart = DecodeEncodedNonAsciiCharacters("\uD83D\uDC99");
                    string emo_calender = DecodeEncodedNonAsciiCharacters("\uD83D\uDCC5");

                    if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                            || !string.IsNullOrEmpty(p.CarPlateText2)
                            || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                            p.CarPlateText2, p.CarPlateText3,
                            p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "ar");
                        }

                        //


                        if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                ///smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;

                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //  smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                           // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {

                            if (isCustomCardConverted)
                            {
                                //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;

                                emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa";
                               // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa";
                               
                                emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                           // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                     

                        if (notificationNo == 2)      // spcial case for whats App Body
                        {
                            if (isCustomCardConverted)
                            {
                     
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                      



                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescAr))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescAr);
                            emailPlainText = emailPlainText.Replace("[%Make%]", p.MakerDescAr);
                            make = p.MakerDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(p.ModelDescAr))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescAr);
                            emailPlainText = emailPlainText.Replace("[%Model%]", p.ModelDescAr);
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            string PolicyExpiryDate = "";
                            PolicyExpiryDate = "في ";
                            PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("en-US"));
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd", new System.Globalization.CultureInfo("ar-eg")) + " ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("MMMM", new System.Globalization.CultureInfo("ar-eg")) + ", ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("yyyy", new System.Globalization.CultureInfo("ar-eg"));
                            smsBody = smsBody.Replace("[%ExpiryDate%]", PolicyExpiryDate);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", PolicyExpiryDate);
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", string.Empty);
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else if (string.IsNullOrEmpty(discountMsg)&& smsBody.Contains("ينتهي تأمين مركبتك"))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "،");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "");
                        }
                    }
                    else
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                           || !string.IsNullOrEmpty(p.CarPlateText2)
                           || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                           p.CarPlateText2, p.CarPlateText3,
                           p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "en");
                        }
                        if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                              //  smsBody = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                              //  smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                           // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                               // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                           // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }

                        if (notificationNo == 2) // whats app body
                        {
                            if (isCustomCardConverted)
                            {
                         
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                 smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                //smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                 smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }


                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescEn))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescEn);
                            emailPlainText = emailPlainText.Replace("[%Make%]", p.MakerDescEn);
                            make = p.MakerDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.ModelDescEn))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescEn);
                            emailPlainText = emailPlainText.Replace("[%Model%]", p.ModelDescEn);
                            model = p.ModelDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", p.PolicyExpiryDate.ToString("dd/MM/yyyy", new CultureInfo("en-us")));
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", string.Empty);
                            model = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", ",");
                        }
                    }
                    smsBody = smsBody.Trim();
                    log.SMSMessage = smsBody;
                    exception = string.Empty;
                    if (notificationNo == 2) // whatisapp only with 14 days notification
                    {
                        var count = WhatsAppLogDataAccess.GetFromWhatsAppNotification(p.ReferenceId);
                        if (count == 0)
                        {
                            _notificationService.SendWhatsAppMessageForPolicyRenewalAsync(p.Phone, smsBody, make, model, VechilePlatInfo, url, SMSMethod.PolicyRenewal.ToString(), p.ReferenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), p.SelectedLanguage).ToLower(), p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                        }
                    }
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = p.Phone,
                        MessageBody = smsBody,
                        Method = SMSMethod.PolicyRenewal.ToString(),
                        Module = Module.Vehicle.ToString()
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode == 12)
                    {
                        log.ErrorCode = 10;
                        log.ErrorDescription = "sms failed to sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (smsOutput.ErrorCode != 0)
                    {
                        log.ErrorCode = 9;
                        log.ErrorDescription = "sms failed to sent: " + smsOutput.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }

                    //// send renewal email for verified email only
                    //if (p.IsEmailVerified)
                    //{
                    //    try
                    //    {
                    //        emailPlainText = emailPlainText.Trim();
                    //        string emailSubject = ((LanguageTwoLetterIsoCode)p.SelectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "اشعار تجديد وثيقة تأمين ( [%PolicyNo%] )" : "Policy Renewal Notification ( [%PolicyNo%] )";
                    //        if (!string.IsNullOrEmpty(p.PolicyNo))
                    //            emailSubject = emailSubject.Replace("[%PolicyNo%]", p.PolicyNo);
                    //        else
                    //            emailSubject = emailSubject.Replace("[%PolicyNo%]", string.Empty);

                    //        //if (emailPlainText.Contains("\n\n"))
                    //        //    emailPlainText = string.Format(emailPlainText.Replace("\n\n", "<br>"));
                            
                    //        MessageBodyModel messageBodyModel = new MessageBodyModel();
                    //        messageBodyModel.MessageBody = emailPlainText;
                    //        messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/Welcome.png";
                    //        messageBodyModel.Language = (((LanguageTwoLetterIsoCode)p.SelectedLanguage).ToString()).ToLower();

                    //        EmailModel emailModel = new EmailModel();
                    //        emailModel.ReferenceId = p.ReferenceId;
                    //        emailModel.To = new List<string>();
                    //        emailModel.To.Add(p.Email);
                    //        emailModel.Subject = emailSubject;
                    //        emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                    //        emailModel.Module = "Vehicle";
                    //        emailModel.Method = "RenewalNotification";
                    //        emailModel.Channel = Channel.Portal.ToString();
                    //        var smemailOutput = _notificationService.SendEmail(emailModel);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SendSmsRenewalNotifications_SendEmail_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", JsonConvert.SerializeObject(ex.ToString()));
                    //    }
                    //}

                    ////
                    /// stop checking for channel and try to send notification regardless policy channel
                    //if (AppNotificationChannels.Contains(p.Channel.ToLower()))
                    _notification.SendFireBaseNotification(p.UserId, "إشعار التجديد - Renewal Notification", smsBody, SMSMethod.PolicyRenewal.ToString(), p.ReferenceId, p.Channel);

                    policy.RenewalNotificationStatus = "Notification"+notificationNo+"Sent";
                    policy.NotificationNo = notificationNo;
                    _policyRepository.Update(policy);
                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SendSmsRenewalNotifications_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", JsonConvert.SerializeObject(exp.ToString()));
                log.ErrorCode = 11;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                SMSNotificationDataAccess.AddToSMSNotification(log);
                return false;
            }
        }

        public PdfGenerationOutput GetPolicyFile(string referenceId)
        {
            PdfGenerationOutput output = new PdfGenerationOutput();
            try
            {
                string companyName = string.Empty;
                output = GeneratePolicyFile(referenceId, out companyName);
                if (output.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
                {
                    return output;
                }
                string exception = string.Empty;
                string filePath = Utilities.SaveCompanyFileFromDashboard(referenceId, output.File, companyName, true, _tameenkConfig.RemoteServerInfo.UseNetworkDownload, _tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP, _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, false, out exception);
                if (string.IsNullOrEmpty(filePath))
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Failed to save pdf file on server";
                    return output;
                }
                PolicyResponse policyResponseMessage = new PolicyResponse();
                policyResponseMessage.ReferenceId = referenceId;
                Guid fileId = SavePolicyFile(policyResponseMessage, filePath, Utilities.GetInternalServerIP(), out exception);
                if (fileId == Guid.Empty)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "Failed to save pdf file on DB due to " + exception;
                    return output;
                }
                output.ErrorDescription = "Success";
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }
        private PdfGenerationOutput GeneratePolicyFile(string referenceId, out string companyName)
        {

            PdfGenerationOutput output = new PdfGenerationOutput();
            companyName = string.Empty;
            try
            {
                var policyProcessingQueue = _policyProcessingQueue.TableNoTracking.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (policyProcessingQueue == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "policyProcessingQueue is null";
                    return output;
                }
                PolicyResponse policyResponseMessage = null;
                if (policyProcessingQueue.CompanyName.ToLower() == "MedGulf".ToLower())
                {
                    var responseValue = JsonConvert.DeserializeObject<MedGulfPolicyResponse>(policyProcessingQueue.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        Errors = responseValue.Errors,
                        PolicyFileUrl = responseValue.PolicyFileUrl,
                        PolicyNo = responseValue.PolicyNo,
                        ReferenceId = responseValue.ReferenceId,
                        StatusCode = responseValue.StatusCode,
                        PolicyEffectiveDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyEffectiveDate),
                        PolicyExpiryDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyExpiryDate),
                        PolicyIssuanceDate = Utilities.ConvertStringToDateTimeFromMedGulf(responseValue.PolicyIssuanceDate)
                    };
                }
                else if (policyProcessingQueue.CompanyName.ToLower() == "Tawuniya".ToLower() && policyProcessingQueue.Chanel != Channel.autoleasing.ToString())
                {
                    var result = JsonConvert.DeserializeObject<Integration.Providers.Tawuniya.Dtos.PolicyResponseDto>(policyProcessingQueue.ServiceResponse);
                    policyResponseMessage = new PolicyResponse
                    {
                        StatusCode = 1,
                        ReferenceId = referenceId,
                        PolicyNo = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyNumber,
                        PolicyFileUrl = result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyDocPrintingLink,
                        PolicyExpiryDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyExpiryDate, "yyyy-MM-dd", new CultureInfo("en-US")),
                        PolicyIssuanceDate = DateTime.ParseExact(result.CreatePolicyResponse.PolicyResponse.PolicyInfo.PolicyInceptionDate, "yyyy-MM-dd", new CultureInfo("en-US"))
                    };

                }
                else
                {
                    policyResponseMessage = JsonConvert.DeserializeObject<PolicyResponse>(policyProcessingQueue.ServiceResponse);
                }
                if (string.IsNullOrEmpty(policyResponseMessage.ReferenceId))
                    policyResponseMessage.ReferenceId = referenceId;
                var checkoutDetails = _checkoutDetailRepository.Table.FirstOrDefault(c => c.ReferenceId == referenceId);
                if (checkoutDetails == null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "checkoutDetails is null";
                    return output;
                }
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                companyName = checkoutDetails.InsuranceCompanyName;
                byte[] policyFileByteArray = null;
                if (!string.IsNullOrEmpty(policyResponseMessage.PolicyFileUrl))
                {
                    string fileURL = policyResponseMessage.PolicyFileUrl;
                    fileURL = fileURL.Replace(@"\\", @"//");
                    fileURL = fileURL.Replace(@"\", @"/");
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        policyFileByteArray = client.DownloadData(fileURL);
                        if (policyFileByteArray == null)
                        {
                            output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
                            output.ErrorDescription = "policy FileByte Array is returned null";
                            output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;
                            return output;
                        }
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                        output.ErrorDescription = "Success";
                        output.EPolicyStatus = EPolicyStatus.Available;
                        output.File = policyFileByteArray;
                        return output;
                    }
                }
                if (policyResponseMessage.PolicyFile != null)
                {
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.File = policyResponseMessage.PolicyFile;
                    return output;
                }
                PdfGenerationLog log = new PdfGenerationLog();
                log.Channel = checkoutDetails.Channel;
                log.ReferenceId = referenceId;
                log.CompanyID = checkoutDetails.InsuranceCompanyId;
                log.CompanyName = checkoutDetails.InsuranceCompanyName;
                log.ServerIP = Utilities.GetInternalServerIP();
                var policyFile = GeneratePolicyFileFromPolicyDetails(policyResponseMessage, checkoutDetails.InsuranceCompanyId.Value, lang, log);
                if (policyFile.ErrorCode != PdfGenerationOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = policyFile.ErrorCode;
                    output.ErrorDescription = policyFile.ErrorDescription;
                    output.EPolicyStatus = EPolicyStatus.PolicyFileGeneraionFailure;
                    return output;
                }
                output.ErrorDescription = "Success";
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                output.File = policyFile.File;
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public bool ResendPolicyFileByMail(byte[] file, string email, string policyNo, string referenceId, string companyKey, int langCode)
        {
            LanguageTwoLetterIsoCode userLanguage = (langCode == 2) ? LanguageTwoLetterIsoCode.En : LanguageTwoLetterIsoCode.Ar;
            PolicyResponse policyResponseMessage = new PolicyResponse() { PolicyNo = policyNo };

            SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
            {
                Channel = "Portal",
                Module = Module.Vehicle.ToString(),
                Method = "PolicyFile",
                PolicyResponseMessage = policyResponseMessage,
                ReceiverEmailAddress = email,
                ReferenceId = referenceId,
                UserLanguage = userLanguage,
                PolicyFileByteArray = file,
                InvoiceFileByteArray = null,
                IsPolicyGenerated = true,
                IsShowErrors = false,
            };
            var emailOutput = _policyEmailService.SendPolicyByMail(sendPolicyViaMailDto, companyKey);
            if (emailOutput.ErrorCode==EmailOutput.ErrorCodes.Success)
                return true;
            else
                return false;
        }

        public PdfGenerationOutput ReplacePolicyFile(byte[] policyFile, string filePath, string serverIP, bool isPdfServer, string adminUsername, string adminDomain, string adminPassword)
        {
            PdfGenerationOutput output = new PdfGenerationOutput();
            try
            {
                if (isPdfServer)
                {
                    string exception = string.Empty;
                    string generatedReportDirPath = Path.GetDirectoryName(filePath);

                    FileNetworkShare fileShare = new FileNetworkShare();
                    if (fileShare.UploadFileToShare(adminDomain, adminUsername, adminPassword, generatedReportDirPath, filePath, policyFile, serverIP, out exception))
                    {
                        output.ErrorDescription = "Success";
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                        return output;
                    }
                    else
                    {
                        output.ErrorDescription = "Failed to replace old file due to "+exception;
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                        return output;
                    }
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
                        output.ErrorDescription = "File Path not exist";
                        return output;
                    }
                    File.Delete(filePath);

                    File.WriteAllBytes(filePath, policyFile);
                    output.ErrorDescription = "Success";
                    output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
                    return output;
                }
            }
            catch (Exception exp)
            {
                output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }

        public void GetAndSendPolicyInformationForRoadAssistance()
        {
            try
            {
                var policyInformation = _checkoutContext.GetPolicyInformationForRoadAssistance();
                if (policyInformation != null)
                {
                    byte[] file = _excelService.GeneratePolicyInformationForRoadAssistance(policyInformation);
                    if (file != null)
                    {
                        System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
                        toCollection.Add("malmutlaq@bcare.com.sa");
                        //toCollection.Add("kalmutlaq@bcare.com.sa");
                        toCollection.Add("adel.h@bcare.com.sa");
                        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                        mail.IsBodyHtml = true;
                        //mail.Body = body;
                        mail.Body = "Dears  <br/> Kindly find the policy information from " + DateTime.Now.AddDays(-2).ToString("dddd, dd MMMM yyyy") + " to " + DateTime.Now.AddDays(-1).ToString("dddd, dd MMMM yyyy") + " for Road Assistance  <br/> Thanks";
                        mail.Subject = "Policies::Road Assistance::From " + DateTime.Now.AddDays(-2).ToString("dddd, dd MMMM yyyy") + " TO " + DateTime.Now.AddDays(-1).ToString("dddd, dd MMMM yyyy");
                        mail.From = new System.Net.Mail.MailAddress("Noreply-bcare2@bcare.com.sa");
                        System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(new MemoryStream(file), "policyInformation.xlsx");
                        mail.Attachments.Add(att);
                        for (int i = 0; i < toCollection.Count; i++)
                        {
                            mail.To.Add(toCollection[i]);
                        }
                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = "smtp.office365.com";
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("Noreply-bcare2@bcare.com.sa", "Bcare@2019");
                        smtp.Port = 587;
                        smtp.Send(mail);
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask3Exp.txt", "mail sent");
                    }
                    else
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask2Exp.txt", "file is null");
                    }
                }
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask1Exp.txt", exp.ToString());
            }
        }



        public void GetAndSubmitAllFailedMorniRequests()
        {
            try
            {
                var policyInformation = _checkoutContext.GetAllFailedMorniRequests();
                if (policyInformation != null)
                {
                    foreach (var itm in policyInformation)
                    {
                        MorniContext morniContext = new MorniContext(_morniRequestRepository);
                        if (morniContext.CreateMorniMembership(itm.ReferenceId, itm.Channel))
                        {
                            string smsBody = string.Empty;
                            if (itm.Lang == 1)
                            {
                                smsBody = "عزيزى العميل سيتم تفعيل خدمة المساعدة على الطريق مجانا مع بداية التأمين";
                                smsBody += " " + "https://bcare.com.sa/rsa.pdf";
                            }
                            else
                            {
                                smsBody = "Dear customer, the roadside assistance service will be activated for free with the start of the insurance";
                                smsBody += " " + "https://bcare.com.sa/rsa.pdf";
                            }
                            string ex = string.Empty;
                            var smsModel = new SMSModel()
                            {
                                PhoneNumber = itm.Phone,
                                MessageBody = smsBody,
                                Method = SMSMethod.MorniSMS.ToString(),
                                Module = Module.Vehicle.ToString(),
                                Channel = itm.Channel
                            };
                            _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask1Exp.txt", exp.ToString());
            }
        }
        public void SendMorniSMS()
        {
            try
            {
                var items = _morniSMSRepository.TableNoTracking.Where(a => a.IsSMSSent == false).ToList();
                if (items != null && items.Count > 0)
                {
                    string exception = string.Empty;
                    foreach (var item in items)
                    {
                        string smsBody = "تم تفعيل عضويتك في مرني للمساعدة على الطريق والمقدمة من بي كير للمركبة هيكل رقم [%CHASSISNUMBER%] ";
                        smsBody += "https://bcare.com.sa/rsa.pdf";

                        string smsBodyEn = "Your membership with MORNI by BCare is activated for Vehicle [%CHASSISNUMBER%] ";
                        smsBodyEn += "https://bcare.com.sa/rsa.pdf";

                        if (item.Lang.HasValue && item.Lang == 1)
                        {
                            smsBody = smsBody.Replace("[%CHASSISNUMBER%]", item.vin);
                            var smsModel = new SMSModel()
                            {
                                PhoneNumber = item.OwnerMobilePhone.ToString(),
                                MessageBody = smsBody,
                                Method = SMSMethod.MorniSMS.ToString(),
                                Module = Module.Vehicle.ToString()
                            };
                            var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                            if (smsOutput.ErrorCode == 0)
                            {
                                var itemInfo = _morniSMSRepository.Table.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (itemInfo != null)
                                {
                                    itemInfo.IsSMSSent = true;
                                    _morniSMSRepository.Update(itemInfo);
                                }
                            }
                        }
                        else if (item.Lang.HasValue && item.Lang == 2)
                        {
                            smsBodyEn = smsBodyEn.Replace("[%CHASSISNUMBER%]", item.vin);
                            var smsModel = new SMSModel()
                            {
                                PhoneNumber = item.OwnerMobilePhone.ToString(),
                                MessageBody = smsBodyEn,
                                Method = SMSMethod.MorniSMS.ToString(),
                                Module = Module.Vehicle.ToString()
                            };
                            var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                            if (smsOutput.ErrorCode == 0)
                            {
                                var itemInfo = _morniSMSRepository.Table.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (itemInfo != null)
                                {
                                    itemInfo.IsSMSSent = true;
                                    _morniSMSRepository.Update(itemInfo);
                                }
                            }
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%CHASSISNUMBER%]", item.vin);
                            smsBodyEn = smsBodyEn.Replace("[%CHASSISNUMBER%]", item.vin);
                            var smsModel = new SMSModel()
                            {
                                PhoneNumber = item.OwnerMobilePhone.ToString(),
                                MessageBody = smsBody,
                                Method = SMSMethod.MorniSMS.ToString(),
                                Module = Module.Vehicle.ToString()
                            };
                            var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                            if (smsOutput.ErrorCode == 0)
                            {
                                smsModel = new SMSModel()
                                {
                                    PhoneNumber = item.OwnerMobilePhone.ToString(),
                                    MessageBody = smsBodyEn,
                                    Method = SMSMethod.MorniSMS.ToString(),
                                    Module = Module.Vehicle.ToString()
                                };
                                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                                var itemInfo = _morniSMSRepository.Table.Where(a => a.ID == item.ID).FirstOrDefault();
                                if (itemInfo != null)
                                {
                                    itemInfo.IsSMSSent = true;
                                    _morniSMSRepository.Update(itemInfo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask22Exp.txt", "items is null");
                }
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\TempTask22Exp.txt", exp.ToString());
            }
        }

        private bool IsValidPdfFile(string filePath)
        {
            try
            {
                PdfReader reader = new PdfReader(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GnerateInvoiceRecord(string referenceId, int paymentMethodId, short insuranceTypeCode, int insuranceCompanyId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var invoice = _invoiceRepository.Table.OrderByDescending(e => e.Id).FirstOrDefault(i => i.ReferenceId == referenceId);
                if (invoice == null)
                {
                    if (paymentMethodId == 2)
                    {
                        var sadad = _sadadRequest.TableNoTracking.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
                        if (sadad != null)
                        {
                            int invoiceNumber = 0;
                            int.TryParse(sadad.CustomerAccountNumber.Substring(2, sadad.CustomerAccountNumber.Length - 2), out invoiceNumber);
                            _orderService.CreateInvoice(referenceId, insuranceTypeCode, insuranceCompanyId, invoiceNumber);
                        }
                        else
                        {
                            _orderService.CreateInvoice(referenceId, insuranceTypeCode, insuranceCompanyId);
                        }
                    }
                    else
                    {
                        _orderService.CreateInvoice(referenceId, insuranceTypeCode, insuranceCompanyId);
                    }
                    return true;
                }
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public bool CheckQuotationStatusAndSendNotification(int insuranceTypeCode, bool isAutoLease, string method)
        {
            List<string> phonesList = new List<string>();
            phonesList.Add("966552660771");
            phonesList.Add("966545000528");
            phonesList.Add("966599000578");
            phonesList.Add("966552127776");

            //phonesList.Add("966504799991");
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).AddMinutes(-5);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            NotificationLog log = new NotificationLog();
            log.StartDate = start;
            log.EndDate = end;
            log.Method = method;
            //log.Phone = JsonConvert.SerializeObject(phonesList);// "966552660771";
            log.InsuranceTypeCode = insuranceTypeCode;
            if (isAutoLease)
            {
                log.Channel = "autoleasing";
            }
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                List<InsuranceCompany> companies = new List<InsuranceCompany>();
                if (isAutoLease && insuranceTypeCode == 2)
                    companies = _insuranceCompanyRepository.Table.Where(a => a.IsActive == true && a.IsActiveAutoleasing == true).OrderBy(a => a.Order).ToList();
                else if (insuranceTypeCode == 2)
                    companies = _insuranceCompanyRepository.Table.Where(a => a.IsActive == true && a.IsActiveComprehensive == true).OrderBy(a => a.Order).ToList();
                else
                    companies = _insuranceCompanyRepository.Table.Where(a => a.IsActive == true && a.IsActiveTPL == true).OrderBy(a => a.Order).ToList();

                if (companies == null || companies.Count() == 0)
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "companies is null";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                    return false;
                }

                foreach (var company in companies)
                {
                    if (company.InsuranceCompanyID == 12 && insuranceTypeCode == 2 && !isAutoLease)// we sent only one request to tawuniya 
                    {
                        continue;
                    }
                    if (company.InsuranceCompanyID == 12 && isAutoLease)
                    {
                        method = "AutoleasingProposal";
                    }
                    else if (company.InsuranceCompanyID == 12 && !isAutoLease)
                    {
                        method = "Proposal";
                    }
                    else if (company.InsuranceCompanyID != 12 && isAutoLease)
                    {
                        method = "AutoleasingQuotation";
                    }
                    else
                    {
                        method = "Quotation";
                    }
                    log = new NotificationLog();
                    log.StartDate = start;
                    log.EndDate = end;
                    log.Method = method;
                    //log.Phone = JsonConvert.SerializeObject(phonesList);
                    log.InsuranceTypeCode = insuranceTypeCode;
                    if (isAutoLease)
                    {
                        log.Channel = "autoleasing";
                    }
                    dtBeforeCalling = DateTime.Now;
                    string exception = string.Empty;
                    string message = string.Empty;
                    log.CompanyName = company.Key;
                    log.CompanyId = company.InsuranceCompanyID;
                    List<IServiceRequestLog> services = new List<IServiceRequestLog>();
                    if (isAutoLease)
                        services = ServiceRequestLogDataAccess.GetServiceRequestLog(company.Key, method, insuranceTypeCode, start, end, true, 1, out exception);
                    else
                        services = ServiceRequestLogDataAccess.GetServiceRequestLog(company.Key, method, insuranceTypeCode, start, end, false, 1, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 2;
                        log.ErrorDescription = "failed to GetServiceRequestLog due to " + exception;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    if (services != null && services.Count() > 0)
                    {
                        //log.ErrorCode = 1;
                        //log.ErrorDescription = "Success And No Failed Quotation";
                        //log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        //NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }

                    if (isAutoLease)
                        message = company.Key + " Autolease Quotation service is down from: " + start.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) + " To:" + end.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));
                    else if (insuranceTypeCode == 2)
                        message = company.Key + " Comprehensive Quotation service is down from: " + start.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) + " To:" + end.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));
                    else
                        message = company.Key + " TPL Quotation service is down from: " + start.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")) + " To:" + end.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));

                    log.Message = message;
                    foreach (string phone in phonesList)
                    {
                        log.Phone = phone;
                        exception = string.Empty;
                        if (NotificationLogDataAccess.CheckIfMessageSentBefore(phone, log.CompanyName, method, insuranceTypeCode, out exception))
                        {
                            log.ErrorCode = 3;
                            log.ErrorDescription = "Message sent before";
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            NotificationLogDataAccess.AddToNotificationLog(log);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(exception))
                        {
                            log.ErrorCode = 5;
                            log.ErrorDescription = "CheckIfMessageSentBefore exception: " + exception;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            NotificationLogDataAccess.AddToNotificationLog(log);
                            continue;
                        }
                        exception = string.Empty;
                        var smsModel = new SMSModel()
                        {
                            PhoneNumber = phone,
                            MessageBody = message,
                            Method = SMSMethod.SystemFailure.ToString(),
                            Module = Module.Vehicle.ToString(),
                            Channel = log.Channel
                        };
                        var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        if (smsOutput.ErrorCode == 12)
                        {
                            log.ErrorCode = 6;
                            log.ErrorDescription = "sms exception: " + exception;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            NotificationLogDataAccess.AddToNotificationLog(log);
                            continue;
                        }
                        if (smsOutput.ErrorCode != 0)
                        {
                            log.ErrorCode = 7;
                            log.ErrorDescription = "sms failed to sent";
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            NotificationLogDataAccess.AddToNotificationLog(log);
                            continue;
                        }
                        log.ErrorCode = 0;
                        log.ErrorDescription = "Success and message sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                    }

                }
                return true;
            }
            catch (Exception exp)
            {
                log.ErrorCode = 8;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                NotificationLogDataAccess.AddToNotificationLog(log);
                return false;
            }
        }

        public bool CheckServicesStatusAndSendNotification(string method)
        {
            List<string> phonesList = new List<string>();
            phonesList.Add("966552660771");
            phonesList.Add("966545000528");
            phonesList.Add("966552010916");
            phonesList.Add("966552127776");
            //phonesList.Add("966504799991");
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).AddMinutes(-10);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            NotificationLog log = new NotificationLog();
            log.StartDate = start;
            log.EndDate = end;
            log.Method = method;
            //log.Phone = JsonConvert.SerializeObject(phonesList);
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                string exception = string.Empty;
                List<IServiceRequestLog> services = ServiceRequestLogDataAccess.GetServiceStatusFromServiceRequestLog(method, 1, start, end, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "failed to GetServiceStatusFromServiceRequestLog due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                    return false;
                }
                if (services != null && services.Count() > 0)
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "Success And No Failed Service";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                    return true;
                }
                string message = "Please note no success response from " + method + " over 10 minutes ago so it may be down or no service calls check urgently from:" + start.ToString("dddd, dd MMMM yyyy") + "; To:" + end.ToString("dddd, dd MMMM yyyy");
                log.Message = message;
                foreach (string phone in phonesList)
                {
                    log.Phone = phone;
                    exception = string.Empty;
                    if (NotificationLogDataAccess.CheckIfMessageSentBefore(phone, string.Empty, method, 0, out exception))
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "Message sent before";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 5;
                        log.ErrorDescription = "CheckIfMessageSentBefore exception: " + exception;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    exception = string.Empty;
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = phone,
                        MessageBody = message,
                        Method = SMSMethod.SystemFailure.ToString(),
                        Module = Module.Vehicle.ToString(),
                        Channel = log.Channel
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode == 12)
                    {
                        log.ErrorCode = 6;
                        log.ErrorDescription = "sms exception: " + exception;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    if (smsOutput.ErrorCode != 0)
                    {
                        log.ErrorCode = 7;
                        log.ErrorDescription = "sms failed to sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }

                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success and message sent";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                log.ErrorCode = 8;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                NotificationLogDataAccess.AddToNotificationLog(log);
                return false;
            }
        }

        public List<AutoleasingPolicyReportInfoModel> GetAutoleasingPolicyReport(AutoleasingPolicyReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            try
            {
                var mainData = GetAutoleasingPolicyReportMainData(filter, bankId, pageIndex, pageSize, out totalCount, out exception);
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


        public List<AutoleasingPolicyReportInfoModel> GetAutoleasingPolicyReportMainData(AutoleasingPolicyReportFilter filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingPolicyReport";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (!string.IsNullOrEmpty(filter.PolicyNumber))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@quotationNumber", Value = filter.PolicyNumber });
                }

                if (!string.IsNullOrEmpty(filter.QuotationNumber))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@quotationNumber", Value = filter.QuotationNumber });
                }

                if (filter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyId", Value = filter.InsuranceCompanyId.Value });
                }

                //if (filter.NajmStatus.HasValue)
                //{
                //    command.Parameters.Add(new SqlParameter() { ParameterName = "najmStatus", Value = filter.NajmStatus.Value });
                //}

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
                List<AutoleasingPolicyReportInfoModel> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleasingPolicyReportInfoModel>(reader).ToList();

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

        //public PdfGenerationOutput GetWalaaFailedPolicyFile(CheckoutDetail checkoutDetails, string referenceId, string url)
        //{
        //    PdfGenerationOutput output = new PdfGenerationOutput();
        //    PdfGenerationLog log = new PdfGenerationLog();
        //    log.CreatedDate = DateTime.Now;
        //    log.CompanyID = 7;
        //    log.CompanyName = "Walaa";
        //    log.ServiceRequest = url;
        //    log.ReferenceId = referenceId;
        //    log.ServerIP = ServicesUtilities.GetServerIP();
        //    log.Channel = checkoutDetails.Channel;

        //    try
        //    {
        //        var policyData = _policyRepository.TableNoTracking.Where(a => a.CheckOutDetailsId == referenceId).FirstOrDefault();
        //        if (policyData == null)
        //        {
        //            output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
        //            output.ErrorDescription = "no policy data with this referenceId: " + referenceId;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
        //            return output;
        //        }

        //        byte[] policyFileByteArray = null;
        //        string fileURL = url;
        //        fileURL = fileURL.Replace(@"\\", @"//");
        //        fileURL = fileURL.Replace(@"\", @"/");
        //        log.ServiceURL = fileURL;

        //        using (System.Net.WebClient client = new System.Net.WebClient())
        //        {
        //            policyFileByteArray = client.DownloadData(fileURL);
        //            if (policyFileByteArray == null)
        //            {
        //                output.ErrorCode = PdfGenerationOutput.ErrorCodes.NullResponse;
        //                output.ErrorDescription = "policy FileByte Array is returned null";
        //                output.EPolicyStatus = EPolicyStatus.PolicyFileDownloadFailure;

        //                log.ErrorCode = (int)output.ErrorCode;
        //                log.ErrorDescription = output.ErrorDescription;
        //                PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
        //                return output;
        //            }
        //        }

        //        string filePath = Utilities.SaveCompanyFile(referenceId, policyFileByteArray, "Walaa", true);
        //        if (string.IsNullOrEmpty(filePath))
        //        {
        //            output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceError;
        //            output.ErrorDescription = "Failed to save pdf file on server";
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
        //            return output;
        //        }

        //        var policyFile = _policyFileRepository.Table.Where(a => a.ID == policyData.PolicyFileId).FirstOrDefault();
        //        if (policyFile == null)
        //        {
        //            output.ErrorCode = PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
        //            output.ErrorDescription = "no policy file data with this FileId: " + policyData.PolicyFileId;
        //            log.ErrorCode = (int)output.ErrorCode;
        //            log.ErrorDescription = output.ErrorDescription;
        //            PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
        //            return output;
        //        }

        //        policyFile.FilePath = filePath;
        //        _policyFileRepository.Update(policyFile);

        //        PolicyResponse policyResponseMessage = new PolicyResponse();
        //        policyResponseMessage.PolicyNo = policyData.PolicyNo;
        //        policyResponseMessage.ReferenceId = referenceId;
        //        policyResponseMessage.PolicyFileUrl = url;
        //        policyResponseMessage.PolicyFile = policyFileByteArray;

        //        SendPolicyViaMailDto sendPolicyViaMailDto = new SendPolicyViaMailDto()
        //        {
        //            PolicyResponseMessage = policyResponseMessage,
        //            ReceiverEmailAddress = checkoutDetails.Email,
        //            ReferenceId = referenceId,
        //            UserLanguage = (checkoutDetails.SelectedLanguage.HasValue) ? checkoutDetails.SelectedLanguage.Value : LanguageTwoLetterIsoCode.Ar,
        //            PolicyFileByteArray = policyFileByteArray,
        //            IsPolicyGenerated = true,
        //            IsShowErrors = false,
        //            TawuniyaFileUrl = "",
        //            InsuranceTypeCode = checkoutDetails.SelectedInsuranceTypeCode
        //        };
        //        if (!_policyEmailService.SendPolicyByMailForWalaa(sendPolicyViaMailDto, "Walaa"))
        //        {
        //            output.ErrorDescription = "Partial Success Failed to send email to client";
        //        }
        //        else
        //        {
        //            output.ErrorDescription = "Success";
        //        }
        //        string smsBody = string.Empty;
        //        string policyFileUrl = "https://bcare.com.sa/Identityapi/api/u/p?r=" + referenceId;
        //        string shortUrl = Utilities.GetShortUrl(policyFileUrl);
        //        if (!string.IsNullOrEmpty(shortUrl))
        //            policyFileUrl = shortUrl;

        //        if (checkoutDetails.SelectedLanguage == LanguageTwoLetterIsoCode.En)
        //        {
        //            smsBody = "Dear Customer, we would like to apologize about technical error from the insurance company applied to your insurance policy. Please find the attached insurance policy according to your request at purchasing time. ";
        //            smsBody += policyFileUrl;/// smsBody.Replace("[%PolicyFileUrl%]", policyFileUrl);
        //        }
        //        else
        //        {
        //            smsBody = "عزيزي العميل نود تقديم إعتذارنا لوجود خطأ في الوثيقة المستلمه مسبقاً بسبب خطأ فني في نظام شركة التأمين، مرفق لكم وثيقة التأمين الخاصة بكم حسب طلبكم عند اصدار الوثيقة";
        //            smsBody += " " + policyFileUrl;
        //        }
        //        var smsModel = new SMSModel()
        //        {
        //            PhoneNumber = checkoutDetails.Phone,
        //            MessageBody = smsBody,
        //            Method = SMSMethod.PolicyFile.ToString(),
        //            Module = Module.Vehicle.ToString(),
        //            Channel = checkoutDetails.Channel,
        //            ReferenceId = checkoutDetails.ReferenceId
        //        };
        //        var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
        //        if (smsOutput.ErrorCode == 0)
        //        {
        //            output.ErrorCode = PdfGenerationOutput.ErrorCodes.Success;
        //            output.ErrorDescription = "Success";
        //        }
        //        else
        //        {
        //            output.ErrorCode = PdfGenerationOutput.ErrorCodes.NoFileUrlNoFileData;
        //            output.ErrorDescription = smsOutput.ErrorDescription;
        //        }
        //        return output;
        //    }
        //    catch (Exception exp)
        //    {
        //        output.ErrorCode = PdfGenerationOutput.ErrorCodes.ServiceException;
        //        output.ErrorDescription = exp.ToString();
        //        log.ErrorCode = (int)output.ErrorCode;
        //        log.ErrorDescription = output.ErrorDescription;
        //        PdfGenerationLogDataAccess.AddtoPdfGenerationLog(log);
        //        return output;
        //    }
        //}

        public bool AddToPolicyRenewal(string nin, string sequenceNumber, CheckoutDetail checkoutDetais, PolicyResponse policyResponseMessage, decimal totalPrice, out string exception)
        {
            exception = string.Empty;
            try
            {
                DateTime? startDate = DateTime.Now;
                DateTime? endDate = DateTime.Now;
                if (policyResponseMessage.PolicyIssuanceDate.HasValue)
                {
                    startDate = policyResponseMessage.PolicyIssuanceDate.Value.AddDays(-30);
                    endDate = policyResponseMessage.PolicyIssuanceDate.Value.AddDays(30);
                }
                else if (policyResponseMessage.PolicyEffectiveDate.HasValue)
                {
                    startDate = policyResponseMessage.PolicyEffectiveDate.Value.AddDays(-30);
                    endDate = policyResponseMessage.PolicyEffectiveDate.Value.AddDays(30);
                }
                else
                {
                    startDate = checkoutDetais.CreatedDateTime.Value.AddDays(-30);
                    endDate = checkoutDetais.CreatedDateTime.Value.AddDays(30);
                }
                var previousPolicyDetails = GetPreviousPolicyDetails(nin, sequenceNumber, startDate.Value, endDate.Value, out exception);
                if (previousPolicyDetails == null)
                    return false;

                var renewalInfo = _renewalPolicies.TableNoTracking.Where(a => a.OldReferenceId == previousPolicyDetails.ReferenceId && a.NewReferenceId == checkoutDetais.ReferenceId).FirstOrDefault();
                if (renewalInfo == null)
                {
                    RenewalPolicies renewalPolicies = new RenewalPolicies();
                    renewalPolicies.VehicleId = sequenceNumber;

                    renewalPolicies.OldNin = previousPolicyDetails.NIN;
                    renewalPolicies.OldPolicyNo = previousPolicyDetails.PolicyNo;
                    renewalPolicies.OldReferenceId = previousPolicyDetails.ReferenceId;
                    renewalPolicies.OldPolicyIssueDate = previousPolicyDetails.PolicyIssueDate;
                    renewalPolicies.OldPolicyEffectiveDate = previousPolicyDetails.PolicyEffectiveDate;
                    renewalPolicies.OldPolicyExpiryDate = previousPolicyDetails.PolicyExpiryDate;
                    if (previousPolicyDetails.InsuranceCompanyId.HasValue)
                    {
                        renewalPolicies.OldCompanyId = previousPolicyDetails.InsuranceCompanyId.Value;
                    }
                    renewalPolicies.OldCompanyKey = previousPolicyDetails.InsuranceCompanyName;
                    if (previousPolicyDetails.SelectedInsuranceTypeCode.HasValue)
                    {
                        renewalPolicies.OldProductType = previousPolicyDetails.SelectedInsuranceTypeCode;
                    }
                    renewalPolicies.OldTotalPrice = previousPolicyDetails.TotalPrice;
                    renewalPolicies.OldChannel = previousPolicyDetails.Channel;
                    renewalPolicies.OldCheckoutCreatedDate = previousPolicyDetails.CheckoutCreatedDate;

                    renewalPolicies.NewNin = nin;
                    renewalPolicies.NewPolicyNo = policyResponseMessage.PolicyNo;
                    renewalPolicies.NewReferenceId = checkoutDetais.ReferenceId;
                    renewalPolicies.NewPolicyIssueDate = policyResponseMessage.PolicyIssuanceDate;
                    renewalPolicies.NewPolicyEffectiveDate = policyResponseMessage.PolicyEffectiveDate;
                    renewalPolicies.NewPolicyExpiryDate = policyResponseMessage.PolicyExpiryDate;
                    renewalPolicies.NewCheckoutCreatedDate = checkoutDetais.CreatedDateTime;
                    if (checkoutDetais.InsuranceCompanyId.HasValue)
                    {
                        renewalPolicies.NewCompanyId = checkoutDetais.InsuranceCompanyId.Value;
                    }
                    renewalPolicies.NewCompanyKey = checkoutDetais.InsuranceCompanyName;
                    if (checkoutDetais.SelectedInsuranceTypeCode.HasValue)
                    {
                        renewalPolicies.NewProductType = checkoutDetais.SelectedInsuranceTypeCode;
                    }
                    renewalPolicies.NewTotalPrice = totalPrice;
                    renewalPolicies.NewChannel = checkoutDetais.Channel;

                    var Quoterequest = _quotationService.GetQuotationRequesByPreviousReferenceId(previousPolicyDetails.ReferenceId);
                    if (Quoterequest != null)
                    {
                        renewalPolicies.FromNotification = true;
                    }
                    else
                    {
                        renewalPolicies.FromNotification = false;
                    }
                    renewalPolicies.CreatedDate = DateTime.Now;
                    _renewalPolicies.Insert(renewalPolicies);

                    var oldPolicy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == previousPolicyDetails.ReferenceId).FirstOrDefault();
                    if (oldPolicy != null)
                    {
                        oldPolicy.IsRenewed = true;
                        _policyRepository.Update(oldPolicy);
                    }
                }
                var newPolicy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == checkoutDetais.ReferenceId).FirstOrDefault();
                if (newPolicy != null)
                {
                    newPolicy.IsRenewed = false;
                    _policyRepository.Update(newPolicy);
                }
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\AddToPolicyRenewal.txt", exp.ToString());
                return false;
            }
        }

        public PreviousPolicyDetails GetPreviousPolicyDetails(string driverNin, string vehicleId, DateTime startDate, DateTime endDate, out string exception)        {            exception = string.Empty;            try
            {
                PreviousPolicyDetails previousPolicyDetails = null;
                IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPreviousPolicyDetails";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParameter = new SqlParameter() { ParameterName = "driverNin", Value = driverNin };
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "VehicleId", Value = vehicleId };
                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = startDate };
                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = endDate };

                command.Parameters.Add(VehicleIdParam);
                command.Parameters.Add(driverNinParameter);
                command.Parameters.Add(startDateParameter);
                command.Parameters.Add(endDateParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                previousPolicyDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PreviousPolicyDetails>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return previousPolicyDetails;
            }            catch (Exception exp)            {
                exception = exp.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetPreviousPolicyDetails.txt", exp.ToString());
                return null;
            }        }

        public List<PreviousPolicyDetails> GetPolicyInfoForRenewal()
        {
            List<PreviousPolicyDetails> previousPolicyDetails = null;
            try
            {
                IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyInfoForRenewal";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                previousPolicyDetails = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PreviousPolicyDetails>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return previousPolicyDetails;
            }            catch (Exception exp)            {
                return null;
            }
        }

        public void HandlePolicyRenewalInfo(PreviousPolicyDetails policy)
        {
            CheckoutDetail checkoutDetais = new CheckoutDetail();
            checkoutDetais.CreatedDateTime = policy.CheckoutCreatedDate;
            checkoutDetais.ReferenceId = policy.ReferenceId;
            checkoutDetais.SelectedInsuranceTypeCode = policy.SelectedInsuranceTypeCode;
            checkoutDetais.InsuranceCompanyId = policy.InsuranceCompanyId;
            checkoutDetais.InsuranceCompanyName = policy.InsuranceCompanyName;
            checkoutDetais.Channel = policy.Channel;

            PolicyResponse policyResponseMessage = new PolicyResponse();
            policyResponseMessage.PolicyIssuanceDate = policy.PolicyIssueDate;
            policyResponseMessage.PolicyEffectiveDate = policy.PolicyEffectiveDate;
            policyResponseMessage.PolicyExpiryDate = policy.PolicyExpiryDate;
            policyResponseMessage.PolicyNo = policy.PolicyNo;

            string exception = string.Empty;
            var result = AddToPolicyRenewal(policy.NIN, policy.SequenceNumber, checkoutDetais, policyResponseMessage, policy.TotalPrice.Value, out exception);

            if (!result && string.IsNullOrEmpty(exception))
            {
                var oldPolicy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == policy.ReferenceId).FirstOrDefault();
                if (oldPolicy != null)
                {
                    oldPolicy.IsRenewed = false;
                    _policyRepository.Update(oldPolicy);
                }
            }
        }

        public NajmResponseOutput GetNajmResponseTimeForConnectWithPolicy(NajmResponseTimeFilter NajmPolicyFilter, int pageIndex, int pageSize, int commandTimeout, out string exception)
        {
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "NajmResponseTime";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                if (!string.IsNullOrWhiteSpace(NajmPolicyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "@PolicyNo", Value = NajmPolicyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(NajmPolicyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "@ReferenceNo", Value = NajmPolicyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (NajmPolicyFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(NajmPolicyFilter.StartDate.Value.Year, NajmPolicyFilter.StartDate.Value.Month, NajmPolicyFilter.StartDate.Value.Day, 0, 0, 0);
                    SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "@StartDate", Value = dtStart };
                    command.Parameters.Add(StartDateParameter);
                }
                if (NajmPolicyFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(NajmPolicyFilter.EndDate.Value.Year, NajmPolicyFilter.EndDate.Value.Month, NajmPolicyFilter.EndDate.Value.Day, 23, 59, 59);
                    SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "@EndDate", Value = dtEnd };
                    command.Parameters.Add(EndDateParameter);
                }

                if (NajmPolicyFilter.CompanyId.HasValue)
                {
                    SqlParameter CompanyIdParameter = new SqlParameter() { ParameterName = "@CompanyId", Value = NajmPolicyFilter.CompanyId };
                    command.Parameters.Add(CompanyIdParameter);
                }
                if (NajmPolicyFilter.Exports)
                {
                    SqlParameter ExportsParameter = new SqlParameter() { ParameterName = "@Export", Value = 1 };
                    command.Parameters.Add(ExportsParameter);
                }
                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "@PageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "@PageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);
                NajmResponseOutput filteredData = new NajmResponseOutput();
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                filteredData.Result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<NajmResponseTimeModel>(reader).ToList();
                if (!NajmPolicyFilter.Exports)
                {
                    reader.NextResult();
                    filteredData.totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        public PolicyNotificationOutput GetpolicyNotificationLog(PolicyNotificationFilter NajmNotificationPolicyFilter, int pageIndex, int pageSize, int commandTimeout, out string exception)
        {
            exception = string.Empty;
            PolicyNotificationOutput filteredData = new PolicyNotificationOutput();
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetPolicyNotificationsLog";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 5 * 60;
                    if (!string.IsNullOrWhiteSpace(NajmNotificationPolicyFilter.PolicyNo))
                    {
                        SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "@PolicyNo", Value = NajmNotificationPolicyFilter.PolicyNo.Trim() };
                        command.Parameters.Add(PolicyNoParameter);
                    }
                    if (!string.IsNullOrWhiteSpace(NajmNotificationPolicyFilter.ReferenceNo))
                    {
                        SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "@ReferenceNo", Value = NajmNotificationPolicyFilter.ReferenceNo.Trim() };
                        command.Parameters.Add(ReferenceNoParameter);
                    }
                    if (NajmNotificationPolicyFilter.InsuranceCompanyId.HasValue)
                    {
                        SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "@InsuranceCompanyId", Value = NajmNotificationPolicyFilter.InsuranceCompanyId };
                        command.Parameters.Add(InsuranceCompanyIdParameter);
                    }
                    if (!string.IsNullOrWhiteSpace(NajmNotificationPolicyFilter.MethodName))
                    {
                        SqlParameter MethodNameParameter = new SqlParameter() { ParameterName = "@MethodName", Value = NajmNotificationPolicyFilter.MethodName.Trim() };
                        command.Parameters.Add(MethodNameParameter);
                    }
                    if (NajmNotificationPolicyFilter.StartDate.HasValue)
                    {
                        DateTime dtStart = new DateTime(NajmNotificationPolicyFilter.StartDate.Value.Year, NajmNotificationPolicyFilter.StartDate.Value.Month, NajmNotificationPolicyFilter.StartDate.Value.Day, 0, 0, 0);
                        SqlParameter StaratDateParameter = new SqlParameter() { ParameterName = "@StartDate", Value = dtStart };
                        command.Parameters.Add(StaratDateParameter);
                    }
                    if (NajmNotificationPolicyFilter.EndDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(NajmNotificationPolicyFilter.EndDate.Value.Year, NajmNotificationPolicyFilter.EndDate.Value.Month, NajmNotificationPolicyFilter.EndDate.Value.Day, 23, 59, 59);
                        SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "@EndDate", Value = dtEnd };
                        command.Parameters.Add(EndDateParameter);
                    }
                    SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "@PageNumber", Value = pageIndex + 1 };
                    command.Parameters.Add(pageNumberParameter);

                    SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "@PageSize", Value = pageSize };
                    command.Parameters.Add(pageSizeParameter);
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    filteredData.Result = ((IObjectContextAdapter)context).ObjectContext.Translate<PolicyNotificationsModel>(reader).ToList();

                    reader.NextResult();
                    filteredData.totalCount = ((IObjectContextAdapter)context).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    return filteredData;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return filteredData;
            }

        }

        #region Vehicle Policy Modification Services
        public List<VehicleSuccessPoliciesForAddBenefitsListing> GetVehicleSuccessPoliciesWithFilterForAddBenefits(VehicleSuccessPoliciesFilterForAddBenefits policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            try
            {
                var dbContext = EngineContext.Current.Resolve<IDbContext>();
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehicleSuccessPoliciesWithFilterForAddBenefits";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.InsurredId))
                {
                    SqlParameter InsurredIdParameter = new SqlParameter() { ParameterName = "InsuredId", Value = policyFilter.InsurredId.Trim() };
                    command.Parameters.Add(InsurredIdParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<VehicleSuccessPoliciesForAddBenefitsListing> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<VehicleSuccessPoliciesForAddBenefitsListing>(reader).ToList();
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }


        public List<VehicleSuccessPoliciesForAddDriverListing> GetVehicleSuccessPoliciesWithFilterForAddDriver(VehicleSuccessPoliciesFilterForAddDriver policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            try
            {
                var dbContext = EngineContext.Current.Resolve<IDbContext>();
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehicleSuccessPoliciesWithFilterForAddvechiledriver";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.InsurredId))
                {
                    SqlParameter InsurredIdParameter = new SqlParameter() { ParameterName = "InsuredId", Value = policyFilter.InsurredId.Trim() };
                    command.Parameters.Add(InsurredIdParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<VehicleSuccessPoliciesForAddDriverListing> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<VehicleSuccessPoliciesForAddDriverListing>(reader).ToList();
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }


        public List<PoliciesForClaimsListingModel> GetAllVehiclePoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            try
            {
                var dbContext = EngineContext.Current.Resolve<IDbContext>();
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllVehiclePoliciesForClaimsFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrWhiteSpace(policyFilter.VehicleId))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "VehicleId", Value = policyFilter.VehicleId.Trim() };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = policyFilter.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<PoliciesForClaimsListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PoliciesForClaimsListingModel>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
        }

        public VehicleClaimRegistrationOutput SendVehicleClaimRegistrationRequest(ClaimRegistrationRequest claim, int companyId, string userId)
        {
            VehicleClaimRegistrationOutput output = new VehicleClaimRegistrationOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            PolicyModificationLog policymodificationlog = new PolicyModificationLog();
            log.ServerIP = policymodificationlog.ServerIP = Utilities.GetInternalServerIP();
            log.ServiceURL = Utilities.GetCurrentURL;
            log.Method = policymodificationlog.MethodName = "SendVehicleClaimRegistrationRequest";
            log.ServiceRequest = JsonConvert.SerializeObject(claim);
            log.Channel = policymodificationlog.Channel = Channel.Dashboard.ToString();
            log.CompanyID = policymodificationlog.CompanyId = companyId;
            if (!string.IsNullOrEmpty(userId))
            {
                Guid userIdGUID = Guid.Empty;
                Guid.TryParse(userId, out userIdGUID);
                log.UserID = userIdGUID;
            }

            //log.UserName = User.Identity.GetUserName();

            if (string.IsNullOrEmpty(claim.ReferenceId))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Refrence ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claim.PolicyNo))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Policy No is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claim.AccidentReportNumber))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.AccidentReportNumber;
                output.ErrorDescription = "AccidentReportNumber ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (claim.InsuredId <= 0)
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InsuredId;
                output.ErrorDescription = "InsuredId ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredMobileNumber))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InsuredMobileNumber;
                output.ErrorDescription = "InsuredMobileNumber ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredIBAN))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InsuredIBAN;
                output.ErrorDescription = "InsuredIBAN ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredBankCode))
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InsuredBankCode;
                output.ErrorDescription = "InsuredBankCode ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }

            var policyNo = _policyRepository.Table.Where(a => a.CheckOutDetailsId == claim.ReferenceId).FirstOrDefault();
            if (policyNo == null)
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "No Policy No with refrence ID:" + claim.ReferenceId;
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }

            var insuranceCompany = _insuranceCompanyService.GetById(companyId);
            string providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null)
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    //output.ErrorCode = 8;
                    log.ErrorDescription = "provider Type is Null";
                    output.ErrorDescription = "provider Type is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

            }
            if (provider == null)
            {
                // output.ErrorCode = 9;
                log.ErrorDescription = "provider Type is Null";
                output.ErrorDescription = "provider Type is Null";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            var response = provider.SendVehicleClaimRegistrationRequest(claim, log);
            if (response.ErrorCode == ClaimRegistrationServiceOutput.ErrorCodes.Success)
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
            }
            else
            {
                output.ErrorCode = VehicleClaimRegistrationOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = response.ErrorDescription;
            }
            output.ClaimRegistrationServiceResponse = response.ClaimRegistrationServiceResponse;
            return output;
        }

        public VehicleClaimNotificationOutput SendVehicleClaimNotificationRequest(ClaimNotificationRequest claimNotification, string userId)
        {
            VehicleClaimNotificationOutput output = new VehicleClaimNotificationOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            PolicyModificationLog policymodificationlog = new PolicyModificationLog();
            log.ServerIP = policymodificationlog.ServerIP = Utilities.GetInternalServerIP();
            log.ServiceURL = Utilities.GetCurrentURL;
            log.Method = policymodificationlog.MethodName = "SendVehicleClaimNotificationRequest";
            log.ServiceRequest = JsonConvert.SerializeObject(claimNotification);
            log.Channel = policymodificationlog.Channel = Channel.Dashboard.ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                Guid userIdGUID = Guid.Empty;
                Guid.TryParse(userId, out userIdGUID);
                log.UserID = userIdGUID;
            }
            //log.UserName = User.Identity.GetUserName();

            if (string.IsNullOrEmpty(claimNotification.ReferenceId))
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Refrence ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claimNotification.PolicyNo))
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Policy No is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (string.IsNullOrEmpty(claimNotification.ClaimNo))
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.AccidentReportNumber;
                output.ErrorDescription = "ClaimNo ID is null";
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }

            var policyNo = _policyRepository.Table.Where(a => a.CheckOutDetailsId == claimNotification.ReferenceId).FirstOrDefault();
            if (policyNo == null)
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "No Policy No with refrence ID:" + claimNotification.ReferenceId;
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }
            if (!policyNo.InsuranceCompanyID.HasValue)
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Insurance Company ID is null in Policy with refrence ID:" + claimNotification.ReferenceId;
                log.ErrorCode = policymodificationlog.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = policymodificationlog.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                PolicyModificationLogDataAccess.AddPolicyModificationLog(policymodificationlog);
                return output;
            }

            log.CompanyID = policyNo.InsuranceCompanyID;
            var insuranceCompany = _insuranceCompanyService.GetById(policyNo.InsuranceCompanyID.Value);
            string providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null)
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    //output.ErrorCode = 8;
                    log.ErrorDescription = "provider Type is Null";
                    output.ErrorDescription = "provider Type is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

            }
            if (provider == null)
            {
                // output.ErrorCode = 9;
                log.ErrorDescription = "provider Type is Null";
                output.ErrorDescription = "provider Type is Null";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            var response = provider.SendVehicleClaimNotificationRequest(claimNotification, log);
            if (response.ErrorCode == ClaimNotificationServiceOutput.ErrorCodes.Success)
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
            }
            else
            {
                output.ErrorCode = VehicleClaimNotificationOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = response.ErrorDescription;
            }
            output.ClaimNotificationServiceResponse = response.ClaimNotificationServiceResponse;
            return output;
        }
        #endregion
        public CancellPolicyOutput SendCancellationRequest(CancelVechilePolicyRequestDto cancelRequest, string userid, string username)
        {
            string providerFullTypeName = "";
            CancellPolicyOutput output = new CancellPolicyOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.CreatedDate = DateTime.Now;
            log.Method = "CancelPolicy";
            log.ServiceRequest = JsonConvert.SerializeObject(cancelRequest);
            log.UserName = username;
            log.Channel = "Vehicle Dashboard";
            try
            {
                if (string.IsNullOrEmpty(cancelRequest.ReferenceId))
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "Refrence ID is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(cancelRequest.PolicyNo))
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "Policy No is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(cancelRequest.CancelDate.ToString()))
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "CancelDate  is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (cancelRequest.CancellationReasonCode == 0)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "Cancellation Reason Code not exist";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var policyNo = _policyRepository.TableNoTracking.Where(a => a.CheckOutDetailsId == cancelRequest.ReferenceId).FirstOrDefault();

                if (policyNo == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.InValidData;
                    output.ErrorDescription = "No Policy No with refrence ID:" + cancelRequest.ReferenceId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.CompanyID = policyNo.InsuranceCompanyID ?? 0;

                var insuranceCompany = _insuranceCompanyService.GetById((int)policyNo.InsuranceCompanyID);
                providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
                IInsuranceProvider provider = null;
                object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
                if (instance != null)
                {
                    provider = instance as IInsuranceProvider;
                }
                if (instance == null)
                {
                    var scope = EngineContext.Current.ContainerManager.Scope();
                    var providerType = Type.GetType(providerFullTypeName);
                    if (providerType == null)
                    {
                        output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.Error;
                        output.ErrorDescription = "provider Type is Null";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceErrorCode = log.ErrorCode.ToString();
                        log.ServiceErrorDescription = log.ServiceErrorDescription;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return output;
                    }

                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                    Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

                }
                if (provider == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "provider is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var response = provider.SubmitVehicleCancelPolicyRequest(cancelRequest, log);

                if (response == null)
                {
                    output.ErrorCode = (int)CancellPolicyOutput.ErrorCodes.ServiceError;
                    output.ErrorDescription = "response is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                output.CancelPolicyResponse = response.CancelPolicyResponse;
                output.ErrorCode = (int)response.ErrorCode;
                output.ErrorDescription = response.ErrorDescription;
                log.ServiceResponse = JsonConvert.SerializeObject(response);
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorDescription = ex.Message;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                return output;
            }
        }

        #region Renewla send SMS message

        public VehicleDiscounts LinkVechWithDiscountCode(VehicleDiscounts VehicleDiscountmodel, out string exception)
        {
            exception = string.Empty;
            try
            {
                if(!string.IsNullOrEmpty(VehicleDiscountmodel.CustomCardNumber))
                {
                   var vehicleInfo= _customCardInfo.TableNoTracking.Where(a => a.CustomCardNumber == VehicleDiscountmodel.CustomCardNumber).FirstOrDefault();
                    if(vehicleInfo!=null&&vehicleInfo.SequenceNumber.HasValue)
                    {
                        VehicleDiscountmodel.SequenceNumber = vehicleInfo.SequenceNumber.Value.ToString();
                    }
                }
                var SelectedVehicleDiscountmodel = _vehicleDiscountsRepository.Table.Where(a => a.Nin == VehicleDiscountmodel.Nin && (a.CustomCardNumber == VehicleDiscountmodel.CustomCardNumber || a.SequenceNumber == VehicleDiscountmodel.SequenceNumber) && a.DiscountCode == VehicleDiscountmodel.DiscountCode).FirstOrDefault();
                if (SelectedVehicleDiscountmodel == null)
                {
                    _vehicleDiscountsRepository.Insert(VehicleDiscountmodel);
                }
                else
                {
                    SelectedVehicleDiscountmodel.ModifiedDate = DateTime.Now;
                    _vehicleDiscountsRepository.Update(SelectedVehicleDiscountmodel);
                }

                return VehicleDiscountmodel;
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return null;
            }
        }

        public SMSOutput SendRenewalSmsMsg(RenewalSendSMS model, out string exception)
        {
            exception = string.Empty;
            try
            {
                string smsBody = "";
                if (model.Lang.ToLower() == "en")
                {
                    string EnUrl = "https://bcare.com.sa/?eid=" + model.ExternalId + "&r=1&re=" + model.ReferenceId;
                    smsBody = "Dear Customer Your Vehicle Insurance " + "[%Make%] [%Model%] ([%PLATE%]) " + " will expire in  [%ExpiryDate%] \n\n" + "Because you are a member of BCare family, we are providing you with coupon discount valid for 48 hours" + "([%PromoCode%])," + "you can only use it once at the payment step.\n\n" + "Compare between 23 insurance companies and renew with one click " + EnUrl;
                }
                else
                {
                    string ArUrl = "https://bcare.com.sa/?eid=" + model.ExternalId + "&r=1&re=" + model.ReferenceId;
                    smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) في  [%ExpiryDate%]  \n\n ولأنك من عائلة بي كير نقدم لك كود خصم صالح لمده 48 ساعة ([%PromoCode%]) يمكنك استخدامه في مرحلة الدفع لمرة واحدة \n\n" + "  قارن بين ٢٣ شركة تأمين وجدد بضغطة زر " + ArUrl;
                }

                if (!string.IsNullOrEmpty(model.Code))
                {
                    smsBody = smsBody.Replace("[%PromoCode%]", model.Code);
                }
                else
                {
                    smsBody = smsBody.Replace("[%PromoCode%]", string.Empty);
                }

                if ((model.ExpiryDate) != null)
                {
                    smsBody = smsBody.Replace("[%ExpiryDate%]", model.ExpiryDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")));
                }
                else
                {
                    smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                }

                if ((model.VehicleModel) != null)
                {
                    smsBody = smsBody.Replace("[%Model%]", model.VehicleModel);
                }
                else
                {
                    smsBody = smsBody.Replace("[%Model%]", string.Empty);
                }

                if ((model.VehicleModel) != null)
                {
                    smsBody = smsBody.Replace("[%Make%]", model.Vehiclemaker);
                }
                else
                {
                    smsBody = smsBody.Replace("[%Make%]", string.Empty);
                }

                //if (model.Plate != null)
                //{
                //    smsBody = smsBody.Replace("([%PLATE%])", model.Plate.ToString());
                //}
                //else
                //{
                    smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
              //  }
                var smsModel = new SMSModel()
                {
                    PhoneNumber = model.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.PolicyRenewal.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = "DashBoard"
                };
                var result = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                return result;
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return null;
            }
        }

        public SMSOutput SendOwnDamageSmsMsg(OwnDamageQueue model, out string exception, string lang)
        {
            OwnDamageSMSLog log = new OwnDamageSMSLog();
            log.CreatedDate = DateTime.Now;
            log.PolicyExpiryDate = model.PolicyExpiryDate;
            log.MobileNumber = model.Phone;
            log.ExternalId = model.ExternalId;
            log.PolicyNo = model.PolicyNo;
            exception = string.Empty;
            try
            {
                string hashed = SecurityUtilities.HashData($"{true}_{model.PolicyNo}_{model.PolicyExpiryDate.Value.ToString("dd/MM/yy hh:mm:ss tt", new CultureInfo("ar-SA"))}_{SecurityUtilities.HashKey}", null);
                string Url = "https://bcare.com.sa/?eid=" + model.ExternalId + "&r=0&c=0&policyNo=" + model.PolicyNo + "&policyExpiryDate=" + model.PolicyExpiryDate?.ToString("yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US")) + "&hashed=" + hashed;
                string emo_heart_face = DecodeEncodedNonAsciiCharacters("\uD83D\uDE0D");
                string smsBody = "";
                if (lang.ToLower() == "en")
                {
                //    smsBody = "We have launched Own Damage product " + emo_heart_face + "\n\n" +
                //         " You can now upgrade your insurance to cover you and others, with comprehensive features and more .." +
                //         " Exploit the service through below link: " + Url;

                    smsBody = "Third Party Insurance? We have launched a Own Damage service " + emo_heart_face + "\n\n" +
                         " You can now extend your coverage and protect your vehicle from damages with comprehensive benefits and more With your third part insurance policy validity date, and minimal cost.." +
                         " Exploit the service through below link: " + Url;
                }
                else
                {
                    smsBody = "تأمينك ضد الغير؟أطلقنا خدمة إصلاح اضرار المركبة" + emo_heart_face +
                        "\n\n تقدر توسّع تغطيتك وتحمي مركبتك من الأضرار وبمميزات الشامل وأكثر بمدة سريان وثيقتك ضد الغير بتكلفة بسيطة.."+
                        "استفد من الخدمة من خلال الرابط: " + Url;
                    //smsBody = "اطلقنا خدمة إصلاح اضرار المركبة " + emo_heart_face +
                    //    "\n\n تقدر الآن ترقي تأمينك ويصير يغطيك ويغطى غيرك , بمميزات الشامل وأكثر.." +
                    //    " استفد من الخدمة من خلال الرابط :" + Url;
                }

                var smsModel = new SMSModel()
                {
                    PhoneNumber = model.Phone,
                    MessageBody = smsBody,
                    Method = SMSMethod.PolicyRenewal.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = "DashBoard"
                };

                var result = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                result.ErrorCode = result.ErrorCode == 0 ? 1 : result.ErrorCode;
                log.ErrorCode = result.ErrorCode == 0 ? 1 : result.ErrorCode;
                log.ErrorDescription = result.ErrorDescription;
                log.SMSMessage = smsBody;
                OwnDamageDataAccess.AddToOwnDamageSMSLog(log);
                return result;
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                log.ErrorDescription = ex.ToString();
                OwnDamageDataAccess.AddToOwnDamageSMSLog(log);
                return null;
            }
        }

        #endregion


        public void ExecutePolicy(string companyName)
        {
            //int maxTries = 1;
            string serverIP = Utilities.GetAppSetting("ServerIP");
            var policiesToProcess = _policyProcessingService.GetProcessingQueue(companyName);
            if (policiesToProcess != null && policiesToProcess.Count > 0)
            {
                foreach (var policy in policiesToProcess)
                {
                    bool SkipAxaPolicyRequestAfter10Tries = false;
                    List<string> MotorChannels = new List<string>()
                {
                    Channel.Portal.ToString().ToLower(),
                    Channel.ios.ToString().ToLower(),
                    Channel.android.ToString().ToLower(),
                    Channel.Dashboard.ToString().ToLower()
                };

                    CheckoutDetail checkout = new CheckoutDetail();
                    string exception = string.Empty;
                    DateTime dtBefore = DateTime.Now;
                    string driverNin = string.Empty;
                    string vehicleId = string.Empty;
                    try
                    {
                        checkout = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == policy.ReferenceId).FirstOrDefault();
                        var selectedLang = LanguageTwoLetterIsoCode.Ar;
                        if (checkout == null)
                        {
                            policy.ErrorDescription = "These is no checkout details object for this policy";
                            continue;
                        }
                        if (checkout.PolicyStatusId == 10)
                        {
                            policy.ModifiedDate = DateTime.Now;
                            continue;
                        }
                        // escape successful policies
                        if (checkout.PolicyStatusId == 4)
                        {
                            policy.ProcessedOn = DateTime.Now;
                            continue;
                        }

                        if (checkout.PolicyStatusId == 6 || checkout.PolicyStatusId == 7)
                        {
                            var output = GetFailedPolicyFile(policy.ReferenceId, Utilities.GetInternalServerIP(), Channel.Portal.ToString());
                            if (output.ErrorCode == PdfGenerationOutput.ErrorCodes.Success)
                            {
                                policy.ProcessedOn = DateTime.Now;
                                policy.ErrorDescription = "Success";
                            }
                            else
                            {
                                policy.ErrorDescription = output.ErrorDescription;
                            }
                        }
                        else
                        {
                            if (checkout.Isleasing)
                            {
                                exception = string.Empty;
                                var orderItem = _orderService.GetLeasingOrderItemByReferenceId(checkout.ReferenceId, out exception);
                                if (!string.IsNullOrEmpty(exception))
                                    File.WriteAllText(@"C:\inetpub\WataniyaLog\GetLeasingOrderItemByReferenceId_error.txt", exception);
                                if (orderItem == null)
                                    continue;

                                if (checkout.ProviderServiceId == CheckoutProviderServicesCodes.PurchaseBenefit)
                                {
                                    var model = new PurchaseBenefitModel();
                                    model.ReferenceId = checkout.ReferenceId;
                                    //model.ReferenceId = checkout.MainPolicyReferenceId;
                                    model.Benefits = new List<AdditionalBenefit>();
                                    if (orderItem.Benefits != null && orderItem.Benefits.Count > 0)
                                        model.Benefits = orderItem.Benefits;

                                    var responseModel = _autoleasingInquiryContext.PurchaseBenefit(model, checkout.UserId, "");
                                    if (!string.IsNullOrEmpty(responseModel.ErrorDescription) || responseModel.ErrorCode != AddBenefitOutput.ErrorCodes.Success)
                                        policy.ErrorDescription = responseModel.ErrorDescription;
                                    else
                                        policy.ProcessedOn = DateTime.Now;
                                }

                                else if (checkout.ProviderServiceId == CheckoutProviderServicesCodes.PurchaseDriver)
                                {
                                    File.WriteAllText(@"C:\inetpub\WataniyaLog\PurchaseDriver.txt", JsonConvert.SerializeObject(orderItem.Driver));

                                    var model = new Tameenk.Models.Checkout.PurchaseDriverModel()
                                    {
                                        ReferenceId = orderItem.Driver.ReferenceId,
                                        PaymentAmount = orderItem.Driver.PaymentAmount
                                    };

                                    var responseModel = _policyModificationContext.PurchaseVechileDriver(model, checkout.UserId, "");
                                    if (!string.IsNullOrEmpty(responseModel.ErrorDescription) || responseModel.ErrorCode != AddDriverOutput.ErrorCodes.Success)
                                        policy.ErrorDescription = responseModel.ErrorDescription;
                                    else
                                        policy.ProcessedOn = DateTime.Now;
                                }
                            }
                            else
                            {
                                if (policy.CompanyID == 25 && MotorChannels.Contains(policy.Chanel.ToLower()) && policy.ProcessingTries >= 10)
                                {
                                    SkipAxaPolicyRequestAfter10Tries = true;
                                    continue;
                                }

                                if (checkout.SelectedLanguage == LanguageTwoLetterIsoCode.En)
                                    selectedLang = LanguageTwoLetterIsoCode.En;

                                var responseModel = SubmitPolicy(policy.ReferenceId, selectedLang, Utilities.GetInternalServerIP(), Channel.Portal.ToString());
                                policy.ErrorCode = responseModel.ErrorCode;
                                if (responseModel.ErrorCode == 1)
                                {
                                    policy.ProcessedOn = DateTime.Now;
                                }
                                else if (responseModel.ErrorCode == 12)
                                {
                                    policy.ErrorDescription = responseModel.ErrorDescription;
                                }
                                else if (!string.IsNullOrEmpty(responseModel.ErrorDescription))
                                {
                                    policy.ErrorDescription = responseModel.ErrorDescription;
                                }
                                if (!string.IsNullOrEmpty(responseModel.DriverNin))
                                {
                                    driverNin = responseModel.DriverNin;
                                }
                                if (!string.IsNullOrEmpty(responseModel.VehicleId))
                                {
                                    vehicleId = responseModel.VehicleId;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        policy.ErrorDescription = ex.ToString();
                    }
                    finally
                    {
                        if (!SkipAxaPolicyRequestAfter10Tries)
                        {
                            DateTime dtAfter = DateTime.Now;
                            policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                            exception = string.Empty;
                            bool value = _policyProcessingService.GetAndUpdatePolicyProcessingQueue(policy.Id, policy, serverIP, driverNin, vehicleId, out exception);
                            if (!value && !string.IsNullOrEmpty(exception))
                            {
                                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\" + companyName + "_ProcessingTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                            }

                            //if (value && policy.Chanel.ToLower() == "autoleasing")
                            //    HandleAutoleaingSmsPortalLink(policy, checkout);
                        }
                    }
                }
            }
        }

        public PolicyOutput HandlePolicyYearlyMaximumPurchaseTask(PolicyProcessingQueue queueItem)
        {
            PolicyOutput output = new PolicyOutput();
            try
            {
                var checkoutDetails = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == queueItem.ReferenceId && x.IsCancelled == false).FirstOrDefault();
                if (checkoutDetails == null)
                {
                    output.ErrorCode = 2;
                    output.ErrorDescription = "checkoutDetails is null "+ queueItem.ReferenceId;
                    return output;
                }
                if (checkoutDetails.PolicyStatusId !=10)
                {
                    output.ErrorCode = 3;
                    output.ErrorDescription = "invalid policy status id "+ checkoutDetails.PolicyStatusId;
                    return output;
                }
                string exception = string.Empty;
                var userSuccessPoliciesDetails = _checkoutsService.GetUserSuccessPoliciesInfo(queueItem.DriverNin, out exception);
                if (userSuccessPoliciesDetails == null)
                {
                    output.ErrorCode = 4;
                    output.ErrorDescription = "success Policies is null "+ queueItem.DriverNin;
                    return output;
                }
                var quoteResponse = _quotationResponseRepository.TableNoTracking
                      .Include(e => e.QuotationRequest)
                       .Include(e => e.QuotationRequest.Insured)
                      .Include(e => e.QuotationRequest.Vehicle)
                      .Include(e => e.QuotationRequest.Driver)
                      .FirstOrDefault(q => q.ReferenceId == queueItem.ReferenceId);
                if (quoteResponse == null)
                {
                    output.ErrorCode = 5;
                    output.ErrorDescription = "quoteResponse is null "+ queueItem.ReferenceId;
                    return output;
                }
                int userSuccessPolicies = 0;
                if (quoteResponse.QuotationRequest.Insured.NationalId.StartsWith("7")) // Company
                {
                    userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == true && a.PolicyStatusId != 10).Count();
                }
                else
                {
                    userSuccessPolicies = userSuccessPoliciesDetails.Where(a => a.IsCompany == false && a.PolicyStatusId != 10).Count();
                }

                if (userSuccessPolicies > UserInsuranceNumberLimitPerYear)//5 per year
                {
                    output.ErrorCode = 7;
                    output.ErrorDescription = "reached maximum "+ queueItem.DriverNin;
                    return output;
                }
              
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                checkoutDetails.ModifiedDate = DateTime.Now;
                _checkoutDetailRepository.Update(checkoutDetails);

                var processQueue = _policyProcessingQueue.Table.Where(a => a.Id == queueItem.Id && a.ProcessedOn == null).FirstOrDefault();
                if (processQueue == null)
                {
                    output.ErrorCode = 8;
                    output.ErrorDescription = "processQueue is null "+ queueItem.ReferenceId;
                    return output;
                }

                processQueue.ErrorCode = 0;
                processQueue.ProcessingTries = 0;
                processQueue.IsLocked = false;
                processQueue.ModifiedDate = DateTime.Now;
                _policyProcessingQueue.Update(processQueue);

                output.ErrorCode = 1;
                output.ErrorDescription = "Success";
                return output;
            }
            catch(Exception exp)
            {
                output.ErrorCode = 500;
                output.ErrorDescription = exp.ToString() +" "+ queueItem.ReferenceId;
                return output;
            }
        }

        public CompanyAvgResponseTimeOutput HandleInsuranceCompanyOrderTask()
        {
            CompanyAvgResponseTimeOutput output = new CompanyAvgResponseTimeOutput();
            try
            {
                string exception = string.Empty; ;
                var responses = QuotationRequestLogDataAccess.GetInsuranceCompanyAvgResponseTime(out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = CompanyAvgResponseTimeOutput.ErrorCodes.DatabaseException;
                    output.ErrorDescription = exception;
                    return output;
                }
                if (responses == null || responses.Count()==0)
                {
                    output.ErrorCode = CompanyAvgResponseTimeOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = exception;
                    return output;
                }
                int order = 0;
                foreach (var response in responses)
                {
                    order++;
                    var company = _insuranceCompanyRepository.Table.Where(a => a.InsuranceCompanyID == response.CompanyId).FirstOrDefault();
                    if (company == null)
                        continue;
                    company.Order = order;
                    company.LastModifiedDate = DateTime.Now;
                    _insuranceCompanyRepository.Update(company);
                }
                output.ErrorCode = CompanyAvgResponseTimeOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = CompanyAvgResponseTimeOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                return output;
            }
        }
        public bool GetPurchaseStatisticsTask()
        {
            DateTime dtBeforeCalling = DateTime.Now;
            List<string> phonesList = new List<string>();
            phonesList.Add("966552660771");
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            NotificationLog log = new NotificationLog();
            log.StartDate = start;
            log.EndDate = end;
            log.Method = "PurchaseStatistics";
            try
            {
                string exception = string.Empty;
                List<PurchaseStatisticsInfo> statistics = GetPurchaseStatisticsInfo(start, end, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "failed to GetPurchaseStatisticsInfo due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                    return false;
                }
                string message = "مبيعات اليوم حتى الان";
                message += "\r\n";
                foreach (var item in statistics)
                {
                    message +=item.ProductDescription + ":" + item.TotalCount+"\r\n";
                }
                message = message.Trim();
                log.Message = message;
                foreach (string phone in phonesList)
                {
                    log.Phone = phone;
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = phone,
                        MessageBody = message,
                        Method = SMSMethod.PurchaseStatistics.ToString(),
                        Module = Module.Vehicle.ToString(),
                        Channel = log.Channel
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode == 12)
                    {
                        log.ErrorCode = 6;
                        log.ErrorDescription = "sms exception: " + exception;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    if (smsOutput.ErrorCode != 0)
                    {
                        log.ErrorCode = 7;
                        log.ErrorDescription = "sms failed to sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }

                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success and message sent";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                log.ErrorCode = 8;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                NotificationLogDataAccess.AddToNotificationLog(log);
                return false;
            }
        }
        public bool GetPaymentMethodsStatisticsInfo()
        {
            DateTime dtBeforeCalling = DateTime.Now;
            List<string> phonesList = new List<string>();
            phonesList.Add("966552660771");
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            NotificationLog log = new NotificationLog();
            log.StartDate = start;
            log.EndDate = end;
            log.Method = "PaymentMethodsStatistics";
            try
            {
                string exception = string.Empty;
                List<PurchaseStatisticsInfo> statistics = GetPaymentMethodsStatisticsInfo(start, end, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "failed to GetPurchaseStatisticsInfo due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                    return false;
                }
                string message = "إحصائيات طرق الدفع اليوم حتى الان";
                message += "\r\n";
                foreach (var item in statistics)
                {
                    message += item.PaymentMethod + ":" + item.TotalCount + "\r\n";
                }
                message = message.Trim();
                log.Message = message;
                foreach (string phone in phonesList)
                {
                    log.Phone = phone;
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = phone,
                        MessageBody = message,
                        Method = SMSMethod.PurchaseStatistics.ToString(),
                        Module = Module.Vehicle.ToString(),
                        Channel = log.Channel
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode == 12)
                    {
                        log.ErrorCode = 6;
                        log.ErrorDescription = "sms exception: " + exception;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }
                    if (smsOutput.ErrorCode != 0)
                    {
                        log.ErrorCode = 7;
                        log.ErrorDescription = "sms failed to sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        NotificationLogDataAccess.AddToNotificationLog(log);
                        continue;
                    }

                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success and message sent";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    NotificationLogDataAccess.AddToNotificationLog(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                log.ErrorCode = 8;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                NotificationLogDataAccess.AddToNotificationLog(log);
                return false;
            }
        }
        public List<PurchaseStatisticsInfo> GetPurchaseStatisticsInfo(DateTime startDate,DateTime endDate,out string exception)
        {
            exception = string.Empty;
            try
            {
                IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPurchaseStatistics";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "StartDate", Value = startDate };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "EndDate", Value = endDate };

                command.Parameters.Add(startDateParameter);
                command.Parameters.Add(endDateParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var purchaseStatistics = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PurchaseStatisticsInfo>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return purchaseStatistics;
            }            catch (Exception exp)            {
                exception = exp.ToString();
                return null;
            }
        }
        public List<PurchaseStatisticsInfo> GetPaymentMethodsStatisticsInfo(DateTime startDate, DateTime endDate, out string exception)
        {
            exception = string.Empty;
            try
            {
                IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPaymentMethodsStatisticsInfo";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter startDateParameter = new SqlParameter() { ParameterName = "StartDate", Value = startDate };
                SqlParameter endDateParam = new SqlParameter() { ParameterName = "EndDate", Value = endDate };

                command.Parameters.Add(startDateParameter);
                command.Parameters.Add(endDateParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var purchaseStatistics = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PurchaseStatisticsInfo>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return purchaseStatistics;
            }            catch (Exception exp)            {
                exception = exp.ToString();
                return null;
            }
        }

        private void HandleAutoleaingSmsPortalLink(PolicyProcessingQueue policy, CheckoutDetail checkout)
        {
            try
            {
                var messaageQueue = _autoleasingPortalLinkProcessingQueueQueueRepository.Table.Where(a => a.ReferenceId == checkout.ReferenceId).FirstOrDefault();
                if (messaageQueue == null)
                    return;

                var portalLinkModel = new Tameenk.Services.Core.Leasing.Models.AutoleasingPortalLinkModel()
                {
                    CheckOutUserId = checkout.UserId,
                    ReferenceId = checkout.ReferenceId,
                    MainDriverId = checkout.MainDriverId,
                    VehicleId = checkout.VehicleId,
                    Phone = checkout.Phone,
                    BankId = messaageQueue.BankId,
                    BankKey = messaageQueue.BankKey,
                    DriverNin = policy.DriverNin,
                    VehicleSequenceOrCustom = policy.VehicleId,
                    CheckoutSelectedlang = checkout.SelectedLanguage ?? LanguageTwoLetterIsoCode.Ar
                };

                string exception = string.Empty;
                var sendMessage = _checkoutContext.LeasingHandleAutoleasingPoliciesToSendSmsPortalLink(portalLinkModel, out exception);
                if (!string.IsNullOrEmpty(exception))
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleAutoleaingSmsPortalLink_" + checkout.ReferenceId + "_ProcessingTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "sendMessageError.txt", " Exception is:" + exception.ToString());

                if (sendMessage)
                {
                    messaageQueue.ProcessingTries += 1;
                    messaageQueue.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "Success";
                    messaageQueue.ModifiedDate = DateTime.Now;
                    messaageQueue.ProcessedOn = DateTime.Now;
                    messaageQueue.IsDone = true;
                    messaageQueue.IsLocked = true;
                    _autoleasingPortalLinkProcessingQueueQueueRepository.Update(messaageQueue);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleAutoleaingSmsPortalLink" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_" + policy.ReferenceId + "Exception.txt", ex.ToString());
            }
        }

        #region Rehit SMS Renewal Notification

        public bool RehitSmsRenewalNotifications(int notificationNo)
        {
            SMSNotification log = new SMSNotification();
            log.StartDate = DateTime.Now;
            log.EndDate = DateTime.Now;
            log.NotificationNo = notificationNo;
            log.TaskName = "SmsRenewalNotificationsRehit";
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                string exception = string.Empty;
                var policies = GetSmsRenewalNotificationsRehitList(out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "failed to GetRenewalPolicies due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                if (policies.Count == 0)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "GetRenewalPolicies count is 0";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                var skippedNumbers = _smsSkippedNumbers.TableNoTracking.ToList();
                var discountCode = _renewalDiscount.TableNoTracking
                                                   .Where(a => a.IsActive && a.MessageType == notificationNo && a.CodeType == 2) //  && (a.EndDate.HasValue && a.EndDate.Value >= end.Date)
                                                   .OrderByDescending(a => a.Id).FirstOrDefault();

                foreach (var p in policies)
                {
                    dtBeforeCalling = DateTime.Now;
                    if (p.Channel.ToLower() == "autoleasing")
                        continue;
                    if (skippedNumbers.Where(a => a.PhoneNo == p.Phone).FirstOrDefault() != null)
                        continue;
                    log = new SMSNotification();
                    log.StartDate = DateTime.Now;
                    log.EndDate = DateTime.Now;
                    log.NotificationNo = notificationNo;
                    log.TaskName = "SmsRenewalNotificationsRehit";

                    string smsBody = string.Empty;
                    log.MobileNumber = p.Phone;
                    log.ReferenceId = p.ReferenceId;
                    exception = string.Empty;
                    string vehicleId = p.SequenceNumber == null ? p.CustomCardNumber : p.SequenceNumber;
                    log.SequenceNumber = p.SequenceNumber;
                    log.CustomCard = p.CustomCardNumber;
                    var policy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == p.ReferenceId).FirstOrDefault();
                    if (policy == null)
                    {
                        log.ErrorCode = 8;
                        log.ErrorDescription = "policy is null";
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    string make = string.Empty;
                    string model = string.Empty;
                    string VechilePlatInfo = string.Empty;
                    string url = string.Empty;
                    bool isCustomCardConverted = false;
                    if (!string.IsNullOrEmpty(p.CustomCardNumber))
                    {
                        CustomCardQueue customCardInfo = new CustomCardQueue();
                        customCardInfo.UserId = p.UserId;
                        customCardInfo.CustomCardNumber = p.CustomCardNumber;
                        customCardInfo.ModelYear = p.ModelYear;
                        customCardInfo.Channel = p.Channel;
                        customCardInfo.CompanyID = p.InsuranceCompanyId;
                        customCardInfo.CompanyName = p.InsuranceCompanyName;
                        customCardInfo.VehicleId = p.VehicleId;
                        customCardInfo.ReferenceId = p.ReferenceId;
                        customCardInfo.PolicyNo = p.PolicyNo;
                        var customCardOutput = _policyNotificationContext.GetCustomCardInfo(customCardInfo);
                        if (customCardOutput.ErrorCode != UpdateCustomCardOutput.ErrorCodes.Success)
                        {
                            log.ErrorCode = 500;
                            log.ErrorDescription = "falied to get custom card info due to " + customCardOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                        }
                        isCustomCardConverted = true;

                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText1))
                            p.CarPlateText1 = customCardOutput.CarPlateText1;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText2))
                            p.CarPlateText2 = customCardOutput.CarPlateText2;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText3))
                            p.CarPlateText3 = customCardOutput.CarPlateText3;
                        if (customCardOutput.CarPlateNumber.HasValue)
                            p.CarPlateNumber = customCardOutput.CarPlateNumber;

                        if (!string.IsNullOrEmpty(customCardOutput.SequenceNumber))
                        {
                            vehicleId = customCardOutput.SequenceNumber;
                            log.SequenceNumber = customCardOutput.SequenceNumber;
                        }
                    }
                    if (!string.IsNullOrEmpty(p.SequenceNumber) || isCustomCardConverted)
                    {
                        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                        {
                            Channel = p.Channel,
                            ServerIP = Utilities.GetInternalServerIP()
                        };
                        if (!string.IsNullOrEmpty(p.UserId))
                        {
                            Guid userIdGUID = Guid.Empty;
                            Guid.TryParse(p.UserId, out userIdGUID);
                            predefinedLogInfo.UserID = userIdGUID;
                        }
                        predefinedLogInfo.VehicleId = vehicleId;
                        predefinedLogInfo.DriverNin = p.CarOwnerNIN;
                        predefinedLogInfo.ReferenceId = p.ReferenceId;

                        var vehicleYakeenRequest = new VehicleYakeenRequestDto()
                        {
                            VehicleId = Convert.ToInt64(vehicleId),
                            VehicleIdTypeId = 1,
                            OwnerNin = p.OwnerTransfer ? Convert.ToInt64(p.NationalId) : Convert.ToInt64(p.CarOwnerNIN)
                        };
                        var driverInfo = _yakeenClient.GetCarInfoBySequenceInfo(vehicleYakeenRequest, predefinedLogInfo);
                        if (driverInfo.ErrorCode != YakeenOutput.ErrorCodes.Success)
                        {
                            policy.RenewalNotificationStatus = "SoldOut";
                            policy.NotificationNo = notificationNo;
                            _policyRepository.Update(policy);

                            log.ErrorCode = 15;
                            log.ErrorDescription = "can't send sms due to " + driverInfo.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                    }
                    DateTime beforeCallingDatabase = DateTime.Now;
                    var policyInfo = _vehicleService.GetVehiclePolicyDetails(vehicleId, out exception);
                    DateTime afterCallingDatabase = DateTime.Now;
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "GetVehiclePolicy returned exp: " + exception + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (policyInfo == null && isCustomCardConverted)
                    {
                        policyInfo = _vehicleService.GetVehiclePolicyDetails(p.CustomCardNumber, out exception);
                    }
                    if (policyInfo == null)
                    {
                        log.ErrorCode = 4;
                        log.ErrorDescription = "policyInfo is null" + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (!policyInfo.PolicyExpiryDate.HasValue)
                    {
                        policy.RenewalNotificationStatus = "PolicyExpiryDate is null";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 5;
                        log.ErrorDescription = "PolicyExpiryDate is null and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    //if (policyInfo.PolicyExpiryDate.HasValue
                    //    && policyInfo.PolicyExpiryDate.Value.Date.Year > end.Year)
                    //{
                    //    policy.IsRenewed = true;
                    //    policy.RenewalNotificationStatus = "AlreadyRenewed";
                    //    policy.NotificationNo = notificationNo;
                    //    _policyRepository.Update(policy);

                    //    log.ErrorCode = 6;
                    //    log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    SMSNotificationDataAccess.AddToSMSNotification(log);
                    //    continue;
                    //}
                    //if (policyInfo.PolicyExpiryDate.HasValue
                    //   && policyInfo.PolicyExpiryDate.Value.Date > end.AddMonths(5).Date)
                    //{
                    //    policy.IsRenewed = true;
                    //    policy.RenewalNotificationStatus = "StillValid";
                    //    policy.NotificationNo = notificationNo;
                    //    _policyRepository.Update(policy);

                    //    log.ErrorCode = 16;
                    //    log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    SMSNotificationDataAccess.AddToSMSNotification(log);
                    //    continue;
                    //}
                    //bool sendSms = true;
                    //if (p.PolicyExpiryDate < start)
                    //{
                    //    sendSms = false;
                    //}
                    //if (p.PolicyExpiryDate > end)
                    //{
                    //    sendSms = false;
                    //}
                    //log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    //if (!sendSms)
                    //{
                    //    log.ErrorCode = 7;
                    //    log.ErrorDescription = "don't send sms due to as p.PolicyExpiryDate is " + p.PolicyExpiryDate + " and start is " + start + " and enddate is " + end + " and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    SMSNotificationDataAccess.AddToSMSNotification(log);
                    //    continue;
                    //}
                    var discountMsg = string.Empty;
                    if (discountCode != null)
                    {
                        VehicleDiscounts VehicleDiscountmodel = new VehicleDiscounts()
                        {
                            CreatedDate = DateTime.Now,
                            DiscountCode = discountCode.Code,
                            IsUsed = false,
                            SequenceNumber = p.SequenceNumber,
                            CustomCardNumber = p.CustomCardNumber,
                            Nin = p.CarOwnerNIN
                        };
                        string ex = string.Empty;
                        var CodeWithVech = LinkVechWithDiscountCode(VehicleDiscountmodel, out ex);
                        if (!string.IsNullOrEmpty(ex))
                        {
                            log.ErrorCode = 5;
                            log.ErrorDescription = "Error happend while insert Data Vehcile Discount , and the error is " + ex;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                        if (CodeWithVech != null && !string.IsNullOrEmpty(CodeWithVech.DiscountCode))
                        {
                            if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                            {
                                discountMsg = "ولأنك من عائلة بي كير نقدم لك كود خصم {0} صالح لمده 48 ساعة  يمكنك استخدامه في مرحلة الدفع لمرة واحدة";
                            }
                            else
                            {
                                discountMsg = "Because you are a member of BCare family, we are providing you with coupon discount {0} valid for 48 hours you can only use it once at the payment step";
                            }
                            discountMsg = discountMsg.Replace("{0}", CodeWithVech.DiscountCode);
                        }
                    }
                    if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                            || !string.IsNullOrEmpty(p.CarPlateText2)
                            || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                            p.CarPlateText2, p.CarPlateText3,
                            p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "ar");
                        }

                        if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {

                            if (isCustomCardConverted)
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescAr))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescAr);
                            make = p.MakerDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(p.ModelDescAr))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescAr);
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            string PolicyExpiryDate = "";
                            PolicyExpiryDate = "في ";
                            PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("en-US"));
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd", new System.Globalization.CultureInfo("ar-eg")) + " ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("MMMM", new System.Globalization.CultureInfo("ar-eg")) + ", ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("yyyy", new System.Globalization.CultureInfo("ar-eg"));
                            smsBody = smsBody.Replace("[%ExpiryDate%]", PolicyExpiryDate);
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else if (string.IsNullOrEmpty(discountMsg) && smsBody.Contains("ينتهي تأمين مركبتك"))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "،");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "");
                        }
                    }
                    else
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                           || !string.IsNullOrEmpty(p.CarPlateText2)
                           || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                           p.CarPlateText2, p.CarPlateText3,
                           p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "en");
                        }
                        if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else if ((notificationNo == 4 || p.PolicyExpiryDate.Date < DateTime.Now.Date) && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescEn))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescEn);
                            make = p.MakerDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.ModelDescEn))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescEn);
                            model = p.ModelDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                            model = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", ",");
                        }
                    }
                    smsBody = smsBody.Trim();
                    log.SMSMessage = smsBody;
                    exception = string.Empty;
                    if (notificationNo == 2) // whatisapp only with 14 days notification
                    {
                        var count = WhatsAppLogDataAccess.GetFromWhatsAppNotification(p.ReferenceId);
                        if (count == 0)
                        {
                            _notificationService.SendWhatsAppMessageForPolicyRenewalAsync(p.Phone, smsBody, make, model, VechilePlatInfo, url, SMSMethod.PolicyRenewal.ToString(), p.ReferenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), p.SelectedLanguage).ToLower(), p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                        }
                    }
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = p.Phone,
                        MessageBody = smsBody,
                        Method = SMSMethod.PolicyRenewal.ToString(),
                        Module = Module.Vehicle.ToString()
                    };
                    var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                    if (smsOutput.ErrorCode == 12)
                    {
                        log.ErrorCode = 10;
                        log.ErrorDescription = "sms failed to sent";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }

                    if (smsOutput.ErrorCode != 0)
                    {
                        log.ErrorCode = 9;
                        log.ErrorDescription = "sms failed to sent: " + smsOutput.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    policy.RenewalNotificationStatus = "Notification" + notificationNo + "Sent";
                    policy.NotificationNo = notificationNo;
                    _policyRepository.Update(policy);
                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SendSmsRenewalNotifications_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", JsonConvert.SerializeObject(exp.ToString()));
                log.ErrorCode = 11;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                SMSNotificationDataAccess.AddToSMSNotification(log);
                return false;
            }
        }

        private List<RenewalPolicyInfo> GetSmsRenewalNotificationsRehitList(out string exception)
        {
            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetSmsRenewalNotificationsRehitList_Context.txt", "enter");

            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetSmsRenewalNotificationsRehit";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<RenewalPolicyInfo> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalPolicyInfo>(reader).ToList();
                return data;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetSmsRenewalNotificationsRehitList_Exception.txt", ex.ToString());
                exception = ex.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion

        #region get vehicle color
        private void GetVehicleColor(out string vehicleColorEn, string vehicleMajorColor, int companyId)
        {
            vehicleColorEn = vehicleMajorColor; //default value
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
                        vehicleColorEn = vColor.EnglishDescription;
                    }
                }
            }
            else
            {
                vehicleColorEn = vColor.EnglishDescription;
            }
        }
        #endregion

        private string HandlePolicySubTotal(ICollection<PriceDetail> priceDetails, PolicyTemplateGenerationModel policyTemplateModel, decimal benefitsPrice)
        {
            decimal subTotal = 0;
            try
            {
                decimal _officePremium = 0;
                decimal _loading = 0;
                decimal _NCDAmount = 0;
                decimal _LoyaltyDiscount = 0;
                decimal _SchemesDiscount = 0;
                decimal _SpecialDiscount2 = 0;

                //if (!string.IsNullOrEmpty(policyTemplateModel.OfficePremium))
                //    decimal.TryParse(policyTemplateModel.OfficePremium, out _officePremium);
                //if (!string.IsNullOrEmpty(policyTemplateModel.ClalmLoadingAmount))
                //    decimal.TryParse(policyTemplateModel.ClalmLoadingAmount, out _loading);
                //if (!string.IsNullOrEmpty(policyTemplateModel.NCDAmount))
                //    decimal.TryParse(policyTemplateModel.NCDAmount, out _NCDAmount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.LoyaltyDiscount))
                //    decimal.TryParse(policyTemplateModel.LoyaltyDiscount, out _LoyaltyDiscount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.SchemesDiscount))
                //    decimal.TryParse(policyTemplateModel.SchemesDiscount, out _SchemesDiscount);
                //if (!string.IsNullOrEmpty(policyTemplateModel.SpecialDiscount2))
                //    decimal.TryParse(policyTemplateModel.SpecialDiscount2, out _SpecialDiscount2);

                //subTotal = (_officePremium + _loading) - (_NCDAmount + _LoyaltyDiscount + _SchemesDiscount + _SpecialDiscount2);

                foreach (var price in priceDetails)
                {
                    switch (price.PriceTypeCode)
                    {
                        case 2: _NCDAmount += price.PriceValue; break;
                        case 3: _LoyaltyDiscount += price.PriceValue; break;
                        case 4: _loading += price.PriceValue; break;
                        case 7: _officePremium += price.PriceValue; break;
                        case 11: _SchemesDiscount += price.PriceValue; break;
                        case 12: _SpecialDiscount2 += price.PriceValue; break;
                    }
                }

                subTotal = (_officePremium + _loading + benefitsPrice) - (_NCDAmount + _LoyaltyDiscount + _SchemesDiscount + _SpecialDiscount2);
            }
            catch (Exception ex)
            {
                subTotal = 0;
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandlePolicySubTotal" + policyTemplateModel.ReferenceNo + ".txt", ex.ToString());
            }

            return subTotal.ToString();
        }

        #region Handle missing invoice pdf

        public void GenerateMissingInvoicePdf()
        {
            var policiesToProcess = GetMissingInvoicePDF();
            foreach (var policy in policiesToProcess)
            {
                string exception = string.Empty;
                DateTime dtBefore = DateTime.Now;

                try
                {
                    bool isInvoiceFileGenerated = GenerateInvoicePdf(policy.ReferenceId, out exception);
                    if (!isInvoiceFileGenerated || !string.IsNullOrEmpty(exception))
                    {
                        policy.IsDone = false;
                        policy.IsLocked = false;
                        policy.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "GenerateInvoicePdf return false";
                    }
                    else
                    {
                        policy.IsDone = true;
                        policy.IsLocked = true;
                    }
                }
                catch (Exception ex)
                {
                    policy.IsDone = false;
                    policy.IsLocked = false;
                    policy.ErrorDescription = ex.ToString();
                }
                finally
                {
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    exception = string.Empty;
                    bool value = UpdateMissingTransactionPolicyProcessingQueue(policy, out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "GenerateMissingInvoicePdf.UpdateMissingTransactionPolicyProcessingQueue return null",
                            Method = "GenerateMissingInvoicePdf",
                            //ServiceRequest = 
                            CreatedDate = DateTime.Now,
                            ReferenceId = policy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    }
                }
            }
        }

        List<MissingPolicyPolicyProcessingQueue> GetMissingInvoicePDF()
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetMissingPolicyTransactions";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<MissingPolicyPolicyProcessingQueue> result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MissingPolicyPolicyProcessingQueue>(reader).ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetMissingInvoicePDF_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        bool UpdateMissingTransactionPolicyProcessingQueue(MissingPolicyPolicyProcessingQueue policy, out string exception)
        {
            exception = string.Empty;
            try
            {
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue == null)
                {
                    exception = "processQueue is null id " + policy.Id;
                    return false;
                }

                processQueue.IsLocked = policy.IsLocked;
                processQueue.IsDone = policy.IsDone;
                processQueue.IsExist = policy.IsExist;
                processQueue.MergingProcessingTries += 1;
                processQueue.MergingErrorDescription = policy.MergingErrorDescription ?? null;
                _missingPolicyPolicyProcessingQueue.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.IsLocked = false;
                    processQueue.IsDone = policy.IsDone;
                    processQueue.MergingProcessingTries += 1;
                    processQueue.MergingErrorDescription = exp.ToString();
                    _missingPolicyPolicyProcessingQueue.Update(processQueue);
                }
                return false;
            }
        }

        #endregion

        //#region Test Invalid BCare Commission Invoices

        //public bool getRefData(myobj myobj)
        //{
        //    string exception = string.Empty;
        //    var invoice = _orderService.GetInvoiceByRefrenceId(myobj.ReferenceId);
        //    var invice1 = CreateInvoice1(myobj.ReferenceId, invoice.InsuranceTypeCode.Value, myobj.CompanyId);
        //    var invice2 = CreateInvoice2(myobj.ReferenceId, invoice.InsuranceTypeCode.Value, myobj.CompanyId);
        //    return true;
        //}

        //private Invoice_1 CreateInvoice1(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0, string odReferenceId = null)
        //{
        //    var checkoutDetail = _checkoutDetailRepository.TableNoTracking
        //        .Include(chk => chk.OrderItems)
        //        //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
        //        .FirstOrDefault(c => c.ReferenceId == referenceId);

        //    bool isUsedNoAService = false;
        //    //var quotation = _quotationService.GetQuotationByReference(referenceId);
        //    //if (quotation != null && quotation.QuotationRequest != null)
        //    //{
        //    //    if (quotation.QuotationRequest.NoOfAccident.HasValue)
        //    //    {
        //    //        isUsedNoAService = true;
        //    //    }
        //    //}

        //    int numberOfDriver = 1;
        //    if (checkoutDetail.AdditionalDriverIdOne.HasValue)
        //    {
        //        numberOfDriver = 2;//main driver+ 1 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdTwo.HasValue)
        //    {
        //        numberOfDriver = 3;//main driver+ 2 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdThree.HasValue)
        //    {
        //        numberOfDriver = 4;//main driver+ 3 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdFour.HasValue)
        //    {
        //        numberOfDriver = 5;//main driver+ 4 additional
        //    }

        //    var selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
        //    decimal vatPrice = 0;
        //    decimal fees = 0;
        //    decimal extraPremiumPrice = 0;
        //    decimal discount = 0;
        //    decimal specialDiscount = 0;
        //    decimal specialDiscountPercentageValue = 0;
        //    decimal discountPercentageValue = 0;

        //    decimal loyaltyDiscountPercentageValue = 0;
        //    decimal loyaltyDiscountValue = 0;
        //    decimal specialDiscount2 = 0;
        //    decimal specialDiscount2PercentageValue = 0;

        //    decimal totalDiscount = 0;

        //    foreach (var orderItem in checkoutDetail.OrderItems)
        //    {
        //        foreach (var price in orderItem.Product.PriceDetails)
        //        {
        //            if (price.PriceTypeCode == 1)
        //            {
        //                totalDiscount += price.PriceValue;
        //                specialDiscount = price.PriceValue;
        //                specialDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 2)
        //            {
        //                totalDiscount += price.PriceValue;
        //                discountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 3)
        //            {
        //                totalDiscount += price.PriceValue;
        //                loyaltyDiscountValue = price.PriceValue;
        //                loyaltyDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 10)
        //            {
        //                totalDiscount += price.PriceValue;
        //            }
        //            if (price.PriceTypeCode == 11)
        //            {
        //                totalDiscount += price.PriceValue;
        //            }
        //            if (price.PriceTypeCode == 12)
        //            {
        //                totalDiscount += price.PriceValue;
        //                specialDiscount2 = price.PriceValue;
        //                specialDiscount2PercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            switch (price.PriceTypeCode)
        //            {
        //                case 8: vatPrice += price.PriceValue; break;
        //                case 6: fees += price.PriceValue; break;
        //                case 7: extraPremiumPrice += price.PriceValue; break;
        //                case 2: discount += price.PriceValue; break;
        //            }
        //        }
        //    }
        //    decimal benefitsPrice = selectedBenefits
        //        .Select(x => x.Price)
        //        .Sum();

        //    decimal vatValue = vatPrice + selectedBenefits
        //        .Select(x => x.Price * (decimal)0.15)
        //        .Sum();

        //    var paymentAmount = CalculateCheckoutDetailTotal(checkoutDetail);
        //    var priceExcludingBenefitsAndVatAndFees = paymentAmount - benefitsPrice - vatValue - fees;
        //    //var basicPremium = extraPremiumPrice - totalDiscount;
        //    var priceExcludingVat = paymentAmount - vatValue;
        //    var basicPrice = paymentAmount - vatValue - fees;
        //    if (insuranceCompanyId == 24)
        //    {
        //        basicPrice = basicPrice + fees;
        //    }
        //    var invoiceBenefits = (from i in selectedBenefits
        //                           select new Invoice_Benefit()
        //                           {
        //                               BenefitId = (short)i.BenefitId,
        //                               BenefitPrice = i.Price
        //                           }).ToArray();

        //    var CommissionsAndFees = GetCommissionsAndFees(insuranceTypeCode, insuranceCompanyId);
        //    decimal totalBCareCommission = 0;
        //    decimal totalBCareFees = 0;
        //    string feesCalculationDetails = string.Empty;
        //    decimal bcareDiscountAmount = 0;
        //    if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 1 && checkoutDetail.DiscountValue.HasValue)
        //    {
        //        bcareDiscountAmount = checkoutDetail.DiscountValue.Value;
        //    }
        //    if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 2 && checkoutDetail.DiscountPercentage.HasValue)
        //    {
        //        bcareDiscountAmount = (paymentAmount * checkoutDetail.DiscountPercentage.Value) / 100;
        //    }
        //    decimal paidAmount = paymentAmount - bcareDiscountAmount;
        //    foreach (var item in CommissionsAndFees)
        //    {
        //        if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != checkoutDetail.PaymentMethodId)
        //            continue;
        //        if (item.Key == "NOA" && !isUsedNoAService)
        //            continue;
        //        if (item.Key == "AMEX")
        //            continue;
        //        //if (item.IsCommission && item.Percentage.HasValue&& (item.CompanyKey== "Allianz"|| item.CompanyKey == "Sagr"))
        //        //{
        //        //    totalBCareCommission += (priceExcludingVat * item.Percentage.Value) / 100;
        //        //}
        //        //else 
        //        if (item.IsCommission && item.Percentage.HasValue)
        //        {
        //            totalBCareCommission += ((basicPrice) * item.Percentage.Value) / 100;
        //        }
        //        else
        //        {
        //            decimal percentageValue = 0;
        //            decimal fixedFeesValue = 0;
        //            if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value && numberOfDriver > 1)
        //            {
        //                percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
        //                fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

        //                totalBCareFees += percentageValue + fixedFeesValue;
        //                if (item.Percentage.HasValue && item.Percentage > 0)
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "+(" + item.Percentage + "%*" + paymentAmount + ")*" + numberOfDriver + "=";
        //                }
        //                else
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "=";
        //                }
        //            }
        //            else
        //            {
        //                percentageValue = (((paymentAmount * item.Percentage.Value) / 100));
        //                fixedFeesValue = (item.FixedFees.Value);

        //                totalBCareFees += percentageValue + fixedFeesValue;
        //                if (item.Percentage.HasValue && item.Percentage > 0)
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "+" + item.Percentage + "%*" + paymentAmount + "=";
        //                }
        //                else
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "=";
        //                }
        //            }
        //            //check negative values and deduct fees from bCare Commissions
        //            if (item.IsPercentageNegative && item.IsFixedFeesNegative)
        //            {
        //                totalBCareFees = totalBCareFees - (percentageValue + fixedFeesValue);
        //                totalBCareCommission = totalBCareCommission - (percentageValue + fixedFeesValue);
        //            }
        //            else if (item.IsPercentageNegative)
        //            {
        //                totalBCareFees = totalBCareFees - percentageValue;
        //                totalBCareCommission = totalBCareCommission - percentageValue;
        //            }
        //            else if (item.IsFixedFeesNegative)
        //            {
        //                totalBCareFees = totalBCareFees - fixedFeesValue;
        //                totalBCareCommission = totalBCareCommission - fixedFeesValue;
        //            }
        //            feesCalculationDetails += "Total:" + Math.Round(totalBCareFees, 3) + "#";
        //        }
        //    }
        //    if (totalBCareCommission > 0)
        //        totalBCareCommission = Math.Round(totalBCareCommission, 2);
        //    if (totalBCareFees > 0)
        //        totalBCareFees = Math.Round(totalBCareFees, 2);

        //    decimal actualBankFees = 0;
        //    decimal actualBankFeesWithDiscount = 0;
        //    decimal edaatAactualBankFees = 8.74M;
        //    decimal madaAactualBankFees = 1.75M;
        //    decimal visaMasterAactualBankFees = 2.3M;
        //    decimal AMEXAactualBankFees = 1.9M;
        //    decimal tabbyBankFees = 3;
        //    //decimal paymentAmountAfterBankFees = paymentAmount;
        //    decimal paymentAmountWithBCareDiscount = paymentAmount - bcareDiscountAmount;
        //    if (insuranceCompanyId == 27 && insuranceTypeCode == 1)
        //    {
        //        edaatAactualBankFees = 7.05M;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 4 || checkoutDetail.PaymentMethodId == 13)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * visaMasterAactualBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1;

        //        actualBankFees = (paymentAmount * visaMasterAactualBankFees) / 100;
        //        actualBankFees = actualBankFees + 1;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 10)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * madaAactualBankFees) / 100;
        //        actualBankFees = (paymentAmount * madaAactualBankFees) / 100;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 12)
        //    {
        //        actualBankFeesWithDiscount = edaatAactualBankFees;
        //        actualBankFees = edaatAactualBankFees;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 13)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * AMEXAactualBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

        //        //actualBankFees = (paymentAmount * AMEXAactualBankFees) / 100;
        //        //actualBankFees = actualBankFees + 1.15M;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 16 && (insuranceTypeCode == 1 || insuranceTypeCode == 7 || insuranceTypeCode == 8))
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * tabbyBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1;

        //        actualBankFees = (paymentAmount * tabbyBankFees) / 100;
        //        actualBankFees = actualBankFees + 1;
        //    }
        //    if (actualBankFees > 0)
        //    {
        //        actualBankFees = Math.Round(actualBankFees, 2);
        //    }
        //    if (actualBankFeesWithDiscount > 0)
        //    {
        //        actualBankFeesWithDiscount = Math.Round(actualBankFeesWithDiscount, 2);
        //    }
        //    decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees - actualBankFees;
        //    if (totalBCareCommission > 0 && (actualBankFees > actualBankFeesWithDiscount)) // difference between Actual bank fees before and after discount should be deducted from Bcare
        //    {
        //        decimal diff = actualBankFees - actualBankFeesWithDiscount;
        //        totalBCareCommission = totalBCareCommission + diff;
        //    }
        //    if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9 || insuranceCompanyId == 27))
        //    {
        //        totalBCareCommission = totalBCareCommission - actualBankFees;
        //        totalCompanyAmount += actualBankFees;
        //    }
        //    if (totalBCareCommission > 0 && bcareDiscountAmount > 0) // deduct discount from bcare commission
        //    {
        //        totalBCareCommission = totalBCareCommission - bcareDiscountAmount;
        //    }
        //    if ((actualBankFees > actualBankFeesWithDiscount))
        //    {
        //        actualBankFees = actualBankFeesWithDiscount;
        //    }

        //    if (insuranceTypeCode == 1) // As per Khaled && Mubarak && Mohamed Badr @@mail (updated commissions and fees ( TPL ) - Urgent)
        //    {
        //        var _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 0.7M) / 100;
        //        totalBCareCommission -= _tabbyTPLCommissionAndFeesWithBcare;
        //    }

        //    var invoice = new Invoice_1();
        //    invoice.InvoiceNo = invoiceNo == 0 ? GetNewInvoiceNumber() : invoiceNo;
        //    invoice.ReferenceId = checkoutDetail.ReferenceId;
        //    invoice.InvoiceDate = DateTime.Now;
        //    invoice.InvoiceDueDate = DateTime.Now + TimeSpan.FromHours(16);
        //    invoice.UserId = checkoutDetail.UserId;
        //    invoice.InsuranceTypeCode = insuranceTypeCode;
        //    invoice.InsuranceCompanyId = insuranceCompanyId;
        //    invoice.ProductPrice = priceExcludingBenefitsAndVatAndFees;
        //    invoice.ExtraPremiumPrice = extraPremiumPrice;
        //    invoice.Discount = discount;
        //    invoice.DiscountPercentageValue = discountPercentageValue;
        //    invoice.Fees = fees;
        //    invoice.Vat = vatValue;
        //    invoice.SubTotalPrice = paymentAmount - vatValue;
        //    invoice.TotalPrice = paymentAmount;
        //    invoice.SpecialDiscount = specialDiscount;
        //    invoice.SpecialDiscountPercentageValue = specialDiscountPercentageValue;
        //    invoice.SpecialDiscount2 = specialDiscount;
        //    invoice.SpecialDiscount2PercentageValue = specialDiscountPercentageValue;
        //    invoice.LoyaltyDiscountValue = loyaltyDiscountValue;
        //    invoice.LoyaltyDiscountPercentage = loyaltyDiscountPercentageValue;
        //    invoice.TotalBenefitPrice = benefitsPrice * (decimal)1.15;

        //    invoice.TotalBCareCommission = Math.Round(totalBCareCommission, 2);
        //    invoice.TotalBCareFees = totalBCareFees;
        //    invoice.TotalCompanyAmount = Math.Round(totalCompanyAmount, 2);

        //    //invoice.Invoice_Benefit = invoiceBenefits;
        //    invoice.ActualBankFees = Math.Round(actualBankFees, 2);
        //    invoice.FeesCalculationDetails = feesCalculationDetails;
        //    invoice.TotalBCareDiscount = Math.Round(bcareDiscountAmount, 2);
        //    invoice.TotalDiscount = Math.Round(totalDiscount, 2);
        //    invoice.BasicPrice = Math.Round(basicPrice, 2);
        //    invoice.PaidAmount = Math.Round(paidAmount, 2);
        //    if (!string.IsNullOrEmpty(odReferenceId))
        //    {
        //        invoice.ODReference = odReferenceId;
        //    }
        //    //get active template 
        //    invoice.ModifiedDate = DateTime.Now;
        //    //var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
        //    //if (templateInfo != null)
        //    //{
        //    //    invoice.TemplateId = templateInfo.Id;
        //    //}
        //    try
        //    {
        //        _Invoice_1.Insert(invoice);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        invoice.InvoiceNo = GetNewInvoiceNumber();
        //        _Invoice_1.Insert(invoice);
        //    }
        //    return invoice;

        //}
        //private Invoice_2 CreateInvoice2(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0, string odReferenceId = null)
        //{
        //    var checkoutDetail = _checkoutDetailRepository.TableNoTracking
        //        .Include(chk => chk.OrderItems)
        //        //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
        //        .FirstOrDefault(c => c.ReferenceId == referenceId);

        //    bool isUsedNoAService = false;
        //    //var quotation = _quotationService.GetQuotationByReference(referenceId);
        //    //if (quotation != null && quotation.QuotationRequest != null)
        //    //{
        //    //    if (quotation.QuotationRequest.NoOfAccident.HasValue)
        //    //    {
        //    //        isUsedNoAService = true;
        //    //    }
        //    //}

        //    int numberOfDriver = 1;
        //    if (checkoutDetail.AdditionalDriverIdOne.HasValue)
        //    {
        //        numberOfDriver = 2;//main driver+ 1 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdTwo.HasValue)
        //    {
        //        numberOfDriver = 3;//main driver+ 2 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdThree.HasValue)
        //    {
        //        numberOfDriver = 4;//main driver+ 3 additional
        //    }
        //    if (checkoutDetail.AdditionalDriverIdFour.HasValue)
        //    {
        //        numberOfDriver = 5;//main driver+ 4 additional
        //    }

        //    var selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
        //    decimal vatPrice = 0;
        //    decimal fees = 0;
        //    decimal extraPremiumPrice = 0;
        //    decimal discount = 0;
        //    decimal specialDiscount = 0;
        //    decimal specialDiscountPercentageValue = 0;
        //    decimal discountPercentageValue = 0;

        //    decimal loyaltyDiscountPercentageValue = 0;
        //    decimal loyaltyDiscountValue = 0;
        //    decimal specialDiscount2 = 0;
        //    decimal specialDiscount2PercentageValue = 0;

        //    decimal totalDiscount = 0;

        //    foreach (var orderItem in checkoutDetail.OrderItems)
        //    {
        //        foreach (var price in orderItem.Product.PriceDetails)
        //        {
        //            if (price.PriceTypeCode == 1)
        //            {
        //                totalDiscount += price.PriceValue;
        //                specialDiscount = price.PriceValue;
        //                specialDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 2)
        //            {
        //                totalDiscount += price.PriceValue;
        //                discountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 3)
        //            {
        //                totalDiscount += price.PriceValue;
        //                loyaltyDiscountValue = price.PriceValue;
        //                loyaltyDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            if (price.PriceTypeCode == 10)
        //            {
        //                totalDiscount += price.PriceValue;
        //            }
        //            if (price.PriceTypeCode == 11)
        //            {
        //                totalDiscount += price.PriceValue;
        //            }
        //            if (price.PriceTypeCode == 12)
        //            {
        //                totalDiscount += price.PriceValue;
        //                specialDiscount2 = price.PriceValue;
        //                specialDiscount2PercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
        //            }
        //            switch (price.PriceTypeCode)
        //            {
        //                case 8: vatPrice += price.PriceValue; break;
        //                case 6: fees += price.PriceValue; break;
        //                case 7: extraPremiumPrice += price.PriceValue; break;
        //                case 2: discount += price.PriceValue; break;
        //            }
        //        }
        //    }
        //    decimal benefitsPrice = selectedBenefits
        //        .Select(x => x.Price)
        //        .Sum();

        //    decimal vatValue = vatPrice + selectedBenefits
        //        .Select(x => x.Price * (decimal)0.15)
        //        .Sum();

        //    var paymentAmount = CalculateCheckoutDetailTotal(checkoutDetail);
        //    var priceExcludingBenefitsAndVatAndFees = paymentAmount - benefitsPrice - vatValue - fees;
        //    //var basicPremium = extraPremiumPrice - totalDiscount;
        //    var priceExcludingVat = paymentAmount - vatValue;
        //    var basicPrice = paymentAmount - vatValue - fees;
        //    if (insuranceCompanyId == 24)
        //    {
        //        basicPrice = basicPrice + fees;
        //    }
        //    var invoiceBenefits = (from i in selectedBenefits
        //                           select new Invoice_Benefit()
        //                           {
        //                               BenefitId = (short)i.BenefitId,
        //                               BenefitPrice = i.Price
        //                           }).ToArray();

        //    var CommissionsAndFees = GetCommissionsAndFees2(insuranceTypeCode, insuranceCompanyId);
        //    decimal totalBCareCommission = 0;
        //    decimal totalBCareFees = 0;
        //    string feesCalculationDetails = string.Empty;
        //    decimal bcareDiscountAmount = 0;
        //    if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 1 && checkoutDetail.DiscountValue.HasValue)
        //    {
        //        bcareDiscountAmount = checkoutDetail.DiscountValue.Value;
        //    }
        //    if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 2 && checkoutDetail.DiscountPercentage.HasValue)
        //    {
        //        bcareDiscountAmount = (paymentAmount * checkoutDetail.DiscountPercentage.Value) / 100;
        //    }
        //    decimal paidAmount = paymentAmount - bcareDiscountAmount;
        //    foreach (var item in CommissionsAndFees)
        //    {
        //        if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != checkoutDetail.PaymentMethodId)
        //            continue;
        //        if (item.Key == "NOA" && !isUsedNoAService)
        //            continue;
        //        if (item.Key == "AMEX")
        //            continue;
        //        //if (item.IsCommission && item.Percentage.HasValue&& (item.CompanyKey== "Allianz"|| item.CompanyKey == "Sagr"))
        //        //{
        //        //    totalBCareCommission += (priceExcludingVat * item.Percentage.Value) / 100;
        //        //}
        //        //else 
        //        if (item.IsCommission && item.Percentage.HasValue)
        //        {
        //            totalBCareCommission += ((basicPrice) * item.Percentage.Value) / 100;
        //        }
        //        else
        //        {
        //            decimal percentageValue = 0;
        //            decimal fixedFeesValue = 0;
        //            if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value && numberOfDriver > 1)
        //            {
        //                percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
        //                fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

        //                totalBCareFees += percentageValue + fixedFeesValue;
        //                if (item.Percentage.HasValue && item.Percentage > 0)
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "+(" + item.Percentage + "%*" + paymentAmount + ")*" + numberOfDriver + "=";
        //                }
        //                else
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "=";
        //                }
        //            }
        //            else
        //            {
        //                percentageValue = (((paymentAmount * item.Percentage.Value) / 100));
        //                fixedFeesValue = (item.FixedFees.Value);

        //                totalBCareFees += percentageValue + fixedFeesValue;
        //                if (item.Percentage.HasValue && item.Percentage > 0)
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "+" + item.Percentage + "%*" + paymentAmount + "=";
        //                }
        //                else
        //                {
        //                    feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "=";
        //                }
        //            }
        //            //check negative values and deduct fees from bCare Commissions
        //            if (item.IsPercentageNegative && item.IsFixedFeesNegative)
        //            {
        //                totalBCareFees = totalBCareFees - (percentageValue + fixedFeesValue);
        //                totalBCareCommission = totalBCareCommission - (percentageValue + fixedFeesValue);
        //            }
        //            else if (item.IsPercentageNegative)
        //            {
        //                totalBCareFees = totalBCareFees - percentageValue;
        //                totalBCareCommission = totalBCareCommission - percentageValue;
        //            }
        //            else if (item.IsFixedFeesNegative)
        //            {
        //                totalBCareFees = totalBCareFees - fixedFeesValue;
        //                totalBCareCommission = totalBCareCommission - fixedFeesValue;
        //            }
        //            feesCalculationDetails += "Total:" + Math.Round(totalBCareFees, 3) + "#";
        //        }
        //    }
        //    if (totalBCareCommission > 0)
        //        totalBCareCommission = Math.Round(totalBCareCommission, 2);
        //    if (totalBCareFees > 0)
        //        totalBCareFees = Math.Round(totalBCareFees, 2);

        //    decimal actualBankFees = 0;
        //    decimal actualBankFeesWithDiscount = 0;
        //    decimal edaatAactualBankFees = 8.74M;
        //    decimal madaAactualBankFees = 1.75M;
        //    decimal visaMasterAactualBankFees = 2.3M;
        //    decimal AMEXAactualBankFees = 1.9M;
        //    decimal tabbyBankFees = 3;
        //    //decimal paymentAmountAfterBankFees = paymentAmount;
        //    decimal paymentAmountWithBCareDiscount = paymentAmount - bcareDiscountAmount;
        //    if (insuranceCompanyId == 27 && insuranceTypeCode == 1)
        //    {
        //        edaatAactualBankFees = 7.05M;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 4 || checkoutDetail.PaymentMethodId == 13)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * visaMasterAactualBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1;

        //        actualBankFees = (paymentAmount * visaMasterAactualBankFees) / 100;
        //        actualBankFees = actualBankFees + 1;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 10)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * madaAactualBankFees) / 100;
        //        actualBankFees = (paymentAmount * madaAactualBankFees) / 100;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 12)
        //    {
        //        actualBankFeesWithDiscount = edaatAactualBankFees;
        //        actualBankFees = edaatAactualBankFees;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 13)
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * AMEXAactualBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

        //        //actualBankFees = (paymentAmount * AMEXAactualBankFees) / 100;
        //        //actualBankFees = actualBankFees + 1.15M;
        //    }
        //    if (checkoutDetail.PaymentMethodId == 16 && (insuranceTypeCode == 1 || insuranceTypeCode == 7 || insuranceTypeCode == 8))
        //    {
        //        actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * tabbyBankFees) / 100;
        //        actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1;

        //        actualBankFees = (paymentAmount * tabbyBankFees) / 100;
        //        actualBankFees = actualBankFees + 1;
        //    }
        //    if (actualBankFees > 0)
        //    {
        //        actualBankFees = Math.Round(actualBankFees, 2);
        //    }
        //    if (actualBankFeesWithDiscount > 0)
        //    {
        //        actualBankFeesWithDiscount = Math.Round(actualBankFeesWithDiscount, 2);
        //    }
        //    decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees - actualBankFees;
        //    if (totalBCareCommission > 0 && (actualBankFees > actualBankFeesWithDiscount)) // difference between Actual bank fees before and after discount should be deducted from Bcare
        //    {
        //        decimal diff = actualBankFees - actualBankFeesWithDiscount;
        //        totalBCareCommission = totalBCareCommission + diff;
        //    }
        //    if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9 || insuranceCompanyId == 27))
        //    {
        //        totalBCareCommission = totalBCareCommission - actualBankFees;
        //        totalCompanyAmount += actualBankFees;
        //    }
        //    if (totalBCareCommission > 0 && bcareDiscountAmount > 0) // deduct discount from bcare commission
        //    {
        //        totalBCareCommission = totalBCareCommission - bcareDiscountAmount;
        //    }
        //    if ((actualBankFees > actualBankFeesWithDiscount))
        //    {
        //        actualBankFees = actualBankFeesWithDiscount;
        //    }

        //    if (insuranceTypeCode == 1) // As per Khaled && Mubarak && Mohamed Badr @@mail (updated commissions and fees ( TPL ) - Urgent)
        //    {
        //        var _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 0.7M) / 100;
        //        totalBCareCommission -= _tabbyTPLCommissionAndFeesWithBcare;
        //    }

        //    var invoice = new Invoice_2();
        //    invoice.InvoiceNo = invoiceNo == 0 ? GetNewInvoiceNumber() : invoiceNo;
        //    invoice.ReferenceId = checkoutDetail.ReferenceId;
        //    invoice.InvoiceDate = DateTime.Now;
        //    invoice.InvoiceDueDate = DateTime.Now + TimeSpan.FromHours(16);
        //    invoice.UserId = checkoutDetail.UserId;
        //    invoice.InsuranceTypeCode = insuranceTypeCode;
        //    invoice.InsuranceCompanyId = insuranceCompanyId;
        //    invoice.ProductPrice = priceExcludingBenefitsAndVatAndFees;
        //    invoice.ExtraPremiumPrice = extraPremiumPrice;
        //    invoice.Discount = discount;
        //    invoice.DiscountPercentageValue = discountPercentageValue;
        //    invoice.Fees = fees;
        //    invoice.Vat = vatValue;
        //    invoice.SubTotalPrice = paymentAmount - vatValue;
        //    invoice.TotalPrice = paymentAmount;
        //    invoice.SpecialDiscount = specialDiscount;
        //    invoice.SpecialDiscountPercentageValue = specialDiscountPercentageValue;
        //    invoice.SpecialDiscount2 = specialDiscount;
        //    invoice.SpecialDiscount2PercentageValue = specialDiscountPercentageValue;
        //    invoice.LoyaltyDiscountValue = loyaltyDiscountValue;
        //    invoice.LoyaltyDiscountPercentage = loyaltyDiscountPercentageValue;
        //    invoice.TotalBenefitPrice = benefitsPrice * (decimal)1.15;

        //    invoice.TotalBCareCommission = Math.Round(totalBCareCommission, 2);
        //    invoice.TotalBCareFees = totalBCareFees;
        //    invoice.TotalCompanyAmount = Math.Round(totalCompanyAmount, 2);

        //    // invoice.Invoice_Benefit = invoiceBenefits;
        //    invoice.ActualBankFees = Math.Round(actualBankFees, 2);
        //    invoice.FeesCalculationDetails = feesCalculationDetails;
        //    invoice.TotalBCareDiscount = Math.Round(bcareDiscountAmount, 2);
        //    invoice.TotalDiscount = Math.Round(totalDiscount, 2);
        //    invoice.BasicPrice = Math.Round(basicPrice, 2);
        //    invoice.PaidAmount = Math.Round(paidAmount, 2);
        //    if (!string.IsNullOrEmpty(odReferenceId))
        //    {
        //        invoice.ODReference = odReferenceId;
        //    }
        //    //get active template 
        //    invoice.ModifiedDate = DateTime.Now;
        //    //var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
        //    //if (templateInfo != null)
        //    //{
        //    //    invoice.TemplateId = templateInfo.Id;
        //    //}
        //    try
        //    {
        //        _Invoice_2.Insert(invoice);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        invoice.InvoiceNo = GetNewInvoiceNumber();
        //        _Invoice_2.Insert(invoice);
        //    }
        //    return invoice;
        //}
        //private int GetNewInvoiceNumber()
        //{
        //    Random rnd = new Random(System.Environment.TickCount);
        //    int invoiceNumber = rnd.Next(111111111, 999999999);

        //    if (_invoiceRepository.Table.Any(i => i.InvoiceNo == invoiceNumber))
        //        return GetNewInvoiceNumber();

        //    return invoiceNumber;
        //}
        //public decimal CalculateCheckoutDetailTotal(CheckoutDetail checkoutDetail)
        //{
        //    decimal totalPaymentAmount = 0;
        //    foreach (var orderItem in checkoutDetail.OrderItems)
        //    {
        //        totalPaymentAmount += orderItem.Price
        //            + orderItem.OrderItemBenefits.Sum(oib => oib.Price * ((decimal)1.15));
        //    }

        //    return totalPaymentAmount;
        //}
        //public List<CommissionsAndFees> GetCommissionsAndFees(int insuranceTypeCode, int companyId)
        //{
        //    try
        //    {
        //        return _commissionsAndFees.TableNoTracking.Where(a => a.CompanyId == companyId && a.InsuranceTypeCode == insuranceTypeCode).ToList();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        //public List<CommissionsAndFees> GetCommissionsAndFees2(int insuranceTypeCode, int companyId)
        //{
        //    try
        //    {
        //        return _commissionsAndFees.TableNoTracking.Where(a => a.CompanyId == companyId && a.InsuranceTypeCode == insuranceTypeCode).ToList();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //#endregion

        public bool SendSmsRenewalNotificationsNew(DateTime start, DateTime end, int notificationNo, string taskName)
        {
            if (notificationNo >3)
            {
                return false;
            }
            SMSNotification log = new SMSNotification();
            log.StartDate = start;
            log.EndDate = end;
            log.NotificationNo = notificationNo;
            log.TaskName = taskName;
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                string exception = string.Empty;
                var policies = _checkoutContext.GetRenewalPolicies(start, end, notificationNo, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 1;
                    log.ErrorDescription = "failed to GetRenewalPolicies due to " + exception;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                if (policies.Count == 0)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "GetRenewalPolicies count is 0";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                    return false;
                }
                var skippedNumbers = _smsSkippedNumbers.TableNoTracking.ToList();
                var discountCode = _renewalDiscount.TableNoTracking
                                                   .Where(a => a.IsActive && a.MessageType == notificationNo && a.CodeType == 2 && (a.EndDate.HasValue && a.EndDate.Value >= end.Date))
                                                   .OrderByDescending(a => a.Id).FirstOrDefault();

                foreach (var p in policies)
                {
                    dtBeforeCalling = DateTime.Now;
                    if (p.Channel.ToLower() == "autoleasing")
                        continue;
                    if (skippedNumbers.Where(a => a.PhoneNo == p.Phone).FirstOrDefault() != null)
                        continue;
                    log = new SMSNotification();
                    log.TaskName = taskName;
                    log.StartDate = start;
                    log.EndDate = end;
                    log.NotificationNo = notificationNo;
                    string smsBody = string.Empty;
                    log.MobileNumber = p.Phone;
                    log.ReferenceId = p.ReferenceId;
                    exception = string.Empty;
                    string vehicleId = p.SequenceNumber == null ? p.CustomCardNumber : p.SequenceNumber;
                    log.SequenceNumber = p.SequenceNumber;
                    log.CustomCard = p.CustomCardNumber;
                    var policy = _policyRepository.Table.Where(a => a.CheckOutDetailsId == p.ReferenceId).FirstOrDefault();
                    if (policy == null)
                    {
                        log.ErrorCode = 8;
                        log.ErrorDescription = "policy is null";
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    var info = SMSNotificationDataAccess.GetFromMSNotification(p.Phone, vehicleId, notificationNo, p.ReferenceId);
                    if (info != null && info.Count() > 0)
                    {
                        log.ErrorCode = 8;
                        log.ErrorDescription = "this referenceId: " + p.ReferenceId + "already sent before with total count " + info.Count() + " and notificationNo is " + notificationNo;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);
                        continue;
                    }
                    string make = string.Empty;
                    string model = string.Empty;
                    string VechilePlatInfo = string.Empty;
                    string url = string.Empty;
                    bool isCustomCardConverted = false;
                    if (!string.IsNullOrEmpty(p.CustomCardNumber))
                    {
                        CustomCardQueue customCardInfo = new CustomCardQueue();
                        customCardInfo.UserId = p.UserId;
                        customCardInfo.CustomCardNumber = p.CustomCardNumber;
                        customCardInfo.ModelYear = p.ModelYear;
                        customCardInfo.Channel = p.Channel;
                        customCardInfo.CompanyID = p.InsuranceCompanyId;
                        customCardInfo.CompanyName = p.InsuranceCompanyName;
                        customCardInfo.VehicleId = p.VehicleId;
                        customCardInfo.ReferenceId = p.ReferenceId;
                        customCardInfo.PolicyNo = p.PolicyNo;
                        var customCardOutput = _policyNotificationContext.GetCustomCardInfo(customCardInfo);
                        if (customCardOutput.ErrorCode != UpdateCustomCardOutput.ErrorCodes.Success)
                        {
                            log.ErrorCode = 500;
                            log.ErrorDescription = "falied to get custom card info due to " + customCardOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                        }
                        isCustomCardConverted = true;

                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText1))
                            p.CarPlateText1 = customCardOutput.CarPlateText1;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText2))
                            p.CarPlateText2 = customCardOutput.CarPlateText2;
                        if (!string.IsNullOrEmpty(customCardOutput.CarPlateText3))
                            p.CarPlateText3 = customCardOutput.CarPlateText3;
                        if (customCardOutput.CarPlateNumber.HasValue)
                            p.CarPlateNumber = customCardOutput.CarPlateNumber;

                        if (!string.IsNullOrEmpty(customCardOutput.SequenceNumber))
                        {
                            vehicleId = customCardOutput.SequenceNumber;
                            log.SequenceNumber = customCardOutput.SequenceNumber;
                        }
                    }
                    if (!string.IsNullOrEmpty(p.SequenceNumber) || isCustomCardConverted)
                    {
                        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog
                        {
                            Channel = p.Channel,
                            ServerIP = Utilities.GetInternalServerIP()
                        };
                        if (!string.IsNullOrEmpty(p.UserId))
                        {
                            Guid userIdGUID = Guid.Empty;
                            Guid.TryParse(p.UserId, out userIdGUID);
                            predefinedLogInfo.UserID = userIdGUID;
                        }
                        predefinedLogInfo.VehicleId = vehicleId;
                        predefinedLogInfo.DriverNin = p.CarOwnerNIN;
                        predefinedLogInfo.ReferenceId = p.ReferenceId;

                        var vehicleYakeenRequest = new VehicleYakeenRequestDto()
                        {
                            VehicleId = Convert.ToInt64(vehicleId),
                            VehicleIdTypeId = 1,
                            OwnerNin = p.OwnerTransfer ? Convert.ToInt64(p.NationalId) : Convert.ToInt64(p.CarOwnerNIN)
                        };
                        var driverInfo = _yakeenClient.GetCarInfoBySequenceInfo(vehicleYakeenRequest, predefinedLogInfo);
                        if (driverInfo.ErrorCode != YakeenOutput.ErrorCodes.Success)
                        {
                            policy.RenewalNotificationStatus = "SoldOut";
                            policy.NotificationNo = notificationNo;
                            _policyRepository.Update(policy);

                            log.ErrorCode = 15;
                            log.ErrorDescription = "can't send sms due to " + driverInfo.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                    }
                    DateTime beforeCallingDatabase = DateTime.Now;
                    var policyInfo = _vehicleService.GetVehiclePolicyDetails(vehicleId, out exception);
                    DateTime afterCallingDatabase = DateTime.Now;
                    if (!string.IsNullOrEmpty(exception))
                    {
                        log.ErrorCode = 3;
                        log.ErrorDescription = "GetVehiclePolicy returned exp: " + exception + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (policyInfo == null && isCustomCardConverted)
                    {
                        policyInfo = _vehicleService.GetVehiclePolicyDetails(p.CustomCardNumber, out exception);
                    }
                    if (policyInfo == null)
                    {
                        log.ErrorCode = 4;
                        log.ErrorDescription = "policyInfo is null" + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (!policyInfo.PolicyExpiryDate.HasValue)
                    {
                        policy.RenewalNotificationStatus = "PolicyExpiryDate is null";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 5;
                        log.ErrorDescription = "PolicyExpiryDate is null and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    if (policyInfo.PolicyExpiryDate.HasValue
                        && policyInfo.PolicyExpiryDate.Value.Date.Year > end.Year)
                    {
                        policy.IsRenewed = true;
                        policy.RenewalNotificationStatus = "AlreadyRenewed";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 6;
                        log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    if (policyInfo.PolicyExpiryDate.HasValue
                       && policyInfo.PolicyExpiryDate.Value.Date > end.AddMonths(5).Date)
                    {
                        policy.IsRenewed = true;
                        policy.RenewalNotificationStatus = "StillValid";
                        policy.NotificationNo = notificationNo;
                        _policyRepository.Update(policy);

                        log.ErrorCode = 16;
                        log.ErrorDescription = "policy already valid or renewed with reference " + policyInfo.CheckOutDetailsId + " PolicyExpiryDate>>end.Year as PolicyExpiryDate is " + policyInfo.PolicyExpiryDate.Value + " and end.Year is " + end.Year + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    bool sendSms = true;
                    if (p.PolicyExpiryDate < start)
                    {
                        sendSms = false;
                    }
                    if (p.PolicyExpiryDate > end)
                    {
                        sendSms = false;
                    }
                    log.PolicyExpiryDate = policyInfo.PolicyExpiryDate;
                    if (!sendSms)
                    {
                        log.ErrorCode = 7;
                        log.ErrorDescription = "don't send sms due to as p.PolicyExpiryDate is " + p.PolicyExpiryDate + " and start is " + start + " and enddate is " + end + " and reference is " + policyInfo.CheckOutDetailsId + "; time consumed for DB:" + afterCallingDatabase.Subtract(beforeCallingDatabase).TotalSeconds;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        SMSNotificationDataAccess.AddToSMSNotification(log);
                        continue;
                    }
                    var discountMsg = string.Empty;
                    if (discountCode != null)
                    {
                        VehicleDiscounts VehicleDiscountmodel = new VehicleDiscounts()
                        {
                            CreatedDate = DateTime.Now,
                            DiscountCode = discountCode.Code,
                            IsUsed = false,
                            SequenceNumber = p.SequenceNumber,
                            CustomCardNumber = p.CustomCardNumber,
                            Nin = p.CarOwnerNIN
                        };
                        string ex = string.Empty;
                        var CodeWithVech = LinkVechWithDiscountCode(VehicleDiscountmodel, out ex);
                        if (!string.IsNullOrEmpty(ex))
                        {
                            log.ErrorCode = 5;
                            log.ErrorDescription = "Error happend while insert Data Vehcile Discount , and the error is " + ex;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                        if (CodeWithVech != null && !string.IsNullOrEmpty(CodeWithVech.DiscountCode))
                        {
                            if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                            {
                                discountMsg = "ولأنك من عائلة بي كير نقدم لك كود خصم {0} صالح لمده 48 ساعة  يمكنك استخدامه في مرحلة الدفع لمرة واحدة";
                            }
                            else
                            {
                                discountMsg = "Because you are a member of BCare family, we are providing you with coupon discount {0} valid for 48 hours you can only use it once at the payment step";
                            }
                            discountMsg = discountMsg.Replace("{0}", CodeWithVech.DiscountCode);
                        }
                    }

                    string emailPlainText = string.Empty;

                    string emo_heart = DecodeEncodedNonAsciiCharacters("\uD83D\uDC99");
                    string emo_calender = DecodeEncodedNonAsciiCharacters("\uD83D\uDCC5");

                    if (!p.SelectedLanguage.HasValue || p.SelectedLanguage == (int)LanguageTwoLetterIsoCode.Ar)
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                            || !string.IsNullOrEmpty(p.CarPlateText2)
                            || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                            p.CarPlateText2, p.CarPlateText3,
                            p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "ar");
                        }

                        //create body of SMS MSG


                        if ((notificationNo == 1) && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                ///smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;

                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/";
                                //url = "https://bcare.com.sa/";
                            }
                        }
                        else if ((notificationNo == 1 ) && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%])، بدون تأمين[%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //  smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {

                            if (isCustomCardConverted)
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;

                                emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                               // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa";
                                // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa";

                                emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%][%discountMsg%] قارن بين ٢٣ شركة تأمين وجدد بضغطة زر https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%] جدد الآن" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }

                        //create body of WhatsApp 

                        if (notificationNo == 2 || notificationNo == 3)      // spcial case for whats App Body
                        {
                            if (isCustomCardConverted)
                            {

                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;

                           //     smsBody = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "أنت تقود مركبتك [%Make%] [%Model%] ([%PLATE%]) بدون تأمين \n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء صلاحية الرابط الحالي ادخل وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "ينتهي تأمين مركبتك [%Make%] [%Model%] ([%PLATE%]) [%ExpiryDate%]\n\n يمكنك الآن التقسيط على 4 أشهر لجميع شركات التأمين " + emo_calender + "\n\n ينتهي عرض التقسيط بانتهاء تأمينك الحالي ادخل الرابط وجدد الآن! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                        }

                        // test  sms and whats  for mubarak and mohamed Majed

                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescAr))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescAr);
                            emailPlainText = emailPlainText.Replace("[%Make%]", p.MakerDescAr);
                            make = p.MakerDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(p.ModelDescAr))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescAr);
                            emailPlainText = emailPlainText.Replace("[%Model%]", p.ModelDescAr);
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            string PolicyExpiryDate = "";
                            PolicyExpiryDate = "في ";
                            PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("en-US"));
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("dd", new System.Globalization.CultureInfo("ar-eg")) + " ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("MMMM", new System.Globalization.CultureInfo("ar-eg")) + ", ";
                            //PolicyExpiryDate += p.PolicyExpiryDate.ToString("yyyy", new System.Globalization.CultureInfo("ar-eg"));
                            smsBody = smsBody.Replace("[%ExpiryDate%]", PolicyExpiryDate);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", PolicyExpiryDate);
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", string.Empty);
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else if (string.IsNullOrEmpty(discountMsg) && smsBody.Contains("ينتهي تأمين مركبتك"))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "،");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "");
                        }
                    }
                    else
                    {
                        string platinfo = string.Empty;
                        if (!string.IsNullOrEmpty(p.CarPlateText1)
                           || !string.IsNullOrEmpty(p.CarPlateText2)
                           || !string.IsNullOrEmpty(p.CarPlateText3))
                        {
                            platinfo = _checkoutContext.GetCarPlateInfo(p.CarPlateText1,
                           p.CarPlateText2, p.CarPlateText3,
                           p.CarPlateNumber.HasValue ? p.CarPlateNumber.Value : 0, "en");
                        }
                        if (notificationNo == 1 && !string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                //smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //  smsBody = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                //  smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                //smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else if (notificationNo == 1  && string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            //smsBody = "Dear Customer, you are driving your vehicle [%Make%] [%Model%] ([%PLATE%]). without insurance[%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "You are driving your vehicle [%Make%] [%Model%] ([%PLATE%]) without insurance \n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once the below link expires, click now and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }
                        else if (!string.IsNullOrEmpty(p.CustomCardNumber))
                        {
                            if (isCustomCardConverted)
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }
                        else
                        {
                            smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            //smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                        }

                        if (notificationNo == 2 || notificationNo == 3) // whats app body
                        {
                            if (isCustomCardConverted)
                            {

                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                url = "https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                            }
                            else
                            {
                                //smsBody = "Dear Customer, your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) will expire in [%ExpiryDate%][%discountMsg%] Compare between 23 insurance companies and renew with one click https://bcare.com.sa/";
                                // smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]" + "click now on the link and renew it!" + " https://bcare.com.sa/?eid=" + p.ExternalId + "&r=1&re=" + p.ReferenceId;
                                smsBody = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                emailPlainText = "Your vehicle insurance [%Make%] [%Model%] ([%PLATE%]) expires on [%ExpiryDate%]\n\n Now you can divide your payment over 4 months with all insurance companies " + emo_calender + "\n\n This offer ends once your insurance policy expires, click now on the link and renew it! " + emo_heart + "\n\n https://bcare.com.sa";
                                url = "https://bcare.com.sa/";
                            }
                        }


                        if (!string.IsNullOrEmpty(platinfo))
                        {
                            smsBody = smsBody.Replace("[%PLATE%]", platinfo);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", platinfo);
                            VechilePlatInfo = platinfo;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("([%PLATE%])", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%PLATE%]", string.Empty);
                            VechilePlatInfo = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.MakerDescEn))
                        {
                            smsBody = smsBody.Replace("[%Make%]", p.MakerDescEn);
                            emailPlainText = emailPlainText.Replace("[%Make%]", p.MakerDescEn);
                            make = p.MakerDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Make%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Make%]", string.Empty);
                            make = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(p.ModelDescEn))
                        {
                            smsBody = smsBody.Replace("[%Model%]", p.ModelDescEn);
                            emailPlainText = emailPlainText.Replace("[%Model%]", p.ModelDescEn);
                            model = p.ModelDescEn;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%Model%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%Model%]", string.Empty);
                            model = string.Empty;
                        }
                        if (p.PolicyExpiryDate != null)
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", p.PolicyExpiryDate.ToString("dd/MM/yyyy", new CultureInfo("en-us")));
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                            model = p.ModelDescAr;
                        }
                        else
                        {
                            smsBody = smsBody.Replace("[%ExpiryDate%]", string.Empty);
                            emailPlainText = emailPlainText.Replace("[%ExpiryDate%]", string.Empty);
                            model = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(discountMsg))
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", "\r\n" + discountMsg + "\r\n");
                        }
                        else
                        {
                            smsBody = smsBody.Trim().Replace("[%discountMsg%]", ",");
                        }
                    }
                    smsBody = smsBody.Trim();
                    log.SMSMessage = smsBody;
                    exception = string.Empty;
                    if (notificationNo == 2|| notificationNo == 3) // whatisapp only with 14 days notification
                    {
                        var count = WhatsAppLogDataAccess.GetFromWhatsAppNotification(p.ReferenceId);
                        if (count == 0)
                        {
                            _notificationService.SendWhatsAppMessageForPolicyRenewalAsync(p.Phone, smsBody, make, model, VechilePlatInfo, url, SMSMethod.PolicyRenewal.ToString(), p.ReferenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), p.SelectedLanguage).ToLower(), p.PolicyExpiryDate.ToString("dd MMMM yyyy", new CultureInfo("en-us")));
                        }
                    }
                    var smsModel = new SMSModel()
                    {
                        PhoneNumber = p.Phone,
                        MessageBody = smsBody,
                        Method = SMSMethod.PolicyRenewal.ToString(),
                        Module = Module.Vehicle.ToString()
                    };
                    // send renewal via stc sms 
                    if (notificationNo==1) 
                    {
                        if (string.IsNullOrEmpty(smsModel.PhoneNumber))
                        {
                            continue;
                        }
                        var smsOutput = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                        if (smsOutput.ErrorCode == 12)
                        {
                            log.ErrorCode = 10;
                            log.ErrorDescription = "sms failed to sent";
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                        if (smsOutput.ErrorCode != 0)
                        {
                            log.ErrorCode = 9;
                            log.ErrorDescription = "sms failed to sent: " + smsOutput.ErrorDescription;
                            log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                            SMSNotificationDataAccess.AddToSMSNotification(log);
                            continue;
                        }
                    }
               

                    //// send renewal email for verified email only
                    //if (p.IsEmailVerified)
                    //{
                    //    try
                    //    {
                    //        emailPlainText = emailPlainText.Trim();
                    //        string emailSubject = ((LanguageTwoLetterIsoCode)p.SelectedLanguage == LanguageTwoLetterIsoCode.Ar) ? "اشعار تجديد وثيقة تأمين ( [%PolicyNo%] )" : "Policy Renewal Notification ( [%PolicyNo%] )";
                    //        if (!string.IsNullOrEmpty(p.PolicyNo))
                    //            emailSubject = emailSubject.Replace("[%PolicyNo%]", p.PolicyNo);
                    //        else
                    //            emailSubject = emailSubject.Replace("[%PolicyNo%]", string.Empty);

                    //        //if (emailPlainText.Contains("\n\n"))
                    //        //    emailPlainText = string.Format(emailPlainText.Replace("\n\n", "<br>"));

                    //        MessageBodyModel messageBodyModel = new MessageBodyModel();
                    //        messageBodyModel.MessageBody = emailPlainText;
                    //        messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/Welcome.png";
                    //        messageBodyModel.Language = (((LanguageTwoLetterIsoCode)p.SelectedLanguage).ToString()).ToLower();

                    //        EmailModel emailModel = new EmailModel();
                    //        emailModel.ReferenceId = p.ReferenceId;
                    //        emailModel.To = new List<string>();
                    //        emailModel.To.Add(p.Email);
                    //        emailModel.Subject = emailSubject;
                    //        emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                    //        emailModel.Module = "Vehicle";
                    //        emailModel.Method = "RenewalNotification";
                    //        emailModel.Channel = Channel.Portal.ToString();
                    //        var smemailOutput = _notificationService.SendEmail(emailModel);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SendSmsRenewalNotifications_SendEmail_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", JsonConvert.SerializeObject(ex.ToString()));
                    //    }
                    //}

                    ////
                    /// stop checking for channel and try to send notification regardless policy channel
                    //if (AppNotificationChannels.Contains(p.Channel.ToLower()))
                    _notification.SendFireBaseNotification(p.UserId, "إشعار التجديد - Renewal Notification", smsBody, SMSMethod.PolicyRenewal.ToString(), p.ReferenceId, p.Channel);

                    policy.RenewalNotificationStatus = "Notification" + notificationNo + "Sent";
                    policy.NotificationNo = notificationNo;
                    _policyRepository.Update(policy);
                    log.ErrorCode = 0;
                    log.ErrorDescription = "Success";
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    SMSNotificationDataAccess.AddToSMSNotification(log);
                }
                return true;
            }
            catch (Exception exp)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\SendSmsRenewalNotifications_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", JsonConvert.SerializeObject(exp.ToString()));
                log.ErrorCode = 11;
                log.ErrorDescription = "exp: " + exp.ToString();
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                SMSNotificationDataAccess.AddToSMSNotification(log);
                return false;
            }
        }

        
    }
}
