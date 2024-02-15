using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
   public  class InvoiceFilters
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id;

        /// <summary>
        /// insurance Company Id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Product Type ID
        /// </summary>
        [JsonProperty("productTypeID")]
        public int ProductTypeID { get; set; }

        /// <summary>
        /// Reference ID
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Invoice Number
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// Policy Number
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
         [JsonProperty("NIN")]
        public string NIN { get; set; }




    }
}
