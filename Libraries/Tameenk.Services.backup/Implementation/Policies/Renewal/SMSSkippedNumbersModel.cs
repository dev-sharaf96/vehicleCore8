using Newtonsoft.Json;
using System;
namespace Tameenk.Services.Core.Policies
{
    public class SMSSkippedNumbersModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("phoneNo")]
        public string PhoneNo { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }
    }
}
