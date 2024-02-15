using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("PolicyModificationLog")]

    public class PolicyModificationLog
    {        
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }      
        public  String? UserId { get; set; }
        public  String? UserName { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? Channel { get; set; }
        public int ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? NIN { get; set; }
        public int InsuranceTypeCode { get; set; }
        public  String? CompanyName { get; set; }
        public int CompanyId { get; set; }
        public  String? RefrenceId { get; set; }
        public  String? PolicyNo{ get; set; }
        public  String? ServerIP { get; set; }
        public  String? MethodName { get; set; }
        public  String? VehicleId { get; set; }
    }
}
