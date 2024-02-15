using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class HyperpayPayoutModel
    {
       
        [JsonProperty("transferOption")]
        public string TransferOption { get; set; }
        [JsonProperty("configId")]
        public string ConfigId { get; set; }
        [JsonProperty("beneficiary")]
        public List<HyperpayPayoutBeneficiaryModel> Beneficiary { get; set; }
    }
}
