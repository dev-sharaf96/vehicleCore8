using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class QuotationDetailsModel
    {
        public Guid DriverId { get; set; }
        public string InsuredNationalId { get; set; }
        public string DriverNin { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CompanyKey { get; set; }
        public int InsuranceCompanyID { get; set; }
    }
}
