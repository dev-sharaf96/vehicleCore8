using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InvoiceMap : EntityTypeConfiguration<Invoice>
    {
        public InvoiceMap()
        {
            ToTable("Invoice");
            HasKey(e => e.Id);

            HasIndex(e => e.InvoiceNo).IsUnique();
            Property(e => e.UserId).IsRequired().HasMaxLength(128);
            Property(e => e.ReferenceId).HasMaxLength(200);

            Property(e => e.ProductPrice)
               .HasPrecision(8, 2);

            Property(e => e.Fees)
                .HasPrecision(8, 2);

            Property(e => e.Vat)
                .HasPrecision(8, 2);

            Property(e => e.SubTotalPrice)
                .HasPrecision(8, 2);

            Property(e => e.TotalPrice)
                .HasPrecision(8, 2);

            HasOptional(e => e.InvoiceFile)
                .WithRequired(e => e.Invoice);
            HasOptional(e => e.InsuranceCompany).WithMany(e => e.Invoices).HasForeignKey(e => e.InsuranceCompanyId);
        }
    }
}
