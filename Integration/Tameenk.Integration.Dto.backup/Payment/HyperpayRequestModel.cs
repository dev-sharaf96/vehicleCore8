using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class HyperpayRequestModel
    {
      
        [JsonProperty("entityId")]
        public object EntityId { get; set; }
      
        [JsonProperty("amount")]
        public string Amount { get; set; }
    
        [JsonProperty("currency")]
        public string Currency { get; set; }
     
        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }
      
        [JsonProperty("notificationUrl")]
        public string NotificationUrl { get; set; }
      
        [JsonProperty("customParameters[payout]")]
        public HyperpayPayoutModel CustomParametersPayout { get; set; }

        [JsonProperty("merchantTransactionId")]
        public string MerchantTransactionId { get; set; }

        [JsonProperty("customer.email")]
        public string CustomerEmail { get; set; }

        [JsonProperty("customParameters[branch_id]")]
        public string CustomParametersBranchId { get; set; }

        [JsonProperty("customParameters[teller_id]")]
        public string CustomParametersLetterId { get; set; }

        [JsonProperty("customParameters[bill_number]")]
        public string CustomParametersBillNumber { get; set; }
        
    }
}
