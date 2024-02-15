using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{
   
    public class EdaatResponse : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }
       
   
        public string Code { set; get; }
        public bool Success { set; get; }
        public string Message { set; get; }
        
        public string InvoiceNo { set; get; }
 
        public string InternalCode { set; get; }
        [ForeignKey("EdaatRequest")]
        public int EdaatRequestId { set; get; }
        public EdaatRequest EdaatRequest { set; get; }
        public string ReferenceId { get; set; }
    }
}
