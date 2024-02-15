using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Identity;

namespace Tameenk.Data.Mapping.Identity
{
    public class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            ToTable("Clients");
            HasKey(e => e.Id);
            Property(e => e.Secret).IsRequired();
            Property(e => e.Name).IsRequired().HasMaxLength(100);
            Property(e => e.AllowedOrigin).HasMaxLength(100);
            Property(e => e.AuthServerUrl).HasMaxLength(100);
            Property(e => e.RedirectUrl).HasMaxLength(100);
        }
    }
}