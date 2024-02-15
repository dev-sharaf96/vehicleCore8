using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class DriverWithOverFivePoliciesFilter
    {
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("isExport")]
        public bool IsExport { get; set; }
    }
}
