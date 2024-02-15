using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class TawuniyaTempTableMap : EntityTypeConfiguration<TawuniyaTempTable>
    {
        public TawuniyaTempTableMap()
        {
            ToTable("TawuniyaTempTable");
            HasKey(x => x.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
