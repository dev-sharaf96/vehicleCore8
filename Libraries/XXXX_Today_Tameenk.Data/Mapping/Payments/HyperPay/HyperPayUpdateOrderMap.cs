using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Payments.HyperPay
{
    public class HyperPayUpdateOrderMap : EntityTypeConfiguration<HyperPayUpdateOrder>
    {
        public HyperPayUpdateOrderMap()
        {
            ToTable("HyperPayUpdateOrder");
            HasKey(e => e.Id);
        }
    }
}
