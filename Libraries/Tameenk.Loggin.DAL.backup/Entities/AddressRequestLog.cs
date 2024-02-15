using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("AddressRequestLogs")]
    public  class AddressRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UserID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        public string ServerIP { get; set; }

        public string UserIP { get; set; }

        public string UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }

        public string ReferenceId { get; set; }

        public string NationalId { get; set; }
        public string ExternalId { get; set; }
        
        public string RequesterUrl { get; set; }
        public string Channel { get; set; }
        
    }
}
