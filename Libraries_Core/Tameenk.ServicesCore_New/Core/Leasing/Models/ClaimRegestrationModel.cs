using Newtonsoft.Json;

namespace Tameenk.Services.Core
{
    public class ClaimRegestrationModel
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public  string? ReferenceId { get; set; }

        /// <summary>
        /// Accident Report Number
        /// </summary>
        [JsonProperty("accidentReportNumber")]
        public  string? AccidentReportNumber { get; set; }

        /// <summary>
        /// Accident Report
        /// </summary>
        [JsonProperty("accidentReport")]
        public byte[]? AccidentReport { get; set; }

        /// <summary>
        /// Accident Report File Name
        /// </summary>
        [JsonProperty("accidentReportFileName")]
        public  string? AccidentReportFileName { get; set; }

        /// <summary>
        /// Accident Report Extension
        /// </summary>
        [JsonProperty("accidentReportExtension")]
        public  string? AccidentReportExtension { get; set; }
    }
}