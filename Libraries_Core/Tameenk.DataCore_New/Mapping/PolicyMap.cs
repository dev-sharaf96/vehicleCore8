using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class PolicyMap : EntityTypeConfiguration<Policy>
    {
        public PolicyMap()
        {
            ToTable("Policy");
            HasKey(e => e.Id);

            Property(e => e.PolicyNo).IsRequired().HasMaxLength(36);
            Property(e => e.CheckOutDetailsId).IsRequired().HasMaxLength(50);


            Property(e => e.NajmStatusId).IsRequired();


            HasOptional(e => e.PolicyDetail)
                .WithRequired(e => e.Policy);

            HasOptional(e => e.InsuranceCompany).WithMany(e => e.Policies).HasForeignKey(e => e.InsuranceCompanyID);
        }
    }
}
