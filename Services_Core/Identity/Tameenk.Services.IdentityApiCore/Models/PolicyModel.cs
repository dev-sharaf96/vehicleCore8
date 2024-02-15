using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.IdentityApi.Models
{
    public class SearchPolicyModel
    {
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public string NIN { get; set; }
        public ValidateCaptchaModel Captcha { get; set; }
        public Tameenk.Services.Checkout.Components.Output.PolicyOutput PolicyOutput { get; set; }
        public string Channel { get; set; } 
        public string Lang { get; set; } = "en";

        [JsonProperty("hashed")]
        public string Hashed { get; set; }

        [JsonProperty("vehicleIdTypeId")]
        public int VehicleTypeId { get; set; }
    }
}