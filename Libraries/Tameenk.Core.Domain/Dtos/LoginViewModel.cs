using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Core.Domain.Dtos
{
    public class LoginViewModel:BaseViewModel
    {
        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }

        public string UserName { get; set; }
        public string PWD { get; set; }
        public string Phone { get; set; }
        public string NationalId { get; set; }
        public int? OTP { get; set; }

        [JsonProperty("birthMonth")]
        public int? BirthMonth { get; set; } = null;

        [JsonProperty("birthYear")]
        public int? BirthYear { get; set; } = null;

        [JsonProperty("fnar")]
        public string FullNameAr { get; set; }

        [JsonProperty("fnen")]
        public string FullNameEn { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }

        [JsonProperty("isYakeenChecked")]
        public bool IsYakeenChecked { get; set; }
    }
}
