using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class PolicyLogDetailsDTO
    {
      
        public string ReferenceId { get; set; }   
        public string InsuranceCompanyName { get; set; }       
        public DateTime? PolicyIssueDate { get; set; }  
        public DateTime? PolicyEffectiveDate { get; set; }  
        public DateTime? PolicyExpiryDate { get; set; }  
        public int? VehicleValue { get; set; }     
        public int? DeductableValue { get; set; }  
        public bool? VehicleAgencyRepair { get; set; }
        public string NationalId { get; set; }
        public string PolicyNo { get; set; }
    }
}
