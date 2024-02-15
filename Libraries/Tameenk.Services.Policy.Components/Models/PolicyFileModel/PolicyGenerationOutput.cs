using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyGenerationOutput
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
            InvoiceIsNull,
            PolicyIsNull,
            PriceDetailIsNull,
            ExceptionError
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
        public PolicyTemplateGenerationModel PolicyModel
        {
            get;
            set;
        }
        public EPolicyStatus EPolicyStatus
        {
            get;
            set;
        }

        public byte[] File
        {
            get;
            set;
        }
        public bool IsPdfGeneratedByBcare
        {
            get;
            set;
        }
    }
}
