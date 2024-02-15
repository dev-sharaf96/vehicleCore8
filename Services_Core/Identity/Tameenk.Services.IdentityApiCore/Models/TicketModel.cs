using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;
using TameenkDAL.Models;

namespace Tameenk.Services.IdentityApi.Models
{
    public class TicketModel
    {
        /// <summary>
        /// User Notes
        /// </summary>
        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }

        /// <summary>
        /// Ticket Id
        /// </summary>
        [JsonProperty("ticketId")]
        public int TicketId { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Insurance Company Name
        /// </summary>
        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        /// <summary>
        /// Status Name
        /// </summary>
        [JsonProperty("statusName")]
        public string StatusName { get; set; }

        /// <summary>
        /// Admin Reply
        /// </summary>
        [JsonProperty("adminReply")]
        public string AdminReply { get; set; }

        /// <summary>
        /// Vehicle Name
        /// </summary>
        [JsonProperty("vehicleName")]
        public string VehicleName { get; set; }

        /// <summary>
        /// Status Id
        /// </summary>
        [JsonProperty("ticketStatusId")]
        public int TicketStatusId { get; internal set; }

        /// <summary>
        /// Vehicle Plate
        /// </summary>
        [JsonProperty("vehiclePlate")]
        public VehiclePlateModel VehiclePlate { get; internal set; }


    }
}