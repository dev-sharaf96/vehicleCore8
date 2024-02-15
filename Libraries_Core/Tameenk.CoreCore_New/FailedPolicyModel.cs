using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class FailedPolicyModel
    {
        public string QuotationNo { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get { return PolicyEffectiveDate?.AddYears(1); } }
        public string EnglishFirstName { get; set; }
        public string EnglishSecondName { get; set; }
        public string EnglishThirdName { get; set; }
        public string EnglishLastName { get; set; }
        public string ReferenceId { get; set; }
        public int InsuranceCompanyID { get; set; }
    }
}
