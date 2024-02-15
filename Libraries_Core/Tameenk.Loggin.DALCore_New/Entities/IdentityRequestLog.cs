using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL
{
    [Table("IdentityRequestLog")]
    public class IdentityRequestLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public  String? ClientId { get; set; }
        public  String? ClientSecret { get; set; }
        public  String? UserId { get; set; }
        public  String? Response { get; set; }
        [StringLength(255)]
        public  String? Method { get; set; } //GetAccessToken or 
        public int? ErrorCode { get; set; }

        public  String? ErrorDescription { get; set; }

        [StringLength(50)]
        public  String? ServerIP { get; set; }

        [StringLength(255)]
        public  String? Channel { get; set; }
    }
}
