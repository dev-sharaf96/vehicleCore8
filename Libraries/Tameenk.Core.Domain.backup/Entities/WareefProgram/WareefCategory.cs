using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
   public class WareefCategory:BaseEntity
    {
        public int Id { set; get; }
        public string NameAr { set; get; }
        public string NameEn { set; get; }
        public string Icon { set; get; }
        public bool IsDeleted { get; set; }
        public string Createdby { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
