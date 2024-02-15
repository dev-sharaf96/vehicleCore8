using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tameenk.Resources.Inquiry;

namespace Tameenk.Services.InquiryGateway.Models
{
    /// <summary>
    /// Represent the insured personal information required for inquiry.
    /// </summary>
    [JsonObject("insured")]
    public class InsuredModel
    {
        /// <summary>
        /// National ID / Iqama ID
        /// </summary>
        [JsonProperty("nationalId")]
        [Required(ErrorMessageResourceType =typeof(SubmitInquiryResource), ErrorMessageResourceName = "NationalIdRequired")]
        [Range(1000000000, 99999999999)]
        public string NationalId { get; set; }

        /// <summary>
        /// Customer birthdate month.
        /// </summary>
        [JsonProperty("birthDateMonth")]
        [Required]
        [Range(1, 12)]
        public byte BirthDateMonth { get; set; }


        /// <summary>
        /// Customer birth date year.
        /// </summary>
        [JsonProperty("birthDateYear")]
        [Required]
        public short BirthDateYear { get; set; }
        
        /// <summary>
        /// Insured Education identifier.
        /// </summary>
        [Required]
        [JsonProperty("edcuationId")]
        public int EducationId { get; set; }

        /// <summary>
        /// Number of children under age 16 years.
        /// </summary>
        [JsonProperty("childrenBelow16Years")]
        public int ChildrenBelow16Years { get; set; }
        
    }
}