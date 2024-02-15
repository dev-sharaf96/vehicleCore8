using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    
    public class ShareQuotationOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            InvalidPhone,
            InvalidEmail,
            FailedToSendSMS,
            FailedToSendEmail,
            MaxLimitExceeded,
            ServiceDown,
            ServiceException
        }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

       

    }
}
