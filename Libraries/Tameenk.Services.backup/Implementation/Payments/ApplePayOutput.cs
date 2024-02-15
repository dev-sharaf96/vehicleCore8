using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Integration.Dto;
using Tameenk.Integration.Dto.Payment;

namespace Tameenk.Services
{
    public class ApplePayOutput
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
            invalidDomain,
            InvalidMerchantIdentifier,
            InvoiceIsNull,
            TotalCompanyAmountIsLessZero,
            InsuranceCompanyIdIsNull,
            BCareBankAccountIsNull,
            CompanyBankAccountIsNull,
            CheckoutIsNull,
            ResponseIsNull,
            InvalidAmount,
            InvalidBrand,
            InvalidPolicyStatus,
            IsCancelled,
            InvalidPayment,
            FailedToUpdateCheckoutStatus,
            HashedNotMatched,
            EmptyInputParamter,
            checkoutDetailIsNull,
            PaidBefore,
            ServiceDown,
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
        public string ReferenceId
        {
            get;
            set;
        }
        public ApplePaySessionResponseModel Result
        {
            get;
            set;
        }
        public ApplePayPaymnetResponseModel PaymnetResponseModel
        {
            get;
            set;
        }
        
       public bool PaymentSucceded
        {
            get;
            set;
        }
        public int PaymentMethodId
        {
            get;
            set;
        }
        public string HyperpayResponseCode { get; set; }
    }
}
