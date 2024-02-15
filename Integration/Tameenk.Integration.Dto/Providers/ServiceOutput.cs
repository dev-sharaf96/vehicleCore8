using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ServiceOutput
    {
        public enum ErrorCodes
        {
            Success=1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ResFileIsNull,
            ServiceException,
            HttpStatusCodeNotOk
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
        public QuotationServiceResponse QuotationServiceResponse
        {
            get;
            set;
        }
        
    }
}
