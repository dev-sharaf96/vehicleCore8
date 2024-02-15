using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class SettlementPmtRec
    {
        public List<SettlementPmtTransId> PmtTransId { get; set; }
        public SettlementCustId CustId { get; set; }
        public SettlementPmtStatus PmtStatus { get; set; }
        public SettlementPmtInfo PmtInfo { get; set; }
    }
}
