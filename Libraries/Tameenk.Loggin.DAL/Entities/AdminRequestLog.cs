using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("AdminRequestLog")]
    public  class AdminRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UserID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string PageURL { get; set; }

        [StringLength(255)]
        public string PageName { get; set; }

        public int? CompanyID { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(50)]
        public string UserIP { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        [StringLength(50)]
        public string ReferenceId { get; set; }

        [StringLength(50)]
        public string DriverNin { get; set; }

        [StringLength(50)]
        public string VehicleId { get; set; }

        [StringLength(255)]
        public string MethodName { get; set; }

        [StringLength(255)]
        public string ApiURL { get; set; }

        public string RequesterUrl { get; set; }
    }
}
