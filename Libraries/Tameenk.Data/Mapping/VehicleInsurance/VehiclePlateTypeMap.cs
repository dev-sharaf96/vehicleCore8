using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehiclePlateTypeMap : EntityTypeConfiguration<VehiclePlateType>
    {
        public VehiclePlateTypeMap() {
            ToTable("VehiclePlateType");
            HasKey(vp => vp.Code);
            Property(vp => vp.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(vp => vp.EnglishDescription).HasMaxLength(200);
            Property(vp => vp.ArabicDescription).HasMaxLength(200);

            HasMany(e => e.Vehicles)
                .WithRequired(e => e.VehiclePlateType)
                .HasForeignKey(e => e.PlateTypeCode)
                .WillCascadeOnDelete(false);
        }
    }
}