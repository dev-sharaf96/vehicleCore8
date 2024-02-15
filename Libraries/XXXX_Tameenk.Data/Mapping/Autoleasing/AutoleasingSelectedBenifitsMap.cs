using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class AutoleasingSelectedBenifitsMap : EntityTypeConfiguration<AutoleasingSelectedBenifits>
    {
        public AutoleasingSelectedBenifitsMap()
        {
            ToTable("AutoleasingSelectedBenifits");
            HasKey(c => c.Id);
        }
    }
}
