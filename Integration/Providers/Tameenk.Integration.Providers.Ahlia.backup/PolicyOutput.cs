﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Ahlia
{
    public class PolicyOutput
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
