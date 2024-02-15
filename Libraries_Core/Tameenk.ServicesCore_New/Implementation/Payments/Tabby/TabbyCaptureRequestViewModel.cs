using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tameenk.Services.Implementation.Payments.Tabby.TabbySharedClasses;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
   public class TabbyCaptureRequestViewModel
    {
        public string amount { get; set; }
        public string tax_amount { get; set; }
        public string shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public string created_at { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        public List<CaptureItems> items { get; set; }
    }
}

