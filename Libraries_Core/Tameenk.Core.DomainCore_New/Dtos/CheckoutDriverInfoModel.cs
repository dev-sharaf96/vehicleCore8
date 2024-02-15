using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Core.Domain.Dtos
{
    public class CheckoutDriverInfoModel : BaseViewModel
    {
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [Required]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [JsonProperty("IBAN")]
        public string IBAN { get; set; }
        
        [JsonProperty("isReceivePolicyByEmailChecked")]
        public bool IsReceivePolicyByEmailChecked { get; set; }
    }
}
