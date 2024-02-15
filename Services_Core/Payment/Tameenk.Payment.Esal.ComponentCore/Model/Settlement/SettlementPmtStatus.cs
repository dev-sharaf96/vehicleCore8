using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class SettlementPmtStatus
    {
        public string StatusCode { get; set; }
        public string EffDt { get; set; }
        public Status status { get; set; }
    }
}
