using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Banks
{
    [JsonObject("BankModel")]
    public class BankModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
        [JsonProperty("nameAr")]
        public string NameAr { get; set; }
        [JsonProperty("iban")]
        public string IBAN { get; set; }
        [JsonProperty("nationalAddress")]
        public string NationalAddress { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("isAcitveWallet")]
        public bool? IsAcitveWallet { get; set; }
        [JsonProperty("hasWallet")]
        public bool? HasWallet { get; set; }
        [JsonProperty("purchaseByNegative")]
        public bool? PurchaseByNegative { get; set; }
        [JsonProperty("balance")]
        public decimal? Balance { get; set; }
    }
}
