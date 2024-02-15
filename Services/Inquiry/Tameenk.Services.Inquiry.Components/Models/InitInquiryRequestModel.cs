using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Inquiry;

namespace Tameenk.Services.Inquiry.Components
{
    public class InitInquiryRequestModel : BaseModel
    {
        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// National ID / Iqama ID
        /// </summary>
        [JsonProperty("nationalId")]
        [Required(ErrorMessageResourceType = typeof(SubmitInquiryResource), ErrorMessageResourceName = "NationalIdRequired")]
        [Range(1000000000, 99999999999)]
        public string NationalId { get; set; }

        /// <summary>
        /// The policy effective date.
        /// </summary>
        [JsonProperty("policyEffectiveDate")]
        [Required]
        public DateTime PolicyEffectiveDate { get; set; }

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


        /// <summary>
        /// The vehicle identity type identifier.
        /// </summary>
        [JsonProperty("VehicleIdTypeId")]
        public int VehicleIdTypeId { get; set; }


        /// <summary>
        /// Is the vehicle owner trnasfer.
        /// </summary>
        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }

        /// <summary>
        /// Owner national identifier.
        /// </summary>
        [JsonProperty("ownerNationalId")]
        public string OwnerNationalId { get; set; }


        [JsonProperty("parentRequestId")]
        public Guid? ParentRequestId { get; set; } = null;


        [JsonProperty("MobileVersion")]
        [DefaultValue(null)]
        public string MobileVersion { get; set; } = null;
        [JsonProperty("mobileNo")]
        public string MobileNo { get; internal set; }
    }
}
