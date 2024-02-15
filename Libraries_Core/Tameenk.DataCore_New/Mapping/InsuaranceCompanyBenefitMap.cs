using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InsuaranceCompanyBenefitMap : EntityTypeConfiguration<InsuaranceCompanyBenefit>
    {
        public InsuaranceCompanyBenefitMap()
        {
            ToTable("InsuaranceCompanyBenefit");
            HasKey(e => e.BenifitID);
            Property(e => e.BenifitPrice).HasPrecision(8, 2);

            HasRequired(e => e.InsuranceCompany).WithMany(e => e.InsuaranceCompanyBenefits).HasForeignKey(e => e.InsurnaceCompanyID);
            HasRequired(e => e.Benefit).WithMany(e => e.InsuaranceCompanyBenefits).HasForeignKey(e => e.BenifitCode);
        }
    }
}
