using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class CheckOutDetailsFilter
    {
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("insuredEmail")]
        public string InsuredEmail { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("productTypeId")]
        public int? ProductTypeId { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("exports")]
        public bool Exports { get; set; }

        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
    }
}
