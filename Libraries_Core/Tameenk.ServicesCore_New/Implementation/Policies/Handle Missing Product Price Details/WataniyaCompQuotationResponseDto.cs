using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    public class WataniyaCompQuotationResponseDto
    {
        public WataniyaCompQuotationResponseDetailsDto Details { get; set; }
        public int InsuranceCompanyCode { get; set; }
        public int QuoteReferenceNo { get; set; }
        public string RequestReferenceNo { get; set; }
        public bool Status { get; set; }
        public List<Errors> errors { get; set; }
    }

    public class WataniyaCompQuotationResponseDetailsDto
    {
        public List<Deductibles> Deductibles { get; set; }
        public short InspectionTypeId { get; set; }
        public long MaxLiability { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpiryDate { get; set; }
        public List<PolicyPremiunFeatures> PolicyPremiumFeatures { get; set; }
        public short PolicyTitleID { get; set; }
        //public List<CustomizedParameter> CustomizedParameter { get; set; }
    }

    public class Deductibles
    {
        public int DeductibleAmount { get; set; }
        public decimal PolicyPremium { get; set; }
        public List<PremiumBreakDown> PremiumBreakDown { get; set; }
        public float TaxableAmount { get; set; }
        public List<Discounts> Discounts { get; set; }
        public string DeductibleReferenceNo { get; set; }
    }

    public class PolicyPremiunFeatures
    {
        public short FeatureID { get; set; }
        public int FeatureTypeID { get; set; }
        public long FeatureAmount { get; set; }
        public long FeatureTaxableAmount { get; set; }
    }

    public class Errors
    {
        public string field { get; set; }
        public string message { get; set; }
        public string code { get; set; }
    }

    public class PremiumBreakDown
    {
        public int BreakDownTypeId { get; set; }
        public decimal BreakDownPercentage { get; set; }
        public decimal BreakDownAmount { get; set; }
    }

    public class Discounts
    {
        public int DiscountTypeId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public long NCDEligibility { get; set; }
    }
}
