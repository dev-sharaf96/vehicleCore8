using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Wathq
{
  public  class WathqResponseModel
    {
        [JsonProperty("crName")]
        public string CrName { get; set; }
        [JsonProperty("crNumber")]
        public long CrNumber { get; set; }
        [JsonProperty("crEntityNumber")]
        public long CrEntityNumber { get; set; }
        [JsonProperty("isMain")]
        public bool IsMain { get; set; }
        [JsonProperty("businessType")]
        public BusinessType BusinessType { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

