using Newtonsoft.Json;

namespace Tameenk.Services.Capcha.API
{
    public class UserData
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}