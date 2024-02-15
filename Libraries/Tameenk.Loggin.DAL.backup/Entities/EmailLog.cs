namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailLog")]
    public partial class EmailLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Email { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Method { get; set; }
        public string Module { get; set; }
        public string Channel { get; set; }
        public string UserIP { get; set; }
        public string ServerIP { get; set; }
        public string UserAgent { get; set; }
        public string SenderEmail { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ReferenceId { get; set; }
        public string EmailMessage { get; set; }
    }
}
