using Newtonsoft.Json;

namespace Tameenk.Services.Capcha.API
{
    public class Tokenresponse

    {
        [JsonProperty("data")]
        public CaptchaModel Data { get; set; }
        [JsonProperty("errors")]
        public string Errors { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
    [JsonObject("captcha")]
    public class CaptchaModel
    {
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("expiredInSeconds")]
        public int ExpiredInSeconds { get; set; }
    }
}