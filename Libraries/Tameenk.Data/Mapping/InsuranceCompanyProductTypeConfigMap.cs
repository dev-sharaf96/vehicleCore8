using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class InsuranceCompanyProductTypeConfigMap : EntityTypeConfiguration<InsuranceCompanyProductTypeConfig>
    {
        public InsuranceCompanyProductTypeConfigMap()
        {
            ToTable("InsuranceCompanyProductTypeConfig");

            HasKey(e => e.ProductTypeCode);
            Property(e => e.ProductTypeCode).HasColumnOrder(0);
            Property(e => e.ProductTypeCode).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            HasKey(e => e.InsuranceCompanyID);
            Property(e => e.InsuranceCompanyID).HasColumnOrder(1);
            Property(e => e.InsuranceCompanyID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);  
        }
    }
}
