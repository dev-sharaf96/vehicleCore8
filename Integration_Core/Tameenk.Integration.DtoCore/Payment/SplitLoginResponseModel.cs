using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class SplitLoginResponseModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public List<string> Errors { get; set; }

       
        public Data data { get; set; }
    }

    [JsonObject("data")]
    public class Data
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}
