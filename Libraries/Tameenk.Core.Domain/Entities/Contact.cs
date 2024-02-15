using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class Contact : BaseEntity
    {
        public Contact()
        {
            InsuranceCompanies = new HashSet<InsuranceCompany>();
        }

        public int Id { get; set; }
        
        public string MobileNumber { get; set; }
        
        public string HomePhone { get; set; }
        
        public string Fax { get; set; }
        
        public string Email { get; set; }
        public string WebSite { get; set; }

        public ICollection<InsuranceCompany> InsuranceCompanies { get; set; }
    }
}
