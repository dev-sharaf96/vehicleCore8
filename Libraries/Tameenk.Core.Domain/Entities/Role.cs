using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Role()
        {
            AspNetUsers = new HashSet<AspNetUser>();
        }

        public Guid ID { get; set; }

        public Guid RoleTypeID { get; set; }
        
        public string RoleNameAR { get; set; }
        
        public string RoleNameEN { get; set; }


        public ICollection<AspNetUser> AspNetUsers { get; set; }

        public RoleType RoleType { get; set; }
    }
}
