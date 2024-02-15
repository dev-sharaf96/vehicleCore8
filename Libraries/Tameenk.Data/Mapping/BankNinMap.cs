using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class BankNinMap : EntityTypeConfiguration<BankNins>
    {
        public BankNinMap()
        {
            ToTable("BankNins");
            HasKey(e => e.Id);

            Property(e => e.NIN).HasMaxLength(500);
            Property(e => e.BankId);
        }
    }
}
