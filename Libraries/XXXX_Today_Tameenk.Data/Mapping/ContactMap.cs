using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            ToTable("Contact");
            HasKey(e => e.Id);

            Property(e => e.MobileNumber).HasMaxLength(50);
            Property(e => e.HomePhone).HasMaxLength(50);
            Property(e => e.Fax).HasMaxLength(50);
            Property(e => e.Email).HasMaxLength(50);
        }
    }
}
