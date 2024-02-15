using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Quotations;

namespace Tameenk.Core.Domain.Enums.Quotations
{
    public enum MedicalCondition
    {
        /// <summary>
        /// No Restriction.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "NoRestriction")]
        NoRestriction = 1,
        /// <summary>
        /// Automatic Car.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "AutomaticCar")]
        AutomaticCar = 2,
        /// <summary>
        /// Prosthetic Limb.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "ProstheticLimb")]
        ProstheticLimb = 3,
        /// <summary>
        /// Vision Augmenting Lenses.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "VisionAugmentingLenses")]
        VisionAugmentingLenses = 4,
        /// <summary>
        /// Only Day Time.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "OnlyDayTime")]
        OnlyDayTime = 5,
        /// <summary>
        /// Hearing Aid
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "HearingAid")]
        HearingAid = 6,
        /// <summary>
        /// Driving Inside KSA Only.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "DrivingInsideKSAOnly")]
        DrivingInsideKSAOnly = 7,
        /// <summary>
        /// Handicap Car.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "HandicapCar")]
        HandicapCar = 8,
        /// <summary>
        /// For Private Use With No Payment.
        /// </summary>
        [LocalizedName(typeof(MedicalConditionResource), "ForPrivateUseWithNoPayment")]
        ForPrivateUseWithNoPayment = 9
    }
}
