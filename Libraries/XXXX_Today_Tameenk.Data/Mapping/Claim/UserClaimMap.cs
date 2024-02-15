using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class UserClaimMap : EntityTypeConfiguration<UserClaim>
    {
        public UserClaimMap()
        {
            ToTable("UserClaim");
            HasKey(c => c.Id);
        }
    }
}
