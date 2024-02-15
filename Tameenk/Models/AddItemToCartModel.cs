using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class AddItemToCartModel
    {
        public string QuotaionRequestExternalId { get; set; }
        public string ReferenceId { get; set; }
        public string ProductId { get; set; }
        public List<int> SelectedProductBenfitId { get; set; }

        [JsonProperty("SelectedProducatBenfits")]
        public List<string> SelectedProductBenfit { get; set; }
    }
}