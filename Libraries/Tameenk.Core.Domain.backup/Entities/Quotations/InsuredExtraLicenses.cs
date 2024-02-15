using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class InsuredExtraLicenses : BaseEntity
    {
        public int Id { get; set; }
        public short LicenseCountryCode { get; set; }
        public int LicenseNumberYears { get; set; }
        public int InsuredId { get; set; }
        public string DriverNin { get; set; }
        public bool IsMainDriver { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Insured Insured { get; set; }

    }
}
