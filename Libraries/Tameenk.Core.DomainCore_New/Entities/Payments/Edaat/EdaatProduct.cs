using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{
     
    public class EdaatProduct : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(128)]
        public string UserId { get; set; }
       
        public int ProductId { set; get; }
        public double Price { set; get; }
        public double Qty { set; get; }
        [ForeignKey("EdaatRequest")]
        public int EdaatRequestId { set; get; }
        public EdaatRequest EdaatRequest { set; get; }
    }
}
