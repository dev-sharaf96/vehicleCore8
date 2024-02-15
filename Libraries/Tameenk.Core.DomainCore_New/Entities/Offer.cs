using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class Offer : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string TextAr { get; set; }
        public string TextEn { get; set; }

        public byte[] Image { get; set; }        
        public string Createdby { get; set; }
        public bool IsDeleted { set; get; } = true;
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDate { get; set; }  
    }
}
