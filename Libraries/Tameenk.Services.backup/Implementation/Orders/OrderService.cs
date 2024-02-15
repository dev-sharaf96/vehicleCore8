using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Implementation.Orders
{
    public class OrderService : IOrderService
    {
        #region Fields

        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRepository<BankCode> _bankCodeRepository;
        private readonly ILogger _logger;
        private readonly IRepository<CheckoutUsers> _checkoutUsersRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderItemBenefit> _orderItemBenfitsRepository;
        private readonly IRepository<Invoice_Benefit> _invoiceBenefitRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<CommissionsAndFees> _commissionsAndFees;
        private readonly IQuotationService _quotationService;
        private readonly IRepository<InvoiceFileTemplates> _invoiceFileTemplatesRepository;
        private readonly IRepository<VehicleDiscounts> _vehicleDiscountsRepository;
        private readonly IRepository<PolicyModification> _policyModificationRepository;

        private readonly HashSet<int> TabbyTPLCommissionAndFeesWithBcare = new HashSet<int> { 2, 4, 8, 12, 13, 14, 18, 20, 22, 23, 24, 27 };

        #endregion

        #region Ctor
        public OrderService(IRepository<CheckoutDetail> checkoutDetailRepository,
            IRepository<Invoice> invoiceRepository,
            IShoppingCartService shoppingCartService
            , IRepository<BankCode> bankCodeRepository
            , ILogger logger, IRepository<CheckoutUsers> checkoutUsersRepository, IRepository<OrderItem> orderItemRepository, IRepository<OrderItemBenefit> orderItemBenfitsRepository, IRepository<Invoice_Benefit> invoiceBenefitRepository, ICacheManager cacheManager,
             IRepository<CommissionsAndFees> commissionsAndFees, IQuotationService quotationService, IRepository<InvoiceFileTemplates> invoiceFileTemplatesRepository,
                                IRepository<VehicleDiscounts> vehicleDiscountsRepository, IRepository<PolicyModification> policyModificationRepository)
        {
            _checkoutDetailRepository = checkoutDetailRepository;
            _invoiceRepository = invoiceRepository;
            _shoppingCartService = shoppingCartService;
            _bankCodeRepository = bankCodeRepository ?? throw new ArgumentNullException(nameof(IRepository<BankCode>));

            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            _checkoutUsersRepository = checkoutUsersRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutUsers>));
            _orderItemRepository = orderItemRepository;
            _orderItemBenfitsRepository = orderItemBenfitsRepository;
            _invoiceBenefitRepository = invoiceBenefitRepository;
            _cacheManager = cacheManager ?? throw new TameenkArgumentNullException(nameof(ICacheManager));
            _commissionsAndFees = commissionsAndFees;
            _quotationService = quotationService;
            _invoiceFileTemplatesRepository = invoiceFileTemplatesRepository;
            _vehicleDiscountsRepository = vehicleDiscountsRepository;
            _policyModificationRepository = policyModificationRepository;
        }

        #endregion

        #region Methods

        #region WebSite profile APIs



        /// <summary>
        /// Get all bank to specific user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<UserBank> GetUserBanks(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {

            var query = (from checkOutDetails in _checkoutDetailRepository.Table
                         join banks in _bankCodeRepository.Table on checkOutDetails.BankCodeId equals banks.Id
                         where checkOutDetails.UserId == id
                         select new UserBank
                         {
                             IBAN = checkOutDetails.IBAN,
                             Code = banks.Code,
                             ValidationCode = banks.ValidationCode,
                             ArabicDescription = banks.ArabicDescription,
                             EnglishDescription = banks.EnglishDescription
                         }).Distinct();

            return new PagedList<UserBank>(query, pageIndx, pageSize, "Code", true);
        }

        #endregion

        public IEnumerable<BankCode> GetBankCodes()
        {
            return _cacheManager.Get("__ALL_bAnk_CodeS", 180, () =>
            {
                return _bankCodeRepository.TableNoTracking.ToList();
            });
        }

        //public Invoice CreateInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0, string odReferenceId = null)
        //{
        //    var checkoutDetail = _checkoutDetailRepository.TableNoTracking
        //        .Include(chk => chk.OrderItems)
        //        //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
        //        .FirstOrDefault(c => c.ReferenceId == referenceId);

        //    bool isUsedNoAService = false;
        //    var quotation = _quotationService.GetQuotationByReference(referenceId);
        //    if(quotation!=null&& quotation.QuotationRequest!=null)
        //    {
        //        if(quotation.QuotationRequest.NoOfAccident.HasValue)
        //        {
        //            isUsedNoAService = true;
        //        }
        //    }

        //    int numberOfDriver = 1;
        //    if(checkoutDetail.AdditionalDriverIdOne.HasValue)
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
        //    var priceExcludingVat= paymentAmount -vatValue;
        //    var basicPrice = paymentAmount - vatValue - fees;
        //    if(insuranceCompanyId==24)
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
        //    decimal paidAmount = paymentAmount- bcareDiscountAmount;
        //    foreach (var item in CommissionsAndFees)
        //    {
        //        if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != checkoutDetail.PaymentMethodId)
        //            continue;
        //         if (item.Key == "NOA" && !isUsedNoAService)
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
        //            if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value&& numberOfDriver>1)
        //            {
        //                percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
        //                fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

        //                totalBCareFees += percentageValue+ fixedFeesValue;
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
        //            else if(item.IsPercentageNegative)
        //            {
        //                totalBCareFees = totalBCareFees - percentageValue ;
        //                totalBCareCommission = totalBCareCommission - percentageValue;
        //            }
        //            else if (item.IsFixedFeesNegative)
        //            {
        //                totalBCareFees = totalBCareFees -  fixedFeesValue;
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
        //    if(insuranceCompanyId==27 && insuranceTypeCode==1)
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
        //    if (checkoutDetail.PaymentMethodId == 16 &&(insuranceTypeCode==1|| insuranceTypeCode==7 || insuranceTypeCode==8))
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
        //    decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees-actualBankFees;
        //    if (totalBCareCommission > 0 && (actualBankFees > actualBankFeesWithDiscount)) // difference between Actual bank fees before and after discount should be deducted from Bcare
        //    {
        //        decimal diff = actualBankFees - actualBankFeesWithDiscount;
        //        totalBCareCommission = totalBCareCommission + diff;
        //    }
        //    if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9|| insuranceCompanyId == 27))
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

        //    if (insuranceTypeCode == 1 && TabbyTPLCommissionAndFeesWithBcare.Contains(insuranceCompanyId)) // As per Khaled && Mubarak && Mohamed Badr @@mail (updated commissions and fees ( TPL ) - Urgent)
        //    {
        //        var _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 0.7M) / 100;
        //        totalBCareCommission -= _tabbyTPLCommissionAndFeesWithBcare;
        //    }

        //    var invoice = new Invoice();
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

        //    invoice.Invoice_Benefit = invoiceBenefits;
        //    invoice.ActualBankFees = Math.Round(actualBankFees, 2);
        //    invoice.FeesCalculationDetails = feesCalculationDetails;
        //    invoice.TotalBCareDiscount = Math.Round(bcareDiscountAmount, 2);
        //    invoice.TotalDiscount = Math.Round(totalDiscount, 2) ;
        //    invoice.BasicPrice = Math.Round(basicPrice, 2) ;
        //    invoice.PaidAmount = Math.Round(paidAmount, 2) ;
        //    if (!string.IsNullOrEmpty(odReferenceId))
        //    {
        //        invoice.ODReference = odReferenceId;
        //    }
        //    //get active template 
        //    invoice.ModifiedDate = DateTime.Now;
        //    var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
        //    if (templateInfo != null)
        //    {
        //        invoice.TemplateId = templateInfo.Id;
        //    }
        //    try
        //    {
        //        _invoiceRepository.Insert(invoice);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        invoice.InvoiceNo = GetNewInvoiceNumber();
        //        _invoiceRepository.Insert(invoice);
        //    }
        //    return invoice;

        //}


        #region Before New Logic (CommissionAndFees contain ActualBankFees)

        public Invoice CreateInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0, string odReferenceId = null)
        {
            var checkoutDetail = _checkoutDetailRepository.TableNoTracking
                .Include(chk => chk.OrderItems)
                //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
                .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
                .FirstOrDefault(c => c.ReferenceId == referenceId);

            bool isUsedNoAService = false;
            var quotation = _quotationService.GetQuotationByReference(referenceId);
            if (quotation != null && quotation.QuotationRequest != null)
            {
                if (quotation.QuotationRequest.NoOfAccident.HasValue)
                {
                    isUsedNoAService = true;
                }
            }

            int numberOfDriver = 1;
            if (checkoutDetail.AdditionalDriverIdOne.HasValue)
            {
                numberOfDriver = 2;//main driver+ 1 additional
            }
            if (checkoutDetail.AdditionalDriverIdTwo.HasValue)
            {
                numberOfDriver = 3;//main driver+ 2 additional
            }
            if (checkoutDetail.AdditionalDriverIdThree.HasValue)
            {
                numberOfDriver = 4;//main driver+ 3 additional
            }
            if (checkoutDetail.AdditionalDriverIdFour.HasValue)
            {
                numberOfDriver = 5;//main driver+ 4 additional
            }

            var selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
            decimal vatPrice = 0;
            decimal fees = 0;
            decimal extraPremiumPrice = 0;
            decimal discount = 0;
            decimal specialDiscount = 0;
            decimal specialDiscountPercentageValue = 0;
            decimal discountPercentageValue = 0;

            decimal loyaltyDiscountPercentageValue = 0;
            decimal loyaltyDiscountValue = 0;
            decimal specialDiscount2 = 0;
            decimal specialDiscount2PercentageValue = 0;

            decimal totalDiscount = 0;

            foreach (var orderItem in checkoutDetail.OrderItems)
            {
                foreach (var price in orderItem.Product.PriceDetails)
                {
                    if (price.PriceTypeCode == 1)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount = price.PriceValue;
                        specialDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 2)
                    {
                        totalDiscount += price.PriceValue;
                        discountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 3)
                    {
                        totalDiscount += price.PriceValue;
                        loyaltyDiscountValue = price.PriceValue;
                        loyaltyDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 10)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 11)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 12)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount2 = price.PriceValue;
                        specialDiscount2PercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    switch (price.PriceTypeCode)
                    {
                        case 8: vatPrice += price.PriceValue; break;
                        case 6: fees += price.PriceValue; break;
                        case 7: extraPremiumPrice += price.PriceValue; break;
                        case 2: discount += price.PriceValue; break;
                    }
                }
            }
            decimal benefitsPrice = selectedBenefits
                .Select(x => x.Price)
                .Sum();

            decimal vatValue = vatPrice + selectedBenefits
                .Select(x => x.Price * (decimal)0.15)
                .Sum();

            var paymentAmount = CalculateCheckoutDetailTotal(checkoutDetail);
            var priceExcludingBenefitsAndVatAndFees = paymentAmount - benefitsPrice - vatValue - fees;
            //var basicPremium = extraPremiumPrice - totalDiscount;
            var priceExcludingVat = paymentAmount - vatValue;
            var basicPrice = paymentAmount - vatValue - fees;
            if (insuranceCompanyId == 24)
            {
                basicPrice = basicPrice + fees;
            }
            var invoiceBenefits = (from i in selectedBenefits
                                   select new Invoice_Benefit()
                                   {
                                       BenefitId = (short)i.BenefitId,
                                       BenefitPrice = i.Price
                                   }).ToArray();

            var CommissionsAndFees = GetCommissionsAndFees(insuranceTypeCode, insuranceCompanyId);
            decimal totalBCareCommission = 0;
            decimal totalBCareFees = 0;
            string feesCalculationDetails = string.Empty;
            decimal bcareDiscountAmount = 0;
            if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 1 && checkoutDetail.DiscountValue.HasValue)
            {
                bcareDiscountAmount = checkoutDetail.DiscountValue.Value;
            }
            if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 2 && checkoutDetail.DiscountPercentage.HasValue)
            {
                bcareDiscountAmount = (paymentAmount * checkoutDetail.DiscountPercentage.Value) / 100;
            }
            decimal paidAmount = paymentAmount - bcareDiscountAmount;
            foreach (var item in CommissionsAndFees)
            {
                if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != checkoutDetail.PaymentMethodId)
                    continue;
                if (item.Key == "NOA" && !isUsedNoAService)
                    continue;
                if (item.Key == "AMEX")
                    continue;
                //if (item.IsCommission && item.Percentage.HasValue&& (item.CompanyKey== "Allianz"|| item.CompanyKey == "Sagr"))
                //{
                //    totalBCareCommission += (priceExcludingVat * item.Percentage.Value) / 100;
                //}
                //else 
                if (item.IsCommission && item.Percentage.HasValue)
                {
                    totalBCareCommission += ((basicPrice) * item.Percentage.Value) / 100;
                }
                else
                {
                    decimal percentageValue = 0;
                    decimal fixedFeesValue = 0;
                    if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value && numberOfDriver > 1)
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
                        fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "+(" + item.Percentage + "%*" + paymentAmount + ")*" + numberOfDriver + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "=";
                        }
                    }
                    else
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100));
                        fixedFeesValue = (item.FixedFees.Value);
                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "+" + item.Percentage + "%*" + paymentAmount + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "=";
                        }
                    }
                    //check negative values and deduct fees from bCare Commissions
                    if (item.IsPercentageNegative && item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - (percentageValue + fixedFeesValue);
                        totalBCareCommission = totalBCareCommission - (percentageValue + fixedFeesValue);
                    }
                    else if (item.IsPercentageNegative)
                    {
                        totalBCareFees = totalBCareFees - percentageValue;
                        totalBCareCommission = totalBCareCommission - percentageValue;
                    }
                    else if (item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - fixedFeesValue;
                        totalBCareCommission = totalBCareCommission - fixedFeesValue;
                    }
                    feesCalculationDetails += "Total:" + Math.Round(totalBCareFees, 3) + "#";
                }
            }
            if (totalBCareCommission > 0)
                totalBCareCommission = Math.Round(totalBCareCommission, 2);
            if (totalBCareFees > 0)
                totalBCareFees = Math.Round(totalBCareFees, 2);

            decimal actualBankFees = 0;
            decimal actualBankFeesWithDiscount = 0;
            decimal edaatAactualBankFees = 8.74M;
            decimal madaAactualBankFees = 2.013M;
            decimal visaMasterAactualBankFees = 2.645M;
            decimal AMEXAactualBankFees = 2.185M;
            decimal tabbyBankFees = 3.45M;
            //decimal paymentAmountAfterBankFees = paymentAmount;
            decimal paymentAmountWithBCareDiscount = paymentAmount - bcareDiscountAmount;
            //if(insuranceCompanyId==27 && insuranceTypeCode==1)
            //{
            //    edaatAactualBankFees = 7.05M;
            //}
            if (checkoutDetail.PaymentMethodId == 4 || checkoutDetail.PaymentMethodId == 13)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * visaMasterAactualBankFees) / 100;
                actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

                actualBankFees = (paymentAmount * visaMasterAactualBankFees) / 100;
                actualBankFees = actualBankFees + 1.15M;
            }
            if (checkoutDetail.PaymentMethodId == 10)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * madaAactualBankFees) / 100;
                actualBankFees = (paymentAmount * madaAactualBankFees) / 100;
            }
            if (checkoutDetail.PaymentMethodId == 12)
            {
                actualBankFeesWithDiscount = edaatAactualBankFees;
                actualBankFees = edaatAactualBankFees;
            }
            if (checkoutDetail.PaymentMethodId == 13)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * AMEXAactualBankFees) / 100;
                actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

                //actualBankFees = (paymentAmount * AMEXAactualBankFees) / 100;
                //actualBankFees = actualBankFees + 1.15M;
            }
            if (checkoutDetail.PaymentMethodId == 16 && (insuranceTypeCode == 1 || insuranceTypeCode == 7 || insuranceTypeCode == 8))
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * tabbyBankFees) / 100;
                actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

                actualBankFees = (paymentAmount * tabbyBankFees) / 100;
                actualBankFees = actualBankFees + 1.15M;
            }

            if (actualBankFees > 0)
            {
                actualBankFees = Math.Round(actualBankFees, 2);
            }
            if (actualBankFeesWithDiscount > 0)
            {
                actualBankFeesWithDiscount = Math.Round(actualBankFeesWithDiscount, 2);
            }
            decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees - actualBankFees;
            if (totalBCareCommission > 0 && (actualBankFees > actualBankFeesWithDiscount)) // difference between Actual bank fees before and after discount should be deducted from Bcare
            {
                decimal diff = actualBankFees - actualBankFeesWithDiscount;
                totalBCareCommission = totalBCareCommission + diff;
            }
            if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9 || insuranceCompanyId == 27))
            {
                totalBCareCommission = totalBCareCommission - actualBankFees;
                totalCompanyAmount += actualBankFees;
            }
            if (totalBCareCommission > 0 && bcareDiscountAmount > 0) // deduct discount from bcare commission
            {
                totalBCareCommission = totalBCareCommission - bcareDiscountAmount;
            }
            if ((actualBankFees > actualBankFeesWithDiscount))
            {
                actualBankFees = actualBankFeesWithDiscount;
            }

            // As per Mohamed Badr to deduct the percentage () from all companies @ 1/11/2023 9:30AM
            //if (insuranceTypeCode == 1 && TabbyTPLCommissionAndFeesWithBcare.Contains(insuranceCompanyId)) // As per Khaled && Mubarak && Mohamed Badr @@mail (updated commissions and fees ( TPL ) - Urgent)
            if (checkoutDetail.PaymentMethodId == 16)
            {
                decimal _tabbyTPLCommissionAndFeesWithBcare = 0;
                if (insuranceCompanyId == 27)
                    _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 1.265M) / 100;
                else
                    _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 0.805M) / 100;

                totalBCareCommission -= _tabbyTPLCommissionAndFeesWithBcare;
            }

            var invoice = new Invoice();
            invoice.InvoiceNo = invoiceNo == 0 ? GetNewInvoiceNumber() : invoiceNo;
            invoice.ReferenceId = checkoutDetail.ReferenceId;
            invoice.InvoiceDate = DateTime.Now;
            invoice.InvoiceDueDate = DateTime.Now + TimeSpan.FromHours(16);
            invoice.UserId = checkoutDetail.UserId;
            invoice.InsuranceTypeCode = insuranceTypeCode;
            invoice.InsuranceCompanyId = insuranceCompanyId;
            invoice.ProductPrice = priceExcludingBenefitsAndVatAndFees;
            invoice.ExtraPremiumPrice = extraPremiumPrice;
            invoice.Discount = discount;
            invoice.DiscountPercentageValue = discountPercentageValue;
            invoice.Fees = fees;
            invoice.Vat = vatValue;
            invoice.SubTotalPrice = paymentAmount - vatValue;
            invoice.TotalPrice = paymentAmount;
            invoice.SpecialDiscount = specialDiscount;
            invoice.SpecialDiscountPercentageValue = specialDiscountPercentageValue;
            invoice.SpecialDiscount2 = specialDiscount;
            invoice.SpecialDiscount2PercentageValue = specialDiscountPercentageValue;
            invoice.LoyaltyDiscountValue = loyaltyDiscountValue;
            invoice.LoyaltyDiscountPercentage = loyaltyDiscountPercentageValue;
            invoice.TotalBenefitPrice = benefitsPrice * (decimal)1.15;

            invoice.TotalBCareCommission = Math.Round(totalBCareCommission, 2);
            invoice.TotalBCareFees = totalBCareFees;
            invoice.TotalCompanyAmount = Math.Round(totalCompanyAmount, 2);

            invoice.Invoice_Benefit = invoiceBenefits;
            invoice.ActualBankFees = Math.Round(actualBankFees, 2);
            invoice.FeesCalculationDetails = feesCalculationDetails;
            invoice.TotalBCareDiscount = Math.Round(bcareDiscountAmount, 2);
            invoice.TotalDiscount = Math.Round(totalDiscount, 2);
            invoice.BasicPrice = Math.Round(basicPrice, 2);
            invoice.PaidAmount = Math.Round(paidAmount, 2);
            if (!string.IsNullOrEmpty(odReferenceId))
            {
                invoice.ODReference = odReferenceId;
            }
            //get active template 
            invoice.ModifiedDate = DateTime.Now;
            var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
            if (templateInfo != null)
            {
                invoice.TemplateId = templateInfo.Id;
            }
            try
            {
                _invoiceRepository.Insert(invoice);
            }
            catch (DbUpdateException ex)
            {
                invoice.InvoiceNo = GetNewInvoiceNumber();
                _invoiceRepository.Insert(invoice);
            }
            return invoice;

        }

        #endregion

        //public Invoice CreateInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0, string odReferenceId = null)
        //{
        //    var checkoutDetail = _checkoutDetailRepository.TableNoTracking
        //        .Include(chk => chk.OrderItems)
        //        //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
        //        .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
        //        .FirstOrDefault(c => c.ReferenceId == referenceId);

        //    bool isUsedNoAService = false;
        //    var quotation = _quotationService.GetQuotationByReference(referenceId);
        //    if (quotation != null && quotation.QuotationRequest != null)
        //    {
        //        if (quotation.QuotationRequest.NoOfAccident.HasValue)
        //        {
        //            isUsedNoAService = true;
        //        }
        //    }

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

        //    bool IsCommissionAndActual = false;
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
        //            IsCommissionAndActual = true;
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
        //    actualBankFees = CalculateFees(checkoutDetail.PaymentMethodId.Value, false, paymentAmount, insuranceCompanyId, insuranceTypeCode);
        //    //if (actualBankFees > 0)
        //    //    actualBankFees = Math.Round(actualBankFees, 2);

        //    decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees - actualBankFees;
        //    if (bcareDiscountAmount > 0)
        //    {
        //        totalBCareCommission -= bcareDiscountAmount; // deduct discount from bcare commission

        //        actualBankFeesWithDiscount = CalculateFees(checkoutDetail.PaymentMethodId.Value, true, (paymentAmount - bcareDiscountAmount), insuranceCompanyId, insuranceTypeCode);
        //        //actualBankFeesWithDiscount = Math.Round(actualBankFeesWithDiscount, 2);
        //        if (actualBankFeesWithDiscount > 0) // difference between Actual bank fees before and after discount should be added from Bcare
        //        {
        //            decimal diff = actualBankFees - actualBankFeesWithDiscount;
        //            if (diff > 0)
        //                totalBCareCommission += diff;
        //        }
        //    }
        //    if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9 || insuranceCompanyId == 27))
        //    {
        //        totalBCareCommission -= actualBankFees;
        //        totalCompanyAmount += actualBankFees;
        //    }

        //    //// 
        //    /// As per Mohamed Badr to deduct the percentage () from all companies @ 1/11/2023 9:30AM
        //    /// if (insuranceTypeCode == 1 && TabbyTPLCommissionAndFeesWithBcare.Contains(insuranceCompanyId)) // As per Khaled && Mubarak && Mohamed Badr @@mail (updated commissions and fees ( TPL ) - Urgent)
        //    if (checkoutDetail.PaymentMethodId == 16)
        //    {
        //        decimal _tabbyTPLCommissionAndFeesWithBcare = 0;
        //        if (insuranceCompanyId == 27)
        //            _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 1.265M) / 100;
        //        else
        //            _tabbyTPLCommissionAndFeesWithBcare = (paymentAmount * 0.805M) / 100;

        //        totalBCareCommission -= _tabbyTPLCommissionAndFeesWithBcare;
        //    }
        //    ////
        //    /// this is related to New Logic --> that BCareCommission contain ActualBankFees, and we will exclude ActualBankFees from the equation that calculate CompanyAmount value
        //    if (IsCommissionAndActual)
        //    {
        //        totalBCareCommission -= actualBankFees;
        //        totalCompanyAmount += actualBankFees;
        //    }

        //    var invoice = new Invoice();
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

        //    invoice.Invoice_Benefit = invoiceBenefits;
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
        //    var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
        //    if (templateInfo != null)
        //    {
        //        invoice.TemplateId = templateInfo.Id;
        //    }
        //    try
        //    {
        //        _invoiceRepository.Insert(invoice);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        invoice.InvoiceNo = GetNewInvoiceNumber();
        //        _invoiceRepository.Insert(invoice);
        //    }
        //    return invoice;

        //}


        /// <summary>
        /// get all IBANs to specific user
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns></returns>
        public IEnumerable<string> GetUserIBANs(string userId)
        {
            return _checkoutDetailRepository.Table.Where(c => c.UserId == userId).Select(c => c.IBAN).Distinct();
        }


        public CheckoutDetail GetCheckoutDetailByReferenceId(string referenceId)
        {
            return _checkoutDetailRepository.Table
                .Include(chk => chk.ProductType)
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits))
                .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
                .Include(chk => chk.OrderItems.Select(oi => oi.Product.InsuranceCompany.Contact))
                .Include(chk => chk.BankCode)
                .FirstOrDefault(c => c.ReferenceId == referenceId);
        }

        public CheckoutDetail UpdateCheckout(CheckoutDetail checkoutDetail)
        {
            _checkoutDetailRepository.Update(checkoutDetail);
            return checkoutDetail;
        }


        /// <summary>
        /// Create new checkout details.
        /// </summary>
        /// <param name="checkoutDetail">The checkout details object.</param>
        /// <returns></returns>
        public CheckoutDetail CreateCheckoutDetails(CheckoutDetail checkoutDetail)
        {
            if (checkoutDetail == null)
            {
                throw new TameenkArgumentNullException(nameof(checkoutDetail));
            }
            _checkoutDetailRepository.Insert(checkoutDetail);
            return checkoutDetail;
        }

        /// <summary>
        /// Create order items from shopping cart items
        /// </summary>
        /// <param name="shoppingCartItems">The shopping cart items</param>
        /// <param name="referenceId">The checkout reference identifier.</param>
        /// <returns></returns>
        public List<OrderItem> CreateOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId)
        {
            var orderItems = new List<OrderItem>();
            foreach (var sci in shoppingCartItems)
            {
                var orderItem = new OrderItem
                {
                    CheckoutDetailReferenceId = referenceId,
                    CreatedOn = DateTime.Now,
                    ProductId = sci.ProductId,
                    Quantity = sci.Quantity,
                    Price = sci.ProductPrice,
                    UpdatedOn = DateTime.Now
                };
                foreach (var benefit in sci.ShoppingCartItemBenefits)
                {
                    var benefitPrice = benefit.BenefitPrice.GetValueOrDefault();
                    if (benefit.IsReadOnly&& benefitPrice==0)
                        benefitPrice = 0;
                    orderItem.OrderItemBenefits.Add(new OrderItemBenefit
                    {
                        BenefitId = benefit.BenefitId.GetValueOrDefault(),
                        BenefitExternalId = benefit.BenefitExternalId,
                        Price = benefitPrice
                    });
                }
                orderItems.Add(orderItem);
            }
            return orderItems;
        }

        /// <summary>
        /// Get user last used IBAN
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public string GetLatestUsedIBANByUser(string userId)
        {
            return _checkoutDetailRepository.TableNoTracking
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.CreatedDateTime.Value)
                    .Select(c => c.IBAN)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Get invoice if it exits
        /// </summary>
        /// <param name="refrenceId">reference identifier</param>
        /// <returns></returns>
        public Invoice GetInvoiceByRefrenceId(string refrenceId)
        {
            return _invoiceRepository.TableNoTracking.Where(i => i.ReferenceId == refrenceId && i.IsCancelled == false).FirstOrDefault();
        }

        public List<CommissionsAndFees> GetCommissionsAndFees(int insuranceTypeCode, int companyId)
        {
            try
            {
                return _commissionsAndFees.TableNoTracking.Where(a => a.CompanyId == companyId && a.InsuranceTypeCode == insuranceTypeCode).ToList();
            }
            catch
            {
                return null;
            }
        }

       
        #endregion

        #region Private methods

        /// <summary>
        /// Calculate the total amount of checkout details
        /// </summary>
        /// <param name="checkoutDetail">The checkout details object.</param>
        /// <returns></returns>
        public decimal CalculateCheckoutDetailTotal(CheckoutDetail checkoutDetail)
        {
            decimal totalPaymentAmount = 0;
            foreach (var orderItem in checkoutDetail.OrderItems)
            {
                totalPaymentAmount += orderItem.Price
                    + orderItem.OrderItemBenefits.Sum(oib => oib.Price * ((decimal)1.15));
            }
            return totalPaymentAmount;
        }

        private int GetNewInvoiceNumber()
        {
            Random rnd = new Random(System.Environment.TickCount);
            int invoiceNumber = rnd.Next(111111111, 999999999);

            if (_invoiceRepository.Table.Any(i => i.InvoiceNo == invoiceNumber))
                return GetNewInvoiceNumber();

            return invoiceNumber;
        }

        public CheckoutUsers CreateCheckoutUser(CheckoutUsers checkoutUser)
        {
            if (checkoutUser == null)
            {
                throw new TameenkArgumentNullException(nameof(checkoutUser));
            }
            _checkoutUsersRepository.Insert(checkoutUser);
            return checkoutUser;
        }

        public bool IsUserHaveVerifiedPhoneNumbers(Guid UserId, string phoneNumber)
        {
            return _checkoutUsersRepository.TableNoTracking.Where(u => u.UserId == UserId && u.PhoneNumber == phoneNumber && u.IsCodeVerified).Any();
        }

        public bool IsUserHaveVerifiedPhoneNumbersByNIN(string driverNIN, string phoneNumber,string referenceId)
        {
            return _checkoutUsersRepository.TableNoTracking.Where(u => u.Nin == driverNIN && u.PhoneNumber == phoneNumber && u.IsCodeVerified&&u.ReferenceId==referenceId).Any();
        }

        public CheckoutUsers GetByUserIdAndPhoneNumber(Guid UserId, string phoneNumber)
        {
            return _checkoutUsersRepository.Table.Where(u => u.UserId == UserId && u.PhoneNumber == phoneNumber).OrderByDescending(u => u.CreatedDate).FirstOrDefault();
        }

        public CheckoutUsers UpdateCheckoutUser(CheckoutUsers checkoutUser)
        {
            _checkoutUsersRepository.Update(checkoutUser);
            return checkoutUser;
        }

        public DateTime GetFirstPolicyIssueDate(string userEmail)
        {

            return _checkoutDetailRepository.Table.Where(c => c.Email == userEmail && (c.PolicyStatusId == 4 || c.PolicyStatusId == 5 || c.PolicyStatusId == 6 || c.PolicyStatusId == 7)).OrderBy(c => c.CreatedDateTime.Value)
                 .Include(chk => chk.Policies).FirstOrDefault().Policies.Select(p => p.PolicyIssueDate.Value).FirstOrDefault();
        }

        public int GetPurchasedCheckoutCount(string userEmail, DateTime durationDate)
        {
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.Email == userEmail && 
                            !c.IsCancelled && c.CreatedDateTime >= durationDate  && 
                            (c.PolicyStatusId == 4 || c.PolicyStatusId == 5 || c.PolicyStatusId == 6 ||
                            c.PolicyStatusId == 7 || c.PolicyStatusId == 2 )).Count();

        }
        public CheckoutDetail GetFromCheckoutDeatilsbyPhoneNo(string phone, DateTime durationDate)
        {
            return _checkoutDetailRepository.Table.Where(c => c.Phone == phone && (c.PolicyStatusId == 4 || c.PolicyStatusId == 5 || c.PolicyStatusId == 6 || c.PolicyStatusId == 7) && c.CreatedDateTime >= durationDate).OrderByDescending(c => c.CreatedDateTime).FirstOrDefault();

        }
        public List<CheckoutUsers> GetByCheckoutUsersByUserId(Guid userId, DateTime durationDate)
        {
            return _checkoutUsersRepository.Table.Where(u => u.UserId == userId && u.IsCodeVerified == true && u.CreatedDate >= durationDate).ToList();
        }
        public List<CheckoutDetail> GetFromCheckoutDeatilsbyPhoneNoList(Guid mainDriverId, string phone, DateTime durationDate)
        {
            DateTime dt = DateTime.Now;
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.MainDriverId != mainDriverId && c.Phone == phone && (c.PolicyStatusId == 4 || c.PolicyStatusId == 5 || c.PolicyStatusId == 6 || c.PolicyStatusId == 7) && c.CreatedDateTime >= dt && c.CreatedDateTime <= durationDate).OrderByDescending(c => c.CreatedDateTime).ToList();
        }

        public bool DeleteInvoiceByRefrenceId(string refrenceId, string userID,out string exception)
        {
            exception = string.Empty;
            try
            {
                var invoices = _invoiceRepository.Table.Where(i => i.ReferenceId == refrenceId).ToList();
                if (invoices != null&& invoices.Any())
                {
                    foreach (var invoice in invoices)
                    {
                        var invoicesBenefits = _invoiceBenefitRepository.Table.Where(i => i.InvoiceId == invoice.Id).ToList();
                        if (invoicesBenefits != null)
                        {
                            _invoiceBenefitRepository.Delete(invoicesBenefits);
                        }
                        _invoiceRepository.Delete(invoice);
                        return true;
                    }
                }
                //else
                //{
                //    var invoiceItems = _invoiceRepository.Table
                //   .Where(i => i.ReferenceId == refrenceId).ToList();
                //    if (invoiceItems != null && invoiceItems.Any())
                //    {
                //        _invoiceRepository.Delete(invoiceItems);
                //        return true;
                //    }
                //}
                return false;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public bool DeleteOrderItemByRefrenceId(string refrenceId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var orderItem = _orderItemRepository.Table.Where(i => i.CheckoutDetailReferenceId == refrenceId).FirstOrDefault();
                if (orderItem != null)
                {
                    var orderItemBenfits= _orderItemBenfitsRepository.Table.Where(o => o.OrderItemId == orderItem.Id).ToList();
                    if(orderItemBenfits!=null)
                    {
                        _orderItemBenfitsRepository.Delete(orderItemBenfits);
                    }
                    _orderItemRepository.Delete(orderItem);
                    return true;
                }
                //else
                //{
                //    var item = _orderItemRepository.Table.Where(i => i.CheckoutDetailReferenceId == refrenceId).FirstOrDefault();
                //    if (item != null)
                //    {
                //        _orderItemRepository.Delete(item);
                //        return true;
                //    }
                //}
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public bool SaveOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var orderItems = new List<OrderItem>();
                foreach (var sci in shoppingCartItems)
                {
                    var orderItem = new OrderItem
                    {
                        CheckoutDetailReferenceId = referenceId,
                        CreatedOn = DateTime.Now,
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        Price = sci.ProductPrice,
                        UpdatedOn = DateTime.Now
                    };
                    foreach (var benefit in sci.ShoppingCartItemBenefits)
                    {
                        var benefitPrice = benefit.BenefitPrice.GetValueOrDefault();
                        if (benefit.IsReadOnly && benefitPrice == 0)
                            benefitPrice = 0;
                        orderItem.OrderItemBenefits.Add(new OrderItemBenefit
                        {
                            BenefitId = benefit.BenefitId.GetValueOrDefault(),
                            BenefitExternalId = benefit.BenefitExternalId,
                            Price = benefitPrice
                        });
                    }
                    orderItems.Add(orderItem);
                }
                if (orderItems.Count > 0)
                    _orderItemRepository.Insert(orderItems);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public CheckoutDetail GetFromCheckoutDeatilsbyReferenceId(string referenceId)
        {
            return _checkoutDetailRepository.Table.Where(c => c.ReferenceId == referenceId).FirstOrDefault();
        }
        public int GetCountFromCheckoutDeatilsbyPhoneNoList(Guid mainDriverId, string phone, DateTime durationDate)
        {
            DateTime dt = DateTime.Now;
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.MainDriverId != mainDriverId && c.Phone == phone && (c.PolicyStatusId == 4 || c.PolicyStatusId == 5 || c.PolicyStatusId == 6 || c.PolicyStatusId == 7) && c.CreatedDateTime >= dt && c.CreatedDateTime <= durationDate).OrderByDescending(c => c.CreatedDateTime).Count();
        }

        public CheckoutDetail GetFromCheckoutDetailByPhoneNumber(string phoneNumber,Guid driverId)
        {
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.Phone == phoneNumber
           &&c.MainDriverId!= driverId && c.PolicyStatusId!=1&& c.PolicyStatusId != 3).FirstOrDefault();
        }

        public CheckoutDetail GetFromCheckoutDetailByEmail(string email, Guid driverId)
        {
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.Email == email
          && c.MainDriverId != driverId && c.PolicyStatusId != 1 && c.PolicyStatusId != 3).FirstOrDefault();
        }
        public CheckoutDetail GetFromCheckoutDetailByIBAN(string iban, Guid driverId)
        {
            iban = iban.ToLower().Trim();
            return _checkoutDetailRepository.TableNoTracking.Where(c => c.IBAN.ToLower().Trim() == iban
          && c.MainDriverId != driverId && c.PolicyStatusId != 1 && c.PolicyStatusId != 3).FirstOrDefault();
        }

        public CheckoutDetailInfo GetCheckoutDetail(string phoneNumber, string email, string iban, Guid driverId,string driverNin)
        {
            iban = iban.ToLower().Trim();
            email = email.ToLower();
            CheckoutDetailInfo info = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            dbContext.DatabaseInstance.CommandTimeout = 60;            var command = dbContext.DatabaseInstance.Connection.CreateCommand();            command.CommandText = "GetCheckoutDetail";            command.CommandType = CommandType.StoredProcedure;            SqlParameter phoneNumberParam = new SqlParameter() { ParameterName = "phoneNumber", Value = phoneNumber };            SqlParameter emailParam = new SqlParameter() { ParameterName = "email", Value = email };            SqlParameter ibanParam = new SqlParameter() { ParameterName = "iban", Value = iban };            SqlParameter driverIdParam = new SqlParameter() { ParameterName = "driverId", Value = driverId };
            SqlParameter driverNinParam = new SqlParameter() { ParameterName = "driverNin", Value = driverNin };
            command.Parameters.Add(phoneNumberParam);            command.Parameters.Add(emailParam);            command.Parameters.Add(ibanParam);            command.Parameters.Add(driverIdParam);
            command.Parameters.Add(driverNinParam);

            dbContext.DatabaseInstance.Connection.Open();            var reader = command.ExecuteReader();            info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutDetailInfo>(reader).FirstOrDefault();
            dbContext.DatabaseInstance.Connection.Close();
            if (info != null)
            {
                return info;
            }
            return info;
        }

        public bool ManagePhoneVerificationForCheckoutUsers(Guid userId, string phoneNumber, string code)        {            var checkoutUser = GetByUserIdAndPhoneNumberWithCodeNotVerified(userId, phoneNumber);            if (checkoutUser != null)            {                if (checkoutUser.VerificationCode == int.Parse(code))                {                    checkoutUser.IsCodeVerified = true;                    UpdateCheckoutUser(checkoutUser);                    return true;                }            }            return false;        }        public CheckoutUsers GetByUserIdAndPhoneNumberWithCodeNotVerified(Guid UserId, string phoneNumber)        {            return _checkoutUsersRepository.Table.Where(u => u.UserId == UserId && u.PhoneNumber == phoneNumber && u.IsCodeVerified == false).OrderByDescending(u => u.CreatedDate).FirstOrDefault();        }

        public bool ConfirmCheckoutDetailEmail(string referenceId, string userId, string email,out string exception)
        {
            try
            {
                exception = string.Empty;
                var checkout = _checkoutDetailRepository.Table.Where(c => c.ReferenceId == referenceId
             && c.Email == email).FirstOrDefault();

                if (checkout == null)
                {
                    exception = "NoRecord";
                    return false;
                }
                checkout.IsEmailVerified = true;
                checkout.ModifiedDate = DateTime.Now;
                _checkoutDetailRepository.Update(checkout);
                return true;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public List<CommissionsAndFees> GetFeesOfPaymentMethod(int insuranceTypeCode, int companyId,int paymentMethodId)
        {
            try
            {
                return _commissionsAndFees.TableNoTracking.Where(a => a.CompanyId == companyId && a.InsuranceTypeCode == insuranceTypeCode&&a.PaymentMethodId==paymentMethodId).ToList();
            }
            catch
            {
                return null;
            }
        }
        public Invoice UpdateInvoiceCommissionsAndFees(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int paymentMethodId)
        {
            var checkoutDetail = _checkoutDetailRepository.TableNoTracking
                .Include(chk => chk.OrderItems)
                //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
                .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
                .FirstOrDefault(c => c.ReferenceId == referenceId);

            bool isUsedNoAService = false;
            var quotation = _quotationService.GetQuotationByReference(referenceId);
            if (quotation != null && quotation.QuotationRequest != null)
            {
                if (quotation.QuotationRequest.NoOfAccident.HasValue)
                {
                    isUsedNoAService = true;
                }
            }

            int numberOfDriver = 1;
            if (checkoutDetail.AdditionalDriverIdOne.HasValue)
            {
                numberOfDriver = 2;//main driver+ 1 additional
            }
            if (checkoutDetail.AdditionalDriverIdTwo.HasValue)
            {
                numberOfDriver = 3;//main driver+ 2 additional
            }
            if (checkoutDetail.AdditionalDriverIdThree.HasValue)
            {
                numberOfDriver = 4;//main driver+ 3 additional
            }
            if (checkoutDetail.AdditionalDriverIdFour.HasValue)
            {
                numberOfDriver = 5;//main driver+ 4 additional
            }

            var selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
            decimal vatPrice = 0;
            decimal fees = 0;
            decimal extraPremiumPrice = 0;
            decimal discount = 0;
            decimal specialDiscount = 0;
            decimal specialDiscountPercentageValue = 0;
            decimal discountPercentageValue = 0;

            decimal loyaltyDiscountPercentageValue = 0;
            decimal loyaltyDiscountValue = 0;
            decimal specialDiscount2 = 0;
            decimal specialDiscount2PercentageValue = 0;

            decimal totalDiscount = 0;

            foreach (var orderItem in checkoutDetail.OrderItems)
            {
                foreach (var price in orderItem.Product.PriceDetails)
                {
                    if (price.PriceTypeCode == 1)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount = price.PriceValue;
                        specialDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 2)
                    {
                        totalDiscount += price.PriceValue;
                        discountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 3)
                    {
                        totalDiscount += price.PriceValue;
                        loyaltyDiscountValue = price.PriceValue;
                        loyaltyDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 10)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 11)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 12)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount2 = price.PriceValue;
                        specialDiscount2PercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    switch (price.PriceTypeCode)
                    {
                        case 8: vatPrice += price.PriceValue; break;
                        case 6: fees += price.PriceValue; break;
                        case 7: extraPremiumPrice += price.PriceValue; break;
                        case 2: discount += price.PriceValue; break;
                    }
                }
            }
            decimal benefitsPrice = selectedBenefits
                .Select(x => x.Price)
                .Sum();

            decimal vatValue = vatPrice + selectedBenefits
                .Select(x => x.Price * (decimal)0.15)
                .Sum();

            var paymentAmount = CalculateCheckoutDetailTotal(checkoutDetail);
            var priceExcludingBenefitsAndVatAndFees = paymentAmount - benefitsPrice - vatValue - fees;
            //var basicPremium = extraPremiumPrice - totalDiscount;
            var priceExcludingVat = paymentAmount - vatValue;
            var basicPrice = paymentAmount - vatValue - fees;
            var invoiceBenefits = (from i in selectedBenefits
                                   select new Invoice_Benefit()
                                   {
                                       BenefitId = (short)i.BenefitId,
                                       BenefitPrice = i.Price
                                   }).ToArray();

            var CommissionsAndFees = GetCommissionsAndFees(insuranceTypeCode, insuranceCompanyId);
            decimal totalBCareCommission = 0;
            decimal totalBCareFees = 0;
            string feesCalculationDetails = string.Empty;
            decimal bcareDiscountAmount = 0;
            if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 1 && checkoutDetail.DiscountValue.HasValue)
            {
                bcareDiscountAmount = checkoutDetail.DiscountValue.Value;
            }
            if (checkoutDetail.DiscountType.HasValue && checkoutDetail.DiscountType == 2 && checkoutDetail.DiscountPercentage.HasValue)
            {
                bcareDiscountAmount = (paymentAmount * checkoutDetail.DiscountPercentage.Value) / 100;
            }
            decimal paidAmount = paymentAmount - bcareDiscountAmount;
            foreach (var item in CommissionsAndFees)
            {
                if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != paymentMethodId)
                    continue;
                if (item.Key == "NOA" && !isUsedNoAService)
                    continue;
                if (item.Key == "AMEX")
                    continue;
                if (item.IsCommission && item.Percentage.HasValue)
                {
                    totalBCareCommission += ((basicPrice) * item.Percentage.Value) / 100;
                }
                else
                {
                    decimal percentageValue = 0;
                    decimal fixedFeesValue = 0;
                    if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value && numberOfDriver > 1)
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
                        fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "+(" + item.Percentage + "%*" + paymentAmount + ")*" + numberOfDriver + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "=";
                        }
                    }
                    else
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100));
                        fixedFeesValue = (item.FixedFees.Value);

                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "+" + item.Percentage + "%*" + paymentAmount + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "=";
                        }
                    }
                    //check negative values and deduct fees from bCare Commissions
                    if (item.IsPercentageNegative && item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - (percentageValue + fixedFeesValue);
                        totalBCareCommission = totalBCareCommission - (percentageValue + fixedFeesValue);
                    }
                    else if (item.IsPercentageNegative)
                    {
                        totalBCareFees = totalBCareFees - percentageValue;
                        totalBCareCommission = totalBCareCommission - percentageValue;
                    }
                    else if (item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - fixedFeesValue;
                        totalBCareCommission = totalBCareCommission - fixedFeesValue;
                    }
                    feesCalculationDetails += "Total:" + Math.Round(totalBCareFees, 3) + "#";
                }
            }
            if (totalBCareCommission > 0)
                totalBCareCommission = Math.Round(totalBCareCommission, 2);
            if (totalBCareFees > 0)
                totalBCareFees = Math.Round(totalBCareFees, 2);

            decimal actualBankFees = 0;
            decimal actualBankFeesWithDiscount = 0;
            decimal edaatAactualBankFees = 8.74M;
            decimal madaAactualBankFees = 1.75M;
            decimal visaMasterAactualBankFees = 2.3M;
            decimal AMEXAactualBankFees = 1.9M;
            //decimal paymentAmountAfterBankFees = paymentAmount;
            decimal paymentAmountWithBCareDiscount = paymentAmount - bcareDiscountAmount;
            if (insuranceCompanyId == 27 && insuranceTypeCode == 1)
            {
                edaatAactualBankFees = 7.05M;
            }
            if (paymentMethodId == 4 || paymentMethodId == 13)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * visaMasterAactualBankFees) / 100;
                actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1;

                actualBankFees = (paymentAmount * visaMasterAactualBankFees) / 100;
                actualBankFees = actualBankFees + 1;
            }
            if (paymentMethodId == 10)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * madaAactualBankFees) / 100;
                actualBankFees = (paymentAmount * madaAactualBankFees) / 100;
            }
            if (paymentMethodId == 12)
            {
                actualBankFeesWithDiscount = edaatAactualBankFees;
                actualBankFees = edaatAactualBankFees;
            }
            if (paymentMethodId == 13)
            {
                actualBankFeesWithDiscount = (paymentAmountWithBCareDiscount * AMEXAactualBankFees) / 100;
                actualBankFeesWithDiscount = actualBankFeesWithDiscount + 1.15M;

                //actualBankFees = (paymentAmount * AMEXAactualBankFees) / 100;
                //actualBankFees = actualBankFees + 1.15M;
            }
            if (actualBankFees > 0)
            {
                actualBankFees = Math.Round(actualBankFees, 2);
            }
            if (actualBankFeesWithDiscount > 0)
            {
                actualBankFeesWithDiscount = Math.Round(actualBankFeesWithDiscount, 2);
            }
            decimal totalCompanyAmount = paymentAmount - totalBCareCommission - totalBCareFees - actualBankFees;
            if (totalBCareCommission > 0 && (actualBankFees > actualBankFeesWithDiscount)) // difference between Actual bank fees before and after discount should be deducted from Bcare
            {
                decimal diff = actualBankFees - actualBankFeesWithDiscount;
                totalBCareCommission = totalBCareCommission + diff;
            }
            if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9 || insuranceCompanyId == 27))
            {
                totalBCareCommission = totalBCareCommission - actualBankFees;
                totalCompanyAmount += actualBankFees;
            }
            if (totalBCareCommission > 0 && bcareDiscountAmount > 0) // deduct discount from bcare commission
            {
                totalBCareCommission = totalBCareCommission - bcareDiscountAmount;
            }
            if ((actualBankFees > actualBankFeesWithDiscount))
            {
                actualBankFees = actualBankFeesWithDiscount;
            }
            var invoice = _invoiceRepository.Table.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            if (invoice != null)
            {
                invoice.TotalBCareFees = totalBCareFees;
                invoice.TotalBCareCommission = totalBCareCommission;
                invoice.TotalCompanyAmount = Math.Round(totalCompanyAmount,2);
                invoice.ActualBankFees = actualBankFees;
                invoice.FeesCalculationDetails = feesCalculationDetails;
                invoice.TotalDiscount = Math.Round(totalDiscount, 2);
                invoice.TotalBCareDiscount = bcareDiscountAmount;
                invoice.PaidAmount = paidAmount;
                invoice.BasicPrice = basicPrice;
                invoice.ModifiedDate = DateTime.Now;
                _invoiceRepository.Update(invoice);
                return invoice;
            }
            return null;
        }

        public Invoice UpdateInvoiceFeesCalculationDetails(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int paymentMethodId)
        {
            var checkoutDetail = _checkoutDetailRepository.TableNoTracking
                .Include(chk => chk.OrderItems)
                //.Include(chk => chk.OrderItems.Select(oi => oi.Product.QuotationResponse))
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
                .Include(chk => chk.OrderItems.Select(oi => oi.Product.PriceDetails.Select(pd => pd.PriceType)))
                .FirstOrDefault(c => c.ReferenceId == referenceId);

            bool isUsedNoAService = false;
            var quotation = _quotationService.GetQuotationByReference(referenceId);
            if (quotation != null && quotation.QuotationRequest != null)
            {
                if (quotation.QuotationRequest.NoOfAccident.HasValue)
                {
                    isUsedNoAService = true;
                }
            }

            int numberOfDriver = 1;
            if (checkoutDetail.AdditionalDriverIdOne.HasValue)
            {
                numberOfDriver = 2;//main driver+ 1 additional
            }
            if (checkoutDetail.AdditionalDriverIdTwo.HasValue)
            {
                numberOfDriver = 3;//main driver+ 2 additional
            }
            if (checkoutDetail.AdditionalDriverIdThree.HasValue)
            {
                numberOfDriver = 4;//main driver+ 3 additional
            }
            if (checkoutDetail.AdditionalDriverIdFour.HasValue)
            {
                numberOfDriver = 5;//main driver+ 4 additional
            }

            var selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
            decimal vatPrice = 0;
            decimal fees = 0;
            decimal extraPremiumPrice = 0;
            decimal discount = 0;
            decimal specialDiscount = 0;
            decimal specialDiscountPercentageValue = 0;
            decimal discountPercentageValue = 0;

            decimal loyaltyDiscountPercentageValue = 0;
            decimal loyaltyDiscountValue = 0;
            decimal specialDiscount2 = 0;
            decimal specialDiscount2PercentageValue = 0;

            decimal totalDiscount = 0;

            foreach (var orderItem in checkoutDetail.OrderItems)
            {
                foreach (var price in orderItem.Product.PriceDetails)
                {
                    if (price.PriceTypeCode == 1)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount = price.PriceValue;
                        specialDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 2)
                    {
                        totalDiscount += price.PriceValue;
                        discountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 3)
                    {
                        totalDiscount += price.PriceValue;
                        loyaltyDiscountValue = price.PriceValue;
                        loyaltyDiscountPercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    if (price.PriceTypeCode == 10)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 11)
                    {
                        totalDiscount += price.PriceValue;
                    }
                    if (price.PriceTypeCode == 12)
                    {
                        totalDiscount += price.PriceValue;
                        specialDiscount2 = price.PriceValue;
                        specialDiscount2PercentageValue = price.PercentageValue.HasValue ? price.PercentageValue.Value : 0;
                    }
                    switch (price.PriceTypeCode)
                    {
                        case 8: vatPrice += price.PriceValue; break;
                        case 6: fees += price.PriceValue; break;
                        case 7: extraPremiumPrice += price.PriceValue; break;
                        case 2: discount += price.PriceValue; break;
                    }
                }
            }
            decimal benefitsPrice = selectedBenefits
                .Select(x => x.Price)
                .Sum();

            decimal vatValue = vatPrice + selectedBenefits
                .Select(x => x.Price * (decimal)0.15)
                .Sum();

            var paymentAmount = CalculateCheckoutDetailTotal(checkoutDetail);
            var priceExcludingBenefitsAndVatAndFees = paymentAmount - benefitsPrice - vatValue - fees;
            var invoiceBenefits = (from i in selectedBenefits
                                   select new Invoice_Benefit()
                                   {
                                       BenefitId = (short)i.BenefitId,
                                       BenefitPrice = i.Price
                                   }).ToArray();

            var CommissionsAndFees = GetCommissionsAndFees(insuranceTypeCode, insuranceCompanyId);
            decimal totalBCareCommission = 0;
            decimal totalBCareFees = 0;

            decimal actualBankFees = 0;
            decimal madaAactualBankFees = 1.75M;
            decimal visaMasterAactualBankFees = 2.3M;
            decimal AMEXAactualBankFees = 1.9M;

            decimal paymentAmountAfterBankFees = paymentAmount;
            if (paymentMethodId == 4)
            {
                actualBankFees = (paymentAmount * visaMasterAactualBankFees) / 100;
                actualBankFees = actualBankFees + 1;
            }
            if (paymentMethodId == 10)
            {
                actualBankFees = (paymentAmount * madaAactualBankFees) / 100;
            }
            if (paymentMethodId == 13)
            {
                actualBankFees = (paymentAmount * AMEXAactualBankFees) / 100;
                //actualBankFees = actualBankFees + 1.15M;
            }
            if (actualBankFees > 0)
            {
                actualBankFees = Math.Round(actualBankFees, 2);
                paymentAmountAfterBankFees = paymentAmount - actualBankFees;
            }
            string feesCalculationDetails = string.Empty;
            foreach (var item in CommissionsAndFees)
            {
                if (item.PaymentMethodId.HasValue && item.PaymentMethodId.Value != paymentMethodId)
                    continue;
                if (item.Key == "NOA" && !isUsedNoAService)
                    continue;
                if (item.IsCommission && item.Percentage.HasValue && (item.CompanyKey == "Allianz" || item.CompanyKey == "Sagr"))
                {
                    totalBCareCommission += ((priceExcludingBenefitsAndVatAndFees + fees + benefitsPrice) * item.Percentage.Value) / 100;
                }
                else if (item.IsCommission && item.Percentage.HasValue)
                {
                    totalBCareCommission += ((priceExcludingBenefitsAndVatAndFees + benefitsPrice) * item.Percentage.Value) / 100;
                }
                else
                {
                    decimal percentageValue = 0;
                    decimal fixedFeesValue = 0;
                    if (item.IncludeAdditionalDriver.HasValue && item.IncludeAdditionalDriver.Value && numberOfDriver > 1)
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100)) * numberOfDriver;
                        fixedFeesValue = (item.FixedFees.Value) * numberOfDriver;

                        //totalBCareFees += (((paymentAmount * item.Percentage.Value) / 100) + item.FixedFees.Value) * numberOfDriver;
                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "+(" + item.Percentage + "%*" + paymentAmount + ")*" + numberOfDriver + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "*" + numberOfDriver + "=";
                        }
                    }
                    else
                    {
                        percentageValue = (((paymentAmount * item.Percentage.Value) / 100));
                        fixedFeesValue = (item.FixedFees.Value);
                        //totalBCareFees += ((paymentAmount * item.Percentage.Value) / 100) + item.FixedFees.Value;
                        totalBCareFees += percentageValue + fixedFeesValue;
                        if (item.Percentage.HasValue && item.Percentage > 0)
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "+" + item.Percentage + "%*" + paymentAmount + "=";
                        }
                        else
                        {
                            feesCalculationDetails += item.Key + ":Value=" + item.FixedFees + "=";
                        }
                    }
                    //check negative values and deduct fees from bCare Commissions
                    if (item.IsPercentageNegative && item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - (percentageValue + fixedFeesValue);
                        totalBCareCommission = totalBCareCommission - (percentageValue + fixedFeesValue);
                    }
                    else if (item.IsPercentageNegative)
                    {
                        totalBCareFees = totalBCareFees - percentageValue;
                        totalBCareCommission = totalBCareCommission - percentageValue;
                    }
                    else if (item.IsFixedFeesNegative)
                    {
                        totalBCareFees = totalBCareFees - fixedFeesValue;
                        totalBCareCommission = totalBCareCommission - fixedFeesValue;
                    }
                    feesCalculationDetails += "Total:" + Math.Round(totalBCareFees, 3) + "#";
                }
            }
            if (totalBCareCommission > 0)
                totalBCareCommission = Math.Round(totalBCareCommission, 2);
            if (totalBCareFees > 0)
                totalBCareFees = Math.Round(totalBCareFees, 2);

            decimal totalCompanyAmount = paymentAmountAfterBankFees - totalBCareFees - totalBCareCommission;
            var invoice = _invoiceRepository.Table.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            if (invoice != null)
            {
                invoice.FeesCalculationDetails = feesCalculationDetails;
                _invoiceRepository.Update(invoice);
                return invoice;
            }
            return null;
        }
        public CheckoutDetailInfo CheckIfPhoneAlreadyUsed(string phoneNumber, string driverNin)
        {
            CheckoutDetailInfo info = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "CheckIfPhoneAlreadyUsed";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter phoneNumberParam = new SqlParameter() { ParameterName = "phoneNumber", Value = phoneNumber };
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "driverNin", Value = driverNin };

                command.Parameters.Add(phoneNumberParam);
                command.Parameters.Add(driverNinParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutDetailInfo>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (info != null)
                {
                    //info.IsYakeenVerified = _checkoutUsersRepository.TableNoTracking.Where(u => u.Nin == driverNin && u.PhoneNumber == phoneNumber && u.IsMobileVerifiedbyYakeen && u.CreatedDate.Date > DateTime.Now.AddYears(-1).Date).Any();
                    return info;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return info;
        }
        public CheckoutDetailInfo CheckIfEmailAlreadyUsed(string email, string driverNin)
        {
            CheckoutDetailInfo info = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "CheckIfEmailAlreadyUsed";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter emailParam = new SqlParameter() { ParameterName = "email", Value = email };
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "driverNin", Value = driverNin };

                command.Parameters.Add(emailParam);
                command.Parameters.Add(driverNinParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutDetailInfo>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (info != null)
                {
                    return info;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return info;
        }
        public CheckoutDetailInfo CheckIfIbanAlreadyUsed(string iban, string driverNin)
        {
            CheckoutDetailInfo info = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "CheckIfIbanAlreadyUsed";
                command.CommandType = CommandType.StoredProcedure;
                //dbContext.DatabaseInstance.CommandTimeout = 60;
                command.CommandTimeout = 60;
                SqlParameter ibanParam = new SqlParameter() { ParameterName = "iban", Value = iban };
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "driverNin", Value = driverNin };

                command.Parameters.Add(ibanParam);
                command.Parameters.Add(driverNinParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                info = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutDetailInfo>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                if (info != null)
                {
                    return info;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return info;
        }

        #endregion

        public bool UpdateDiscountCodeToBeConsumed(Guid? vehicleId, string discountCode,string referenceId)
        {
            try
            {
                var vehicleDiscountsInfo = _vehicleDiscountsRepository.Table.Where(a => a.VehicleId == vehicleId && a.DiscountCode == discountCode && a.IsUsed == false).FirstOrDefault();
                if (vehicleDiscountsInfo != null)
                {
                    vehicleDiscountsInfo.IsUsed = true;
                    vehicleDiscountsInfo.ReferenceId = referenceId;
                    vehicleDiscountsInfo.ModifiedDate = DateTime.Now;
                    _vehicleDiscountsRepository.Update(vehicleDiscountsInfo);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool IsUserHaveVerifiedPhoneNumbersByNINAndSequenceNumber(string driverNIN, string phoneNumber,string sequenceNumber)
        {
            return _checkoutUsersRepository.TableNoTracking.Where(u => u.Nin == driverNIN && u.PhoneNumber == phoneNumber && u.IsCodeVerified && u.SequenceNumber == sequenceNumber).Any();
        }

        public bool IsUserHaveVerifiedPhoneByYakeen(string driverNIN, string phoneNumber)
        {
            var becnhMark = DateTime.Now.AddYears(-1);
            return _checkoutUsersRepository.TableNoTracking.Where(u => u.Nin == driverNIN && u.PhoneNumber == phoneNumber && u.IsMobileVerifiedbyYakeen && u.CreatedDate > becnhMark).Any();
        }

        #region Leasing

        public Invoice CreateLeasingInvoice(string referenceId, short insuranceTypeCode, int insuranceCompanyId, int invoiceNo = 0)
        {
            var checkoutDetail = _checkoutDetailRepository.TableNoTracking
                .Include(chk => chk.OrderItems)
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemBenefits.Select(pb => pb.Benefit)))
                .Include(chk => chk.OrderItems.Select(oi => oi.OrderItemDrivers))
                .FirstOrDefault(c => c.ReferenceId == referenceId);

            decimal vatPrice = 0;
            decimal fees = 0;
            decimal extraPremiumPrice = 0;
            decimal discount = 0;
            decimal specialDiscount = 0;
            decimal specialDiscountPercentageValue = 0;
            decimal discountPercentageValue = 0;
            decimal loyaltyDiscountPercentageValue = 0;
            decimal loyaltyDiscountValue = 0;
            decimal totalBCareFees = 0;
            decimal bcareDiscountAmount = 0;
            decimal actualBankFees = 0;
            string feesCalculationDetails = string.Empty;

            IEnumerable<OrderItemBenefit> selectedBenefits = null;
            List<Invoice_Benefit> invoiceBenefits = null;
            PolicyModification purchasedDriver = null;
            decimal paymentAmount = 0;
            decimal benefitsOrDriverPrice = 0;
            decimal benefitsOrDriverPriceVat = 0;
            if (checkoutDetail.ProviderServiceId == Tameenk.Core.Domain.Enums.CheckoutProviderServicesCodes.PurchaseBenefit)
            {
                selectedBenefits = checkoutDetail.OrderItems.SelectMany(oi => oi.OrderItemBenefits);
                benefitsOrDriverPrice = selectedBenefits.Select(x => x.Price).Sum();
                benefitsOrDriverPriceVat = benefitsOrDriverPrice * (decimal)0.15;
                paymentAmount = CalculateLeasingCheckoutDetailTotal(checkoutDetail);

                invoiceBenefits = (from i in selectedBenefits
                                   select new Invoice_Benefit()
                                   {
                                       BenefitId = (short)i.BenefitId,
                                       BenefitPrice = i.Price
                                   }).ToList();
            }

            else if (checkoutDetail.ProviderServiceId == Tameenk.Core.Domain.Enums.CheckoutProviderServicesCodes.PurchaseDriver)
            {
                purchasedDriver = _policyModificationRepository.TableNoTracking.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
                //benefitsOrDriverPrice = purchasedDriver.TotalAmount ?? 0;
                benefitsOrDriverPrice = purchasedDriver.TaxableAmount ?? 0;
                benefitsOrDriverPriceVat = benefitsOrDriverPrice * (decimal)0.15;
                paymentAmount = CalculateLeasingCheckoutDetailTotal(checkoutDetail);
            }

            decimal vatValue = vatPrice + benefitsOrDriverPriceVat;
            var priceExcludingBenefitsAndVatAndFees = (paymentAmount - benefitsOrDriverPriceVat) - benefitsOrDriverPrice - vatValue - fees; // here we deduct (benefitsPriceVat) as (benefit or drivver) vat calculated from company side not from our side
            var priceExcludingVat = paymentAmount - vatValue;
            var priceExcludingVatAndFees = paymentAmount - vatValue - fees;
            decimal totalBCareCommission = (paymentAmount - benefitsOrDriverPriceVat) * (decimal)0.15; // here we deduct (benefitsPriceVat) as (benefit or drivver) vat calculated from company side not from our side
            decimal paymentAmountAfterBcareDiscount = paymentAmount - bcareDiscountAmount;
            decimal paymentAmountAfterBankFees = paymentAmount;

            decimal madaAactualBankFees = 1.75M;
            decimal visaMasterAactualBankFees = 2.3M;
            decimal AMEXAactualBankFees = 1.9M;
            if (checkoutDetail.PaymentMethodId == 4)
                actualBankFees = ((paymentAmountAfterBcareDiscount * visaMasterAactualBankFees) / 100) + 1;
            if (checkoutDetail.PaymentMethodId == 10)
                actualBankFees = (paymentAmountAfterBcareDiscount * madaAactualBankFees) / 100;
            if (checkoutDetail.PaymentMethodId == 13)
                actualBankFees = (paymentAmountAfterBcareDiscount * AMEXAactualBankFees) / 100;

            if (actualBankFees > 0)
            {
                actualBankFees = Math.Round(actualBankFees, 2);
                if (insuranceTypeCode == 2 && (insuranceCompanyId == 2 || insuranceCompanyId == 9))
                {
                    paymentAmountAfterBankFees = paymentAmount;
                    totalBCareCommission = totalBCareCommission - actualBankFees;
                }
                else
                {
                    paymentAmountAfterBankFees = paymentAmount - actualBankFees;
                }
            }

            decimal totalCompanyAmount = paymentAmountAfterBankFees - totalBCareFees - totalBCareCommission;

            var invoice = new Invoice();
            invoice.InvoiceNo = invoiceNo == 0 ? GetNewInvoiceNumber() : invoiceNo;
            invoice.ReferenceId = checkoutDetail.ReferenceId;
            invoice.MainPolicyReferance = checkoutDetail.MainPolicyReferenceId;
            invoice.UserId = checkoutDetail.UserId;
            invoice.InsuranceTypeCode = insuranceTypeCode;
            invoice.InsuranceCompanyId = insuranceCompanyId;
            invoice.ProductPrice = priceExcludingBenefitsAndVatAndFees;
            invoice.ExtraPremiumPrice = extraPremiumPrice;
            invoice.Discount = discount;
            invoice.DiscountPercentageValue = discountPercentageValue;
            invoice.Fees = fees;
            invoice.Vat = vatValue;
            invoice.SubTotalPrice = paymentAmount - vatValue;
            invoice.TotalPrice = paymentAmount;
            invoice.SpecialDiscount = specialDiscount;
            invoice.SpecialDiscountPercentageValue = specialDiscountPercentageValue;
            invoice.SpecialDiscount2 = specialDiscount;
            invoice.SpecialDiscount2PercentageValue = specialDiscountPercentageValue;
            invoice.LoyaltyDiscountValue = loyaltyDiscountValue;
            invoice.LoyaltyDiscountPercentage = loyaltyDiscountPercentageValue;
            invoice.TotalBenefitPrice = benefitsOrDriverPrice;
            invoice.TotalBCareCommission = Math.Round(totalBCareCommission, 2);
            invoice.TotalBCareFees = totalBCareFees;
            invoice.TotalCompanyAmount = Math.Round(totalCompanyAmount, 2);
            invoice.Invoice_Benefit = invoiceBenefits;
            invoice.ActualBankFees = Math.Round(actualBankFees, 2);
            invoice.FeesCalculationDetails = feesCalculationDetails;
            invoice.TotalBCareDiscount = Math.Round(bcareDiscountAmount, 2);
            invoice.InvoiceDate = DateTime.Now;
            invoice.InvoiceDueDate = DateTime.Now + TimeSpan.FromHours(16);
            invoice.ModifiedDate = DateTime.Now;

            //get active template 
            var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Active == true).FirstOrDefault();
            if (templateInfo != null)
                invoice.TemplateId = templateInfo.Id;

            try
            {
                _invoiceRepository.Insert(invoice);
            }
            catch (DbUpdateException ex)
            {
                invoice.InvoiceNo = GetNewInvoiceNumber();
                _invoiceRepository.Insert(invoice);
            }
            return invoice;
        }

        public decimal CalculateLeasingCheckoutDetailTotal(CheckoutDetail checkoutDetail)
        {
            decimal totalPaymentAmount = 0;
            foreach (var orderItem in checkoutDetail.OrderItems)
            {
                if (checkoutDetail.ProviderServiceId == Tameenk.Core.Domain.Enums.CheckoutProviderServicesCodes.PurchaseBenefit)
                    totalPaymentAmount += orderItem.OrderItemBenefits.Sum(oib => oib.Price);

                else if (checkoutDetail.ProviderServiceId == Tameenk.Core.Domain.Enums.CheckoutProviderServicesCodes.PurchaseDriver)
                {
                    var purchasedDriver = _policyModificationRepository.TableNoTracking.Where(a => a.ReferenceId == checkoutDetail.ReferenceId).FirstOrDefault();
                    totalPaymentAmount = purchasedDriver.TaxableAmount ?? 0;
                    //totalPaymentAmount = purchasedDriver.TotalAmount ?? 0;
                }
            }
            return totalPaymentAmount * (decimal)1.15;
        }

        public List<OrderItem> CreateLeasingOrderItems(IList<ShoppingCartItemDB> shoppingCartItems, string referenceId, out string exception)
        {
            File.WriteAllText(@"C:\inetpub\WataniyaLog\CreateLeasingOrderItems_shoppingCartItems.txt", JsonConvert.SerializeObject(shoppingCartItems));
            File.WriteAllText(@"C:\inetpub\WataniyaLog\CreateLeasingOrderItems_referenceId.txt", referenceId);

            exception = string.Empty;
            var orderItems = new List<OrderItem>();
            try
            {
                decimal orderItemPrice = 0;
                foreach (var sci in shoppingCartItems)
                {
                    var orderItem = new OrderItem
                    {
                        CheckoutDetailReferenceId = referenceId,
                        CreatedOn = DateTime.Now,
                        ProductId = sci.ProductId,
                        Quantity = sci.Quantity,
                        //Price = sci.ProductPrice,
                        UpdatedOn = DateTime.Now
                    };
                    foreach (var benefit in sci.ShoppingCartItemBenefits)
                    {
                        var benefitPrice = benefit.BenefitPrice.GetValueOrDefault();
                        if (benefit.IsReadOnly)
                            benefitPrice = 0;
                        orderItem.OrderItemBenefits.Add(new OrderItemBenefit
                        {
                            BenefitId = benefit.BenefitId.GetValueOrDefault(),
                            BenefitExternalId = benefit.BenefitExternalId,
                            Price = benefitPrice
                        });
                        orderItemPrice += benefitPrice;
                    }
                    foreach (var driver in sci.ShoppingCartItemDrivers)
                    {
                        File.WriteAllText(@"C:\inetpub\WataniyaLog\SaveOrderItems_DriverId_" + driver.DriverId + ".txt", JsonConvert.SerializeObject(driver));
                        orderItem.OrderItemDrivers.Add(new OrderItemDriver
                        {
                            DriverId = driver.DriverId,
                            DriverExternalId = driver.DriverExternalId,
                            Price = driver.DriverPrice
                        });
                        orderItemPrice += driver.DriverPrice;
                    }

                    orderItem.Price = orderItemPrice;
                    orderItems.Add(orderItem);
                }
                if (orderItems.Count > 0)
                    _orderItemRepository.Insert(orderItems);
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
            }

            return orderItems;
        }

        public LeasingOrderItemDetails GetLeasingOrderItemByReferenceId(string referenceId, out string exception)
        {
            exception = string.Empty;
            LeasingOrderItemDetails orderItem = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLeasingOrderItemByReferenceId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                orderItem = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<LeasingOrderItemDetails>(reader).FirstOrDefault();
                if (orderItem != null)
                {
                    reader.NextResult();
                    orderItem.Benefits = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AdditionalBenefit>(reader).ToList();

                    reader.NextResult();
                    AdditionalDriver orderItemDriver = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AdditionalDriver>(reader).FirstOrDefault();
                    if (orderItemDriver != null)
                        orderItem.Driver = orderItemDriver;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            finally
            {
                dbContext.DatabaseInstance.Connection.Close();
            }

            return orderItem;
        }

        #endregion

        #region Calculate Actual Fees

        private decimal CalculateFees(int paymentMethodId, bool hasDiscount, decimal paymentAmount, int insuranceCompanyId, int insuranceTypeCode)
        {
            decimal fees = 0;

            decimal edaatAactualBankFees = 8.74M;
            decimal madaAactualBankFees = 2.013M;
            decimal visaMasterAactualBankFees = 2.645M;
            decimal AMEXAactualBankFees = 2.185M;
            decimal tabbyBankFees = 3.45M;

            if (paymentMethodId == 4 || paymentMethodId == 13) // || (paymentMethodId == 13 && !hasDiscount)
                fees = ((paymentAmount * visaMasterAactualBankFees) / 100) + 1.15M;
            else if (paymentMethodId == 10)
                fees = (paymentAmount * madaAactualBankFees) / 100;
            else if (paymentMethodId == 12)
                fees = edaatAactualBankFees;
            //else if (hasDiscount && paymentMethodId == 13)
            //    fees = ((paymentAmount * AMEXAactualBankFees) / 100) + 1.15M;
            else if (paymentMethodId == 16)
            {
                if (insuranceTypeCode == 1 || insuranceTypeCode == 7 || insuranceTypeCode == 8)
                    fees = ((paymentAmount * tabbyBankFees) / 100) + 1.15M;
            }

            return fees;
        }

        #endregion
    }
}
