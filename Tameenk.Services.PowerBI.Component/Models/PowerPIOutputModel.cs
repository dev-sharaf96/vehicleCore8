namespace Tameenk.Services.PowerBI.Component
{
    public class PowerPIOutputModel<TResult>
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            OwnerNationalIdAndNationalIdAreEqual = 6,
            NotFound = 7,
            CanNotCreate = 8,
            CanNotSendSMS = 9,
            ModelBinderError = 10,
            ExceptionError = 11,
            NotAuthorized = 12,
            LoginIncorrectPhoneNumberNotVerifed = 13,
            VerificationFaield = 14,
            unAuthorized = 15,
            InValidResponse = 16,
            NotSuccess = 17,
            ConfirmEmailNotEqualEmail,
            AccountLocked,
            EmailIsNotConfirmed,
            PhoneNumberIsNotConfirmed,
            TokenIsEmpty,
            UserNotFound,
            EmailNotSent,
            NewPassordNotMatchConfirmNewPassword,
            CanNotChangePassword,
            NullResult,
            ErrorHashing,
            HashedExpired,
            RefreshCaptcha,
            RenewalFlagError,
            EmptyResult,
            InvalidData,
            HashedNotMatched
        }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public TResult Result { get; set; }
    }
}
