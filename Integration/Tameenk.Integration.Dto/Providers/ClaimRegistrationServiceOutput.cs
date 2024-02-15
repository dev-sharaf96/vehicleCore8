using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ClaimRegistrationServiceOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ResFileIsNull,
            ServiceException,
            HttpStatusCodeNotOk,
            Error
        }
        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public object Output { get; set; }
        public ClaimRegistrationServiceResponse ClaimRegistrationServiceResponse { get; set; }
    }
}
