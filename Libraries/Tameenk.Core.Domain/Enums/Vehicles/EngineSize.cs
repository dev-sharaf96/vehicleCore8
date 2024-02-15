using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum EngineSize
    {
        /// <summary>
        /// Not available
        /// </summary>
        [LocalizedName(typeof(EngineSizeResource), "NotAvailable")]
        NotAvailable = 0,
        /// <summary>
        /// Up To 2,000 CC
        /// </summary>
        [LocalizedName(typeof(EngineSizeResource), "UpTo2K")]
        UpTo2K = 1,
        /// <summary>
        /// 2000 CC to 4,000 CC
        /// </summary>
        [LocalizedName(typeof(EngineSizeResource), "Between2KAnd4K")]
        Between2KAnd4K = 2,
        /// <summary>
        /// Above 4,000 CC
        /// </summary>
        [LocalizedName(typeof(EngineSizeResource), "Above4K")]
        Above4K = 3
    }
}
