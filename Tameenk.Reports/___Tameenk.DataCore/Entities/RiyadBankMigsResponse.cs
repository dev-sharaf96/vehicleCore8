namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RiyadBankMigsResponse")]
    public partial class RiyadBankMigsResponse
    {
        public int Id { get; set; }

        public int RiyadBankMigsRequestId { get; set; }

        [StringLength(200)]
        public string Vpc3DSECI { get; set; }

        [StringLength(200)]
        public string Vpc3DSXID { get; set; }

        [StringLength(200)]
        public string Vpc3DSenrolled { get; set; }

        [StringLength(200)]
        public string Vpc3DSstatus { get; set; }

        [StringLength(200)]
        public string AVSResultCode { get; set; }

        [StringLength(200)]
        public string AcqAVSRespCode { get; set; }

        [StringLength(200)]
        public string AcqCSCRespCode { get; set; }

        [StringLength(200)]
        public string AcqResponseCode { get; set; }

        public decimal Amount { get; set; }

        [StringLength(200)]
        public string AuthorizeId { get; set; }

        [StringLength(200)]
        public string BatchNo { get; set; }

        [StringLength(200)]
        public string CSCResultCode { get; set; }

        [StringLength(200)]
        public string Card { get; set; }

        [StringLength(200)]
        public string CardNum { get; set; }

        [StringLength(200)]
        public string Command { get; set; }

        [StringLength(200)]
        public string Locale { get; set; }

        [StringLength(200)]
        public string MerchTxnRef { get; set; }

        [StringLength(200)]
        public string MerchantId { get; set; }

        public string Message { get; set; }

        [StringLength(200)]
        public string OrderInfo { get; set; }

        [StringLength(200)]
        public string ReceiptNo { get; set; }

        [StringLength(200)]
        public string SecureHash { get; set; }

        [StringLength(200)]
        public string SecureHashType { get; set; }

        [StringLength(200)]
        public string TransactionNo { get; set; }

        [StringLength(200)]
        public string TxnResponseCode { get; set; }

        [StringLength(200)]
        public string VerSecurityLevel { get; set; }

        [StringLength(200)]
        public string VerStatus { get; set; }

        [StringLength(200)]
        public string VerToken { get; set; }

        [StringLength(200)]
        public string VerType { get; set; }

        [StringLength(200)]
        public string Version { get; set; }

        public virtual RiyadBankMigsRequest RiyadBankMigsRequest { get; set; }
    }
}
