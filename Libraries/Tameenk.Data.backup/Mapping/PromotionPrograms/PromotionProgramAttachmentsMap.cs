using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Data.Mapping.PromotionPrograms
{
    public class PromotionProgramAttachmentsMap : EntityTypeConfiguration<PromotionProgramAttachments>
    {
        public PromotionProgramAttachmentsMap()
        {
            ToTable("PromotionProgramAttachments");
            HasKey(e => e.Id);
        }
    }
}
