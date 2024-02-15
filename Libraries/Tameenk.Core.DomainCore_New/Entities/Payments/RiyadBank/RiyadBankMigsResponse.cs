using System;

namespace Tameenk.Core.Domain.Entities.Payments.RiyadBank
{
    public class RiyadBankMigsResponse : BaseEntity
    {
        public int Id { get; set; }
        public int RiyadBankMigsRequestId { get; set; }
        public string Vpc3DSECI { get; set; }
        public string Vpc3DSXID { get; set; }
        public string Vpc3DSenrolled { get; set; }
        public string Vpc3DSstatus { get; set; }
        public string AVSResultCode { get; set; }
        public string AcqAVSRespCode { get; set; }
        public string AcqCSCRespCode { get; set; }
        public string AcqResponseCode { get; set; }
        public decimal Amount { get; set; }
        public string AuthorizeId { get; set; }
        public string BatchNo { get; set; }
        public string CSCResultCode { get; set; }
        public string Card { get; set; }
        public string CardNum { get; set; }
        public string Command { get; set; }
        public string Locale { get; set; }
        public string MerchTxnRef { get; set; }
        public string MerchantId { get; set; }
        public string Message { get; set; }
        public string OrderInfo { get; set; }
        public string ReceiptNo { get; set; }
        public string SecureHash { get; set; }
        public string SecureHashType { get; set; }
        public string TransactionNo { get; set; }
        public string TxnResponseCode { get; set; }
        public string VerSecurityLevel { get; set; }
        public string VerStatus { get; set; }
        public string VerToken { get; set; }
        public string VerType { get; set; }
        public string Version { get; set; }
        public DateTime? CreatedDate { get; set; }
        public RiyadBankMigsRequest RiyadBankMigsRequest { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
    }
}
