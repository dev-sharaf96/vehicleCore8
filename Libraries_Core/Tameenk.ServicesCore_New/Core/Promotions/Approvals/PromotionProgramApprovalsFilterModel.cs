using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Core.Promotions
{
    public class PromotionProgramApprovalsFilterModel
    {
        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}