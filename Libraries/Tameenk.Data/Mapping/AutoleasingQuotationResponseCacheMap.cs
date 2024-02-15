using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AutoleasingQuotationResponseCacheMap : EntityTypeConfiguration<AutoleasingQuotationResponseCache>
    {
        public AutoleasingQuotationResponseCacheMap()
        {
            ToTable("AutoleasingQuotationResponseCache");
            HasKey(c => c.ID);
        }
    }
}
