using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public class IbanValidationOutput
    {
        public enum StatusCode
        {
            Success = 0,
            Failure = 2
        }
        public StatusCode ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool? IsValid { get; set; }
        public List<ValidationMessage> SuccessValidationMessages { get; set; }
        public List<ValidationMessage> FailureValidationMessages { get; set; }
    }
    public class ValidationMessage
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
