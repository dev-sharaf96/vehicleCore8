using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Tabby;

namespace Tameenk.Data.Mapping.Payments.Tabby
{
    class TabbyWebHookDetailMap : EntityTypeConfiguration<TabbyWebHookDetails>
    {
        public TabbyWebHookDetailMap()
        {
            ToTable("TabbyWebHookDetails");
            HasKey(e => e.Id);
        }
    }
}
