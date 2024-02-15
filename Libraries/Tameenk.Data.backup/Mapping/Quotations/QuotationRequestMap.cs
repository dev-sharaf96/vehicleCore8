using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping.Quotations
{
    public class QuotationRequestMap : EntityTypeConfiguration<QuotationRequest>
    {
        public QuotationRequestMap()
        {
            ToTable("QuotationRequest");

            Property(p => p.ExternalId).IsRequired().HasMaxLength(50);
            Property(p => p.UserId).HasMaxLength(128);
            Property(p => p.NajmNcdRefrence).HasMaxLength(128);
            HasRequired(p => p.Driver).WithMany(d => d.QuotationRequests).HasForeignKey(q => q.MainDriverId);
            HasMany(e => e.QuotationResponses)
                .WithOptional(e => e.QuotationRequest)
                .HasForeignKey(e => e.RequestId);
            HasRequired(e => e.Insured).WithMany().HasForeignKey(e => e.InsuredId);
        }
    }
}