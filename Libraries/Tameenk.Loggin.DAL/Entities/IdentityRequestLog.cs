using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("IdentityRequestLog")]
    public class IdentityRequestLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserId { get; set; }
        public string Response { get; set; }
        [StringLength(255)]
        public string Method { get; set; } //GetAccessToken or 
        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(255)]
        public string Channel { get; set; }
    }
}
