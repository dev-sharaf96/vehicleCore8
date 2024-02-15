using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class LeasingUsersLocationsDeviceInfoMap : EntityTypeConfiguration<LeasingUsersLocationsDeviceInfo>
    {

        public LeasingUsersLocationsDeviceInfoMap()
        {
            ToTable("LeasingUsersLocationsDeviceInfo");
            HasKey(e => e.Id);
        }
    }
}
