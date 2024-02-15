using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutInsuredMappingTempMap : EntityTypeConfiguration<CheckoutInsuredMappingTemp>
    {
        public CheckoutInsuredMappingTempMap()
        {
            ToTable("CheckoutInsuredMappingTemp");
            HasKey(e => e.Id);
        }
    }
}
