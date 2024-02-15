using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Core.BlockNins
{
    public  class BlockedNinsListModel
    {
        [JsonProperty("nationalId")]
        public int NationalId { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }
    }
}
