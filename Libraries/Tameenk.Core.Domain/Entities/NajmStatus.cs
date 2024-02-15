using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
   public class NajmStatus : BaseEntity
    {
        public NajmStatus()
        {
            Policies = new List<Policy>();
        }

        public int Id { get; set; }

        public string Code { get; set; }

        public string NameEn { get; set; }

        public string NameAr { get; set; }

        public  ICollection<Policy> Policies { get; set; }
    }
}
