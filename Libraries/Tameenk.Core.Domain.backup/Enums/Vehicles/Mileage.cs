using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum Mileage
    {
        /// <summary>
        /// <summary>
        /// Manual.
        /// </summary>
        [LocalizedName(typeof(MileageResource), "kilometersFrom1To20")]
        kilometersFrom1To20 = 1,
        [LocalizedName(typeof(MileageResource), "kilometersFrom20To40")]
        kilometersFrom20To40 = 2,
        [LocalizedName(typeof(MileageResource), "kilometersMoreThan40")]
        kilometersMoreThan40 = 3
    }
}
