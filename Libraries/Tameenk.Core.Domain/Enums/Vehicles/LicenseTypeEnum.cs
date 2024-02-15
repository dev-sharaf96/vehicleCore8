using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum LicenseTypeEnum
    {
        [LocalizedName(typeof(LicenseTypeResource), "TEMPORARY")]
        TEMPORARY = 1,
        
        [LocalizedName(typeof(LicenseTypeResource), "MOTORCYCLE")]
        MOTORCYCLE = 2,

        [LocalizedName(typeof(LicenseTypeResource), "PRIVATE")]
        PRIVATE = 3,

        [LocalizedName(typeof(LicenseTypeResource), "PUBLICTAXI")]
        PUBLICTAXI = 4,

        [LocalizedName(typeof(LicenseTypeResource), "PICKUPPASSINGCARS")]
        PICKUPPASSINGCARS = 5,

        [LocalizedName(typeof(LicenseTypeResource), "LIGHTTRANSPORT")]
        LIGHTTRANSPORT = 6,

        [LocalizedName(typeof(LicenseTypeResource), "HEAVYTRANSPORT")]
        HEAVYTRANSPORT = 7,

        [LocalizedName(typeof(LicenseTypeResource), "PUBLICWORKSVEHICLES")]
        PUBLICWORKSVEHICLES = 8,

        [LocalizedName(typeof(LicenseTypeResource), "SMALLBUS")]
        SMALLBUS = 9,

        [LocalizedName(typeof(LicenseTypeResource), "LARGEBUS")]
        LARGEBUS = 10,

        [LocalizedName(typeof(LicenseTypeResource), "PUBLICPRIVATE")]
        PUBLICPRIVATE = 11
    }
}
