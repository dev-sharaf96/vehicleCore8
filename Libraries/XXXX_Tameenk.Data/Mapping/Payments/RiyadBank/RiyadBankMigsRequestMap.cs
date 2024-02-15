using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;

namespace Tameenk.Data.Mapping.Payments.RiyadBank
{
    public class RiyadBankMigsRequestMap : EntityTypeConfiguration<RiyadBankMigsRequest>
    {
        public RiyadBankMigsRequestMap()
        {
            ToTable("RiyadBankMigsRequest");
            HasKey(e => e.Id);

            Property(e => e.Amount).HasPrecision(19, 4);


            HasMany(e => e.RiyadBankMigsResponses)
                .WithRequired(e => e.RiyadBankMigsRequest)
                .HasForeignKey(e => e.RiyadBankMigsRequestId)
                .WillCascadeOnDelete(false);


            HasMany(e => e.CheckoutDetails)
               .WithMany()
               .Map(m => m.ToTable("Checkout_RiyadBankMigsRequest").MapLeftKey("RiyadBankMigsRequestId").MapRightKey("CheckoutdetailsId"));
        }
    }
}
