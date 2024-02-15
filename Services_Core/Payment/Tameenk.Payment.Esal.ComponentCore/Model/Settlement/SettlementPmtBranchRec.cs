using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class SettlementPmtBranchRec
    {
        public string BranchCode { get; set; }
        public string CurAmt { get; set; }
        public List<SettlementPmtRec> PmtRec { get; set; }
    }
}
