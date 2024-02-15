using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class MobileAppVersions : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(20)]
        [Required]
        public string Version { get; set; }

        [Required]
        public string URL { get; set; }
        public string Platform { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        

    }

    
}
