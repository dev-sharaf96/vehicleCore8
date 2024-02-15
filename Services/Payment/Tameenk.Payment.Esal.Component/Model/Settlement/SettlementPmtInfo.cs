using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model.Settlement
{
    public class SettlementPmtInfo
    {
        public string CurAmt { get; set; }
        public string PrcDt { get; set; }
        public string DueDt { get; set; }
        public string BillCycle { get; set; }
        public string BillNumber { get; set; }
        public SettlementAccountId AccountId { get; set; }
        public string BankId { get; set; }
        public string DistrictCode { get; set; }
        public string BranchCode { get; set; }
        public string AccessChannel { get; set; }
        public string PmtMethod { get; set; }
        public string PmtType { get; set; }
        public string ChkDigit { get; set; }
        public string ServiceType { get; set; }
        public string PmtRefInfo { get; set; }
        public SettlementIncludeInfoPlus IncludeInfoPlus { get; set; }

        public SettlementBeneficiaryId BeneficiaryId { get; set; }
        public SettlementPhoneNum PhoneNum { get; set; }
    }
}
