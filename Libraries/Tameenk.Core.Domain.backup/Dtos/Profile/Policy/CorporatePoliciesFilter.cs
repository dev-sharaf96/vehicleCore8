using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class CorporatePoliciesFilter
    {
        public string PolicyNumber { get; set; }
        public string InsuredNIN { get; set; }
        public string SequenceOrCustomCardNumber { get; set; }
    }
}
