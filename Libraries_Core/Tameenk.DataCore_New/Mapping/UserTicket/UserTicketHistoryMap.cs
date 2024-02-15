using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping.UserTicket
{
    public class UserTicketHistoryMap : EntityTypeConfiguration<Tameenk.Core.Domain.Entities.UserTicketHistory>
    {
        public UserTicketHistoryMap()
        {
            ToTable("UserTicketHistory");
            HasKey(c => c.Id);
        }
    }
}
