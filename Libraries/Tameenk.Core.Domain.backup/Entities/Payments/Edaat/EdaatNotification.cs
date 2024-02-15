using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{
    public class EdaatNotification : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; } 
        public string BillNo { get; set; }  
        public string InvoiceNo { get; set; }  
        public string InternalCode { get; set; }     
        public string PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
      
        public int? EdaatRequestId { set; get; }
     
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ReferenceId { get; set; }
        public string JsonRequest { get; set; }
        public string Channel { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        //public string UserAgent { get; set; }
        //public string RequesterUrl { get; set; }
    }
}
