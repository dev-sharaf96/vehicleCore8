using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Data.Mapping.Payments.Edaat
{
    public class OfferMap: EntityTypeConfiguration<Offer>
    {
        public OfferMap()
        {
            ToTable("Offer");
            HasKey(e => e.Id); 
        }
    }
}
