using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationFormWithSelectedBenfitsViewModel
    {
        [JsonProperty("selectedBenfits")]
        public List<SelectedBenfitsViewModel> SelectedBenfits { get; set; }
        public string qtRqstExtrnlId { get; set; }
        public string channel { get; set; }
        public string lang { get; set; }
        public bool AgencyRepair { get; set; }
        public int deductible { get; set; }
        public int SelectedCompany { get; set; }
        public bool IsRenewal { get; set; }
        public int PoliciesCount { get; set; }
        public decimal VehiclOriginaleValue { get; set; }
    }

    public class SelectedBenfitsViewModel
    {
        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("benfitIds")]
        public List<short?> BenfitIds { get; set; }

        [JsonProperty("useBenefitExternalId")]
        public bool UseExternalId { get; set; }

        [JsonProperty("benfitExternalIds")]
        public List<string> BenfitExternalIds { get; set; }
    }
}
