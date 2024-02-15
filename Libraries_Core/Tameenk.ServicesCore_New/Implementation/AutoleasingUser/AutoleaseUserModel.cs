using Newtonsoft.Json;

namespace Tameenk.Services
{
    public class AutoleaseUserModel
    {
        /// <summary>
        /// user id
        /// </summary>
        [JsonProperty("id")]
        public  string? UserId { get; set; }

        /// <summary>
        /// user FullName
        /// </summary>
        [JsonProperty("fullName")]
        public  string? FullName { get; set; }

        /// <summary>
        /// user Email
        /// </summary>
        [JsonProperty("email")]
        public  string? Email { get; set; }

        /// <summary>
        /// user PhoneNumber
        /// </summary>
        [JsonProperty("phoneNumber")]
        public  string? PhoneNumber { get; set; }

        /// <summary>
        /// user BankName
        /// </summary>
        [JsonProperty("bankName")]
        public  string? BankName { get; set; }

        /// <summary>
        /// user BankId
        /// </summary>
        [JsonProperty("bankId")]
        public int BankId { get; set; }

        /// <summary>
        /// user isLock
        /// </summary>
        [JsonProperty("isLock")]
        public bool IsLock { get; set; }

        /// <summary>
        /// lang
        /// </summary>
        [JsonProperty("lang")]
        public  string? lang { get; set; } = "en";
    }
}