using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data
{
    class WareefMap : EntityTypeConfiguration<Wareef>
    {
        public WareefMap()
        {
            ToTable("Wareef");
            HasKey(e => e.Id);
        }
    }
}
