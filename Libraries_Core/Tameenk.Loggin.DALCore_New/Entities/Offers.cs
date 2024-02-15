using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Loggin.DAL.Entities
{
    [Table("Offers")]
    public class Offer
    {
        public int Id { get; set; }
        public  String? Name { get; set; }
        public  String? NameEn { get; set; }
        public  String? RouteName { get; set; }
        public  String? Body { get; set; }
        public  String? BodyEn { get; set; }
        public byte[] Logo { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

}
