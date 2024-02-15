using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tameenk.Services.InquiryGateway.Models.ExceptionHandlerModel;

namespace Tameenk.Services.InquiryGateway.Models
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
        [NotMapped]
        [JsonProperty("inquiryOutputModel")]
        public InquiryOutputModel InquiryOutputModel { get; set; }

    }
}