using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class Language : BaseEntity
    {
        public Language()
        {
            AspNetUsers = new HashSet<AspNetUser>();
        }

        public Guid Id { get; set; }
        
        public string NameAR { get; set; }
        
        public string NameEN { get; set; }

        public bool isDefault { get; set; }


        public ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
