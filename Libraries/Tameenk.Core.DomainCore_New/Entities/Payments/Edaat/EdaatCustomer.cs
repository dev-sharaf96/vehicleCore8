using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{ 
    public class EdaatCustomer : BaseEntity
    {

        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }
     
      
        public string NationalID { set; get; }
 
        public string FirstNameAr { set; get; }
      
        public string FatherNameAr { set; get; }
   
        public string GrandFatherNameAr { set; get; }
        
        public string LastNameAr { set; get; }
       
        public string FirstNameEn { set; get; }
       
        public string FatherNameEn { set; get; }
 
        public string GrandFatherNameEn { set; get; }
     
        public string LastNameEn { set; get; }
       
        public string Email { set; get; }
       
        public string MobileNo { set; get; }
        public DateTime DateOfBirth { set; get; }
      
        public string DateOfBirthHijri { set; get; }
        
        public string CustomerRefNumber { set; get; }
        [ForeignKey("EdaatRequest")]
        public int EdaatRequestId { set; get; }
        public virtual EdaatRequest EdaatRequest { set; get; }
    }
}
