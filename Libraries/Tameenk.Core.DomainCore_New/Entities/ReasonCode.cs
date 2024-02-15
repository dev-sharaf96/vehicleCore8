using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class ReasonCode : BaseEntity
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
    }
}
