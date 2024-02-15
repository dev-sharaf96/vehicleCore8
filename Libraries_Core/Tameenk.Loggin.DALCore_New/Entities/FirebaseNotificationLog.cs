using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("FirebaseNotificationLog")]

    public class FirebaseNotificationLog
    {
        public int ID { get; set; }
        public  String? MethodName { get; set; }
        public  String? UserId { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? ServerIP { get; set; }
        public  String? Channel { get; set; }
        public int ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? ServiceRequest { get; set; }
        public  String? ServiceResponse { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? ReferenceId { get; set; }

    }
}
