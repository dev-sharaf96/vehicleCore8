using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{

    public class ExpiredTokens : BaseEntity
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
