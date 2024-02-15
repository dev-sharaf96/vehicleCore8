using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class UserClaimModel
    {
        [JsonProperty("claims")]
        public List<UserClaimListingModel> Claims { get; set; }

        [JsonProperty("claimsCount")]
        public int ClaimsCount { get; set; }
    }
}