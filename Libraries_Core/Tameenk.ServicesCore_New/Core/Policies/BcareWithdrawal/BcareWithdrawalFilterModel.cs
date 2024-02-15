using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class BcareWithdrawalFilterModel
    {
        [JsonProperty("productType")]
        public int ProductType { get; set; }

        [JsonProperty("returnedNumber")]
        public int ReturnedNumber { get; set; }

        [JsonProperty("weekNumber")]
        public int WeekNumber { get; set; }

        [JsonProperty("prizeNumber")]
        public int PrizeNumber { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}