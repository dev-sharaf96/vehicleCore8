using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Promotions
{
    public class PromotionProgramApprovalActionModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string EnrolledType { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("Channel")]
        public string Channel { get; set; }
    }
}
