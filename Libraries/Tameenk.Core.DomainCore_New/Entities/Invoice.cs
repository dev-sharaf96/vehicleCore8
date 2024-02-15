using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            Invoice_Benefit = new HashSet<Invoice_Benefit>();
        }

        public int Id { get; set; }
        
        public int InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime InvoiceDueDate { get; set; }
        
        public string UserId { get; set; }
        
        public string ReferenceId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        public int? InsuranceCompanyId { get; set; }

        public int? PolicyId { get; set; }

        public decimal? ProductPrice { get; set; }
        public decimal? ExtraPremiumPrice { get; set; }

        public decimal? Discount { get; set; }

        public decimal? Fees { get; set; }

        public decimal? Vat { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? SpecialDiscount { get; set; }
        public decimal? SpecialDiscountPercentageValue { get; set; }
        public decimal? SpecialDiscount2 { get; set; }
        public decimal? SpecialDiscount2PercentageValue { get; set; }
        public decimal? DiscountPercentageValue { get; set; }
        public decimal? LoyaltyDiscountValue { get; set; }
        public decimal? LoyaltyDiscountPercentage { get; set; }
        public decimal? TotalBenefitPrice { get; set; }

        public decimal? TotalBCareFees { get; set; }
        public decimal? TotalBCareCommission { get; set; }
        public decimal? TotalCompanyAmount { get; set; }

        public decimal? ActualBankFees { get; set; }

        public AspNetUser AspNetUser { get; set; }

        public InsuranceCompany InsuranceCompany { get; set; }

        public ICollection<Invoice_Benefit> Invoice_Benefit { get; set; }

        public Policy Policy { get; set; }

        public ProductType ProductType { get; set; }

        public InvoiceFile InvoiceFile { get; set; }
        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public string FeesCalculationDetails { get; set; }
        public int? TemplateId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? TotalBCareDiscount { get; set; }
        public string TaxInvoiceNumber { get; set; }
        public decimal? TotalDiscount { get; set; }
        public string ODReference { get; set; }
        public decimal? BasicPrice { get; set; }
        public decimal? PaidAmount { get; set; }
        public string MainPolicyReferance { get; set; }
    }
}
