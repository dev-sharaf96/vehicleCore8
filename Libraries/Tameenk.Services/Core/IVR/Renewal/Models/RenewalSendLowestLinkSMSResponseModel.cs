using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalSendLowestLinkSMSResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
    }
}
