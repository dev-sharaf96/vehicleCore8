using System.ComponentModel.DataAnnotations.Schema;


namespace Tameenk.Loggin.DAL
{
    [Table("RegistrationRequestsLog")]
    public partial class RegistrationRequestsLog
    {
        public int ID { get; set; }
        public String? Mobile { get; set; }
        public String? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public String? Channel { get; set; }
        public int ErrorCode { get; set; }
        public String? ErrorDescription { get; set; }
        public String? UserIP { get; set; }
        public String? UserAgent { get; set; }
        public String? Method { get; set; }
        public String? ServerIP { get; set; }
        public Guid? UserID { get; set; }
        public String? Code { get; set; }
        public String? Nin { get; set; }
    }
}