using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Data.Mapping
{
    public class OccupationMap : EntityTypeConfiguration<Occupation>
    {
        public OccupationMap()
        {
            ToTable("Occupation");
            HasKey(e => e.ID);
            Property(e => e.NameAr).HasMaxLength(200);
            Property(e => e.NameEn).HasMaxLength(200);
            Property(e => e.Code).HasMaxLength(50);

            HasMany<Insured>(g => g.Insureds).WithOptional(s => s.Occupation).HasForeignKey<int?>(s => s.OccupationId);
            HasMany<Driver>(g => g.Drivers).WithOptional(s => s.Occupation).HasForeignKey<int?>(s => s.OccupationId);

        }
    }
}
