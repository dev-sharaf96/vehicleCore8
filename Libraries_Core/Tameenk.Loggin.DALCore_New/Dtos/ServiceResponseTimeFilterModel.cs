using Newtonsoft.Json;

namespace Tameenk.Loggin.DAL
{
    [JsonObject("ServiceResponseTimeFilter")]
    public class ServiceResponseTimeFilterModel
    {
        /// <summary>
        /// Status Code
        /// </summary> 
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; }

        [JsonProperty("moduleId")]
        public int? ModuleId { get; set; }
        /// <summary>
        /// createdOn
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("insuranceCompanyId")]
        public string InsuranceCompanyId { get; set; }

        [JsonProperty("insuranceTypeId")]
        public string InsuranceTypeId { get; set; }
    }
}

