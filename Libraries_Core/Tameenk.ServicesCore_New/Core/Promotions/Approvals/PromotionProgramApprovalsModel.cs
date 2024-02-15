using Newtonsoft.Json;
using System.IO;

namespace Tameenk.Services.Core.Promotions
{
    public class PromotionProgramApprovalsModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nin")]
        public string NationalId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("enrolledType")]
        public string EnrolledType { get; set; }

        [JsonProperty("file")]
        public string AttachmentPath { get; set; }

        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("ninVerified")]
        public bool NinVerified { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        //[JsonProperty("fileBytes")]
        //public byte[] FileBytes { get { return File.ReadAllBytes(AttachmentPath); } }
    }
}