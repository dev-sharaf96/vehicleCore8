using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Data.Mapping.Policies
{
    public class PolicyProcessingQueueMap : EntityTypeConfiguration<PolicyProcessingQueue>

    {
        public PolicyProcessingQueueMap()
        {
            ToTable("PolicyProcessingQueue");
            HasKey(e => e.Id);
        }
    }
}
