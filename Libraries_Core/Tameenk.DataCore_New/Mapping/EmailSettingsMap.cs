using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class EmailSettingsMap : EntityTypeConfiguration<EmailSettings>
    {
        public EmailSettingsMap()
        {
            ToTable("EmailSettings");
            HasKey(e => e.Id);
        }
    }
}
