using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class AutoLeaseProposalsResponse
    {

        [JsonProperty("RequestReferenceNo")]
        public string RequestReferenceNo { get; set; }

        [JsonProperty("DriverDetails")]
        public List<DriverDetails> DriverDetails { get; set; }

        [JsonProperty("CompQuotes")]
        public List<CompQuotes> CompQuotes { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
        public string LesseeID { get; set; }
        public string VehicleUsagePercentage { get; set; }
        public string LesseeDateOfBirthG { get; set; }
        public string LesseeDateOfBirthH { get; set; }
        public string LesseeGender { get; set; }
        public string NCDEligibility { get; set; }
        public string NajmCaseDetails { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class CompQuotes
    {
        [JsonProperty("QuoteReferenceNo")]
        public string QuoteReferenceNo { get; set; }

        [JsonProperty("PolicyTitleID")]
        public string PolicyTitleID { get; set; }

        [JsonProperty("Deductibles")]
        public List<Deductibles> Deductibles { get; set; }

        [JsonProperty("PolicyPremiumFeatures")]
        public List<PolicyPremiumFeatures> PolicyPremiumFeatures { get; set; }
    }

    public class Deductibles
    {
        [JsonProperty("DeductibleAmount")]
        public string DeductibleAmount { get; set; }

        //[JsonProperty("DeductibleReferenceNo")]
        //public string DeductibleReferenceNo { get; set; }

        [JsonProperty("PolicyPremium")]
        public string PolicyPremium { get; set; }

        [JsonProperty("TaxableAmount")]
        public string TaxableAmount { get; set; }

        [JsonProperty("BasePremium")]
        public string BasePremium { get; set; }

        [JsonProperty("PremiumBreakdown")]
        public List<PremiumBreakdown> PremiumBreakdown { get; set; }

        //[JsonProperty("DynamicPremiumFeatures")]
        //public List<DynamicPremiumFeatures> DynamicPremiumFeatures { get; set; }

        [JsonProperty("Discounts")]
        public List<Discounts> Discounts { get; set; }

    }

    public class PolicyPremiumFeatures
    {
        [JsonProperty("FeatureID")]
        public string FeatureID { get; set; }
        [JsonProperty("FeatureTypeID")]
        public string FeatureTypeID { get; set; }
        [JsonProperty("FeatureAmount")]
        public string FeatureAmount { get; set; }
        [JsonProperty("FeatureTaxableAmount")]
        public string FeatureTaxableAmount { get; set; }
    }

    public class PremiumBreakdown
    {
        [JsonProperty("BreakdownTypeID")]
        public string BreakdownTypeID { get; set; }

        [JsonProperty("BreakdownAmount")]
        public string BreakdownAmount { get; set; }
    }


    public class DynamicPremiumFeatures
    {
        [JsonProperty("FeatureID")]
        public string FeatureID { get; set; }

        [JsonProperty("FeatureTypeID ")]
        public int FeatureTypeID { get; set; }
        [JsonProperty("FeatureAmount")]
        public float FeatureAmount { get; set; }
        [JsonProperty("FeatureTaxableAmount")]
        public float FeatureTaxableAmount { get; set; }
    }

    public class Discounts
    {
        [JsonProperty("DiscountTypeID")]
        public string DiscountTypeID { get; set; }

        [JsonProperty("DiscountPercentage")]
        public string DiscountPercentage { get; set; }

        [JsonProperty("DiscountAmount")]
        public string DiscountAmount { get; set; }
    }

}
