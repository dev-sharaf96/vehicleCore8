namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SMSLog")]
    public partial class SMSLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(255)]
        public string MobileNumber { get; set; }

        public string SMSMessage { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string Method { get; set; }

        public string UserIP { get; set; }

        public string ServerIP { get; set; }

        public string UserAgent { get; set; }

        public string SMSProvider { get; set; }

        public string ServiceURL { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ReferenceId { get; set; }
        public string Module { get; set; }
        public string Channel { get; set; }
    }
}
