
using Newtonsoft.Json;

namespace Tameenk.Core.Domain.Enums
{
    public enum ESTCSmsResponse
    {
        Success = 0,
        VariablesMissing = 1,
        InvalidLoginInfo = 2,
        ExceedNumberOfSendersAllowed = 22,
        SenderNameIsActiveOrUnderActivationOrRefused = 23,
        SenderNameShouldBeInEnglishOrNumber = 24,
        InvalidSenderNameLength = 25,
        SenderNameIsAlreadyActivatedOrNotFound = 26,
        ActivationCodeIsNotCorrect = 27,
        YouReachMaximumNumberOfAttemptsSenderNameIsLocked = 28,
        InvalidSenderName = 29,
        SenderNameShouldEndedWithAD = 30,
        MaximumAllowedSizeOfUploadedFileIs5MB = 31,
        FileExtensionNotAllowed = 32,
        SenderTypeShouldBeNormalOrWhitelistOnly = 33,
        PleaseUsePOSTMethod = 34,
        MsgBodyIsEmpty = 50,
        BalanceIsNotEnough = 60,
        MsgDuplicated = 61,
        SenderNameIsMissingOrIncorrect = 1110,
        MobileNumbersIsNotCorrect = 1120,
        MsgLengthIsTooLong = 1140,
        GenericError = 9999
    }
    public class STCSmsResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
