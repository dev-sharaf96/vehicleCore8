using Newtonsoft.Json;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("SamaReportStatistics")]
    public class SamaReportStatisticsModel
    {
        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}