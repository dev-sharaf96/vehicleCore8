using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PaymentMap : EntityTypeConfiguration<Payment>
    {
        public PaymentMap()
        {
            ToTable("Payment");
            HasKey(e => e.BillNumber);
            Property(e => e.ReferenceID).HasMaxLength(50);
            Property(e => e.UserID).HasMaxLength(50);
            Property(e => e.IBNA).HasMaxLength(25);
        }
    }
}
