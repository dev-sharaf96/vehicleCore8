using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.WareefMap
{
   public class WareefDescountsMap : EntityTypeConfiguration<WareefDiscounts>
    {
            public WareefDescountsMap()
            {
                ToTable("WareefDescounts");
                HasKey(e => e.Id);
            }
    }
}
