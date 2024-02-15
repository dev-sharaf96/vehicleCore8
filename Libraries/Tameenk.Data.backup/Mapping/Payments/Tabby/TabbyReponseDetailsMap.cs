using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Tabby;

namespace Tameenk.Data.Mapping.Payments.Tabby
{
    class TabbyReponseDetailsMap : EntityTypeConfiguration<TabbyResponseDetail>
    {
        public TabbyReponseDetailsMap()
        {
            ToTable("TabbyResponseDetails");
            HasKey(e => e.Id);
        }
    }
}
