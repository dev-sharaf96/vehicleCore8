using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ProductTypeMap : EntityTypeConfiguration<ProductType>
    {
        public ProductTypeMap()
        {
            ToTable("ProductType");
            HasKey(p => p.Code);
            Property(p => p.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasMany(p => p.QuotationResponses).WithOptional(p => p.ProductType).HasForeignKey(p => p.InsuranceTypeCode);
            Property(p => p.EnglishDescription).HasMaxLength(200);
            Property(p => p.ArabicDescription).HasMaxLength(200);
            HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.ProductType)
                .HasForeignKey(e => e.SelectedInsuranceTypeCode);

            HasMany(e => e.InsuranceCompanyProductTypeConfigs)
                .WithRequired(e => e.ProductType)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Invoices)
                .WithOptional(e => e.ProductType)
                .HasForeignKey(e => e.InsuranceTypeCode);


            
        }
    }
}