using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping.UserTicket
{
    public class UserTicketTypeMap : EntityTypeConfiguration<Tameenk.Core.Domain.Entities.UserTicketType>
    {
        public UserTicketTypeMap()
        {
            ToTable("UserTicketType");
            HasKey(c => c.Id);
        }
    }
}
