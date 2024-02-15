using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class PayoutModel
    {
        public string transferOption { get; set; }
        public string configId { get; set; }
        public string period { get; set; }
        public List<BeneficiaryModel> beneficiary { get; set; }
    }
}
