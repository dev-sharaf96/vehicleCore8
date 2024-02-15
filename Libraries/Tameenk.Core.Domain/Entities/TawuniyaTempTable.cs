using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class TawuniyaTempTable : BaseEntity
    {
        public int Id { get; set; }
        public int QtReqId { get; set; }
        public string QtServiceRequestMessage { get; set; }
        public string PorposalResponse { get; set; }
    }
}
