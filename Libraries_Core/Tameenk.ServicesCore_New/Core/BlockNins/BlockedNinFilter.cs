using Newtonsoft.Json;

namespace Tameenk.Services.Core.BlockNins
{
    public  class BlockedNinFilter
    {
       
            [JsonProperty("nationalId")]
            public string? NationalId { get; set; }
            [JsonProperty("isExport")]
            public bool IsExport { get; set; }
            [JsonProperty("pageIndex")]
            public int PageIndex { get; set; }
            [JsonProperty("pageSize")]
            public int PageSize { get; set; }
            [JsonProperty("lang")]
            public string? Lang { get; set; }
    }
}
