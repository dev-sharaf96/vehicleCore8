using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class WhatsAppResponse
    {
        [JsonProperty("details")]
        public string Details { get; set; }
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
        [JsonProperty("messages")]
        public Message [] message { get; set; }
    }
    public class Message
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
        [JsonProperty("parts")]
        public string Parts { get; set; }
        [JsonProperty("messageDetails")]
        public string MessageDetails { get; set; }
        [JsonProperty("messageErrorCode")]
        public string MessageErrorCode { get; set; }

    }
    public enum WhatsAppResponseErrorCodes
    {
        Success=0,
        UnknownError=1,
        AuthenticationFailed=2,
        TheAccountUsingThisAuthenticationHasInsufficientBalance=3,
        TheProductTokenIsIncorrect=4,
        ThisRequestHasOneOrMoreErrorsInItsMessages=5,
        ThisRequestIsMalformed=6,
        TheRequestMSGArrayIsIncorrect=7,
        ThisMSGHasAnInvalidFromField=8,
        ThisMSGHasAnInvalidToField=9,
        ThisMSGHasAnInvalidPhoneNumberInTheToField=10,
        ThisMSGHasAnInvalidBodyField=11,
        ThisMSGHasAnInvalidField=12,
        MessageHasBeenSpamFiltered=13,
        MessageHasBeenBlacklisted=14,
        MessageHasBeenRejected=15,
        AnInternalErrorHasOccurred=16,
        ResponseIsNull=17,
        FailedToDeserialize=18,
        GenericError = 9999
    }
}
