using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entitie;

namespace Tameenk.Data.Mapping.Autoleasing
{
    class RenwalStatitics_OldMap : EntityTypeConfiguration<RenwalStatitics_Old>
    {
        public RenwalStatitics_OldMap()
        {
            ToTable("RenwalStatitics_Old");
            HasKey(c => c.Id);
        }
    }
}
