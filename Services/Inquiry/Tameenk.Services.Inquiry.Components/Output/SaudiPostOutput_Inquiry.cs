using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Tameenk.Services.Inquiry.Components
{
    public class SaudiPostOutput_Inquiry
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
            ServiceException,
            TotalSearchResultsIsZero,
            InvalidPublicID,
            StatusdescriptionIsNotSuccess
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
        public SaudiPostApiResult_Inquiry Output
        {
            get;
            set;
        }

    }
}