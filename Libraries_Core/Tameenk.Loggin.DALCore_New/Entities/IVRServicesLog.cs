using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("IVRServicesLogs")]
    public class IVRServicesLog
    {
        public int ID { get; set; }
        public  String? ServiceRequest { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public int ModuleId { get; set; }
        public  String? ModuleName { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? RequesterUrl { get; set; }
    }
}
