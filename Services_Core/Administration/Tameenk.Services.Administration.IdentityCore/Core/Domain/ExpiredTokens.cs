using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    
    [Table("ExpiredTokens")]
    public class ExpiredTokens
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
