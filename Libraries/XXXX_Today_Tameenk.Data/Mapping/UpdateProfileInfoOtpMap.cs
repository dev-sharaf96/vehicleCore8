using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Payments.Edaat
{
    public class UpdateProfileInfoOtpMap : EntityTypeConfiguration<UpdateProfileInfoOtp>
    {
        public UpdateProfileInfoOtpMap()
        {
            ToTable("UpdateProfileInfoOtp");
            HasKey(e => e.Id); 
        }
    }
}
