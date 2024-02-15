using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services
{
    public class CorporateNewUserModel : BaseViewModel
    {
        /// <summary>
        /// user Id
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// user FullName
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// user Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// user Password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// user PhoneNumber
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// user AccountId
        /// </summary>
        [JsonProperty("accountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// lang
        /// </summary>
        [JsonProperty("lang")]
        public string Lang { get; set; } = "en";

        /// <summary>
        /// isSuperAdmin
        /// </summary>
        [JsonProperty("isSuperAdmin")]
        public bool IsSuperAdmin { get; set; }
    }
}
