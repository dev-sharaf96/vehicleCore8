using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Esal
{
    public class EsalSettlement : BaseEntity
    {
        public long ID { get; set; }

        public int? StatusCode { get; set; }
        public string ShortDesc { get; set; }
        public string SeverityType { get; set; }

        public string RqUID { get; set; }

        public string BillerReconUploadRqPrcDt { get; set; }
        public string BillerReconUploadRqCollectPmtAmt { get; set; }
        public string BillerReconUploadRqReconPmtAmt { get; set; }
        public string BillerReconUploadRqUnReconPmtAmt { get; set; }
        public string BillerReconUploadRqTransFees { get; set; }

        public int? PmtBankRecBankId { get; set; }
        public string PmtBankRecCurAmt { get; set; }

        public string PmtBranchRecBranchCode { get; set; }
        public string PmtBranchRecCurAmt { get; set; }

        public string PmtRecPmtTransIdPmtId { get; set; }
        public string PmtRecPmtTransIdPmtIdType { get; set; }

        public string CustIdOfficialId { get; set; }
        public string CustIdofficialIdType { get; set; }

        public string PmtStatusPmtStatusCode { get; set; }
        public string PmtStatusEffDt { get; set; }

        public int? StatusStatusCode { get; set; }
        public string StatusShortDesc { get; set; }
        public string StatusSeverityType { get; set; }

        public string PmtInfoCurAmt { get; set; }
        public string PmtInfoPrcDt { get; set; }
        public string PmtInfoDueDt { get; set; }
        public string PmtInfoBillCycle { get; set; }
        public string PmtInfoBillNumber { get; set; }
        public string AccountIdBillingAcct { get; set; }
        public string AccountIdBillerId { get; set; }
        public string BankId { get; set; }
        public string DistrictCode { get; set; }
        public string BranchCode { get; set; }
        public string AccessChannel { get; set; }
        public string PmtMethod { get; set; }
        public string PmtType { get; set; }
        public string ChkDigit { get; set; }
        public string ServiceType { get; set; }
        public string PmtRefInfo { get; set; }

        public bool? IncludeInfoPlusIncPaymentRanges { get; set; }

        public string BeneficiaryIdOfficialId { get; set; }
        public string BeneficiaryIdOfficialIdType { get; set; }

        public string PhoneNumBeneficiaryPhoneNum { get; set; }


    }
}
