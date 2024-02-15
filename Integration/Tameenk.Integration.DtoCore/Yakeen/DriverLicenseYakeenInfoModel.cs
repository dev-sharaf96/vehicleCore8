using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Integration.Dto.Yakeen
{
    public class DriverLicenseYakeenInfoModel
    {
        public int LicenseId { get; set; }
        public Guid DriverId { get; set; }
        public byte TypeCode { get; set; }
        public string ExpiryDateH { get; set; }
        public string IssueDateH { get; set; }
    }
}