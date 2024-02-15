using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class NajmStatusHistoryMap : EntityTypeConfiguration<NajmStatusHistory>
    {
        public NajmStatusHistoryMap() {
            ToTable("NajmStatusHistory");
            Property(e => e.ReferenceId).IsRequired().HasMaxLength(50);
            Property(e => e.PolicyNo).IsRequired().HasMaxLength(50);
            Property(e => e.StatusDescription).HasMaxLength(2000);
            Property(e => e.UploadedReference).HasMaxLength(50);
        }
    }
}
