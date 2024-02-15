using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("QuotationRequestLog")]

    public class QuotationRequestLog
    {
        public  String? ServerIP { get; set; }
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserId { get; set; }
        public  String? UserName { get; set; }

        public  String? UserIP { get; set; }

        public  String? UserAgent { get; set; }
        public  String? Channel { get; set; }
        public int ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? VehicleId { get; set; }
        public  String? NIN { get; set; }
        public int InsuranceTypeCode { get; set; }
        public  String? CompanyName { get; set; }
        public int CompanyId { get; set; }
        public  String? RefrenceId { get; set; }
        public  String? ExtrnlId { get; set; }
        public  String? ServiceRequest { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public double? DabaseResponseTimeInSeconds { get; set; }
        public double?RequestMessageResponseTimeInSeconds { get; set; }
        public double? ProductResponseTimeInSeconds { get; set; }
        public double? TotalResponseTimeInSeconds { get; set; }
        public  String? Referer { get; set; }
    }
}
