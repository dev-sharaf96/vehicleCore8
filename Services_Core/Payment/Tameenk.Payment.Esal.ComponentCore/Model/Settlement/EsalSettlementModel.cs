using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class EsalSettlementModel
    {
        public long ID { get; set; }
        public SettlementStatus Status { get; set; }
        public string RqUID { get; set; }
        public SettlementBillerReconUploadRq BillerReconUploadRq { get; set; }

    }
}
