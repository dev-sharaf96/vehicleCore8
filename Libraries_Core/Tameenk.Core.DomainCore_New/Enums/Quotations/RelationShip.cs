using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Inquiry;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    public enum RelationShip
    {
        /// <summary>
        /// No relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "None")]
        None = 0,

        /// <summary>
        /// Father / Mother relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "FatherMother")]
        FatherMother = 1,

        /// <summary>
        /// Husband / Wife relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "HusbandWife")]
        HusbandWife = 2,

        /// <summary>
        /// Son / Daughter relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "SonDaughter")]
        SonDaughter = 3,

        /// <summary>
        /// Brother / Sister relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "BrotherSister")]
        BrotherSister = 4,

        /// <summary>
        /// Empoyee relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "Empoyee")]
        Empoyee = 5,

        /// <summary>
        /// Sponsored relationship.
        /// </summary>
        [LocalizedName(typeof(RelationShipResource), "Sponsored")]
        Sponsored = 6,
    }
}
