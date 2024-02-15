using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("CheckoutRequestLog")]
    public class CheckoutRequestLog
    {
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserId { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(50)]
        public  String? UserIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        [StringLength(50)]
        public  String? Channel { get; set; }

        public int? ErrorCode { get; set; }

     
        public  String? ErrorDescription { get; set; }

        [MaxLength(50)]
        public  String? MethodName { get; set; }

        [StringLength(50)]
        public  String? ReferenceId { get; set; }

        public  String? VehicleId { get; set; }

        public  String? DriverNin { get; set; }

        public int? CompanyId { get; set; }

        public  String? CompanyName { get; set; }

        public  String? PaymentMethod { get; set; }

        public decimal? Amount { get; set; }
        public double? ResponseTimeInSeconds { get; set; }
        public  String? ServiceRequest { get; set; }

        public  String? RequesterUrl { get; set; }
        public  String? ServiceResponse { get; set; }
    }
}
