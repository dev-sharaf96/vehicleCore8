using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    public class WataniyaAutoleasePolicyResponse
    {
        public string PolicyNO { get; set; }
        public string QuotationNumber { get; set; }
        public int Status { get; set; }
        public List<AutoleaseError> ErrorList { get; set; }
    }
}
