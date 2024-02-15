using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Generic.Components.Output
{
    public class UserInfoOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyParams,
            InvalidInput,
            AlreadyExits,
            Failed,
            InvalidPhoneNumber,
            NotFound,
            MissingData,
            GetYakeenInfoError,
            ServiceException
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
