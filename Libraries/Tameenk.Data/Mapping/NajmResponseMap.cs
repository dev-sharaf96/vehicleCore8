using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class NajmResponseMap : EntityTypeConfiguration<NajmResponseEntity>
    {
        public NajmResponseMap() {
            ToTable("NajmResponse");
            HasKey(c => c.Id);
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.PolicyHolderNin).HasMaxLength(20);
            Property(c => c.VehicleId).HasMaxLength(30);
            Property(c => c.NCDReference).HasMaxLength(50);
        }
    }
}