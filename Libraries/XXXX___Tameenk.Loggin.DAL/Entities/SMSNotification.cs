namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SMSNotification")]
    public partial class SMSNotification
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(255)]
        public string MobileNumber { get; set; }

        [StringLength(255)]
        public string SMSMessage { get; set; }

        public string ReferenceId { get; set; }
        public string SequenceNumber { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NotificationNo { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string CustomCard { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string TaskName { get; set; }
    }
}
