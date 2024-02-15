using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class BenefitMap : EntityTypeConfiguration<Benefit>
    {
        public BenefitMap()
        {
            ToTable("Benefit");
            HasKey(e => e.Code);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);

            HasMany(e => e.InsuaranceCompanyBenefits)
                .WithRequired(e => e.Benefit)
                .HasForeignKey(e => e.BenifitCode)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Invoice_Benefits)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);

            HasMany(e => e.Product_Benefits)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);

            HasMany(e => e.Quotation_Product_Benefits)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);
        }
    }
}
