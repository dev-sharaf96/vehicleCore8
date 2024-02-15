using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutAdditionalDriverMap : EntityTypeConfiguration<CheckoutAdditionalDriver>
    {
        public CheckoutAdditionalDriverMap()
        {
            Property(e => e.CheckoutDetailsId).IsRequired().HasMaxLength(50);
        }
    }
}
