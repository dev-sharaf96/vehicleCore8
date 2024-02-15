using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    [JsonObject("InitInquiryResponse")]
    public class InquiryResponseModel
    {
        /// <summary>
        /// The city code.
        /// </summary>
        [JsonProperty("cityCode")]
        public long? CityCode { get; set; }

        /// <summary>
        /// The policy effective date.
        /// </summary>
        [JsonProperty("policyEffectiveDate")]
        public DateTime PolicyEffectiveDate { get; set; }

        /// <summary>
        /// Is the vehicle user commercially.
        /// </summary>
        [JsonProperty("isVehicleUsedCommercially")]
        public bool IsVehicleUsedCommercially { get; set; }

        /// <summary>
        /// Is the current cuatomer is the owner of this vehicle.
        /// </summary>
        [JsonProperty("isCustomerCurrentOwner")]
        public bool IsCustomerCurrentOwner { get; set; }

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
        public List<InquiryDriverModel> Drivers { get; set; }

        /// <summary>
        /// The insured person information.
        /// </summary>
        [JsonProperty("insured")]
        public InquiryInsuredModel Insured { get; set; }

        /// <summary>
        /// The vheicle.
        /// </summary>
        [JsonProperty("vehicle")]
        public InquiryVehicleModel Vehicle { get; set; }

        [JsonProperty("isMainDriverExist")]
        public bool IsMainDriverExist { get; set; }

        [JsonProperty("isVehicleExist")]
        public bool IsVehicleExist { get; set; }

        [JsonProperty("isRenualRequest")]
        public bool IsRenualRequest { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("oDInsuranceTypeCode")]
        public int? ODInsuranceTypeCode { get; set; }

        [JsonProperty("oDTPLExternalId")]
        public string ODTPLExternalId { get; set; }


        [JsonProperty("oDPolicyExpiryDate")]
        public DateTime? ODPolicyExpiryDate { get; set; }
    }
}
