using Newtonsoft.Json;
using System;

namespace Tameenk.Services.IdentityApi.Models
{
    /// <summary>
    /// ConfirmationEmailModel
    /// </summary>
    public class ConfirmationEmailModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// RequestedDate
        /// </summary>
        [JsonProperty("requestedDate")]
        public DateTime RequestedDate { get; set; }
    }
}