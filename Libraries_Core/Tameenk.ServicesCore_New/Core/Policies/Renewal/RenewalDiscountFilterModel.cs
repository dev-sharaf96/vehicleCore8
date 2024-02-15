using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class RenewalDiscountFilterModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("percentage")]
        public decimal? Percentage { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("messageType")]
        public int MessageType { get; set; }
        
        [JsonProperty("codeType")]
        public int CodeType { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("discountType")]
        public int DiscountType { get; set; }

        [JsonProperty("value")]
        public decimal? Amount { get; set; }
    }
}
