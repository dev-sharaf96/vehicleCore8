using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums.Payments;

namespace Tameenk.Core.Domain.Entities.Payments
{
    /// <summary>
    /// Payment method
    /// </summary>
    public class PaymentMethod : BaseEntity
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The payment method name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is payment method active or not. 
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// The payment method code.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The payemtn method display order.
        /// </summary>
        public int Order { get; set; }

        public string Brands { get; set; }

        public bool? AndroidEnabled { get; set; }

        public bool? IosEnabled { get; set; }
        public string LogoUrl { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        /// <summary>
        /// The enumration of the payment method.
        /// </summary>
        public PaymentMethodCode PaymentMethodCode {
            get {
                return (PaymentMethodCode)Code;
            }
            set {
                Code = (int)value;
            }
        }
    }
}
