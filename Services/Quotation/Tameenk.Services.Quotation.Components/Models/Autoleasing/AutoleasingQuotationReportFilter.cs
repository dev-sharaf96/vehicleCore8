using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Quotation.Components
{
    public class AutoleasingQuotationReportFilter : BaseViewModel
    {
        [JsonProperty("isExcel")]
        public bool IsExcel { get; set; } = false;

        [JsonProperty("quotationNumber")]
        public string QuotationNumber { get; set; }

        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("chassisNo")]
        public string ChassisNo { get; set; }
    }
}