using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class LicenseModel
    {
        public int LicenseId { get; set; }

        public Guid DriverId { get; set; }

        public short? TypeDesc { get; set; }

        public string ExpiryDateH { get; set; }

        public string IssueDateH { get; set; }

        public string licnsTypeDesc { get; set; }
    }
}
