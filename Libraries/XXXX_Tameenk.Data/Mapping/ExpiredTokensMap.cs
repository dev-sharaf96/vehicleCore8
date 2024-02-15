using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ExpiredTokensMap : EntityTypeConfiguration<ExpiredTokens>
    {
        public ExpiredTokensMap() {
            ToTable("ExpiredTokens");
            HasKey(c => c.Id);
          
        }
    }
}