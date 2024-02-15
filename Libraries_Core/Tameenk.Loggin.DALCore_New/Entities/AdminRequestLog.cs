using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("AdminRequestLog")]
    public  class AdminRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserID { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(255)]
        public  String? PageURL { get; set; }

        [StringLength(255)]
        public  String? PageName { get; set; }

        public int? CompanyID { get; set; }

        [StringLength(255)]
        public  String? CompanyName { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? ServiceRequest { get; set; }

        public  String? ServiceResponse { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        [StringLength(50)]
        public  String? UserIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        [StringLength(50)]
        public  String? ReferenceId { get; set; }

        [StringLength(50)]
        public  String? DriverNin { get; set; }

        [StringLength(50)]
        public  String? VehicleId { get; set; }

        [StringLength(255)]
        public  String? MethodName { get; set; }

        [StringLength(255)]
        public  String? ApiURL { get; set; }

        public  String? RequesterUrl { get; set; }
    }
}
