using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class NCDFreeYearMap : EntityTypeConfiguration<NCDFreeYear>
    {
        public NCDFreeYearMap()
        {
            ToTable("NCDFreeYear");
            HasKey(e => e.Code);
            Property(e => e.EnglishDescription).HasMaxLength(200);
            Property(e => e.ArabicDescription).HasMaxLength(200);
        }
    }
}
