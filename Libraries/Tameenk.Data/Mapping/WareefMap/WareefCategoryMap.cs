using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data
{
    class WareefCategoryMap : EntityTypeConfiguration<WareefCategory>
    {
        public WareefCategoryMap()
        {
            ToTable("WareefCategory");
            HasKey(e => e.Id);
        }
    }
}
