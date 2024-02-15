using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Tameenk.Services.Checkout.Components
{
    public class SaudiPostOutput
    {
        public enum ErrorCodes
        {
            Success= 1,
            SuccessAdded=1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            DriverIsNull,
            ResultIsNull,
            ResultIsFailed,
            ServiceException
        }
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public string ErrorDescription
        {
            get;
            set;
        }
        public SaudiPostApiResult Output
        {
            get;
            set;
        }

    }
}