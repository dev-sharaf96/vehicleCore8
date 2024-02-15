using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    /// <summary>
    /// The national id card type.
    /// </summary>
    public enum CardIdType
    {
        /// <summary>
        /// The national id card for citizen.
        /// </summary>
        [LocalizedName(typeof(CardIdTypeResource), "Citizen")]
        Citizen = 1,

        /// <summary>
        /// The national id card for resident.
        /// </summary>
        [LocalizedName(typeof(CardIdTypeResource), "Resident")]
        Resident = 2
    }
}
