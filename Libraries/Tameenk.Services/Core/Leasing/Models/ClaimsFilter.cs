using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class ClaimsFilter
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("statusId")]
        public int? StatusId { get; set; }

        [JsonProperty("requesterTypeId")]
        public int? RequesterTypeId { get; set; }

        [JsonProperty("accidentReportNumber")]
        public string AccidentReportNumber { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("export")]
        public bool Export { get; set; }
    }
}