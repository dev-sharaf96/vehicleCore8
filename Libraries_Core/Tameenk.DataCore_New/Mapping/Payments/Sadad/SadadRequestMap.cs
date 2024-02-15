using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Data.Mapping.Payments.Sadad
{
    public class SadadRequestMap : EntityTypeConfiguration<SadadRequest>
    {
        public SadadRequestMap()
        {
            ToTable("SadadRequest");
            HasKey(e => e.Id);
            Property(e => e.CustomerAccountNumber).IsRequired().HasMaxLength(20);
            HasIndex(e => e.CustomerAccountNumber).IsUnique();
            Property(e => e.CustomerAccountName).IsRequired().HasMaxLength(200);

            Property(e => e.BillAmount)
                .HasPrecision(6, 2);
            Property(e => e.BillMaxAdvanceAmount)
                            .HasPrecision(6, 2);
            Property(e => e.BillMinAdvanceAmount)
                            .HasPrecision(6, 2);
            Property(e => e.BillMinPartialAmount)
                            .HasPrecision(6, 2);
            HasMany(e => e.SadadResponses)
                            .WithRequired(e => e.SadadRequest)
                            .WillCascadeOnDelete(false);

        }
    }
}
