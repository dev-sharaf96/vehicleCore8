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
    public enum EducationAr
    {
        /// <summary>
        /// No education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "غير متعلم")]
        None = 1,
        /// <summary>
        /// Primary education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "تعليم ابتدائي")]
        Primary = 2,
        /// <summary>
        /// Secondary education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "تعليم ثانوي")]
        Secondary = 3,
        /// <summary>
        /// Academic education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "تعليم جامعي")]
        Academic = 4,
        /// <summary>
        /// High education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "دراسات عليا")]
        HighEducation = 5,
        /// <summary>
        /// Phd education.
        /// </summary>
        [LocalizedName(typeof(EducationResource), "دكتوراه")]
        PhD = 6
    }
}
