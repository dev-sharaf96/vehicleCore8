using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies.Renewal
{
   public class PolicyFilterOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceDown,
            ServiceException,
            NotFound,
            ExceptionError,
            NotAuthorized,
            UserNotFound,
            NullResult,
        }
        public string ErrorDescription { get; set; }

        public ErrorCodes ErrorCode { get; set; }

        public List<RenewalPolicyDetails> Result { get; set; }

        public int? TotalCount { get; set; }
    }
}
