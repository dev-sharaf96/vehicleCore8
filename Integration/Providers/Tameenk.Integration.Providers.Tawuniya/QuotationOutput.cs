using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya
{
    public class QuotationOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            HttpStatusCodeNotOk,
            ProductTypeCodeNotSupported,
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
        public string ServiceRequest
        {
            get;
            set;
        }
        public string ServiceResponse
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
