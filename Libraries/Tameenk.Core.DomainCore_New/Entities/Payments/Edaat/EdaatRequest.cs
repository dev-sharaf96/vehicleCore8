using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tameenk.Core.Domain.Entities.Payments.Edaat
{
     
    public class EdaatRequest :BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }  = DateTime.Now;
        
        public string UserId { get; set; }
       
        public EdaatRequest()
        {
            Products = new List<EdaatProduct>();
            Customers = new List<EdaatCustomer>();
            Companys = new List<EdaatCompany>();
            EdaatResponses = new List<EdaatResponse>();
        }
       
        public string ReferenceId { set; get; }
        public bool IsClientEnterpise { set; get; }
       
        public string RegistrationNo { get; set; }
       
        public string NationalID { get; set; }

       
        public string InternalCode { set; get; }

        public DateTime IssueDate { set; get; }
        public DateTime DueDate { set; get; }
        public decimal TotalAmount { set; get; }
   
        public string Conditions { set; get; }
        
        public string SubBillerRegistrationNo { set; get; }      
        // public List<int> TaxIds { set; get; }
        // public List<int> DiscountIDs { set; get; }
        public bool HasValidityPeriod { set; get; }
         
        public string FromDurationTime { set; get; }
    
        public string ToDurationTime { set; get; }
        public bool ExportToSadad { set; get; }
        public virtual List<EdaatProduct> Products { set; get; }
        public virtual List<EdaatCustomer> Customers { set; get; }
        public virtual List<EdaatCompany> Companys { set; get; }
        public virtual List<EdaatResponse> EdaatResponses { set; get; }
        public int CompanyId { set; get; }
      
        public string CompanyName { set; get; }
        public DateTime ExpiryDate { set; get; }
        public decimal? SubBillerShareAmount { set; get; }
        public decimal? SubBillerSharePercentage { set; get; }
        public string InsuredNationalId { get; set; }

    }
}
