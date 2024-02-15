using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PayfortPaymentRequestMap : EntityTypeConfiguration<PayfortPaymentRequest>
    {
        public PayfortPaymentRequestMap()
        {
            ToTable("PayfortPaymentRequest");
            HasKey(e => e.ID);
            Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.UserId).IsRequired().HasMaxLength(128);
            HasIndex(e => e.ReferenceNumber).IsUnique();
            Property(e => e.ReferenceNumber).HasMaxLength(20);

            Property(e => e.Amount)
                .HasPrecision(10, 4);

            HasMany(e => e.PayfortPaymentResponses)
                .WithRequired(e => e.PayfortPaymentRequest)
                .HasForeignKey(e => e.RequestId)
                .WillCascadeOnDelete(false);
        }
    }
}
