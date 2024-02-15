namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NotificationLog")]
    public partial class NotificationLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Method { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CompanyName { get; set; }
        public int InsuranceTypeCode { get; set; }
        public int CompanyId { get; set; }
        public string Channel { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
    }
}
