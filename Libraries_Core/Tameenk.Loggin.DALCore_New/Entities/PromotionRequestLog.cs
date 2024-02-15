using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("PromotionRequestLog")]
    public class PromotionRequestLog
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? UserID { get; set; }

        public  String? UserName { get; set; }

        public  String? MethodName { get; set; }

        public  String? ApiURL { get; set; }

        public  String? Channel { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? ServiceRequest { get; set; }

        public  String? ServerIP { get; set; }

        public  String? UserIP { get; set; }

        public  String? UserAgent { get; set; }

        public  String? RequesterUrl { get; set; }
    }
}
