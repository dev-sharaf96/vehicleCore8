using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class DeductibleMap : EntityTypeConfiguration<Deductible>
    {
        public DeductibleMap()
        {
            ToTable("Deductible");
            HasKey(e => e.ID);
            Property(e => e.Name).HasPrecision(8, 2);

            HasRequired(e => e.InsuranceCompany).WithMany(e => e.Deductibles).HasForeignKey(e => e.InsuranceCompanyID);
        }
    }
}
