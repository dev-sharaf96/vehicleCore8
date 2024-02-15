using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum ParkingLocation
    {
        /// <summary>
        /// Road-Side
        /// </summary>
        [LocalizedName(typeof(ParkingLocationResource), "RoadSide")]
        RoadSide = 1,
        /// <summary>
        /// Drive-Way
        /// </summary>
        [LocalizedName(typeof(ParkingLocationResource), "DriveWay")]
        DriveWay = 2,
        /// <summary>
        /// Garaged
        /// </summary>
        [LocalizedName(typeof(ParkingLocationResource), "Garaged")]
        Garaged = 3
    }
}
