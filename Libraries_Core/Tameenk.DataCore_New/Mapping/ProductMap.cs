using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {

        public ProductMap()
        {
            ToTable("Product");
            HasKey(e => e.Id);
            Property(p => p.ProductPrice).HasPrecision(19, 4);
            Property(p => p.ExternalProductId).HasMaxLength(100);
            Property(p => p.QuotaionNo).IsRequired().HasMaxLength(50);
            Property(p => p.ProductImage).HasMaxLength(250);

            Property(p => p.ProductImage).IsUnicode(false);
            HasRequired(p => p.QuotationResponse).WithMany(qr => qr.Products).HasForeignKey(p => p.QuotationResponseId);
            HasMany(e => e.PriceDetails).WithRequired(e => e.Product).WillCascadeOnDelete(false);

            HasOptional(e => e.InsuranceCompany).WithMany(e => e.Products).HasForeignKey(e => e.ProviderId);
            
        }
    }
}