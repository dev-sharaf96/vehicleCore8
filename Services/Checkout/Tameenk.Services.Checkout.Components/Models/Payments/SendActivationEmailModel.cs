using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Payments;

namespace Tameenk.Models.Payments
{
    public class SendActivationEmailModel
    {
        public string CheckoutEmail { get; set; }
        public string ReferenceId { get; set; }
    }
}