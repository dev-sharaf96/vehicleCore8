using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("SamaReportResult")]
    public class SamaReportResultModel
    {

        [JsonProperty("samaReports")]
        public List<SamaReportModel> SamaReports { get; set; }

        [JsonProperty("samaReportStatistics")]
        public List<SamaReportStatisticsModel> SamaReportStatistics { get; set; }
    }
}