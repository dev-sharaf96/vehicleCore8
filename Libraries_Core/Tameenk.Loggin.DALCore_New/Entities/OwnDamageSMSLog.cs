using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("OwnDamageSMSLog")]
    public class OwnDamageSMSLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? MobileNumber { get; set; }
        public  String? SMSMessage { get; set; }
        public  String? PolicyNo { get; set; }
        public  String? ExternalId { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public int ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
    }
}
