using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    /// <summary>
    /// Wallet Add Balance Model
    /// </summary>
    public class WalletAddBalanceModel
    {
        /// <summary>
        /// account id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// BalanceAddedBy
        /// </summary>
        [JsonProperty("balanceAddedBy")]
        public string BalanceAddedBy { get; set; }

        /// <summary>
        /// TransactionTypeId
        /// 1 --> deposit(adding)
        /// 2 --> withdraw(deducting)
        /// </summary>
        [JsonProperty("transactionTypeId")]
        public int TransactionTypeId { get; set; }
    }
}
