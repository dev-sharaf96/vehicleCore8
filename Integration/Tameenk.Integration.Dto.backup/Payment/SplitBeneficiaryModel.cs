using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class SplitBeneficiaryModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
        [JsonProperty("payoutBeneficiaryAddress1")]
        public string Address1 { get; set; }
        [JsonProperty("payoutBeneficiaryAddress2")]
        public string Address2 { get; set; }
        [JsonProperty("payoutBeneficiaryAddress3")]
        public string Address3 { get; set; }
        [JsonProperty("debitCurrency")]
        public string DebitCurrency { get; set; }
        [JsonProperty("transferAmount")]
        public string TransferAmount { get; set; }
        [JsonProperty("transferCurrency")]
        public string TransferCurrency { get; set; }
      
    }
}
