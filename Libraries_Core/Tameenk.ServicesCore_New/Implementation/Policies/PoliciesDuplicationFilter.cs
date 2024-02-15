using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    /// <summary>
    /// All Policies filter
    /// use class to filter policies with required enum
    /// </summary>
    [JsonObject("repeatedDataPoliciesFilter")]
    public class PoliciesDuplicationFilter
    {
        /// <summary>
        /// duplicated policies by  Email or Phone or IBAN
        /// </summary>
        [JsonProperty("duplicatedData")]
        public int? DuplicatedData { get; set; }

        /// <summary>
        /// end Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }


        /// <summary>
        /// start date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// is Export
        /// </summary>
        [JsonProperty("isExport")]
        public bool IsExport { get; set; }

        /// <summary>
        /// page Index
        /// </summary>
        [JsonProperty("pageIndex")]
        public int? PageIndex { get; set; }

        /// <summary>
        /// page Size
        /// </summary>
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
