using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum TransmissionType
    {
        /// <summary>
        /// Manual.
        /// </summary>
        [LocalizedName(typeof(TransmissionTypeResource), "Manual")]
        Manual = 1,
        /// <summary>
        /// Automatic.
        /// </summary>
        [LocalizedName(typeof(TransmissionTypeResource), "Automatic")]
        Automatic = 2
    }
}
