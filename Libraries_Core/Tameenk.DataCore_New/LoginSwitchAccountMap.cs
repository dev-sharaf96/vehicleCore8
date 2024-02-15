using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data
{
    public class LoginSwitchAccountMap : EntityTypeConfiguration<LoginSwitchAccount>
    {
        public LoginSwitchAccountMap()
        {
            ToTable("LoginSwitchAccount");
            HasKey(c => c.Id);
        }
    }
}
