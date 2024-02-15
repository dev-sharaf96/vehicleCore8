using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutDriverInfoMap : EntityTypeConfiguration<CheckoutDriverInfo>
    {
        public CheckoutDriverInfoMap()
        {
            ToTable("CheckoutDriverInfo");
            HasKey(c => c.ID);
        }
    }
}
