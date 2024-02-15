using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum BrakingSystem
    {
        [LocalizedName(typeof(BrakingSystemResource), "Standard")]
        Standard = 1,
        [LocalizedName(typeof(BrakingSystemResource), "AntiLock")]
        AntiLock = 2,
        [LocalizedName(typeof(BrakingSystemResource), "AutoBraking")]
        AutoBraking = 3
    }
}
