using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Payment
{
    public class HyperpayOrderModel
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("configId")]
        public string ConfigId { get; set; }
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }
        [JsonProperty("beneficiaryAccountId")]
        public string BeneficiaryAccountId { get; set; }

    }
}
