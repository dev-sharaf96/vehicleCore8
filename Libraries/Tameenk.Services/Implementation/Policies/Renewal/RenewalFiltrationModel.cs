using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class RenewalFiltrationModel
    {
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        [JsonProperty("insuranceTypeCode")]
        public int? InsuranceTypeCode { get; set; }

        [JsonProperty("expirationDateFrom")]
        public DateTime? ExpirationDateFrom { get; set; }

        [JsonProperty("expirationDateTo")]
        public DateTime? ExpirationDateTo { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("export")]
        public bool Export { get; set; }
    }
}
