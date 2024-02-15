using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
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

        //[JsonProperty("isCorporateUser")]
        //public bool IsCorporateUser { get; set; }
    }
}
