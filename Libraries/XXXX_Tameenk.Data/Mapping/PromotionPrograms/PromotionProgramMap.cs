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
    public class PromotionProgramMap : EntityTypeConfiguration<PromotionProgram>
    {
        public PromotionProgramMap()
        {
            ToTable("PromotionProgram");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasMany(e => e.PromotionProgramCodes).WithRequired(e => e.PromotionProgram).HasForeignKey(e => e.PromotionProgramId);
            HasMany(e => e.PromotionProgramUsers).WithRequired(e => e.PromotionProgram).HasForeignKey(e => e.PromotionProgramId);
            HasMany(e => e.PromotionProgramDomains).WithRequired(e => e.PromotionProgram).HasForeignKey(e => e.PromotionProgramId);
            HasMany(e => e.PromotionProgramNins).WithRequired(e => e.PromotionProgram).HasForeignKey(e => e.PromotionProgramId);
        }
    }
}
