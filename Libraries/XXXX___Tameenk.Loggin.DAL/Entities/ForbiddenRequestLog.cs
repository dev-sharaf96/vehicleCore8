namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ForbiddenRequestLogs")]
    public partial class ForbiddenRequestLog
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ServerIP { get; set; }
        public string UserAgent { get; set; }
        public string UserIP { get; set; }
        public string Referer { get; set; }
        public string Request { get; set; }
        public string UserName { get; set; }

    }
}
