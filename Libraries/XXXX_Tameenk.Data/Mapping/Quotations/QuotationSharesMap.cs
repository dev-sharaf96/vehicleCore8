using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Data.Mapping
{
    public class QuotationSharesMap : EntityTypeConfiguration<QuotationShares>
    {
        public QuotationSharesMap() {
            ToTable("QuotationShares");
            HasKey(c => c.Id);
        }
    }
}