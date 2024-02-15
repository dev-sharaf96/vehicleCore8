using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping.Quotations
{
    class QuotationBlockedNinsMap : EntityTypeConfiguration<QuotationBlockedNins>
    {
        public QuotationBlockedNinsMap()
        {
            ToTable("QuotationBlockedNins");
            HasKey(e => e.Id);
        }
      
    }
}
