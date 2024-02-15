using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Data.Mapping.Payments.Edaat
{
    public class EdaatRequestMap: EntityTypeConfiguration<EdaatRequest>
    {
        public EdaatRequestMap()
        {
            ToTable("EdaatRequest");
            HasKey(e => e.Id);
            Property(e => e.TotalAmount) .HasPrecision(18, 2);
        }
    }
}
