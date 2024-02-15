using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class SplitOrdersModel
    {
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; set; }
        [JsonProperty("transferOption")]
        public string TransferOption { get; set; }

        [JsonProperty("configId")]
        public string ConfigId { get; set; }
      
        [JsonProperty("beneficiary")]
        public List<SplitBeneficiaryModel> Beneficiary { get; set; }

        [JsonProperty("merchantRequestId")]
        public string MerchantRequestId { get; set; }

    }
}
