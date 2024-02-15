using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Integration.Dto;
using Tameenk.Integration.Dto.Payment;
using Tameenk.Integration.Dto.Payment.CreateOrder;

namespace Tameenk.Services
{
    public class HyperpayCreateOrderOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            EmptyResponse,
            UnspecifiedError,
            ServiceError,
            NullRequest,
            MissingInput,
            StatusFailed,
            ServiceException,
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
        public CreateOrderResponseModel ResponseModel
        {
            get;
            set;
        }
    }
}
