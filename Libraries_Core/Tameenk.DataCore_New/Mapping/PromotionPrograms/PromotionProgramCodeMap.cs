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
    public class PromotionProgramCodeMap : EntityTypeConfiguration<PromotionProgramCode>
    {
        public PromotionProgramCodeMap()
        {
            ToTable("PromotionProgramCode");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(e => e.ProductType).WithMany().HasForeignKey(e=>e.InsuranceTypeCode);
            HasRequired(e => e.InsuranceCompany).WithMany().HasForeignKey(e => e.InsuranceCompanyId);
            HasOptional(code => code.Creator).WithMany().HasForeignKey(usr => usr.CreatorId);
            HasOptional(code => code.Modifier).WithMany().HasForeignKey(usr => usr.ModifierId);
        }
    }
}
