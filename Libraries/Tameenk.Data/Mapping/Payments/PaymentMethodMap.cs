using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments;

namespace Tameenk.Data.Mapping.Payments
{
    public class PaymentMethodMap : EntityTypeConfiguration<PaymentMethod>
    {
        public PaymentMethodMap()
        {
            ToTable("PaymentMethod");
            HasKey(e => e.Id);
            Ignore(e => e.PaymentMethodCode);
        }
    }
}
