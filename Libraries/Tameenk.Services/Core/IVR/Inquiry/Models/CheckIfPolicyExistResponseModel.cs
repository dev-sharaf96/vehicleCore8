using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class CheckIfPolicyExistResponseModel
    {
        [JsonProperty("isExist")]
        public bool IsExist { get; set; }
    }
}
