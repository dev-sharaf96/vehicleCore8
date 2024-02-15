using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AutoleasingBenefitMap : EntityTypeConfiguration<AutoleasingBenefit>
    {
        public AutoleasingBenefitMap()
        {
            ToTable("AutoleasingBenefit");
            HasKey(e => e.Id);
            Property(e => e.Code);
            Property(e => e.ArabicDescription).HasMaxLength(500);
            Property(e => e.EnglishDescription).HasMaxLength(500);

        }
    }
}
