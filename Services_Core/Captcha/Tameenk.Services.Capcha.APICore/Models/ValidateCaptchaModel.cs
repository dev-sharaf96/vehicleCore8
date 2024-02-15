using Newtonsoft.Json;

namespace Tameenk.Services.Capcha.API
{
    [JsonObject("validateCaptchaModel")]

    public class ValidateCaptchaModel
    {
            [JsonProperty("token")]
            public string Token { get; set; }
            [JsonProperty("input")]
            public string Input { get; set; }
    }
}