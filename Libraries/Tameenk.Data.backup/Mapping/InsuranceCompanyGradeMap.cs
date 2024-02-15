using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    class InsuranceCompanyGradeMap : EntityTypeConfiguration<InsuranceCompanyGrade>
        {
            public InsuranceCompanyGradeMap()
            {
                ToTable("InsuranceCompanyGrade");
                HasKey(e => e.Id);
            }
        }
}
