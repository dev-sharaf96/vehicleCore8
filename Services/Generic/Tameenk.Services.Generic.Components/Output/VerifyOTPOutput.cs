using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Generic.Components.Output
{
    public class VerifyOTPOutput 
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidOTP = 2,
            OTPExpired = 3,
            EmptyParams = 4,
            ServiceException = 5,
            Failed = 6,
            NotFound = 7
        }
        public string ErrorDescription
        {
            get;
            set;
        }
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
    }
}
