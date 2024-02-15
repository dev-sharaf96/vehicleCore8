using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    public class ProposalResponseInfo
    {

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("idNumber")]
        public string IdNumber { get; set; }

        [JsonProperty("requiredInceptionDate")]
        public string RequiredInceptionDate { get; set; }

        [JsonProperty("policyFee")]
        public decimal PolicyFee { get; set; }

        [JsonProperty("VATRate")]
        public decimal? VATRate { get; set; }

        [JsonProperty("VATAmount")]
        public decimal VATAmount { get; set; }

        [JsonProperty("totalVehiclePremium")]
        public decimal TotalVehiclePremium { get; set; }

        [JsonProperty("totalLoading")]
        public decimal TotalLoading { get; set; }

        [JsonProperty("totalDiscount")]
        public string TotalDiscount { get; set; }

        [JsonProperty("paymentAmount")]
        public decimal PaymentAmount { get; set; }

        [JsonProperty("NCDRate")]
        public decimal? NCDRate { get; set; }

        [JsonProperty("NCDAmount")]
        public decimal NCDAmount { get; set; }

        [JsonProperty("proposalNumber")]
        public string ProposalNumber { get; set; }

        [JsonProperty("ExpiryDate")]
        public string ExpiryDate { get; set; }

        [JsonProperty("initiatedDateTime")]
        public string InitiatedDateTime { get; set; }

        [JsonProperty("deductible")]
        public int? Deductible { get; set; }
        [JsonProperty("promoAmount")]
        public decimal? PromoAmount { get; set; }

        [JsonProperty("promoPercentage")]
        public decimal? PromoPercentage { get; set; }

        [JsonProperty("vehicleArray")]
        public List<VehicleArray> VehicleArray { get; set; }

        [JsonProperty("VehicleLimitValue")]
        public decimal? VehicleLimitValue { get; set; }
    }

    public class VehicleArray
    {
        [JsonProperty("applicableFeatureGroup")]
        public List<ApplicableFeatureGroup> ApplicableFeatureGroup { get; set; }
    }

    public class ApplicableFeatureGroup
    {
        [JsonProperty("featureGroupNameEn")]
        public string FeatureGroupNameEn { get; set; }

        [JsonProperty("featureGroupNameAr")]
        public string FeatureGroupNameAr { get; set; }

        [JsonProperty("featureGroupDescEn")]
        public string FeatureGroupDescEn { get; set; }

        [JsonProperty("featureGroupDescAr")]
        public string FeatureGroupDescAr { get; set; }

        [JsonProperty("featureItems")]
        public FeatureItems FeatureItems { get; set; }
    }

    public class FeatureItems
    {
        [JsonProperty("featureItemsArray")]
        public List<FeatureItemsArray> FeatureItemsArray { get; set; }
    }

    public class FeatureItemsArray
    {
        [JsonProperty("featureCode")]
        public string FeatureCode { get; set; }

        [JsonProperty("featureText")]
        public string FeatureText { get; set; }

        [JsonProperty("featureDescriptionEn")]
        public string FeatureDescriptionEn { get; set; }

        [JsonProperty("featureDescriptionAr")]
        public string FeatureDescriptionAr { get; set; }

        [JsonProperty("priceDetails")]
        public PriceDetails PriceDetails { get; set; }
    }

    public class PriceDetails
    {
        [JsonProperty("priceDetailsArray")]
        public List<PriceDetailsArray> PriceDetailsArray { get; set; }
    }

    public class PriceDetailsArray
    {
        [JsonProperty("priceValue")]
        public decimal PriceValue { get; set; }
    }

    public class ProposalResponseFeature
    {

        [JsonProperty("featureText")]
        public string FeatureText { get; set; }

        [JsonProperty("featurePrice")]
        public decimal FeaturePrice { get; set; }

        [JsonProperty("featureOptionId")]
        public string FeatureOptionId { get; set; }

        [JsonProperty("featureOptionSetId")]
        public string FeatureOptionSetId { get; set; }

        [JsonProperty("ratingId")]
        public string RatingId { get; set; }

        [JsonProperty("ratingCategoryId")]
        public string RatingCategoryId { get; set; }

        [JsonProperty("totalContribution")]
        public string TotalContribution { get; set; }

        [JsonProperty("additionalContribution")]
        public string AdditionalContribution { get; set; }

        [JsonProperty("contributionRate")]
        public string ContributionRate { get; set; }

        [JsonProperty("extraContributionRate")]
        public string ExtraContributionRate { get; set; }

        [JsonProperty("descriptionId")]
        public string DescriptionId { get; set; }

        [JsonProperty("extraContributionAmount")]
        public string ExtraContributionAmount { get; set; }
    }

    public class ProposalResult
    {

        [JsonProperty("resultCode")]
        public string ResultCode { get; set; }

        [JsonProperty("resultMessage")]
        public string ResultMessage { get; set; }
    }

    public class ProposalResponse
    {

        [JsonProperty("proposalInfo")]
        public IList<ProposalResponseInfo> ProposalInfo { get; set; }

        [JsonProperty("features")]
        public IList<ProposalResponseFeature> Features { get; set; }

        [JsonProperty("proposalResult")]
        public ProposalResult ProposalResult { get; set; }
    }

    public class ProposalError
    {

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
    }

    public class GetProposalsResponse
    {

        [JsonProperty("proposalResponse")]
        public ProposalResponse ProposalResponse { get; set; }

        [JsonProperty("exception")]
        public IList<ProposalError> Errors { get; set; }
    }

    public class ProposalsResponseDto
    {

        [JsonProperty("getProposalsResponse")]
        public GetProposalsResponse GetProposalsResponse { get; set; }
    }


}
