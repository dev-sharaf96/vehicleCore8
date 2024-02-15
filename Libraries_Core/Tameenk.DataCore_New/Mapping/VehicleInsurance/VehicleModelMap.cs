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
    public class VehicleModelMap : EntityTypeConfiguration<VehicleModel>
    {
        public VehicleModelMap()
        {
            ToTable("VehicleModel");
            HasKey(e => new { e.Code, e.VehicleMakerCode });
            Property(e => e.Code).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.VehicleMakerCode).HasColumnOrder(1).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.EnglishDescription).HasMaxLength(50);
            Property(e => e.ArabicDescription).HasMaxLength(50);
        }
    }
}
