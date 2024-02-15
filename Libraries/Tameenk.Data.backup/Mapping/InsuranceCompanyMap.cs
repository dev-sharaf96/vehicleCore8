using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InsuranceCompanyMap : EntityTypeConfiguration<InsuranceCompany>
    {
        public InsuranceCompanyMap()
        {
            ToTable("InsuranceCompany");

            HasKey(e => e.InsuranceCompanyID);
            Property(e => e.InsuranceCompanyID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.NameAR).IsRequired().HasMaxLength(50);
            Property(e => e.NameEN).IsRequired().HasMaxLength(50);
            Property(e => e.Key).IsRequired().HasMaxLength(50);
            Property(e => e.DescAR).HasMaxLength(1000);
            Property(e => e.DescEN).HasMaxLength(1000);
            
            HasMany(e => e.Deductibles)
                .WithRequired(e => e.InsuranceCompany)
                .WillCascadeOnDelete(false);

            HasMany(e => e.InsuaranceCompanyBenefits)
                            .WithRequired(e => e.InsuranceCompany)
                            .HasForeignKey(e => e.InsurnaceCompanyID)
                            .WillCascadeOnDelete(false);

            HasMany(e => e.Products)
                .WithOptional(e => e.InsuranceCompany)
                .HasForeignKey(e => e.ProviderId);

            HasOptional(e => e.Address).WithMany(e => e.InsuranceCompanies).HasForeignKey(e => e.AddressId);
            HasOptional(e => e.Contact).WithMany(e => e.InsuranceCompanies).HasForeignKey(e => e.ContactId);

        }
    }
}
