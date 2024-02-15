using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Data.Mapping.Payments.Esal
{
    public class EsalResponseMap: EntityTypeConfiguration<EsalResponse>
    {
        public EsalResponseMap()
        {
            ToTable("EsalResponse");
            HasKey(e => e.ID);
        }
    }
}
