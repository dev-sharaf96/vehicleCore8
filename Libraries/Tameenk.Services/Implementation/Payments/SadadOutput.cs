using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Services.Implementation.Payments
{
    public class SadadOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            GenerateRequestXMLFailed,
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
        public SadadResponse Output
        {
            get;
            set;
        }

    }
}
