using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("LeasingAddDriverLog")]
    public class LeasingAddDriverLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? UserID { get; set; }
        public  String? UserName { get; set; }
        public  String? PageURL { get; set; }
        public  String? PageName { get; set; }
        public int? BankID { get; set; }
        public  String? BankName { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? ServiceRequest { get; set; }
        public  String? ServiceResponse { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public  String? MethodName { get; set; }
        public  String? ApiURL { get; set; }
    }
}
