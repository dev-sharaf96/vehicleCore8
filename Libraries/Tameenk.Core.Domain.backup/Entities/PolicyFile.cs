using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class PolicyFile : BaseEntity
    {
        public PolicyFile()
        {
            Policies = new HashSet<Policy>();
        }

        public Guid ID { get; set; }
        
        public byte[] PolicyFileByte { get; set; }

        public string FilePath { get; set; }
        public string ServerIP { get; set; }

        public ICollection<Policy> Policies { get; set; }
       // public string AlternativePolicyFileUrl { get; set; }
    }
}
