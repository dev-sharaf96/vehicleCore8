using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class PolicyStatisticsFilterModel
    {
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }
        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
        [JsonProperty("isExcel")]
        public int? IsExcel { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}
