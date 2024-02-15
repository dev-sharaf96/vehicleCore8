using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{

    [Table("CompetitionRequestLogs")]
   public class CompetitionRequestLog
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? UserId { get; set; }
        public  String? UserName { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? ServerIP { get; set; }
        public  String? Channel { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public  String? VehicleId { get; set; }
        public  String? Nin { get; set; }
        public  String? ServiceRequest { get; set; }
    }
}
