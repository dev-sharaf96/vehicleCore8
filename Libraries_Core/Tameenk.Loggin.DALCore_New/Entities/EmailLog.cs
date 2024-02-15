namespace Tameenk.Loggin.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmailLog")]
    public partial class EmailLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? Email { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public  String? Module { get; set; }
        public  String? Channel { get; set; }
        public  String? UserIP { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? SenderEmail { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public  String? ReferenceId { get; set; }
        public  String? EmailMessage { get; set; }
    }
}
