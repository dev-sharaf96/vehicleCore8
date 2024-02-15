using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("AutoleasingAdminRequestLog")]
    public  class AutoleasingAdminRequestLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public  String? UserID { get; set; }

        [StringLength(255)]
        public  String? UserName { get; set; }

        [StringLength(255)]
        public  String? PageURL { get; set; }

        [StringLength(255)]
        public  String? PageName { get; set; }

        public int? BankID { get; set; }

        [StringLength(255)]
        public  String? BankName { get; set; }

        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        public  String? ServiceRequest { get; set; }

        public  String? ServiceResponse { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        [StringLength(50)]
        public  String? UserIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }
        [StringLength(255)]
        public  String? MethodName { get; set; }

        [StringLength(255)]
        public  String? ApiURL { get; set; }
    }
}
