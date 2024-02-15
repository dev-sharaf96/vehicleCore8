using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping.UserTicket
{
    public class UserTicketMap : EntityTypeConfiguration<Tameenk.Core.Domain.Entities.UserTicket>
    {
        public UserTicketMap()
        {
            ToTable("UserTicket");
            HasKey(c => c.Id);
        }
    }
}
