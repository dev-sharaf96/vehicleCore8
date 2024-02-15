using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class WataniyaDraftPolicyMap : EntityTypeConfiguration<WataniyaDraftPolicy>
    {
        public WataniyaDraftPolicyMap()
        {
            ToTable("WataniyaDraftPolicy");
            HasKey(e => e.Id);
        }
    }
}
