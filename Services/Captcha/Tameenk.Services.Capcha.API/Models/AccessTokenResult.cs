using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Capcha.API
{
    public class AccessTokenResult
    {
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
        [JsonProperty("tokenExpirationDate")]
        public DateTime TokenExpirationDate { get; set; }
        [JsonProperty("canPurchase")]
        public bool CanPurchase { get; set; }
    }
}