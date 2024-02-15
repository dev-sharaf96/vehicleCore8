using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Generic.Components.Output
{
    public class CareerOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            Failed = 6
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
