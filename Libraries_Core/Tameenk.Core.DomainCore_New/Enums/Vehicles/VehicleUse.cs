using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum VehicleUse
    {
       
        /// <summary>
        /// Private use.
        /// </summary>
        [LocalizedName(typeof(VehicleUseResource), "Private")]
        Private = 1,
        /// <summary>
        /// Commercial use.
        /// </summary>
        [LocalizedName(typeof(VehicleUseResource), "Commercial")]
        Commercial = 2
    }
}
