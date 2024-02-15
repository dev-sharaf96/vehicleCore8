using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class PolicyUpdatePaymentMap : EntityTypeConfiguration<PolicyUpdatePayment>
    {
        public PolicyUpdatePaymentMap()
        {
            ToTable("PolicyUpdatePayment");
            HasKey(e => e.Id);
            HasRequired(e => e.PolicyUpdateRequest).WithMany(e => e.PolicyUpdatePayments).HasForeignKey(e => e.PolicyUpdateRequestId);
        }

    }
}
