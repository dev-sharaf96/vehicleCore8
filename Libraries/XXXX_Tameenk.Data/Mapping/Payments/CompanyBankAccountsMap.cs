using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments;

namespace Tameenk.Core.Domain.Entities
{
    
    public class CompanyBankAccountsMap : EntityTypeConfiguration<CompanyBankAccounts>
    {
        public CompanyBankAccountsMap()
        {
            ToTable("CompanyBankAccounts");
            HasKey(e => e.Id);
        }
    }
}
