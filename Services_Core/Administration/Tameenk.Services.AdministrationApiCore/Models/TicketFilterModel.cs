using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class TicketFilterModel
    {
        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        /// <summary>
        /// Status Id
        /// </summary>
        [JsonProperty("statusId")]
        public int? StatusId { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [JsonProperty("fromDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [JsonProperty("toDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nin")]
        public string NationalId { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// invoice No
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// Checkout Email
        /// </summary>
        [JsonProperty("checkoutEmail")]
        public string CheckoutEmail { get; set; }

        /// <summary>
        /// Checkout phone
        /// </summary>
        [JsonProperty("checkoutphone")]
        public string Checkoutphone { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        [JsonProperty("channel")]
        public int? ChannelId { get; set; }

        /// <summary>
        /// ticket Type
        /// </summary>
        [JsonProperty("ticketTypeId")]
        public int? TicketTypeId { get; set; }
    }
}