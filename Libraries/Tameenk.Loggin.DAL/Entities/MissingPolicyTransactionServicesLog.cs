using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("MissingPolicyTransactionServicesLogs")]
    public class MissingPolicyTransactionServicesLog
    {
        public int ID { get; set; }
        public string ServiceRequest { get; set; }
        public string ErrorDescription { get; set; }
        public string Method { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReferenceId { get; set; }
    }
}
