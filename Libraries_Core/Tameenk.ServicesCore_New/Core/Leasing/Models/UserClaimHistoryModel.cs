using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class UserClaimHistoryModel
    {
        [JsonProperty("claimId")]
        public int ClaimId { get; set; }

        [JsonProperty("historyId")]
        public int HistoryId { get; set; }

        [JsonProperty("claimStatusId")]
        public int ClaimStatusId { get; set; }

        [JsonProperty("claimStatusName")]
        public string ClaimStatusName { get; set; }

        [JsonProperty("historyCreatedDate")]
        public DateTime HistoryCreatedDate { get; set; }

        [JsonProperty("historyCreatedBy")]
        public string HistoryCreatedBy { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("fileId")]
        public int? FileId { get; set; }

        [JsonProperty("claimFilePath")]
        public string ClaimFilePath { get; set; }

        [JsonProperty("claimFileName")]
        public string ClaimFileName { get; set; }
    }
}