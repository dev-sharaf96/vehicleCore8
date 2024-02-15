using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class DriverTypeMap : EntityTypeConfiguration<DriverType>
    {
        public DriverTypeMap()
        {
            ToTable("DriverType");
            HasKey(e => e.Code);
            Property(e => e.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);
        }
    }
}
