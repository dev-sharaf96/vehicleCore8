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
   public class PromotionProgramDomainMap: EntityTypeConfiguration<PromotionProgramDomain>
    {
        public PromotionProgramDomainMap()
        {
            ToTable("PromotionProgramDomain");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(e => e.PromotionProgram);

        }
    }
}
