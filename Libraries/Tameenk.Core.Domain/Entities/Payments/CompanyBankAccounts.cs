using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    
    public class CompanyBankAccounts : BaseEntity
    {

        public int Id { get; set; }

        public int CompanyId { get; set; }
        public string ComapnyKey { get; set; }

        public string BeneficiaryName  { get; set; }
        
        public string Bank { get; set; }
        
        public string IBAN  { get; set; }
        
        public string SWIFTCODE { get; set; }
        public decimal? TransferedPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
         
        public string CrNumber { set; get; }
    }
}
