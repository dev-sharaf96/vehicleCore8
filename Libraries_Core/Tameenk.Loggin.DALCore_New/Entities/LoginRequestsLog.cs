

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
namespace Tameenk.Loggin.DAL
{
    [Table("LoginRequestsLog")]
    public partial class LoginRequestsLog
    {
        public int ID { get; set; }

        [StringLength(20)]
        public  String? Mobile { get; set; }

        public  String? UserID { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        [StringLength(255)]
        public  String? ServerIP { get; set; }

        [StringLength(255)]
        public  String? UserAgent { get; set; }

        [StringLength(255)]
        public  String? Email { get; set; }

        [StringLength(255)]
        public  String? Channel { get; set; }

        [StringLength(255)]
        public  String? UserIP { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public  String? Method { get; set; }
        //public  String? MachineUniqueUUID { get; set; }
    }
}
