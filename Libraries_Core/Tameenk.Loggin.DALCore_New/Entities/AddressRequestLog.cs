using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("AddressRequestLogs")]
    public  class AddressRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserID { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? ServiceRequest { get; set; }

        public  String? ServiceResponse { get; set; }

        public  String? ServerIP { get; set; }

        public  String? UserIP { get; set; }

        public  String? UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        public  String? ReferenceId { get; set; }

        public  String? NationalId { get; set; }
        public  String? ExternalId { get; set; }
        
        public  String? RequesterUrl { get; set; }
        public  String? Channel { get; set; }
        
    }
}
