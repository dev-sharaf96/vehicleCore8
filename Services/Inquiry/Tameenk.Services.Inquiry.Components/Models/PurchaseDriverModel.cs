using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Tameenk.Services.Inquiry.Components
{

    public class PurchaseDriverModel : BaseModel
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }         

        [JsonProperty("paymentAmount")]
        public decimal PaymentAmount { get; set; }      

    }
}