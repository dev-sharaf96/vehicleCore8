using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Models
{
    /// <summary>
    /// ConfirmationEmailAfterVerifyCodeModel
    /// </summary>
    public class ConfirmationEmailAfterVerifyCodeModel
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