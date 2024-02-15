using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class CancellationRequestMap : EntityTypeConfiguration<CancellationRequest>

    {
        public CancellationRequestMap()
        {
            ToTable("CancellationRequest");
            HasKey(e => e.Id);
        }
    }
}
