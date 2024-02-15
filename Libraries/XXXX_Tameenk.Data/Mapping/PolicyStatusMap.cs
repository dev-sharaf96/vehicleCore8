using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PolicyStatusMap : EntityTypeConfiguration<PolicyStatus>
    {
        public PolicyStatusMap()
        {
            Property(e => e.Key).HasMaxLength(200);
            Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            Property(e => e.NameAr).IsRequired().HasMaxLength(200);


            HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.PolicyStatus)
                .HasForeignKey(e => e.PolicyStatusId);
        }
    }
}
