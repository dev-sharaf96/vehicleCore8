using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Models
{
    public class PdfGenerationOutput
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
        public PolicyTemplateGenerationModel Output
        {
            get;
            set;
        }

    }
}
