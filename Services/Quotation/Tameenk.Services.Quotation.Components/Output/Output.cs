using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class Output<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
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
            NullResult = 18
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
