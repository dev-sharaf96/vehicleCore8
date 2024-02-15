using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutUsersMap : EntityTypeConfiguration<CheckoutUsers>
    {
        public CheckoutUsersMap()
        {
            ToTable("CheckoutUsers");
            HasKey(c => c.Id);
        }
    }
}
