using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Data.Mapping.PromotionPrograms
{
    public class PromotionProgramNinsMap : EntityTypeConfiguration<PromotionProgramNins>
    {
        public PromotionProgramNinsMap()
        {
            ToTable("PromotionProgramNins");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
