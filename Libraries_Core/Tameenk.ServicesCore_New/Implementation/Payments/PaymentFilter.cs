using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments
{
    /// <summary>
    /// Payment Filter
    /// </summary>
    public class PaymentFilter
    {
        public string ReferenceId { get; set; }


        /// <summary>
        /// Payment method Id
        /// </summary>
        public int? PaymentMethodId { get; set; }
        public string InvoiceNumber { get; set; }
        public string MerchantId { get; set; }
    }
}
