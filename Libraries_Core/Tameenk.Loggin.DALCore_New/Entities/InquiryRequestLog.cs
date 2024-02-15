using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{

    [Table("InquiryRequestLog")]
   public class InquiryRequestLog
    {
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserId { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(50)]
        public  String? UserIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        public  String? ServerIP { get; set; }

        public  String? Channel { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }

        [MaxLength(50)]
        public  String? MethodName { get; set; }
        public Guid? RequestId { get; set; }
        public  String? VehicleId { get; set; }
        public  String? NIN { get; set; }
        public  String? ExternalId { get; set; }
        public int CityCode { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public  String? NajmNcdRefrence { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public  String? ServiceRequest { get; set; }
        public  String? MobileVersion { get; set; }
    }
}
