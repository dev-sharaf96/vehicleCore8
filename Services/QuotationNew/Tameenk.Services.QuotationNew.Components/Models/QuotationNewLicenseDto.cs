using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationNewLicenseDto
    {
        public int LicenseCountryCode { get; set; }
        public int LicenseNumberYears { get; set; }
        public string DriverLicenseTypeCode { get; set; }
        public string DriverLicenseExpiryDate { get; set; }
    }
}
