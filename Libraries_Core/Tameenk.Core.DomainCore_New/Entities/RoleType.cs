using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class RoleType : BaseEntity
    {
        public RoleType()
        {
            Roles = new HashSet<Role>();
        }

        public Guid ID { get; set; }
        
        public string TypeNameAR { get; set; }
        
        public string TypeNameEN { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
