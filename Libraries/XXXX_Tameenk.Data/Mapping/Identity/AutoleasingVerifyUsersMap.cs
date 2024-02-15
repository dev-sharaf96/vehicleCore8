using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Identity;

namespace Tameenk.Data.Mapping.Identity
{
    public class AutoleasingVerifyUsersMap : EntityTypeConfiguration<AutoleasingVerifyUsers>
    {

        public AutoleasingVerifyUsersMap()
        {
            ToTable("AutoleasingVerifyUsers");
            HasKey(e => e.Id);
        }
    }
}
