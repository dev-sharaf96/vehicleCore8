using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Payments;

namespace Tameenk.Models.Payments
{
    public class PaymentMethodModel
    {
        public List<PaymentMethodCode> PaymentMethodCodes { get; set; }
    }
}