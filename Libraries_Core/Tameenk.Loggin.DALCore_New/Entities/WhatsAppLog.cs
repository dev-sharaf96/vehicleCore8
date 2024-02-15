using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tameenk.Loggin.DAL
{    
    

    [Table("WhatsAppLog")]
    public partial class WhatsAppLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(255)]
        public  String? MobileNumber { get; set; }

      
        public  String? WhatsAppMessage { get; set; }

        public  String? ServiceRequest { get; set; }
        public  String? ServiceResponse { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? Method { get; set; }
        public  String? ReferenceId { get; set; }
    }
}
