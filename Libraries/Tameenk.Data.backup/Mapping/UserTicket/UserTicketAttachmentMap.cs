using System.Data.Entity.ModelConfiguration;

namespace Tameenk.Data.Mapping.UserTicket
{
    public class UserTicketAttachmentMap : EntityTypeConfiguration<Tameenk.Core.Domain.Entities.UserTicketAttachment>
    {
        public UserTicketAttachmentMap()
        {
            ToTable("UserTicketAttachment");
            HasKey(c => c.Id);
        }
    }
}
