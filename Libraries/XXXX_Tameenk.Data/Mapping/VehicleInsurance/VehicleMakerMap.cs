using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping.VehicleInsurance
{
    public class VehicleMakerMap : EntityTypeConfiguration<VehicleMaker>
    {
        public VehicleMakerMap()
        {
            ToTable("VehicleMaker");
            HasKey(vm => vm.Code);
            Property(vm => vm.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(vm => vm.EnglishDescription).HasMaxLength(50);
            Property(vm => vm.ArabicDescription).HasMaxLength(50);

            HasMany(e => e.VehicleModels)
                .WithRequired(e => e.VehicleMaker)
                .WillCascadeOnDelete(false);    
        }
    }
}