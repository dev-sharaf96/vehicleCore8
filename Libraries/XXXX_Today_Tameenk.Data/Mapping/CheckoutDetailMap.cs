using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class CheckoutDetailMap : EntityTypeConfiguration<CheckoutDetail>
    {
        public CheckoutDetailMap()
        {
            HasKey(e => e.ReferenceId);
            Property(e => e.ReferenceId).HasMaxLength(50);
            Property(e => e.Email).HasMaxLength(50);
            Property(e => e.Phone).HasMaxLength(50);
            Property(e => e.Channel);
            Property(e => e.InsuranceCompanyName);
            Property(e => e.BankCodeId);
            Property(e => e.IBAN).HasMaxLength(50);
            Property(e => e.UserId).IsRequired().HasMaxLength(128);

            HasOptional(checkout => checkout.InsuranceCompany).WithMany().HasForeignKey(qr => qr.InsuranceCompanyId);

            HasMany(e => e.CheckoutAdditionalDrivers)
                .WithRequired(e => e.CheckoutDetail)
                .HasForeignKey(e => e.CheckoutDetailsId)
                .WillCascadeOnDelete(false);

            HasMany(e => e.PayfortPaymentRequests)
                .WithMany(e => e.CheckoutDetails)
                .Map(m => m.ToTable("Checkout_PayfortPaymentReq").MapLeftKey("CheckoutdetailsId").MapRightKey("PayfortPaymentRequestId"));

            HasMany(e => e.Policies)
                .WithRequired(e => e.CheckoutDetail)
                .HasForeignKey(e => e.CheckOutDetailsId)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.PaymentMethod).WithMany().HasForeignKey(e => e.PaymentMethodId);

            HasOptional(e => e.BankCode).WithMany().HasForeignKey(e => e.BankCodeId);
            //HasOptional(e => e.AdditionalInfo)
            //   .WithRequired(e => e.CheckoutDetail);

            HasMany(e => e.HyperpayRequests)
                .WithMany(e => e.CheckoutDetails)
             .Map(m => m.ToTable("Checkout_HyperpayPaymentReq").MapLeftKey("CheckoutdetailsId").MapRightKey("HyperpayPaymenRequestId"));

        }
    }
}
