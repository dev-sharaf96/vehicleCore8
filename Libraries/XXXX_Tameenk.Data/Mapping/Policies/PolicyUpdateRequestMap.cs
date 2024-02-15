using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class PolicyUpdateRequestMap : EntityTypeConfiguration<PolicyUpdateRequest>
    {
        public PolicyUpdateRequestMap()
        {
            ToTable("PolicyUpdateRequest");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.PolicyId).IsRequired();
            Property(e => e.RequestTypeId).IsRequired();
            Property(e => e.Guid).IsRequired();
            Ignore(e => e.RequestType);
            Ignore(e => e.Status);
            HasMany(e => e.PolicyUpdateRequestAttachments);
            HasRequired(e => e.Policy).WithMany(e => e.PolicyUpdateRequests).HasForeignKey(e => e.PolicyId);
            HasMany(e=>e.PayfortPaymentRequests).WithMany(e=>e.PolicyUpdateRequests)
                .Map(m => m.ToTable("PolicyUpdReq_PayfortPaymentReq").MapLeftKey("PolicyUpdateRequestId").MapRightKey("PayfortPaymentRequestId"));
        }
    }
}
