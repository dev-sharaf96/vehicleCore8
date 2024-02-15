using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Policy Checkout Model
    /// </summary>
    [JsonObject("policyCheckoutModel")]
    public class PolicyCheckoutModel
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("iban")]
        public string IBAN { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonProperty("isDeleted")]
        public bool? IsDeleted { get; set; } = false;

        [JsonProperty("deletedDate")]
        public DateTime? DeletedDate { get; set; }

        [JsonProperty("deletedBy")]
        public string DeletedBy { get; set; }

    }
}