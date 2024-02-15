using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Tameenk.Services.Inquiry.Components
{

    public class AddBenefitModel : BaseModel
    {
        [Required]
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        [Required]
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        [Required]
        [JsonProperty("benefitStartDate")]
        public DateTime BenefitStartDate { get; set; }

      

    }
}