using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
//using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Models.Checkout;
using Tameenk.Models.Payments.Sadad;
using Tameenk.Payment.Esal.Component;
using Tameenk.Payment.Esal.Component.Model;
using Tameenk.Services.Core.Payments;

namespace Tameenk.Services.Checkout.Components.Output
{
    public class HyperPayOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceDown,
            InvalidAmount,
            CheckoutIsNull,
            IsCancelled,
            InvalidPolicyStatus,
            FailedToUpdateCheckoutStatus,
            InvalidPayment,
            InvalidBrand,
            InvalidLogin,
            BcareBankAccountIsNull,
            CompanyBankAccountIsNull,
            FailedToUpdateBCareAmount,
            FailedToUpdateCompanyAmount,
            HyperpayRequestIsNull,
            InvoiceIsNull,
            HyperpayToValidResponseServiceException,
            ServiceException,
            InvalidValue,
            MerchantTransactionIdIsNullOrEmpty
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        public int? PaymentId { get;  set; }
        public bool? PaymentSucceded { get; internal set; }
        public int PaymentMethodId { get; internal set; }
        public CheckoutDetail CheckoutDetail { get; internal set; }
        //public HyperpayResponse HyperpayResponse { get; internal set; }
        public string HyperpayResponseCode { get; set; }
    }
}
