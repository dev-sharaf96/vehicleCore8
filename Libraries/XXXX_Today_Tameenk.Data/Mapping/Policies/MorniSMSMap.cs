using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class MorniSMSMap : EntityTypeConfiguration<MorniSMS>
    {
        public MorniSMSMap() {
            ToTable("MorniSMS");
            HasKey(c => c.ID);
        }
    }
}