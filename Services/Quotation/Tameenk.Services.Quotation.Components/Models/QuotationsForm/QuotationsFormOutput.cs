using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            QuotationResponseIsNull,
            CheckoutDetailsIsNull,
            OrderItemIsNull,
            InsuranceCompanyIsNull,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            PolicyScheduleError,
            ProviderTypeIsNull,
            ProviderIsNull,
            DatabaseException,
            ServiceException,
            NoFileUrlNoFileData,
            ExceptionError,
            DepreciationSettingsNotFound,
            RepairMethodSettingsNotFoundForThisBank,
            NullResult,
            FileIsNull
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

        public byte[] File
        {
            get;
            set;
        }
    }
}
