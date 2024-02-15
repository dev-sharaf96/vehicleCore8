using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("ApplepayErrorLogs")]
    public  class ApplepayErrorLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public  String? UserID { get; set; }
        public  String? ErrorDescription { get; set; }
        [StringLength(50)]
        public  String? ServerIP { get; set; }
        [StringLength(50)]
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        [StringLength(50)]
        public  String? ReferenceId { get; set; }
        public  String? RequesterUrl { get; set; }
    }
}
