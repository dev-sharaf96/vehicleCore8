using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class YakeenDriversMap : EntityTypeConfiguration<YakeenDrivers>
    {
        public YakeenDriversMap()
        {
            ToTable("YakeenDrivers");
            HasKey(c => c.DriverId);
        }
    }
}
