using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments;

namespace Tameenk.Core.Domain.Entities
{

    public class CommissionsAndFeesMap : EntityTypeConfiguration<CommissionsAndFees>
    {
        public CommissionsAndFeesMap()
        {
            ToTable("CommissionsAndFees");
            HasKey(e => e.Id);
        }
    }
}
