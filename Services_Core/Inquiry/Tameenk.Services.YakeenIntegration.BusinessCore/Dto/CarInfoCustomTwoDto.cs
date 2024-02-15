using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class CarInfoCustomTwoDto
    {
        public string CustomCrdNumber { get; set; }
        public short? ModelYear { get; set; }
        public bool Export { get; set; } = false;
    }
}
