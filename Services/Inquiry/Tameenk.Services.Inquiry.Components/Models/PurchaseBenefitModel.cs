using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Inquiry.Components
{

    public class PurchaseBenefitModel : BaseModel
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("benefits")]
        public List<AdditionalBenefit> Benefits { get; set; }

    }

}