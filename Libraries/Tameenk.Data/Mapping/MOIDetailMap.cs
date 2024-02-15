using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class MOIDetailMap : EntityTypeConfiguration<MOIDetail>
    {
        public MOIDetailMap() {
            ToTable("MOIDetail");
            HasKey(c => c.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}