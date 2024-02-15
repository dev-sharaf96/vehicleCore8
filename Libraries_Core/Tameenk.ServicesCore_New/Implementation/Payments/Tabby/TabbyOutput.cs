using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyOutput
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
            NullResult = 18,
            EmptyPaymentId,
            TabbyResponseObjectIsNull,
            Unauthorized,
            InternalServerError,
            BadRequest,
            ResponseIsNull,
            checkoutDetailsIsNull,
            InvalidTabbyNotificationStatus,
            InvoiceIsNulll,
            InvalidAmount,
            UserLockedOut,
            UserNotFound,
            tabbyWebHookModel,
            WebhookNotAuthorized
        }

        public ErrorCodes ErrorCode { get; set; }
        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }
        public TabbyResponseHandler TabbyResponseHandler { get; set; }
        public TabbyResponseStatus TabbyResponseStatus { get; set; }
        public TabbyErrorStatus TabbyErrorStatus { get; set; }
        public TabbyResponseMessage TabbyResponseMessage { get; set; }
        public TabbyNotificationResponseModel TabbyNotificationResponseModel { get; set; }
        public TabbyCaptureResponseViewModel TabbyCaptureResponseViewModel { get; set; }
        

    }
}
