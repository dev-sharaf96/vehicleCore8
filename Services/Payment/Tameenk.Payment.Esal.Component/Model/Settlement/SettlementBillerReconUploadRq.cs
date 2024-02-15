using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class SettlementBillerReconUploadRq
    {
        public string PrcDt { get; set; }
        public string  CollectPmtAmt { get; set; }
        public string  ReconPmtAmt { get; set; }
        public string  UnReconPmtAmt { get; set; }
        public string  TransFees { get; set; }
        public List<SettlementPmtBankRec> PmtBankRec { get; set; }
    }
}
