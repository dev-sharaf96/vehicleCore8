using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
        public class RenewalSendSMS
        {
            [JsonProperty("lang")]
            public string Lang { get; set; }

            [JsonProperty("percentage")]
            public decimal? Percentage { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("startDate")]
            public DateTime? StartDate { get; set; }

            [JsonProperty("endDate")]
            public DateTime? EndDate { get; set; }

            [JsonProperty("messageType")]
            public int? MessageType { get; set; }

            [JsonProperty("policyNo")]
            public string PolicyNo { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("discountType")]
            public int? DiscountType { get; set; }

            [JsonProperty("value")]
            public decimal? Value { get; set; }

            [JsonProperty("vehiclemaker")]
            public string Vehiclemaker { get; set; }

            [JsonProperty("vehicleModel")]
            public string VehicleModel { get; set; }

            [JsonProperty("expiryDate")]
            public DateTime? ExpiryDate { get; set; }

            [JsonProperty("externalId")]
            public string ExternalId { get; set; }

            [JsonProperty("referenceId")]
            public string ReferenceId { get; set; }

            [JsonProperty("plate")]
            public short Plate { get; set; }

            [JsonProperty("sequenceNumber")]
            public string SequenceNumber { get; set; }

            [JsonProperty("customCardNumber")]
            public string CustomCardNumber { get; set; }

            [JsonProperty("nIN")]
            public string NIN { get; set; }


    }

    
}
