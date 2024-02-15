using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Generic.Components.Models
{
    public class VerifyOTPModel :BaseViewModel
    {
        [JsonProperty("otp")]
        public int otp { get; set; }
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }
    }
}
