using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleUsageMap : EntityTypeConfiguration<VehicleUsage>
    {
        public VehicleUsageMap()
        {
            ToTable("VehicleUsage");
            HasKey(a => a.Id);
        }
    }
}
