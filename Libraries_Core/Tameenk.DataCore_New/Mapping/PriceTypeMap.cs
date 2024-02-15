using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PriceTypeMap : EntityTypeConfiguration<PriceType>
    {
        public PriceTypeMap()
        {
            ToTable("PriceType");
            HasKey(e => e.Code);
            Property(e => e.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);
            HasMany(e => e.PriceDetails).WithRequired(e => e.PriceType).WillCascadeOnDelete(false);

            HasMany(e => e.PriceDetails)
                .WithRequired(e => e.PriceType)
                .WillCascadeOnDelete(false);

        }
    }
}