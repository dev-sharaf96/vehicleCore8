namespace Tameenk.Loggin.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NotificationLog")]
    public partial class NotificationLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? Phone { get; set; }
        public  String? Message { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public  String? CompanyName { get; set; }
        public int InsuranceTypeCode { get; set; }
        public int CompanyId { get; set; }
        public  String? Channel { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
    }
}
