namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyRequestLog")]
    public partial class PolicyRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UserId { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string UserIP { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        public Guid? RequestID { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        [StringLength(255)]
        public string QuotationNo { get; set; }

        public int? ProductID { get; set; }

        [StringLength(50)]
        public string InsuredID { get; set; }

        [StringLength(50)]
        public string InsuredMobileNumber { get; set; }

        [StringLength(255)]
        public string InsuredEmail { get; set; }

        [StringLength(255)]
        public string InsuredCity { get; set; }

        [StringLength(500)]
        public string InsuredAddress { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public decimal? PaymentAmount { get; set; }

        [StringLength(50)]
        public string PaymentBillNumber { get; set; }

        [StringLength(50)]
        public string InsuredBankCode { get; set; }

        [StringLength(255)]
        public string InsuredBankName { get; set; }

        [StringLength(255)]
        public string InsuredIBAN { get; set; }
    }
}
