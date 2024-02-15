using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Data.Mapping.Payments.Sadad
{
    public class SadadResponseMap : EntityTypeConfiguration<SadadResponse>
    {
        public SadadResponseMap()
        {
            ToTable("SadadResponse");
            HasKey(e => e.Id);
            Property(e => e.Status).IsRequired().HasMaxLength(10);
            Property(e => e.Description).IsRequired();

            HasRequired(e => e.SadadRequest).WithMany(e => e.SadadResponses).HasForeignKey(e => e.SadadRequestId);
        }
    }
}
