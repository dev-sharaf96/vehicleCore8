namespace Tameenk.Loggin.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ForbiddenRequestLogs")]
    public partial class ForbiddenRequestLog
    {
        public int ID { get; set; }
        public  String? UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? UserIP { get; set; }
        public  String? Referer { get; set; }
        public  String? Request { get; set; }
        public  String? UserName { get; set; }

    }
}
