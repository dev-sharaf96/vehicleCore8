using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Tameenk.Loggin.DAL
{
    [Table("PolicyRequestLog")]
    public partial class PolicyRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UserId { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(50)]
        public  String? UserIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        public Guid? RequestID { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        [StringLength(255)]
        public  String? CompanyName { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        [StringLength(255)]
        public  String? QuotationNo { get; set; }

        public int? ProductID { get; set; }

        [StringLength(50)]
        public  String? InsuredID { get; set; }

        [StringLength(50)]
        public  String? InsuredMobileNumber { get; set; }

        [StringLength(255)]
        public  String? InsuredEmail { get; set; }

        [StringLength(255)]
        public  String? InsuredCity { get; set; }

        [StringLength(500)]
        public  String? InsuredAddress { get; set; }

        [StringLength(50)]
        public  String? PaymentMethod { get; set; }

        public decimal? PaymentAmount { get; set; }

        [StringLength(50)]
        public  String? PaymentBillNumber { get; set; }

        [StringLength(50)]
        public  String? InsuredBankCode { get; set; }

        [StringLength(255)]
        public  String? InsuredBankName { get; set; }

        [StringLength(255)]
        public  String? InsuredIBAN { get; set; }
    }
}
