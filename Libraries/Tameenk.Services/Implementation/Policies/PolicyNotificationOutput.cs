using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class PolicyNotificationOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            ServiceDown,
            ServiceException,
            NotFound,
            ExceptionError,
            NullResult,
        }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public List<PolicyNotificationsModel> Result { get; set; }

        public int totalCount { get; set; }
    }
}
