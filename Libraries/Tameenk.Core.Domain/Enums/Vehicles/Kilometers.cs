using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.kilometers
{
    public enum kilometers
    {
        /// <summary>
        /// Manual.
        /// </summary>
        [LocalizedName(typeof(TransmissionTypeResource), "kilometersFrom1To20")]
        kilometersFrom1To20 = 1,
        [LocalizedName(typeof(TransmissionTypeResource), "kilometersFrom20To40")]
        kilometersFrom20To40 = 2,
        [LocalizedName(typeof(TransmissionTypeResource), "kilometersMoreThan40")]
        kilometersMoreThan40 = 3
    }
}
