using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Integration.Dto.Payment.CreateOrder
{
    public class CreateOrderResponseModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        public data data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class data
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
        public List<BeneficiaryResponse> Beneficiary { get; set; }
    }

    [JsonObject("beneficiary")]
    public class BeneficiaryResponse
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
    }
}

