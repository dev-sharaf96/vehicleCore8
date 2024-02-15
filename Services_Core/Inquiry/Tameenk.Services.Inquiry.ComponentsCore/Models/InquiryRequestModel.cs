using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    /// <summary>
    /// The inquery request parameter.
    /// </summary>
    [JsonObject("inqueryRequest")]
    public class InquiryRequestModel 
    {
        /// <summary>
        /// The city code.
        /// </summary>
        [JsonProperty("cityCode")]
        [Required]
        public long CityCode { get; set; }

        /// <summary>
        /// The policy effective date.
        /// </summary>
        [JsonProperty("policyEffectiveDate")]
        [Required]
        public DateTime PolicyEffectiveDate { get; set; }


        /// <summary>
        /// Is the vehicle user commercially.
        /// </summary>
        [JsonProperty("isVehicleUsedCommercially")]
        public bool IsVehicleUsedCommercially { get; set; }


        /// <summary>
        /// Old owner national identification number.
        /// </summary>
        [JsonProperty("oldOwnerNin")]
        public long? OldOwnerNin { get; set; }

        /// <summary>
        /// Is customer have special need.
        /// </summary>
        [JsonProperty("isCustomerSpecialNeed")]
        public bool? IsCustomerSpecialNeed { get; set; }

        /// <summary>
        /// The addtional driver(s).
        /// </summary>
        [JsonProperty("drivers")]
        public List<DriverModel> Drivers { get; set; }

        /// <summary>
        /// The insured person information.
        /// </summary>
        [JsonProperty("insured")]
        public InsuredModel Insured { get; set; }

        /// <summary>
        /// The vheicle.
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// User captcha input.
        /// </summary>
        [JsonProperty("captchaInput")]
        public string CaptchaInput { get; set; }

        /// <summary>
        /// The captcha token.
        /// </summary>
        [JsonProperty("captchaToken")]
        public string CaptchaToken { get; set; }

        [JsonProperty("isEditRequest")]
        public bool IsEditRequest { get; set; }

        [JsonProperty("parentRequestId")]
        public Guid? ParentRequestId { get; set; } = null;

        [JsonProperty("Language")]
        public string Language { get; set; }
        [JsonProperty("Channel")]
        public string Channel { get; set; }
        [JsonProperty("isRenualRequest")]        public bool IsRenewalRequest { get; set; }
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("referenceId")]
        public string PreviousReferenceId { get; set; }

        [JsonProperty("MobileVersion")]
        [DefaultValue (null)]
        public string MobileVersion { get; set; }

        [NotMapped]
        //[JsonProperty("inquiryOutputModel")]
        [JsonIgnore]
        public InquiryOutputModel InquiryOutputModel { get; set; }
        [JsonProperty("benfits")]
        public List<int> Benfits { get; set; }
        [JsonProperty("vehicleAgencyRepair")]
        public bool VehicleAgencyRepair { get; set; }

        [JsonProperty("deductibleValue")]
        public int DeductibleValue { get; set; }
        [JsonProperty("isInitialOption")]
        public bool IsInitialOption { get; set; } = false;

        [JsonProperty("isBulkOption")]
        public bool IsBulkOption { get; set; } = false;

        [JsonProperty("contractDuration")]
        public int ContractDuration { get; set; }
        [JsonProperty("benifitsIds")]
        public List<short> BenifitsIds { get; set; }

        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }

        [JsonProperty("purchasedBenefits")]
        public List<AutoleasingRenewalPoliciesOldBenefitsModel> PurchasedBenefits { get; set; }

        [JsonProperty("oDInsuranceTypeCode")]
        public int? ODInsuranceTypeCode { get; set; }

        [JsonProperty("oDTPLExternalId")]
        public string ODTPLExternalId { get; set; }


        [JsonProperty("oDPolicyExpiryDate")]
        public DateTime? ODPolicyExpiryDate { get; set; }
    }
}
