using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ReasonCodeMap : EntityTypeConfiguration<ReasonCode>
    {
        public ReasonCodeMap()
        {
            ToTable("ReasonCode");
            HasKey(e => e.Id);
            
        }
    }
}
