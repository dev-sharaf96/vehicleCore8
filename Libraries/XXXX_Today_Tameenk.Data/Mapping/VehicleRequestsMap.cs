using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class VehicleRequestsMap : EntityTypeConfiguration<VehicleRequests>
    {
        public VehicleRequestsMap()
        {
            ToTable("VehicleRequests");
            HasKey(c => c.ID);
        }
    }
}
