using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping.Quotations
{
    public class QuotationResponseMap : EntityTypeConfiguration<QuotationResponse>
    {
        public QuotationResponseMap() {
            ToTable("QuotationResponse");
            Property(qr => qr.ReferenceId).IsRequired().HasMaxLength(50);

            HasRequired(qr => qr.InsuranceCompany).WithMany().HasForeignKey(qr => qr.InsuranceCompanyId);
        }
    }
}