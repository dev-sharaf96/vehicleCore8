using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleIDTypeMap : EntityTypeConfiguration<VehicleIDType>
    {
        public VehicleIDTypeMap()
        {
            ToTable("VehicleIDType");
            HasKey(e => e.Code);
            Property(e => e.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);
        }
    }
}
