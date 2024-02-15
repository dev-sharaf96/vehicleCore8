using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ClaimRegistrationRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string AccidentReportNumber { get; set; }
        public int InsuredId { get; set; }
        public string InsuredMobileNumber { get; set; }
        public string InsuredIBAN { get; set; }
        public string InsuredBankCode { get; set; }
        public string DriverLicenseExpiryDate { get; set; }
        public string DriverLicenseTypeCode { get; set; }
        public byte[] AccidentReport { get; set; }
    }
}
