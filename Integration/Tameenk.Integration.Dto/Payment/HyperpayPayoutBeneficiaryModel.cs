using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class HyperpayPayoutBeneficiaryModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("address2")]
        public string Address2 { get; set; }
        [JsonProperty("address3")]
        public string Address3 { get; set; }
        [JsonProperty("bankIdBIC")]
        public string BankIdBIC { get; set; }
        [JsonProperty("debitCurrency")]
        public string DebitCurrency { get; set; }
        [JsonProperty("transferAmount")]
        public string TransferAmount { get; set; }
        [JsonProperty("transferCurrency")]
        public string TransferCurrency { get; set; }

    }
}
