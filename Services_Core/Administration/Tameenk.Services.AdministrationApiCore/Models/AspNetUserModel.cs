using Newtonsoft.Json;
using System;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("userModel")]

    public class AspNetUserModel
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }

        [JsonProperty("lastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        [JsonProperty("languageNameAr")]
        public string LanguageNameAr { get; set; }

        [JsonProperty("languageNameEn")]
        public string LanguageNameEN { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [JsonProperty("lockoutEndDateUtc")]
        public DateTime? LockoutEndDateUtc { get; set; }

        [JsonProperty("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [JsonProperty("accessFailedCount")]
        public int AccessFailedCount { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        public Guid RoleId { get; set; }

        public string DeviceToken { get; set; }

        public Guid LanguageId { get; set; }
        public string LockedBy { get; set; }
        public string LockedReason { get; set; }

        [JsonProperty("isPhoneVerifiedByYakeen")]
        public bool IsPhoneVerifiedByYakeen { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("fullNameAr")]
        public string FullNameAr { get; set; }
    }
}