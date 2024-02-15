using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Payments.Edaat
{
    public class ProfileUpdatePhoneHistoryMap : EntityTypeConfiguration<ProfileUpdatePhoneHistory>
    {
        public ProfileUpdatePhoneHistoryMap()
        {
            ToTable("ProfileUpdatePhoneHistory");
            HasKey(e => e.Id); 
        }
    }
}
