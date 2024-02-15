using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    /// <summary>
    /// Represent the enumaration for gender.
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// The gender is not available.
        /// </summary>
        [Code("N")]
        [LocalizedName(typeof(GenderResource), "NotAvailable")]
        NotAvailable = 0,
        /// <summary>
        /// Male gender.
        /// </summary>
        [Code("M")]
        [LocalizedName(typeof(GenderResource), "Male")]
        Male = 1,
        /// <summary>
        /// Female gender.
        /// </summary>
        [Code("F")]
        [LocalizedName(typeof(GenderResource), "Female")]
        Female = 2
    }
}
