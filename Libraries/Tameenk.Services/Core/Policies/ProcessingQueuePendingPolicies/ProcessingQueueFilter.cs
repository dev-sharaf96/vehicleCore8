using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class ProcessingQueueFilter
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("companyId")]
        public int? CompanyId { get; set; }

        [JsonProperty("productTypeId")]
        public int? ProductTypeId { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [JsonProperty("isExport")]
        public bool IsExport { get; set; }
    }
}
