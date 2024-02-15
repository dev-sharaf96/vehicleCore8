using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models.OutPut
{
    public class CommissionOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            policyResFileNullResponse,
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
        public object Output
        {
            get;
            set;
        }

    }
}