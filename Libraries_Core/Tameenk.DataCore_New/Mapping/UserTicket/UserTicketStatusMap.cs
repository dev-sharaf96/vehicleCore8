using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping.UserTicket
{
    public class UserTicketStatusMap : EntityTypeConfiguration<Tameenk.Core.Domain.Entities.UserTicketStatus>
    {
        public UserTicketStatusMap()
        {
            ToTable("UserTicketStatus");
            HasKey(c => c.Id);
        }
    }
}
