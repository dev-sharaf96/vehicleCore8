using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;

namespace Tameenk.Services.InquiryGateway.Services.Implementation.SaudiPost
{
    public class SaudiPostOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
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