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
    public class WalletPaymentOutput
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
            InvoiceIsNull,
            ServiceException,
           
        }

        public string ErrorDescription { get; set; }

        public ErrorCodes ErrorCode { get; set; }
        
    }
}
