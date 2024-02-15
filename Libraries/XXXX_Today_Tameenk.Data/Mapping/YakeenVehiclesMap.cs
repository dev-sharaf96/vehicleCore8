using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class YakeenVehiclesMap : EntityTypeConfiguration<YakeenVehicles>
    {
        public YakeenVehiclesMap()
        {
            ToTable("YakeenVehicles");
            HasKey(c => c.ID);
        }
    }
}
