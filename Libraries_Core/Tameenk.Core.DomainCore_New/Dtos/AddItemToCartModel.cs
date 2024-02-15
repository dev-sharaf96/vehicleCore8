using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Dtos
{
    public class AddItemToCartModel
    {
        public string QuotaionRequestExternalId { get; set; }
        public string ReferenceId { get; set; }
        public string ProductId { get; set; }
        public List<long> SelectedProductBenfitId { get; set; }

        [JsonProperty("SelectedProducatBenfits")]
        public List<string> SelectedProductBenfit { get; set; }

        public string lang { get; set; }
        public string Channel { get; set; }
        public bool IsBulk { get; set; } = false;
        public bool IsIVRRequest { get; set; }
    }
}
