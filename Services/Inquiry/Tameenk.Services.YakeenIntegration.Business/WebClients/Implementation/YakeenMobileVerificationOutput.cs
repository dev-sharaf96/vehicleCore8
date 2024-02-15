using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation
{
    public class YakeenMobileVerificationOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            ServiceException,
            YakeenServiceException,
            DateOfBirthGIsEmpty,
            NoAddressFound,
            InternalError,
            InvalidId,
            InvalidMobileNumber,
            InvalidServiceKey,
            InvalidMobileOwner
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
      public MobileVerificationModel mobileVerificationModel { get; set; }
    }
}
