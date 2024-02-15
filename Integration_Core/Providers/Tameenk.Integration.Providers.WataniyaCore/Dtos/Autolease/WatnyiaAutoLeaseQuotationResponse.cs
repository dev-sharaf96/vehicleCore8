using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    class WatnyiaAutoLeaseQuotationResponse
    {
        public AutoLeasingPolicy Policy { get; set; }
        public List<PolicyRiskList> PolicyRiskList { get; set; }
        public int Status { get; set; }
        public List<AutoleaseError> ErrorList { get; set; }
    }

    public class AutoLeasingPolicy
    {
        public string ReferenceNumber { get; set; }
        public string QuoteExpiryDate { get; set; }
    }

    public class PolicyRiskList
    {
        public string NcdDiscount { get; set; }
        public List<PolicyDriverList> PolicyDriverList { get; set; }
        public List<PolicyPlanList> PolicyPlanList { get; set; }
        public string RiskID { get; set; }
    }

    public class PolicyDriverList
    {
        public string ArabicName { get; set; }
        public DateTime BirthDate { get; set; }
        public string BuildingNumber { get; set; }
        public string City { get; set; }
        public string DistrictName { get; set; }
        public int DriverIdType { get; set; }
        public string DriverName { get; set; }
        public string Gender { get; set; }
        public string Lessee { get; set; }
        public string LicenseNo { get; set; }
        public int LicenseType { get; set; }
        public int LicenseYear { get; set; }
        public int MaritalStatus { get; set; }
        public string StreetName { get; set; }
        public int Usage { get; set; }
    }

    public class PolicyPlanList
    {
        public decimal ActualPremium { get; set; }
        public string DeductibleType { get; set; }
        public int MinDeductibleForPartialLoss { get; set; }
        public string PlanCode { get; set; }
        public List<PolicyCoverage> PolicyCoverageList { get; set; }
    }

    public class PolicyCoverage
    {
        [JsonProperty("ActualPremium", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? ActualPremium { get; set; }

        [JsonProperty("AnnualPremiumBFD", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? AnnualPremiumBFD { get; set; }

        [JsonProperty("CoverageCode", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverageCode { get; set; }

        [JsonProperty("ODLimit", NullValueHandling = NullValueHandling.Ignore)]
        public int? ODLimit { get; set; }

        [JsonProperty("TPLLimit", NullValueHandling = NullValueHandling.Ignore)]
        public int? TPLLimit { get; set; }

        [JsonProperty("CoveredCountry", NullValueHandling = NullValueHandling.Ignore)]
        public string CoveredCountry { get; set; }

        [JsonProperty("AveragePremium", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? AveragePremium { get; set; }

        [JsonProperty("LIMIT", NullValueHandling = NullValueHandling.Ignore)]
        public int? LIMIT { get; set; }
    }

    public class AutoleaseError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        //public string Field { get; set; }
    }
}
