using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("PolicyNotificationLogs")]
    public class PolicyNotificationLog
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? Channel { get; set; }
        public int? StatusCode { get; set; }
        public  String? StatusDescription { get; set; }
        public  String? CompanyName { get; set; }
        public int? CompanyId { get; set; }
        public  String? MethodName { get; set; }
        public  String? Requester { get; set; }
        public  String? ServiceRequest { get; set; }
        public  String? ReferenceId { get; set; }
        public  String? PolicyNo { get; set; }
        public DateTime? UploadedDate { get; set; }
        public  String? UploadedReference { get; set; }


    }
}
