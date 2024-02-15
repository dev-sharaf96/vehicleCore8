using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class CreateTicketModel
    {
        ///// <summary>
        ///// User Email
        ///// </summary>
        //[JsonProperty("userEmail")]
        //public string UserEmail { get; set; }

        /// <summary>
        /// User Phone
        /// </summary>
        [JsonProperty("userPhone")]
        public string UserPhone { get; set; }

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
        /// Language
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Attached Files
        /// </summary>
        [JsonProperty("attachedFiles")]
        public List<AttachedFiles> AttachedFiles { get; set; }

        /// <summary>
        /// NIN
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }
    }

    
}