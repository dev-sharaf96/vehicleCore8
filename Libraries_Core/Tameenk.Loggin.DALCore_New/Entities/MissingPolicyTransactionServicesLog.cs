using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("MissingPolicyTransactionServicesLogs")]
    public class MissingPolicyTransactionServicesLog
    {
        public int ID { get; set; }
        public  String? ServiceRequest { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public DateTime CreatedDate { get; set; }
        public  String? ReferenceId { get; set; }
    }
}
