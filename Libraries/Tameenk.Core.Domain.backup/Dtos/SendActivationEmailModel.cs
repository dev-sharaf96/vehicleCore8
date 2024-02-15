using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Core.Domain.Dtos
{
    public class SendActivationEmailModel : BaseViewModel
    {
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [Required]
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
