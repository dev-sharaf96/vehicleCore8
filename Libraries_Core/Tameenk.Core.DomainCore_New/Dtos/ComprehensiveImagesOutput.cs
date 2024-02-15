using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tameenk.Core.Domain.Dtos
{
    public class ComprehensiveImagesOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullInput,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
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
        public string ReferenceId
        {
            get;
            set;
        }
        public string StatusCode
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
