
using Newtonsoft.Json;
using System;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Generic.Components.Models
{

    public class CareerModel 
    {
        [JsonProperty("fullName")]
        public string FullName { set; get; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mobileNo")]
        public string MobileNo { get; set; }
        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }
        [JsonProperty("cityId")]
        public int CityId { set; get; }
        [JsonProperty("cityName")]
        public string CityName { get; set; }
        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("fileToUpload")]
        public Byte[] FileToUpload { get; set; }

        [JsonProperty("attachmentName")]
        public string AttachmentName { set; get; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; } = "ar";

        [JsonProperty("channel")]
        public Channel Channel { get; set; } = Channel.Portal;
    }
}
