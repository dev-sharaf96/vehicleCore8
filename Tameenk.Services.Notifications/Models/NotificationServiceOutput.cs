using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Services.Notifications
{
    public enum ErrorCode
    {
        Success,
        EmptyTitle,
        EmptyBody,
        FireBaseError,
        NullMessageId,
        Exception,
        NullUserId,
        EmptyToken,
        NoUserTokenFound,
        TitleExceedsLimit,
        BodyExceedsLimit
    }
    public class NotificationServiceOutput
    {
        public ErrorCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
