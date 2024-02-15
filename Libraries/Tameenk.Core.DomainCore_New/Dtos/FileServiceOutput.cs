using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tameenk.Core.Domain.Dtos
{
    public class FileServiceOutput
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
            policyNoIsNull,
            ReferenceIdIsNull,
            SchedulePolicyReturnError,
            ServiceException,
            SchedulePolicyDeserializeError
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
        public string PolicyFileUrl
        {
            get;
            set;
        }
        public byte[] PolicyFile
        {
            get;
            set;
        }
        public bool IsSchedule
        {
            get;
            set;
        }
        public string ServiceUrl
        {
            get;
            set;
        }
        public string ServiceResponse
        {
            get;
            set;
        }
        public double ServiceResponseTimeInSeconds
        {
            get;
            set;
        }
    }
}
