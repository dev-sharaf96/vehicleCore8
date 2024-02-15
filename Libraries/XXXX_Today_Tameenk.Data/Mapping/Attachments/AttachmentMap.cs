using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Attachments
{
    public class AttachmentMap : EntityTypeConfiguration<Attachment>
    {
        public AttachmentMap()
        {
            ToTable("Attachment");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.AttachmentFile).IsRequired();
            Property(e => e.AttachmentType).IsRequired();
            Property(e => e.AttachmentName).IsRequired();
            Property(e => e.Guid).IsRequired();
            HasMany(e => e.PolicyUpdateRequestAttachments);
            
        }
    }
}
