using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Esal
{
    public class EsalResponse: BaseEntity
    {
        public int ID { get; set; }
        public string RequestID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string SadadBillsId { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public string ReferenceId { get; set; }
        public string BankDetailbankId { get; set; }
        public string BankDetailBranchCode { get; set; }
        public string BankDetailDistrictCode { get; set; }
        public string BankDetailAccessChannel { get; set; }
        public string BillerId { get; set; }
        public string CurAmt { get; set; }
        public string PmtRefInfo { get; set; }
        public string PayorOfficialId { get; set; }
        public string PayorOfficialIdType { get; set; }
        public string PrcDt { get; set; }
        public string DueDt { get; set; }
        public string EfftDt { get; set; }
        public string SadadPmtId { get; set; }
        public string BankPmtId { get; set; }
        public string BankReversalId { get; set; }
        public string PmtMethod { get; set; }
        public string ProxyPayorOfficialId { get; set; }
        public string ProxyPayorOfficialIdType { get; set; }
        public string PaymentStatus { get; set; }
        public string PrePaidPaymentRefBllingAcct { get; set; }
        public string PrePaidPaymentRefBeneficiaryPhoneNum { get; set; }
        public string PrePaidPaymentRefServiceCode { get; set; }
        public string PrePaidPaymentRefOfficialId { get; set; }
        public string PrePaidPaymentRefOfficialIdType { get; set; }
        public string PrePaidPaymentRefChkDigit { get; set; }
        public string PostPaidPaymentRefBllingAcct { get; set; }
        public string PostPaidPaymentRefBillNumber { get; set; }
        public string PostPaidPaymentRefBillNumberWithAccountBillingAcct { get; set; }
        public string PostPaidPaymentRefBillNumberWithAccountBillNumber { get; set; }
        public string PostPaidPaymentRefBillCycle { get; set; }
        public string PostPaidPaymentRefServiceCode { get; set; }
        public string PostPaidPaymentRefChkDigit { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Method { get; set; }
        public string ServerIP { get; set; }
        public string ServerRequest { get; set; }
        public string RequestIPAddress { get; set; }
        public bool IsCancelled { get; set; }
    }
}
