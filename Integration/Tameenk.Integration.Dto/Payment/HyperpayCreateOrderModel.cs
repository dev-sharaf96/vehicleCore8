using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Integration.Dto.Payment
{
    public class HyperpayCreateOrderModel
    {
        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; set; }

        [JsonProperty("transferOption")]
        public string TransferOption { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("batchDescription")]
        public string BatchDescription { get; set; }

        [JsonProperty("configId")]
        public string ConfigId { get; set; }

        [JsonProperty("beneficiary")]
        public List<BeneficiaryRequest> Beneficiary { get; set; }
    }

    [JsonObject("beneficiary")]
    public class BeneficiaryRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("debitCurrency")]
        public string DebitCurrency { get; set; }

        [JsonProperty("transferAmount")]
        public string TransferAmount { get; set; }

        [JsonProperty("transferCurrency")]
        public string TransferCurrency { get; set; }

        [JsonProperty("bankIdBIC")]
        public string BankIdBIC { get; set; }

        [JsonProperty("payoutBeneficiaryAddress1")]
        public string PayoutBeneficiaryAddress1 { get; set; }

        [JsonProperty("payoutBeneficiaryAddress2")]
        public string PayoutBeneficiaryAddress2 { get; set; }

        [JsonProperty("payoutBeneficiaryAddress3")]
        public string PayoutBeneficiaryAddress3 { get; set; }
    }
}
