using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Quotation.Components
{
    public class AddVehicleBenefitModel:BaseModel
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