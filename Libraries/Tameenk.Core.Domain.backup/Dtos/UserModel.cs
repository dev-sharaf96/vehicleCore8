using Newtonsoft.Json;
using System;

namespace Tameenk.Core.Domain.Dtos
{
    /// <summary>
    /// AspNet User Model
    /// </summary>
    [JsonObject("user")]
    public class UserModel
    {

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        [JsonProperty("roleId")]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last Modified Date
        /// </summary>
        [JsonProperty("lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Last Login Date
        /// </summary>
        [JsonProperty("lastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Full Name
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}