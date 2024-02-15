using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    /// <summary>
    /// The education level.
    /// </summary>
    public enum Education
    {
        /// <summary>
        /// No education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "None")]
        None = 1,
        /// <summary>
        /// Primary education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "Primary")]
        Primary = 2,
        /// <summary>
        /// Secondary education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "Secondary")]
        Secondary = 3,
        /// <summary>
        /// Academic education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "Academic")]
        Academic = 4,
        /// <summary>
        /// High education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "HighEducation")]
        HighEducation = 5
    }
}
