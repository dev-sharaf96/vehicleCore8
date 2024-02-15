using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PayfortPaymentResponseMap : EntityTypeConfiguration<PayfortPaymentResponse>
    {
        public PayfortPaymentResponseMap()
        {
            ToTable("PayfortPaymentResponse");
            Property(e => e.ResponseMessage).HasMaxLength(200);
            Property(e => e.PaymentOption).HasMaxLength(20);
            Property(e => e.CardNumber).HasMaxLength(20);
            Property(e => e.CardHolerName).HasMaxLength(50);
            Property(e => e.CardExpiryDate).HasMaxLength(5);
            Property(e => e.CustomerIP).HasMaxLength(50);
            Property(e => e.FortId).HasMaxLength(25);

            Property(e => e.Amount)
                .HasPrecision(10, 4);
        }
    }
}
