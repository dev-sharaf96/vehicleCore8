using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class MorniMembershipOutput
    {
        public enum StatusCode
        {
            Success = 1,
            Failure = 2,
            RequestIsNull=3
        }
        public StatusCode ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public MorniOutput MorniOutput { get; set; }
      

    }

    public class MorniOutput
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
