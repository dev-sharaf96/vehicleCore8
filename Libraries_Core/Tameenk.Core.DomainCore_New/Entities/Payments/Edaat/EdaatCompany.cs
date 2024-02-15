using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{ 
    public class EdaatCompany : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }      
      
        public string RegistrationNo { set; get; }
        
        public string NameAr { set; get; }
   
        public string NameEn { set; get; }
       
        public string CommissionerNationalId { set; get; }
      
        public string CommissionerName { set; get; }
         
        public string CommissionerEmail { set; get; }
         
        public string CommissionerMobileNo { set; get; }
      
        public string CustomerRefNumber { set; get; }
        [ForeignKey("EdaatRequest")]
        public int EdaatRequestId { set; get; }
        public virtual EdaatRequest EdaatRequest { set; get; }
    }
}
