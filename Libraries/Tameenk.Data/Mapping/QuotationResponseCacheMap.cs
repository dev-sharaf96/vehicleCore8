using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class QuotationResponseCacheMap : EntityTypeConfiguration<QuotationResponseCache>
    {
        public QuotationResponseCacheMap()
        {
            ToTable("QuotationResponseCache");
            HasKey(c => c.ID);
        }
    }
}
