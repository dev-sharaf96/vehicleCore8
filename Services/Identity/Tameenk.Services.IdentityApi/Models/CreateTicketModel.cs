using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.IdentityApi.Models
{
    public class CreateTicketModel: BaseViewModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// ticket Type
        /// </summary>
        [JsonProperty("ticketTypeId")]
        public int TicketTypeId { get; set; }

        /// <summary>
        /// Sequence Or Custom Card Number
        /// </summary>
        [JsonProperty("sequenceOrCustomCardNumber")]
        public string SequenceOrCustomCardNumber { get; set; }

        /// <summary>
        /// User Notes
        /// </summary>
        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }

        /// <summary>
        /// National ID
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }
    }
}