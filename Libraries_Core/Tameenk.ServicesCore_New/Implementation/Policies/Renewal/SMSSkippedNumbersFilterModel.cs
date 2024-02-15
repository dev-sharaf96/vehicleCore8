using Newtonsoft.Json;
using System;
namespace Tameenk.Services.Core.Policies
{
    public class SMSSkippedNumbersFilterModel
    {
        [JsonProperty("phoneNo")]
        public string PhoneNo { get; set; }
    }
}
