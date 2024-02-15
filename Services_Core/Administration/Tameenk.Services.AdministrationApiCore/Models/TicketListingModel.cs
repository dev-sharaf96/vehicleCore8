using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class TicketListingModel
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
        /// User Name
        /// </summary>
        [JsonProperty("userPhone")]
        public string UserPhone { get; set; }

        /// <summary>
        /// Checkout Email
        /// </summary>
        [JsonProperty("checkoutEmail")]
        public string CheckedoutEmail { get; set; }

        /// <summary>
        /// Checkout Phone
        /// </summary>
        [JsonProperty("checkoutPhone")]
        public string CheckedoutPhone { get; set; }

        /// <summary>
        /// User Notes
        /// </summary>
        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }

        /// <summary>
        /// Status Name Ar
        /// </summary>
        [JsonProperty("statusNameAr")]
        public string StatusNameAr { get; set; }

        /// <summary>
        /// Status Name En
        /// </summary>
        [JsonProperty("statusNameEn")]
        public string StatusNameEn { get; set; }

        /// <summary>
        /// Ticket Type Name Ar
        /// </summary>
        [JsonProperty("ticketTypeNameAr")]
        public string TicketTypeNameAr { get; set; }

        /// <summary>
        /// Ticket Type Name En
        /// </summary>
        [JsonProperty("ticketTypeNameEn")]
        public string TicketTypeNameEn { get; set; }

        /// <summary>
        /// ticket Type
        /// </summary>
        [JsonProperty("ticketType")]
        public string TicketType { get; set; }

        /// <summary>
        /// Policy Id
        /// </summary>
        [JsonProperty("policyId")]
        public string PolicyId { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// invoice No
        /// </summary>
        [JsonProperty("invoiceId")]
        public int? InvoiceId { get; set; }

        /// <summary>
        /// invoice No
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// NIN
        /// </summary>
        [JsonProperty("nin")]
        public string DriverNin { get; set; }

        /// <summary>
        /// User Ticket Attachments
        /// </summary>
        [JsonProperty("userTicketAttachments")]
        public ICollection<UserTicketAttachment> UserTicketAttachments { get; set; }

        /// <summary>
        /// User Ticket Attachments Bytes
        /// </summary>
        [JsonProperty("userTicketAttachmentsBytes")]
        public List<string> UserTicketAttachmentsBytes { get; set; }


        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// User Ticket Attachments Ids
        /// </summary>
        [JsonProperty("userTicketAttachmentsIds")]
        public List<int> UserTicketAttachmentsIds { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Custom Card Number
        /// </summary>
        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get;  set; }

        /// <summary>
        /// Created by
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; internal set; }
    }
}