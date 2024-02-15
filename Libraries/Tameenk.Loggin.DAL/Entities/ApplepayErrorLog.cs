using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("ApplepayErrorLogs")]
    public  class ApplepayErrorLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserID { get; set; }
        public string ErrorDescription { get; set; }
        [StringLength(50)]
        public string ServerIP { get; set; }
        [StringLength(50)]
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        [StringLength(50)]
        public string ReferenceId { get; set; }
        public string RequesterUrl { get; set; }
    }
}
