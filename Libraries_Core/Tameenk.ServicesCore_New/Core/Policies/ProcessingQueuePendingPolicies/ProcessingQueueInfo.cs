using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Core.Policies
{
    public class ProcessingQueueInfo
    {
        [JsonProperty("referenceId")]
        public  string? ReferenceId { get; set; }

        [JsonProperty("triesNumber")]
        public int ProcessingTries { get; set; }

        [JsonProperty("errorDescription")]
        public  string? ErrorDescription { get; set; }

        [JsonProperty("companyName")]
        public  string? CompanyName { get; set; }

        public int? InsuranceTypeCode { get; set; }

        [JsonProperty("nationalId")]
        public  string? DriverNin { get; set; }

        [JsonProperty("vehicleId")]
        public  string? VehicleId { get; set; }

        [JsonProperty("serviceRequest")]
        public  string? ServiceRequest { get; set; }

        [JsonProperty("serviceResponse")]
        public  string? ServiceResponse { get; set; }

        [JsonProperty("chanel")]
        public  string? Chanel { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }
    }
}
