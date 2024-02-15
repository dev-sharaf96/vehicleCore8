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
    public class EdaatResponseMap: EntityTypeConfiguration<EdaatResponse>
    {
        public EdaatResponseMap()
        {
            ToTable("EdaatResponse");
            HasKey(e => e.Id); 
        }
    }
}
