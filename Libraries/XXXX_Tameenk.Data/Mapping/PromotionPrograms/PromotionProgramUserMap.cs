using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Data.Mapping.PromotionPrograms
{
    public class PromotionProgramUserMap : EntityTypeConfiguration<PromotionProgramUser>
    {
        public PromotionProgramUserMap()
        {
            ToTable("PromotionProgramUser");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        }
    }
}
