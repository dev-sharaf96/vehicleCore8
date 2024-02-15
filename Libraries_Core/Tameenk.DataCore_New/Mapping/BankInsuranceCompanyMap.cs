using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class BankInsuranceCompanyMap : EntityTypeConfiguration<BankInsuranceCompany>
    {
        public BankInsuranceCompanyMap()
        {
            ToTable("BankInsuranceCompany");
            HasKey(e => e.Id);

            Property(e => e.CompanyId);
            Property(e => e.BankId);

        }
    }
}
