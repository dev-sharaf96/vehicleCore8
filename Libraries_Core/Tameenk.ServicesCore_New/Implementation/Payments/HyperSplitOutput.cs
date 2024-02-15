using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
namespace Tameenk.Services
{
    public class HyperSplitOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            GenerateRequestXMLFailed,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            MissingInput,
            InvoiceIsNull,
            TotalCompanyAmountIsLessZero,
            InsuranceCompanyIdIsNull,
            BCareBankAccountIsNull,
            CompanyBankAccountIsNull,
            ResponseIsNull,
            ServiceException
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
        public string AccessToken
        {
            get;
            set;
        }
        public HyperpayRequest HyperpayRequest
        {
            get;
            set;
        }
        
    }
}
