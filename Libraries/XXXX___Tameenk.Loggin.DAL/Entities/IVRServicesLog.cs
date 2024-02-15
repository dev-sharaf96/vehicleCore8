using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("IVRServicesLogs")]
    public class IVRServicesLog
    {
        public int ID { get; set; }
        public string ServiceRequest { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Method { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string RequesterUrl { get; set; }
    }
}
