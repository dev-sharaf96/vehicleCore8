using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalOutput
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
        public EsalResponseDto UploadInvoiceResponse
        {
            get;
            set;
        }

        public EsalCancelResponseDto CancelInvoiceResponse
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }
    }
}
