using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Loggin.DAL
{
    public class ServiceRequestResponseTimeFromDBModel
    {
        [JsonProperty("companyName")] 
        public string CompanyName { get; set; }
        [JsonProperty("avgInSec")]
        public double? AvgInSec { get; set; }

        [JsonProperty("slowestResponse")]
        public double? SlowestResponse { get; set; }

        [JsonProperty("fastestResponse")]
        public double? FastestResponse { get; set; }

    }
}