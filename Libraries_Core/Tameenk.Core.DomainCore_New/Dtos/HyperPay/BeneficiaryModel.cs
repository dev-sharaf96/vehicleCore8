using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class BeneficiaryModel
    {
        public string name { get; set; }
        public string accountId { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string bankIdBIC { get; set; }
        public string debitCurrency { get; set; }
        public string transferAmount { get; set; }
        public string transferCurrency { get; set; }
    }
}
