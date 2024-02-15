using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components.Output
{
    public class EdaatOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            UnAuthoriztation,
            NullResponse,
            NullRequest,
            ServiceError,
            ServiceException,
            AuthorizationFailed
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

        public string Token
        {
            get;
            set;
        }

        public string SubBillerRegistrationNo
        {
            set; get;
        }
    }
}
