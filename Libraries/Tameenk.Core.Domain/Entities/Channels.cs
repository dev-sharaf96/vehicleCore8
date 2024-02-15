using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    /// <summary>
    /// Channel
    /// </summary>
    public class Channels : BaseEntity
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
        /// The payment method code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The enumration of the payment method.
        /// </summary>
        public Tameenk.Common.Utilities.Channel ChannelCode
        {
            get {
                return (Tameenk.Common.Utilities.Channel)Code;
            }
            set {
                Code = (int)value;
            }
        }
    }
}
