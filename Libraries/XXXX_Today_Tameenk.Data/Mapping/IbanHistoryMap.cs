using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class IbanHistoryMap :EntityTypeConfiguration<IbanHistory>
    {
        public IbanHistoryMap()
        {
            ToTable("IbanHistory");
            HasKey(e => e.Id);
        }
    }
}
