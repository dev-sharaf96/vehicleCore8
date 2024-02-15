using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class ForgotPasswordTokenMap : EntityTypeConfiguration<ForgotPasswordToken>
    {
        public ForgotPasswordTokenMap()
        {
            ToTable("ForgotPasswordToken");
            HasKey(e => e.Id);
        }
    }
}
