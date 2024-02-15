using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Banks
{
  public  class AutoleasingWalletUpdateModel
    {
        [JsonProperty("bankId")]
        public int Id { get; set; }
        [JsonProperty("balance")]
        public decimal? Balance { get; set; }
        [JsonProperty("transactionTypeId")]
        public int TransactionTypeId { get; set; }
    }
}
