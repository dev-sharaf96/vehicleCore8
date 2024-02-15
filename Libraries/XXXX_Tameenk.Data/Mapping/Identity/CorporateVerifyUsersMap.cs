using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;

namespace Tameenk.Data.Mapping.Identity
{
    public class CorporateVerifyUsersMap : EntityTypeConfiguration<CorporateVerifyUsers>
    {
        public CorporateVerifyUsersMap()
        {
            ToTable("CorporateVerifyUsers");
            HasKey(e => e.Id);
        }
    }
}
