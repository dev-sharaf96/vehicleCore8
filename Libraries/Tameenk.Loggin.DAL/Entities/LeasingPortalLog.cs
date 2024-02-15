using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("LeasingPortalLog")]
    public class LeasingPortalLog
    {
        public int ID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UserID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string PageURL { get; set; }

        [StringLength(255)]
        public string PageName { get; set; }

        public int? BankID { get; set; }

        [StringLength(255)]
        public string BankName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(50)]
        public string UserIP { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }
        [StringLength(255)]
        public string MethodName { get; set; }

        [StringLength(255)]
        public string ApiURL { get; set; }
    }
}
