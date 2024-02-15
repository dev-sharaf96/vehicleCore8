using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    public enum SocialStatus
    {
        [LocalizedName(typeof(SocialStatusResource), "NotAvailable")]
        NotAvailable = 0,

        [LocalizedName(typeof(SocialStatusResource), "SingleMale")]
        SingleMale = 1,

        [LocalizedName(typeof(SocialStatusResource), "MarriedMale")]
        MarriedMale = 2,

        [LocalizedName(typeof(SocialStatusResource), "SingleFemale")]
        SingleFemale = 3,

        [LocalizedName(typeof(SocialStatusResource), "MarriedFemale")]
        MarriedFemale = 4,

        [LocalizedName(typeof(SocialStatusResource), "DivorcedFemale")]
        DivorcedFemale = 5,


        [LocalizedName(typeof(SocialStatusResource), "WidowedFemale")]
        WidowedFemale = 6,


        [LocalizedName(typeof(SocialStatusResource), "Other")]
        Other = 7

    }
}
