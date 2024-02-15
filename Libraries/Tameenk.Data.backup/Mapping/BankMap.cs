using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class BankMap : EntityTypeConfiguration<Bank>
    {
        public BankMap()
        {
            ToTable("Bank");
            HasKey(e => e.Id);

            Property(e => e.NameAr).HasMaxLength(500);
            Property(e => e.NameEn).HasMaxLength(500);
            
        }
    }
}
